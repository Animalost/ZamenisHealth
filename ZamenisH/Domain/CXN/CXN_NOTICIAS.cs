using System;

namespace Domain.CXN
{
    public class CXN_NOTICIAS
    {
        public int Id { get; set; }
        public string URL { get; set; }
        public DateTime Fecha { get; set; }
        public bool Estado { get; set; }
        public string Resumen { get; set; }
        public string Titulo { get; set; }
    }
}