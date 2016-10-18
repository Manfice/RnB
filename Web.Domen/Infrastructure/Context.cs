using System.Data.Entity;
using Web.Domen.Models;

namespace Web.Domen.Infrastructure
{
    public class Context:DbContext
    {
        public Context():base("RnBData")
        {
            Database.SetInitializer(new RnBDbInitiliser());
        }

        public DbSet<Paty> Paties { get; set; }
        public DbSet<PatyCategory> PatyCategories { get; set; }
        public DbSet<PatyImage> PatyImages { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<CustImage> CustImages { get; set; }
    }

    public class RnBDbInitiliser : CreateDatabaseIfNotExists<Context>
    {
    }
}