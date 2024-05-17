using FirstApp.Application.Abstractions.IServices;
using FirstApp.Application.APIResponse;
using FirstApp.Application.DTOS;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FirstApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController(ICommentsService service) : ControllerBase
    {
        [HttpPost]
        public async Task<APIResponse<int>> AddComment(CommentRequest model) =>
            await service.AddComment(model);


        [HttpPost("reply")]
        public async Task<APIResponse<int>> ReplyComment(CommentReplyRequest model) =>
            await service.ReplyComment(model);


        [HttpPost("fetch-comments")]
        public async Task<APIResponse<List<CommentViewResponse>>> FetchCommentsByVideoIdAsync(CommentsFilter model) =>
            await service.FetchCommentsByVideoIdAsync(model);


        [HttpPost("fetch-replies")]
        public async Task<APIResponse<List<ReplyViewResponse>>> FetchRepliesByCommentAsync(ReplyFilter model) =>
            await service.FetchRepliesByCommentAsync(model);


        [HttpPut]
        public async Task<APIResponse<int>> UpdateComment(CommentUpdateRequest model) =>
            await service.UpdateComment(model);
    }
}
