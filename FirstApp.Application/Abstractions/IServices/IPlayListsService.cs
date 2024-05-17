
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
    public interface IPlayListsService 
    {
        Task<APIResponse<int>> CreatePlayList(PlayListsRequest model);
        Task<APIResponse<List<PlayListViewResponse>>> ViewPlayLists(ObjectId channelId);
    }
}
