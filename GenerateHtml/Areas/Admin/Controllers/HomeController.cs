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
                        ImageName = x.ImageName,
                        CategoryId = x.CategoryId,
                        CategoryName = x.HtmlComponentCategory.Name
                    }).ToList();
                return Json(new { data = compList }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public ActionResult AddOrEdit(int id = 0)
        {
            using (var db = new GenerateHtmlDbContext())
            {
                ViewBag.Category = db.HtmlComponentCategories
                    .Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.Name
                    }).ToList();
                if (id == 0)
                {
                    return View(new HtmlComponentViewModel());
                }
                var compList = db.HtmlComponents
                    .Select(x => new HtmlComponentViewModel
                    {
                        Id = x.Id,
                        Name = x.Name,
                        HtmlBody = x.HtmlBody,
                        ScriptPath = x.ScriptPath,
                        CssPath = x.CssPath,
                        ImageName = x.ImageName,
                        CategoryId = x.CategoryId,
                        CategoryName = x.HtmlComponentCategory.Name
                    }).FirstOrDefault(x => x.Id == id);
                if (compList != null && Directory.Exists(Server.MapPath(@"/Media/" + id)))
                {
                    var di = new DirectoryInfo(Server.MapPath(@"/Media/" + id));
                    var count = di.GetFiles().Length;
                    compList.MediaInfo = count + "files";
                }
                return View(compList);
            }
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult AddOrEdit(HtmlComponentViewModel comp, HttpPostedFileBase styleSheetFile, HttpPostedFileBase scriptFile, HttpPostedFileBase imageFile, List<HttpPostedFileBase> mediaFiles)
        {
            try
            {
                using (var db = new GenerateHtmlDbContext())
                {
                    ViewBag.Category = db.HtmlComponentCategories.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name }).ToList();
                    if (!ModelState.IsValid)
                    {
                        return PartialView("~/Areas/Admin/Views/Home/_form.cshtml", comp);
                    }
                    if (comp.Id == 0)
                    {
                        var obj = new HtmlComponent
                        {
                            Name = comp.Name,
                            HtmlBody = comp.HtmlBody,
                            ScriptPath = comp.ScriptPath,
                            CssPath = comp.CssPath,
                            ImageName = comp.ImageName,
                            CategoryId = comp.CategoryId
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
                        if (mediaFiles.Any())
                        {
                            HandleMediaImages(obj.Id, mediaFiles);
                        }
                        db.Entry(obj).State = EntityState.Modified;
                        db.SaveChanges();
                        return Json(new { success = true, message = "Lưu thành công." }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var item = db.HtmlComponents.FirstOrDefault(x => x.Id == comp.Id);
                        if (item != null)
                        {
                            item.Name = comp.Name;
                            item.HtmlBody = comp.HtmlBody;
                            item.CategoryId = comp.CategoryId;
                            if (imageFile != null)
                            {
                                HandleDeleteFile(comp.ImageName, Utils.ImagePath);
                                var imageFilePath = HandleUploadFile(item.Id, imageFile, Utils.ImagePath);
                                item.ImageName = imageFilePath;
                            }
                            if (styleSheetFile != null && Path.GetExtension(styleSheetFile.FileName) == ".css")
                            {
                                HandleDeleteFile(comp.CssPath, Utils.CssPath);
                                var cssFilePath = HandleUploadFile(item.Id, styleSheetFile, Utils.CssPath);
                                item.CssPath = cssFilePath;
                            }
                            if (scriptFile != null && Path.GetExtension(scriptFile.FileName) == ".js")
                            {
                                HandleDeleteFile(comp.ScriptPath, Utils.ScriptsPath);
                                var scriptFilePath = HandleUploadFile(item.Id, scriptFile, Utils.ScriptsPath);
                                item.ScriptPath = scriptFilePath;
                            }
                            if (mediaFiles.Any())
                            {
                                HandleMediaImages(item.Id, mediaFiles);
                            }
                            db.Entry(item).State = EntityState.Modified;
                            db.SaveChanges();
                            return Json(new { success = true, message = "Sửa thành công." }, JsonRequestBehavior.AllowGet);
                        }
                        return Json(new { success = false, message = "Sửa thất bại" }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Sửa thất bại" }, JsonRequestBehavior.AllowGet);
            }
        }

        private void HandleMediaImages(int id, List<HttpPostedFileBase> mediaFiles)
        {
            var mediaFolderPath = Server.MapPath(@"/Media");
            var mediaItemPath = Path.Combine(mediaFolderPath, id.ToString());
            if (!Directory.Exists(mediaItemPath))
            {
                Directory.CreateDirectory(mediaItemPath);
            }
            foreach (var item in mediaFiles)
            {
                if (item != null && Utils.CheckExtensionImage(item.FileName))
                {
                    var stream = item.InputStream;
                    var path = Path.Combine(mediaItemPath, item.FileName);
                    using (var fileStream = System.IO.File.Create(path))
                    {
                        stream.CopyTo(fileStream);
                    }
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
                    return Json(new { success = true, message = "Xóa thành công." }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { success = false, message = "Đã có lỗi xảy ra." }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}