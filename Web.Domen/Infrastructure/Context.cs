using System.Data.Entity;

namespace Web.Domen.Infrastructure
{
    public class Context:DbContext
    {
        public Context():base("RnB")
        {
            Database.SetInitializer(new CreateDatabaseIfNotExists<Context>());
        }
    }
}