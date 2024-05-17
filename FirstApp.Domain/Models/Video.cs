using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstApp.Domain.Models
{
    public class Video : BaseEntity
    {
        [BsonRequired]
        public string Title { get; set; } = null!;

        [BsonRequired]
        public string Description { get; set; } = null!;

        [BsonRequired]
        public string Thumbnail { get; set; } = null!;

        [BsonRequired]
        public string Url { get; set; } = null!;

        [BsonRequired]
        public ObjectId ChannelId { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId? PlayListId { get; set; }

        [BsonRequired]
        public string Duration { get; set; } = null!;

    }

}
