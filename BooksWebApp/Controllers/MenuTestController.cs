using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using BooksApi.Models.Global;
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
    public class MenuTestController : Controller
    {
        [NonAction]
        private async Task<dynamic> SetViewData(string idSelected = null)
        {
            // select list parent menus !
            List<MenuTest> parents = await ApiHelper<List<MenuTest>>.RunGetAsync(StaticVar.ApiUrlMenus);
            parents = parents.Where(m => m.Parent == null || m.Parent == "").ToList();
            parents.Insert(0, new MenuTest
            {
                Code = "", Name = "-- Chọn menu cha"
            });
            ViewBag.Parent = new SelectList(parents, "Code", "Name", idSelected);
            return ViewBag;
        }

        [NonAction]
        private List<MenuTest> GetTrees(ref List<MenuTest> myTrees, List<MenuTest> myList, string space = "", string parent = default)
        {
            if (myTrees == null)
            {
                myTrees = new List<MenuTest>();
            }

            List<MenuTest> _myList = myList.Where(m => m.Parent == parent).OrderBy(m => m.Sort).ToList();

            foreach (var row in _myList)
            {
                myTrees.Add(new MenuTest
                {
                    Id = row.Id,
                    Code = row.Code,
                    Name = $"{space}{row.Name}",
                    Sort = row.Sort,
                    Url = row.Url,
                    Icon = row.Icon,
                    Parent = row.Parent
                });
                myTrees = GetTrees(ref myTrees, myList, $"{space}| - - ", row.Code);
            }

            return myTrees;
        }

        [NonAction]
        private async Task<ListWithStatusViewModel<MenuTest>> GetListMenu()
        {
            List<MenuTest> _temp = new List<MenuTest>();
            _temp = GetTrees(ref _temp, await ApiHelper<List<MenuTest>>.RunGetAsync(StaticVar.ApiUrlMenus));
            var CustomeViewModel = new ListWithStatusViewModel<MenuTest>()
            {
                MyList = _temp
            };
            
            return CustomeViewModel;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                return View(await GetListMenu());
            }
            catch(Exception ex)
            {
                return Json(new { html = MyViewHelper.RenderRazorViewToString(this, "Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier, Message = ex.Message }) });
            }
        }

        [HttpGet]
        [NoDirectAccess]
        public async Task<IActionResult> AddOrEdit(string id)
        {
            if (String.IsNullOrEmpty(id))
            {
                await SetViewData();
                return View(new MenuTest());
            }  
            else
            {
                if (!(await ApiHelper<MenuTest>.RunGetAsync($"{StaticVar.ApiUrlMenus}/GetDetails/{id}") is MenuTest _data))
                {
                    return NotFound();
                }
                await SetViewData(_data.Parent);
                return View(_data);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit(string id, MenuTest data)
        {
            if (ModelState.IsValid)
            {
                //Insert
                if (String.IsNullOrEmpty(id))
                {
                    // check isset code ?
                    if (await ApiHelper<bool>.CheckIssetCode($"{StaticVar.ApiUrlMenus}/ExistsCode/{data.Code}"))
                    {
                        ModelState.AddModelError("", StaticVar.MessageCodeDuplicated);
                        await SetViewData(data.Parent);
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
                            MenuTest result = await ApiHelper<MenuTest>.RunPostAsync(StaticVar.ApiUrlMenus, data);
                        }
                        catch (Exception ex)
                        {
                            await SetViewData();
                            return Json(new { isValid = false, html = MyViewHelper.RenderRazorViewToString(this, "Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier, Message = ex.Message }) });
                        }
                    }

                }
                //Update
                else
                {
                    try
                    {
                        await ApiHelper<MenuTest>.RunPutAsync($"{StaticVar.ApiUrlMenus}/{id}", data);
                    }
                    catch (Exception ex)
                    {
                        return Json(new { isValid = false, html = MyViewHelper.RenderRazorViewToString(this, "Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier, Message = ex.Message }) });
                    }
                }
                return Json(new
                {
                    isValid = true,
                    html = MyViewHelper.RenderRazorViewToString(this, "_ViewAll", await GetListMenu())
                });
            }
            else
            {
                await SetViewData(data.Parent);
            }
            return Json(new { isValid = false, html = MyViewHelper.RenderRazorViewToString(this, "AddOrEdit", data) });
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var result = await ApiHelper<dynamic>.RunDeleteAsync($"{StaticVar.ApiUrlMenus}/{id}");
            return Json(new
            {
                html = MyViewHelper.RenderRazorViewToString(this, "_ViewAll", await ApiHelper<List<MenuTest>>.RunGetAsync(StaticVar.ApiUrlMenus))
            });
        }

        [HttpPost, ActionName("Change")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeSort(StatusUpdate statusUpdate)
        {
            try
            {
                await ApiHelper<dynamic>.RunPostAsync($"{StaticVar.ApiUrlMenus}/ChangeStatus", statusUpdate);
                return Json(new { isValid = true, html = MyViewHelper.RenderRazorViewToString(this, "_ViewAll", await GetListMenu()) });
            }
            catch(Exception ex)
            {
                return Json(new { isValid = false, html = MyViewHelper.RenderRazorViewToString(this, "Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier, Message = ex.Message }) });
            }
        }
    }
}