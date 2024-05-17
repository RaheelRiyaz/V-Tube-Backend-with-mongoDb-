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
    public class CommentRepliesRepository : BaseRepository<CommentReplies>, ICommentRepliesRepository
    {
        public CommentRepliesRepository(IMongoDatabase database) : base(database)
        {
        }

        public async Task<List<ReplyDBResponse>> FetchRepliesByCommentAsync(ReplyFilter model, ObjectId userId)
        {
            var pipeline = new BsonDocument[]
            {
        new BsonDocument("$match",
            new BsonDocument("CommentId", new ObjectId(model.CommentId))),
        new BsonDocument("$lookup",
            new BsonDocument
            {
                { "from", "Likes" },
                { "localField", "_id" },
                { "foreignField", "CommentId" },
                { "as", "Likes" }
            }),
        new BsonDocument("$lookup",
            new BsonDocument
            {
                { "from", "Likes" },
                { "let", new BsonDocument("commentId", "$_id") },
                { "pipeline", new BsonArray
                    {
                        new BsonDocument("$match",
                            new BsonDocument("$expr",
                                new BsonDocument("$and", new BsonArray
                                {
                                    new BsonDocument("$eq", new BsonArray { "$CommentId", "$$commentId" }),
                                    new BsonDocument("$eq", new BsonArray { "$UserId", userId })
                                })))
                    }
                },
                { "as", "UserLike" }
            }),
        new BsonDocument("$lookup",
            new BsonDocument
            {
                { "from", "Users" },
                { "localField", "RepliedBy" },
                { "foreignField", "_id" },
                { "as", "Replier" }
            }),
        new BsonDocument("$project",
            new BsonDocument
            {
                { "_id", 1 },
                { "CommentId", 1 },
                { "Title", 1 },
                { "RepliedBy", 1 },
                { "Replier", new BsonDocument("$arrayElemAt", new BsonArray { "$Replier.UserName", 0 }) },
                { "CreatedAt", 1 },
                { "DisLikes",
                    new BsonDocument("$size",
                        new BsonDocument("$filter",
                            new BsonDocument
                            {
                                { "input", "$Likes" },
                                { "as", "dislike" },
                                { "cond", new BsonDocument("$eq", new BsonArray { "$$dislike.IsLiked", false }) }
                            })) },
                { "HasUserLiked",
                    new BsonDocument("$cond",
                        new BsonDocument
                        {
                            { "if",
                                new BsonDocument("$gt",
                                    new BsonArray
                                    {
                                        new BsonDocument("$size", "$UserLike"),
                                        0
                                    }) },
                            { "then",
                                new BsonDocument("$arrayElemAt",
                                    new BsonArray
                                    {
                                        "$UserLike.IsLiked",
                                        0
                                    }) },
                            { "else", BsonNull.Value }
                        }) },
                { "Likes",
                    new BsonDocument("$size",
                        new BsonDocument("$filter",
                            new BsonDocument
                            {
                                { "input", "$Likes" },
                                { "as", "like" },
                                { "cond", new BsonDocument("$eq", new BsonArray { "$$like.IsLiked", true }) }
                            })) }
            }),
        new BsonDocument("$sort",
            new BsonDocument("CreatedAt", -1)), // Sort by CreatedAt descending
        new BsonDocument("$skip", (model.PageNo - 1) * model.PageSize), // Pagination: skip documents
        new BsonDocument("$limit", model.PageSize) // Pagination: limit number of documents
            };

            var result = await _collection.Aggregate<ReplyDBResponse>(pipeline).ToListAsync();
            return result;
        }


    }
}
