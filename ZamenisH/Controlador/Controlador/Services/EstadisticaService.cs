using Domain;
using Domain.CXN;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Controlador.Services
{
    public class EstadisticaService
    {
        public async Task<List<ExportInExcel>> RptRecPaciente(DateTime desde, DateTime hasta, string tipoReporte) //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "POST";

                var datos = new
                {
                    Desde = Convert.ToDateTime(desde),
                    Hasta = Convert.ToDateTime(hasta),
                    Doc = tipoReporte
                };

                return APIController.SendMessageToAPI<List<ExportInExcel>>(datos, "/api/Gerencia/RptRecPaciente", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }
    }
}
