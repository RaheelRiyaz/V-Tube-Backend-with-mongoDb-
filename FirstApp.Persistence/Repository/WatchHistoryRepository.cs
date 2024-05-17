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
    public class WatchHistoryRepository : BaseRepository<WatchHistory>, IWatchHistoryRepository
    {
        public WatchHistoryRepository(IMongoDatabase database) : base(database)
        {
        }

        public async Task<List<HistoryDBResponse>> FetchUserHistory(FilterModel model, ObjectId userId)
        {
            var pipeline = new BsonDocument[]
       {
             new BsonDocument("$match",
    new BsonDocument("UserId",
    new ObjectId("6639bb42c92b6748cb77c6fa"))),
    new BsonDocument("$lookup",
    new BsonDocument
        {
            { "from", "Videos" },
            { "localField", "VideoId" },
            { "foreignField", "_id" },
            { "as", "Videos" }
        }),
    new BsonDocument("$lookup",
    new BsonDocument
        {
            { "from", "Views" },
            { "localField", "VideoId" },
            { "foreignField", "VideoId" },
            { "as", "Views" }
        }),
    new BsonDocument("$lookup",
    new BsonDocument
        {
            { "from", "Channels" },
            { "let",
    new BsonDocument("channelId",
    new BsonDocument("$arrayElemAt",
    new BsonArray
                    {
                        "$Videos.ChannelId",
                        0
                    })) },
            { "pipeline",
    new BsonArray
            {
                new BsonDocument("$match",
                new BsonDocument("$expr",
                new BsonDocument("$eq",
                new BsonArray
                            {
                                "$_id",
                                "$$channelId"
                            })))
            } },
            { "as", "Channel" }
        }),
    new BsonDocument("$project",
    new BsonDocument
        {
            { "VideoId", 1 },
            { "ChannelId",
    new BsonDocument("$arrayElemAt",
    new BsonArray
                {
                    "$Channel._id",
                    0
                }) },
            { "PlayListId",
    new BsonDocument("$arrayElemAt",
    new BsonArray
                {
                    "$Videos.PlayListId",
                    0
                }) },
            { "Title",
    new BsonDocument("$arrayElemAt",
    new BsonArray
                {
                    "$Videos.Title",
                    0
                }) },
            { "Description",
    new BsonDocument("$arrayElemAt",
    new BsonArray
                {
                    "$Videos.Description",
                    0
                }) },
            { "DurationViewed", 1 },
            { "TotalDuration",
    new BsonDocument("$arrayElemAt",
    new BsonArray
                {
                    "$Videos.Duration",
                    0
                }) },
            { "Thumbnail",
    new BsonDocument("$arrayElemAt",
    new BsonArray
                {
                    "$Videos.Thumbnail",
                    0
                }) },
            { "ChannelName",
    new BsonDocument("$arrayElemAt",
    new BsonArray
                {
                    "$Channel.Name",
                    0
                }) },
             { "ChannelCover",
    new BsonDocument("$arrayElemAt",
    new BsonArray
                {
                    "$Channel.CoverUrl",
                    0
                }) },
            { "Url",
    new BsonDocument("$arrayElemAt",
    new BsonArray
                {
                    "$Videos.Url",
                    0
                }) },
            { "Views",
    new BsonDocument("$size", "$Views") },
            { "CreatedAt", 1 }
        }),
    new BsonDocument("$sort",
    new BsonDocument("CreatedAt", model.SortOrder)),
            new BsonDocument("$skip", (model.PageNo - 1) * model.PageSize),
            new BsonDocument("$limit", model.PageSize)
       };

            var result = await _collection.Aggregate<HistoryDBResponse>(pipeline).ToListAsync();
            return result;
        }

        public async Task<long> DeleteWatchHistoryByUserId(ObjectId userId)
        {
            var filter = Builders<WatchHistory>.Filter.Eq("UserId", userId);

            var deleteResult = await _collection.DeleteManyAsync(filter);

            return deleteResult.DeletedCount;
        }
    }
}
