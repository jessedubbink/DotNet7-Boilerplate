using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Boilerplate7.Entity
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        private readonly IConfiguration _config;

        public ApplicationDbContextFactory(IConfiguration config)
        {
            _config = config;
        }

        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

            optionsBuilder.UseSqlServer(_config.GetConnectionString("Default"));
            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}
