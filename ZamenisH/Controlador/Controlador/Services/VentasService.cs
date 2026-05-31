using Domain.CXN;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Controlador.Services
{
    public class VentasService
    {
        public async Task<bool> insertarVenta(CXN_VENTAS P) //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "PATCH";

                var datos = new
                {
                    Ven_Cod = P.Ven_Cod,
                    Ven_Cantidad = P.Ven_Cantidad,
                    Ven_Precio = P.Ven_Precio,
                    Ven_Total = P.Ven_Total,
                    Ven_Cod_Pac = P.Ven_Cod_Pac,
                    Ven_Cod_Cia = P.Ven_Cod_Cia,
                    Ven_Estado = P.Ven_Estado,
                    Ven_Factura = P.Ven_Factura,
                    Ven_Usr_Graba = P.Ven_Usr_Graba,
                    Ven_Item = P.Ven_Item,
                    Ven_Res = P.Ven_Res,
                    Ven_Dcto = P.Ven_Dcto,
                    Ven_Fecha = Convert.ToDateTime(P.Ven_Fecha),
                    Ven_Tipo_Doc = P.Ven_Tipo_Doc
                };

                return APIController.SendMessageToAPI<bool>(datos, "/api/Ventas/insertarVenta", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        public async Task<List<CXN_VENTAS>> getPrevios(int idpac) //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "POST";

                var datos = new
                {
                    CompanyCode = idpac
                };

                return APIController.SendMessageToAPI<List<CXN_VENTAS>>(datos, "/api/Ventas/getPrevios", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

       
    }
}
