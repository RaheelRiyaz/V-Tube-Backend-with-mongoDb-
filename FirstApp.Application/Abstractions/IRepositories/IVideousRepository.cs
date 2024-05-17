
using FirstApp.Application.DTOS;
using FirstApp.Domain.Models;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstApp.Application.Abstractions.IRepositories
{
    public interface IVideousRepository : IBaseRepository<Video>
    {
        Task<List<string>> GetVideoSearchSuggestions(string searchTerm);
        Task<List<DBVideoModel>> FetchVideosByChannel(VideoFilter model,ObjectId userId);
    }
}
