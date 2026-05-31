using Domain;
using Domain.CXN;
using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using ZamenisHealth.Clases;

namespace ZamenisHealth.Comunes.Extras
{
    public partial class MisDatos : Forma
    {
        private readonly IPacientes repoPacientes = new MPacientes();
        private readonly IGenerales repoGenerales = new MGenerales();
        private readonly ILogin repoLogin = new MLogin();

        private MensajesGeneral MG;

        public MisDatos()
        {
            InitializeComponent();
        }

        private void MisDatos_Load(object sender, EventArgs e)
        {
            try
            {
                Titulo.Text = "Mis Datos";
                SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";
                LogoMain.Image = Properties.Resources.Splash;

                ToolStripButton btnPatron = new ToolStripButton();
                btnPatron = createToolButton("Cambiar Patron");
                MenuLateral.Items.Add(btnPatron);
                btnPatron.Click += button3_Click;

                ToolStripButton btnTyC = new ToolStripButton();
                btnTyC = createToolButton("Terminos");
                MenuLateral.Items.Add(btnTyC);
                btnTyC.Click += button1_Click;

                ToolStripButton btnGrabar = new ToolStripButton();
                btnGrabar = createToolButton("Grabar");
                MenuLateral.Items.Add(btnGrabar);
                btnGrabar.Click += button2_Click;

                textBox2.MaxLength = 10;

                CXN_LOGIN login =  repoLogin.getUser(Contenedor.UsuarioLogueado);                
                
                if (login != null) 
                {
                    textBox1.Text = login.Log_PrimerA + " " + login.Log_SegundoA + " " + login.Log_PrimerN + " " + login.Log_SegundoA;
                    textBox2.Text = login.Log_Celular;
                    textBox3.Text = login.Log_Email;
                    textBox4.Text = login.Log_Identificacion;

                    chargeAvatar();
                    _ = ConfigForm.FadeInControl(pictureBox1);
                }
                else
                {
                    MG = new MensajesGeneral();
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "No se logro cargar los datos de su cuenta";
                    MG.ShowDialog();

                    this.Dispose();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        void chargeAvatar()
        {
            try
            {
                byte[] _getAvatar = null;

                
                    _getAvatar = repoLogin.getAvatar(Contenedor.UsuarioLogueado);
                

                if (_getAvatar != null)
                {
                    Image recoveryImage = repoGenerales.ByteToImage(_getAvatar);
                    pictureBox1.Image = recoveryImage;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }     
        private void pictureBox1_DoubleClick(object sender, EventArgs e)
        {
            ChangeAvatar();
        }
        void ChangeAvatar()
        {
            try
            {
                OpenFileDialog o = new OpenFileDialog();
                o.InitialDirectory = "C:\\";
                o.Filter = "Image Files|*.PNG;*.JPG;*.JPEG";

                try
                {
                    if (o.ShowDialog() == DialogResult.OK)
                    {
                        string imagePath = o.FileName;
                        pictureBox1.Image = Image.FromFile(imagePath);
                        Byte[] bytes = File.ReadAllBytes(imagePath);

                        
                            repoLogin.saveAvatar(bytes, Contenedor.UsuarioLogueado);
                                                
                    };
                }
                catch (Exception)
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ChangeAvatar();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (repoPacientes.ValidaCelular(textBox2.Text) == false)
                {
                    MG = new MensajesGeneral();
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "El numero de celular es invalido, debe ser de 10 digitos numericos sin espacios de Colombia";
                    MG.ShowDialog();
                    return;
                }
                if (repoPacientes.ValidaEmail(textBox3.Text) == false)
                {
                    MG = new MensajesGeneral();
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "El email es invalido, debe diligenciar uno valido";
                    MG.ShowDialog();
                    return;
                }

                CXN_LOGIN L = new CXN_LOGIN
                {
                    Log_Identificacion = textBox4.Text.Trim(),
                    Log_Celular = textBox2.Text.Trim(),
                    Log_Email = textBox3.Text.Trim(),
                    Log_Usuario = Contenedor.UsuarioLogueado
                };

                int guarda = 0;

                
                    guarda = repoLogin.ActualizarMisDatos(L);
                

                if (guarda >= 1) 
                {
                    MG = new MensajesGeneral();
                    MG.TipoImagen = 3;
                    MG.Mensaje = "Actualiado con exito";
                    MG.ShowDialog();

                    this.Dispose();
                    this.Close();
                }
                else
                {
                    MG = new MensajesGeneral();
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "No se lograron actualizar sus datos";
                    MG.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            TyC t = new TyC(Contenedor.UsuarioLogueado);
            t.boton3.Visible = true;
            t.boton1.Visible = false;
            t.boton2.Visible = false;
            t.ShowDialog();
        }
        private void button3_Click(object sender, EventArgs e)
        {
            PatronAsign patronAsign = new PatronAsign();
            patronAsign.ShowDialog();
        }
    }
}
