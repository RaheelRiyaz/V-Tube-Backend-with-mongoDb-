using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstApp.Domain.Models
{
    public class AbusiveWord
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string WordEn { get; set; } = null!;
        public string WordUr { get; set; } = null!;
        public string WordEs { get; set; } = null!;
        public string WordFr { get; set; } = null!;
        public string WordDe { get; set; } = null!;
        public string WordZh { get; set; } = null!;
        public string WordHi { get; set; } = null!;
        public string WordAr { get; set; } = null!;
        public string WordRu { get; set; } = null!;
    }
}
