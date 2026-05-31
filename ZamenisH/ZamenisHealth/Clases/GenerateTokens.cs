using DocumentosElectronicos.Servicio;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ZamenisHealth.Clases
{
    public class GenerateTokens
    {
        private static readonly GenerarToken webService = new GenerarToken();
        private static readonly IHelisa webHelisa = new MHelisa();
        private static readonly IFacElectron webElectron = new MFacElectron();

        public static bool GenerarTokenNuevo(int Cia)
        {
            try
            {
                Dictionary<string, string> dataWS = webHelisa.Claves("Factura1Token", Cia);
                if (dataWS == null)
                {
                    MessageBox.Show("No se encontraron las claves de acceso para el prestador especificado.");
                    return false;
                }
                else
                {
                    var resAPIToken = webService.GetToken(dataWS["User"], dataWS["Pass"], Cia, Program.URLApiConexion).GetAwaiter().GetResult();
                    if (resAPIToken.StatusCode == "OK")
                    {
                        if (webElectron.insertToken(resAPIToken.xml, Cia) == "OK")
                        {
                            //MessageBox.Show("Token generado exitosamente", "Generado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return true;
                        }
                        else
                        {
                            MessageBox.Show("No se pudo grabar el token en la base de datos.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return false;
                        }
                    }
                    else
                    {
                        MessageBox.Show("No se logro generar el token");
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }
    }
}
