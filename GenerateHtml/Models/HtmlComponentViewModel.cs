using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace GenerateHtml.Models
{
    public class HtmlComponentViewModel
    {
        [HiddenInput(DisplayValue = false)]
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        [AllowHtml]
        public string HtmlBody { get; set; }
        public string ImageName { get; set; }
        public string CssPath { get; set; }
        public string ScriptPath { get; set; }
    }
}