using Domain;
using Domain.CXN;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZamenisHealth.Clases;
using ZamenisHealth.Comunes.Extras;
using ZamenisHealth.PagosApp;

namespace ZamenisHealth.Comunes
{
    public partial class Contenedor : ConfigForm.BaseForm
    {
        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
        [DllImport("user32.dll")]
        private static extern bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);
        // Constantes que representan el botón de cerrar
        private const uint SC_CLOSE = 0xF060;
        private const uint MF_BYCOMMAND = 0x00000000;
        private const uint MF_GRAYED = 0x00000001; // Para deshabilitar el botón

        private readonly static IPayments repoPayments = new MPayments();
        private readonly static IMensajeria repositorioMensajeria = new MMensajeria();
        private readonly static IConfSystem repositorioConfSystem = new MConfSystem();
        private readonly static IGenerales repositorioGenerales = new MGenerales();
        private readonly static ILogin repositorioLogin = new MLogin();       
        
        public static string UsuarioLogueado;
        public string UsuarioLogueado1;
        public string PassChange;

        private Dictionary<string, string> getCon = new Dictionary<string, string>();
        SpeechRecognitionEngine oSpeechRecognitionEngine = null;
        System.Speech.Synthesis.SpeechSynthesizer oSpeechSynthesizer = null;

        public Contenedor()
        {
            InitializeComponent();
            
            ConfigForm.MoverForma(label1, this);
            linkLabel12.Visible = true;
            linkLabel11.Visible = true;
        }
     
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            // Obtener el menú del sistema del formulario (la barra de título)
            IntPtr hMenu = GetSystemMenu(this.Handle, false);

