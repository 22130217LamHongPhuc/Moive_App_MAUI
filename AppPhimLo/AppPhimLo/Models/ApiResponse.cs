using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppPhimLo.Models
{
  public  class ApiResponse
    {
        public bool status { get; set; }
        public string msg { get; set; }
        public ObservableCollection<MovieItem> items { get; set; }
        public Pagination pagination { get; set; }
        
    }
}
