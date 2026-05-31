using Domain.CXN;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Controlador.Services
{
    public class DisponibilidadService
    {
        public async Task<List<CXN_DISPONIBILIDAD_2>> HorariosHabilitados(int bodega, string dia) //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "POST";

                var datos = new
                {
                    Bodega = bodega,
                    Dia = dia
                };

                return APIController.SendMessageToAPI<List<CXN_DISPONIBILIDAD_2>>(datos, "/api/Disponibilidad/HorariosHabilitados", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        public async Task<int> HorariosOcupados(int bodega, DateTime fecha) //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "POST";

                var datos = new
                {
                    Bodega = bodega,
                    Fecha = fecha,
                };

                return APIController.SendMessageToAPI<int>(datos, "/api/Disponibilidad/HorariosOcupados", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return 0;
            }
        }

        public async Task<string> Festivos(DateTime fecha) //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "POST";

                var datos = new
                {
                    Fecha = Convert.ToDateTime(fecha),
                };

                return APIController.SendMessageToAPI<string>(datos, "/api/Disponibilidad/Festivos", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return "";
            }
        }      
    }
}
