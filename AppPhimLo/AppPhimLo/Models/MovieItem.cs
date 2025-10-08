using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppPhimLo.Models
{
    public class MovieItem
    {
        public string _id { get; set; }
        public string name { get; set; }
        public string slug { get; set; }
        public string origin_name { get; set; }
        public string poster_url { get; set; }
        public string thumb_url { get; set; }
        public int year { get; set; }

        public string FullThumbUrl => $"https://phimimg.com/{thumb_url}";

    }
}
