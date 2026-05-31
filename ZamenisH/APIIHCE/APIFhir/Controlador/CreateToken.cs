using System.Collections.Generic;
using System.Threading.Tasks;

namespace APIFhir.Controlador
{
    public interface CreateToken
    {
        Task<string> ObtenerTokenIHCE(int Prestador);
        Dictionary<string, string> getDataMinSalud(int Prestador);
    }
}
