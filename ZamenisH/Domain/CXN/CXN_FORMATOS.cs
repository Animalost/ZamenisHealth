using System;

namespace Domain.CXN
{
    public class CXN_FORMATOS : ComplementoFormatos
    {
        public string Nombre { get; set; }
        public string Cuerpo { get; set; }
        public string Firma { get; set; }
        public string Firma2 { get; set; }
        public string Pie { get; set; }
        public string CiudadFecha { get; set; }
        public DateTime Fecha { get; set; }
        public string Usuario { get; set; }
        public int Id { get; set; }
        
    }

    public class ComplementoFormatos
    {
        public int Paciente { get; set; }
        public int Compañia { get; set; }
    }
}
