using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;

namespace Generate.EntityFrameWork.Models
{
    public class HtmlComponent
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string HtmlBody { get; set; }
        public string ImageName { get; set; }
        public string CssPath { get; set; }
        public string ScriptPath { get; set; }
        [ForeignKey("HtmlComponentCategory")]
        public int CategoryId { get; set; }
        public HtmlComponentCategory HtmlComponentCategory { get; set; }
    }
}
