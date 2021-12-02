using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BooksApi.Models.Book;
using BooksApi.Services;
using BooksWebApp.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace BooksWebApp.Controllers
{
    [Authorize]
    public class SseController : Controller
    {
        [NonAction]
        static async Task<List<Book>> GetListBooks()
        {
            return await ApiHelper<List<Book>>.RunGetAsync(StaticVar.ApiUrlBooks);
            /*List<Book> _books = await ApiHelper<List<Book>>.RunGetAsync(StaticVar.ApiUrlBooks);
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
            return query.ToList();*/

        }

        #region Single SSE Table
        public IActionResult Index()
        {
            return View();
        }

        public async Task Process()
        {
            Response.ContentType = "text/event-stream";
            // Each object
            /*foreach (Book obj in await GetListBooks())
            {
                string jsonBook = JsonConvert.SerializeObject(obj);
                string data = $"data: {jsonBook}\n\n";
                System.Threading.Thread.Sleep(3000);
                await HttpContext.Response.WriteAsync(data);
                HttpContext.Response.Body.Flush();
            }*/

            // Full list
            string jsonBook = JsonConvert.SerializeObject(await GetListBooks());
            string data = $"data: {jsonBook}\n\n";
            System.Threading.Thread.Sleep(3000);
            await HttpContext.Response.WriteAsync(data);
            HttpContext.Response.Body.Flush();

            Response.Body.Close();
        }
        public async Task<IActionResult> Process2()
        {
            StringBuilder sb = new();
            foreach (Book obj in await GetListBooks())
            {
                string jsonBook = JsonConvert.SerializeObject(obj);
                sb.AppendFormat("data: {0}\n\n", jsonBook);
            }
            return Content(sb.ToString(), "text/event-stream");
        }
        #endregion

        public IActionResult MoreTable()
        {
            return View();
        }
    }
}