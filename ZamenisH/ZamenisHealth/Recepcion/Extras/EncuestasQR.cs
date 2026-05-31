using Domain;
using Domain.CXN;
using FormAndControls;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ZamenisHealth.Clases;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.Recepcion.Extras
{
    public partial class EncuestasQR : Forma2
    {
        private readonly static IConfSystem repoConf = new MConfSystem();
        private readonly static IGenerales repogen = new MGenerales();
        private readonly static IAgendaC repoagenda = new MAgendaC();
        private readonly static IDatosEmail repoDatosEmail = new MDatosEmail();
        private readonly static IPacientes repoPac = new MPacientes();
        private readonly static IEncuestasSatis repoEncu = new MEncuestasSatis();

        private MensajesGeneral MG;
        private int Admision, idPac;
        private string URLCita;

        public EncuestasQR(int admision)
        {
            InitializeComponent();
            Admision = admision;
        }

        private void EncuestasQR_Load(object sender, EventArgs e)
        {
            try
            {                
                this.Titulo.Text = "Generar Encuesta QR";
                URLCita = repoConf.getListado()["URLEncuestaQR"] + "?a=" + repogen.Base64Encode(this.Admision.ToString().Trim());                              

                pictureBox1.Image = FormAndControls.ClasesExtra.Generales.GenerateQRCode(URLCita);

                otrosDatosPacienteHorario data =  repoagenda.cargarAdmision(this.Admision, "'H','P'");                

                if (data != null) 
                {
                    textBox1.Text = data.Pac_Email.ToString().Trim();
                    idPac = data.Hor_Pac_Id;

                    string Mes = Capitalize(Convert.ToDateTime(data.Hor_Pac_Fecha_Cita).Month);
                    bool CantActualEncCU = false;

                    
                        CantActualEncCU = repoEncu.getCantEncuestaCU("CU", Mes, Convert.ToDateTime(data.Hor_Pac_Fecha_Cita).Year);
                    
                   
                    if (CantActualEncCU == true) 
                    {
                        CXN_CONFENCUESTA C = new CXN_CONFENCUESTA
                        {
                            Mes = Mes,
                            Año = Convert.ToDateTime(data.Hor_Pac_Fecha_Cita).Year,
                            Servicio = "CU"
                        };

                        bool g =  repoEncu.getCantEncuestasPaciente(idPac, C);                        

                        if (g == false)
                        {
                            MG = new MensajesGeneral();
                            MG.Mensaje = "Este paciente ya cuenta con una encuesta en este mes, debe esperar al proximo mes para realizar otra encuesta";
                            MG.TipoImagen = 1000;
                            MG.ShowDialog();

                            this.Dispose();
                            this.Close();
                        }
                    }
                    else
                    {
                        MG = new MensajesGeneral();
                        MG.Mensaje = "Ya se han completado las encuestas por este mes.  Gracias";
                        MG.TipoImagen = 3;
                        MG.ShowDialog();

                        this.Dispose();
                        this.Close();
                    }                    
                }
                else
                {
                    MG = new MensajesGeneral();
                    MG.Mensaje = "Admision en estado invalido, debe estar admisionada o hecha";
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

        string Capitalize(int Mes)
        {
            switch (Mes)
            {
                case 1:
                    return "Enero";
                case 2:
                    return "Febrero";
                case 3:
                    return "Marzo";
                case 4:
                    return "Abril";
                case 5:
                    return "Mayo";
                case 6:
                    return "Junio";
                case 7:
                    return "Julio";
                case 8:
                    return "Agosto";
                case 9:
                    return "Septiembre";
                case 10:
                    return "Octubre";
                case 11:
                    return "Noviembre";
                case 12:
                    return "Diciembre";
                default:
                    return "";
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                otrosDatosPacienteHorario s =  repoagenda.cargarAdmision(this.Admision, "'H','P'");                
                
                if (s != null) 
                {
                    List<CertificadoAsistencia> C = new List<CertificadoAsistencia>();
                    C.Add(new CertificadoAsistencia { 
                        Logo = repogen.GetBytes(pictureBox1.Image),
                        Texto_Estatico = Convert.ToDateTime(s.Hor_Pac_Fecha_Cita).ToString("yyyy-MM-dd") + " - " + s.Hor_Imp_Age.ToString()
                    });

                    ConfigForm.GenerarReportViewer("Dataset_QREncuesta",
                                                   "ZamenisHealth.Reportes.RDLC_QREncuesta.rdlc",
                                                   C);
                }
                else
                {
                    MG = new MensajesGeneral();
                    MG.Mensaje = "Admision no valida o su estado no lo permite, la admision debe estar en color Azul";
                    MG.TipoImagen = 1000;
                    MG.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                MG = new MensajesGeneral();

                //enviar por correo
                if (repoPac.ValidaEmail(textBox1.Text) == false)
                {
                    MG.Mensaje = "El formato del Email no es valido";
                    MG.TipoImagen = 1000;
                    MG.ShowDialog();
                    return;
                }

                CXN_EMAIL getFirstEmail = repoDatosEmail.FirstEmail();

               string html = "<center>" +
                                      "<img src='cid:imagen' />" +
                                      "<h2 style='background:#3D38B2; color:white;'>ENCUESTA DE SATISFACCION</h2>" +
                                      "<p><a style='background:blue; color:white; padding:3px; border: 3px solid #000; border-color:black;' href='" + URLCita.ToString() + "'>Haga clic AQUI para realizar la encuesta de satisfaccion</a><br><br>" +
                                        "Este link de encuesta de satisfaccion solo puede usarse una sola vez, posteriormente sera deshabilitada la opcion de respuesta." +
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
                    EmailTo = textBox1.Text,
                    EmailPassword = getFirstEmail.Ema_Pass,
                    EmailBcc1 = getFirstEmail.Ema_Email,
                    EmailBcc2 = null,
                    Asunto = "ENCUESTA DE SATISFACCION - ZAMENIS HEALTH",
                    AttachmentFile = null,
                    BodyMessage = html,
                    TipoArchivo = null
                }, Program.URLApiConexion).GetAwaiter().GetResult();

                repoPac.Actualiza_Email(idPac, textBox1.Text.Trim());

                MG.TipoImagen = 0;
                MG.Mensaje = enviarAPI.ToString();
                MG.ShowDialog();
                               
                this.Dispose();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
