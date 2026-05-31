using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Persistence.CXN.Interfaces;
using System.Drawing.Imaging;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Office.Interop.Excel;
using Domain;
using Domain.CXN;

namespace Persistence.CXN.Metodos
{
    public class MGenerales : IGenerales
    {
        Dictionary<string, string> IGenerales.SugerenciaServicio(int Paciente)
        {
            try
            {
                var dataConection = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(dataConection["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }
                    String Query = "SELECT TOP 1 HC_ServCatalogo, HC_CupCatalogo " +
                                    "FROM CXN_HCMG " +
                                    "WHERE HC_PacId = '" + Paciente + "' " +
                                    "ORDER BY HC_Fecha DESC";
                    SqlCommand Commando = new SqlCommand(Query, con);
                    SqlDataReader Reader = (Commando.ExecuteReader());
                    if (Reader.Read() == true)
                    {
                        Dictionary<string, string> DIC = new Dictionary<string, string>();
                        DIC.Add("Servicio", Reader["HC_ServCatalogo"].ToString());
                        DIC.Add("Cup", Reader["HC_CupCatalogo"].ToString());
                        return DIC;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        List<CXN_DISCAPACIDAD> IGenerales.ListaDiscapacidades()
        {
            try
            {
                var dataConection = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(dataConection["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }
                    String Query = "SELECT * " +
                                   "FROM CXN_DISCAPACIDAD";

                    using (SqlCommand Commando = new SqlCommand(Query, con))
                    {
                        using (SqlDataReader Reader = (Commando.ExecuteReader()))
                        {
                            if (Reader.HasRows)
                            {
                                List<CXN_DISCAPACIDAD> s = new List<CXN_DISCAPACIDAD>();

                                while (Reader.Read())
                                {
                                    CXN_DISCAPACIDAD Dato = new CXN_DISCAPACIDAD
                                    {
                                        Id = Convert.ToInt32(Reader["Id"]),
                                        Codigo = Reader["Codigo"].ToString(),
                                        Discapacidad = Reader["Discapacidad"].ToString()
                                    };
                                    s.Add(Dato);
                                }

                                return s;
                            }
                            else
                            {
                                return null;
                            }
                        }
                    }                                            
                }
            }
            catch
            {
                return null;
            }
        }
        string IGenerales.GetCodeDiscapacidad(string NameDiscapacidad)
        {
            try
            {
                var dataConection = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(dataConection["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }
                    String Query = "SELECT * " +
                                   "FROM CXN_DISCAPACIDAD " +
                                   "WHERE Discapacidad = @param1";

                    using (SqlCommand Commando = new SqlCommand(Query, con))
                    {
                        Commando.Parameters.AddWithValue("@param1", NameDiscapacidad);

                        using (SqlDataReader Reader = (Commando.ExecuteReader()))
                        {
                            if (Reader.Read() == true)
                            {
                                return Reader["Codigo"].ToString();
                            }
                            else
                            {
                                return "08";
                            }
                        }
                    }
                }
            }
            catch
            {
                return "08";
            }
        }
        List<CXN_ETNIA> IGenerales.ListaEtnias()
        {
            try
            {
                var dataConection = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(dataConection["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }
                    String Query = "SELECT * " +
                                   "FROM CXN_ETNIA";

                    using (SqlCommand Commando = new SqlCommand(Query, con))
                    {
                        using (SqlDataReader Reader = (Commando.ExecuteReader()))
                        {
                            if (Reader.HasRows)
                            {
                                List<CXN_ETNIA> s = new List<CXN_ETNIA>();

                                while (Reader.Read())
                                {
                                    CXN_ETNIA Dato = new CXN_ETNIA
                                    {
                                        Id = Convert.ToInt32(Reader["Id"]),
                                        Codigo = Reader["Codigo"].ToString(),
                                        Etnia = Reader["Etnia"].ToString()
                                    };
                                    s.Add(Dato);
                                }

                                return s;
                            }
                            else
                            {
                                return null;
                            }
                        }
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        string IGenerales.GetCodeEtnia(string NameEtnia)
        {
            try
            {
                var dataConection = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(dataConection["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }
                    String Query = "SELECT * " +
                                   "FROM CXN_ETNIA " +
                                   "WHERE Etnia = @param1";

                    using (SqlCommand Commando = new SqlCommand(Query, con))
                    {
                        Commando.Parameters.AddWithValue("@param1", NameEtnia);

                        using (SqlDataReader Reader = (Commando.ExecuteReader()))
                        {
                            if (Reader.Read() == true)
                            {
                                return Reader["Codigo"].ToString();
                            }
                            else
                            {
                                return "6";
                            }
                        }
                    }
                }
            }
            catch
            {
                return "6";
            }
        }

        /*int IGenerales.getCodeRegimen(string Regimen)
        {
            switch (Regimen)
            {
                case "Contributivo":
                    return 1;
                case "Subsidiado":
                    return 2;
                case "N/A":
                    return 0;
                case "Vinculado":
                    return 3;
                case "Particular":
                    return 4;
                case "Otro":
                    return 5;
                case "Victima Afiliada Contributivo":
                    return 6;
                case "Victima Afiliada Subsidiada":
                    return 7;
                case "Victima No Asegurada":
                    return 8;
                default:
                    return 0;
            }
        }*/

        System.Drawing.Image IGenerales.CodifyQR(string T_Codifica)
        {
            Bitmap mapTemp = Conexion.GenerateQRCode(T_Codifica);
            Image image = (Image)mapTemp;
            return image;
        }

        bool IGenerales.ValidarNumerico(string Dato)
        {
            if (Regex.IsMatch(Dato, @"^[0-9]+$"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        string IGenerales.GetMD5(string str)
        {
            MD5 md5 = MD5CryptoServiceProvider.Create();
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] stream = null;
            StringBuilder sb = new StringBuilder();
            stream = md5.ComputeHash(encoding.GetBytes(str));
            for (int i = 0; i < stream.Length; i++) sb.AppendFormat("{0:x2}", stream[i]);
            return sb.ToString();
        }

        byte[] IGenerales.GetBytes(System.Drawing.Image imageIn)
        {
            MemoryStream ms = new MemoryStream();
            imageIn.Save(ms, ImageFormat.Png);
            return ms.ToArray();
        }

        string IGenerales.Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        string IGenerales.Base64Encode(string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        Bitmap IGenerales.ByteToImage(byte[] blob)
        {
            MemoryStream mStream = new MemoryStream();
            byte[] pData = blob;
            mStream.Write(pData, 0, Convert.ToInt32(pData.Length));
            Bitmap bm = new Bitmap(mStream, false);
            mStream.Dispose();
            return bm;
        }

        void IGenerales.ExportarGrilla(DataGridView dataGridX)
        {
            try
            {
                var timer = System.Diagnostics.Stopwatch.StartNew();

                Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();
                excel.Application.Workbooks.Add(true);

                object[,] datos = new object[dataGridX.Rows.Count + 1, dataGridX.Columns.Count]; // +1 por la cabecera
                for (int j = 0; j < dataGridX.Columns.Count; j++) //cabeceras
                {
                    datos[0, j] = dataGridX.Columns[j].Name;
                }

                for (int i = 0; i < dataGridX.Rows.Count; i++)
                {
                    for (int j = 0; j < dataGridX.Columns.Count; j++)
                    {
                        datos[i + 1, j] = dataGridX.Rows[i].Cells[j].Value;
                    }
                }

                excel.Range[excel.Cells[1, 1], excel.Cells[datos.GetLength(0), datos.GetLength(1)]].Value = datos;
                excel.Visible = true;
                Worksheet worksheet = (Worksheet)excel.ActiveSheet;
                worksheet.Activate();
                timer.Stop();
                var elapsed = timer.Elapsed;
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
            }
        }           
    }
}
