using BooksApi.Models.Test;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BooksWebApp.Models
{
    public class FunctionByGroup
    {
        public List<SystemFunctionTest> ListSysFuntions { get; set; }
        //public List<UserTest> ListUsers { get; set; }
        public List<MenuTest> ListMenus { get; set; }

        public string[] ArrSystemFunctionsSelected { get; set; }
        public string[] ArrMenusSelected { get; set; }
        public MultiSelectList ArrUsersAll { get; set; }
    }
}
