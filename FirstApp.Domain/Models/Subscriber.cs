using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstApp.Domain.Models
{
    public class Subscriber : BaseEntity
    {
        public ObjectId ChannelId { get; set; }
        public ObjectId SubscribedBy { get; set; }
        public bool HasNotified { get; set; }
    }
}
