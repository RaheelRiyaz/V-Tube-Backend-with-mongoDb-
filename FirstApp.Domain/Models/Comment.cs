using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstApp.Domain.Models
{
    public class Comment : BaseEntity
    {
        public ObjectId VideoId { get; set; }
        public ObjectId CommentedBy { get; set; }
        public string Title { get; set; } = null!;
    }
}
