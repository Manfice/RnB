using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Web.Domen.Models
{
    public class ImageGalary
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public virtual ICollection<ImageData> Photos { get; set; }
        public virtual PhotoAlbom Albom { get; set; }
    }

    public class PhotoAlbom
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Укажите наименование альбома")]
        public string Title { get; set; }
        public string Description { get; set; }
        public string Avatar { get; set; }
        public string AvatarFullPath { get; set; }
        public int Viewed { get; set; }
        public int Likes { get; set; }
        public bool Active { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime AlbomDate { get; set; }
        public virtual ICollection<ImageGalary> Regions { get; set; }
        public virtual PatyCategory Category { get; set; }
    }

    public class ImageData
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Path { get; set; }
        public string FullPath { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public bool TitleView { get; set; }
        public int Viewed { get; set; }
        public int Likes { get; set; }
        public string VideoLink { get; set; }
        public virtual ImageGalary Region { get; set; }
    }
}