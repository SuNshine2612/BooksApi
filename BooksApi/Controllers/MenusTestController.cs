using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BooksApi.Models.Global;
using BooksApi.Models.Test;
using BooksApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BooksApi.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class MenusTestController : ControllerBase
    {
        private readonly GenericService<MenuTest> _serviceMenuTest;

        public MenusTestController()
        {
            _serviceMenuTest = new GenericService<MenuTest>();
        }

        #region Get list & detail
        [HttpGet]
        public async Task<List<MenuTest>> Get()
        {
            // admin_tms thì load hết, ko thì kiểm tra trong token, lấy ra ds MenuCode !
            /* ClaimsPrincipal claimPrincipal = HttpContext.User;
             if (claimPrincipal.Identity.IsAuthenticated)
             {
                 var userId = UserClaim.UserId ?? "anonymous";
                 if (userId.Equals("admin_tms", StringComparison.OrdinalIgnoreCase))
                 {
                     return await _serviceMenuTest.GetAllAsync();
                 }
                 else
                 {
                     var arrMenuCode = claimPrincipal.FindFirst(StaticVar.ClaimArrMenu).Value;
                     if (!String.IsNullOrEmpty(arrMenuCode))
                     {
                         return await _serviceMenuTest.SearchMatchArray("Code", arrMenuCode.Split(",").ToList());
                     }
                 }
             }
             return null;*/
            return await _serviceMenuTest.GetAllAsync();
        }

        [Route("[action]/{id}")]
        [HttpGet]
        public async Task<MenuTest> GetDetails(string id)
        {
            MenuTest data = await _serviceMenuTest.GetByIDAsync(id);
            return data;
        }
        #endregion

        #region Create
        [HttpPost]
        public async Task<ActionResult<MenuTest>> Post([FromBody] MenuTest data)
        {

            try
            {
                data.Code = data.Code.ToLower().Trim();
                data.CreatedOn = DateTime.Now;
                data.CreatedBy = UserClaim.UserId;
                return await _serviceMenuTest.InsertAsync(data);
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
        public async Task<ActionResult> Put(string id, [FromBody] MenuTest value)
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
                    var result = await _serviceMenuTest.UpdateAsync(id, value);
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
                var find = await _serviceMenuTest.GetByIDAsync(id);
                if (find != null)
                {
                    return Ok(await _serviceMenuTest.DeleteAsync(id));
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
            return await _serviceMenuTest.CheckIssetByID(id);
        }


        [Route("[action]")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> ChangeStatus([FromBody] StatusUpdate statusUpdate)
        {
            try
            {
                var result = await _serviceMenuTest.ChangeStatus(
                statusUpdate.FilterName,
                statusUpdate.FilterValue,
                statusUpdate.UpdateName,
                statusUpdate.UpdateValue);

                return Ok(result);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion
    }
}