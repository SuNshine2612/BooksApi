using BooksApi.Models.Book;
using BooksApi.Models.Global;
using BooksApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BooksApi.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class NewsController : ControllerBase
    {
        private readonly GenericService<News> _service;

        public NewsController()
        {
            _service = new GenericService<News>();
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
                        .Add("$expr", new BsonDocument("$eq", new BsonArray { "$Code", "$$idUser" }))
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

                #region Create pipeline
                pipeline.Add(
                    new BsonDocument("$lookup",
                            new BsonDocument("from", "UserTest")
                            .Add("let", new BsonDocument("idUser", "$CreatedBy"))
                            .Add("pipeline", subPipelineUser)
                            .Add("as", "ObjUsers"))
                );
                #endregion

                var result = await _service._collection.Aggregate<News>(pipeline).ToListAsync();
                result.ForEach(x =>
                {
                    if (x.ObjUsers.Count > 0)
                    {
                        x.CreatedBy = x.ObjUsers.FirstOrDefault().FullName;
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
        public async Task<IActionResult> GetDetails(string id)
        {
            try
            {
                var result = await _service.GetByIDAsync(id);
                if (result is not null)
                    return Ok(result);
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion

        #region Create
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] News data)
        {
            try
            {
                data.CreatedOn = DateTime.Now;
                data.CreatedBy = UserClaim.UserId;
                var result = await _service.InsertAsync(data);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion

        #region Update
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Put(string id, [FromBody] News value)
        {
            try
            {
                //kiểm tra giống ID hay không
                if (id.Equals(value.Id, StringComparison.OrdinalIgnoreCase))
                {
                    //cập nhật UpdatedOn
                    value.UpdatedOn = DateTime.Now;
                    //cập nhật UpdatedOn
                    value.UpdatedOn = DateTime.Now;
                    value.UpdatedBy = UserClaim.UserId;
                    //gọi hàm update
                    var result = await _service.UpdateAsync(id, value);
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
                var find = await _service.GetByIDAsync(id);
                if (find != null)
                {
                    return Ok(await _service.DeleteAsync(id));
                }
                return NotFound(StaticVar.MessageNotFound);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        #endregion
    }
}
