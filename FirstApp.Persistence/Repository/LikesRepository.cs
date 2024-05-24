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
    public class LikesRepository : BaseRepository<Likes>, ILikesRepository
    {
        public LikesRepository(IMongoDatabase database) : base(database)
        {
        }

        public async Task<List<LikedVideoDBResponse>> FetchUserLikedVideos(FilterModel model, ObjectId userId)
        {
            var pipeline = new BsonDocument[]
 {
    new BsonDocument("$match",
    new BsonDocument
        {
            { "LikeType", 1 },
            { "UserId",userId
     },
            { "VideoId",
    new BsonDocument("$ne", BsonNull.Value) }
        }),
    new BsonDocument("$lookup",
    new BsonDocument
        {
            { "from", "Videos" },
            { "localField", "VideoId" },
            { "foreignField", "_id" },
            { "as", "Video" }
        }),
    new BsonDocument("$lookup",
    new BsonDocument
        {
            { "from", "Channels" },
            { "localField", "Video.ChannelId" },
            { "foreignField", "_id" },
            { "as", "Channel" }
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
            { "from", "WatchHistory" },
            { "let",
    new BsonDocument
            {
                { "videoId", "$VideoId" },
                { "userId",
    userId }
            } },
            { "pipeline",
    new BsonArray
            {
                new BsonDocument("$match",
                new BsonDocument("$expr",
                new BsonDocument("$and",
                new BsonArray
                            {
                                new BsonDocument("$eq",
                                new BsonArray
                                    {
                                        "$VideoId",
                                        "$$videoId"
                                    }),
                                new BsonDocument("$eq",
                                new BsonArray
                                    {
                                        "$UserId",
                                        "$$userId"
                                    })
                            })))
            } },
            { "as", "History" }
        }),
    new BsonDocument("$project",
    new BsonDocument
        {
            { "VideoId", 1 },
            { "Title",
    new BsonDocument("$arrayElemAt",
    new BsonArray
                {
                    "$Video.Title",
                    0
                }) },
            { "Description",
    new BsonDocument("$arrayElemAt",
    new BsonArray
                {
                    "$Video.Description",
                    0
                }) },
            { "Url",
    new BsonDocument("$arrayElemAt",
    new BsonArray
                {
                    "$Video.Url",
                    0
                }) },
            { "Thumbnail",
    new BsonDocument("$arrayElemAt",
    new BsonArray
                {
                    "$Video.Thumbnail",
                    0
                }) },
            { "Duration",
    new BsonDocument("$arrayElemAt",
    new BsonArray
                {
                    "$Video.Duration",
                    0
                }) },
            { "Views",
    new BsonDocument("$size", "$Views") },
            { "ChannelName",
    new BsonDocument("$arrayElemAt",
    new BsonArray
                {
                    "$Channel.Name",
                    0
                }) },
            { "CreatedAt",
    new BsonDocument("$arrayElemAt",
    new BsonArray
                {
                    "$Video.CreatedAt",
                    0
                }) },
            { "DurationWatched",
    new BsonDocument("$cond",
    new BsonDocument
                {
                    { "if",
    new BsonDocument("$gt",
    new BsonArray
                        {
                            new BsonDocument("$size", "$History"),
                            0
                        }) },
                    { "then",
    new BsonDocument("$arrayElemAt",
    new BsonArray
                        {
                            "$History.DurationViewed",
                            0
                        }) },
                    { "else", BsonNull.Value }
                }) }
        }),
    new BsonDocument("$skip", ((model.PageNo -1) * model.PageSize)),
    new BsonDocument("$limit", model.PageSize),
    new BsonDocument("$sort",
    new BsonDocument("CreatedAt", model.SortOrder ?? -1))
 };

            var result = await _collection.Aggregate<LikedVideoDBResponse>(pipeline).ToListAsync();

            return result;
        }
    }
}
