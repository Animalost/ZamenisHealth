using System.Xml.Serialization;

namespace DocumentosElectronicos.Controlador
{
    public class XmlEmptyTagString
    {
        private string _value;

        public XmlEmptyTagString() { }

        public XmlEmptyTagString(string value)
        {
            _value = value;
        }

        public static implicit operator string(XmlEmptyTagString x) => x?._value;
        public static implicit operator XmlEmptyTagString(string s) => new XmlEmptyTagString(s);

        [XmlText]
        public string Value
        {
            get => _value ?? string.Empty;
            set => _value = value;
        }

        public bool ShouldSerializeValue() => true;
    }
}
