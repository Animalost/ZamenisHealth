using Domain;
using Domain.CXN;
using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ZamenisHealth.Comunes.Extras
{
    public partial class Opiniones : Forma
    {
        private static readonly IDatosEmail repoEmail = new MDatosEmail();

        MensajesGeneral MG;
        Espera E;
        string opinion;

        public Opiniones()
        {
            InitializeComponent();
        }

        private async void toolStripButton2_Click(object sender, EventArgs e)
        {
            try
            {
                if (richTextBox1.Text == "")
                {
                    MG = new MensajesGeneral();
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "Debe escribir una opinion";
                    MG.ShowDialog();
                    return;
                }

                opinion = richTextBox1.Text;
                Shows();
                Task oTask = new Task(Enviar);
                oTask.Start();
                await oTask;
                Hides();

                MG = new MensajesGeneral();
                MG.TipoImagen = 3;
                MG.Mensaje = "Sus opiniones son valiosas para nosotros ya la hemos recibido y entrara en estudio.  Gracias";
                MG.ShowDialog();

                this.Dispose();
                this.Close();
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        void Enviar()
        {
            try
            {
                CXN_EMAIL getFirstEmail = repoEmail.FirstEmail();

                string html = "<center>" +
                                      "<img src='cid:imagen' />" +
                                      "<h2 style='background:#3D38B2; color:white;'>Hola, Opinion Hecha</h2>" +
                                      "<p>" + Contenedor.UsuarioLogueado + " ----> " + opinion + "<br><br>" +
                                      "</p>" +
                                      "<b style='color:gray'>" +
                                      "AVISO LEGAL: La información transmitida a través de este correo electrónico es confidencial y dirigida única y exclusivamente para uso de " +
                                      "su(s) destinatario(s). Su reproducción, lectura o uso está prohibido a cualquier persona o entidad diferente, sin autorización previa por " +
                                      "escrito. Si usted lo ha recibido por error, por favor notifíquelo inmediatamente al remitente y elimínelo de su sistema. Cualquier uso, " +
                                      "divulgación, copia, distribución, impresión o acto derivado del conocimiento total o parcial de este mensaje sin autorización del remitente " +
                                      "será sancionado de acuerdo con las normas legales vigentes. Las opiniones, conclusiones y otra información contenida en este correo, " +
                                      "no relacionadas con las actividades de la institucion medica, deben entenderse como personales y de ninguna manera son avaladas por dicha " +
                                      "Institución. Aunque la institucion medica ha realizado su mejor esfuerzo para asegurar que el presente mensaje y sus archivos anexos se " +
                                      "encuentran libre de virus y defectos que puedan llegar a afectar los computadores o sistemas que lo reciban, no se hace responsable por la " +
                                      "eventual transmisión de virus o programas dañinos por este conducto, y por lo tanto es responsabilidad del destinatario confirmar la existencia " +
                                      "de este tipo de elementos al momento de recibirlo y abrirlo. No se acepta responsabilidad alguna por eventuales daños o alteraciones derivados de " +
                                      "la recepción o uso del presente mensaje." +
                                  "</center>";

                APIController.Services.Email.EmailService emailService = new APIController.Services.Email.EmailService();
                string enviarAPI = emailService.SendEmailAPI(new APIController.Clases.EmailRequest
                {
                    EmailFrom = getFirstEmail.Ema_Email,
                    EmailTo = "fgamba00@gmail.com",
                    EmailPassword = getFirstEmail.Ema_Pass,
                    EmailBcc1 = null,
                    EmailBcc2 = null,
                    Asunto = "OPINIONES ZAMENIS HEALTH",
                    AttachmentFile = null,
                    BodyMessage = html,
                    TipoArchivo = null
                }, Program.URLApiConexion).GetAwaiter().GetResult();

                Hides();

                MG = new MensajesGeneral();
                MG.TipoImagen = 0;
                MG.Mensaje = enviarAPI.ToString();
                MG.ShowDialog();
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        void Shows()
        {
            E = new Espera();
            E.Show();
        }
        void Hides()
        {
            if (E != null)
                E.Close();
        }
        private void Opiniones_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Opiniones";
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";
            LogoMain.Image = Properties.Resources.Splash;

            ToolStripButton btnSend = new ToolStripButton();
            btnSend = createToolButton("Enviar Opinion");
            MenuLateral.Items.Add(btnSend);
            btnSend.Click += toolStripButton2_Click;
        }
    }
}
