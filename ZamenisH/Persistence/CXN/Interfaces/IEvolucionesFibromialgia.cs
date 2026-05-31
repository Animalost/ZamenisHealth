using Domain.CXN;

namespace Persistence.CXN.Interfaces
{
    public interface IEvolucionesFibromialgia
    {
        CXN_EVOFIB getContador(CXN_EVOFIB E);
        bool saveEvolution(CXN_EVOFIB E);
    }
}
