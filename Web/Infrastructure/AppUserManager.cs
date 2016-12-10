using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.DataProtection;
using Web.Models;

namespace Web.Infrastructure
{
    public class AppUserManager:UserManager<AppUser>
    {
        public AppUserManager(IUserStore<AppUser> store) : base(store)
        {
        }

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

            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider!=null)
            {
                manager.UserTokenProvider = new DataProtectorTokenProvider<AppUser>(dataProtectionProvider.Create("Red&Black Club"))
                { TokenLifespan = TimeSpan.FromDays(10)};
                
            }

            manager.UserValidator = new UserValidator<AppUser>(manager)
            {
                RequireUniqueEmail = true, AllowOnlyAlphanumericUserNames = false
            };
            return manager;
        }
    }
}