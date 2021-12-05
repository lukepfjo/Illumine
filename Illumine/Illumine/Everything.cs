﻿using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Illumine
{
	internal static class Everything
	{
		public static class StatusCodes
		{
			public enum Codes
			{
				OK = 0,
				ERROR_MEMORY = 1,
				ERROR_IPC = 2,
				ERROR_REGISTERCLASSEX = 3,
				ERROR_CREATEWINDOW = 4,
				ERROR_CREATETHREAD = 5,
				ERROR_INVALIDINDEX = 6,
				ERROR_INVALIDCALL = 7
			}

			public static string TranslateStatusCode(uint code)
			{
				return (Codes)code switch
				{
					Codes.OK => "The operation completed successfully.",
					Codes.ERROR_MEMORY => "Failed to allocate memory for the search query.",
					Codes.ERROR_IPC => "IPC is not available.",
					Codes.ERROR_REGISTERCLASSEX => "Failed to register the search query window class.",
					Codes.ERROR_CREATEWINDOW => "Failed to create the search query window.",
					Codes.ERROR_CREATETHREAD => "Failed to create the search query thread.",
					Codes.ERROR_INVALIDINDEX => "Invalid index. The index must be greater or equal to 0 and less than the number of visible results.",
					Codes.ERROR_INVALIDCALL => "Invalid call.",
					_ => throw new ArgumentException("Invalid code")
				};
			}
		}

		public static class RequestSettings
		{
			public const int FILE_NAME = 0x00000001,
					  		 PATH = 0x00000002,
							 FULL_PATH_AND_FILE_NAME = 0x00000004,
							 EXTENSION = 0x00000008,
							 SIZE = 0x00000010,
							 DATE_CREATED = 0x00000020,
							 DATE_MODIFIED = 0x00000040,
							 DATE_ACCESSED = 0x00000080,
							 ATTRIBUTES = 0x00000100,
							 FILE_LIST_FILE_NAME = 0x00000200,
							 RUN_COUNT = 0x00000400,
							 DATE_RUN = 0x00000800,
							 DATE_RECENTLY_CHANGED = 0x00001000,
							 HIGHLIGHTED_FILE_NAME = 0x00002000,
							 HIGHLIGHTED_PATH = 0x00004000,
							 HIGHLIGHTED_FULL_PATH_AND_FILE_NAME = 0x00008000;
		}

		public static class SortSettings
		{
			public const int NAME_ASCENDING = 1,
							 NAME_DESCENDING = 2,
							 PATH_ASCENDING = 3,
							 PATH_DESCENDING = 4,
							 SIZE_ASCENDING = 5,
							 SIZE_DESCENDING = 6,
							 EXTENSION_ASCENDING = 7,
							 EXTENSION_DESCENDING = 8,
							 TYPE_NAME_ASCENDING = 9,
							 TYPE_NAME_DESCENDING = 10,
							 DATE_CREATED_ASCENDING = 11,
							 DATE_CREATED_DESCENDING = 12,
							 DATE_MODIFIED_ASCENDING = 13,
							 DATE_MODIFIED_DESCENDING = 14,
							 ATTRIBUTES_ASCENDING = 15,
							 ATTRIBUTES_DESCENDING = 16,
							 FILE_LIST_FILENAME_ASCENDING = 17,
							 FILE_LIST_FILENAME_DESCENDING = 18,
							 RUN_COUNT_ASCENDING = 19,
							 RUN_COUNT_DESCENDING = 20,
							 DATE_RECENTLY_CHANGED_ASCENDING = 21,
							 DATE_RECENTLY_CHANGED_DESCENDING = 22,
							 DATE_ACCESSED_ASCENDING = 23,
							 DATE_ACCESSED_DESCENDING = 24,
							 DATE_RUN_ASCENDING = 25,
							 DATE_RUN_DESCENDING = 26;
		}

		#region Everything DLL Imports

		[DllImport("Everything64.dll", CharSet = CharSet.Unicode)]
		public static extern UInt32 Everything_SetSearchW(string lpSearchString);
		[DllImport("Everything64.dll")]
		public static extern void Everything_SetMatchPath(bool bEnable);
		[DllImport("Everything64.dll")]
		public static extern void Everything_SetMatchCase(bool bEnable);
		[DllImport("Everything64.dll")]
		public static extern void Everything_SetMatchWholeWord(bool bEnable);
		[DllImport("Everything64.dll")]
		public static extern void Everything_SetRegex(bool bEnable);
		[DllImport("Everything64.dll")]
		public static extern void Everything_SetMax(UInt32 dwMax);
		[DllImport("Everything64.dll")]
		public static extern void Everything_SetOffset(UInt32 dwOffset);

		[DllImport("Everything64.dll")]
		public static extern bool Everything_GetMatchPath();
		[DllImport("Everything64.dll")]
		public static extern bool Everything_GetMatchCase();
		[DllImport("Everything64.dll")]
		public static extern bool Everything_GetMatchWholeWord();
		[DllImport("Everything64.dll")]
		public static extern bool Everything_GetRegex();
		[DllImport("Everything64.dll")]
		public static extern UInt32 Everything_GetMax();
		[DllImport("Everything64.dll")]
		public static extern UInt32 Everything_GetOffset();
		[DllImport("Everything64.dll")]
		public static extern IntPtr Everything_GetSearchW();
		[DllImport("Everything64.dll")]
		public static extern UInt32 Everything_GetLastError();

		[DllImport("Everything64.dll")]
		public static extern bool Everything_QueryW(bool bWait);

		[DllImport("Everything64.dll")]
		public static extern void Everything_SortResultsByPath();

		[DllImport("Everything64.dll")]
		public static extern UInt32 Everything_GetNumFileResults();
		[DllImport("Everything64.dll")]
		public static extern UInt32 Everything_GetNumFolderResults();
		[DllImport("Everything64.dll")]
		public static extern UInt32 Everything_GetNumResults();
		[DllImport("Everything64.dll")]
		public static extern UInt32 Everything_GetTotFileResults();
		[DllImport("Everything64.dll")]
		public static extern UInt32 Everything_GetTotFolderResults();
		[DllImport("Everything64.dll")]
		public static extern UInt32 Everything_GetTotResults();
		[DllImport("Everything64.dll")]
		public static extern bool Everything_IsVolumeResult(UInt32 nIndex);
		[DllImport("Everything64.dll")]
		public static extern bool Everything_IsFolderResult(UInt32 nIndex);
		[DllImport("Everything64.dll")]
		public static extern bool Everything_IsFileResult(UInt32 nIndex);
		[DllImport("Everything64.dll")]
		public static extern IntPtr Everything_GetResultPath(UInt32 nIndex);
		[DllImport("Everything64.dll", CharSet = CharSet.Unicode)]
		public static extern void Everything_GetResultFullPathName(UInt32 nIndex, StringBuilder lpString, UInt32 nMaxCount);
		[DllImport("Everything64.dll")]
		public static extern void Everything_Reset();

		[DllImport("Everything64.dll", CharSet = CharSet.Unicode)]
		public static extern IntPtr Everything_GetResultFileName(UInt32 nIndex);

		// Everything 1.4
		[DllImport("Everything64.dll")]
		public static extern void Everything_SetSort(UInt32 dwSortType);
		[DllImport("Everything64.dll")]
		public static extern UInt32 Everything_GetSort();
		[DllImport("Everything64.dll")]
		public static extern UInt32 Everything_GetResultListSort();
		[DllImport("Everything64.dll")]
		public static extern void Everything_SetRequestFlags(UInt32 dwRequestFlags);
		[DllImport("Everything64.dll")]
		public static extern UInt32 Everything_GetRequestFlags();
		[DllImport("Everything64.dll")]
		public static extern UInt32 Everything_GetResultListRequestFlags();
		[DllImport("Everything64.dll", CharSet = CharSet.Unicode)]
		public static extern IntPtr Everything_GetResultExtension(UInt32 nIndex);
		[DllImport("Everything64.dll")]
		public static extern bool Everything_GetResultSize(UInt32 nIndex, out long lpFileSize);
		[DllImport("Everything64.dll")]
		public static extern bool Everything_GetResultDateCreated(UInt32 nIndex, out long lpFileTime);
		[DllImport("Everything64.dll")]
		public static extern bool Everything_GetResultDateModified(UInt32 nIndex, out long lpFileTime);
		[DllImport("Everything64.dll")]
		public static extern bool Everything_GetResultDateAccessed(UInt32 nIndex, out long lpFileTime);
		[DllImport("Everything64.dll")]
		public static extern UInt32 Everything_GetResultAttributes(UInt32 nIndex);
		[DllImport("Everything64.dll", CharSet = CharSet.Unicode)]
		public static extern IntPtr Everything_GetResultFileListFileName(UInt32 nIndex);
		[DllImport("Everything64.dll")]
		public static extern UInt32 Everything_GetResultRunCount(UInt32 nIndex);
		[DllImport("Everything64.dll")]
		public static extern bool Everything_GetResultDateRun(UInt32 nIndex, out long lpFileTime);
		[DllImport("Everything64.dll")]
		public static extern bool Everything_GetResultDateRecentlyChanged(UInt32 nIndex, out long lpFileTime);
		[DllImport("Everything64.dll", CharSet = CharSet.Unicode)]
		public static extern IntPtr Everything_GetResultHighlightedFileName(UInt32 nIndex);
		[DllImport("Everything64.dll", CharSet = CharSet.Unicode)]
		public static extern IntPtr Everything_GetResultHighlightedPath(UInt32 nIndex);
		[DllImport("Everything64.dll", CharSet = CharSet.Unicode)]
		public static extern IntPtr Everything_GetResultHighlightedFullPathAndFileName(UInt32 nIndex);
		[DllImport("Everything64.dll")]
		public static extern UInt32 Everything_GetRunCountFromFileName(string lpFileName);
		[DllImport("Everything64.dll")]
		public static extern bool Everything_SetRunCountFromFileName(string lpFileName, UInt32 dwRunCount);
		[DllImport("Everything64.dll")]
		public static extern UInt32 Everything_IncRunCountFromFileName(string lpFileName);

		#endregion
	}
}
