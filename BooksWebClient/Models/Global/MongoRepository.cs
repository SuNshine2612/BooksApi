using BooksApi.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace BooksWebClient.Models.Global
{
    public class MongoRepository<T> : IMongoRepository<T>
    {
        public readonly IMongoCollection<T> _collection;
        readonly MongoClient _client;
        readonly IMongoDatabase _database;
        readonly FilterDefinition<T> filterBase = Builders<T>.Filter.Eq("IsActive", true);

        //protected readonly DatabaseSettings _settings;

        public MongoRepository(IDatabaseSettings settings)
        {
            _client = new MongoClient(settings.ConnectionString);
            _database = _client.GetDatabase(settings.DatabaseName);
            _collection = _database.GetCollection<T>(GetCollectionName(typeof(T)));
        }

        /*public MongoRepository(IOptions<DatabaseSettings> settings)
        {
            _settings = settings.Value;
            _client = new MongoClient(_settings.ConnectionString);
            _database = _client.GetDatabase(_settings.DatabaseName);
            _collection = _database.GetCollection<T>(GetCollectionName(typeof(T)));
        }*/


        // https://docs.microsoft.com/en-us/dotnet/standard/attributes/retrieving-information-stored-in-attributes
        // https://stackoverflow.com/questions/6538366/access-to-the-value-of-a-custom-attribute/46341017
        protected static string GetCollectionName(Type T)
        {
            // lấy tên collection từ thuộc tính BsonCollection
            var attribute = (BsonCollectionAttribute)T.GetCustomAttributes(typeof(BsonCollectionAttribute), true).FirstOrDefault();
            if(attribute is null) // Không có BsonCollection, lấy tên class Model thử !
            {
                var modelType = typeof(T);
                return modelType.Name;
            }
            return attribute.CollectionName;
        }

        public IQueryable<T> AsQueryable()
        {
            return  _collection.AsQueryable();
        }

        public void DeleteByIdForever(string id)
        {
            var filter = Builders<T>.Filter.Eq("Id", new ObjectId(id));
            //var filter = new BsonDocument("_id", new ObjectId(id));
            _collection.FindOneAndDelete(filter);
        }

        public Task DeleteByIdForeverAsync(string id, IClientSessionHandle clientSession = null)
        {
            return Task.Run(() =>
            {
                var objectId = new ObjectId(id);
                var filter = Builders<T>.Filter.Eq("Id", objectId);
                if (clientSession is not null)
                    _collection.FindOneAndDeleteAsync(clientSession, filter).ConfigureAwait(false);
                else
                    _collection.FindOneAndDeleteAsync(filter).ConfigureAwait(false);
            });
        }

        public void DeleteManyForever(FilterDefinition<T> filter)
        {
            _collection.DeleteMany(filter);
        }

        public Task DeleteManyForeverAsync(FilterDefinition<T> filter, IClientSessionHandle clientSession = null)
        {
            if(clientSession is null)
                return Task.Run(() => _collection.DeleteManyAsync(filter).ConfigureAwait(false));
            else
                return Task.Run(() => _collection.DeleteManyAsync(clientSession, filter).ConfigureAwait(false));
        }

        public void DeleteOneForever(FilterDefinition<T> filter)
        {
            _collection.FindOneAndDelete(filter);
        }

        public Task DeleteOneForeverAsync(FilterDefinition<T> filterExpression, IClientSessionHandle clientSession = null)
        {
            return Task.Run(() => 
            {
                if (clientSession is null)
                    _collection.FindOneAndDeleteAsync(filterExpression).ConfigureAwait(false);
                else
                    _collection.FindOneAndDeleteAsync(clientSession, filterExpression).ConfigureAwait(false);
            });
        }

        public IEnumerable<T> FilterBy(FilterDefinition<T> filter)
        {
            var filterFinal = filterBase;
            filterFinal &= filter;

            return _collection.Find(filterFinal).ToEnumerable();
        }

        public async Task<List<T>> FilterByAsync(FilterDefinition<T> filter)
        {
            var filterFinal = filterBase;
            filterFinal &= filter;

            return await _collection.Find(filterFinal).ToListAsync();
        }

        public IEnumerable<TProjected> FilterBy<TProjected>(FilterDefinition<T> filter, ProjectionDefinition<T, TProjected> projectionExpression)
        {
            var filterFinal = filterBase;
            filterFinal &= filter;

            return _collection.Find(filterFinal).Project(projectionExpression).ToEnumerable();
        }

        public async Task<List<TProjected>> FilterByAsync<TProjected>(FilterDefinition<T> filter, ProjectionDefinition<T, TProjected> projectionExpression)
        {
            var filterFinal = filterBase;
            filterFinal &= filter;

            return await _collection.Find(filterFinal).Project(projectionExpression).ToListAsync();
        }

        public T FindById(string id)
        {
            var objectId = new ObjectId(id);
            var filter = Builders<T>.Filter.Eq("Id", objectId);
            //var filter = new BsonDocument("_id", new ObjectId(id));
            return _collection.Find(filter).SingleOrDefault();
        }

        public Task<T> FindByIdAsync(string id)
        {
            return Task.Run(() =>
            {
                var objectId = new ObjectId(id);
                var filter = Builders<T>.Filter.Eq("Id", objectId);
                return _collection.Find(filter).SingleOrDefaultAsync();
            });
        }

        public T FindOne(FilterDefinition<T> filter)
        {
            var filterFinal = filterBase;
            filterFinal &= filter;

            return _collection.Find(filterFinal).FirstOrDefault();
        }

        public Task<T> FindOneAsync(FilterDefinition<T> filter)
        {
            var filterFinal = filterBase;
            filterFinal &= filter;

            return Task.Run(() => _collection.Find(filterFinal).FirstOrDefaultAsync());
        }

        public void InsertMany(ICollection<T> documents)
        {
            _collection.InsertMany(documents);
        }

        public async Task InsertManyAsync(ICollection<T> documents, IClientSessionHandle clientSession = null)
        {
            if (clientSession is null)
                await _collection.InsertManyAsync(documents).ConfigureAwait(false);
            else
                await _collection.InsertManyAsync(clientSession, documents).ConfigureAwait(false);
        }

        public void InsertOne(T document)
        {
            _collection.InsertOne(document);
        }

        public async Task InsertOneAsync(T document, IClientSessionHandle clientSession = null)
        {
            //return Task.Run(() => _collection.InsertOneAsync(document));
            if(clientSession is null)
                await _collection.InsertOneAsync(document).ConfigureAwait(false);
            else
                await _collection.InsertOneAsync(clientSession, document).ConfigureAwait(false);
        }

        public void ReplaceOne(FilterDefinition<T> filter, T document)
        {
            _collection.FindOneAndReplace(filter, document);
        }

        public Task ReplaceOneAsync(FilterDefinition<T> filter, T document, IClientSessionHandle clientSession = null)
        {
            return Task.Run(() =>
            {
                if(clientSession is null)
                    _collection.FindOneAndReplaceAsync(filter, document).ConfigureAwait(false);
                else
                    _collection.FindOneAndReplaceAsync(clientSession, filter, document).ConfigureAwait(false);
            });
        }
    }
}
