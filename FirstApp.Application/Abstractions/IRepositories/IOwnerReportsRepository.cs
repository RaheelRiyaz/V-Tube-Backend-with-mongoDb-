using FirstApp.Application.DTOS;
using FirstApp.Domain.Models;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstApp.Application.Abstractions.IRepositories
{
    public interface IOwnerReportsRepository : IBaseRepository<OwnerReports>
    {
        Task<List<OwnerReportDBResponse>> GetOwnerReportsAsync(ObjectId ownerId);

    }
}
