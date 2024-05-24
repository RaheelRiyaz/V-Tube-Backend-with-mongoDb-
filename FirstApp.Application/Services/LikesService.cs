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
    public class LikesService
        (
        ILikesRepository repository,
        IContextService contextService
        )
        : ILikesService
    {
        public async Task<APIResponse<int>> AddLike(LikeRequest model)
        {
            var userId = ObjectId.Parse("6639bb42c92b6748cb77c6fa");
            //var userId = contextService.GetUserId();

            if (userId == ObjectId.Empty)
                return APIResponse<int>.ErrorResponse("Not authorized");


            switch (model.LikeType)
            {
                case LikeType.Comment:
                    if (model.CommentId is null)
                        return APIResponse<int>.ErrorResponse("Comment id is required in this case");

                    var commentId = ObjectId.Parse(model.CommentId);

                    var Like = await repository.FirstOrDefaultAsync
                        (_ => _.CommentId == commentId && _.LikeType == LikeType.Comment && _.UserId == userId);

                    if(Like is not null)
                    {
                        if (Like.IsLiked == model.IsLiked)
                            return APIResponse<int>.ErrorResponse("Already responded");

                       if(model.IsLiked is null)
                        {
                            var deletedLike = await repository.DeleteAsync(Like.Id);
                            return deletedLike > 0 ? APIResponse<int>.SuccessResponse(deletedLike, "Comment response removed"):
                                APIResponse<int>.ErrorResponse();
                        }

                        Like.IsLiked = Convert.ToBoolean(model.IsLiked);
                        Like.UpdatedAt = DateTime.Now;

                        var updatedLikeResponse = await repository.UpdateAsync(Like);

                        return updatedLikeResponse > 0 ? APIResponse<int>.SuccessResponse(updatedLikeResponse, "Comment response updated") :
                            APIResponse<int>.ErrorResponse();
                    }

                    var newLike = new Likes
                    {
                        CommentId = ObjectId.Parse(model.CommentId),
                        IsLiked = Convert.ToBoolean(model.IsLiked),
                        LikeType = LikeType.Comment,
                        UserId = userId
                    };

                    var addedLikeResponse = await repository.InsertAsync(newLike);

                    return addedLikeResponse > 0 ? APIResponse<int>.SuccessResponse(addedLikeResponse, "Comment response added") :
                        APIResponse<int>.ErrorResponse();

                case LikeType.Video:

                    if (model.VideoId is null)
                        return APIResponse<int>.ErrorResponse("Video id is required in this case");
                    var videoID = ObjectId.Parse(model.VideoId);

                    var videoLike = await repository.FirstOrDefaultAsync
                        (_ => _.VideoId == videoID && _.UserId == userId && _.LikeType == LikeType.Video);

                    if(videoLike is not null)
                    {
                        if (videoLike.IsLiked == model.IsLiked)
                            return APIResponse<int>.ErrorResponse("ALready responsed to this video");

                        if(model.IsLiked is null)
                        {
                            var deleteVideoLikeResponse = await repository.DeleteAsync(videoLike.Id);

                            return deleteVideoLikeResponse > 0 ? APIResponse<int>.SuccessResponse(deleteVideoLikeResponse, "Video response deleted") :
                                APIResponse<int>.ErrorResponse();
                        }

                        videoLike.IsLiked = Convert.ToBoolean(model.IsLiked);
                        videoLike.UpdatedAt = DateTime.Now;

                        var updateVideoLikeRespose = await repository.UpdateAsync(videoLike);

                        return updateVideoLikeRespose > 0 ? APIResponse<int>.SuccessResponse(updateVideoLikeRespose,"Video response updated"):
                            APIResponse<int>.ErrorResponse();
                    }


                    var newVideoLike = new Likes
                    {
                        IsLiked = Convert.ToBoolean(model.IsLiked),
                        LikeType = LikeType.Video,
                        VideoId = videoID,
                        UserId = userId,
                    };


                    var addedVideoLikeResponse = await repository.InsertAsync(newVideoLike);

                    return addedVideoLikeResponse > 0 ? APIResponse<int>.SuccessResponse(addedVideoLikeResponse, "Video response added") :
                        APIResponse<int>.ErrorResponse();
            }

            return APIResponse<int>.ErrorResponse();
        }

        public async Task<APIResponse<List<LikedVideoResponse>>> FetchUserlikedVideos(FilterModel model)
        {
            var userId = ObjectId.Parse("6639bb42c92b6748cb77c6fa");
            //var userId = contextService.GetUserId();

            if (userId == ObjectId.Empty)
                return APIResponse<List<LikedVideoResponse>>.ErrorResponse("Not authorized");

            var likedVideos = await repository.FetchUserLikedVideos(model, userId);

            var response = likedVideos.Select(_ => new LikedVideoResponse
            {
                ChannelName = _.ChannelName,
                CreatedAt = _.CreatedAt,
                Description = _.Description,
                Duration = _.Duration,
                DurationWatched = _.DurationWatched,
                Id = _.Id.ToString(),
                Thumbnail  =_.Thumbnail,
                Title = _.Title,
                Url = _.Url,
                VideoId = _.VideoId.ToString(),
                Views = _.Views
            }).ToList();

            return APIResponse<List<LikedVideoResponse>>.SuccessResponse(response, "Liked videos fetched");
        }
    }
}
