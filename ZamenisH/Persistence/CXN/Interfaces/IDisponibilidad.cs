using Domain.CXN;
using System;
using System.Collections.Generic;

namespace Persistence.CXN.Interfaces
{
    public interface IDisponibilidad
    {
        List<CXN_DISPONIBILIDAD_2> GetHorarioByMedAndDay(int Bodega, string Dia);
        List<CXN_DISPONIBILIDAD_2> getHorariosByCodeMed(int Bodega);
        bool ConsultarCodigo(CXN_DISPONIBILIDAD_2 D);
        bool CrearHora(CXN_DISPONIBILIDAD_2 D);
        (string Habilita, DateTime Hora) ConsultarHora(int Posision);
        bool UpdateHora(CXN_DISPONIBILIDAD_2 D);
    }
}
