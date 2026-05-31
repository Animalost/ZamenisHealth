using System.Collections.Generic;

namespace Domain.FHIR
{
    public class Identifier
    {
        public string id { get; set; }
        public string use { get; set; }
        public CodeableConcept type { get; set; }
        public string system { get; set; }
        public string value { get; set; }
    }
    public class Address
    {
        public string id { get; set; }
        public string use { get; set; }
        public string type { get; set; }
        public string city { get; set; }
        public ExtensionContainer _city { get; set; }
        public string country { get; set; }
        public ExtensionContainer _country { get; set; }
        public List<Extension> extension { get; set; } 
    }
    public class CodeableConcept
    {
        public List<Coding> coding { get; set; }
        public string text { get; set; }
    }
    public class Coding
    {
        public string system { get; set; }
        public string code { get; set; }
        public string display { get; set; }
    }
    public class Reference
    {
        public string reference { get; set; }
    }
    public class Extension
    {
        public string url { get; set; }
        public Coding valueCoding { get; set; }
        public string valueString { get; set; }
        public string valueTime { get; set; }
    }
    public class ExtensionContainer
    {
        public List<Extension> extension { get; set; } 
    }
    public class HumanName
    {
        public string use { get; set; }
        public string family { get; set; }
        public List<string> given { get; set; } 
        public ExtensionContainer _family { get; set; }   // extensiones en family
    }


    //Inmunizacion Consulta
    public class Parameters
    {
        public string resourceType { get; set; } = "Parameters";
        public List<Parameter> parameter { get; set; }
    }
    public class Parameter
    {
        public string name { get; set; }
        public List<Part> part { get; set; }
        public string valueString { get; set; }
    }
    public class Part
    {
        public string name { get; set; }
        public string valueString { get; set; }
    }
}
