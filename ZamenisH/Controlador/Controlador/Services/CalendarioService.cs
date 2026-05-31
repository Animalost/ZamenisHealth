using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using Domain.CXN;

namespace Controlador.Services
{
    public class CalendarioService
    {
        public async Task<List<DiasAgenda>> FechasFestivas()  // HECHO
        {
            try
            {
                APIController.TypeEndPoint = "GET";
                return APIController.SendMessageToAPI<List<DiasAgenda>>(null, "/api/calendario/FechasFestivas", false).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
                return null;
            }
        }

        public async Task<List<CXN_DIAS_WEB>> CargarListBlocked(int _codeProfesional) // HECHO
        {
            try
            {
                APIController.TypeEndPoint = "POST";

                var datos = new
                {
                    CodeProfesional = _codeProfesional
                };

                return APIController.SendMessageToAPI<List<CXN_DIAS_WEB>>(datos, "/api/calendario/CargarListBlocked", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        public async Task<int> ConsultarFecha(int _codeProfesional, DateTime fecha) // HECHO
        {
            try
            {
                APIController.TypeEndPoint = "POST";

                var datos = new
                {
                    CodeProfesional = _codeProfesional,
                    Fecha = fecha
                };

                return APIController.SendMessageToAPI<int>(datos, "/api/Agenda/ConsultarFecha", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return 0;
            }
        }

        public async Task<bool> Grabar(CXN_DIAS_WEB dias) // HECHO
        {
            try
            {
                APIController.TypeEndPoint = "PATCH";

                var datos = new
                {
                    F_Fecha = Convert.ToDateTime(dias.F_Fecha),
                    R_Razon = dias.R_Razon,
                    F_Prof = dias.F_Prof,
                    F_Bloquea = dias.F_Bloquea,
                    F_Estado = dias.F_Estado
                };

                return APIController.SendMessageToAPI<bool>(datos, "/api/calendario/Grabar", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        public async Task<bool> Desbloquear(int position, string user) // HECHO
        {
            try
            {
                APIController.TypeEndPoint = "PATCH";

                var datos = new
                {
                    Position = position,
                    User = user
                };

                return APIController.SendMessageToAPI<bool>(datos, "/api/calendario/Desbloquear", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }
    }
}
