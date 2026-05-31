using Domain.CXN;
using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZamenisHealth.Clases;

namespace ZamenisHealth.Comunes.Extras
{
    public partial class PatronRecovery : Forma2
    {
        private static readonly ILogin repoLogin = new MLogin(); 
        private static readonly IPacientes repositorioPacientes = new MPacientes();
        private static readonly IDatosEmail repositorioDatosEmail = new MDatosEmail();
        private static readonly IRestorePass repositorioRPass = new MRestorePass();
        private static readonly IGenerales repositorioGenerales = new MGenerales();

        private string User;
        private MensajesGeneral MG;
        private string destiny, PatronBD;

        public PatronRecovery(string user)
        {
            InitializeComponent();
            User = user;
        }

        private void PatronRecovery_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Recuperar Contraseña";
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";

            recoveryPass1.MouseUp += RecoveryPass1_MouseUp;

            var user = repoLogin.getUser(User.ToUpper().Trim());
            if (user != null)
            {
                if (string.IsNullOrEmpty(user.Patron))
                {
                    recoveryPass1.PatronMarcado = "";
                    MessageBox.Show("El usuario digitado no tiene un patron de desbloqueo actualmente, contacte a soporte", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    this.Dispose();
                    this.Close();
                }
                else
                {
                    PatronBD = user.Patron.Trim();
                }
            }
            else
            {
                recoveryPass1.PatronMarcado = "";
                MessageBox.Show("El usuario digitado no existe", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                this.Dispose();
                this.Close();
            }
        }
        private void RecoveryPass1_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(recoveryPass1.PatronMarcado))
                {
                    if (PatronBD.Trim() == recoveryPass1.PatronMarcado.Trim())
                    {
                        recoveryPass1.PatronMarcado = "";

                        Claves claves = new Claves(User.Trim().ToUpper());
                        claves.ShowDialog();
                    }
                    else
                    {
                        recoveryPass1.PatronMarcado = "";
                        MessageBox.Show("El patron dibujado no es valido para el usuario", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private async void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(User))
                {
                    MG = new MensajesGeneral();
                    MG.Mensaje = "Debe digitar su usuario para recordar su clave";
                    MG.TipoImagen = 1000;
                    MG.ShowDialog();
                    return;
                }

                CXN_LOGIN getEmailUser = repoLogin.getUser(User);
                if (getEmailUser != null)
                {
                    if (string.IsNullOrEmpty(getEmailUser.Log_Email))
                    {
                        MG = new MensajesGeneral();
                        MG.Mensaje = "Su usuario no tiene registrado un correo electronico para recordar su clave, contacte al administrador del sistema en su IPS";
                        MG.TipoImagen = 1000;
                        MG.ShowDialog();
                        return;
                    }

                    if (repositorioPacientes.ValidaEmail(getEmailUser.Log_Email) == false)
                    {
                        MG = new MensajesGeneral();
                        MG.Mensaje = "El Email registrado en su usuario para recordar claves, no tiene el formato correcto.  Contacto al administrador del sistema en su IPS";
                        MG.TipoImagen = 1000;
                        MG.ShowDialog();
                        return;
                    }

                    destiny = getEmailUser.Log_Email;

                    Shows();
                    Task oTask = new Task(Enviar);
                    oTask.Start();
                    await oTask;
                    Hides();

                    MG = new MensajesGeneral();
                    MG.Mensaje = "Se ha enviado un recordatorio de claves a su correo electronico registrado en sistema " + CodificadorVC.MaskEmail(getEmailUser.Log_Email) + ", revise la Bandeja de Entrada y el SPAM";
                    MG.TipoImagen = 3;
                    MG.ShowDialog();
                }
                else
                {
                    MG = new MensajesGeneral();
                    MG.Mensaje = "El usuario digitado no existe";
                    MG.TipoImagen = 1000;
                    MG.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MG = new MensajesGeneral();
                MG.Mensaje = ex.Message;
                MG.TipoImagen = 1000;
                MG.ShowDialog();
            }
        }
        void Shows()
        {
            pictureBox1.Visible = true;
        }
        void Hides()
        {
            pictureBox1.Visible = false;
        }
        void Enviar()
        {
            MG = new MensajesGeneral();

            CXN_RESTOREPASS rP = new CXN_RESTOREPASS
            {
                Fecha = DateTime.Now,
                Habilitado = "A",
                Usuario = User.ToString().Trim(),
            };

            int resultadoId = repositorioRPass.InsertRestorePass(rP);
            if (resultadoId == 0)
            {
                MG.TipoImagen = 1000;
                MG.Mensaje = "No se logro restaurar la clave";
                MG.ShowDialog();
                return;
            }

            string UrlRestorePass = Conexion.getURLRestorePass() + repositorioGenerales.Base64Encode(resultadoId.ToString()).ToString();

            string html = "<center>" +
                                      "<img src='cid:imagen' />" +
                                      "<h2 style='background:#3D38B2; color:white;'>Recordatorio de Claves para Zamenis Health</h2>" +
                                      "<p><a style='background:blue; color:white; padding:3px; border: 3px solid #000; border-color:black;' href='" + UrlRestorePass + "'>Haga clic AQUI para restaurar su clave</a><br><br>" +
                                        "Este link esta disponible por 1 hora o 1 vez de uso, posteriormente sera deshabilitado y debera solictar un nuevo enlace." +
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

            CXN_EMAIL getFirstEmail = repositorioDatosEmail.FirstEmail();

            APIController.Services.Email.EmailService emailService = new APIController.Services.Email.EmailService();
            string enviarAPI = emailService.SendEmailAPI(new APIController.Clases.EmailRequest
            {
                EmailFrom = getFirstEmail.Ema_Email,
                EmailTo = destiny,
                EmailPassword = getFirstEmail.Ema_Pass,
                EmailBcc1 = getFirstEmail.Ema_Email,
                EmailBcc2 = null,
                Asunto = "RECORDATORIO DE CLAVES - ZAMENIS HEALTH",
                AttachmentFile = null,
                BodyMessage = html,
                TipoArchivo = null
            }, Program.URLApiConexion).GetAwaiter().GetResult();

            MG.TipoImagen = 0;
            MG.Mensaje = enviarAPI.ToString();
            MG.ShowDialog();
        }
    }
}
