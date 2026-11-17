using Newtonsoft.Json;
using System.Collections.Generic;

namespace ICD11.ClasesICD11
{
    public class ICD11SearchResponse
    {
        [JsonProperty("destinationEntities")]
        public List<ICD11SearchResult> DestinationEntities { get; set; }

        [JsonProperty("error")]
        public bool Error { get; set; }

        [JsonProperty("errorMessage")]
        public string ErrorMessage { get; set; }

        [JsonProperty("resultChopped")]
        public bool ResultChopped { get; set; }

        [JsonProperty("wordSuggestionsChopped")]
        public bool WordSuggestionsChopped { get; set; }

        [JsonProperty("guessType")]
        public int GuessType { get; set; }

        [JsonProperty("uniqueSearchId")]
        public string UniqueSearchId { get; set; }

        [JsonProperty("words")]
        public object Words { get; set; }
    }
}
