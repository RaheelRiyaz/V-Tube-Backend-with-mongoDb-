using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstApp.Application.DTOS
{
    public class WatchLaterResponse
    {
       
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Url { get; set; } = null!;
        public string Thumbnail { get; set; } = null!;
        public TimeSpan Duration { get; set; }
        public DateTime CreatedAt { get; set; }
        public int Views { get; set; }
        public string ChannelName { get; set; } = null!;    
    }

    public class WatchLaterDbResponse : WatchLaterResponse
    {
        public ObjectId ChannelId { get; set; }
        public ObjectId? PlayListId { get; set; }
        public ObjectId Id { get; set; }
        public ObjectId VideoId { get; set; }
    }



    public class WatchLaterViewResponse : WatchLaterResponse
    {
        public string ChannelId { get; set; } = null!;
        public string? PlayListId { get; set; }
        public string Id { get; set; } = null!;
        public string VideoId { get; set; } = null!;
    }


    public class WatchLaterWithHeader
    {
        public WatchLaterHeader Header { get; set; } = null!;
        public List<WatchLaterViewResponse> Videos { get; set; } = null!;
    }
    public class WatchLaterHeader
    {
        public int Videos { get; set; }
        public string CoverUrl { get; set; } = null!;
    }

    public class WatchLaterHeaderFilter
    {
        public ObjectId UserId { get; set; } 

        [AllowedValues(1,-1,ErrorMessage = "Only 1 and -1 are allowed")]
        public int SortOrder { get; set; }
    }
}
