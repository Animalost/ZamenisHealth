using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Controlador.Services
{
    public class PlanosService
    {
        public async Task<string> ExpPlanoOrdenesPendientes(DateTime desde, DateTime hasta) //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "POST";

                var datos = new
                {
                    Desde = Convert.ToDateTime(desde),
                    Hasta = Convert.ToDateTime(hasta)
                };

                return APIController.SendMessageToAPI<string>(datos, "/api/OMRec/ExpPlanoOrdenesPendientes", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }
    }
}
