using Newtonsoft.Json;
using System.Collections.Generic;

namespace Domain.HCE_MinSalud
{
    public class Bundle
    {
        [JsonProperty("resourceType")]
        public string ResourceType { get; set; } = "Bundle";

        [JsonProperty("type")]
        public string Type { get; set; } = "document";

        [JsonProperty("identifier")]
        public Identifier Identifier { get; set; }

        [JsonProperty("timestamp")]
        public string Timestamp { get; set; }

        [JsonProperty("entry")]
        public List<Entry> Entry { get; set; }
    }

    public class Identifier
    {
        [JsonProperty("system")]
        public string System { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }

    public class Entry
    {
        [JsonProperty("fullUrl")]
        public string FullUrl { get; set; }

        [JsonProperty("resource")]
        public object Resource { get; set; } // puede ser Patient, Encounter, Condition, etc.
    }

    // Ejemplo de Patient
    public class Patient
    {
        [JsonProperty("resourceType")]
        public string ResourceType { get; set; } = "Patient";

        [JsonProperty("identifier")]
        public List<IdentifierDetail> Identifier { get; set; }

        [JsonProperty("name")]
        public List<Name> Name { get; set; }

        [JsonProperty("gender")]
        public string Gender { get; set; }

        [JsonProperty("birthDate")]
        public string BirthDate { get; set; }
    }

    public class IdentifierDetail
    {
        [JsonProperty("system")]
        public string System { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }

    public class Name
    {
        [JsonProperty("family")]
        public string Family { get; set; }

        [JsonProperty("given")]
        public List<string> Given { get; set; }
    }
}
