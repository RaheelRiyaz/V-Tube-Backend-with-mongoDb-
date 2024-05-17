using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstApp.Domain.Models
{
    public class Channel : BaseEntity
    {
        public string Name { get; set; } = null!;
        public string Handle { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string CoverUrl { get; set; } = null!;
        public ObjectId Owner { get; set; }
       
    }
}
