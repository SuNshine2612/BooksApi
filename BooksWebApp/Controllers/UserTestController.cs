using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BooksApi.Models.Test;
using BooksWebApp.Helper;
using Microsoft.AspNetCore.Mvc;
using BooksApi.Services;
using BooksWebApp.Models;
using System.Diagnostics;
using BooksApi.Models.TMS;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace BooksWebApp.Controllers
{
   
    [Authorize]
    public class UserTestController : Controller
    {
        public async Task<IActionResult> Index()
        {
            return View(await ApiHelper<List<UserTest>>.RunGetAsync(StaticVar.ApiUrlUsers));
        }

        [HttpGet]
        [NoDirectAccess]
        public async Task<IActionResult> AddOrEdit(string id)
        {
            if (String.IsNullOrEmpty(id))
                return View(new UserTest());
            else
            {
                var _data = await ApiHelper<UserTest>.RunGetAsync($"{StaticVar.ApiUrlUsers}/GetDetails/{id}");
                if (_data == null)
                {
                    return NotFound();
                }
                return View(_data);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit(string id, UserTest data)
        {
            if (ModelState.IsValid)
            {
                //Insert
                if (String.IsNullOrEmpty(id))
                {
                    // check isset username ?
                    if(await ApiHelper<bool>.CheckIssetCode($"{StaticVar.ApiUrlUsers}/ExistsCode/{data.Code}"))
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
                            UserTest result = await ApiHelper<UserTest>.RunPostAsync(StaticVar.ApiUrlUsers, data);
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
                        await ApiHelper<UserTest>.RunPutAsync($"{StaticVar.ApiUrlUsers}/{id}", data);
                    }
                    catch (Exception ex)
                    {
                        return Json(new { isValid = false, html = MyViewHelper.RenderRazorViewToString(this, "Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier, Message = ex.Message }) });
                    }
                }
                return Json(new
                {
                    isValid = true,
                    html = MyViewHelper.RenderRazorViewToString(this, "_ViewAll", await ApiHelper<List<UserTest>>.RunGetAsync($"{StaticVar.ApiUrlUsers}"))
                });
            }
            return Json(new { isValid = false, html = MyViewHelper.RenderRazorViewToString(this, "AddOrEdit", data) });
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            await ApiHelper<dynamic>.RunDeleteAsync($"{StaticVar.ApiUrlUsers}/{id}");
            return Json(new
            {
                html = MyViewHelper.RenderRazorViewToString(this, "_ViewAll", await ApiHelper<List<UserTest>>.RunGetAsync(StaticVar.ApiUrlUsers))
            });
        }


        [AllowAnonymous]
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }


        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(Authenticate auth)
        {
            try
            {
                string _token = await ApiHelper.GetUserToken(auth.Code, auth.Password);

                if(_token != null)
                {
                    var _tmp = _token;
                    var handler = new JwtSecurityTokenHandler();
                    var tokenS = handler.ReadToken(_tmp) as JwtSecurityToken;

                    var FullName = tokenS.Claims.Where(c => c.Type == StaticVar.ClaimName).FirstOrDefault();
                    var Email = tokenS.Claims.Where(c => c.Type == StaticVar.ClaimEmail).FirstOrDefault();

                    // save userClaim, not use SESSION !!
                    // https://www.c-sharpcorner.com/article/cookie-authentication-in-net-core-3-0/
                    var userClaims = new List<Claim>()
                    {
                        new Claim(ClaimTypes.Name, FullName?.Value),
                        new Claim(ClaimTypes.Email, Email?.Value),
                        new Claim(StaticVar.SessionUserToken, _token)
                    };
                    var grandmaIdentity = new ClaimsIdentity(userClaims, "User Identity");
                    var userPrincipal = new ClaimsPrincipal(new[] { grandmaIdentity });
                    await HttpContext.SignInAsync(userPrincipal);

                    // use session !! Not use cookies
                    //Helper.AppContext.Current.Session.SetString(StaticVar.SessionUserToken, _token);

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    if (!_token.Equals(""))
                    {
                        SetAlert(_token, "error");
                    }
                }
            }
            catch(Exception ex)
            {
                SetAlert(ex.Message, "error");
            }
            return View();
        }

        
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("CookieAuthentication");
            return RedirectToAction("Login");
        }

        [NonAction]
        protected void SetAlert(string Message, string Type)
        {
            TempData["AlertMessage"] = Message;
            if (Type == "success")
            {
                TempData["AlertType"] = "alert-success";
            }
            else if (Type == "warning")
            {
                TempData["AlertType"] = "alert-warning";
            }
            else if (Type == "error")
            {
                TempData["AlertType"] = "alert-danger";
            }

        }
    }
}