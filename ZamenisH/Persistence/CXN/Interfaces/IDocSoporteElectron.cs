using Domain.Contabilidad;
using Domain.CXN;
using System;
using System.Collections.Generic;

namespace Persistence.CXN.Interfaces
{
    public interface IDocSoporteElectron
    {
        List<CXN_PERSONASCOMPROBANTES> GetAllPeople();
        CXN_PERSONASCOMPROBANTES GetPersonDocOrId(string DocId, string TipoBusqueda);
        bool CrearPresona(CXN_PERSONASCOMPROBANTES P);
        bool ActualizarPersona(CXN_PERSONASCOMPROBANTES P);
        bool InsertarDocumentoSoporte(CXN_FACTURADOCSOPORTE P);
        bool InsertarCargo(CXN_CARGOSDOCSOPORTE P);
        List<DocumentoSoporte> GetDocumentoSoportePDF(int Cia, int Orden);
        List<CXN_FACTURADOCSOPORTE> GetDocumentsCliente(int IdCliente);
        List<CXN_FACTURA> GetDocumentsToSign(int Cia, DateTime Desde, DateTime Hasta);
        bool EliminarDocumento(int FacZam, int Cia);
    }
}
