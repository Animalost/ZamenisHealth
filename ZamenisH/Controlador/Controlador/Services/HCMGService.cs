using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections.Generic;

namespace Controlador.Services
{
    public  class HCMGService
    {
        public async Task<Dictionary<string, string>> SugerenciaServicio(string paciente) //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "POST";

                var datos = new
                {
                    Admision = paciente
                };

                return APIController.SendMessageToAPI<Dictionary<string, string>>(datos, "/api/Agendamiento/SugerenciaServicio", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }
    }
}
