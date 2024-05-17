using FirstApp.Application.Abstractions.IRepositories;
using FirstApp.Persistence.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstApp.Persistence
{
    public static class AssemblyReference
    {
        public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IMongoClient>(sp =>
            {
                var connectionString = configuration["MongoDb:Connectionstring"];
                return new MongoClient(connectionString);
            });

            services.AddScoped<IMongoDatabase>(sp =>
            {
                var client = sp.GetRequiredService<IMongoClient>();
                var databaseName = configuration["MongoDb:DatabaseName"];
                return client.GetDatabase(databaseName);
            });

            services.AddScoped<IUsersRepository, UsersRepository>();
            services.AddScoped<IChannelsRepository, ChannelsRepository>();
            services.AddScoped<IPlayListsRepository, PlayListsRepository>();
            services.AddScoped<IVideousRepository, VideousRepository>();
            services.AddScoped<IViewsRepository, ViewsRepository>();
            services.AddScoped<ICommentsRepository, CommentsRepository>();
            services.AddScoped<IWatchHistoryRepository, WatchHistoryRepository>();
            services.AddScoped<ICommentRepliesRepository, CommentRepliesRepository>();
            services.AddScoped<ILikesRepository, LikesRepository>();
            services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
            services.AddScoped<INotificationsRepository, NotificationsRepository>();
            services.AddScoped<IWatchLaterRepository, WatchLaterRepository>();

            return services;
        }
    }
}
