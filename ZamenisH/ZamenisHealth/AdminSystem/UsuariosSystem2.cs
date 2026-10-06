using Domain;
using Domain.CXN;
using FormAndControls;
using Newtonsoft.Json.Linq;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Collections.Generic;
using System.Linq;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.AdminSystem
{
    public partial class UsuariosSystem2 : Forma2
    {
        private static readonly ILogin repoLogin = new MLogin();
        private static readonly IRoles repoRoles = new MRoles();
        private static readonly IMensajeria repoMens = new MMensajeria();

        private string Funcionario;
        private MensajesGeneral MG;

        public UsuariosSystem2()
        {
            InitializeComponent();
        }
        private void btnZamenis1_ButtonClick(object sender, EventArgs e)
        {
            try
            {
                Domain.Recepcion R = new Domain.Recepcion
                {
                    AgendaMedica = checkBox1.Checked ? "A" : "I",
                    Ventas = checkBox4.Checked ? "A" : "I",
                    ListarPrecios = checkBox7.Checked ? "A" : "I",
                    CrearEditarPaciente = checkBox2.Checked ? "A" : "I",
                    Cargos = checkBox5.Checked ? "A" : "I",
                    Cotizaciones = checkBox9.Checked ? "A" : "I",
                    VerAsistencia = checkBox3.Checked ? "A" : "I",
                    CopiaDocumentos = checkBox6.Checked ? "A" : "I"
                };

                Domain.Administracion A = new Domain.Administracion
                {
                    Facturacion = checkBox18.Checked ? "A" : "I",
                    Anulaciones = checkBox15.Checked ? "A" : "I",
                    Formatos = checkBox12.Checked ? "A" : "I",
                    Cargos = checkBox17.Checked ? "A" : "I",
                    Inventarios = checkBox19.Checked ? "A" : "I",
                    Mensajero = checkBox11.Checked ? "A" : "I",
                    Rips = checkBox16.Checked ? "A" : "I",
                    Adherencia = checkBox13.Checked ? "A" : "I",
                    Envios = checkBox10.Checked ? "A" : "I",
                    BarCodes = checkBox41.Checked ? "A" : "I",
                    Autorizaciones = checkBox20.Checked ? "A" : "I",
                    RDA = checkBox39.Checked ? "A" : "I",
                    AdministracionDetalles = new AdministracionDetalles
                    {
                        GenerarFactura = checkBox37.Checked ? "A" : "I",
                        Reportes = checkBox32.Checked ? "A" : "I",
                        DetalleGrupal = checkBox33.Checked ? "A" : "I",
                        FacturaAbierta = checkBox35.Checked ? "A" : "I",
                        Homologos = checkBox36.Checked ? "A" : "I",
                        FacturacionElectronica = checkBox34.Checked ? "A" : "I",
                        ReportesPagos = checkBox40.Checked ? "A" : "I"
                    }
                };

                Domain.Opciones O = new Domain.Opciones
                {
                    Compañias = checkBox49.Checked ? "A" : "I",
                    CrearEditarUsuario = checkBox46.Checked ? "A" : "I",
                    Productos = checkBox43.Checked ? "A" : "I",
                    Horarios = checkBox48.Checked ? "A" : "I",
                    Convenios = checkBox45.Checked ? "A" : "I",
                    Preferencias = checkBox42.Checked ? "A" : "I",
                    Festivos = checkBox47.Checked ? "A" : "I",
                    CIE10 = checkBox44.Checked ? "A" : "I",
                    ECuentas = checkBox50.Checked ? "A" : "I",
                    Compras = checkBox51.Checked ? "A" : "I",
                    Bodegas = checkBox52.Checked ? "A" : "I"
                };

                Domain.Enfermeria E = new Domain.Enfermeria
                {
                    Notas = checkBox30.Checked ? "A" : "I",
                    Plantillas = checkBox27.Checked ? "A" : "I",
                    Imagenes = checkBox25.Checked ? "A" : "I",
                    NAclaratoria = checkBox29.Checked ? "A" : "I",
                    SearchImages = checkBox21.Checked ? "A" : "I",
                    Inventario = checkBox24.Checked ? "A" : "I",
                    CManejo = checkBox28.Checked ? "A" : "I",
                    Registros = checkBox26.Checked ? "A" : "I",
                    Estadisticas = checkBox23.Checked ? "A" : "I",
                    Cargos = checkBox22.Checked ? "A" : "I",
                    SubirDocumentos = checkBox8.Checked ? "A" : "I",
                    DocumentosWEB = checkBox14.Checked ? "A" : "I"
                };

                Domain.MedicinaGeneral MGe = new Domain.MedicinaGeneral
                {
                    Historia = checkBox61.Checked ? "A" : "I",
                    Registros = checkBox58.Checked ? "A" : "I",
                    Retomar = checkBox55.Checked ? "A" : "I",
                    CManejo = checkBox60.Checked ? "A" : "I",
                    Subir = checkBox57.Checked ? "A" : "I",
                    Estadistica = checkBox54.Checked ? "A" : "I",
                    Nota = checkBox59.Checked ? "A" : "I",
                    Inventario = checkBox56.Checked ? "A" : "I",
                    DocumentosWEB = checkBox53.Checked ? "A" : "I",
                    GrabarImagenes = checkBox38.Checked ? "A" : "I",
                    Ordenes = checkBox31.Checked ? "A" : "I",
                    Solicitudes = checkBox62.Checked ? "A" : "I",
                    BuscarImagenes = checkBox64.Checked ? "A" : "I",
                    Consentimientos = checkBox65.Checked ? "A" : "I",
                    Salidas = checkBox66.Checked ? "A" : "I"
                };

                Domain.Gerencial G = new Domain.Gerencial
                {
                    FacturaPaciente = checkBox77.Checked ? "A" : "I",
                    Reportes = checkBox74.Checked ? "A" : "I",
                    Consentimientos = checkBox72.Checked ? "A" : "I",
                    EliminarCierreCaja = checkBox76.Checked ? "A" : "I"
                };

                Domain.Fisiatria F = new Domain.Fisiatria
                {
                    CrearHistoria = checkBox81.Checked ? "A" : "I",
                    Retomar = checkBox78.Checked ? "A" : "I",
                    SubirHistoria = checkBox73.Checked ? "A" : "I",
                    NotaAclaratoria = checkBox80.Checked ? "A" : "I",
                    CompletarJuntas = checkBox68.Checked ? "A" : "I",
                    FirmarHistorias = checkBox71.Checked ? "A" : "I",
                    CrearOrdenes = checkBox79.Checked ? "A" : "I",
                    BuscarRegistros = checkBox75.Checked ? "A" : "I"
                };

                CXN_DESKTOP_ROLES Dr = new CXN_DESKTOP_ROLES
                {
                    Recepcion = new JObject
                    {
                        ["Recepción"] = JObject.FromObject(R)
                    }.ToString(),

                    Administracion = new JObject
                    {
                        ["Administracion"] = JObject.FromObject(A)
                    }.ToString(),

                    Opciones = new JObject
                    {
                        ["Opciones"] = JObject.FromObject(O)
                    }.ToString(),

                    Enfermeria = new JObject
                    {
                        ["Enfermeria"] = JObject.FromObject(E)
                    }.ToString(),

                    MedicinaGeneral = new JObject
                    {
                        ["MedicinaGeneral"] = JObject.FromObject(MGe)
                    }.ToString(),

                    Gerencial = new JObject
                    {
                        ["Gerencial"] = JObject.FromObject(G)
                    }.ToString(),

                    Fisiatria = new JObject
                    {
                        ["Fisiatria"] = JObject.FromObject(F)
                    }.ToString(),

                    Usuario = this.Funcionario
                };

                bool result = repoRoles.SaveRoles(Dr, Funcionario);
                if (result == true)
                {
                    MG = new MensajesGeneral()
                    {
                        Mensaje = "Roles actualizados correctamente",
                        TipoImagen = 1
                    };
                    MG.ShowDialog();

                    this.Close();
                }
                else
                {
                    MG = new MensajesGeneral()
                    {
                        Mensaje = "Error grave actualizando roles, contacte al administrador",
                        TipoImagen = 1000
                    };
                    MG.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void UsuariosSystem2_Load(object sender, EventArgs e)
        {
            try
            {
                Titulo.Text = "Usuarios";
                SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";
                
                List<CXN_LOGIN> getUsers = repoLogin.getUsersforSendMessage();
                if (getUsers != null)
                {
                    getUsers = getUsers.Where(x => x.Log_Habilitado == "A").ToList();

                    foreach (var i in getUsers)
                    {
                        comboBox1.Items.Add(i.Log_PrimerA + " " + i.Log_SegundoA + " " + i.Log_PrimerN + " " + i.Log_SegundoN);
                    }

                    comboBox1.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                this.Funcionario = repoMens.getUsertoSendMessage(comboBox1.Text);
                if (this.Funcionario == "" || this.Funcionario == null)
                {
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "Error grave obteniendo datos del usuario, contacte al administrador";
                    MG.ShowDialog();

                    this.Dispose();
                    this.Close();
                }
                else
                {                   
                    CXN_LOGIN getPermisos = repoLogin.getUser(this.Funcionario);
                    if (getPermisos != null)
                    {
                        groupBox1.Visible = getPermisos.Log_Rol_Recepcion == "A" ? true : false; //Recepcion
                        groupBox2.Visible = getPermisos.Log_Rol_AdminI == "A" ? true : false; //Administracion
                        groupBox4.Visible = getPermisos.Log_Rol_AdminI == "A" ? true : false; //Administracion
                        groupBox5.Visible = getPermisos.Log_Rol_Admin == "A" ? true : false; //Opciones
                        groupBox3.Visible = getPermisos.Log_Rol_Enfermero == "A" ? true : false; //Enfermeria
                        groupBox6.Visible = getPermisos.Log_Rol_MedGen == "A" ? true : false; //Enfermeria
                        groupBox7.Visible = getPermisos.Log_Rol_Gerencial == "A" ? true : false; //Gerencial
                        groupBox8.Visible = getPermisos.Log_Rol_FI == "A" ? true : false; //Fisiatria

                        CXN_DESKTOP_ROLES Rol = repoRoles.getDesktopRoles(this.Funcionario);
                        if (Rol == null)
                        {
                            //Si no esta en la tabla de Roles se muestra todo
                            groupBox1.Visible = true; //Recepcion
                            groupBox2.Visible = true; //Administracion
                            groupBox4.Visible = true; //Administracion2
                            groupBox5.Visible = true; //Opciones
                            groupBox3.Visible = true; //Enfermeria
                            groupBox6.Visible = true; //Enfermeria

                            //Recepcion
                            checkBox1.Checked = false;
                            checkBox4.Checked = false;
                            checkBox7.Checked = false;
                            checkBox2.Checked = false;
                            checkBox5.Checked = false;
                            checkBox9.Checked = false;
                            checkBox3.Checked = false;
                            checkBox6.Checked = false;

                            //Administracion
                            checkBox15.Checked = false;
                            checkBox15.Checked = false;
                            checkBox12.Checked = false;
                            checkBox17.Checked = false;
                            checkBox18.Checked = false;
                            checkBox19.Checked = false;
                            checkBox11.Checked = false;
                            checkBox16.Checked = false;
                            checkBox13.Checked = false;
                            checkBox10.Checked = false;
                            checkBox41.Checked = false;
                            checkBox20.Checked = false;
                            checkBox39.Checked = false;
                            checkBox37.Checked = false;
                            checkBox32.Checked = false;
                            checkBox33.Checked = false;
                            checkBox35.Checked = false;
                            checkBox36.Checked = false;
                            checkBox34.Checked = false;
                            checkBox40.Checked = false;

                            //Opciones
                            checkBox49.Checked = false;
                            checkBox46.Checked = false;
                            checkBox43.Checked = false;
                            checkBox48.Checked = false;
                            checkBox45.Checked = false;
                            checkBox42.Checked = false;
                            checkBox47.Checked = false;
                            checkBox44.Checked = false;
                            checkBox50.Checked = false;
                            checkBox51.Checked = false;
                            checkBox52.Checked = false;

                            //Enfermeria
                            checkBox30.Checked = false;
                            checkBox27.Checked = false;
                            checkBox25.Checked = false;
                            checkBox29.Checked = false;
                            checkBox21.Checked = false;
                            checkBox24.Checked = false;
                            checkBox28.Checked = false;
                            checkBox26.Checked = false;
                            checkBox23.Checked = false;
                            checkBox22.Checked = false;
                            checkBox8.Checked = false;
                            checkBox14.Checked = false;

                            //Medicina general
                            checkBox61.Checked = false;
                            checkBox58.Checked = false;
                            checkBox55.Checked = false;
                            checkBox60.Checked = false;
                            checkBox57.Checked = false;
                            checkBox54.Checked = false;
                            checkBox59.Checked = false;
                            checkBox56.Checked = false;
                            checkBox53.Checked = false;
                            checkBox38.Checked = false;
                            checkBox31.Checked = false;
                            checkBox62.Checked = false;
                            checkBox64.Checked = false;
                            checkBox65.Checked = false;
                            checkBox66.Checked = false;

                            //Gerencial
                            checkBox77.Checked = false;
                            checkBox74.Checked = false;
                            checkBox72.Checked = false;
                            checkBox76.Checked = false;

                            //Fisiatria
                            checkBox81.Checked = false;
                            checkBox78.Checked = false;
                            checkBox73.Checked = false;
                            checkBox80.Checked = false;
                            checkBox68.Checked = false;
                            checkBox71.Checked = false;
                            checkBox79.Checked = false;
                            checkBox75.Checked = false;
                        }
                        else
                        {
                            //Si existe en la tabla de roles se verifica Json
                            JObject obj = JObject.Parse(Rol.Recepcion);
                            JObject obj2 = JObject.Parse(Rol.Administracion);
                            JObject obj3 = JObject.Parse(Rol.Opciones);
                            JObject obj4 = JObject.Parse(Rol.Enfermeria);
                            JObject obj5 = JObject.Parse(Rol.MedicinaGeneral);
                            JObject obj6 = JObject.Parse(Rol.Gerencial);
                            JObject obj7 = JObject.Parse(Rol.Fisiatria);

                            //ROL RECEPCION
                            if (obj["Recepción"] != null)
                            {
                                checkBox1.Checked = obj["Recepción"]["AgendaMedica"] != null ?
                                                    obj["Recepción"]["AgendaMedica"].ToString() == "A" ? true : false : false;
                                checkBox4.Checked = obj["Recepción"]["Ventas"] != null ?
                                                   obj["Recepción"]["Ventas"].ToString() == "A" ? true : false : false;
                                checkBox7.Checked = obj["Recepción"]["ListarPrecios"] != null ?
                                                  obj["Recepción"]["ListarPrecios"].ToString() == "A" ? true : false : false;
                                checkBox2.Checked = obj["Recepción"]["CrearEditarPaciente"] != null ?
                                                  obj["Recepción"]["CrearEditarPaciente"].ToString() == "A" ? true : false : false;
                                checkBox5.Checked = obj["Recepción"]["Cargos"] != null ?
                                                  obj["Recepción"]["Cargos"].ToString() == "A" ? true : false : false;
                                checkBox9.Checked = obj["Recepción"]["Cotizaciones"] != null ?
                                                 obj["Recepción"]["Cotizaciones"].ToString() == "A" ? true : false : false;
                                checkBox3.Checked = obj["Recepción"]["VerAsistencia"] != null ?
                                                 obj["Recepción"]["VerAsistencia"].ToString() == "A" ? true : false : false;
                                checkBox6.Checked = obj["Recepción"]["CopiaDocumentos"] != null ?
                                                obj["Recepción"]["CopiaDocumentos"].ToString() == "A" ? true : false : false;
                            }
                            else
                            {
                                checkBox1.Checked = false;
                                checkBox4.Checked = false;
                                checkBox7.Checked = false;
                                checkBox2.Checked = false;
                                checkBox5.Checked = false;
                                checkBox9.Checked = false;
                                checkBox3.Checked = false;
                                checkBox6.Checked = false;
                            }
                            //ROL ADMINISTRACION
                            if (obj2["Administracion"] != null)
                            {
                                checkBox18.Checked = obj2["Administracion"]["Facturacion"] != null ?
                                                    obj2["Administracion"]["Facturacion"].ToString() == "A" ? true : false : false;
                                checkBox15.Checked = obj2["Administracion"]["Anulaciones"] != null ?
                                                   obj2["Administracion"]["Anulaciones"].ToString() == "A" ? true : false : false;
                                checkBox12.Checked = obj2["Administracion"]["Formatos"] != null ?
                                                  obj2["Administracion"]["Formatos"].ToString() == "A" ? true : false : false;
                                checkBox17.Checked = obj2["Administracion"]["Cargos"] != null ?
                                                  obj2["Administracion"]["Cargos"].ToString() == "A" ? true : false : false;
                                checkBox19.Checked = obj2["Administracion"]["Inventarios"] != null ?
                                                  obj2["Administracion"]["Inventarios"].ToString() == "A" ? true : false : false;
                                checkBox11.Checked = obj2["Administracion"]["Mensajero"] != null ?
                                                 obj2["Administracion"]["Mensajero"].ToString() == "A" ? true : false : false;
                                checkBox16.Checked = obj2["Administracion"]["Rips"] != null ?
                                                 obj2["Administracion"]["Rips"].ToString() == "A" ? true : false : false;
                                checkBox13.Checked = obj2["Administracion"]["Adherencia"] != null ?
                                                obj2["Administracion"]["Adherencia"].ToString() == "A" ? true : false : false;
                                checkBox10.Checked = obj2["Administracion"]["Envios"] != null ?
                                                obj2["Administracion"]["Envios"].ToString() == "A" ? true : false : false;
                                checkBox41.Checked = obj2["Administracion"]["BarCodes"] != null ?
                                                obj2["Administracion"]["BarCodes"].ToString() == "A" ? true : false : false;
                                checkBox20.Checked = obj2["Administracion"]["Autorizaciones"] != null ?
                                                obj2["Administracion"]["Autorizaciones"].ToString() == "A" ? true : false : false;                             
                                checkBox39.Checked = obj2["Administracion"]["RDA"] != null ?
                                                obj2["Administracion"]["RDA"].ToString() == "A" ? true : false : false;

                                if (obj2["Administracion"]["AdministracionDetalles"] != null)
                                {
                                    checkBox37.Checked = obj2["Administracion"]["AdministracionDetalles"]["GenerarFactura"] != null ?
                                                    obj2["Administracion"]["AdministracionDetalles"]["GenerarFactura"].ToString() == "A" ? true : false : false;
                                    checkBox32.Checked = obj2["Administracion"]["AdministracionDetalles"]["Reportes"] != null ?
                                                    obj2["Administracion"]["AdministracionDetalles"]["Reportes"].ToString() == "A" ? true : false : false;
                                    checkBox33.Checked = obj2["Administracion"]["AdministracionDetalles"]["DetalleGrupal"] != null ?
                                                    obj2["Administracion"]["AdministracionDetalles"]["DetalleGrupal"].ToString() == "A" ? true : false : false;
                                    checkBox35.Checked = obj2["Administracion"]["AdministracionDetalles"]["FacturaAbierta"] != null ?
                                                    obj2["Administracion"]["AdministracionDetalles"]["FacturaAbierta"].ToString() == "A" ? true : false : false;
                                    checkBox36.Checked = obj2["Administracion"]["AdministracionDetalles"]["Homologos"] != null ?
                                                    obj2["Administracion"]["AdministracionDetalles"]["Homologos"].ToString() == "A" ? true : false : false;
                                    checkBox34.Checked = obj2["Administracion"]["AdministracionDetalles"]["FacturacionElectronica"] != null ?
                                                    obj2["Administracion"]["AdministracionDetalles"]["FacturacionElectronica"].ToString() == "A" ? true : false : false;
                                    checkBox40.Checked = obj2["Administracion"]["AdministracionDetalles"]["ReportesPagos"] != null ?
                                                    obj2["Administracion"]["AdministracionDetalles"]["ReportesPagos"].ToString() == "A" ? true : false : false;
                                }
                             }
                            else
                            {
                                checkBox15.Checked = false;
                                checkBox15.Checked = false;
                                checkBox12.Checked = false;
                                checkBox17.Checked = false;
                                checkBox18.Checked = false;
                                checkBox19.Checked = false;
                                checkBox11.Checked = false;
                                checkBox16.Checked = false;
                                checkBox13.Checked = false;
                                checkBox10.Checked = false;
                                checkBox41.Checked = false;
                                checkBox20.Checked = false;
                                checkBox39.Checked = false;
                                checkBox37.Checked = false;
                                checkBox32.Checked = false;
                                checkBox33.Checked = false;
                                checkBox35.Checked = false;
                                checkBox36.Checked = false;
                                checkBox34.Checked = false;
                                checkBox40.Checked = false;
                            }
                            //ROL OPCIONES
                            if (obj3["Opciones"] != null)
                            {
                                checkBox49.Checked = obj3["Opciones"]["Compañias"] != null ?
                                                    obj3["Opciones"]["Compañias"].ToString() == "A" ? true : false : false;
                                checkBox46.Checked = obj3["Opciones"]["CrearEditarUsuario"] != null ?
                                                   obj3["Opciones"]["CrearEditarUsuario"].ToString() == "A" ? true : false : false;
                                checkBox43.Checked = obj3["Opciones"]["Productos"] != null ?
                                                  obj3["Opciones"]["Productos"].ToString() == "A" ? true : false : false;
                                checkBox48.Checked = obj3["Opciones"]["Horarios"] != null ?
                                                  obj3["Opciones"]["Horarios"].ToString() == "A" ? true : false : false;
                                checkBox45.Checked = obj3["Opciones"]["Convenios"] != null ?
                                                  obj3["Opciones"]["Convenios"].ToString() == "A" ? true : false : false;
                                checkBox42.Checked = obj3["Opciones"]["Preferencias"] != null ?
                                                 obj3["Opciones"]["Preferencias"].ToString() == "A" ? true : false : false;
                                checkBox47.Checked = obj3["Opciones"]["Festivos"] != null ?
                                                 obj3["Opciones"]["Festivos"].ToString() == "A" ? true : false : false;
                                checkBox44.Checked = obj3["Opciones"]["CIE10"] != null ?
                                                obj3["Opciones"]["CIE10"].ToString() == "A" ? true : false : false;
                                checkBox50.Checked = obj3["Opciones"]["ECuentas"] != null ?
                                              obj3["Opciones"]["ECuentas"].ToString() == "A" ? true : false : false;
                                checkBox51.Checked = obj3["Opciones"]["Compras"] != null ?
                                              obj3["Opciones"]["Compras"].ToString() == "A" ? true : false : false;
                                checkBox52.Checked = obj3["Opciones"]["Bodegas"] != null ?
                                              obj3["Opciones"]["Bodegas"].ToString() == "A" ? true : false : false;
                            }
                            else
                            {
                                checkBox49.Checked = false;
                                checkBox46.Checked = false;
                                checkBox43.Checked = false;
                                checkBox48.Checked = false;
                                checkBox45.Checked = false;
                                checkBox42.Checked = false;
                                checkBox47.Checked = false;
                                checkBox44.Checked = false;
                                checkBox50.Checked = false;
                                checkBox51.Checked = false;
                                checkBox52.Checked = false;
                            }

                            //ROL ENFERMERIA
                            if (obj4["Enfermeria"] != null)
                            {
                                checkBox30.Checked = obj4["Enfermeria"]["Notas"] != null ?
                                                    obj4["Enfermeria"]["Notas"].ToString() == "A" ? true : false : false;
                                checkBox27.Checked = obj4["Enfermeria"]["Plantillas"] != null ?
                                                   obj4["Enfermeria"]["Plantillas"].ToString() == "A" ? true : false : false;
                                checkBox25.Checked = obj4["Enfermeria"]["Imagenes"] != null ?
                                                  obj4["Enfermeria"]["Imagenes"].ToString() == "A" ? true : false : false;
                                checkBox29.Checked = obj4["Enfermeria"]["NAclaratoria"] != null ?
                                                  obj4["Enfermeria"]["NAclaratoria"].ToString() == "A" ? true : false : false;
                                checkBox21.Checked = obj4["Enfermeria"]["SearchImages"] != null ?
                                                  obj4["Enfermeria"]["SearchImages"].ToString() == "A" ? true : false : false;
                                checkBox24.Checked = obj4["Enfermeria"]["Inventario"] != null ?
                                                 obj4["Enfermeria"]["Inventario"].ToString() == "A" ? true : false : false;
                                checkBox28.Checked = obj4["Enfermeria"]["CManejo"] != null ?
                                                 obj4["Enfermeria"]["CManejo"].ToString() == "A" ? true : false : false;
                                checkBox26.Checked = obj4["Enfermeria"]["Registros"] != null ?
                                                obj4["Enfermeria"]["Registros"].ToString() == "A" ? true : false : false;
                                checkBox23.Checked = obj4["Enfermeria"]["Estadisticas"] != null ?
                                              obj4["Enfermeria"]["Estadisticas"].ToString() == "A" ? true : false : false;
                                checkBox22.Checked = obj4["Enfermeria"]["Cargos"] != null ?
                                              obj4["Enfermeria"]["Cargos"].ToString() == "A" ? true : false : false;
                                checkBox8.Checked = obj4["Enfermeria"]["SubirDocumentos"] != null ?
                                              obj4["Enfermeria"]["SubirDocumentos"].ToString() == "A" ? true : false : false;
                                checkBox14.Checked = obj4["Enfermeria"]["DocumentosWEB"] != null ?
                                             obj4["Enfermeria"]["DocumentosWEB"].ToString() == "A" ? true : false : false;
                            }
                            else
                            {
                                checkBox30.Checked = false;
                                checkBox27.Checked = false;
                                checkBox25.Checked = false;
                                checkBox29.Checked = false;
                                checkBox21.Checked = false;
                                checkBox24.Checked = false;
                                checkBox28.Checked = false;
                                checkBox26.Checked = false;
                                checkBox23.Checked = false;
                                checkBox22.Checked = false;
                                checkBox8.Checked = false;
                                checkBox14.Checked = false;
                            }

                            //ROL MEDICINA GENERAL
                            if (obj5["MedicinaGeneral"] != null)
                            {
                                checkBox61.Checked = obj5["MedicinaGeneral"]["Historia"] != null ?
                                                    obj5["MedicinaGeneral"]["Historia"].ToString() == "A" ? true : false : false;
                                checkBox58.Checked = obj5["MedicinaGeneral"]["Registros"] != null ?
                                                   obj5["MedicinaGeneral"]["Registros"].ToString() == "A" ? true : false : false;
                                checkBox55.Checked = obj5["MedicinaGeneral"]["Retomar"] != null ?
                                                  obj5["MedicinaGeneral"]["Retomar"].ToString() == "A" ? true : false : false;
                                checkBox60.Checked = obj5["MedicinaGeneral"]["CManejo"] != null ?
                                                  obj5["MedicinaGeneral"]["CManejo"].ToString() == "A" ? true : false : false;
                                checkBox57.Checked = obj5["MedicinaGeneral"]["Subir"] != null ?
                                                  obj5["MedicinaGeneral"]["Subir"].ToString() == "A" ? true : false : false;
                                checkBox54.Checked = obj5["MedicinaGeneral"]["Estadistica"] != null ?
                                                 obj5["MedicinaGeneral"]["Estadistica"].ToString() == "A" ? true : false : false;
                                checkBox59.Checked = obj5["MedicinaGeneral"]["Nota"] != null ?
                                                 obj5["MedicinaGeneral"]["Nota"].ToString() == "A" ? true : false : false;
                                checkBox56.Checked = obj5["MedicinaGeneral"]["Inventario"] != null ?
                                                obj5["MedicinaGeneral"]["Inventario"].ToString() == "A" ? true : false : false;
                                checkBox53.Checked = obj5["MedicinaGeneral"]["DocumentosWEB"] != null ?
                                              obj5["MedicinaGeneral"]["DocumentosWEB"].ToString() == "A" ? true : false : false;
                                checkBox38.Checked = obj5["MedicinaGeneral"]["GrabarImagenes"] != null ?
                                              obj5["MedicinaGeneral"]["GrabarImagenes"].ToString() == "A" ? true : false : false;
                                checkBox31.Checked = obj5["MedicinaGeneral"]["Ordenes"] != null ?
                                              obj5["MedicinaGeneral"]["Ordenes"].ToString() == "A" ? true : false : false;
                                checkBox62.Checked = obj5["MedicinaGeneral"]["Solicitudes"] != null ?
                                             obj5["MedicinaGeneral"]["Solicitudes"].ToString() == "A" ? true : false : false;
                                checkBox64.Checked = obj5["MedicinaGeneral"]["BuscarImagenes"] != null ?
                                             obj5["MedicinaGeneral"]["BuscarImagenes"].ToString() == "A" ? true : false : false;
                                checkBox65.Checked = obj5["MedicinaGeneral"]["Consentimientos"] != null ?
                                             obj5["MedicinaGeneral"]["Consentimientos"].ToString() == "A" ? true : false : false;
                                checkBox66.Checked = obj5["MedicinaGeneral"]["Salidas"] != null ?
                                           obj5["MedicinaGeneral"]["Salidas"].ToString() == "A" ? true : false : false;
                            }
                            else
                            {
                                checkBox61.Checked = false;
                                checkBox58.Checked = false;
                                checkBox55.Checked = false;
                                checkBox60.Checked = false;
                                checkBox57.Checked = false;
                                checkBox54.Checked = false;
                                checkBox59.Checked = false;
                                checkBox56.Checked = false;
                                checkBox53.Checked = false;
                                checkBox38.Checked = false;
                                checkBox31.Checked = false;
                                checkBox62.Checked = false;
                                checkBox64.Checked = false;
                                checkBox65.Checked = false;
                                checkBox66.Checked = false;
                            }

                            //ROL GERENCIAL
                            if (obj6["Gerencial"] != null)
                            {
                                checkBox77.Checked = obj6["Gerencial"]["FacturaPaciente"] != null ?
                                                    obj6["Gerencial"]["FacturaPaciente"].ToString() == "A" ? true : false : false;
                                checkBox74.Checked = obj6["Gerencial"]["Reportes"] != null ?
                                                   obj6["Gerencial"]["Reportes"].ToString() == "A" ? true : false : false;
                                checkBox72.Checked = obj6["Gerencial"]["Consentimientos"] != null ?
                                                  obj6["Gerencial"]["Consentimientos"].ToString() == "A" ? true : false : false;
                                checkBox76.Checked = obj6["Gerencial"]["EliminarCierreCaja"] != null ?
                                                  obj6["Gerencial"]["EliminarCierreCaja"].ToString() == "A" ? true : false : false;                                
                            }
                            else
                            {
                                checkBox77.Checked = false;
                                checkBox74.Checked = false;
                                checkBox72.Checked = false;
                                checkBox76.Checked = false;
                            }

                            //ROL FISIATRIA
                            if (obj7["Fisiatria"] != null)
                            {
                                checkBox81.Checked = obj7["Fisiatria"]["CrearHistoria"] != null ?
                                                    obj7["Fisiatria"]["CrearHistoria"].ToString() == "A" ? true : false : false;
                                checkBox78.Checked = obj7["Fisiatria"]["Retomar"] != null ?
                                                   obj7["Fisiatria"]["Retomar"].ToString() == "A" ? true : false : false;
                                checkBox73.Checked = obj7["Fisiatria"]["SubirHistoria"] != null ?
                                                  obj7["Fisiatria"]["SubirHistoria"].ToString() == "A" ? true : false : false;
                                checkBox80.Checked = obj7["Fisiatria"]["NotaAclaratoria"] != null ?
                                                  obj7["Fisiatria"]["NotaAclaratoria"].ToString() == "A" ? true : false : false;
                                checkBox68.Checked = obj7["Fisiatria"]["CompletarJuntas"] != null ?
                                                  obj7["Fisiatria"]["CompletarJuntas"].ToString() == "A" ? true : false : false;
                                checkBox71.Checked = obj7["Fisiatria"]["FirmarHistorias"] != null ?
                                                  obj7["Fisiatria"]["FirmarHistorias"].ToString() == "A" ? true : false : false;
                                checkBox79.Checked = obj7["Fisiatria"]["CrearOrdenes"] != null ?
                                                  obj7["Fisiatria"]["CrearOrdenes"].ToString() == "A" ? true : false : false;
                                checkBox75.Checked = obj7["Fisiatria"]["BuscarRegistros"] != null ?
                                                  obj7["Fisiatria"]["BuscarRegistros"].ToString() == "A" ? true : false : false;
                            }
                            else
                            {
                                checkBox81.Checked = false;
                                checkBox78.Checked = false;
                                checkBox73.Checked = false;
                                checkBox80.Checked = false;
                                checkBox68.Checked = false;
                                checkBox71.Checked = false;
                                checkBox79.Checked = false;
                                checkBox75.Checked = false;
                            }
                        }
                    }
                    else
                    {
                        MG = new MensajesGeneral()
                        {
                            Mensaje = "Error grave obteniendo datos del usuario, contacte al administrador",
                            TipoImagen = 1000
                        };

                        MG.ShowDialog();
                        this.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
    }
}
