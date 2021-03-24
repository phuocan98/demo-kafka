using Microsoft.EntityFrameworkCore;

namespace Project.App.Databases
{
    public class MariaDBContext : DbContext
    {
        public MariaDBContext(DbContextOptions<MariaDBContext> options) : base(options)
        {

        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<User>().HasQueryFilter(p => p.Enable);
        }
    }
}
