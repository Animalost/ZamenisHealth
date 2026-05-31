using System.Collections.Generic;

namespace ZamenisHealth.FrontFHIR.VisorZamenis
{
    public class DataConsultaRDA
    {
        public string resourceType { get; set; }
        public List<Parameter> parameter { get; set; }
    }

    public class Parameter
    {
        public string name { get; set; }
        public string valueString { get; set; }   // <-- para humanuser
        public List<Part> part { get; set; }       // <-- solo cuando aplica
    }

    public class Part
    {
        public string name { get; set; }
        public string valueString { get; set; }
    }
}
