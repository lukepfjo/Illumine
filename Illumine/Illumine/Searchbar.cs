﻿using System;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Illumine
{
    public partial class Searchbar : Form
    {
        public CancellationTokenSource resultListUpdateCancelHandler = null;

        // ====
        private readonly IntPtr mainWindow;
        private SearchResults searchResults = null;
        private readonly int searchResultsDisplayLimit = 150;

        // We need this to be defined for proper disposal
        private Dictionary<string, int> keybind;
        private GlobalHotkeys.GlobalHotkeys showHotkey;
        private readonly KeybindSetter keybindSetter;

        private readonly SearchEngine searchEngine;
        private static readonly List<Keys> searchInputIgnoreKeys = new() { Keys.Left, Keys.Right, Keys.Home, Keys.End, Keys.Escape };

        // ========
        public Searchbar()
        {
            InitializeComponent();

            // TODO :: Set with config file
            ShowOnScreen(0);

            // Round the corners
            Region = Region.FromHrgn(WinDisplayFuncs.CreateRoundRectRgn(0, 0, Width, Height, 20, 20));

            // Glue the searchbar window to the desktop
            mainWindow = Handle;
            WinDisplayFuncs.SetParent(mainWindow, WinDisplayFuncs.GetDesktopWindow());

            SearchInput.SelectionAlignment = HorizontalAlignment.Center;
            AddSearchInputContextMenu();

            // Capture ESC regardless of control focus
            KeyPreview = true;

            // TODO :: Set with config file
            keybind = new Dictionary<string, int>()
            {
                { "keys", (int)Keys.OemPeriod },
                { "mods", (int)(GlobalHotkeys.Modifiers.Ctrl | GlobalHotkeys.Modifiers.Win) }
            };

            showHotkey = new GlobalHotkeys.GlobalHotkeys((GlobalHotkeys.Modifiers)keybind["mods"], (Keys)keybind["keys"], this, true);

            keybindSetter = new KeybindSetter();
            keybindSetter.RegisterCallback(SetHotkey);

            searchEngine = new SearchEngine();
        }

        private void ExitCleanup()
        {
            showHotkey.Unregister();
            showHotkey.Dispose();
            searchEngine.ClearSearch();
            Environment.Exit(0);
        }

        public void AddSearchInputContextMenu()
        {
            if (SearchInput.ContextMenuStrip == null)
            {
                ContextMenuStrip contextMenu = new()
                {
                    ShowImageMargin = false
                };

                ToolStripMenuItem tsmiCut = new("Cut");
                tsmiCut.Click += (sender, e) => SearchInput.Cut();
                contextMenu.Items.Add(tsmiCut);

                ToolStripMenuItem tsmiCopy = new("Copy");
                tsmiCopy.Click += (sender, e) => SearchInput.Copy();
                contextMenu.Items.Add(tsmiCopy);

                ToolStripMenuItem tsmiPaste = new("Paste");
                tsmiPaste.Click += (sender, e) => SearchInput.Paste();
                contextMenu.Items.Add(tsmiPaste);

                ToolStripMenuItem tsmiSelectAll = new("Select All");
                tsmiSelectAll.Click += (sender, e) => SearchInput.SelectAll();
                contextMenu.Items.Add(tsmiSelectAll);

                contextMenu.Items.Add(new ToolStripSeparator());

                ToolStripMenuItem tsmiSetKeybind = new("Set Keybind");
                tsmiSetKeybind.Click += (sender, e) => keybindSetter.Show();
                contextMenu.Items.Add(tsmiSetKeybind);

                contextMenu.Items.Add(new ToolStripSeparator());

                ToolStripMenuItem tsmiExit = new("Exit");
                tsmiExit.Click += (sender, e) => ExitCleanup();
                contextMenu.Items.Add(tsmiExit);

                contextMenu.Opening += (sender, e) =>
                {
                    tsmiCut.Enabled = SearchInput.SelectionLength > 0;
                    tsmiCopy.Enabled = SearchInput.SelectionLength > 0;
                    tsmiPaste.Enabled = Clipboard.ContainsText();
                    tsmiSelectAll.Enabled = SearchInput.TextLength > 0 && SearchInput.SelectionLength < SearchInput.TextLength;
                };

                SearchInput.ContextMenuStrip = contextMenu;
            }
        }

        #region Window position and focus handling

        protected override CreateParams CreateParams
        {
            // Hide window from ALT+TAB
            get
            {
                CreateParams cp = base.CreateParams;
                // turn on WS_EX_TOOLWINDOW style bit
                cp.ExStyle |= 0x80;
                return cp;
            }
        }

        public void TakeFocus()
        {
            WinDisplayFuncs.SetWindowPos(mainWindow, WinDisplayFuncs.HWND_TOP, 0, 0, 0, 0,
                                         (uint)(WinDisplayFuncs.WindowPosAttr.NOMOVE | WinDisplayFuncs.WindowPosAttr.NOSIZE));
            ActiveControl = SearchInput;
            SearchInput.Text = "";
            WinDisplayFuncs.SetForegroundWindow(mainWindow);
        }

        public void LoseFocus()
        {
            ActiveControl = null;
            SearchInput.Text = "";
            TopMost = false;
            SendWindowToBack();
        }

        void ShowOnScreen(int screenNumber)
        {
            // Probably doesn't work since form is always maximized
            Screen[] screens = Screen.AllScreens;

            if (screenNumber >= 0 && screenNumber < screens.Length)
            {
                bool maximised = false;
                if (WindowState == FormWindowState.Maximized)
                {
                    WindowState = FormWindowState.Normal;
                    maximised = true;
                }
                Location = screens[screenNumber].WorkingArea.Location;
                if (maximised)
                {
                    WindowState = FormWindowState.Maximized;
                }
            }
        }

        private void SendWindowToBack()
        {
            // Pin searchbar to lowest z-index
            WinDisplayFuncs.SetWindowPos(mainWindow, WinDisplayFuncs.HWND_BOTTOM, 0, 0, 0, 0,
                                         (uint)(WinDisplayFuncs.WindowPosAttr.NOACTIVATE | WinDisplayFuncs.WindowPosAttr.NOMOVE | WinDisplayFuncs.WindowPosAttr.NOSIZE));
        }

        private void Searchbar_VisibleChanged(object sender, EventArgs e)
        {
            // Send to back of window stack on load
            WindowState = FormWindowState.Minimized;
            SendWindowToBack();
        }

        private void Searchbar_SizeChanged(object sender, EventArgs e)
        {
            // Reset to normal size on size change (minimize/maximize)
            if (WindowState != FormWindowState.Normal)
            {
                WindowState = FormWindowState.Normal;
            }
        }

        private void Searchbar_Load(object sender, EventArgs e)
        {
            // Hide caret by focusing hidden label
            Top = 20;
            ActiveControl = FocusThiefLabel;
            SendWindowToBack();
        }

        private void Searchbar_Deactivate(object sender, EventArgs e)
        {
            if (!ContainsFocus && (searchResults == null || !searchResults.ContainsFocus))
            {
                LoseFocus();
            }
        }

        private void Searchbar_Click(object sender, EventArgs e)
        {
            TakeFocus();
        }

        public void HandleResultsClose(object sender, EventArgs e)
        {
            searchResults.FormClosed -= HandleResultsClose;
            searchResults.Dispose();
            searchResults = null;

            LoseFocus();
        }

        #endregion

        private bool SetHotkey(HashSet<Keys> hotkey)
        {
            Dictionary<string, int> oldKeybind = new(keybind);
            keybind["keys"] = 0;
            keybind["mods"] = 0;

            foreach (Keys k in hotkey)
            {
                // Is a modifier, needs to be converted
                if (k == Keys.ShiftKey || k == Keys.ControlKey || k == Keys.Menu || k == Keys.LWin || k == Keys.RWin)
                {
                    keybind["mods"] |= GlobalHotkeys.ModifierKeysToGlobalHotkeys.Convert(k);
                }
                else
                {
                    keybind["keys"] |= (int)k;
                }
            }

            showHotkey.Unregister();
            showHotkey.Dispose();

            try
            {
                showHotkey = new GlobalHotkeys.GlobalHotkeys((GlobalHotkeys.Modifiers)keybind["mods"], (Keys)keybind["keys"], this, true);
            }
            catch (GlobalHotkeys.GlobalHotkeysException)
            {
                keybind = oldKeybind;
                showHotkey = new GlobalHotkeys.GlobalHotkeys((GlobalHotkeys.Modifiers)keybind["mods"], (Keys)keybind["keys"], this, true);
                return false;
            }

            return true;
        }

        protected override void WndProc(ref Message m)
        {
            // This gets called when any global hotkey is pressed, so we need to filter the messages to ensure we're the handler
            if (m.Msg == GlobalHotkeys.Constants.WM_HOTKEY_MSG_ID)
            {
                // Bitmask magic to get key codes
                int keyCode = ((int)m.LParam >> 16) & 0xFFFF;
                int modifierKeys = (int)m.LParam & 0xFFFF;

                if (keyCode == keybind["keys"] && modifierKeys == keybind["mods"])  // LWin and RWin are treated as the same
                {
                    if (TopMost)
                    {
                        LoseFocus();
                    }
                    else
                    {
                        TakeFocus();
                    }
                }
            }

            base.WndProc(ref m);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Escape))
            {
                HandleEscapeHotkey();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void HandleEscapeHotkey()
        {
            if (searchResults != null)
            {
                searchResults.Close();
            }
        }

        private void SearchInput_KeyUp(object sender, KeyEventArgs e)
        {
            if (searchInputIgnoreKeys.Contains(e.KeyCode))
            {
                return;
            }

            if (SearchInput.Text != "")
            {
                if (searchResults == null || searchResults.IsDisposed)
                {
                    searchResults = new SearchResults();
                    searchResults.FormClosed += HandleResultsClose;
                    searchResults.Show();

                    // Receive keypresses from results file list
                    searchResults.KeypressPassthrough += new SearchResults.PassthroughKeypressEvent((KeyEventArgs ptKE) =>
                    {
                        if (ptKE.KeyCode == Keys.Escape)
                        {
                            HandleEscapeHotkey();
                        }
                    });

                    TopMost = true;
                    Activate();
                }

                if (resultListUpdateCancelHandler != null)
                {
                    resultListUpdateCancelHandler.Cancel();
                }

                Task pauseForCancel = Task.Run(() => Task.Delay(20));
                pauseForCancel.Wait();

                resultListUpdateCancelHandler = new CancellationTokenSource();
                searchEngine.cancelationHandler = resultListUpdateCancelHandler;
                searchEngine.DoSearch(SearchInput.Text, SearchEngineCallback);
            }

            else
            {
                if (searchResults != null && searchResults.results.Count > 0)
                {
                    searchResults.results.Clear();
                    searchEngine.ClearSearch();
                }
            }
        }

        public void SearchEngineCallback(ValueTuple<long, List<SearchResult>> results)
        {
            searchResults.PauseResultsListUpdates();

            searchResults.results.Clear();
            for (int i = 0; i < Math.Min(searchResultsDisplayLimit, results.Item2.Count); i++)
            {
                SearchResult result = results.Item2[i];
                searchResults.results.Add(result.fileName + "  ||  " + result.filePath);

                if (resultListUpdateCancelHandler.IsCancellationRequested)
                {
                    return;
                }
            }

            searchResults.currentSearchQuery = SearchInput.Text;

            searchResults.ResumeResultsListUpdates();

            Console.WriteLine("Updated results successfully");
        }
    }
}
