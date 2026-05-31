using System;

namespace Domain.CXN
{
    public class CXN_RDA
    {
        public int Id { get; set; }
        public string Especialidad { get; set; }
        public string RDAPaciente { get; set; }
        public string RDAAmbulatorio { get; set; }
        public DateTime? FechaReporteRDAPaciente { get; set; }
        public DateTime? FechaReporteRDAAmbulatorio { get; set; }
        public int Admision { get; set; }
        public string PersonaReportaRDAPaciente { get; set; }
        public string PersonaReportaRDAAmbulatorio { get; set; }
        public string URLPdf { get; set; }     
        public string idCompositionRecorded { get; set; }


        public string FechaReporteRDAPacienteText { get; set; }
        public string FechaReporteRDAAmbulatorioText { get; set; }
    }
}
