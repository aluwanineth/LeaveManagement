namespace LeaveManagement.Application.Interfaces.Repositories;

public interface IGenericRepositoryAsync<T> where T : class
{
    Task<T> GetByIdAsync(params object[] keyValues);
    Task<IReadOnlyList<T>> GetAllAsync();
    Task<IReadOnlyList<T>> GetPagedReponseAsync(int pageNumber, int pageSize);
    Task<T> AddAsync(T entity);
    Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(params object[] keyValues);
    Task DeleteRangeAsync(IEnumerable<T> entities);
}
