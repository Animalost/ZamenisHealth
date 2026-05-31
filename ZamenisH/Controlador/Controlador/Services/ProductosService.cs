using Domain.CXN;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Controlador.Services
{
    public class ProductosService
    {
        public async Task<CXN_INVENTARIO> getProductbyCode(string code) //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "POST";

                var datos = new
                {
                    Codigo = code
                };

                return APIController.SendMessageToAPI<CXN_INVENTARIO>(datos, "/api/Ventas/getProductbyCode", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }
        public async Task<CXN_INVENTARIO> ConsultarValor(string code, int ase) //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "POST";

                var datos = new
                {
                    Codigo = code,
                    Ase = ase
                };

                return APIController.SendMessageToAPI<CXN_INVENTARIO>(datos, "/api/Particulares/ConsultarValor", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }
        public async Task<List<CXN_INVENTARIO>> getAllElements(string code, int ase) //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "POST";

                var datos = new
                {
                    Ase = ase,
                    Codigo = code
                };

                return APIController.SendMessageToAPI<List<CXN_INVENTARIO>>(datos, "/api/Productos/getAllElements", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }
        public async Task<List<CXN_INVENTARIO>> getProductbyName(string name) //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "POST";

                var datos = new
                {
                    Codigo = name
                };

                return APIController.SendMessageToAPI<List<CXN_INVENTARIO>>(datos, "/api/Productos/getProductbyName", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

    }
}
