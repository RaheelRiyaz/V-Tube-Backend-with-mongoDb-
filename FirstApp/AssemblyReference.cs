﻿using FirstApp.GlobalExceptionMiddleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace FirstApp
{
    public static class AssemblyReference
    {
        public static IServiceCollection AddApiServices(this IServiceCollection services,IConfiguration configuration)
        {
            services.AddAuthentication((options) =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new()
                {
                    ValidateAudience = true,
                    ValidAudience = configuration["Jwt:Audience"],
                    ValidateIssuer = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.ASCII.GetBytes(configuration["Jwt:Key"]!)),
                    RequireExpirationTime = true
                };
            });
            services.AddHttpContextAccessor();

            services.AddExceptionHandler<ExceptionMiddleware>();
            return services;
        }
    }
}
