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
            var strStyleSheets = "<link href='css/owl.carousel.min.css' /><link href='css/fonts.css' />";
            var strHeader = "<div class='container'>test</div>";
            var strContent = "<div class='wrap-components'><div class='form-group'><input type = 'text' class='form-control' placeholder='Website Name' name='WebsiteName' value='' /></div></div>";
            var strFooter = "";
            var strScript = "";
            var websiteName = "testWeb";
            var fileName = "index";
            var textFile = ReplaceTextFile(strStyleSheets, strHeader, strContent, strFooter, strScript);
            GenarateHtmlFile(textFile, websiteName, fileName);
            return null;
        }

        private void GenarateCss()
        {

        }
        private void GenarateHtmlFile(string textFile, string websiteName, string fileName)
        {
            var websitePath = GetLocation(websiteName);
            using (var stream = new MemoryStream())
            {
                var writer = new StreamWriter(stream);
                writer.Write(textFile);
                writer.Flush();
                var path = websitePath + "/" + fileName + ".html";
                stream.Seek(0, SeekOrigin.Begin);
                using (var fs = new FileStream(path, FileMode.OpenOrCreate))
                {
                    stream.WriteTo(fs);
                    fs.Flush();
                }
            }
        }

        private string GetLocation(string websiteName)
        {
            var downLoadPath = Utils.GetDownloadFolderPath();
            var locationPath = downLoadPath + "/" + Utils.FolderDownload;
            var locationPathExists = System.IO.Directory.Exists(locationPath);
            if (!locationPathExists)
            {
                System.IO.Directory.CreateDirectory(locationPath);
            }
            var websitePath = locationPathExists + "/" + websiteName;
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