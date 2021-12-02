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
    public class CategoryController : Controller
    {
        public async Task<IActionResult> Index()
        {
            try
            {
                return View(await ApiHelper<List<Category>>.RunGetAsync(StaticVar.ApiUrlCategories));
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
                return View(new Category());
            }
            else
            {
                try
                {
                    if (await ApiHelper<Category>.RunGetAsync($"{StaticVar.ApiUrlCategories}/GetDetails/{id}") is not Category _data)
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
        public async Task<IActionResult> AddOrEdit(string id, Category data)
        {
            if (ModelState.IsValid)
            {
                #region Insert
                if (string.IsNullOrEmpty(id))
                {
                    try
                    {
                        // Check isset code ?
                        if (await ApiHelper<bool>.CheckIssetCode($"{StaticVar.ApiUrlCategories}/ExistsCode/{data.Code}"))
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
                                Category result = await ApiHelper<Category>.RunPostAsync(StaticVar.ApiUrlCategories, data);
                            }
                            catch (Exception ex)
                            {

                                return Json(new { isValid = false, html = MyViewHelper.RenderRazorViewToString(this, "Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier, Message = ex.Message }) });
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        return Json(new { isValid = false, mes = ex.Message });
                    }
                }
                #endregion
                #region Update
                else
                {
                    // Check isset code , but allow itself !!
                    Category _object = await ApiHelper<Category>.RunGetAsync($"{StaticVar.ApiUrlCategories}/GetDetails/{data.Code}");
                    if (_object == null || _object.Id.Equals(data.Id))
                    {
                        try
                        {
                            await ApiHelper<Category>.RunPutAsync($"{StaticVar.ApiUrlCategories}/{id}", data);
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
                #endregion
                return Json(new
                {
                    isValid = true,
                    html = MyViewHelper.RenderRazorViewToString(this, "_ViewAll", await ApiHelper<List<Category>>.RunGetAsync(StaticVar.ApiUrlCategories))
                });
            }

            return Json(new { isValid = false, html = MyViewHelper.RenderRazorViewToString(this, "AddOrEdit", data) });
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            try
            {
                await ApiHelper<dynamic>.RunDeleteAsync($"{StaticVar.ApiUrlCategories}/{id}");
                return Json(new
                {
                    html = MyViewHelper.RenderRazorViewToString(this, "_ViewAll", await ApiHelper<List<Category>>.RunGetAsync(StaticVar.ApiUrlCategories))
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
