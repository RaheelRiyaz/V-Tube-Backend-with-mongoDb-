using FirstApp.Application.Abstractions.IRepositories;
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
    public class ReportsRepository : BaseRepository<Report>, IReportsRepository
    {
        public ReportsRepository(IMongoDatabase database) : base(database)
        {
        }

        public async Task<int> CheckNoOfReportsForTheEntity(string entityId)
        {
            var filter = Builders<Report>.Filter.Eq("EntityId", new ObjectId(entityId));

            var count = await _collection.CountDocumentsAsync(filter);

            return (int)count;
        }
    }
}
