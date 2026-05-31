using Domain.CXN;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Controlador.Services
{
    public class ConfSystemService
    {
        public async Task<Dictionary<string, string>> getListado() //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "GET";
                return APIController.SendMessageToAPI<Dictionary<string, string>>(null, "/api/configuracion/GetConfSystem", false).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> updateImagenSystem(CXN_IMAGEN_SYSTEM I) //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "PATCH";

                var datos = new
                {
                    Clave = I.Tab_Clave,
                    Nombre = I.Tab_Nombre
                };

                return APIController.SendMessageToAPI<bool>(datos, "/api/configuracion/updateImagenSystem", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

    }
}
