using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BooksApi.Models.Test;
using Microsoft.AspNetCore.Mvc;

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
    }
}