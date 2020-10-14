using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BooksApi.Models.Paging
{
    public class DatatablesPaging
    {
        public string Draw { get; set; }
        public string Start { get; set; }
        public string Length { get; set; }
        public string SortColumn { get; set; }
        public string SortColumnDirection { get; set; }
        //tìm kiếm toàn bộ các cột
        public string SearchValue { get; set; }
        //tìm kiếm theo giá trị của từng cột
        public Dictionary<string, string> SearchArray { get; set; }
        public int PageSize { get; set; }
        public int Skip { get; set; }
    }
}
