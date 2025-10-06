using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppPhimLo.Models
{
   public class TheLoaiItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }

        public Color Color { get; set; }
    }
}
