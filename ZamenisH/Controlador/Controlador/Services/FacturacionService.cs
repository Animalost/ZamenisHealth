using Domain.CXN;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Controlador.Services
{
    public class FacturacionService
    {
        public async Task<string> insertarDocumento(CXN_FACTURA C) //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "PATCH";

                var datos = new
                {
                    Fac_Tipo_Doc = C.Fac_Tipo_Doc,
                    Fac_Num_Fac = Convert.ToInt32(C.Fac_Num_Fac),
                    Fac_Cia = C.Fac_Cia,
                    Fac_Pac = C.Fac_Pac,
                    Fac_Res = C.Fac_Res,
                    Homologo = C.Homologo,
                    Fac_Ase = C.Fac_Ase,
                    Fac_Observa = C.Fac_Observa,
                    Fac_Usr_Graba = C.Fac_Usr_Graba,
                    Fac_Estado = C.Fac_Estado
                };

                return APIController.SendMessageToAPI<string>(datos, "/api/Particulares/insertarDocumento", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        public async Task<List<CXN_FACTURA>> Filter(string tipo, string tID, string iD, int cia) //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "POST";

                var datos = new
                {
                    Tipo = tipo,
                    Tid = tID,
                    Id = iD,
                    Cia = cia
                };

                return APIController.SendMessageToAPI<List<CXN_FACTURA>>(datos, "/api/Gerencia/Filter", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }
    }
}
