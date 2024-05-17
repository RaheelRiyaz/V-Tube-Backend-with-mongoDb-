using FirstApp.Application.Abstractions.IServices;
using FirstApp.Application.APIResponse;
using FirstApp.Application.DTOS;
using FirstApp.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FirstApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController
        (
        IUsersService service,
        IWatchHistoryService watchHistoryService
        ) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<APIResponse<int>> Register(UserRequest model) =>
            await service.Register(model);


        [HttpPost("login")]
        public async Task<APIResponse<LoginResponse>> Login(LoginRequest model) =>
            await service.Login(model);


        [HttpPut("change-password")]
        public async Task<APIResponse<int>> ChangePassword(ChangePasswordRequest model) =>
            await service.ChangePassword(model);


        [HttpPost("fetch-history")]
        public async Task<APIResponse<List<HistoryViewResponse>>> FetchUserHistory(FilterModel model) =>
            await watchHistoryService.FetchUserHistory(model);


        [HttpPost("delete-history")]
        public async Task<APIResponse<int>> RemoveVideoFromHistory(List<string> videoIds) =>
            await watchHistoryService.RemoveVideoFromHistory(videoIds);


        [HttpDelete("clear-history")]
        public async Task<APIResponse<int>> ClearWatchHistory() =>
            await watchHistoryService.ClearWatchHistory();


        [HttpGet("refresh-token/{refreshToken:alpha}")]
        public async Task<APIResponse<LoginResponse>> RefreshToken(string refreshToken) =>
            await service.RefreshToken(refreshToken);
    }
}
