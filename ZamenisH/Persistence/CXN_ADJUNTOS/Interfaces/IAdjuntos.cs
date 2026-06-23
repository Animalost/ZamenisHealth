using Domain.CXN;
using Domain.CXN_ADJUNTOS;
using System.Collections.Generic;

namespace Persistence.CXN_ADJUNTOS.Interfaces
{
    public interface IAdjuntos
    {
        int uploadFile(Adj_Archivos A);
        List<CXN_HORARIO> getAdjuntos(int Paciente);
        byte[] getPDF(int Posision);
    }
}
