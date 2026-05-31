using Domain.CXN;

namespace Persistence.CXN.Interfaces
{
    public interface ITerapiaOcupacional
    {
        bool insertHistoria(CXN_HCTO H);
        CXN_HCTO getLastHistoy(int Paciente);
    }
}
