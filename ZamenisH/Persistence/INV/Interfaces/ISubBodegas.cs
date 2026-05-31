using Domain.INV;
using System;
using System.Collections.Generic;

namespace Persistence.INV.Interfaces
{
    public interface ISubBodegas
    {
        (int PosisionSub, int CantidadSub) GetInventarySubByBodPosProd(int Bodega, int PosProdTable);
        bool IngresarNuevo(INV_INVENTARIOBODEGAS I);
        bool ActualizarCantidad(INV_INVENTARIOBODEGAS I);
        List<INV_HISTORICOPPAL> LoadInventary(int SubBodega, string Filtro);
        (int Cantidad, int PosIdTablePpal) GetInventarySubByBodPos(int PosisionTablaInv);
        bool Reset(int SubBodega);
        void IngresarSalidaSub(INV_SALIDASSUB S);
        List<INV_HISTORICOPPAL> getHistorial(DateTime Desde, DateTime Hasta);
    }
}
