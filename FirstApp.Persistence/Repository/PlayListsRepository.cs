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
    public class PlayListsRepository : BaseRepository<PlayList>, IPlayListsRepository
    {
        public PlayListsRepository(IMongoDatabase database) : base(database)
        {
        }

        public async Task<List<PlayListResponse>> ViewPlayLists(ObjectId channelId)
        {
            var pipeline = new BsonDocument[]
{
                            new BsonDocument("$match", new BsonDocument("ChannelId", channelId)),
                            new BsonDocument("$lookup", new BsonDocument
                            {
                                { "from", "Videos" },
                                { "localField", "_id" },
                                { "foreignField", "PlayListId" },
                                { "as", "videos" }
                            }),
                            new BsonDocument("$unwind", "$videos"),
                            new BsonDocument("$sort", new BsonDocument("videos.CreatedAt", -1)),
                            new BsonDocument("$group", new BsonDocument
                            {
                                { "_id", "$_id" },
                                { "Name", new BsonDocument("$first", "$Name") },
                                { "Description", new BsonDocument("$first", "$Description") },
                                { "ChannelId", new BsonDocument("$first", "$ChannelId") },
                                { "videos", new BsonDocument("$push", "$videos") }
                            }),
                            new BsonDocument("$addFields", new BsonDocument
                            {
                                { "CoverUrl", new BsonDocument("$arrayElemAt", new BsonArray { "$videos.Thumbnail", 0 }) }
                            }),
                            new BsonDocument("$project", new BsonDocument
                            {
                                { "_id", 1 },
                                { "Name", 1 },
                                { "Description", 1 },
                                { "ChannelId", 1 },
                                { "VideoCount", new BsonDocument("$size", "$videos") },
                                { "CoverUrl", 1 }
                            })
};

            var result = await _collection.AggregateAsync<PlayListResponse>(pipeline);
            return result.ToList();
        }
    }
}
