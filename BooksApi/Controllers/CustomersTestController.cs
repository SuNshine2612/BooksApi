using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BooksApi.Models.Global;
using BooksApi.Models.Paging;
using BooksApi.Models.Book;
using BooksApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BooksApi.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class CustomersTestController : ControllerBase
    {
        private readonly GenericService<CustomerTest> _serviceCustomerTest;

        public CustomersTestController()
        {
            _serviceCustomerTest = new GenericService<CustomerTest>();
        }

        #region Get list & detail
        [HttpGet]
        public async Task<List<CustomerTest>> Get()
        {
            return await _serviceCustomerTest.GetAllAsync();
        }

        [Route("[action]/{id}")]
        [HttpGet]
        public async Task<CustomerTest> GetDetails(string id)
        {
            CustomerTest _data = await _serviceCustomerTest.GetByIDAsync(id);
            return _data;
        }
        #endregion

        #region Create
        [HttpPost]
        public async Task<ActionResult<CustomerTest>> Post([FromBody] CustomerTest data)
        {

            try
            {
                data.Code = data.Code.ToLower().Trim();
                data.CreatedOn = DateTime.Now;
                data.CreatedBy = UserClaim.UserId;
                return await _serviceCustomerTest.InsertAsync(data);
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
        public async Task<ActionResult> Put(string id, [FromBody] CustomerTest value)
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
                    var result = await _serviceCustomerTest.UpdateAsync(id, value);
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
                var find = await _serviceCustomerTest.GetByIDAsync(id);
                if (find != null)
                {
                    return Ok(await _serviceCustomerTest.DeleteAsync(id));
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
            return await _serviceCustomerTest.CheckIssetByID(id);
        }
        #endregion

        #region Paging Example
        [HttpPost("Paging")]
        public dynamic Paging(DatatablesPaging datatablesPaging)
        {
            return _serviceCustomerTest.PagingFilter(datatablesPaging);
        }
        #endregion
    }
}