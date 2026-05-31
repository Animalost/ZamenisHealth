using Domain.CXN;
using System.Collections.Generic;
using System.Data;

namespace Persistence.CXN.Interfaces
{
    public interface IBodegas
    {
        List<string> Profesionales(string SeleccionProfesional);
        (int CodProf, string TipoBod) ProfesionalId(string IdProf);
        string ProfesionalNombre(int Code);
        List<CXN_BODEGAS> Filtrar();
        void ActivarDesactivarBodega(int Bode, string Estado);
        bool EsProfesional(string Tipo, string User);
        string NombreProfesionalXUser(string User);
        List<string> getProfByTipo(string Tipo);
        CXN_BODEGAS getDatosUser(string User);
        bool updateUser(CXN_BODEGAS B);
        bool createUser(CXN_BODEGAS B);
        bool registerFirma(CXN_BODEGAS B);
        List<string> getTipos();
        DataTable Profesionales2(string SeleccionProfesional);
        List<string> getBodegas();
        CXN_BODEGAS getDatosName(string Responsable);
        CXN_BODEGAS getDatosCode(int Code);
        List<int> FiltrarCodesByTipose(string Tipo);
    }
}
