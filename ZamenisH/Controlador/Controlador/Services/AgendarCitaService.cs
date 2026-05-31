using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using Domain.CXN;

namespace Controlador.Services
{
    public class AgendarCitaService
    {
        public async Task<List<string>> ListaDocs() //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "GET";
                return APIController.SendMessageToAPI<List<string>>(null, "/api/Agendamiento/ListaDocs", false).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
                return null;
            }
        }

        public async Task<List<string>> ListaRegimen() //HECHO
        {
            try
            {
                try
                {
                    APIController.TypeEndPoint = "GET";
                    return APIController.SendMessageToAPI<List<string>>(null, "/api/Agendamiento/ListaRegimen", false).GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
                return null;
            }
        }

        public async Task<string> Calcular2(string paciente, string tipo, string inicio) //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "POST";

                var datos = new
                {
                    Tipo = tipo,
                    Admision = paciente,
                    InicioSesion = inicio
                };

                return APIController.SendMessageToAPI<string>(datos, "/api/Agendamiento/Calcular2", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }        
     
        public async Task<List<CXN_HORARIO>> CitasProximas(int paciente, DateTime fecha) //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "POST";

                var datos = new
                {
                    IdPaciente = paciente,
                    Fecha = fecha
                };

                return APIController.SendMessageToAPI<List<CXN_HORARIO>>(datos, "/api/Agendamiento/CitasProximas", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }
       
        public async Task<List<CXN_HORARIO>> CargarPrevios(int paciente) //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "POST";

                var datos = new
                {
                    IdPaciente = paciente
                };

                return APIController.SendMessageToAPI<List<CXN_HORARIO>>(datos, "/api/Agendamiento/CargarPrevios", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }
       
        public async Task<string> Observacioprevia(int admision) //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "POST";

                var datos = new
                {
                    IdPaciente = admision
                };

                return APIController.SendMessageToAPI<string>(datos, "/api/Agendamiento/Observacioprevia", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }
       
        public async Task<List<CXN_HORARIO>> ListarCitasXPaciente(int idpaciente, DateTime fecha) //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "POST";

                var datos = new
                {
                    IdPaciente = idpaciente,
                    Fecha = Convert.ToDateTime(fecha)
                };

                return APIController.SendMessageToAPI<List<CXN_HORARIO>>(datos, "/api/Agendamiento/ListarCitasXPaciente", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }
      
        public async Task<bool> EspacioRobado(int Cia, int Bod, string idHora, DateTime Fecha) //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "POST";

                var datos = new
                {
                    IdHora = idHora,
                    BodNumero = Bod,
                    CiaCode = Cia,
                    Fecha = Convert.ToDateTime(Fecha),
                };

                return APIController.SendMessageToAPI<bool>(datos, "/api/Agendamiento/EspacioRobado", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }
      
        public async Task<bool> ActualizarPaciente(CXN_PACIENTES P) //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "PATCH";

                var datos = new
                {
                    Pac_PrimerN = P.Pac_PrimerN,
                    Pac_SegundoN = P.Pac_SegundoN,
                    Pac_PrimerA = P.Pac_PrimerA,
                    Pac_SegundoA = P.Pac_SegundoA,
                    Pac_Aseguradora = P.Pac_Aseguradora,
                    Pac_Telefono = P.Pac_Telefono,
                    Pac_TelefonoAux = P.Pac_TelefonoAux,
                    Pac_Email = P.Pac_Email,
                    Pac_IdNum = P.Pac_IdNum,
                    Pac_TipoId = P.Pac_TipoId,
                    Pac_Direccion = P.Pac_Direccion,
                    Pac_FechaNto = Convert.ToDateTime(P.Pac_FechaNto),
                    Pac_Regimen = P.Pac_Regimen,
                    Pac_Doble = P.Pac_Doble,
                    Pac_Id = P.Pac_Id,
                    Pac_2VXS = P.Pac_2VXS,
                    Pac_Especial = P.Pac_Especial,
                    Pac_Categoria = P.Pac_Categoria,
                    Pac_Sexo = P.Pac_Sexo
                };

                return APIController.SendMessageToAPI<bool>(datos, "/api/Agendamiento/ActualizarPaciente", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }
       
        public async Task<bool> updateObservaTemp(string ObTemp, int admision) //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "PATCH";

                var datos = new
                {
                    Admision = admision,
                    Filtro = ObTemp,
                };

                return APIController.SendMessageToAPI<bool>(datos, "/api/Agendamiento/updateObservaTemp", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

    }
}
