using BooksWebClient.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BooksWebClient.Controllers
{
    public class TestController : Controller
    {
        readonly TestRepository testRepository;
        public TestController(TestRepository _test)
        {
            testRepository = _test;
        }

        public IActionResult Index() => Json(testRepository.Read());
    }
}
