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
    public class PlayListsController(IPlayListsService service) : ControllerBase
    {
        [HttpPost]
        public async Task<APIResponse<int>> CreatePlayList(PlayListsRequest model) =>
            await service.CreatePlayList(model);

        [HttpGet("{channelId}")]
        public async Task<APIResponse<List<PlayListViewResponse>>> ViewPlayLists([FromRoute] string channelId) =>
            await service.ViewPlayLists(ObjectId.Parse(channelId));
    }
}
