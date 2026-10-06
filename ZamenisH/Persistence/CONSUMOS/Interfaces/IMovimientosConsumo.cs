using Domain.CONSUMOS;
using System;
using System.Collections.Generic;

namespace Persistence.CONSUMOS.Interfaces
{
    public interface IMovimientosConsumo
    {
        bool SaveConsumo(CON_CONSUMOS C);
        List<CON_CONSUMOS> GetConsumos(int IdBodega, DateTime Fecha);
        bool AnularMovimiento(CON_CONSUMOS C);
        List<CON_CONSUMOS> GetConsumos(DateTime Fecha);
    }
}
