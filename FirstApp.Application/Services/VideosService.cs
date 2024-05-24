using FirstApp.Application.Abstractions.IRepositories;
using FirstApp.Application.Abstractions.IServices;
using FirstApp.Application.APIResponse;
using FirstApp.Application.DTOS;
using FirstApp.Domain.Models;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstApp.Application.Services
{
    public class VideosService
        (
        IContextService contextService,
        IVideousRepository repository,
        IChannelsRepository channelsRepository,
        ICloudinaryService cloudinaryService,
        INotificationsService notificationsService
        )
        : IVideosService
    {
        public async Task<APIResponse<int>> DeleteVideo(string id)
        {
            var video = await repository.FindOneAsync(new ObjectId(id));

            if (video is null)
                return APIResponse<int>.ErrorResponse("Invalid video");

            var userId = ObjectId.Parse("6639a524cb32b4eca722a251");
            //var userId = contextService.GetUserId();

            var channel = await channelsRepository.FindOneAsync(video.ChannelId);

            if (channel?.Owner != userId)
                return APIResponse<int>.ErrorResponse("Only channel owner are permissible to delete a video");

            var deletedResponse = await repository.DeleteAsync(video.Id);

            await cloudinaryService.DeleteFileOnCloudinary(video.Url,video.Thumbnail);

            return deletedResponse > 0 ? APIResponse<int>.SuccessResponse(deletedResponse, "Video has been deleted successfully") :
                APIResponse<int>.ErrorResponse();

        }

        public async Task<APIResponse<List<VideoViewModel>>> FetchVideosByChannel(VideoFilter model)
        {
            var userId = ObjectId.Parse("6639bb42c92b6748cb77c6fa");
            //var userId = contextService.GetUserId();

            var videos = await repository.FetchVideosByChannel(model,userId);

            var result = videos.Select(_ => new VideoViewModel
            {
                ChannelId = _.ChannelId.ToString(),
                CommentsTurnedOff = _.CommentsTurnedOff,
                ChannelName = _.ChannelName,
                Description = _.Description,
                Duration = _.Duration,
                Thumbnail = _.Thumbnail,
                CreatedAt = _.CreatedAt,
                Id = _.Id.ToString(),
                Title = _.Title,
                TotalComments = _.TotalComments,
                IsOwner = _.IsOwner,
                DurationInHistory = _.DurationInHistory,
                TotalCommentsAndReplies = _.TotalCommentsAndReplies,
                TotalLikes = _.TotalLikes,
                TotalReplies = _.TotalReplies,
                TotalViews = _.TotalViews,
                Url = _.Url,
                CoverUrl = _.CoverUrl,
                PlayListId = _.PlayListId.ToString(),
                TotalDislikes = _.TotalDislikes,
                HasUserLiked = _.HasUserLiked
            }).
            ToList();

            return APIResponse<List<VideoViewModel>>.SuccessResponse(result);
        }

        public async Task<APIResponse<List<string>>> GetVideoSearchSuggestions(string searchTerm)
        {
            var suggestions = await repository.GetVideoSearchSuggestions(searchTerm);

            return APIResponse<List<string>>.SuccessResponse(suggestions);
        }

        public async Task<APIResponse<int>> TurnCommentsOff(string id)
        {
            var video = await repository.FindOneAsync(new ObjectId(id));

            if (video is null)
                return APIResponse<int>.ErrorResponse("Invalid video");

            var userId = ObjectId.Parse("6639a524cb32b4eca722a251");
            //var userId = contextService.GetUserId();

            var channel = await channelsRepository.FindOneAsync(video.ChannelId);

            if (channel?.Owner != userId)
                return APIResponse<int>.ErrorResponse("Only channel owner are permissible to change settings of a video");

            video.CommentsTurnedOff = !video.CommentsTurnedOff;

            var updatedVideoResponse = await repository.UpdateAsync(video);

            return updatedVideoResponse > 0 ? APIResponse<int>.SuccessResponse(updatedVideoResponse, "Video settings has been updated successfully") :
                APIResponse<int>.ErrorResponse();
        }

        public async Task<APIResponse<int>> UpdateVideo(UpdateVideoRequest model)
        {
            var video = await repository.FirstOrDefaultAsync
                (_ => _.Id == new ObjectId(model.Id));

            if (video is null)
                return APIResponse<int>.ErrorResponse("Video is invalid");

            if(model.Thumbnail is not null)
            {
                var cloudinaryResponse = await cloudinaryService.UploadFileOnCloudinary(model.Thumbnail);

                if (cloudinaryResponse is null)
                    return APIResponse<int>.ErrorResponse("Couldn't change thumbnail please try again later");

                video.Thumbnail = cloudinaryResponse.Url.ToString();
            }

            video.UpdatedAt = DateTime.Now;
            video.Title = model.Title;
            video.Description = model.Description;
            video.PlayListId = model.PlayListId;

            var updateVideoResponse = await repository.UpdateAsync(video);

            return updateVideoResponse > 0 ? APIResponse<int>.SuccessResponse(updateVideoResponse, "Video has been updated") :
                APIResponse<int>.ErrorResponse();
        }

        public async Task<APIResponse<int>> UploadVideo(VideoRequest model)
        {
            var userId = contextService.GetUserId();

            if (userId == ObjectId.Empty)
                return APIResponse<int>.ErrorResponse("You are not authorized");

            var channel = await channelsRepository.FirstOrDefaultAsync(_=>_.Owner == userId);

            if (channel is null)
                return APIResponse<int>.ErrorResponse("Please create a channel");

            var (clouidnaryVideoresult,duration) = await cloudinaryService.UploadVideoFileOnCloudinary(model.File);

            if (clouidnaryVideoresult is null)
                return APIResponse<int>.ErrorResponse("Couldn't upload a video");

            var cloduinarythumbnailresult = await cloudinaryService.UploadFileOnCloudinary(model.Thumbnail);

            if(cloduinarythumbnailresult is null)
                return APIResponse<int>.ErrorResponse("Couldn't upload a thumbnail");

            var video = new Video
            {
                Description = model.Description,
                Thumbnail = cloduinarythumbnailresult.Url.ToString(),
                Url = clouidnaryVideoresult.Url.ToString(),
                ChannelId = channel.Id,
                Duration = duration ?? "00:00:00",
                PlayListId = model.PlayListId is null ? null : new ObjectId(model.PlayListId),
                Title = model.Title
            };


            var videoResult = await repository.InsertAsync(video);

            var notifyModel = new NotifySubscribersRequest(channel.Id,video.Title,video.Id);

            await notificationsService.NotifySubscribers(notifyModel);

            return videoResult > 0 ? APIResponse<int>.SuccessResponse(videoResult, "Video uploaded successfully") :
                APIResponse<int>.ErrorResponse();
        }
    }
}
