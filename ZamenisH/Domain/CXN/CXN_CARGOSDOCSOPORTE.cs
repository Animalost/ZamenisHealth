namespace Domain.CXN
{
    public class CXN_CARGOSDOCSOPORTE
    {
        public int Id { set; get; }
        public string Concepto { set; get; }
        public int Cantidad { set; get; }
        public decimal VrUnitario { set; get; }
        public decimal VrTotal { set; get; }
        public string Documentoelectronico { set; get; }
        public int DocumentoOrden { set; get; }
    }
}
