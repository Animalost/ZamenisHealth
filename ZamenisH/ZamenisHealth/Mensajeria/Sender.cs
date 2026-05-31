using Domain.CXN;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using ShortLink.SDK.Clases;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ZamenisHealth.Clases;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.Mensajeria
{
    public partial class Sender : ConfigForm.BaseForm
    {
        private static readonly ISender repoSender = new MSender();
        private static readonly IDatosEmail repoEmail = new MDatosEmail();
        private static readonly IGenerales repoGenerales = new MGenerales();
        private static readonly IHelisa repoHelisa = new MHelisa();

        DateTime Fecha;
        string combo2;

        MensajesGeneral MG;

        public Sender()
        {
            InitializeComponent();

            btnZamenis1.ButtonClick += btnZamenis1_ButtonClick;
            btnZamenis2.ButtonClick += btnZamenis2_ButtonClick;

            btnZamenis1.captionBtn = "Enviar Email";
            btnZamenis1.tooltipBtn = "Envia correos electronicos";

            btnZamenis2.captionBtn = "Enviar SMS";
            btnZamenis2.tooltipBtn = "Envia mensajes de texto";
        }
        private async void btnZamenis1_ButtonClick(object sender, EventArgs e)
        {
            combo2 = comboBox2.Text;
            Fecha = Convert.ToDateTime(dateTimePicker1.Value.Date);

            btnZamenis1.Enabled = false;
            btnZamenis2.Enabled = false;

            Shows();
            Task oTask = new Task(Email);
            oTask.Start();
            await oTask;
            Hides();

            btnZamenis1.Enabled = true;
            btnZamenis2.Enabled = true;
        }
        private async void btnZamenis2_ButtonClick(object sender, EventArgs e)
        {
            combo2 = comboBox2.Text;
            Fecha = Convert.ToDateTime(dateTimePicker1.Value.Date);

            btnZamenis1.Enabled = false;
            btnZamenis2.Enabled = false;

            Shows();
            Task oTask = new Task(SMS);
            oTask.Start();
            await oTask;
            Hides();

            btnZamenis1.Enabled = true;
            btnZamenis2.Enabled = true;
        }

        private void Sender_Load(object sender, EventArgs e)
        {
            this.Titulo.Text = "Envio de Recordatorio de Citas";         
        }
        void Shows()
        {
            pictureBox2.Visible = true;
        }

        void Hides()
        {
            pictureBox2.Visible = false;
        }  

        async void SMS()
        {
            string Tip;

            switch (combo2)
            {
                case "Citas de Curaciones":
                    Tip = "CU";
                    break;

                case "Citas de Medicina General":
                    Tip = "MG";
                    break;

                case "Citas de Fisiatria":
                    Tip = "FI";
                    break;

                case "Citas de Piscologia":
                    Tip = "PS";
                    break;

                case "Citas de Terapia Fisica":
                    Tip = "TF";
                    break;

                case "Citas de Terapia Ocupacional":
                    Tip = "TO";
                    break;

                case "Citas de Radiologia":
                    Tip = "RA";
                    break;

                default:
                    Tip = "";
                    return;
            }           

            List<CXN_HORARIO> sendSMS = repoSender.EnviarSMS(Tip, Fecha);
            if (sendSMS != null)
            {
                var getURLS = repoEmail.FirstEmail();
                if (getURLS != null)
                {
                    MSender M = new MSender();
                    Dictionary<string, string> getCon = Conexion.Conection();

                    foreach (CXN_HORARIO i in sendSMS)
                    {
                        Dictionary<string, string> getClaves = repoHelisa.Claves("ShortLinks", i.Hor_Pac_Cia);
                        var Comprobar_Clave = repoGenerales.Base64Encode(i.Hor_Id.ToString());
                        var NitCoded = repoGenerales.Base64Encode(i.Com_Identificacion.ToString());
                        string LinkToSave = getURLS.Ema_URL_CitasM + "?a=" + Comprobar_Clave + "&Nit=" + NitCoded;

                        Dictionary<string, string> gtData = repoHelisa.Claves("ShortLinks", i.Hor_Pac_Cia);

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
                            UrlOriginal = LinkToSave
                        };

                        ShortLink.SDK.ShortLinks Sl = new ShortLink.SDK.ShortLinks();
                        ShortLink.SDK.EnviarSMS sms = new ShortLink.SDK.EnviarSMS();
                        ResponseShortLinks res = await Sl.CrearLinkCorto(Rsl);

                        if (res != null)
                        {
                            if (res.Estado == "OK")
                            {
                                string _message = repoSender.Envia_SMS(i.PacienteTelefono,
                                            i.Hor_Id.ToString(),
                                            Tip,
                                            i.Hor_Pac_Cia,
                                            getURLS.Ema_URL_CitasS,
                                            Contenedor.UsuarioLogueado,
                                            res.Url.Trim());

                                RequestSMS rSMS = new RequestSMS
                                {
                                    CodeUser = Datos.CodeUserSDK,
                                    Mensaje = _message,
                                    NumCelular = i.PacienteTelefono,
                                    User = Datos.UserSDK
                                };

                                string resSMS = await sms.EnviarMensaje(rSMS);

                                M.GrabaSQL_Evidencia("API SMS",
                                    Convert.ToInt32(i.Hor_Id),
                                    i.PacienteTelefono,
                                    resSMS.ToString(),
                                    "ASP SMS");
                            }
                            else
                            {
                                M.GrabaSQL_Evidencia("API SMS",
                                    Convert.ToInt32(i.Hor_Id),
                                    i.PacienteTelefono,
                                    res.Detalles.ToString(),
                                    "API SMS");
                            }
                        }
                        else
                        {
                            M.GrabaSQL_Evidencia("API SMS",
                                    Convert.ToInt32(i.Hor_Id),
                                    i.PacienteTelefono,
                                    "ERROR",
                                    "API SMS");
                        }
                    }

                    MG = new MensajesGeneral();
                    MG.TipoImagen = 3;
                    MG.Mensaje = "Mensajes enviados, puede consultar el log de envios para ver el estado de cada mensaje";
                    MG.ShowDialog();
                }
                else
                {
                    MG = new MensajesGeneral();
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "No hay links disponibles en el servidor de correo seleccionado";
                    MG.ShowDialog();
                }
            }
            else
            {
                MG = new MensajesGeneral();
                MG.TipoImagen = 1000;
                MG.Mensaje = "No hay citas disponibles para el dia seleccionado";
                MG.ShowDialog();
            }            
        }

        private async void Email()
        {
            string Tip;
            switch (combo2)
            {
                case "Citas de Curaciones":
                    Tip = "CU";
                    break;

                case "Citas de Medicina General":
                    Tip = "MG";
                    break;

                case "Citas de Fisiatria":
                    Tip = "FI";
                    break;

                case "Citas de Piscologia":
                    Tip = "PS";
                    break;

                case "Citas de Terapia Fisica":
                    Tip = "TF";
                    break;

                case "Citas de Terapia Ocupacional":
                    Tip = "TO";
                    break;

                case "Citas de Radiologia":
                    Tip = "RA";
                    break;

                default:
                    Tip = "";
                    return;
            }

            var sendEmail = repoSender.EnviarEMAIL(Tip, Fecha);
            if (sendEmail != null)
            {
                var getFirstEmail = repoEmail.FirstEmail();
                if (getFirstEmail != null)
                {
                    MSender M = new MSender();                    

                    foreach (var i in sendEmail)
                    {
                        Dictionary<string, string> getClaves = repoHelisa.Claves("ShortLinks", i.Hor_Pac_Cia);
                        var Comprobar_Clave = repoGenerales.Base64Encode(i.Hor_Id.ToString());
                        var NitCoded = repoGenerales.Base64Encode(i.Com_Identificacion.ToString());
                        string LinkToSave = getFirstEmail.Ema_URL_CitasM + "?a=" + Comprobar_Clave + "&Nit=" + NitCoded;

                        Dictionary<string, string> gtData = repoHelisa.Claves("ShortLinks", i.Hor_Pac_Cia);

                        DatosAPI Datos = new DatosAPI
                        {
                            CodeUserSDK = gtData["Code"],
                            PassSDK = gtData["Pass"],
                            UserSDK = gtData["User"]
                        };

                        //GenerarShortLink
                        ShortLink.SDK.Clases.RequestShortLink Rsl = new ShortLink.SDK.Clases.RequestShortLink
                        {
                            CodeUser = Datos.CodeUserSDK,
                            User = Datos.UserSDK,
                            Pass = Datos.PassSDK,
                            UrlOriginal = LinkToSave
                        };

                        ShortLink.SDK.ShortLinks Sl = new ShortLink.SDK.ShortLinks();
                        ShortLink.SDK.EnviarSMS sms = new ShortLink.SDK.EnviarSMS();
                        ResponseShortLinks res = await Sl.CrearLinkCorto(Rsl);

                        if (res != null)
                        {
                            if (res.Estado == "OK")
                            {
                                var e = repoSender.Envia_Mail(Tip,
                                              i.Hor_Id,
                                              i.Com_Email,
                                              i.Hor_Pac_Cia,
                                              getFirstEmail.Ema_URL_CitasM,
                                              getFirstEmail.Ema_Baja_Email,
                                              i.Hor_Pac_Fecha_Cita,
                                              i.Hor_Pac_Hora_Cita,
                                              i.Hor_Imp_Age,
                                              i.Com_Direccion,
                                              i.Com_Telefono,
                                              res.Url.ToString());

                                if (e.TextoCuerpo != null || e.GoogleCalendar != null)
                                {
                                    string calendarB64 = Convert.ToBase64String(e.GoogleCalendar);

                                    APIController.Services.Email.EmailService emailService = new APIController.Services.Email.EmailService();
                                    string enviarAPI = emailService.SendEmailAPI(new APIController.Clases.EmailRequest
                                    {
                                        EmailFrom = getFirstEmail.Ema_Email,
                                        EmailTo = i.Com_Email,
                                        EmailPassword = getFirstEmail.Ema_Pass,
                                        EmailBcc1 = getFirstEmail.Ema_Email,
                                        EmailBcc2 = null,
                                        Asunto = "RECORDATORIO DE CITAS",
                                        AttachmentFile = calendarB64,
                                        BodyMessage = e.TextoCuerpo,
                                        TipoArchivo = "ics"
                                    }, Program.URLApiConexion).GetAwaiter().GetResult();

                                    M.GrabaSQL_Evidencia("API EMAIL",
                                             Convert.ToInt32(i.Hor_Id),
                                             i.Com_Email,
                                             enviarAPI.ToString(),
                                             "ASP EMAIL");
                                }
                            }
                            else
                            {
                                M.GrabaSQL_Evidencia("API EMAIL",
                                    Convert.ToInt32(i.Hor_Id),
                                    i.Com_Email,
                                    res.Detalles,
                                    "API EMAIL");
                            }
                        }
                        else
                        {
                            M.GrabaSQL_Evidencia("API EMAIL",
                                    Convert.ToInt32(i.Hor_Id),
                                    i.Com_Email,
                                    "ERROR",
                                    "API EMAIL");
                        }                                             
                    }

                    Hides();

                    MG = new MensajesGeneral();
                    MG.TipoImagen = 3;
                    MG.Mensaje = "Mensajes enviados, puede consultar el log de envios para ver el estado de cada mensaje";
                    MG.ShowDialog();
                }
                else
                {
                    Hides();
                    MG = new MensajesGeneral();
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "No hay links disponibles en el servidor de correo seleccionado";
                    MG.ShowDialog();
                }
            }
            else
            {
                Hides();
                MG = new MensajesGeneral();
                MG.TipoImagen = 1000;
                MG.Mensaje = "No hay citas disponibles para el dia seleccionado";
                MG.ShowDialog();
            }         
        }
    }
}
