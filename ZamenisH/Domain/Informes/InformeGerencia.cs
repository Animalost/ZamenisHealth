using System;
using System.Numerics;

namespace Domain.Informes
{
    public class InformeGerencia : InformeGerencia_DatoPaciente
    {
        public string Mes { get; set; }
        public int Año { get; set; }
        public int Aseguradora { get; set; }
        public int Compañia { get; set; }
        public DateTime FechaCorte { get; set; }
    }

    public class InformeGerencia_DatoPaciente 
    {
        public int IdPaciente { get; set; }
        public int FacturaZamenis { get; set; }
        public string FacturaElectronica { get; set; }
        public string TipoDoc { get; set; }
        public string Patologia { get; set; }
    }

    public class InformeGerencia_Imprime : InformeGerencia
    {
        public DateTime FechaFactura { get; set; }
        public DateTime FechaFacturaDesde { get; set; }
        public DateTime FechaFacturaHasta { get; set; }        
        public string Paciente { get; set; }
        public string Identificacion { get; set; }
        public int ValorFactura { get; set; }
        public int ValorTotalFacturacion { get; set; }
        public int TotalFacturaPorPaciente { get; set; }
        public string UsuarioGeneraFactura { get; set; }
        public byte[] Logo { get; set; }
        public string Empresa { get; set; }
        public string MesGeneracion { get; set; }
        public int TotalFacturasGlobal { get; set; }
        public BigInteger VrFacturasGlobal { get; set; }
    }
}
