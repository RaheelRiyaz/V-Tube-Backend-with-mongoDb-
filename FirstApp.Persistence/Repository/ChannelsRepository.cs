using FirstApp.Application.Abstractions.IRepositories;
using FirstApp.Application.DTOS;
using FirstApp.Domain.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstApp.Persistence.Repository
{
    public class ChannelsRepository : BaseRepository<Channel>, IChannelsRepository
    {
        public ChannelsRepository(IMongoDatabase database) : base(database)
        {
        }


        public async Task<ChannelDBResponse> ViewChannel(ObjectId channelId, ObjectId userId)
        {
            var matchStage = new BsonDocument("$match", new BsonDocument("_id", channelId));

            var lookupVideosStage = new BsonDocument("$lookup",
                new BsonDocument
                {
            { "from", "Videos" },
            { "localField", "_id" },
            { "foreignField", "ChannelId" },
            { "as", "Videos" }
                });

            var lookupSubscribersStage = new BsonDocument("$lookup",
                new BsonDocument
                {
            { "from", "Subscribers" },
            { "localField", "_id" },
            { "foreignField", "ChannelId" },
            { "as", "Subscribers" }
                });

            var lookupPlayListsStage = new BsonDocument("$lookup",
                new BsonDocument
                {
            { "from", "PlayLists" },
            { "localField", "_id" },
            { "foreignField", "ChannelId" },
            { "as", "PlayLists" }
                });

            var userSubscribersPipeline = new BsonArray
    {
        new BsonDocument("$match",
            new BsonDocument("$expr",
                new BsonDocument("$and",
                    new BsonArray
                    {
                        new BsonDocument("$eq",
                            new BsonArray
                            {
                                "$ChannelId",
                                channelId
                            }),
                        new BsonDocument("$eq",
                            new BsonArray
                            {
                                "$SubscribedBy",
                                userId
                            })
                    })))
    };

            var lookupUserSubscribersStage = new BsonDocument("$lookup",
                new BsonDocument
                {
            { "from", "Subscribers" },
            { "pipeline", userSubscribersPipeline },
            { "as", "UserSubscribers" }
                });

            var projectStage = new BsonDocument("$project",
                new BsonDocument
                {
            { "_id", true },
            { "Name", true },
            { "Owner", true },
            { "CoverUrl", true },
            { "Handle", true },
            { "Description", true },
            { "PlayLists", new BsonDocument("$size", "$PlayLists") },
            { "Videos", new BsonDocument("$size", "$Videos") },
            { "Subscribers", new BsonDocument("$size", "$Subscribers") },
            { "HasUserSubscribed", new BsonDocument("$cond",
                new BsonDocument
                {
                    { "if", new BsonDocument("$gt", new BsonArray { new BsonDocument("$size", "$UserSubscribers"), 0 }) },
                    { "then", true },
                    { "else", false }
                }) },
            { "HasNotified", new BsonDocument("$cond",
                new BsonDocument
                {
                    { "if", new BsonDocument("$gt", new BsonArray { new BsonDocument("$size", "$UserSubscribers"), 0 }) },
                    { "then", new BsonDocument("$arrayElemAt", new BsonArray { "$UserSubscribers.HasNotified", 0 }) },
                    { "else", BsonNull.Value }
                }) }
                });

            var pipeline = new[] { matchStage, lookupVideosStage, lookupSubscribersStage, lookupPlayListsStage, lookupUserSubscribersStage, projectStage };

            var result = await _collection.Aggregate<ChannelDBResponse>(pipeline).FirstOrDefaultAsync();

            return result;
        }
    }
}
