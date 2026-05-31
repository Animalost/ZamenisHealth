using System;
using System.Collections.Generic;

namespace Domain.CXN
{
    public class CXN_MESSENGER
    {
        public string Men_Mensaje { get; set; }
        public string Men_Usuario_Para { get; set; }
        public string Men_Usuario_De { get; set; }
        public DateTime Fecha { get; set; }
        public DateTime Hora { get; set; }
        public string Estado { get; set; }
        public int Men_Id { get; set; }


        public CXN_MESSENGER cXN_MESSENGER { get; set; }
        public List<CXN_MESSENGER> ListaOrdenada { get; set; }

    }
}
