using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using BooksApi.Models.Paging;
using BooksApi.Models.Test;
using BooksApi.Services;
using BooksWebApp.Helper;
using BooksWebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BooksWebApp.Controllers
{
    [Authorize]
    public class SystemFunctionTestController : BaseController
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(IFormCollection request)
        {
            try
            {
                // before send to API, we need init DataPaging !!
                var database = ApiHelper<DatatablesPaging>.InitDatatable(request);
                // after init, we send to API - use for DataTable ( paging ) !!
                var listItems = await ApiHelper<dynamic>.RunPostAsync($"{StaticVar.ApiUrlSystemFunctions}/Paging", database);

                if (listItems == null)
                {
                    return Json(new
                    {
                        isValid = false,
                        html = MyViewHelper.RenderRazorViewToString(this, "Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier, Message = StaticVar.MessageNotFound })
                    });
                }
                else
                {
                    return Json(listItems);
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    isValid = false,
                    html = MyViewHelper.RenderRazorViewToString(this, "Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier, Message = ex.Message })
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> AddOrEdit(string id)
        {
            if (String.IsNullOrEmpty(id))
            {
                return View(new SystemFunctionTest());
            }
            else
            {
                if (!(await ApiHelper<SystemFunctionTest>.RunGetAsync($"{StaticVar.ApiUrlSystemFunctions}/GetDetails/{id}") is SystemFunctionTest _data))
                {
                    return NotFound();
                }
                return View(_data);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddOrEdit(string id, SystemFunctionTest data)
        {
            if (ModelState.IsValid)
            {
                //Insert
                if (String.IsNullOrEmpty(id))
                {
                    // Check isset code ?
                    if (await ApiHelper<bool>.CheckIssetCode($"{StaticVar.ApiUrlSystemFunctions}/ExistsCode/{data.Code}"))
                    {
                        ModelState.AddModelError("", StaticVar.MessageCodeDuplicated);
                        return Json(new
                        {
                            isValid = false,
                            html = MyViewHelper.RenderRazorViewToString(this, "AddOrEdit", data)
                        });
                    }
                    else
                    {
                        try
                        {
                            SystemFunctionTest result = await ApiHelper<SystemFunctionTest>.RunPostAsync(StaticVar.ApiUrlSystemFunctions, data);
                        }
                        catch (Exception ex)
                        {

                            return Json(new { isValid = false, html = MyViewHelper.RenderRazorViewToString(this, "Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier, Message = ex.Message }) });
                        }
                    }

                }
                //Update
                else
                {
                    // Check isset code , but allow itself !!
                    SystemFunctionTest _object = await ApiHelper<SystemFunctionTest>.RunGetAsync($"{StaticVar.ApiUrlSystemFunctions}/GetDetails/{data.Code}");
                    if (_object == null || _object.Id.Equals(data.Id))
                    {
                        try
                        {
                            await ApiHelper<SystemFunctionTest>.RunPutAsync($"{StaticVar.ApiUrlSystemFunctions}/{id}", data);
                        }
                        catch (Exception ex)
                        {
                            return Json(new { isValid = false, html = MyViewHelper.RenderRazorViewToString(this, "Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier, Message = ex.Message }) });
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", StaticVar.MessageCodeDuplicated);
                        return Json(new
                        {
                            isValid = false,
                            html = MyViewHelper.RenderRazorViewToString(this, "AddOrEdit", data)
                        });
                    }

                }
                return Json(new
                {
                    isValid = true,
                    html = MyViewHelper.RenderRazorViewToString(this, "_ViewAll")
                });
            }

            return Json(new { isValid = false, html = MyViewHelper.RenderRazorViewToString(this, "AddOrEdit", data) });
        }

        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            await ApiHelper<dynamic>.RunDeleteAsync($"{StaticVar.ApiUrlSystemFunctions}/{id}");
            return Json(new
            {
                html = MyViewHelper.RenderRazorViewToString(this, "_ViewAll")
            });
        }


        /* List Functions for ROLE */
        #region List Functions for ROLE 
        [NonAction]
        private async Task<dynamic> SetViewData(string idSelected = null)
        {
            // select list groups members
            List<GroupTest> groups = await ApiHelper<List<GroupTest>>.RunGetAsync(StaticVar.ApiUrlGroups);
            groups.Insert(0, new GroupTest
            {
                Code = String.Empty,
                Name = "Chọn nhóm người dùng"
            });
            ViewBag.Group = new SelectList(groups, "Code", "Name", idSelected);
            return ViewBag;
        }

        [HttpGet]
        public async Task<IActionResult> Role()
        {
            await SetViewData();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Role(string group)
        {
            
            if (!String.IsNullOrEmpty(group))
            {
                // Check detail group isset
                if (await ApiHelper<GroupTest>.RunGetAsync($"{StaticVar.ApiUrlGroups}/GetDetails/{group}") is GroupTest DataGroup)
                {
                    List<UserTest> listUserSelected = await ApiHelper<List<UserTest>>.RunGetAsync($"{StaticVar.ApiUrlUsers}/GetListUserByGroup/{DataGroup.Code}");

                    List<MenuTest> _temp = new List<MenuTest>();
                    _temp = GetTrees(ref _temp, await ApiHelper<List<MenuTest>>.RunGetAsync(StaticVar.ApiUrlMenus));

                    FunctionByGroup functionByGroup = new FunctionByGroup
                    {
                        // get list system functions full
                        ListSysFuntions = await ApiHelper<List<SystemFunctionTest>>.RunGetAsync(StaticVar.ApiUrlSystemFunctions),
                        // get list menus full (ordered by parent and child )
                        ListMenus = _temp,
                        // set selected to switch bootstrap and dual selectlist user
                        ArrSystemFunctionsSelected = DataGroup.ArrFunctionId,
                        ArrMenusSelected = DataGroup.ArrMenuId,
                        ArrUsersAll = new MultiSelectList(
                            await ApiHelper<List<UserTest>>.RunGetAsync(StaticVar.ApiUrlUsers),
                            "Code",
                            "FullName",
                            listUserSelected.Select(x => x.Code)
                        )
                    };

                    return Json(new
                    {
                        isValid = true,
                        html = MyViewHelper.RenderRazorViewToString(this, "_ViewRole", functionByGroup),
                        mes = StaticVar.MessageOkLoadData,
                        type = "success"
                    });
                }
                else
                {
                    return Json(new
                    {
                        isValid = false,
                        html = MyViewHelper.RenderRazorViewToString(this, "Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier, Message = StaticVar.MessageNotFound })
                    });
                }
            }
            else
            {
                return Json(new
                {
                    isValid = true,
                    html = MyViewHelper.RenderRazorViewToString(this, "_ViewRole"),
                    mes = "Chưa Chọn Nhóm",
                    type = "warning"
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ChangeFunction(string group, string id)
        {
            try
            {
                if(!String.IsNullOrEmpty(group) && !String.IsNullOrEmpty(id))
                {
                    await ApiHelper<GroupTest>.RunPostAsync($"{StaticVar.ApiUrlGroups}/SetPermission/{group}/{id}");
                    return Json(new{ isValid = true });
                }
                else
                {
                    return Json(new
                    {
                        isValid = false,
                        mes = StaticVar.MessageNotFound,
                        type = "warning"
                    });
                }
            }
            catch(Exception ex)
            {
                return Json(new
                {
                    isValid = false,
                    mes = ex.Message,
                    type = "error"
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ChangeMenu(string group, string id)
        {
            try
            {
                if (!String.IsNullOrEmpty(group) && !String.IsNullOrEmpty(id))
                {
                    await ApiHelper<GroupTest>.RunPostAsync($"{StaticVar.ApiUrlGroups}/SetMenu/{group}/{id}");
                    return Json(new { isValid = true });
                }
                else
                {
                    return Json(new
                    {
                        isValid = false,
                        mes = StaticVar.MessageNotFound,
                        type = "warning"
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    isValid = false,
                    mes = ex.Message,
                    type = "error"
                });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeUser(IFormCollection _collection)
        {
            if(_collection != null)
            {
                var group = _collection["groupId"];
                if (!String.IsNullOrEmpty(group))
                {
                    var selectedUsers = _collection["listUsers"].ToList();
                    if(selectedUsers.Count > 0)
                    {
                        try
                        {
                            await ApiHelper<dynamic>.RunPostAsync($"{StaticVar.ApiUrlUsers}/SetGroup/{group}", selectedUsers);
                            return Json(new
                            {
                                isValid = true,
                                mes = StaticVar.MessageOk
                            });

                        }
                        catch(Exception ex)
                        {
                            return Json(new
                            {
                                isValid = false,
                                mes = ex.Message,
                                type = "error"
                            });
                        }
                    }
                    else
                    {
                        return Json(new
                        {
                            isValid = false,
                            mes = "Chưa chọn người",
                            type = "warning"
                        });
                    }
                }
            }
            return View();
        }
        #endregion
    }
}