using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Web.Infrastructure;
using Web.Models;

namespace Web.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Web.Infrastructure.AppIdentityDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            ContextKey = "Web.Infrastructure.AppIdentityDbContext";
        }

        protected override void Seed(Web.Infrastructure.AppIdentityDbContext context)
        {
            var userManager = new AppUserManager(new UserStore<AppUser>(context));
            var roleManager = new AppRoleManager(new RoleStore<AppRole>(context));

            const string role = "Admin";
            const string userName = "Administrator";
            const string passs = "1q2w3eOP";
            const string email = "c592@yandex.ru";

            if (!roleManager.RoleExists(role))
            {
                roleManager.Create(new AppRole(role));
            }

            var user = userManager.FindByName(email);
            if (user == null)
            {
                userManager.Create(new AppUser { Email = email, UserName = userName}, passs);
            }
            user = userManager.FindByEmail(email);
            if (!userManager.IsInRole(user.Id, role))
            {
                userManager.AddToRole(user.Id, role);
            }
            context.SaveChanges();
        }

    }
}
