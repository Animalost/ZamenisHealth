using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Domain.CXN;
using Domain;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using ZamenisHealth.Comunes;
using System.Threading;
using Persistence;
using System.Threading.Tasks;
using ZamenisHealth.Clases;
using Microsoft.Reporting.WinForms;
using FormAndControls;

namespace ZamenisHealth.Medicina
{
    public partial class OrdenesMedicasT : Forma2
    {
        private static readonly IOrdenes repositorioOrdenes = new MOrdenes();
        private static readonly IPacientes repositorioPacientes = new MPacientes();
        private static readonly IDatosEmail repositorioDatosEmail = new MDatosEmail();
        private static readonly ILogSender repositorioLogSender = new MLogSender();

        private int Cia, Orden;
        private string Usuario, Namereport, Namedatasource;
        private int IdPAc = 0;
        Thread thread;
        List<Ordenes> Exportar;
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                switch (comboBox1.Text)
                {
                    case "Formato 1":
                        label2.Text = "Impresion a media hoja";
                        pictureBox1.Visible = true;
                        pictureBox2.Visible = false;
                        break;

                    case "Formato 2":
                        label2.Text = "Impresion a hoja completa";
                        pictureBox2.Visible = true;
                        pictureBox1.Visible = false;
                        break;

                    default:
                        label2.Text = "";
                        pictureBox2.Visible = false;
                        pictureBox1.Visible = false;
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                Comunes.MensajesGeneral MG = new Comunes.MensajesGeneral();
                Exportar = new List<Ordenes>();

                if (comboBox1.Text == "")
                {
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "Seleccione un reporte valido";
                    MG.ShowDialog();
                    return;
                }
           
                Exportar = repositorioOrdenes.Generar_OrdenMedica(this.Orden, this.Cia, this.Usuario);
                if (Exportar == null)
                {
                    MessageBox.Show("La orden medica se genero pero no se logro exportar, ingrese por la opcion " +
                        "de busqueda de ordenes medicas",
                        "Inconsistencia",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation);
                    this.Dispose();
                    this.Close();
                    return;
                }
                else
                {
                    if (comboBox1.Text == "Formato 1")
                    {                       
                        Namereport = "ZamenisHealth.Reportes.RDLC_OrdenesServicios2.rdlc";
                        Namedatasource = "DataSet_Ordenes";
                        thread = new Thread(M);
                        thread.SetApartmentState(ApartmentState.STA); // Configura el subproceso en STA
                        thread.Start();
                    }
                    else if (comboBox1.Text == "Formato 2")
                    {
                        Exportar = repositorioOrdenes.Generar_OrdenMedica(this.Orden, this.Cia, this.Usuario);
                        if (Exportar == null)
                        {
                            MessageBox.Show("La orden medica se genero pero no se logro exportar, ingrese por la opcion " +
                                "de busqueda de ordenes medicas",
                                "Inconsistencia",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Exclamation);
                            this.Dispose();
                            this.Close();
                            return;
                        }                        

                        Namereport = "ZamenisHealth.Reportes.RDLC_OrdenesServicios.rdlc";
                        Namedatasource = "Dataset_OM";
                        thread = new Thread(M);
                        thread.SetApartmentState(ApartmentState.STA); // Configura el subproceso en STA
                        thread.Start();
                    }
                    else
                    {
                        MG.TipoImagen = 1000;
                        MG.Mensaje = "Seleccione un tipo de formato";
                        MG.ShowDialog();
                        return;
                    }

                    this.Dispose();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        void EnviarEmail()
        {
            try
            {
                MensajesGeneral MG = new MensajesGeneral();

                Exportar = repositorioOrdenes.Generar_OrdenMedica(this.Orden, this.Cia, this.Usuario);
                if (Exportar == null)
                {
                    MessageBox.Show("La orden medica se genero pero no se logro exportar, ingrese por la opcion " +
                        "de busqueda de ordenes medicas",
                        "Inconsistencia",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation);
                    this.Dispose();
                    this.Close();
                    return;
                }

                if (repositorioPacientes.ValidaEmail(textBox1.Text) == true)
                {
                    repositorioPacientes.Actualiza_Email(this.IdPAc, textBox1.Text);

                    bool not = Notificar(Exportar, "Formato2");
                    if (not == false)
                    {
                        return;
                    }
                }
                else
                {
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "El formato del email no es valido";
                    MG.ShowDialog();
                    return;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        void M()
        {
            try
            {
                ConfigForm.GenerarReportViewer(this.Namedatasource,
              this.Namereport,
              Exportar);
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void OrdenesMedicasT_Load(object sender, EventArgs e)
        {
            try
            {
                Titulo.Text = "Oden Medica";

                var getIdPac = repositorioOrdenes.Generar_OrdenMedica(this.Orden, this.Cia, Contenedor.UsuarioLogueado);
                if (getIdPac != null)
                {
                    IdPAc = 0;

                    foreach (var i in getIdPac)
                    {
                        IdPAc = Convert.ToInt32(i.OM_Cantidad);
                        break;
                    }

                    var getMail = repositorioPacientes.LlamarPacientebyId(IdPAc);
                    if (getMail != null)
                    {
                        textBox1.Text = getMail.Pac_Email.ToString();
                    }
                    else
                    {
                        textBox1.Text = "";
                    }
                }
                else
                {
                    textBox1.Text = "";
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            pictureBox3.Visible = true;
            Task oTask = new Task(EnviarEmail);
            oTask.Start();
            await oTask;
            pictureBox3.Visible = false;            
        }

        private void OrdenesMedicasT_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Escape)
            {
                this.Dispose();
                this.Close();
            }
        }

        public OrdenesMedicasT(int _cia, int _orden, string _usuario)
        {
            InitializeComponent();
            this.Usuario = _usuario;
            this.Orden = _orden;
            this.Cia = _cia;
            
        }
        bool Notificar(List<Ordenes> O, string TipoOrden)
        {
            try
            {
                MensajesGeneral MG = new MensajesGeneral();

                bool verificaMail = repositorioPacientes.ValidaEmail(textBox1.Text);
                if (verificaMail == true)
                {
                    //Formato 2
                    string TO = "ZamenisHealth.Reportes.RDLC_OrdenesServicios.rdlc";
                    string DS = "Dataset_OM";

                    if (TipoOrden == "Formato1")
                    {
                        TO = "ZamenisHealth.Reportes.RDLC_OrdenesServicios2.rdlc";
                        DS = "DataSet_Ordenes";
                    }

                    DateTime H = DateTime.Now.Date;

                    CXN_LOG_SENDER L = new CXN_LOG_SENDER
                    {
                        Log_Fecha_Envio = H,
                        Log_Estado = "Send",
                        Log_Usuario = Contenedor.UsuarioLogueado,
                        Log_Mensaje = "Envio de orden medica",
                        Log_Destinatario = textBox1.Text,
                        Log_Admision = this.Orden,
                        Log_Tipo = "EMAIL"
                    };

                    repositorioLogSender.GrabaSQL_Evidencia(L);

                    ReportViewer R = new ReportViewer();
                    R.LocalReport.DataSources.Clear();
                    R.LocalReport.DataSources.Add(new ReportDataSource(DS.ToString(), O));
                    R.LocalReport.ReportEmbeddedResource = TO.ToString();
                    R.SetDisplayMode(DisplayMode.PrintLayout);
                    R.ZoomMode = ZoomMode.Percent;
                    R.ZoomPercent = 100;
                    R.LocalReport.EnableExternalImages = true;
                    R.RefreshReport();
                    //maestro.Visible = true;
                    R.Dock = System.Windows.Forms.DockStyle.Fill;

                    byte[] bytes = R.LocalReport.Render("PDF");
                    /*FileStream fss = new FileStream("C:\\CXN\\Reportes\\" + this.Orden.ToString() + ".pdf", FileMode.Create);
                    fss.Write(bytes, 0, bytes.Length);
                    fss.Close();*/

                    string OrdenB64 = Convert.ToBase64String(bytes);
                    CXN_EMAIL getFirstEmail = repositorioDatosEmail.FirstEmail();

                    string html = "<center>" +
                                      "<img src='cid:imagen' />" +
                                      "<h2 style='background:#3D38B2; color:white;'>Hola, Enviamos tu orden medica instantanea</h2>" +
                                      "<p>Te enviamos a continuacion la orden medica generada en tu atencion medica: <br><br>" +
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
                                      "</p><br><hr>Numero de orden medica: <b>" + Orden.ToString() + "</b><hr>" +
                                  "</center>";

                    APIController.Services.Email.EmailService emailService = new APIController.Services.Email.EmailService();
                    string enviarAPI = emailService.SendEmailAPI(new APIController.Clases.EmailRequest
                    {
                        EmailFrom = getFirstEmail.Ema_Email,
                        EmailTo = textBox1.Text,
                        EmailPassword = getFirstEmail.Ema_Pass,
                        EmailBcc1 = getFirstEmail.Ema_Email,
                        EmailBcc2 = null,
                        Asunto = "ORDENES MEDICAS GENERADAS",
                        AttachmentFile = OrdenB64,
                        BodyMessage = html,
                        TipoArchivo = "pdf"
                    }, Program.URLApiConexion).GetAwaiter().GetResult();

                    MG.TipoImagen = 0;
                    MG.Mensaje = enviarAPI.ToString();
                    MG.ShowDialog();

                    return true;
                }
                else
                {
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "El formato del Email es invalido";
                    MG.ShowDialog();
                    return false;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);

                Comunes.MensajesGeneral MG = new MensajesGeneral();
                MG.TipoImagen = 1000;
                MG.Mensaje = "No se logro enviar la orden medica al correo del paciente, puede que este saturado el servidor o halla inconvenientes de red";
                MG.ShowDialog();

                return false;
            }
        }
    }
}
