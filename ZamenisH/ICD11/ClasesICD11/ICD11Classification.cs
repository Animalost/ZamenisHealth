using ICD11.ClasesICD11;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace ICD11
{
    public class ICD11Classification
    {
        [JsonProperty("@context")]
        public string Context { get; set; }

        [JsonProperty("@id")]
        public string Id { get; set; }

        [JsonProperty("title")]
        public ICD11LocalizedValue Title { get; set; }

        [JsonProperty("releaseId")]
        public string ReleaseId { get; set; }

        [JsonProperty("availableLanguages")]
        public List<string> AvailableLanguages { get; set; }

        [JsonProperty("releaseDate")]
        public string ReleaseDate { get; set; }

        [JsonProperty("child")]
        public List<string> Children { get; set; }

        [JsonProperty("browserUrl")]
        public string BrowserUrl { get; set; }
    }
}
