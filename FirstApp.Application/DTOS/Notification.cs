using FirstApp.Domain.Models;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstApp.Application.DTOS
{
    public record NotifySubscribersRequest
        (
        ObjectId ChannelId,
        string VideoTitle, 
        ObjectId VideoId
        );

    /*public class NotificationResponse
    {
        public string Id { get; set; } = null!;
        public string Id { get; set; } = null!;
    }*/
}
