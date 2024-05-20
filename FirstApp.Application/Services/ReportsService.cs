
using FirstApp.Application.Abstractions.IRepositories;
using FirstApp.Application.Abstractions.IServices;
using FirstApp.Application.APIResponse;
using FirstApp.Application.DTOS;
using FirstApp.Domain.Models;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstApp.Application.Services
{
    public class ReportsService
        (
        IContextService contextService,
        IReportsRepository reportsRepository
        )
        : IReportsService
    {
        public async Task<APIResponse<int>> AddReport(ReportRequest model)
        {
            var userId = contextService.GetUserId();

            if (userId == ObjectId.Empty)
                return APIResponse<int>.ErrorResponse("Unauthorized");

            var hasAlreadyReported = await reportsRepository.ExistsAsync
                (_ => _.EntityId == new ObjectId(model.EntityId));

            if (hasAlreadyReported)
                return APIResponse<int>.ErrorResponse($"You have already reported");

            var newReport = new Report
            {
                EntityId = new ObjectId(model.EntityId),
                ReportContentType = new ObjectId(model.ReportContentType),
                ReportedBy = userId,
                ReportedOn = model.ReportedOn
            };

            var insertedReportResponse = await reportsRepository.InsertAsync(newReport);
            return default;
        }
    }
}
