using Domain.CXN;

namespace Persistence.CXN.Interfaces
{
    public interface ITerapiaFisica
    {
        bool insertHistoria(CXN_HCTF H);
        CXN_HCTF getLastHistory(int Paciente);
    }
}
