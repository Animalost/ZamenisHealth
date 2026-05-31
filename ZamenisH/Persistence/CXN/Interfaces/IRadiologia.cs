using Domain.CXN;
using System;

namespace Persistence.CXN.Interfaces
{
    public interface IRadiologia
    {
        bool InsertarHistoria(CXN_HCRADIOLOGIA HC);
        bool ActualizaHistoria(CXN_HCRADIOLOGIA HC);
        CXN_HCRADIOLOGIA getLastHistory(int Paciente, DateTime Fecha);
        CXN_HCRADIOLOGIA Ingresado(int Admision);
    }
}
