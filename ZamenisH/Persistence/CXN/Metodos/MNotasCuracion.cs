using Domain;
using Domain.CXN;
using Org.BouncyCastle.Asn1.IsisMtt.X509;
using Persistence.CXN.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Persistence.CXN.Metodos
{
    public class MNotasCuracion : INotasCuracion
    {
        private static readonly IGenerales repoGen = new MGenerales();
        private static readonly ICIE10 repoCIE10 = new MCIE10();

        void INotasCuracion.ActualizarServCuracion(CXN_CARGOS C, string User)
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

                    var Ob = Observacion(C.Car_Adm_Id);
                    string ObservaUpdate = Ob.ToString() + " || EDITADO ENFERMERO: -> Cambio de CUPS " + User + " || ";

                    SqlCommand Horario = new SqlCommand(@"UPDATE CXN_HORARIO " +
                                      "SET " +
                                      "Hor_Pac_Cup = @param1, " +
                                      "Hor_Observacion = @param2 " +
                                      "WHERE Hor_id = '" + C.Car_Adm_Id + "'", con);
                    Horario.Parameters.AddWithValue("@param1", C.Car_Cod);
                    Horario.Parameters.AddWithValue("@param2", ObservaUpdate);
                    Horario.ExecuteNonQuery();

                    SqlCommand Cargos = new SqlCommand(@"UPDATE CXN_CARGOS " +
                                      "SET " +
                                      "Car_Cod = @param1, " +
                                      "Car_Item = @param2, " +
                                      "Car_Val_Un = @param3, " +
                                      "Car_Val_Tot = @param4 " +
                                      "WHERE Car_Adm_Id = '" + C.Car_Adm_Id + "' " +
                                      "AND Car_Tipo = '" + C.Car_Tipo + "'", con);
                    Cargos.Parameters.AddWithValue("@param1", C.Car_Cod);
                    Cargos.Parameters.AddWithValue("@param2", C.Car_Item);
                    Cargos.Parameters.AddWithValue("@param3", C.Car_Val_Un);
                    Cargos.Parameters.AddWithValue("@param4", C.Car_Val_Tot);
                    Cargos.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
            }
        }
        string Observacion(int Admision)
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
                    String Query = "SELECT Hor_Observacion " +
                                   "FROM CXN_HORARIO " +
                                   "WHERE Hor_Id = '" + Admision + "'";
                    SqlCommand Commando = new SqlCommand(Query, con);
                    SqlDataReader Reader = (Commando.ExecuteReader());
                    if (Reader.Read() == true)
                    {
                        return Reader["Hor_Observacion"].ToString();
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
        CXN_NOTAS INotasCuracion.getLastNota(int Paciente)
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

                    String CONV = "SELECT TOP 1 N.Not_Nota, N.Not_Recomienda, N.Not_Observa, N.Not_CaracTej, N.Not_Edad " +
                                 "FROM CXN_NOTAS N " +
                                 "INNER JOIN CXN_HORARIO H ON N.Not_Adm = H.Hor_Id " +
                                 "WHERE H.Hor_Pac_Id = '" + Paciente + "' " +
                                 "ORDER BY H.Hor_Pac_Fecha_Cita DESC";
                    SqlCommand Com_CONV = new SqlCommand(CONV, con);
                    SqlDataReader Lec_CONV = (Com_CONV.ExecuteReader());
                    if (Lec_CONV.Read() == true)
                    {
                        CXN_NOTAS N = new CXN_NOTAS
                        {
                            Not_Nota = Lec_CONV["Not_Nota"].ToString(),
                            Not_Recomienda = Lec_CONV["Not_Recomienda"].ToString(),
                            Not_Observa = Lec_CONV["Not_Observa"].ToString(),
                            Not_Edad = Lec_CONV["Not_Edad"].ToString()
                        };

                        return N;
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
        List<CXN_HORARIO> INotasCuracion.getHistorial(int Paciente)
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

                    String Cargar_Hora2 = "SELECT C.Car_Adm_Id, H.Hor_Pac_Fecha_Cita, B.Bod_Responsable " +
                                         "FROM CXN_NOTAS N " +
                                         "INNER JOIN CXN_CARGOS C ON N.Not_Adm = C.Car_Adm_Id " +
                                         "INNER JOIN CXN_BODEGAS B ON C.Car_Prof = B.Bod_Numero " +
                                         "INNER JOIN CXN_HORARIO H ON C.Car_Adm_Id = H.Hor_Id " +
                                         "WHERE C.Car_Pac = '" + Paciente + "' " +
                                         "AND C.Car_Tipo = 'Nota' " +
                                         "ORDER BY C.Car_Fecha DESC";
                    SqlCommand Carga_Command2 = new SqlCommand(Cargar_Hora2, con);
                    SqlDataReader Lectura_Hora2 = (Carga_Command2.ExecuteReader());
                    if (Lectura_Hora2.HasRows)
                    {
                        List<CXN_HORARIO> H = new List<CXN_HORARIO>();

                        while (Lectura_Hora2.Read() == true)
                        {
                            H.Add(new CXN_HORARIO
                            {
                                Hor_Id = Convert.ToInt32(Lectura_Hora2["Car_Adm_Id"]),
                                Hor_Pac_Fecha_Cita = Convert.ToDateTime(Lectura_Hora2["Hor_Pac_Fecha_Cita"]),
                                Hor_Imp_Age = Lectura_Hora2["Bod_Responsable"].ToString()
                            });
                        }

                        return H;
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
        bool INotasCuracion.GrabarNota(CXN_NOTAS N)
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

                    SqlCommand cmd = new SqlCommand(@"Insert into CXN_NOTAS " +
                                                         "(Not_Pac, " +
                                                         "Not_Med, " +
                                                         "Not_Ase, " +
                                                         "Not_Fecha, " +
                                                         "Not_Cia, " +
                                                         "Not_Edad, " +
                                                         "Not_Nota, " +
                                                         "Not_Recomienda, " +
                                                         "Not_Observa, " +
                                                         "Not_Cup, " +
                                                         "Not_Cant, " +
                                                         "Not_Patologia, " +
                                                         "Not_Adm, " +
                                                         "Not_Epidemia, " +
                                                         "Not_CaracTej, " +
                                                         "Not_Adherencia, " +
                                                         "Not_Acompañante, " +
                                                         "Not_Telefono) " +
                                     "VALUES                  (@param1, " +
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
                                                              "@param12, @param13, @param14, @param15, @param16, @param17, @param18)", con);

                    cmd.Parameters.AddWithValue("@param1", N.Not_Pac);
                    cmd.Parameters.AddWithValue("@param2", N.Not_Med);
                    cmd.Parameters.AddWithValue("@param3", N.Not_Ase);
                    cmd.Parameters.Add(new SqlParameter("@param4", SqlDbType.DateTime)).Value = N.Not_Fecha;
                    cmd.Parameters.AddWithValue("@param5", N.Not_Cia);
                    cmd.Parameters.AddWithValue("@param6", N.Not_Edad);
                    cmd.Parameters.AddWithValue("@param7", N.Not_Nota);
                    cmd.Parameters.AddWithValue("@param8", N.Not_Recomienda);
                    cmd.Parameters.AddWithValue("@param9", N.Not_Observa);
                    cmd.Parameters.AddWithValue("@param10", N.Not_Cup);
                    cmd.Parameters.AddWithValue("@param11", N.Not_Cant);
                    cmd.Parameters.AddWithValue("@param12", N.Not_Patologia);
                    cmd.Parameters.AddWithValue("@param13", N.Not_Adm);
                    cmd.Parameters.AddWithValue("@param14", N.Not_Epidemia);
                    cmd.Parameters.AddWithValue("@param15", N.Not_CaracTej);
                    cmd.Parameters.AddWithValue("@param16", N.Not_Adherencia);
                    cmd.Parameters.AddWithValue("@param17", (N.Not_Acompañante != null ? N.Not_Acompañante : ""));
                    cmd.Parameters.AddWithValue("@param18", (N.Not_Telefono != null ? N.Not_Telefono : ""));
                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return false;
            }
        }
        Dictionary<string, string> INotasCuracion.Diagnosticos(int Paciente)
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

                    String Query = "SELECT TOP 1 HC_Patologia, HC_DX1, HC_DX2, HC_DX3, HC_Imp_Dx " +
                                   "FROM CXN_HCMG " +
                                   "WHERE HC_PacId = '" + Paciente + "' " +
                                   "ORDER BY HC_Fecha DESC";
                    SqlCommand Commando = new SqlCommand(Query, con);
                    SqlDataReader Reader = (Commando.ExecuteReader());
                    if (Reader.Read() == true)
                    {
                        Dictionary<string, string> D = new Dictionary<string, string>();

                        D.Add("DX1", Reader["HC_DX1"].ToString());
                        D.Add("DX2", Reader["HC_DX2"].ToString());
                        D.Add("DX3", Reader["HC_DX3"].ToString());
                        D.Add("Impresion", Reader["HC_Imp_Dx"].ToString());
                        D.Add("Patologia", Reader["HC_Patologia"].ToString());

                        return D;
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
        CXN_NOTAS INotasCuracion.seeNotaPrevReport(int Adm_Nota_Export)
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

                    String Cargar_Hora2 = "SELECT Com_Nombre, Com_Direccion, Com_Telefono, Bod_Responsable, Pac_PrimerN, Pac_SegundoN, Pac_PrimerA, Pac_SegundoA, " +
                                          "Ase_Descripcion, Pac_TipoId, Pac_IdNum, Bod_Reg_Med, Pac_Sexo, Not_Edad, Pac_FechaNto, Hor_Pac_Fecha_Cita, Not_Nota, Not_Observa, " +
                                          "Not_Recomienda , Not_Adm, Not_Adherencia, Not_NotaAcla " +
                                          "FROM CXN_NOTAS " +
                                          "INNER JOIN CXN_HORARIO ON CXN_NOTAS.Not_Adm = CXN_HORARIO.Hor_id " +
                                          "INNER JOIN CXN_PACIENTES ON CXN_HORARIO.Hor_Pac_Id = CXN_PACIENTES.Pac_Id " +
                                          "INNER JOIN CXN_CIA ON CXN_HORARIO.Hor_Pac_Cia = CXN_CIA.Com_Identificador " +
                                          "INNER JOIN CXN_BODEGAS ON CXN_HORARIO.Hor_Pac_Bod = CXN_BODEGAS.Bod_Numero " +
                                          "INNER JOIN CXN_ASEGURADORA ON CXN_HORARIO.Hor_Pac_Ase = CXN_ASEGURADORA.Ase_Identificador " +
                                          "INNER JOIN CXN_CARGOS ON CXN_HORARIO.Hor_Id = CXN_CARGOS.Car_Adm_Id " +
                                          "WHERE CXN_NOTAS.Not_Adm = '" + Adm_Nota_Export + "' " +
                                          "AND CXN_CARGOS.Car_Tipo = 'Nota'";
                    SqlCommand Carga_Command2 = new SqlCommand(Cargar_Hora2, con);
                    SqlDataReader Lectura_Hora2 = (Carga_Command2.ExecuteReader());
                    if (Lectura_Hora2.Read() == true)
                    {
                        CXN_NOTAS N = new CXN_NOTAS
                        {
                            Not_Epidemia = Lectura_Hora2["Pac_PrimerA"].ToString() + " " + Lectura_Hora2["Pac_SegundoA"].ToString() + " " + Lectura_Hora2["Pac_PrimerN"].ToString() + " " + Lectura_Hora2["Pac_SegundoN"].ToString(),
                            Not_CaracTej = Lectura_Hora2["Pac_TipoId"].ToString() + " " + Lectura_Hora2["Pac_IdNum"].ToString(),
                            Not_Nota = Lectura_Hora2["Not_Nota"].ToString(),
                            Not_Observa = Lectura_Hora2["Not_Observa"].ToString(),
                            Not_Recomienda = Lectura_Hora2["Not_Recomienda"].ToString(),
                            Not_Fecha = Convert.ToDateTime(Lectura_Hora2["Hor_Pac_Fecha_Cita"]),
                            Not_Cup = Lectura_Hora2["Bod_Responsable"].ToString(),
                            Not_Adm = Convert.ToInt32(Lectura_Hora2["Not_Adm"]),
                            Not_Adherencia = Lectura_Hora2["Not_Adherencia"].ToString(),
                            Not_NotaAcla = Lectura_Hora2["Not_NotaAcla"].ToString(),
                            Not_Edad = Lectura_Hora2["Ase_Descripcion"].ToString()
                        };

                        return N;
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
        bool INotasCuracion.consNotaJefe(int Adm_Nota_Export)
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

                    String Cargar_Hora2 = "SELECT Car_Tipo " +
                                          "FROM CXN_NOTAS " +
                                          "INNER JOIN CXN_CARGOS ON CXN_NOTAS.Not_Adm = CXN_CARGOS.Car_Adm_Id " +
                                          "WHERE CXN_NOTAS.Not_Adm = '" + Adm_Nota_Export + "' " +
                                          "AND CXN_CARGOS.Car_Tipo = 'Historia'";
                    SqlCommand Carga_Command2 = new SqlCommand(Cargar_Hora2, con);
                    SqlDataReader Lectura_Hora2 = (Carga_Command2.ExecuteReader());
                    if (Lectura_Hora2.Read() == true)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return false;
            }
        }
        List<ReportNotas> INotasCuracion.NotasJefe(int Admision)
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
                    String Cargar_Hora2 = "SELECT Com_Nombre, Com_Direccion, Com_Telefono, Bod_Responsable, Pac_PrimerN, Pac_SegundoN, Pac_PrimerA, Pac_SegundoA, " +
                                          "Ase_Descripcion, Pac_TipoId, Pac_IdNum, Bod_Reg_Med, Pac_Sexo, Not_Edad, Pac_FechaNto, Hor_Pac_Fecha_Cita, Not_Nota, Not_Observa, " +
                                          "Not_Recomienda , Not_Adm, Car_Cod, Car_Item, Car_Detalle, Car_Cant, Not_Epidemia, Not_Adherencia, Not_NotaAcla " +
                                          "FROM CXN_NOTAS " +
                                          "INNER JOIN CXN_HORARIO ON CXN_NOTAS.Not_Adm = CXN_HORARIO.Hor_id " +
                                          "INNER JOIN CXN_PACIENTES ON CXN_HORARIO.Hor_Pac_Id = CXN_PACIENTES.Pac_Id " +
                                          "INNER JOIN CXN_CIA ON CXN_HORARIO.Hor_Pac_Cia = CXN_CIA.Com_Identificador " +
                                          "INNER JOIN CXN_BODEGAS ON CXN_HORARIO.Hor_Pac_Bod = CXN_BODEGAS.Bod_Numero " +
                                          "INNER JOIN CXN_ASEGURADORA ON CXN_HORARIO.Hor_Pac_Ase = CXN_ASEGURADORA.Ase_Identificador " +
                                          "INNER JOIN CXN_CARGOS ON CXN_HORARIO.Hor_Id = CXN_CARGOS.Car_Adm_Id " +
                                          "WHERE CXN_NOTAS.Not_Adm = '" + Admision + "' " +
                                          "AND CXN_CARGOS.Car_Tipo = 'Historia'";
                    SqlCommand Carga_Command2 = new SqlCommand(Cargar_Hora2, con);
                    SqlDataReader Lectura_Hora2 = (Carga_Command2.ExecuteReader());
                    if (Lectura_Hora2.HasRows)
                    {
                        var LogoImage = Genera_QR_NotasJefe(Admision);
                        if (LogoImage == null)
                        {
                            return null;
                        }

                        List<ReportNotas> export_notas_report = new List<ReportNotas>();

                        while (Lectura_Hora2.Read() == true)
                        {
                            var Dato1 = repoCIE10.BuscaDX(LogoImage.DX1.ToString());
                            var Dato2 = repoCIE10.BuscaDX(LogoImage.DX2.ToString());
                            var Dato3 = repoCIE10.BuscaDX(LogoImage.DX3.ToString());

                            export_notas_report.Add(new ReportNotas
                            {
                                Car_Cod = Lectura_Hora2["Car_Cod"].ToString(),
                                Car_Item = Lectura_Hora2["Car_Item"].ToString(),
                                Car_Cant = Convert.ToInt32(Lectura_Hora2["Car_Cant"]),
                                Car_Detalle = Lectura_Hora2["Car_Detalle"].ToString(),
                                EmpresaNombre = Lectura_Hora2["Com_Nombre"].ToString(), //
                                EmpresaDireccion = Lectura_Hora2["Com_Direccion"].ToString(),  //
                                EmpresaTelefono = Lectura_Hora2["Com_Telefono"].ToString(), //
                                ProfesionalNombre = Lectura_Hora2["Bod_Responsable"].ToString(), //
                                PacienteNombre = Lectura_Hora2["Pac_PrimerN"].ToString() + " " + Lectura_Hora2["Pac_SegundoN"].ToString() + " " + Lectura_Hora2["Pac_PrimerA"].ToString() + " " + Lectura_Hora2["Pac_SegundoA"].ToString(), //
                                PacienteAseguradora = Lectura_Hora2["Ase_Descripcion"].ToString(), //
                                PacienteIdentificacion = Lectura_Hora2["Pac_TipoId"].ToString() + " " + Lectura_Hora2["Pac_IdNum"].ToString(), //
                                RegistroMedico = Lectura_Hora2["Bod_Reg_Med"].ToString(), //
                                Pac_Sexo = Lectura_Hora2["Pac_Sexo"].ToString(),
                                Edad = Lectura_Hora2["Not_Edad"].ToString(),
                                FNto = Convert.ToDateTime(Lectura_Hora2["Pac_FechaNto"]), //
                                FechaBase = Convert.ToDateTime(Lectura_Hora2["Hor_Pac_Fecha_Cita"]), //
                                Nota = Lectura_Hora2["Not_Nota"].ToString().ToUpper(),
                                Observa = Lectura_Hora2["Not_Observa"].ToString().ToUpper(),
                                Recomienda = Lectura_Hora2["Not_Recomienda"].ToString().ToUpper(),
                                Admision = Admision,
                                Epidemia = Lectura_Hora2["Not_Epidemia"].ToString().ToUpper(),
                                Adherencia = Lectura_Hora2["Not_Adherencia"].ToString(),
                                NotaAclaratoria = Lectura_Hora2["Not_NotaAcla"].ToString().ToUpper(),
                                Diagnostico1 = LogoImage.DX1.ToString() + " - " + Dato1.ToString(),
                                Diagnostico_Rel2 = LogoImage.DX2.ToString() + " - " + Dato2.ToString(),
                                Diagnostico_Rel3 = LogoImage.DX3.ToString() + " - " + Dato3.ToString(),
                                Logo = LogoImage.QRNota
                            });
                        }
                        return export_notas_report;
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
        ListaNota Genera_QR_NotasJefe(int Admi)
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

                    String Cargar_Hora2 = "SELECT Top 1 P.Pac_PrimerA, P.Pac_SegundoA, P.Pac_PrimerN, P.Pac_SegundoN, P.Pac_PrimerN, " +
                                          "P.Pac_SegundoN, H.Hor_Pac_Fecha_Cita, P.Pac_TipoId, " +
                                          "P.Pac_IdNum, C.Com_Nombre, CA.Car_Dx1, CA.Car_Dx2, CA.Car_Dx3 " +
                                          "FROM CXN_NOTAS N " +
                                          "INNER JOIN CXN_HORARIO H ON N.Not_Adm = H.Hor_id " +
                                          "INNER JOIN CXN_PACIENTES P ON H.Hor_Pac_Id = P.Pac_Id " +
                                          "INNER JOIN CXN_CIA C ON H.Hor_Pac_Cia = C.Com_Identificador " +
                                          "INNER JOIN CXN_CARGOS CA ON H.Hor_Id = CA.Car_Adm_Id " +
                                          "WHERE N.Not_Adm = '" + Admi + "' " +
                                          "AND CA.Car_Tipo = 'Historia'";
                    SqlCommand Carga_Command2 = new SqlCommand(Cargar_Hora2, con);
                    SqlDataReader Lectura_Hora2 = (Carga_Command2.ExecuteReader());
                    if (Lectura_Hora2.Read() == true)
                    {
                        ListaNota L = new ListaNota();
                        string Datos = "PACIENTE: " + Lectura_Hora2["Pac_PrimerA"].ToString() + " " + Lectura_Hora2["Pac_SegundoA"].ToString() + " " + Lectura_Hora2["Pac_PrimerN"].ToString() + " " + Lectura_Hora2["Pac_SegundoN"].ToString() +
                                       " FECHA ATENCION: " + Convert.ToDateTime(Lectura_Hora2["Hor_Pac_Fecha_Cita"]).ToString(getData["Format_Fecha"]) +
                                       " IDENTIFICACION: " + Lectura_Hora2["Pac_TipoId"].ToString() + " " + Lectura_Hora2["Pac_IdNum"].ToString() +
                                       " CLINICA: " + Lectura_Hora2["Com_Nombre"].ToString();
                        var ImaRes = repoGen.CodifyQR(Datos);

                        L.DX1 = Lectura_Hora2["Car_Dx1"].ToString();
                        L.DX2 = Lectura_Hora2["Car_Dx2"].ToString();
                        L.DX3 = Lectura_Hora2["Car_Dx3"].ToString();
                        L.QRNota = repoGen.GetBytes(ImaRes);
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
        List<CXN_CARGOS> INotasCuracion.CargoNota(int Adm_Nota_Export)
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
                    String Cargar_Hora = "SELECT Com_Nombre, Com_Direccion, Com_Telefono, Bod_Responsable, Pac_PrimerN, Pac_SegundoN, Pac_PrimerA, Pac_SegundoA, " +
                                         "Ase_Descripcion, Pac_TipoId, Pac_IdNum, Bod_Reg_Med, Pac_Sexo, Not_Edad, Pac_FechaNto, Hor_Pac_Fecha_Cita, Not_Nota, Not_Observa, " +
                                         "Not_Recomienda , Not_Adm, Car_Cod, Car_Item, Car_Detalle, Car_Cant, Car_Usr_Graba " +
                                         "FROM CXN_NOTAS " +
                                         "INNER JOIN CXN_HORARIO ON CXN_NOTAS.Not_Adm = CXN_HORARIO.Hor_id " +
                                         "INNER JOIN CXN_PACIENTES ON CXN_HORARIO.Hor_Pac_Id = CXN_PACIENTES.Pac_Id " +
                                         "INNER JOIN CXN_CIA ON CXN_HORARIO.Hor_Pac_Cia = CXN_CIA.Com_Identificador " +
                                         "INNER JOIN CXN_BODEGAS ON CXN_HORARIO.Hor_Pac_Bod = CXN_BODEGAS.Bod_Numero " +
                                         "INNER JOIN CXN_ASEGURADORA ON CXN_HORARIO.Hor_Pac_Ase = CXN_ASEGURADORA.Ase_Identificador " +
                                         "INNER JOIN CXN_CARGOS ON CXN_HORARIO.Hor_Id = CXN_CARGOS.Car_Adm_Id " +
                                         "WHERE CXN_NOTAS.Not_Adm = '" + Adm_Nota_Export + "' " +
                                         "AND CXN_CARGOS.Car_Tipo IN ('Cargo','Nota')";
                    SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con);
                    SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader());
                    if (Lectura_Hora.HasRows)
                    {
                        List<CXN_CARGOS> C = new List<CXN_CARGOS>();

                        while (Lectura_Hora.Read() == true)
                        {
                            C.Add(new CXN_CARGOS
                            {
                                Car_Cod = Lectura_Hora["Car_Cod"].ToString(),
                                Car_Item = Lectura_Hora["Car_Item"].ToString(),
                                Car_Cant = Convert.ToInt32(Lectura_Hora["Car_Cant"]),
                                Car_Usr_Graba = Lectura_Hora["Car_Usr_Graba"].ToString()
                            });
                        }

                        return C;
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
        void INotasCuracion.Estadisticas_Curaciones(CXN_ESTADISTICAS E)
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

                    DateTime Hoy = DateTime.Now.Date;

                    SqlCommand cmd = new SqlCommand(@"Insert into CXN_ESTADISTICAS " +
                                                         "(Est_Admision, " +
                                                         "Est_Edad, " +
                                                         "Est_Patologia, " +
                                                         "Est_Localizacion, " +
                                                         "Est_Evolucion, " +
                                                         "Est_Infectado, " +
                                                         "Est_Ingreso, " +
                                                         "Est_Alta, " +
                                                         "Est_Fecha, " +
                                                         "Est_USuario) " +
                                     "VALUES                  (@param1, " +
                                                              "@param2, " +
                                                              "@param3, " +
                                                              "@param4, " +
                                                              "@param5, " +
                                                              "@param6, " +
                                                              "@param7, " +
                                                              "@param8, " +
                                                              "@param9, " +
                                                              "@param10)", con);

                    cmd.Parameters.AddWithValue("@param1", E.Est_Admision);
                    cmd.Parameters.AddWithValue("@param2", E.Est_Edad);
                    cmd.Parameters.AddWithValue("@param3", E.Est_Patologia);
                    cmd.Parameters.AddWithValue("@param4", E.Est_Localizacion);
                    cmd.Parameters.AddWithValue("@param5", E.Est_Evolucion);
                    cmd.Parameters.AddWithValue("@param6", E.Est_Infectado);
                    cmd.Parameters.AddWithValue("@param7", E.Est_Ingreso);
                    cmd.Parameters.AddWithValue("@param8", E.Est_Alta);
                    cmd.Parameters.Add(new SqlParameter("@param9", SqlDbType.DateTime)).Value = E.Est_Fecha;
                    cmd.Parameters.AddWithValue("@param10", E.Est_Usuario);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
            }
        }
        bool INotasCuracion.insertarHerida(CXN_NOTASMED H)
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

                    DateTime Hoy = DateTime.Now;

                    SqlCommand cmd = new SqlCommand(@"Insert into CXN_NOTASMED (Admision, " + 
                                                          "Largo, " + 
                                                          "Ancho, " + 
                                                          "Profundidad, " + 
                                                          "Total, " +
                                                          "Evolucion, " +
                                                          "NovedadHerida, " +
                                                          "txtOtros, " +
                                                          "txtFibrina, " +
                                                          "txtNecroticoHumedo, " +
                                                          "txtNecroticoSeco, " +
                                                          "txtEpitelizacion, " +
                                                          "txtGranulacion, " +
                                                          "txtLocalizacion, " +
                                                          "selExudado, " +
                                                          "RazonNoEvolucion, " +
                                                          "RazonExudado) " + 
                                 "values                  (@param1, " + 
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
                                                          "@param14, " +
                                                          "@param15," +
                                                          "@param16," +
                                                          "@param17)", con); 

                    cmd.Parameters.AddWithValue("@param1", H.Admision);
                    cmd.Parameters.AddWithValue("@param2", H.Largo);
                    cmd.Parameters.AddWithValue("@param3", H.Ancho);
                    cmd.Parameters.AddWithValue("@param4", H.Profundidad);
                    cmd.Parameters.AddWithValue("@param5", H.Total);
                    cmd.Parameters.AddWithValue("@param6", H.Evolucion);
                    cmd.Parameters.AddWithValue("@param7", H.NovedadHerida);
                    cmd.Parameters.AddWithValue("@param8", H.txtOtros);
                    cmd.Parameters.AddWithValue("@param9", H.txtFibrina);
                    cmd.Parameters.AddWithValue("@param10", H.txtNecroticoHumedo);
                    cmd.Parameters.AddWithValue("@param11", H.txtNecroticoSeco);
                    cmd.Parameters.AddWithValue("@param12", H.txtEpitelizacion);
                    cmd.Parameters.AddWithValue("@param13", H.txtGranulacion);
                    cmd.Parameters.AddWithValue("@param14", H.txtLocalizacion);
                    cmd.Parameters.AddWithValue("@param15", H.selExudado);
                    cmd.Parameters.AddWithValue("@param16", H.RazonNoEvolucion);
                    cmd.Parameters.AddWithValue("@param17", H.RazonExudado);

                    int c = cmd.ExecuteNonQuery();
                    if (c > 0) { return true;}
                    return false;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return false;
            }
        }

        int LoadLastMedidaHeridas(int Paciente, int AdmisionActual)
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

                    String Cargar_Hora = "SELECT TOP 1 CXN_NOTASMED.Admision " +
                                         "FROM CXN_NOTASMED " +
                                         "INNER JOIN CXN_NOTAS ON CXN_NOTASMED.Admision = CXN_NOTAS.Not_Adm " +
                                         "WHERE CXN_NOTAS.Not_Pac = @param1 " +
                                         "AND CXN_NOTASMED.Admision <> @param2 " +
                                         "ORDER BY CXN_NOTAS.Not_Fecha DESC";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", Paciente);
                        Carga_Command.Parameters.AddWithValue("@param2", AdmisionActual);

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.Read() == true)
                            {                                
                                return Convert.ToInt32(Lectura_Hora["Admision"]);
                            }
                            else
                            {
                                return 0;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return 0;
            }
        }
        List<CXN_NOTASMED> INotasCuracion.LoadHeridas(int Admision)
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

                    String Cargar_Hora = "SELECT * " +
                                         "FROM CXN_NOTASMED " +
                                         "INNER JOIN CXN_NOTAS ON CXN_NOTASMED.Admision = CXN_NOTAS.Not_Adm " +
                                         "WHERE CXN_NOTASMED.Admision = @param1 " +
                                         "ORDER BY CXN_NOTAS.Not_Fecha DESC";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", Admision);

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.HasRows)
                            {
                                List<CXN_NOTASMED> C = new List<CXN_NOTASMED>();

                                while (Lectura_Hora.Read() == true)
                                {
                                    C.Add(new CXN_NOTASMED
                                    {
                                        Largo = Convert.ToDecimal(Lectura_Hora["Largo"]),
                                        Ancho = Convert.ToDecimal(Lectura_Hora["Ancho"]),
                                        Profundidad = Convert.ToDecimal(Lectura_Hora["Profundidad"]),
                                        Total = Convert.ToDecimal(Lectura_Hora["Total"]),
                                        Evolucion = Lectura_Hora["Evolucion"].ToString(),
                                        NovedadHerida = Lectura_Hora["NovedadHerida"].ToString(),
                                        txtOtros = Lectura_Hora["txtOtros"].ToString(),
                                        txtFibrina = Lectura_Hora["txtFibrina"].ToString(),
                                        txtNecroticoHumedo = Lectura_Hora["txtNecroticoHumedo"].ToString(),
                                        txtNecroticoSeco = Lectura_Hora["txtNecroticoSeco"].ToString(),
                                        txtEpitelizacion = Lectura_Hora["txtEpitelizacion"].ToString(),
                                        txtGranulacion = Lectura_Hora["txtGranulacion"].ToString(),
                                        txtLocalizacion = Lectura_Hora["txtLocalizacion"].ToString(),
                                        selExudado = Lectura_Hora["selExudado"].ToString(),
                                        RazonNoEvolucion = Lectura_Hora["RazonNoEvolucion"].ToString(),
                                        RazonExudado = Lectura_Hora["RazonExudado"].ToString(),
                                        Admision = Convert.ToInt32(Lectura_Hora["Admision"]),
                                        Id = Convert.ToInt32(Lectura_Hora["Id"])
                                    });
                                }

                                return C;
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
        List<CXN_NOTASMED> INotasCuracion.LoadHeridas(int Paciente, int Admision)
        {
            try
            {
                int Adm = LoadLastMedidaHeridas(Paciente, Admision);
                if (Adm == 0)
                {
                    return null;
                }

                Dictionary<string, string> getData = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getData["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    String Cargar_Hora = "SELECT * " +
                                         "FROM CXN_NOTASMED " +
                                         "INNER JOIN CXN_NOTAS ON CXN_NOTASMED.Admision = CXN_NOTAS.Not_Adm " +
                                         "WHERE CXN_NOTASMED.Admision = @param1";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", Adm);

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.HasRows)
                            {
                                List<CXN_NOTASMED> C = new List<CXN_NOTASMED>();

                                while (Lectura_Hora.Read() == true)
                                {
                                    C.Add(new CXN_NOTASMED
                                    {
                                        Largo = Convert.ToDecimal(Lectura_Hora["Largo"]),
                                        Ancho = Convert.ToDecimal(Lectura_Hora["Ancho"]),
                                        Profundidad = Convert.ToDecimal(Lectura_Hora["Profundidad"]),
                                        Total = Convert.ToDecimal(Lectura_Hora["Total"]),
                                        Evolucion = Lectura_Hora["Evolucion"].ToString(),
                                        NovedadHerida = Lectura_Hora["NovedadHerida"].ToString(),
                                        txtOtros = Lectura_Hora["txtOtros"].ToString(),
                                        txtFibrina = Lectura_Hora["txtFibrina"].ToString(),
                                        txtNecroticoHumedo = Lectura_Hora["txtNecroticoHumedo"].ToString(),
                                        txtNecroticoSeco = Lectura_Hora["txtNecroticoSeco"].ToString(),
                                        txtEpitelizacion = Lectura_Hora["txtEpitelizacion"].ToString(),
                                        txtGranulacion = Lectura_Hora["txtGranulacion"].ToString(),
                                        txtLocalizacion = Lectura_Hora["txtLocalizacion"].ToString(),
                                        selExudado = Lectura_Hora["selExudado"].ToString(),
                                        RazonNoEvolucion = Lectura_Hora["RazonNoEvolucion"].ToString(),
                                        RazonExudado = Lectura_Hora["RazonExudado"].ToString(),
                                        //Admision = Convert.ToInt32(Lectura_Hora["Admision"]),
                                        Id = Convert.ToInt32(Lectura_Hora["Id"])                                        
                                    });
                                }

                                return C;
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
        List<CXN_NOTASMED> INotasCuracion.VerCitas(int Paciente)
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
                    String Query = "SELECT DISTINCT TOP 30 H.Hor_Pac_Fecha_Cita, N.Admision, B.Bod_Responsable " +
                                   "FROM CXN_HORARIO H " +
                                   "INNER JOIN CXN_NOTASMED N ON H.Hor_Id = N.Admision " +
                                   "INNER JOIN CXN_BODEGAS B ON H.Hor_Pac_Bod = B.Bod_Numero " +
                                   "WHERE Hor_Pac_Id = @param1 " +
                                   "GROUP BY H.Hor_Pac_Fecha_Cita, N.Admision, B.Bod_Responsable " +
                                   "ORDER BY Hor_Pac_Fecha_Cita DESC";

                    using (SqlCommand Commando = new SqlCommand(Query, con))
                    {
                        Commando.Parameters.AddWithValue("@param1", Paciente);

                        using (SqlDataReader Reader = (Commando.ExecuteReader()))
                        {
                            if (Reader.HasRows)
                            {
                                List<CXN_NOTASMED> NM = new List<CXN_NOTASMED>();

                                while (Reader.Read() == true)
                                {
                                    NM.Add(new CXN_NOTASMED { 
                                        txtGranulacion = Convert.ToDateTime(Reader["Hor_Pac_Fecha_Cita"]).ToString("yyyy-MM-dd"),
                                        Admision = Convert.ToInt32(Reader["Admision"]),
                                        Evolucion = Reader["Bod_Responsable"].ToString()
                                    });
                                }

                                return NM;
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
        CXN_NOTASMED INotasCuracion.VerPosisionHerida(int Id)
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
                    String Query = "SELECT * " +
                                   "FROM CXN_NOTASMED " +
                                   "WHERE Id = @param1";

                    using (SqlCommand Commando = new SqlCommand(Query, con))
                    {
                        Commando.Parameters.AddWithValue("@param1", Id);

                        using (SqlDataReader Reader = (Commando.ExecuteReader()))
                        {
                            if (Reader.Read() == true)
                            {
                                CXN_NOTASMED NM = new CXN_NOTASMED
                                {
                                    Admision = Convert.ToInt32(Reader["Admision"]),
                                    Id = Convert.ToInt32(Reader["Id"]),
                                    Largo = Convert.ToDecimal(Reader["Largo"]),
                                    Ancho = Convert.ToDecimal(Reader["Ancho"]),
                                    Profundidad = Convert.ToDecimal(Reader["Profundidad"]),
                                    Total = Convert.ToDecimal(Reader["Total"]),
                                    Evolucion = Reader["Evolucion"].ToString(),
                                    NovedadHerida = Reader["NovedadHerida"].ToString(),
                                    txtEpitelizacion = Reader["txtEpitelizacion"].ToString(),
                                    txtFibrina = Reader["txtFibrina"].ToString(),
                                    txtGranulacion = Reader["txtGranulacion"].ToString(),
                                    txtNecroticoHumedo = Reader["txtNecroticoHumedo"].ToString(),
                                    txtNecroticoSeco = Reader["txtNecroticoSeco"].ToString(),
                                    txtOtros = Reader["txtOtros"].ToString(),
                                    txtLocalizacion = Reader["txtLocalizacion"].ToString(),
                                };                              

                                return NM;
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
        List<CXN_NOTASMED> INotasCuracion.LoadHistorialHeridas(int Paciente)
        {
            try
            {
                Dictionary<string, string> getData = Conexion.Conection();

                List<int> admisiones = Admisiones(Paciente);

                if (admisiones == null)
                {
                    return null;
                }

                List<CXN_NOTASMED> C = new List<CXN_NOTASMED>();

                foreach (int i in admisiones)
                {
                    List<CXN_NOTASMED> temp = LoadHeridas(i);
                    if (temp != null)
                    {
                        foreach (CXN_NOTASMED i2 in temp)
                        {
                            C.Add(new CXN_NOTASMED
                            {
                                Admision = i2.Admision,
                                Ancho = i2.Ancho,
                                Largo = i2.Largo,
                                Profundidad = i2.Profundidad,
                                Total = i2.Total,
                                txtEpitelizacion = i2.txtEpitelizacion,
                                txtFibrina = i2.txtFibrina,
                                txtGranulacion = i2.txtGranulacion,
                                txtNecroticoHumedo = i2.txtNecroticoHumedo,
                                txtNecroticoSeco = i2.txtNecroticoSeco,
                                Evolucion = i2.Evolucion,
                                Estado = i2.Estado,
                                NovedadHerida = i2.NovedadHerida,
                                Id = i2.Id,
                                txtOtros = i2.txtOtros,
                                Fecha = i2.Fecha,
                                txtLocalizacion = i2.txtLocalizacion,
                                selExudado = i2.selExudado
                            });
                        }
                    }
                }

                if (C.Count > 0)
                {
                    return C;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        List<int> Admisiones(int Paciente)
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

                    String Cargar_Hora = "SELECT TOP 20 Hor_Id " +
                                         "FROM CXN_HORARIO " +
                                         "WHERE Hor_Pac_Id = @param1 " +
                                         "AND Hor_Pac_Tipo_Serv = @param2 " +
                                         "AND Hor_Estado = @param3 " +
                                         "ORDER BY Hor_Pac_Fecha_Cita DESC";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", Paciente);
                        Carga_Command.Parameters.AddWithValue("@param2", "CU");
                        Carga_Command.Parameters.AddWithValue("@param3", "H");

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.HasRows)
                            {
                                List<int> C = new List<int>();

                                while (Lectura_Hora.Read() == true)
                                {
                                    C.Add(Convert.ToInt32(Lectura_Hora["Hor_Id"]));
                                }

                                return C;
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
        List<CXN_NOTASMED> LoadHeridas(int Admision)
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

                    String Cargar_Hora = "SELECT * " +
                                         "FROM CXN_NOTASMED " +
                                         "INNER JOIN CXN_HORARIO ON CXN_NOTASMED.Admision = CXN_HORARIO.Hor_Id " +
                                         "WHERE Admision = @param1 ";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", Admision);

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.HasRows)
                            {
                                List<CXN_NOTASMED> C = new List<CXN_NOTASMED>();

                                while (Lectura_Hora.Read() == true)
                                {
                                    C.Add(new CXN_NOTASMED
                                    {
                                        Admision = Convert.ToInt32(Lectura_Hora["Admision"]),
                                        Id = Convert.ToInt32(Lectura_Hora["Id"]),
                                        Largo = Convert.ToDecimal(Lectura_Hora["Largo"]),
                                        Ancho = Convert.ToDecimal(Lectura_Hora["Ancho"]),
                                        Profundidad = Convert.ToDecimal(Lectura_Hora["Profundidad"]),
                                        Total = Convert.ToDecimal(Lectura_Hora["Total"]),
                                        txtGranulacion = Lectura_Hora["txtGranulacion"].ToString(),
                                        txtEpitelizacion = Lectura_Hora["txtEpitelizacion"].ToString(),
                                        txtFibrina = Lectura_Hora["txtFibrina"].ToString(),
                                        txtNecroticoHumedo = Lectura_Hora["txtNecroticoHumedo"].ToString(),
                                        txtNecroticoSeco = Lectura_Hora["txtNecroticoSeco"].ToString(),
                                        Evolucion = Lectura_Hora["Evolucion"].ToString(),
                                        Estado = "",
                                        NovedadHerida = Lectura_Hora["NovedadHerida"].ToString(),
                                        txtOtros = Lectura_Hora["txtOtros"].ToString(),
                                        Fecha = Convert.ToDateTime(Lectura_Hora["Hor_Pac_Fecha_Cita"]),
                                        txtLocalizacion = Lectura_Hora["txtLocalizacion"].ToString()
                                    });
                                }

                                return C;
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
