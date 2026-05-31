using System;

namespace Domain.CXN
{
    public class CXN_FACTURA
    {
        public int Fac_Num_Fac { get; set; }
        public string Fac_Estado { get; set; }
        public int Fac_Ase { get; set; }
        public int Fac_Cia { get; set; }
        public DateTime Fac_Fecha { get; set; }
        public DateTime Fac_Fecha_Des { get; set; }
        public DateTime Fac_Fecha_Has { get; set; }
        public string Fac_Num_Aut { get; set; }
        public string Fac_Descuento { get; set; }
        public string Fac_Observa { get; set; }
        public string Fac_Usr_Graba { get; set; }
        public int Fac_Pac { get; set; }
        public string DocE_1 { get; set; }
        public string DocE_2 { get; set; }
        public string DocE_3 { get; set; }
        public string DocE_4 { get; set; }
        public int DocE_5 { get; set; }
        public int DocE_6 { get; set; }
        public DateTime DocE_7 { get; set; }
        public DateTime DocE_8 { get; set; }
        public string Fac_Res { get; set; }
        public string Homologo { get; set; }
        public int Fac_Id { get; set; }
        public string Grafica { get; set; }
        public string Grafica_Val { get; set; }
        public string Fac_Tipo_Doc { get; set; }
        public string Fac_ConSub { get; set; }
        public int Num_Cruce { get; set; }
        public string Cufe { get; set; }
        public string QRCufe { get; set; }
        public int VrCompartido { get; set; }
        public int Copago { get; set; }
        public int Anticipo { get; set; }
        public string CodPrestador { get; set; }
        public string ContratoPoliza { get; set; }
        public string Cobertura { get; set; }
        public string ModPago { get; set; }
        public DateTime Hora { get; set; }
        public string CUV { get; set; }
        public int DiasVencimiento { get; set; }
        public string MetodoPago { get; set; }
        public string MedioPago { get; set; }

        public string FacResNumeracion { get; set; }
        public string FormaPago { get; set; }

        public string PercentICA { get; set; }
        public string PercentFUENTE { get; set; }
        public int VrICA { get; set; }
        public int VrFUENTE { get; set; }

        public string Fac_UsrCruce { get; set; }
        public string Fac_Cruce { get; set; }
        public string Fac_FechaCruce  { get; set; }
    }
}
