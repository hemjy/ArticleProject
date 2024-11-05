﻿

using System.Linq.Expressions;

namespace ArticleProject.Application.Interfaces.Repositories
{
    public interface IGenericRepositoryAsync<T> where T : class
    {
        Task<T> GetByIdAsync<V>(V id);
        Task SaveChangesAsync();
        Task<IReadOnlyList<T>> GetAllAsync();
        Task<IReadOnlyList<T>> GetPagedResponseAsync(int pageNumber, int pageSize);
        Task<T> AddAsync(T entity, bool commit = true);
        Task<List<T>> AddAsync(List<T> entities, bool commit = true);
        Task UpdateAsync(T entity, bool commit = true);
        Task UpdateAsync(List<T> entities, bool commit = true);
        Task DeleteAsync(T entity, bool commit = true);
        Task DeleteAsync(List<T> entities, bool commit = true);
        Task<bool> IsUniqueAsync(Expression<Func<T, bool>> predicate);
        System.Linq.IQueryable<T> GetAllQuery();
        Task<T> GetByAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeExpressions);
        Task<IReadOnlyList<T>> ListAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeExpressions);
        Task<int> CountAsync();
    }
}