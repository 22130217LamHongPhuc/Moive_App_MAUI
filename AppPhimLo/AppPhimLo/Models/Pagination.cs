using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppPhimLo.Models
{
   public class Pagination
    {
        public int totalItems { get; set; }
        public int totalItemsPerPage { get; set; }
        public int currentPage { get; set; }
        public int totalPages { get; set; }
    }
}
