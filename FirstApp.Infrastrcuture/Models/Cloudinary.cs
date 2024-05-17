using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstApp.Infrastrcuture.Models
{
    public class CloudinaryInstance
    {
        public string CloudName { get; set; } = null!;
        public string ApiKey { get; set; } = null!;
        public string ApiSecret { get; set; } = null!;
        public string APiEnvironmentVariable { get; set; } = null!;
    }
}
