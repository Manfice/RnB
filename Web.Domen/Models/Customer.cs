using System;
using System.Collections.Generic;

namespace Web.Domen.Models
{
    public class Customer
    {
        public int Id { get; set; }
        public string NickName { get; set; }
        public string Fio { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string City { get; set; }
        public DateTime? Birthday { get; set; }
        public string Work { get; set; }
        public string WorkPlace { get; set; }
        public string User { get; set; }
        public int Rate { get; set; }
        public virtual CustImage Avatar { get; set; }
        public virtual ICollection<Paty> Paties { get; set; }
    }

    public class CustImage
    {
        public int Id { get; set; }
        public string Path { get; set; }
        public string FullPath { get; set; }
        public string ContentType { get; set; }

    }
    
}