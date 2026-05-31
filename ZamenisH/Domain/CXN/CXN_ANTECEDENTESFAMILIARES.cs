using System;

namespace Domain.CXN
{
    public class CXN_ANTECEDENTESFAMILIARES
    {
        public int Id { get; set; }
        public string Parentesco { get; set; }
        public string CIECod { get; set; }
        public string CieDesc { get; set; }
        public int Paciente { get; set; }
        public string Tipo { get; set; }
        public DateTime Fecha { get; set; }
        public string Usuario { get; set; }
    }
}
