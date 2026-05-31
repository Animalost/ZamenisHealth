using Domain.CXN;
using System.Collections.Generic;

namespace Persistence.CXN.Interfaces
{
    public interface ICambiosSolicitados
    {
        void InsertSolicitud(CXN_CAMBIOSSOLICITADOS B);
        List<CXN_CAMBIOSSOLICITADOS> GetCambios();
        CXN_CAMBIOSSOLICITADOS GetSolicitud(int Posision);
        void CambiarEstadoSolicitud(int Id, string Estado);
        (string DX1, string DX2, string DX3) getDX(int admision);
    }
}
