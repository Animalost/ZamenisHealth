using ICD11.ClasesICD11;
using Newtonsoft.Json;
using Persistence;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ICD11.ServiceICD11
{
    public class ICD11ApiService
    {
        private string BaseUrl = Conexion.URL_CIE11; // proxy inverso
        private string Release = Conexion.URL_Version_CIE11; 
        private static readonly HttpClient httpClient;

        static ICD11ApiService()
        {
            httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Add(
                "API-Version", "v2");

            httpClient.DefaultRequestHeaders.Add(
                "Accept-Language", "es");

            httpClient.DefaultRequestHeaders.Add(
                "Accept", "application/json");

            httpClient.Timeout = TimeSpan.FromSeconds(30);
        }
        public async Task<ICD11Classification> ObtenerClasificacionAsync()
        {
            string url = $"{BaseUrl}/icd/release/11/{Release}/mms";

            HttpResponseMessage response = await httpClient.GetAsync(url);

            response.EnsureSuccessStatusCode();

            string json = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<ICD11Classification>(json);
        }
        public async Task<ICD11SearchResponse> BuscarDiagnosticosAsync(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                throw new ArgumentException("Debe ingresar un diagnóstico para buscar.");

            string textoCodificado = Uri.EscapeDataString(texto.Trim());

            string url = $"{BaseUrl}/icd/release/11/{Release}/mms/search?q={textoCodificado}";

            HttpResponseMessage response = await httpClient.GetAsync(url);

            response.EnsureSuccessStatusCode();

            string json = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<ICD11SearchResponse>(json);
        }
        public async Task<ICD11Diagnosis> ObtenerDiagnosticoAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("El identificador del diagnóstico es obligatorio.");

            // La API devuelve identificadores URI de la OMS.
            // Extraemos el último segmento para consultar el servidor local.
            string identificador = id.TrimEnd('/');

            int posicion = identificador.LastIndexOf('/');

            if (posicion >= 0)
                identificador = identificador.Substring(posicion + 1);

            if (string.IsNullOrWhiteSpace(identificador))
                throw new ArgumentException("El identificador del diagnóstico no es válido.");

            string url = $"{BaseUrl}/icd/release/11/{Release}/mms/{Uri.EscapeDataString(identificador)}";

            HttpResponseMessage response = await httpClient.GetAsync(url);

            response.EnsureSuccessStatusCode();

            string json = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<ICD11Diagnosis>(json);
        }
    }
}
