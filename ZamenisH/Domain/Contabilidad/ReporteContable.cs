using System;


namespace Domain.Contabilidad
{
    public class ReporteContable
    {        
        public DateTime Fecha { get; set; }
        public string TipoDocumento { get; set; }
        public int NumeroDocumento { get; set; }
        public string Cuenta { get; set; }
        public string Concepto { get; set; }
        public string Identidad { get; set; }
        public string CentroCosto { get; set; }
        public int Valor { get; set; }
        public decimal ValorD { get; set; }
        public string Naturaleza { get; set; }
        public string Clase { get; set; }
        public int ConsecutivoContable { get; set; }
        public int Descuentos { get; set; }
        public string IVA { get; set; }
        public string Tabla { get; set; }
        public int IdentificadorAse { get; set; }
        public string ClaseFactura { get; set; }
        public int FacturaZamenis { get; set; }
        public string ICA { get; set; }
        public string ReteFuente { get; set; }
    }
}
