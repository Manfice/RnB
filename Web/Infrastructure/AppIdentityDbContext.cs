using Microsoft.AspNet.Identity.EntityFramework;
using Web.Models;
using System.Data.Entity;
using Microsoft.AspNet.Identity;

namespace Web.Infrastructure
{
    public class AppIdentityDbContext:IdentityDbContext<AppUser>
    {
        public AppIdentityDbContext():base("RnB"){ }

        static AppIdentityDbContext()
        {
            Database.SetInitializer(new IdentityDbInit());
        }

        public static AppIdentityDbContext Create()
        {
            return new AppIdentityDbContext();
        }
    }

    public class IdentityDbInit : CreateDatabaseIfNotExists<AppIdentityDbContext>
    {
        protected override void Seed(AppIdentityDbContext context)
        {
            PerformInitialSetup(context);
            base.Seed(context);
        }

        public void PerformInitialSetup(AppIdentityDbContext context)
        {
            var userManager = new AppUserManager(new UserStore<AppUser>(context));
            var roleManager = new AppRoleManager(new RoleStore<AppRole>(context));

            var role = "Admin";
            var userName = "Administrator";
            var passs = "1q2w3eOP";
            var email = "c592@yandex.ru";

            if (!roleManager.RoleExists(role))
            {
                roleManager.Create(new AppRole(role));
            }

            var user = userManager.FindByName(email);
            if (user==null)
            {
                userManager.Create(new AppUser {Email = email, UserName = email, Fio = userName}, passs);
            }
            user = userManager.FindByName(email);
            if (!userManager.IsInRole(user.Id, role))
            {
                userManager.AddToRole(user.Id, role);
            }

        }
    }
}