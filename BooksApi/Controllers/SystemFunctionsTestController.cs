using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BooksApi.Models.Global;
using BooksApi.Models.Paging;
using BooksApi.Models.Test;
using BooksApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BooksApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SystemFunctionsTestController : ControllerBase
    {
        private readonly GenericService<SystemFunctionTest> _serviceSysFuctionTest;

        public SystemFunctionsTestController()
        {
            _serviceSysFuctionTest = new GenericService<SystemFunctionTest>();
        }

        #region Get list & detail
        [HttpGet]
        public async Task<List<SystemFunctionTest>> Get()
        {
            return await _serviceSysFuctionTest.GetAllAsync();
        }

        [Route("[action]/{id}")]
        [HttpGet]
        public async Task<SystemFunctionTest> GetDetails(string id)
        {
            SystemFunctionTest _data = await _serviceSysFuctionTest.GetByIDAsync(id);
            return _data;
        }
        #endregion

        #region Create
        [HttpPost]
        public async Task<ActionResult<SystemFunctionTest>> Post([FromBody] SystemFunctionTest data)
        {

            try
            {
                data.Code = data.Code.ToLower().Trim();
                data.CreatedOn = DateTime.Now;
                data.CreatedBy = UserClaim.UserId;
                return await _serviceSysFuctionTest.InsertAsync(data);
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
        public async Task<ActionResult> Put(string id, [FromBody] SystemFunctionTest value)
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
                    var result = await _serviceSysFuctionTest.UpdateAsync(id, value);
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
                var find = await _serviceSysFuctionTest.GetByIDAsync(id);
                if (find != null)
                {
                    return Ok(await _serviceSysFuctionTest.DeleteAsync(id));
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
            return await _serviceSysFuctionTest.CheckIssetByID(id);
        }
        #endregion

        #region Paging Example
        [HttpPost("Paging")]
        public dynamic Paging(DatatablesPaging datatablesPaging)
        {
            return _serviceSysFuctionTest.PagingFilter(datatablesPaging);
        }
        #endregion
    }
}