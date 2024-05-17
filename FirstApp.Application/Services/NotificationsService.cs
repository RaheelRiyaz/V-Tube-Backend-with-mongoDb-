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
    public class NotificationsService 
        (
        INotificationsRepository repository,
        ISubscriptionRepository subscriptionRepository,
        IChannelsRepository channelsRepository
        ) : INotificationsService
    {
        public async Task<APIResponse<int>> NotifySubscribers(NotifySubscribersRequest model)
        {
            var subscribers = await subscriptionRepository.FilterAsync
                (_=>_.ChannelId == model.ChannelId && _.HasNotified == true);

            if (!subscribers.Any())
                return APIResponse<int>.ErrorResponse("No Subscriber has notified for this channel");

            var channel = await channelsRepository.FindOneAsync(model.ChannelId);

            if (channel is null)
                return APIResponse<int>.ErrorResponse("No such channel found");

            var notificationsList = new List<Notification>();

            foreach (var subscriber in subscribers)
            {
                notificationsList.Add(new Notification
                {
                   Title = $"{channel.Name} has uploaded new video ({model.VideoTitle})",
                   UserId = subscriber.SubscribedBy,
                   VideoId = model.VideoId,
                });
            }


            var insertResult = await repository.InsertRangeAsync(notificationsList);

            return insertResult > 0 ? APIResponse<int>.SuccessResponse(insertResult, "Notification sent to subscribers") :
                APIResponse<int>.ErrorResponse();
        }


    }
}
