using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Generate.EntityFrameWork.DbContext;
using Generate.EntityFrameWork.Models;
using GenerateHtml.Models;

namespace GenerateHtml.Controllers
{
    public class AdministratorController : Controller
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
                        Name = x.Name
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
                        Name = x.Name
                    }).ToList();
                var view = compList.FirstOrDefault(x => x.Id == id);

                return View(view);
            }
        }
        [HttpPost]
        public ActionResult AddOrEdit(HtmlComponentViewModel comp) 
        {
            using (var db = new GenerateHtmlDbContext())
            {
                if (comp.Id == 0)
                {
                    var obj = new HtmlComponent
                    {
                        Name = comp.Name
                    };
                    db.HtmlComponents.Add(obj);
                    db.SaveChanges();
                    return Json(new { success = true, message = "Saved successfully!!!" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var obj = new HtmlComponent
                    {
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