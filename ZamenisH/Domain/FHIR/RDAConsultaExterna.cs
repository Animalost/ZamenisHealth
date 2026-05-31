using System;
using System.Collections.Generic;

namespace Domain.FHIR.CE
{
    public class RDAConsultaExterna
    {
        public class Coding
        {
            public string system { get; set; }
            public string code { get; set; }
            public string display { get; set; }
        }
        public class ClinicalStatus
        {
            public List<Coding> coding { get; set; }
        }
        public class VerificationStatus
        {
            public List<Coding> coding { get; set; }
        }
        public class Category
        {
            public List<Coding> coding { get; set; }
        }
        public class Reference
        {
            public string reference { get; set; }
        }
        public class Meta
        {
            public List<string> profile { get; set; }           
        }
        
        public class ServiceRequest
        {
            public string resourceType { get; set; } = "ServiceRequest";
            public string id { get; set; }
            public Meta meta { get; set; }
            public Subject subject { get; set; }
            public Encounter encounter { get; set; }
            public Code code { get; set; }
            public string status { get; set; }
            public string intent { get; set; }
            public Category category { get; set; }
            public string authoredOn { get; set; }
            public ReasonCode reasonCode { get; set; }
        }
        
        public class Type3
        {
            public Coding coding { get; set; }
        }
        public class Type
        {
            public List<Coding> coding { get; set; }
        }
        public class Identifier
        {
            public Type type { get; set; }
            public string use { get; set; }
            public string system { get; set; }
            public string value { get; set; }
            public string id { get; set; }
        }
        public class Bundle
        {
            public string resourceType { get; set; } = "Bundle";
            public string language { get; set; } = "es-CO";
            public string type { get; set; } = "document";
            public List<BundleEntry> entry { get; set; } 
        }
        public class BundleEntry
        {
            public object resource { get; set; }
        }
        public class Patient
        {
            public string resourceType { get; set; }
            public string id { get; set; }
            public Meta meta { get; set; }
            public List<Extension2> extension { get; set; }
            public List<Identifier> identifier { get; set; }
            public List<Name> name { get; set; }
            public List<Address> address { get; set; }
            public string gender { get; set; }
            public Gender _gender { get; set; }
            public DateTime? birthDate { get; set; }
            public BirthDate _birthDate { get; set; }
            public bool? active { get; set; }
            public bool? deceasedBoolean { get; set; }
            public string reference { get; set; }

        }
        public class Practitioner
        {
            public string resourceType { get; set; } = "Practitioner";
            public string id { get; set; }
            public Meta meta { get; set; }
            public List<Identifier> identifier { get; set; }
            public List<Name> name { get; set; }
        }
        public class AllergyIntolerance
        {
            public string resourceType { get; set; } = "Practitioner";
            public string id { get; set; }
            public Meta meta { get; set; }
            public ClinicalStatus clinicalStatus { get; set; }
            public Code code { get; set; }
            public Patient patient { get; set; }
            public Encounter encounter { get; set; }
        }
        public class Organization
        {
            public string resourceType { get; set; } = "Organization";
            public string id { get; set; }
            public string name { get; set; }
        }
        public class Encounter2
        {
            public string reference { get; set; }
        }
        public class Encounter
        {
            public string resourceType { get; set; } 
            public string id { get; set; }
            public Meta meta { get; set; }
            public string status { get; set; }          
            public List<Type3> type { get; set; }
            public Reference subject { get; set; }
            public List<EncounterParticipant> participant { get; set; }
            public List<Identifier> identifier { get; set; }
            public List<ReasonCode> reasonCode { get; set; }
            public List<Diagnosis> diagnosis { get; set; }
            public Period period { get; set; }
            public Class @class { get; set; }
            public ServiceType serviceType { get; set; }
            public List<Extension> extension { get; set; }
            public string reference { get; set; }
            public List<Location> location { get; set; }
            public ServiceProvider serviceProvider { get; set; }
        }      
        public class Use
        {
            public List<Coding> coding { get; set; }
        }
        public class Diagnosis
        {
            public string id { get; set; }
            public List<Extension> extension { get; set; }
            public Condition condition { get; set; }
            public Use use { get; set; }
            public int rank { get; set; }
        }
        public class ServiceType
        {
            public Coding coding { get; set; }
        }
        public class Class
        {
            public string system { get; set; }
            public string code { get; set; }
            public string display { get; set; }
        }
        public class EncounterParticipant
        {
            public string id { get; set; }
            public List<Type> type { get; set; }
            public Reference individual { get; set; }
        }
        public class Period
        {
            public DateTimeOffset start { get; set; }
            public DateTimeOffset end { get; set; }
        }
        public class Composition
        {
            public string status { get; set; }
            public Meta meta { get; set; }
            public string resourceType { get; set; }
            public Type type { get; set; }
            public Subject subject { get; set; }
            public Encounter2 encounter { get; set; }
            public DateTimeOffset date { get; set; }
            public List<Author> author { get; set; }
            public string title { get; set; }
            //public List<CompositionSection> section { get; set; }
            public string confidentiality { get; set; }
            public List<Attester> attester { get; set; }
            public Custodian custodian { get; set; }
            public Event @event { get; set; }
            public List<Section> section { get; set; }
        }
        public class Attester
        {
            public string mode { get; set; }
            public Party party { get; set; }
        }
        public class Party
        {
            public string reference { get; set; }
        }
        public class Custodian
        {
            public string reference { get; set; }
        }
        public class Event
        {
            public Period period { get; set; }
        }
        public class Section
        {
            public string title { get; set; }
            public Code2 code { get; set; }
            public List<Entry> entry { get; set; }
            public EmptyReason emptyReason { get; set; }
            public Text text { get; set; }
        }
        public class EmptyReason
        {
            public List<Coding> coding { get; set; }
           
        }
        public class Code2
        {
            public List<Coding> coding { get; set; }           
        }
        public class Code
        {
            public List<Coding> coding { get; set; }
            public List<Entry> entry { get; set; }
            public string text { get; set; }
        }
        public class Entry
        {
            public string reference { get; set; }
        }
        public class Resource
        {
            public string status { get; set; }
            public string resourceType { get; set; }
            public string id { get; set; }
            public Meta meta { get; set; }
            public List<Identifier> identifier { get; set; }
        }
        public class BirthDate
        {
            public List<Extension> extension { get; set; }
        }
        public class Gender
        {
            public List<Extension> extension { get; set; }
        }
        public class Address
        {
            public string id { get; set; }
            public string use { get; set; }
            public string type { get; set; }
            public string city { get; set; }
            public City _city { get; set; }
            public string country { get; set; }
            public Country _country { get; set; }
            public List<Extension2> extension { get; set; }
        }
        public class Country
        {
            public List<Extension> extension { get; set; }
        }
        public class City
        {
            public List<Extension> extension { get; set; }
        }
        public class Name
        {
            public List<string> given { get; set; }   
            public string use { get; set; }
            public string family { get; set; }
            public Family _family { get; set; }
        }      
        public class Family
        {
            public List<Extension3> extension { get; set; }
        }
        public class ServiceProvider
        {
            public string reference { get; set; }
        }
        public class Location
        {
            public string resourceType { get; set; }
            public string id { get; set; }
            public Meta meta { get; set; }
            public Location location { get; set; }
            public string reference { get; set; }
            public List<Identifier> identifier { get; set; }
            public string name { get; set; }
            public ManagingOrganization managingOrganization { get; set; }
        }
        public class Observation
        {
            public string resourceType { get; set; }
            public string id { get; set; }
            public Meta meta { get; set; }
            public string status { get; set; }
            public Code code { get; set; }
            public Subject subject { get; set; }
            public Encounter encounter { get; set; }
            public List<Component> component { get; set; }
            public ValueCodeableConcept valueCodeableConcept { get; set; }

        }   
        public class RiskAssessment
        {
            public string resourceType { get; set; }
            public string id { get; set; }
            public Meta meta { get; set; }
            public string status { get; set; }
            public Code code { get; set; }
            public Subject subject { get; set; }
            public Encounter encounter { get; set; }
            public EmptyReason emptyReason { get; set; }
            public Text text { get; set; }
        }
        public class ValueQuantity
        {
            public int value { get; set; }
            public string unit { get; set; }
            public string system { get; set; }
            public string code { get; set; }
        }
        public class ValueCodeableConcept
        {
            public List<Coding> coding { get; set; }
        }
        public class Component
        {
            public string id { get; set; }
            public Code code { get; set; }
            public ValueCodeableConcept valueCodeableConcept { get; set; }
            public ValueQuantity valueQuantity { get; set; }
        }
        public class ManagingOrganization
        {
            public string reference { get; set; }
        }
        public class Extension2
        {
            public string url { get; set; }
            public ValueCoding valueCoding { get; set; }
        }
        public class Extension3
        {
            public string url { get; set; }
            public string valueString { get; set; }
        }
        public class Extension
        {
            public string url { get; set; }
            public string valueString { get; set; }  
            public ValueCoding valueCoding { get; set; }
            public ValueReference valueReference { get; set; }
            public string valueTime { get; set; }
            public List<Extension> extension { get; set; }            
        }
        public class ValueReference
        {
            public string reference { get; set; }
        }
        public class ValueCoding
        {
            public string system { get; set; }
            public string code { get; set; }
            public string display { get; set; }
        }
        public class RdaBundleBuilder
        {
            private readonly Bundle _bundle;

