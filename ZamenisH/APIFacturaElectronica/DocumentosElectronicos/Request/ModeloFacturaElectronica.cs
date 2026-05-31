using DocumentosElectronicos.Controlador;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Serialization;

namespace DocumentosElectronicos.Request
{
    [XmlRoot("Factura")]
    public class Factura
    {
        public Encabezado Encabezado { get; set; }
        [XmlArray("Detalle")]
        [XmlArrayItem("Det")]       

        public List<Detalle> Detalle { get; set; }
        [XmlArray("Impuestos")]
        [XmlArrayItem("Imp")]

        public List<Impuesto> Impuestos { get; set; }
        
        [XmlArray("Salud")]
        [XmlArrayItem("Sal")]        
        public List<Salud> Salud { get; set; }
        
        [XmlArray("Descuentos")]
        [XmlArrayItem("Des")]
        public List<Descuentos> Descuentos { get; set; }
    }
    public class periodoFacturacion
    {
        [XmlElement("FechaInicial")]
        public XmlEmptyTagString FechaInicial { get; set; } // Formato: yyyy-MM-dd
        [XmlElement("FechaFin")]
        public XmlEmptyTagString FechaFin { get; set; } // Formato: yyyy-MM-dd
    }
    public class Encabezado
    {
        public periodoFacturacion periodoFacturacion { get; set; }
        [XmlElement("llaveComprobante")]
        public XmlEmptyTagString llaveComprobante { get; set; }
        [XmlElement("nitemisor")]
        public XmlEmptyTagString nitemisor { get; set; }
        [XmlElement("codSucursal")]
        public XmlEmptyTagString codSucursal { get; set; }
        [XmlElement("tiporeceptor")]
        public XmlEmptyTagString tiporeceptor { get; set; }
        [XmlElement("tipoDocRec")]
        public XmlEmptyTagString tipoDocRec { get; set; }
        [XmlElement("nitreceptor")]
        public XmlEmptyTagString nitreceptor { get; set; }
        [XmlElement("digitoverificacion")]
        public XmlEmptyTagString digitoverificacion { get; set; }
        [XmlElement("nombrereceptor")]
        public XmlEmptyTagString nombrereceptor { get; set; } // Aunque no se usa, se define para mantener la estructura
        [XmlElement("mailreceptor")]
        public XmlEmptyTagString mailreceptor { get; set; }
        [XmlElement("tipocomprobante")]
        public XmlEmptyTagString tipocomprobante { get; set; }
        [XmlElement("noresolucion")]
        public XmlEmptyTagString noresolucion { get; set; }
        [XmlElement("prefijo")]
        public XmlEmptyTagString prefijo { get; set; }
        [XmlElement("folio")]
        public XmlEmptyTagString folio { get; set; }
        [XmlElement("fecha")]
        public XmlEmptyTagString fecha { get; set; }
        [XmlElement("hora")]
        public XmlEmptyTagString hora { get; set; }
        [XmlElement("moneda")]
        public XmlEmptyTagString moneda { get; set; }
        [XmlElement("metodopago")]
        public XmlEmptyTagString metodopago { get; set; }
        [XmlElement("mediopago")]
        public XmlEmptyTagString mediopago { get; set; }
        [XmlElement("fechavencimiento")]
        public XmlEmptyTagString fechavencimiento { get; set; } // Formato: yyyy-MM-dd
        [XmlElement("terminospago")]
        public XmlEmptyTagString terminospago { get; set; } // Aunque no se usa, se define para mantener la estructura
        [XmlElement("montoletra")]
        public XmlEmptyTagString montoletra { get; set; }
        [XmlElement("tipoOpera")]
        public XmlEmptyTagString tipoOpera { get; set; }
        [XmlElement("extra1")]
        public XmlEmptyTagString extra1 { get; set; }
        [XmlElement("extra2")]
        public XmlEmptyTagString extra2 { get; set; }
        [XmlElement("ordenCompra")]
        public XmlEmptyTagString ordenCompra { get; set; } // Aunque no se usa, se define para mantener la estructura

        //DEMOGRAFIA
        [XmlElement("paisreceptor")]
        public XmlEmptyTagString paisreceptor { get; set; }
        [XmlElement("mailreceptorcontacto")]
        public XmlEmptyTagString mailreceptorcontacto { get; set; }
       

        [XmlIgnore]
        public decimal subtotal { get; set; }
        [XmlElement("subtotal")]
        public string subtotalFormatted
        {
            get => subtotal.ToString("F2", CultureInfo.InvariantCulture);
            set => subtotal = decimal.Parse(value, CultureInfo.InvariantCulture);
        }
        
