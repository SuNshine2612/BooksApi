using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BooksApi.Models.Global;
using BooksApi.Models.Book;
using BooksApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Linq;

namespace BooksApi.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly GenericService<Book> _serviceBook;

        public BooksController()
        {
            _serviceBook = new GenericService<Book>();
        }

        #region Get list & detail
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                List<BsonDocument> pipeline = new();

                #region Join With UserTest
                BsonArray subPipelineUser = new();
                subPipelineUser
                .Add(
                    new BsonDocument("$match", new BsonDocument()
                        .Add("$expr", new BsonDocument("$eq", new BsonArray { "$_id", "$$idUser" }))
                        .Add("IsActive", new BsonBoolean(true))
                    )
                )
                .Add(
                    new BsonDocument("$project", new BsonDocument().AddRange(new List<BsonElement>() {
                        new BsonElement("_id", 1),
                        new BsonElement("Code", 1),
                        new BsonElement("FullName", 1)
                    }))
                );
                #endregion

                #region Join With Category
                BsonArray subPipelineCategory = new();
                subPipelineCategory
                .Add(
                    new BsonDocument("$match", new BsonDocument()
                        .Add("$expr", new BsonDocument("$eq", new BsonArray { "$_id", "$$idCategory" }))
                        .Add("IsActive", new BsonBoolean(true))
                    )
                )
                .Add(
                    new BsonDocument("$project", new BsonDocument().AddRange(new List<BsonElement>() {
                        new BsonElement("_id", 1),
                        new BsonElement("Code", 1),
                        new BsonElement("Name", 1)
                    }))
                );
                #endregion

                #region Create pipeline
                pipeline.Add(
                    new BsonDocument("$lookup",
                            new BsonDocument("from", "UserTest")
                            .Add("let", new BsonDocument("idUser", "$Author"))
                            .Add("pipeline", subPipelineUser)
                            .Add("as", "ObjAuthors"))
                );
                pipeline.Add(
                    new BsonDocument("$lookup",
                            new BsonDocument("from", "Category")
                            .Add("let", new BsonDocument("idCategory", "$Category"))
                            .Add("pipeline", subPipelineCategory)
                            .Add("as", "ObjCategories"))
                );
                pipeline.Add(
                    new BsonDocument("$sort", new BsonDocument("Category", 1))    
                );
                #endregion

                var result = await _serviceBook._collection.Aggregate<Book>(pipeline).ToListAsync();
                result.ForEach(x =>
                {
                    if (x.ObjAuthors.Count > 0)
                    {
                        x.AuthorName = x.ObjAuthors.FirstOrDefault()?.FullName;
                    }
                    if (x.ObjCategories.Count > 0)
                    {
                        x.CategoryName = x.ObjCategories.FirstOrDefault()?.Name;
                    }
                });
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("[action]/{id}")]
        [HttpGet]
        public async Task<Book> GetDetails(string id)
        {
            Book book = await _serviceBook.GetByIDAsync(id);
            return book;
        }
        #endregion

        #region Create
        [HttpPost]
        public async Task<ActionResult<Book>> Post([FromBody] Book data)
        {
            var session = ConfigFile.MyMongoClient.StartSession();
            try
            {
                session.StartTransaction();
                data.Code = data.Code.ToLower().Trim();
                data.CreatedOn = DateTime.Now;
                data.CreatedBy = UserClaim.UserId;
                var result = await _serviceBook.InsertAsync(data, session);
                session.CommitTransaction();
                return Ok(result);
            }
            catch(Exception ex)
            {
                session.AbortTransaction();
                return BadRequest(ex.Message);
            }
        }
        #endregion

        #region Update
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Put(string id, [FromBody] Book value)
        {
            try
            {
                //kiểm tra giống ID hay không
                if (id.Equals(value.Id, StringComparison.OrdinalIgnoreCase))
                {
                    value.Code = value.Code.ToLower().Trim();
                    //cập nhật UpdatedOn
                    value.UpdatedOn = DateTime.Now;
                    //không cho chỉnh người tạo và IsActive- đề phòng hack
                    /*value.CreatedBy = null;
                    value.IsActive = true;*/
                    //cập nhật UpdatedOn
                    value.UpdatedOn = DateTime.Now;
                    value.UpdatedBy = UserClaim.UserId;
                    //gọi hàm update
                    var result = await _serviceBook.UpdateAsync(id, value);
                    return Ok(result);
                }
                return BadRequest(StaticVar.MessageNotFound);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        #endregion

        #region Delete
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult> Delete(string id)
        {
            try
            {
                var find = await _serviceBook.GetByIDAsync(id);
                if (find != null)
                {
                    return Ok(await _serviceBook.DeleteAsync(id));
                }
                return BadRequest(StaticVar.MessageNotFound);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        #endregion

        #region Other Helper
        [Route("[action]/{id}")]
        [HttpGet]
        public async Task<bool> ExistsCode(string id)
        {
            return await _serviceBook.CheckIssetByID(id);
        }
        #endregion

    }
}