using System;
using System.Web.Mvc;

namespace Web.Domen.Models
{
    public class Blog
    {
        public int Id { get; set; }
        public DateTime BlogData { get; set; }
        public string Title { get; set; }
        [AllowHtml]
        public string BlogBody { get; set; }
        public string Avatar { get; set; }
        public string FullPath { get; set; }
    }
}