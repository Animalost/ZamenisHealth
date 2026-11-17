using Domain;
using Domain.CXN;
using Domain.Licence;

using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using ZXing;

namespace Persistence
{
    public class Conexion
    {
        private static readonly IConfSystem repoConf = new MConfSystem();
        public static string BloqueosAgenda { get; set; }
        public static string SMSAgenda { get; set; }
        public static string EmailAgenda { get; set; }
        public static string URL_CIE11 { get; set; }
        public static string URL_Version_CIE11 { get; set; }
        public static Dictionary<string, string> ConectionDictionary = new Dictionary<string, string>();
        public static string VersionApp = "5.3.0 R1";
        public static string EmailDestinoDefecto = "FacturacionInterna@caihcron.com";

        public static Dictionary<string, string> Conection()
        {
            return ConectionDictionary;
        }

        //CONEXION PRINCIPAL DEL SISTEMA DESDE WEB
        public static Dictionary<string, string> ConectionInitialWEB(ZhealthClass z)
        {
            try
            {
                ConectionDictionary.Add("Rol1", z.Roles.Recepcion.ToString());
                ConectionDictionary.Add("Rol2", z.Roles.Enfermeria.ToString());
                ConectionDictionary.Add("Rol3", z.Roles.Medicina_General.ToString());
                ConectionDictionary.Add("Rol4", z.Roles.Opciones.ToString());
                ConectionDictionary.Add("Rol5", z.Roles.Gerencial.ToString());
                ConectionDictionary.Add("Rol6", z.Roles.Administracion.ToString());
                ConectionDictionary.Add("Rol7", z.Roles.Fisiatria.ToString());
                ConectionDictionary.Add("Rol8", z.Roles.Psicologia.ToString());
                ConectionDictionary.Add("Rol9", z.Roles.TFisica.ToString());
                ConectionDictionary.Add("Rol10", z.Roles.TOcupacional.ToString());
                ConectionDictionary.Add("Rol11", z.Roles.Radiologia.ToString());

                ConectionDictionary.Add("smsuserdec", z.Recordatorios.User.ToString());
                ConectionDictionary.Add("smspassdec", z.Recordatorios.Pass.ToString());
                ConectionDictionary.Add("smsrestdec", z.Recordatorios.API.ToString());
                ConectionDictionary.Add("Format_Fecha", z.Generales.FormatoFecha.ToString());

                ConectionDictionary.Add("Conexion", z.Conexiones.ConexionPrincipal);
                ConectionDictionary.Add("ConexionAdjuntos", z.Conexiones.ConexionAdjuntos);

                ConectionDictionary.Add("Videoconferencia", z.OtrosPermisos.VideoConferencia.ToString());
                ConectionDictionary.Add("Recordatorios", z.OtrosPermisos.Recordatorios.ToString());
                ConectionDictionary.Add("Perplexity", z.OtrosPermisos.Perplexity.ToString());

                ConectionDictionary.Add("MasterKey", z.Generales.KeyAdmin.ToString());
                ConectionDictionary.Add("Tercero", z.Prestador.Tercero.ToString());
                ConectionDictionary.Add("Nit", z.Prestador.Nit.ToString());
                ConectionDictionary.Add("Telefono", z.Prestador.Telefono.ToString());
                ConectionDictionary.Add("Direccion", z.Prestador.Direccion.ToString());

                ConectionDictionary.Add("URLApi", z.Generales.UrlAPI.ToString());
                ConectionDictionary.Add("URLPagos", z.Pagos.URL.ToString());
                ConectionDictionary.Add("Vencimiento", z.Generales.Vencimiento.ToString());
                ConectionDictionary.Add("Mensaje", z.Generales.Mensaje.ToString());
                ConectionDictionary.Add("TextLicence", "Zamenis Health V.5.0.0 R15 Copyright 2018");
                

                return ConectionDictionary;
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = "Clase de Conexion Principal", Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return null;
            }
        }

        public static string getURLRestorePass()
        {
            List<CXN_IMAGEN_SYSTEM> d = repoConf.getConfSystem();
            if (d != null)
            {
                CXN_IMAGEN_SYSTEM d2 = d.FirstOrDefault(x => x.Tab_Nombre == "URLRestorePass");
                if (d2 != null)
                {
                    return d2.Tab_Clave;
                }
            }

            return "NO DISPONIBLE";
        }
        public static string getURLPrincipal()
        {
            List<CXN_IMAGEN_SYSTEM> d = repoConf.getConfSystem();
            if (d != null)
            {
                CXN_IMAGEN_SYSTEM d2 = d.FirstOrDefault(x => x.Tab_Nombre == "URLPrincipal");
                if (d2 != null)
                {
                    return d2.Tab_Clave;
                }
            }

            return "NO DISPONIBLE";
        }
        public static string getCadenaSQLServer(string Servidor, string Usuario, string Base, string Clave)
        {
            return "Data Source=" + Servidor + ";Initial Catalog=" + Usuario + ";User ID=" + Base + ";Password=" + Clave + ";MultipleActiveResultSets=true;Encrypt=True;TrustServerCertificate=True;";
        }
        public static Bitmap GenerateQRCode(string content)
        {
            BarcodeWriter barcodeWriter = new BarcodeWriter();
            barcodeWriter.Format = BarcodeFormat.QR_CODE;
            barcodeWriter.Options = new ZXing.Common.EncodingOptions
            {
                Width = 300,
                Height = 300
            };

            Bitmap bitmap = barcodeWriter.Write(content);
            //bitmap.Save(filePath);
            return bitmap;
        }
    }
}
