using FirstApp.Application.Abstractions.IRepositories;
using FirstApp.Application.Abstractions.IServices;
using FirstApp.Application.APIResponse;
using FirstApp.Application.DTOS;
using FirstApp.Application.Utilis;
using FirstApp.Domain.Models;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstApp.Application.Services
{
    public class UsersService 
        (
        IUsersRepository repository,
        ITokenService tokenService,
        IContextService contextService,
        ICloudinaryService cloudinaryService
        ) : IUsersService
    {
        public async Task<APIResponse<int>> ChangePassword(ChangePasswordRequest model)
        {
            var userid = contextService.GetUserId();

            if (userid == ObjectId.Empty)
                return APIResponse<int>.ErrorResponse("You are not authorized");

            var user = await repository.FindOneAsync(userid);

            if (user is null)
                return APIResponse<int>.ErrorResponse("You are not authorized");

            if(!AppEncryption.VerifyPassword(model.OldPassword,user.Password))
                return APIResponse<int>.ErrorResponse("Old password is incorrect");


            if(AppEncryption.VerifyPassword(model.NewPassword,user.Password))
                return APIResponse<int>.ErrorResponse("Old password and new password cannot be same");


            user.Password = AppEncryption.HashPassword(model.ConfirmPassword);

            var res = await repository.UpdateAsync(user);

            return res > 0 ? APIResponse<int>.SuccessResponse(res, "Password changed successfully") :
                APIResponse<int>.ErrorResponse();

        }

        public async Task<APIResponse<LoginResponse>> Login(LoginRequest model)
        {
            var user = await repository.FirstOrDefaultAsync(_ => _.UserName == model.UserName || _.Email == model.UserName);

            if (user is null)
                return APIResponse<LoginResponse>.ErrorResponse("Invalid Credentials");

            if(!AppEncryption.VerifyPassword(model.Password,user.Password))
                return APIResponse<LoginResponse>.ErrorResponse("Invalid Credentials");

            var response = new LoginResponse(Convert.ToString(user.Id)!, tokenService.GenerateAccessToken(user), tokenService.GenerateRefreshToken(user.Id));

            return APIResponse<LoginResponse>.SuccessResponse(response, "Logged in successfully");
        }

        public async Task<APIResponse<LoginResponse>> RefreshToken(string refreshToken)
        {
            var userId = contextService.GetUserId();

            if (userId == ObjectId.Empty)
                return APIResponse<LoginResponse>.ErrorResponse("You are not authorized");

            var user = await repository.FindOneAsync(userId);

            if (user is null)
                return APIResponse<LoginResponse>.ErrorResponse("Invalid user");

            if (user.RefreshToken != refreshToken)
                return APIResponse<LoginResponse>.ErrorResponse("Refresh token is invalid");

            if (AppEncryption.IsTokenExpired(refreshToken))
                return APIResponse<LoginResponse>.ErrorResponse("Refresh token has been expired please log in again now");

            user.RefreshToken = tokenService.GenerateRefreshToken(userId);

            var updatedUserResult = await repository.UpdateAsync(user);

            var accessToken = tokenService.GenerateAccessToken(user);

            var response = new LoginResponse
                (
                userId.ToString(),
                accessToken,
                refreshToken
                );

            return updatedUserResult > 0 ? APIResponse<LoginResponse>.SuccessResponse(response, "Your token has been refreshed") :
                APIResponse<LoginResponse>.ErrorResponse();

        }

        public async Task<APIResponse<int>> Register(UserRequest model)
        {

            if (string.IsNullOrEmpty(model.UserName) || string.IsNullOrEmpty(model.Password) || string.IsNullOrEmpty(model.Email))
                return APIResponse<int>.ErrorResponse("All model fields are required");


            var isEmailOrUserNameTaken = await repository.ExistsAsync(_=>_.UserName == model.UserName || _.Email == model.Email);

            if (isEmailOrUserNameTaken)
                return APIResponse<int>.ErrorResponse("Email or username is already taken");

            var user = new User
            {
                UserName = model.UserName,
                Email = model.Email,
                Password = AppEncryption.HashPassword(model.Password),
            };

            user.RefreshToken = tokenService.GenerateRefreshToken(user.Id);

            var result = await repository.InsertAsync(user);

            return result > 0 ? APIResponse<int>.SuccessResponse(result, "User registered successfully") :
                APIResponse<int>.ErrorResponse();
        }

    }
}
