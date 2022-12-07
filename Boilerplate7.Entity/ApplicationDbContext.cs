using Boilerplate7.Entity.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Boilerplate7.Entity
{
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
        }

        public async Task<int> SaveChangesAsync()
        {
            return await base.SaveChangesAsync();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // modelBuilder.Seed();
        }
    }
}
