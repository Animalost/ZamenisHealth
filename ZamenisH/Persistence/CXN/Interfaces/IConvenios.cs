using Domain.CXN;
using System.Collections.Generic;

namespace Persistence.CXN.Interfaces
{
    public interface IConvenios
    {
        CXN_CONVENIOS ServicioNombre(string CUP, int Ase, string Tipo);
        List<string> CargarServiciosxASE(int Ase);
        CXN_CONVENIOS ServicioCUP(string Nombre, int Ase);
        List<string> CargarServicios(string TipoMed, int Ase);
        string NameServiceCUP(string Name);
        List<string> CargarServicios();
        List<CXN_CONVENIOS> getConvenios();
        CXN_CONVENIOS getConvenio(int Posision);
        bool updateConvenio(CXN_CONVENIOS C);
        bool createConvenio(CXN_CONVENIOS C);
        List<CXN_CONVENIOS> getServicesXAseServ(int Ase, string Serv);
        List<string> CargarCUPS(int Posision);
        CXN_CONVENIOS DatosServicioXNameAse(int Ase, string Name);
        List<CXN_CONVENIOS> getConvenios(int Ase);
    }
}
