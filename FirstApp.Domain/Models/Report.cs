using FirstApp.Domain.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstApp.Domain.Models
{
    public class Report : BaseEntity
    {
        [BsonId]
        public ObjectId ReportContentType { get; set; }


        [BsonId]
        public ObjectId ReportedBy { get; set; }


        [BsonId]
        public ObjectId EntityId { get; set; }


        public ReportType ReportedOn { get; set; }
    }
}
