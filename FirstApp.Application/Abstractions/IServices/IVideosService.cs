using FirstApp.Application.APIResponse;
using FirstApp.Application.DTOS;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstApp.Application.Abstractions.IServices
{
    public interface IVideosService 
    {
        Task<APIResponse<int>> UploadVideo(VideoRequest model);
        Task<APIResponse<int>> UpdateVideo(UpdateVideoRequest model);
        Task<APIResponse<int>> DeleteVideo(string id);
        Task<APIResponse<List<string>>> GetVideoSearchSuggestions(string searchTerm);
        Task<APIResponse<List<VideoViewModel>>> FetchVideosByChannel(VideoFilter model);
    }
}
