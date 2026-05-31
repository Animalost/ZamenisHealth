using Domain.CXN;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using Controlador.Clases;

namespace Controlador.Services
{
    public class AseguradoraService
    {
        public async Task<List<CXN_ASEGURADORA>> GetAseguradoras() //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "GET";
                return APIController.SendMessageToAPI<List<CXN_ASEGURADORA>>(null, "/api/Agendamiento/GetAseguradoras", false).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
                return null;
            }
        }

        public async Task<CXN_ASEGURADORA> GetInfoFromAsebyCode(int codeaseguradora) //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "POST";

                var datos = new
                {
                    IdOrNameAseguradora = codeaseguradora
                };

                return APIController.SendMessageToAPI<CXN_ASEGURADORA>(datos, "/api/Agendamiento/GetInfoFromAsebyCode", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        public async Task<CXN_ASEGURADORA> GetInfoFromAsebyName(string nameAse) //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "POST";

                var datos = new
                {
                    IdOrNameAseguradora = nameAse
                };

                return APIController.SendMessageToAPI<CXN_ASEGURADORA>(datos, "/api/Agendamiento/getInfoFromAsebyName", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        public async Task<List<string>> CargarAseguradorasXServ(string tserv) //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "POST";

                var datos = new
                {
                    TServ = tserv
                };

                return APIController.SendMessageToAPI<List<string>>(datos, "/api/Admisiones/CargarAseguradorasXServ", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }
    }
}
