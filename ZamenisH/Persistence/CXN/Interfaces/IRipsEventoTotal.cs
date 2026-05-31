using System;

namespace Persistence.CXN.Interfaces
{
    public interface IRipsEventoTotal
    {
        void setDatos(int _aseguradora, int _compañia, DateTime _desde, DateTime _hasta, string _tipo_documento, string _regimen);
    }
}
