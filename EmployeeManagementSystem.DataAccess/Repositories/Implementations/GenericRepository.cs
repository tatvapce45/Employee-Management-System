using System.Linq.Expressions;
using EmployeeManagementSystem.DataAccess.Models;
using EmployeeManagementSystem.DataAccess.Repositories.Interfaces;
using EmployeeManagementSystem.DataAccess.Results;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementSystem.DataAccess.Repositories.Implementations
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly EmployeeManagementSystemContext _context;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(EmployeeManagementSystemContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task<RepositoryResult<T>> AddAsync(T entity)
        {
            var result = new RepositoryResult<T>();
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await _dbSet.AddAsync(entity);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                result.Success = true;
                result.Data = entity;
            }
            catch (DbUpdateException ex)
            {
                await transaction.RollbackAsync();
                result.Success = false;
                result.ErrorMessage = $"Database adding error: {ex.Message}";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                result.Success = false;
                result.ErrorMessage = $"Unexpected error: {ex.Message}";
            }

            return result;
        }

        public async Task<List<T>> GetAsync(int pageNumber, int pageSize, string sortBy, string sortOrder, string searchTerm, Expression<Func<T, bool>>? filter = null, params Expression<Func<T, string>>[] searchProperties)
        {
            IQueryable<T> query = filter != null ? _dbSet.Where(filter) : _dbSet.AsQueryable();
            if (!string.IsNullOrEmpty(searchTerm) && searchProperties != null && searchProperties.Length > 0)
            {
                var lowerSearchTerm = searchTerm.ToLower();
                Expression? combinedExpression = null;
                var parameter = Expression.Parameter(typeof(T), "x");

                foreach (var searchProperty in searchProperties)
                {
                    var invokedExpr = Expression.Invoke(searchProperty, parameter);

                    var toLowerMethod = typeof(string).GetMethod(nameof(string.ToLower), Type.EmptyTypes)!;
                    var containsMethod = typeof(string).GetMethod(nameof(string.Contains), [typeof(string)])!;

                    var toLowerCall = Expression.Call(invokedExpr, toLowerMethod);
                    var searchTermConstant = Expression.Constant(lowerSearchTerm);
                    var containsCall = Expression.Call(toLowerCall, containsMethod, searchTermConstant);

                    combinedExpression = combinedExpression == null ? containsCall : Expression.OrElse(combinedExpression, containsCall);
                }

                if (combinedExpression != null)
                {
                    var lambda = Expression.Lambda<Func<T, bool>>(combinedExpression, parameter);
                    query = query.Where(lambda);
                }
            }

            if (!string.IsNullOrEmpty(sortBy))
            {
                var parameter = Expression.Parameter(typeof(T), "x");
                Expression property;

                try
                {
                    property = Expression.PropertyOrField(parameter, sortBy);
                }
                catch (ArgumentException)
                {
                    property = GenericRepository<T>.GetDefaultSortProperty(parameter);
                }

                var lambda = Expression.Lambda(property, parameter);

                string methodName = sortOrder?.ToLower() == "desc" ? "OrderByDescending" : "OrderBy";

                var orderByMethod = typeof(Queryable).GetMethods().Where(m => m.Name == methodName && m.GetParameters().Length == 2).Single().MakeGenericMethod(typeof(T), property.Type);
                query = (IQueryable<T>)orderByMethod.Invoke(null, [query, lambda])!;
            }
            else
            {
                var parameter = Expression.Parameter(typeof(T), "x");
                var property = GenericRepository<T>.GetDefaultSortProperty(parameter);
                if (property != null)
                {
                    var lambda = Expression.Lambda(property, parameter);

                    var orderByMethod = typeof(Queryable).GetMethods().Where(m => m.Name == "OrderBy" && m.GetParameters().Length == 2)
                                        .Single().MakeGenericMethod(typeof(T), property.Type);

                    query = (IQueryable<T>)orderByMethod.Invoke(null, [query, lambda])!;
                }
            }

            query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
            return await query.ToListAsync();
        }

        private static MemberExpression GetDefaultSortProperty(ParameterExpression parameter)
        {
            var idProp = typeof(T).GetProperty("Id");
            if (idProp != null)
            {
                return Expression.Property(parameter, idProp);
            }

            var stringProp = typeof(T).GetProperties().FirstOrDefault(p => p.PropertyType == typeof(string));
            if (stringProp != null)
                return Expression.Property(parameter, stringProp);

            throw new InvalidOperationException($"No default sort property found for type {typeof(T).Name}");
        }

        public async Task<RepositoryResult<T>> GetById(int id)
        {
            var result = new RepositoryResult<T>();
            try
            {
                var entity = await _dbSet.FindAsync(id);
                if (entity != null)
                {
                    result.Data = entity;
                    result.Success = true;
                }
                else
                {
                    result.Success = true;
                    result.ErrorMessage = "entity not found";
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = $"Unexpected error: {ex.Message}";
            }
            return result;
        }

        public async Task<RepositoryResult<T>> UpdateAsync(T entity)
        {
            var result = new RepositoryResult<T>();
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _dbSet.Update(entity);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                result.Success = true;
                result.Data = entity;
            }
            catch (DbUpdateException ex)
            {
                await transaction.RollbackAsync();
                result.Success = false;
                result.ErrorMessage = $"Database update error: {ex.Message}";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                result.Success = false;
                result.ErrorMessage = $"Unexpected error: {ex.Message}";
            }
            return result;
        }

        public async Task<RepositoryResult<T>> DeleteAsync(T entity)
        {
            var result = new RepositoryResult<T>();
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                result.Success = true;
                result.Data = entity;
            }
            catch (DbUpdateException ex)
            {
                await transaction.RollbackAsync();
                result.Success = false;
                result.ErrorMessage = $"Database update error: {ex.Message}";
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                result.Success = false;
                result.ErrorMessage = $"Unexpected error: {ex.Message}";
            }
            return result;
        }
    }
}