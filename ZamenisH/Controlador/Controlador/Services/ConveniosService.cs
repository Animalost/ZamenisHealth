using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using Domain.CXN;
using System.Collections.Generic;

namespace Controlador.Services
{
    public class ConveniosService
    {
        public async Task<string> NameServiceCUP(string nameservicio) // HECHO
        {
            try
            {
                APIController.TypeEndPoint = "POST";

                var datos = new
                {
                    NameService = nameservicio,
                };

                return APIController.SendMessageToAPI<string>(datos, "/api/Convenios/GetServiceName", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        public async Task<List<string>> CargarServicios(string tbodega, int nbodega) //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "POST";

                var datos = new
                {
                    TipoBodega = tbodega,
                    NumBodega = nbodega,
                };

                return APIController.SendMessageToAPI<List<string>>(datos, "/api/Convenios/CargarServicios", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        public async Task<CXN_CONVENIOS> ServicioCUP(int Ase, string Service) //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "POST";

                var datos = new
                {
                    NameService = Service,
                    Aseguradora = Ase
                };

                return APIController.SendMessageToAPI<CXN_CONVENIOS>(datos, "/api/Convenios/ServicioCUP", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        public async Task<CXN_CONVENIOS> ServicioNombre(string cup, int ase, string serv) //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "POST";

                var datos = new
                {
                    Cup = cup,
                    Ase = ase,
                    Serv = serv
                };

                return APIController.SendMessageToAPI<CXN_CONVENIOS>(datos, "/api/Convenios/ServicioNombre", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        public async Task<List<string>> CargarServiciosxASE(int ase) //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "POST";

                var datos = new
                {
                    Ase = ase
                };

                return APIController.SendMessageToAPI<List<string>>(datos, "/api/Particulares/CargarServiciosxASE", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }
    }
}
