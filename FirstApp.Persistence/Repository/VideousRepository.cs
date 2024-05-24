using FirstApp.Application.Abstractions.IRepositories;
using FirstApp.Application.APIResponse;
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
    public class VideousRepository : BaseRepository<Video>, IVideousRepository
    {
        public VideousRepository(IMongoDatabase database) : base(database)
        {
        }


        public async Task<List<VideoDBResponse>> FetchVideos(string searchTerm)
        {
            var pipeline = new[]
            {
        BsonDocument.Parse("{ $match: { $or: [ { Title: { $regex: '" + searchTerm + "', $options: 'i' } }, { Description: { $regex: '" + searchTerm + "', $options: 'i' } } ] } }"),
        BsonDocument.Parse("{ $lookup: { from: 'Views', localField: '_id', foreignField: 'VideoId', as: 'views' } }"),
        BsonDocument.Parse("{ $lookup: { from: 'Comments', localField: '_id', foreignField: 'VideoId', as: 'comments' } }"),
        BsonDocument.Parse("{ $addFields: { TotalViews: { $size: '$views' }, TotalComments: { $size: '$comments' } } }"),
        BsonDocument.Parse("{ $lookup: { from: 'CommentReplies', localField: 'comments._id', foreignField: 'CommentId', as: 'replies' } }"),
        BsonDocument.Parse("{ $addFields: { TotalReplies: { $size: '$replies' } } }"),
        BsonDocument.Parse("{ $lookup: { from: 'Likes', localField: '_id', foreignField: 'VideoId', as: 'userLikes' } }"),
        BsonDocument.Parse("{ $match: { 'userLikes.UserId': ObjectId('6639bb42c92b6748cb77c6fa') } }"),
        BsonDocument.Parse("{ $addFields: { HasLiked: { $gt: [ { $size: '$userLikes' }, 0 ] } } }"),
        BsonDocument.Parse("{ $project: { _id: 0, Title: 1, Description: 1, Thumbnail: 1, Url: 1, TotalViews: 1, TotalComments: 1, TotalReplies: 1, HasLiked: 1 } }")
    };

            var aggregateOptions = new AggregateOptions { AllowDiskUse = true };

            var cursor = await _collection.AggregateAsync<VideoDBResponse>(pipeline, aggregateOptions);

            var result = await cursor.ToListAsync();
            return result;
        }



        public async Task<List<string>> GetVideoSearchSuggestions(string searchTerm)
        {
            var filter = Builders<Video>.Filter.Or(
                Builders<Video>.Filter.Regex("Title", new BsonRegularExpression(searchTerm, "i")),
                Builders<Video>.Filter.Regex("Description", new BsonRegularExpression(searchTerm, "i"))
            );

            var projection = Builders<Video>.Projection.Include(doc => doc.Title);

            var cursor = await _collection.Find(filter).Project(projection).Limit(20).ToListAsync();

            return cursor.Select(doc => doc.GetValue("Title").AsString).ToList();
        }



        /* public async Task<List<DBVideoModel>> FetchVideosByChannel(VideoFilter model, ObjectId userId)
         {
             var pipeline = new List<BsonDocument>();
             var channelId = ObjectId.Empty;
             var playListId = ObjectId.Empty;

             string searchTerm = model.Term ?? "";

             var isChannelIdValid = ObjectId.TryParse(model.ChannelId, out channelId);
             var isPlayListIdValid = ObjectId.TryParse(model.PlayListId, out playListId);

             if ((!isPlayListIdValid && !isChannelIdValid && string.IsNullOrWhiteSpace(searchTerm)))
                 return new List<DBVideoModel>();

             var pageNumber = model.PageNumber;
             var pageSize = model.PageSize;

             if(playListId == ObjectId.Empty && channelId == ObjectId.Empty && !string.IsNullOrWhiteSpace(searchTerm))
             {
                 var textSearchStage = new BsonDocument("$match", new BsonDocument
                    {
                     { "$text", new BsonDocument { { "$search", searchTerm } } },
                     { "IsDeleted", false }
                    });

                 pipeline.Add(textSearchStage);
             }

             else if(playListId == ObjectId.Empty && channelId != ObjectId.Empty && string.IsNullOrWhiteSpace(searchTerm))
             {
                 var channelMatchStage = new BsonDocument("$match", new BsonDocument
                    {
                     { "ChannelId", channelId },
                     { "IsDeleted", false }
                    });

                 pipeline.Add(channelMatchStage);
             }

             else if(playListId != ObjectId.Empty && channelId == ObjectId.Empty && string.IsNullOrWhiteSpace(searchTerm))
             {
                 var playListMatchStage = new BsonDocument("$match", new BsonDocument
                    {
                     { "PlayListId", playListId },
                     { "IsDeleted", false }
                    });

                 pipeline.Add(playListMatchStage);
             }

             else if (playListId != ObjectId.Empty && channelId != ObjectId.Empty && string.IsNullOrWhiteSpace(searchTerm))
             {
                 var playListMatchStage = new BsonDocument("$match", new BsonDocument
                    {
                     { "ChannelId", channelId },
                     { "PlayListId", playListId },
                     { "IsDeleted", false }
                    });

                 pipeline.Add(playListMatchStage);
             }

             else if (playListId != ObjectId.Empty && channelId != ObjectId.Empty && !string.IsNullOrWhiteSpace(searchTerm))
             {
                 var playListMatchStage = new BsonDocument("$match", new BsonDocument
                    {
                     { "ChannelId", channelId },
                     { "PlayListId", playListId },
                     { "IsDeleted", false },
                     { "$text", new BsonDocument { { "$search", searchTerm } } }
                    });

                 pipeline.Add(playListMatchStage);
             }

             else if (playListId != ObjectId.Empty && channelId == ObjectId.Empty && !string.IsNullOrWhiteSpace(searchTerm))
             {
                 var playListMatchStage = new BsonDocument("$match", new BsonDocument
                    {
                     { "PlayListId", playListId },
                     { "IsDeleted", false },
                     { "$text", new BsonDocument { { "$search", searchTerm } } }
                    });

                 pipeline.Add(playListMatchStage);
             }

             else if (playListId == ObjectId.Empty && channelId != ObjectId.Empty && !string.IsNullOrWhiteSpace(searchTerm))
             {
                 var playListMatchStage = new BsonDocument("$match", new BsonDocument
                    {
                     { "ChannelId", channelId },
                     { "IsDeleted", false },
                     { "$text", new BsonDocument { { "$search", searchTerm } } }
                    });

                 pipeline.Add(playListMatchStage);
             }





             pipeline.Add(new BsonDocument("$lookup", new BsonDocument
     {
         { "from", "Channels" },
         { "localField", "ChannelId" },
         { "foreignField", "_id" },
         { "as", "channel" }
     }));

             pipeline.Add(new BsonDocument("$unwind", "$channel"));

             pipeline.Add(new BsonDocument("$lookup", new BsonDocument
     {
         { "from", "Likes" },
         { "localField", "_id" },
         { "foreignField", "VideoId" },
         { "as", "likes" }
     }));

             pipeline.Add(new BsonDocument("$lookup", new BsonDocument
     {
         { "from", "Views" },
         { "localField", "_id" },
         { "foreignField", "VideoId" },
         { "as", "views" }
     }));

             pipeline.Add(new BsonDocument("$lookup", new BsonDocument
     {
         { "from", "Comments" },
         { "localField", "_id" },
         { "foreignField", "VideoId" },
         { "as", "comments" }
     }));

             pipeline.Add(new BsonDocument("$lookup", new BsonDocument
     {
         { "from", "CommentReplies" },
         { "localField", "comments._id" },
         { "foreignField", "CommentId" },
         { "as", "commentReplies" }
     }));

             pipeline.Add(new BsonDocument("$lookup", new BsonDocument
     {
         { "from", "Likes" },
         { "let", new BsonDocument("videoId", "$_id") },
         { "pipeline", new BsonArray
             {
                 new BsonDocument("$match", new BsonDocument
                 {
                     { "$expr", new BsonDocument("$and", new BsonArray
                         {
                             new BsonDocument("$eq", new BsonArray { "$VideoId", "$$videoId" }), // Match VideoId
                             new BsonDocument("$eq", new BsonArray { "$UserId", userId }) // Match UserId
                         })
                     }
                 })
             }
         },
         { "as", "userLikes" }
     }));

             pipeline.Add(new BsonDocument("$lookup", new BsonDocument
     {
         { "from", "WatchHistory" },
         { "let", new BsonDocument("videoId", "$_id") },
         { "pipeline", new BsonArray
             {
                 new BsonDocument("$match", new BsonDocument
                 {
                     { "$expr", new BsonDocument("$and", new BsonArray
                         {
                             new BsonDocument("$eq", new BsonArray { "$VideoId", "$$videoId" }), // Match VideoId
                             new BsonDocument("$eq", new BsonArray { "$UserId", userId }) // Match UserId
                         })
                     }
                 })
             }
         },
         { "as", "userWatchHistory" }
     }));


             pipeline.Add(new BsonDocument("$project", new BsonDocument
     {
         { "_id", 1 },
         { "ChannelId", 1 },
         { "Thumbnail", 1 },
         { "Url", 1 },
         { "CreatedAt", 1 },
         { "ChannelName", "$channel.Name" },
         { "CoverUrl", "$channel.CoverUrl" },
         { "Title", 1 },
         { "Description", 1 },
         { "Duration", 1 },
         { "PlayListId", 1 },
         { "TotalLikes", new BsonDocument("$size", "$likes") },
         { "TotalViews", new BsonDocument("$size", "$views") },
         { "TotalComments", new BsonDocument("$size", "$comments") },
         { "TotalReplies", new BsonDocument("$size", "$commentReplies") },
         { "TotalCommentsAndReplies", new BsonDocument("$add", new BsonArray { new BsonDocument("$size", "$comments"), new BsonDocument("$size", "$commentReplies") }) },
         { "TotalDislikes", new BsonDocument("$size", new BsonDocument("$filter", new BsonDocument
             {
                 { "input", "$likes" },
                 { "as", "like" },
                 { "cond", new BsonDocument("$eq", new BsonArray { "$$like.IsLiked", false }) }
             }))
         },
         {
             "HasUserLiked", new BsonDocument("$cond", new BsonArray
             {
                 new BsonDocument("$gt", new BsonArray { new BsonDocument("$size", "$userLikes"), 0 }), // Check if userLikes array has elements
                 new BsonDocument("$arrayElemAt", new BsonArray { "$userLikes.IsLiked", 0 }), // If yes, return the value of IsLiked
                 BsonNull.Value // If not, return null
             })
         },
         {
             "DurationInHistory", new BsonDocument("$cond", new BsonArray
             {
                 new BsonDocument("$gt", new BsonArray { new BsonDocument("$size", "$userWatchHistory"), 0 }), // Check if userWatchHistory array has elements
                 new BsonDocument("$arrayElemAt", new BsonArray { "$userWatchHistory.DurationViewed", 0 }), // If yes, return the value of DurationViewed
                 BsonNull.Value // If not, return null
             })
         }
     }));

             // Pagination: Skip records and limit to the page size
             pipeline.Add(new BsonDocument("$skip", (pageNumber - 1) * pageSize));
             pipeline.Add(new BsonDocument("$limit", pageSize));
             pipeline.Add(new BsonDocument("$sort", new BsonDocument("CreatedAt",model.SortOrder ?? 1)));

             var aggregateOptions = new AggregateOptions { AllowDiskUse = true };

             // Execute the aggregation pipeline
             var cursor = await _collection.AggregateAsync<DBVideoModel>(pipeline, aggregateOptions);

             var result = await cursor.ToListAsync();
             return result;
         }
 */



        public async Task<List<DBVideoModel>> FetchVideosByChannel(VideoFilter model, ObjectId userId)
        {
            var pipeline = new List<BsonDocument>();
            var channelId = ObjectId.Empty;
            var playListId = ObjectId.Empty;

            string searchTerm = model.Term ?? "";

            var isChannelIdValid = ObjectId.TryParse(model.ChannelId, out channelId);
            var isPlayListIdValid = ObjectId.TryParse(model.PlayListId, out playListId);



            var pageNumber = model.PageNumber;
            var pageSize = model.PageSize;

            if ((playListId == ObjectId.Empty && channelId == ObjectId.Empty && string.IsNullOrWhiteSpace(searchTerm)))
            {
                var emptyStage = new BsonDocument("$match", new BsonDocument());
                pipeline.Add(emptyStage);
            }

            else if (playListId == ObjectId.Empty && channelId == ObjectId.Empty && !string.IsNullOrWhiteSpace(searchTerm))
            {
                var textSearchStage = new BsonDocument("$match", new BsonDocument
                   {
                    { "$text", new BsonDocument { { "$search", searchTerm } } },
                    { "IsDeleted", false }
                   });

                pipeline.Add(textSearchStage);
            }

            else if (playListId == ObjectId.Empty && channelId != ObjectId.Empty && string.IsNullOrWhiteSpace(searchTerm))
            {
                var channelMatchStage = new BsonDocument("$match", new BsonDocument
                   {
                    { "ChannelId", channelId },
                    { "IsDeleted", false }
                   });

                pipeline.Add(channelMatchStage);
            }

            else if (playListId != ObjectId.Empty && channelId == ObjectId.Empty && string.IsNullOrWhiteSpace(searchTerm))
            {
                var playListMatchStage = new BsonDocument("$match", new BsonDocument
                   {
                    { "PlayListId", playListId },
                    { "IsDeleted", false }
                   });

                pipeline.Add(playListMatchStage);
            }

            else if (playListId != ObjectId.Empty && channelId != ObjectId.Empty && string.IsNullOrWhiteSpace(searchTerm))
            {
                var playListMatchStage = new BsonDocument("$match", new BsonDocument
                   {
                    { "ChannelId", channelId },
                    { "PlayListId", playListId },
                    { "IsDeleted", false }
                   });

                pipeline.Add(playListMatchStage);
            }

            else if (playListId != ObjectId.Empty && channelId != ObjectId.Empty && !string.IsNullOrWhiteSpace(searchTerm))
            {
                var playListMatchStage = new BsonDocument("$match", new BsonDocument
                   {
                    { "ChannelId", channelId },
                    { "PlayListId", playListId },
                    { "IsDeleted", false },
                    { "$text", new BsonDocument { { "$search", searchTerm } } }
                   });

                pipeline.Add(playListMatchStage);
            }

            else if (playListId != ObjectId.Empty && channelId == ObjectId.Empty && !string.IsNullOrWhiteSpace(searchTerm))
            {
                var playListMatchStage = new BsonDocument("$match", new BsonDocument
                   {
                    { "PlayListId", playListId },
                    { "IsDeleted", false },
                    { "$text", new BsonDocument { { "$search", searchTerm } } }
                   });

                pipeline.Add(playListMatchStage);
            }

            else if (playListId == ObjectId.Empty && channelId != ObjectId.Empty && !string.IsNullOrWhiteSpace(searchTerm))
            {
                var playListMatchStage = new BsonDocument("$match", new BsonDocument
                   {
                    { "ChannelId", channelId },
                    { "IsDeleted", false },
                    { "$text", new BsonDocument { { "$search", searchTerm } } }
                   });

                pipeline.Add(playListMatchStage);
            }





            pipeline.Add(new BsonDocument("$lookup",
      new BsonDocument
          {
            { "from", "Channels" },
            { "localField", "ChannelId" },
            { "foreignField", "_id" },
            { "as", "Channel" }
          }));

            pipeline.Add(
      new BsonDocument("$lookup",
      new BsonDocument
          {
            { "from", "PlayLists" },
            { "localField", "PlayListId" },
            { "foreignField", "_id" },
            { "as", "PlayList" }
          }));

            pipeline.Add(
      new BsonDocument("$lookup",
    new BsonDocument
        {
            { "from", "Comments" },
            { "let",
    new BsonDocument("videoId", "$_id") },
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
                                        "$IsDeleted",
                                        false
                                    })
                            })))
            } },
            { "as", "Comments" }
        }));

            pipeline.Add(
      new BsonDocument("$lookup",
      new BsonDocument
          {
            { "from", "CommentReplies" },
            { "localField", "Comments._id" },
            { "foreignField", "CommentId" },
            { "as", "Replies" }
          }));

            pipeline.Add(
      new BsonDocument("$lookup",
      new BsonDocument
          {
            { "from", "Users" },
            { "let",
    new BsonDocument("userId",
    userId) },
            { "pipeline",
    new BsonArray
            {
                new BsonDocument("$match",
                new BsonDocument("$expr",
                new BsonDocument("$eq",
                new BsonArray
                            {
                                "$_id",
                                "$$userId"
                            })))
            } },
            { "as", "User" }
          }));
            pipeline.Add(
      new BsonDocument("$lookup",
      new BsonDocument
          {
            { "from", "Likes" },
            { "localField", "_id" },
            { "foreignField", "VideoId" },
            { "as", "TotalLikes" }
          }));

            pipeline.Add(
      new BsonDocument("$lookup",
      new BsonDocument
          {
            { "from", "Likes" },
            { "let",
    new BsonDocument
            {
                { "videoId", "$_id" },
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
            { "as", "UserLike" }
          }));
            pipeline.Add(
      new BsonDocument("$lookup",
      new BsonDocument
          {
            { "from", "WatchHistory" },
            { "localField", "_id" },
            { "foreignField", "VideoId" },
            { "as", "History" }
          }));

            pipeline.Add(
      new BsonDocument("$lookup",
      new BsonDocument
          {
            { "from", "Views" },
            { "localField", "_id" },
            { "foreignField", "VideoId" },
            { "as", "Views" }
          }));

            pipeline.Add(
      new BsonDocument("$project",
      new BsonDocument
          {
            { "IsOwner",
    new BsonDocument("$cond",
    new BsonDocument
                {
                    { "if",
    new BsonDocument("$eq",
    new BsonArray
                        {
                            new BsonDocument("$arrayElemAt",
                            new BsonArray
                                {
                                    "$Channel.Owner",
                                    0
                                }),
                            userId
                        }) },
                    { "then", true },
                    { "else", false }
                }) },
            { "Title", 1 },
            { "CommentsTurnedOff", 1 },
            { "TotalViews",
    new BsonDocument("$size", "$Views") },
            { "Thumbnail", 1 },
            { "Description", 1 },
            { "TotalReplies",
    new BsonDocument("$size", "$Replies") },
            { "TotalCommentsAndReplies",
    new BsonDocument("$add",
    new BsonArray
                {
                    new BsonDocument("$size", "$Comments"),
                    new BsonDocument("$size", "$Replies")
                }) },
            { "DurationInHistory",
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
                }) },
            { "Url", 1 },
            { "Duration", 1 },
            { "ChannelId", 1 },
            { "PlayListId", 1 },
            { "ChannelName",
    new BsonDocument("$arrayElemAt",
    new BsonArray
                {
                    "$Channel.Name",
                    0
                }) },
            { "CoverUrl",
    new BsonDocument("$arrayElemAt",
    new BsonArray
                {
                    "$Channel.CoverUrl",
                    0
                }) },
            { "CreatedAt", 1 },
            { "TotalLikes",
    new BsonDocument("$size",
    new BsonDocument("$filter",
    new BsonDocument
                    {
                        { "input", "$TotalLikes" },
                        { "as", "like" },
                        { "cond",
    new BsonDocument("$eq",
    new BsonArray
                            {
                                "$$like.IsLiked",
                                true
                            }) }
                    })) },
            { "TotalDislikes",
    new BsonDocument("$size",
    new BsonDocument("$filter",
    new BsonDocument
                    {
                        { "input", "$TotalLikes" },
                        { "as", "like" },
                        { "cond",
    new BsonDocument("$eq",
    new BsonArray
                            {
                                "$$like.IsLiked",
                                false
                            }) }
                    })) },
            { "TotalComments",
    new BsonDocument("$size", "$Comments") },
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
                }) }
          }));

            // Pagination: Skip records and limit to the page size
            pipeline.Add(new BsonDocument("$skip", (pageNumber - 1) * pageSize));
            pipeline.Add(new BsonDocument("$limit", pageSize));
            pipeline.Add(new BsonDocument("$sort", new BsonDocument("CreatedAt", model.SortOrder ?? 1)));

            var aggregateOptions = new AggregateOptions { AllowDiskUse = true };

            // Execute the aggregation pipeline
            var cursor = await _collection.AggregateAsync<DBVideoModel>(pipeline, aggregateOptions);

            var result = await cursor.ToListAsync();
            return result;
        }


    }
}
