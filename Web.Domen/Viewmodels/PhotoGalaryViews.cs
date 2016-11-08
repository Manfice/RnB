using System;
using System.Collections.Generic;
using Web.Domen.Models;

namespace Web.Domen.Viewmodels
{
    public class PhotoGalaryViews
    {
         
    }

    public class AlbomsViewmodel
    {
        public IEnumerable<PhotoAlbom> Alboms { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }

    public class PagingInfo
    {
        public int TotalItems { get; set; }
        public int ItemsPerPage { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages => (int) Math.Ceiling((decimal) TotalItems/ItemsPerPage);
    }
}