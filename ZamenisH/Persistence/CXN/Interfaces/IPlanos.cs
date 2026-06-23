using Domain.CXN;
using System;

namespace Persistence.CXN.Interfaces
{
    public interface IPlanos
    {
        void CA(DateTime desde, DateTime hasta);
        void RE(DateTime desde, DateTime hasta);
        void ANCS(DateTime desde, DateTime hasta);
        void RI(DateTime desde, DateTime hasta);
        void ExpPlanoFacturacion(CXN_FACTURA F);
        void ExpPlanoOrdenesPendientes(DateTime Desde, DateTime Hasta);
    }
}
