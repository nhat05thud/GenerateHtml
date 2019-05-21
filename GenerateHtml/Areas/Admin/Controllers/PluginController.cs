using System;
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
    public class PluginController : Controller
    {
        // GET: Admin/Plugins
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetData()
        {
            try
            {
                using (var db = new GenerateHtmlDbContext())
                {
                    var plList = db.Plugins.Select(x => new PluginViewModel
                    {
                        Id = x.Id,
                        Name = x.Name
                    }).ToList();
                    return Json(new { data = plList }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        [HttpGet]
        public ActionResult AddOrEdit(int id = 0)
        {
            if (id == 0)
            {
                return View(new PluginViewModel());
            }
            using (var db = new GenerateHtmlDbContext())
            {
                var plList = db.Plugins
                    .Select(x => new PluginViewModel
                    {
                        Id = x.Id,
                        Name = x.Name,
                        CssPath = x.CssPath,
                        ScriptPath = x.ScriptPath
                    }).FirstOrDefault(x => x.Id == id);
                return View(plList);
            }
        }
        [HttpPost]
        public ActionResult AddOrEdit(PluginViewModel plugin, HttpPostedFileBase styleSheetFile, HttpPostedFileBase scriptFile)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("~/Areas/Admin/Views/Plugin/_form.cshtml", plugin);
            }
            using (var db = new GenerateHtmlDbContext())
            {
                if (plugin.Id == 0)
                {
                    var item = db.Plugins.FirstOrDefault(x =>
                        x.Name.Trim().ToLower() == plugin.Name.Trim().ToLower());
                    if (item != null)
                    {
                        return Json(new { success = false, message = "Plugin đã tồn tại." }, JsonRequestBehavior.AllowGet);
                    }
                    var obj = new Plugin
                    {
                        Id = plugin.Id,
                        Name = plugin.Name
                    };
                    db.Plugins.Add(obj);
                    db.SaveChanges();
                    if (styleSheetFile != null && Path.GetExtension(styleSheetFile.FileName) == ".css")
                    {
                        var cssFilePath = HandleUploadFile(obj.Id, styleSheetFile, obj.Name, Utils.CssPath);
                        obj.CssPath = cssFilePath;
                    }
                    if (scriptFile != null && Path.GetExtension(scriptFile.FileName) == ".js")
                    {
                        var scriptFilePath = HandleUploadFile(obj.Id, scriptFile, obj.Name, Utils.ScriptsPath);
                        obj.ScriptPath = scriptFilePath;
                    }
                    db.Entry(obj).State = EntityState.Modified;
                    db.SaveChanges();
                    return Json(new { success = true, message = "Lưu thành công." }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var item = db.Plugins.FirstOrDefault(x => x.Id == plugin.Id);
                    if (item != null)
                    {
                        var data = db.Plugins.FirstOrDefault(x =>
                            x.Name.Trim().ToLower() == plugin.Name.Trim().ToLower() && x.Id != plugin.Id);
                        if (data != null)
                        {
                            return Json(new { success = false, message = "Plugin đã tồn tại." }, JsonRequestBehavior.AllowGet);
                        }
                        item.Name = plugin.Name;
                        if (styleSheetFile != null && Path.GetExtension(styleSheetFile.FileName) == ".css")
                        {
                            HandleDeleteFile(plugin.CssPath, Utils.CssPath, plugin.Name);
                            var cssFilePath = HandleUploadFile(item.Id, styleSheetFile, item.Name, Utils.CssPath);
                            item.CssPath = cssFilePath;
                        }
                        if (scriptFile != null && Path.GetExtension(scriptFile.FileName) == ".js")
                        {
                            HandleDeleteFile(plugin.ScriptPath, Utils.ScriptsPath, plugin.Name);
                            var scriptFilePath = HandleUploadFile(item.Id, scriptFile, item.Name, Utils.ScriptsPath);
                            item.ScriptPath = scriptFilePath;
                        }
                        db.Entry(item).State = EntityState.Modified;
                        db.SaveChanges();
                        return Json(new { success = true, message = "Sửa thành công." }, JsonRequestBehavior.AllowGet);
                    }
                    return Json(new { success = false, message = "Lỗi." }, JsonRequestBehavior.AllowGet);
                }
            }
        }
        private string HandleUploadFile(int id, HttpPostedFileBase file, string libsName , string folderName)
        {
            var folderPath = Utils.CreateDirectoryIfNotExists(Utils.LibsFolder, ChangeSymbol.DoChange(libsName), folderName, true);
            var stream = file.InputStream;
            var filePath = Path.Combine(folderPath, file.FileName);
            using (var fileStream = System.IO.File.Create(filePath))
            {
                stream.CopyTo(fileStream);
            }
            return file.FileName;
        }
        private void HandleDeleteFile(string path, string folderName, string libsName)
        {
            var libsPathExists = Directory.Exists(Server.MapPath(@"/libs"));
            if (libsPathExists)
            {
                if (!string.IsNullOrEmpty(path))
                {
                    var folder = Path.Combine(Server.MapPath(@"/libs"), ChangeSymbol.DoChange(libsName), folderName);
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
        [HttpPost]
        public ActionResult Delete(int id)
        {
            using (var db = new GenerateHtmlDbContext())
            {
                var comp = db.Plugins.FirstOrDefault(x => x.Id == id);
                if (comp != null)
                {
                    db.Plugins.Remove(comp);
                    db.SaveChanges();
                    return Json(new { success = true, message = "Xóa thành công." }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { success = false, message = "Đã có lỗi xảy ra." }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}