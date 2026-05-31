using APIController;
using Domain.CXN;
using FormAndControls;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using ShortLink.SDK.Clases;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZamenisHealth.Clases;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.Medicina.VirtualMedic
{
    public partial class TwilioForm : Forma
    {
        private string roomName = "";
        private int Admision, Cia;

        private MensajesGeneral MG;

        private readonly ILogin repoLogin = new MLogin();
        private readonly IAgendaC repoAgenda = new MAgendaC();
        private readonly IConfSystem repoConf = new MConfSystem();
        private readonly IHelisa repoHelisa = new MHelisa();
        private readonly IDatosEmail repoEmail = new MDatosEmail();
        private readonly IPacientes repoPac = new MPacientes();
        private readonly ITwilio repoTwi = new MTwilio();

        private string Medico, Paciente, URLSALA;

        ToolStripButton btnSendSMS;
        ToolStripButton btnCrear;
        ToolStripButton btnSendEmail;
        ToolStripButton btnIngresar;
        ToolStripButton btnHistorialTwilio;

        public TwilioForm(int admision)
        {
            InitializeComponent();
            Admision = admision;
        }
        private void button4_Click(object sender, EventArgs e)
        {
            if (repoPac.ValidaCelular(textBox3.Text) == false)
            {
                MG = new MensajesGeneral();
                MG.Mensaje = "El formato del celular no es válido.";
                MG.TipoImagen = 1000;
                MG.ShowDialog();
                return;
            }

            EnviarSMS(textBox1.Text);
        }
        private void button3_Click(object sender, EventArgs e)
        {
            if (repoPac.ValidaEmail(textBox4.Text) == false)
            {
                MG = new MensajesGeneral();
                MG.Mensaje = "El formato del correo electrónico no es válido.";
                MG.TipoImagen = 1000;
                MG.ShowDialog();
                return;
            }
            EnviarEMAIL(textBox1.Text);
        }
        void EnviarEMAIL(string URL)
        {
            try
            {
                string mensaje = "<center>" +
                                   "<h2 style='background:#3D38B2; color:white;'>Hola, " + Paciente + "</h2>" +
                                   "<p>Link de Acceso a la Videollamada: " + URL + "<br><br>" +
                                   "<br><br>" +
                                   "</p>" +
                                   "AVISO LEGAL: La información transmitida a través de este correo electrónico es confidencial y dirigida única y exclusivamente para uso de su(s) destinatario(s). " +
                                   "Su reproducción, lectura o uso está prohibido a cualquier persona o entidad diferente, sin autorización previa por escrito. Si usted lo ha recibido por error, " +
                                   "por favor notifíquelo inmediatamente al remitente y elimínelo de su sistema. Cualquier uso, divulgación, copia, distribución, impresión o acto derivado del " +
                                   "conocimiento total o parcial de este mensaje sin autorización del remitente será sancionado de acuerdo con las normas legales vigentes. Las opiniones, " +
                                   "conclusiones y otra información contenida en este correo, no relacionadas con las actividades de la institucion medica, deben entenderse como personales " +
                                   "y de ninguna manera son avaladas por dicha Institución. Aunque la institucion medica ha realizado su mejor esfuerzo para asegurar que el presente mensaje y " +
                                   "sus archivos anexos se encuentran libre de virus y defectos que puedan llegar a afectar los computadores o sistemas que lo reciban, no se hace responsable " +
                                   "por la eventual transmisión de virus o programas dañinos por este conducto, y por lo tanto es responsabilidad del destinatario confirmar la existencia " +
                                   "de este tipo de elementos al momento de recibirlo y abrirlo. No se acepta responsabilidad alguna por eventuales daños o alteraciones derivados de la recepción " +
                                   "o uso del presente mensaje." +
                                   "</p>" +
                               "</center>";

                var getFirstEmail = repoEmail.FirstEmail();
                APIController.Services.Email.EmailService emailService = new APIController.Services.Email.EmailService();
                string enviarAPI = emailService.SendEmailAPI(new APIController.Clases.EmailRequest
                {
                    EmailFrom = getFirstEmail.Ema_Email,
                    EmailTo = textBox4.Text.Trim(),
                    EmailPassword = getFirstEmail.Ema_Pass,
                    EmailBcc1 = getFirstEmail.Ema_Email,
                    EmailBcc2 = null,
                    Asunto = "CITA TELEMEDICINA",
                    AttachmentFile = null,
                    BodyMessage = mensaje,
                    TipoArchivo = null
                }, Program.URLApiConexion).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        async void EnviarSMS(string URL)
        {
            try
            {
                Dictionary<string, string> gtData = repoHelisa.Claves("ShortLinks", Cia);

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
                    UrlOriginal = "Videollamada: " + URL
                };

                ShortLink.SDK.ShortLinks Sl = new ShortLink.SDK.ShortLinks();
                ShortLink.SDK.EnviarSMS sms = new ShortLink.SDK.EnviarSMS();
                ResponseShortLinks res = await Sl.CrearLinkCorto(Rsl);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }        
        private void TwilioForm_Load(object sender, EventArgs e)
        {
            try
            {
                Titulo.Text = "Adherencia";
                LogoMain.Image = Properties.Resources.Splash;

                btnCrear = new ToolStripButton();
                btnCrear = createToolButton("Crear Reunion");
                MenuLateral.Items.Add(btnCrear);
                btnCrear.Click += button1_Click;

                btnSendSMS = new ToolStripButton();
                btnSendSMS = createToolButton("Enviar SMS");
                MenuLateral.Items.Add(btnSendSMS);
                btnSendSMS.Click += button4_Click;
                btnSendSMS.Enabled = false;

                btnSendEmail = new ToolStripButton();
                btnSendEmail = createToolButton("Enviar Email");
                MenuLateral.Items.Add(btnSendEmail);
                btnSendEmail.Click += button3_Click;
                btnSendEmail.Enabled = false;

                btnIngresar = new ToolStripButton();
                btnIngresar = createToolButton("INGRESAR");
                MenuLateral.Items.Add(btnIngresar);
                btnIngresar.Click += button2_Click;
                btnIngresar.Enabled = false;

                btnHistorialTwilio = new ToolStripButton();
                btnHistorialTwilio = createToolButton("Historial URLS");
                MenuLateral.Items.Add(btnHistorialTwilio);
                btnHistorialTwilio.Click += His_Click;

                textBox3.MaxLength = 10;

                CXN_LOGIN login = repoLogin.getUser(Contenedor.UsuarioLogueado);
                if (login != null)
                {
                    Medico = login.Log_PrimerN + " " + login.Log_PrimerA;

                    otrosDatosPacienteHorario paciente = repoAgenda.cargarAdmision(Admision, "'P'");
                    if (paciente != null)
                    {
                        URLSALA = repoConf.getListado()["VideoconferenciaTwilio1"];
                        Paciente = paciente.Pac_PrimerN + "_" + paciente.Pac_PrimerA;
                        Cia = paciente.Hor_Pac_Cia;
                        textBox4.Text = paciente.Pac_Email;
                        textBox3.Text = paciente.Pac_Telefono;
                        this.BringToFront();
                    }
                    else
                    {
                        MG = new MensajesGeneral();
                        MG.Mensaje = "Error al cargar los datos del paciente o esta cita ya esta generada y no puede establecer sala virtual";
                        MG.TipoImagen = 1000;
                        MG.ShowDialog();

                        this.Dispose();
                        this.Close();
                    }
                }
                else
                {
                    MG = new MensajesGeneral();
                    MG.Mensaje = "Error al cargar los datos del usuario";
                    MG.TipoImagen = 1000;
                    MG.ShowDialog();

                    this.Dispose();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void His_Click(object sender, EventArgs e)
        {
            HistorialTwilio historialTwilio = new HistorialTwilio();
            historialTwilio.ShowDialog();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(textBox2.Text);
        }
        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Dispose();
            this.Close();
        }
        public async Task<string> GenerarLinkAsync(string Name, string Rol, string Room)
        {
            try
            {
                HttpClientHandler handler = new HttpClientHandler();
                handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;

                using (HttpClient client = new HttpClient(handler))
                {
                    client.Timeout = TimeSpan.FromSeconds(ControladorConfiguracion.TimeOutConection);

                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    // Construimos los parámetros en la URL                                                            
                    string endpoint = Program.URLApiConexion + $"/api/Twilio/token?roomName={Room}&identitys={Name}&rol={Rol}"; //TOKEN 

                    // En GET no enviamos contenido en el body
                    HttpResponseMessage response = await client.GetAsync(endpoint).ConfigureAwait(false);

                    var res = await response.Content.ReadAsStringAsync();

                    if (Rol == "cliente")
                    { 
                        string urlC = URLSALA + "?identity=" + Name + "&roomName=" + Room + "&rol=" + Rol + "&token=" + res;                        
                        textBox1.Text = await GenerarLink(urlC);
                    }
                    if (Rol == "moderador")
                    {
                        string urlM = URLSALA + "?identity=" + Name + "&roomName=" + Room + "&rol=" + Rol + "&token=" + res;
                        textBox2.Text = await GenerarLink(urlM);
                    }

                    return res;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return ex.Message;
            }
        }
        async Task<string> GenerarLink(string URLOriginal)
        {
            try
            {
                Dictionary<string, string> gtData = repoHelisa.Claves("ShortLinks", Cia);

                DatosAPI Datos = new DatosAPI
                {
                    CodeUserSDK = gtData["Code"],
                    PassSDK = gtData["Pass"],
                    UserSDK = gtData["User"]
                };
                //c8a56c6d-f56e-4c0e-9e1e-52a1b3eb0df9
                ShortLink.SDK.Clases.RequestShortLink Rsl = new ShortLink.SDK.Clases.RequestShortLink
                {
                    CodeUser = Datos.CodeUserSDK,
                    User = Datos.UserSDK,
                    Pass = Datos.PassSDK,
                    UrlOriginal = URLOriginal
                };

                ShortLink.SDK.ShortLinks Sl = new ShortLink.SDK.ShortLinks();
                ResponseShortLinks res = await Sl.CrearLinkCorto(Rsl);

                if (res != null)
                {
                    if (res.Estado == "OK")
                    {
                        return res.Url;
                    }
                    else
                    {
                        return res.Detalles;
                    }
                }
                else
                {
                    return "No se logro generar el link";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(" ---- Error Generar Link: " + ex.Message);
                return "ERROR DE LINK - Genere uno Nuevo";
            }
        }
        private async void button1_Click(object sender, EventArgs e)
        {
            roomName = Guid.NewGuid().ToString();
            Console.WriteLine("Este es el GUID --> " + roomName);
            GenerarLinkAsync(Medico, "moderador", roomName).Wait();
            GenerarLinkAsync(Paciente, "cliente", roomName).Wait();

            CXN_LINKSCORTOS c = new CXN_LINKSCORTOS()
            {
                Admision = Admision,
                Cliente = textBox1.Text.Trim(),
                Fecha = DateTime.Now,
                Servidor = textBox2.Text.Trim(),
                Usuario = Contenedor.UsuarioLogueado
            };

            repoTwi.Insertar(c);

            btnCrear.Enabled = true;
            btnSendEmail.Enabled = true;
            btnIngresar.Enabled = true;
        }
    }
}
