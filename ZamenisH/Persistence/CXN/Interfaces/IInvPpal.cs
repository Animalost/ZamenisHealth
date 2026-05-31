using Domain.CXN;
using System.Collections.Generic;

namespace Persistence.CXN.Interfaces
{
    public interface IInvPpal
    {
        List<CXN_INVPPALLISTA> GetInventario(int Prestador);
        List<CXN_INVPPALLISTA> GetProducto(int Prestador, int Proveedor, string Codigo);
        int GetPosProducto(int Prestador, int Proveedor, string Codigo, int Valor);
        int InsertarProducto(CXN_INVPPALLISTA L);
        int InsertarProductoEnInventario(CXN_INVPPAL L);
        int GetPosInInventario(int CodePosision, int Prestador);
        int UpdateCantidad(CXN_INVPPAL I);
        CXN_INVPPALLISTA GetProdByPos(int Posision);
        void InsertarCargo(CXN_INVPPALCARGOS C);
        List<CXN_INVPPALCARGOS> GetCargos(int Prestador, string PrestadorName);
    }
}
