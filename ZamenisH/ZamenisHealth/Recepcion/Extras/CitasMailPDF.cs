using Domain;
using Domain.CXN;

using FormAndControls;

using Microsoft.Reporting.WinForms;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ZamenisHealth.Clases;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.Recepcion.Extras
{
    public partial class CitasMailPDF : Forma
    {
        private static readonly IAgendaC repositorioHorario2 = new MAgendaC();
        private static readonly IPacientes repositorioPacientes = new MPacientes();
        private static readonly IDatosEmail repositorioDatosEmail = new MDatosEmail();

        private int Admision, PACID2, cia;
        private string TDOC, DOC;
        private MensajesGeneral MG;

        private void CitasMailPDF_Load(object sender, EventArgs e)
        {
            try
            {
                Titulo.Text = "Enviar Citas por Email PDF";
                LogoMain.Image = Properties.Resources.Splash;

                ToolStripButton btnSend = new ToolStripButton();
                btnSend = createToolButton("Enviar Correo");
                MenuLateral.Items.Add(btnSend);
                btnSend.Click += button1_Click;

                ConfigForm.SoloNumeros(textBox4);

                
                textBox4.MaxLength = 10;

                CXN_HORARIO _getIdPac = new CXN_HORARIO();


                _getIdPac = repositorioHorario2.getIdPacByAdmitionReportCitas(Admision);


                if (_getIdPac != null)
                {
                    CXN_PACIENTES PACID = repositorioPacientes.LlamarPacientebyId(_getIdPac.Hor_Pac_Id);
                    if (PACID == null)
                    {
                        MessageBox.Show("Este paciente presenta fallas", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        this.Dispose();
                        this.Close();
                        return;
                    }

                    PACID2 = PACID.Pac_Id;
                    TDOC = PACID.Pac_TipoId.ToString();
                    DOC = PACID.Pac_IdNum.ToString().Trim();
                    cia = _getIdPac.Hor_Pac_Cia;
                    textBox1.Text = PACID.Pac_PrimerA.ToString() + " " + PACID.Pac_SegundoA.ToString() + " " + PACID.Pac_PrimerN.ToString() + " " + PACID.Pac_SegundoN.ToString();
                    textBox2.Text = PACID.Pac_TipoId.ToString() + " " + PACID.Pac_IdNum.ToString().Trim();
                    textBox3.Text = PACID.Pac_Email.ToString();
                    textBox4.Text = PACID.Pac_Telefono.ToString();
                }
                else
                {
                    MessageBox.Show("Este paciente presenta fallas", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Dispose();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        public CitasMailPDF(int Adm)
        {
            InitializeComponent();
            this.Admision = Adm;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                var BoolEmail = repositorioPacientes.ValidaEmail(textBox3.Text);
                if (BoolEmail == false)
                {
                    MessageBox.Show("El correo del paciente es incorrecto", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                Actualiza_Email();

                List<MReportes.PrntAgendas> report = MReportes.PrntAgendas.Genera_CitasXPac_Report(DOC,
                                                              dateTimePicker1.Value.Date,
                                                              dateTimePicker2.Value.Date);

                if (report == null)
                {
                    MensajesGeneral MGs = new MensajesGeneral();
                    MGs.TipoImagen = 1000;
                    MGs.Mensaje = "No hay datos para exportar";
                    MGs.ShowDialog();
                    return;
                }

                ReportViewer R = new ReportViewer();

                R.LocalReport.DataSources.Clear();
                R.LocalReport.DataSources.Add(new ReportDataSource("DataSet_CitasGenerales", report));
                R.LocalReport.ReportEmbeddedResource = "ZamenisHealth.Reportes.RDLC_CitasXPAC.rdlc";
                R.SetDisplayMode(DisplayMode.PrintLayout);
                R.ZoomMode = ZoomMode.Percent;
                R.ZoomPercent = 100;
                R.LocalReport.EnableExternalImages = true;
                R.RefreshReport();
                R.Visible = true;
                R.Dock = System.Windows.Forms.DockStyle.Fill;
                byte[] DescargaPDF = R.LocalReport.Render("PDF");

                string mensaje = "A continuacion remitimos su listado de citas entre los dias " + dateTimePicker1.Value.Date.ToString(Conexion.ConectionDictionary["Format_Fecha"]) + " y " + dateTimePicker2.Value.Date.ToString(Conexion.ConectionDictionary["Format_Fecha"]);

                CXN_EMAIL getFirstEmail = repositorioDatosEmail.FirstEmail();
                string horarioB64 = Convert.ToBase64String(DescargaPDF);

                string html = "<center>" +
                                              "<img src='cid:imagen' />" +
                                              "<h2 style='background:#3D38B2; color:white;'>Lista de Citas Medicas</h2>" +
                                              "<p>" + mensaje + "<br><br>" +
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
                    EmailTo = textBox3.Text,
                    EmailPassword = getFirstEmail.Ema_Pass,
                    EmailBcc1 = getFirstEmail.Ema_Email,
                    EmailBcc2 = null,
                    Asunto = "Listado de Citas Medicas",
                    AttachmentFile = horarioB64,
                    BodyMessage = html,
                    TipoArchivo = "pdf"
                }, Program.URLApiConexion).GetAwaiter().GetResult();

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

        private void Actualiza_Email()
        {
            try
            {
                repositorioPacientes.Actualiza_Email(PACID2, textBox3.Text);
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
    }
}
