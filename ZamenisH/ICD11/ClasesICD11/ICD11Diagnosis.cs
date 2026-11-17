using Newtonsoft.Json;
using System.Collections.Generic;

namespace ICD11.ClasesICD11
{
    public class ICD11Diagnosis
    {
        [JsonProperty("@context")]
        public string Context { get; set; }

        [JsonProperty("@id")]
        public string Id { get; set; }

        [JsonProperty("parent")]
        public List<string> Parent { get; set; }

        [JsonProperty("browserUrl")]
        public string BrowserUrl { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("source")]
        public string Source { get; set; }

        [JsonProperty("classKind")]
        public string ClassKind { get; set; }

        [JsonProperty("title")]
        public ICD11LocalizedValue Title { get; set; }

        [JsonProperty("definition")]
        public ICD11LocalizedValue Definition { get; set; }

        [JsonProperty("inclusion")]
        public List<ICD11Label> Inclusion { get; set; }

        [JsonProperty("exclusion")]
        public List<ICD11Exclusion> Exclusion { get; set; }

        [JsonProperty("relatedEntitiesInMaternalChapter")]
        public List<string> RelatedEntitiesInMaternalChapter { get; set; }

        [JsonProperty("indexTerm")]
        public List<ICD11IndexTerm> IndexTerm { get; set; }
    }
}
