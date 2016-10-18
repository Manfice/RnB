using Microsoft.AspNet.Identity.EntityFramework;
using Web.Models;
using System.Data.Entity;
using Microsoft.AspNet.Identity;

namespace Web.Infrastructure
{
    public class AppIdentityDbContext:IdentityDbContext<AppUser>
    {
        public AppIdentityDbContext():base("RnBIdentity"){ }

        static AppIdentityDbContext()
        {
            Database.SetInitializer(new IdentityDbInit());
        }

        public static AppIdentityDbContext Create()
        {
            return new AppIdentityDbContext();
        }
    }

    public class IdentityDbInit : NullDatabaseInitializer<AppIdentityDbContext>
    {

    }
}