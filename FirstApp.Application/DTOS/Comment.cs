using FirstApp.Domain.Enums;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstApp.Application.DTOS
{
    public class CommentRequest
    {
        public string VideoId { get; set; } = null!;
        public string Title { get; set; } = null!;
    }


    public class CommentReplyRequest
    {
        public string CommentId { get; set; } = null!;
        public string Title { get; set; } = null!;
    };

    public class CommentViewResponse : CommentBaseResponse
    {
        public string Id { get; set; } = null!;
        public string CommentedBy { get; set; } = null!;
    }
    
    
    public class CommentDBResponse : CommentBaseResponse
    {
        public ObjectId Id { get; set; } 
        public ObjectId CommentedBy { get; set; }
    }


    public class CommentBaseResponse
    {
       
        public string Title { get; set; } = null!;
        public DateTime CreatedAt { get; set; } 
        public int TotalReplies { get; set; }
        public int Likes { get; set; }
        public int Dislikes { get; set; }
        public string Commenter { get; set; } = null!;
        public bool? HasUserLiked { get; set; }
    }


    public record CommentsFilter
        (
        string VideoId,
        int PageNo,
        int PageSize
        );

    public record ReplyFilter
        (
          string CommentId,
        int PageNo,
        int PageSize
        );


    public class ReplyBaseResponse 
    {
        public string Title { get; set; } = null!;
        public string Replier { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public int DisLikes { get; set; }
        public bool? HasUserLiked { get; set; }
        public int Likes { get; set; }
    }


    public class ReplyDBResponse : ReplyBaseResponse
    {
        public ObjectId Id { get; set; }
        public ObjectId CommentId { get; set; }
        public ObjectId RepliedBy { get; set; }

    }

    public class ReplyViewResponse : ReplyBaseResponse
    {
        public string Id { get; set; } = null!;
        public string CommentId { get; set; } = null!;
        public string RepliedBy { get; set; } = null!;
    }


    public record CommentUpdateRequest
        (
        string Id,
        string Title,
        CommentType CommentType
        );
}
