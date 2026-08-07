using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using Persistence.CXN.Interfaces;
using Domain.CXN;
using Domain;

namespace Persistence.CXN.Metodos
{
    public class MBodegas : IBodegas
    {
        private string Query;

        List<CXN_BODEGAS> IBodegas.GetAllProfesionales(string Tipo)
        {
            try
            {
                var getCon = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getCon["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    String QueryCons = "";

                    if (Tipo == "" || Tipo == "Todos")
                    {
                        QueryCons = "SELECT Bod_Responsable, Bod_Numero, Bod_Usuario, Bod_Tipo " +
                                    "FROM CXN_BODEGAS " +
                                    "WHERE Bod_Estado = 'A' " +
                                    "ORDER BY Bod_Responsable ASC";
                    }
                    else
                    {
                        QueryCons = "SELECT Bod_Responsable, Bod_Numero, Bod_Usuario, Bod_Tipo " +
                                    "FROM CXN_BODEGAS " +
                                    "WHERE Bod_Estado = 'A' " +
                                    "AND Bod_Tipo = @param1 " +
                                    "ORDER BY Bod_Responsable ASC";
                    }

                    using (SqlCommand Command = new SqlCommand(QueryCons, con))
                    {
                        if (Tipo.Length == 2)
                        {
                            Command.Parameters.AddWithValue("@param1", Tipo);
                        }
                        
                        using (SqlDataReader Reader = (Command.ExecuteReader()))
                        {
                            if (Reader.HasRows)
                            {
                                List<CXN_BODEGAS> L = new List<CXN_BODEGAS>();

                                while (Reader.Read() == true)
                                {
                                    L.Add(new CXN_BODEGAS 
                                    { 
                                        Bod_Responsable = Reader["Bod_Responsable"].ToString(),
                                        Bod_Numero = Convert.ToInt32(Reader["Bod_Numero"]),
                                        Bod_Usuario = Reader["Bod_Usuario"].ToString(),
                                        Bod_Tipo = Reader["Bod_Tipo"].ToString()
                                    });
                                }
                                return L;
                            }
                            else
                            {
                                return null;
                            }
                        }
                    }                    
                }
            }
            catch (Exception x)
            {
                Console.WriteLine(x.Message);
                return null;
            }
        }
        List<string> IBodegas.Profesionales(string SeleccionProfesional)
        {
            try
            {
                var getCon = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getCon["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    switch (SeleccionProfesional)
                    {
                        case "Enfermeros":
                            this.Query = "SELECT Bod_Responsable " +
                                         "FROM CXN_BODEGAS " +
                                         "WHERE Bod_Estado = 'A' " +
                                         "AND Bod_Tipo IN ('CU') " +
                                         "ORDER BY Bod_Responsable ASC";
                            break;

                        case "Medicos Generales":
                            this.Query = "SELECT Bod_Responsable " +
                                         "FROM CXN_BODEGAS " +
                                         "WHERE Bod_Estado = 'A' " +
                                         "AND Bod_Tipo IN ('MG') " +
                                         "ORDER BY Bod_Responsable ASC";
                            break;

                        case "Fisiatras":
                            this.Query = "SELECT Bod_Responsable " +
                                         "FROM CXN_BODEGAS " +
                                         "WHERE Bod_Estado = 'A' " +
                                         "AND Bod_Tipo IN ('FI') " +
                                         "ORDER BY Bod_Responsable ASC";
                            break;

                        case "Terapeutas":
                            this.Query = "SELECT Bod_Responsable " +
                                         "FROM CXN_BODEGAS " +
                                         "WHERE Bod_Estado = 'A' " +
                                         "AND Bod_Tipo IN ('TF','TO','PS') " +
                                         "ORDER BY Bod_Responsable ASC";
                            break;

                        case "TerapeutasFisicas":
                            this.Query = "SELECT Bod_Responsable " +
                                         "FROM CXN_BODEGAS " +
                                         "WHERE Bod_Estado = 'A' " +
                                         "AND Bod_Tipo IN ('TF') " +
                                         "ORDER BY Bod_Responsable ASC";
                            break;

                        case "Radiologia":
                            this.Query = "SELECT Bod_Responsable " +
                                         "FROM CXN_BODEGAS " +
                                         "WHERE Bod_Estado = 'A' " +
                                         "AND Bod_Tipo IN ('RA') " +
                                         "ORDER BY Bod_Responsable ASC";
                            break;

                        default:
                            this.Query = "SELECT Bod_Responsable " +
                                    "FROM CXN_BODEGAS " +
                                    "WHERE Bod_Estado = 'A' " +
                                    "ORDER BY Bod_Responsable ASC";
                            break;
                    }

                    SqlCommand Command = new SqlCommand(Query, con);
                    SqlDataReader Reader = (Command.ExecuteReader());
                    if (Reader.HasRows)
                    {
                        List<string> L = new List<string>();

                        while (Reader.Read() == true)
                        {
                            L.Add(Reader["Bod_Responsable"].ToString());
                        }
                        return L;
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
        (int CodProf, string TipoBod) IBodegas.ProfesionalId(string IdProf)
        {
            try
            {
                var getCon = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getCon["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    String Query = "SELECT Bod_Numero, Bod_Tipo " +
                                   "FROM CXN_BODEGAS " +
                                   "WHERE Bod_Responsable = '" + IdProf + "'";
                    SqlCommand Command = new SqlCommand(Query, con);
                    SqlDataReader Reader = (Command.ExecuteReader());
                    if (Reader.Read() == true)
                    {
                        return (Convert.ToInt32(Reader["Bod_Numero"]), Reader["Bod_Tipo"].ToString());
                    }
                    else
                    {
                        return (0, "");
                    }
                }
            }
            catch
            {
                return (0, "");
            }
        }
        string IBodegas.ProfesionalNombre(int Code)
        {
            try
            {
                var getCon = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getCon["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    String Query = "SELECT Bod_Responsable " +
                                   "FROM CXN_BODEGAS " +
                                   "WHERE Bod_Numero = '" + Code + "'";
                    SqlCommand Command = new SqlCommand(Query, con);
                    SqlDataReader Reader = (Command.ExecuteReader());
                    if (Reader.Read() == true)
                    {
                        return Reader["Bod_Responsable"].ToString();
                    }
                    else
                    {
                        return "";
                    }
                }
            }
            catch
            {
                return "";
            }
        }
        List<CXN_BODEGAS> IBodegas.Filtrar()
        {
            try
            {
                var getCon = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getCon["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }
                    String Cargar_Hora = "SELECT DISTINCT B.Bod_Responsable, B.Bod_Estado, B.Bod_Numero, B.Bod_Tipo " +
                                         "FROM CXN_BODEGAS B " +
                                         "INNER JOIN CXN_DISPONIBILIDAD_2 D ON B.Bod_Numero = D.Med " +
                                         "GROUP BY B.Bod_Responsable, B.Bod_Estado, B.Bod_Numero, B.Bod_Tipo " +
                                         "ORDER BY B.Bod_Responsable ASC";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.HasRows)
                            {
                                List<CXN_BODEGAS> B = new List<CXN_BODEGAS>();

                                while (Lectura_Hora.Read() == true)
                                {
                                    B.Add(new CXN_BODEGAS
                                    {
                                        Bod_Numero = Convert.ToInt32(Lectura_Hora["Bod_Numero"]),
                                        Bod_Responsable = Lectura_Hora["Bod_Responsable"].ToString(),
                                        Bod_Estado = Lectura_Hora["Bod_Estado"].ToString(),
                                        Bod_Tipo = Lectura_Hora["Bod_Tipo"].ToString()
                                    });
                                }
                                return B;
                            }
                            else
                            {
                                return null;
                            }
                        }    
                    }                    
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        void IBodegas.ActivarDesactivarBodega(int Bode, string Estado)
        {
            var getCon = Conexion.Conection();

            using (SqlConnection con = new SqlConnection(getCon["Conexion"]))
            {
                if (con != null && con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                string Busqueda = "UPDATE CXN_BODEGAS " +
                                  "SET Bod_Estado = '" + Estado + "' " +
                                  "WHERE Bod_Numero = '" + Bode + "'";
                SqlCommand Accion = new SqlCommand(Busqueda, con);
                int Guarda;
                Guarda = Accion.ExecuteNonQuery();
            }
        }
        bool IBodegas.EsProfesional(string Tipo, string User)
        {
            try
            {
                var getCon = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getCon["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    String Query = "";

                    if (Tipo == "Enfermero")
                    {
                        Query = "SELECT Bod_Numero " +
                               "FROM CXN_BODEGAS B " +
                               "WHERE Bod_Usuario = '" + User + "' " +
                               "AND Bod_Tipo = 'CU'";
                    }

                    if (Tipo == "Medico")
                    {
                        Query = "SELECT Bod_Numero " +
                                "FROM CXN_BODEGAS B " +
                                "WHERE Bod_Usuario = '" + User + "' " +
                                "AND Bod_Tipo IN ('MG','FI','RA')";
                    }

                    SqlCommand Commando = new SqlCommand(Query, con);
                    SqlDataReader Reader = (Commando.ExecuteReader());
                    if (Reader.Read() == true)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch
            {
                return false;
            }
        }
        string IBodegas.NombreProfesionalXUser(string User)
        {
            try
            {
                var getCon = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getCon["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }
                    String Query = "SELECT Bod_Responsable " +
                                   "FROM CXN_BODEGAS " +
                                   "WHERE Bod_Usuario = '" + User + "'";
                    SqlCommand Commando = new SqlCommand(Query, con);
                    SqlDataReader Reader = (Commando.ExecuteReader());
                    if (Reader.Read() == true)
                    {
                        return Reader["Bod_Responsable"].ToString();

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
        List<string> IBodegas.getProfByTipo(string Tipo)
        {
            try
            {
                var getCon = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getCon["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    String Query = "SELECT Bod_Responsable " +
                                   "FROM CXN_BODEGAS " +
                                   "WHERE Bod_Tipo = '" + Tipo + "'";
                    SqlCommand Commando = new SqlCommand(Query, con);
                    SqlDataReader Reader = (Commando.ExecuteReader());
                    if (Reader.HasRows)
                    {
                        List<string> lista = new List<string>();

                        while (Reader.Read() == true)
                        {
                            lista.Add(Reader["Bod_Responsable"].ToString());
                        }

                        return lista;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return null;
            }
        }
        CXN_BODEGAS IBodegas.getDatosName(string Responsable)
        {
            try
            {
                var getCon = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getCon["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    String Query = "SELECT * " +
                                   "FROM CXN_BODEGAS " +
                                   "WHERE Bod_Responsable = '" + Responsable + "'";
                    SqlCommand Command = new SqlCommand(Query, con);
                    SqlDataReader Reader = (Command.ExecuteReader());
                    if (Reader.Read() == true)
                    {
                        CXN_BODEGAS B = new CXN_BODEGAS
                        {
                            Bod_Id = Convert.ToInt32(Reader["Bod_Id"]),
                            Bod_Usuario = Reader["Bod_Usuario"].ToString(),
                            Bod_Responsable = Reader["Bod_Responsable"].ToString(),
                            Bod_Reg_Med = Reader["Bod_Reg_Med"].ToString(),
                            Bod_Tipo = Reader["Bod_Tipo"].ToString(),
                            Bod_Estado = Reader["Bod_Estado"].ToString(),
                            Bod_Firma = Reader["Bod_Firma"].ToString(),
                            Bod_Numero = Convert.ToInt32(Reader["Bod_Numero"])
                        };

                        return B;
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
        CXN_BODEGAS IBodegas.getDatosCode(int Code)
        {
            try
            {
                var getCon = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getCon["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    String Query = "SELECT * " +
                                   "FROM CXN_BODEGAS " +
                                   "WHERE Bod_Numero = @param1";

                    using (SqlCommand Command = new SqlCommand(Query, con))
                    {
                        Command.Parameters.AddWithValue("@param1", Code);

                        using (SqlDataReader Reader = (Command.ExecuteReader()))
                        {
                            if (Reader.Read() == true)
                            {
                                CXN_BODEGAS B = new CXN_BODEGAS
                                {
                                    Bod_Id = Convert.ToInt32(Reader["Bod_Id"]),
                                    Bod_Usuario = Reader["Bod_Usuario"].ToString(),
                                    Bod_Responsable = Reader["Bod_Responsable"].ToString(),
                                    Bod_Reg_Med = Reader["Bod_Reg_Med"].ToString(),
                                    Bod_Tipo = Reader["Bod_Tipo"].ToString(),
                                    Bod_Estado = Reader["Bod_Estado"].ToString(),
                                    Bod_Firma = Reader["Bod_Firma"].ToString(),
                                    Bod_Numero = Convert.ToInt32(Reader["Bod_Numero"])
                                };

                                return B;
                            }
                            else
                            {
                                return null;
                            }
                        }
                    }                    
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        CXN_BODEGAS IBodegas.getDatosUser(string User)
        {
            try
            {
                var getCon = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getCon["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    String Query = "SELECT * " +
                                   "FROM CXN_BODEGAS " +
                                   "WHERE Bod_Usuario = @param1";

                    using (SqlCommand Command = new SqlCommand(Query, con))
                    {
                        Command.Parameters.AddWithValue("@param1", User);

                        using (SqlDataReader Reader = (Command.ExecuteReader()))
                        {
                            if (Reader.Read() == true)
                            {
                                CXN_BODEGAS B = new CXN_BODEGAS
                                {
                                    Bod_Id = Convert.ToInt32(Reader["Bod_Id"]),
                                    Bod_Usuario = Reader["Bod_Usuario"].ToString(),
                                    Bod_Responsable = Reader["Bod_Responsable"].ToString(),
                                    Bod_Reg_Med = Reader["Bod_Reg_Med"].ToString(),
                                    Bod_Tipo = Reader["Bod_Tipo"].ToString(),
                                    Bod_Estado = Reader["Bod_Estado"].ToString(),
                                    Bod_Firma = Reader["Bod_Firma"].ToString(),
                                    Bod_Numero = Convert.ToInt32(Reader["Bod_Numero"])
                                };

                                return B;
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
        bool IBodegas.updateUser(CXN_BODEGAS B)
        {
            try
            {
                var getCon = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getCon["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    string Busqueda = "UPDATE CXN_BODEGAS " +
                                      "SET Bod_Firma = @param1, " +
                                      "Bod_Estado = @param2, " +
                                      "Bod_Reg_Med = @param3 " +
                                      "WHERE Bod_Numero = @param4";

                    using (SqlCommand Accion = new SqlCommand(Busqueda, con))
                    {
                        Accion.Parameters.AddWithValue("@param1", B.Bod_Firma);
                        Accion.Parameters.AddWithValue("@param2", B.Bod_Estado);
                        Accion.Parameters.AddWithValue("@param3", B.Bod_Reg_Med);
                        Accion.Parameters.AddWithValue("@param4", B.Bod_Numero);

                        return Accion.ExecuteNonQuery() > 0 ? true : false;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }
        bool IBodegas.createUser(CXN_BODEGAS B)
        {
            try
            {
                var getCon = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getCon["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    SqlCommand cmd = new SqlCommand(@"Insert into CXN_BODEGAS " +
                                                             "(Bod_Numero, " +
                                                             "Bod_Usuario, " +
                                                             "Bod_Responsable, " +
                                                             "Bod_Reg_Med, " +
                                                             "Bod_Tipo, " +
                                                             "Bod_Firma, " +
                                                             "Bod_Estado) " +
                                         "values             (@param1, " +
                                                             "@param2, " +
                                                             "@param3, " +
                                                             "@param4, " +
                                                             "@param5, " +
                                                             "@param6, " +
                                                             "@param7)", con);

                    cmd.Parameters.AddWithValue("@param1", B.Bod_Numero);
                    cmd.Parameters.AddWithValue("@param2", B.Bod_Usuario);
                    cmd.Parameters.AddWithValue("@param3", B.Bod_Responsable);
                    cmd.Parameters.AddWithValue("@param4", B.Bod_Reg_Med);
                    cmd.Parameters.AddWithValue("@param5", B.Bod_Tipo);
                    cmd.Parameters.AddWithValue("@param6", B.Bod_Firma);
                    cmd.Parameters.AddWithValue("@param7", B.Bod_Estado);
                    return cmd.ExecuteNonQuery() > 0 ? true : false;                    
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        List<string> IBodegas.getTipos()
        {
            try
            {
                var getCon = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getCon["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    String Query = "SELECT DISTINCT Bod_Tipo  " +
                                   "FROM CXN_BODEGAS " +
                                   "ORDER BY Bod_Tipo ASC";
                    SqlCommand Command = new SqlCommand(Query, con);
                    SqlDataReader Reader = (Command.ExecuteReader());
                    if (Reader.HasRows)
                    {
                        List<string> lTipos = new List<string>();

                        while (Reader.Read() == true)
                        {
                            lTipos.Add(Reader["Bod_Tipo"].ToString());
                        }

                        return lTipos;
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
        List<string> IBodegas.getBodegas()
        {
            try
            {
                var getCon = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getCon["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    String Query = "SELECT *   " +
                                   "FROM CXN_BODEGAS " +
                                   "ORDER BY Bod_Responsable ASC";
                    SqlCommand Command = new SqlCommand(Query, con);
                    SqlDataReader Reader = (Command.ExecuteReader());
                    if (Reader.HasRows)
                    {
                        List<string> lTipos = new List<string>();

                        while (Reader.Read() == true)
                        {
                            lTipos.Add(Reader["Bod_Responsable"].ToString());
                        }

                        return lTipos;
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
        DataTable IBodegas.Profesionales2(string SeleccionProfesional)
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

                    if (SeleccionProfesional == "Todos")
                    {
                        DataTable dt_Prof = new DataTable();
                        string query_Prof = "SELECT Bod_Responsable " +
                                            "FROM CXN_BODEGAS " +
                                            "WHERE Bod_Estado = 'A' " +
                                            "ORDER BY Bod_Responsable ASC";
                        SqlCommand cmd_Prof = new SqlCommand(query_Prof, con);
                        SqlDataAdapter da_Prof = new SqlDataAdapter(cmd_Prof);
                        da_Prof.Fill(dt_Prof);
                        return dt_Prof;
                    }
                    else
                    {
                        string TP;
                        switch (SeleccionProfesional)
                        {
                            case "Enfermeros":
                                TP = "CU";
                                break;

                            case "Medicos Generales":
                                TP = "MG";
                                break;

                            case "Fisiatras":
                                TP = "FI";
                                break;

                            case "Terapeutas":
                                DataTable dt_Prof3 = new DataTable();
                                string query_Prof3 = "SELECT Bod_Responsable " +
                                                    "FROM CXN_BODEGAS " +
                                                    "WHERE Bod_Estado = 'A' " +
                                                    "AND Bod_Tipo IN ('TF','TO','PS') " +
                                                    "ORDER BY Bod_Responsable ASC";
                                SqlCommand cmd_Prof3 = new SqlCommand(query_Prof3, con);
                                SqlDataAdapter da_Prof3 = new SqlDataAdapter(cmd_Prof3);
                                da_Prof3.Fill(dt_Prof3);
                                return dt_Prof3;

                            default:
                                DataTable dt_Prof2 = new DataTable();
                                string query_Prof2 = "SELECT Bod_Responsable " +
                                                    "FROM CXN_BODEGAS " +
                                                    "WHERE Bod_Estado = 'A' " +
                                                    "ORDER BY Bod_Responsable ASC";
                                SqlCommand cmd_Prof2 = new SqlCommand(query_Prof2, con);
                                SqlDataAdapter da_Prof2 = new SqlDataAdapter(cmd_Prof2);
                                da_Prof2.Fill(dt_Prof2);
                                return dt_Prof2;
                        }

                        DataTable dt_Prof = new DataTable();
                        string query_Prof = "SELECT Bod_Responsable " +
                                            "FROM CXN_BODEGAS " +
                                            "WHERE Bod_Estado = 'A' " +
                                            "AND Bod_Tipo = '" + TP + "' " +
                                            "ORDER BY Bod_Responsable ASC";
                        SqlCommand cmd_Prof = new SqlCommand(query_Prof, con);
                        SqlDataAdapter da_Prof = new SqlDataAdapter(cmd_Prof);
                        da_Prof.Fill(dt_Prof);
                        return dt_Prof;
                    }
                }
            }
            catch
            {
                return null;
            }
        }
        List<int> IBodegas.FiltrarCodesByTipose(string Tipo)
        {
            try
            {
                var getCon = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getCon["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    String Cargar_Hora = "SELECT DISTINCT Bod_Numero " +
                                         "FROM CXN_BODEGAS " +
                                         "WHERE Bod_Tipo = @param1";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", Tipo);

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.HasRows)
                            {
                                List<int> B = new List<int>();

                                while (Lectura_Hora.Read() == true)
                                {
                                    B.Add(Convert.ToInt32(Lectura_Hora["Bod_Numero"]));
                                }

                                return B;
                            }
                            else
                            {
                                return null;
                            }
                        }
                    }                    
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return null;
            }
        }
    }
}
