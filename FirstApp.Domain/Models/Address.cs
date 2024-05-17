using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstApp.Domain.Models
{
    public class Address : BaseEntity
    {
        public string Street { get; set; } = null!;
        //public string City { get; set; } = null!;
        //public string State { get; set; } = null!;
        public string ZipCode { get; set; } = null!;
    }
}
