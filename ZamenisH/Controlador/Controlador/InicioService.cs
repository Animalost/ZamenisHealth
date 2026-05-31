using Domain.Licence;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Controlador
{
    public class InicioService
    {
        public async Task<ZHealth> GetLicence(string serial, string UrlBase) //HECHO
        {
            try
            {
                return APIController.GetLicence(serial, "/api/Licence/GetLicence", UrlBase).GetAwaiter().GetResult(); 
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }
    }
}
