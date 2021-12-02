using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace BooksWebClient.Models.Global
{
    /// <summary>
    /// defines all public methods that our repository will use
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IMongoRepository<T>
    {
        #region GET ALL, GET ONE
        IQueryable<T> AsQueryable();

        IEnumerable<T> FilterBy(FilterDefinition<T> filter);

        Task<List<T>> FilterByAsync(FilterDefinition<T> filter);

        IEnumerable<TProjected> FilterBy<TProjected>(FilterDefinition<T> filter, ProjectionDefinition<T, TProjected> projectionExpression);

        Task<List<TProjected>> FilterByAsync<TProjected>(FilterDefinition<T> filter, ProjectionDefinition<T, TProjected> projectionExpression);

        T FindOne(FilterDefinition<T> filter);

        Task<T> FindOneAsync(FilterDefinition<T> filter);

        T FindById(string id);
        Task<T> FindByIdAsync(string id);

        #endregion

        #region INSERT
        void InsertOne(T document);

        Task InsertOneAsync(T document, IClientSessionHandle clientSession = null);

        void InsertMany(ICollection<T> documents);

        Task InsertManyAsync(ICollection<T> documents, IClientSessionHandle clientSession = null);
        #endregion

        #region UPDATE
        void ReplaceOne(FilterDefinition<T> filter, T document);

        Task ReplaceOneAsync(FilterDefinition<T> filter, T document, IClientSessionHandle clientSession = null);
        #endregion

        #region DELETE FOREVER ( xóa thật sự document khỏi collection )
        void DeleteOneForever(FilterDefinition<T> filter);

        Task DeleteOneForeverAsync(FilterDefinition<T> filterExpression, IClientSessionHandle clientSession = null);

        void DeleteByIdForever(string id);

        Task DeleteByIdForeverAsync(string id, IClientSessionHandle clientSession = null);

        void DeleteManyForever(FilterDefinition<T> filter);

        Task DeleteManyForeverAsync(FilterDefinition<T> filter, IClientSessionHandle clientSession = null);
        #endregion
    }
}
