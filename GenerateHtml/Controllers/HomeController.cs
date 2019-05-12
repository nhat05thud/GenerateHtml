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
using Ionic.Zip;

namespace GenerateHtml.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            using (var db = new GenerateHtmlDbContext())
            {
                var compList = db.HtmlComponents
                    .GroupBy(x => new HtmlComponentGroup
                    {
                        Id = x.CategoryId,
                        Name = x.HtmlComponentCategory.Name
                    }, (key, group) => new ListComponentWithCategory
                    {
                        CategoryId = key.Id,
                        CategoryName = key.Name,
                        ListComponent = group.Select(y => new HtmlComponentViewModel
                        {
                            Id = y.Id,
                            Name = y.Name,
                            ImageName = y.ImageName,
                            CategoryId = y.CategoryId,
                            CategoryName = y.HtmlComponentCategory.Name
                        }).ToList()
                    })
                    .ToList();
                return View(compList);
            }
        }
        public ActionResult LoadHtmlComponent(int elementId)
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
        [HttpPost, ValidateInput(false)]
        public ActionResult HandleSaveHtmlFile(string html, string elementIds, string websiteName, string fileName)
        {
            using (var db = new GenerateHtmlDbContext())
            {
                websiteName = ChangeSymbol.DoChange(websiteName.Trim());
                fileName = ChangeSymbol.DoChange(fileName.Trim());
                var compList = db.HtmlComponents
                    .Select(x => new HtmlComponentViewModel
                    {
                        Id = x.Id,
                        ScriptPath = x.ScriptPath,
                        CssPath = x.CssPath
                    }).Where(x => elementIds.Contains(x.Id.ToString())).ToList();
                var listStyleSheetLinks = compList.Where(x => x.CssPath != null).Select(x => x.CssPath).ToList();
                var strStyleSheets = string.Empty;
                foreach (var item in listStyleSheetLinks)
                {
                    strStyleSheets += "<link href='" + Path.Combine(Utils.CssPath, item).Replace("\\", "/") +
                                      "' rel='stylesheet' />\n";
                }

                var listScriptLinks = compList.Where(x => x.ScriptPath != null).Select(x => x.ScriptPath).ToList();
                var strScript = string.Empty;
                foreach (var item in listScriptLinks)
                {
                    strScript += "<script src='" + Path.Combine(Utils.ScriptsPath, item).Replace("\\", "/") +
                                 "'></script>\n";
                }

                // replace html body vào layout
                var textFile = ReplaceTextFile(strStyleSheets, html, strScript);
                // tạo file html
                GenarateHtmlFile(textFile, websiteName, fileName);
                // chép file css vào folder
                GenarateCss(websiteName, listStyleSheetLinks);
                // chép file script vào folder
                GenarateScript(websiteName, listScriptLinks);
                return Json(new { success = true, message = "Add new html page success !!!" }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult HandleDownloadHtml(string websiteName)
        {
            websiteName = ChangeSymbol.DoChange(websiteName.Trim());
            try
            {
                var sourceFolderPath = Server.MapPath(Path.Combine(Utils.DownLoadFolder, websiteName));
                if (Directory.Exists(sourceFolderPath))
                {
                    var memoryStream = new MemoryStream();
                    using (var zip = new ZipFile())
                    {
                        zip.AddDirectory(sourceFolderPath);
                        //zip.Save(Path.Combine(Server.MapPath(Utils.DownLoadFolder), websiteName + ".zip"));
                        zip.Save(memoryStream);
                        memoryStream.Seek(0, SeekOrigin.Begin);
                        return File(memoryStream.ToArray(), "application/zip", websiteName + ".zip");
                    }
                }

                return Json(new { success = false, message = "Website not exists !!!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        private void DeleteAllFiles()
        {
            var tempFolder = new DirectoryInfo(Server.MapPath(Utils.DownLoadFolder));
            foreach (var file in tempFolder.GetFiles())
            {
                file.Delete();
            }
            foreach (var directory in tempFolder.GetDirectories())
            {
                directory.Delete(true);
            }
        }
        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            var dir = new DirectoryInfo(sourceDirName);
            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            var dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            var files = dir.GetFiles();
            foreach (var file in files)
            {
                var temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (var subdir in dirs)
                {
                    var temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }
        private void GenarateCss(string websiteName, IEnumerable<string> listStyleSheetLinks)
        {
            var websitePath = GetLocation(websiteName);
            var cssPath = Utils.CreateDirectoryIfNotExists(websitePath, Utils.CssPath, false);
            foreach (var item in listStyleSheetLinks)
            {
                var fullItemLink = Path.Combine(Utils.UploadFolder, Utils.CssPath, item);
                var path = Path.Combine(cssPath, item);
                if (!System.IO.File.Exists(path))
                {
                    GenarateFile(fullItemLink, path);
                }
            }
        }
        private void GenarateScript(string websiteName, List<string> listScriptLinks)
        {
            var websitePath = GetLocation(websiteName);
            var scriptsPath = Utils.CreateDirectoryIfNotExists(websitePath, Utils.ScriptsPath, false);
            foreach (var item in listScriptLinks)
            {
                var fullItemLink = Path.Combine(Utils.UploadFolder, Utils.ScriptsPath, item);
                var path = Path.Combine(scriptsPath, item);
                if (!System.IO.File.Exists(path))
                {
                    GenarateFile(fullItemLink, path);
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
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                    path = Path.Combine(websitePath, fileName + ".html");
                }
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
            var downLoadPath = Server.MapPath(Utils.DownLoadFolder);
            var websitePath = Path.Combine(downLoadPath, websiteName);
            var websitePathExists = Directory.Exists(websitePath);
            if (!websitePathExists)
            {
                Directory.CreateDirectory(websitePath);
                // copy tất cả file mặc định vào folder mới
                DirectoryCopy(Server.MapPath(@"/CurrentLayout/assets"), Server.MapPath(Path.Combine(Utils.DownLoadFolder, websiteName)), true);
            }
            return websitePath;
        }
        private string ReplaceTextFile(string strStyleSheets, string html, string strScript)
        {
            try
            {
                var fileStream = new FileStream(Server.MapPath(@"/CurrentLayout/layout.html"), FileMode.Open, FileAccess.Read);
                using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
                {
                    var textFile = streamReader.ReadToEnd();
                    textFile = textFile.Replace(Utils.LayoutStyleSheet, strStyleSheets);
                    textFile = textFile.Replace(Utils.LayoutBody, html);
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