using Domain.CXN;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Controlador.Services
{
    public class ZonasService
    {
        public async Task<string> DepartamentoNombre(string zonCode) //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "POST";

                var datos = new
                {
                    ZonCode = zonCode
                };

                return APIController.SendMessageToAPI<string>(datos, "/api/Admisiones/DepartamentoNombre", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return "";
            }
        }

        public async Task<string> DepartamentoCodigo(string NomDep) //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "POST";

                var datos = new
                {
                    ZonCode = NomDep
                };

                return APIController.SendMessageToAPI<string>(datos, "/api/Zonas/DepartamentoCodigo", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return "";
            }
        }

        public async Task<string> MunicipioNombre(string CodMun, string DepCod) //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "POST";

                var datos = new
                {
                    DepCode = DepCod,
                    MunCode = CodMun
                };

                return APIController.SendMessageToAPI<string>(datos, "/api/Admisiones/MunicipioNombre", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return "";
            }
        }

        public async Task<string> MunicipioCodigo(string NomMun, string NomDep) //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "POST";

                var datos = new
                {
                    MunCode = NomMun,
                    DepCode = NomDep
                };

                return APIController.SendMessageToAPI<string>(datos, "/api/Zonas/MunicipioCodigo", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return "";
            }
        }

        public async Task<List<string>> getListPaises() //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "GET";
                return APIController.SendMessageToAPI<List<string>>(null, "/api/Admisiones/getListPaises", false).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
                return null;
            }
        }
    
        public async Task<string> getNamePais(string namePais) //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "POST";

                var datos = new
                {
                    ZonName = namePais
                };

                return APIController.SendMessageToAPI<string>(datos, "/api/Admisiones/getNamePais", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return "";
            }
        }
   
        public async Task<string> getCodePais(string code) //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "POST";

                var datos = new
                {
                    Filtro = code,
                    Admision = 0
                };

                return APIController.SendMessageToAPI<string>(datos, "/api/Admisiones/getCodePais", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return "";
            }
        }

        public async Task<List<CXN_ZONAS>> _listadoCodigos(string code, string mun, string dep) //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "POST";

                var datos = new
                {
                    Filtro = code,
                    MunCode = mun,
                    DepCode = dep
                };

                return APIController.SendMessageToAPI<List<CXN_ZONAS>>(datos, "/api/Zonas/_listadoCodigos", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }
    }
}
