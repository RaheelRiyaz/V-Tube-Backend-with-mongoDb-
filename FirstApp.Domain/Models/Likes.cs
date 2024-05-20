using FirstApp.Domain.Enums;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstApp.Domain.Models
{
    public class Likes : BaseEntity
    {
        public ObjectId? VideoId { get; set; }
        public ObjectId UserId { get; set; }
        public ObjectId? CommentId { get; set; }
        public bool IsLiked { get; set; }
        public Enums.LikeType LikeType { get; set; }
    }
}
