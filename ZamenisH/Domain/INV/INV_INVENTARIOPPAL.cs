namespace Domain.INV
{
    public class INV_INVENTARIOPPAL
    {
        public int Id { get; set; }
        public int CodProducto { get; set; }
        public string Lote { get; set; }
        public string Factura { get; set; }
        public int Cantidad { get; set; }
        public int Costo { get; set; }
        public int IVA { get; set; }
    }
}
