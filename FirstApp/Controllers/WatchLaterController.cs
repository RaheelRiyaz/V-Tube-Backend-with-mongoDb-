using FirstApp.Application.Abstractions.IServices;
using FirstApp.Application.APIResponse;
using FirstApp.Application.DTOS;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace FirstApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WatchLaterController(IWatchLaterService service) : ControllerBase
    {
        [HttpGet("add/{videoId}")]
        public async Task<APIResponse<int>> AddVideoToWatchLater(string videoId) =>
            await service.AddVideoToWatchLater(videoId);


        [HttpPost("fetch")]
        public async Task<APIResponse<List<WatchLaterViewResponse>>> FetchWatchLaterVideos(WatchLaterFilterModel model) =>
            await service.FetchWatchLaterVideos(model);


        [HttpGet("header/{sortOrder:int}")]
        public async Task<APIResponse<WatchLaterHeader>> FetchWatchlaterHeader(int sortOrder) =>
            await service.FetchWatchlaterHeader(sortOrder);


        [HttpDelete("{id}")]
        public async Task<APIResponse<int>> RemoveVideoFromWatchLater(string id) =>
            await service.RemoveVideoFromWatchLater(id);
    }
}
