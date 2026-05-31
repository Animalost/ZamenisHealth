using System.Collections.Generic;

namespace Domain.CXN
{
    public class CXN_BODEGAS
    {
        public int Bod_Numero { get; set; }
        public string Bod_Usuario { get; set; }
        public string Bod_Responsable { get; set; }
        public string Bod_Reg_Med { get; set; }
        public string Bod_Tipo { get; set; }
        public string Bod_Estado { get; set; }
        public int Bod_Id { get; set; }
        public string Bod_Firma { get; set; }

        public List<CXN_BODEGAS> ListaProfesionales { get; set; }
        public CXN_BODEGAS cXN_BODEGAS { get; set; }
    }
}
