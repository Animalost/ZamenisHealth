using System.Collections.Generic;

namespace Persistence.Informes.Interfaces
{
    public interface IInformeEnfermeria
    {
        int getCantidadAtendidos(int Bodega, string Mes, string Año);
        int getCantidadAtendidos2(string Mes, string Año, string TSERV);
        int getCantidadAtendidosMG(int Bodega, string Mes, string Año);
        int getCantidadPatologia(string Mes, string Año, string Patologia);
        int getCantidadPatologia2(string Mes, string Año, string Patologia);
        int getCantidadEdad(string Mes, string Año, string Edad);
        List<int> getEnfermerosYear(string Año);
        List<int> getMedicosYear(string Año);
        int getCantPacsForMonth(string Month, string Year, string TSERV);
        int getCantPacsForYear(string Month, string Year, string TSERV);
    }
}
