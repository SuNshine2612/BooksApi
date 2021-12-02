using BooksApi.Models.Book;
using BooksWebClient.Models.Global;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BooksWebClient.Models
{
    public class TestRepository
    {
        readonly IMongoCollection<Category> _category;

        public TestRepository(IDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _category = database.GetCollection<Category>("Category");
        }

        public IList<Category> Read() =>
            _category.Find(sub => true).ToList();

        public Category Find(string id) =>
            _category.Find(sub => sub.Id == id).SingleOrDefault();
    }
}
