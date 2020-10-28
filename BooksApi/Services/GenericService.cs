using BooksApi.Models.Global;
using BooksApi.Models.Paging;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace BooksApi.Services
{
    public class GenericService<T> where T:class
    {
        public IMongoCollection<T> _collection;

        private readonly FilterDefinition<T> filterGlobal;
        private readonly FilterDefinition<T> filterBase = Builders<T>.Filter.Eq("IsActive", true);
        //private readonly FilterDefinition<T> filterBase = new BsonDocument("IsActive", true);
        // private readonly FilterDefinition<T> filterBase = "{ IsActive: true }";
        private FilterDefinition<T> filterFinal;

        #region Constructor GenericService
        public GenericService()
        {
            var modelType = typeof(T);

            if(modelType != null && !String.IsNullOrEmpty(modelType.Name))
            {
                string modelName = modelType.Name;
                _collection = ConfigFile.MyMongoDatabase.GetCollection<T>(modelName);
                filterGlobal = filterBase;
            }
        }
        #endregion

        #region Get All List

        /// <summary>
        /// get all objects
        /// </summary>
        /// <returns></returns>
        public List<T> GetAll()
        {
            filterFinal = filterGlobal;
            return _collection.Find(filterFinal).ToList();
        }

 
        /// <summary>
        /// get all objects with option filter and sort
        /// </summary>
        /// <param name="filterOption"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        public async Task<List<T>> GetAllAsync(FilterDefinition<T> filterOption = null, SortDefinition<T> sort = null)
        {
            filterFinal = filterGlobal;
            if(filterOption != null)
            {
                filterFinal &= filterOption;
            }

            if (sort != null)
            {
                return await _collection.Find(filterFinal).Sort(sort).ToListAsync();
            }

            return await _collection.Find(filterFinal).ToListAsync();
        }

        #endregion

        #region Get ID

        /// <summary>
        /// get id not async, no matter id or code
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public T GetByID(string id)
        {
            filterFinal = filterGlobal;

            if(ObjectId.TryParse(id, out ObjectId _outObject))
            {
                filterFinal &= (Builders<T>.Filter.Eq("Id", _outObject));
            }
            else
            {
                filterFinal &= (Builders<T>.Filter.Eq("Code", id.ToLower()));
            }

            return _collection.Find(filterFinal).FirstOrDefault();
        }


        public async Task<T> GetByIDAsync(string id, FilterDefinition<T> optionFilter = null)
        {
            filterFinal = filterGlobal;

            if (ObjectId.TryParse(id, out ObjectId _outObject))
            {
                filterFinal &= (Builders<T>.Filter.Eq("Id", _outObject));
            }
            else
            {
                filterFinal &= (Builders<T>.Filter.Eq("Code", id.ToLower()));
            }


            if (optionFilter != null)
            {
                filterFinal &= optionFilter;
            }

            return await _collection.Find(filterFinal).FirstOrDefaultAsync();
        }
        #endregion

        #region Insert

        /// <summary>
        /// Insert new object
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public T Insert(T t)
        {
            _collection.InsertOne(t);
            return t;
        }

        public async Task<T> InsertAsync(T t, IClientSessionHandle clientSession = null)
        {
            if (clientSession == null)
                await _collection.InsertOneAsync(t);
            else
                await _collection.InsertOneAsync(clientSession, t);
            return t;
        }

        /// <summary>
        /// Dùng để insert một array documents
        /// </summary>
        /// <param name="arrT"></param>
        /// <returns></returns>
        public string InsertMany(List<T> arrT)
        {
            try
            {
                //thuộc tính IsOrdered = true nghĩa là nếu có dòng nào đó trùng nhau thì sẽ xảy ra lỗi và KHÔNG tiếp tục insert tiếp
                //thuộc tính IsOrdered = false nghĩa là nếu có dòng nào đó trùng nhau thì sẽ xảy ra lỗi và SẼ tiếp tục insert tiếp
                var options = new InsertManyOptions { IsOrdered = true };
                _collection.InsertMany(arrT, options);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "Success";
        }

        /// <summary>
        /// Dùng để insert Async một array documents 
        /// </summary>
        /// <param name="arrT"></param>
        /// <param name="clientSession"></param>
        /// <returns></returns>
        public async Task InsertManyAsync(IList<T> arrT, IClientSessionHandle clientSession = null)
        {
            var options = new InsertManyOptions { IsOrdered = true };
            if (clientSession == null)
                await _collection.InsertManyAsync(arrT, options);
            else
                await _collection.InsertManyAsync(clientSession, arrT, options);
        }

        #endregion

        #region Update
        public ReplaceOneResult Update(string id, T tIn) =>
            _collection.ReplaceOne(new BsonDocument("_id", new ObjectId(id)), tIn);

        public async Task<UpdateResult> UpdateAsync(string id, T item, IClientSessionHandle clientSession = null)
        {
            var builder = Builders<T>.Update;
            UpdateDefinition<T> update = null;
            foreach (PropertyInfo prop in typeof(T).GetProperties())
            {
                // Mongo không cho phép thay đổi Mongo IDs
                if (prop.PropertyType == typeof(ObjectId))
                    continue;
                // Nếu không có giá trị nào được gán thì giữ nguyên cái đang có dưới database
                if (prop.GetValue(item) == null)
                    continue;
                //nếu là thuộc tính datetime thì C# tự gán ngày mặc định chứ không phải null, phải kiểm tra và bỏ qua nó
                if (prop.PropertyType == typeof(DateTime))
                {
                    if (Convert.ToDateTime(prop.GetValue(item)) == DateTime.MinValue)
                        continue;
                }
                //gán lần đầu tiên, cũng chính là gán Object Id
                if (update == null)
                    update = builder.Set(prop.Name, prop.GetValue(item));
                else
                {
                    update = update.Set(prop.Name, prop.GetValue(item));
                }
            }

            var filter = new BsonDocument("_id", new ObjectId(id));

            var option = new UpdateOptions
            {
                IsUpsert = false
            };

            if (clientSession == null)
                return await _collection.UpdateOneAsync(filter, update, option).ConfigureAwait(false);

            return await _collection.UpdateOneAsync(clientSession, filter, update, option).ConfigureAwait(false);
        }

        public async Task<string> UpdateManyAsync(IEnumerable<T> entities, IClientSessionHandle clientSession = null)
        {
            var updates = new List<WriteModel<T>>();
            var filterBuilder = Builders<T>.Filter;

            foreach (var doc in entities)
            {
                foreach (PropertyInfo prop in typeof(T).GetProperties())
                {
                    // Nếu không có giá trị nào được gán thì giữ nguyên cái đang có dưới database
                    // if (prop.GetValue(doc) == null)
                    //     continue;
                    if (prop.Name == "Id")
                    {
                        if (prop.GetValue(doc) == null)
                            return "Id không có giá trị, không thể update";
                        var filter = filterBuilder.Eq(prop.Name, prop.GetValue(doc));
                        updates.Add(new ReplaceOneModel<T>(filter, doc));

                        break;
                    }
                }
            }
            BulkWriteResult result;
            if (clientSession == null)
                result = await _collection.BulkWriteAsync(updates).ConfigureAwait(false);
            else
            {
                result = await _collection.BulkWriteAsync(clientSession, updates).ConfigureAwait(false);
            }
            return result.ModifiedCount.ToString();
        }

        #endregion

        #region Delete

        //Chỉ update Status = 1 và UpdatedOn là thời gian hiện tại
        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="id"></param>
        /// <param name="clientSession"></param>
        /// <returns></returns>
        public UpdateResult Delete(string id, IClientSessionHandle clientSession = null)
        {
            FilterDefinition<T> filter = new BsonDocument("_id", new ObjectId(id));

            filterFinal = filterGlobal;
            filterFinal &= filter;

            UpdateDefinition<T> update = new BsonDocument("$set", new BsonDocument { { "IsActive", false }, { "UpdatedOn", DateTime.Now }, { "UpdatedBy", UserClaim.UserId } });
            if (clientSession == null)
                return _collection.UpdateOne(filterFinal, update);

            return _collection.UpdateOne(clientSession, filterFinal, update);
        }

        /// <summary>
        /// Delete Async
        /// </summary>
        /// <param name="id"></param>
        /// <param name="clientSession"></param>
        /// <returns></returns>
        public async Task<UpdateResult> DeleteAsync(string id, IClientSessionHandle clientSession = null)
        {
            FilterDefinition<T> filter = new BsonDocument("_id", new ObjectId(id));

            filterFinal = filterGlobal;
            filterFinal &= filter;

            UpdateDefinition<T> update = new BsonDocument("$set", new BsonDocument { { "IsActive", false }, { "UpdatedOn", DateTime.Now }, { "UpdatedBy", UserClaim.UserId } });
          
            if (clientSession == null)
                return await _collection.UpdateOneAsync(filterFinal, update).ConfigureAwait(false);

            return await _collection.UpdateOneAsync(clientSession, filterFinal, update).ConfigureAwait(false);
        }

        /// <summary>
        /// Hàm xoá nhiều dòng, chủ yếu dùng trong xoá raw data
        /// </summary>
        /// <param name="strFieldName">Tên Trường Khoá ngoại của bảng </param>
        /// <param name="strValue">giá trị, thường id của của bảng chính</param>
        /// <param name="clientSession">Biến clientSession</param>
        /// <returns></returns>
        public async Task<UpdateResult> DeleteManyAsync(string strFieldName, string strValue, IClientSessionHandle clientSession = null)
        {
            FilterDefinition<T> filter = new BsonDocument(strFieldName, strValue);
            filterFinal = filterGlobal;
            filterFinal &= filter;

            UpdateDefinition<T> update = new BsonDocument("$set", new BsonDocument { { "IsActive", false }, { "UpdatedOn", DateTime.Now }, { "UpdatedBy", "Admin" } });
            if (clientSession == null)
                return await _collection.UpdateManyAsync(filterFinal, update).ConfigureAwait(false);
            return await _collection.UpdateManyAsync(clientSession, filterFinal, update).ConfigureAwait(false);
        }

        /// <summary>
        /// Hàm xoá VĨNH VIỄN nhiều dòng, chủ yếu dùng trong xoá raw data
        /// </summary>
        /// <param name="filter">Điều kiện lọc để xóa </param>
        /// <param name="cancellationToken">Biến cancellationToken, cho phép hủy</param>
        /// <returns></returns>
        public async Task<DeleteResult> DeleteManyForeverAsync(FilterDefinition<T> filter, CancellationToken cancellationToken = default)
        {
            if (filter == null)
                return null;
            filterFinal = filterGlobal;
            filterFinal &= filter;
            return await _collection.DeleteManyAsync(filterFinal, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Hàm này dùng để cập nhật 1 số trường trong database theo objectId, hay dùng để cập nhật trạng thái
        /// </summary>
        /// <param name="id">objectId</param>
        /// <param name="bsonDocument"> biến này tạo ở controller với name là tên trường update, value là giá trị tương ứng, </param>
        /// <param name="clientSession"></param>
        /// <returns></returns>
        public async Task<UpdateResult> UpdateCustomizeFieldByIdAsync(string id, BsonDocument bsonDocument, IClientSessionHandle clientSession = null)
        {
            FilterDefinition<T> filter;
            if (ObjectId.TryParse(id, out ObjectId objId))
            {
                //nếu truyền vào là ObjectId thì lọc theo ObjectId
                filter = (Builders<T>.Filter.Eq("Id", objId));
            }
            else
            {
                //ngược lại thì lọc theo Code
                filter = (Builders<T>.Filter.Eq("Code", id));
            }
            //FilterDefinition<T> filter = new BsonDocument("_id", new ObjectId(id));
            UpdateDefinition<T> update = new BsonDocument("$set", bsonDocument.Add("UpdatedOn", DateTime.Now));

            filterFinal = filterGlobal;
            filterFinal &= filter;
            if (clientSession == null)
                return await _collection.UpdateOneAsync(filterFinal, update).ConfigureAwait(false);
            return await _collection.UpdateOneAsync(clientSession, filterFinal, update).ConfigureAwait(false);
        }
       
        /// <summary>
        /// Hàm dùng cập nhật 1 số trường trong database theo điều kiện filter
        /// </summary>
        /// <param name="bsArrFilter"> BsonArray Mảng điều kiện filter</param>
        /// <param name="bsUpdate"> BsonDocument các trường cần cập nhật</param>
        /// <param name="clientSession"></param>
        /// <returns></returns>
        public async Task<UpdateResult> UpdateCustomizeFieldAsync(BsonArray bsArrFilter, BsonDocument bsUpdate, IClientSessionHandle clientSession = null)
        {
            if (bsArrFilter == null || bsUpdate == null)
                return null;
            FilterDefinition<T> filter = new BsonDocument("$and", bsArrFilter.Add(new BsonDocument("IsActive", true)));
            UpdateDefinition<T> update = new BsonDocument("$set", bsUpdate.Add("UpdatedOn", DateTime.Now));
            if (clientSession == null)
                return await _collection.UpdateManyAsync(filter, update).ConfigureAwait(false);
            return await _collection.UpdateManyAsync(clientSession, filter, update).ConfigureAwait(false);
        }

        #endregion

        #region Other helper

        /// <summary>
        /// Check object isse ? return true if isset !
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> CheckIssetByID(string id)
        {
            if(ObjectId.TryParse(id, out ObjectId _outObject))
            {
                return await _collection.Find(new BsonDocument("_id", _outObject)).CountDocumentsAsync() > 0;
            }
            else
            {
                return await _collection.Find(Builders<T>.Filter.Eq("Code", id.ToLower())).CountDocumentsAsync() > 0;
            }
        }

        /// <summary>
        /// Đổi trạng thái
        /// </summary>
        /// <param name="filterName">Thuộc tính như Id</param>
        /// <param name="filterdValue">Giá trị thuộc tính</param>
        /// <param name="updateName">Tên thuộc tính cần update như Status</param>
        /// <param name="updateValue">Giá trị thuộc tính cần Update như 1,2,3,4</param>
        /// <param name="clientSession"></param>
        /// <returns></returns>
        public async Task<UpdateResult> ChangeStatus(string filterName, string filterValue, string updateName, int updateValue, IClientSessionHandle clientSession = null)
        {
            FilterDefinition<T> filter;

            if (ObjectId.TryParse(filterValue, out ObjectId _outObject))
            {
                filter = new BsonDocument(filterName, _outObject);
            }
            else
            {
                filter = new BsonDocument(filterName, filterValue);
            }
            
            UpdateDefinition<T> update = new BsonDocument("$set", new BsonDocument { 
                { updateName, updateValue }, 
                { "IsActive", true }, 
                { "UpdatedOn", DateTime.Now }, 
                { "UpdatedBy", UserClaim.UserId }
            });

            filterFinal = filterGlobal;
            filterFinal &= filter;
            if (clientSession == null)
                return await _collection.UpdateManyAsync(filterFinal, update).ConfigureAwait(false);
            return await _collection.UpdateManyAsync(clientSession, filterFinal, update).ConfigureAwait(false);
        }


        /// <summary>
        /// Phân trang - có tìm kiếm theo từng column và tìm kiếm chung cho tất cả columns( text search )
        /// </summary>
        /// <param name="datatablesPaging"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public dynamic PagingFilter(DatatablesPaging datatablesPaging, FilterDefinition<T> filter = null)
        {
            // https://www.codewithmukesh.com/blog/jquery-datatable-in-aspnet-core/
            filterFinal = filterGlobal;

            if (filter != null)
                filterFinal &= filter;

            if (datatablesPaging == null)
                return null;

            datatablesPaging.PageSize = Convert.ToInt32(datatablesPaging.Length);
            //default Ascending Sort by Code !
            
            datatablesPaging.SortColumn = char.ToUpper(datatablesPaging.SortColumn[0]) + datatablesPaging.SortColumn.Substring(1);
            SortDefinition<T> v_Sort = Builders<T>.Sort.Ascending(datatablesPaging.SortColumn ?? "Code");

            //nếu có chỉ định column cần sort và chỉ thị sort (asc, desc) thì làm theo
            if ( !String.IsNullOrEmpty(datatablesPaging.SortColumnDirection) && 
                datatablesPaging.SortColumnDirection.Equals("desc", StringComparison.OrdinalIgnoreCase) )
            {
                v_Sort = Builders<T>.Sort.Descending(datatablesPaging.SortColumn ?? "Code");
            }

            //lấy tổng số documents có trong collection 
            //ước lượng dựa vào metadata nên nó sẽ chạy nhanh hơn (lượng data lớn), nhưng nếu có nhiều cluster thì có thể không chính xác 
            long totalDocuments = _collection.EstimatedDocumentCount();

            List<T> result = new List<T>();
            //lấy tất cả những document đang active

            if (!string.IsNullOrEmpty(datatablesPaging.SearchValue))
            {
                datatablesPaging.SearchValue = "/.*" + datatablesPaging.SearchValue + ".*/";
                try
                {
                    //trùng tên sẽ tự động không tạo nữa, nhưng khác tên mà trùng key ($**) thì báo lỗi exception
                    IndexKeysDefinition<T> keys = Builders<T>.IndexKeys.Text("$**");
                    var options = new CreateIndexOptions()
                    {
                        DefaultLanguage = "english",
                        Name = "TextIndex"
                    };
                    var indexModel = new CreateIndexModel<T>(keys, options);
                    _collection.Indexes.CreateOne(indexModel);
                    // https://www.codeproject.com/Articles/524602/Beginners-guide-to-using-MongoDB-4-0-2-and-the-off
                    // https://mongodb.github.io/mongo-csharp-driver/2.11/getting_started/admin_quick_tour/#list-the-databases
                    // https://viblo.asia/p/tim-hieu-ve-index-trong-mongodb-924lJL4WKPM
                    // use CreateIndexModel
                }
                catch (Exception) { }

                //nếu có điều kiện tìm kiếm (searchValue có giá trị) thì lọc theo điều kiện trước khi lấy
                var v_search = Builders<T>.Filter.Text(datatablesPaging.SearchValue, new TextSearchOptions { CaseSensitive = false });
                filterFinal &= v_search;
            }

            //tìm kiếm xem có filter theo column hay không, nếu có thì add thêm filter vào
            if (datatablesPaging.SearchArray != null && datatablesPaging.SearchArray.Count > 0)
            {
                foreach (KeyValuePair<string, string> pair in datatablesPaging.SearchArray)
                {
                    filterFinal &= new BsonDocument { { pair.Key, new BsonDocument { { "$regex", pair.Value }, { "$options", "i" } } } };
                }
            }

            //lấy tổng số documents sau khi đã lọc theo điều kiện tìm kiếm
            long recordsFiltered = _collection.CountDocuments(filterFinal);

            if (datatablesPaging.PageSize == -1)
            {
                result = _collection.Find(filterFinal)
                 .Sort(v_Sort)
                 .ToList();
            }
            else
            {
                result = _collection.Find(filterFinal)
                     .Skip(Convert.ToInt32(datatablesPaging.Start))
                     .Limit(datatablesPaging.PageSize)
                     .Sort(v_Sort)
                     .ToList();
            }

            if (recordsFiltered == 0)
                recordsFiltered = totalDocuments;

            //trả về dữ liệu theo chuẩn paging của datatable.net
            dynamic resjs = new
            {
                draw = datatablesPaging.Draw,
                recordsTotal = recordsFiltered, // totalDocuments
                recordsFiltered,
                data = result
            };
            return resjs;
            // https://stackoverflow.com/questions/58072703/jsonresultobject-causes-the-collection-type-newtonsoft-json-linq-jtoken-is
            // use return Json(dynamic type)
        }

        /// <summary>
        /// Tìm danh sách khớp với điều kiện 1 array truyền vào
        /// </summary>
        /// <param name="docPropertyName"></param>
        /// <param name="lstValue"></param>
        /// <param name="filter"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        public async Task<List<T>> SearchMatchArray(string docPropertyName, List<string> lstValue, FilterDefinition<T> filter = null, SortDefinition<T> sort = null)
        {
            //reset
            filterFinal = filterGlobal;
            filterFinal &= Builders<T>.Filter.In(docPropertyName, lstValue.Distinct());

            if (filter != null)
                filterFinal &= filter;
            if (sort != null)
                return await _collection.Find(filterFinal).Sort(sort).ToListAsync().ConfigureAwait(false);
            return await _collection.Find(filterFinal).ToListAsync().ConfigureAwait(false);
        }
        
        /// <summary>
        /// Tìm danh sách khớp với điều kiện 1 array truyền vào
        /// </summary>
        /// <param name="docPropertyName"></param>
        /// <param name="lstValue"></param>
        /// <param name="filter"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        public List<T> SearchMatchArray_NotAsync(string docPropertyName, List<string> lstValue, FilterDefinition<T> filter = null, SortDefinition<T> sort = null)
        {
            //reset
            filterFinal = filterGlobal;
            filterFinal &= Builders<T>.Filter.In(docPropertyName, lstValue.Distinct());

            if (filter != null)
                filterFinal &= filter;
            if (sort != null)
                return _collection.Find(filterFinal).Sort(sort).ToList();
            return _collection.Find(filterFinal).ToList();
        }

        /// <summary>
        /// Lấy danh sách T theo điều kiện tìm kiếm
        /// </summary>
        /// <param name="documentPropertyName">DocumentPropertyName tên cột tìm dạng string</param>
        /// <param name="searchValue">SearchValue giá trị để tìm dạng string</param>
        /// <returns></returns>
        public async Task<List<T>> Search(string documentPropertyName, string searchValue)
        {
            //reset
            searchValue = WebUtility.UrlDecode(searchValue);
            filterFinal = filterGlobal;
            filterFinal &= Builders<T>.Filter.Eq(documentPropertyName, searchValue);
            return await _collection.Find(filterFinal).ToListAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Lấy danh sách theo điều kiện lọc
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public List<T> GetListByfilteNoAsync(FilterDefinition<T> filter)
        {
            filter &= filterGlobal;
            return _collection.Find(filter).ToList();
        }
        #endregion
    }
}
