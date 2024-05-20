
using FirstApp.Application.Abstractions.IServices;
using FirstApp.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstApp.Application
{
    public static class AssemblyReference
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services,string webrootPath)
        {
            services.AddSingleton<IStorageService>(new StorageService(webrootPath));
            services.AddScoped<IUsersService, UsersService>();
            services.AddScoped<IChannelsService, ChannelsService>();
            services.AddScoped<IPlayListsService, PlayListsService>();
            services.AddScoped<IVideosService, VideosService>();
            services.AddScoped<IViewsService, ViewsService>();
            services.AddScoped<ICommentsService, CommentsService>();
            services.AddScoped<IWatchHistoryService, WatchHistoryService>();
            services.AddScoped<ILikesService, LikesService>();
            services.AddScoped<INotificationsService, NotificationsService>();
            services.AddScoped<IWatchLaterService, WatchLaterService>();
            services.AddScoped<IReportsService, ReportsService>();
            services.AddScoped<IReportContentTypesService, ReportContentTypesService>();
            return services;
        }
    }
}
