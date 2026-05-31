namespace Domain.CXN
{
    public class CXN_FIRMASDIGITALES
    {
        public int Id { get; set; }
        public int Admision { get; set; }
        public int Paciente { get; set; }
        public byte[] Firma { get; set; }
        public string Usuario { get; set; }
    }
}
