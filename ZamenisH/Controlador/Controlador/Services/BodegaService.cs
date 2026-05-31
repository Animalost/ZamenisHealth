using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Windows.Forms;
using Domain.CXN;

namespace Controlador.Services
{
    public class BodegaService
    {
        public async Task<List<string>> ObtenerListaProfesionales(string Tipo) // HECHO
        {
            try
            {
                APIController.TypeEndPoint = "POST";

                var datos = new
                {
                    TipoBodega = Tipo
                };

                return APIController.SendMessageToAPI<List<string>>(datos, "/api/bodegas/ListaBodegas", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        public async Task<CXN_BODEGAS> getDataBodega(string NameBodega) //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "POST";

                var datos = new
                {
                    NombreBodega = NameBodega
                };

                return APIController.SendMessageToAPI<CXN_BODEGAS>(datos, "/api/bodegas/getDataBodega", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        public async Task<CXN_BODEGAS> GetDataBodegaCode(int codeBodega) //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "POST";

                var datos = new
                {
                    CodeBodega = codeBodega
                };

                return APIController.SendMessageToAPI<CXN_BODEGAS>(datos, "/api/Agendamiento/GetDataBodegaCode", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        public async Task<List<CXN_BODEGAS>> Filtrar() // HECHO
        {
            try
            {
                APIController.TypeEndPoint = "GET";
                return APIController.SendMessageToAPI<List<CXN_BODEGAS>>(null, "/api/Recepcion/Filtrar", false).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
                return new List<CXN_BODEGAS>();
            }
        }

        public async Task<bool> ActivarDesactivarBodega(int Bode, string Estado) //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "POST";

                var datos = new
                {
                    CodeBodega = Bode,
                    Estado = Estado
                };

                return APIController.SendMessageToAPI<bool>(datos, "/api/Recepcion/ActivarDesactivarBodega", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        public async Task<DataTable> Profesionales2(string tprofesional) //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "POST";

                var datos = new
                {
                    NombreBodega = tprofesional
                };

                return APIController.SendMessageToAPI<DataTable>(datos, "/api/bodegas/Profesionales2", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }
    }
}
