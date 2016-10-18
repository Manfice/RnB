using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Web.Models
{
    [Table("Customer")]
    public class AppUser:IdentityUser
    {
        //public string Fio { get; set; }
    }
}