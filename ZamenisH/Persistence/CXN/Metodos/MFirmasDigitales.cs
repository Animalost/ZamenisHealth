using Domain.CXN;
using Persistence.CXN.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Persistence.CXN.Metodos
{
    public class MFirmasDigitales : IFirmasDigitales
    {
        bool IFirmasDigitales.InsertSign(CXN_FIRMASDIGITALES F)
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

                    SqlCommand cmd = new SqlCommand(@"Insert into CXN_FIRMASDIGITALES " +
                                                             "(Admision, " +
                                                             "Paciente, " +
                                                             "Firma, " +
                                                             "Usuario) " +
                                         "values             (@param1, " +
                                                             "@param2, " +
                                                             "@param3, " +
                                                             "@param4)", con);

                    cmd.Parameters.AddWithValue("@param1", F.Admision);
                    cmd.Parameters.AddWithValue("@param2", F.Paciente);
                    cmd.Parameters.AddWithValue("@param3", F.Firma);
                    cmd.Parameters.AddWithValue("@param4", F.Usuario);

                    int s = cmd.ExecuteNonQuery();
                    return s > 0 ? true : false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        int IFirmasDigitales.InsertSign_Med(CXN_FIRMASDIGITALES_MED F) 
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

                    SqlCommand cmd = new SqlCommand(@"Insert into CXN_FIRMASDIGITALES_MED " +
                                                             "(Admision, " +
                                                             "Paciente, " +
                                                             "FirmaMedico, " +
                                                             "FirmaPaciente, " +
                                                             "Fecha, " +
                                                             "Usuario, " +
                                                             "Tipo, " +
                                                             "QuienFirma, " +
                                                             "Fotos, " +
                                                             "Acudiente, " +
                                                             "IdAcudiente, " +
                                                             "Parentesco, " +
                                                             "Telefono, " +
                                                             "Hora) " +
                                         "values             (@param1, " +
                                                             "@param2, " +
                                                             "@param3, " +
                                                             "@param4, " +
                                                             "@param5, " +
                                                             "@param6, " +
                                                             "@param7, " +
                                                             "@param8, " +
                                                             "@param9, " +
                                                             "@param10, " +
                                                             "@param11, " +
                                                             "@param12, " +
                                                             "@param13, " +
                                                             "@param14); SELECT SCOPE_IDENTITY()", con);

                    cmd.Parameters.AddWithValue("@param1", F.Admision);
                    cmd.Parameters.AddWithValue("@param2", F.Paciente);
                    cmd.Parameters.AddWithValue("@param3", F.FirmaMedico);
                    cmd.Parameters.AddWithValue("@param4", F.FirmaPaciente);
                    cmd.Parameters.AddWithValue("@param5", Convert.ToDateTime(F.Fecha).Date);
                    cmd.Parameters.AddWithValue("@param6", F.Usuario);
                    cmd.Parameters.AddWithValue("@param7", F.Tipo);
                    cmd.Parameters.AddWithValue("@param8", F.QuienFirma);
                    cmd.Parameters.AddWithValue("@param9", F.Fotos);
                    cmd.Parameters.AddWithValue("@param10", F.Acudiente);
                    cmd.Parameters.AddWithValue("@param11", F.IdAcudiente);
                    cmd.Parameters.AddWithValue("@param12", F.Parentesco);
                    cmd.Parameters.AddWithValue("@param13", F.Telefono);
                    cmd.Parameters.AddWithValue("@param14", Convert.ToDateTime(DateTime.Now));

                    var s = cmd.ExecuteScalar();
                    return Convert.ToInt32(s) > 0 ? Convert.ToInt32(s) : 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 0;
            }
        }
        CXN_FIRMASDIGITALES IFirmasDigitales.getFirmas(int Admision)
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
                                   "FROM CXN_FIRMASDIGITALES " +
                                   "WHERE Admision = @param1";

                    using (SqlCommand Command = new SqlCommand(Query, con))
                    {
                        Command.Parameters.AddWithValue("@param1", Admision);

                        using (SqlDataReader Reader = (Command.ExecuteReader()))
                        {
                            if (Reader.Read() == true)
                            {
                                CXN_FIRMASDIGITALES F = new CXN_FIRMASDIGITALES()
                                {
                                    Usuario = Reader["Usuario"].ToString(),
                                    Admision = Convert.ToInt32(Reader["Admision"]),
                                    Firma = (byte[])Reader["Firma"],
                                    Paciente = Convert.ToInt32(Reader["Paciente"])
                                };

                                return F;
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
        CXN_FIRMASDIGITALES_MED IFirmasDigitales.getFirmas_MED(int Posision)
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
                                   "FROM CXN_FIRMASDIGITALES_MED " +
                                   "WHERE Id = @param1";

                    using (SqlCommand Command = new SqlCommand(Query, con))
                    {
                        Command.Parameters.AddWithValue("@param1", Posision);

                        using (SqlDataReader Reader = (Command.ExecuteReader()))
                        {
                            if (Reader.Read() == true)
                            {
                                CXN_FIRMASDIGITALES_MED F = new CXN_FIRMASDIGITALES_MED()
                                {
                                    Usuario = Reader["Usuario"].ToString(),
                                    Admision = Convert.ToInt32(Reader["Admision"]),
                                    FirmaMedico = (byte[])Reader["FirmaMedico"],
                                    FirmaPaciente = (byte[])Reader["FirmaPaciente"],
                                    Paciente = Convert.ToInt32(Reader["Paciente"]),
                                    Fecha = Convert.ToDateTime(Reader["Fecha"]),
                                    Id = Convert.ToInt32(Reader["Id"]),
                                    Tipo = Reader["Tipo"].ToString()
                                };

                                return F;
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
        List<(int Adm, string Aut, int Cant)> IFirmasDigitales.getAdmitions(int PacienteID, int CIA, DateTime FechaLimite)
        {
            try
            {
                DateTime? getLastInicio = GetLastInicio(PacienteID, CIA);
                if (getLastInicio == null) { return null; }

                var getCon = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getCon["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    String Query = "SELECT Hor_Id, Hor_IniciaSesion, Hor_Autoriza, Hor_CantSesion " +
                                   "FROM CXN_HORARIO " +
                                   "WHERE Hor_Pac_Id = @param2 " +
                                   "AND Hor_Pac_Tipo_Serv IN ('MG','CU') " +
                                   "AND Hor_Pac_Cia = @param3 " +
                                   "AND Hor_Pac_Fecha_Cita BETWEEN @param4 AND @param6 " +
                                   "AND Hor_Estado = @param5 " +
                                   "ORDER BY Hor_Pac_Fecha_Cita ASC";

                    using (SqlCommand Command = new SqlCommand(Query, con))
                    {
                        Command.Parameters.AddWithValue("@param2", PacienteID);
                        Command.Parameters.AddWithValue("@param3", CIA);
                        Command.Parameters.AddWithValue("@param4", Convert.ToDateTime(getLastInicio));
                        Command.Parameters.AddWithValue("@param5", "H");
                        Command.Parameters.AddWithValue("@param6", Convert.ToDateTime(FechaLimite.Date));                         

                        using (SqlDataReader Reader = (Command.ExecuteReader()))
                        {
                            List<(int Adm, string Aut, int Cant)> lista = new List<(int Adm, string Aut, int Cant)>();
                            string Aut = "";
                            int Cant = 0;

                            if (Reader.HasRows)
                            {
                                while (Reader.Read() == true)
                                {
                                    if (Reader["Hor_IniciaSesion"].ToString() == "S")
                                    {
                                        Aut = Reader["Hor_Autoriza"].ToString();
                                        Cant = Reader["Hor_CantSesion"] == DBNull.Value ? 0 : Convert.ToInt32(Reader["Hor_CantSesion"]);
                                    }

                                    lista.Add((Convert.ToInt32(Reader["Hor_Id"]), Aut, Cant)); 
                                }

                                return lista;
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
        List<(int Adm, string Aut, int Cant)> IFirmasDigitales.getAdmitionsByFacturacion(int PacienteID, int CIA, DateTime FechaLimite, DateTime Desde)
        {
            try
            {
                /* DateTime? getLastInicio = GetLastInicio(PacienteID, CIA);
                 if (getLastInicio == null) { return null; }*/

                var getCon = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getCon["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    String Query = "SELECT Hor_Id, Hor_IniciaSesion, Hor_Autoriza, Hor_CantSesion " +
                                   "FROM CXN_HORARIO " +
                                   "WHERE Hor_Pac_Id = @param2 " +
                                   "AND Hor_Pac_Tipo_Serv IN ('MG','CU') " +
                                   "AND Hor_Pac_Cia = @param3 " +
                                   "AND Hor_Pac_Fecha_Cita BETWEEN @param4 AND @param6 " +
                                   "AND Hor_Estado = @param5 " +
                                   "ORDER BY Hor_Pac_Fecha_Cita ASC";

                    using (SqlCommand Command = new SqlCommand(Query, con))
                    {
                        Command.Parameters.AddWithValue("@param2", PacienteID);
                        Command.Parameters.AddWithValue("@param3", CIA);
                        //Command.Parameters.AddWithValue("@param4", Convert.ToDateTime(getLastInicio));
                        Command.Parameters.AddWithValue("@param4", Convert.ToDateTime(Desde.Date));
                        Command.Parameters.AddWithValue("@param5", "H");
                        Command.Parameters.AddWithValue("@param6", Convert.ToDateTime(FechaLimite.Date));

                        using (SqlDataReader Reader = (Command.ExecuteReader()))
                        {
                            List<(int Adm, string Aut, int Cant)> lista = new List<(int Adm, string Aut, int Cant)>();
                            string Aut = "";
                            int Cant = 0;

                            if (Reader.HasRows)
                            {
                                while (Reader.Read() == true)
                                {
                                    if (Reader["Hor_IniciaSesion"].ToString() == "S")
                                    {
                                        Aut = Reader["Hor_Autoriza"].ToString();
                                        Cant = Reader["Hor_CantSesion"] == DBNull.Value ? 0 : Convert.ToInt32(Reader["Hor_CantSesion"]);
                                    }

                                    lista.Add((Convert.ToInt32(Reader["Hor_Id"]), Aut, Cant));
                                }

                                return lista;
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
        DateTime? GetLastInicio(int PacienteID, int CIA)
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

                    String Query = "SELECT Hor_Pac_Fecha_Cita " +
                                   "FROM CXN_HORARIO " +
                                   "WHERE Hor_IniciaSesion = @param1 " +
                                   "AND Hor_Pac_Id = @param2 " +
                                   "AND Hor_Pac_Tipo_Serv IN ('MG','CU') " +
                                   "AND Hor_Pac_Cia = @param3 " +
                                   "AND Hor_Estado = @param5 " +
                                   "ORDER BY Hor_Pac_Fecha_Cita DESC";

                    using (SqlCommand Command = new SqlCommand(Query, con))
                    {
                        Command.Parameters.AddWithValue("@param1", "S");
                        Command.Parameters.AddWithValue("@param2", PacienteID);
                        Command.Parameters.AddWithValue("@param3", CIA);
                        Command.Parameters.AddWithValue("@param5", "H");

                        using (SqlDataReader Reader = (Command.ExecuteReader()))
                        {
                            if (Reader.Read() == true)
                            {                               
                                return Convert.ToDateTime(Reader["Hor_Pac_Fecha_Cita"]);
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
        string IFirmasDigitales.ImageNull()
        {
            return "iVBORw0KGgoAAAANSUhEUgAAAogAAAA5CAYAAAC1QNpDAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAABuJSURBVHhe7Z1HllxFE4VxG+AHNgAIFoARKwDEXIC0AZwWAIixcGOcxsJoipsizFAIGAo3FmYFuP7PFeQhOzoz7k1XVaLjO+cOEP3S1avMWxGZ7123FwRBEARBEAQZ19l/CIIgCIIgCA43YRCDIAiCIAiCfYRBDIIgCIIgCPYRBjEIgiAIgiDYRxjEIAiCIAiCYB9hEIMgCIIgCIJ9TDGIP/zww96bb76599RTT+099NBDe9ddd90+3XvvvVf//eWXX977+uuv7eVBEATBlsD8/e677+49//zzV+dpzNd2DocefPDBvccee+zqPP7VV1/ZYnYGrDGvvPLK1baW+pL68cYbb+x9//339vIgCP5hyCB+/vnnRUPIdMcdd1ydZH799VdbZBH8HcwnrrNloX6YUwVMHKUJI+nmm2++OnGMmFi0FWWgLNtnTMCrwTjZfo3Ka/eszwZ/Z8esRelHCNr60UcfyfdWjdQvW48V6sXfYZFdBb4r3n2bhPtutN85GEdWL/4/2rcLYD668847D7RRFe5h3EP4PGGYRj9TXP/www8fqGeGnnvuub2//vrLVimD++Stt97au++++w6UrQrfV7Sjx2R9/PHHV8fm+uuv31cmPgPcxz1lvvfee12fP+7hd955pzqe999//4FrVgtj+91339mmXOWZZ545MG4tgkHGff7SSy/tffbZZ9V+zwSGnX02aNcHH3wwrT34/h07dqx7rO65556r4/Tkk09e/TGxqbFK4DuKz/qWW2450LYk3CePPvro3qVLl5a0rcsgJhNkG9sqdE4xY5j47bVWDLRZNSD4O9TZA4yOLa+lnSOwukdUY8Zng3vAXjNDWOiV+6tE6/2NewaGajbMoFmh3bP43//+d6D8mnYhotTSXlVYID788ENblcTjjz/evTgx4X77888/bZUUzINYrGePFRbRX375xVZXpWQOc8G8qAsevuMjRjcJ37Wff/55X9lffPHF3g033HDgbzeh0hjA1M9uTzL6LZ9fC998883ejTfeeKDektCWP/74wxbRxYkTJ9x7rEdoH+71TZjFs2fPyuN2/PjxaeOWw1dvA76M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";
        }
        bool IFirmasDigitales.EliminarFirma(int Admision)
        {
            var getCon = Conexion.Conection();

            using (SqlConnection con = new SqlConnection(getCon["Conexion"]))
            {
                if (con != null && con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                string Busqueda2 = "DELETE FROM CXN_FIRMASDIGITALES " +
                                   "WHERE Admision = @param1";

                using (SqlCommand Accion2 = new SqlCommand(Busqueda2, con))
                {
                    Accion2.Parameters.AddWithValue("@param1", Admision);
                    return Accion2.ExecuteNonQuery() > 0 ? true : false;                    
                }                                   
            }
        }
        List<CXN_FIRMASDIGITALES_MED> IFirmasDigitales.getFirmas_MEDICAL(string Paciente)
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
                                   "FROM CXN_FIRMASDIGITALES_MED M " +
                                   "INNER JOIN CXN_PACIENTES P ON M.Paciente = P.Pac_Id " +
                                   "INNER JOIN CXN_BODEGAS B ON M.Usuario = B.Bod_Usuario " +
                                   "WHERE P.Pac_IdNum = @param1 " +
                                   "ORDER BY Fecha DESC";

                    using (SqlCommand Command = new SqlCommand(Query, con))
                    {
                        Command.Parameters.AddWithValue("@param1", Paciente);

                        using (SqlDataReader Reader = (Command.ExecuteReader()))
                        {                            
                            if (Reader.HasRows)
                            {
                                List<CXN_FIRMASDIGITALES_MED> lista = new List<CXN_FIRMASDIGITALES_MED>();

                                while (Reader.Read() == true)
                                {
                                    lista.Add(new CXN_FIRMASDIGITALES_MED 
                                    {
                                        Usuario = Reader["Usuario"].ToString(),
                                        Admision = Convert.ToInt32(Reader["Admision"]),
                                        FirmaMedico = (byte[])Reader["FirmaMedico"],
                                        FirmaPaciente = (byte[])Reader["FirmaPaciente"],
                                        Paciente = Convert.ToInt32(Reader["Paciente"]),
                                        Fecha = Convert.ToDateTime(Reader["Fecha"]),
                                        Id = Convert.ToInt32(Reader["Id"]),
                                        Tipo = Reader["Tipo"].ToString(),
                                        PacienteName = Reader["Pac_PrimerA"].ToString() + " " +
                                                       Reader["Pac_SegundoA"].ToString() + " " +
                                                       Reader["Pac_PrimerN"].ToString() + " " +
                                                       Reader["Pac_SegundoN"].ToString(),
                                        IdPaciente = Reader["Pac_TipoId"].ToString() + " " + Reader["Pac_IdNum"].ToString(),
                                        DirPaciente = Reader["Pac_Direccion"].ToString(),
                                        IdProfesional = Reader["Bod_Reg_Med"].ToString(),
                                        TelPaciente = Reader["Pac_Telefono"].ToString()
                                    });
                                }

                                return lista;
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
    }
}
