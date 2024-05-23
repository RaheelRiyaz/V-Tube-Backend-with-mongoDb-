using FirstApp.Application.Abstractions.IServices;
using FirstApp.Application.APIResponse;
using FirstApp.Application.DTOS;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FirstApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController
        (
        IReportsService reportsService,
        IReportContentTypesService reportContentTypesService
        ) : ControllerBase
    {

        [HttpPost("submit")]
        public async Task<APIResponse<int>> AddReport(ReportRequest model) =>
            await reportsService.AddReport(model);


        [HttpGet("report-type-contents")]
        public async Task<APIResponse<List<ReportContentTypeResponse>>> GetReportContentTypesAsync() =>
            await reportContentTypesService.GetReportContentTypesAsync();


        [HttpPost("counter-report")]
        public async Task<APIResponse<int>> CounterReport(CounterReportRequest model) =>
            await reportsService.CounterReport(model);


        [HttpGet("owner-reports")]
        public async Task<APIResponse<List<OwnerReportResponse>>> GetOwnerReportsAsync() =>
            await reportsService.GetOwnerReportsAsync();
    }
}
