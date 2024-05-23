using FirstApp.Domain.Enums;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstApp.Domain.Models
{
    public class CounterReport : BaseEntity
    {
        public ObjectId CounteredBy { get; set; }
        public ObjectId EntityId { get; set; }
        public ReportType ReportType { get; set; }
        public bool HasJustified { get; set; }
        public bool Inspected { get; set; }
        public string Justification { get; set; } = null!;
        public ObjectId? InspectedBy { get; set; } 
        public DateTime? InspectedAt { get; set; } 
        public DateTime? ResolvedAt { get; set; }
        public ReportResolution Resolution { get; set; } = ReportResolution.Pending;
    }

}
