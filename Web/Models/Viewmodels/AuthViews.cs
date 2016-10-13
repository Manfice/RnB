using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Web.Models.Viewmodels
{
    public class AuthViews
    {
         
    }

    public class RoleEditView
    {
        public AppRole Role { get; set; }
        public IEnumerable<AppUser> Members { get; set; }
        public IEnumerable<AppUser> NonMember { get; set; }
    }

    public class RoleModModel
    {
        [Required]
        public string Name { get; set; }

        public string[] IdsToAdd { get; set; }
        public string[] IdsToRemove { get; set; }
    }
}