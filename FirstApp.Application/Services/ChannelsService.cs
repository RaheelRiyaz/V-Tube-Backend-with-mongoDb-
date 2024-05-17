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
    public class ChannelsService(
        ICloudinaryService cloudinaryService,
        IChannelsRepository repository,
        IContextService contextService,
        ISubscriptionRepository subscriptionRepository
        ) : IChannelsService
    {
        public async Task<APIResponse<int>> CreateChannel(ChannelRequest model)
        {
            var userId = contextService.GetUserId();

            if (userId == ObjectId.Empty)
                return APIResponse<int>.ErrorResponse("You are not authorized");

            var channelExists = await repository.ExistsAsync(_ => _.Owner == userId);

            if (channelExists)
                return APIResponse<int>.ErrorResponse("Already have a channel");

            var clodudinaryResult = await cloudinaryService.UploadFileOnCloudinary(model.File);

            if (clodudinaryResult is null)
                return APIResponse<int>.ErrorResponse("Couldn't upload a file please try again later");

            var channel = new Channel
            {
                CoverUrl = clodudinaryResult.Url.ToString(),
                Description = model.Description,
                Handle = model.Handle,
                Name = model.Name,
                Owner = userId,
            };

            var createdResult = await repository.InsertAsync(channel);

            return createdResult > 0 ? APIResponse<int>.SuccessResponse(createdResult, "Channel created successfully"):
                APIResponse<int>.ErrorResponse();
        }

        public async Task<APIResponse<int>> Subscription(string channelId)
        {
            //var userId = contextService.GetUserId();
            var userId = ObjectId.Parse("6639bb42c92b6748cb77c6fa");

            var subscriber = await subscriptionRepository.FirstOrDefaultAsync
                (_ => _.ChannelId == new ObjectId(channelId) && _.SubscribedBy == userId);

            if(subscriber is null)
            {
                var newSubscriber = new Subscriber
                {
                    SubscribedBy = userId,
                    ChannelId = new ObjectId(channelId),
                };

                var addedSubscriberResponse = await subscriptionRepository.InsertAsync(newSubscriber);

                return addedSubscriberResponse > 0 ? APIResponse<int>.SuccessResponse(addedSubscriberResponse, "You have subscribed to this channel"):
                    APIResponse<int>.ErrorResponse();
            }

            var removeSubscriberResponse = await subscriptionRepository.DeleteAsync(subscriber.Id);

            return removeSubscriberResponse > 0 ? APIResponse<int>.SuccessResponse(removeSubscriberResponse,"You have unsubscribed to this channel"):
                APIResponse<int>.ErrorResponse();
        }

        public async Task<APIResponse<int>> ToggleNotify(string channelId)
        {
            //var userId = contextService.GetUserId();
            var userId = ObjectId.Parse("6639bb42c92b6748cb77c6fa");

            var subscriber = await subscriptionRepository.FirstOrDefaultAsync
                (_ => _.ChannelId == new ObjectId(channelId) && _.SubscribedBy == userId);

            if (subscriber is null)
                return APIResponse<int>.ErrorResponse("You haven't subscribed to this channel");

            subscriber.HasNotified = !subscriber.HasNotified;


            var updateResult = await subscriptionRepository.UpdateAsync(subscriber);

            return updateResult > 0 ? APIResponse<int>.SuccessResponse(updateResult, "You have toggled the notify option"):
                APIResponse<int>.ErrorResponse();
        }

        public async Task<APIResponse<int>> UpdateChannel(UpdateChannelRequest model)
        {
            var channel = await repository.FindOneAsync(new ObjectId(model.Id));

            if (channel is null)
                return APIResponse<int>.ErrorResponse("Invalid channel");

            if(model.File is not null)
            {
                var cloudinaryResponse = await cloudinaryService.UploadFileOnCloudinary(model.File);

                if (cloudinaryResponse is null)
                    return APIResponse<int>.ErrorResponse("Couldn't upload cover file for the channel");

                channel.CoverUrl = cloudinaryResponse.Url.ToString();
            }

            channel.Description = model.Description;
            channel.Handle = model.Handle;
            channel.Name = model.Name;
            channel.UpdatedAt = DateTime.Now;

            var updateChannelResponse = await repository.UpdateAsync(channel);

            return updateChannelResponse > 0 ? APIResponse<int>.SuccessResponse(updateChannelResponse, "Channel info has been updated") :
                APIResponse<int>.ErrorResponse();
        }

        public async Task<APIResponse<UserChannelResponse>> ViewChannel(string channelId)
        {
            //var userId = contextService.GetUserId();
            var userId = ObjectId.Parse("6639bb42c92b6748cb77c6fa");

            var channel = await repository.ViewChannel(new ObjectId(channelId), userId);

            var response = new UserChannelResponse
            {
                CoverUrl = channel.CoverUrl,
                Description = channel.Description,
                Handle = channel.Handle,
                HasNotified = channel.HasNotified,
                HasUserSubscribed = channel.HasUserSubscribed,
                Id = channel.Id.ToString(),
                Owner = channel.Owner.ToString(),
                Name = channel.Name,
                PlayLists = channel.PlayLists,
                Subscribers = channel.Subscribers,
                Videos = channel.Videos,
            };

            return APIResponse<UserChannelResponse>.SuccessResponse(response);
        }

        public async Task<APIResponse<List<UserSubscribedViewResponse>>> ViewUserSubscribedChannels(FilterModel model)
        {
            //var userId = contextService.GetUserId();
            var userId = ObjectId.Parse("6639bb42c92b6748cb77c6fa");

            var channels = await subscriptionRepository.ViewUserSubscribedChannels(userId, model);

            var result = channels.Select(_ =>
                new UserSubscribedViewResponse
                {
                    ChannelId = _.ChannelId.ToString(),
                    Id = _.Id.ToString(),
                    Owner = _.Owner.ToString(),
                    CoverUrl = _.CoverUrl,
                    Description = _.Description,
                    Handle = _.Handle,
                    HasNotified = _.HasNotified,
                    Name = _.Name,
                    SubscribedAt = _.SubscribedAt
                }
            ).ToList();

            return APIResponse<List<UserSubscribedViewResponse>>.SuccessResponse(result, "Channels fetched successfully");
        }
    }
}
