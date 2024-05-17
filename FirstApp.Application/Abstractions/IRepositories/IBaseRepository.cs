using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FirstApp.Application.Abstractions.IRepositories
{
    public interface IBaseRepository<T>
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> FindOneAsync(ObjectId id);
        Task<T?> FindOneAsync(Expression<Func<T, bool>> expression);
        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> expression);
        Task<IQueryable<T>> FilterAsync(Expression<Func<T, bool>> expression);
        Task<bool> ExistsAsync(Expression<Func<T, bool>> expression);
        Task<int> InsertAsync(T model);
        Task<int> InsertRangeAsync(List<T> models);
        Task<int> UpdateAsync(T model);
        Task<int> DeleteAsync(ObjectId id);
        Task<int> DeleteRangeAsync(IEnumerable<ObjectId> ids);
        Task<int> DeleteRangeAsync(IEnumerable<T> models);
        Task<int> DeleteAsync(T model);
    }
}
