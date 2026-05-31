using Domain.CXN;
using System.Collections.Generic;

namespace Persistence.CXN.Interfaces
{
    public interface IDocumentosWEB
    {
        List<ConsentimientosTemp> GetHechos(int Cia, string Documento);
        List<ConsentimientosTemp> Export(int Pos);
    }
}
