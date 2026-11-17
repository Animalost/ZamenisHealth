using Newtonsoft.Json;
using System.Collections.Generic;

namespace ICD11.ClasesICD11
{
    public class ICD11SearchResult
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("stemId")]
        public string StemId { get; set; }

        [JsonProperty("isLeaf")]
        public bool IsLeaf { get; set; }

        [JsonProperty("postcoordinationAvailability")]
        public int PostcoordinationAvailability { get; set; }

        [JsonProperty("hasCodingNote")]
        public bool HasCodingNote { get; set; }

        [JsonProperty("hasMaternalChapterLink")]
        public bool HasMaternalChapterLink { get; set; }

        [JsonProperty("hasPerinatalChapterLink")]
        public bool HasPerinatalChapterLink { get; set; }

        [JsonProperty("matchingPVs")]
        public List<ICD11MatchingPV> MatchingPVs { get; set; }

        [JsonProperty("propertiesTruncated")]
        public bool PropertiesTruncated { get; set; }

        [JsonProperty("isResidualOther")]
        public bool IsResidualOther { get; set; }

        [JsonProperty("isResidualUnspecified")]
        public bool IsResidualUnspecified { get; set; }

        [JsonProperty("chapter")]
        public string Chapter { get; set; }

        [JsonProperty("theCode")]
        public string Code { get; set; }

        [JsonProperty("score")]
        public double Score { get; set; }

        [JsonProperty("titleIsASearchResult")]
        public bool TitleIsASearchResult { get; set; }

        [JsonProperty("titleIsTopScore")]
        public bool TitleIsTopScore { get; set; }

        [JsonProperty("entityType")]
        public int EntityType { get; set; }

        [JsonProperty("important")]
        public bool Important { get; set; }

        [JsonProperty("descendants")]
        public List<string> Descendants { get; set; }
    }

    public class ICD11MatchingPV
    {
        [JsonProperty("propertyId")]
        public string PropertyId { get; set; }

        [JsonProperty("label")]
        public string Label { get; set; }

        [JsonProperty("score")]
        public double Score { get; set; }

        [JsonProperty("important")]
        public bool Important { get; set; }

        [JsonProperty("foundationUri")]
        public string FoundationUri { get; set; }

        [JsonProperty("propertyValueType")]
        public int PropertyValueType { get; set; }
    }
}
