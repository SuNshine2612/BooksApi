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
    [ApiController]
    public class ConfigsController : ControllerBase
    {
        private readonly GenericService<Config> _service;

        public ConfigsController()
        {
            _service = new GenericService<Config>();
        }

        #region Get list & detail
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                return Ok(await _service.GetAllAsync());
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
        public async Task<IActionResult> Post([FromBody] Config data)
        {
            try
            {
                data.Code = data.Code.ToLower().Trim();
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
        public async Task<IActionResult> Put(string id, [FromBody] Config value)
        {
            try
            {
                //kiểm tra giống ID hay không
                if (id.Equals(value.Id, StringComparison.OrdinalIgnoreCase))
                {
                    value.Code = value.Code.ToLower().Trim();
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

        #region Other Helper
        [Route("[action]/{id}")]
        [HttpGet]
        public async Task<bool> ExistsCode(string id)
        {
            return await _service.CheckIssetByID(id);
        }
        #endregion
    }
}
