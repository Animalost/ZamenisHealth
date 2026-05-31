using Domain.FHIR.CE;
using iText.Layout.Element;
using System;
using System.Collections.Generic;

namespace Domain.FHIR
{
    #region Bundle
    public class Bundle
    {
        public string resourceType { get; set; } = "Bundle";
        public string language { get; set; } = "es-CO";
        public string type { get; set; } = "document";
        public List<Entry> entry { get; set; } = new List<Entry>();
    }
    public class Entry
    {
        public Resource resource { get; set; }
    }
    public abstract class Resource
    {
        public string resourceType { get; set; }
        public string id { get; set; }
        public Meta meta { get; set; }
    }
    public class Meta
    {
        public List<string> profile { get; set; }
    }
    #endregion Bundle
    #region Compisition
    public class Composition : Resource
    {
        public string status { get; set; }
        public CodeableConcept type { get; set; }
        public Reference subject { get; set; }
        public DateTimeOffset date { get; set; }
        public List<Reference> author { get; set; } = new List<Reference>();
        public string title { get; set; }
        public string confidentiality { get; set; }
        public List<Attester> attester { get; set; } = new List<Attester>();
        public Reference custodian { get; set; }
        public List<Event> @event { get; set; } = new List<Event>();
        public List<Section> section { get; set; } = new List<Section>();
    }

    public class Attester
    {
        public string mode { get; set; }
        public Reference party { get; set; }
    }

    public class Event
    {
        public List<CodeableConcept> code { get; set; } = new List<CodeableConcept>();
        public Period period { get; set; }
    }

    public class Period
    {
        public DateTimeOffset start { get; set; }
        public DateTimeOffset end { get; set; }
    }

    public class Section
    {
        public string title { get; set; }
        public CodeableConcept code { get; set; }
        public List<Reference> entry { get; set; } = new List<Reference>();
        public CodeableConcept emptyReason { get; set; }
        public Text text { get; set; }
    }

    #endregion Compisition
    #region Patient
    public class Patient : Resource
    {
        public List<Identifier> identifier { get; set; } 
        public List<HumanName> name { get; set; } 
        public List<Address> address { get; set; } 
        public bool active { get; set; }
        public string gender { get; set; }
        public ExtensionContainer _gender { get; set; }   // soporte para extensiones en gender
        public DateTime birthDate { get; set; }
        public ExtensionContainer _birthDate { get; set; } // soporte para extensiones en birthDate
        public bool deceasedBoolean { get; set; }
        public List<Extension> extension { get; set; }  // extensiones generales
    }

    #endregion
    #region Organization
    public class Organization : Resource
    {
        public List<Identifier> identifier { get; set; } = new List<Identifier>();
    }

    #endregion Organization
    #region Practitioner
    public class Practitioner : Resource
    {
        public List<Identifier> identifier { get; set; } = new List<Identifier>();
        public List<HumanName> name { get; set; } = new List<HumanName>();
    }

    #endregion Practitioner
    #region Condition
    public class Text
    {
        public string status { get; set; }
        public string div { get; set; }
    }
    public class Condition : Resource
    {
        public CodeableConcept clinicalStatus { get; set; }
        public CodeableConcept verificationStatus { get; set; }
        public List<CodeableConcept> category { get; set; } = new List<CodeableConcept>();
        public CodeableConcept code { get; set; }
        public Reference subject { get; set; }        
    }

    #endregion Condition
    #region AllergyIntolerance
    public class AllergyIntolerance : Resource
    {
        public CodeableConcept clinicalStatus { get; set; }
        public CodeableConcept verificationStatus { get; set; }
        public CodeableConcept code { get; set; }
        public Reference patient { get; set; }
    }

    #endregion AllergyIntolerance
    #region FamilyMemberHistory
    public class FamilyMemberHistory : Resource
    {
        public string status { get; set; }
        public Reference patient { get; set; }
        public CodeableConcept relationship { get; set; }
        public List<ConditionFMH> condition { get; set; } = new List<ConditionFMH>();
    }

    public class ConditionFMH
    {
        public CodeableConcept code { get; set; }
    }

    #endregion FamilyMemberHistory
    #region MedicationStatement
    public class MedicationStatement : Resource
    {
        public string status { get; set; }
        public CodeableConcept medicationCodeableConcept { get; set; }
        public Reference subject { get; set; }
        public Meta meta { get; set; }
    }
    #endregion MedicationStatement  
}
