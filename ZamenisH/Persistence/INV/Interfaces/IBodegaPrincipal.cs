using Domain.CXN;
using Domain.INV;

using System.Collections.Generic;

namespace Persistence.INV.Interfaces
{
    public interface IBodegaPrincipal
    {
        List<CXN_BODEGAS> LoadConsultorios();
        INV_INVENTARIOPPAL GetCantidad(INV_INVENTARIOPPAL I);
        bool ActualizarCantidad(INV_INVENTARIOPPAL P, string User, bool Historico);
        bool IngresarNuevo(INV_INVENTARIOPPAL H, string User);
        List<INV_HISTORICOPPAL> GetInventary(INV_PRODUCTOS I);
        INV_HISTORICOPPAL GetInventaryPpalByPos(int PosisionTabla);
        List<INV_HISTORICOPPAL> LoadInventary(int Prestador);
        bool Reset();
    }
}
