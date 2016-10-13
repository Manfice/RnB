using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Web.Infrastructure;
using Web.Models;

namespace Web.Migrations
{


    internal sealed class Configuration : DbMigrationsConfiguration<AppIdentityDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            //ContextKey = "Web.Infrastructure.AppIdentityDbContext";
            AutomaticMigrationDataLossAllowed = false;
        }

        protected override void Seed(AppIdentityDbContext context)
        {
            PerformInitialSetup(context);
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
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
            if (user == null)
            {
                userManager.Create(new AppUser { Email = email, UserName = email, Fio = userName }, passs);
            }
            user = userManager.FindByName(email);
            if (!userManager.IsInRole(user.Id, role))
            {
                userManager.AddToRole(user.Id, role);
            }

        }

    }
}
