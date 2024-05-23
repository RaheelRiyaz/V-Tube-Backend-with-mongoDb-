using FirstApp.Domain.Enums;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstApp.Application.DTOS
{
    public class ReportRequest
    {
        public string ReportContentType { get; set; } = null!;

        public string EntityId { get; set; } = null!;

        public ReportType ReportedOn { get; set; }
    }

    public class ReportContentTypeResponse
    {
        public string Id { get; set; } = null!;
        public string Title { get; set; } = null!;
    }

    public class CounterReportRequest
    {
        public string EntityId { get; set; } = null!;
        public string Justification { get; set; } = null!;
        public ReportType ReportType { get; set; }
    }

    public class OwnerReportBaseResponse
    {
        public ReportType ReportType { get; set; }
        public int ReportCount { get; set; }
        public bool IsTemporarilyRemoved { get; set; }
        public DateTime? TemporarilyRemovedAt { get; set; }
        public bool IsPermanentlyRemoved { get; set; }
        public DateTime? PermanentlyRemovedAt { get; set; }
        public bool IsRestored { get; set; }
        public DateTime? RestoredAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string? ReplyTitle { get; set; }
        public string? VideoTitle { get; set; }
        public string? CommentTitle { get; set; }
    }

    public class OwnerReportDBResponse : OwnerReportBaseResponse
    {
        public ObjectId Id { get; set; }
        public ObjectId EntityId { get; set; }
        public ObjectId OwnerId { get; set; }
    }


    public class OwnerReportResponse : OwnerReportBaseResponse
    {
        public string Id { get; set; } = null!;
        public string EntityId { get; set; } = null!;
        public string OwnerId { get; set; } = null!;
    }
}
