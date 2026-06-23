using Domain.CXN;
using System.Collections.Generic;

namespace Persistence.CXN.Interfaces
{
    public interface IInventario
    {
        List<Domain.CXN.CXN_INVENTARIO> getProductbyName(string Name);
        Domain.CXN.CXN_INVENTARIO getProductbyCode(string Code);
        Domain.CXN.CXN_INVENTARIO getProductbyId(int Pos);
        (int valor, string item, string detalle) ConsultarValor(int Ase, string Cod);
        List<Domain.CXN.CXN_INVENTARIO> getAllElements(int Convenio, string Dato);
        string listaPrecios(int Convenio);
        bool CrearProducto(Domain.CXN.CXN_INVENTARIO I);
        List<Domain.CXN.CXN_INVENTARIO> getAllProducts();
        bool updateProducto(Domain.CXN.CXN_INVENTARIO I);       
        List<Domain.CXN.CXN_INVENTARIO> getAllProducts(int Aseguradora);
        CXN_INVENTARIO ConsultarValor2(string Item, int Ase);
        List<Domain.CXN.CXN_INVENTARIO> getAllProductsByType(int Aseguradora, string Tipo);
    }
}