        [XmlIgnore]
        public decimal baseimpuesto { get; set; }
        [XmlElement("baseimpuesto")]
        public string baseimpuestoFormatted
        {
            get => baseimpuesto.ToString("F2", CultureInfo.InvariantCulture);
            set => baseimpuesto = decimal.Parse(value, CultureInfo.InvariantCulture);
        }

        [XmlIgnore]
        public decimal totalsindescuento { get; set; }
        [XmlElement("totalsindescuento")]
        public string totalsindescuentoFormatted
        {
            get => totalsindescuento.ToString("F2", CultureInfo.InvariantCulture);
            set => totalsindescuento = decimal.Parse(value, CultureInfo.InvariantCulture);
        }

        [XmlIgnore]
        public decimal totaldescuentos { get; set; } // Aunque no se usa, se define para mantener la estructura 
        [XmlElement("totaldescuentos")]
        public string totaldescuentosFormatted
        {
            get => totaldescuentos.ToString("F2", CultureInfo.InvariantCulture);
            set => totaldescuentos = decimal.Parse(value, CultureInfo.InvariantCulture);
        }

        [XmlIgnore]
        public decimal totalimpuestos { get; set; }
        [XmlElement("totalimpuestos")]
        public string totalimpuestosFormatted
        {
            get => totalimpuestos.ToString("F2", CultureInfo.InvariantCulture);
            set => totalimpuestos = decimal.Parse(value, CultureInfo.InvariantCulture);
        }

        [XmlIgnore]
        public decimal totalimpuestosretenidos { get; set; } // Aunque no se usa, se define para mantener la estructura
        [XmlElement("totalimpuestosretenidos")]
        public string totalimpuestosretenidosFormatted
        {
            get => totalimpuestosretenidos.ToString("F2", CultureInfo.InvariantCulture);
            set => totalimpuestosretenidos = decimal.Parse(value, CultureInfo.InvariantCulture);
        }

        [XmlIgnore]
        public decimal total { get; set; }
        [XmlElement("total")]
        public string totalFormatted
        {
            get => total.ToString("F2", CultureInfo.InvariantCulture);
            set => total = decimal.Parse(value, CultureInfo.InvariantCulture);
        }

        //para nota credito
        [XmlElement("ncidfact")]
        public XmlEmptyTagString ncidfact { get; set; }
        [XmlElement("nccod")]
        public XmlEmptyTagString nccod { get; set; }
        [XmlElement("nciddoc")]
        public XmlEmptyTagString nciddoc { get; set; }
        [XmlElement("ncuuid")]
        public XmlEmptyTagString ncuuid { get; set; }
        [XmlElement("ncfecha")]
        public XmlEmptyTagString ncfecha { get; set; }
        [XmlElement("ndidfact")]
        public XmlEmptyTagString ndidfact { get; set; }
        [XmlElement("ndcod")]
        public XmlEmptyTagString ndcod { get; set; }
        [XmlElement("ndiddoc")]
        public XmlEmptyTagString ndiddoc { get; set; }
        [XmlElement("nduuid")]
        public XmlEmptyTagString nduuid { get; set; }
        [XmlElement("ndfecha")]
        public XmlEmptyTagString ndfecha { get; set; }
    }
    public class Detalle
    {
        [XmlElement("llaveComprobante")]
        public XmlEmptyTagString llaveComprobante { get; set; }
        [XmlElement("idConcepto")]
        public XmlEmptyTagString idConcepto { get; set; }
        [XmlElement("cantidad")]
        public XmlEmptyTagString cantidad { get; set; }
        [XmlElement("unidadmedida")]
        public XmlEmptyTagString unidadmedida { get; set; }
        [XmlElement("descripcion")]
        public XmlEmptyTagString descripcion { get; set; }
        [XmlElement("tasa")]
        public XmlEmptyTagString tasa { get; set; }
        [XmlElement("tipo")]
        public XmlEmptyTagString tipo { get; set; }
        [XmlElement("identificacionproductos")]
        public XmlEmptyTagString identificacionproductos { get; set; } // Aunque no se usa, se define para mantener la estructura


        [XmlIgnore]        
        public decimal precioUnitario { get; set; }                
        [XmlElement("precioUnitario")]
        public string precioUnitarioFormatted
        {
            get => precioUnitario.ToString("F2", CultureInfo.InvariantCulture);
            set => precioUnitario = decimal.Parse(value, CultureInfo.InvariantCulture);
        }

        [XmlIgnore]
        public decimal importe { get; set; }
        [XmlElement("importe")]
        public string importeFormatted
        {
            get => importe.ToString("F2", CultureInfo.InvariantCulture);
            set => importe = decimal.Parse(value, CultureInfo.InvariantCulture);
        }

        [XmlIgnore]
        public decimal impuestolinea { get; set; }
        [XmlElement("impuestolinea")]
        public string impuestolineaFormatted
        {
            get => impuestolinea.ToString("F2", CultureInfo.InvariantCulture);
            set => impuestolinea = decimal.Parse(value, CultureInfo.InvariantCulture);
        }
        
