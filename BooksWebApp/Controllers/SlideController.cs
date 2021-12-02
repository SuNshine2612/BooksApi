using BooksApi.Models.Book;
using BooksApi.Services;
using BooksWebApp.Helper;
using BooksWebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace BooksWebApp.Controllers
{
    [Authorize]
    public class SlideController : Controller
    {
        public async Task<IActionResult> Index()
        {
            try
            {
                return View(await ApiHelper<List<Slide>>.RunGetAsync(StaticVar.ApiUrlSlides));
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
                return View(new Slide());
            }
            else
            {
                try
                {
                    if (await ApiHelper<Slide>.RunGetAsync($"{StaticVar.ApiUrlSlides}/GetDetails/{id}") is not Slide _data)
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
        public async Task<IActionResult> AddOrEdit(string id, Slide data)
        {
            if (ModelState.IsValid)
            {
                #region Insert
                if (string.IsNullOrEmpty(id))
                {
                    try
                    {
                        Slide result = await ApiHelper<Slide>.RunPostAsync(StaticVar.ApiUrlSlides, data);
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
                        await ApiHelper<Slide>.RunPutAsync($"{StaticVar.ApiUrlSlides}/{id}", data);
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
                    html = MyViewHelper.RenderRazorViewToString(this, "_ViewAll", await ApiHelper<List<Slide>>.RunGetAsync(StaticVar.ApiUrlSlides))
                });
            }

            return Json(new { isValid = false, html = MyViewHelper.RenderRazorViewToString(this, "AddOrEdit", data) });
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            try
            {
                await ApiHelper<dynamic>.RunDeleteAsync($"{StaticVar.ApiUrlSlides}/{id}");
                return Json(new
                {
                    html = MyViewHelper.RenderRazorViewToString(this, "_ViewAll", await ApiHelper<List<Slide>>.RunGetAsync(StaticVar.ApiUrlSlides))
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
