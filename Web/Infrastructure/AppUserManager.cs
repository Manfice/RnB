using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Web.Models;

namespace Web.Infrastructure
{
    public class AppUserManager:UserManager<AppUser>
    {
        public AppUserManager(IUserStore<AppUser> store):base(store) { }

        public static AppUserManager Create(IdentityFactoryOptions<AppUserManager> options, IOwinContext context )
        {
            var db = context.Get<AppIdentityDbContext>();
            var manager = new AppUserManager(new UserStore<AppUser>(db))
            {
                PasswordValidator = new PasswordValidator
                {
                    RequiredLength = 6,
                    RequireDigit = false,
                    RequireNonLetterOrDigit = false,
                    RequireLowercase = false,
                    RequireUppercase = false
                }
            };

            manager.UserValidator = new UserValidator<AppUser>(manager)
            {
                RequireUniqueEmail = true, AllowOnlyAlphanumericUserNames = true
            };
            return manager;
        }
    }
}