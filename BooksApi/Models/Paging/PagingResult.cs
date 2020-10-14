using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BooksApi.Models.Paging
{
    public class PagingResult<T> where T : class
    {
        public string Draw { get; set; }
        public long RecordsTotal { get; set; }
        public long RecordsFiltered { get; set; }
        public List<T> Data { get; set; }
    }
}
