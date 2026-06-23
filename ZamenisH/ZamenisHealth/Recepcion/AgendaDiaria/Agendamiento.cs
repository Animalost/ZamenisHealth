using Domain;
using Domain.CXN;
using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using ShortLink.SDK.Clases;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using ZamenisHealth.Clases;
using ZamenisHealth.Comunes;
using ZamenisHealth.Recepcion.Extras;

namespace ZamenisHealth.Recepcion.AgendaDiaria
{
    public partial class Agendamiento : Forma
    {
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, bool wParam, int lParam);
        private const int WM_SETREDRAW = 0x000B;

        private IBodegas repositorioBodegas;
        private ICompañia repositorioCompañias;
        private IDisponibilidad repositorioDisponibilidad;
        private IAgendaC repositorioAgenda;
        private IAgenda repositorioAgenda2;
        private IConfSystem repositorioConfsystem;
        private IPacientes repositorioPacientes;
        private IDatosEmail repositorioEmail;
        private IHelisa repositorioHelisa;
        private ILogSender repositorioLogSender;

        private int CodeProfesional, CodePrestador, TipoConsultaBarras;
        private bool Amplia, Colorimetria;
        private Image ImaEmail, ImaSMS, ImaBlocked;

        private MensajesGeneral MG;
        DataTable dt;
        DataColumn IdHora;
        DataColumn EstadoHora;
        DataColumn EstadoCita;
        DataColumn CodePaciente;

        DataColumn Hora;
        DataColumn Admisiona;
        DataColumn Paciente;
        DataColumn Sesiones;
        DataColumn SendEmail;
        DataColumn SendSMS;
        DataColumn Aseguradora;
        DataColumn ColorFila;
        DataColumn BlockEspaces;
        DataColumn P2VxS;

        public Agendamiento()
        {
            InitializeComponent();
            repositorioBodegas = new MBodegas();
            repositorioCompañias = new MCompañia();
            repositorioDisponibilidad = new MDisponibilidad();
            repositorioAgenda = new MAgendaC();
            repositorioConfsystem = new MConfSystem();
            repositorioAgenda2 = new MAgenda();
            repositorioPacientes = new MPacientes();
            repositorioEmail = new MDatosEmail();
            repositorioHelisa = new MHelisa();
            repositorioLogSender = new MLogSender();
        }
        internal class ListaProfs
        {
            public string Funcionario { get; set; }
            public int Bodega { get; set; }
            public string Usuario { get; set; }
        }
        void CargarImagenes()
        {
            var getData = repositorioConfsystem.getListado();
            if (getData != null)
            {
                ImaEmail = Base64ToImage(Convert.FromBase64String(getData["Email"]));
                ImaSMS = Base64ToImage(Convert.FromBase64String(getData["SMS"]));
                ImaBlocked = Base64ToImage(Convert.FromBase64String(getData["Bloqueo"]));
            }        
        }

