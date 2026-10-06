using System;
using System.Numerics;

namespace Domain.CONSUMOS
{
    public class CON_CONSUMOS : CON_PRODUCTOS
    {
        public int Con_Cons_Id { get; set; }
        public int Con_Cons_IdProducto { get; set; }
        public int Con_Cons_Cantidad { get; set; }
        public DateTime Con_Cons_Fecha { get; set; }
        public string Con_Cons_User { get; set; }
        public bool Con_Cons_Status { get; set; }
        public int Con_Cons_Idconsultorio { get; set; }
        public string Con_Cons_Usr_Anula { get; set; }
        public DateTime Con_Cons_Fecha_Anula { get; set; }

        //Extra
        public BigInteger UniqueCode { get; set; }
    }
}
