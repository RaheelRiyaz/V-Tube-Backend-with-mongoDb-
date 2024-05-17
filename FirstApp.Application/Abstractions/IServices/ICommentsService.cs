
using FirstApp.Application.APIResponse;
using FirstApp.Application.DTOS;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstApp.Application.Abstractions.IServices
{
    public interface ICommentsService
    {
        Task<APIResponse<int>> AddComment(CommentRequest model);
        Task<APIResponse<int>> ReplyComment(CommentReplyRequest model);
        Task<APIResponse<int>> UpdateComment(CommentUpdateRequest model);
        Task<APIResponse<List<CommentViewResponse>>> FetchCommentsByVideoIdAsync(CommentsFilter model);
        Task<APIResponse<List<ReplyViewResponse>>> FetchRepliesByCommentAsync(ReplyFilter model);
    }
}
