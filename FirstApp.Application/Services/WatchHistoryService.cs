
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
    public class WatchHistoryService
        (
        IWatchHistoryRepository repository,
        IContextService contextService
        )
        : IWatchHistoryService
    {
        public async Task<APIResponse<int>> ClearWatchHistory()
        {
            //var userId = contextService.GetUserId();
            var userId = ObjectId.Parse("6639bb42c92b6748cb77c6fa");

            var res = await repository.DeleteWatchHistoryByUserId(userId);

            return res > 0 ? APIResponse<int>.SuccessResponse((int)res, "History cleared successfully"):
                APIResponse<int>.ErrorResponse();
        }

        public async Task<APIResponse<List<HistoryViewResponse>>> FetchUserHistory(FilterModel model)
        {
            //var userId = contextService.GetUserId();
            var userId = ObjectId.Parse("6639bb42c92b6748cb77c6fa");

            var history = await repository.FetchUserHistory(model,userId);

            var response = history.Select(_ => new HistoryViewResponse
            {
                CreatedAt = _.CreatedAt,
                Description = _.Description,
                DurationViewed = _.DurationViewed,
                Id = _.Id.ToString(),
                VideoId = _.VideoId.ToString(),
                Thumbnail = _.Thumbnail,
                TotalDuration = _.TotalDuration,
                ChannelName = _.ChannelName,
                ChannelCover = _.ChannelCover,
                Title = _.Title,
                Url = _.Url,
                Views = _.Views,
                PlayListId = _.PlayListId.ToString(),
                ChannelId = _.ChannelId.ToString(),
            })
                .ToList();


            return APIResponse<List<HistoryViewResponse>>.SuccessResponse(response, "Historyfetched successfully");
        }

        public async Task<APIResponse<int>> RemoveVideoFromHistory(List<string> videoIds)
        {
            var ids = videoIds.Select(_ => new ObjectId(_));

            var res = await repository.DeleteRangeAsync(ids);
            return res > 0 ? APIResponse<int>.SuccessResponse(res) :
                APIResponse<int>.ErrorResponse();
        }

        public async Task<APIResponse<int>> UpdateWatchHistory(WatchHistoryRequest model,ObjectId userId)
        {

            var videoInHistory = await repository.FirstOrDefaultAsync
                (_ => _.UserId == userId && _.VideoId == model.VideoId);


            if (videoInHistory is not null)
            {
                videoInHistory.DurationViewed = model.DurationViewed;
                videoInHistory.UpdatedAt = DateTime.Now;

                var updateResponse = await repository.UpdateAsync(videoInHistory);

                return updateResponse > 0 ? APIResponse<int>.SuccessResponse(updateResponse, "Video history updated") :
                    APIResponse<int>.ErrorResponse();
            }

            var newVideo = new WatchHistory
            {
                DurationViewed = model.DurationViewed,
                UserId = userId,
                VideoId = model.VideoId
            };

            var addedResponse = await repository.InsertAsync(newVideo);
            return addedResponse > 0 ? APIResponse<int>.SuccessResponse(addedResponse, "Video history updated") :
                APIResponse<int>.ErrorResponse();
        }
    }
}
