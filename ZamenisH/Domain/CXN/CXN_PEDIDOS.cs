using System;

namespace Domain.CXN
{
    public class CXN_PEDIDOS
    {
        public int Cod_Pro { get; set; }
        public string Cod_Ins { get; set; }
        public string Item { get; set; }
        public DateTime Fecha { get; set; }
        public string Num_Pedido { get; set; }
        public string Usuario { get; set; }
        public string Cod_ItemI { get; set; }
        public string Estado { get; set; }
        public int Cantidad { get; set; }
        public string Cod_Item_Pro { get; set; }
        public string Observacion { get; set; }
        public string Tipo { get; set; }
        public int Id { get; set; }
    }
}
