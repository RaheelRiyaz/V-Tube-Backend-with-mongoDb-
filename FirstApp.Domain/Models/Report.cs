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
        public ObjectId ReportContentType { get; set; }


        public ObjectId ReportedBy { get; set; }


        public ObjectId EntityId { get; set; }


        public ReportType ReportedOn { get; set; }
    }
}
