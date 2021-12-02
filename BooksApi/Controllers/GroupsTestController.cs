using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BooksApi.Models.Global;
using BooksApi.Models.Book;
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
        private readonly GenericService<SystemFunctionTest> _serviceSysFuctionTest;
        private readonly GenericService<MenuTest> _serviceMenuTest;

        public GroupsTestController()
        {
            _serviceGroupTest = new GenericService<GroupTest>();
            _serviceSysFuctionTest = new GenericService<SystemFunctionTest>();
            _serviceMenuTest = new GenericService<MenuTest>();
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

        #region Role 
        /// <summary>
        /// Phân quyền chức năng vào group
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="functionId"></param>
        /// <returns></returns>
        [HttpPost("SetPermission/{groupId}/{functionId}")]
        public async Task<ActionResult> SetPermission(string groupId, string functionId)
        {
            try
            {
                if(!(await _serviceGroupTest.GetByIDAsync(groupId) is GroupTest memberGroup))
                {
                    return BadRequest("Nhóm quyền không tồn tại hoặc đã bị thay đổi.");
                }

                if(!(await _serviceSysFuctionTest.GetByIDAsync(functionId) is SystemFunctionTest systemFunction))
                {
                    return BadRequest("Mã Chức năng không tồn tại hoặc đã bị thay đổi.");
                }

                List<string> permission;
                if (memberGroup.ArrFunctionId != null && memberGroup.ArrFunctionId.Length > 0)
                    permission = new List<string>(memberGroup.ArrFunctionId);
                else
                    permission = new List<string>();

                if (permission == null || permission.Count == 0)
                {
                    permission.Add(functionId);
                }
                else
                {
                    //nếu có thì xóa quyền
                    if (permission.Contains(functionId))
                    {
                        permission.Remove(functionId);
                    }
                    //nếu chưa có thì thêm vào
                    else
                    {
                        permission.Add(functionId);
                    }
                }
                memberGroup.ArrFunctionId = permission.Distinct().ToArray();
                //lưu DB
                var updateResult = await _serviceGroupTest.UpdateAsync(memberGroup.Id, memberGroup);
                if (updateResult.IsAcknowledged)
                    return Ok();
                else
                    return BadRequest("Gán quyền thất bại");
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Phân quyền menu vào group
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="menuId"></param>
        /// <returns></returns>
        [HttpPost("SetMenu/{groupId}/{menuId}")]
        public async Task<ActionResult> SetMenu(string groupId, string menuId)
        {
            try
            {
                if (!(await _serviceGroupTest.GetByIDAsync(groupId) is GroupTest memberGroup))
                {
                    return BadRequest("Nhóm quyền không tồn tại hoặc đã bị thay đổi.");
                }

                if (!(await _serviceMenuTest.GetByIDAsync(menuId) is MenuTest menuTest))
                {
                    return BadRequest("Mã menu không tồn tại hoặc đã bị thay đổi.");
                }

                List<string> permission;
                if (memberGroup.ArrMenuId != null && memberGroup.ArrMenuId.Length > 0)
                    permission = new List<string>(memberGroup.ArrMenuId);
                else
                    permission = new List<string>();

                if (permission == null || permission.Count == 0)
                {
                    permission.Add(menuId);
                }
                else
                {
                    //nếu có thì xóa
                    if (permission.Contains(menuId))
                    {
                        permission.Remove(menuId);
                    }
                    //nếu chưa có thì thêm vào
                    else
                    {
                        permission.Add(menuId);
                    }
                }
                memberGroup.ArrMenuId = permission.Distinct().ToArray();
                //lưu DB
                var updateResult = await _serviceGroupTest.UpdateAsync(memberGroup.Id, memberGroup);
                if (updateResult.IsAcknowledged)
                    return Ok();
                else
                    return BadRequest("Gán menu thất bại");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion
    }
}