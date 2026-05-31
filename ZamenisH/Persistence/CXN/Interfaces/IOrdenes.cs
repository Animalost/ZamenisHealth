using Domain.CXN;
using Domain;
using System;
using System.Collections.Generic;

namespace Persistence.CXN.Interfaces
{
    public interface IOrdenes
    {
      
        void Rpt_Ordenes(DateTime Desde, DateTime Hasta);
        bool CrearOrden(CXN_OM OM);
        List<string> GetUMM();
        string GetUMMCode(string UMM);
        bool CrearOrdenFHIR(CXN_ORDENESFHIR OM);
        bool CrearOrdenFHIRMED(CXN_ORDENESFHIR OM);
        List<Ordenes> Generar_OrdenMedica(int Numero, int Compañia, string UserImprime);
        int getIdOMMEDFHIR(int Paciente, DateTime Fecha);
        List<CXN_ORDENESFHIR> getOrdenesAdmition(int Admision);
        bool CrearOrdenM(CXN_OM OM);             
        List<Ordenes> Genera_Orden_Medicamento(int Numero, int Compañia, string UserPrint);
        List<CXN_OM> getOrdenes(string TID, string NID, int Cia);
        CXN_OM getOrden(int Numero, string Tipo, int Cia);
        bool insertOM(CXN_OM O);
        bool SearchAutorization(string Autorization);
        bool updateAutorizacion(int Orden, string Autorizacion, int Cia);
        bool updateRadicar(int Orden, int Cia);
        string SearchTypeOrden(int Orden, int Cia);
        void consumirAutorizacion(int Paciente, string Autorizacion, string Estado);
        List<CXN_OM> getOrdenes(int Paciente, string Especialidad);
        int getIdOMFHIR(int Paciente, DateTime Fecha);
    }
}
