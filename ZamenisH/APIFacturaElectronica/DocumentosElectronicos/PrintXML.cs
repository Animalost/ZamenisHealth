using DocumentosElectronicos.Request;
using System.IO;
using System.Xml.Serialization;

namespace DocumentosElectronicos
{
    public static class PrintXML
    {
        public static void Print(Factura factura, string Doc)
        {
            var serializer = new XmlSerializer(typeof(Factura));
            string rutaArchivo = Path.Combine("C:\\CXN\\RespuestasDIAN\\RespuestaXML\\", Doc + ".xml");

            using (var writer = new StreamWriter(rutaArchivo))
            {
                serializer.Serialize(writer, factura);
            }
        }
    }
}
