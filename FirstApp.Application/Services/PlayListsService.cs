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
    public class PlayListsService
        (
        IPlayListsRepository repository,
        IChannelsRepository channelsRepository,
        IContextService contextService
        )
        : IPlayListsService
    {
        public async Task<APIResponse<int>> CreatePlayList(PlayListsRequest model)
        {
            var userId = contextService.GetUserId();

            if (userId == ObjectId.Empty)
                return APIResponse<int>.ErrorResponse("You are not authorized");

            var channel = await channelsRepository.FirstOrDefaultAsync(_ => _.Owner == userId);

            if (channel is null)
                return APIResponse<int>.ErrorResponse("Please create a channel first");

            var playlistExists = await repository.ExistsAsync
                (_ => _.Name.ToLower() == model.Name.ToLower() && _.ChannelId == channel.Id);

            if (playlistExists)
                return APIResponse<int>.ErrorResponse("Playlist already exists by this name");

            var newPlayList = new PlayList
            {
                ChannelId = channel.Id,
                Name = model.Name,
                Description = model.Description,
            };

            var addedResult = await repository.InsertAsync(newPlayList);

            return addedResult > 0 ? APIResponse<int>.SuccessResponse(addedResult, "Playlist created successfully") :
                APIResponse<int>.ErrorResponse();
        }

        public async Task<APIResponse<List<PlayListViewResponse>>> ViewPlayLists(ObjectId channelId)
        {
            var playlists = await repository.ViewPlayLists(channelId);

            var response = playlists.Select(_ => new PlayListViewResponse
            {
                ChannelId = _.ChannelId.ToString(),
                Id = _.Id.ToString(),
                CoverUrl = _.CoverUrl,
                Description = _.Description,
                Name = _.Name,
                VideoCount = _.VideoCount
            });
            return APIResponse<List<PlayListViewResponse>>.SuccessResponse(response.ToList(), "Fetched");
        }
    }
}
