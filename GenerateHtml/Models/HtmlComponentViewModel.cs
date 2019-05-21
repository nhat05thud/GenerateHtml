using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Generate.EntityFrameWork.Models;

namespace GenerateHtml.Models
{
    public class HtmlComponentViewModel
    {
        [HiddenInput(DisplayValue = false)]
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Nhập tên")]
        public string Name { get; set; }
        [AllowHtml]
        [Required(ErrorMessage = "Nhập html")]
        public string HtmlBody { get; set; }
        public string ImageName { get; set; }
        public string CssPath { get; set; }
        public string ScriptPath { get; set; }
        public string MediaInfo { get; set; }
        [Required(ErrorMessage = "Chọn danh mục")]
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public List<HtmlComponentCategory> HtmlComponentCategories { get; set; }
    }

    public class HtmlComponentGroup
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}