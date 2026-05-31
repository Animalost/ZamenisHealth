using Domain.CXN;

namespace Persistence.CXN.Interfaces
{
    public interface IPsicologia
    {
        CXN_HCPSI getLastHistory(int Paciente);
        bool saveHistory(CXN_HCPSI H);
    }
}
