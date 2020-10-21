using System;
using System.Diagnostics;
using System.Threading.Tasks;
using BooksApi.Models.Paging;
using BooksApi.Models.Test;
using BooksApi.Services;
using BooksWebApp.Helper;
using BooksWebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BooksWebApp.Controllers
{
    [Authorize]
    public class CustomerTestController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            // return View(await ApiHelper<List<CustomerTest>>.RunGetAsync(StaticVar.ApiUrlCustomers));
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(IFormCollection request)
        {
            try
            {
                // before send to API, we need init DataPaging !!
                var database = ApiHelper<DatatablesPaging>.InitDatatable(request);
                // after init, we send to API - use for DataTable ( paging ) !!
                var listItems = await ApiHelper<dynamic>.RunPostAsync($"{StaticVar.ApiUrlCustomers}/Paging", database);

                if(listItems == null)
                {
                    return Json(new
                    {
                        isValid = false,
                        html = MyViewHelper.RenderRazorViewToString(this, "Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier, Message = StaticVar.MessageNotFound })
                    });
                }
                else
                {
                    return Json(listItems);
                }
            }
            catch(Exception ex)
            {
                return Json(new { isValid = false, mes = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> AddOrEdit(string id)
        {
            if (String.IsNullOrEmpty(id))
            {
                return View(new CustomerTest());
            }
            else
            {
                try
                {
                    if (!(await ApiHelper<CustomerTest>.RunGetAsync($"{StaticVar.ApiUrlCustomers}/GetDetails/{id}") is CustomerTest _data))
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
        public async Task<IActionResult> AddOrEdit(string id, CustomerTest data)
        {
            if (ModelState.IsValid)
            {
                //Insert
                if (String.IsNullOrEmpty(id))
                {
                    try
                    {
                        // Check isset code ?
                        if (await ApiHelper<bool>.CheckIssetCode($"{StaticVar.ApiUrlCustomers}/ExistsCode/{data.Code}"))
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
                                CustomerTest result = await ApiHelper<CustomerTest>.RunPostAsync(StaticVar.ApiUrlCustomers, data);
                            }
                            catch (Exception ex)
                            {

                                return Json(new { isValid = false, html = MyViewHelper.RenderRazorViewToString(this, "Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier, Message = ex.Message }) });
                            }
                        }
                    }
                    catch(Exception ex)
                    {
                        return Json(new { isValid = false, mes = ex.Message});
                    }
                }
                //Update
                else
                {
                    // Check isset code , but allow itself !!
                    CustomerTest _object = await ApiHelper<CustomerTest>.RunGetAsync($"{StaticVar.ApiUrlCustomers}/GetDetails/{data.Code}");
                    if (_object == null || _object.Id.Equals(data.Id))
                    {
                        try
                        {
                            await ApiHelper<CustomerTest>.RunPutAsync($"{StaticVar.ApiUrlCustomers}/{id}", data);
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
                return Json(new
                {
                    isValid = true,
                    html = MyViewHelper.RenderRazorViewToString(this, "_ViewAll")
                });
            }

            return Json(new { isValid = false, html = MyViewHelper.RenderRazorViewToString(this, "AddOrEdit", data) });
        }

        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            try
            {
                await ApiHelper<dynamic>.RunDeleteAsync($"{StaticVar.ApiUrlCustomers}/{id}");
                return Json(new
                {
                    html = MyViewHelper.RenderRazorViewToString(this, "_ViewAll")
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