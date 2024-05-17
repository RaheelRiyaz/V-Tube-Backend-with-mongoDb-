using FirstApp.Application.Abstractions.IServices;
using FirstApp.Application.APIResponse;
using FirstApp.Application.DTOS;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FirstApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LikesController (ILikesService service): ControllerBase
    {
        [HttpPost]
        public async Task<APIResponse<int>> AddLike(LikeRequest model) =>
            await service.AddLike(model);
    }
}
