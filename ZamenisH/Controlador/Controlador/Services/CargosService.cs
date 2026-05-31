using Domain;
using Domain.CXN;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Controlador.Services
{
    public  class CargosService
    {
        public async Task<ListaCarga> Carga_Plantilla() //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "GET";
                return APIController.SendMessageToAPI<ListaCarga>(null, "/api/Particulares/Carga_Plantilla", false).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
                return null;
            }
        }

        public async Task<List<CXN_CARGOS>> cotizacionesPrevias(string tid, string id) //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "POST";

                var datos = new
                {
                    TID = tid,
                    ID = id
                };

                return APIController.SendMessageToAPI<List<CXN_CARGOS>>(datos, "/api/Particulares/cotizacionesPrevias", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }
    }
}
