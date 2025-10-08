using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;


namespace AppPhimLo.Models
{


  
    public class MovieResponse
    {
        [JsonPropertyName("status")]
        public bool Status { get; set; }

        [JsonPropertyName("msg")]
        public string? Message { get; set; }

        [JsonPropertyName("movie")]
        public MovieDetail? Movie { get; set; }

        [JsonPropertyName("episodes")]
        public List<EpisodeServer>? Episodes { get; set; }
    }

    public class MovieDetail
    {
        [JsonPropertyName("_id")] public string? Id { get; set; }
        [JsonPropertyName("name")] public string? Name { get; set; }
        [JsonPropertyName("origin_name")] public string? OriginName { get; set; }
        [JsonPropertyName("content")] public string? Content { get; set; }
        [JsonPropertyName("year")] public int Year { get; set; }
        [JsonPropertyName("poster_url")] public string? PosterUrl { get; set; }
        [JsonPropertyName("thumb_url")] public string? ThumbUrl { get; set; }
        [JsonPropertyName("trailer_url")] public string? TrailerUrl { get; set; }
        [JsonPropertyName("time")] public string? Duration { get; set; }
        [JsonPropertyName("episode_current")] public string? EpisodeCurrent { get; set; }
        [JsonPropertyName("episode_total")] public string? EpisodeTotal { get; set; }
        [JsonPropertyName("quality")] public string? Quality { get; set; }
        [JsonPropertyName("lang")] public string? Language { get; set; }

        [JsonPropertyName("actor")] public List<string>? Actors { get; set; }
        [JsonPropertyName("director")] public List<string>? Directors { get; set; }
        [JsonPropertyName("category")] public List<Category>? Categories { get; set; }
        [JsonPropertyName("country")] public List<Country>? Countries { get; set; }
    }

  


  


}
