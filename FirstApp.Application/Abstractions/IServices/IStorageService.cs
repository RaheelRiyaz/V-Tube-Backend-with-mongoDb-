using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstApp.Application.Abstractions.IServices
{
    public interface IStorageService
    {
        Task<string> SaveLocalFile(IFormFile file);
        int DeleteLocalFile(string filePath);
        string GetVideoDuration(string filePath);
    }
}
