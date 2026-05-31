using Domain;
using System;

namespace Persistence.CXN.Interfaces
{
    public interface IRIPS
    {
        void setDatos(int _aseguradora, int _compañia, DateTime _desde, DateTime _hasta, string _tipo_documento, string _regimen, string FacElectron);
        void RepararRIPSCupCarVsHor(RIPS_Class R);
        int TipoRipCargo(string Valor, string Tipo);
        string TipoRipCargo(int Valor, string Tipo);
        void updateCargo(int ambito, int personal, int cexterna, int finalidad, int motivo, int impdx, int adm, int CodeEgreso);
        void updateRegimenMP(DateTime Desde, DateTime Hasta, int Cia, int Ase);
    }
}
