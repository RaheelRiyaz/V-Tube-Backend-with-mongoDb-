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
    public class OwnerReportsRepository : BaseRepository<OwnerReports>, IOwnerReportsRepository
    {
        public OwnerReportsRepository(IMongoDatabase database) : base(database)
        {
        }

        public async Task<List<OwnerReportDBResponse>> GetOwnerReportsAsync(ObjectId ownerId)
        {

            var pipeline = new BsonDocument[]
        {
            new BsonDocument("$match", new BsonDocument("OwnerId", ownerId)),
            new BsonDocument("$lookup",
                new BsonDocument
                {
                    { "from", "Videos" },
                    { "localField", "EntityId" },
                    { "foreignField", "_id" },
                    { "as", "Videos" }
                }),
            new BsonDocument("$lookup",
                new BsonDocument
                {
                    { "from", "Comments" },
                    { "localField", "EntityId" },
                    { "foreignField", "_id" },
                    { "as", "Comments" }
                }),
            new BsonDocument("$lookup",
                new BsonDocument
                {
                    { "from", "CommentReplies" },
                    { "localField", "EntityId" },
                    { "foreignField", "_id" },
                    { "as", "Replies" }
                }),
            new BsonDocument("$project",
                new BsonDocument
                {
                    { "_id", 1 },
                    { "CreatedAt", 1 },
                    { "UpdatedAt", 1 },
                    { "EntityId", 1 },
                    { "ReportType", "$ReportType" },
                    { "OwnerId", 1 },
                    { "ReportCount", 1 },
                    { "IsTemporarilyRemoved", 1 },
                    { "TemporarilyRemovedAt", 1 },
                    { "IsPermanentlyRemoved", 1 },
                    { "PermanentlyRemovedAt", 1 },
                    { "IsRestored", 1 },
                    { "RestoredAt", 1 },
                    { "ReplyTitle",
                        new BsonDocument("$cond",
                            new BsonDocument
                            {
                                { "if",
                                    new BsonDocument("$gt",
                                        new BsonArray
                                        {
                                            new BsonDocument("$size", "$Replies"),
                                            0
                                        }) },
                                { "then",
                                    new BsonDocument("$arrayElemAt",
                                        new BsonArray
                                        {
                                            "$Replies.Title",
                                            0
                                        }) },
                                { "else", BsonNull.Value }
                            }) },
                    { "VideoTitle",
                        new BsonDocument("$arrayElemAt",
                            new BsonArray
                            {
                                "$Videos.Title",
                                0
                            }) },
                    { "CommentTitle",
                        new BsonDocument("$cond",
                            new BsonDocument
                            {
                                { "if",
                                    new BsonDocument("$gt",
                                        new BsonArray
                                        {
                                            new BsonDocument("$size", "$Comments"),
                                            0
                                        }) },
                                { "then",
                                    new BsonDocument("$arrayElemAt",
                                        new BsonArray
                                        {
                                            "$Comments.Title",
                                            0
                                        }) },
                                { "else", BsonNull.Value }
                            })
                    }
                })
        };

            var aggregation = _collection.Aggregate<OwnerReportDBResponse>(pipeline);

            return await aggregation.ToListAsync();
        }
    }
}
