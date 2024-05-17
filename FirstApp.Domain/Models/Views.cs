using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstApp.Domain.Models
{
    public class Views : BaseEntity
    {
        public ObjectId VideoId { get; set; }
        public ObjectId ViewedBy { get; set; }
        public string DurationViewed { get; set; } = null!;
    }
}
