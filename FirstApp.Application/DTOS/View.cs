using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstApp.Application.DTOS
{
   public class ViewRequest
    {
        public string VideoId { get; set; } = null!;
        public string DurationViewed { get; set; } = null!;
    }

    public class ViewBaseResponse
    {
        public DateTime ViewedAt { get; set; }

        public string Viewer { get; set; } = null!;
        public int DurationViewed { get; set; }
    }

    public class ViewDBResponse : ViewBaseResponse
    {
        public ObjectId Id { get; set; }

        public ObjectId VideoId { get; set; }

        public ObjectId ViewedBy { get; set; }

    }


    public class ViewAPIResponse : ViewBaseResponse
    {
        public string ViewedBy { get; set; } = null!;
        public string VideoId { get; set; } = null!;
        public string Id { get; set; } = null!;

    }
}
