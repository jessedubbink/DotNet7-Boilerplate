using Microsoft.EntityFrameworkCore;

namespace Boilerplate7.Entity.Interfaces
{
    internal interface IApplicationDbContext
    {
        public Task<int> SaveChangesAsync();
    }
}
