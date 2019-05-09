using System;
using System.IO;
using System.Web;
using Microsoft.Win32;

namespace GenerateHtml.ClassHelpers
{
    public class Utils
    {
        public static readonly string LayoutStyleSheet = "##stylesheet#";
        public static readonly string LayoutBody = "##body#";
        public static readonly string LayoutScript = "##script#";
        public static readonly string FolderDownload = "GenerateHtml";
        public static readonly string UploadFolder = "/Uploads";
        public static readonly string ImagePath = "images";
        public static readonly string CssPath = "css";
        public static readonly string ScriptsPath = "scripts";
        public static readonly string FontFolder = "fonts";
        public static readonly string DownLoadFolder = "/Download";
        public static string GetDownloadFolderPath()
        {
            return Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders", "{374DE290-123F-4565-9164-39C4925E467B}", String.Empty).ToString();
        }
        public static string CreateDirectoryIfNotExists(string parentPath, string childPath, bool isUpLoadFolder)
        {
            if (isUpLoadFolder)
            {
                var parentPathExists = Directory.Exists(HttpContext.Current.Server.MapPath(@"/Uploads"));
                if (!parentPathExists)
                {
                    Directory.CreateDirectory(HttpContext.Current.Server.MapPath(@"/Uploads"));
                }

                parentPath = HttpContext.Current.Server.MapPath(@"/Uploads");
            }
            var path = Path.Combine(parentPath, childPath);
            var pathExists = Directory.Exists(path);
            if (!pathExists)
            {
                Directory.CreateDirectory(path);
            }
            return path;
        }
    }
}