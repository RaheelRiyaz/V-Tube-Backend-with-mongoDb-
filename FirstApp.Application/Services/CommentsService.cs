
using FirstApp.Application.Abstractions.IRepositories;
using FirstApp.Application.Abstractions.IServices;
using FirstApp.Application.APIResponse;
using FirstApp.Application.DTOS;
using FirstApp.Domain.Enums;
using FirstApp.Domain.Models;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstApp.Application.Services
{
    public class CommentsService
        (
        ICommentsRepository repository,
        IVideousRepository videousRepository,
        ICommentRepliesRepository commentRepliesRepository,
        IContextService contextService,
        ICommentAbuserepository commentAbuserepository
        )
        : ICommentsService
    {
        public async Task<APIResponse<int>> AddComment(CommentRequest model)
        {
            //var userId = contextService.GetUserId();
            var userId = ObjectId.Parse("6639bb42c92b6748cb77c6fa");

            if (userId == ObjectId.Empty)
                return APIResponse<int>.ErrorResponse("Not authorized");

            var isAbusiveComment = await commentAbuserepository.IsAbusiveComment(model.Title);

            if (isAbusiveComment)
                return APIResponse<int>.ErrorResponse("Your comment contains some abusive words");

            var videoExists = await videousRepository.ExistsAsync
                (_ => _.Id == ObjectId.Parse(model.VideoId));

            if (!videoExists)
                return APIResponse<int>.ErrorResponse("No such video exists");

            var comment = new Comment
            {
                VideoId = ObjectId.Parse(model.VideoId),
                CommentedBy = userId,
                Title = model.Title,
            };

            var addResponse = await repository.InsertAsync(comment);

            return addResponse > 0 ? APIResponse<int>.SuccessResponse(addResponse, "Comment added") :
                APIResponse<int>.ErrorResponse();
        }

        public async Task<APIResponse<List<CommentViewResponse>>> FetchCommentsByVideoIdAsync(CommentsFilter model)
        {
            //var userId = contextService.GetUserId();
            var userId = ObjectId.Parse("6639bb42c92b6748cb77c6fa");

            var comments = await repository.FetchCommentsByVideoIdAsync(model, userId);

            var response = comments.Select(_ => new CommentViewResponse
            {
                CommentedBy = _.CommentedBy.ToString(),
                Id = _.Id.ToString(),
                Commenter = _.Commenter,
                CreatedAt = _.CreatedAt,
                HasUserLiked = _.HasUserLiked,
                Title = _.Title,
                Dislikes = _.Dislikes,
                Likes = _.Likes,
                TotalReplies = _.TotalReplies
            }).
            OrderByDescending(_ => _.CreatedAt)
            .ToList();

            return APIResponse<List<CommentViewResponse>>.SuccessResponse(response, "Comments fetched");
        }

        public async Task<APIResponse<List<ReplyViewResponse>>> FetchRepliesByCommentAsync(ReplyFilter model)
        {
            //var userId = contextService.GetUserId();
            var userId = ObjectId.Parse("6639bb42c92b6748cb77c6fa");

            var replies = await commentRepliesRepository.FetchRepliesByCommentAsync(model, userId);

            var response = replies.Select(_ => new ReplyViewResponse
            {
                CommentId = _.CommentId.ToString(),
                Id = _.Id.ToString(),
                RepliedBy = _.RepliedBy.ToString(),
                CreatedAt = _.CreatedAt,
                DisLikes = _.DisLikes,
                HasUserLiked = _.HasUserLiked,
                Likes = _.Likes,
                Replier = _.Replier,
                Title = _.Title
            }).ToList();

            return APIResponse<List<ReplyViewResponse>>.SuccessResponse(response, "Replies fetched successfully");
        }

        public async Task<APIResponse<int>> ReplyComment(CommentReplyRequest model)
        {
            //var userId = contextService.GetUserId();
            var userId = ObjectId.Parse("6639bb42c92b6748cb77c6fa");

            if (userId == ObjectId.Empty)
                return APIResponse<int>.ErrorResponse("Not authorized");

            var commentId = ObjectId.Parse(model.CommentId);

            var commentExist = await repository.ExistsAsync(_ => _.Id == commentId);

            if (!commentExist)
                return APIResponse<int>.ErrorResponse("Such comment doesn't exixts");

            var reply = new CommentReplies
            {
                CommentId = commentId,
                RepliedBy = userId,
                Title = model.Title
            };

            var addedReplyResponse = await commentRepliesRepository.InsertAsync(reply);

            return addedReplyResponse > 0 ? APIResponse<int>.SuccessResponse(addedReplyResponse, "You have replied to this comment") :
                APIResponse<int>.ErrorResponse();
        }

        public async Task<APIResponse<int>> UpdateComment(CommentUpdateRequest model)
        {
            //var userId = contextService.GetUserId();
            var userId = ObjectId.Parse("6639bb42c92b6748cb77c6fa");

            switch (model.CommentType)
            {
                case CommentType.Comment:
                    var comment = await repository.FindOneAsync(new ObjectId(model.Id));

                    if (comment is null)
                        return APIResponse<int>.ErrorResponse("Invalid comment id");

                    if (comment.CommentedBy != userId)
                        return APIResponse<int>.ErrorResponse("You are not allowed to update this comment");

                    comment.Title = model.Title;
                    comment.UpdatedAt = DateTime.Now;

                    var updatedResult = await repository.UpdateAsync(comment);

                    return updatedResult > 0 ? APIResponse<int>.SuccessResponse(updatedResult, "Comment has been updated") :
                        APIResponse<int>.ErrorResponse();

                case CommentType.Reply:
                    var reply = await commentRepliesRepository.FindOneAsync(new ObjectId(model.Id));

                    if (reply is null)
                        return APIResponse<int>.ErrorResponse("Invalid comment id");

                    if (reply.RepliedBy != userId)
                        return APIResponse<int>.ErrorResponse("You are not allowed to update this comment");

                    reply.Title = model.Title;
                    reply.UpdatedAt = DateTime.Now;

                    var updatedReplyResult = await commentRepliesRepository.UpdateAsync(reply);

                    return updatedReplyResult > 0 ? APIResponse<int>.SuccessResponse(updatedReplyResult, "Comment reply has been updated") :
                        APIResponse<int>.ErrorResponse();

                default:
                    return APIResponse<int>.ErrorResponse();
            }
        }
    }
}
