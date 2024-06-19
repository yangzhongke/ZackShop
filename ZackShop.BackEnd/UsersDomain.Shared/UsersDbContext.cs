using Microsoft.EntityFrameworkCore;


namespace UsersDomain.Shared
{
    public class UsersDbContext:DbContext
    {
        public DbSet<Entities.User> Users { get; set; }
        public UsersDbContext(DbContextOptions<UsersDbContext> options):base(options)
        {
        }

       protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(UsersDbContext).Assembly);
        }
    }
}
