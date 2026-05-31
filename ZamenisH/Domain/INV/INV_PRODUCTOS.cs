namespace Domain.INV
{
    public class INV_PRODUCTOS
    {
        public int Id { get; set; }
        public string CodigoInterno { get; set; }
        public string CodigoProveedor { get; set; }
        public string Nombre { get; set; }
        public string Observacion { get; set; }
        public int Proveedor { get; set; }
        public int Prestador { get; set; }
    }
}
