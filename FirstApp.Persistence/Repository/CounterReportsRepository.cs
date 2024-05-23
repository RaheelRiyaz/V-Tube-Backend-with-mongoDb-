
using FirstApp.Application.Abstractions.IRepositories;
using FirstApp.Domain.Models;
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
    }
}
