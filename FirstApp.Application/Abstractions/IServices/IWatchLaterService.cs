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
    public interface IWatchLaterService
    {
        Task<APIResponse<int>> AddVideoToWatchLater(string videoId);
        Task<APIResponse<int>> RemoveVideoFromWatchLater(string id);
        Task<APIResponse<List<WatchLaterViewResponse>>> FetchWatchLaterVideos(WatchLaterFilterModel model);
        Task<APIResponse<WatchLaterHeader>> FetchWatchlaterHeader(int sortOrder);
    }
}
