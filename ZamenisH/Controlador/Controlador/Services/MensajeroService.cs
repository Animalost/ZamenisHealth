using Domain.CXN;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Controlador.Services
{
    public class MensajeroService
    {
        public async Task<CXN_MESSENGER> GetMessages(string User) //HEHCO
        {
            try
            {
                APIController.TypeEndPoint = "POST";

                var datos = new
                {
                    Men_Usuario_Para = User.ToUpper().Trim()
                };

                return APIController.SendMessageToAPI<CXN_MESSENGER>(datos, "/api/mensajero/GetMessages", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        public async Task<int> Leido(int Idd) //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "PATCH";

                var datos = new
                {
                    Men_Usuario_Para = "",
                    Id = Idd
                };

                return APIController.SendMessageToAPI<int>(datos, "/api/mensajero/Leido", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return 0;
            }
        }

        public async Task<string> ObtenerUsuario(string NameUser) //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "POST";

                var datos = new
                {
                    Men_Usuario_Para = NameUser,
                    Id = 0
                };

                return APIController.SendMessageToAPI<string>(datos, "/api/mensajero/ObtenerUsuario", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return "";
            }
        }

        public async Task<List<CXN_MESSENGER>> ObtenerListaMensajes(string _anotherUser, string meUser, DateTime Fecha) //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "POST";

                var datos = new
                {
                    _anotherUser = _anotherUser,
                    meUser = meUser,
                    Fecha = Fecha
                };

                return APIController.SendMessageToAPI<List<CXN_MESSENGER>>(datos, "/api/mensajero/ObtenerListaMensajes", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        public async Task<bool> SendMessage(CXN_MESSENGER messenger) //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "PATCH";

                var datos = new
                {
                    Estado = messenger.Estado,
                    Men_Mensaje = messenger.Men_Mensaje,
                    Men_Usuario_Para = messenger.Men_Usuario_Para,
                    Men_Usuario_De = messenger.Men_Usuario_De
                };

                return APIController.SendMessageToAPI<bool>(datos, "/api/mensajero/SendMessage", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        public async Task<byte[]> getAvatar(string men_Usuario) //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "POST";

                var datos = new
                {
                    Men_Usuario = men_Usuario
                };

                return APIController.SendMessageToAPI<byte[]>(datos, "/api/mensajero/getAvatar", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }




    }
}
