using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstApp.Domain.Models
{
    public class User : BaseEntity
    {
        public string Email { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string ResetCode { get; set; } = null!;
        public DateTime? ResetExpiry { get; set; }
        public string RefreshToken { get; set; } = null!;
    }
}