            // Deshabilitar el botón de cerrar
            EnableMenuItem(hMenu, SC_CLOSE, MF_BYCOMMAND | MF_GRAYED);
        }        

        private void Contenedor_Load(object sender, EventArgs e)
        {
            try
            {
                if (Conexion.ConectionDictionary["Mensaje"] == "Vencido" || Conexion.ConectionDictionary["Mensaje"] == "Cancelado")
                {
                    label3.Visible = true;
                }
                else
                {
                    label3.Visible = false;
                }

                Titulo.Visible = false;
                ImageClose.Visible = false;              

                UsuarioLogueado = UsuarioLogueado1;

                ToolTip toolTip1 = new ToolTip();
                ToolTip toolTip12 = new ToolTip();
                ToolTip toolTip14 = new ToolTip();
                ToolTip toolTip15 = new ToolTip();
                ToolTip toolTip78 = new ToolTip();
                ToolTip toolTipWebOficial = new ToolTip();
                ToolTip toolTipAPP = new ToolTip();
                ToolTip toolTipNovedades = new ToolTip();

                toolTip1.ShowAlways = true;
                toolTip12.ShowAlways = true;
                toolTip14.ShowAlways = true;
                toolTip15.ShowAlways = true;
                toolTip78.ShowAlways = true;
                toolTipWebOficial.ShowAlways = true;
                toolTipAPP.ShowAlways = true;
                toolTipNovedades.ShowAlways = true;

                toolTip1.SetToolTip(pictureBox2, "Cerrar la Aplicacion");
                toolTip12.SetToolTip(pictureBox3, "Minimizar Aplicacion");
                toolTip14.SetToolTip(linkLabel5, "Abrir Reloj Analogo");
                toolTip15.SetToolTip(pictureBox4, "Haga doble click aqui para cambiar su avatar");
                toolTip78.SetToolTip(label78, "¿Que es esto?");
                toolTipWebOficial.SetToolTip(linkLabel2, "Este vinculo te direcciona a la web oficial de productos Zamenis del Autor Fabian Gamba");
                toolTipAPP.SetToolTip(linkLabel1, "Aqui puedes descargar la APP solo disponible para Android 13 en adelante, si eres medico o enfermero puedes subir imagenes de tus pacientes y consultar tu agenda diaria");
                toolTipNovedades.SetToolTip(linkLabel8, "Aqui consultas los ultimos cambios del Software y las ultimas novedades");

                getCon = Conexion.Conection();               

                chargeAvatar();
                _ = ConfigForm.FadeInControl(pictureBox4);
                _ = ConfigForm.FadeInControl(pictureBox1);

                bool noticia = false;

                
                    if (repositorioConfSystem.getDatoNoticias().Noticia == true)
                    {
                        noticia = true;
                    }
                

                if (noticia == true)
                {
                    Comunes.Noticias N = new Comunes.Noticias();
                    N.ShowDialog();
                    label2.Visible = true;
                }
                else
                {
                    label2.Visible = false;
                }

                toolStripButton2.MouseMove += ToolStripButton2_MouseMove;
                toolStripButton2.MouseLeave += ToolStripButton2_MouseLeave;
                toolStripButton11.MouseMove += ToolStripButton11_MouseMove;
                toolStripButton11.MouseLeave += ToolStripButton11_MouseLeave;
                toolStripButton10.MouseMove += ToolStripButton10_MouseMove;
                toolStripButton10.MouseLeave += ToolStripButton10_MouseLeave;
                toolStripButton9.MouseMove += ToolStripButton9_MouseMove;
                toolStripButton9.MouseLeave += ToolStripButton9_MouseLeave;
                toolStripButton8.MouseMove += ToolStripButton8_MouseMove;
                toolStripButton8.MouseLeave += ToolStripButton8_MouseLeave;
                toolStripButton7.MouseMove += ToolStripButton7_MouseMove;
                toolStripButton7.MouseLeave += ToolStripButton7_MouseLeave;
                toolStripButton6.MouseMove += ToolStripButton6_MouseMove;
                toolStripButton6.MouseLeave += ToolStripButton6_MouseLeave;
                toolStripButton5.MouseMove += ToolStripButton5_MouseMove;
                toolStripButton5.MouseLeave += ToolStripButton5_MouseLeave;
                toolStripButton4.MouseMove += ToolStripButton4_MouseMove;
                toolStripButton4.MouseLeave += ToolStripButton4_MouseLeave;
                toolStripButton3.MouseMove += ToolStripButton3_MouseMove;
                toolStripButton3.MouseLeave += ToolStripButton3_MouseLeave;
                toolStripButton14.MouseMove += ToolStripButton14_MouseMove;
                toolStripButton14.MouseLeave += ToolStripButton14_MouseLeave;

                Conexion.BloqueosAgenda = repositorioConfSystem.getListado()["Bloqueo"];
                Conexion.EmailAgenda = repositorioConfSystem.getListado()["Email"];
                Conexion.SMSAgenda = repositorioConfSystem.getListado()["SMS"];

                label4.Text = repositorioConfSystem.getListado()["Base"];

                GestionPagos();
                ConsultarIA();
                ConsultarPatron();
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        void ConsultarPatron()
        {
            try
            {
                var l = repositorioLogin.getUser(UsuarioLogueado);
                if (l != null) 
                { 
                    if (string.IsNullOrEmpty(l.Patron))
                    {
                        MensajesGeneral M = new MensajesGeneral();
                        M.Mensaje = "Actualmente su usuario no tiene un patron de desbloqueo, asigne uno ahora";
                        M.TipoImagen = 1000;
                        M.ShowDialog();

                        PatronAsign P = new PatronAsign();
                        P.ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        void ConsultarIA()
        {
            try
            {
                if (Conexion.ConectionDictionary["Perplexity"] == "A")
                {
                    var IA_Consult = repositorioConfSystem.getListado()["Perplexity"];
                    if (IA_Consult == "A")
                    {
                        linkLabel18.Visible = true;
                        linkLabel19.Visible = true;
                    }
                    else
                    {
                        linkLabel19.Visible = false;
                        linkLabel18.Visible = false;
                    }
                }
                else
                {
                    linkLabel19.Visible = false;
                    linkLabel18.Visible = false;
                }               
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        #region // Over and Leaves
        void Over(ToolStripButton T)
        {
            T.BackColor = Color.Aqua;
            T.ForeColor = Color.Black;
        }
        void Leaves(ToolStripButton T)
        {
            T.BackColor = Color.FromArgb(55, 85, 120);
            T.ForeColor = Color.WhiteSmoke;
        }


        private void ToolStripButton2_MouseMove(object sender, EventArgs e)
        {
            Over(toolStripButton2);
        }
        private void ToolStripButton14_MouseMove(object sender, EventArgs e)
        {
            Over(toolStripButton14);
        }
        private void ToolStripButton2_MouseLeave(object sender, EventArgs e)
        {
            Leaves(toolStripButton2);
        }
        private void ToolStripButton11_MouseMove(object sender, EventArgs e)
        {
            Over(toolStripButton11);
        }
        private void ToolStripButton11_MouseLeave(object sender, EventArgs e)
        {
            Leaves(toolStripButton11);
        }
        private void ToolStripButton14_MouseLeave(object sender, EventArgs e)
        {
            Leaves(toolStripButton14);
        }
        private void ToolStripButton10_MouseMove(object sender, EventArgs e)
        {
            Over(toolStripButton10);
        }
        private void ToolStripButton10_MouseLeave(object sender, EventArgs e)
        {
            Leaves(toolStripButton10);
        }
        private void ToolStripButton9_MouseMove(object sender, EventArgs e)
        {
            Over(toolStripButton9);
        }
        private void ToolStripButton9_MouseLeave(object sender, EventArgs e)
        {
            Leaves(toolStripButton9);
        }
        private void ToolStripButton8_MouseMove(object sender, EventArgs e)
        {
            Over(toolStripButton8);
        }
        private void ToolStripButton8_MouseLeave(object sender, EventArgs e)
        {
            Leaves(toolStripButton8);
        }
        private void ToolStripButton7_MouseMove(object sender, EventArgs e)
        {
            Over(toolStripButton7);
        }
        private void ToolStripButton7_MouseLeave(object sender, EventArgs e)
        {
            Leaves(toolStripButton7);
        }
        private void ToolStripButton6_MouseMove(object sender, EventArgs e)
        {
            Over(toolStripButton6);
        }
        private void ToolStripButton6_MouseLeave(object sender, EventArgs e)
        {
            Leaves(toolStripButton6);
        }
        private void ToolStripButton5_MouseMove(object sender, EventArgs e)
        {
            Over(toolStripButton5);
        }
        private void ToolStripButton5_MouseLeave(object sender, EventArgs e)
        {
            Leaves(toolStripButton5);
        }
        private void ToolStripButton4_MouseMove(object sender, EventArgs e)
        {
            Over(toolStripButton4);
        }
        private void ToolStripButton4_MouseLeave(object sender, EventArgs e)
        {
            Leaves(toolStripButton4);
        }
        private void ToolStripButton3_MouseMove(object sender, EventArgs e)
        {
            Over(toolStripButton3);
        }
        private void ToolStripButton3_MouseLeave(object sender, EventArgs e)
        {
            Leaves(toolStripButton3);
        }
        #endregion //Over and Leaves

        void chargeAvatar()
        {
            try
            {
                byte[] _getAvatar =repositorioLogin.getAvatar(this.UsuarioLogueado1);                
                if (_getAvatar != null)
                {
                    Image recoveryImage = repositorioGenerales.ByteToImage(_getAvatar);
                    pictureBox4.Image = recoveryImage;
                    pictureBox1.Image = recoveryImage;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                label72.Text = DateTime.Now.ToString("hh:mm:ss");
                label73.Text = DateTime.Now.ToLongDateString();
            }
            catch (Exception ex)
            {
                label72.Visible = false;
                label73.Visible = false;
                timer1.Enabled = false;
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void toolStripButton11_Click(object sender, EventArgs e)
        {
            string Permiso = getCon["Rol5"];
            if (Permiso == "A")
            {
                AbrirFormEnPanel2(new ConfigContenedor.Administracion(), false);
                CerrarPanel.Enabled = false;
                CerrarPanel.Enabled = true;
            }
            else
            {
                MensajesGeneral MG = new MensajesGeneral();
                MG.Mensaje = "El permiso a esta opcion se encuentra habilitado, pero no esta autorizado dentro de " +
                    "su licencia adquirida, contacte a soporte para adquirir este modulo";
                MG.TipoImagen = 1000;
                MG.ShowDialog();
                return;
            }
        }

        private void toolStripButton10_Click(object sender, EventArgs e)
        {
            string Permiso = getCon["Rol1"];
            if (Permiso == "A")
            {
                AbrirFormEnPanel2(new ConfigContenedor.Recepcion(), false);
                CerrarPanel.Enabled = false;
                CerrarPanel.Enabled = true;
            }
            else
            {
                MensajesGeneral MG = new MensajesGeneral();
                MG.Mensaje = "El permiso a esta opcion se encuentra habilitado, pero no esta autorizado dentro de " +
                    "su licencia adquirida, contacte a soporte para adquirir este modulo";
                MG.TipoImagen = 1000;
                MG.ShowDialog();
                return;
            }
        }

        private void toolStripButton9_Click(object sender, EventArgs e)
        {
            string Permiso = getCon["Rol6"];
            if (Permiso == "A")
            {
                AbrirFormEnPanel2(new ConfigContenedor.Gerencia(), false);
                CerrarPanel.Enabled = false;
                CerrarPanel.Enabled = true;
            }
            else
            {
                MensajesGeneral MG = new MensajesGeneral();
                MG.Mensaje = "El permiso a esta opcion se encuentra habilitado, pero no esta autorizado dentro de " +
                    "su licencia adquirida, contacte a soporte para adquirir este modulo";
                MG.TipoImagen = 1000;
                MG.ShowDialog();
                return;
            }
        }

        private void toolStripButton8_Click(object sender, EventArgs e)
        {

            string Permiso = getCon["Rol2"];
            if (Permiso == "A")
            {
                AbrirFormEnPanel2(new ConfigContenedor.Enfermeria(), false);
                CerrarPanel.Enabled = false;
                CerrarPanel.Enabled = true;
            }
            else
            {
                MensajesGeneral MG = new MensajesGeneral();
                MG.Mensaje = "El permiso a esta opcion se encuentra habilitado, pero no esta autorizado dentro de " +
                    "su licencia adquirida, contacte a soporte para adquirir este modulo";
                MG.TipoImagen = 1000;
                MG.ShowDialog();
                return;
            }
        }

        private void toolStripButton7_Click(object sender, EventArgs e)
        {
            string Permiso = getCon["Rol3"];
            if (Permiso == "A")
            {
                AbrirFormEnPanel2(new ConfigContenedor.MGeneral(), false);
                CerrarPanel.Enabled = false;
                CerrarPanel.Enabled = true;
            }
            else
            {
                MensajesGeneral MG = new MensajesGeneral();
                MG.Mensaje = "El permiso a esta opcion se encuentra habilitado, pero no esta autorizado dentro de " +
                    "su licencia adquirida, contacte a soporte para adquirir este modulo";
                MG.TipoImagen = 1000;
                MG.ShowDialog();
                return;
            }
        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            var Permiso = getCon["Rol7"];
            if (Permiso == "A")
            {
                AbrirFormEnPanel2(new ConfigContenedor.Fisiatria(), false);
                CerrarPanel.Enabled = false;
                CerrarPanel.Enabled = true;
            }
            else
            {
                MensajesGeneral MG = new MensajesGeneral();
                MG.Mensaje = "El permiso a esta opcion se encuentra habilitado, pero no esta autorizado dentro de " +
                    "su licencia adquirida, contacte a soporte para adquirir este modulo";
                MG.TipoImagen = 1000;
                MG.ShowDialog();
                return;
            }
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            var Permiso = getCon["Rol10"];
            if (Permiso == "A")
            {
                AbrirFormEnPanel2(new ConfigContenedor.Terapia(), false);
                CerrarPanel.Enabled = false;
                CerrarPanel.Enabled = true;
            }
            else
            {
                MensajesGeneral MG = new MensajesGeneral();
                MG.Mensaje = "El permiso a esta opcion se encuentra habilitado, pero no esta autorizado dentro de " +
                    "su licencia adquirida, contacte a soporte para adquirir este modulo";
                MG.TipoImagen = 1000;
                MG.ShowDialog();
                return;
            }
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            var Permiso = getCon["Rol9"];
            if (Permiso == "A")
            {
                AbrirFormEnPanel2(new ConfigContenedor.Terapia(), false);
                CerrarPanel.Enabled = false;
                CerrarPanel.Enabled = true;
            }
            else
            {
                MensajesGeneral MG = new MensajesGeneral();
                MG.Mensaje = "El permiso a esta opcion se encuentra habilitado, pero no esta autorizado dentro de " +
                    "su licencia adquirida, contacte a soporte para adquirir este modulo";
                MG.TipoImagen = 1000;
                MG.ShowDialog();
                return;
            }
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            var Permiso = getCon["Rol8"];
            if (Permiso == "A")
            {
                AbrirFormEnPanel2(new ConfigContenedor.Terapia(), false);
                CerrarPanel.Enabled = false;
                CerrarPanel.Enabled = true;
            }
            else
            {
                MensajesGeneral MG = new MensajesGeneral();
                MG.Mensaje = "El permiso a esta opcion se encuentra habilitado, pero no esta autorizado dentro de " +
                    "su licencia adquirida, contacte a soporte para adquirir este modulo";
                MG.TipoImagen = 1000;
                MG.ShowDialog();
                return;
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            try
            {
                panel1.Visible = false;
                CerrarPanel.Enabled = false;               
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void toolStripButton12_Click(object sender, EventArgs e)
        {           
            AbrirFormEnPanel2(new ConfigContenedor.DatosPac(), true);
            CerrarPanel.Enabled = false;
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            var Permiso = getCon["Rol4"];
            if (Permiso == "A")
            {
                AbrirFormEnPanel2(new ConfigContenedor.Opciones(), false);
                CerrarPanel.Enabled = false;
                CerrarPanel.Enabled = true;
            }
            else
            {
                MensajesGeneral MG = new MensajesGeneral();
                MG.Mensaje = "El permiso a esta opcion se encuentra habilitado, pero no esta autorizado dentro de " +
                    "su licencia adquirida, contacte a soporte para adquirir este modulo";
                MG.TipoImagen = 1000;
                MG.ShowDialog();
                return;
            }
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private async void pictureBox2_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Desea Salir de la Aplicacion?",
                                                  "Zamenis Health - Cerrar Aplicacion",
                                                  MessageBoxButtons.YesNo,
                                                  MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }
    
        private async void timer3_Tick(object sender, EventArgs e)
        {
            try
            {
                var _mensaje = await Task.Run(() => repositorioMensajeria.Mensajes(UsuarioLogueado));
                if (_mensaje.Men_Id == 1)
                {
                    timer3.Enabled = false;

                    this.Invoke((Action)(() =>
                    {
                        MensajesGeneral MG = new MensajesGeneral();
                        MG.Mensaje = "Se ha detenido el servicio de envio y recepcion de mensajes Zamenis Health, los mensajes " +
                            "error en conexion de internet.";
                        MG.TipoImagen = 1000;
                        MG.Show(this);
                        MessageBox.Show(_mensaje.Men_Mensaje);
                        linkLabel15.Visible = true;
                        linkLabel17.Visible = false;
                        linkLabel16.Visible = false;
                    }));                    
                }
                else if (_mensaje.Men_Id == 0)
                {
                    this.Invoke((Action)(() =>
                    {
                        linkLabel15.Visible = false;
                        linkLabel17.Visible = true;
                        linkLabel16.Visible = true;
                    }));                    
                }
                else
                {
                    this.Invoke((Action)(() =>
                    {
                        CXN_MESSENGER M = new CXN_MESSENGER
                        {
                            Men_Mensaje = _mensaje.Men_Mensaje,
                            Men_Id = _mensaje.Men_Id,
                            Men_Usuario_De = _mensaje.Men_Usuario_De
                        };

                        MensajeroSend2 mensjaes_I = new MensajeroSend2(M);
                        mensjaes_I.TopMost = true;
                        mensjaes_I.lodaAvatar();
                        mensjaes_I.Show(this);

                        linkLabel15.Visible = false;
                        linkLabel17.Visible = true;
                        linkLabel16.Visible = true;
                    }));                    
                }
            }
            catch (Exception ex)
            {
                /*this.Invoke((Action)(() =>
                {
                    timer3.Enabled = false;
                    
                    MensajesGeneral MG = new MensajesGeneral();
                    MG.Mensaje = "Se ha detenido el servicio de envio y recepcion de mensajes Zamenis Health, los mensajes " +
                        "error en conexion de internet.";
                    MG.TipoImagen = 1000;
                    MG.Show(this);
                    
                    TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
                    
                    linkLabel15.Visible = true;
                    linkLabel17.Visible = false;
                    linkLabel16.Visible = false;
                }));       */         
            }
        }

        private void AbrirFormEnPanel2(object Formhijo, bool Grande)
        {
            Panel pnltemp = new Panel();
            pnltemp = panel1;

            if (pnltemp.Controls.Count > 0)
                pnltemp.Controls.RemoveAt(0);
            Form fh = Formhijo as Form;
            fh.TopLevel = false;
            fh.Dock = DockStyle.Fill;
            pnltemp.Controls.Add(fh);
            pnltemp.Tag = fh;
            pnltemp.Visible = true;            

            if (Grande == false)
            {
                foreach (Control control in fh.Controls)
                {
                    if (control is System.Windows.Forms.Label)
                    {
                        _ = FadeInControl(control);
                    }
                }
            }

            CerrarPanel.Enabled = true;
            fh.Show();
        }     

        private async Task FadeInControl(Control control)
        {
            try
            {
                if (control is PictureBox pictureBox)
                {                    
                    int cTemp = control.Width;

                    System.Drawing.Drawing2D.GraphicsPath gp = new System.Drawing.Drawing2D.GraphicsPath();
                    gp.AddEllipse(0, 0, pictureBox.Width - 1, pictureBox.Height - 1);
                    Region rg = new Region(gp);
                    pictureBox.Region = rg;

                    pictureBox.Width = 0;

                    for (int i = 0; i <= cTemp; i = i + 2)
                    {
                        pictureBox.Width = i;
                        await Task.Delay(10);
                    }

                    pictureBox.MouseMove += (sender, e) =>
                    {
                        pictureBox.BackColor = Color.PaleTurquoise;
                    };

                    pictureBox.MouseLeave += (sender, e) =>
                    {
                        pictureBox.BackColor = Color.Transparent;
                    };
                }
                else if (control is System.Windows.Forms.Label label)
                {
                    string lTemp = control.Text;
                    label.Text = "";

                    for (int i = 0; i <= 10; i++)
                    {
                        label.Text += " ";
                        await Task.Delay(50);
                    }

                    label.Text = lTemp;

                    label.MouseMove += (sender, e) =>
                    {
                        label.BackColor = Color.LightBlue;
                    };

                    label.MouseLeave += (sender, e) =>
                    {
                        label.BackColor = Color.Transparent;
                    };
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        public void panelToAdmin2()
        {
            AbrirFormEnPanel2(new ConfigContenedor.Administracion2(), false);
        }

        public void Claves()
        {
            Claves C = new Claves(PassChange, UsuarioLogueado1);
            C.toolStripButton3.Enabled = true;
            C.ShowDialog();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            Extras.AcercaDE s = new AcercaDE();
            s.ShowDialog();
        }

        private void label2_Click(object sender, EventArgs e)
        {
            Noticias N = new Noticias();
            N.ShowDialog();
        }

        private void Contenedor_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyData == Keys.F11)
                {
                    string Rol = repositorioLogin.getUser(this.UsuarioLogueado1).Log_Rol_AdminI;                    

                    if (Rol == "A")
                    {
                        ConfigGen C = new ConfigGen();
                        C.ShowDialog();
                    }
                    else
                    {
                        MensajesGeneral MG = new MensajesGeneral();
                        MG.TipoImagen = 1000;
                        MG.Mensaje = "Su usuario no esta autorizado para cambiar la configuracion del sistema";
                        MG.ShowDialog();
                    }                    
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void label3_Click(object sender, EventArgs e)
        {
            Opiniones O = new Opiniones();
            O.ShowDialog();
        }

        private void pictureBox4_DoubleClick(object sender, EventArgs e)
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
                        pictureBox4.Image = Image.FromFile(imagePath);
                        Byte[] bytes = File.ReadAllBytes(imagePath);

                        
                            repositorioLogin.saveAvatar(bytes, this.UsuarioLogueado1);
                                                
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

        private void label78_Click(object sender, EventArgs e)
        {

            try
            {
                MensajesGeneral MG = new MensajesGeneral();

                label78.Visible = false;
                Send();
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        async void Send()
        {
            try
            {
                Task oTask = null;
                oTask = new Task(Voz);

                if (oTask != null)
                {
                    oTask.Start();
                    await oTask;
                    label78.Visible = true;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        void Voz()
        {
            try
            {
                cbVoice.Items.Clear();

                SpeechSynthesizer synth = new SpeechSynthesizer();

                foreach (var voice in synth.GetInstalledVoices())
                    cbVoice.Items.Add(voice.VoiceInfo.Name);
                cbVoice.SelectedIndex = 0;

                if (oSpeechSynthesizer == null)
                {
                    oSpeechSynthesizer = new System.Speech.Synthesis.SpeechSynthesizer();
                    oSpeechSynthesizer.SetOutputToDefaultAudioDevice();
                }

                if (cbVoice.Text != "")
                    oSpeechSynthesizer.SelectVoice(cbVoice.Text);

                oSpeechSynthesizer.Speak("Un avatar es una imagen que puede subir y cada vez que envíe un mensaje a otra persona a través de Zamenis Health, esta " +
                    "persona podrá ver su imagen como un avatar");
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void toolStripButtonMenu1_Click(object sender, EventArgs e)
        {
            AbrirFormEnPanel2(new ConfigContenedor.Menu(), false);
            CerrarPanel.Enabled = false;
            CerrarPanel.Enabled = true;
        }

        public void CierraPanel()
        {
            CerrarPanel.Enabled = false;
            panel1.Visible = false;
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(linkLabel2.Text);
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            /*System.Diagnostics.Process.Start(Conexion.getURLPrincipal() + "/Android");
            System.Diagnostics.Process.Start(repositorioConfSystem.getListado()["APPAndroid"]);*/
            System.Diagnostics.Process.Start(Conexion.getURLPrincipal() + ":8200");
        }

        private void linkLabel5_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Analogo f = new Analogo();
            f.Show();
            linkLabel5.Visible = false;
            linkLabel6.Visible = false;
        }

        private void linkLabel8_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(Conexion.getURLPrincipal() + "/UpdateZamenis");
        }

        private void linkLabel12_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Manuales m = new Manuales();
            m.ShowDialog();
        }

        private void linkLabel15_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            timer3.Enabled = false;
            linkLabel15.Visible = false;
            linkLabel17.Visible = true;
            linkLabel16.Visible = true;
        }

        private void linkLabel17_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Comunes.MensajeroSend mensjaes_I = new Comunes.MensajeroSend();
            mensjaes_I.ShowDialog();
        }

        private void toolStripButton14_Click(object sender, EventArgs e)
        {
            string Permiso = getCon["Rol11"];
            if (Permiso == "A")
            {
                AbrirFormEnPanel2(new ConfigContenedor.Radiologia(), false);
                CerrarPanel.Enabled = false;
                CerrarPanel.Enabled = true;
            }
            else
            {
                MensajesGeneral MG = new MensajesGeneral();
                MG.Mensaje = "El permiso a esta opcion se encuentra habilitado, pero no esta autorizado dentro de " +
                    "su licencia adquirida, contacte a soporte para adquirir este modulo";
                MG.TipoImagen = 1000;
                MG.ShowDialog();
                return;
            }
        }
        void GestionPagos()
        {
            try
            {
                LinkPagos lP = repoPayments.getStatusMonth();
                if (lP == null)
                {
                    label5.Text = "Sin Novedades - Click para ver Estado de Cuenta";
                }
                else
                {
                    if (lP.Estado != "A")
                    {
                        label5.Text = "Sin Novedades - Click para ver Estado de Cuenta";
                    }
                    else
                    {
                        label5.Text = "Se ha generado una nueva Factura haz click AQUI";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void label5_Click(object sender, EventArgs e)
        {
            EstadoCuenta P = new EstadoCuenta();
            P.ShowDialog();
        }

        private void toolStrip2_MouseDown(object sender, MouseEventArgs e)
        {
            ConfigForm.ReleaseCapturing();
            ConfigForm.SendMessageMove(this.Handle, 0x112, 0xf012, 0);
        }

        private void linkLabel19_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            IAS.Perplexity perplexity = new IAS.Perplexity();   
            perplexity.ShowDialog();
        }
        
    }
}
