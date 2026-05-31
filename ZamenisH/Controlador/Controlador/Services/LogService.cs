using System;
using System.Threading.Tasks;
using Domain.CXN;
using System.Windows.Forms;

namespace Controlador.Services
{
    public class LogService
    {
        public async Task<bool> Log(CXN_LOG_SENDER M) //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "POST";

                var datos = new
                {
                    Log_Tipo = M.Log_Tipo,
                    Log_Fecha_Envio = M.Log_Fecha_Envio,
                    Log_Estado = M.Log_Estado,
                    Log_Usuario = M.Log_Usuario,
                    Log_Mensaje = M.Log_Mensaje,
                    Log_Destinatario = M.Log_Destinatario,
                    Log_Admision = M.Log_Admision,
                    Id = 0
                };

                return APIController.SendMessageToAPI<bool>(datos, "/api/Logs/Log", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }
    }
}
