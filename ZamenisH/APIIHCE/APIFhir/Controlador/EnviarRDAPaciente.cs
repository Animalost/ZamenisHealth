using System.Threading.Tasks;

namespace APIFhir.Controlador
{
    public interface EnviarRDAPaciente
    {
        Task<(string Resp, string Est)> SendBundleAsync(string json, int Prestador, string TipoRDA);
    }
}
