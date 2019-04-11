using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Win32;

namespace GenerateHtml
{
    public class Utils
    {
        public static readonly string LayoutStyleSheet = "##stylesheet#";
        public static readonly string LayoutHeader = "##header#";
        public static readonly string LayoutContent = "##content#";
        public static readonly string LayoutFooter = "##footer#";
        public static readonly string LayoutScript = "##script#";
        public static readonly string FolderDownload = "GenerateHtml";
        public static string GetDownloadFolderPath()
        {
            return Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders", "{374DE290-123F-4565-9164-39C4925E467B}", String.Empty).ToString();
        }
    }
}