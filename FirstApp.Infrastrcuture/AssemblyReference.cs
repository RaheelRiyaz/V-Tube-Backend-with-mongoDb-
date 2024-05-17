﻿using FirstApp.Application.Abstractions.IServices;
using FirstApp.Infrastrcuture.Models;
using FirstApp.Infrastrcuture.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstApp.Infrastrcuture
{
    public static class AssemblyReference
    {
        public static IServiceCollection AddInfrastrcutureServices(this IServiceCollection services,IConfiguration configuration)
        {
            services.AddSingleton<ITokenService>(new TokenService(configuration));
            services.AddScoped<IContextService, ContextService>();
            services.AddScoped<ICloudinaryService, CloudinaryService>();
            services.Configure<CloudinaryInstance>(configuration.GetSection("Cloudinary"));
            return services;
        }
    }
}
