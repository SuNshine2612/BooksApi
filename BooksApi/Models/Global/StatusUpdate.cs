using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BooksApi.Models.Global
{
    public class StatusUpdate
    {
        public string FilterName { get; set; } 
        public string FilterValue { get; set; }
        public string UpdateName { get; set; }
        public int UpdateValue { get; set; }
    }
}
