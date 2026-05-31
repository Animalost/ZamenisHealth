using APIFhir.Controlador;
using APIFhir.Servicio;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace APIFhir.VisorConsultas
{
    public interface IConsultaPaciente
    {
        Task<(string Resp, string Est)> GetData(string json, int Prestador, string TipoRDA, string IdComposition);
    }

    public class ConsultaPaciente : IConsultaPaciente
    {
        private readonly CreateToken datosMINSALUD = new EndPoint_Token();
        private readonly IFHIR _context = new MFHIR();
        private readonly HttpClient _httpClient;

        public ConsultaPaciente()
        {
            _httpClient = new HttpClient();
        }

        async Task<(string Resp, string Est)> IConsultaPaciente.GetData(string json, int Prestador, string TipoRDA, string IdComposition) 
        {
            try
            {
                string token = _context.RecuperarToken(Prestador);
                if (string.IsNullOrEmpty(token))
                    throw new Exception("No se pudo recuperar el token de autenticación.");

                string SERVIDOR = datosMINSALUD.getDataMinSalud(10)["EndPoint"];
                string endpointUrl = "";

                if (TipoRDA == "DataPac")
                {
                    endpointUrl = $"{SERVIDOR}/Patient/$consultar-paciente-exacto"; //Hecho
                }
                else if (TipoRDA == "ListComposition") //Hecho
                {
                    endpointUrl = $"{SERVIDOR}/Composition/$consultar-rda-paciente"; //Hecho
                }
                else if (TipoRDA == "GetIdComposition") //Hecho
                {
                    endpointUrl = $"{SERVIDOR}/Composition/{IdComposition}/$document"; //Hecho
                }
                else if (TipoRDA == "Organization") //Hecho
                {
                    endpointUrl = $"{SERVIDOR}/{IdComposition}"; //Hecho
                }
                else if (TipoRDA == "Ocupacion") //Hecho
                {
                    endpointUrl = $"{SERVIDOR}/{IdComposition}"; //Hecho
                }
                else if (TipoRDA == "Medicamentos") //Hecho
                {
                    endpointUrl = $"{SERVIDOR}/{IdComposition}"; //Hecho
                }
                else if (TipoRDA == "DX") //Hecho
                {
                    endpointUrl = $"{SERVIDOR}/{IdComposition}"; //Hecho
                }
                else if (TipoRDA == "Allergy") //Hecho
                {
                    endpointUrl = $"{SERVIDOR}/{IdComposition}"; //Hecho
                }
                else if (TipoRDA == "ServiceRequest") //Hecho
                {
                    endpointUrl = $"{SERVIDOR}/{IdComposition}"; //Hecho
                }
                else if (TipoRDA == "Practitioner") //Hecho
                {
                    endpointUrl = $"{SERVIDOR}/{IdComposition}"; //Hecho
                }
                else if (TipoRDA == "ListEncounters") //Hecho
                {
                    endpointUrl = $"{SERVIDOR}/Composition/$consultar-rda-encuentros-clinicos"; //Hecho
                }
                else if (TipoRDA == "Inmunization") //Hecho
                {
                    endpointUrl = $"{SERVIDOR}/Immunization/$consultar-inmunizacion"; //Hecho
                }
                else if (TipoRDA == "Incapacidad") //Hecho
                {
                    endpointUrl = $"{SERVIDOR}/{IdComposition}"; //Hecho
                }
                else if (TipoRDA == "Encounter") //Hecho
                {
                    endpointUrl = $"{SERVIDOR}/{IdComposition}"; //Hecho
                }
                else
                {
                    return ("RDA No Definido", "ERROR RDA");
                }                

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

                _httpClient.DefaultRequestHeaders.Accept.Clear();
                _httpClient.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/fhir+json"));

                StringContent content = null;
                HttpResponseMessage response = null;

                if (TipoRDA == "DataPac") //Hecho
                {
                    content = new StringContent(json, Encoding.UTF8, "application/fhir+json");
                    response = await _httpClient.PostAsync(endpointUrl, content);
                }
                else if (TipoRDA == "ListComposition") //Hecho
                {
                    content = new StringContent(json, Encoding.UTF8, "application/fhir+json");
                    response = await _httpClient.PostAsync(endpointUrl, content);
                }
                else if (TipoRDA == "ListEncounters") //Hecho
                {
                    content = new StringContent(json, Encoding.UTF8, "application/fhir+json");
                    response = await _httpClient.PostAsync(endpointUrl, content);
                }
                else if (TipoRDA == "GetIdComposition") //Hecho
                {
                    response = await _httpClient.GetAsync(endpointUrl);
                }
                else if (TipoRDA == "Organization") //Hecho
                {
                    response = await _httpClient.GetAsync(endpointUrl);
                }
                else if (TipoRDA == "Ocupacion") //Hecho
                {
                    response = await _httpClient.GetAsync(endpointUrl);
                }
                else if (TipoRDA == "Medicamentos") //Hecho
                {
                    response = await _httpClient.GetAsync(endpointUrl);
                }
                else if (TipoRDA == "Allergy") //Hecho
                {
                    response = await _httpClient.GetAsync(endpointUrl);
                }
                else if (TipoRDA == "ServiceRequest") //Hecho
                {
                    response = await _httpClient.GetAsync(endpointUrl);
                }
                else if (TipoRDA == "DX") //Hecho
                {
                    response = await _httpClient.GetAsync(endpointUrl);
                }
                else if (TipoRDA == "Practitioner") //Hecho
                {
                    response = await _httpClient.GetAsync(endpointUrl);
                }
                else if (TipoRDA == "Inmunization") //Hecho
                {
                    content = new StringContent(json, Encoding.UTF8, "application/fhir+json");
                    response = await _httpClient.PostAsync(endpointUrl, content);
                }
                else if (TipoRDA == "Incapacidad") //Hecho
                {
                    response = await _httpClient.GetAsync(endpointUrl);
                }
                else if (TipoRDA == "Encounter") //Hecho
                {
                    response = await _httpClient.GetAsync(endpointUrl);
                }

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
