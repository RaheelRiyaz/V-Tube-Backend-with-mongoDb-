
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
    public interface IWatchHistoryService 
    {
        Task<APIResponse<int>> UpdateWatchHistory(WatchHistoryRequest model,ObjectId userId);
        Task<APIResponse<List<HistoryViewResponse>>> FetchUserHistory(FilterModel model);
        Task<APIResponse<int>> RemoveVideoFromHistory(List<string> videoIds);
        Task<APIResponse<int>> ClearWatchHistory();
    }
}
