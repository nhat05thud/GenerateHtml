using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Generate.EntityFrameWork.Models
{
    public class HtmlComponentCategory
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<HtmlComponent> HtmlComponents { get; set; }
    }
}
