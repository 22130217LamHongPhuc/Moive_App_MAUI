using System.Text.Json.Serialization;

namespace AppPhimLo.Models
{
    public class Movie
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public string Thumb_Url { get; set; }
        public string Poster_Url { get; set; }
        public string Episode_Current { get; set; }
        public int Year { get; set; }
        public List<Country> Country { get; set; }
        public List<Category> Category { get; set; }

        private const string ImageDomain = "https://phimimg.com/";

        // URL đầy đủ cho ảnh
        public string ThumbUrl => string.IsNullOrEmpty(Thumb_Url) ? null : $"{ImageDomain}{Thumb_Url}";
        public string PosterUrl => string.IsNullOrEmpty(Poster_Url) ? null : $"{ImageDomain}{Poster_Url}";

        // Properties tiện dụng để hiển thị
        public string CountryNames => Country != null ? string.Join(", ", Country.Select(c => c.Name)) : "";
        public string CategoryNames => Category != null ? string.Join(", ", Category.Select(c => c.Name)) : "";
    }

    public class Country
    {
        public string Name { get; set; }
    }

    public class Category
    {
        public string Name { get; set; }
    }
}

