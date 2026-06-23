using Domain;
using Domain.CXN;
using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Windows.Forms;
using ZamenisHealth.Clases;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.AdminSystem
{
    public partial class UsuariosSystem : Forma
    {
        private static readonly IPacientes repoPacientes = new MPacientes();
        private static readonly IMensajeria repoMens = new MMensajeria();
        private static readonly ILogin repoLogin = new MLogin();

        private ToolStripButton btnGrabar, btnReset, btnPermisos;
        private string getUser;

        string Habilita;
        string Recepcion;
        string Admin;
        string AdminI;
        string Enfermero;
        string MedGen;
        string Gerencial;
        string Psicologia;
        string TFisica;
        string TOcupacional;
        string Fisiatria;
        string Varios;
        string Fotos;
        string NotaAcla;
        string Radiologia;

        public UsuariosSystem()
        {
            InitializeComponent();
        }
        private void btnZamenis1_ButtonClick(object sender, EventArgs e)
        {
            try
            {
                if (textBox1.Text == "") { MessageBox.Show("Debe diligenciar los campos de texto"); return; }
                if (textBox2.Text == "") { MessageBox.Show("Debe diligenciar los campos de texto"); return; }
                if (textBox3.Text == "") { MessageBox.Show("Debe diligenciar los campos de texto"); return; }
                if (textBox4.Text == "") { MessageBox.Show("Debe diligenciar los campos de texto"); return; }
                if (textBox5.Text == "") { MessageBox.Show("Debe diligenciar los campos de texto"); return; }
                if (textBox6.Text == "") { MessageBox.Show("Debe diligenciar los campos de texto"); return; }
                if (textBox7.Text == "") { MessageBox.Show("Debe diligenciar los campos de texto"); return; }
                if (textBox8.Text == "") { MessageBox.Show("Debe diligenciar los campos de texto"); return; }

                var Cel = repoPacientes.ValidaCelular(textBox8.Text);
                if (Cel != true)
                {
                    MessageBox.Show("El numero celular esta mal diligenciado, este debe ser de 10 numero sin espacios", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var Ema = repoPacientes.ValidaEmail(textBox7.Text);
                if (Ema != true)
                {
                    MessageBox.Show("El correo esta mal diligenciado, este debe ser de un dominio valido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (btnGrabar.Text == "Actualizar") { Actualiza(); }
                if (btnGrabar.Text == "Grabar") { Grabacion_User(); }

            }
            catch (Exception ex) { TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T); }
        }
        private void btnZamenis2_ButtonClick(object sender, EventArgs e)
        {
            try
            {
                Comunes.MensajesGeneral MG = new Comunes.MensajesGeneral();

                if (textBox8.Text == "") { MessageBox.Show("Diligencie un celular", "Faltan Datos", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
                if (textBox7.Text == "") { MessageBox.Show("Diligencie un correo", "Faltan Datos", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

                var Cel = repoPacientes.ValidaCelular(textBox8.Text);
                if (Cel != true)
                {
                    MessageBox.Show("El numero celular esta mal diligenciado, este debe ser de 10 numero sin espacios", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var Ema = repoPacientes.ValidaEmail(textBox7.Text);
                if (Ema != true)
                {
                    MessageBox.Show("El correo esta mal diligenciado, este debe ser de un dominio valido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                DialogResult result = MessageBox.Show("El restablecimiento de contraseña se notificara al dueño del usuario via SMS y Email, " +
                    "Esto reseteara la clave actual y sera reemplazada por una nueva que se le notificara al dueño del usuario.  ¿Desea continuar?",
                                                  "Zamenis Health - Cerrar Aplicacion",
                                                  MessageBoxButtons.YesNo,
                                                  MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    bool reset = repoLogin.resetClave(textBox6.Text);
                    if (reset != true)
                    {
                        MG.TipoImagen = 1000;
                        MG.Mensaje = "No se logro restablecer la clave, intente mas tarde";
                        MG.ShowDialog();
                    }
                    else
                    {
                        MG.TipoImagen = 3;
                        MG.Mensaje = "Se ha restablecido la clave, ahora es 123";
                        MG.ShowDialog();

                        this.Dispose();
                        this.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void btnZamenis3_ButtonClick(object sender, EventArgs e)
        {
            UsuariosSystem2 F = new UsuariosSystem2();
            F.ShowDialog();
        }
        private void UsuariosSystem_Load(object sender, EventArgs e)
        {
            try
            {
                Titulo.Text = "Usuarios";
                SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";
                LogoMain.Image = Properties.Resources.Splash;

                btnGrabar = new ToolStripButton();
                btnGrabar = createToolButton("Grabar");
                MenuLateral.Items.Add(btnGrabar);
                btnGrabar.Click += btnZamenis1_ButtonClick;

                btnReset = new ToolStripButton();
                btnReset = createToolButton("Reset");
                MenuLateral.Items.Add(btnReset);
                btnReset.Click += btnZamenis2_ButtonClick;

                btnPermisos = new ToolStripButton();
                btnPermisos = createToolButton("Permisos");
                MenuLateral.Items.Add(btnPermisos);
                btnPermisos.Click += btnZamenis3_ButtonClick;

                ConfigForm.SoloNumeros(textBox8);

                
                textBox8.MaxLength = 10;

                var getUsers = repoLogin.getUsersforSendMessage();
                if (getUsers != null)
                {
                    foreach (var i in getUsers)
                    {
                        comboBox1.Items.Add(i.Log_PrimerA + " " + i.Log_SegundoA + " " + i.Log_PrimerN + " " + i.Log_SegundoN);
                    }
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void Actualiza()
        {
            try
            {
                Comunes.MensajesGeneral MG = new Comunes.MensajesGeneral();

                if (checkBox1.Checked == true) { Habilita = "A"; } else { Habilita = "N"; }
                if (checkBox2.Checked == true) { Recepcion = "A"; } else { Recepcion = "N"; }
                if (checkBox3.Checked == true) { Admin = "A"; } else { Admin = "N"; }
                if (checkBox4.Checked == true) { AdminI = "A"; } else { AdminI = "N"; }
                if (checkBox5.Checked == true) { Enfermero = "A"; } else { Enfermero = "N"; }
                if (checkBox6.Checked == true) { MedGen = "A"; } else { MedGen = "N"; }
                if (checkBox7.Checked == true) { Gerencial = "A"; } else { Gerencial = "N"; }
                if (checkBox8.Checked == true) { Psicologia = "A"; } else { Psicologia = "N"; }
                if (checkBox9.Checked == true) { TFisica = "A"; } else { TFisica = "N"; }
                if (checkBox10.Checked == true) { TOcupacional = "A"; } else { TOcupacional = "N"; }
                if (checkBox11.Checked == true) { Fisiatria = "A"; } else { Fisiatria = "N"; }
                if (checkBox12.Checked == true) { Varios = "A"; } else { Varios = "N"; }
                if (checkBox13.Checked == true) { Fotos = "A"; } else { Fotos = "N"; }
                if (checkBox14.Checked == true) { Radiologia = "A"; } else { Radiologia = "N"; }
                if (checkBox15.Checked == true) { NotaAcla = "A"; } else { NotaAcla = "N"; }

                CXN_LOGIN L = new CXN_LOGIN
                {
                    Log_PrimerN = textBox2.Text,
                    Log_SegundoN = textBox3.Text,
                    Log_PrimerA = textBox4.Text,
                    Log_SegundoA = textBox5.Text,
                    Log_Email = textBox7.Text,
                    Log_Habilitado = Habilita,
                    Log_Rol_Admin = Admin,
                    Log_Rol_Recepcion = Recepcion,
                    Log_Rol_Enfermero = Enfermero,
                    Log_Rol_AdminI = AdminI,
                    Log_Rol_MedGen = MedGen,
                    Log_Rol_Gerencial = Gerencial,
                    Log_Rol_Psicologia = Psicologia,
                    Log_Varios = Varios,
                    Log_Rol_TO = TOcupacional,
                    Log_Rol_TF = TFisica,
                    Log_Rol_FI = Fisiatria,
                    Log_Identificacion = textBox1.Text,
                    Log_Celular = textBox8.Text,
                    Log_Usuario = getUser,
                    Log_Fotos = Fotos,
                    Log_NotasAcla = NotaAcla,
                    Log_UsuarioGraba = Comunes.Contenedor.UsuarioLogueado,
                    Log_RolRadiologia = Radiologia
                };

                bool _update = repoLogin.ActualizarFuncionario(L);
                if (_update != true)
                {
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "No logro ser actualizado el funcionario";
                    MG.ShowDialog();
                }
                else
                {
                    MG.TipoImagen = 3;
                    MG.Mensaje = "Funcionario actualizado con exito";
                    MG.ShowDialog();

                    this.Dispose();
                    this.Close();
                }
            }
            catch (Exception ex) { TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T); }
        }
        private void Grabacion_User()
        {
            try
            {
                Comunes.MensajesGeneral MG = new Comunes.MensajesGeneral();

                if (checkBox1.Checked == true) { Habilita = "A"; } else { Habilita = "N"; }
                if (checkBox2.Checked == true) { Recepcion = "A"; } else { Recepcion = "N"; }
                if (checkBox3.Checked == true) { Admin = "A"; } else { Admin = "N"; }
                if (checkBox4.Checked == true) { AdminI = "A"; } else { AdminI = "N"; }
                if (checkBox5.Checked == true) { Enfermero = "A"; } else { Enfermero = "N"; }
                if (checkBox6.Checked == true) { MedGen = "A"; } else { MedGen = "N"; }
                if (checkBox7.Checked == true) { Gerencial = "A"; } else { Gerencial = "N"; }
                if (checkBox8.Checked == true) { Psicologia = "A"; } else { Psicologia = "N"; }
                if (checkBox9.Checked == true) { TFisica = "A"; } else { TFisica = "N"; }
                if (checkBox10.Checked == true) { TOcupacional = "A"; } else { TOcupacional = "N"; }
                if (checkBox11.Checked == true) { Fisiatria = "A"; } else { Fisiatria = "N"; }
                if (checkBox12.Checked == true) { Varios = "A"; } else { Varios = "N"; }
                if (checkBox13.Checked == true) { Fotos = "A"; } else { Fotos = "N"; }
                if (checkBox14.Checked == true) { Radiologia = "A"; } else { Radiologia = "N"; }
                if (checkBox15.Checked == true) { NotaAcla = "A"; } else { NotaAcla = "N"; }

                DateTime Hoy = DateTime.Now.Date;

                CXN_LOGIN L = new CXN_LOGIN
                {
                    Log_PrimerN = textBox2.Text,
                    Log_SegundoN = textBox3.Text,
                    Log_PrimerA = textBox4.Text,
                    Log_SegundoA = textBox5.Text,
                    Log_Email = textBox7.Text,
                    Log_Identificacion = textBox1.Text,
                    Log_Usuario = textBox6.Text,
                    Log_Celular = textBox8.Text,
                    Log_ClaveC = "202cb962ac59075b964b07152d234b70",
                    Log_Habilitado = Habilita,
                    Log_Rol_Admin = Admin,
                    Log_Rol_Recepcion = Recepcion,
                    Log_Rol_Enfermero = Enfermero,
                    Log_Rol_AdminI = AdminI,
                    Log_Rol_MedGen = MedGen,
                    Log_Rol_Gerencial = Gerencial,
                    Log_Rol_Psicologia = Psicologia,
                    Log_Varios = Varios,
                    Log_Rol_TO = TOcupacional,
                    Log_Rol_TF = TFisica,
                    Log_Rol_FI = Fisiatria,
                    Log_Fotos = Fotos,
                    Log_NotasAcla = NotaAcla,
                    Log_RolRadiologia = Radiologia,
                    Log_UsuarioGraba = Comunes.Contenedor.UsuarioLogueado,
                    Log_UpdatePass = Convert.ToDateTime(Hoy),
                    TyC = "N"
                };

                string ConsultarUsuario = repoLogin.getClave(L.Log_Usuario);
                if (ConsultarUsuario == "")
                {
                    bool _create = repoLogin.Graba_Funcionario(L);
                    if (_create != true)
                    {
                        MG.TipoImagen = 1000;
                        MG.Mensaje = "No logro ser creado el funcionario";
                        MG.ShowDialog();
                    }
                    else
                    {
                        MG.TipoImagen = 3;
                        MG.Mensaje = "Usuario Creado, informe al nuevo usuario que debe cambiar su clave, " +
                                        "en este momento puede ingresar con la clave 123";
                        MG.ShowDialog();

                        this.Dispose();
                        this.Close();
                    }
                }
                else
                {
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "Este nombre de usuario ya existe en sistema, escoja uno diferente";
                    MG.ShowDialog();
                }
            }
            catch (Exception ex) { TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T); }
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                Comunes.MensajesGeneral MG = new Comunes.MensajesGeneral();

                getUser = repoMens.getUsertoSendMessage(comboBox1.Text);
                if (getUser == "" || getUser == null)
                {
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "Error grave obteniendo datos del usuario, contacte al administrador";
                    MG.ShowDialog();

                    this.Dispose();
                    this.Close();
                }
                else
                {
                    var getDataUser = repoLogin.getUser(getUser);
                    if (getDataUser != null)
                    {
                        textBox1.Text = getDataUser.Log_Identificacion.ToString();
                        textBox2.Text = getDataUser.Log_PrimerN.ToString();
                        textBox3.Text = getDataUser.Log_SegundoN.ToString();
                        textBox4.Text = getDataUser.Log_PrimerA.ToString();
                        textBox5.Text = getDataUser.Log_SegundoA.ToString();
                        textBox6.Text = getDataUser.Log_Usuario.ToString();
                        textBox7.Text = getDataUser.Log_Email.ToString();
                        textBox8.Text = getDataUser.Log_Celular.ToString();

                        textBox6.Enabled = false;
                        textBox1.Enabled = false;
                        btnGrabar.Text = "Actualizar";
                        btnReset.Enabled = true;
                        btnGrabar.Enabled = true;

                        if (getDataUser.Log_Habilitado.ToString() != "A") { checkBox1.Checked = false; } else { checkBox1.Checked = true; }
                        if (getDataUser.Log_Rol_Recepcion.ToString() != "A") { checkBox2.Checked = false; } else { checkBox2.Checked = true; }
                        if (getDataUser.Log_Rol_Admin.ToString() != "A") { checkBox3.Checked = false; } else { checkBox3.Checked = true; }
                        if (getDataUser.Log_Rol_AdminI.ToString() != "A") { checkBox4.Checked = false; } else { checkBox4.Checked = true; }
                        if (getDataUser.Log_Rol_Enfermero.ToString() != "A") { checkBox5.Checked = false; } else { checkBox5.Checked = true; }
                        if (getDataUser.Log_Rol_MedGen.ToString() != "A") { checkBox6.Checked = false; } else { checkBox6.Checked = true; }
                        if (getDataUser.Log_Rol_Gerencial.ToString() != "A") { checkBox7.Checked = false; } else { checkBox7.Checked = true; }
                        if (getDataUser.Log_Rol_Psicologia.ToString() != "A") { checkBox8.Checked = false; } else { checkBox8.Checked = true; }
                        if (getDataUser.Log_Rol_TF.ToString() != "A") { checkBox9.Checked = false; } else { checkBox9.Checked = true; }
                        if (getDataUser.Log_Rol_TO.ToString() != "A") { checkBox10.Checked = false; } else { checkBox10.Checked = true; }
                        if (getDataUser.Log_Rol_FI.ToString() != "A") { checkBox11.Checked = false; } else { checkBox11.Checked = true; }
                        if (getDataUser.Log_Varios.ToString() != "A") { checkBox12.Checked = false; } else { checkBox12.Checked = true; }
                        if (getDataUser.Log_Fotos.ToString() != "A") { checkBox13.Checked = false; } else { checkBox13.Checked = true; }
                        if (getDataUser.Log_NotasAcla.ToString() != "A") { checkBox15.Checked = false; } else { checkBox15.Checked = true; }
                        if (getDataUser.Log_RolRadiologia.ToString() != "A") { checkBox14.Checked = false; } else { checkBox14.Checked = true; }
                    }
                    else
                    {
                        MG.TipoImagen = 1000;
                        MG.Mensaje = "Error grave obteniendo datos del usuario, contacte al administrador";
                        MG.ShowDialog();

                        btnReset.Enabled = false;
                        this.Dispose();
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
