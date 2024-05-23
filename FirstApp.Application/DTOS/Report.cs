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
}
