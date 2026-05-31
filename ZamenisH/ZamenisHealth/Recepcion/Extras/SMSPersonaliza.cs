using Domain;
using Domain.CXN;
using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using ShortLink.SDK.Clases;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ZamenisHealth.Clases;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.Recepcion.Extras
{
    public partial class SMSPersonaliza : Forma
    {
        private static readonly ICompañia repositorioCompañia = new MCompañia();
        private static readonly ILogSender repositorioLogSender = new MLogSender();
        private static readonly IHelisa repoHelisa = new MHelisa(); 

        private int Admi, CIA;
        private string Men, Cel;
        private Comunes.MensajesGeneral MG;

        private async void button1_Click(object sender, EventArgs e)
        {
            try
            {
                CXN_CIA getCantSMS = repositorioCompañia.getPrestadorbyCode(CIA);
                if (getCantSMS.Com_SMS <= 0)
                {
                    MG = new MensajesGeneral();
                    MG.Mensaje = "No hay mensajes disponibles, recargue su cuenta";
                    MG.TipoImagen = 1000;
                    MG.ShowDialog();
                    return;
                }

                Dictionary<string, string> gtData = repoHelisa.Claves("ShortLinks", CIA);

                DatosAPI Datos = new DatosAPI
                {
                    CodeUserSDK = gtData["Code"],
                    PassSDK = gtData["Pass"],
                    UserSDK = gtData["User"]
                };

                ShortLink.SDK.Clases.RequestShortLink Rsl = new ShortLink.SDK.Clases.RequestShortLink
                {
                    CodeUser = Datos.CodeUserSDK,
                    User = Datos.UserSDK,
                    Pass = Datos.PassSDK,
                    UrlOriginal = richTextBox1.Text.Trim()
                };

                ShortLink.SDK.ShortLinks Sl = new ShortLink.SDK.ShortLinks();
                ShortLink.SDK.EnviarSMS sms = new ShortLink.SDK.EnviarSMS();
                ResponseShortLinks res = await Sl.CrearLinkCorto(Rsl);

                Log(Admi, res.Detalles, richTextBox1.Text, Cel);

                this.Dispose();
                this.Close();
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void SMSPersonaliza_Load(object sender, EventArgs e)
        {
            
            richTextBox1.MaxLength = 160;
            richTextBox1.Text = Men.ToString();

            Titulo.Text = "Enviar Mensaje de Texto";
            LogoMain.Image = Properties.Resources.Splash;

            ToolStripButton btnEnviar = new ToolStripButton();
            btnEnviar = createToolButton("Enviar");
            MenuLateral.Items.Add(btnEnviar);
            btnEnviar.Click += button1_Click;
        }

        public SMSPersonaliza(int _admi, string _men, string _cel, int _cia)
        {
            InitializeComponent();
            this.Admi = _admi;
            this.Men = _men;
            this.Cel = _cel;
            this.CIA = _cia;     
        }

        public void Log(int Admision,
                      string Salida,
                      string Mensaje_SMS,
                      string Celular)
        {
            try
            {
                
                    repositorioLogSender.Log(Admision, Salida, Mensaje_SMS, Celular, Comunes.Contenedor.UsuarioLogueado, "SMS");
                                
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
    }
}
