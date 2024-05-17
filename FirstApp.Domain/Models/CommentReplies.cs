using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstApp.Domain.Models
{
    public class CommentReplies : BaseEntity
    {
        public ObjectId CommentId { get; set; }
        public ObjectId RepliedBy { get; set; }
        public string Title { get; set; } = null!;
    }
}
