using Microsoft.AspNet.Identity.EntityFramework;
using Web.Models;
using System.Data.Entity;

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
            
        }
    }
}