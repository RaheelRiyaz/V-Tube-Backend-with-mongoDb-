using FirstApp.Application.APIResponse;
using FirstApp.Application.DTOS;
using FirstApp.Domain.Models;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstApp.Application.Abstractions.IServices
{
    public interface IUsersService
    {
        Task<APIResponse<int>> Register(UserRequest model);
        Task<APIResponse<LoginResponse>> Login(LoginRequest model);
        Task<APIResponse<int>> ChangePassword(ChangePasswordRequest model);
        Task<APIResponse<LoginResponse>> RefreshToken(string refreshToken);
    }
}
