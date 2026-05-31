using Domain.CXN;
using Domain;
using Persistence.CXN.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Windows.Forms;
using System.Drawing;

namespace Persistence.CXN.Metodos
{
    public class MFormatos : IFormatos
    {
        private readonly static IGenerales repoGen = new MGenerales();

        List<string> IFormatos.getFormatos()
        {
            try
            {
                Dictionary<string,string> getData = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getData["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    String Cargar_Hora = "SELECT Nombre FROM CXN_FORMATOS order by Nombre ASC";
                    SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con);
                    SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader());
                    if (Lectura_Hora.HasRows)
                    {
                        List<string> C = new List<string>();

                        while (Lectura_Hora.Read() == true)
                        {
                            C.Add(Lectura_Hora["Nombre"].ToString());
                        }

                        return C;
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

        CXN_FORMATOS IFormatos.getFormatoSelected(string Nombre)
        {
            try
            {
                Dictionary<string,string> getData = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getData["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    String Cargar_Hora = "SELECT * FROM CXN_FORMATOS WHERE Nombre = '" + Nombre + "'";
                    SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con);
                    SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader());
                    if (Lectura_Hora.Read() == true)
                    {
                        CXN_FORMATOS C = new CXN_FORMATOS
                        {
                            CiudadFecha = Lectura_Hora["CiudadFecha"].ToString(),
                            Cuerpo = Lectura_Hora["Cuerpo"].ToString(),
                            Firma = Lectura_Hora["Firma"].ToString(),
                            Firma2 = Lectura_Hora["Firma2"].ToString(),
                            Pie = Lectura_Hora["Pie"].ToString()
                        };

                        return C;
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

        bool IFormatos.insertarFormato(CXN_FORMATOS F)
        {
            try
            {
                Dictionary<string,string> getData = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getData["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    SqlCommand cmd = new SqlCommand(@"Insert into CXN_FORMATOS_2 (paciente, " + //param1
                                                         "CiudadFecha, " + //param2
                                                         "compañia, " + //param3
                                                         "Cuerpo, " + //param4
                                                         "Pie, " + //param5
                                                         "Firmas, " + //param6
                                                         "Firmas2, " + //param7
                                                         "FECHA, " + //param8
                                                         "Usuario) " + //param16
                                "values                  (@param1, " + // Hor_Estado
                                                         "@param2, " + // Hor_Pac_Id
                                                         "@param3, " + // Hor_Pac_Bod
                                                         "@param4, " + // Hor_Pac_Tipo_Serv
                                                         "@param5, " + // Hor_Pac_Cia
                                                         "@param6, " + // Hor_Pac_Ase
                                                         "@param7, " + // Hor_Pac_Cup
                                                         "@param8, " + // Hor_Pac_UsrGraba
                                                         "@param9)", con); // Hor_Pac_Sal

                    cmd.Parameters.AddWithValue("@param1", F.Paciente);
                    cmd.Parameters.AddWithValue("@param2", F.CiudadFecha);
                    cmd.Parameters.AddWithValue("@param3", F.Compañia);
                    cmd.Parameters.AddWithValue("@param4", F.Cuerpo);
                    cmd.Parameters.AddWithValue("@param5", F.Pie);
                    cmd.Parameters.AddWithValue("@param6", F.Firma);
                    cmd.Parameters.AddWithValue("@param7", F.Firma2);
                    cmd.Parameters.Add(new SqlParameter("@param8", SqlDbType.DateTime)).Value = F.Fecha; // Fecha que graba cita
                    cmd.Parameters.AddWithValue("@param9", F.Usuario);
                    cmd.ExecuteNonQuery();

                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        bool IFormatos.updateFormato(CXN_FORMATOS F)
        {
            try
            {
                Dictionary<string,string> getData = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getData["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    string Busqueda = "UPDATE CXN_FORMATOS " +
                                      "SET Cuerpo = '" + F.Cuerpo + "', " +
                                      "Firma = '" + F.Firma + "', " +
                                      "Firma2 = '" + F.Firma2 + "', " +
                                      "Pie = '" + F.Pie + "', " +
                                      "CiudadFecha = '" + F.CiudadFecha + "', " +
                                      "FECHA = '" + Convert.ToDateTime(F.Fecha).ToString(getData["Format_Fecha"]) + "', " +
                                      "Usuario = '" + F.Usuario + "' " +
                                      "WHERE Nombre = '" + F.Nombre + "'";
                    SqlCommand Accion = new SqlCommand(Busqueda, con);
                    int Guarda;
                    Guarda = Accion.ExecuteNonQuery();

                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        bool IFormatos.crearFormato(CXN_FORMATOS F)
        {
            try
            {
                Dictionary<string,string> getData = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getData["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    SqlCommand cmd = new SqlCommand(@"Insert into CXN_FORMATOS (Nombre, " + //param1
                                                           "Cuerpo, " + //param2
                                                           "Firma, " + //param3
                                                           "Firma2, " + //param4
                                                           "Pie, " + //param5
                                                           "CiudadFecha, " + //param6
                                                           "FECHA, " + //param7
                                                           "Usuario) " + //param16
                                  "values                  (@param1, " + // Hor_Estado
                                                           "@param2, " + // Hor_Pac_Id
                                                           "@param3, " + // Hor_Pac_Bod
                                                           "@param4, " + // Hor_Pac_Tipo_Serv
                                                           "@param5, " + // Hor_Pac_Cia
                                                           "@param6, " + // Hor_Pac_Ase
                                                           "@param7, " + // Hor_Pac_Cup
                                                           "@param8)", con); // Hor_Pac_Sal

                    cmd.Parameters.AddWithValue("@param1", F.Nombre);
                    cmd.Parameters.AddWithValue("@param2", F.Cuerpo);
                    cmd.Parameters.AddWithValue("@param3", F.Firma);
                    cmd.Parameters.AddWithValue("@param4", F.Firma2);
                    cmd.Parameters.AddWithValue("@param5", F.Pie);
                    cmd.Parameters.AddWithValue("@param6", F.CiudadFecha);
                    cmd.Parameters.Add(new SqlParameter("@param7", SqlDbType.DateTime)).Value = F.Fecha; // Fecha que graba cita
                    cmd.Parameters.AddWithValue("@param8", F.Usuario);

                    cmd.ExecuteNonQuery();

                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        List<CXN_FORMATOS> IFormatos.getFormatosxPaciente(string TID, string NID)
        {
            try
            {
                Dictionary<string,string> getData = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getData["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    String Query = "SELECT * " +
                                   "FROM CXN_FORMATOS_2 F " +
                                   "INNER JOIN CXN_PACIENTES P ON F.Paciente = P.Pac_id " +
                                   "WHERE P.Pac_TipoId = '" + TID + "' " +
                                   "AND Pac_IdNum = '" + NID + "'";
                    SqlCommand Commando = new SqlCommand(Query, con);
                    SqlDataReader Reader = (Commando.ExecuteReader());
                    if (Reader.HasRows)
                    {
                        List<CXN_FORMATOS> F = new List<CXN_FORMATOS>();

                        while (Reader.Read() == true)
                        {
                            F.Add(new CXN_FORMATOS
                            {
                                Id = Convert.ToInt32(Reader["Id"]),
                                Cuerpo = Reader["Pac_PrimerA"].ToString() + " " + Reader["Pac_SegundoA"].ToString() + " " + Reader["Pac_PrimerN"].ToString() + " " + Reader["Pac_SegundoN"].ToString(),
                                Fecha = Convert.ToDateTime(Reader["Fecha"]),
                                Usuario = Reader["Usuario"].ToString()
                            });
                        }

                        return F;
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

        List<FormatosR> IFormatos.Plantilla(CXN_FORMATOS F)
        {
            try
            {
                Dictionary<string,string> getData = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getData["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }
                    String Cargar_Hora = "Select *, 'AQUI VA EL NOMBRE DEL PRESTADOR' AS NOPRES " +
                                         "FROM CXN_FORMATOS WHERE Nombre = '" + F.Nombre + "'";
                    SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con);
                    SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader());
                    if (Lectura_Hora.Read() == true)
                    {
                        List<FormatosR> Class_Formatos1 = new List<FormatosR>();

                        var Ser = Carga_Ima_Temp();
                        if (Ser == null)
                        {
                            return null;
                        }

                        string Bod_Firma1 = Ser; //trae base64
                        Byte[] bytes = Convert.FromBase64String(Bod_Firma1); //convierte a bytes
                        MemoryStream stmBLOBData = new MemoryStream(bytes);
                        PictureBox pic = new PictureBox();
                        pic.Image = Image.FromStream(stmBLOBData);

                        Class_Formatos1.Add(new FormatosR
                        {
                            EmpresaNombre = Lectura_Hora["NOPRES"].ToString(),
                            Firmas = F.Firma,
                            Firmas2 = F.Firma2,
                            Pie = F.Pie,
                            CiudadFecha = Lectura_Hora["CiudadFecha"].ToString(),
                            Cuerpo = F.Cuerpo,
                            Logo = repoGen.GetBytes(pic.Image)
                        });

                        return Class_Formatos1;
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

        string Carga_Ima_Temp()
        {
            try
            {
                Dictionary<string,string> getData = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getData["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }
                    String Cargar_Hora = "SELECT TOP 1 Com_Logo " +
                                         "FROM CXN_CIA";
                    SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con);
                    SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader());
                    if (Lectura_Hora.Read() == true)
                    {
                        return Lectura_Hora["Com_Logo"].ToString();
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

        List<FormatosR> IFormatos.Export_Cert(int Pac, int Doc, string Tipo)
        {
            try
            {
                Dictionary<string,string> getData = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getData["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    String Cargar_Hora = "";

                    if (Tipo == "Nuevo")
                    {
                        Cargar_Hora = "SELECT TOP 1 * " +
                                      "FROM CXN_FORMATOS_2 F " +
                                      "INNER JOIN CXN_CIA C ON F.Compañia = C.Com_Identificador " +
                                      "WHERE Paciente = '" + Pac + "' ORDER BY Id DESC";
                    }
                    if (Tipo == "Previo")
                    {
                        Cargar_Hora = "SELECT TOP 1 * " +
                                      "FROM CXN_FORMATOS_2 F " +
                                      "INNER JOIN CXN_CIA C ON F.Compañia = C.Com_Identificador " +
                                      "WHERE Id = '" + Doc + "'";
                    }

                    SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con);
                    SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader());
                    if (Lectura_Hora.Read() == true)
                    {
                        List<FormatosR> Class_Formatos = new List<FormatosR>();

                        string Bod_Firma1 = Lectura_Hora["Com_Logo"].ToString(); //trae base64
                        Byte[] bytes = Convert.FromBase64String(Bod_Firma1); //convierte a bytes
                        MemoryStream stmBLOBData = new MemoryStream(bytes);
                        PictureBox pic = new PictureBox();
                        pic.Image = Image.FromStream(stmBLOBData);

                        Class_Formatos.Add(new FormatosR
                        {
                            EmpresaNombre = Lectura_Hora["Com_Nombre"].ToString(),
                            Firmas = Lectura_Hora["Firmas"].ToString(),
                            Firmas2 = Lectura_Hora["Firmas2"].ToString(),
                            Pie = Lectura_Hora["Pie"].ToString(),
                            CiudadFecha = Lectura_Hora["CiudadFecha"].ToString(),
                            Cuerpo = Lectura_Hora["Cuerpo"].ToString(),
                            Logo = repoGen.GetBytes(pic.Image)
                        });

                        return Class_Formatos;
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
    }
}
