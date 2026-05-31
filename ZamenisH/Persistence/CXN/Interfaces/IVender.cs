using Domain;
using Domain.CXN;
using System.Collections.Generic;
using System.Drawing;

namespace Persistence.CXN.Interfaces
{
    public interface IVender
    {
        CXN_PACIENTES LlamarPacienteNumDoc(string NId);
        void ActualizarCliente(CXN_PACIENTES P);
        List<CXN_CIA> getAllCompañias();
        CXN_CIA getPrestadorbyName(string NamePrestador);
        CXN_INVENTARIO getProductbyCode(string Code);
        CXN_PACIENTES LlamarPacientebyId(int pacid);
        CXN_CIA getPrestadorbyCode(int code);
        List<CXN_MEDIOSPAGO> ListaMediosPago();
        int Fuente(int SubTotal, string FuenteTarifa);
        decimal ICA(string IcaTarifa);
        bool insertarVenta(CXN_VENTAS ventas);
        bool UpdateFuenteICA(int Fuente, int Ica, int Orden, int Cia);
        List<FacturacionRpt> Exp_Fac_Ven(int Docu_Ven, int cia, string Tipo);
        List<CXN_VENTAS> getPrevios(int idPac);
        bool ConsecutivoActualiza(int Cia, string TipoDoc, int NuevoCons);
        List<CXN_INVENTARIO> getProductbyName(string Name);
        Dictionary<string, string> getListado();
        string getTipoDoc(string Tipo);
        bool ValidaEmail(string Val_Email);
        Dictionary<string, string> Claves(string nameClient, int Prestador);
        string GetTokenSaved(int Cia);
        Image CodifyQR(string T_Codifica);
        byte[] GetBytes(Image ImageIn);
    }
}
