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
    public class ViewsRepository : BaseRepository<Views>, IViewsRepository
    {
        public ViewsRepository(IMongoDatabase database) : base(database)
        {
        }

        public async Task<List<ViewDBResponse>> FetchVideoViews(CommentsFilter model)
        {
            var pipeline = new BsonDocument[]
            {
            new BsonDocument("$match", new BsonDocument("VideoId", new ObjectId(model.VideoId))),
            new BsonDocument("$lookup",
                new BsonDocument
                {
                    { "from", "Users" },
                    { "foreignField", "_id" },
                    { "localField", "ViewedBy" },
                    { "as", "User" }
                }),
            new BsonDocument("$project",
                new BsonDocument
                {
                    { "_id", true },
                    { "ViewedAt", "$CreatedAt" },
                    { "ViewedBy", true },
                    { "Viewer",
                        new BsonDocument("$arrayElemAt",
                            new BsonArray
                            {
                                "$User.UserName",
                                0
                            }) },
                    { "VideoId", true },
                    { "DurationViewed", true }
                }),
            new BsonDocument("$sort", new BsonDocument("CreatedAt", -1)), 
            new BsonDocument("$skip", (model.PageNo - 1) * model.PageSize),
            new BsonDocument("$limit", model.PageSize) 
            };

            var result = await _collection.Aggregate<ViewDBResponse>(pipeline).ToListAsync();
            return result.ToList();
        }
    }
}
