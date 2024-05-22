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
    public class ChannelsController(IChannelsService service) : ControllerBase
    {

        [HttpPost("create")]
        public async Task<APIResponse<int>> CreateChannel([FromForm] ChannelRequest model) =>
          await service.CreateChannel(model);


        [HttpGet("subscription/{channelId}")]
        public async Task<APIResponse<int>> Subscription(string channelId) => 
            await service.Subscription(channelId);


        [HttpGet("toggle-notify/{channelId}")]
        public async Task<APIResponse<int>> ToggleNotify(string channelId) =>
            await service.ToggleNotify(channelId);


        [HttpGet("{channelId}")]
        public async Task<APIResponse<UserChannelResponse>> ViewChannel(string channelId) =>
            await service.ViewChannel(channelId);


        [HttpPut]
        public async Task<APIResponse<int>> UpdateChannel(UpdateChannelRequest model) =>
            await service.UpdateChannel(model);


        [HttpPost("subscribed-channels")]
        public async Task<APIResponse<List<UserSubscribedViewResponse>>> ViewUserSubscribedChannels(FilterModel model) =>
            await service.ViewUserSubscribedChannels(model);



        [HttpGet("download")]
        public async Task<FileContentResult> Download(string fileName)
        {
            var path = "D:\\localRepository\\V-Tube-Backend-with-mongoDb-\\FirstApp\\wwwroot\\Files";
            var filePath = Path.Combine(path, "quranproject.png");

            var bytes = await System.IO.File.ReadAllBytesAsync(filePath);

            return File(bytes, "image/jpeg", fileName);
        }
    }
}
