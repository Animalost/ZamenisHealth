using System;

namespace Domain.CXN_ADJUNTOS
{
    public class PDF
    {
        public int Id { get; set; }
        public byte[] Imagen { get; set; }
        public string Carpeta { get; set; }
        public string Archivo { get; set; }
        public DateTime Fecha { get; set; }
        public string Usuario { get; set; }
    }
}
