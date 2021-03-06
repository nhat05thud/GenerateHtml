﻿using System.ComponentModel;
using System.Data.Entity;
using Generate.EntityFrameWork.Models;

namespace Generate.EntityFrameWork.DbContext
{
    public class GenerateHtmlDbContext : System.Data.Entity.DbContext
    {
        public GenerateHtmlDbContext()
            : base("GenerateHtmlDbContext")
        {
        }
        public virtual DbSet<HtmlComponent> HtmlComponents { get; set; }
        public virtual DbSet<HtmlComponentCategory> HtmlComponentCategories { get; set; }
        public virtual DbSet<Plugin> Plugins { get; set; }
    }
}
