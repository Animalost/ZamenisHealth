using Domain.CXN;
using System.Collections.Generic;

namespace Persistence.CXN.Interfaces
{
    public interface IConfSystem
    {
        bool updateImagenSystem(CXN_IMAGEN_SYSTEM I);
        Dictionary<string, string> getListado();
        (bool Noticia, string Ruta) getDatoNoticias();
        string getURLConsentimientos(string TipoCon);
    }
}
