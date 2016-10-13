using System.Data.Entity;

namespace Web.Domen.Infrastructure
{
    public class Context:DbContext
    {
        public Context():base("RnB")
        {
            Database.SetInitializer(new RnBDbInitiliser());
        }
    }

    public class RnBDbInitiliser : CreateDatabaseIfNotExists<Context>
    {
        protected override void Seed(Context context)
        {
            base.Seed(context);
        }
    }
}