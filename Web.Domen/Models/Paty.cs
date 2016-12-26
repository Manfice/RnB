using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace Web.Domen.Models
{
    public class Paty
    {
        public int Id { get; set; }
        public DateTime PatyDate { get; set; }
        public string Title { get; set; }
        [StringLength(50, ErrorMessage = "Поле Url дожно содержать от 6 до 50 символов", MinimumLength = 6)]
        [Required(ErrorMessage = "Поле Url обязательное поле")]
        public string RouteTitle { get; set; }
        public string Descr { get; set; }
        public int MaxGuests { get; set; }
        public decimal Price { get; set; }
        public string PatyInterest { get; set; }
        public int AddRate { get; set; }
        public string Dres { get; set; }
        public string Seets { get; set; }
        public string Place { get; set; }//Посадочные места.
        public string Address { get; set; }//Адрес вечеринки.
        public string MetaDescription { get; set; }
        public virtual PatyImage Avatar { get; set; }
        public virtual PatyCategory Category { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }

    public class PatyCategory
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Title { get; set; }
        [StringLength(50,ErrorMessage = "Поле Url дожно содержать от 6 до 50 символов",MinimumLength = 6)]
        [Required(ErrorMessage = "Поле Url обязательное поле")]
        public string RouteTitle { get; set; }
        public string MetaDescription { get; set; }
        public string Description { get; set; }
        public virtual PatyImage Avatar { get; set; }
        public virtual PatyImage Avatar1920 { get; set; }
        public virtual PatyCategory ParentCategory { get; set; }
        public virtual ICollection<Paty> Paties { get; set; }
        public virtual ICollection<PhotoAlbom> PhotoAlboms { get; set; }
    }

    public class PatyImage
    {
        [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Path { get; set; }
        public string FullPath { get; set; }
        public string ContentType { get; set; }
        public int ContentLength { get; set; }
    }

    public class PatyActionResult
    {
        public string[] Errors { get; set; }
        public bool Success { get; set; }
        public Paty Paty { get; set; }
    }
}