using BooksApi.Models.Book;
using BooksApi.Services;
using BooksWebClient.Models;
using BooksWebClient.Models.Global;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace BooksWebClient.Controllers
{
    public class HomeController : Controller
    {
        readonly MongoRepository<Category> _category;
        readonly MongoRepository<Book> _book;
        readonly MongoRepository<Slide> _slide;
        readonly MongoRepository<Comment> _comment;
        readonly MongoRepository<News> _news;

        public HomeController(IDatabaseSettings setting)
        {
            _category = new(setting);
            _book = new(setting);
            _slide = new(setting);
            _comment = new(setting);
            _news = new(setting);
        }
        
        public async Task<IActionResult> Index()
        {
            // Main slides home
            var filterMainSlide = Builders<Slide>.Filter.Eq(x => x.SlideType, StaticVar.MainSlide);
            var filterSubSlide = Builders<Slide>.Filter.Eq(x => x.SlideType, StaticVar.SubSlide);
            ViewBag.Slides = await _slide.FilterByAsync(filterMainSlide);
            ViewBag.SubSlides = await _slide.FilterByAsync(filterSubSlide);

            // Join with customer to show avatar !!???
            var collectionComment = _comment._collection;
            var comments = collectionComment.Aggregate()
                .Lookup("CustomerTest", "Customer", "_id", "ObjCustomers")
                .As<Comment>()
                .ToList();
            ViewBag.Comments = comments;

            // Blogs
            var news = _news.AsQueryable().Where(x => x.IsActive == true).OrderByDescending(x => x.Id).Take(2);
            ViewBag.News = news;

            // Books
            var books = _book.AsQueryable().Where(x => x.IsActive == true).OrderByDescending(x => x.Id).Take(8);
            ViewBag.Books = books;
            return View();
        }

        
        // Test data
        public IActionResult Category() => Json(_category.AsQueryable());
        public IActionResult Book() => Json(_book.AsQueryable());

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
