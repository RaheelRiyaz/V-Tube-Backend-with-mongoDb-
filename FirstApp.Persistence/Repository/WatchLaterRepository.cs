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
    public class WatchLaterRepository : BaseRepository<WatchLater>, IWatchLaterRepository
    {
        public WatchLaterRepository(IMongoDatabase database) : base(database)
        {
        }

        public async Task<List<WatchLaterDbResponse>> FetchWatchLaterVideos(ObjectId userId, WatchLaterFilterModel model)
        {
            var pipeline = new BsonDocument[]
            {
            new BsonDocument("$match", new BsonDocument("UserId", userId)),
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
                    { "foreignField", "VideoId" },
                    { "localField", "VideoId" },
                    { "as", "Views" }
                }),
            new BsonDocument("$lookup",
                new BsonDocument
                {
                    { "from", "Channels" },
                    { "let", new BsonDocument("channelId", new BsonDocument("$arrayElemAt", new BsonArray { "$Videos.ChannelId", 0 })) },
                    { "pipeline", new BsonArray
                        {
                            new BsonDocument("$match",
                                new BsonDocument("$expr",
                                    new BsonDocument("$eq",
                                        new BsonArray { "$_id", "$$channelId" })))
                        }
                    },
                    { "as", "ChannelInfo" }
                }),
            new BsonDocument("$sort", new BsonDocument("CreatedAt", model.SortOrder)),
            new BsonDocument("$skip", (model.PageNo -1 ) * model.PageSize),
            new BsonDocument("$limit", model.PageSize),
            new BsonDocument("$project",
                new BsonDocument
                {
                    { "_id", 1 },
                    { "VideoId", 1 },
                    { "CreatedAt", 1 },
                    { "Title", new BsonDocument("$arrayElemAt", new BsonArray { "$Videos.Title", 0 }) },
                    { "Thumbnail", new BsonDocument("$arrayElemAt", new BsonArray { "$Videos.Thumbnail", 0 }) },
                    { "Description", new BsonDocument("$arrayElemAt", new BsonArray { "$Videos.Description", 0 }) },
                    { "Url", new BsonDocument("$arrayElemAt", new BsonArray { "$Videos.Url", 0 }) },
                    { "Duration", new BsonDocument("$arrayElemAt", new BsonArray { "$Videos.Duration", 0 }) },
                    { "Views", new BsonDocument("$size", "$Views") },
                    { "ChannelId", new BsonDocument("$arrayElemAt", new BsonArray { "$Videos.ChannelId", 0 }) },
                    { "PlayListId", new BsonDocument("$arrayElemAt", new BsonArray { "$Videos.PlayListId", 0 }) },
                    { "ChannelName", new BsonDocument("$arrayElemAt", new BsonArray { "$ChannelInfo.Name", 0 }) }
                })
            };

            var result = await _collection.Aggregate<WatchLaterDbResponse>(pipeline).ToListAsync();
            return result;
        }


        public async Task<WatchLaterHeader> FetchWatchLaterHeader(WatchLaterHeaderFilter model)
        {
            var pipeline = new BsonDocument[]
        {
              new BsonDocument("$match",
    new BsonDocument("UserId",
    model.UserId)),
    new BsonDocument("$sort",
    new BsonDocument("CreatedAt", model.SortOrder)),
    new BsonDocument("$limit", 1),
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
            { "from", "WatchLater" },
            { "localField", "UserId" },
            { "foreignField", "UserId" },
            { "as", "History" }
        }),
    new BsonDocument("$project",
    new BsonDocument
        {
            { "_id", 0 },
            { "Videos",
    new BsonDocument("$size", "$History") },
            { "CoverUrl",
    new BsonDocument("$arrayElemAt",
    new BsonArray
                {
                    "$Videos.Thumbnail",
                    0
                }) }
        })
        };

            var result = await _collection.AggregateAsync<WatchLaterHeader>(pipeline);

            return await result.FirstOrDefaultAsync();
        }

    }
}
