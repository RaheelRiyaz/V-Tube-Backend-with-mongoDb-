using FirstApp.Application.Abstractions.IRepositories;
using FirstApp.Application.Utilis;
using FirstApp.Domain.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FirstApp.Persistence.Repository
{
    public class BaseRepository<T> : IBaseRepository<T> where T : BaseEntity
    {
        protected readonly IMongoCollection<T> _collection;

        public BaseRepository(IMongoDatabase database)
        {
            _collection = database.GetCollection<T>(AppCollections.CollectionNames[typeof(T)]); 
        }

        public async Task<int> DeleteAsync(ObjectId id)
        {
            var result = await _collection.DeleteOneAsync(Builders<T>.Filter.Eq("_id", id));
            return (int)result.DeletedCount;
        }

        public async Task<int> DeleteAsync(T model)
        {
            var result = await _collection.DeleteOneAsync(Builders<T>.Filter.Eq("_id", model.Id));
            return (int)result.DeletedCount;
        }

        public async Task<int> DeleteRangeAsync(IEnumerable<ObjectId> ids)
        {
            var result = await _collection.DeleteManyAsync(Builders<T>.Filter.In("_id", ids));
            return (int)result.DeletedCount;
        }

        public async Task<int> DeleteRangeAsync(IEnumerable<T> models)
        {
            var ids = models.Select(m => m.Id);
            var result = await _collection.DeleteManyAsync(Builders<T>.Filter.In("_id", ids));
            return (int)result.DeletedCount;
        }

        public async Task<bool> ExistsAsync(Expression<Func<T, bool>> expression)
        {
            return await _collection.Find(expression).AnyAsync();
        }
        public async Task<T?> FindOneAsync(ObjectId id)
        {
            return await _collection.Find(Builders<T>.Filter.Eq("_id", id)).FirstOrDefaultAsync();
        }

        public async Task<T?> FindOneAsync(Expression<Func<T, bool>> expression)
        {
            return await _collection.Find(expression).FirstOrDefaultAsync();
        }

        public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> expression)
        {
            return await _collection.Find(expression).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _collection.Find(_=>true).ToListAsync();
        }

        public async Task<int> InsertAsync(T model)
        {
            await _collection.InsertOneAsync(model);
            return 1; 
        }

        public async Task<int> InsertRangeAsync(List<T> models)
        {
            var options = new InsertManyOptions { IsOrdered = false };
            await _collection.InsertManyAsync(models, options);
            return models.Count;
        }

        public async Task<int> UpdateAsync(T model)
        {
            var result = await _collection.ReplaceOneAsync(Builders<T>.Filter.Eq("_id", model.Id), model);
            return (int)result.ModifiedCount;
        }

        public async Task<IQueryable<T>> FilterAsync(Expression<Func<T, bool>> expression)
        {
            var cursor = await _collection.FindAsync(expression);
            return cursor.ToList().AsQueryable();
        }
    }
}
