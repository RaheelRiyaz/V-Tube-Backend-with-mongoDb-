using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstApp.Application.DTOS
{
    public class PlayListsRequest
    {
        [Required]
        public string Name { get; set; } = null!;

        [Required]
        public string Description { get; set; } = null!;
    }


    public class BasePlayListResponse
    {
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string CoverUrl { get; set; } = null!;
        public int VideoCount { get; set; }
    }
    public class PlayListResponse : BasePlayListResponse
    {
        public ObjectId Id { get; set; }
        public ObjectId ChannelId { get; set; }
       
    }

    public class PlayListViewResponse : BasePlayListResponse
    {
        public string Id { get; set; } = null!; 
        public string ChannelId { get; set; } = null!;  
    }
}
