using BooksApi.Models.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BooksWebApp.Models
{
    public class ListWithStatusViewModel<T>
    {
        public StatusUpdate MyStatus { get; set; }
        public List<T> MyList { get; set; }

    }
}
