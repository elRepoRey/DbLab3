using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;


namespace Lab3.DataModels
{
    public class QuizDAO
{
        [BsonId]
        private ObjectId _id { get; set; }
           
        public string Title { get; set; }
        public List<Question> Questions { get; set; }

        public QuizDAO() 
        {
            Questions = new List<Question>();
            Title = string.Empty;
        }
    }
}
