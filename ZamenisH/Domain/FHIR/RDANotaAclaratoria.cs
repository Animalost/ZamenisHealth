using System.Collections.Generic;

namespace Domain.FHIR
{
    public class RDANotaAclaratoria
    {
        public class BundleEntry
        {
            public object resource { get; set; }
        }
        public class Bundle
        {
            public string resourceType { get; set; }
            public string type { get; set; }
            public List<Entry> entry { get; set; }
            
        }
        public class Entry
        {
            public object resource { get; set; }
            public Request request { get; set; }
        }
        public class RdaBundleBuilder
        {
            private readonly Bundle _bundle;

            public RdaBundleBuilder()
            {
                _bundle = new Bundle
                {
                    resourceType = "Bundle",
                    type = "transaction",
                    entry = new List<Entry>(),                    
                };
            }

            public void AddResource(object resource, Request request)
            {
                _bundle.entry.Add(new Entry
                {
                    resource = resource,
                    request = request
                });
            }

            public Bundle Build()
            {
                return _bundle;
            }
        }
        public class Observation
        {
            public string resourceType { get; set; }
            public string id { get; set; }
            public Meta meta { get; set; }
            public string status { get; set; }
            public Code code { get; set; }
            public Subject subject { get; set; }
            public List<Focus> focus { get; set; }
            public Encounter encounter { get; set; }
            public string effectiveDateTime { get; set; }
            public List<Performer> performer { get; set; }
            public string valueString { get; set; }
        }
        public class Request
        {
            public string method { get; set; }
            public string url { get; set; }
        }
        public class Focus
        {
            public string id { get; set; }
            public string reference { get; set; }
        }
        public class Performer
        {
            public string reference { get; set; }
        }
        public class Encounter
        {
            public string reference { get; set; }
        }
        public class Subject
        {
            public string reference { get; set; }
        }
        public class Meta
        {
            public List<string> profile { get; set; }
        }
        public class Code
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
        public class Practitioner
        {
            public string resourceType { get; set; }
            public string id { get; set; }
            public Meta meta { get; set; }
            public List<Identifier> identifier { get; set; }
            public string active { get; set; }
            public List<Name> name { get; set; }
        }
        public class Name
        {
            public string use { get; set; }
            public string family { get; set; }
            public Family _family { get; set; }
            public List<string> given { get; set; }
        }      
        public class Family
        {
            public List<Extension> extension { get; set; }            
        }
        public class Identifier
        {
            public string id { get; set; }
            public string use { get; set; }
            public @Type type { get; set; }
            public string value { get; set; }
        }
        public class @Type
        {
            public List<Coding> coding { get; set; }
        }
        public class Extension
        {
            public string url { get; set; }
            public Coding valueCoding { get; set; }
            public string valueString { get; set; }
            public string valueTime { get; set; }
        }
    }
}
