﻿
namespace Illumine
{
    partial class SearchResults
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.ResultsFileList = new System.Windows.Forms.ListView();
            this.SuspendLayout();
            // 
            // ResultsFileList
            // 
            this.ResultsFileList.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.ResultsFileList.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.ResultsFileList.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ResultsFileList.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ResultsFileList.ForeColor = System.Drawing.Color.White;
            this.ResultsFileList.FullRowSelect = true;
            this.ResultsFileList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.ResultsFileList.HideSelection = false;
            this.ResultsFileList.Location = new System.Drawing.Point(0, 0);
            this.ResultsFileList.MultiSelect = false;
            this.ResultsFileList.Name = "ResultsFileList";
            this.ResultsFileList.Size = new System.Drawing.Size(1300, 550);
            this.ResultsFileList.TabIndex = 1;
            this.ResultsFileList.UseCompatibleStateImageBehavior = false;
            this.ResultsFileList.View = System.Windows.Forms.View.Details;
            this.ResultsFileList.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ResultsFileList_KeyDown);
            this.ResultsFileList.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.ResultsFileList_MouseDoubleClick);
            // 
            // SearchResults
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(1300, 550);
            this.ControlBox = false;
            this.Controls.Add(this.ResultsFileList);
            this.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SearchResults";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Illumine";
            this.TransparencyKey = System.Drawing.Color.Black;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SearchResults_FormClosing);
            this.VisibleChanged += new System.EventHandler(this.SearchResults_VisibleChanged);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView ResultsFileList;
    }
}