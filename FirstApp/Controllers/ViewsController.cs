using FirstApp.Application.Abstractions.IServices;
using FirstApp.Application.APIResponse;
using FirstApp.Application.DTOS;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FirstApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ViewsController(IViewsService service) : ControllerBase
    {
        [HttpPost]
        public async Task<APIResponse<int>> AddView(ViewRequest model) =>
            await service.AddView(model);


        [HttpPost("fetch-views")]
        public async Task<APIResponse<List<ViewAPIResponse>>> FetchVideoViews(CommentsFilter model) =>
            await service.FetchVideoViews(model);
    }
}
