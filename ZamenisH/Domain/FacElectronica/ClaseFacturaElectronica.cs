using System.Collections.Generic;

namespace Domain.FacElectronica
{
    public class XMLEncabezado
    {
        public string llaveComprobante { get; set; }
        public string nitemisor { get; set; }
        public string codSucursal { get; set; }
        public string tiporeceptor { get; set; }
        public string tipoDocRec { get; set; }
        public string nitreceptor { get; set; }
        public string digitoverificacion { get; set; }
        public string nombrereceptor { get; set; }
        public string mailreceptor { get; set; }
        public string tipocomprobante { get; set; }
        public string noresolucion { get; set; }
        public string prefijo { get; set; }
        public string folio { get; set; }
        public string mailreceptorcontacto { get; set; }
        public string paisreceptor { get; set; }
        public string fecha { get; set; }
        public string hora { get; set; }
        public string moneda { get; set; }
        public decimal subtotal { get; set; }
        public string metodopago { get; set; }
        public string mediopago { get; set; }
        public string fechavencimiento { get; set; }
        public string terminospago { get; set; }
        public decimal baseimpuesto { get; set; }
        public decimal totalsindescuento { get; set; }
        public decimal totaldescuentos { get; set; }
        public decimal totalimpuestos { get; set; }
        public decimal totalimpuestosretenidos { get; set; }
        public decimal total { get; set; }
        public string montoletra { get; set; }
        public string tipoOpera { get; set; }
        public string extra1 { get; set; }
        public string ordenCompra { get; set; }
        public string FechaInicial { get; set; }
        public string FechaFin { get; set; }
        public List<XMLDetalle> Detalle { get; set; }
        public List<XMLImpuesto> Impuesto { get; set; }
        public List<XMLSalud> Salud { get; set; }
        public List<DatosConexion> datosConexion { get; set; }
    }
    public class XMLDetalle
    {
        public string llaveComprobante { get; set; }
        public string idConcepto { get; set; }
        public decimal cantidad { get; set; }
        public string unidadmedida { get; set; }
        public string descripcion { get; set; }
        public decimal precioUnitario { get; set; }
        public decimal importe { get; set; }
        public decimal impuestolinea { get; set; }
        public decimal tasa { get; set; }
        public string tipo { get; set; }
        public decimal baseimpuestos { get; set; }
        public string identificacionproductos { get; set; }

    }
    public class XMLImpuesto
    {
        public string llaveComprobante { get; set; }
        public string idImpuesto { get; set; }
        public decimal baseimpuestos { get; set; }
        public decimal tasa { get; set; }
        public string tipoImpuesto { get; set; }
        public decimal importe { get; set; }

    }
    public class XMLSalud
    {
        public string llaveComprobante { get; set; }
        public string codPresSS { get; set; }
        public string modConPag { get; set; }
        public string cobPan { get; set; }
        public string numCont { get; set; }
        public string numPol { get; set; }
        public decimal copago { get; set; }
        public decimal cuotaM { get; set; }
        public decimal cuotaR { get; set; }
        public decimal pagosComp { get; set; }
    }
    public class DatosConexion
    {
        public string User { get; set; }
        public string Pass { get; set; }
        public string Orden { get; set; }
    }
}
