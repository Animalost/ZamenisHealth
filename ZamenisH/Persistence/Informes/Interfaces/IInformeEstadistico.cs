using Domain;
using Domain.CXN;
using System;
using System.Collections.Generic;

namespace Persistence.Informes.Interfaces
{
    public interface IInformeEstadistico
    {
        List<CXN_HORARIO> getCitasAsistidas(int Pac, string Tipo);
        string getClase(int Admision);
        string getClaseCU(int Admision);
        void updateTable(string Table, string Tipe, int Adm);
        List<ExportInExcel> RptRecPaciente(DateTime Desde, DateTime Hasta, string tipoReporte);
    }
}
