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
    public class ConfigController : Controller
    {
        public async Task<IActionResult> Index()
        {
            try
            {
                return View(await ApiHelper<List<Config>>.RunGetAsync(StaticVar.ApiUrlConfigs));
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
                return View(new Config());
            }
            else
            {
                try
                {
                    if (await ApiHelper<Config>.RunGetAsync($"{StaticVar.ApiUrlConfigs}/GetDetails/{id}") is not Config _data)
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
        public async Task<IActionResult> AddOrEdit(string id, Config data)
        {
            if (ModelState.IsValid)
            {
                #region Insert
                if (string.IsNullOrEmpty(id))
                {
                    try
                    {
                        if (await ApiHelper<bool>.CheckIssetCode($"{StaticVar.ApiUrlConfigs}/ExistsCode/{data.Code}"))
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
                                Config result = await ApiHelper<Config>.RunPostAsync(StaticVar.ApiUrlConfigs, data);
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
                    Config _object = await ApiHelper<Config>.RunGetAsync($"{StaticVar.ApiUrlConfigs}/GetDetails/{data.Code}");
                    if (_object == null || _object.Id.Equals(data.Id))
                    {
                        try
                        {
                            await ApiHelper<Config>.RunPutAsync($"{StaticVar.ApiUrlConfigs}/{id}", data);
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
                    html = MyViewHelper.RenderRazorViewToString(this, "_ViewAll", await ApiHelper<List<Config>>.RunGetAsync(StaticVar.ApiUrlConfigs))
                }) ;
            }
            return Json(new { isValid = false, html = MyViewHelper.RenderRazorViewToString(this, "AddOrEdit", data) });
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            try
            {
                await ApiHelper<dynamic>.RunDeleteAsync($"{StaticVar.ApiUrlConfigs}/{id}");
                return Json(new
                {
                    html = MyViewHelper.RenderRazorViewToString(this, "_ViewAll", await ApiHelper<List<Config>>.RunGetAsync(StaticVar.ApiUrlConfigs))
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


        [HttpGet]
        public async Task<IActionResult> Update(string id)
        {
            try
            {
                Config config = await ApiHelper<Config>.RunGetAsync($"{StaticVar.ApiUrlConfigs}/GetDetails/{id}");
                if (config is null)
                    return Redirect("/");
                return View(config);
            }
            catch (Exception)
            {
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(string id, Config data)
        {
            // Check isset code , but allow itself !!
            Config _object = await ApiHelper<Config>.RunGetAsync($"{StaticVar.ApiUrlConfigs}/GetDetails/{id}");
            if (_object == null || _object.Code.Equals(data.Code))
            {
                await ApiHelper<Config>.RunPutAsync($"{StaticVar.ApiUrlConfigs}/{data.Id}", data);
                TempData["message"] = StaticVar.MessageOk;
            }
            else
            {
                ModelState.AddModelError("", StaticVar.MessageCodeDuplicated);
            }
            return View(data);
        }
    }
}
