using Domain.CXN;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Controlador.Clases;

namespace Controlador.Services
{
    public class AgendaService
    {
        public async Task<List<CXN_HORARIO>> CargarAgenda(DateTime desde, int medico, int compañia, string dia) //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "POST";

                var datos = new
                {
                    Desde = Convert.ToDateTime(desde),
                    Medico = medico,
                    Compañia = compañia,
                    Dia = dia
                };

                return APIController.SendMessageToAPI<List<CXN_HORARIO>>(datos, "/api/Agenda/Horario", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }
        public async Task<string> consularAdmisionEstado(int admision) //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "POST";

                var datos = new
                {
                    Admision = admision
                };

                return APIController.SendMessageToAPI<string>(datos, "/api/Agenda/VerEstadoAdmision", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        } 
        public async Task<bool> updateTipoCitaMG(int Admision, string Inicio, string Tipo) //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "PATCH";

                var datos = new
                {
                    Admision = Admision,
                    Tipo = Tipo,
                    Inicio = Inicio
                };

                return APIController.SendMessageToAPI<bool>(datos, "/api/Agenda/UpdateTipoCitaMG", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }
        public async Task<CXN_HORARIO> DatosforMailSMS(int Admision) //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "POST";

                var datos = new
                {
                    Admision = Admision
                };

                return APIController.SendMessageToAPI<CXN_HORARIO>(datos, "/api/Agenda/DatosforMailSMS", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }
        public async Task<int> CrearCitaMedica(CXN_HORARIO request) //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "POST";

                string pacSal = (request.Hor_Pac_Sal != null || request.Hor_Pac_Sal != "" ? request.Hor_Pac_Sal : "");

                var datos = new
                {
                    Hor_Estado = request.Hor_Estado,
                    Hor_Pac_Id = request.Hor_Pac_Id,
                    Hor_Pac_Bod = request.Hor_Pac_Bod,
                    Hor_Pac_Tipo_Serv = request.Hor_Pac_Tipo_Serv,
                    Hor_Pac_Cia = request.Hor_Pac_Cia,
                    Hor_Pac_Ase = request.Hor_Pac_Ase,
                    Hor_Pac_Cup = request.Hor_Pac_Cup,
                    Hor_Pac_UsrGraba = request.Hor_Pac_UsrGraba,
                    Hor_Imp_Age = request.Hor_Imp_Age,
                    Hor_Pac_Fecha_Cita = request.Hor_Pac_Fecha_Cita,
                    Hor_Pac_Id_Hora = request.Hor_Pac_Id_Hora,
                    Hor_Pac_Hora_Cita = request.Hor_Pac_Hora_Cita,
                    Hor_Observacion = request.Hor_Observacion,
                    Hor_Pac_Sal = request.Hor_Pac_Sal,
                    Hor_Vales = request.Hor_Vales,
                    Hor_Pac_Modalidad = request.Hor_Pac_Modalidad,
                    Hor_BloqEspaces = request.Hor_BloqEspaces,
                    Hor_GrupoServicios = request.Hor_GrupoServicios,
                    Hor_Regimen = request.Hor_Regimen
                };

                return APIController.SendMessageToAPI<int>(datos, "/api/Agenda/CrearCitaMedica", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return 0;
            }
        }
        public async Task<otrosDatosPacienteHorario> CargarAdmision(int admision, string filtro) //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "POST";

                var datos = new
                {
                    Admision = admision,
                    Filtro = filtro
                };

                return APIController.SendMessageToAPI<otrosDatosPacienteHorario>(datos, "/api/Agenda/CargarAdmision", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }
        public async Task<int> AnularAdmision(int admision, string usuario) //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "PATCH";

                var datos = new
                {
                    Admision = admision,
                    Filtro = usuario
                };

