namespace Domain.CXN
{
    public class CXN_ALERGIASINSITE
    {
        public int Id { get; set; }
        public int Paciente { get; set; }
        public int Admision { get; set; }
        public string Alergia { get; set; }
        public string Observacion { get; set; }
        public string CodeFHIR { get; set; }
    }
}
