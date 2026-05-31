using Domain.CXN;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Controlador.Services
{
    public class ParticularesService
    {
        public async Task<string> insertService(CXN_CARGOS C) //HECHO
        {
            try
            {
                APIController.TypeEndPoint = "PATCH";

                var datos = new
                {
                    Car_Cant = C.Car_Cant,
                    Car_Val_Tot = C.Car_Val_Tot,
                    Car_Pac = C.Car_Pac,
                    Car_Cia = C.Car_Cia,
                    Car_Ase = C.Car_Ase,
                    Car_Cod = C.Car_Cod,
                    Car_Item = C.Car_Item,
                    Car_Factura = C.Car_Factura,
                    Car_Estado = C.Car_Estado,
                    Car_Tipo = C.Car_Tipo,
                    Car_Tipo_Doc = C.Car_Tipo_Doc,
                    Car_Usr_Graba = C.Car_Usr_Graba

                    //Car_Fecha = Convert.ToDateTime(C.Car_Fecha),
                    //Car_Val_Un = C.Car_Val_Un,
                    //Car_Adm_Id = C.Car_Pac
                };

                return APIController.SendMessageToAPI<string>(datos, "/api/Particulares/insertService", true).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }
    }
}
