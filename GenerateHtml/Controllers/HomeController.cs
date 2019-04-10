using System;
using System.IO;
using System.Text;
using System.Web.Mvc;

namespace GenerateHtml.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ChooseOption()
        {
            var styleSheets = "<link href='css/owl.carousel.min.css' /><link href='css/fonts.css' />";
            var strContent = "<div class='wrap-components'><div class='form-group'><input type = 'text' class='form-control' placeholder='Website Name' name='WebsiteName' value='' /></div></div>";
            var textFile = ReplaceTextFile(styleSheets, strContent);
            var fileName = "index";
            using (var stream = new MemoryStream())
            {
                var writer = new StreamWriter(stream);
                writer.Write(textFile);
                writer.Flush();
                var subPath = "/_output";
                var exists = System.IO.Directory.Exists(Server.MapPath(subPath));
                if (!exists)
                {
                    System.IO.Directory.CreateDirectory(Server.MapPath(subPath));
                }
                var path = subPath + "/" + fileName + ".html";
                stream.Seek(0, SeekOrigin.Begin);
                using (var fs = new FileStream(Server.MapPath(path), FileMode.OpenOrCreate))
                {
                    stream.WriteTo(fs);
                    fs.Flush();
                }
            }
            return null;
        }
        private string ReplaceTextFile(string styleSheets, string strContent)
        {
            try
            {
                var fileStream = new FileStream(Server.MapPath(@"/CurrentLayout/layout.html"), FileMode.Open, FileAccess.Read);
                using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
                {
                    var textFile = streamReader.ReadToEnd();
                    textFile = textFile.Replace(Utils.LayoutStyleSheet, styleSheets);
                    textFile = textFile.Replace(Utils.LayoutContent, strContent);
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