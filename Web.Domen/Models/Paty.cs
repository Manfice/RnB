using System;
using System.ComponentModel.DataAnnotations;

namespace Web.Domen.Models
{
    public class Paty
    {
        public string Id { get; set; }
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime PatyDate { get; set; }
        public string Title { get; set; }
        public string Descr { get; set; }
        public int MaxGuests { get; set; }
        public decimal Price { get; set; }
        public string[] PatyInterest { get; set; }
        public virtual PatyImage Avatar { get; set; }
        public virtual PatyCategory Category { get; set; }
    }

    public class PatyCategory
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public virtual PatyImage Avatar { get; set; }
        public virtual PatyCategory ParentCategory { get; set; }
    }

    public class PatyImage
    {
        public string Id { get; set; }
        public string Path { get; set; }
        public string FullPath { get; set; }
        public string ContentType { get; set; }
        public int ContentLength { get; set; }
    }
}