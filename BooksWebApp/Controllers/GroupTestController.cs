using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using BooksApi.Models.Book;
using BooksApi.Services;
using BooksWebApp.Helper;
using BooksWebApp.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BooksWebApp.Controllers
{
    [Authorize]
    public class GroupTestController : Controller
    {
        public async Task<IActionResult> Index()
        {
            try
            {
                return View(await ApiHelper<List<GroupTest>>.RunGetAsync(StaticVar.ApiUrlGroups));
            }
            catch(Exception ex)
            {
                if (ex.Message.Equals(StaticVar.ExpiredToken))
                {
                    // Clear Cookies !!
                    await HttpContext.SignOutAsync("CookieAuthentication");
                    return RedirectToAction("Login", "UserTest");
                }
                else
                {
                    return View(viewName: "Error", model: new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier, Message = ex.Message });
                }
            }
        }

        [HttpGet]
        [NoDirectAccess]
        public async Task<IActionResult> AddOrEdit(string id)
        {
            if (String.IsNullOrEmpty(id))
                return View(new GroupTest());
            else
            {
                try
                {
                    var _data = await ApiHelper<GroupTest>.RunGetAsync($"{StaticVar.ApiUrlGroups}/GetDetails/{id}");
                    if (_data == null)
                    {
                        return NotFound();
                    }
                    return View(_data);
                }
                catch(Exception ex)
                {
                    return Json(new { isValid = false, mes = ex.Message });
                }
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
                    try
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
                    catch(Exception ex)
                    {
                        return Json(new { isValid = false, mes = ex.Message });
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
            try
            {
                await ApiHelper<dynamic>.RunDeleteAsync($"{StaticVar.ApiUrlGroups}/{id}");
                return Json(new
                {
                    html = MyViewHelper.RenderRazorViewToString(this, "_ViewAll", await ApiHelper<List<GroupTest>>.RunGetAsync(StaticVar.ApiUrlGroups))
                });
            }
            catch(Exception ex)
            {
                return Json(new { isValid = false, mes = ex.Message });
            }
        }
    }
}