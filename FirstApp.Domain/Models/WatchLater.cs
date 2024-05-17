using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstApp.Domain.Models
{
    public class WatchLater : BaseEntity
    {
        public ObjectId UserId { get; set; }
        public ObjectId VideoId { get; set; }
    }
}
