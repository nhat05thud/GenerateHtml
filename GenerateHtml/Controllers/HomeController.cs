using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Antlr.Runtime.Tree;
using Generate.EntityFrameWork.DbContext;
using GenerateHtml.ClassHelpers;
using GenerateHtml.Models;

namespace GenerateHtml.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            using (var db = new GenerateHtmlDbContext())
            {
                var compList = db.HtmlComponents
                    .Select(x => new HtmlComponentViewModel
                    {
                        Id = x.Id,
                        ImageName = x.ImageName
                    }).ToList();
                return View(compList);
            }
        }
        public ActionResult TestLoadHtmlPage(int elementId)
        {
            using (var db = new GenerateHtmlDbContext())
            {
                var compList = db.HtmlComponents
                    .Select(x => new HtmlComponentViewModel
                    {
                        Id = x.Id,
                        Name = x.Name,
                        HtmlBody = x.HtmlBody,
                        ScriptPath = x.ScriptPath,
                        CssPath = x.CssPath
                    }).FirstOrDefault(x => x.Id == elementId);
                return compList != null 
                    ? Json(new { success = true, css = compList.CssPath, script = compList.ScriptPath, html = compList.HtmlBody }, JsonRequestBehavior.AllowGet) 
                    : Json(new { success = false, message = "an error occurred !!!" }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult ChooseOption()
        {
            var listStyleSheetLinks = new List<string>
            {
                "/css/owl.carousel.min.css",
                "/css/fonts.css"
            };
            var strStyleSheets = string.Empty;
            foreach (var item in listStyleSheetLinks)
            {
                strStyleSheets += "<link href='" + item + "' rel='stylesheet' />";
            }
            var strHeader = "<div class='container'>test</div>";
            var strContent = "<div class='wrap-components'><div class='form-group'><input type = 'text' class='form-control' placeholder='Website Name' name='WebsiteName' value='' /></div></div>";
            var strFooter = "";
            var strScript = string.Empty;
            var listScriptLinks = new List<string>
            {
                "/script/owl.carousel.min.js",
                "/script/jquery.min.js"
            };
            foreach (var item in listScriptLinks)
            {
                strScript += "<script src='" + item + "'></script>";
            }
            var websiteName = "testWeb";
            var fileName = "index";
            var textFile = ReplaceTextFile(strStyleSheets, strHeader, strContent, strFooter, strScript);
            GenarateHtmlFile(textFile, websiteName, fileName);
            GenarateCss(websiteName, listStyleSheetLinks);
            GenarateScript(websiteName, listScriptLinks);
            CopyAllFont(Server.MapPath(@"/fonts"), Utils.FontFolder);
            return null;
        }

        private void CopyAllFont(string stableFolder, string updateFolder)
        {
            var originalFiles = Directory.GetFiles(stableFolder, "*", SearchOption.AllDirectories);
            Array.ForEach(originalFiles, (originalFileLocation) =>
            {
                var originalFile = new FileInfo(originalFileLocation);
                var destFile = new FileInfo(originalFileLocation.Replace(stableFolder, updateFolder));
                if (destFile.Exists)
                {
                    if (originalFile.Length > destFile.Length)
                    {
                        originalFile.CopyTo(destFile.FullName, true);
                    }
                }
                else
                {
                    Directory.CreateDirectory(destFile.DirectoryName);
                    originalFile.CopyTo(destFile.FullName, false);
                }
            });
        }
        private void GenarateCss(string websiteName, IEnumerable<string> listStyleSheetLinks)
        {
            var websitePath = GetLocation(websiteName);
            var cssPath = Utils.CreateDirectoryIfNotExists(websitePath, Utils.CssPath, false);
            foreach (var item in listStyleSheetLinks)
            {
                var path = Path.Combine(cssPath, item.Split('/')[item.Split('/').Length - 1]);
                if (!System.IO.File.Exists(path))
                {
                    GenarateFile(item, path);
                }
            }
        }
        private void GenarateScript(string websiteName, List<string> listScriptLinks)
        {
            var websitePath = GetLocation(websiteName);
            var scriptsPath = Utils.CreateDirectoryIfNotExists(websitePath, Utils.ScriptsPath, false);
            foreach (var item in listScriptLinks)
            {
                var path = Path.Combine(scriptsPath, item.Split('/')[item.Split('/').Length - 1]);
                if (!System.IO.File.Exists(path))
                {
                    GenarateFile(item, path);
                }
            }
        }
        private void GenarateHtmlFile(string textFile, string websiteName, string fileName)
        {
            var websitePath = GetLocation(websiteName);
            using (var stream = new MemoryStream())
            {
                var writer = new StreamWriter(stream);
                writer.Write(textFile);
                writer.Flush();
                var path = Path.Combine(websitePath, fileName + ".html");
                stream.Seek(0, SeekOrigin.Begin);
                using (var fs = new FileStream(path, FileMode.OpenOrCreate))
                {
                    stream.WriteTo(fs);
                    fs.Flush();
                }
            }
        }
        private void GenarateFile(string linkFile, string path)
        {
            var fileStream = new FileStream(Server.MapPath(linkFile), FileMode.Open, FileAccess.Read);
            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
            {
                var textFile = streamReader.ReadToEnd();
                using (var stream = new MemoryStream())
                {
                    var writer = new StreamWriter(stream);
                    writer.Write(textFile);
                    writer.Flush();
                    stream.Seek(0, SeekOrigin.Begin);
                    using (var fs = new FileStream(path, FileMode.OpenOrCreate))
                    {
                        stream.WriteTo(fs);
                        fs.Flush();
                    }
                }
            }
        }
        private string GetLocation(string websiteName)
        {
            var downLoadPath = Utils.GetDownloadFolderPath();
            var locationPath = Path.Combine(downLoadPath, Utils.FolderDownload);
            var locationPathExists = System.IO.Directory.Exists(locationPath);
            if (!locationPathExists)
            {
                System.IO.Directory.CreateDirectory(locationPath);
            }
            var websitePath = Path.Combine(locationPath, websiteName);
            var websitePathExists = System.IO.Directory.Exists(websitePath);
            if (!websitePathExists)
            {
                System.IO.Directory.CreateDirectory(websitePath);
            }
            return websitePath;
        }
        private string ReplaceTextFile(string strStyleSheets, string strHeader, string strContent, string strFooter, string strScript)
        {
            try
            {
                var fileStream = new FileStream(Server.MapPath(@"/CurrentLayout/layout.html"), FileMode.Open, FileAccess.Read);
                using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
                {
                    var textFile = streamReader.ReadToEnd();
                    textFile = textFile.Replace(Utils.LayoutStyleSheet, strStyleSheets);
                    textFile = textFile.Replace(Utils.LayoutHeader, strHeader);
                    textFile = textFile.Replace(Utils.LayoutContent, strContent);
                    textFile = textFile.Replace(Utils.LayoutFooter, strFooter);
                    textFile = textFile.Replace(Utils.LayoutScript, strScript);
                    return textFile;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}