using Microsoft.AspNet.Identity.EntityFramework;

namespace Web.Models
{
    public class AppUser:IdentityUser
    {
        public string Fio { get; set; }
    }
}