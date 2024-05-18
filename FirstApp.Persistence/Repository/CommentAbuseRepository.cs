using FirstApp.Application.Abstractions.IRepositories;
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
    public class CommentAbuseRepository(IMongoDatabase database) : ICommentAbuserepository
    {
        public async Task<bool> IsAbusiveComment(string comment)
        {
            var words = comment.Split(new[] { ' ', ',', '.', '!', '?' }, StringSplitOptions.RemoveEmptyEntries);

            var filterDefinitions = new List<FilterDefinition<AbusiveWord>>();

            foreach (var word in words)
            {
                filterDefinitions.Add(Builders<AbusiveWord>.Filter.Eq("WordEn", word.ToLower()));
                filterDefinitions.Add(Builders<AbusiveWord>.Filter.Eq("WordUr", word.ToLower()));
                filterDefinitions.Add(Builders<AbusiveWord>.Filter.Eq("WordEs", word.ToLower()));
                filterDefinitions.Add(Builders<AbusiveWord>.Filter.Eq("WordFr", word.ToLower()));
                filterDefinitions.Add(Builders<AbusiveWord>.Filter.Eq("WordDe", word.ToLower()));
                filterDefinitions.Add(Builders<AbusiveWord>.Filter.Eq("WordZh", word.ToLower()));
                filterDefinitions.Add(Builders<AbusiveWord>.Filter.Eq("WordHi", word.ToLower()));
                filterDefinitions.Add(Builders<AbusiveWord>.Filter.Eq("WordAr", word.ToLower()));
                filterDefinitions.Add(Builders<AbusiveWord>.Filter.Eq("WordRu", word.ToLower()));
            }

            var combinedFilter = Builders<AbusiveWord>.Filter.Or(filterDefinitions);

            var collection = database.GetCollection<AbusiveWord>("AbusiveWords");

            var abusiveWords = await collection.Find(combinedFilter).ToListAsync();

            return abusiveWords.Count > 0;
        }
    }
}
