
using FirstApp.Application.Abstractions.IRepositories;
using FirstApp.Application.DTOS;
using FirstApp.Domain.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstApp.Persistence.Repository
{
    public class CounterReportsRepository : BaseRepository<CounterReport>, ICounterReportsRepository
    {
        public CounterReportsRepository(IMongoDatabase database) : base(database)
        {
        }

        public async Task<List<CounterReport>> FetchCounterReports(FilterModel model)
        {
            var filter = Builders<CounterReport>.Filter.Empty; 

            var sort = model.SortOrder == 1 ? Builders<CounterReport>.Sort.Ascending("CreatedAt"):
                Builders<CounterReport>.Sort.Ascending("CreatedAt"); 

            var result = await _collection
                .Find(filter)  
                .Sort(sort)  
                .Skip((model.PageNo - 1) * model.PageSize)  
                .Limit(model.PageSize)
                .ToListAsync();

            return result;
        }
    }
}
