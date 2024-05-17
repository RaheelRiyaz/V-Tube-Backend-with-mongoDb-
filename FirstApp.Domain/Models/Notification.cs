using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstApp.Domain.Models
{
    public class Notification : BaseEntity
    {
        public ObjectId UserId { get; set; }
        public string Title { get; set; } = null!;
        public ObjectId? VideoId { get; set; }
        public ObjectId? CommentId { get; set; }
        public bool HasRead { get; set; } = false;
    }
}
