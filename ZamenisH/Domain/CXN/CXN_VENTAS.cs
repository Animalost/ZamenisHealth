using System;
using System.Collections.Generic;

namespace Domain.CXN
{
    public class CXN_VENTAS
    {
        public string Ven_Cod { get; set; }
        public int Ven_Cantidad { get; set; }
        public int Ven_Precio { get; set; }
        public int Ven_Total { get; set; }
        public int Ven_Cod_Pac { get; set; }
        public int Ven_Cod_Cia { get; set; }
        public string Ven_Estado { get; set; }
        public string Ven_Factura { get; set; }
        public DateTime Ven_Fecha { get; set; }
        public string Ven_Item { get; set; }
        public DateTime Ven_Fecha_Anula { get; set; }
        public string Ven_Usr_Graba { get; set; }
        public string Ven_Usr_Anula { get; set; }
        public string Ven_Res { get; set; }
        public string Ven_Mot_Anula { get; set; }
        public string Ven_Homologo { get; set; }
        public int Ven_Deducciones { get; set; }
        public int Ven_Id { get; set; }
        public string Grafica { get; set; }
        public int Ven_Dcto { get; set; }
        public string Ven_Tipo_Doc { get; set; }
        public string Cufe { get; set; }
        public DateTime Hora { get; set; }
        public int DiasVencimiento { get; set; }
        public string MetodoPago { get; set; }
        public string MedioPago { get; set; }
        public string FormaPago { get; set; }

        public string PercentICA { get; set; }
        public string PercentFUENTE { get; set; }
        public decimal VrICA { get; set; }
        public decimal VrFUENTE { get; set; }

        public string Ven_UsrCruce { get; set; }
        public string Ven_Cruce { get; set; }
        public DateTime Ven_FechaCruce { get; set; }
        public int Num_Cruce { get; set; }
    }
    public class CXN_VENTAS_TEMP
    {
        public int IdPac { get; set; }
        public int IdCia { get; set; }
        public string MedioPago { get; set; }
        public string FormaPago { get; set; }
        public string MetodoPago { get; set; }
        public int Vencimiento { get; set; }
        public int Descuento { get; set; }
        public string RFuente { get; set; }
        public string RICA { get; set; }
        public List<CXN_VENTAS> Cargos { get; set; }
    }
}
