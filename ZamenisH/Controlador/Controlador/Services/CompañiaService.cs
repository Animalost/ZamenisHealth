using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using Domain.CXN;

namespace Controlador.Services
{
    public class CompañiaService
    {
        public async Task<List<string>> ObtenerListaPrestadores() // HECHO
        {
            try
            {
                APIController.TypeEndPoint = "GET";
                return APIController.SendMessageToAPI<List<string>>(null, "/api/compañias/ListaCompañias", false).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
                return new List<string>();
            }
        }

        public async Task<CXN_CIA> getPrestadorbyName(string NamePrestador) //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "POST";

                var datos = new
                {
                    CompanyName = NamePrestador
                };

                return APIController.SendMessageToAPI<CXN_CIA>(datos, "/api/compañias/getPrestadorbyName", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        public async Task<CXN_CIA> getPrestadorbyCode(int code) //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "POST";

                var datos = new
                {
                    CompanyCode = code
                };

                return APIController.SendMessageToAPI<CXN_CIA>(datos, "/api/Ventas/getPrestadorbyCode", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        public async Task<bool> ConsecutivoActualiza(int code, int nuecons, string tdoc) //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "PATCH";

                var datos = new
                {
                    CompanyCode = code,
                    NueCons = nuecons,
                    TDoc = tdoc
                };

                return APIController.SendMessageToAPI<bool>(datos, "/api/Ventas/ConsecutivoActualiza", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

    }
}
