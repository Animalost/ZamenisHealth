using Domain;
using Domain.CXN;
using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ZamenisHealth.Clases;

namespace ZamenisHealth.Comunes
{
    public partial class MensajeroSend2 : Forma
    {
        private CXN_MESSENGER _messageSend;
        private static readonly IMensajeria repositorioMensajeria = new MMensajeria();
        private static readonly IGenerales repositorioGenerales = new MGenerales();
        private static readonly ILogin repositorioLogin = new MLogin();

        public MensajeroSend2(CXN_MESSENGER M)
        {
            InitializeComponent();
            _messageSend = new CXN_MESSENGER();
            this._messageSend = M;
        }

        private void MensajeroSend2_Load(object sender, EventArgs e)
        {
            try
            {
                this.ImageClose.Visible = false;
                this.ImageMinimize.Visible = false;
                this.Titulo.Text = "Mensaje Recibido";
                SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";

                LogoMain.Image = Properties.Resources.Splash;

                ToolStripButton btnGrabar = new ToolStripButton();
                btnGrabar = createToolButton("Responder");
                MenuLateral.Items.Add(btnGrabar);
                btnGrabar.Click += button2_Click;

                ToolStripButton btnClose = new ToolStripButton();
                btnClose = createToolButton("Cerrar");
                MenuLateral.Items.Add(btnClose);
                btnClose.Click += toolStripButton3_Click;

                Contenedor f2 = Application.OpenForms.OfType<Contenedor>().SingleOrDefault();
                f2.timer3.Enabled = false;

                consultarEnviado();

                richTextBox4.Text = this._messageSend.Men_Usuario_De + "\n\r" + this._messageSend.Men_Mensaje;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }   
        }
        
        void consultarEnviado()
        {
            try
            {
                if (this._messageSend != null)
                {
                    CXN_LOGIN getUsear = repositorioLogin.getUser(this._messageSend.Men_Usuario_De.ToString());                    

                    label1.Text = getUsear.Log_PrimerA + " " + getUsear.Log_SegundoA + " " + getUsear.Log_PrimerN + " " + getUsear.Log_SegundoN;                 
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        public void lodaAvatar()
        {
            try
            {
                pictureBox1.Visible = true;
                byte[] _getAvatar = null;

                _getAvatar = repositorioLogin.getAvatar(this._messageSend.Men_Usuario_De);

                if (_getAvatar != null)
                {
                    Image recoveryImage = repositorioGenerales.ByteToImage(_getAvatar);
                    pictureBox1.Image = recoveryImage;
                }

                _ = ConfigForm.FadeInControl(pictureBox1);
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            repositorioMensajeria.Leido(this._messageSend.Men_Id);
            
            MensajeroSend mensajeroSend = new MensajeroSend(label1.Text);
            mensajeroSend.ShowDialog();

            this.Dispose();
            this.Close();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            Contenedor f2 = Application.OpenForms.OfType<Contenedor>().SingleOrDefault();
            f2.timer3.Enabled = true;
            repositorioMensajeria.Leido(this._messageSend.Men_Id);

            this.Dispose();
            this.Close();
        }

        private void MensajeroSend2_FormClosing(object sender, FormClosingEventArgs e)
        {
            Contenedor f2 = Application.OpenForms.OfType<Contenedor>().SingleOrDefault();
            f2.timer3.Enabled = true;
            repositorioMensajeria.Leido(this._messageSend.Men_Id);
        }
    }
}
