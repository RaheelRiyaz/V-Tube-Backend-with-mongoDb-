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
    public class ViewsService
        (
        IViewsRepository repository,
        IChannelsRepository channelsRepository,
        IVideousRepository videousRepository,
        IContextService contextService,
        IWatchHistoryService watchHistoryService
        )
        : IViewsService
    {
        public async Task<APIResponse<int>> AddView(ViewRequest model)
        {
            //var userId = contextService.GetUserId();
            var videoId = ObjectId.Parse(model.VideoId);
            var userId = ObjectId.Parse("6639bb42c92b6748cb77c6fa");

            if (userId == ObjectId.Empty)
                return APIResponse<int>.ErrorResponse("Not authorized");

            var video = await videousRepository.FirstOrDefaultAsync(_=>_.Id == videoId);

            if (video is null)
                return APIResponse<int>.ErrorResponse("No Such video found");

            var view = await repository.FirstOrDefaultAsync
                (_=>_.VideoId == videoId && _.ViewedBy == userId);

            var history = new WatchHistoryRequest
            {
                DurationViewed = model.DurationViewed,
                VideoId = videoId
            };

            await watchHistoryService.UpdateWatchHistory(history, userId);

            if (view is not null)
            {
                TimeSpan timeSpan1 = TimeSpan.Parse(view.DurationViewed);
                TimeSpan timeSpan2 = TimeSpan.Parse(model.DurationViewed);

                if (timeSpan1 < timeSpan2)
                {
                    view.DurationViewed = model.DurationViewed;
                    view.UpdatedAt = DateTime.Now;

                    var updateDurationResilt = await repository.UpdateAsync(view);
                    await watchHistoryService.UpdateWatchHistory(history, userId);

                    return updateDurationResilt > 0 ? APIResponse<int>.SuccessResponse(updateDurationResilt, "View duartion updated") :
                        APIResponse<int>.ErrorResponse();
                }

                return APIResponse<int>.ErrorResponse("Already have viewed");
            }


            var newView = new Views
            {
                DurationViewed = model.DurationViewed,
                VideoId = videoId,
                ViewedBy = userId
            };

            var addedViewResult = await repository.InsertAsync(newView);

            return addedViewResult > 0 ? APIResponse<int>.SuccessResponse(addedViewResult,"View added to this video") :
                APIResponse<int>.ErrorResponse();
        }

        public async Task<APIResponse<List<ViewAPIResponse>>> FetchVideoViews(CommentsFilter model)
        {
            var views = await repository.FetchVideoViews(model);

            var response = views.Select(_ => new ViewAPIResponse
            {
                ViewedAt = _.ViewedAt,
                DurationViewed = _.DurationViewed,
                Id = _.Id.ToString(),
                VideoId = _.VideoId.ToString(),
                ViewedBy = _.ViewedBy.ToString(),
                Viewer = _.Viewer
            })
                .ToList();

            return APIResponse<List<ViewAPIResponse>>.SuccessResponse(response, "Views fetched succcessfully");
        }
    }
}
