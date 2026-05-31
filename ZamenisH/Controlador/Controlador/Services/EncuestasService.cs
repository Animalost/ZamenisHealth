using Domain.CXN;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Controlador.Services
{
    public class EncuestasService
    {
        public async Task<bool> getCantEncuestaCU(string service, string mes, int año) //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "POST";

                var datos = new
                {
                    Service = service,
                    Month = mes,
                    Year = año
                };

                return APIController.SendMessageToAPI<bool>(datos, "/api/Email/getCantEncuestaCU", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        public async Task<bool> getCantEncuestasPaciente(int Paciente, CXN_CONFENCUESTA C) //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "POST";

                var datos = new
                {
                    Service = C.Servicio,
                    Month = C.Mes,
                    Year = C.Año,
                    IdPac = Paciente
                };

                return APIController.SendMessageToAPI<bool>(datos, "/api/Email/getCantEncuestasPaciente", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }
    }
}
