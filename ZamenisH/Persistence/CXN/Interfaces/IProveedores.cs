using Domain.CXN;
using System;
using System.Collections.Generic;

namespace Persistence.CXN.Interfaces
{
    public interface IProveedores
    {
        List<CXN_PROVEEDORES> getListadoProvs();
        void deleteProds(int Num_Ord);
        bool AddProducto(CXN_PEDIDOS P);
        List<CXN_PEDIDOS> getProdAdded(int Orden);
        bool InsertarPedido(CXN_PEDIDOSF F);
        Inventario_Proveedores getProducto(string Cod_Pro, int Id_Pro);
        bool deleteProdFromOrder(int Num_Orden);
        int getCodProvbyName(string name);
        CXN_PROVEEDORES getProvByCode(int Code);
        bool updateProveedor(CXN_PROVEEDORES P);
        bool createProveedor(CXN_PROVEEDORES P);
        List<CXN_PEDIDOSF> getPedidos(DateTime Desde, DateTime Hasta);
        List<Inventario_Proveedores> getInventario();
        Inventario_Proveedores getProdById(int Id);
        bool updateInventario(Inventario_Proveedores P);
        bool createInventario(Inventario_Proveedores P);
        string getNameProvbCode(int Code);
        bool updateInventario2(Inventario_Proveedores P);
    }
}
