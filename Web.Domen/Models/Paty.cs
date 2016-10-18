using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web.Domen.Models
{
    public class Paty
    {
        [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime PatyDate { get; set; }
        public string Title { get; set; }
        public string Descr { get; set; }
        public int MaxGuests { get; set; }
        public decimal Price { get; set; }
        public string PatyInterest { get; set; }
        public virtual PatyImage Avatar { get; set; }
        public virtual PatyCategory Category { get; set; }
    }

    public class PatyCategory
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public virtual PatyImage Avatar { get; set; }
        public virtual PatyCategory ParentCategory { get; set; }
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
}