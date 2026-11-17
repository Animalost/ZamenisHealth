using Domain;
using Domain.CXN;
using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
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
    public partial class Contenedor : Forma
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
        private static readonly IRoles repoRoles = new MRoles();

        public static string UsuarioLogueado;
        public string UsuarioLogueado1;
        public string PassChange;
        public string Tittle;
        public string SubTittle;

        private ToolStripButton toolStripButton2 = new ToolStripButton();
        private ToolStripButton toolStripButton11 = new ToolStripButton();
        private ToolStripButton toolStripButton10 = new ToolStripButton();
        private ToolStripButton toolStripButton9 = new ToolStripButton();
        private ToolStripButton toolStripButton8 = new ToolStripButton();
        private ToolStripButton toolStripButton7 = new ToolStripButton();
        private ToolStripButton toolStripButton6 = new ToolStripButton();
        private ToolStripButton toolStripButton5 = new ToolStripButton();
        private ToolStripButton toolStripButton4 = new ToolStripButton();
        private ToolStripButton toolStripButton3 = new ToolStripButton();

        private Dictionary<string, string> getCon = new Dictionary<string, string>();
        SpeechRecognitionEngine oSpeechRecognitionEngine = null;
        System.Speech.Synthesis.SpeechSynthesizer oSpeechSynthesizer = null;

        public Contenedor()
        {
            InitializeComponent();
            
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
        void Botones()
        {
            ToolStripButton Espaciado = new ToolStripButton();
            Espaciado = createToolButton("");
            MenuLateral.Items.Add(Espaciado);

            toolStripButton2 = new ToolStripButton();
            toolStripButton2 = createToolButton("Opciones");
            MenuLateral.Items.Add(toolStripButton2);
            toolStripButton2.Click += toolStripButton2_Click;

            toolStripButton11 = new ToolStripButton();
            toolStripButton11 = createToolButton("Administracion");
            MenuLateral.Items.Add(toolStripButton11);
            toolStripButton11.Click += toolStripButton11_Click;

            toolStripButton10 = new ToolStripButton();
            toolStripButton10 = createToolButton("Recepcion");
            MenuLateral.Items.Add(toolStripButton10);
            toolStripButton10.Click += toolStripButton10_Click;

            toolStripButton9 = new ToolStripButton();
            toolStripButton9 = createToolButton("Gerencial");
            MenuLateral.Items.Add(toolStripButton9);
            toolStripButton9.Click += toolStripButton9_Click;

            toolStripButton8 = new ToolStripButton();
            toolStripButton8 = createToolButton("Enfermeria");
            MenuLateral.Items.Add(toolStripButton8);
            toolStripButton8.Click += toolStripButton8_Click;

            toolStripButton7 = new ToolStripButton();
            toolStripButton7 = createToolButton("Medico General");
            MenuLateral.Items.Add(toolStripButton7);
            toolStripButton7.Click += toolStripButton7_Click;

            toolStripButton6 = new ToolStripButton();
            toolStripButton6 = createToolButton("Fisiatria");
            MenuLateral.Items.Add(toolStripButton6);
            toolStripButton6.Click += toolStripButton6_Click;

            toolStripButton5 = new ToolStripButton();
            toolStripButton5 = createToolButton("T. Ocupacional");
            MenuLateral.Items.Add(toolStripButton5);
            toolStripButton5.Click += toolStripButton5_Click;

            toolStripButton4 = new ToolStripButton();
            toolStripButton4 = createToolButton("T. Fisica");
            MenuLateral.Items.Add(toolStripButton4);
            toolStripButton4.Click += toolStripButton4_Click;

            toolStripButton3 = new ToolStripButton();
            toolStripButton3 = createToolButton("Psicologia");
            MenuLateral.Items.Add(toolStripButton3);
            toolStripButton3.Click += toolStripButton3_Click;

            toolStripButton2.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
            toolStripButton3.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
            toolStripButton4.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
            toolStripButton5.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
            toolStripButton6.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
            toolStripButton7.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
            toolStripButton8.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
            toolStripButton9.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
            toolStripButton10.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
            toolStripButton11.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;

            toolStripButton2.Image = Properties.Resources.Opciones;
            toolStripButton3.Image = Properties.Resources.Opciones;
            toolStripButton4.Image = Properties.Resources.Opciones;
            toolStripButton5.Image = Properties.Resources.Opciones;
            toolStripButton6.Image = Properties.Resources.Opciones;
            toolStripButton7.Image = Properties.Resources.Opciones;
            toolStripButton8.Image = Properties.Resources.Opciones;
            toolStripButton9.Image = Properties.Resources.Opciones;
            toolStripButton10.Image = Properties.Resources.Opciones;
            toolStripButton11.Image = Properties.Resources.Opciones;

            toolStripButton2.ImageAlign = ContentAlignment.MiddleLeft;
            toolStripButton3.ImageAlign = ContentAlignment.MiddleLeft;
            toolStripButton4.ImageAlign = ContentAlignment.MiddleLeft;
            toolStripButton5.ImageAlign = ContentAlignment.MiddleLeft;
            toolStripButton6.ImageAlign = ContentAlignment.MiddleLeft;
            toolStripButton7.ImageAlign = ContentAlignment.MiddleLeft;
            toolStripButton8.ImageAlign = ContentAlignment.MiddleLeft;
            toolStripButton9.ImageAlign = ContentAlignment.MiddleLeft;
            toolStripButton10.ImageAlign = ContentAlignment.MiddleLeft;
            toolStripButton11.ImageAlign = ContentAlignment.MiddleLeft;

            toolStripButton2.TextAlign = ContentAlignment.MiddleLeft;
            toolStripButton3.TextAlign = ContentAlignment.MiddleLeft;
            toolStripButton4.TextAlign = ContentAlignment.MiddleLeft;
            toolStripButton5.TextAlign = ContentAlignment.MiddleLeft;
            toolStripButton6.TextAlign = ContentAlignment.MiddleLeft;
            toolStripButton7.TextAlign = ContentAlignment.MiddleLeft;
            toolStripButton8.TextAlign = ContentAlignment.MiddleLeft;
            toolStripButton9.TextAlign = ContentAlignment.MiddleLeft;
            toolStripButton10.TextAlign = ContentAlignment.MiddleLeft;
            toolStripButton11.TextAlign = ContentAlignment.MiddleLeft;

            toolStripButton2.Size = new Size(MenuLateral.Width, 45);
            toolStripButton3.Size = new Size(MenuLateral.Width, 45);
            toolStripButton4.Size = new Size(MenuLateral.Width, 45);
            toolStripButton5.Size = new Size(MenuLateral.Width, 45);
            toolStripButton6.Size = new Size(MenuLateral.Width, 45);
            toolStripButton7.Size = new Size(MenuLateral.Width, 45);
            toolStripButton8.Size = new Size(MenuLateral.Width, 45);
            toolStripButton9.Size = new Size(MenuLateral.Width, 45);
            toolStripButton10.Size = new Size(MenuLateral.Width, 45);
            toolStripButton11.Size = new Size(MenuLateral.Width, 45);

            Espaciado.Size = new Size(MenuLateral.Width, 25);
            Espaciado.BackColor = Color.FromArgb(25, 55, 90);

            toolStripButton2.MouseEnter += CursorHand;
            toolStripButton2.MouseLeave += UnCursorHand;
            toolStripButton3.MouseEnter += CursorHand;
            toolStripButton3.MouseLeave += UnCursorHand;
            toolStripButton4.MouseEnter += CursorHand;
            toolStripButton4.MouseLeave += UnCursorHand;
            toolStripButton5.MouseEnter += CursorHand;
            toolStripButton5.MouseLeave += UnCursorHand;
            toolStripButton6.MouseEnter += CursorHand;
            toolStripButton6.MouseLeave += UnCursorHand;
            toolStripButton7.MouseEnter += CursorHand;
            toolStripButton7.MouseLeave += UnCursorHand;
            toolStripButton8.MouseEnter += CursorHand;
            toolStripButton8.MouseLeave += UnCursorHand;
            toolStripButton9.MouseEnter += CursorHand;
            toolStripButton9.MouseLeave += UnCursorHand;
            toolStripButton10.MouseEnter += CursorHand;
            toolStripButton10.MouseLeave += UnCursorHand;
            toolStripButton11.MouseEnter += CursorHand;
            toolStripButton11.MouseLeave += UnCursorHand;

            Permisos();
        }
        void Permisos()
        {
            try
            {
                CXN_LOGIN LogAdminForUser = repositorioLogin.getUser(UsuarioLogueado1);
                if (LogAdminForUser != null)
                {
                    toolStripButton2.Visible = LogAdminForUser.Log_Rol_AdminI == "A" ? true : false; // Opciones
                    toolStripButton3.Visible = LogAdminForUser.Log_Rol_Psicologia == "A" ? true : false; //Psicologia
                    toolStripButton4.Visible = LogAdminForUser.Log_Rol_TF == "A" ? true : false; // T Fisica
                    toolStripButton5.Visible = LogAdminForUser.Log_Rol_TO == "A" ? true : false;  //T Ocupacional
                    toolStripButton6.Visible = LogAdminForUser.Log_Rol_FI == "A" ? true : false; //Fisiatria
                    toolStripButton7.Visible = LogAdminForUser.Log_Rol_MedGen == "A" ? true : false; //Medicina general
                    toolStripButton8.Visible = LogAdminForUser.Log_Rol_Enfermero == "A" ? true : false; //Enfermeria
                    toolStripButton9.Visible = LogAdminForUser.Log_Rol_Gerencial == "A" ? true : false; //Gerencial
                    toolStripButton10.Visible = LogAdminForUser.Log_Rol_Recepcion == "A" ? true : false;  //Recepcion
                    toolStripButton11.Visible = LogAdminForUser.Log_Rol_Admin == "A" ? true : false; // Administracion
                }
                else
                {
                    toolStripButton2.Visible = false;
                    toolStripButton3.Visible = false;
                    toolStripButton4.Visible = false;
                    toolStripButton5.Visible = false;
                    toolStripButton6.Visible = false;
                    toolStripButton7.Visible = false;
                    toolStripButton8.Visible = false;
                    toolStripButton9.Visible = false;
                    toolStripButton10.Visible = false;
                    toolStripButton11.Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void CursorHand(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Hand;
        }
        private void UnCursorHand(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Default;
        }
        private void Contenedor_Load(object sender, EventArgs e)
        {
            try
            {
                PanelTitulo.Height = 75;

                Titulo.Text = Tittle;
                LogoMain.Image = Properties.Resources.Splash;
                SubTitulo.Text = SubTittle;
                Titulo.Padding = new Padding(110, 7, 0, 0);
                Titulo.Width = PanelTitulo.Width - 100;
                SubTitulo.Padding = new Padding(90, 10, 0, 0);

                //Boton Cerrar
                ImageClose.Visible = false;
                PanelTitulo.Controls.Add(pictureBox2);
                pictureBox2.BackColor = Color.FromArgb(237, 237, 237);
                pictureBox2.Location = new Point(PanelTitulo.Width - 180, 21);
                pictureBox2.Anchor = AnchorStyles.Top;
                pictureBox2.Anchor = AnchorStyles.Right;
                pictureBox2.Cursor = Cursors.Hand;
                pictureBox2.BringToFront();
                //FIN Boton Cerrar

                label78.BringToFront();
                label7.BringToFront();
                label72.BringToFront();
                label73.BringToFront();
                pictureBox4.BringToFront();
                pictureBox4.BackColor = Color.FromArgb(237, 237, 237);

                Botones();

                if (Conexion.ConectionDictionary["Mensaje"] == "Vencido" || Conexion.ConectionDictionary["Mensaje"] == "Cancelado")
                {
                    label3.Visible = true;
                }
                else
                {
                    label3.Visible = false;
                }

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
                toolTip12.SetToolTip(ImageMinimize, "Minimizar Aplicacion");
                toolTip14.SetToolTip(linkLabel5, "Abrir Reloj Analogo");
                toolTip15.SetToolTip(pictureBox4, "Haga doble click aqui para cambiar su avatar");
                toolTip78.SetToolTip(label78, "¿Que es esto?");
                toolTipWebOficial.SetToolTip(linkLabel2, "Este vinculo te direcciona a la web oficial de productos Zamenis del Autor Fabian Gamba");
                toolTipAPP.SetToolTip(linkLabel1, "Aqui puedes descargar la APP solo disponible para Android 13 en adelante, si eres medico o enfermero puedes subir imagenes de tus pacientes y consultar tu agenda diaria");
                toolTipNovedades.SetToolTip(linkLabel8, "Aqui consultas los ultimos cambios del Software y las ultimas novedades");

                getCon = Conexion.Conection();               

                chargeAvatar();
                _ = ConfigForm.FadeInControl(pictureBox4);

                bool noticia = false;

                List<CXN_IMAGEN_SYSTEM> d = repositorioConfSystem.getConfSystem();
                if (d != null)
                {
                    CXN_IMAGEN_SYSTEM d2 = d.FirstOrDefault(x => x.Tab_Nombre == "Noticia");
                    if (d2 != null)
                    {
                        noticia = d2.Tab_Clave == "A" ? true : false;
                    }
                }

                if (noticia == true)
                {
                    Comunes.Noticias N = new Comunes.Noticias();
                    N.ShowDialog();
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

                List<CXN_IMAGEN_SYSTEM> d22 = repositorioConfSystem.getConfSystem();
                if (d22 != null)
                {
                    Conexion.BloqueosAgenda = d22.FirstOrDefault(x => x.Tab_Nombre == "Bloqueo").Tab_Clave;
                    Conexion.EmailAgenda = d22.FirstOrDefault(x => x.Tab_Nombre == "Email").Tab_Clave;
                    Conexion.SMSAgenda = d22.FirstOrDefault(x => x.Tab_Nombre == "SMS").Tab_Clave;
                    label4.Text = d22.FirstOrDefault(x => x.Tab_Nombre == "Base").Tab_Clave;
                }                

                GestionPagos();
                ConsultarPatron();
                EstablecerCIE11();
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        void EstablecerCIE11()
        {
            try
            {
                List<CXN_IMAGEN_SYSTEM> d = repositorioConfSystem.getConfSystem();
                if (d != null)
                {
                    CXN_IMAGEN_SYSTEM d2 = d.FirstOrDefault(x => x.Tab_Nombre == "API-CIE11");
                    if (d2 != null)
                    {
                        Conexion.URL_CIE11 = d2.Tab_Clave;
                        Conexion.URL_Version_CIE11 = d2.Tab_Config;
                    }
                    else
                    {
                        Conexion.URL_CIE11 = "";
                        Conexion.URL_Version_CIE11 = "";
                    }
                }
                else
                {
                    Conexion.URL_CIE11 = "";
                    Conexion.URL_Version_CIE11 = "";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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
            NoticiasHistorial N = new NoticiasHistorial();
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
        private void pictureBox2_Click(object sender, EventArgs e)
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
    }
}