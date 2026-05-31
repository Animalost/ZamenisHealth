using Domain;
using Domain.CXN;
using System.Collections.Generic;

namespace Persistence.CXN.Interfaces
{
    public interface IFormatos
    {
        List<string> getFormatos();
        CXN_FORMATOS getFormatoSelected(string Nombre);
        bool insertarFormato(CXN_FORMATOS F);
        bool updateFormato(CXN_FORMATOS F);
        bool crearFormato(CXN_FORMATOS F);
        List<CXN_FORMATOS> getFormatosxPaciente(string TID, string NID);
        List<FormatosR> Plantilla(CXN_FORMATOS F);
        List<FormatosR> Export_Cert(int Pac, int Doc, string Tipo);
    }
}
