using Domain.CXN;
using Persistence.CXN.Interfaces;
using System.Data.SqlClient;
using System.Data;
using System;
using System.Collections.Generic;
using Domain;

namespace Persistence.CXN.Metodos
{
    public class MRoles : IRoles
    {
        CXN_ROLES IRoles.getRoles(string User)  
        {
            try
            {
                Dictionary<string, string> dataConection = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(dataConection["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    String Query = "SELECT * " +
                                   "FROM CXN_ROLES " +
                                   "WHERE Rol_R_User = @param1";

                    using (SqlCommand Commando = new SqlCommand(Query, con))
                    {
                        Commando.Parameters.AddWithValue("@param1", User);

                        using (SqlDataReader Reader = (Commando.ExecuteReader()))
                        {
                            if (Reader.Read() == true)
                            {
                                CXN_ROLES DH = new CXN_ROLES
                                {
                                    Rol_A_Adherencia = Reader["Rol_A_Adherencia"].ToString(),
                                    Rol_A_Anulaciones = Reader["Rol_A_Anulaciones"].ToString(),
                                    Rol_A_Autorizaciones = Reader["Rol_A_Autorizaciones"].ToString(),
                                    Rol_A_Cargos = Reader["Rol_A_Cargos"].ToString(),
                                    Rol_A_Facturacion = Reader["Rol_A_Facturacion"].ToString(),
                                    Rol_A_Formatos = Reader["Rol_A_Formatos"].ToString(),
                                    Rol_A_Inventario = Reader["Rol_A_Inventario"].ToString(),
                                    Rol_A_Mensajero = Reader["Rol_A_Mensajero"].ToString(),
                                    Rol_A_Productos = Reader["Rol_A_Productos"].ToString(),
                                    Rol_A_RIPS = Reader["Rol_A_RIPS"].ToString(),
                                    Rol_A_SMSEmail = Reader["Rol_A_SMSEmail"].ToString(),
                                    Rol_O_Bodegas = Reader["Rol_O_Bodegas"].ToString(),
                                    Rol_R_Agenda_R = Reader["Rol_R_Agenda_R"].ToString(),
                                    Rol_Id = Convert.ToInt32(Reader["Rol_Id"]),
                                    Rol_R_Asistencia = Reader["Rol_R_Asistencia"].ToString(),
                                    Rol_O_CIE10 = Reader["Rol_O_CIE10"].ToString(),
                                    Rol_O_Compañias = Reader["Rol_O_Compañias"].ToString(),
                                    Rol_O_Convenios = Reader["Rol_O_Convenios"].ToString(),
                                    Rol_O_Festivos = Reader["Rol_O_Festivos"].ToString(),
                                    Rol_O_Horarios = Reader["Rol_O_Horarios"].ToString(),
                                    Rol_O_Mensajero = Reader["Rol_O_Mensajero"].ToString(),
                                    Rol_O_Proveedores = Reader["Rol_O_Proveedores"].ToString(),
                                    Rol_O_UsuariosSystem = Reader["Rol_O_UsuariosSystem"].ToString(),
                                    Rol_R_Cargos = Reader["Rol_R_Cargos"].ToString(),
                                    Rol_R_Copias = Reader["Rol_R_Copias"].ToString(),
                                    Rol_R_Cotizaciones = Reader["Rol_R_Cotizaciones"].ToString(),
                                    Rol_R_CrearPacientes = Reader["Rol_R_CrearPacientes"].ToString(),
                                    Rol_R_Mensajero = Reader["Rol_R_Mensajero"].ToString(),
                                    Rol_R_Precios = Reader["Rol_R_Precios"].ToString(),
                                    Rol_R_User = Reader["Rol_R_User"].ToString(),
                                    Rol_R_Ventas = Reader["Rol_R_Ventas"].ToString(),
                                    Rol_A_FacElectron = Reader["Rol_A_FacElectron"].ToString(),
                                    AdminFactura = Reader["AdminFactura"].ToString(),
                                    AdminFacturaAbierta = Reader["AdminFacturaAbierta"].ToString(),
                                    AdminElectronica = Reader["AdminElectronica"].ToString(),
                                    AdminReportes = Reader["AdminReportes"].ToString(),
                                    AdminHomologos = Reader["AdminHomologos"].ToString(),
                                    AdminGPacientes = Reader["AdminGPacientes"].ToString(),
                                    AdminGrupal = Reader["AdminGrupal"].ToString(),
                                    AdminHelisa = Reader["AdminHelisa"].ToString(),
                                    AdminGenerarToken = Reader["AdminGenerarToken"].ToString(),
                                    AdminFHIR = Reader["AdminFHIR"].ToString(),
                                    AdminReportesPagos = Reader["AdminReportesPagos"].ToString()
                                };

                                return DH;
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
        bool IRoles.updateRoles(CXN_ROLES R)
        {
            try
            {
                Dictionary<string, string> dataConection = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(dataConection["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    SqlCommand Busqueda = new SqlCommand(@"UPDATE CXN_ROLES " +
                                                          "SET  " +
                                                          "Rol_R_Agenda_R = @param1, " +
                                                          "Rol_R_CrearPacientes = @param2, " +
                                                          "Rol_R_Asistencia = @param3, " +
                                                          "Rol_R_Ventas = @param4, " +
                                                          "Rol_R_Cargos = @param5, " +
                                                          "Rol_R_Copias = @param6, " +
                                                          "Rol_R_Precios = @param7, " +
                                                          "Rol_R_Mensajero = @param8, " +
                                                          "Rol_R_Cotizaciones = @param9, " +
                                                          "Rol_A_SMSEmail = @param10, " +
                                                          "Rol_A_Mensajero = @param11, " +
                                                          "Rol_A_Formatos = @param12, " +
                                                          "Rol_A_Adherencia = @param13, " +
                                                          "Rol_A_Productos = @param14, " +
                                                          "Rol_A_Anulaciones = @param15, " +
                                                          "Rol_A_RIPS = @param16, " +
                                                          "Rol_A_Cargos = @param17, " +
                                                          "Rol_A_Facturacion = @param18, " +
                                                          "Rol_A_Inventario = @param19, " +
                                                          "Rol_A_Autorizaciones = @param20, " +
                                                          "Rol_O_Mensajero = @param21, " +
                                                          "Rol_O_Bodegas = @param22, " +
                                                          "Rol_O_CIE10 = @param23, " +
                                                          "Rol_O_Convenios = @param24, " +
                                                          "Rol_O_UsuariosSystem = @param25, " +
                                                          "Rol_O_Festivos = @param26, " +
                                                          "Rol_O_Proveedores = @param27, " +
                                                          "Rol_O_Horarios = @param28, " +
                                                          "Rol_O_Compañias = @param29, " +
                                                          "AdminFactura = @param30, " +
                                                          "AdminFacturaAbierta = @param31, " +
                                                          "AdminElectronica = @param32, " +
                                                          "AdminReportes = @param33, " +
                                                          "AdminHomologos = @param34, " +
                                                          "AdminGPacientes = @param35, " +
                                                          "AdminGrupal = @param36, " +
                                                          "AdminHelisa = @param37, " +
                                                          "Rol_A_FacElectron = @param38, " +
                                                          "AdminGenerarToken = @param39, " +
                                                          "AdminFHIR = @param40, " +
                                                          "AdminReportesPagos = @param41 " +
                                                          "WHERE Rol_R_User = '" + R.Rol_R_User + "'", con);

                    Busqueda.Parameters.AddWithValue("@param1", R.Rol_R_Agenda_R);
                    Busqueda.Parameters.AddWithValue("@param2", R.Rol_R_CrearPacientes);
                    Busqueda.Parameters.AddWithValue("@param3",R.Rol_R_Asistencia);
                    Busqueda.Parameters.AddWithValue("@param4",R.Rol_R_Ventas);
                    Busqueda.Parameters.AddWithValue("@param5",R.Rol_R_Cargos);
                    Busqueda.Parameters.AddWithValue("@param6",R.Rol_R_Copias);
                    Busqueda.Parameters.AddWithValue("@param7",R.Rol_R_Precios);
                    Busqueda.Parameters.AddWithValue("@param8",R.Rol_R_Mensajero);
                    Busqueda.Parameters.AddWithValue("@param9",R.Rol_R_Cotizaciones);
                    Busqueda.Parameters.AddWithValue("@param10",R.Rol_A_SMSEmail);
                    Busqueda.Parameters.AddWithValue("@param11",R.Rol_A_Mensajero);
                    Busqueda.Parameters.AddWithValue("@param12",R.Rol_A_Formatos);
                    Busqueda.Parameters.AddWithValue("@param13",R.Rol_A_Adherencia);
                    Busqueda.Parameters.AddWithValue("@param14",R.Rol_A_Productos);
                    Busqueda.Parameters.AddWithValue("@param15",R.Rol_A_Anulaciones);
                    Busqueda.Parameters.AddWithValue("@param16",R.Rol_A_RIPS);
                    Busqueda.Parameters.AddWithValue("@param17",R.Rol_A_Cargos);
                    Busqueda.Parameters.AddWithValue("@param18",R.Rol_A_Facturacion);
                    Busqueda.Parameters.AddWithValue("@param19",R.Rol_A_Inventario);
                    Busqueda.Parameters.AddWithValue("@param20",R.Rol_A_Autorizaciones);
                    Busqueda.Parameters.AddWithValue("@param21",R.Rol_O_Mensajero);
                    Busqueda.Parameters.AddWithValue("@param22",R.Rol_O_Bodegas);
                    Busqueda.Parameters.AddWithValue("@param23",R.Rol_O_CIE10);
                    Busqueda.Parameters.AddWithValue("@param24",R.Rol_O_Convenios);
                    Busqueda.Parameters.AddWithValue("@param25",R.Rol_O_UsuariosSystem);
                    Busqueda.Parameters.AddWithValue("@param26",R.Rol_O_Festivos);
                    Busqueda.Parameters.AddWithValue("@param27",R.Rol_O_Proveedores);
                    Busqueda.Parameters.AddWithValue("@param28",R.Rol_O_Horarios);
                    Busqueda.Parameters.AddWithValue("@param29",R.Rol_O_Compañias);
                    Busqueda.Parameters.AddWithValue("@param30", R.AdminFactura);
                    Busqueda.Parameters.AddWithValue("@param31", R.AdminFacturaAbierta);
                    Busqueda.Parameters.AddWithValue("@param32", R.AdminElectronica);
                    Busqueda.Parameters.AddWithValue("@param33", R.AdminReportes);
                    Busqueda.Parameters.AddWithValue("@param34", R.AdminHomologos);
                    Busqueda.Parameters.AddWithValue("@param35", R.AdminGPacientes);
                    Busqueda.Parameters.AddWithValue("@param36", R.AdminGrupal);
                    Busqueda.Parameters.AddWithValue("@param37", R.AdminHelisa);
                    Busqueda.Parameters.AddWithValue("@param38", R.Rol_A_FacElectron);
                    Busqueda.Parameters.AddWithValue("@param39", R.AdminGenerarToken);
                    Busqueda.Parameters.AddWithValue("@param40", R.AdminFHIR);
                    Busqueda.Parameters.AddWithValue("@param41", R.AdminReportesPagos);

                    int s = Busqueda.ExecuteNonQuery();
                    if (s > 0)
                    {
                        return true;
                    }

                    return false;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return false;
            }          
        }
        bool IRoles.insertRoles(CXN_ROLES R)
        {
            try
            {
                Dictionary<string, string> dataConection = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(dataConection["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    DateTime Hoy = DateTime.Now;
                    DateTime Ven_Fecha = DateTime.Now.Date;

                    SqlCommand cmd = new SqlCommand(@"Insert into CXN_ROLES ( " + //param1
                                                          "Rol_R_Agenda_R, " + //param2
                                                          "Rol_R_CrearPacientes, " + //param3
                                                          "Rol_R_Asistencia, " + //param4
                                                          "Rol_R_Ventas, " + //param5
                                                          "Rol_R_Cargos, " + //param6
                                                          "Rol_R_Copias, " + //param7
                                                          "Rol_R_Precios, " + //param           
                                                          "Rol_R_Mensajero, " + //param10
                                                          "Rol_R_Cotizaciones, " + //param11
                                                          "Rol_A_SMSEmail, " +
                                                          "Rol_A_Mensajero, " +
                                                          "Rol_A_Formatos, " +
                                                          "Rol_A_Adherencia, " +
                                                          "Rol_A_Productos, " +
                                                          "Rol_A_Anulaciones, " +
                                                          "Rol_A_RIPS, " +
                                                          "Rol_A_Cargos, " +
                                                          "Rol_A_Facturacion, " +
                                                          "Rol_A_Inventario, " +
                                                          "Rol_A_Autorizaciones, " +
                                                          "Rol_O_Mensajero, " +
                                                          "Rol_O_Bodegas, " +
                                                          "Rol_O_CIE10, " +
                                                          "Rol_O_Convenios, " +
                                                          "Rol_O_UsuariosSystem, " +
                                                          "Rol_O_Festivos, " +
                                                          "Rol_O_Proveedores, " +
                                                          "Rol_O_Horarios, " +
                                                          "Rol_O_Compañias, " +
                                                          "Rol_R_User, " +
                                                          "AdminFactura, " +
                                                          "AdminFacturaAbierta, " +
                                                          "AdminElectronica, " +
                                                          "AdminReportes, " +
                                                          "AdminHomologos, " +
                                                          "AdminGPacientes, " +
                                                          "AdminGrupal, " +
                                                          "AdminHelisa, " +
                                                          "Rol_A_FacElectron, " +
                                                          "AdminGenerarToken, " +
                                                          "AdminFHIR, " +
                                                          "AdminReportesPagos) " + 
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
                                                          "@param12, " +
                                                           "@param13, " +
                                                            "@param14, " +
                                                             "@param15, " +
                                                              "@param16, " +
                                                               "@param17, " +
                                                                "@param18, " +
                                                                 "@param19, " +
                                                                  "@param20, " +
                                                                   "@param21, " +
                                                                    "@param22, " +
                                                                     "@param23, " +
                                                                      "@param24, " +
                                                                       "@param25, " +
                                                                        "@param26, " +
                                                                         "@param27, " +
                                                                          "@param28, " +
                                                                          "@param29, " +
                                                                          "@param30, " +
                                                                          "@param31, " +
                                                                          "@param32, " +
                                                                          "@param33, " +
                                                                          "@param34, " +
                                                                          "@param35, " +
                                                                          "@param36, " +
                                                                          "@param37, " +
                                                          "@param38, " +
                                                          "@param39, " +
                                                          "@param40, " +
                                                          "@param41, " +
                                                          "@param42)", con); // Hor_Pac_Sal

                    cmd.Parameters.AddWithValue("@param1", R.Rol_R_Agenda_R);
                    cmd.Parameters.AddWithValue("@param2", R.Rol_R_CrearPacientes);
                    cmd.Parameters.AddWithValue("@param3", R.Rol_R_Asistencia);
                    cmd.Parameters.AddWithValue("@param4", R.Rol_R_Ventas);
                    cmd.Parameters.AddWithValue("@param5", R.Rol_R_Cargos);
                    cmd.Parameters.AddWithValue("@param6", R.Rol_R_Copias);
                    cmd.Parameters.AddWithValue("@param7", R.Rol_R_Precios); 
                    cmd.Parameters.AddWithValue("@param8", R.Rol_R_Mensajero);
                    cmd.Parameters.AddWithValue("@param9", R.Rol_R_Cotizaciones);
                    cmd.Parameters.AddWithValue("@param10", R.Rol_A_SMSEmail);
                    cmd.Parameters.AddWithValue("@param11", R.Rol_A_Mensajero);
                    cmd.Parameters.AddWithValue("@param12", R.Rol_A_Formatos);
                    cmd.Parameters.AddWithValue("@param13", R.Rol_A_Adherencia);
                    cmd.Parameters.AddWithValue("@param14", R.Rol_A_Productos);
                    cmd.Parameters.AddWithValue("@param15", R.Rol_A_Anulaciones);
                    cmd.Parameters.AddWithValue("@param16", R.Rol_A_RIPS);
                    cmd.Parameters.AddWithValue("@param17", R.Rol_A_Cargos);
                    cmd.Parameters.AddWithValue("@param18", R.Rol_A_Facturacion);
                    cmd.Parameters.AddWithValue("@param19", R.Rol_A_Inventario);
                    cmd.Parameters.AddWithValue("@param20", R.Rol_A_Autorizaciones);
                    cmd.Parameters.AddWithValue("@param21", R.Rol_O_Mensajero);
                    cmd.Parameters.AddWithValue("@param22", R.Rol_O_Bodegas);
                    cmd.Parameters.AddWithValue("@param23", R.Rol_O_CIE10);
                    cmd.Parameters.AddWithValue("@param24", R.Rol_O_Convenios);
                    cmd.Parameters.AddWithValue("@param25", R.Rol_O_UsuariosSystem);
                    cmd.Parameters.AddWithValue("@param26", R.Rol_O_Festivos);
                    cmd.Parameters.AddWithValue("@param27", R.Rol_O_Proveedores);
                    cmd.Parameters.AddWithValue("@param28", R.Rol_O_Horarios);
                    cmd.Parameters.AddWithValue("@param29", R.Rol_O_Compañias);
                    cmd.Parameters.AddWithValue("@param30", R.Rol_R_User);
                    cmd.Parameters.AddWithValue("@param31", R.AdminFactura);
                    cmd.Parameters.AddWithValue("@param32", R.AdminFacturaAbierta);
                    cmd.Parameters.AddWithValue("@param33", R.AdminElectronica);
                    cmd.Parameters.AddWithValue("@param34", R.AdminReportes);
                    cmd.Parameters.AddWithValue("@param35", R.AdminHomologos);
                    cmd.Parameters.AddWithValue("@param36", R.AdminGPacientes);
                    cmd.Parameters.AddWithValue("@param37", R.AdminGrupal);
                    cmd.Parameters.AddWithValue("@param38", R.AdminHelisa);
                    cmd.Parameters.AddWithValue("@param39", R.Rol_A_FacElectron);
                    cmd.Parameters.AddWithValue("@param40", R.AdminGenerarToken);
                    cmd.Parameters.AddWithValue("@param41", R.AdminFHIR);
                    cmd.Parameters.AddWithValue("@param42", R.AdminReportesPagos);

                    int s = cmd.ExecuteNonQuery();
                    if (s > 0)
                    {
                        return true;
                    }

                    return false;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.GetType().Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = "BackEnd" }; OverridesExtern.GenerarTXTException(T);
                return false;
            }
        }
    }
}