            public RdaBundleBuilder()
            {
                _bundle = new Bundle
                {
                    resourceType = "Bundle",
                    type = "document",
                    language = "es-CO",
                    entry = new List<BundleEntry>()
                };
            }

            public void AddResource(object resource)
            {
                _bundle.entry.Add(new BundleEntry
                {
                    resource = resource
                });
            }

            public Bundle Build()
            {
                return _bundle;
            }
        }
        public class Condition
        {
            public string reference { get; set; }
            public string resourceType { get; set; }
            public string id { get; set; }
            public Meta meta { get; set; }
            public ClinicalStatus clinicalStatus { get; set; }
            public VerificationStatus verificationStatus { get; set; }
            public List<Category> category { get; set; }
            public Code code { get; set; }
            public Reference subject { get; set; }

        }
        public class DocumentReference
        {
            public string resourceType { get; set; } = "ServiceRequest";
            public string id { get; set; }
            public Meta meta { get; set; }
            public Text text { get; set; }
            public string status { get; set; }
            public Type type { get; set; }
            public List<Category> category { get; set; }
            public Subject subject { get; set; }
            public string date { get; set; }
            public List<Author> author { get; set; }
            public Custodian custodian { get; set; }
            public string description { get; set; }
            public List<SecurityLabel> securityLabel { get; set; }
            public List<Content> content { get; set; }
            public Context context { get; set; }

        }
        public class Context
        {
            public List<Encounter> encounter { get; set; }

        }
        public class Format
        {
            public string system { get; set; }
            public string code { get; set; }
            public string display { get; set; }
        }
        public class Content
        {
            public Attachment attachment { get; set; }
            public Format format { get; set; }
        }
        public class Attachment
        {
            public string data { get; set; }
        }
        public class SecurityLabel
        {
            public List<Coding> coding { get; set; }
        }
        public class Author
        {
            public string reference { get; set; }
        }
        public class Text
        {
            public string status { get; set; }
            public string div { get; set; }
        }
        public class MedicationRequest
        {
            public string resourceType { get; set; } = "MedicationRequest";
            public string id { get; set; }
            public Meta meta { get; set; }
            public string status { get; set; }
            public string intent { get; set; }
            public List<Category> category { get; set; }
            public Encounter encounter { get; set; }
            public Requester requester { get; set; }
            public DateTimeOffset authoredOn { get; set; }
            public List<Dosage> dosageInstruction { get; set; }
            public bool reportedBoolean { get; set;  }
            public MedicationCodeableConcept medicationCodeableConcept { get; set; }
            public Subject subject { get; set; }
            public List<ReasonCode> reasonCode { get; set; }
        }
        public class ReasonCode
        {
            public List<Coding> coding { get; set; }
        }
        public class Requester
        {
            public string reference { get; set; }
        }
        public class Subject
        {
            public string reference { get; set; }
        }
        public class MedicationCodeableConcept
        {
            public List<Coding> coding { get; set; }
        }       
        public class Dosage
        {
            public Timing timing { get; set; }
            public Code code { get; set; }
            public Route route { get; set; }
            public List<DoseAndRate> doseAndRate { get; set; }
        }
        public class DoseAndRate
        {
            public DoseQuantity doseQuantity { get; set; }
            public RateQuantity rateQuantity { get; set; }
        }
        public class RateQuantity
        {
            public int value { get; set; }
            public string unit { get; set; }
            public string system { get; set; }
            public string code { get; set; }
        }
        public class DoseQuantity
        {
            public int value { get; set;  }
            public string unit { get; set; }
            public string system { get; set; }
            public string code { get; set; }
        }
        public class Route
        {
            public List<Coding> coding { get; set; }
        }
        public class Timing
        {
            public Repeat repeat { get; set; }
            public Code code { get; set; }
        }
        public class Repeat
        {
            public int duration { get; set; }
            public string durationUnit { get; set; }
        }
    }
}
