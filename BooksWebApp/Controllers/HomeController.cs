using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BooksWebApp.Models;
using BooksApi.Models.Book;
using BooksWebApp.Helper;
using BooksApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace BooksWebApp.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        // Join ở đây không đúng lắm ?? Join ở api r trả về sẽ hợp lý hơn
        [NonAction]
        static async Task<List<Book>> GetListBooks()
        {
            #region Sử dụng linq để join tại client ???
            /*List<Book> _books = await ApiHelper<List<Book>>.RunGetAsync(StaticVar.ApiUrlBooks);
            List<UserTest> _authors = await ApiHelper<List<UserTest>>.RunGetAsync(StaticVar.ApiUrlUsers);

            var query = from b in _books
                        join a in _authors on b.Author equals a.Id
                        select new Book
                        {
                            Id = b.Id,
                            Code = b.Code,
                            Name = b.Name,
                            Category = b.Category,
                            AuthorName = a.FullName,
                            Price = b.Price
                        };
            return query.ToList();*/
            #endregion

            return await ApiHelper<List<Book>>.RunGetAsync(StaticVar.ApiUrlBooks);
        }

        // Must check expired token !!! Redirect lo login page ! 
        public async Task<IActionResult> Index()
        {
            try
            {
                // return normal
                //return View(await ApiHelper<List<Book>>.RunGetAsync(StaticVar.ApiUrlBooks));
                // return with name author
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
                    return View(viewName: "Error", model: new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier, Message = ex.Message });
                }
            }
        }

        [HttpGet]
        public async Task<IActionResult> AddOrEdit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                 await SetViewBookData();
                return View(new Book());
            }
            else
            {
                try
                {
                    if (await ApiHelper<Book>.RunGetAsync($"{StaticVar.ApiUrlBooks}/GetDetails/{id}") is not Book _data)
                    {
                        return NotFound();
                    }
                    await SetViewBookData();
                    return View(_data);
                }
                catch(Exception ex)
                {
                    // Because call ajax, we must return Json Result !! Not return View()
                    //return View(viewName: "Error", model: new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier, Message = ex.Message });
                    return Json(new { isValid = false, mes = ex.Message });
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit(string id, Book data)
        {
            if (ModelState.IsValid)
            {
                #region Insert
                if (string.IsNullOrEmpty(id))
                {
                    try
                    {
                        if (await ApiHelper<bool>.CheckIssetCode($"{StaticVar.ApiUrlBooks}/ExistsCode/{data.Code}"))
                        {
                            ModelState.AddModelError("", StaticVar.MessageCodeDuplicated);
                            await SetViewBookData();
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
                                await SetViewBookData();
                                return Json(new { isValid = false, html = MyViewHelper.RenderRazorViewToString(this, "Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier, Message = ex.Message }) });
                            }
                        }
                    }
                    catch(Exception ex)
                    {
                        return Json(new { isValid = false, mes = ex.Message });
                    }
                }
                #endregion
                #region Update
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
                            await SetViewBookData();
                            return Json(new { isValid = false, html = MyViewHelper.RenderRazorViewToString(this, "Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier, Message = ex.Message }) });
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Mã sách bị trùng !!");
                        await SetViewBookData();
                        return Json(new
                        {
                            isValid = false,
                            html = MyViewHelper.RenderRazorViewToString(this, "AddOrEdit", data)
                        });
                    }
                    
                }
                #endregion
                return Json(new { 
                    isValid = true, 
                    html = MyViewHelper.RenderRazorViewToString(this, "_ViewAll", await GetListBooks())
                });
            }
            else
            {
                await SetViewBookData();
            }
            return Json(new { isValid = false, html = MyViewHelper.RenderRazorViewToString(this, "AddOrEdit", data) });
        }


        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken] 
        // Nên giữ ! Bỏ khi dùng SSE !!
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            try
            {
                await ApiHelper<dynamic>.RunDeleteAsync($"{StaticVar.ApiUrlBooks}/{id}");
                return Json(new
                {
                    isValid = true,
                    html = MyViewHelper.RenderRazorViewToString(this, "_ViewAll", await GetListBooks())
                });
            }
            catch(Exception ex)
            {
                return Json(new { isValid = false, mes = ex.Message });
            }
        }

    }
}
