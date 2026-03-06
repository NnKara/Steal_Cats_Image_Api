using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace StealCatsImage.Infrastructure.Clients.Models
{
    public class CatApiImageResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = null!;

        [JsonPropertyName("url")]
        public string Url { get; set; } = null!;

        [JsonPropertyName("width")]
        public int Width { get; set; }

        [JsonPropertyName("height")]
        public int Height { get; set; }

        [JsonPropertyName("breeds")]
        public List<CatApiBreedResponse> Breeds { get; set; } = new();
    }

    public class CatApiBreedResponse
    {
        [JsonPropertyName("temperament")]
        public string? Temperament { get; set; }
    }
}

