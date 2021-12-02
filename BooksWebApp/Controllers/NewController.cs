using BooksApi.Models.Book;
using BooksApi.Services;
using BooksWebApp.Helper;
using BooksWebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace BooksWebApp.Controllers
{
    [Authorize]
    public class NewController : Controller
    {
        public async Task<IActionResult> Index()
        {
            try
            {
                return View(await ApiHelper<List<News>>.RunGetAsync(StaticVar.ApiUrlNews));
            }
            catch (Exception ex)
            {
                return View(viewName: "Error", model: new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier, Message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> AddOrEdit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return View(new News());
            }
            else
            {
                try
                {
                    if (await ApiHelper<News>.RunGetAsync($"{StaticVar.ApiUrlNews}/GetDetails/{id}") is not News _data)
                    {
                        return NotFound();
                    }
                    return View(_data);
                }
                catch (Exception ex)
                {
                    return Json(new
                    {
                        isValid = false,
                        mes = ex.Message
                    });
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit(string id, News data)
        {
            if (ModelState.IsValid)
            {
                #region Insert
                if (string.IsNullOrEmpty(id))
                {
                    try
                    {
                        News result = await ApiHelper<News>.RunPostAsync(StaticVar.ApiUrlNews, data);
                    }
                    catch (Exception ex)
                    {
                        return Json(new { isValid = false, html = MyViewHelper.RenderRazorViewToString(this, "Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier, Message = ex.Message }) });
                    }
                }
                #endregion
                #region Update
                else
                {
                    try
                    {
                        await ApiHelper<News>.RunPutAsync($"{StaticVar.ApiUrlNews}/{id}", data);
                    }
                    catch (Exception ex)
                    {
                        return Json(new { isValid = false, html = MyViewHelper.RenderRazorViewToString(this, "Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier, Message = ex.Message }) });
                    }
                }
                #endregion
                return Json(new
                {
                    isValid = true,
                    html = MyViewHelper.RenderRazorViewToString(this, "_ViewAll", await ApiHelper<List<News>>.RunGetAsync(StaticVar.ApiUrlNews))
                });
            }

            return Json(new { isValid = false, html = MyViewHelper.RenderRazorViewToString(this, "AddOrEdit", data) });
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            try
            {
                await ApiHelper<dynamic>.RunDeleteAsync($"{StaticVar.ApiUrlNews}/{id}");
                return Json(new
                {
                    html = MyViewHelper.RenderRazorViewToString(this, "_ViewAll", await ApiHelper<List<News>>.RunGetAsync(StaticVar.ApiUrlNews))
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    isValid = false,
                    mes = ex.Message
                });
            }
        }
    }
}
