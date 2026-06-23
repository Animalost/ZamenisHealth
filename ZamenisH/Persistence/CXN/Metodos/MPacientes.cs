using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using Persistence.CXN.Interfaces;
using Domain.CXN;
using Domain;

namespace Persistence.CXN.Metodos
{
    public class MPacientes : IPacientes
    {
        string IPacientes.getTipoDoc(string Tipo)
        {
            try
            {
                var getConect = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getConect["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    String Query = "SELECT * FROM CXN_DOCUMENTOS WHERE Tipo_Documento = @param1";

                    using (SqlCommand Command = new SqlCommand(Query, con))
                    {
                        Command.Parameters.AddWithValue("@param1", Tipo);

                        using (SqlDataReader Reader = (Command.ExecuteReader()))
                        {
                            if (Reader.Read() == true)
                            {
                                return Reader["Tipo_Abreviacion"].ToString();
                            }
                            else
                            {
                                return "CC";
                            }
                        }
                    }                                           
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return "CC";
            }            
        }
        CXN_PACIENTES IPacientes.LlamarPacienteNumDoc(string NId)
        {
            try
            {
                var getConect = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getConect["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    String Query = "SELECT * " +
                                   "FROM CXN_PACIENTES " +
                                   "WHERE Pac_IdNum = @param1";

                    using (SqlCommand Command = new SqlCommand(Query, con))
                    {
                        Command.Parameters.AddWithValue("@param1", NId);

                        using (SqlDataReader Reader = (Command.ExecuteReader()))
                        {
                            if (Reader.Read() == true)
                            {
                                CXN_PACIENTES DP = new CXN_PACIENTES
                                {
                                    Pac_TipoId = (Reader["Pac_TipoId"] == DBNull.Value ? "" : Reader["Pac_TipoId"].ToString()),
                                    Pac_IdNum = (Reader["Pac_IdNum"] == DBNull.Value ? "" : Reader["Pac_IdNum"].ToString()),
                                    Pac_FechaNto = (Reader["Pac_FechaNto"] == DBNull.Value ? DateTime.Now.Date : Convert.ToDateTime(Reader["Pac_FechaNto"])),
                                    Pac_PrimerN = (Reader["Pac_PrimerN"] == DBNull.Value ? "" : Reader["Pac_PrimerN"].ToString()),
                                    Pac_SegundoN = (Reader["Pac_SegundoN"] == DBNull.Value ? "" : Reader["Pac_SegundoN"].ToString()),
                                    Pac_PrimerA = (Reader["Pac_PrimerA"] == DBNull.Value ? "" : Reader["Pac_PrimerA"].ToString()),
                                    Pac_SegundoA = (Reader["Pac_SegundoA"] == DBNull.Value ? "" : Reader["Pac_SegundoA"].ToString()),
                                    Pac_Sexo = (Reader["Pac_Sexo"] == DBNull.Value ? "" : Reader["Pac_Sexo"].ToString()),
                                    Pac_Telefono = (Reader["Pac_Telefono"] == DBNull.Value ? "" : Reader["Pac_Telefono"].ToString()),
                                    Pac_TelefonoAux = (Reader["Pac_TelefonoAux"] == DBNull.Value ? "" : Reader["Pac_TelefonoAux"].ToString()),
                                    Pac_Direccion = (Reader["Pac_Direccion"] == DBNull.Value ? "" : Reader["Pac_Direccion"].ToString()),
                                    Pac_Email = (Reader["Pac_Email"] == DBNull.Value ? "" : Reader["Pac_Email"].ToString()),
                                    Pac_Mun_Cod = (Reader["Pac_Mun_Cod"] == DBNull.Value ? "" : Reader["Pac_Mun_Cod"].ToString()),
                                    Pac_Zona = (Reader["Pac_Zona"] == DBNull.Value ? "" : Reader["Pac_Zona"].ToString()),
                                    Pac_Localidad = (Reader["Pac_Localidad"] == DBNull.Value ? "" : Reader["Pac_Localidad"].ToString()),
                                    Pac_Acudiente = (Reader["Pac_Acudiente"] == DBNull.Value ? "" : Reader["Pac_Acudiente"].ToString()),
                                    Pac_Parentesco = (Reader["Pac_Parentesco"] == DBNull.Value ? "" : Reader["Pac_Parentesco"].ToString()),
                                    Pac_DireccionAcu = (Reader["Pac_DireccionAcu"] == DBNull.Value ? "" : Reader["Pac_DireccionAcu"].ToString()),
                                    Pac_TelefonoAcu = (Reader["Pac_TelefonoAcu"] == DBNull.Value ? "" : Reader["Pac_TelefonoAcu"].ToString()),
                                    Pac_CorreoAcu = (Reader["Pac_CorreoAcu"] == DBNull.Value ? "" : Reader["Pac_CorreoAcu"].ToString()),
                                    Pac_Dep_Cod = (Reader["Pac_Dep_Cod"] == DBNull.Value ? "" : Reader["Pac_Dep_Cod"].ToString()),
                                    Pac_Id = Convert.ToInt32(Reader["Pac_Id"]),
                                    Pac_Doble = (Reader["Pac_Doble"] == DBNull.Value ? "" : Reader["Pac_Doble"].ToString()),
                                    Pac_2VXS = (Reader["Pac_2VXS"] == DBNull.Value ? "" : Reader["Pac_2VXS"].ToString()),
                                    Pac_Especial = (Reader["Pac_Especial"] == DBNull.Value ? "N" : Reader["Pac_Especial"].ToString()),
                                    Pac_Contrato = (Reader["Pac_Contrato"] == DBNull.Value ? "" : Reader["Pac_Contrato"].ToString()),
                                    Pac_Aseguradora = (Reader["Pac_Aseguradora"] == DBNull.Value ? 0 : Convert.ToInt32(Reader["Pac_Aseguradora"])),
                                    Pac_Regimen = (Reader["Pac_Regimen"] == DBNull.Value ? "01" : Reader["Pac_Regimen"].ToString()),
                                    Pac_FibInf = (Reader["Pac_FibInf"] == DBNull.Value ? "Incluido" : "Excluido"),
                                    Pac_PaisOrigen = (Reader["Pac_PaisOrigen"] == DBNull.Value ? "" : Reader["Pac_PaisOrigen"].ToString()),
                                    Pac_Categoria = (Reader["Pac_Categoria"] == DBNull.Value ? "" : Reader["Pac_Categoria"].ToString()),
                                    Pac_Residencia = (Reader["Pac_Residencia"] == DBNull.Value ? "" : Reader["Pac_Residencia"].ToString()),
                                    Pac_ECivil = (Reader["Pac_ECivil"] == DBNull.Value ? "" : Reader["Pac_ECivil"].ToString()),
                                    Pac_Ocupacion = (Reader["Pac_Ocupacion"] == DBNull.Value ? "" : Reader["Pac_Ocupacion"].ToString()),
                                    Discapacidad = (Reader["Discapacidad"] == DBNull.Value ? "" : Reader["Discapacidad"].ToString()),
                                    Etnia = (Reader["Etnia"] == DBNull.Value ? "" : Reader["Etnia"].ToString()),
                                    VIH = (Reader["VIH"] == DBNull.Value ? "" : Reader["VIH"].ToString()),
                                    Hepatitis = (Reader["Hepatitis"] == DBNull.Value ? "" : Reader["Hepatitis"].ToString()),
                                    HoraNto = (Reader["HoraNto"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(Reader["HoraNto"])),
                                    IdentidadGenero = (Reader["IdentidadGenero"] == DBNull.Value ? "04" : Reader["IdentidadGenero"].ToString())
                                };

                                return DP;
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
        CXN_PACIENTES IPacientes.LlamarPacienteDOC(string TipoId, string NId)
        {
            var gerdataCone = Conexion.Conection();
            using (SqlConnection con = new SqlConnection(gerdataCone["Conexion"]))
            {
                if (con != null && con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                String Query = "SELECT * " +
                               "FROM CXN_PACIENTES " +
                               "WHERE Pac_TipoId = @param1 " +
                               "AND Pac_IdNum = @param2";

                using (SqlCommand Command = new SqlCommand(Query, con))
                {
                    Command.Parameters.AddWithValue("@param1", TipoId);
                    Command.Parameters.AddWithValue("@param2", NId);

                    using (SqlDataReader Reader = (Command.ExecuteReader()))
                    {
                        if (Reader.Read() == true)
                        {
                            CXN_PACIENTES DP = new CXN_PACIENTES
                            {
                                Pac_TipoId = (Reader["Pac_TipoId"] == DBNull.Value ? "" : Reader["Pac_TipoId"].ToString()),
                                Pac_IdNum = (Reader["Pac_IdNum"] == DBNull.Value ? "" : Reader["Pac_IdNum"].ToString()),
                                Pac_FechaNto = (Reader["Pac_FechaNto"] == DBNull.Value ? DateTime.Now.Date : Convert.ToDateTime(Reader["Pac_FechaNto"])),
                                Pac_PrimerN = (Reader["Pac_PrimerN"] == DBNull.Value ? "" : Reader["Pac_PrimerN"].ToString()),
                                Pac_SegundoN = (Reader["Pac_SegundoN"] == DBNull.Value ? "" : Reader["Pac_SegundoN"].ToString()),
                                Pac_PrimerA = (Reader["Pac_PrimerA"] == DBNull.Value ? "" : Reader["Pac_PrimerA"].ToString()),
                                Pac_SegundoA = (Reader["Pac_SegundoA"] == DBNull.Value ? "" : Reader["Pac_SegundoA"].ToString()),
                                Pac_Sexo = (Reader["Pac_Sexo"] == DBNull.Value ? "" : Reader["Pac_Sexo"].ToString()),
                                Pac_Telefono = (Reader["Pac_Telefono"] == DBNull.Value ? "" : Reader["Pac_Telefono"].ToString()),
                                Pac_TelefonoAux = (Reader["Pac_TelefonoAux"] == DBNull.Value ? "" : Reader["Pac_TelefonoAux"].ToString()),
                                Pac_Direccion = (Reader["Pac_Direccion"] == DBNull.Value ? "" : Reader["Pac_Direccion"].ToString()),
                                Pac_Email = (Reader["Pac_Email"] == DBNull.Value ? "" : Reader["Pac_Email"].ToString()),
                                Pac_Mun_Cod = (Reader["Pac_Mun_Cod"] == DBNull.Value ? "" : Reader["Pac_Mun_Cod"].ToString()),
                                Pac_Zona = (Reader["Pac_Zona"] == DBNull.Value ? "" : Reader["Pac_Zona"].ToString()),
                                Pac_Localidad = (Reader["Pac_Localidad"] == DBNull.Value ? "" : Reader["Pac_Localidad"].ToString()),
                                Pac_Acudiente = (Reader["Pac_Acudiente"] == DBNull.Value ? "" : Reader["Pac_Acudiente"].ToString()),
                                Pac_Parentesco = (Reader["Pac_Parentesco"] == DBNull.Value ? "" : Reader["Pac_Parentesco"].ToString()),
                                Pac_DireccionAcu = (Reader["Pac_DireccionAcu"] == DBNull.Value ? "" : Reader["Pac_DireccionAcu"].ToString()),
                                Pac_TelefonoAcu = (Reader["Pac_TelefonoAcu"] == DBNull.Value ? "" : Reader["Pac_TelefonoAcu"].ToString()),
                                Pac_CorreoAcu = (Reader["Pac_CorreoAcu"] == DBNull.Value ? "" : Reader["Pac_CorreoAcu"].ToString()),
                                Pac_Dep_Cod = (Reader["Pac_Dep_Cod"] == DBNull.Value ? "" : Reader["Pac_Dep_Cod"].ToString()),
                                Pac_Id = Convert.ToInt32(Reader["Pac_Id"]),
                                Pac_Doble = (Reader["Pac_Doble"] == DBNull.Value ? "" : Reader["Pac_Doble"].ToString()),
                                Pac_2VXS = (Reader["Pac_2VXS"] == DBNull.Value ? "" : Reader["Pac_2VXS"].ToString()),
                                Pac_Especial = (Reader["Pac_Especial"] == DBNull.Value ? "N" : Reader["Pac_Especial"].ToString()),
                                Pac_Contrato = (Reader["Pac_Contrato"] == DBNull.Value ? "" : Reader["Pac_Contrato"].ToString()),
                                Pac_Aseguradora = (Reader["Pac_Aseguradora"] == DBNull.Value ? 0 : Convert.ToInt32(Reader["Pac_Aseguradora"])),
                                Pac_Regimen = (Reader["Pac_Regimen"] == DBNull.Value ? "01" : Reader["Pac_Regimen"].ToString()),
                                Pac_FibInf = (Reader["Pac_FibInf"] == DBNull.Value ? "Incluido" : "Excluido"),
                                Pac_PaisOrigen = (Reader["Pac_PaisOrigen"] == DBNull.Value ? "" : Reader["Pac_PaisOrigen"].ToString()),
                                Pac_Categoria = (Reader["Pac_Categoria"] == DBNull.Value ? "" : Reader["Pac_Categoria"].ToString()),
                                Pac_Residencia = (Reader["Pac_Residencia"] == DBNull.Value ? "" : Reader["Pac_Residencia"].ToString()),
                                Discapacidad = (Reader["Discapacidad"] == DBNull.Value ? "" : Reader["Discapacidad"].ToString()),
                                Etnia = (Reader["Etnia"] == DBNull.Value ? "" : Reader["Etnia"].ToString()),
                                VIH = (Reader["VIH"] == DBNull.Value ? "" : Reader["VIH"].ToString()),
                                Hepatitis = (Reader["Hepatitis"] == DBNull.Value ? "" : Reader["Hepatitis"].ToString()),       
                                Pac_ECivil = (Reader["Pac_ECivil"] == DBNull.Value ? "" : Reader["Pac_ECivil"].ToString()),
                                Pac_Ocupacion = (Reader["Pac_Ocupacion"] == DBNull.Value ? "" : Reader["Pac_Ocupacion"].ToString()),
                                HoraNto = (Reader["HoraNto"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(Reader["HoraNto"])),
                                IdentidadGenero = (Reader["IdentidadGenero"] == DBNull.Value ? "04" : Reader["IdentidadGenero"].ToString())
                            };


                            return DP;
                        }
                        else
                        {
                            return null;
                        }
                    }
                }               
            }
        }
        bool IPacientes.CrearClientes(CXN_PACIENTES p)
        {
            try
            {
                var gerdataCone = Conexion.Conection();
                using (SqlConnection con = new SqlConnection(gerdataCone["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    SqlCommand cmd = new SqlCommand(@"Insert into CXN_PACIENTES (Pac_PrimerN, " + //param1
                                                                                "Pac_SegundoN, " + //param2
                                                                                "Pac_PrimerA, " + //param3
                                                                                "Pac_SegundoA, " + //param4
                                                                                "Pac_Telefono, " + //param5
                                                                                "Pac_Email, " + //param6
                                                                                "Pac_Aseguradora, " + //param7
                                                                                "Pac_TipoId, " + //param8
                                                                                "Pac_IdNum, " +
                                                                                "Pac_Categoria) " + //param16
                                 "values                  (@param1, " + // Hor_Estado
                                                          "@param2, " + // Hor_Pac_Id
                                                          "@param3, " + // Hor_Pac_Bod
                                                          "@param4, " + // Hor_Pac_Tipo_Serv
                                                          "@param5, " + // Hor_Pac_Cia
                                                          "@param6, " + // Hor_Pac_Ase
                                                          "@param7, " + // Hor_Pac_Cup
                                                          "@param8, " + // Hor_Pac_UsrGraba
                                                          "@param9, " +
                                                          "@param10)", con); // Hor_Pac_Sal

                    cmd.Parameters.AddWithValue("@param1", p.Pac_PrimerN);
                    cmd.Parameters.AddWithValue("@param2", p.Pac_SegundoN);
                    cmd.Parameters.AddWithValue("@param3", p.Pac_PrimerA);
                    cmd.Parameters.AddWithValue("@param4", p.Pac_SegundoA);
                    cmd.Parameters.AddWithValue("@param5", p.Pac_Telefono);
                    cmd.Parameters.AddWithValue("@param6", p.Pac_Email);
                    cmd.Parameters.AddWithValue("@param7", p.Pac_Aseguradora);
                    cmd.Parameters.AddWithValue("@param8", p.Pac_TipoId);
                    cmd.Parameters.AddWithValue("@param9", p.Pac_IdNum.TrimStart().TrimEnd());
                    cmd.Parameters.AddWithValue("@param10", p.Pac_Categoria);
                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
        bool IPacientes.ValidaCelular(string Val_Cel)
        {
            if (Val_Cel.Length != 10)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        bool IPacientes.ValidaEmail(string Val_Email)
        {

            String expresion;
            expresion = "\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*";

            if (Regex.IsMatch(Val_Email, expresion))
            {
                if (Regex.Replace(Val_Email, expresion, String.Empty).Length == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        CXN_PACIENTES IPacientes.LlamarPacientebyId(int pacid)
        {
            var getDatCone = Conexion.Conection();
            using (SqlConnection con = new SqlConnection(getDatCone["Conexion"]))
            {
                if (con != null && con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                String Query = "SELECT * " +
                               "FROM CXN_PACIENTES " +
                               "WHERE Pac_Id = @param1";

                using (SqlCommand Command = new SqlCommand(Query, con))
                {
                    Command.Parameters.AddWithValue("@param1", pacid);

                    using (SqlDataReader Reader = (Command.ExecuteReader()))
                    {
                        if (Reader.Read() == true)
                        {
                            CXN_PACIENTES DP = new CXN_PACIENTES
                            {
                                Pac_TipoId = (Reader["Pac_TipoId"] == DBNull.Value ? "" : Reader["Pac_TipoId"].ToString()),
                                Pac_IdNum = (Reader["Pac_IdNum"] == DBNull.Value ? "" : Reader["Pac_IdNum"].ToString()),
                                Pac_FechaNto = (Reader["Pac_FechaNto"] == DBNull.Value ? DateTime.Now.Date : Convert.ToDateTime(Reader["Pac_FechaNto"])),
                                Pac_PrimerN = (Reader["Pac_PrimerN"] == DBNull.Value ? "" : Reader["Pac_PrimerN"].ToString()),
                                Pac_SegundoN = (Reader["Pac_SegundoN"] == DBNull.Value ? "" : Reader["Pac_SegundoN"].ToString()),
                                Pac_PrimerA = (Reader["Pac_PrimerA"] == DBNull.Value ? "" : Reader["Pac_PrimerA"].ToString()),
                                Pac_SegundoA = (Reader["Pac_SegundoA"] == DBNull.Value ? "" : Reader["Pac_SegundoA"].ToString()),
                                Pac_Sexo = (Reader["Pac_Sexo"] == DBNull.Value ? "" : Reader["Pac_Sexo"].ToString()),
                                Pac_Telefono = (Reader["Pac_Telefono"] == DBNull.Value ? "" : Reader["Pac_Telefono"].ToString()),
                                Pac_TelefonoAux = (Reader["Pac_TelefonoAux"] == DBNull.Value ? "" : Reader["Pac_TelefonoAux"].ToString()),
                                Pac_Direccion = (Reader["Pac_Direccion"] == DBNull.Value ? "" : Reader["Pac_Direccion"].ToString()),
                                Pac_Email = (Reader["Pac_Email"] == DBNull.Value ? "" : Reader["Pac_Email"].ToString()),
                                Pac_Mun_Cod = (Reader["Pac_Mun_Cod"] == DBNull.Value ? "" : Reader["Pac_Mun_Cod"].ToString()),
                                Pac_Zona = (Reader["Pac_Zona"] == DBNull.Value ? "" : Reader["Pac_Zona"].ToString()),
                                Pac_Localidad = (Reader["Pac_Localidad"] == DBNull.Value ? "" : Reader["Pac_Localidad"].ToString()),
                                Pac_Acudiente = (Reader["Pac_Acudiente"] == DBNull.Value ? "" : Reader["Pac_Acudiente"].ToString()),
                                Pac_Parentesco = (Reader["Pac_Parentesco"] == DBNull.Value ? "" : Reader["Pac_Parentesco"].ToString()),
                                Pac_DireccionAcu = (Reader["Pac_DireccionAcu"] == DBNull.Value ? "" : Reader["Pac_DireccionAcu"].ToString()),
                                Pac_TelefonoAcu = (Reader["Pac_TelefonoAcu"] == DBNull.Value ? "" : Reader["Pac_TelefonoAcu"].ToString()),
                                Pac_CorreoAcu = (Reader["Pac_CorreoAcu"] == DBNull.Value ? "" : Reader["Pac_CorreoAcu"].ToString()),
                                Pac_Dep_Cod = (Reader["Pac_Dep_Cod"] == DBNull.Value ? "" : Reader["Pac_Dep_Cod"].ToString()),
                                Pac_Id = Convert.ToInt32(Reader["Pac_Id"]),
                                Pac_Doble = (Reader["Pac_Doble"] == DBNull.Value ? "" : Reader["Pac_Doble"].ToString()),
                                Pac_2VXS = (Reader["Pac_2VXS"] == DBNull.Value ? "" : Reader["Pac_2VXS"].ToString()),
                                Pac_Especial = (Reader["Pac_Especial"] == DBNull.Value ? "N" : Reader["Pac_Especial"].ToString()),
                                Pac_Contrato = (Reader["Pac_Contrato"] == DBNull.Value ? "" : Reader["Pac_Contrato"].ToString()),
                                Pac_Aseguradora = (Reader["Pac_Aseguradora"] == DBNull.Value ? 0 : Convert.ToInt32(Reader["Pac_Aseguradora"])),
                                Pac_Regimen = (Reader["Pac_Regimen"] == DBNull.Value ? "01" : Reader["Pac_Regimen"].ToString()),
                                Pac_FibInf = (Reader["Pac_FibInf"] == DBNull.Value ? "Incluido" : "Excluido"),
                                Pac_PaisOrigen = (Reader["Pac_PaisOrigen"] == DBNull.Value ? "" : Reader["Pac_PaisOrigen"].ToString()),
                                Pac_Categoria = (Reader["Pac_Categoria"] == DBNull.Value ? "" : Reader["Pac_Categoria"].ToString()),
                                Pac_Residencia = (Reader["Pac_Residencia"] == DBNull.Value ? "" : Reader["Pac_Residencia"].ToString()),
                                Pac_ECivil = (Reader["Pac_ECivil"] == DBNull.Value ? "" : Reader["Pac_ECivil"].ToString()),
                                Pac_Ocupacion = (Reader["Pac_Ocupacion"] == DBNull.Value ? "" : Reader["Pac_Ocupacion"].ToString()),
                                Discapacidad = (Reader["Discapacidad"] == DBNull.Value ? "" : Reader["Discapacidad"].ToString()),
                                Etnia = (Reader["Etnia"] == DBNull.Value ? "" : Reader["Etnia"].ToString()),
                                VIH = (Reader["VIH"] == DBNull.Value ? "" : Reader["VIH"].ToString()),
                                Hepatitis = (Reader["Hepatitis"] == DBNull.Value ? "" : Reader["Hepatitis"].ToString()),
                                HoraNto = (Reader["HoraNto"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(Reader["HoraNto"])),
                                IdentidadGenero = (Reader["IdentidadGenero"] == DBNull.Value ? "04" : Reader["IdentidadGenero"].ToString())
                            };


                            return DP;
                        }
                        else
                        {
                            return null;
                        }
                    }
                }                
            }
        }
        void IPacientes.Actualiza(CXN_PACIENTES P) 
        {
            try
            {
                var datConect = Conexion.Conection();
                using (SqlConnection con = new SqlConnection(datConect["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    SqlCommand Busqueda = new SqlCommand(@"UPDATE CXN_PACIENTES " +
                                      "SET  " +
                                      "Pac_PrimerN = @param1, " +
                                      "Pac_SegundoN = @param2, " +
                                      "Pac_PrimerA = @param3, " +
                                      "Pac_SegundoA = @param4, " +
                                      "Pac_Telefono = @param5, " +
                                      "Pac_TelefonoAux = @param6, " +
                                      "Pac_Email = @param7, " +
                                      "Pac_Direccion = @param8, " +
                                      "Pac_Localidad = @param9, " +
                                      "Pac_FechaNto = @param10, " +
                                      "Pac_Acudiente = @param11, " +
                                      "Pac_TelefonoAcu = @param12, " +
                                      "Pac_DireccionAcu = @param13, " +
                                      "Pac_Parentesco = @param14, " +
                                      "Pac_CorreoAcu = @param15, " +
                                      "Pac_TipoId = @param16, " +
                                      "Pac_ECivil = @param17, " +
                                      "Pac_Ocupacion = @param18, " +
                                      "Discapacidad = @param19, " +
                                      "Etnia = @param20, " +

                                      "HoraNto = @param21, " +
                                      "Pac_Sexo = @param22, " +
                                      "IdentidadGenero = @param23, " +
                                      "Pac_PaisOrigen = @param24, " +
                                      "Pac_Residencia = @param25, " +
                                      "Pac_Zona = @param26, " +
                                      "Pac_Aseguradora = @param27 " +

                                      "WHERE Pac_Id = @param28", con);

                    Busqueda.Parameters.AddWithValue("@param1", P.Pac_PrimerN);
                    Busqueda.Parameters.AddWithValue("@param2", P.Pac_SegundoN);
                    Busqueda.Parameters.AddWithValue("@param3", P.Pac_PrimerA);
                    Busqueda.Parameters.AddWithValue("@param4", P.Pac_SegundoA);
                    Busqueda.Parameters.AddWithValue("@param5", P.Pac_Telefono);
                    Busqueda.Parameters.AddWithValue("@param6", P.Pac_TelefonoAux);
                    Busqueda.Parameters.AddWithValue("@param7", P.Pac_Email);
                    Busqueda.Parameters.AddWithValue("@param8", P.Pac_Direccion);
                    Busqueda.Parameters.AddWithValue("@param9", P.Pac_Localidad);
                    Busqueda.Parameters.Add(new SqlParameter("@param10", SqlDbType.DateTime)).Value = Convert.ToDateTime(P.Pac_FechaNto).ToString(datConect["Format_Fecha"]);
                    Busqueda.Parameters.AddWithValue("@param11", P.Pac_Acudiente);
                    Busqueda.Parameters.AddWithValue("@param12", P.Pac_TelefonoAcu);
                    Busqueda.Parameters.AddWithValue("@param13", P.Pac_DireccionAcu);
                    Busqueda.Parameters.AddWithValue("@param14", P.Pac_Parentesco);
                    Busqueda.Parameters.AddWithValue("@param15", P.Pac_CorreoAcu);
                    Busqueda.Parameters.AddWithValue("@param16", P.Pac_TipoId);
                    Busqueda.Parameters.AddWithValue("@param17", P.Pac_ECivil);
                    Busqueda.Parameters.AddWithValue("@param18", P.Pac_Ocupacion);
                    Busqueda.Parameters.AddWithValue("@param19", P.Discapacidad);
                    Busqueda.Parameters.AddWithValue("@param20", P.Etnia);

                    Busqueda.Parameters.Add(new SqlParameter("@param21", SqlDbType.DateTime)).Value = Convert.ToDateTime(P.HoraNto);
                    Busqueda.Parameters.AddWithValue("@param22", P.Pac_Sexo);
                    Busqueda.Parameters.AddWithValue("@param23", P.IdentidadGenero);
                    Busqueda.Parameters.AddWithValue("@param24", P.Pac_PaisOrigen);
                    Busqueda.Parameters.AddWithValue("@param25", P.Pac_Residencia);
                    Busqueda.Parameters.AddWithValue("@param26", P.Pac_Zona);
                    Busqueda.Parameters.AddWithValue("@param27", P.Pac_Aseguradora);

                    Busqueda.Parameters.AddWithValue("@param28", P.Pac_Id);

                    Busqueda.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
            }
        }
        List<CXN_PACIENTES> IPacientes.LlamarPacienteDOCSimilares(string PApellido, string PNombre)
        {
            var datConect = Conexion.Conection();
            using (SqlConnection con = new SqlConnection(datConect["Conexion"]))
            {
                if (con != null && con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                String Query = "Select Pac_IdNum, Pac_TipoId, Pac_PrimerA, Pac_SegundoA, Pac_PrimerN, Pac_SegundoN " +
                               "FROM CXN_PACIENTES " +
                               "WHERE Pac_PrimerA LIKE '%" + PApellido + "%' " +
                               "AND Pac_PrimerN LIKE '%" + PNombre + "%' " +
                               "ORDER BY Pac_PrimerA ASC";
                SqlCommand Command = new SqlCommand(Query, con);
                SqlDataReader Reader = (Command.ExecuteReader());
                if (Reader.HasRows)
                {
                    List<CXN_PACIENTES> DP = new List<CXN_PACIENTES>();

                    while (Reader.Read() == true)
                    {
                        string TID = "";
                        string ID = "";
                        string PN = "";
                        string SN = "";
                        string PA = "";
                        string SA = "";

                        if (Reader["Pac_TipoId"] != DBNull.Value)
                        {
                            TID = Reader["Pac_TipoId"].ToString();
                        }


                        if (Reader["Pac_IdNum"] != DBNull.Value)
                        {
                            ID = Reader["Pac_IdNum"].ToString();
                        }


                        if (Reader["Pac_PrimerN"] != DBNull.Value)
                        {
                            PN = Reader["Pac_PrimerN"].ToString();
                        }


                        if (Reader["Pac_SegundoN"] != DBNull.Value)
                        {
                            SN = Reader["Pac_SegundoN"].ToString();
                        }


                        if (Reader["Pac_PrimerA"] != DBNull.Value)
                        {
                            PA = Reader["Pac_PrimerA"].ToString();
                        }


                        if (Reader["Pac_SegundoA"] != DBNull.Value)
                        {
                            SA = Reader["Pac_SegundoA"].ToString();
                        }

                        DP.Add(new CXN_PACIENTES
                        {
                            Pac_TipoId = TID,
                            Pac_IdNum = ID,
                            Pac_PrimerN = PA + " " + SA + " " + PN + " " + SN
                        });
                    }

                    return DP;
                }
                else
                {
                    return null;
                }
            }
        }        
        string IPacientes.Carga_Regimen(string Cod_Reg)
        {
            try
            {
                var getConect = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getConect["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    String Query = "SELECT * FROM CXN_REGIMEN WHERE Reg_Codigo = @param1";

                    using (SqlCommand Command = new SqlCommand(Query, con))
                    {
                        Command.Parameters.AddWithValue("@param1", Cod_Reg);

                        using (SqlDataReader Reader = (Command.ExecuteReader()))
                        {
                            if (Reader.Read() == true)
                            {
                                return Reader["Reg_Regimen"].ToString();
                            }
                            else
                            {
                                return "Contributivo cotizante";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return "Contributivo cotizante";
            }
        }
        List<string> IPacientes.ListaRegimen()
        {
            try
            {
                var getConect = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getConect["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    String Query = "SELECT * FROM CXN_REGIMEN ORDER BY Reg_Id ASC";

                    using (SqlCommand Command = new SqlCommand(Query, con))
                    {
                        using (SqlDataReader Reader = (Command.ExecuteReader()))
                        {
                            if (Reader.HasRows)
                            {
                                List<string> Regs = new List<string>();

                                while (Reader.Read() == true)
                                {
                                    Regs.Add(Reader["Reg_Regimen"].ToString());
                                }

                                return Regs;
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
        string IPacientes.Regimen(string Val_Reg) 
        {
            try
            {
                switch (Val_Reg)
                {
                    case "Contributivo cotizante":
                        return "01";

                    case "Contributivo beneficiario":
                        return "02";

                    case "Contributivo adicional":
                        return "03";

                    case "Subsidiado":
                        return "04";

                    case "No afiliado":
                        return "05";

                    case "Especial o Excepción cotizante":
                        return "06";

                    case "Especial o Excepción beneficiario":
                        return "07";

                    case "Personas privadas de la libertad a cargo del Fondo Nacional de Salud":
                        return "08";

                    case "Tomador / Amparado ARL":
                        return "09";

                    case "Tomador / Amparado SOAT":
                        return "10";

                    case "Tomador / Amparado Planes voluntarios de salud":
                        return "11";

                    case "Particular":
                        return "12";

                    default:
                        return "01";
                }
            }
            catch
            {
                return "01";
            }
        }
        bool IPacientes.Crea_Paciente(CXN_PACIENTES P)
        {
            try
            {
                var datConect = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(datConect["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    SqlCommand cmd = new SqlCommand(@"Insert into CXN_PACIENTES (Pac_TipoId, " + //param1
                                                                                "Pac_IdNum, " + //param2
                                                                                "Pac_PrimerN, " + //param3
                                                                                "Pac_PrimerA, " + //param4
                                                                                "Pac_SegundoN, " + //param5
                                                                                "Pac_SegundoA, " + //param6
                                                                                "Pac_FechaNto, " + //param7
                                                                                "Pac_Sexo, " + //param8
                                                                                "Pac_Telefono, " + //param9
                                                                                "Pac_TelefonoAux, " + //param10
                                                                                "Pac_Direccion, " + //param11
                                                                                "Pac_Email, " + //param12
                                                                                "Pac_Mun_Cod, " + //param13
                                                                                "Pac_Dep_Cod, " + //param14
                                                                                "Pac_Zona, " + //param15
                                                                                "Pac_Localidad, " + //param16
                                                                                "Pac_Aseguradora, " + //param17
                                                                                "Pac_Acudiente, " + //param18
                                                                                "Pac_Parentesco, " + //param19
                                                                                "Pac_DireccionAcu, " + //param20
                                                                                "Pac_TelefonoAcu, " + //param21
                                                                                "Pac_CorreoAcu, " + //param22
                                                                                "Pac_UsrGraba, " + //param23
                                                                                "Pac_Regimen, " + //param24
                                                                                "Pac_Contrato, " + //param25
                                                                                "Pac_Categoria, " + //param26
                                                                                "Pac_Ocupacion, " + //param27
                                                                                "Discapacidad, " + //param28
                                                                                "Etnia) " + //param29
                                 "values                  (@param1, " + // Hor_Estado
                                                          "@param2, " + // Hor_Pac_Id
                                                          "@param3, " + // Hor_Pac_Bod
                                                          "@param4, " + // Hor_Pac_Tipo_Serv
                                                          "@param5, " + // Hor_Pac_Cia
                                                          "@param6, " + // Hor_Pac_Ase
                                                          "@param7, " + // Hor_Pac_Cup
                                                          "@param8, " + // Hor_Pac_UsrGraba
                                                          "@param9, " + // Hor_Imp_Age
                                                          "@param10, " + // Hor_Pac_Fecha
                                                          "@param11, " + // Hor_Pac_Fecha_Cita
                                                          "@param12, " + // Hor_Pac_Hora
                                                          "@param13, " + // Hor_Pac_Id_Hora
                                                          "@param14, " + // Hor_Pac_Hora_Cita
                                                          "@param15, " + // Hor_Observacion
                                                          "@param16, " + // Hor_Observacion
                                                          "@param17, " + // Hor_Observacion
                                                          "@param18, " + // Hor_Observacion
                                                          "@param19, " + // Hor_Observacion
                                                          "@param20, " + // Hor_Observacion
                                                          "@param21, " + // Hor_Observacion
                                                          "@param22, " + // Hor_Observacion
                                                          "@param23, " + // Hor_Observacion
                                                          "@param24, " +
                                                          "@param25, " +
                                                          "@param26, " +
                                                          "@param27, " +
                                                          "@param28, " +
                                                          "@param29)", con); // Hor_Pac_Sal

                    cmd.Parameters.AddWithValue("@param1", P.Pac_TipoId);
                    cmd.Parameters.AddWithValue("@param2", P.Pac_IdNum.TrimStart().TrimEnd());
                    cmd.Parameters.AddWithValue("@param3", P.Pac_PrimerN);
                    cmd.Parameters.AddWithValue("@param4", P.Pac_PrimerA);
                    cmd.Parameters.AddWithValue("@param5", P.Pac_SegundoN);
                    cmd.Parameters.AddWithValue("@param6", P.Pac_SegundoA);
                    cmd.Parameters.Add(new SqlParameter("@param7", SqlDbType.DateTime)).Value = P.Pac_FechaNto;
                    cmd.Parameters.AddWithValue("@param8", P.Pac_Sexo);
                    cmd.Parameters.AddWithValue("@param9", P.Pac_Telefono);
                    cmd.Parameters.AddWithValue("@param10", P.Pac_TelefonoAux);
                    cmd.Parameters.AddWithValue("@param11", P.Pac_Direccion);
                    cmd.Parameters.AddWithValue("@param12", P.Pac_Email);
                    cmd.Parameters.AddWithValue("@param13", P.Pac_Mun_Cod);
                    cmd.Parameters.AddWithValue("@param14", P.Pac_Dep_Cod);
                    cmd.Parameters.AddWithValue("@param15", P.Pac_Zona);
                    cmd.Parameters.AddWithValue("@param16", P.Pac_Localidad);
                    cmd.Parameters.AddWithValue("@param17", P.Pac_Aseguradora);
                    cmd.Parameters.AddWithValue("@param18", P.Pac_Acudiente);
                    cmd.Parameters.AddWithValue("@param19", P.Pac_Parentesco);
                    cmd.Parameters.AddWithValue("@param20", P.Pac_DireccionAcu);
                    cmd.Parameters.AddWithValue("@param21", P.Pac_TelefonoAcu);
                    cmd.Parameters.AddWithValue("@param22", P.Pac_CorreoAcu);
                    cmd.Parameters.AddWithValue("@param23", P.Pac_UsrGraba);
                    cmd.Parameters.AddWithValue("@param24", P.Pac_Regimen);
                    cmd.Parameters.AddWithValue("@param25", P.Pac_Contrato);
                    cmd.Parameters.AddWithValue("@param26", P.Pac_Categoria);
                    cmd.Parameters.AddWithValue("@param27", P.Pac_Ocupacion);
                    cmd.Parameters.AddWithValue("@param28", P.Discapacidad);
                    cmd.Parameters.AddWithValue("@param29", P.Etnia);
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
        bool IPacientes.Existente(string Doc, int pacid)
        {
            try
            {
                var datConect = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(datConect["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    String Query = "SELECT *  " +
                                   "FROM CXN_PACIENTES " +
                                   "WHERE Pac_IdNum = '" + Doc + "' " +
                                   "AND Pac_Id <> '" + pacid + "'";
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
                return true;
            }
        }
        bool IPacientes.Edita_Paciente(CXN_PACIENTES p)
        {
            try
            {
                var datConect = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(datConect["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    string Busqueda = (@"UPDATE CXN_PACIENTES " +
                                          "SET Pac_TipoId = @Pac_TipoId, " +
                                          "Pac_IdNum = @Pac_IdNum, " +
                                          "Pac_PrimerN = @Pac_PrimerN, " +
                                          "Pac_SegundoN = @Pac_SegundoN, " +
                                          "Pac_PrimerA = @Pac_PrimerA, " +
                                          "Pac_SegundoA = @Pac_SegundoA, " +
                                          "Pac_FechaNto = @Pac_FechaNto, " +
                                          "Pac_Sexo = @Pac_Sexo, " +
                                          "Pac_Telefono = @Pac_Telefono, " +
                                          "Pac_TelefonoAux = @Pac_TelefonoAux, " +
                                          "Pac_Direccion = @Pac_Direccion, " +
                                          "Pac_Email = @Pac_Email, " +
                                          "Pac_Mun_Cod = @Pac_Mun_Cod, " +
                                          "Pac_Zona = @Pac_Zona, " +
                                          "Pac_Localidad = @Pac_Localidad, " +
                                          "Pac_Aseguradora = @Pac_Aseguradora, " +
                                          "Pac_Acudiente = @Pac_Acudiente, " +
                                          "Pac_Parentesco = @Pac_Parentesco, " +
                                          "Pac_DireccionAcu = @Pac_DireccionAcu, " +
                                          "Pac_TelefonoAcu = @Pac_TelefonoAcu, " +
                                          "Pac_CorreoAcu = @Pac_CorreoAcu, " +
                                          "Pac_Dep_Cod = @Pac_Dep_Cod, " +
                                          "Pac_UsrGraba = @Pac_UsrGraba, " +
                                          "Pac_Regimen = @Pac_Regimen, " +
                                          "Pac_Contrato = @Pac_Contrato, " +
                                          "Pac_Categoria = @Pac_Categoria, " +
                                          "Pac_Ocupacion = @Pac_Ocupacion, " +
                                          "Discapacidad = @Discapacidad, " +
                                          "Etnia = @Etnia " +
                                          "WHERE Pac_Id = @pac_id");
                    SqlCommand Accion = new SqlCommand(Busqueda, con);

                    Accion.Parameters.Add(new SqlParameter("@Pac_TipoId", p.Pac_TipoId));
                    Accion.Parameters.Add(new SqlParameter("@Pac_IdNum", p.Pac_IdNum.TrimStart().TrimEnd()));
                    Accion.Parameters.Add(new SqlParameter("@Pac_PrimerN", p.Pac_PrimerN));
                    Accion.Parameters.Add(new SqlParameter("@Pac_SegundoN", p.Pac_SegundoN));
                    Accion.Parameters.Add(new SqlParameter("@Pac_PrimerA", p.Pac_PrimerA));
                    Accion.Parameters.Add(new SqlParameter("@Pac_SegundoA", p.Pac_SegundoA));
                    Accion.Parameters.Add(new SqlParameter("@Pac_FechaNto", SqlDbType.DateTime)).Value = p.Pac_FechaNto;
                    Accion.Parameters.Add(new SqlParameter("@Pac_Sexo", p.Pac_Sexo));
                    Accion.Parameters.Add(new SqlParameter("@Pac_Telefono", p.Pac_Telefono));
                    Accion.Parameters.Add(new SqlParameter("@Pac_TelefonoAux", p.Pac_TelefonoAux));
                    Accion.Parameters.Add(new SqlParameter("@Pac_Direccion", p.Pac_Direccion));
                    Accion.Parameters.Add(new SqlParameter("@Pac_Email", p.Pac_Email));
                    Accion.Parameters.Add(new SqlParameter("@Pac_Mun_Cod", p.Pac_Mun_Cod));
                    Accion.Parameters.Add(new SqlParameter("@Pac_Zona", p.Pac_Zona));
                    Accion.Parameters.Add(new SqlParameter("@Pac_Localidad", p.Pac_Localidad));
                    Accion.Parameters.Add(new SqlParameter("@Pac_Aseguradora", p.Pac_Aseguradora));
                    Accion.Parameters.Add(new SqlParameter("@Pac_Acudiente", p.Pac_Acudiente));
                    Accion.Parameters.Add(new SqlParameter("@Pac_Parentesco", p.Pac_Parentesco));
                    Accion.Parameters.Add(new SqlParameter("@Pac_DireccionAcu", p.Pac_DireccionAcu));
                    Accion.Parameters.Add(new SqlParameter("@Pac_TelefonoAcu", p.Pac_TelefonoAcu));
                    Accion.Parameters.Add(new SqlParameter("@Pac_CorreoAcu", p.Pac_CorreoAcu));
                    Accion.Parameters.Add(new SqlParameter("@Pac_Dep_Cod", p.Pac_Dep_Cod));
                    Accion.Parameters.Add(new SqlParameter("@Pac_UsrGraba", p.Pac_UsrGraba));
                    Accion.Parameters.Add(new SqlParameter("@Pac_Regimen", p.Pac_Regimen));
                    Accion.Parameters.Add(new SqlParameter("@Pac_Contrato", p.Pac_Contrato));
                    Accion.Parameters.Add(new SqlParameter("@Pac_Categoria", p.Pac_Categoria));
                    Accion.Parameters.Add(new SqlParameter("@Pac_Ocupacion", p.Pac_Ocupacion));
                    Accion.Parameters.Add(new SqlParameter("@Discapacidad", p.Discapacidad));
                    Accion.Parameters.Add(new SqlParameter("@Etnia", p.Etnia));

                    Accion.Parameters.Add(new SqlParameter("@pac_id", p.Pac_Id));

                    int s = Accion.ExecuteNonQuery();
                    return s > 0 ? true : false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        bool IPacientes.ActualizarPaciente(CXN_PACIENTES pacientes)
        {
            try
            {
                var getConecti = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getConecti["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    string Busqueda = (@"UPDATE CXN_PACIENTES " +
                                          "SET Pac_PrimerN = @P1, " +
                                          "Pac_SegundoN = @P2, " +
                                          "Pac_PrimerA = @P3, " +
                                          "Pac_SegundoA = @P4, " +
                                          "Pac_Aseguradora = @P5, " +
                                          "Pac_Telefono = @P6, " +
                                          "Pac_TelefonoAux = @P7, " +
                                          "Pac_Email = @P8, " +
                                          "Pac_IdNum = @P9, " +
                                          "Pac_TipoId = @P10, " +
                                          "Pac_Direccion = @P11, " +
                                          "Pac_FechaNto = @P12, " +
                                          "Pac_Regimen = @P13, " +
                                          "Pac_Doble = @P14, " +
                                          "Pac_2VXS = @P15, " +
                                          "Pac_Especial = @P16, " +
                                          "Pac_Categoria = @P17, " +
                                          "Pac_Sexo = @P18 " +
                                          "WHERE Pac_Id = '" + pacientes.Pac_Id + "'");
                    SqlCommand Accion = new SqlCommand(Busqueda, con);

                    Accion.Parameters.Add(new SqlParameter("@P1", pacientes.Pac_PrimerN));
                    Accion.Parameters.Add(new SqlParameter("@P2", pacientes.Pac_SegundoN));
                    Accion.Parameters.Add(new SqlParameter("@P3", pacientes.Pac_PrimerA));
                    Accion.Parameters.Add(new SqlParameter("@P4", pacientes.Pac_SegundoA));
                    Accion.Parameters.Add(new SqlParameter("@P5", pacientes.Pac_Aseguradora));
                    Accion.Parameters.Add(new SqlParameter("@P6", pacientes.Pac_Telefono));
                    Accion.Parameters.Add(new SqlParameter("@P7", pacientes.Pac_TelefonoAux));
                    Accion.Parameters.Add(new SqlParameter("@P8", pacientes.Pac_Email));
                    Accion.Parameters.Add(new SqlParameter("@P9", pacientes.Pac_IdNum.TrimStart().TrimEnd()));
                    Accion.Parameters.Add(new SqlParameter("@P10", pacientes.Pac_TipoId));
                    Accion.Parameters.Add(new SqlParameter("@P11", pacientes.Pac_Direccion));
                    Accion.Parameters.Add(new SqlParameter("@P12", SqlDbType.DateTime)).Value = pacientes.Pac_FechaNto;
                    Accion.Parameters.Add(new SqlParameter("@P13", pacientes.Pac_Regimen));
                    Accion.Parameters.Add(new SqlParameter("@P14", pacientes.Pac_Doble));
                    Accion.Parameters.Add(new SqlParameter("@P15", pacientes.Pac_2VXS));
                    Accion.Parameters.Add(new SqlParameter("@P16", pacientes.Pac_Especial));
                    Accion.Parameters.Add(new SqlParameter("@P17", pacientes.Pac_Categoria));
                    Accion.Parameters.Add(new SqlParameter("@P18", pacientes.Pac_Sexo));
                    Accion.ExecuteNonQuery();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
        bool IPacientes.ActualizarCelular(string Celular, int Admision)
        {
            try
            {
                var getConecti = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getConecti["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    string Busqueda = "UPDATE CXN_PACIENTES " +
                                      "SET Pac_Telefono = '" + Celular + "' " +
                                      "WHERE Pac_Id = '" + Admision + "'";
                    SqlCommand Accion = new SqlCommand(Busqueda, con);
                    int Guarda;
                    Guarda = Accion.ExecuteNonQuery();
                    return true;
                }
            }
            catch
            {
                return true;
            }
        }
        void IPacientes.Actualiza_Email(int Paciente, string Email)
        {
            var getConecti = Conexion.Conection();

            using (SqlConnection con = new SqlConnection(getConecti["Conexion"]))
            {
                if (con != null && con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                string Busqueda = "UPDATE CXN_PACIENTES " +
                                  "SET Pac_Email = '" + Email + "' " +
                                  "WHERE Pac_Id = '" + Paciente + "'";
                SqlCommand Accion = new SqlCommand(Busqueda, con);
                int Guarda;
                Guarda = Accion.ExecuteNonQuery();
            }
        }
        void IPacientes.Actualiza_Pac(CXN_PACIENTES P)
        {
            var getConect = Conexion.Conection();

            using (SqlConnection con = new SqlConnection(getConect["Conexion"]))
            {
                if (con != null && con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                SqlCommand Busqueda = new SqlCommand(@"UPDATE CXN_PACIENTES " +
                                      "SET  " +
                                      "Pac_Telefono = @param1, " +
                                      "Pac_TelefonoAux = @param2, " +
                                      "Pac_PrimerN = @param3, " +
                                      "Pac_SegundoN = @param4, " +
                                      "Pac_PrimerA = @param5, " +
                                      "Pac_SegundoA = @param6, " +
                                      "Pac_Email = @param7, " +
                                      "Pac_Regimen = @param8, " +
                                      "Pac_Sexo = @param9, " +
                                      "Pac_Dep_Cod = @param10, " +
                                      "Pac_Mun_Cod = @param11, " +
                                      "Pac_Aseguradora = @param12, " +
                                      "Pac_Zona = @param13, " +
                                      "Pac_FechaNto = @param14, " +
                                      "Pac_TipoId = @param15, " +
                                      "Pac_Contrato = @param16, " +
                                      "Pac_PaisOrigen = @param17, " +
                                      "Pac_Residencia = @param18, " +
                                      "Pac_Categoria = @param19, " +
                                      "Pac_ECivil = @param20, " +
                                      "Pac_Acudiente = @param21, " +
                                      "Pac_CorreoAcu = @param22, " +
                                      "Pac_DireccionAcu = @param23, " +
                                      "Pac_Parentesco = @param24, " +
                                      "Pac_TelefonoAcu = @param25  " +
                                      "WHERE Pac_Id = @param26", con);

                Busqueda.Parameters.AddWithValue("@param1", P.Pac_Telefono);
                Busqueda.Parameters.AddWithValue("@param2", P.Pac_TelefonoAux);
                Busqueda.Parameters.AddWithValue("@param3", P.Pac_PrimerN);
                Busqueda.Parameters.AddWithValue("@param4", P.Pac_SegundoN);
                Busqueda.Parameters.AddWithValue("@param5", P.Pac_PrimerA);
                Busqueda.Parameters.AddWithValue("@param6", P.Pac_SegundoA);
                Busqueda.Parameters.AddWithValue("@param7", P.Pac_Email);
                Busqueda.Parameters.AddWithValue("@param8", P.Pac_Regimen);
                Busqueda.Parameters.AddWithValue("@param9", P.Pac_Sexo);
                Busqueda.Parameters.AddWithValue("@param10", P.Pac_Dep_Cod);
                Busqueda.Parameters.AddWithValue("@param11", P.Pac_Mun_Cod);
                Busqueda.Parameters.AddWithValue("@param12", P.Pac_Aseguradora);
                Busqueda.Parameters.AddWithValue("@param13", P.Pac_Zona);
                Busqueda.Parameters.Add(new SqlParameter("@param14", SqlDbType.DateTime)).Value = Convert.ToDateTime(P.Pac_FechaNto).ToString(getConect["Format_Fecha"]);
                Busqueda.Parameters.AddWithValue("@param15", P.Pac_TipoId);
                Busqueda.Parameters.AddWithValue("@param16", P.Pac_Contrato);
                Busqueda.Parameters.AddWithValue("@param17", P.Pac_PaisOrigen);
                Busqueda.Parameters.AddWithValue("@param18", P.Pac_Residencia);
                Busqueda.Parameters.AddWithValue("@param19", P.Pac_Categoria);
                Busqueda.Parameters.AddWithValue("@param20", P.Pac_ECivil);
                Busqueda.Parameters.AddWithValue("@param21", P.Pac_Acudiente);
                Busqueda.Parameters.AddWithValue("@param22", P.Pac_CorreoAcu);
                Busqueda.Parameters.AddWithValue("@param23", P.Pac_DireccionAcu);
                Busqueda.Parameters.AddWithValue("@param24", P.Pac_Parentesco);
                Busqueda.Parameters.AddWithValue("@param25", P.Pac_TelefonoAcu);
                Busqueda.Parameters.AddWithValue("@param26", P.Pac_Id);
                Busqueda.ExecuteNonQuery();               
            }
        }
        void IPacientes.Actualiza_Pac2(CXN_PACIENTES P)
        {
            try
            {
                var getConect = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getConect["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    SqlCommand Busqueda = new SqlCommand(@"UPDATE CXN_PACIENTES " +
                                          "SET  " +
                                          "Pac_Telefono = @param1, " +
                                          "Pac_TelefonoAux = @param2, " +
                                          "Pac_PrimerN = @param3, " +
                                          "Pac_SegundoN = @param4, " +
                                          "Pac_PrimerA = @param5, " +
                                          "Pac_SegundoA = @param6, " +
                                          "Pac_Email = @param7, " +
                                          "Pac_Regimen = @param8, " +
                                          "Pac_Sexo = @param9, " +
                                          "Pac_Dep_Cod = @param10, " +
                                          "Pac_Mun_Cod = @param11, " +
                                          "Pac_Aseguradora = @param12, " +
                                          "Pac_Zona = @param13, " +
                                          "Pac_FechaNto = @param14, " +
                                          "Pac_TipoId = @param15, " +
                                          "Pac_Contrato = @param16, " +
                                          "Pac_PaisOrigen = @param17, " +
                                          "Pac_Residencia = @param18, " +
                                          "Pac_Categoria = @param19, " +
                                          "Pac_ECivil = @param20  " +
                                          "WHERE Pac_Id = @param26", con);

                    Busqueda.Parameters.AddWithValue("@param1", P.Pac_Telefono);
                    Busqueda.Parameters.AddWithValue("@param2", P.Pac_TelefonoAux);
                    Busqueda.Parameters.AddWithValue("@param3", P.Pac_PrimerN);
                    Busqueda.Parameters.AddWithValue("@param4", P.Pac_SegundoN);
                    Busqueda.Parameters.AddWithValue("@param5", P.Pac_PrimerA);
                    Busqueda.Parameters.AddWithValue("@param6", P.Pac_SegundoA);
                    Busqueda.Parameters.AddWithValue("@param7", P.Pac_Email);
                    Busqueda.Parameters.AddWithValue("@param8", P.Pac_Regimen);
                    Busqueda.Parameters.AddWithValue("@param9", P.Pac_Sexo);
                    Busqueda.Parameters.AddWithValue("@param10", P.Pac_Dep_Cod);
                    Busqueda.Parameters.AddWithValue("@param11", P.Pac_Mun_Cod);
                    Busqueda.Parameters.AddWithValue("@param12", P.Pac_Aseguradora);
                    Busqueda.Parameters.AddWithValue("@param13", P.Pac_Zona);
                    Busqueda.Parameters.Add(new SqlParameter("@param14", SqlDbType.DateTime)).Value = Convert.ToDateTime(P.Pac_FechaNto).ToString(getConect["Format_Fecha"]);
                    Busqueda.Parameters.AddWithValue("@param15", P.Pac_TipoId);
                    Busqueda.Parameters.AddWithValue("@param16", P.Pac_Contrato);
                    Busqueda.Parameters.AddWithValue("@param17", P.Pac_PaisOrigen);
                    Busqueda.Parameters.AddWithValue("@param18", P.Pac_Residencia);
                    Busqueda.Parameters.AddWithValue("@param19", P.Pac_Categoria);
                    Busqueda.Parameters.AddWithValue("@param20", P.Pac_ECivil);
                    Busqueda.Parameters.AddWithValue("@param26", P.Pac_Id);
                    Busqueda.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }            
        }
        void IPacientes.Actualiza_Pac3(CXN_PACIENTES P)
        {
            var getConect = Conexion.Conection();

            using (SqlConnection con = new SqlConnection(getConect["Conexion"]))
            {
                if (con != null && con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                SqlCommand Busqueda = new SqlCommand(@"UPDATE CXN_PACIENTES " +
                                      "SET  " +
                                      "Pac_Acudiente = @param1, " +
                                      "Pac_Parentesco = @param2, " +
                                      "Pac_DireccionAcu = @param3, " +
                                      "Pac_TelefonoAcu = @param4, " +
                                      "Pac_CorreoAcu = @param5, " +
                                      "Discapacidad = @param6, " +
                                      "Pac_Ocupacion = @param7  " +
                                      "WHERE Pac_Id = @param8", con);

                Busqueda.Parameters.AddWithValue("@param1", P.Pac_Acudiente);
                Busqueda.Parameters.AddWithValue("@param2", P.Pac_Parentesco);
                Busqueda.Parameters.AddWithValue("@param3", P.Pac_DireccionAcu);
                Busqueda.Parameters.AddWithValue("@param4", P.Pac_TelefonoAcu);
                Busqueda.Parameters.AddWithValue("@param5", P.Pac_CorreoAcu);
                Busqueda.Parameters.AddWithValue("@param6", P.Discapacidad);
                Busqueda.Parameters.AddWithValue("@param7", P.Pac_Ocupacion);
                Busqueda.Parameters.AddWithValue("@param8", P.Pac_Id);

                Busqueda.ExecuteNonQuery();
            }
        }
        void IPacientes.Rpt_Atenciones2(DateTime Desde, DateTime Hasta)
        {
            var getConect = Conexion.Conection();

            using (SqlConnection con = new SqlConnection(getConect["Conexion"]))
            {
                if (con != null && con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                DateTime Hoy = DateTime.Now;

                FileStream Query = new FileStream("C:/Cxn/Reportes/Informe_" + Convert.ToDateTime(Hoy).ToString("dd-MM-yyyy") + ".xls", FileMode.Append, FileAccess.Write);
                StreamWriter Escriba = new StreamWriter(Query);

                SqlCommand comando = new SqlCommand("SELECT H.Hor_Imp_Age, H.Hor_Pac_Fecha_Cita, E.Est_Admision, E.Est_Edad, E.Est_Patologia, E.Est_Localizacion, " +
                                                    "E.Est_Evolucion, E.Est_Infectado, E.Est_Ingreso, E.Est_Alta, E.Est_Usuario, A.Ase_Descripcion " +
                                                    "FROM  CXN_ESTADISTICAS E " +
                                                    "INNER JOIN CXN_HORARIO H ON E.Est_Admision = H.Hor_Id " +
                                                    "INNER JOIN CXN_ASEGURADORA A ON H.Hor_Pac_Ase = A.Ase_Identificador " +
                                                    "WHERE H.Hor_Pac_Fecha_Cita BETWEEN '" + Convert.ToDateTime(Desde).ToString(getConect["Format_Fecha"]) + "' AND '" + Convert.ToDateTime(Hasta).ToString(getConect["Format_Fecha"]) + "'", con);

                SqlDataReader leer;
                leer = comando.ExecuteReader();

                Escriba.Write("PACIENTE" + "|" + "FECHA DE ATENCION" + "|" + "ADMISION" + "|" + "EDAD" + "|" + "PATOLOGIA" + "|" + "LOCALIZACION" + "|" + "EVOLUCION" + "|" + "INFECTADO" +
                              "|" + "INGRESO" + "|" + "ALTA" + "|" + "PROFESIONAL" + "|" + "ASEGURADORA");
                Escriba.WriteLine();
                Escriba.Flush();

                while (leer.Read())
                {
                    byte[] utf8Bytes = System.Text.Encoding.UTF8.GetBytes(leer["Ase_Descripcion"].ToString());
                    byte[] utf8Bytes2 = System.Text.Encoding.UTF8.GetBytes(leer["Hor_Imp_Age"].ToString());

                    Escriba.Write(System.Text.Encoding.UTF8.GetString(utf8Bytes2) + "|");
                    Escriba.Write(Convert.ToDateTime(leer["Hor_Pac_Fecha_Cita"].ToString()).ToString(getConect["Format_Fecha"]) + "|");
                    Escriba.Write(leer["Est_Admision"].ToString().ToString() + "|");
                    Escriba.Write(leer["Est_Edad"].ToString() + "|");
                    Escriba.Write(leer["Est_Patologia"].ToString() + "|");
                    Escriba.Write(leer["Est_Localizacion"].ToString() + "|");
                    Escriba.Write(leer["Est_Evolucion"].ToString() + "|");
                    Escriba.Write(leer["Est_Infectado"].ToString() + "|");
                    Escriba.Write(leer["Est_Ingreso"].ToString() + "|");
                    Escriba.Write(leer["Est_Alta"].ToString() + "|");
                    Escriba.Write(leer["Est_Usuario"].ToString() + "|");
                    Escriba.Write(System.Text.Encoding.UTF8.GetString(utf8Bytes));
                    Escriba.WriteLine();
                    Escriba.Flush();
                }
                Escriba.Close();
            }
        }
        List<string> IPacientes.ListaDocs()
        {
            try
            {
                var getConect = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getConect["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    String Query = "SELECT * FROM CXN_DOCUMENTOS ORDER BY Tipo_Id ASC";

                    using (SqlCommand Command = new SqlCommand(Query, con))
                    {
                        using (SqlDataReader Reader = (Command.ExecuteReader()))
                        {
                            if (Reader.HasRows)
                            {
                                List<string> L = new List<string>();

                                while (Reader.Read() == true)
                                {
                                    L.Add(Reader["Tipo_Documento"].ToString());
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
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return null;
            }
        }
        void IPacientes.VariasHeridas(int IdPac, string Heridas)
        {
            try
            {
                var getConect = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getConect["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }
                    string Busqueda = "UPDATE CXN_PACIENTES " +
                                      "SET Pac_Doble = '" + Heridas + "' " +
                                      "WHERE Pac_Id = '" + IdPac + "'";
                    SqlCommand Accion = new SqlCommand(Busqueda, con);
                    int Guarda;
                    Guarda = Accion.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
            }
        }
        void IPacientes.VariosDias(int IdPac, string Heridas)
        {
            try
            {
                var getConect = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getConect["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }
                    string Busqueda = "UPDATE CXN_PACIENTES " +
                                      "SET Pac_2VXS = '" + Heridas + "' " +
                                      "WHERE Pac_Id = '" + IdPac + "'";
                    SqlCommand Accion = new SqlCommand(Busqueda, con);
                    int Guarda;
                    Guarda = Accion.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
            }
        }
        void IPacientes.PacEspecial(int IdPac, string Especial)
        {
            try
            {
                var getConect = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getConect["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }
                    string Busqueda = "UPDATE CXN_PACIENTES " +
                                      "SET Pac_Especial = '" + Especial + "' " +
                                      "WHERE Pac_Id = '" + IdPac + "'";
                    SqlCommand Accion = new SqlCommand(Busqueda, con);
                    int Guarda;
                    Guarda = Accion.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
            }
        }
        void IPacientes.SexAndDate(int IdPac, string Sex, DateTime Date)
        {
            try
            {
                var getConect = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getConect["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }
                    string Busqueda = "UPDATE CXN_PACIENTES " +
                                      "SET Pac_Sexo = '" + Sex + "', " +
                                      "Pac_FechaNto = '" + Convert.ToDateTime(Date).ToString(Conexion.ConectionDictionary["Format_Fecha"]) + "' " +
                                      "WHERE Pac_Id = '" + IdPac + "'";
                    SqlCommand Accion = new SqlCommand(Busqueda, con);
                    int Guarda;
                    Guarda = Accion.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
            }
        }
        bool IPacientes.UpdatePacFibro(int Paciente, string Estado)
        {
            try
            {
                var getConect = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getConect["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    string _est = null;

                    if (Estado == "Incluido")
                    {
                        _est = "E";
                    }
                    else if (Estado == "Excluido")
                    {
                        string Busqueda2 = "UPDATE CXN_PACIENTES " +
                                      "SET Pac_FibInf = NULL " +
                                      "WHERE Pac_Id = '" + Paciente + "'";
                        SqlCommand Accion2 = new SqlCommand(Busqueda2, con);
                        int Guarda2;
                        Guarda2 = Accion2.ExecuteNonQuery();
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                    string Busqueda = "UPDATE CXN_PACIENTES " +
                                      "SET Pac_FibInf = '" + _est + "' " +
                                      "WHERE Pac_Id = '" + Paciente + "'";
                    SqlCommand Accion = new SqlCommand(Busqueda, con);
                    int Guarda;
                    Guarda = Accion.ExecuteNonQuery();
                    return true;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return false;
            }
        }
        void IPacientes.RptCancelaciones(DateTime Desde, DateTime Hasta)
        {
            try
            {
                var getConect = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getConect["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    DateTime Hoy = DateTime.Now;

                    FileStream Query = new FileStream("C:/Cxn/Reportes/Cancelaciones_" + Convert.ToDateTime(Hoy).ToString("dd-MM-yyyy") + ".xls", FileMode.Append, FileAccess.Write);
                    StreamWriter Escriba = new StreamWriter(Query);

                    SqlCommand comando = new SqlCommand("SELECT H.Hor_Imp_Age, H.Hor_Pac_Fecha_Cita, H.Hor_Id, H.Hor_Usr_Cancela, H.Hor_Pac_MCancela, H.Hor_Pac_RCancela, " +
                                                        "C.Con_Nombre, A.Ase_Descripcion " +
                                                        "FROM  CXN_HORARIO H " +
                                                        "INNER JOIN CXN_CONVENIOS C ON H.Hor_Pac_Cup = C.Con_Id_Serv " +
                                                        "INNER JOIN CXN_ASEGURADORA A ON H.Hor_Pac_Ase = A.Ase_Identificador " +
                                                        "WHERE H.Hor_Pac_Fecha_Cita BETWEEN '" + Convert.ToDateTime(Desde).ToString(getConect["Format_Fecha"]) + "' AND '" + Convert.ToDateTime(Hasta).ToString(getConect["Format_Fecha"]) + "' " +
                                                        "AND H.Hor_Pac_Ase = C.Con_Aseguradora " +
                                                        "AND H.Hor_Estado = 'C'", con);

                    SqlDataReader leer;
                    leer = comando.ExecuteReader();

                    Escriba.Write("PACIENTE" + "|" + "FECHA DE CITA" + "|" + "ADMISION" + "|" + "USUARIO QUE CANCELA" + "|" + "CAPITULO CANCELACION" + "|" + "SUBCAPITULO CANCELACION" + "|" + "SERVICIO" + "|" + "ASEGURADORA");
                    Escriba.WriteLine();
                    Escriba.Flush();

                    while (leer.Read())
                    {
                        byte[] utf8Bytes = System.Text.Encoding.UTF8.GetBytes(leer["Ase_Descripcion"].ToString());
                        byte[] utf8Bytes2 = System.Text.Encoding.UTF8.GetBytes(leer["Hor_Imp_Age"].ToString());

                        Escriba.Write(System.Text.Encoding.UTF8.GetString(utf8Bytes2) + "|");
                        Escriba.Write(Convert.ToDateTime(leer["Hor_Pac_Fecha_Cita"].ToString()).ToString(getConect["Format_Fecha"]) + "|");
                        Escriba.Write(leer["Hor_Id"].ToString().ToString() + "|");
                        Escriba.Write(leer["Hor_Usr_Cancela"].ToString() + "|");
                        Escriba.Write(leer["Hor_Pac_MCancela"].ToString() + "|");
                        Escriba.Write(leer["Hor_Pac_RCancela"].ToString() + "|");
                        Escriba.Write(leer["Con_Nombre"].ToString() + "|");
                        Escriba.Write(System.Text.Encoding.UTF8.GetString(utf8Bytes));
                        Escriba.WriteLine();
                        Escriba.Flush();
                    }

                    Escriba.Close();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
            }           
        }
        List<string> IPacientes.getListPaises()
        {
            var getConect = Conexion.Conection();

            using (SqlConnection con = new SqlConnection(getConect["Conexion"]))
            {
                if (con != null && con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                String Query = "SELECT Pais_Nombre " +
                               "FROM CXN_PAIS " +
                               "ORDER BY Pais_Nombre ASC";
                SqlCommand Command = new SqlCommand(Query, con);
                SqlDataReader Reader = (Command.ExecuteReader());
                if (Reader.HasRows)
                {
                    List<string> P = new List<string>();

                    while (Reader.Read() == true)
                    {
                        P.Add(Reader["Pais_Nombre"].ToString());
                    }

                    return P;
                }
                else
                {
                    return null;
                }
            }
        }
        string IPacientes.getCodePais(string Pais)
        {
            var getConect = Conexion.Conection();

            using (SqlConnection con = new SqlConnection(getConect["Conexion"]))
            {
                if (con != null && con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                String Query = "SELECT Pais_Cod " +
                               "FROM CXN_PAIS " +
                               "WHERE Pais_Nombre = '" + Pais + "'";
                SqlCommand Command = new SqlCommand(Query, con);
                SqlDataReader Reader = (Command.ExecuteReader());
                if (Reader.Read() == true)
                {
                    return Reader["Pais_cod"].ToString();
                }
                else
                {
                    return "";
                }
            }
        }
        string IPacientes.getNamePais(string Code)
        {
            var getConect = Conexion.Conection();

            using (SqlConnection con = new SqlConnection(getConect["Conexion"]))
            {
                if (con != null && con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                String Query = "SELECT Pais_Nombre " +
                               "FROM CXN_PAIS " +
                               "WHERE Pais_Cod = '" + Code + "'";
                SqlCommand Command = new SqlCommand(Query, con);
                SqlDataReader Reader = (Command.ExecuteReader());
                if (Reader.Read() == true)
                {
                    return Reader["Pais_Nombre"].ToString();
                }
                else
                {
                    return "";
                }
            }
        }
        void IPacientes.UpdateFac2(int IdPac, string Contrato, string Regimen) 
        {
            try
            {
                var getConect = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getConect["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }
                    string Busqueda = "UPDATE CXN_PACIENTES " +
                                      "SET Pac_Contrato = '" + Contrato + "'," +
                                      "Pac_Regimen = '" + Regimen + "' " +
                                      "WHERE Pac_Id = '" + IdPac + "'";
                    SqlCommand Accion = new SqlCommand(Busqueda, con);
                    int Guarda;
                    Guarda = Accion.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
            }
        }
        void IPacientes.updateEmail(int IdPac, string Email)
        {
            try
            {
                var getConect = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getConect["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }
                    string Busqueda = "UPDATE CXN_PACIENTES " +
                                      "SET Pac_Email = '" + Email + "' " +
                                      "WHERE Pac_Id = '" + IdPac + "'";
                    SqlCommand Accion = new SqlCommand(Busqueda, con);
                    int Guarda;
                    Guarda = Accion.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
            }
        }
        void IPacientes.setEnfermedades(int Paciente, string vih, string hepatitis)
        {
            try
            {
                Dictionary<string, string> getData = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getData["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    string Busqueda = "UPDATE CXN_PACIENTES " +
                                      "SET VIH = @param1, " +
                                      "Hepatitis = @param2 " +
                                      "WHERE Pac_Id = @param3";

                    using (SqlCommand Accion = new SqlCommand(Busqueda, con))
                    {
                        Accion.Parameters.AddWithValue("@param1", vih);
                        Accion.Parameters.AddWithValue("@param2", hepatitis);
                        Accion.Parameters.AddWithValue("@param3", Paciente);
                        Accion.ExecuteNonQuery();
                    }                        
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        List<CXN_GENDERIDENTITY> IPacientes.ListaIdentidadGenero()
        {
            var datConect = Conexion.Conection();
            using (SqlConnection con = new SqlConnection(datConect["Conexion"]))
            {
                if (con != null && con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                String Query = "SELECT * " +
                               "FROM CXN_GENDERIDENTITY " +
                               "ORDER BY Codigo ASC";

                using (SqlCommand Command = new SqlCommand(Query, con))
                {
                    using (SqlDataReader Reader = (Command.ExecuteReader()))
                    {
                        if (Reader.HasRows)
                        {
                            List<CXN_GENDERIDENTITY> L = new List<CXN_GENDERIDENTITY>();

                            while (Reader.Read() == true)
                            {
                                L.Add(new CXN_GENDERIDENTITY
                                {
                                    Id = Convert.ToInt32(Reader["Id"].ToString()),
                                    Identidad = Reader["Identidad"].ToString(),
                                    Codigo = Reader["Codigo"].ToString()
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
        string IPacientes.NameIdentidadGenero(string Code)
        {
            var getConect = Conexion.Conection();

            using (SqlConnection con = new SqlConnection(getConect["Conexion"]))
            {
                if (con != null && con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                String Query = "SELECT Identidad " +
                               "FROM CXN_GENDERIDENTITY " +
                               "WHERE Codigo = @param1";

                using (SqlCommand Command = new SqlCommand(Query, con))
                {
                    Command.Parameters.AddWithValue("@param1", Code);

                    using (SqlDataReader Reader = (Command.ExecuteReader()))
                    {
                        if (Reader.Read() == true)
                        {
                            return Reader["Identidad"].ToString();
                        }
                        else
                        {
                            return "Neutro";
                        }
                    }
                }               
            }
        }
        string IPacientes.CodeIdentidadGenero(string Name)
        {
            var getConect = Conexion.Conection();

            using (SqlConnection con = new SqlConnection(getConect["Conexion"]))
            {
                if (con != null && con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                String Query = "SELECT Codigo " +
                               "FROM CXN_GENDERIDENTITY " +
                               "WHERE Identidad = @param1";

                using (SqlCommand Command = new SqlCommand(Query, con))
                {
                    Command.Parameters.AddWithValue("@param1", Name);

                    using (SqlDataReader Reader = (Command.ExecuteReader()))
                    {
                        if (Reader.Read() == true)
                        {
                            return Reader["Codigo"].ToString();
                        }
                        else
                        {
                            return "04";
                        }
                    }
                }
            }
        }
    }
}
