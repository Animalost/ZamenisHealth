using System;

namespace Domain.CXN
{
    public class CXN_SALIDASENFERMERIA
    {
        public int Sal_Id { get; set; }
        public int Sal_Adm { get; set; }
        public DateTime Sal_Fecha { get; set; }
        public int Sal_Prof { get; set; }
        public bool Sal_Estado { get; set; }
    }
}
