using Domain.CXN;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using Domain;

namespace Controlador.Services
{
    public class AdmisionesService
    {
        public async Task<List<CXN_HORARIO>> ListarCitasXPaciente(int pacid, int bodega, DateTime fecha) //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "POST";

                var datos = new
                {
                    PacId = pacid,
                    Fecha = Convert.ToDateTime(fecha),
                    Bodega = bodega
                };

                return APIController.SendMessageToAPI<List<CXN_HORARIO>>(datos, "/api/Admisiones/ListarCitasXPaciente", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        public async Task<string> consultarCitasMismoDia(int pacid, int bodega, DateTime fecha) //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "POST";

                var datos = new
                {
                    PacId = pacid,
                    Fecha = Convert.ToDateTime(fecha),
                    Bodega = bodega
                };

                return APIController.SendMessageToAPI<string>(datos, "/api/Admisiones/consultarCitasMismoDia", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        public async Task<int> updateCitaAdmisionar(CXN_HORARIO H) //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "PATCH";

                var datos = new
                {
                    Estado = H.Hor_Estado,
                    Autorizacion = H.Hor_Autoriza,
                    Validacion = H.Hor_Valida,
                    AseguradoraCode = H.Hor_Pac_Ase,
                    PacienteName = H.Hor_Imp_Age,
                    Regimen = H.Hor_Regimen,
                    Llegada = Convert.ToDateTime(H.Hor_Pac_Llegada),
                    AdmisionUsr = H.Hor_Usr_Admisiona,
                    Minutos = H.Hor_Pac_Minutos,
                    Razon = H.Hor_Pac_Razon,
                    Cup = H.Hor_Pac_Cup,
                    RegistroAtencion = H.Hor_RegAtn,
                    CantidadSesiones = H.Hor_CantSesion,
                    InicioSesion = H.Hor_IniciaSesion,
                    Observacion = H.Hor_Observacion,
                    ValidacionDerechos = H.Hor_ValDerechos,
                    Admision = H.Hor_Id
                };

                return APIController.SendMessageToAPI<int>(datos, "/api/Admisiones/updateCitaAdmisionar", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return 0;
            }
        }

        public async Task<int> PendientesChecked(int admision, string estado) //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "PATCH";

                var datos = new
                {
                    Admision = admision,
                    Usuario = estado
                };

                return APIController.SendMessageToAPI<int>(datos, "/api/Admisiones/PendientesChecked", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return 0;
            }
        }

        public async Task<int> InicioControlCuraciones(int admision, string estado) //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "PATCH";

                var datos = new
                {
                    Admision = admision,
                    Usuario = estado
                };

                return APIController.SendMessageToAPI<int>(datos, "/api/Admisiones/InicioControlCuraciones", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return 0;
            }
        }

        public async Task<bool> consularMGMismoDia(int admision, DateTime fecha) //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "POST";

                var datos = new
                {
                    Admision = admision,
                    Fecha = Convert.ToDateTime(fecha)
                };

                return APIController.SendMessageToAPI<bool>(datos, "/api/Admisiones/consularMGMismoDia", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        public async Task<(int Cantidad, string Clase)> GenerarImprentaAutomatica(int admision) //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "POST";

                var datos = new
                {
                    Admision = admision,
                };

                return APIController.SendMessageToAPI<(int Cantidad, string Clase)>(datos, "/api/Admisiones/GenerarImprentaAutomatica", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return (0, "");
            }
        }

        public async Task<List<FirmasR>> Firmas_Print(int admision, FirmasR F, bool autocomplete) //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "POST";

                var datos = new
                {
                    Admision = admision,
                    EmpresaNombre = F.Com_Nombre,
                    EmpresaDireccion = F.Com_Direccion,
                    EmpresaTelefono = F.Com_Telefono,
                    Logo = F.Com_Logo,
                    Autocompletar = autocomplete
                };

                return APIController.SendMessageToAPI<List<FirmasR>>(datos, "/api/Admisiones/Firmas_Print", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        public async Task<bool> OPend(CXN_OPEND O) //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "PATCH";

                var datos = new
                {
                    Admision = O.OP_Adm,
                    InicioSesion = O.OP_Estado,
                    Tipo = O.OP_Registra
                };

                return APIController.SendMessageToAPI<bool>(datos, "/api/Admisiones/OPend", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        public async Task<bool> _updateAseHorario(int ase, string pac, int adm) //HECHO
        {
            APIController.TypeEndPoint = "PATCH";

            var datos = new 
            { 
                Ase = ase, 
                Pac = pac,
                Adm = adm
            };

            return APIController.SendMessageToAPI<bool>(datos, "/api/Admisiones/_updateAseHorario", true).GetAwaiter().GetResult();
        }

        public async Task<bool> OpenAdmition(int admision) //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "POST";

                var datos = new
                {
                    Admision = admision
                };

                return APIController.SendMessageToAPI<bool>(datos, "/api/Agenda/OpenAdmition", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        public async Task<List<string>> CargarRazones() // HECHO
        {
            try
            {
                APIController.TypeEndPoint = "GET";
                return APIController.SendMessageToAPI<List<string>>(null, "/api/Admisiones/CargarRazones", false).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
                return new List<string>();
            }
        }

    }
}
