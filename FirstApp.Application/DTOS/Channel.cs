using FirstApp.Application.Validators;
using Microsoft.AspNetCore.Http;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstApp.Application.DTOS
{
   public class ChannelRequest
    {
        public string Name { get; set; } = null!;
        public string Handle { get; set; } = null!;
        public string Description { get; set; } = null!;

        [ImageValidate("image/jpeg", "image/png", ErrorMessage = "Only jpg and png fromat is accepted")]
        public IFormFile File { get; set; } = null!;

    }

    public class UpdateChannelRequest
    {
        public string Name { get; set; } = null!;
        public string Handle { get; set; } = null!;
        public string Description { get; set; } = null!;

        [ImageValidate("image/jpeg", "image/png", ErrorMessage = "Only jpg and png fromat is accepted")]
        public IFormFile? File { get; set; } = null!;
        public string Id { get; set; } = null!;
    }

    public class ChannelDBResponse : ChannelResponse
    {
        public ObjectId Id { get; set; } 
        public ObjectId Owner { get; set; }
    }
    public class UserChannelResponse : ChannelResponse
    {
        public string Id { get; set; } = null!;
        public string Owner { get; set; } = null!;
    }


    public class ChannelResponse 
    {  
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Handle { get; set; } = null!;
        public string CoverUrl { get; set; } = null!;
        public int PlayLists { get; set; }
        public int Videos { get; set; }
        public int Subscribers { get; set; }
        public bool? HasNotified { get; set; }
        public bool HasUserSubscribed { get; set; }
    }

   

    public class UserSubscribedDBResponse : UserSubscribedBasicResponse
    {
        public ObjectId Id { get; set; }
        public ObjectId ChannelId { get; set; }
        public ObjectId Owner { get; set; }
    }

    public class UserSubscribedViewResponse : UserSubscribedBasicResponse
    {
        public string Id { get; set; } = null!;
        public string ChannelId { get; set; } = null!;
        public string Owner { get; set; } = null!;
    }


    public class UserSubscribedBasicResponse 
    {
        public string Name { get; set; } = null!;
        public string Handle { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string CoverUrl { get; set;} = null!;
        public bool HasNotified { get; set; }
        public DateTime SubscribedAt { get; set; }
    }
}
