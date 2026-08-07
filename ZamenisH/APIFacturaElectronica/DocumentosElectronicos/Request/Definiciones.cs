using Domain;
using Persistence;
using System;
using System.Data;
using System.Data.SqlClient;

namespace DocumentosElectronicos.Request
{
    public class Definiciones
    {
        public static string tipoDocRec(int Ase, string TDoc)
        {
            if (Ase == 99 || Ase == 88)
            {
                switch (TDoc)
                {
                    case "RC":
                        return "11"; 
                    case "TI":
                        return "12"; 
                    case "CC":
                        return "13"; 
                    case "CE":
                        return "21"; 
                    case "DE":
                        return "22";
                    case "NI":
                        return "31"; 
                    case "PA":
                        return "41"; 
                    case "PE":
                        return "47";
                    case "PT":
                        return "48"; 
                    case "NU":
                        return "91"; 

                    default:
                        return "13"; 
                }
            }
            else
            {
                return "31";
            }            
        }
        public static string tipoDocRec(string TDoc)
        {
            switch (TDoc)
            {
                case "Registro Civil":
                    return "11";
                case "Tarjeta de Identificacion":
                    return "12";
                case "Cedula de Ciudadania":
                    return "13";
                case "Cedula de Extranjeria":
                    return "21";
                case "Documento Extranjero":
                    return "22";
                case "Pasaporte":
                    return "41";
                case "Permiso especial de Permanencia ":
                    return "47";
                case "Proteccion Temporal":
                    return "48";
                case "NUIP":
                    return "91";

                default:
                    return "13";
            }
        }


        public static string tipocomprobante(string claseComprobante)
        {
            switch (claseComprobante)
            {
                //Factura de Venta Nacional  
                case "Factura de Venta Nacional":
                    return "01";
                //Factura de Exportación   
                case "Factura de Exportación":
                    return "02";
                //Documento electrónico de transmisión – tipo 03
                case "Documento electrónico de transmisión – tipo 03":
                    return "03";
                //Factura electrónica de Venta - tipo 04
                case "Factura electrónica de Venta - tipo 04":
                    return "04";
                //Documento Soporte Electrónico
                case "Documento Soporte Electrónico":
                    return "05";
                // Nota Crédito   
                case "Nota Crédito":
                    return "91";
                // Nota Débito  
                case "Nota Débito":
                    return "92";
                //Eventos (ApplicationResponse)
                case "Eventos (ApplicationResponse)":
                    return "96";

                default:
                    return "X";
            }
        }
        public static string modConPag(string claseModPago)
        {
            switch (claseModPago)
            {
                case "04 - Pago por evento":
                    return "04";
                case "01 - Pago individual por caso/Conjunto integral de atenciones / Paquete / Canasta":
                    return "01";
                case "02 - Pago global prospectivo":
                    return "02";
                case "03 - Pago por capitación":
                    return "03";
                case "05 - Otra modalidad (específica)":
                    return "05";

                default:
                    return "04";
            }
        }
        public static string cobPan(string clasecobPan)
        {
            switch (clasecobPan)
            {
                case "02 - Presupuesto máximo":
                    return "02";
                case "03 - Prima EPS / EOC, no asegurados SOAT":
                    return "03";
                case "04 - Cobertura Póliza SOAT":
                    return "04";
                case "05 - Cobertura ARL":
                    return "05";
                case "06 - Cobertura ADRES":
                    return "06";
                case "07 - Cobertura Salud Pública":
                    return "07";
                case "08 - Cobertura entidad territorial, recursos de oferta":
                    return "08";
                case "09 - Urgencias población migrante":
                    return "09";
                case "10 - Plan complementario en salud":
                    return "10";
                case "11 - Plan medicina prepagada":
                    return "11";
                case "12 - Pólizas en salud":
                    return "12";
                case "13 - Cobertura Régimen Especial o Excepción":
                    return "13";
                case "14 - Cobertura Fondo Nacional de Salud de las Personas Privadas de la Libertad":
                    return "14";
                case "15 - Particular":
                    return "15";
                case "16 - Plan de beneficios en Salud dinanciado con UPC contributivo":
                    return "16";
                case "17 - Plan de beneficios en Salud dinanciado con UPC subsidiado":
                    return "17";
                default:
                    return "16";
            }
        }
        public static string metodopago(string clasemetodopago)
        {
            switch (clasemetodopago)
            {
                case "Credito":
                    return "2";
                case "Contado":
                    return "1";
                default:
                    return "1";
            }
        }
        public static string medioPago(string clasemedioPago)
        {
            try
            {
                var getDataConection = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getDataConection["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    String Query = "SELECT * " +
                                   "FROM CXN_MEDIOSPAGO " +
                                   "WHERE Medio = @param1";

                    using (SqlCommand Commando = new SqlCommand(Query, con))
                    {
                        Commando.Parameters.AddWithValue("@param1", clasemedioPago);

                        using (SqlDataReader Reader = (Commando.ExecuteReader()))
                        {
                            if (Reader.Read() == true)
                            {
                                return Reader["Codigo"].ToString();
                            }
                            else
                            {
                                return "42";
                            }
                        }
                    }                  
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = "Clase Definiciones API FacElectron Metodo medioPago", Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return "42";
            }
        }        
    }
}
