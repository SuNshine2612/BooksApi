using BooksApi.Models.Book;
using BooksApi.Services;
using BooksWebApp.Controllers;
using BooksWebApp.Helper;
using BooksWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace BooksWebApp.ViewComponents
{
    [ViewComponent(Name = "MenuLeft")]
    public class MenuLayoutAdmin : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            try
            {
                List<MenuTest> menu = await ApiHelper<List<MenuTest>>.RunGetAsync(StaticVar.ApiUrlMenus);
                MenuViewModel MenuView = new()
                {
                    ListParent = menu.Where(m => string.IsNullOrEmpty(m.Parent)).OrderBy(m => m.Sort).ToList(),
                    ListChild = menu.Where(m => !string.IsNullOrEmpty(m.Parent)).OrderBy(m => m.Sort).ToList()
                };
                return View(MenuView);
            }
            catch(Exception ex)
            {
                return View(viewName: "Error", model: new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier, Message = ex.Message });
            }
        }

    }

    
    /*public class MenuLeftViewComponent : ViewComponent
    {

    }*/
}
