
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
    public class OwnerReports : BaseEntity
    {
        public ObjectId EntityId { get; set; }

        public ReportType ReportType { get; set; }

        public ObjectId OwnerId { get; set; }

        public int ReportCount { get; set; }

        public bool IsTemporarilyRemoved { get; set; }

        public DateTime TemporarilyRemovedAt { get; set; } 

        public bool IsPermanentlyRemoved { get; set; } = false;

        public DateTime? PermanentlyRemovedAt { get; set; } 

        public bool IsRestored { get; set; } 

        public DateTime? RestoredAt { get; set; } 
    }
}
