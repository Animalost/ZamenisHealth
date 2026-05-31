using Domain.CXN;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Controlador.Services
{
    public class OMService
    {
        public async Task<bool> consumirAutorizacion(int Paciente, string Autorizacion, string Estado) //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "PATCH";

                var datos = new
                {
                    Admision = Paciente,
                    InicioSesion = Estado,
                    Tipo = Autorizacion
                };

                return APIController.SendMessageToAPI<bool>(datos, "/api/Admisiones/consumirAutorizacion", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        public async Task<bool> OPendUpdate(CXN_OPEND O) //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "PATCH";

                var datos = new
                {
                    Estado = O.OP_Estado,
                    Usrcambia = O.OP_Cambia,
                    EstadoCambia = Convert.ToDateTime(O.OP_EstadoChange),
                    Admision = O.OP_Adm
                };

                return APIController.SendMessageToAPI<bool>(datos, "/api/DatosCitas/Calcular3", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        public async Task<List<CXN_OM>> getOrdenes(string tid, string nid, int cia) //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "POST";

                var datos = new
                {
                    Cia = cia,
                    TID = tid,
                    NID = nid
                };

                return APIController.SendMessageToAPI<List<CXN_OM>>(datos, "/api/OMRec/getOrdenes", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        public async Task<List<CXN_HORARIO>> getListPendientes() // HECHO
        {
            try
            {
                APIController.TypeEndPoint = "GET";
                return APIController.SendMessageToAPI<List<CXN_HORARIO>>(null, "/api/OMRec/getListPendientes", false).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
                return new List<CXN_HORARIO>();
            }
        }
    }
}
