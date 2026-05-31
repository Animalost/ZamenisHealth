namespace Domain.INV
{
    public class INV_INVENTARIOBODEGAS
    {
        public int Id { get; set; }
        public int CodInvPpal { get; set; }
        public int Cantidad { get; set; }
        public int Bodega { get; set; }

        public string Lote { get; set; }
        public string Factura { get; set; }
        public int Costo { get; set; }
        public int IVA { get; set; }
    }
}
