namespace Domain.CXN
{
    public class CXN_ORDENESFHIR
    {
        public int Id { get; set; }
        public int Admision { get; set; }
        public string CUP { get; set; }
        public string Servicio { get; set; }
        public string Tipo { get; set; }
        public int Paciente { get; set; }
        public string DX1 { get; set; }
        public string DX2 { get; set; }
        public string DX3 { get; set; }
        public string Medico { get; set; }

        public string Medicamento { get; set; }
        public string CodMedicamento { get; set; }
        public string Via { get; set; }
        public string Cada { get; set; }
        public string FrecAdmi { get; set; }
        public string Cantidad { get; set; }
        public string UMM { get; set; }
        public string Duracion { get; set; }
        public string Tiempo { get; set; }
        public string TipoTecnologia { get; set; }
        public string Observacion { get; set; }

    }
}
