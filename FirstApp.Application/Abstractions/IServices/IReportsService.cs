using FirstApp.Application.APIResponse;
using FirstApp.Application.DTOS;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstApp.Application.Abstractions.IServices
{
    public interface IReportsService
    {
        Task<APIResponse<int>> AddReport(ReportRequest model);
        Task<APIResponse<int>> CounterReport(CounterReportRequest model);
        Task<APIResponse<List<OwnerReportResponse>>> GetOwnerReportsAsync();
        Task<APIResponse<List<CounterReportResponse>>> FetchCounterReports(FilterModel model);
    }
}
