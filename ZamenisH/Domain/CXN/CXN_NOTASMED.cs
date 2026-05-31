using System;

namespace Domain.CXN
{
    public class CXN_NOTASMED
    {
        public int Id { get; set; }
        public int Admision { get; set; }
        public decimal Largo { get; set; }
        public decimal Ancho { get; set; }
        public decimal Profundidad { get; set; }
        public decimal Total { get; set; }
        public string Evolucion { get; set; }
        public string NovedadHerida { get; set; }
        public string txtGranulacion { get; set; }
        public string txtFibrina { get; set; }
        public string txtNecroticoHumedo { get; set; }
        public string txtNecroticoSeco { get; set; }
        public string txtEpitelizacion { get; set; }
        public string txtOtros { get; set; }
        public string txtLocalizacion { get; set; }
        public string Estado { get; set; }
        public string selExudado { get; set; }
        public DateTime Fecha { get; set; }



        public string RazonNoEvolucion { get; set; }
        public string RazonOtros { get; set; }
        public string RazonExudado { get; set; }
    }
}
