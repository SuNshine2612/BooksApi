using BooksApi.Models.Book;
using BooksApi.Models.Global;
using BooksApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BooksApi.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly GenericService<Category> _serviceCategory;

        public CategoriesController()
        {
            _serviceCategory = new GenericService<Category>();
        }

        #region Get list & detail
        [HttpGet]
        public async Task<List<Category>> Get()
        {
            return await _serviceCategory.GetAllAsync();
        }

        [Route("[action]/{id}")]
        [HttpGet]
        public async Task<Category> GetDetails(string id)
        {
            Category category = await _serviceCategory.GetByIDAsync(id);
            return category;
        }
        #endregion

        #region Create
        [HttpPost]
        public async Task<ActionResult<Category>> Post([FromBody] Category data)
        {
            try
            {
                data.Code = data.Code.ToLower().Trim();
                data.CreatedOn = DateTime.Now;
                data.CreatedBy = UserClaim.UserId;
                var result = await _serviceCategory.InsertAsync(data);

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
        public async Task<ActionResult> Put(string id, [FromBody] Category value)
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
                    var result = await _serviceCategory.UpdateAsync(id, value);
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
                var find = await _serviceCategory.GetByIDAsync(id);
                if (find != null)
                {
                    return Ok(await _serviceCategory.DeleteAsync(id));
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
            return await _serviceCategory.CheckIssetByID(id);
        }
        #endregion
    }
}
