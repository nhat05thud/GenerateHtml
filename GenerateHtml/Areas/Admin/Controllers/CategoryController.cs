using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Generate.EntityFrameWork.DbContext;
using Generate.EntityFrameWork.Models;
using GenerateHtml.Models;

namespace GenerateHtml.Areas.Admin.Controllers
{
    public class CategoryController : Controller
    {
        // GET: Admin/Category
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
                    var compList = db.HtmlComponentCategories.Select(x => new HtmlComponentCategoryViewModel
                    {
                        Id = x.Id,
                        Name = x.Name
                    }).ToList();
                    return Json(new { data = compList }, JsonRequestBehavior.AllowGet);
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
                return View(new HtmlComponentCategoryViewModel());
            }
            using (var db = new GenerateHtmlDbContext())
            {
                var compList = db.HtmlComponentCategories
                    .Select(x => new HtmlComponentCategoryViewModel
                    {
                        Id = x.Id,
                        Name = x.Name
                    }).FirstOrDefault(x => x.Id == id);
                return View(compList);
            }
        }
        [HttpPost]
        public ActionResult AddOrEdit(HtmlComponentCategoryViewModel comp)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("~/Areas/Admin/Views/Category/_form.cshtml", comp);
            }
            using (var db = new GenerateHtmlDbContext())
            {
                if (comp.Id == 0)
                {
                    var obj = new HtmlComponentCategory
                    {
                        Name = comp.Name
                    };
                    db.HtmlComponentCategories.Add(obj);
                    db.SaveChanges();
                    return Json(new { success = true, message = "Saved successfully!!!" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var obj = new HtmlComponentCategory
                    {
                        Id = comp.Id,
                        Name = comp.Name
                    };
                    db.Entry(obj).State = EntityState.Modified;
                    db.SaveChanges();
                    return Json(new { success = true, message = "Updated successfully!!!" }, JsonRequestBehavior.AllowGet);
                }
            }
        }
        [HttpPost]
        public ActionResult Delete(int id)
        {
            using (var db = new GenerateHtmlDbContext())
            {
                var comp = db.HtmlComponentCategories.FirstOrDefault(x => x.Id == id);
                if (comp != null)
                {
                    db.HtmlComponentCategories.Remove(comp);
                    db.SaveChanges();
                    return Json(new { success = true, message = "Deleted successfully!!!" }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { success = false, message = "Error!!!" }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}