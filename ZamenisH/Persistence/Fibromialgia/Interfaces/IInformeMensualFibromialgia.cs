using Domain.Fibromialgia;
using System;
using System.Collections.Generic;

namespace Persistence.Fibromialgia.Interfaces
{
    public interface IInformeMensualFibromialgia
    {
        int getCantidad(string TipoBod, string Mes, int Año);
        (string Profesional, int Cantidad) getCantidadByProfesional(string TipoBod, string Mes, int Año, int Med);
        Byte[] GraficoCantidadPorProfesional(List<ClaseReportsFibro> datos);
        int getCantidadPacientesMES(string Mes, int Año, bool Total);
        List<(string Codigo, string Servicio)> getServices();
        int getCantidadByServ(string Codigo, string Mes, int Año);
    }
}
