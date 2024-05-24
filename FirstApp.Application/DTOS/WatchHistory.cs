using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstApp.Application.DTOS
{
    public class WatchHistoryRequest
    {
        [Required]
        public ObjectId VideoId { get; set; }

        [Required]
        public string DurationViewed { get; set; } = null!;
    }


    public class BaseHistoryResponse
    {
        public DateTime CreatedAt { get; set; }
        public string Title { get; set; } = null!;
        public string Thumbnail { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Url { get; set; } = null!;
        public string DurationViewed { get; set; } = null!;
        public string TotalDuration { get; set; } = null!;
        public string ChannelName { get; set; } = null!;
        public string ChannelCover { get; set; } = null!;
        public int Views { get; set; }
    };

    public class HistoryDBResponse : BaseHistoryResponse
    {
        public ObjectId Id { get; set; }
        public ObjectId VideoId { get; set; }
        public ObjectId ChannelId { get; set; }
        public ObjectId? PlayListId { get; set; }
    }

    public class HistoryViewResponse : BaseHistoryResponse
    {
        public string Id { get; set; } = null!;
        public string VideoId { get; set; } = null!;
        public string ChannelId { get; set; } = null!;
        public string? PlayListId { get; set; } = null!;
    }

    public class FilterModel
    {
        public int PageNo { get; set; }
        public int PageSize { get; set; }

        [AllowedValues(1, -1, ErrorMessage = "only 1 and -1 are allowed")]
        public int? SortOrder { get; set; } = -1;
    }
        
    public class WatchLaterFilterModel
    {
        public int PageNo { get; set; }
        public int PageSize { get; set; }

        [AllowedValues(1, -1, ErrorMessage = "Only 1 and -1 are allowed")]
        public int SortOrder { get; set; }
    }

}
