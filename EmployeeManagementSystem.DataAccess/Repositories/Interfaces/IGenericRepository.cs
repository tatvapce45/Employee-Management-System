using System.Linq.Expressions;
using EmployeeManagementSystem.DataAccess.Results;

namespace EmployeeManagementSystem.DataAccess.Repositories.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<RepositoryResult<T>> AddAsync(T entity);

        Task<List<T>> GetAsync(int pageNumber,int pageSize,string sortBy,string sortOrder,string searchTerm,Expression<Func<T, bool>>? filter = null,params Expression<Func<T, string>>[] searchProperties);

        Task<RepositoryResult<T>> GetById(int id);

        Task<RepositoryResult<T>> UpdateAsync(T entity);

        Task<RepositoryResult<T>> DeleteAsync(T entity);
    }
}