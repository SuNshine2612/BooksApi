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
    [ViewComponent(Name = "Header")]
    public class HeaderComponent : ViewComponent
    {
        readonly MongoRepository<Category> _category;
        readonly MongoRepository<Config> _config;

        public HeaderComponent(IDatabaseSettings setting)
        {
            _category = new(setting);
            _config = new(setting);
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var filterCategory = Builders<Category>.Filter.Eq(x => x.IsActive, true);
            ProjectionDefinition<Category, dynamic> projectCategory = new FindExpressionProjectionDefinition<Category, dynamic>(p => new
            {
                Code = p.Code.ToLower(),
                p.Name
            });

            var category = await _category.FilterByAsync(filterCategory, projectCategory);
            ViewBag.Category = category;

            var filterConfig = Builders<Config>.Filter.Eq(x => x.IsActive, true)
                & Builders<Config>.Filter.Eq(x => x.Code, "bookstore");
            var config = await _config.FindOneAsync(filterConfig);
            ViewBag.Config = config;

            return View();
        }
    }
}
