using FirstApp.Application.Abstractions.IRepositories;
using FirstApp.Application.Abstractions.IServices;
using FirstApp.Application.APIResponse;
using FirstApp.Application.DTOS;
using FirstApp.Domain.Models;
using MongoDB.Bson;

namespace FirstApp.Application.Services
{
    public class WatchLaterService
        (
        IWatchLaterRepository repository,
        IContextService contextService
        ) : IWatchLaterService
    {
        public async Task<APIResponse<int>> AddVideoToWatchLater(string videoId)
        {
            //var userId = contextService.GetUserId();
            var userId = ObjectId.Parse("6639bb42c92b6748cb77c6fa");

            var video = await repository.FirstOrDefaultAsync
                (_ => _.VideoId == new ObjectId(videoId) && _.UserId == userId);

            if(video is not null)
            {
                video.CreatedAt = DateTime.UtcNow;
                video.UpdatedAt = DateTime.UtcNow;

                var updateVideoWatchLaterResponse = await repository.UpdateAsync(video);

                return updateVideoWatchLaterResponse > 0 ? APIResponse<int>.SuccessResponse(updateVideoWatchLaterResponse, "Watch later updated") :
                    APIResponse<int>.ErrorResponse();
            }

            var newVideo = new WatchLater
            {
                UserId = userId,
                VideoId = new ObjectId(videoId)
            };

            var addToLaterResult = await repository.InsertAsync(newVideo);

            return addToLaterResult > 0 ? APIResponse<int>.SuccessResponse(addToLaterResult, "Video added in watch later") :
                APIResponse<int>.ErrorResponse();
        }

        public async Task<APIResponse<WatchLaterHeader>> FetchWatchlaterHeader(int sortOrder)
        {
            /*if (sortOrder != 1 || sortOrder != -1)
                return APIResponse<WatchLaterHeader>.ErrorResponse("Only 1 and -1 are allowed");*/

            //var userId = contextService.GetUserId();
            var userId = ObjectId.Parse("6639bb42c92b6748cb77c6fa");

            var header = await repository.FetchWatchLaterHeader(new WatchLaterHeaderFilter
            {
                SortOrder = sortOrder,
                UserId = userId
            });

            return APIResponse<WatchLaterHeader>.SuccessResponse(header);
        }

        public async Task<APIResponse<List<WatchLaterViewResponse>>> FetchWatchLaterVideos(WatchLaterFilterModel model)
        {
            //var userId = contextService.GetUserId();
            var userId = ObjectId.Parse("6639bb42c92b6748cb77c6fa");

            var videos = await repository.FetchWatchLaterVideos(userId, model);

            var response = videos.Select(_ => new WatchLaterViewResponse
            {
                ChannelId = _.ChannelId.ToString(),
                Id = _.Id.ToString(),
                PlayListId = Convert.ToString(_.PlayListId),
                ChannelName = _.ChannelName,
                CreatedAt = _.CreatedAt,
                Description = _.Description,
                Duration = _.Duration,
                Title = _.Title,
                Url = _.Url,
                VideoId = _.VideoId.ToString(),
                Views = _.Views,
                Thumbnail = _.Thumbnail
            });
            return APIResponse<List<WatchLaterViewResponse>>.SuccessResponse(response.ToList());
        }

        public async Task<APIResponse<int>> RemoveVideoFromWatchLater(string id)
        {
            //var userId = contextService.GetUserId();
            var userId = ObjectId.Parse("6639bb42c92b6748cb77c6fa");

            var video = await repository.FindOneAsync(new ObjectId(id));

            if (video is null)
                return APIResponse<int>.ErrorResponse("Invalid video id");

            var removedResponse = await repository.DeleteAsync(video);

            return removedResponse > 0 ? APIResponse<int>.SuccessResponse(removedResponse, "Video has been removed from watch later") :
                APIResponse<int>.ErrorResponse();
        }
    }
}
