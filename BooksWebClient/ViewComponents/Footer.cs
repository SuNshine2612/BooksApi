using BooksApi.Models.Book;
using BooksWebClient.Models.Global;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BooksWebClient.ViewComponents
{
    [ViewComponent(Name = "Footer")]
    public class FooterComponent : ViewComponent
    {
        readonly MongoRepository<Config> _config;

        public FooterComponent(IDatabaseSettings setting)
        {
            _config = new(setting);
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var filterConfig = Builders<Config>.Filter.Eq(x => x.IsActive, true)
                & Builders<Config>.Filter.Eq(x => x.Code, "bookstore");
            return View(await _config.FindOneAsync(filterConfig));
        }
    }
}
