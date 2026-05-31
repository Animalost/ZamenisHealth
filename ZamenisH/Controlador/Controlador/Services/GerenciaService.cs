using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Controlador.Services
{
    public class GerenciaService
    {
        public async Task<string> Individual(DateTime desde, DateTime hasta, string doc) //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "POST";

                var datos = new
                {
                    Desde = Convert.ToDateTime(desde),
                    Hasta = Convert.ToDateTime(hasta),
                    Doc = doc
                };

                return APIController.SendMessageToAPI<string>(datos, "/api/Gerencia/Individual", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        public async Task<string> Total(DateTime desde, DateTime hasta) //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "POST";

                var datos = new
                {
                    Desde = Convert.ToDateTime(desde),
                    Hasta = Convert.ToDateTime(hasta),
                };

                return APIController.SendMessageToAPI<string>(datos, "/api/Gerencia/Total", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }
    }
}
