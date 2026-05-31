using System;

namespace Domain.CXN
{
    public class CXN_IMAGENES : CXN_PACIENTES
    {
        public int Ima_Id { get; set; }
        public int Ima_Adm { get; set; }
        public int Ima_Med { get; set; }
        public int Ima_Pac { get; set; }
        public string Ima_Nota { get; set; }
        public string Ima_Ruta { get; set; }
        public DateTime Ima_Fecha { get; set; }
        public byte[] Ima_Grafica { get; set; }
    }
}
