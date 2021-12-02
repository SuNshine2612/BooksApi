using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using BooksApi.Models.Book;
using BooksApi.Services;
using BooksWebApp.Helper;
using BooksWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BooksWebApp.Controllers
{
    public class BaseController : Controller
    {
        [NonAction]
        protected List<MenuTest> GetTrees(ref List<MenuTest> myTrees, List<MenuTest> myList, string space = "", string parent = default)
        {
            if (myTrees == null)
            {
                myTrees = new List<MenuTest>();
            }

            List<MenuTest> _myList = myList.Where(m => m.Parent == parent).OrderBy(m => m.Sort).ToList();

            foreach (var row in _myList)
            {
                myTrees.Add(new MenuTest
                {
                    Id = row.Id,
                    Code = row.Code,
                    Name = $"{space}{row.Name}",
                    Sort = row.Sort,
                    Url = row.Url,
                    Icon = row.Icon,
                    Parent = row.Parent
                });
                myTrees = GetTrees(ref myTrees, myList, $"{space}| - - ", row.Code);
            }

            return myTrees;
        }

        [NonAction]
        public async Task<dynamic> SetViewBookData()
        {
            // select list user to choose author ! Always use try/catch when use ApiHeper
            try
            {
                var authors = await ApiHelper<List<UserTest>>.RunGetAsync(StaticVar.ApiUrlUsers);
                ViewBag.Author = new SelectList(authors, "Id", "FullName");

                var categories = await ApiHelper<List<Category>>.RunGetAsync(StaticVar.ApiUrlCategories);
                ViewBag.Categories = new SelectList(categories, "Id", "Name");

                return ViewBag;
            }
            catch (Exception ex)
            {
                return View(viewName: "Error", model: new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier, Message = ex.Message });
            }
        }

        [NonAction]
        public async Task<dynamic> SetViewCommentData()
        {
            try
            {
                var customers = await ApiHelper<List<CustomerTest>>.RunGetAsync(StaticVar.ApiUrlCustomers);
                ViewBag.Customers = new SelectList(customers, "Id", "FullName");

                return ViewBag;
            }
            catch (Exception ex)
            {
                return View(viewName: "Error", model: new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier, Message = ex.Message });
            }
        }
    }
}