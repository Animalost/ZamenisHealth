using System;

namespace Domain.CXN
{
    public class CXN_FIRMASDIGITALES_MED
    {
        public int Id { get; set; }
        public int Admision { get; set; }
        public int Paciente { get; set; }
        public string PacienteName { get; set; }
        public byte[] FirmaMedico { get; set; }
        public byte[] FirmaPaciente { get; set; }
        public DateTime Fecha { get; set; }
        public string Usuario { get; set; }
        public string Tipo { get; set; }


        public string IdPaciente { get; set; }
        public string IdProfesional { get; set; }
        public string DirPaciente { get; set; }
        public string TelPaciente { get; set; }

    }
}
