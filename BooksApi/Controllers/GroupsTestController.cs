using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BooksApi.Models.Global;
using BooksApi.Models.Test;
using BooksApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GroupTestsApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class GroupsTestController : ControllerBase
    {
        private readonly GenericService<GroupTest> _serviceGroupTest;

        public GroupsTestController()
        {
            _serviceGroupTest = new GenericService<GroupTest>();
        }

        #region Get list & detail
        [HttpGet]
        public async Task<List<GroupTest>> Get()
        {
            return await _serviceGroupTest.GetAllAsync();
        }

        [Route("[action]/{id}")]
        [HttpGet]
        public async Task<GroupTest> GetDetails(string id)
        {
            GroupTest GroupTest = await _serviceGroupTest.GetByIDAsync(id);
            return GroupTest;
        }
        #endregion

        #region Create
        [HttpPost]
        public async Task<ActionResult<GroupTest>> Post([FromBody] GroupTest data)
        {

            try
            {
                data.Code = data.Code.ToLower().Trim();
                data.CreatedOn = DateTime.Now;
                data.CreatedBy = UserClaim.UserId;
                return await _serviceGroupTest.InsertAsync(data);
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
        public async Task<ActionResult> Put(string id, [FromBody] GroupTest value)
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
                    var result = await _serviceGroupTest.UpdateAsync(id, value);
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
                var find = await _serviceGroupTest.GetByIDAsync(id);
                if (find != null)
                {
                    return Ok(await _serviceGroupTest.DeleteAsync(id));
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
            return await _serviceGroupTest.CheckIssetByID(id);
        }
        #endregion
    }
}