using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppPhimLo.Models
{
    public class FavoriteItem
    {
        public long? id { get; set; }
        public int? userId { get; set; }
        public string? slug { get; set; }
        public string? movieName { get; set; }
        public string? posterUrl { get; set; }
        public string? timeAgo { get; set; }
    }

}
