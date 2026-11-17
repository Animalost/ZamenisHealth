using Newtonsoft.Json;

namespace ICD11.ClasesICD11
{
    public class ICD11Terms
    {
        [JsonProperty("@language")]
        public string Language { get; set; }

        [JsonProperty("@value")]
        public string Value { get; set; }
    }
    public class ICD11LocalizedValue
    {
        [JsonProperty("@language")]
        public string Language { get; set; }

        [JsonProperty("@value")]
        public string Value { get; set; }
    }

    // Etiquetas de inclusión
    public class ICD11Label
    {
        [JsonProperty("label")]
        public ICD11LocalizedValue Label { get; set; }
    }

    // Diagnósticos excluidos
    public class ICD11Exclusion
    {
        [JsonProperty("label")]
        public ICD11LocalizedValue Label { get; set; }

        [JsonProperty("foundationReference")]
        public string FoundationReference { get; set; }

        [JsonProperty("linearizationReference")]
        public string LinearizationReference { get; set; }
    }

    // Términos de indexación
    public class ICD11IndexTerm
    {
        [JsonProperty("label")]
        public ICD11LocalizedValue Label { get; set; }

        [JsonProperty("foundationReference")]
        public string FoundationReference { get; set; }
    }
}
