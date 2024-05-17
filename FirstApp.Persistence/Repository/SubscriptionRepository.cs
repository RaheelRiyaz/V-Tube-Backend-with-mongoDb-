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
    public class SubscriptionRepository : BaseRepository<Subscriber>, ISubscriptionRepository
    {
        public SubscriptionRepository(IMongoDatabase database) : base(database)
        {
        }

        public async Task<List<UserSubscribedDBResponse>> ViewUserSubscribedChannels(ObjectId userId, FilterModel model)
        {
            var pipeline = new BsonDocument[]
{
                    new BsonDocument("$match",
                    new BsonDocument("SubscribedBy",
                    userId)),
                    new BsonDocument("$lookup",
                    new BsonDocument
                        {
                            { "from", "Channels" },
                            { "localField", "ChannelId" },
                            { "foreignField", "_id" },
                            { "as", "Channel" }
                        }),
                    new BsonDocument("$project",
                    new BsonDocument
                        {
                            { "HasNotified", true },
                            { "ChannelId", true },
                            { "Name",
                    new BsonDocument("$arrayElemAt",
                    new BsonArray
                                {
                                    "$Channel.Name",
                                    0
                                }) },
                            { "Owner",
                    new BsonDocument("$arrayElemAt",
                    new BsonArray
                                {
                                    "$Channel.Owner",
                                    0
                                }) },
                            { "Handle",
                    new BsonDocument("$arrayElemAt",
                    new BsonArray
                                {
                                    "$Channel.Handle",
                                    0
                                }) },
                            { "Description",
                    new BsonDocument("$arrayElemAt",
                    new BsonArray
                                {
                                    "$Channel.Description",
                                    0
                                }) },
                            { "CoverUrl",
                    new BsonDocument("$arrayElemAt",
                    new BsonArray
                                {
                                    "$Channel.CoverUrl",
                                    0
                                }) },
                            { "SubscribedAt", "$CreatedAt" }
                        }),
                    new BsonDocument("$sort",
                    new BsonDocument("CreatedAt", model.SortOrder)),
                     new BsonDocument("$skip", (model.PageNo - 1) * model.PageSize),
                        new BsonDocument("$limit", model.PageSize)
};


            var result = await _collection.Aggregate<UserSubscribedDBResponse>(pipeline).ToListAsync();
            return result;
        }
    }
}
