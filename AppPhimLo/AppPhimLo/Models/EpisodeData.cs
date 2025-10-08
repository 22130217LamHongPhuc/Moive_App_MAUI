using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AppPhimLo.Models
{
    public class EpisodeData
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("slug")]
        public string? Slug { get; set; }

        [JsonPropertyName("filename")]
        public string? FileName { get; set; }

        [JsonPropertyName("link_embed")]
        public string? LinkEmbed { get; set; }

        [JsonPropertyName("link_m3u8")]
        public string? LinkM3u8 { get; set; }
    }


}
