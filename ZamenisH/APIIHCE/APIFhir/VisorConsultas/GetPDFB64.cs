using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace APIFhir.VisorConsultas
{
    public interface IGetPDFB64
    {
        Task<(string Resp, string Est)> GetDataB64(int Prestador, string endpointUrl);
    }

    public class GetPDFB64 : IGetPDFB64
    {
        private readonly IFHIR _context = new MFHIR();
        private readonly HttpClient _httpClient;

        public GetPDFB64()
        {
            _httpClient = new HttpClient();
        }

        async Task<(string Resp, string Est)> IGetPDFB64.GetDataB64(int Prestador, string endpointUrl)
        {
            try
            {
                string token = _context.RecuperarToken(Prestador);
                if (string.IsNullOrEmpty(token))
                    throw new Exception("No se pudo recuperar el token de autenticación.");                

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

                _httpClient.DefaultRequestHeaders.Accept.Clear();
                _httpClient.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/fhir+json"));

                StringContent content = null;
                HttpResponseMessage response = null;
               
                response = await _httpClient.GetAsync(endpointUrl);                

                var responseBody = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                    return (responseBody, "OK");

                return ($"{response.StatusCode}: {responseBody}", "ERROR");
            }
            catch (Exception ex)
            {
                return ($"Error al enviar el Bundle: {ex.Message}", "Exception");
            }
        }
    }
}
