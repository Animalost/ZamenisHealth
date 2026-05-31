using System;

namespace Domain.CXN
{
    public class CXN_OPEND
    {
        public int OP_Id { get; set; }
        public int OP_Adm { get; set; }
        public string OP_Estado { get; set; }
        public string OP_Registra { get; set; }
        public string OP_Cambia { get; set; }
        public DateTime OP_EstadoChange { get; set; }
    }
}
