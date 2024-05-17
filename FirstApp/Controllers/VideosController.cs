using FirstApp.Application.Abstractions.IServices;
using FirstApp.Application.APIResponse;
using FirstApp.Application.DTOS;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FirstApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VideosController (IVideosService service): ControllerBase
    {
        [HttpPost]
        public async Task<APIResponse<int>> UploadVideo(VideoRequest model) =>
            await service.UploadVideo(model);


        [HttpGet("search/{searchTerm}")]
        public async Task<APIResponse<List<string>>> GetVideoSearchSuggestions(string searchTerm) =>
            await service.GetVideoSearchSuggestions(searchTerm);


        [HttpPost("fetch-videos")]
        public async Task<APIResponse<List<VideoViewModel>>> FetchVideosByChannel(VideoFilter model) =>
            await service.FetchVideosByChannel(model);


        [HttpPut]
        public async Task<APIResponse<int>> UpdateVideo(UpdateVideoRequest model) =>
            await service.UpdateVideo(model);


        [HttpDelete("{id}")]
        public async Task<APIResponse<int>> DeleteVideo(string id) =>
            await service.DeleteVideo(id);
    }
}
