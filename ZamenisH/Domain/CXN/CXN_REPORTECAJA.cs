using System;

namespace Domain.CXN
{
    public class CXN_REPORTECAJA
    {
        public int Id { get; set; }
        public string Consecutivo { get; set; }
        public int Cia { get; set; }
        public DateTime Desde { get; set; }
        public DateTime Hasta { get; set; }
        public string Observacion { get; set; }
        public string Usuario { get; set; }
        public int BCincuenta { get; set; }
        public int BCincuentaCantidad { get; set; }
        public int BCincuentaValor { get; set; }
        public int BCien { get; set; }
        public int BCienCantidad { get; set; }
        public int BCienValor { get; set; }
        public int BVeinte { get; set; }
        public int BVeinteCantidad { get; set; }
        public int BVeinteValor { get; set; }
        public int BDiez { get; set; }
        public int BDiezCantidad { get; set; }
        public int BDiezValor { get; set; }
        public int BCinco { get; set; }
        public int BCincoCantidad { get; set; }
        public int BCincoValor { get; set; }
        public int BDosMil { get; set; }
        public int BDosMilCantidad { get; set; }
        public int BDosMilValor { get; set; }
        public int BMil { get; set; }
        public int BMilCantidad { get; set; }
        public int BMilValor { get; set; }
        public int MMil { get; set; }
        public int MMilCantidad { get; set; }
        public int MMilValor { get; set; }
        public int MQuinientos { get; set; }
        public int MQuinientosCantidad { get; set; }
        public int MQuinientosValor { get; set; }
        public int MDoscientos { get; set; }
        public int MDoscientosCantidad { get; set; }
        public int MDoscientosValor { get; set; }
        public int MCien { get; set; }
        public int MCienCantidad { get; set; }
        public int MCienValor { get; set; }
        public int MCincuenta { get; set; }
        public int MCincuentaCantidad { get; set; }
        public int MCincuentaValor { get; set; }
        public string EgresoRazon1 { get; set; }
        public int EgresoValor1 { get; set; }
        public string EgresoRazon2 { get; set; }
        public int EgresoValor2 { get; set; }
        public string EgresoRazon3 { get; set; }
        public int EgresoValor3 { get; set; }
        public string EgresoRazon4 { get; set; }
        public int EgresoValor4 { get; set; }
        public string EgresoRazon5 { get; set; }
        public int EgresoValor5 { get; set; }


        public string TipoPagoEfectivo { get; set; }
        public int TipoPagoEfectivoValor { get; set; }
        public string TipoPagoTC { get; set; }
        public int TipoPagoTCValor { get; set; }
        public string TipoPagoDB { get; set; }
        public int TipoPagoDBValor { get; set; }
        public string TipoPagoNequi { get; set; }
        public int TipoPagoNequiValor { get; set; }
        public string TipoPagoDaviplata { get; set; }
        public int TipoPagoDaviplataValor { get; set; }
        public string TipoPagoOtraBilletera { get; set; }
        public int TipoPagoOtraBilleteraValor { get; set; }

        public int TotalIngresos { get; set; }
        public int TotalEgresos { get; set; }
        public int TotalBilletes { get; set; }
        public int TotalMonedas { get; set; }
        public int AEntregar {  get; set; }
        public DateTime FechaGeneracion { get; set; }
    }
}
