using BooksApi.Models.Book;
using BooksWebClient.Models.Global;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BooksWebClient.Controllers
{
    public class Test2Controller : Controller
    {
        readonly IMongoRepository<Category> _category;
        public Test2Controller(IMongoRepository<Category> repository)
        {
            _category = repository;
        }

        public IActionResult Index() => Json(_category.AsQueryable());
    }
}