                return APIController.SendMessageToAPI<int>(datos, "/api/Agenda/AnularAdmision", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return 0;
            }
        }
        public async Task<bool> getPendientes() //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "GET";
                return APIController.SendMessageToAPI<bool>(null, "/api/Agenda/getPendientes", false).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }
        public async Task<bool> desbloquearEspacio(int admision, string usuario) //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "PATCH";

                var datos = new
                {
                    Admision = admision,
                    Usuario = usuario
                };

                return APIController.SendMessageToAPI<bool>(datos, "/api/Agenda/desbloquearEspacio", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }
        public async Task<Dictionary<int, string>> getSaleConsultas(DateTime fecha, string tipo) //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "POST";

                var datos = new
                {
                    Tipo = tipo,
                    Fecha = Convert.ToDateTime(fecha),
                };

                return APIController.SendMessageToAPI<Dictionary<int, string>>(datos, "/api/Agenda/getSaleConsultas", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }
        public async Task<bool> UpdateSaleConsultas(int admision) //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "PATCH";

                var datos = new
                {
                    Admision = admision,
                };

                return APIController.SendMessageToAPI<bool>(datos, "/api/Agenda/UpdateSaleConsultas", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }
        public async Task<Dictionary<string, string>> ConsultaDatosAutorizacionMedGen(int pacid, DateTime fecha)
        {
            try
            {
                HttpClientHandler handler = new HttpClientHandler();
                handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;

                using (HttpClient client = new HttpClient(handler))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ControladorConfiguracion.Token);
                    client.Timeout = TimeSpan.FromSeconds(ControladorConfiguracion.TimeOutConection);

                    var DataJSON = new
                    {
                        Tipo = AESHelper.Encrypt(pacid.ToString()),
                        Fecha = Convert.ToDateTime(fecha)
                    };

                    var json = JsonConvert.SerializeObject(DataJSON);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    string endpoint = ControladorConfiguracion.URLAPIConexion.TrimEnd('/') + "/api/Admisiones/ConsultaDatosAutorizacionMedGen";
                    HttpResponseMessage response = await client.PostAsync(endpoint, content).ConfigureAwait(false);

                    if (response.IsSuccessStatusCode)
                    {
                        var jsonResponse = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonResponse);

                        return result;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }
        public async Task<CXN_HORARIO> GenerarImprentaAutomatica(int admision) //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "POST";

                var datos = new
                {
                    Admision = admision
                };

                CXN_HORARIO variable = APIController.SendMessageToAPI<CXN_HORARIO>(datos, "/api/Agenda/GenerarImprentaAutomatica", true).GetAwaiter().GetResult();

                return variable;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }
        public async Task<bool> consumirAutorizacion(int paciente, string autorizacion, string estado) //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "PATCH";

                var datos = new
                {
                    Paciente = paciente,
                    Autorizacion = autorizacion,
                    Estado = estado
                };

                return APIController.SendMessageToAPI<bool>(datos, "/api/Agenda/consumirAutorizacion", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }
        public async Task<bool> CancelacionInterna(string razon, string user, int horId, string motivo) //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "PATCH";

                var datos = new
                {
                    HorId = horId,
                    Razon = razon,
                    User = user,
                    Motivo = motivo
                };

                return APIController.SendMessageToAPI<bool>(datos, "/api/Agenda/CancelacionInterna", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }
        public async Task<bool> Inasistencia_Cita(string razon, int horId) //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "PATCH";

                var datos = new
                {
                    Admision = horId,
                    Filtro = razon
                };

                return APIController.SendMessageToAPI<bool>(datos, "/api/Agenda/Inasistencia_Cita", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }
        public async Task<bool> Retardo_Cita(string razon, int horId, string minutos) //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "PATCH";

                var datos = new
                {
                    HorId = horId,
                    Razon = razon,
                    Minutos = minutos
                };

                return APIController.SendMessageToAPI<bool>(datos, "/api/Agenda/Retardo_Cita", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }
        public async Task<List<CXN_DIAS_WEB>> CargarList(int codeprof) //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "POST";

                var datos = new
                {
                    Admision = codeprof
                };

                return APIController.SendMessageToAPI<List<CXN_DIAS_WEB>>(datos, "/api/Agenda/CargarList", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }
        public async Task<List<CXN_HORARIO>> consultaCancelaWEB(string Document) //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "POST";

                var datos = new
                {
                    document = Document
                };

                return APIController.SendMessageToAPI<List<CXN_HORARIO>>(datos, "/api/Agenda/consultaCancelaWEB", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }
        public async Task<bool> insert016(int pacid, DateTime fecha) //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "PATCH";

                var datos = new
                {
                    Admision = pacid,
                    Fecha = Convert.ToDateTime(fecha)
                };

                return APIController.SendMessageToAPI<bool>(datos, "/api/Agenda/insert016", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }
        public async Task<string> Calcular3(int Paciente, string TipoServicio, string Recepcion) //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "POST";

                var datos = new
                {
                    Admision = Paciente,
                    Tipo = TipoServicio,
                    InicioSesion = Recepcion
                };

                return APIController.SendMessageToAPI<string>(datos, "/api/DatosCitas/Calcular3", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }
        public async Task<bool> addValidacionPin(int admision, string Pin) //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "PATCH";

                var datos = new
                {
                    Admision = admision,
                    Filtro = Pin,
                };

                return APIController.SendMessageToAPI<bool>(datos, "/api/DatosCitas/addValidacionPin", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }
        public async Task<bool> addRegAtn(int admision, string registro) //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "PATCH";

                var datos = new
                {
                    Admision = admision,
                    Filtro = registro,
                };

                return APIController.SendMessageToAPI<bool>(datos, "/api/DatosCitas/addRegAtn", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }
        public async Task<bool> addAutroizacion(int horid, string autroizacion, int Cantidad) //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "PATCH";

                var datos = new
                {
                    HorId = horid,
                    Cantidad = Cantidad,
                    Autorizacion = autroizacion
                };

                return APIController.SendMessageToAPI<bool>(datos, "/api/Agenda/addAutroizacion", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }
        public async Task<bool> changeProfesional(int admision, 
                                                  int bodega, 
                                                  string idHora, 
                                                  string observacion, 
                                                  DateTime fechaCita, 
                                                  DateTime hora) //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "PATCH";

                var datos = new
                {
                    Admision = admision,
                    Bodega = bodega,
                    IdeHora = idHora,
                    Observacion = observacion,
                    FechaCita = fechaCita, 
                    Hora = hora
                };

                return APIController.SendMessageToAPI<bool>(datos, "/api/Agenda/changeProfesional", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

    }
}
