namespace Boilerplate7.Entity.Interfaces
{
    public interface IApplicationDbContext
    {
        public Task<int> SaveChangesAsync();
    }
}
