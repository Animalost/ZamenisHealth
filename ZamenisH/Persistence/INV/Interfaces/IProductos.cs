using Domain.INV;
using System;
using System.Collections.Generic;

namespace Persistence.INV.Interfaces
{
    public interface IProductos
    {
        INV_PRODUCTOS GetProducto(INV_PRODUCTOS P);
        INV_PRODUCTOS GetProductobyId(int Posision);
        int CrearProducto(INV_PRODUCTOS P);
        List<INV_PRODUCTOS> CargarProductos(string Filtro, INV_PRODUCTOS P);
        bool ActualizarProducto(INV_PRODUCTOS P);
        
        //Reportes
        List<ReportsINV> ReporteIngresos(DateTime Desde, DateTime Hasta);
        List<ReportsINV> ReporteSalidas(DateTime Desde, DateTime Hasta);
    }
}
