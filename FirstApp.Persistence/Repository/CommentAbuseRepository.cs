using FirstApp.Application.Abstractions.IRepositories;
using FirstApp.Domain.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FirstApp.Persistence.Repository
{
    public class CommentAbuseRepository(IMongoDatabase database) : ICommentAbuserepository
    {
        /* public async Task<bool> IsAbusiveComment(string comment)
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
         }*/


        public async Task<bool> IsAbusiveComment(string comment)
        {
            var collection = database.GetCollection<AbusiveWord>("AbusiveWords");

            // Get all abusive words from the database
            var abusiveWords = await collection.Find(_ => true).ToListAsync();

            // Construct a regular expression pattern to match whole words in all languages
            var regexPattern = string.Join("|", abusiveWords
                .SelectMany(word => new[]
                {
            word.WordEn,
            word.WordUr,
            word.WordEs,
            word.WordFr,
            word.WordDe,
            word.WordZh,
            word.WordHi,
            word.WordAr,
            word.WordRu,
                })
                .Distinct());

            // Construct a regular expression with word boundaries
            var regex = new Regex($@"\b({regexPattern})\b", RegexOptions.IgnoreCase);

            // Check if the comment contains any abusive word
            return regex.IsMatch(comment);
        }

    }
}
