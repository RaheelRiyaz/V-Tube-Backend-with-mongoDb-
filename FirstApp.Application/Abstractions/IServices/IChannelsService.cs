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
    public interface IChannelsService
    {
        Task<APIResponse<int>> CreateChannel(ChannelRequest model);
        Task<APIResponse<int>> Subscription(string channelId);
        Task<APIResponse<int>> ToggleNotify(string channelId);
        Task<APIResponse<int>> UpdateChannel(UpdateChannelRequest model);
        Task<APIResponse<UserChannelResponse>> ViewChannel(string channelId);
        Task<APIResponse<List<UserSubscribedViewResponse>>> ViewUserSubscribedChannels(FilterModel model);
    }
}
