using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using BooksApi.Models.Test;
using BooksApi.Services;
using BooksWebApp.Helper;
using BooksWebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BooksWebApp.Controllers
{
    [Authorize]
    public class GroupTestController : Controller
    {
        public async Task<IActionResult> Index()
        {
            return View(await ApiHelper<List<GroupTest>>.RunGetAsync(StaticVar.ApiUrlGroups));
        }

        [HttpGet]
        [NoDirectAccess]
        public async Task<IActionResult> AddOrEdit(string id)
        {
            if (String.IsNullOrEmpty(id))
                return View(new GroupTest());
            else
            {
                var _data = await ApiHelper<GroupTest>.RunGetAsync($"{StaticVar.ApiUrlGroups}/GetDetails/{id}");
                if (_data == null)
                {
                    return NotFound();
                }
                return View(_data);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit(string id, GroupTest data)
        {
            if (ModelState.IsValid)
            {
                //Insert
                if (String.IsNullOrEmpty(id))
                {
                    // check isset username ?
                    if (await ApiHelper<bool>.CheckIssetCode($"{StaticVar.ApiUrlGroups}/ExistsCode/{data.Code}"))
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
                            GroupTest result = await ApiHelper<GroupTest>.RunPostAsync(StaticVar.ApiUrlGroups, data);
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
                    try
                    {
                        await ApiHelper<GroupTest>.RunPutAsync($"{StaticVar.ApiUrlGroups}/{id}", data);
                    }
                    catch (Exception ex)
                    {
                        return Json(new { isValid = false, html = MyViewHelper.RenderRazorViewToString(this, "Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier, Message = ex.Message }) });
                    }
                }
                return Json(new
                {
                    isValid = true,
                    html = MyViewHelper.RenderRazorViewToString(this, "_ViewAll", await ApiHelper<List<GroupTest>>.RunGetAsync($"{StaticVar.ApiUrlGroups}"))
                });
            }
            return Json(new { isValid = false, html = MyViewHelper.RenderRazorViewToString(this, "AddOrEdit", data) });
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var result = await ApiHelper<dynamic>.RunDeleteAsync($"{StaticVar.ApiUrlGroups}/{id}");
            return Json(new
            {
                html = MyViewHelper.RenderRazorViewToString(this, "_ViewAll", await ApiHelper<List<GroupTest>>.RunGetAsync(StaticVar.ApiUrlGroups))
            });
        }
    }
}