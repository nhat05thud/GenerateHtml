using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Generate.EntityFrameWork.DbContext;
using Generate.EntityFrameWork.Models;
using GenerateHtml.ClassHelpers;
using GenerateHtml.Models;

namespace GenerateHtml.Areas.Admin.Controllers
{
    public class HomeController : Controller
    {
        // GET: Administrator
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetData()
        {
            using (var db = new GenerateHtmlDbContext())
            {
                var compList = db.HtmlComponents
                    .Select(x => new HtmlComponentViewModel
                    {
                        Id = x.Id,
                        Name = x.Name,
                        ImageName = x.ImageName
                    }).ToList();
                return Json(new { data = compList }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public ActionResult AddOrEdit(int id = 0)
        {
            if (id == 0)
            {
                return View(new HtmlComponentViewModel());
            }
            using (var db = new GenerateHtmlDbContext())
            {
                var compList = db.HtmlComponents
                    .Select(x => new HtmlComponentViewModel
                    {
                        Id = x.Id,
                        Name = x.Name,
                        HtmlBody = x.HtmlBody,
                        ScriptPath = x.ScriptPath,
                        CssPath = x.CssPath,
                        ImageName = x.ImageName
                    }).FirstOrDefault(x => x.Id == id);
                return View(compList);
            }
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult AddOrEdit(HtmlComponentViewModel comp, HttpPostedFileBase styleSheetFile, HttpPostedFileBase scriptFile, HttpPostedFileBase imageFile)
        {
            using (var db = new GenerateHtmlDbContext())
            {
                if (comp.Id == 0)
                {
                    var obj = new HtmlComponent
                    {
                        Name = comp.Name,
                        HtmlBody = comp.HtmlBody,
                        ScriptPath = comp.ScriptPath,
                        CssPath = comp.CssPath,
                        ImageName = comp.ImageName
                    };
                    db.HtmlComponents.Add(obj);
                    db.SaveChanges();
                    if (imageFile != null)
                    {
                        var imageFilePath = HandleUploadFile(obj.Id, imageFile, Utils.ImagePath);
                        obj.ImageName = imageFilePath;
                    }
                    if (styleSheetFile != null && Path.GetExtension(styleSheetFile.FileName) == ".css")
                    {
                        var cssFilePath = HandleUploadFile(obj.Id, styleSheetFile, Utils.CssPath);
                        obj.CssPath = cssFilePath;
                    }
                    if (scriptFile != null && Path.GetExtension(scriptFile.FileName) == ".js")
                    {
                        var scriptFilePath = HandleUploadFile(obj.Id, scriptFile, Utils.ScriptsPath);
                        obj.ScriptPath = scriptFilePath;
                    }
                    db.Entry(obj).State = EntityState.Modified;
                    db.SaveChanges();
                    return Json(new { success = true, message = "Saved successfully!!!" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var componentItem = db.HtmlComponents
                        .Select(x => new HtmlComponentViewModel
                        {
                            Id = x.Id,
                            ScriptPath = x.ScriptPath,
                            CssPath = x.CssPath,
                            ImageName = x.ImageName
                        }).FirstOrDefault(x => x.Id == comp.Id);
                    if (componentItem != null)
                    {
                        var obj = new HtmlComponent
                        {
                            Id = comp.Id,
                            Name = comp.Name,
                            HtmlBody = comp.HtmlBody,
                            CssPath = componentItem.CssPath,
                            ScriptPath = componentItem.ScriptPath,
                            ImageName = componentItem.ImageName
                        };

                        if (imageFile != null)
                        {
                            HandleDeleteFile(comp.ImageName, Utils.ImagePath);
                            var imageFilePath = HandleUploadFile(obj.Id, imageFile, Utils.ImagePath);
                            obj.ImageName = imageFilePath;
                        }
                        if (styleSheetFile != null && Path.GetExtension(styleSheetFile.FileName) == ".css")
                        {
                            HandleDeleteFile(comp.CssPath, Utils.CssPath);
                            var cssFilePath = HandleUploadFile(obj.Id, styleSheetFile, Utils.CssPath);
                            obj.CssPath = cssFilePath;
                        }
                        if (scriptFile != null && Path.GetExtension(scriptFile.FileName) == ".js")
                        {
                            HandleDeleteFile(comp.ScriptPath, Utils.ScriptsPath);
                            var scriptFilePath = HandleUploadFile(obj.Id, scriptFile, Utils.ScriptsPath);
                            obj.ScriptPath = scriptFilePath;
                        }
                        db.Entry(obj).State = EntityState.Modified;
                        db.SaveChanges();
                        return Json(new { success = true, message = "Updated successfully!!!" }, JsonRequestBehavior.AllowGet);
                    }
                    return Json(new { success = false, message = "Updated fail!!!" }, JsonRequestBehavior.AllowGet);
                }
            }
        }

        private void HandleDeleteFile(string path, string folderName)
        {
            var uploadPathExists = Directory.Exists(Server.MapPath(@"/Uploads"));
            if (uploadPathExists)
            {
                if (!string.IsNullOrEmpty(path))
                {
                    var folder = Path.Combine(Server.MapPath(@"/Uploads"), folderName);
                    var di = new DirectoryInfo(folder);
                    foreach (var fileDi in di.GetFiles())
                    {
                        if (fileDi.Name == path)
                        {
                            fileDi.Delete();
                        }
                    }
                }
            }
        }

        private string HandleUploadFile(int id, HttpPostedFileBase file, string folderPath)
        {
            var cssPath = Utils.CreateDirectoryIfNotExists(Utils.UploadFolder, folderPath, true);
            var stream = file.InputStream;
            var fileEx = Path.GetExtension(file.FileName);
            var fileName = folderPath + "_" + id + fileEx;
            var path = Path.Combine(cssPath, fileName);
            using (var fileStream = System.IO.File.Create(path))
            {
                stream.CopyTo(fileStream);
            }

            return fileName;
        }
        [HttpPost]
        public ActionResult Delete(int id)
        {
            using (var db = new GenerateHtmlDbContext())
            {
                var comp = db.HtmlComponents.FirstOrDefault(x => x.Id == id);
                if (comp != null)
                {
                    db.HtmlComponents.Remove(comp);
                    db.SaveChanges();
                    return Json(new { success = true, message = "Deleted successfully!!!" }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { success = false, message = "Error!!!" }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}