        #region BOTONES
        void CargarBotones()
        {
            try
            {
                ToolStripButton btnAsistencia = new ToolStripButton();
                btnAsistencia = createToolButton("Ver Asistencia");
                MenuLateral.Items.Add(btnAsistencia);
                btnAsistencia.Click += btnAsistencia_Click;

                ToolStripButton btnOrdenes = new ToolStripButton();
                btnOrdenes = createToolButton("Ver Ordenes");
                MenuLateral.Items.Add(btnOrdenes);
                btnOrdenes.Click += btnOrdenes_Click;

                ToolStripButton btnPrintAgenda = new ToolStripButton();
                btnPrintAgenda = createToolButton("Imprimir Agendas");
                MenuLateral.Items.Add(btnPrintAgenda);
                btnPrintAgenda.Click += btnPrintAgenda_Click;

                ToolStripButton btnVentas = new ToolStripButton();
                btnVentas = createToolButton("Ventas");
                MenuLateral.Items.Add(btnVentas);
                btnVentas.Click += btnVentas_Click;

                ToolStripButton btnMensajero = new ToolStripButton();
                btnMensajero = createToolButton("Mensajero");
                MenuLateral.Items.Add(btnMensajero);
                btnMensajero.Click += btnMensajero_Click;

                ToolStripButton btnBloqueos = new ToolStripButton();
                btnBloqueos = createToolButton("Bloquear Dias");
                MenuLateral.Items.Add(btnBloqueos);
                btnBloqueos.Click += btnBloqueos_Click;

                ToolStripButton btnCanceladas = new ToolStripButton();
                btnCanceladas = createToolButton("Citas Canceladas");
                MenuLateral.Items.Add(btnCanceladas);
                btnCanceladas.Click += btnCanceladas_Click;

                ToolStripButton btnAutPendiente = new ToolStripButton();
                btnAutPendiente = createToolButton("Aut. Pendientes");
                MenuLateral.Items.Add(btnAutPendiente);
                btnAutPendiente.Click += btnAutPendiente_Click;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        void btnAsistencia_Click(object sender, EventArgs e)
        {
            Asistencia f = new Asistencia
            {
                StartPosition = FormStartPosition.CenterScreen,
                FormBorderStyle = FormBorderStyle.FixedSingle,
                AutoScroll = false
            };
            f.ShowDialog();
        }
        void btnOrdenes_Click(object sender, EventArgs e)
        {
            Extras.O_GeneratedP O = new Extras.O_GeneratedP(0);
            O.listView1.Size = new Size(956, 344);
            O.listView1.Location = new Point(12, 116);
            O.ShowDialog();
        }
        void btnPrintAgenda_Click(object sender, EventArgs e)
        {
            Extras.PrintAgendas printAgendas = new Extras.PrintAgendas();
            printAgendas.ShowDialog();
        }
        void btnVentas_Click(object sender, EventArgs e)
        {
            Ventas.BuscarPaciente P = new Ventas.BuscarPaciente();
            P.ShowDialog();
        }
        void btnMensajero_Click(object sender, EventArgs e)
        {
            Comunes.MensajeroSend M = new Comunes.MensajeroSend();
            M.ShowDialog();
        }
        void btnBloqueos_Click(object sender, EventArgs e)
        {
            Extras.BloqueoDias bloqueoDias = new Extras.BloqueoDias();
            bloqueoDias.ShowDialog();
        }
        void btnCanceladas_Click(object sender, EventArgs e)
        {
            List<CXN_HORARIO> getCancelWEB = new List<CXN_HORARIO>();
            getCancelWEB = repositorioAgenda.consultaCancelaWEB(Convert.ToDateTime(calendarHQ1.dTPCalendar.Value.Date));
            if (getCancelWEB != null)
            {
                CancelacionesWEB c = new CancelacionesWEB(getCancelWEB);
                c.ShowDialog();
            }
            else
            {
                MG = new MensajesGeneral()
                {
                    Mensaje = "No hay citas canceladas por el paciente via WEB el dia seleccionado de la agenda",
                    TipoImagen = 0
                };
                MG.ShowDialog();
            }           
        }
        void btnAutPendiente_Click(object sender, EventArgs e)
        {
            Extras.OrdenesPendientes P = new Extras.OrdenesPendientes();
            P.ShowDialog();
        }
        #endregion

        private void Agendamiento_Load(object sender, EventArgs e)
        {
            try
            {   CargarImagenes();
                CargarBotones();

                Colorimetria = (Preferencias.Colorimetria == "A" ? true : false);
                if (this.Colorimetria == false)
                {
                    label21.Visible = false;
                    label7.Visible = false;
                }

                calendarHQ1.Cargar();

                typeof(DataGridView).InvokeMember("DoubleBuffered",
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty,
                null, dataGridView1, new object[] { true });

                Titulo.Text = "Agenda Medica";
                LogoMain.Image = Properties.Resources.Splash;
                SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";

                CargarProfesionales();
                CargarPrestadores();                

                calendarHQ1.dTPCalendar.Value = new DateTime(DateTime.Now.Date.Year, DateTime.Now.Date.Month, DateTime.Now.Date.Day);                

                dataGridView1.CellClick += DataGridView1_CellClick;
                calendarHQ1.dTPCalendar.ValueChanged += dTPCalendar_ValueChanged;

                comboBox3.SelectedIndex = 0;
                //EventoInicial();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }            
        }
        void CargarPrestadores()
        {
            comboBox2.DataSource = null;
            comboBox2.Items.Clear();

            List<CXN_CIA> Lista = repositorioCompañias.getAllCompañias();
            if (Lista != null)
            {
                foreach (var i in Lista)
                {
                    comboBox2.Items.Add(i.Com_Nombre.ToString());
                }

                comboBox2.SelectedIndex = 0;
            }
        }
        public void CargarProfesionales()
        {
            try
            {
                string tip = "";

                if (comboBox3.Text != "Todos")
                {
                    switch (comboBox3.Text)
                    {
                        case "Medicos Generales":
                            tip = "MG";
                            break;
                        case "Enfermeros":
                            tip = "CU";
                            break;
                        case "Fisiatras":
                            tip = "FI";
                            break;
                        case "Teraputas Fisicos":
                            tip = "TF";
                            break;
                        case "Teraputas Ocupacionales":
                            tip = "TO";
                            break;
                        case "Psicologos":
                            tip = "PS";
                            break;
                        case "Radiologos":
                            tip = "RA";
                            break;
                        default:
                            tip = "Todos";
                            break;
                    }
                }
                else
                {
                    tip = "Todos";
                }

                comboBox1.DataSource = null;
                comboBox1.Items.Clear();
                
                List<CXN_BODEGAS> profesionales = repositorioBodegas.GetAllProfesionales(tip);
                if (profesionales != null)
                {
                    List<ListaProfs> L = new List<ListaProfs>();

                    foreach (var i in profesionales)
                    {
                        L.Add(new ListaProfs
                        {
                            Bodega = i.Bod_Numero,
                            Funcionario = i.Bod_Responsable,
                            Usuario = i.Bod_Usuario
                        });
                    }

                    comboBox1.DataSource = L;
                    comboBox1.DisplayMember = "Funcionario";
                    comboBox1.ValueMember = "Usuario";

                    comboBox1.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }        
        private void boton1_Click(object sender, EventArgs e)
        {
            if (dataGridView1.DataSource != null)
            {
                if (calendarHQ1.Visible == true)
                {
                    Amplia = true;

                    label3.Visible = false;
                    label4.Visible = false;
                    calendarHQ1.Visible = false;
                    dataGridView1.Size = new Size(1020, 596);
                }
                else
                {
                    Amplia = false;

                    label3.Visible = true;
                    label4.Visible = true;
                    calendarHQ1.Visible = true;
                    dataGridView1.Size = new Size(620, 596);
                }

                if (Amplia == true)
                {
                    dataGridView1.Columns["Paciente"].Width = 368;
                    dataGridView1.Columns["Aseguradora"].Width = 368;
                }
                else
                {
                    dataGridView1.Columns["Paciente"].Width = 325;
                    dataGridView1.Columns["Aseguradora"].Width = 325;
                }
            }            
        }
        private void DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                dataGridView1.ClearSelection();

                if (e.RowIndex >= 0)
                {
                    if (e.ColumnIndex == 6)
                    {
                        if (string.IsNullOrEmpty(dataGridView1.CurrentRow.Cells[6].Value.ToString()))
                        {
                            //AGENDAR CITA
                            (int CodProf, string TipoBod) DatoProf = repositorioBodegas.ProfesionalId(comboBox1.Text);
                            CXN_CIA IdCia = repositorioCompañias.getPrestadorbyName(comboBox2.Text);

                            string IDE = dataGridView1.CurrentRow.Cells[0].Value.ToString();
                            string estadohora = dataGridView1.CurrentRow.Cells[1].Value.ToString();

                            AgendarCita Agenda_Cita = new AgendarCita(DatoProf.CodProf, //Profesional
                                                                  calendarHQ1.txtDia.Text, //Nombre del dia
                                                                  Convert.ToDateTime(dataGridView1.Rows[e.RowIndex].Cells[4].Value.ToString()), //Hora Hora
                                                                  DatoProf.TipoBod, // Tipo bodega
                                                                  IDE.ToString(), //Ide Hora
                                                                  IdCia.Com_Identificador, //Prestador
                                                                  Convert.ToDateTime(calendarHQ1.txtFecha.Text), //Fecha
                                                                  estadohora); //Verde o Rojo Horario

                            Agenda_Cita.ShowDialog();
                        }
                        else
                        {
                            //cargar forma si no esta vacio el espacio (datos de cita, cancelar, anular, etc)

                            //string estadoCita = dataGridView1.CurrentRow.Cells[2].Value.ToString();
                            string admision = dataGridView1.CurrentRow.Cells[5].Value.ToString();
                            string estadoCita = repositorioAgenda2.consularAdmisionEstado(Convert.ToInt32(admision));

                            if (estadoCita != "0")
                            {
                                //Admisionar
                                if (estadoCita == "A")
                                {
                                    //string admision = dataGridView1.CurrentRow.Cells[5].Value.ToString();
                                    string idHora = dataGridView1.CurrentRow.Cells[0].Value.ToString();
                                    string Hora = dataGridView1.CurrentRow.Cells[4].Value.ToString();

                                    MenuOpcionesAgenda menuOpcionesAgenda = new MenuOpcionesAgenda(admision, idHora, CodePrestador, Hora, CodeProfesional);
                                    menuOpcionesAgenda.ShowDialog();
                                }
                                else if (estadoCita == "B") //Bloqueos
                                {
                                    DialogResult result = MessageBox.Show("¿Desea desbloquear este espacio?", "Zamenis Health - Agendas", MessageBoxButtons.YesNo);

                                    if (result == DialogResult.Yes)
                                    {
                                        repositorioAgenda2.desbloquearEspacio(Comunes.Contenedor.UsuarioLogueado, Convert.ToInt32(admision));
                                        EventoInicial();
                                        //CargarProfesionales();
                                    }
                                }
                                else if (estadoCita == "P") //Datos Cita y Anular Admision
                                {
                                    DatosCita f = new DatosCita(Convert.ToInt32(admision), "Agenda");
                                    f.ShowDialog();
                                }
                                else if (estadoCita == "H") //Datos Cita y Anular Admision
                                {
                                    DatosCita f = new DatosCita(Convert.ToInt32(admision), "Agenda");
                                    f.ShowDialog();
                                }
                                else //Solo datos cita
                                {
                                    DatosCita f = new DatosCita(Convert.ToInt32(admision), "Agenda");
                                    f.ShowDialog();
                                }
                            }
                            else
                            {
                                //AGENDAR CITA
                                (int CodProf, string TipoBod) DatoProf = repositorioBodegas.ProfesionalId(comboBox1.Text);
                                CXN_CIA IdCia = repositorioCompañias.getPrestadorbyName(comboBox2.Text);

                                string IDE = dataGridView1.CurrentRow.Cells[0].Value.ToString();
                                string estadohora = dataGridView1.CurrentRow.Cells[1].Value.ToString();

                                AgendarCita Agenda_Cita = new AgendarCita(DatoProf.CodProf, //Profesional
                                                                      calendarHQ1.txtDia.Text, //Nombre del dia
                                                                      Convert.ToDateTime(dataGridView1.Rows[e.RowIndex].Cells[4].Value.ToString()), //Hora Hora
                                                                      DatoProf.TipoBod, // Tipo bodega
                                                                      IDE.ToString(), //Ide Hora
                                                                      IdCia.Com_Identificador, //Prestador
                                                                      Convert.ToDateTime(calendarHQ1.txtFecha.Text), //Fecha
                                                                      estadohora); //Verde o Rojo Horario

                                Agenda_Cita.ShowDialog();
                            }
                        }
                    }
                    else if (e.ColumnIndex == 8) //EMAIL
                    {
                        if (dataGridView1.Rows[e.RowIndex].Cells[5].Value.ToString() != "") //Admision
                        {
                            EnviarEmail(Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[5].Value.ToString()));
                        }
                        else
                        {
                            MG = new MensajesGeneral()
                            {
                                Mensaje = "No es posible enviar un recordatorio de citas ya que no hay paciente agendado aqui",
                                TipoImagen = 1000
                            };
                            MG.ShowDialog();
                        }
                    }
                    else if (e.ColumnIndex == 9) //SMS
                    {
                        if (dataGridView1.Rows[e.RowIndex].Cells[5].Value.ToString() != "") //Admision
                        {
                            EnviarSMS(Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[5].Value.ToString()), "SMSAgenda"); //Admision
                        }
                        else
                        {
                            MG = new MensajesGeneral()
                            {
                                Mensaje = "No es posible enviar un recordatorio de citas ya que no hay paciente agendado aqui",
                                TipoImagen = 1000
                            };
                            MG.ShowDialog();
                        }
                    }
                    else if (e.ColumnIndex == 5) //Menu Principal
                    {                        
                        string admision = dataGridView1.CurrentRow.Cells[5].Value.ToString();
                        string idHora = dataGridView1.CurrentRow.Cells[0].Value.ToString();
                        string Hora = dataGridView1.CurrentRow.Cells[4].Value.ToString();

                        MenuOpcionesAgenda menuOpcionesAgenda = new MenuOpcionesAgenda(admision, idHora, CodePrestador, Hora, CodeProfesional);
                        menuOpcionesAgenda.ShowDialog();
                    }
                }
                else //agendar cita
                {
                    //AGENDAR CITA
                    (int CodProf, string TipoBod) DatoProf = repositorioBodegas.ProfesionalId(comboBox1.Text);
                    CXN_CIA IdCia = repositorioCompañias.getPrestadorbyName(comboBox2.Text);

                    string IDE = dataGridView1.CurrentRow.Cells[0].Value.ToString();
                    string estadohora = dataGridView1.CurrentRow.Cells[1].Value.ToString();

                    AgendarCita Agenda_Cita = new AgendarCita(DatoProf.CodProf, //Profesional
                                                          calendarHQ1.txtDia.Text, //Nombre del dia
                                                          Convert.ToDateTime(dataGridView1.Rows[e.RowIndex].Cells[4].Value.ToString()), //Hora Hora
                                                          DatoProf.TipoBod, // Tipo bodega
                                                          IDE.ToString(), //Ide Hora
                                                          IdCia.Com_Identificador, //Prestador
                                                          Convert.ToDateTime(calendarHQ1.txtFecha.Text), //Fecha
                                                          estadohora); //Verde o Rojo Horario

                    Agenda_Cita.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        async void EnviarSMS(int Admision, string Tipo)
        {
            try
            {
                CXN_HORARIO _datosCitaSMS = repositorioAgenda2.DatosforMailSMS(Admision);

                if (_datosCitaSMS != null)
                {
                    if (_datosCitaSMS.Hor_Estado != "A")
                    {
                        MG = new MensajesGeneral()
                        {
                            Mensaje = "Para enviar un recordatorio de citas el estado de la cita no puede estar consumido, " +
                            "debe estar en color negro el estado de la cita",
                            TipoImagen = 1000
                        };
                        MG.ShowDialog();
                    }
                    else
                    {
                        DateTime Hoy = DateTime.Now.Date;

                        string Celular = _datosCitaSMS.Hor_RegAtn;
                        DateTime Fecha = Convert.ToDateTime(_datosCitaSMS.Hor_Pac_Fecha_Cita);
                        DateTime Hora = Convert.ToDateTime(_datosCitaSMS.Hor_Pac_Hora_Cita);

                        if (Convert.ToDateTime(Fecha.ToString(Conexion.ConectionDictionary["Format_Fecha"])) < Convert.ToDateTime(Hoy.ToString(Conexion.ConectionDictionary["Format_Fecha"])))
                        {
                            MG = new MensajesGeneral()
                            {
                                Mensaje = "Para enviar un recordatorio de citas el estado de la cita no puede estar consumido, " +
                                "debe estar en color negro el estado de la cita",
                                TipoImagen = 1000
                            };
                            MG.ShowDialog();
                        }
                        else
                        {
                            bool ValidaCel = repositorioPacientes.ValidaCelular(Celular);
                            if (ValidaCel != true)
                            {
                                MG = new MensajesGeneral()
                                {
                                    Mensaje = "El numero de celular del paciente contiene caracteres NO numericos, " +
                                    "actualize el numero y vuelva a intentar -> " + Celular,
                                    TipoImagen = 1000
                                };
                                MG.ShowDialog();

                                ActualizarCel(Convert.ToInt32(_datosCitaSMS.Hor_Pac_Id));
                            }
                            else
                            {
                                string Mensaje_SMS = "Cita agendada dia " + Convert.ToDateTime(Fecha).ToString(Conexion.ConectionDictionary["Format_Fecha"]) +
                                             " hora " + Convert.ToDateTime(Hora).ToString("H:mm tt") + " en " + _datosCitaSMS.Com_Nombre_SMS + " Tel " + _datosCitaSMS.Com_Telefono_SMS + " Dir " + _datosCitaSMS.Com_Direccion +
                                             " favor asista 20 minutos antes";

                                if (Tipo == "SMSAgenda")
                                {
                                    Dictionary<string, string> gtData = repositorioHelisa.Claves("ShortLinks", _datosCitaSMS.Hor_Pac_Cia);

                                    DatosAPI Datos = new DatosAPI
                                    {
                                        CodeUserSDK = gtData["Code"],
                                        PassSDK = gtData["Pass"],
                                        UserSDK = gtData["User"]
                                    };

                                    ShortLink.SDK.Clases.RequestSMS s = new RequestSMS()
                                    {
                                        CodeUser = Datos.CodeUserSDK,
                                        Mensaje = Mensaje_SMS,
                                        NumCelular = Celular,
                                        User = Datos.UserSDK
                                    };

                                    ShortLink.SDK.EnviarSMS sms = new ShortLink.SDK.EnviarSMS();
                                    var res = await sms.EnviarMensaje(s);

                                    Log(Admision, res, Mensaje_SMS, Celular);

                                    MG = new MensajesGeneral()
                                    {
                                        Mensaje = "Su mensaje de texto ha sido enviado al numero de celular: " + Celular + " exitosamente",
                                        TipoImagen = 2
                                    };
                                    
                                    MG.ShowDialog();
                                }

                                if (Tipo == "Personaliza")
                                {
                                    Extras.SMSPersonaliza f = new Extras.SMSPersonaliza(Admision, Mensaje_SMS, Celular, _datosCitaSMS.Hor_Pac_Cia);
                                    f.ShowDialog();
                                }
                            }
                        }
                    }                                      
                }
                else
                {
                    MG = new MensajesGeneral()
                    {
                        Mensaje = "Hay un inconveniente con esta admision, posiblemente no esta en estado Agendado",
                        TipoImagen = 1000
                    };

                    MG.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void ActualizarCel(int Admision)
        {
            try
            {
                string texto = Microsoft.VisualBasic.Interaction.InputBox(
                        "Digite el numero de celular a actualizar: ",
                        "Actualizacion de Celular de Paciente");

                if (texto != "")
                {
                    try
                    {
                        var _celBool = repositorioPacientes.ValidaCelular(texto);
                        if (_celBool == true)
                        {
                            bool _updateCel =  repositorioPacientes.ActualizarCelular(texto, Admision);

                            if (_updateCel != true)
                            {
                                MG = new MensajesGeneral()
                                {
                                    Mensaje = "El numero digitado no es valido",
                                    TipoImagen = 1000
                                }; 
                                MG.ShowDialog();
                            }
                            else
                            {
                                MG = new MensajesGeneral()
                                {
                                    Mensaje = "Actualizado con exito",
                                    TipoImagen = 3
                                };
                                MG.ShowDialog();
                            }
                        }
                        else
                        {
                            MG = new MensajesGeneral()
                            {
                                Mensaje = "El numero digitado no es valido",
                                TipoImagen = 1000
                            };
                            MG.ShowDialog();
                        }
                    }
                    catch (Exception ex)
                    {
                        TXTException T = new TXTException
                        {
                            FechaHora = DateTime.Now,
                            Error = ex.Message,
                            Formulario = this.Name,
                            Metodo = OverridesExtern.GetCurrentMethodName(),
                            Usuario = Contenedor.UsuarioLogueado
                        };

                        OverridesExtern.GenerarTXTException(T);
                    }
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException
                {
                    FechaHora = DateTime.Now,
                    Error = ex.Message,
                    Formulario = this.Name,
                    Metodo = OverridesExtern.GetCurrentMethodName(),
                    Usuario = Contenedor.UsuarioLogueado
                };

                OverridesExtern.GenerarTXTException(T);
            }
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
                TXTException T = new TXTException
                {
                    FechaHora = DateTime.Now,
                    Error = ex.Message,
                    Formulario = this.Name,
                    Metodo = OverridesExtern.GetCurrentMethodName(),
                    Usuario = Contenedor.UsuarioLogueado
                };

                OverridesExtern.GenerarTXTException(T);
            }
        }
        void EnviarEmail(int Admision)
        {
            try
            {
                if (Conexion.ConectionDictionary["Recordatorios"] == "A")
                {
                    CXN_HORARIO _datosCitaforEmail = repositorioAgenda2.DatosforMailSMS(Admision);
                    if (_datosCitaforEmail != null)
                    {
                        if (_datosCitaforEmail.Hor_Estado != "A")
                        {
                            MG = new MensajesGeneral()
                            {
                                Mensaje = "Para enviar un recordatorio de citas el estado de la cita no puede estar consumido, " +
                                            "debe estar en color negro el estado de la cita",
                                TipoImagen = 0
                            };
                            MG.ShowDialog();
                        }
                        else
                        {
                            string Hoy = Convert.ToDateTime(DateTime.Now).ToString(Conexion.ConectionDictionary["Format_Fecha"]);
                            string FCita = Convert.ToDateTime(_datosCitaforEmail.Hor_Pac_Fecha_Cita).ToString(Conexion.ConectionDictionary["Format_Fecha"]);

                            if (Convert.ToDateTime(FCita) < Convert.ToDateTime(Hoy))
                            {
                                MG = new MensajesGeneral()
                                {
                                    Mensaje = "Para enviar un recordatorio de citas el estado de la cita no puede estar consumido, " +
                                                "debe estar en color negro el estado de la cita",
                                    TipoImagen = 0
                                };                                
                                MG.ShowDialog();
                            }
                            else
                            {
                                var valida = repositorioPacientes.ValidaEmail(_datosCitaforEmail.Hor_Observacion);
                                if (valida != true)
                                {
                                    MG = new MensajesGeneral()
                                    {
                                        Mensaje = "El formato del correo del paciente es incorrecto",
                                        TipoImagen = 1000
                                    };
                                    MG.ShowDialog();
                                }
                                else
                                {
                                    CXN_EMAIL getFirstEmail = repositorioEmail.FirstEmail();
                                    MSender sender = new MSender();
                                    byte[] calendar = sender.EnviarCorreoConInvitacion("Recordatorio de Citas",
                                                                                        "Cita Medica Proxima",
                                                                                        Convert.ToDateTime(_datosCitaforEmail.Hor_Pac_Hora_Cita),
                                                                                        Convert.ToDateTime(_datosCitaforEmail.Hor_Pac_Hora_Cita.AddMinutes(20)),
                                                                                        Convert.ToDateTime(_datosCitaforEmail.Hor_Pac_Fecha_Cita),
                                                                                        "Centro Medico",
                                                                                        getFirstEmail.Ema_Email,
                                                                                        _datosCitaforEmail.Com_Nombre);

                                    if (calendar != null)
                                    {
                                        string html = "<center>" +
                                                              "<img src='cid:imagen' />" +
                                                              "<h2 style='background:#3D38B2; color:white;'>Hola, " + _datosCitaforEmail.Hor_Imp_Age + "</h2>" +
                                                              "<p>Queremos recordarte que tienes una cita pendiente para el servicio de: " + _datosCitaforEmail.Hor_Pac_Inasistencia.ToUpper() + ", asignada de la siguiente manera: <br><br>" +
                                                              "<b style='color:blue'>Prestador: </b><b style='color:black'>" + _datosCitaforEmail.Com_Nombre + "</b><br><br>" +
                                                              "<b style='color:blue'>Direccion: </b><b style='color:black'>" + _datosCitaforEmail.Com_Direccion + "</b><br><br>" +
                                                              "<b style='color:blue'>Telefono: </b><b style='color:black'>" + _datosCitaforEmail.Com_Telefono + "</b><br><br>" +
                                                              "<b style='color:blue'>Fecha: </b><b style='color:black'>" + Convert.ToDateTime(_datosCitaforEmail.Hor_Pac_Fecha_Cita).ToString("yyyy-MM-dd") + "</b><br><br>" +
                                                              "<b style='color:blue'>Hora: </b><b style='color:black'>" + Convert.ToDateTime(_datosCitaforEmail.Hor_Pac_Hora_Cita).ToString("HH:mm tt") + "</b><br><br>" +
                                                              "</p><p><b>Conoce nuestros Productos: <a href='https://slsoft.net'>https://slsoft.net</a></b></p>" +
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
                                                              "</p><br><hr>Identificador de Cita: <b>" + Admision + "</b><hr>" +
                                                          "</center>";

                                        string calendarB64 = Convert.ToBase64String(calendar);

                                        APIController.Services.Email.EmailService emailService = new APIController.Services.Email.EmailService();
                                        string enviarAPI = emailService.SendEmailAPI(new APIController.Clases.EmailRequest
                                        {
                                            EmailFrom = getFirstEmail.Ema_Email,
                                            EmailTo = _datosCitaforEmail.Hor_Observacion,
                                            EmailPassword = getFirstEmail.Ema_Pass,
                                            EmailBcc1 = getFirstEmail.Ema_Email,
                                            EmailBcc2 = null,
                                            Asunto = "RECORDATORIO DE CITAS",
                                            AttachmentFile = calendarB64,
                                            BodyMessage = html,
                                            TipoArchivo = "ics"
                                        }, Program.URLApiConexion).GetAwaiter().GetResult();

                                        MG = new MensajesGeneral()
                                        {
                                            TipoImagen = 0,
                                            Mensaje = enviarAPI.ToString() + " ... HECHO ..."
                                        };
                                        MG.ShowDialog();
                                    }
                                    else
                                    {
                                        MG = new MensajesGeneral()
                                        {
                                            TipoImagen = 1000,
                                            Mensaje = "No fue posible crear el Google Calendar, no se envio el mensaje"
                                        };
                                        MG.ShowDialog();                                        
                                    }
                                }
                            }
                        }                                            
                    }
                    else
                    {
                        MG = new MensajesGeneral()
                        {
                            Mensaje = "No hay datos suficientes para enviar recordatorio de citas",
                            TipoImagen = 1000
                        };
                        MG.ShowDialog();
                    }
                }
                else
                {
                    MG = new Comunes.MensajesGeneral()
                    {
                        Mensaje = "Su licencia no permite hacer el uso de esta herramienta",
                        TipoImagen = 1000
                    };
                    MG.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public void EventoInicial()
        {
            try
            {
                if (comboBox2.Items.Count > 0)
                {
                    CodePrestador = repositorioCompañias.getPrestadorbyName(comboBox2.Text).Com_Identificador;
                }
                else
                {
                    return;
                }

                if (calendarHQ1.CalControlDGV.CurrentCell.Style.BackColor == Color.LightGray)
                {
                    MG = new MensajesGeneral()
                    {
                        Mensaje = "Este dia esta bloqueado para el profesional",
                        TipoImagen = 0
                    };
                    MG.ShowDialog();
                    dataGridView1.DataSource = null;
                    return;
                }

                DateTime fechasel = calendarHQ1.dTPCalendar.Value.Date;
                //Console.WriteLine(fechasel.DayOfWeek);
                string diaSel = "";
                switch (fechasel.DayOfWeek.ToString())
                {
                    case "Monday":
                        diaSel = "Lunes";
                        break;
                    case "Tuesday":
                        diaSel = "Martes";
                        break;
                    case "Wednesday":
                        diaSel = "Miércoles";
                        break;
                    case "Thursday":
                        diaSel = "Jueves";
                        break;
                    case "Friday":
                        diaSel = "Viernes";
                        break;
                    case "Saturday":
                        diaSel = "Sábado";
                        break;
                    case "Sunday":
                        diaSel = "Domingo";
                        break;
                    default:
                        return;
                }

                var item = (ListaProfs)comboBox1.SelectedItem;
                if (item != null)
                {
                    List<CXN_DISPONIBILIDAD_2> H = repositorioDisponibilidad.GetHorarioByMedAndDay(item.Bodega, diaSel);
                    CodeProfesional = item.Bodega;

                    calendarHQ1.CodeProfesional = CodeProfesional;

                    SendMessage(dataGridView1.Handle, WM_SETREDRAW, false, 0);
                    CargarGrilla(H);
                    SendMessage(dataGridView1.Handle, WM_SETREDRAW, true, 0);
                    dataGridView1.Refresh();

                    calendarHQ1.CodeProfesional = CodeProfesional;
                    calendarHQ1.getBloqueosPforesional();
                }                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void dTPCalendar_ValueChanged(object sender, EventArgs e)
        {            
            EventoInicial();                                    
        }
        void CargarGrilla(List<CXN_DISPONIBILIDAD_2> H)
        {
            try
            {
                if (H != null)
                {
                    List<CXN_HORARIO> agendados = repositorioAgenda.ObtenerCitasDelDia(CodePrestador, calendarHQ1.dTPCalendar.Value, CodeProfesional);

                    Encabezados();      

                    foreach (var i in H)
                    {                        
                        string EstadoCitaAagendada = "";
                        string AdmisionAgenda = "";
                        string PacienteAgendado = "";
                        string AseguradoraAgendada = "";
                        string CodigoPaciente = "";
                        string RespuestaSesion = "0";
                        string colorFila = "";
                        string bloqEspaces = "";
                        string P2VxS = "";

                        if (agendados != null)
                        {
                            CXN_HORARIO datoCita = agendados.FirstOrDefault(x => x.Hor_Pac_Id_Hora == i.Ide && x.Hor_Estado != "C");
                            if (datoCita != null)
                            {
                                EstadoCitaAagendada = datoCita.Hor_Estado;
                                AdmisionAgenda = datoCita.Hor_Id.ToString();
                                PacienteAgendado = datoCita.Hor_Imp_Age;
                                AseguradoraAgendada = datoCita.PacienteAseguradora;
                                CodigoPaciente = datoCita.Hor_Pac_Id.ToString();
                                colorFila = datoCita.Hor_Color.ToString();
                                bloqEspaces = datoCita.Hor_BloqEspaces.ToString();
                                P2VxS = datoCita.Hor_Valida.ToString();

                                if (datoCita.Hor_Pac_Tipo_Serv == "CU" || datoCita.Hor_Pac_Tipo_Serv == "MG")
                                {
                                    string canactual = repositorioAgenda.Calcular2(datoCita.Hor_Pac_Id, datoCita.Hor_Pac_Tipo_Serv);
                                    int v1, v2;
                                    ObtenerValores(canactual, out v1, out v2);
                                    RespuestaSesion = $"{v1} / {v2}";
                                }
                                
                            }                            
                        }

                        DataRow row = dt.NewRow();

                        row[IdHora] = i.Ide;
                        row[EstadoHora] = i.Habilita;
                        row[EstadoCita] = EstadoCitaAagendada;
                        row[CodePaciente] = CodigoPaciente;

                        row[Hora] = Convert.ToDateTime(i.Hora).ToString("HH:mm tt");
                        row[Admisiona] = AdmisionAgenda;
                        row["Paciente"] = PacienteAgendado;
                        row[Sesiones] = RespuestaSesion;
                        row[SendEmail] = !string.IsNullOrEmpty(PacienteAgendado) ? ImaEmail : ImaBlocked;
                        row[SendSMS] = !string.IsNullOrEmpty(PacienteAgendado) ? ImaSMS : ImaBlocked;
                        row["Aseguradora"] = AseguradoraAgendada;
                        row["ColorFila"] = colorFila;
                        row["BlockEspaces"] = bloqEspaces;
                        row["P2VxS"] = P2VxS;

                        dt.Rows.Add(row);
                        dt.AcceptChanges();
                    }

                    dataGridView1.DataSource = dt;

                    Estilos();
                }
                else
                {
                    Encabezados();
                }
            }
            catch (Exception ex)
            {
                Encabezados();
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        void Estilos()
        {
            try
            {
                if (dataGridView1.RowCount > 0)
                {
                    dataGridView1.EnableHeadersVisualStyles = false;
                    dataGridView1.ScrollBars = ScrollBars.Both;

                    dataGridView1.Columns["Hora"].Width = 80;
                    dataGridView1.Columns["Admision"].Width = 65;

                    if (Amplia == true)
                    {
                        dataGridView1.Columns["Paciente"].Width = 500;
                        dataGridView1.Columns["Aseguradora"].Width = 500;
                    }
                    else
                    {
                        dataGridView1.Columns["Paciente"].Width = 325;
                        dataGridView1.Columns["Aseguradora"].Width = 325;
                    }

                    dataGridView1.Columns["Sesion"].Width = 50;
                    dataGridView1.Columns["@"].Width = 27;
                    dataGridView1.Columns["S"].Width = 27;

                    dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font(dataGridView1.Font, FontStyle.Bold);
                    dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 8, FontStyle.Bold);
                    dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.LightGray;
                    dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;
                    dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;

                    dataGridView1.Columns["Hora"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    dataGridView1.Columns["Admision"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    dataGridView1.Columns["Paciente"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    dataGridView1.Columns["Aseguradora"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

                    dataGridView1.Columns["IdHora"].Visible = false;
                    dataGridView1.Columns["EstadoHora"].Visible = false;
                    dataGridView1.Columns["EstadoCita"].Visible = false;
                    dataGridView1.Columns["CodePaciente"].Visible = false;
                    dataGridView1.Columns["ColorFila"].Visible = false;
                    dataGridView1.Columns["BlockEspaces"].Visible = false;
                    dataGridView1.Columns["P2VxS"].Visible = false;                    

                    dataGridView1.Columns["Admision"].DefaultCellStyle.BackColor = Color.LightGray;
                    dataGridView1.Columns["Admision"].DefaultCellStyle.Font = new Font("Arial", 9, FontStyle.Bold);
                    dataGridView1.Columns["Hora"].DefaultCellStyle.BackColor = Color.LightGray;                    
                    dataGridView1.Columns["Hora"].DefaultCellStyle.Font = new Font("Arial", 9, FontStyle.Bold);
                    dataGridView1.Columns["Sesion"].DefaultCellStyle.Font = new Font("Arial", 9, FontStyle.Bold);
                    
                    #region VALIDACIONES
                    if (Colorimetria == true)
                    {
                        foreach (DataGridViewRow row in dataGridView1.Rows)
                        {
                            Console.WriteLine(row.Cells["P2VxS"].Value.ToString());
                            if (row.Cells["P2VxS"].Value.ToString() == "S")
                            {
                                int currentIndex = row.Index;

                                if (currentIndex < dataGridView1.Rows.Count - 1)
                                {
                                    DataGridViewRow nextRow = dataGridView1.Rows[currentIndex + 1];

                                    if (nextRow.Cells["Paciente"].Value == null || nextRow.Cells["Paciente"].Value.ToString() == "")
                                    {
                                        dataGridView1.Rows[currentIndex + 1].Visible = false;
                                    }
                                }
                            }
                            if (row.Cells["EstadoHora"].Value != null)
                            {
                                string estadohora = row.Cells["EstadoHora"].Value.ToString();

                                if (estadohora == "A")
                                {
                                    row.Cells["Hora"].Style.ForeColor = Color.Green;

                                }
                                else
                                {
                                    row.Cells["Hora"].Style.ForeColor = Color.Red;
                                }
                            }
                            if (row.Cells["EstadoCita"].Value != null)
                            {
                                string estado = row.Cells["EstadoCita"].Value.ToString();
                                string colorFila = row.Cells["ColorFila"].Value.ToString();

                                if (estado == "P")
                                {
                                    switch (colorFila)
                                    {                                       
                                        case "N": //Nuevo
                                            row.Cells["Paciente"].Style.BackColor = Color.FromArgb(255, 192, 255);
                                            row.Cells["Paciente"].Style.ForeColor = Color.Purple;                                            
                                            break;

                                        case "I": //Inicio Paquete
                                            row.Cells["Paciente"].Style.BackColor = Color.DarkKhaki;
                                            row.Cells["Paciente"].Style.ForeColor = Color.Sienna;
                                            break;

                                        default: //Control Normal
                                            row.Cells["Paciente"].Style.BackColor = Color.LightGreen;
                                            row.Cells["Paciente"].Style.ForeColor = Color.Green;
                                            break;
                                    }
                                   
                                }
                                else if (estado == "H")
                                {
                                    row.Cells["Paciente"].Style.BackColor = Color.LightBlue;
                                    row.Cells["Paciente"].Style.ForeColor = Color.Blue;
                                }
                                else if (estado == "B")
                                {
                                    row.Cells["Paciente"].Style.BackColor = Color.LightGray;
                                    row.Cells["Paciente"].Style.ForeColor = Color.DimGray;
                                }
                                else
                                {
                                    row.Cells["Paciente"].Style.BackColor = Color.White;
                                    row.Cells["Paciente"].Style.ForeColor = Color.Black;
                                }
                            }
                        }
                    }
                    else //Si no hay colorimetria
                    {
                        foreach (DataGridViewRow row in dataGridView1.Rows)
                        {
                            if (row.Cells["P2VxS"].Value.ToString() == "S")
                            {
                                int currentIndex = row.Index;

                                if (currentIndex < dataGridView1.Rows.Count - 1)
                                {
                                    DataGridViewRow nextRow = dataGridView1.Rows[currentIndex + 1];

                                    if (nextRow.Cells["Paciente"].Value == null || nextRow.Cells["Paciente"].Value.ToString() == "")
                                    {
                                        dataGridView1.Rows[currentIndex + 1].Visible = false;
                                    }
                                }
                            }
                            if (row.Cells["EstadoHora"].Value != null)
                            {
                                string estadohora = row.Cells["EstadoHora"].Value.ToString();

                                if (estadohora == "A")
                                {
                                    row.Cells["Hora"].Style.ForeColor = Color.Green;

                                }
                                else
                                {
                                    row.Cells["Hora"].Style.ForeColor = Color.Red;
                                }
                            }
                            if (row.Cells["EstadoCita"].Value != null)
                            {
                                string estado = row.Cells["EstadoCita"].Value.ToString();

                                if (estado == "P")
                                {
                                    row.Cells["Paciente"].Style.BackColor = Color.LightGreen;
                                    row.Cells["Paciente"].Style.ForeColor = Color.Green;
                                }
                                else if (estado == "H")
                                {
                                    row.Cells["Paciente"].Style.BackColor = Color.LightBlue;
                                    row.Cells["Paciente"].Style.ForeColor = Color.Blue;
                                }
                                else if (estado == "B")
                                {
                                    row.Cells["Paciente"].Style.BackColor = Color.LightGray;
                                    row.Cells["Paciente"].Style.ForeColor = Color.DimGray;
                                }
                                else
                                {
                                    row.Cells["Paciente"].Style.BackColor = Color.White;
                                    row.Cells["Paciente"].Style.ForeColor = Color.Black;
                                }
                            }
                        }
                    }
                    #endregion

                    ExcluirIdesBloqueadosRange();
                    dataGridView1.ClearSelection();
                }
            }
            catch (Exception ex)
            {
                return;
                //MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        void ExcluirIdesBloqueadosRange()
        {
            try
            {
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (row.Cells["BlockEspaces"].Value != null)
                    {
                        if (row.Cells["BlockEspaces"].Value.ToString() != "")
                        {
                            int Conteo = Convert.ToInt32(row.Cells["BlockEspaces"].Value);
                            
                            if (Conteo > 0)
                            {
                                for (int i = 1; i < Conteo; i++)
                                {
                                    int currentIndex = row.Index;

                                    if (currentIndex < dataGridView1.Rows.Count - 1)
                                    {
                                        DataGridViewRow nextRow = dataGridView1.Rows[currentIndex + i];

                                        if (nextRow.Cells["Paciente"].Value == null || nextRow.Cells["Paciente"].Value.ToString() == "")
                                        {
                                            dataGridView1.Rows[currentIndex + i].Visible = false;
                                        }
                                    }
                                }
                            }

                        }                                               
                    }                       
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        void Encabezados()
        {
            dataGridView1.DataSource = null;
            dt = new DataTable();
            IdHora = dt.Columns.Add("IdHora", typeof(string));
            EstadoHora = dt.Columns.Add("EstadoHora", typeof(string));
            EstadoCita = dt.Columns.Add("EstadoCita", typeof(string));
            CodePaciente = dt.Columns.Add("CodePaciente", typeof(string));

            Hora = dt.Columns.Add("Hora", typeof(string));
            Admisiona = dt.Columns.Add("Admision", typeof(string));
            Paciente = dt.Columns.Add("Paciente", typeof(string));
            Sesiones = dt.Columns.Add("Sesion", typeof(string));
            SendEmail = dt.Columns.Add("@", typeof(Image));
            SendSMS = dt.Columns.Add("S", typeof(Image));
            Aseguradora = dt.Columns.Add("Aseguradora", typeof(string));
            ColorFila = dt.Columns.Add("ColorFila", typeof(string));
            BlockEspaces = dt.Columns.Add("BlockEspaces", typeof(string));
            P2VxS = dt.Columns.Add("P2VxS", typeof(string)); 
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            EventoInicial();
        }
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            EventoInicial();
        }
        private void label18_Click(object sender, EventArgs e)
        {
            ActivarProfesionales f = new ActivarProfesionales();
            f.ShowDialog();
        }
        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyData == Keys.Enter)
                {
                    if (TipoConsultaBarras == 1)
                    {
                        int _admTem = Convert.ToInt32(textBox1.Text);
                        string Estado = "";

                        otrosDatosPacienteHorario dataCita = repositorioAgenda.cargarAdmision(_admTem, "'A','P','H'");
                        if (dataCita == null)
                        {
                            MG = new MensajesGeneral()
                            {
                                TipoImagen = 1000,
                                Mensaje = "No hay resultados para esta admision"
                            };
                            MG.ShowDialog();
                            return;
                        }

                        Estado = repositorioAgenda2.consularAdmisionEstado(_admTem);

                        if (Estado != "0")
                        {
                            if (Estado == "A")
                            {
                                MenuOpcionesAgenda menuOpcionesAgenda = new MenuOpcionesAgenda(_admTem.ToString(), dataCita.Hor_Pac_Id_Hora, dataCita.Hor_Pac_Cia, dataCita.Hor_Pac_Hora_Cita.ToString(), dataCita.Hor_Pac_Bod);
                                menuOpcionesAgenda.ShowDialog();
                            }
                            else if (Estado == "P")
                            {
                                DatosCita f = new DatosCita(Convert.ToInt32(_admTem.ToString()), "Agenda");
                                f.ShowDialog();
                            }
                            else if (Estado == "H")
                            {
                                DatosCita f = new DatosCita(Convert.ToInt32(_admTem.ToString()), "Agenda");
                                f.ShowDialog();
                            }
                            else if (Estado == "C")
                            {
                                MG = new MensajesGeneral()
                                {
                                    TipoImagen = 0,
                                    Mensaje = "El estado de esta admision es CITA CANCELADA"
                                };
                                MG.ShowDialog();
                            }
                            else
                            {
                                MG = new MensajesGeneral()
                                {
                                    TipoImagen = 1000,
                                    Mensaje = "El estado de esta admision no permite su consumo"
                                };
                                MG.ShowDialog();
                            }
                        }
                        else
                        {
                            MG = new MensajesGeneral()
                            {
                                TipoImagen = 1000,
                                Mensaje = "No hay resultados para esta admision"
                            };
                            MG.ShowDialog();
                        }
                    }
                    else if (TipoConsultaBarras == 2)
                    {
                        Busqueda B = new Busqueda(calendarHQ1.dTPCalendar.Value, textBox1.Text);
                        B.ShowDialog();
                    }
                    else
                    {
                        richTextBox1.Focus();
                    }

                    richTextBox1.Focus();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void textBox1_Leave(object sender, EventArgs e)
        {
            label4.Text = "Busqueda Standar";
            label4.BackColor = Color.White;
            TipoConsultaBarras = 0;
        }
        private void Agendamiento_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyData == Keys.F5)
                {
                    Busqueda B = new Busqueda(calendarHQ1.dTPCalendar.Value);
                    B.ShowDialog();
                }
                else if (e.KeyData == Keys.Escape)
                {
                    this.Close();
                }
                else if (e.KeyData == Keys.F12)
                {
                    label4.Text = "Busqueda por Admision Barras";

                    TipoConsultaBarras = 1;

                    textBox1.Text = "";
                    textBox1.Focus();
                    textBox1.Text = "";

                    if (textBox1.Focus() == true)
                    {
                        label4.BackColor = Color.LightGreen;
                    }
                }
                else if (e.KeyData == Keys.F11)
                {
                    label4.Text = "Busqueda por Documento Barras";

                    TipoConsultaBarras = 2;

                    textBox1.Text = "";
                    textBox1.Focus();
                    textBox1.Text = "";

                    if (textBox1.Focus() == true)
                    {
                        label4.BackColor = Color.LightBlue;
                    }
                }
                else
                {
                    label4.Text = "Busqueda Standar";
                    label4.BackColor = Color.White;
                    TipoConsultaBarras = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarProfesionales();
        }
        Image Base64ToImage(byte[] bytes)
        {
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                return Image.FromStream(ms);
            }
        }
        public void ObtenerValores(string canactual, out int valor1, out int valor2)
        {
            valor1 = 0;
            valor2 = 0;

            if (string.IsNullOrEmpty(canactual))
                return;

            MatchCollection matches = Regex.Matches(canactual, @"\d+");

            if (matches.Count >= 2)
            {
                valor1 = int.Parse(matches[0].Value);
                valor2 = int.Parse(matches[1].Value);
            }
        }
    }
}
