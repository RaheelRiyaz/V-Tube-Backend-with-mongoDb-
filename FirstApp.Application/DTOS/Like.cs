using FirstApp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstApp.Application.DTOS
{
    public class LikeRequest
    {
        public string? VideoId { get; set; } 
        public string? CommentId { get; set; }
        public LikeType LikeType { get; set; }
        public bool? IsLiked { get; set; }
    }
}