        [XmlIgnore]
        public decimal baseimpuestos { get; set; }
        [XmlElement("baseimpuestos")]
        public string baseimpuestosFormatted
        {
            get => baseimpuestos.ToString("F2", CultureInfo.InvariantCulture);
            set => baseimpuestos = decimal.Parse(value, CultureInfo.InvariantCulture);
        }        
    }
    public class Descuentos
    {
        [XmlElement("llaveComprobante")]
        public XmlEmptyTagString llaveComprobante { get; set; }
        [XmlElement("razon")]
        public XmlEmptyTagString razon { get; set; }
        [XmlElement("coddescuento")]
        public XmlEmptyTagString coddescuento { get; set; }
        [XmlElement("porcentaje")]
        public XmlEmptyTagString porcentaje { get; set; }
        [XmlIgnore]
        public decimal basedescuento { get; set; }
        [XmlElement("basedescuento")]
        public string basedescuentoFormatted
        {
            get => basedescuento.ToString("F2", CultureInfo.InvariantCulture);
            set => basedescuento = decimal.Parse(value, CultureInfo.InvariantCulture);
        }
        [XmlIgnore]
        public decimal importe { get; set; }
        [XmlElement("importe")]
        public string importeFormatted
        {
            get => importe.ToString("F2", CultureInfo.InvariantCulture);
            set => importe = decimal.Parse(value, CultureInfo.InvariantCulture);
        }
    }
    public class Impuesto
    {
        [XmlElement("llaveComprobante")]
        public XmlEmptyTagString llaveComprobante { get; set; }
        [XmlElement("idImpuesto")]
        public XmlEmptyTagString idImpuesto { get; set; }
        [XmlElement("tipoImpuesto")]
        public XmlEmptyTagString tipoImpuesto { get; set; }


        [XmlIgnore]
        public decimal baseimpuestos { get; set; }
        [XmlElement("baseimpuestos")]
        public string baseimpuestosFormatted
        {
            get => baseimpuestos.ToString("F2", CultureInfo.InvariantCulture);
            set => baseimpuestos = decimal.Parse(value, CultureInfo.InvariantCulture);
        }

        [XmlIgnore]
        public decimal tasa { get; set; }
        [XmlElement("tasa")]
        public string tasaFormatted
        {
            get => tasa.ToString("F2", CultureInfo.InvariantCulture);
            set => tasa = decimal.Parse(value, CultureInfo.InvariantCulture);
        }
        
        [XmlIgnore]
        public decimal importe { get; set; }
        [XmlElement("importe")]
        public string importeFormatted
        {
            get => importe.ToString("F2", CultureInfo.InvariantCulture);
            set => importe = decimal.Parse(value, CultureInfo.InvariantCulture);
        }
    }
    public class Salud
    {
        [XmlElement("llaveComprobante")]
        public XmlEmptyTagString llaveComprobante { get; set; }
        [XmlElement("codPresSS")]
        public XmlEmptyTagString codPresSS { get; set; }
        [XmlElement("modConPag")]
        public XmlEmptyTagString modConPag { get; set; }
        [XmlElement("cobPan")]
        public XmlEmptyTagString cobPan { get; set; }
        [XmlElement("numCont")]
        public XmlEmptyTagString numCont { get; set; }
        [XmlElement("numPol")]
        public XmlEmptyTagString numPol { get; set; }

        [XmlIgnore]
        public decimal copago { get; set; }
        [XmlElement("copago")]
        public string copagoFormatted
        {
            get => copago.ToString("F2", CultureInfo.InvariantCulture);
            set => copago = decimal.Parse(value, CultureInfo.InvariantCulture);
        }

        [XmlIgnore]
        public decimal cuotaM { get; set; }
        [XmlElement("cuotaM")]
        public string cuotaMFormatted
        {
            get => cuotaM.ToString("F2", CultureInfo.InvariantCulture);
            set => cuotaM = decimal.Parse(value, CultureInfo.InvariantCulture);
        }

        [XmlIgnore]
        public decimal cuotaR { get; set; }
        [XmlElement("cuotaR")]
        public string cuotaRFormatted
        {
            get => cuotaR.ToString("F2", CultureInfo.InvariantCulture);
            set => cuotaR = decimal.Parse(value, CultureInfo.InvariantCulture);
        }

        [XmlIgnore]
        public decimal pagosComp { get; set; }
        [XmlElement("pagosComp")]
        public string pagosCompFormatted
        {
            get => pagosComp.ToString("F2", CultureInfo.InvariantCulture);
            set => pagosComp = decimal.Parse(value, CultureInfo.InvariantCulture);
        }
    }
}
