using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AppPhimLo.Models
{
    public class EpisodeServer
    {
        [JsonPropertyName("server_name")]
        public string? ServerName { get; set; }

        [JsonPropertyName("server_data")]
        public List<EpisodeData>? ServerData { get; set; }
    }
}
