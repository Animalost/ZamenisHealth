using Domain.CXN;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Controlador.Services
{
    public class AsistenciaService
    {
        public async Task<List<CXN_HORARIO>> Asistencia(string idNum) //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "POST";

                var datos = new
                {
                    IdNum = idNum
                };

                return APIController.SendMessageToAPI<List<CXN_HORARIO>>(datos, "/api/Asistencia/SeeAsistence", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        public async Task<List<CXN_HORARIO>> AsistenciaLastAut(string idNum) //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "POST";

                var datos = new
                {
                    IdNum = idNum
                };

                return APIController.SendMessageToAPI<List<CXN_HORARIO>>(datos, "/api/Asistencia/AsistenciaLastAut", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        public async Task<List<CXN_CIA>> certificadoAsistencia(int admision, string texto) //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "POST";

                var datos = new
                {
                    Admision = admision,
                    Texto = texto
                };

                return APIController.SendMessageToAPI<List<CXN_CIA>>(datos, "/api/CAsistencia/certificadoAsistencia", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }
    }
}
