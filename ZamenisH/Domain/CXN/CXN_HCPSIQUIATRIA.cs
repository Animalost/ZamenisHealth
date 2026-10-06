using System;

namespace Domain.CXN
{
    public class CXN_HCPSIQUIATRIA : ComplementPsiquiatria
    {
        public int PQ_Id { get; set; }
        public int PQ_Admision { get; set; }
        public string PQ_MotConsulta { get; set; }
        public string PQ_EnfActual { get; set; }
        public string PQ_Antecedentes { get; set; }
        public string PQ_Objetivo { get; set; }
        public string PQ_Analisis { get; set; }
        public string PQ_Plan { get; set; }
        public string PQ_DX1 { get; set; }
        public string PQ_DX2 { get; set; }
        public string PQ_DX3 { get; set; }
        public string PQ_NDX1 { get; set; }
        public string PQ_NDX2 { get; set; }
        public string PQ_NDX3 { get; set; }
        public string ImpDiagnostica { get; set; }
        public string TipoConsulta { get; set; }
    }

    public class ComplementPsiquiatria : CXN_CIA
    {
        public string Profesional { get; set; }
        public string IddProfesional { get; set; }
        public DateTime FechaNtoPac { get; set; }
        public string Sexo { get; set; }
        public string Servicio { get; set; }
    }
}
