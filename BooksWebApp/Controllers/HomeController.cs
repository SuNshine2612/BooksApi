using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BooksWebApp.Models;
using BooksApi.Models.Test;
using BooksWebApp.Helper;
using BooksApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using Microsoft.AspNetCore.Authentication;

namespace BooksWebApp.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        [NonAction]
        private  async Task<dynamic> SetViewData(string idSelected = null)
        {
            // select list user to choose author !
            var authors = await ApiHelper<List<UserTest>>.RunGetAsync(StaticVar.ApiUrlUsers);
            ViewBag.Author = new SelectList(authors, "Code", "FullName", idSelected);
            return ViewBag;
        }

        [NonAction]
        private async Task<List<Book>> GetListBooks()
        {

            List<Book> _books = await ApiHelper<List<Book>>.RunGetAsync(StaticVar.ApiUrlBooks);
            List<UserTest> _authors = await ApiHelper<List<UserTest>>.RunGetAsync(StaticVar.ApiUrlUsers);

            var query = from b in _books
                        join a in _authors on b.Author equals a.Code
                        select new Book
                        {
                            Id = b.Id,
                            Code = b.Code,
                            Name = b.Name,
                            Category = b.Category,
                            AuthorName = a.FullName,
                            Price = b.Price
                        };
            return query.ToList();

        }

        // Must check expired token !!! Redirect lo login page ! 
        public async Task<IActionResult> Index()
        {
            // return normal
            //return View(await ApiHelper<List<Book>>.RunGetAsync(StaticVar.ApiUrlBooks));
            // return with name author
            try
            {
                return View(await GetListBooks());
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
                    return Json(new
                    {
                        isValid = false,
                        html = MyViewHelper.RenderRazorViewToString(this, "Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier, Message = ex.Message })
                    });
                }
            }
        }

        [HttpGet]
        public async Task<IActionResult> AddOrEdit(string id)
        {
            if (String.IsNullOrEmpty(id))
            {
                 await SetViewData();
                return View(new Book());
            }
            else
            {
                if (!(await ApiHelper<Book>.RunGetAsync($"{StaticVar.ApiUrlBooks}/GetDetails/{id}") is Book _data))
                {
                    return NotFound();
                }
                await SetViewData(_data.Author);
                return View(_data);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit(string id, Book data)
        {
            if (ModelState.IsValid)
            {
                //Insert
                if (String.IsNullOrEmpty(id))
                {
                    // Check isset code ?
                    if(await ApiHelper<bool>.CheckIssetCode($"{StaticVar.ApiUrlBooks}/ExistsCode/{data.Code}")){
                        ModelState.AddModelError("", StaticVar.MessageCodeDuplicated);
                        await SetViewData(data.Author);
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
                            Book result = await ApiHelper<Book>.RunPostAsync(StaticVar.ApiUrlBooks, data);
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
                    // Check isset code , but allow itself !!
                    Book _object = await ApiHelper<Book>.RunGetAsync($"{StaticVar.ApiUrlBooks}/GetDetails/{data.Code}");
                    if (_object == null || _object.Id.Equals(data.Id))
                    {
                        try
                        {
                            await ApiHelper<Book>.RunPutAsync($"{StaticVar.ApiUrlBooks}/{id}", data);
                        }
                        catch (Exception ex)
                        {
                            await SetViewData(data.Author);
                            return Json(new { isValid = false, html = MyViewHelper.RenderRazorViewToString(this, "Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier, Message = ex.Message }) });
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Mã sách bị trùng !!");
                        await SetViewData(data.Author);
                        return Json(new
                        {
                            isValid = false,
                            html = MyViewHelper.RenderRazorViewToString(this, "AddOrEdit", data)
                        });
                    }
                    
                }
                return Json(new { 
                        isValid = true, 
                        html = MyViewHelper.RenderRazorViewToString(this, "_ViewAll", await GetListBooks())
                    });
            }
            else
            {
                await SetViewData(data.Author);
            }
            return Json(new { isValid = false, html = MyViewHelper.RenderRazorViewToString(this, "AddOrEdit", data) });
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            await ApiHelper<dynamic>.RunDeleteAsync($"{StaticVar.ApiUrlBooks}/{id}");
            return Json(new { 
                html = MyViewHelper.RenderRazorViewToString(this, "_ViewAll", await GetListBooks()) 
            });
        }

    }
}
