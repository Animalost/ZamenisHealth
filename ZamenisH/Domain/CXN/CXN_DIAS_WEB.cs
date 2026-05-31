using System;
using System.Collections.Generic;

namespace Domain.CXN
{
    public class CXN_DIAS_WEB
    {
        public DateTime F_Fecha { get; set; }
        public string R_Razon { get; set; }
        public int F_Prof { get; set; }
        public int F_Id { get; set; }
        public string F_Bloquea { get; set; }
        public string F_Desbloquea { get; set; }
        public DateTime F_FBloquea { get; set; }
        public DateTime F_FDesbloquea { get; set; }
        public string F_Estado { get; set; }
        public List<CXN_DIAS_WEB> cXN_DIAS_WEB { get; set; }
    }

    public class DiasAgenda
    {
        public string aOr { get; set; }
        public string oAño { get; set; }
        public string oMes { get; set; }
        public string oDia { get; set; }
    }
}
