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
    public class CommentController : BaseController
    {
        public async Task<IActionResult> Index()
        {
            try
            {
                return View(await ApiHelper<List<Comment>>.RunGetAsync(StaticVar.ApiUrlComments));
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
                await SetViewCommentData();
                return View(new Comment());
            }
            else
            {
                try
                {
                    if (await ApiHelper<Comment>.RunGetAsync($"{StaticVar.ApiUrlComments}/GetDetails/{id}") is not Comment _data)
                    {
                        return NotFound();
                    }
                    await SetViewCommentData();
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
        public async Task<IActionResult> AddOrEdit(string id, Comment data)
        {
            if (ModelState.IsValid)
            {
                #region Insert
                if (string.IsNullOrEmpty(id))
                {
                    try
                    {
                        Comment result = await ApiHelper<Comment>.RunPostAsync(StaticVar.ApiUrlComments, data);
                    }
                    catch (Exception ex)
                    {
                        await SetViewCommentData();
                        return Json(new { isValid = false, html = MyViewHelper.RenderRazorViewToString(this, "Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier, Message = ex.Message }) });
                    }
                }
                #endregion
                #region Update
                else
                {
                    try
                    {
                        await ApiHelper<Comment>.RunPutAsync($"{StaticVar.ApiUrlComments}/{id}", data);
                    }
                    catch (Exception ex)
                    {
                        await SetViewCommentData();
                        return Json(new { isValid = false, html = MyViewHelper.RenderRazorViewToString(this, "Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier, Message = ex.Message }) });
                    }
                }
                #endregion
                return Json(new
                {
                    isValid = true,
                    html = MyViewHelper.RenderRazorViewToString(this, "_ViewAll", await ApiHelper<List<Comment>>.RunGetAsync(StaticVar.ApiUrlComments))
                });
            }

            return Json(new { isValid = false, html = MyViewHelper.RenderRazorViewToString(this, "AddOrEdit", data) });
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            try
            {
                await ApiHelper<dynamic>.RunDeleteAsync($"{StaticVar.ApiUrlComments}/{id}");
                return Json(new
                {
                    html = MyViewHelper.RenderRazorViewToString(this, "_ViewAll", await ApiHelper<List<Comment>>.RunGetAsync(StaticVar.ApiUrlComments))
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
