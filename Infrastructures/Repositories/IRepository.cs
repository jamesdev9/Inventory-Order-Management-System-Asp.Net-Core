namespace Indotalent.Infrastructures.Repositories
{
    public interface IRepository<T> where T : class
    {
        IQueryable<T> GetAllArchive();
        IQueryable<T> GetAll();
        Task<T?> GetByIdAsync(int? id);
        Task<T?> GetByRowGuidAsync(Guid? rowGuid);
        Task AddAsync(T? entity);
        Task UpdateAsync(T? entity);
        Task DeleteByIdAsync(int? id);
        Task DeleteByRowGuidAsync(Guid? rowGuid);
    }
}
