using System;

namespace Persistence.CXN.Interfaces
{
    public interface IHistoriasFolios
    {
        void Genera_Export_Notas(int Pac_Id, DateTime Desde, DateTime Hasta);
        void Genera_Export_HCMG(int Pac_Id, DateTime Desde, DateTime Hasta);
    }
}
