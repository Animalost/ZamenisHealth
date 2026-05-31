using Domain.CXN;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Controlador.Services
{
    public  class EmailService
    {
        public async Task<List<CXN_EMAIL>> getAllEmails() //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "GET";
                return APIController.SendMessageToAPI<List<CXN_EMAIL>>(null, "/api/CitasSender/getAllEmails", false).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
                return null;
            }
        }
        public async Task<CXN_EMAIL> Datos_Mail(string email) //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "POST";

                var datos = new
                {
                    Email = email
                };

                return APIController.SendMessageToAPI<CXN_EMAIL>(datos, "/api/Email/Datos_Mail", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

    }
}
