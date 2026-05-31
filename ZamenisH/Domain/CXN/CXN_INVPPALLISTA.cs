using System;

namespace Domain.CXN
{
    public class CXN_INVPPALLISTA : CXN_PROVEEDORES
    {
        public int Id { get; set; }
        public string Producto { get; set; }
        public int Proveedor { get; set; }
        public string CodigoProveedor { get; set; }
        public int ValorUnitario { get; set; }
        public DateTime Fecha { get; set; }
        public string CodigoPrestador { get; set; }
        public int Prestador { get; set; }
    }
}
