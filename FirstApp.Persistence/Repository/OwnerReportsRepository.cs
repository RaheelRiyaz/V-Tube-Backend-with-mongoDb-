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
    public class OwnerReportsRepository : BaseRepository<OwnerReports>, IOwnerReportsRepository
    {
        public OwnerReportsRepository(IMongoDatabase database) : base(database)
        {
        }
    }
}
