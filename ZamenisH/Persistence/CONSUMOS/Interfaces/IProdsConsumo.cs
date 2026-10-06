using Domain.CONSUMOS;
using System.Collections.Generic;

namespace Persistence.CONSUMOS.Interfaces
{
    public interface IProdsConsumo
    {
        List<CON_PRODUCTOS> GetProductos(string Filtro);
        CON_PRODUCTOS GetProductoById(int Id);
        bool CreaProducto(CON_PRODUCTOS C);
        bool UpdateProducto(CON_PRODUCTOS C);
        CON_PRODUCTOS GetProductoByCodeExtern(string Code);
        List<CON_PRODUCTOS> GetProductoByCodeInterno(string Filtro);
    }
}
