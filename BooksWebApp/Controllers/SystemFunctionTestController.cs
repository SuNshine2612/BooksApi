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
    public class SystemFunctionTestController : Controller
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
                return View();
            }
            else
            {
                return View(new FunctionByGroup());
            }
        }
    }
}