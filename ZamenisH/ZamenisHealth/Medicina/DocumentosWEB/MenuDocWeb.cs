using APIController.Services.SignDocuments;
using Domain.CXN;
using FormAndControls;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using ShortLink.SDK.Clases;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ZamenisHealth.Clases;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.Medicina.DocumentosWEB
{
    public partial class MenuDocWeb : Forma
    {
        private readonly ICompañia repoCia = new MCompañia();
        private readonly IPacientes repoPac = new MPacientes();
        private readonly IBodegas repoBod = new MBodegas();
        private readonly IConfSystem repoConf = new MConfSystem();
        private readonly IHelisa repoHelisa = new MHelisa();
        private readonly IDatosEmail repoEmail = new MDatosEmail();
        private readonly IGenerales repoGeneral = new MGenerales();

        private readonly APIController.Services.SignDocuments.SignDocument repoSign = new APIController.Services.SignDocuments.SignDocument();
        private int Cia;
        private MensajesGeneral MG;
        private string DocumentoPac, DocumentoMed, NombreProfesional, FirmaMed, NIT;

        public MenuDocWeb()
        {
            InitializeComponent();
            SoloNumeros(textBox4);
        }
        private void MenuDocWeb_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Documentos WEB";
            LogoMain.Image = Properties.Resources.Splash;

            ToolStripButton btnBuscar = new ToolStripButton();
            btnBuscar = createToolButton("Buscar");
            MenuLateral.Items.Add(btnBuscar);
            btnBuscar.Click += button1_Click;

            ToolStripButton btnHistorico = new ToolStripButton();
            btnHistorico = createToolButton("Historico");
            MenuLateral.Items.Add(btnHistorico);
            btnHistorico.Click += button4_Click;

            ToolStripButton btnNotificar = new ToolStripButton();
            btnNotificar = createToolButton("Notificar");
            MenuLateral.Items.Add(btnNotificar);
            btnNotificar.Click += button2_Click;

            CargarCia();
            CargarDatoDoc();
            LoadTipos();
        }
        void LoadTipos()
        {
            Dictionary<string,string> getT = repoConf.getListado();
            if (getT == null)
            {
                MG = new MensajesGeneral();
                MG.Mensaje = "Nohay tipos de consentimeintos";
                MG.TipoImagen = 1000;
                MG.ShowDialog();

                this.Dispose();
                this.Close();
            }
            else
            {
                if (getT.ContainsKey("ConsentimientosWEB1"))
                {
                    comboBox1.Items.Add("Consentimiento Informado Curaciones");
                }
                if (getT.ContainsKey("ConsentimientosWEBPSI1"))
                {
                    comboBox1.Items.Add("Consentimiento Informado Fibromialgia");
                }
            }
        }
        void CargarDatoDoc()
        {
            try
            {
                CXN_BODEGAS detDataMed = repoBod.getDatosUser(Contenedor.UsuarioLogueado);
                if (detDataMed == null)
                {
                    MG = new MensajesGeneral();
                    MG.Mensaje = "Usted no se encuentra registrado como usuario asistencial";
                    MG.TipoImagen = 1000;
                    MG.ShowDialog();

                    this.Dispose();
                    this.Close();
                }
                else
                {
                    DocumentoMed = detDataMed.Bod_Reg_Med;
                    NombreProfesional = detDataMed.Bod_Responsable;
                    FirmaMed = detDataMed.Bod_Firma;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        void CargarCia()
        {
            List<CXN_CIA> getCias = repoCia.getAllCompañias();
            foreach (CXN_CIA c in getCias)
            {
                comboBox2.Items.Add(c.Com_Nombre);
            }

            comboBox2.SelectedIndex = 0;
        }
        private void textBox1_DoubleClick(object sender, EventArgs e)
        {
            BuscarPacientes B = new BuscarPacientes();
            B.Tipo_Busca_Pac = "ConsentimientosWEB";
            B.ShowDialog();
        }
        public void setDoc(string Doc)
        {
            textBox1.Text = Doc.Trim();
        }
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                CXN_CIA C = repoCia.getPrestadorbyName(comboBox2.Text);
                if (C != null)
                {
                    Cia = C.Com_Identificador;
                    NIT = C.Com_Identificacion;
                }
                else
                {
                    Cia = 0;
                    NIT = "";
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }            
        }
        private void button4_Click(object sender, EventArgs e)
        {
            Historico historico = new Historico();
            historico.ShowDialog();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (comboBox1.Text == "")
                {
                    MG = new MensajesGeneral();
                    MG.Mensaje = "Seleccione tipo de documento a enviar";
                    MG.TipoImagen = 0;
                    MG.ShowDialog();
                    return;
                }

                CXN_PACIENTES P = repoPac.LlamarPacienteNumDoc(textBox1.Text.Trim());
                if (P != null)
                {
                    textBox2.Text = P.Pac_PrimerA + " " + P.Pac_SegundoN + " " + P.Pac_PrimerN + " " + P.Pac_SegundoN;
                    textBox3.Text = P.Pac_Email.Trim();
                    textBox4.Text = P.Pac_Telefono;
                    DocumentoPac = P.Pac_IdNum.Trim();
                }
                else
                {
                    textBox2.Text = "";
                    textBox3.Text = "";
                    textBox4.Text = "";
                    DocumentoPac = "";

                    MG = new MensajesGeneral();
                    MG.Mensaje = "Paciente no encontrado";
                    MG.TipoImagen = 1000;
                    MG.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        async void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (comboBox1.Text == "")
                {
                    MG = new MensajesGeneral();
                    MG.Mensaje = "Debe seleccionar un tipo de documento";
                    MG.TipoImagen = 1000;
                    MG.ShowDialog();
                }
                else
                {
                    if (repoPac.ValidaEmail(textBox3.Text.Trim()) == false)
                    {
                        MG = new MensajesGeneral();
                        MG.Mensaje = "El correo electronico diligenciado no es valido";
                        MG.TipoImagen = 1000;
                        MG.ShowDialog();
                    }
                    else if (repoPac.ValidaCelular(textBox4.Text) == false)
                    {
                        MG = new MensajesGeneral();
                        MG.Mensaje = "El formato del numero celular es invalido";
                        MG.TipoImagen = 1000;
                        MG.ShowDialog();
                    }
                    else
                    {
                        if (comboBox1.Text == "Consentimiento Informado Curaciones")
                        {
                            await CrearConsentimientoAsync("ConsentimientosWEB");
                        }
                        else if (comboBox1.Text == "Consentimiento Informado Fibromialgia")
                        {
                            await CrearConsentimientoAsync("ConsentimientosWEBPSI");
                        }
                        else
                        {
                            MG = new MensajesGeneral();
                            MG.Mensaje = "Seleccion invalida";
                            MG.TipoImagen = 1000;
                            MG.ShowDialog();
                        }
                    }                    
                }                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public async System.Threading.Tasks.Task CrearConsentimientoAsync(string TipoCon)
        {
            try
            {
                ContenidoDocumento datos = new ContenidoDocumento
                {
                    NombrePaciente = textBox2.Text,
                    Documento = this.DocumentoPac,
                    Telefono = textBox4.Text,
                    Email = textBox3.Text,
                    NombreDoctor = this.NombreProfesional,
                    RegistroDoctor = this.DocumentoMed,
                    FirmaDoctorBase64 = this.FirmaMed,
                    TipoConsentimiento = comboBox1.Text,
                    Compañia = Cia.ToString(),
                    NIT = repoGeneral.Base64Encode(this.NIT)
                };

                string url = repoConf.getURLConsentimientos(TipoCon);

                (string URL, string CODE) response = await repoSign.GenerarLink(datos, url);
                
                if (response.CODE != "OK")
                {
                    textBox5.Text = "No se logro Generar la URL";
                }
                else
                {
                    textBox5.Text = response.URL + "&n=" + repoGeneral.Base64Encode(this.NIT);
                    GenerarLink();                   
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }            
        }
        async void GenerarLink()
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
                    UrlOriginal = textBox5.Text
                };

                ShortLink.SDK.ShortLinks Sl = new ShortLink.SDK.ShortLinks();
                ResponseShortLinks res = await Sl.CrearLinkCorto(Rsl);

                if (res.Estado != "OK")
                {
                    MG = new MensajesGeneral();
                    MG.Mensaje = "No se logro generar el link Email";
                    MG.TipoImagen = 1000;
                    MG.ShowDialog();
                }
                else
                {
                    EnviarSMS(res.Url);
                    EnviarEMAIL(res.Url);

                    MG = new MensajesGeneral();
                    MG.Mensaje = "Notificado, por favor que el paciente revise su correo electronico y mensajes de texto, tambien en la carpeta spam";
                    MG.TipoImagen = 3;
                    MG.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        void EnviarEMAIL(string URL)
        {
            try
            {
                string mensaje = "<center>" +
                                   "<h2 style='background:#3D38B2; color:white;'>Hola, " + textBox2.Text + "</h2>" +
                                   "<p>Este es el link del Consentimiento Informado: " + URL + "<br><br>" +                                   
                                   "<b style='color:black'>Si deseas dejar tus comentarios o sugerencias puedes hacer clic </b><b style='color:blue'>" + "<a href='https://slsoft.net:5010/PQRSF' style='color:red; font-size:12px;'>AQUI</a>" + "</b><br><br>" +
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
                    EmailTo = textBox3.Text.Trim(),
                    EmailPassword = getFirstEmail.Ema_Pass,
                    EmailBcc1 = getFirstEmail.Ema_Email,
                    EmailBcc2 = null,
                    Asunto = "CONSENTIMIENTO INFORMADO " + comboBox2.Text,
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

                RequestSMS rSMS = new RequestSMS
                {
                    CodeUser = Datos.CodeUserSDK,
                    Mensaje = "Consentimiento: " + URL,
                    NumCelular = textBox4.Text.Trim(),
                    User = Datos.UserSDK
                };

                ShortLink.SDK.EnviarSMS sms = new ShortLink.SDK.EnviarSMS();
                string resSMS = await sms.EnviarMensaje(rSMS);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
