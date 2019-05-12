using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GenerateHtml.Models
{
    public class HtmlComponentCategoryViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Nhập tên")]
        public string Name { get; set; }
    }
    public class ListComponentWithCategory
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public List<HtmlComponentViewModel> ListComponent { get; set; }
    }
}