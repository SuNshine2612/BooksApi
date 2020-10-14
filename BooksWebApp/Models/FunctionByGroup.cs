using BooksApi.Models.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BooksWebApp.Models
{
    public class FunctionByGroup
    {
        public List<SystemFunctionTest> ListSysFuntions { get; set; }
        public List<UserTest> ListUsers { get; set; }
        public List<MenuTest> ListMenus { get; set; }
    }
}
