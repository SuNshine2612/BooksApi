using BooksApi.Models.Global;
using BooksApi.Models.Test;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BooksApi.Services
{
    /*
    https://www.domstamand.com/securing-asp-net-core-webapi-with-an-api-key/
    https://forums.asp.net/t/2151654.aspx?IAsyncAuthorizationFilter+In+Asp+net+Core 
    https://stackoverflow.com/questions/60039594/authorization-in-net-core-3-order-in-which-requirements-are-being-executed
    https://jasonwatmore.com/post/2019/10/16/aspnet-core-3-role-based-authorization-tutorial-with-example-api
    use Attribute
    https://blog.honosoft.com/2019/07/09/dotnet-core-what-can-you-do-in-case-you-wish-to-authorize-without-authorize/
    */
    public class CustomAuthorize : IAsyncAuthorizationFilter
    {
        private readonly GenericService<SystemFunctionTest> _serviceSysFunction;
        private readonly GenericService<GroupTest> _serviceMemberGroup;

        public CustomAuthorize()
        {
            _serviceSysFunction = new GenericService<SystemFunctionTest>();
            _serviceMemberGroup = new GenericService<GroupTest>();

        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            if (context == null)
            {
                return;
            }
            if (context.ActionDescriptor.RouteValues["controller"].Equals("UsersTest", StringComparison.OrdinalIgnoreCase) 
                && context.ActionDescriptor.RouteValues["action"].Equals("Login", StringComparison.OrdinalIgnoreCase))
            {
                //reset
                UserClaim.Reset();
            }

            string actionName = string.Empty;
            string controllerName = string.Empty;

            try
            {
                var claimPrincipal = context.HttpContext.User as ClaimsPrincipal;
                //không phải là admin
                if (claimPrincipal.Identity.IsAuthenticated && !UserClaim.UserId.Equals("admin_tms", StringComparison.OrdinalIgnoreCase))
                {
                    //(*)lấy controller và action từ Context (request)
                    var routeData = context.RouteData;
                    actionName = routeData.Values["action"]?.ToString();
                    controllerName = routeData.Values["controller"]?.ToString();

                    //(*)lấy danh sách quyền trong token ra
                    var arrFunc = claimPrincipal.FindFirst(StaticVar.ClaimArrFunction)?.Value;
                    var arrGroup = claimPrincipal.FindFirst(StaticVar.ClaimArrMemberGroup)?.Value;
                    if (string.IsNullOrEmpty(arrFunc) || string.IsNullOrEmpty(arrGroup))
                    {
                        context.Result = new CustomUnauthorizedResult($"Chưa phân quyền chức năng {controllerName}/{actionName}");
                        return;
                    }

                    //(*)nếu quyền trong token khác với quyền trong DB, đá ra, bắt login lại để có quyền mới
                    // Cách 1: SearchMatchArray
                    List<GroupTest> memberGroups = await _serviceMemberGroup.SearchMatchArray("Code", arrGroup.Split(",").ToList()).ConfigureAwait(false);
                    // Cách 2: Filter, nếu sử dụng nhiều điều kiện lọc, example:
                    //var v_filter = Builders<GroupTest>.Filter.In("Code", arrGroup.Split(",").ToList());
                    // & Builders<A>.Filter.Where(p => p.a == xyz)
                    //List<GroupTest> memberGroups = _serviceMemberGroup.GetListByfilteNoAsync(v_filter);

                    //Gom nhóm chức năng lại (TRONG TRƯỜNG HỢP 1 MEMBER THUỘC NHIỀU NHÓM TRONG 1 KHO)
                    string[] strArrFunction = Array.Empty<string>();
                    //string[] strArrMenu = Array.Empty<string>();
                    foreach (GroupTest member in memberGroups)
                    {
                        if (member.ArrFunctionId != null &&
                            member.ArrFunctionId.Length > 0)
                        {
                            //gom nhóm các chức năng lại, loại bỏ trùng (nếu muốn giữ trùng thì dùng Concat thay cho Union)
                            strArrFunction = strArrFunction.Union(member.ArrFunctionId).ToArray();
                            //strArrMenu = strArrMenu.Union(member.ArrMenuId).ToArray();
                        }
                    }
                    string[] lstFuncFromToken = arrFunc.Split(",");
                    bool isArrayTheSame = (strArrFunction.Except(lstFuncFromToken).Any()); //true or false
                    if (isArrayTheSame == true) //không giống nhau
                    {
                        context.Result = new CustomUnauthorizedResult($"Phân quyền chức năng không phù hợp: {controllerName}/{actionName}");
                        return;
                    }

                    //(*)get FUNC from DB
                    List<SystemFunctionTest> functions = await _serviceSysFunction.SearchMatchArray("Code", lstFuncFromToken.ToList()).ConfigureAwait(false);
                    var permission = functions.Where(x => x.ControllerName == controllerName)
                                              .Where(x => x.ActionName == actionName)
                                              .ToList();

                    if (permission == null || permission.Count == 0)
                    {
                        context.Result = new CustomUnauthorizedResult($"Không có quyền truy cập chức năng này: {controllerName}/{actionName}");
                        return;
                    }
                }
            }
            catch(Exception ex)
            {
                // Thông báo lỗi không có quyền truy cập ControllerName và ActionName !!!
                context.Result = new CustomUnauthorizedResult($"{controllerName}/{actionName}. Exception: {ex.Message}");
                return;
            }
        }
    }

    public class CustomUnauthorizedResult : JsonResult
    {
        public CustomUnauthorizedResult(string message)
            : base(new CustomError(message))
        {
            StatusCode = StatusCodes.Status401Unauthorized;
        }
    }
    public class CustomError
    {
        public string Error { get; set; }

        public CustomError(string message)
        {
            Error = message;
        }
    }
}
