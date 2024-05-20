using FirstApp.Application.Abstractions.IRepositories;
using FirstApp.Application.Abstractions.IServices;
using FirstApp.Application.APIResponse;
using FirstApp.Application.DTOS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstApp.Application.Services
{
    public class ReportContentTypesService
        (
        IReportContentTypesRepository repository
        )
        : IReportContentTypesService
    {
        public async Task<APIResponse<List<ReportContentTypeResponse>>> GetReportContentTypesAsync()
        {
            var types = await repository.GetAllAsync();

            var response = types.Select(_ => new ReportContentTypeResponse
            {
                Id = _.Id.ToString(),
                Title = _.Title,
            })
                .ToList();

            return APIResponse<List<ReportContentTypeResponse>>.SuccessResponse(response, "Report type content fetched");
        }
    }
}
