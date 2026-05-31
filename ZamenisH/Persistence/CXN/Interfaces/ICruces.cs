using Domain.CXN;

namespace Persistence.CXN.Interfaces
{
    public interface ICruces
    {
        CXN_VENTAS GetVentaRecepcion(string FacElectron);
        bool Cruzar(int Pos, string Tabla, string Usuario);
        CXN_HORARIO GetRecaudosBonos(string FacElectron);
        CXN_FACTURA Particulares(string FacElectron);
        void EliminaCierre(int Num_Cierre);
    }
}
