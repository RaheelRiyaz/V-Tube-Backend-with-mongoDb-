using CloudinaryDotNet.Actions;
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
    public class CommentsRepository : BaseRepository<Comment>, ICommentsRepository
    {
        public CommentsRepository(IMongoDatabase database) : base(database)
        {
        }


        public async Task<List<CommentDBResponse>> FetchCommentsByVideoIdAsync(CommentsFilter model, ObjectId userId)
        {
            var pipeline = new BsonDocument[]
 {
     new BsonDocument("$match",
        new BsonDocument
        {
            { "IsDeleted", false },
            { "VideoId", new ObjectId(model.VideoId) }
        }),
    new BsonDocument("$lookup",
    new BsonDocument
        {
            { "from", "Users" },
            { "localField", "CommentedBy" },
            { "foreignField", "_id" },
            { "as", "User" }
        }),
    new BsonDocument("$unwind", "$User"),
    new BsonDocument("$lookup",
    new BsonDocument
        {
            { "from", "CommentReplies" },
            { "localField", "_id" },
            { "foreignField", "CommentId" },
            { "as", "Replies" }
        }),
    new BsonDocument("$lookup",
    new BsonDocument
        {
            { "from", "Likes" },
            { "let",
    new BsonDocument
            {
                { "commentId", "$_id" },
                { "userId", userId }
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
                                        "$CommentId",
                                        "$$commentId"
                                    }),
                                new BsonDocument("$eq",
                                new BsonArray
                                    {
                                        "$UserId",
                                        "$$userId"
                                    })
                            })))
            } },
            { "as", "CommentLikes" }
        }),
    new BsonDocument("$lookup",
    new BsonDocument
        {
            { "from", "Likes" },
            { "localField", "_id" },
            { "foreignField", "CommentId" },
            { "as", "Likes" }
        }),
    new BsonDocument("$addFields",
    new BsonDocument("HasUserLiked",
    new BsonDocument("$cond",
    new BsonDocument
                {
                    { "if",
    new BsonDocument("$gt",
    new BsonArray
                        {
                            new BsonDocument("$size", "$CommentLikes"),
                            0
                        }) },
                    { "then",
    new BsonDocument("$arrayElemAt",
    new BsonArray
                        {
                            "$CommentLikes.IsLiked",
                            0
                        }) },
                    { "else", BsonNull.Value }
                }))),
    new BsonDocument("$project",
    new BsonDocument
        {
            { "_id", 1 },
            { "Title", 1 },
            { "CommentedBy", 1 },
            { "CreatedAt", 1 },
            { "TotalReplies",
    new BsonDocument("$size", "$Replies") },
            { "Commenter", "$User.UserName" },
            { "HasUserLiked", 1 },
            { "Likes",
    new BsonDocument("$size",
    new BsonDocument("$filter",
    new BsonDocument
                    {
                        { "input", "$Likes" },
                        { "cond",
    new BsonDocument("$and",
    new BsonArray
                            {
                                new BsonDocument("$eq",
                                new BsonArray
                                    {
                                        "$$this.CommentId",
                                        "$_id"
                                    }),
                                new BsonDocument("$eq",
                                new BsonArray
                                    {
                                        "$$this.IsLiked",
                                        true
                                    })
                            }) }
                    })) },
            { "Dislikes",
    new BsonDocument("$size",
    new BsonDocument("$filter",
    new BsonDocument
                    {
                        { "input", "$Likes" },
                        { "cond",
    new BsonDocument("$and",
    new BsonArray
                            {
                                new BsonDocument("$eq",
                                new BsonArray
                                    {
                                        "$$this.CommentId",
                                        "$_id"
                                    }),
                                new BsonDocument("$eq",
                                new BsonArray
                                    {
                                        "$$this.IsLiked",
                                        false
                                    })
                            }) }
                    })) }
        })
   ,
    new BsonDocument("$skip", (model.PageNo - 1) * model.PageSize),
    new BsonDocument("$limit", model.PageSize)
 };


            var result = await _collection.Aggregate<CommentDBResponse>(pipeline).ToListAsync();
            return result;
        }





    }
}
