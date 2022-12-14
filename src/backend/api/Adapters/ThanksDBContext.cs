using api.Entities;
using Microsoft.EntityFrameworkCore;

namespace api.Adapters
{
    public class ThanksDBContext : DbContext
    {
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Activity> Activities { get; set; }

        public ThanksDBContext()
        {
        }

        public ThanksDBContext(DbContextOptions<ThanksDBContext> dbContextOptions)
            :base(dbContextOptions)
        {

        }

#if DEBUG
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    => optionsBuilder.LogTo(Console.WriteLine).EnableSensitiveDataLogging(true);
#endif
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrganizationMembership>()
           .HasKey(t => new { t.OrganizationId, t.UserId });
        }
    }
}
