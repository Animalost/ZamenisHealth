using Domain;
using Domain.CXN;
using Persistence.CXN.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Persistence.CXN.Metodos
{
    public class MRadiologia : IRadiologia
    {
        bool IRadiologia.InsertarHistoria(CXN_HCRADIOLOGIA HC)
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

                    SqlCommand Accion = new SqlCommand(@"INSERT INTO CXN_HCRADIOLOGIA " +
                                                         "(PacId, " +
                                                         "PacienteNombre, " +
                                                         "PacienteTipoId, " +
                                                         "PacienteId, " +
                                                         "MotivoConsulta, " +
                                                         "EnfermedadActual, " +
                                                         "EvolucionSintomas, " +
                                                         "AntecedentesRelevantes, " +
                                                         "MedicamentosActuales, " +
                                                         "AntecedentesRenales, " +
                                                         "AntecedentesCardioVasculares, " +
                                                         "Embarazo, " +
                                                         "ImplantesMetalicos, " +
                                                         "MenorEdad, " +
                                                         "Presion, " +
                                                         "Peso, " +
                                                         "GlassHow, " +
                                                         "Talla, " +
                                                         "FResp, " +
                                                         "FCar, " +
                                                         "RH, " +
                                                         "Conciencia, " +
                                                         "IMC, " +
                                                         "ObservacionExaMedico, " +
                                                         "TipoEstudio, " +
                                                         "MedioContraste, " +
                                                         "ReaccionAdversa, " +
                                                         "Tecnica, " +
                                                         "Hallazgos, " +
                                                         "DX1, " +
                                                         "DX2, " +
                                                         "DX3, " +
                                                         "NotaDX1, " +
                                                         "NotaDX2, " +
                                                         "NotaDX3, " +
                                                         "CausaExterna, " +
                                                         "ImpDX1, " +
                                                         "ImpDX2, " +
                                                         "ImpDX3, " +
                                                         "EstudioComplementario, " +
                                                         "ControlSeguimiento, " +
                                                         "Compañia, " +
                                                         "Aseguradora, " +
                                                         "Medico, " +
                                                         "Fecha, " +
                                                         "HCAdm, " +
                                                         "HCCant) " +
                                     "VALUES                  (@param1, " +
                                                              "@param2, @param3, @param4, @param5, " +
                                                              "@param6, @param7, @param8, @param9, " +
                                                              "@param10, @param11, @param12, @param13, " +
                                                              "@param14, @param15, @param16, @param17, " +
                                                              "@param18, @param19, @param20, @param21, " +
                                                              "@param22, @param23, @param24, @param25, " +
                                                              "@param26, @param27, @param28, @param29, " +
                                                              "@param30, " +
                                                              "@param31, @param32, @param33, " +
                                                              "@param34, @param35, @param36, " +
                                                              "@param37, @param38, " +
                                                              "@param39, @param40, @param41, @param42, " +
                                                              "@param43, @param44, @param45, @param46, @param47)", con);

                    Accion.Parameters.AddWithValue("@param1", HC.PacId);
                    Accion.Parameters.AddWithValue("@param2", HC.PacienteNombre);
                    Accion.Parameters.AddWithValue("@param3", HC.PacienteTipoId);
                    Accion.Parameters.AddWithValue("@param4", HC.PacienteId);
                    Accion.Parameters.AddWithValue("@param5", HC.MotivoConsulta);
                    Accion.Parameters.AddWithValue("@param6", HC.EnfermedadActual);
                    Accion.Parameters.AddWithValue("@param7", HC.EvolucionSintomas);
                    Accion.Parameters.AddWithValue("@param8", HC.AntecedentesRelevantes);
                    Accion.Parameters.AddWithValue("@param9", HC.MedicamentosActuales);
                    Accion.Parameters.AddWithValue("@param10", HC.AntecedentesRenales);
                    Accion.Parameters.AddWithValue("@param11", HC.AntecedentesCardioVasculares);
                    Accion.Parameters.AddWithValue("@param12", HC.Embarazo);
                    Accion.Parameters.AddWithValue("@param13", HC.ImplantesMetalicos);
                    Accion.Parameters.AddWithValue("@param14", HC.MenorEdad);
                    Accion.Parameters.AddWithValue("@param15", HC.Presion);
                    Accion.Parameters.AddWithValue("@param16", HC.Peso);
                    Accion.Parameters.AddWithValue("@param17", HC.GlassHow);
                    Accion.Parameters.AddWithValue("@param18", HC.Talla);
                    Accion.Parameters.AddWithValue("@param19", HC.FResp);
                    Accion.Parameters.AddWithValue("@param20", HC.FCar);
                    Accion.Parameters.AddWithValue("@param21", HC.RH);
                    Accion.Parameters.AddWithValue("@param22", HC.Conciencia);
                    Accion.Parameters.AddWithValue("@param23", HC.IMC);
                    Accion.Parameters.AddWithValue("@param24", HC.ObservacionExaMedico);
                    Accion.Parameters.AddWithValue("@param25", HC.TipoEstudio);
                    Accion.Parameters.AddWithValue("@param26", HC.MedioContraste);
                    Accion.Parameters.AddWithValue("@param27", HC.ReaccionAdversa);
                    Accion.Parameters.AddWithValue("@param28", HC.Tecnica);
                    Accion.Parameters.AddWithValue("@param29", HC.Hallazgos);
                    Accion.Parameters.AddWithValue("@param30", HC.DX1);
                    Accion.Parameters.AddWithValue("@param31", HC.DX2);
                    Accion.Parameters.AddWithValue("@param32", HC.DX3);
                    Accion.Parameters.AddWithValue("@param33", HC.NotaDX1);
                    Accion.Parameters.AddWithValue("@param34", HC.NotaDX2);
                    Accion.Parameters.AddWithValue("@param35", HC.NotaDX3);
                    Accion.Parameters.AddWithValue("@param36", HC.CausaExterna);
                    Accion.Parameters.AddWithValue("@param37", HC.ImpDX1);
                    Accion.Parameters.AddWithValue("@param38", HC.ImpDX2);
                    Accion.Parameters.AddWithValue("@param39", HC.ImpDX3);
                    Accion.Parameters.AddWithValue("@param40", HC.EstudioComplementario);
                    Accion.Parameters.AddWithValue("@param41", HC.ControlSeguimiento);
                    Accion.Parameters.AddWithValue("@param42", HC.Compañia);
                    Accion.Parameters.AddWithValue("@param43", HC.Aseguradora);
                    Accion.Parameters.AddWithValue("@param44", HC.Medico);
                    Accion.Parameters.AddWithValue("@param45", Convert.ToDateTime(HC.Fecha));
                    Accion.Parameters.AddWithValue("@param46", HC.HCAdm);
                    Accion.Parameters.AddWithValue("@param47", HC.HCCant);

                    int c = Accion.ExecuteNonQuery();
                    if (c > 0) { return true; }
                    return false;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return false;
            }
        }
        bool IRadiologia.ActualizaHistoria(CXN_HCRADIOLOGIA HC)
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

                    string Busqueda = (@"UPDATE CXN_HCRADIOLOGIA " +
                                      "SET PacId = @param1, " +
                                                         "PacienteNombre = @param2, " +
                                                         "PacienteTipoId = @param3, " +
                                                         "PacienteId = @param4, " +
                                                         "MotivoConsulta = @param5, " +
                                                         "EnfermedadActual = @param6, " +
                                                         "EvolucionSintomas = @param7, " +
                                                         "AntecedentesRelevantes = @param8, " +
                                                         "MedicamentosActuales = @param9, " +
                                                         "AntecedentesRenales = @param10, " +
                                                         "AntecedentesCardioVasculares = @param11, " +
                                                         "Embarazo = @param12, " +
                                                         "ImplantesMetalicos = @param13, " +
                                                         "MenorEdad = @param14, " +
                                                         "Presion = @param15, " +
                                                         "Peso = @param16, " +
                                                         "GlassHow = @param17, " +
                                                         "Talla = @param18, " +
                                                         "FResp = @param19, " +
                                                         "FCar = @param20, " +
                                                         "RH = @param21, " +
                                                         "Conciencia = @param22, " +
                                                         "IMC = @param23, " +
                                                         "ObservacionExaMedico = @param24, " +
                                                         "TipoEstudio = @param25, " +
                                                         "MedioContraste = @param26, " +
                                                         "ReaccionAdversa = @param27, " +
                                                         "Tecnica = @param28, " +
                                                         "Hallazgos = @param29, " +
                                                         "DX1 = @param30, " +
                                                         "DX2 = @param31, " +
                                                         "DX3 = @param32, " +
                                                         "NotaDX1 = @param33, " +
                                                         "NotaDX2 = @param34, " +
                                                         "NotaDX3 = @param35, " +
                                                         "CausaExterna = @param36, " +
                                                         "ImpDX1 = @param37, " +
                                                         "ImpDX2 = @param38, " +
                                                         "ImpDX3 = @param39, " +
                                                         "EstudioComplementario = @param40, " +
                                                         "ControlSeguimiento = @param41, " +
                                                         "Compañia = @param42, " +
                                                         "Aseguradora = @param43, " +
                                                         "Medico = @param44, " +
                                                         "Fecha = @param45, " +
                                                         "HCCant = @param46 " +
                                      "WHERE HCAdm = @param47 " +
                                      "AND HCCant = '0'");
                    SqlCommand Accion = new SqlCommand(Busqueda, con);

                    Accion.Parameters.AddWithValue("@param1", HC.PacId);
                    Accion.Parameters.AddWithValue("@param2", HC.PacienteNombre);
                    Accion.Parameters.AddWithValue("@param3", HC.PacienteTipoId);
                    Accion.Parameters.AddWithValue("@param4", HC.PacienteId);
                    Accion.Parameters.AddWithValue("@param5", HC.MotivoConsulta);
                    Accion.Parameters.AddWithValue("@param6", HC.EnfermedadActual);
                    Accion.Parameters.AddWithValue("@param7", HC.EvolucionSintomas);
                    Accion.Parameters.AddWithValue("@param8", HC.AntecedentesRelevantes);
                    Accion.Parameters.AddWithValue("@param9", HC.MedicamentosActuales);
                    Accion.Parameters.AddWithValue("@param10", HC.AntecedentesRenales);
                    Accion.Parameters.AddWithValue("@param11", HC.AntecedentesCardioVasculares);
                    Accion.Parameters.AddWithValue("@param12", HC.Embarazo);
                    Accion.Parameters.AddWithValue("@param13", HC.ImplantesMetalicos);
                    Accion.Parameters.AddWithValue("@param14", HC.MenorEdad);
                    Accion.Parameters.AddWithValue("@param15", HC.Presion);
                    Accion.Parameters.AddWithValue("@param16", HC.Peso);
                    Accion.Parameters.AddWithValue("@param17", HC.GlassHow);
                    Accion.Parameters.AddWithValue("@param18", HC.Talla);
                    Accion.Parameters.AddWithValue("@param19", HC.FResp);
                    Accion.Parameters.AddWithValue("@param20", HC.FCar);
                    Accion.Parameters.AddWithValue("@param21", HC.RH);
                    Accion.Parameters.AddWithValue("@param22", HC.Conciencia);
                    Accion.Parameters.AddWithValue("@param23", HC.IMC);
                    Accion.Parameters.AddWithValue("@param24", HC.ObservacionExaMedico);
                    Accion.Parameters.AddWithValue("@param25", HC.TipoEstudio);
                    Accion.Parameters.AddWithValue("@param26", HC.MedioContraste);
                    Accion.Parameters.AddWithValue("@param27", HC.ReaccionAdversa);
                    Accion.Parameters.AddWithValue("@param28", HC.Tecnica);
                    Accion.Parameters.AddWithValue("@param29", HC.Hallazgos);
                    Accion.Parameters.AddWithValue("@param30", HC.DX1);
                    Accion.Parameters.AddWithValue("@param31", HC.DX2);
                    Accion.Parameters.AddWithValue("@param32", HC.DX3);
                    Accion.Parameters.AddWithValue("@param33", HC.NotaDX1);
                    Accion.Parameters.AddWithValue("@param34", HC.NotaDX2);
                    Accion.Parameters.AddWithValue("@param35", HC.NotaDX3);
                    Accion.Parameters.AddWithValue("@param36", HC.CausaExterna);
                    Accion.Parameters.AddWithValue("@param37", HC.ImpDX1);
                    Accion.Parameters.AddWithValue("@param38", HC.ImpDX2);
                    Accion.Parameters.AddWithValue("@param39", HC.ImpDX3);
                    Accion.Parameters.AddWithValue("@param40", HC.EstudioComplementario);
                    Accion.Parameters.AddWithValue("@param41", HC.ControlSeguimiento);
                    Accion.Parameters.AddWithValue("@param42", HC.Compañia);
                    Accion.Parameters.AddWithValue("@param43", HC.Aseguradora);
                    Accion.Parameters.AddWithValue("@param44", HC.Medico);
                    Accion.Parameters.AddWithValue("@param45", Convert.ToDateTime(HC.Fecha));
                    Accion.Parameters.AddWithValue("@param46", HC.HCCant);
                    Accion.Parameters.AddWithValue("@param47", HC.HCAdm);

                    int c = Accion.ExecuteNonQuery();
                    if (c > 0) { return true; }
                    return false;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return false;
            }
        }
        CXN_HCRADIOLOGIA IRadiologia.getLastHistory(int Paciente, DateTime Fecha)
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

                    String Cargar_Hora = "SELECT TOP 1 * " +
                                         "FROM CXN_HCRADIOLOGIA " +
                                         "WHERE PacId = @param1 " +
                                         "AND Fecha <> @param2 " +
                                         "AND HCCant = @param3 " +
                                         "ORDER BY Fecha DESC";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", Paciente);
                        Carga_Command.Parameters.AddWithValue("@param2", Convert.ToDateTime(Fecha.Date));
                        Carga_Command.Parameters.AddWithValue("@param3", 1);

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.Read() == true)
                            {
                                CXN_HCRADIOLOGIA C = new CXN_HCRADIOLOGIA
                                {
                                    PacienteNombre = Lectura_Hora["PacienteNombre"].ToString(),
                                    PacienteTipoId = Lectura_Hora["PacienteTipoId"].ToString(),
                                    PacienteId = Lectura_Hora["PacienteId"].ToString(),
                                    MotivoConsulta = Lectura_Hora["MotivoConsulta"].ToString(),
                                    EnfermedadActual = Lectura_Hora["EnfermedadActual"].ToString(),
                                    EvolucionSintomas = Lectura_Hora["EvolucionSintomas"].ToString(),
                                    AntecedentesRelevantes = Lectura_Hora["AntecedentesRelevantes"].ToString(),
                                    MedicamentosActuales = Lectura_Hora["MedicamentosActuales"].ToString(),
                                    AntecedentesRenales = Lectura_Hora["AntecedentesRenales"].ToString(),
                                    AntecedentesCardioVasculares = Lectura_Hora["AntecedentesCardioVasculares"].ToString(),
                                    Embarazo = Lectura_Hora["Embarazo"].ToString(),
                                    ImplantesMetalicos = Lectura_Hora["ImplantesMetalicos"].ToString(),
                                    MenorEdad = Lectura_Hora["MenorEdad"].ToString(),
                                    Presion = Lectura_Hora["Presion"].ToString(),
                                    Peso = Lectura_Hora["Peso"].ToString(),
                                    GlassHow = Lectura_Hora["GlassHow"].ToString(),
                                    Talla = Lectura_Hora["Talla"].ToString(),
                                    FResp = Lectura_Hora["FResp"].ToString(),
                                    FCar = Lectura_Hora["FCar"].ToString(),
                                    RH = Lectura_Hora["RH"].ToString(),
                                    Conciencia = Lectura_Hora["Conciencia"].ToString(),
                                    IMC = Lectura_Hora["IMC"].ToString(),
                                    ObservacionExaMedico = Lectura_Hora["ObservacionExaMedico"].ToString(),
                                    TipoEstudio = Lectura_Hora["TipoEstudio"].ToString(),
                                    MedioContraste = Lectura_Hora["MedioContraste"].ToString(),
                                    ReaccionAdversa = Lectura_Hora["ReaccionAdversa"].ToString(),
                                    Tecnica = Lectura_Hora["Tecnica"].ToString(),
                                    Hallazgos = Lectura_Hora["Hallazgos"].ToString(),
                                    DX1 = Lectura_Hora["DX1"].ToString(),
                                    DX2 = Lectura_Hora["DX2"].ToString(),
                                    DX3 = Lectura_Hora["DX3"].ToString(),
                                    NotaDX1 = Lectura_Hora["NotaDX1"].ToString(),
                                    NotaDX2 = Lectura_Hora["NotaDX2"].ToString(),
                                    NotaDX3 = Lectura_Hora["NotaDX3"].ToString(),
                                    CausaExterna = Lectura_Hora["CausaExterna"].ToString(),
                                    ImpDX1 = Lectura_Hora["ImpDX1"].ToString(),
                                    ImpDX2 = Lectura_Hora["ImpDX2"].ToString(),
                                    ImpDX3 = Lectura_Hora["ImpDX3"].ToString(),
                                    EstudioComplementario = Lectura_Hora["EstudioComplementario"].ToString(),
                                    ControlSeguimiento = Lectura_Hora["ControlSeguimiento"].ToString()
                                };

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
        CXN_HCRADIOLOGIA IRadiologia.Ingresado(int Admision)
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

                    String Cargar_Hora = "SELECT TOP 1 * " +
                                         "FROM CXN_HCRADIOLOGIA " +
                                         "WHERE HCAdm = @param1 " +
                                         "AND HCCant = @param2";

                    using (SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con))
                    {
                        Carga_Command.Parameters.AddWithValue("@param1", Admision);
                        Carga_Command.Parameters.AddWithValue("@param2", 0);

                        using (SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader()))
                        {
                            if (Lectura_Hora.Read() == true)
                            {
                                CXN_HCRADIOLOGIA C = new CXN_HCRADIOLOGIA
                                {
                                    PacienteNombre = Lectura_Hora["PacienteNombre"].ToString(),
                                    PacienteTipoId = Lectura_Hora["PacienteTipoId"].ToString(),
                                    PacienteId = Lectura_Hora["PacienteId"].ToString(),
                                    MotivoConsulta = Lectura_Hora["MotivoConsulta"].ToString(),
                                    EnfermedadActual = Lectura_Hora["EnfermedadActual"].ToString(),
                                    EvolucionSintomas = Lectura_Hora["EvolucionSintomas"].ToString(),
                                    AntecedentesRelevantes = Lectura_Hora["AntecedentesRelevantes"].ToString(),
                                    MedicamentosActuales = Lectura_Hora["MedicamentosActuales"].ToString(),
                                    AntecedentesRenales = Lectura_Hora["AntecedentesRenales"].ToString(),
                                    AntecedentesCardioVasculares = Lectura_Hora["AntecedentesCardioVasculares"].ToString(),
                                    Embarazo = Lectura_Hora["Embarazo"].ToString(),
                                    ImplantesMetalicos = Lectura_Hora["ImplantesMetalicos"].ToString(),
                                    MenorEdad = Lectura_Hora["MenorEdad"].ToString(),
                                    Presion = Lectura_Hora["Presion"].ToString(),
                                    Peso = Lectura_Hora["Peso"].ToString(),
                                    GlassHow = Lectura_Hora["GlassHow"].ToString(),
                                    Talla = Lectura_Hora["Talla"].ToString(),
                                    FResp = Lectura_Hora["FResp"].ToString(),
                                    FCar = Lectura_Hora["FCar"].ToString(),
                                    RH = Lectura_Hora["RH"].ToString(),
                                    Conciencia = Lectura_Hora["Conciencia"].ToString(),
                                    IMC = Lectura_Hora["IMC"].ToString(),
                                    ObservacionExaMedico = Lectura_Hora["ObservacionExaMedico"].ToString(),
                                    TipoEstudio = Lectura_Hora["TipoEstudio"].ToString(),
                                    MedioContraste = Lectura_Hora["MedioContraste"].ToString(),
                                    ReaccionAdversa = Lectura_Hora["ReaccionAdversa"].ToString(),
                                    Tecnica = Lectura_Hora["Tecnica"].ToString(),
                                    Hallazgos = Lectura_Hora["Hallazgos"].ToString(),
                                    DX1 = Lectura_Hora["DX1"].ToString(),
                                    DX2 = Lectura_Hora["DX2"].ToString(),
                                    DX3 = Lectura_Hora["DX3"].ToString(),
                                    NotaDX1 = Lectura_Hora["NotaDX1"].ToString(),
                                    NotaDX2 = Lectura_Hora["NotaDX2"].ToString(),
                                    NotaDX3 = Lectura_Hora["NotaDX3"].ToString(),
                                    CausaExterna = Lectura_Hora["CausaExterna"].ToString(),
                                    ImpDX1 = Lectura_Hora["ImpDX1"].ToString(),
                                    ImpDX2 = Lectura_Hora["ImpDX2"].ToString(),
                                    ImpDX3 = Lectura_Hora["ImpDX3"].ToString(),
                                    EstudioComplementario = Lectura_Hora["EstudioComplementario"].ToString(),
                                    ControlSeguimiento = Lectura_Hora["ControlSeguimiento"].ToString(),
                                    Fecha = Convert.ToDateTime(Lectura_Hora["Fecha"])
                                };

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
    }
}
