using BooksApi.Models.Book;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BooksWebApp.Models
{
    public class MenuViewModel
    {
        public List<MenuTest> ListParent { get; set; }
        public List<MenuTest> ListChild { get; set; }
    }
}
