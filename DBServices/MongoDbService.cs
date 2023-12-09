using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using Lab3.DataModels;
using Newtonsoft.Json;
using System.IO;
using System.Buffers.Text;
using System.Windows.Media.Imaging;
using Microsoft.Extensions.Configuration;
using System.Windows;

namespace Lab3.DBServices
{
    public class MongodbService
    {
        private readonly IMongoDatabase _database;
       

        public MongodbService()
        {
            var connectionString = ""; // TODO: Add your connection string here

            if(connectionString == "")
            {
                MessageBox.Show("Please enter your connectionstring in the DBServices/MongodbServices");
                Application.Current.Shutdown();
            }
            var client = new MongoClient(connectionString);
            _database = client.GetDatabase("MyQuizDatabase"); 

        }

        public async Task<QuizDAO?> ReadData(string title)
        {
            var collection = _database.GetCollection<QuizDAO>("quizzes");
            var filter = Builders<QuizDAO>.Filter.Eq("Title", title);
            return await collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task WriteData(QuizDAO data, string title)
        {
            var collection = _database.GetCollection<QuizDAO>("quizzes");
            var filter = Builders<QuizDAO>.Filter.Eq("Title", title);
            var update = Builders<QuizDAO>.Update
                .Set("Questions", data.Questions)
                .Set("Title", data.Title);

            await collection.UpdateOneAsync(filter, update, new UpdateOptions { IsUpsert = true });
        }

        public async IAsyncEnumerable<List<Question>> GetAllQuestionsAsync()
        {
            var collection = _database.GetCollection<QuizDAO>("quizzes");
            var quizzes = await collection.Find(_ => true).ToListAsync();

            foreach (var quiz in quizzes)
            {
                yield return quiz.Questions;
            }
        }

        public async Task<List<string>> GetAllQuizTitles()
        {
            var collection = _database.GetCollection<QuizDAO>("quizzes");
            var quizzes = await collection.Find(_ => true).ToListAsync();
            var titles = new List<string>();

            foreach (var quiz in quizzes)
            {
                titles.Add(quiz.Title);
            }

            return titles;
        }

        public async Task SeedFromData()
        {
            var collection = _database.GetCollection<QuizDAO>("quizzes");

            if (!await collection.Find(_ => true).AnyAsync())
            {
                var quizzes = JsonConvert.DeserializeObject<List<QuizDAO>>(File.ReadAllText("QuizSeedData.json"));

                if (quizzes != null)
                {
                    foreach (var quiz in quizzes)
                    {
                        await collection.InsertOneAsync(quiz);
                    }
                }
            }
        }

        public async Task DeleteData(string title)
        {
            var collection = _database.GetCollection<QuizDAO>("quizzes");
            var filter = Builders<QuizDAO>.Filter.Eq("Title", title);
            await collection.DeleteOneAsync(filter);
        }


       
    }
}
