using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using Domain.CXN;
using System.Collections.Generic;

namespace Controlador.Services
{
    public class RcCajaService
    {
        public async Task<int> AnularRcCaja(int admision) //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "PATCH";

                var datos = new
                {
                    Admision = admision,
                };

                return APIController.SendMessageToAPI<int>(datos, "/api/Agenda/AnularRcCaja", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return 0;
            }
        }

        public async Task<List<RCCAJA>> GetRcCaja(int admision) //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "POST";

                var datos = new
                {
                    Admision = admision
                };

                return APIController.SendMessageToAPI<List<RCCAJA>>(datos, "/api/RcCaja/ReciboRpt", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        public async Task<bool> updateReciboHorario(int valor, int admision, string conRecaudo, string docFE) //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "PATCH";

                var datos = new
                {
                    Admision = admision,
                    Valor = valor,
                    ConRecaudo = conRecaudo,
                    DocFE = docFE
                };

                return APIController.SendMessageToAPI<bool>(datos, "/api/RcCaja/updateReciboHorario", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        public async Task<bool> AgregarRecibo(CXN_RC_CAJA R) //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "PATCH";

                var datos = new
                {
                    Rc_Caja_Pac = R.Rc_Caja_Pac,
                    Rc_Caja_Ase = R.Rc_Caja_Ase,
                    Rc_Caja_Cia = R.Rc_Caja_Cia,
                    Rc_Caja_Fecha = Convert.ToDateTime(R.Rc_Caja_Fecha),
                    Rc_Caja_UsrGraba = R.Rc_Caja_UsrGraba,
                    Rc_Caja_Valor = R.Rc_Caja_Valor,
                    Rc_Caja_Adm = R.Rc_Caja_Adm,
                    Rc_Caja_Observacion = R.Rc_Caja_Observacion
                };

                return APIController.SendMessageToAPI<bool>(datos, "/api/RcCaja/AgregarRecibo", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        public async Task<List<CXN_RC_CAJA>> RCCAJAS(string tid, string id) //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "POST";

                var datos = new
                {
                    TID = tid,
                    ID = id
                };

                return APIController.SendMessageToAPI<List<CXN_RC_CAJA>>(datos, "/api/RcCaja/RCCAJAS", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }
    }
}
