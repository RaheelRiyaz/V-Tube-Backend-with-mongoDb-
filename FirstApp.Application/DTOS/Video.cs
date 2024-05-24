using FirstApp.Application.Validators;
using Microsoft.AspNetCore.Http;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstApp.Application.DTOS
{
    public class VideoRequest
    {
        public string? PlayListId { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;

        [ImageValidate("image/jpeg", "image/png",ErrorMessage = "Only jpg and png fromat is accepted")]
        public IFormFile Thumbnail { get; set; } = null!;

        [VideoValidation("video/mp4", ErrorMessage = "Only mp4 file accepetd")]
        public IFormFile File { get; set; } = null!;
    }


    public class UpdateVideoRequest
    {
        [BsonRequired]
        public string Id { get; set; } = null!;
        public ObjectId? PlayListId { get; set; }
        [BsonRequired]

        public string Title { get; set; } = null!;
        [BsonRequired]

        public string Description { get; set; } = null!;

        [ImageValidate("image/jpeg", "image/png", ErrorMessage = "Only jpg and png fromat is accepted")]
        public IFormFile? Thumbnail { get; set; } = null!;
    }

    public class VideoDBResponse
    {
        public ObjectId Id { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Thumbnail { get; set; } = null!;
        public string Url { get; set; } = null!;
        public int TotalViews { get; set; }
        public int TotalComments { get; set; }
        public int TotalReplies { get; set; }
        public bool HasLiked { get; set; }
    }


    public class DBVideoModel : BaseVideoModel
    {
        public ObjectId Id { get; set; }
        public ObjectId ChannelId { get; set; }
        public ObjectId? PlayListId { get; set; }
    }

    public class VideoViewModel : BaseVideoModel
    {
        public string Id { get; set; } = null!;
        public string ChannelId { get; set; } = null!;
        public string? PlayListId { get; set; } = null!;
    }

    public class BaseVideoModel
    {
        public string Thumbnail { get; set; } = null!;
        public bool? HasUserLiked { get; set; }
        public bool IsOwner { get; set; }
        public bool CommentsTurnedOff { get; set; }
        public string Url { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public string ChannelName { get; set; } = null!;
        public string CoverUrl { get; set; } = null!;          
        public string Title { get; set; } = null!;          
        public string Description { get; set; } = null!;
        public string Duration { get; set; } = null!;
        public int TotalLikes { get; set; }
        public int TotalDislikes { get; set; }
        public int TotalViews { get; set; }
        public string? DurationInHistory { get; set; }
        public int TotalComments { get; set; }
        public int TotalReplies { get; set; }
        public int TotalCommentsAndReplies { get; set; }
    }


    public record VideoFilter
        (
        string? ChannelId, 
        string? Term, 
        string? PlayListId, 
        int PageNumber,
        int PageSize,
        int? SortOrder
        );


    public class LikedVideoDBResponse : LikedVideoBaseResponse
    {
        public ObjectId Id { get; set; }
        public ObjectId VideoId { get; set; }
    }

    public class LikedVideoResponse : LikedVideoBaseResponse
    {
        public string Id { get; set; } = null!;
        public string VideoId { get; set; } = null!;
    }
    public class LikedVideoBaseResponse
    {
        public string Title { get; set; } = null!;
        public string Url { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string ChannelName { get; set; } = null!;
        public string Thumbnail { get; set; } = null!;
        public string Duration { get; set; } = null!;
        public string? DurationWatched { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public int Views { get; set; }
    }
}
