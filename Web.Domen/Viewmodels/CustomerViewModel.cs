using Web.Domen.Models;

namespace Web.Domen.Viewmodels
{
    public class CustomerVmLk
    {
        public Customer Customer { get; set; }
        public bool Fio { get; set; } 
        public bool Phone { get; set; } 
        public bool Mail { get; set; } 
        public bool City { get; set; } 
        public bool Birthday { get; set; } 
        public bool WorkPlace { get; set; } 
        public bool F { get; set; } 
        public bool I { get; set; } 
        public bool V { get; set; } 
        public bool O { get; set; } 
        public bool T { get; set; } 
        public string Sex { get; set; }
        public string OldPass { get; set; }
        public string NewPass { get; set; }
        public string Confirm { get; set; }
    }
}