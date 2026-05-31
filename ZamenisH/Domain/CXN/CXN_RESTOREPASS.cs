using System;

namespace Domain.CXN
{
    public class CXN_RESTOREPASS
    {
        public int Id { get; set; }
        public string Usuario { get; set; }
        public DateTime Fecha { get; set; }
        public string Habilitado { get; set; }
    }
}
