
namespace Domain.CONSUMOS
{
    public class CON_PRODUCTOS : CON_ASIGNACION
    {
        public int Con_Prod_Id { get; set; }
        public string Con_Prod_Cod_Interno { get; set; }
        public string Con_Prod_Cod_Externo { get; set; }
        public string Con_Prod_Name { get; set; }
        public bool Con_Prod_Status { get; set; }
    }
}
