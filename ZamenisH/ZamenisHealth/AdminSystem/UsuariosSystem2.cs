using Domain;
using Domain.CXN;
using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Collections.Generic;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.AdminSystem
{
    public partial class UsuariosSystem2 : Forma2
    {
        private static readonly ILogin repoLogin = new MLogin();
        private static readonly IRoles repoRoles = new MRoles();
        private static readonly IMensajeria repoMens = new MMensajeria();

        private string Funcionario;
        private bool Existence;

        public UsuariosSystem2()
        {
            InitializeComponent();
        }
        private void btnZamenis1_ButtonClick(object sender, EventArgs e)
        {
            try
            {
                MensajesGeneral MG = new MensajesGeneral();

                CXN_ROLES Rol = new CXN_ROLES
                {
                    Rol_R_Agenda_R = (checkBox1.Checked == true ? "A" : "N"),
                    Rol_R_CrearPacientes = (checkBox2.Checked == true ? "A" : "N"),
                    Rol_R_Asistencia = (checkBox3.Checked == true ? "A" : "N"),
                    Rol_R_Ventas = (checkBox4.Checked == true ? "A" : "N"),
                    Rol_R_Cargos = (checkBox5.Checked == true ? "A" : "N"),
                    Rol_R_Copias = (checkBox6.Checked == true ? "A" : "N"),
                    Rol_R_Precios = (checkBox7.Checked == true ? "A" : "N"),
                    Rol_R_Mensajero = (checkBox8.Checked == true ? "A" : "N"),
                    Rol_R_Cotizaciones = (checkBox9.Checked == true ? "A" : "N"),

                    Rol_A_SMSEmail = (checkBox10.Checked == true ? "A" : "N"),
                    Rol_A_Mensajero = (checkBox11.Checked == true ? "A" : "N"),
                    Rol_A_Formatos = (checkBox12.Checked == true ? "A" : "N"),
                    Rol_A_Adherencia = (checkBox13.Checked == true ? "A" : "N"),
                    Rol_A_Productos = (checkBox14.Checked == true ? "A" : "N"),
                    Rol_A_Anulaciones = (checkBox15.Checked == true ? "A" : "N"),
                    Rol_A_RIPS = (checkBox16.Checked == true ? "A" : "N"),
                    Rol_A_Cargos = (checkBox17.Checked == true ? "A" : "N"),
                    Rol_A_Facturacion = (checkBox18.Checked == true ? "A" : "N"),
                    Rol_A_Inventario = (checkBox19.Checked == true ? "A" : "N"),
                    Rol_A_Autorizaciones = (checkBox20.Checked == true ? "A" : "N"),

                    Rol_O_Mensajero = (checkBox21.Checked == true ? "A" : "N"),
                    Rol_O_Bodegas = (checkBox22.Checked == true ? "A" : "N"),
                    Rol_O_CIE10 = (checkBox23.Checked == true ? "A" : "N"),
                    Rol_O_Convenios = (checkBox24.Checked == true ? "A" : "N"),
                    Rol_O_UsuariosSystem = (checkBox25.Checked == true ? "A" : "N"),
                    Rol_O_Festivos = (checkBox26.Checked == true ? "A" : "N"),
                    Rol_O_Proveedores = (checkBox27.Checked == true ? "A" : "N"),
                    Rol_O_Horarios = (checkBox28.Checked == true ? "A" : "N"),
                    Rol_O_Compañias = (checkBox29.Checked == true ? "A" : "N"),

                    AdminFactura = (checkBox37.Checked == true ? "A" : "N"),
                    AdminFacturaAbierta = (checkBox35.Checked == true ? "A" : "N"),
                    AdminElectronica = (checkBox34.Checked == true ? "A" : "N"), //este es iguala a abajo
                    Rol_A_FacElectron = (checkBox34.Checked == true ? "A" : "N"), //este es iguala a abajo
                    AdminReportes = (checkBox32.Checked == true ? "A" : "N"),
                    AdminHomologos = (checkBox36.Checked == true ? "A" : "N"),
                    AdminGPacientes = (checkBox31.Checked == true ? "A" : "N"),
                    AdminGrupal = (checkBox33.Checked == true ? "A" : "N"),
                    AdminHelisa = (checkBox30.Checked == true ? "A" : "N"),
                    AdminGenerarToken = (checkBox38.Checked == true ? "A" : "N"),
                    AdminFHIR = (checkBox39.Checked == true ? "A" : "N"),
                    AdminReportesPagos = (checkBox40.Checked == true ? "A" : "N"),

                    Rol_R_User = this.Funcionario
                };

                if (Existence == true) //editar
                {
                    bool _update = repoRoles.updateRoles(Rol);
                    if (_update == true)
                    {
                        MG.Mensaje = "Actualizado con exito";
                        MG.TipoImagen = 3;
                        MG.ShowDialog();

                        this.Dispose();
                        this.Close();
                    }
                    else
                    {
                        MG.Mensaje = "No se logro actualizar";
                        MG.TipoImagen = 1000;
                        MG.ShowDialog();
                    }
                }

                if (Existence == false) //insertar
                {
                    bool _insert = repoRoles.insertRoles(Rol);
                    if (_insert == true)
                    {
                        MG.Mensaje = "Asignado con exito";
                        MG.TipoImagen = 3;
                        MG.ShowDialog();

                        this.Dispose();
                        this.Close();
                    }
                    else
                    {
                        MG.Mensaje = "No se logro asignar";
                        MG.TipoImagen = 1000;
                        MG.ShowDialog();
                    }
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
                Comunes.MensajesGeneral MG = new Comunes.MensajesGeneral();

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
                    CXN_ROLES getPermisos = repoRoles.getRoles(this.Funcionario);
                    if (getPermisos != null)
                    {
                        checkBox1.Checked = (getPermisos.Rol_R_Agenda_R == "A" ? true : false);
                        checkBox2.Checked = (getPermisos.Rol_R_CrearPacientes == "A" ? true : false);
                        checkBox3.Checked = (getPermisos.Rol_R_Asistencia == "A" ? true : false);
                        checkBox4.Checked = (getPermisos.Rol_R_Ventas == "A" ? true : false);
                        checkBox5.Checked = (getPermisos.Rol_R_Cargos == "A" ? true : false);
                        checkBox6.Checked = (getPermisos.Rol_R_Copias == "A" ? true : false);
                        checkBox7.Checked = (getPermisos.Rol_R_Precios == "A" ? true : false);
                        checkBox8.Checked = (getPermisos.Rol_R_Mensajero == "A" ? true : false);
                        checkBox9.Checked = (getPermisos.Rol_R_Cotizaciones == "A" ? true : false);

                        checkBox10.Checked = (getPermisos.Rol_A_SMSEmail == "A" ? true : false);
                        checkBox11.Checked = (getPermisos.Rol_A_Mensajero == "A" ? true : false);
                        checkBox12.Checked = (getPermisos.Rol_A_Formatos == "A" ? true : false);
                        checkBox13.Checked = (getPermisos.Rol_A_Adherencia == "A" ? true : false);
                        checkBox14.Checked = (getPermisos.Rol_A_Productos == "A" ? true : false);
                        checkBox15.Checked = (getPermisos.Rol_A_Anulaciones == "A" ? true : false);
                        checkBox16.Checked = (getPermisos.Rol_A_RIPS == "A" ? true : false);
                        checkBox17.Checked = (getPermisos.Rol_A_Cargos == "A" ? true : false);
                        checkBox18.Checked = (getPermisos.Rol_A_Facturacion == "A" ? true : false);
                        checkBox19.Checked = (getPermisos.Rol_A_Inventario == "A" ? true : false);
                        checkBox20.Checked = (getPermisos.Rol_A_Autorizaciones == "A" ? true : false);

                        checkBox21.Checked = (getPermisos.Rol_O_Mensajero == "A" ? true : false);
                        checkBox22.Checked = (getPermisos.Rol_O_Bodegas == "A" ? true : false);
                        checkBox23.Checked = (getPermisos.Rol_O_CIE10 == "A" ? true : false);
                        checkBox24.Checked = (getPermisos.Rol_O_Convenios == "A" ? true : false);
                        checkBox25.Checked = (getPermisos.Rol_O_UsuariosSystem == "A" ? true : false);
                        checkBox26.Checked = (getPermisos.Rol_O_Festivos == "A" ? true : false);
                        checkBox27.Checked = (getPermisos.Rol_O_Proveedores == "A" ? true : false);
                        checkBox28.Checked = (getPermisos.Rol_O_Horarios == "A" ? true : false);
                        checkBox29.Checked = (getPermisos.Rol_O_Compañias == "A" ? true : false);

                        checkBox37.Checked = (getPermisos.AdminFactura == "A" ? true : false);
                        checkBox36.Checked = (getPermisos.AdminHomologos == "A" ? true : false);
                        checkBox35.Checked = (getPermisos.AdminFacturaAbierta == "A" ? true : false);
                        checkBox34.Checked = (getPermisos.AdminElectronica == "A" ? true : false);
                        checkBox33.Checked = (getPermisos.AdminGrupal == "A" ? true : false);
                        checkBox32.Checked = (getPermisos.AdminReportes == "A" ? true : false);
                        checkBox31.Checked = (getPermisos.AdminGPacientes == "A" ? true : false);
                        checkBox30.Checked = (getPermisos.AdminHelisa == "A" ? true : false);
                        checkBox38.Checked = (getPermisos.AdminGenerarToken == "A" ? true : false);
                        checkBox39.Checked = (getPermisos.AdminFHIR == "A" ? true : false);
                        checkBox40.Checked = (getPermisos.AdminReportesPagos == "A" ? true : false);

                        Existence = true;
                    }
                    else
                    {
                        checkBox1.Checked = false;
                        checkBox2.Checked = false;
                        checkBox3.Checked = false;
                        checkBox4.Checked = false;
                        checkBox5.Checked = false;
                        checkBox6.Checked = false;
                        checkBox7.Checked = false;
                        checkBox8.Checked = false;
                        checkBox9.Checked = false;
                        checkBox10.Checked = false;
                        checkBox11.Checked = false;
                        checkBox12.Checked = false;
                        checkBox13.Checked = false;
                        checkBox14.Checked = false;
                        checkBox15.Checked = false;
                        checkBox16.Checked = false;
                        checkBox17.Checked = false;
                        checkBox18.Checked = false;
                        checkBox19.Checked = false;
                        checkBox20.Checked = false;
                        checkBox21.Checked = false;
                        checkBox22.Checked = false;
                        checkBox23.Checked = false;
                        checkBox24.Checked = false;
                        checkBox25.Checked = false;
                        checkBox26.Checked = false;
                        checkBox27.Checked = false;
                        checkBox28.Checked = false;
                        checkBox29.Checked = false;

                        checkBox37.Checked = false;
                        checkBox36.Checked = false;
                        checkBox35.Checked = false;
                        checkBox34.Checked = false;
                        checkBox33.Checked = false;
                        checkBox32.Checked = false;
                        checkBox31.Checked = false;
                        checkBox30.Checked = false;
                        checkBox38.Checked = false;
                        checkBox39.Checked = false;
                        checkBox40.Checked = false;

                        Existence = false;
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
