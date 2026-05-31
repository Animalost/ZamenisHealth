using Domain;
using Domain.CXN;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using ShortLink.SDK.Clases;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using ZamenisHealth.Clases;
using ZamenisHealth.Comunes;
using ZamenisHealth.Recepcion.Extras;

namespace ZamenisHealth.Recepcion
{
    public partial class Agenda : ConfigForm.BaseForm
    {
        private static readonly IAgendaC repositorioAgendaC = new MAgendaC();
        private static readonly IOrdenes repositorioOrdenes = new MOrdenes();
        private static readonly IBodegas repositorioBodegas = new MBodegas();
        private static readonly ICompañia repositorioCompañias = new MCompañia();
        private static readonly IAgenda repositorioHorario = new MAgenda();
        private static readonly IPacientes repositorioPacientes = new MPacientes();
        private static readonly ILogSender repositorioLogSender = new MLogSender();
        private static readonly IRcCaja repositorioRcCaja = new MRcCaja();
        private static readonly IAgendaC repositorioFechasAgendaa = new MAgendaC();
        private static readonly IConvenios repositorioConvenios = new MConvenios();
        private static readonly IReportes repositorioReportes = new MReportes();
        private static readonly IDisponibilidad repositorioDisponibilidad = new MDisponibilidad();
        private static readonly IConfSystem RepoConfSystem = new MConfSystem();
        private static readonly IDatosEmail RepoEmail = new MDatosEmail();
        private static readonly IHelisa repoHelisa = new MHelisa();
        private static readonly IAseguradoras repositorioAseguradoras = new MAseguradoras();
        private static readonly IFirmasDigitales fDigitales = new MFirmasDigitales();
        private static readonly IPlanos planos = new MPlanos();

        private bool accionarCombos = false;
        private bool FirmasAutoComplete;
        private DateTime Max15;
        private DateTime fechaa;
        private bool Colorimetria;
        private int diasBloq;
        public int IdProf, IdCom;
        int Adm_Selected = 0;
        private string name_Selected;
        private string dia;
        private string idHOUR, idIDEHOUR, tipoBod;
        private bool isHandlingClick = false;
        private int TipoConsultaBarras = 0;
        private bool EncuestaCU = false;

        private List<CXN_HORARIO> getCancelWEB;
        private Dictionary<int, string> listaSaleConsulta;
        private Dictionary<int, GridRowData> gridRowDataDictionary;

        List<FirmasR> modelo;
        List<RCCAJA> Exporta;
        DataTable dt;

        private List<string> listaDobleEspacio;
        public List<DateTime> listaFestiva;
        static List<DateTime> listaBloqueos;

        public Agenda()
        {
            InitializeComponent();

            Control.CheckForIllegalCrossThreadCalls = false;

            ConfigForm.customToolstriplabel(toolStripLabel3);
            ConfigForm.customToolstriplabel(toolStripButton2);
            ConfigForm.customToolstriplabel(toolStripLabel4);
            ConfigForm.customToolstriplabel(toolStripButton1);
            ConfigForm.customToolstriplabel(toolStripButton4);
            ConfigForm.customToolstriplabel(toolStripButton3);
            ConfigForm.customToolstriplabel(toolStripButton5);

            
            ConfigForm.MoverForma(label52, this);
        }

        public void setDayAgenda(string DiasCal, string DiaNameCal)
        {
            textoAgendaDia.Text = DiasCal;
            textoAgendaFecha.Text = DiaNameCal;
        }
        private void CargarProfesionales()
        {
            comboBox1.DataSource = null;
            comboBox1.Items.Clear();

            List<string> ListaProf =  repositorioBodegas.Profesionales(comboBox3.Text);       

            if (ListaProf != null)
            {
                comboBox1.Items.Clear();

                foreach (var i in ListaProf)
                {
                    comboBox1.Items.Add(i.ToString());
                }

                comboBox1.SelectedIndex = 0;
            }
        }
        private void CargarPrestadores()
        {
            comboBox2.DataSource = null;
            comboBox2.Items.Clear();

            List<string> ListaCia = new List<string>();

            
                List<CXN_CIA> Lista = repositorioCompañias.getAllCompañias();
                if (Lista != null)
                {
                    foreach (var i in Lista)
                    {
                        ListaCia.Add(i.Com_Nombre.ToString());
                    }
                }
            

            if (ListaCia != null)
            {
                foreach (var i in ListaCia)
                {
                    comboBox2.Items.Add(i.ToString());
                }

                comboBox2.SelectedIndex = 0;
            }
        }
        void ValidarBarras()
        {
            try
            {
                label12.Text = "ESC = Cerrar  F5 = Buscar Paciente";

                Dictionary<string, string> getConfig = new Dictionary<string, string>();

                
                    getConfig = RepoConfSystem.getListado();
                

                if (getConfig != null)
                {
                    if (Preferencias.TicketCitas == "A")
                    {
                        textBox1.Enabled = true;
                        textBox1.Visible = true;

                        label12.Text = label12.Text + "  F11 = Barras Doc  F12 = Barras Admision";
                    }
                    else
                    {
                        textBox1.Enabled = false;
                        textBox1.Visible = false;
                    }

                    if (getConfig["AutocompletarFirmas"] == "A")
                    {
                        FirmasAutoComplete = true;
                    }
                    else
                    {
                        FirmasAutoComplete = false;
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
        private void Agenda_Load(object sender, EventArgs e)
        {
            try
            {
                toolStripLabel3.MouseLeave += ToolStripButton_MouseLeave;
                toolStripLabel4.MouseLeave += ToolStripButton_MouseLeave;
                toolStripButton2.MouseLeave += ToolStripButton_MouseLeave;
                toolStripButton1.MouseLeave += ToolStripButton_MouseLeave;
                toolStripButton4.MouseLeave += ToolStripButton_MouseLeave;
                toolStripButton3.MouseLeave += ToolStripButton_MouseLeave;
                toolStripButton5.MouseLeave += ToolStripButton_MouseLeave;


                label52.Text = "Agenda Medica: - " + Contenedor.UsuarioLogueado;

                this.Titulo.Visible = false;
                this.ImageClose.Visible = false;

                ValidarBarras();

                this.Colorimetria = (Preferencias.Colorimetria == "A" ? true : false);
                if (this.Colorimetria == false)
                {
                    label5.Visible = false;
                    label21.Visible = false;
                    label4.Visible = false;
                    label19.Visible = false;
                    label13.Visible = false;
                }

                Max15 = DateTime.Now.Date;

                diasBloq = Preferencias.Bloqueo;
                if (diasBloq >= 1)
                {
                    Max15 = Convert.ToDateTime(Max15.AddDays(diasBloq));
                }

                if (Preferencias.TCPIP == "A")
                {
                    TimerSalidas.Enabled = true;
                    button2.Visible = true;
                }

                comboBox3.Text = "Todos";
                CargarProfesionales();
                CargarPrestadores();

                CXN_BODEGAS getBod =repositorioBodegas.getDatosName(comboBox1.Text);

                this.IdProf = getBod.Bod_Numero;

                CXN_CIA getCia = new CXN_CIA();

                getCia = repositorioCompañias.getPrestadorbyName(comboBox2.Text);


                this.IdCom = getCia.Com_Identificador;

                DateTime Hoy = DateTime.Now.Date;

                calZamenis1.Visible = true;
                calZamenis1.IdProf = this.IdProf;
                calZamenis1.Max15 = this.Max15;
                calZamenis1.diasBloq = this.diasBloq;

                calZamenis1.calendarFestivo = repositorioAgendaC.getCalendario();
                calZamenis1.getListaBloqueos = repositorioAgendaC.CargarListBlocked(this.IdProf);
                calZamenis1.getlistasLunes = repositorioDisponibilidad.getHorariosHabilitados(this.IdProf, "Lunes");
                calZamenis1.getlistasMartes = repositorioDisponibilidad.getHorariosHabilitados(this.IdProf, "Martes");
                calZamenis1.getlistasMiercoles = repositorioDisponibilidad.getHorariosHabilitados(this.IdProf, "Miércoles");
                calZamenis1.getlistasJueves = repositorioDisponibilidad.getHorariosHabilitados(this.IdProf, "Jueves");
                calZamenis1.getlistasViernes = repositorioDisponibilidad.getHorariosHabilitados(this.IdProf, "Viernes");
                calZamenis1.getlistasSabado = repositorioDisponibilidad.getHorariosHabilitados(this.IdProf, "Sábado");
                calZamenis1.getlistasDomingo = repositorioDisponibilidad.getHorariosHabilitados(this.IdProf, "Domingo");

                calZamenis1.getCantDiaLunes = repositorioAgendaC.getOcupados(Convert.ToDateTime(DateTime.Now.Date), this.IdProf);
                calZamenis1.getCantDiaMartes = repositorioAgendaC.getOcupados(Convert.ToDateTime(DateTime.Now.Date), this.IdProf);
                calZamenis1.getCantDiaMiercoles = repositorioAgendaC.getOcupados(Convert.ToDateTime(DateTime.Now.Date), this.IdProf);
                calZamenis1.getCantDiaJueves = repositorioAgendaC.getOcupados(Convert.ToDateTime(DateTime.Now.Date), this.IdProf);
                calZamenis1.getCantDiaViernes = repositorioAgendaC.getOcupados(Convert.ToDateTime(DateTime.Now.Date), this.IdProf);
                calZamenis1.getCantDiaSabado = repositorioAgendaC.getOcupados(Convert.ToDateTime(DateTime.Now.Date), this.IdProf);
                calZamenis1.getCantDiaDomingo = repositorioAgendaC.getOcupados(Convert.ToDateTime(DateTime.Now.Date), this.IdProf);

                calZamenis1.Cargar();
                accionarCombos = true;

                label9.BackColor = Color.DarkBlue;

                textoAgendaDia.Text = DateTime.Now.Date.ToString("dddd");
                textoAgendaFecha.Text = DateTime.Now.Date.ToString("yyyy/MM/dd");

                if (calZamenis1.Anclar == false)
                {
                    calZamenis1.Visible = false;
                }

                if (RepoConfSystem.getListado()["EncuestaQRWeb"] == "A")
                {
                    this.EncuestaCU = true;
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
        private void ToolStripButton_MouseLeave(object sender, EventArgs e)
        {
            toolStripLabel3.ForeColor = Color.WhiteSmoke;
            toolStripLabel3.BackColor = Color.FromArgb(55, 85, 120);

            toolStripLabel4.ForeColor = Color.WhiteSmoke;
            toolStripLabel4.BackColor = Color.FromArgb(55, 85, 120);

            toolStripButton2.ForeColor = Color.WhiteSmoke;
            toolStripButton2.BackColor = Color.FromArgb(55, 85, 120);

            toolStripButton1.ForeColor = Color.WhiteSmoke;
            toolStripButton1.BackColor = Color.FromArgb(55, 85, 120);

            toolStripButton4.ForeColor = Color.WhiteSmoke;
            toolStripButton4.BackColor = Color.FromArgb(55, 85, 120);

            toolStripButton3.ForeColor = Color.WhiteSmoke;
            toolStripButton3.BackColor = Color.FromArgb(55, 85, 120);

            toolStripButton5.ForeColor = Color.WhiteSmoke;
            toolStripButton5.BackColor = Color.FromArgb(55, 85, 120);
        }
        public void fecha()
        {
            try
            {
                List<CXN_DIAS_WEB> lBl = new List<CXN_DIAS_WEB>();

                
                    lBl = repositorioAgendaC.CargarListBlocked(this.IdProf);
                

                if (lBl != null)
                {
                    listaBloqueos = new List<DateTime>();
                    foreach (CXN_DIAS_WEB i in lBl)
                    {
                        listaBloqueos.Add(Convert.ToDateTime(i.F_Fecha));
                    }

                    DateTime G = (listaBloqueos.Find(X => X == Convert.ToDateTime(calZamenis1.dTPCalendar.Value.Date)));
                    if (G == Convert.ToDateTime(calZamenis1.dTPCalendar.Value.Date))
                    {
                        //panel2.Visible = true;
                        pictureBox2.Visible = true;
                        label7.Visible = true;
                        D.DataSource = null;
                        
                        CXN_DIAS_WEB razon = (lBl.Find(X => X.F_Fecha == Convert.ToDateTime(calZamenis1.dTPCalendar.Value.Date)));
                        if (razon != null)
                        {
                            label11.Visible = true;
                            label11.Text = razon.R_Razon.ToString();
                        }
                    }
                    else
                    {
                        DateTime G2 = (listaFestiva.Find(X => X == Convert.ToDateTime(calZamenis1.dTPCalendar.Value.Date)));
                        if (G2 == Convert.ToDateTime(calZamenis1.dTPCalendar.Value.Date))
                        {
                            string getRazonFest = "";

                            
                                getRazonFest = repositorioAgendaC.Festivo(Convert.ToDateTime(G2));
                            
                            
                            MensajesGeneral m = new MensajesGeneral();
                            m.TipoImagen = 0;
                            m.Mensaje = "Este dia es festivo por razon de: \n\r\n" + getRazonFest + "\n\r\n" + "Aun asi, la agenda esta disponible para agendar citas";
                            m.ShowDialog();
                        }

                        pictureBox2.Visible = false;
                        label7.Visible = false;
                        label11.Visible = false;

                        CXN_CIA getCia = new CXN_CIA();
                        CXN_BODEGAS getBod = new CXN_BODEGAS();

                        
                            getCia = repositorioCompañias.getPrestadorbyName(comboBox2.Text);
                        

                        
                            getBod = repositorioBodegas.getDatosName(comboBox1.Text);
                        
           
                        Grilla(getBod.Bod_Numero, getCia.Com_Identificador, Convert.ToDateTime(calZamenis1.dTPCalendar.Value.Date), calZamenis1.txtDia.Text);
                    }
                }
                else
                {
                    pictureBox2.Visible = false;
                    label7.Visible = false;

                    CXN_CIA getCia = new CXN_CIA();
                    CXN_BODEGAS getBod = new CXN_BODEGAS();

                    
                        getCia = repositorioCompañias.getPrestadorbyName(comboBox2.Text);
                    

                    
                        getBod = repositorioBodegas.getDatosName(comboBox1.Text);
                    

                    Grilla(getBod.Bod_Numero, getCia.Com_Identificador, Convert.ToDateTime(calZamenis1.dTPCalendar.Value.Date), calZamenis1.txtDia.Text);
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
        private void Cierra_Agenda()
        {
            try
            {
                this.Dispose();
                this.Close();
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

                this.Dispose();
                this.Close();
            }
        }
        void CargarPendientes()
        {
            try
            {
                bool getPend = false;

                
                    getPend = repositorioHorario.getPendientes();
                
                
                if (getPend == true)
                {
                    label22.Text = "Existen Autorizaciones pendientes de consumir";
                    label22.Visible = true;
                }
                else
                {
                    label22.Text = "";
                    label22.Visible = false;
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
        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarProfesionales();

            comboBox1.Focus();
            richTextBox1.Focus();
        }

        #region Grilla

        private void label6_Click(object sender, EventArgs e)
        {
            CancelacionesWEB c = new CancelacionesWEB(getCancelWEB);
            c.ShowDialog();
        }
        void CargarCanceladas(DateTime Fec)
        {
            try
            {
                getCancelWEB = new List<CXN_HORARIO>();
                getCancelWEB = repositorioAgendaC.consultaCancelaWEB(Convert.ToDateTime(Fec));
                if (getCancelWEB != null)
                {
                    label6.Visible = true;
                }
                else
                {
                    label6.Visible = false;
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
        void Grilla(int IdProf, int IdCom, DateTime fecha, string dia)
        {
            try
            {
                this.IdProf = IdProf;
                this.IdCom = IdCom;
                this.fechaa = fecha;
                this.dia = dia;

                EstiloGrilla();
                EjecucionGrilla();
                DataGridView_RowPrePaint();

                CargarCanceladas(fecha);
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
        void EstiloGrilla()
        {
            try
            {
                dt = new DataTable();
                DataColumn Hora = dt.Columns.Add("Hora", typeof(string));
                DataColumn Admision = dt.Columns.Add("Admision", typeof(string));
                DataColumn Paciente = dt.Columns.Add("Paciente", typeof(string));
                DataColumn R = dt.Columns.Add("Ret", typeof(string));
                DataColumn E = dt.Columns.Add("@", typeof(Image));
                DataColumn S = dt.Columns.Add("M", typeof(Image));
                DataColumn Modalidad = dt.Columns.Add("Modalidad", typeof(string));
                DataColumn Novedades = dt.Columns.Add("Novedades", typeof(string));
                DataColumn Sesion = dt.Columns.Add("Sesion", typeof(string)); 
                DataColumn Aseguradora = dt.Columns.Add("Aseguradora", typeof(string));

                D.DataSource = dt;
                D.Cursor = Cursors.Hand;
                D.Font = new Font("Arial", 9, FontStyle.Bold);
                //D.Size = new Size(695, 643);
                //D.Location = new Point(13, 56);
                D.EnableHeadersVisualStyles = false;
                D.RowHeadersVisible = false;
                D.GridColor = Color.Blue;
                D.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                D.ScrollBars = ScrollBars.Both;
                D.AllowUserToResizeColumns = false;
                D.AllowUserToResizeRows = false;
                D.ReadOnly = true;
                D.MultiSelect = false;
                D.AllowDrop = false;
                D.BorderStyle = BorderStyle.FixedSingle;
                D.AllowUserToAddRows = false;
                D.CellBorderStyle = DataGridViewCellBorderStyle.Single;
                D.BackgroundColor = Color.White;

                D.MouseWheel += new MouseEventHandler(dataGridView1_MouseWheel);

                D.Columns["Hora"].Width = 75;
                D.Columns["Paciente"].Width = 300;
                D.Columns["Admision"].Width = 60;
                D.Columns["M"].Width = 30;
                D.Columns["@"].Width = 30;
                D.Columns["Ret"].Width = 40;
                D.Columns["Modalidad"].Width = 80;
                D.Columns["Aseguradora"].Width = 500;
                D.Columns["Novedades"].Width = 100;
                D.Columns["Sesion"].Width = 60;

                D.ColumnHeadersDefaultCellStyle.Font = new Font(D.Font, FontStyle.Bold);
                D.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 8, FontStyle.Bold);
                D.ColumnHeadersDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#bfdbff");
                D.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;
                D.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;

                D.Columns["Hora"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                D.Columns["Admision"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                D.Columns["Paciente"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                D.Columns["Ret"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                D.Columns["Modalidad"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                D.Columns["M"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                D.Columns["@"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                D.Columns["Aseguradora"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                D.Columns["Novedades"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                D.Columns["Sesion"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

                D.Columns["Hora"].SortMode = DataGridViewColumnSortMode.NotSortable;
                D.Columns["Admision"].SortMode = DataGridViewColumnSortMode.NotSortable;
                D.Columns["Paciente"].SortMode = DataGridViewColumnSortMode.NotSortable;
                D.Columns["Ret"].SortMode = DataGridViewColumnSortMode.NotSortable;
                D.Columns["M"].SortMode = DataGridViewColumnSortMode.NotSortable;
                D.Columns["@"].SortMode = DataGridViewColumnSortMode.NotSortable;
                D.Columns["Modalidad"].SortMode = DataGridViewColumnSortMode.NotSortable;
                D.Columns["Aseguradora"].SortMode = DataGridViewColumnSortMode.NotSortable;
                D.Columns["Novedades"].SortMode = DataGridViewColumnSortMode.NotSortable;
                D.Columns["Sesion"].SortMode = DataGridViewColumnSortMode.NotSortable;

                D.Columns["Ret"].ToolTipText = "Llegada tarde de pacientes";
                D.Columns["M"].ToolTipText = "Envia mensaje de texto de recordatorio de citas";
                D.Columns["@"].ToolTipText = "Envia correo electronico de recordatorio de citas";

                D.Columns["Hora"].Frozen = true;
                D.Columns["Admision"].Frozen = true;

                DataGridViewCellStyle style = new DataGridViewCellStyle();
                style.Font = new Font(D.Font, FontStyle.Bold);

                D.ClearSelection();
                D.Visible = false;
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
        string Modalidad(string codMod)
        {
            switch (codMod)
            {
                case "01":
                    return "Presencial";

                case "06":
                    return "Virtual";

                case "03":
                    return "Domicilio";

                case "02":
                    return "Extramural unidad móvil";

                case "04":
                    return "Extramural jornada de salud";

                case "07":
                    return "Telemedicina no interactiva";

                case "08":
                    return "Telemedicina telexperticia";

                case "09":
                    return "Telemedicina telemonitoreo";

                default:
                    return "";
            }
        }
        static Image Base64ToImage(byte[] bytes)
        {
            // Crear la imagen desde el byte[]
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
        void EjecucionGrilla()
        {
            try
            {
                List<CXN_HORARIO> _chargeGrill = repositorioHorario.CargarAgenda(IdProf, IdCom, Convert.ToDateTime(fechaa), dia);                

                if (_chargeGrill != null)
                {
                    gridRowDataDictionary = new Dictionary<int, GridRowData>();
                    listaDobleEspacio = new List<string>();
                    int indexRow = 0;

                    foreach (CXN_HORARIO i in _chargeGrill)
                    {
                        //string canactual = repositorioFechasAgendaa.Calcular2(i.Hor_Pac_Id, i.Hor_Pac_Tipo_Serv);
                        string canactual = repositorioFechasAgendaa.Calcular2(i.Hor_Pac_Id, "CU");
                        int v1, v2;
                        ObtenerValores(canactual, out v1, out v2);
                        string RespuestaSesion = $"{v1} / {v2}";

                        Image image1;
                        Image image2;

                        image1 = Base64ToImage(i.ImageSMS);
                        image2 = Base64ToImage(i.ImageEmail);

                        if (i.Hor_Pac_Id_Hora != null && i.Hor_Pac_Id_Hora != "")
                        {
                            DataRow row = dt.NewRow();

                            row["Hora"] = Convert.ToDateTime(i.Hor_Pac_Hora_Cita).ToString("HH:mm tt");
                            row["Admision"] = i.Hor_Pac_Sal.ToString();
                            row["Paciente"] = i.Hor_Imp_Age;
                            row["Ret"] = i.Hor_Pac_Minutos.ToString();
                            row["M"] = image1;
                            row["@"] = image2;
                            row["Modalidad"] = Modalidad(i.Hor_Pac_Modalidad);
                            row["Novedades"] = i.HorObservaTemp;
                            row["Sesion"] = RespuestaSesion == "0/0" ? "" : RespuestaSesion;
                            row["Aseguradora"] = i.Hor_ValDerechos;

                            gridRowDataDictionary[indexRow] = new GridRowData
                            {
                                IDE = i.Hor_Pac_Id_Hora,
                                Habilita = i.Hor_Autoriza,
                                Paciente_Est = i.Hor_Estado,
                                Observacion = i.Hor_Observacion,
                                InPaquete = i.Hor_IniciaSesion,
                                PacSal = i.Hor_Pac_Razon,
                                CUP = i.Hor_Pac_Cup,
                                BLOQUEOS = i.Hor_BloqEspaces.ToString(),
                                Arrastra = i.Hor_ArrastraHistoria
                            };

                            if (i.DatosPaciente != null && i.DatosPaciente.Pac_Doble == "S")
                            {
                                listaDobleEspacio.Add(i.Hor_Pac_Id_Hora);
                            }

                            dt.Rows.Add(row);
                            dt.AcceptChanges();

                            indexRow++;
                        }
                    }

                    D.ClearSelection();
                    //D.Visible = true;
                }
                else
                {
                    TXTException T = new TXTException
                    {
                        FechaHora = DateTime.Now,
                        Error = "Inconveniente no hay datos para cargar en el metodo EjecucionGrilla",
                        Formulario = this.Name,
                        Metodo = OverridesExtern.GetCurrentMethodName(),
                        Usuario = Contenedor.UsuarioLogueado
                    };

                    OverridesExtern.GenerarTXTException(T);
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
        void DataGridView_RowPrePaint()
        {
            try
            {
                D.ClearSelection();

                int indexrow = 0;

                foreach (DataGridViewRow row in D.Rows)
                {
                    if (gridRowDataDictionary.ContainsKey(indexrow))
                    {
                        GridRowData rowData = gridRowDataDictionary[indexrow];

                        string Hab = (rowData.Habilita != null ? rowData.Habilita.ToString() : "");
                        string P_Est = (rowData.Paciente_Est != null ? rowData.Paciente_Est.ToString() : "");
                        string PacCell = (row.Cells["Paciente"].Value != null ? row.Cells["Paciente"].Value.ToString() : "");

                        if (PacCell != "")
                        {
                            string Serv =  (repositorioConvenios.NameServiceCUP(rowData.CUP.ToString()) != null ? repositorioConvenios.NameServiceCUP(rowData.CUP.ToString()) : "");
                                                       
                            D.Rows[row.Index].Cells["Paciente"].ToolTipText = D.Rows[row.Index].Cells["Admision"].Value + " - " +
                                                                              D.Rows[row.Index].Cells["Paciente"].Value + " - " + "\n\r" +
                                                                              rowData.Observacion.ToString() + "\n\r" + Serv;

                            D.Rows[row.Index].Cells["Ret"].ToolTipText = "Llegada tarde de pacientes";

                            D.Rows[row.Index].Cells["M"].ToolTipText = "Envia mensaje de texto de recordatorio de citas";
                            D.Rows[row.Index].Cells["@"].ToolTipText = "Envia correo electronico de recordatorio de citas";
                            row.Cells["@"].Style.BackColor = Color.LightGray;
                            row.Cells["M"].Style.BackColor = Color.LightGray;
                        }
                        else
                        {
                            D.Rows[row.Index].Cells["M"].ToolTipText = "Esta opcion no esta disponible";
                            D.Rows[row.Index].Cells["@"].ToolTipText = "Esta opcion no esta disponible";
                            row.Cells["@"].Style.BackColor = Color.LightGray;
                            row.Cells["M"].Style.BackColor = Color.LightGray;
                        }

                        row.Cells["Admision"].Style.BackColor = Color.LightGray;
                        row.Cells["Ret"].Style.BackColor = Color.LightGray;
                        row.Cells["Modalidad"].Style.BackColor = Color.LightGray;
                        row.Cells["Hora"].Style.BackColor = Color.LightGray;

                        if (Hab != "A")
                        {
                            row.Cells["Hora"].Style.ForeColor = Color.Red;
                        }
                        else
                        {
                            row.Cells["Hora"].Style.ForeColor = Color.Green;
                        }

                        if (this.Colorimetria == true)
                        {
                            if (P_Est == "A")
                            {
                                row.Cells["Paciente"].Style.ForeColor = Color.Black;
                                row.Cells["Paciente"].Style.BackColor = Color.White;
                            }
                            if (P_Est == "P")
                            {
                                row.Cells["Paciente"].Style.ForeColor = Color.Green;
                                row.Cells["Paciente"].Style.BackColor = Color.LightGreen;
                            }
                            if (P_Est == "B")
                            {
                                row.Cells["Paciente"].Style.ForeColor = Color.Red;
                                row.Cells["Paciente"].Style.BackColor = Color.Orange;
                            }

                            string pacsal = (rowData.PacSal != null ? rowData.PacSal.ToString() : "");
                            string inpaquete = (rowData.InPaquete != null ? rowData.InPaquete.ToString() : "");

                            if (pacsal == "C" && P_Est == "P") //Control amarillo //CONTROL CON AUTROIZACION
                            {
                                row.Cells["Paciente"].Style.ForeColor = Color.Orange;
                                row.Cells["Paciente"].Style.BackColor = Color.Yellow;
                            }
                            if (pacsal == "Z" && P_Est == "P") //
                            {
                                row.Cells["Paciente"].Style.ForeColor = Color.White;
                                row.Cells["Paciente"].Style.BackColor = Color.Navy;
                            }
                            
                            if (inpaquete == "S" && P_Est == "P") //Consume autorizacion 
                            {
                                if (pacsal == "N" && P_Est == "P") //Paciente Nuevo
                                {
                                    row.Cells["Paciente"].Style.ForeColor = Color.Purple;
                                    row.Cells["Paciente"].Style.BackColor = Color.LightPink;
                                }
                                if (rowData.PacSal.ToString() == "A" && P_Est == "P") //Paciente Nuevo
                                {
                                    row.Cells["Paciente"].Style.ForeColor = Color.Sienna; //Paciente inicio paquete
                                    row.Cells["Paciente"].Style.BackColor = Color.DarkKhaki;
                                }                                
                            }
                            if (rowData.Arrastra == "S" && P_Est == "P") //control comun
                            {
                                row.Cells["Paciente"].Style.ForeColor = Color.IndianRed;
                                row.Cells["Paciente"].Style.BackColor = Color.PapayaWhip;
                            }

                            if (P_Est == "H")
                            {
                                row.Cells["Paciente"].Style.ForeColor = Color.Blue;
                                row.Cells["Paciente"].Style.BackColor = Color.LightBlue;
                            }
                        }
                        else
                        {
                            if (P_Est == "A")
                            {
                                row.Cells["Paciente"].Style.ForeColor = Color.Black;
                                row.Cells["Paciente"].Style.BackColor = Color.White;
                            }
                            if (P_Est == "P")
                            {
                                row.Cells["Paciente"].Style.ForeColor = Color.Green;
                                row.Cells["Paciente"].Style.BackColor = Color.LightGreen;
                            }
                            if (P_Est == "B")
                            {
                                row.Cells["Paciente"].Style.ForeColor = Color.Red;
                                row.Cells["Paciente"].Style.BackColor = Color.Orange;
                            }
                            if (P_Est == "H")
                            {
                                row.Cells["Paciente"].Style.ForeColor = Color.Blue;
                                row.Cells["Paciente"].Style.BackColor = Color.LightBlue;
                            }
                        }
                    }

                    indexrow++;
                }

                ExcluirIdesHoraDoblePaciente();
                ExcluirIdesBloqueadosRange();
                D.ClearSelection();
                CargarPendientes();
                D.Visible = true;
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
        void FirmasPrint(int Adm_Selected)
        {
            try
            {
                otrosDatosPacienteHorario dataAdm1 = repositorioFechasAgendaa.cargarAdmision(Adm_Selected, "'H','A','P'");
                CXN_CIA DatNombre = repositorioCompañias.getPrestadorbyCode(IdCom);
                CXN_PACIENTES pacData = repositorioPacientes.LlamarPacientebyId(dataAdm1.Hor_Pac_Id);
                CXN_ASEGURADORA aseData = repositorioAseguradoras.getInfoFromAsebyCode(pacData.Pac_Aseguradora);

                DateTime FechaLimite = dataAdm1.Hor_Pac_Fecha_Cita;
                List<(int Adm, string Aut, int Cant)> listAdmition = fDigitales.getAdmitions(dataAdm1.Hor_Pac_Id, IdCom, dataAdm1.Hor_Pac_Fecha_Cita.Date);
  
                if (listAdmition != null)
                {
                    List<FirmasR> lista = new List<FirmasR>();
                    int Contador = 1;

                    foreach (var i in listAdmition)
                    {
                        otrosDatosPacienteHorario dataAdm = repositorioFechasAgendaa.cargarAdmision(i.Adm, "'H'");

                        CXN_FIRMASDIGITALES fTemp = fDigitales.getFirmas(i.Adm);
                        byte[] firmaByte = null;

                        if (fTemp == null)
                        {
                            firmaByte = Convert.FromBase64String(fDigitales.ImageNull());
                        }
                        else
                        {
                            using (MemoryStream ms = new MemoryStream(fTemp.Firma))
                            using (Bitmap bmp = new Bitmap(ms))
                            using (MemoryStream ms2 = new MemoryStream())
                            {
                                bmp.Save(ms2, System.Drawing.Imaging.ImageFormat.Png);
                                firmaByte = ms2.ToArray();
                            }
                            /*
                            Bitmap ss = null;
                            using (MemoryStream ms = new MemoryStream(firmaByte))
                            {
                                ss = new Bitmap(ms);
                            }
                            PictureBox pp = new PictureBox();
                            pp.Image = ss;
                            pp.Image.Save("C:\\CXN\\Reportes\\Walaaa.png", System.Drawing.Imaging.ImageFormat.Png);*/
                        }

                        lista.Add(new FirmasR
                        {
                            PacienteNombre = pacData.Pac_PrimerA.ToString() + " " +
                                             pacData.Pac_SegundoA.ToString() + " " +
                                             pacData.Pac_PrimerN.ToString() + " " +
                                             pacData.Pac_SegundoN.ToString(),
                            PacienteAseguradora = aseData.Ase_Descripcion.ToString(),
                            PacienteIdentificacion = pacData.Pac_TipoId.ToString() + " " +
                                                     pacData.Pac_IdNum.ToString(),
                            PacienteTelefono = pacData.Pac_Telefono.ToString(),
                            PacienteDireccion = pacData.Pac_Direccion.ToString(),

                            EmpresaNombre = DatNombre.Com_Nombre,
                            EmpresaDireccion = DatNombre.Com_Direccion,
                            Com_UsuarioGraba = DatNombre.Com_Tipo_Doc + " " + DatNombre.Com_Identificacion, //idd prestaddor
                            EmpresaTelefono = dataAdm.Com_Nombre_SMS.Contains("CONSULTA") ? "C" :  Contador.ToString(),
                            Logo = Convert.FromBase64String(DatNombre.Com_Logo),

                            FechaBase = Convert.ToDateTime(dataAdm.Hor_Pac_Fecha_Cita),
                            FirmaByte = firmaByte,
                            Con_Nombre = dataAdm.Com_Nombre_SMS,
                            Com_Direccion = listAdmition[0].Aut, //autorizacion
                            Admision = i.Adm,
                            Cantidad = i.Cant
                        });

                        if (!dataAdm.Com_Nombre_SMS.Contains("CONSULTA"))
                        {
                            Contador++;
                        }                        
                    }

                    ConfigForm.GenerarReportViewer("DataSet_Firmas", "ZamenisHealth.Reportes.FirmasDigitales.rdlc", lista);
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
        void Firmas_Print3(object sender, EventArgs s)
        {
            FirmasPrint(Adm_Selected);
        }
        private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (calZamenis1.Anclar == false)
            {
                calZamenis1.Visible = false;
            }

            if (isHandlingClick)
                return;

            isHandlingClick = true;

            try
            {
                D.ClearSelection();

                if (e.Button == MouseButtons.Right)
                {
                    if (D.Rows[e.RowIndex].Cells[4].Value.ToString() == "") //Email
                    {
                        return;
                    }

                    Adm_Selected = Convert.ToInt32(D.Rows[e.RowIndex].Cells[1].Value.ToString());

                    ContextMenuStrip menu = new ContextMenuStrip();
                    menu.Font = new Font("Arial", 12);
                    menu.Padding = new Padding(5, 5, 5, 30);

                    name_Selected = D.Rows[e.RowIndex].Cells[2].Value.ToString(); //paciente
                    D.CurrentCell = D.Rows[e.RowIndex].Cells[e.ColumnIndex];

                    if (this.tipoBod != "MG")
                    {
                        ToolStripMenuItem itemPrincipal = new ToolStripMenuItem("Hojas de Firmas", Properties.Resources2.MayorQue_Black);

                        if (this.tipoBod == "CU")
                        {
                            ToolStripMenuItem Firmas_Print_CU = new ToolStripMenuItem("Hoja de Firmas Curaciones Automatica");
                            itemPrincipal.DropDownItems.Add(Firmas_Print_CU);
                            Firmas_Print_CU.Click += new EventHandler(Firmas_Print);
                            ToolStripMenuItem Firmas_Print_CU2 = new ToolStripMenuItem("Hoja de Firmas Curaciones Defecto");
                            itemPrincipal.DropDownItems.Add(Firmas_Print_CU2);
                            Firmas_Print_CU2.Click += new EventHandler(Firmas_Print2);

                            if (Preferencias.TabletaFirmas == "A")
                            {
                                ToolStripMenuItem Firmas_Print_CU3 = new ToolStripMenuItem("Hoja de Firmas Digitales");
                                itemPrincipal.DropDownItems.Add(Firmas_Print_CU3);
                                Firmas_Print_CU3.Click += new EventHandler(Firmas_Print3);
                            }                           
                        }
                        else if (this.tipoBod == "TF")
                        {
                            ToolStripMenuItem Firmas_Print_TF = new ToolStripMenuItem("Hoja de Firmas Terapia Fisica");
                            itemPrincipal.DropDownItems.Add(Firmas_Print_TF);
                            Firmas_Print_TF.Click += new EventHandler(Firmas_Print_TF_Void);
                           /* menu.Items.Add("Asignar Junta Medica", Properties.Resources2.MayorQue_Black).Name = "J_MED";
                            menu.Items["J_MED"].Click += J_MED;*/
                        }
                        else if (this.tipoBod == "TO")
                        {
                            ToolStripMenuItem Firmas_Print_TO = new ToolStripMenuItem("Hoja de Firmas Terapia Ocupacional");
                            itemPrincipal.DropDownItems.Add(Firmas_Print_TO);
                            Firmas_Print_TO.Click += new EventHandler(Firmas_Print_TO_Void);
                           /* menu.Items.Add("Asignar Junta Medica", Properties.Resources2.MayorQue_Black).Name = "J_MED";
                            menu.Items["J_MED"].Click += J_MED;*/
                        }
                        else if (this.tipoBod == "PS")
                        {
                            ToolStripMenuItem Firmas_Print_PS = new ToolStripMenuItem("Hoja de Firmas Psicologia");
                            itemPrincipal.DropDownItems.Add(Firmas_Print_PS);
                            Firmas_Print_PS.Click += new EventHandler(Firmas_Print_PS_Void);
                           /* menu.Items.Add("Asignar Junta Medica", Properties.Resources2.MayorQue_Black).Name = "J_MED";
                            menu.Items["J_MED"].Click += J_MED;*/
                        }

                        menu.Items.Add(itemPrincipal);
                    }

                    if (this.EncuestaCU == true && this.tipoBod != "MG")
                    {
                        ToolStripMenuItem itemEncuestas = new ToolStripMenuItem("Encuestas", Properties.Resources2.MayorQue_Black);

                        ToolStripMenuItem encuestaCU = new ToolStripMenuItem("Encuesta de Curaciones");
                        itemEncuestas.DropDownItems.Add(encuestaCU);
                        encuestaCU.Click += new EventHandler(EncuestaCUbtn);

                        menu.Items.Add(itemEncuestas);
                    }

                    if (this.Colorimetria == true && this.tipoBod == "MG")
                    {
                        if (name_Selected != "")
                        {
                            if (gridRowDataDictionary.ContainsKey(e.RowIndex))
                            {
                                GridRowData rowData = gridRowDataDictionary[e.RowIndex];
                                if (rowData.Paciente_Est != "H")
                                {
                                    ToolStripMenuItem itemSecundario = new ToolStripMenuItem("Cambiar Tipo de Cita", Properties.Resources2.MayorQue_Black);

                                    ToolStripMenuItem PN = new ToolStripMenuItem("PACIENTE NUEVO (Nunca ha venido a la institucion)");
                                    ToolStripMenuItem IP = new ToolStripMenuItem("INICIO PAQUETE (Comienza nueva autorizacion)");
                                    ToolStripMenuItem RI = new ToolStripMenuItem("REINGRESO (Mas de 30 dias sin Venir)");
                                    ToolStripMenuItem CA = new ToolStripMenuItem("CONTROL CIERRE AUTORIZACION (Generar Orden Nueva)");
                                    ToolStripMenuItem CT = new ToolStripMenuItem("CONTROL TRATAMIENTO (No Generar Orden)");
                                    ToolStripMenuItem SA = new ToolStripMenuItem("SALIDA (Fin Tratamiento no vuelve mas)");

                                    itemSecundario.DropDownItems.Add(PN);
                                    itemSecundario.DropDownItems.Add(IP);
                                    itemSecundario.DropDownItems.Add(RI);
                                    itemSecundario.DropDownItems.Add(CA);
                                    itemSecundario.DropDownItems.Add(CT);
                                    itemSecundario.DropDownItems.Add(SA);

                                    PN.Click += new EventHandler(ChangeTipoMG_PN);
                                    IP.Click += new EventHandler(ChangeTipoMG_IP);
                                    RI.Click += new EventHandler(ChangeTipoMG_RI);
                                    CA.Click += new EventHandler(ChangeTipoMG_CA);
                                    CT.Click += new EventHandler(ChangeTipoMG_CT);
                                    SA.Click += new EventHandler(ChangeTipoMG_SA);

                                    menu.Items.Add(itemSecundario);
                                }
                            }                            
                        }                        
                    }

                    if (Conexion.ConectionDictionary["Recordatorios"] == "A")
                    {
                        menu.Items.Add("Editar contenido del mensaje SMS", Properties.Resources2.MayorQue_Black).Name = "E_SMS";
                        menu.Items["E_SMS"].Click += E_SMS;
                    }                   

                    menu.Items.Add("Ver asistencia previa de este paciente", Properties.Resources2.MayorQue_Black).Name = "A_PRE";
                    menu.Items["A_PRE"].Click += A_PRE;

                    menu.Items.Add("Ver ordenes generadas", Properties.Resources2.MayorQue_Black).Name = "O_Generated";
                    menu.Items["O_Generated"].Click += O_Generated;

                    menu.Items.Add("Imprimir recibo de caja", Properties.Resources2.MayorQue_Black).Name = "RC_Print";
                    menu.Items["RC_Print"].Click += RC_Print;

                    if (Preferencias.TicketCitas == "A")
                    {
                        menu.Items.Add("Imprimir Ticket de citas", Properties.Resources2.MayorQue_Black).Name = "Ticket_Print";
                        menu.Items["Ticket_Print"].Click += Ticket_Print;
                    }

                    menu.Items.Add("Certificados de asistencia", Properties.Resources2.MayorQue_Black).Name = "C_Asist";
                    menu.Items["C_Asist"].Click += C_Asist;

                    menu.Items.Add("Anular esta admision", Properties.Resources2.MayorQue_Black).Name = "A_Admi";
                    menu.Items["A_Admi"].Click += A_Admi;

                    menu.Items.Add("Copiar numero de documento", Properties.Resources2.MayorQue_Black).Name = "CopyCC";
                    menu.Items["CopyCC"].Click += CopyCC;

                    menu.Items.Add("Copiar nombre del paciente", Properties.Resources2.MayorQue_Black).Name = "CopyName";
                    menu.Items["CopyName"].Click += CopyName;

                    menu.Items.Add("Generar reporte de citas", Properties.Resources2.MayorQue_Black).Name = "RCitas";
                    menu.Items["RCitas"].Click += RCitas;

                    menu.Items.Add("Mover paciente a otro profesional", Properties.Resources2.MayorQue).Name = "MoverAgendas";
                    menu.Items["MoverAgendas"].Click += MoverAgendas;                    

                    Rectangle coordenada = D.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, false);

                    int anchoCelda = coordenada.Location.X - 100;
                    int altoCelda = coordenada.Location.Y;

                    int X = anchoCelda + D.Location.X;
                    int Y = altoCelda + D.Location.Y;

                    menu.Show(D, new System.Drawing.Point(X, Y));
                    D.ClearSelection();

                    return;
                }

                if (e.Button == MouseButtons.Left)
                {
                    if (e.ColumnIndex == 0) //hora
                    {
                        if (D.Rows[e.RowIndex].Cells[2].Value.ToString() == "") //paciente
                        {
                            if (gridRowDataDictionary.ContainsKey(e.RowIndex)) //indice de la fila
                            {
                                GridRowData rowData = gridRowDataDictionary[e.RowIndex]; //indice de la fila

                                this.idHOUR = D.Rows[e.RowIndex].Cells[0].Value.ToString(); //hora
                                this.idIDEHOUR = rowData.IDE.ToString(); //ide hora    

                                ContextMenuStrip menu = new ContextMenuStrip();
                                menu.Font = new Font("Arial", 12);
                                menu.Padding = new Padding(5, 5, 5, 30);

                                menu.Items.Add("Bloquear Rango de Horas", Properties.Resources2.MayorQue_Black).Name = "BloqRange";
                                menu.Items["BloqRange"].Click += BloqRange;

                                Rectangle coordenadas = D.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, false);

                                int anchoCeldas = coordenadas.Location.X - 100;
                                int altoCeldas = coordenadas.Location.Y;

                                int Xs = anchoCeldas + D.Location.X;
                                int Ys = altoCeldas + D.Location.Y;

                                menu.Show(D, new System.Drawing.Point(Xs, Ys));
                            }
                        }

                        return;
                    }

                    //EMAIL
                    if (e.ColumnIndex == 4) 
                    {
                        if (Conexion.ConectionDictionary["Recordatorios"] == "A")
                        {
                            if (D.Rows[e.RowIndex].Cells[1].Value.ToString() != "") //Admision
                            {
                                Send_Mailer(Convert.ToInt32(D.Rows[e.RowIndex].Cells[1].Value.ToString())); //Admision
                                return;
                            }
                            else
                            {
                                Comunes.MensajesGeneral MG = new Comunes.MensajesGeneral();
                                MG.Mensaje = "No es posible enviar un recordatorio de citas ya que no hay paciente agendado aqui";
                                MG.TipoImagen = 1000;
                                MG.ShowDialog();
                                return;
                            }
                        }
                        else
                        {
                            Comunes.MensajesGeneral MG33 = new Comunes.MensajesGeneral();
                            MG33.Mensaje = "Su licencia no permite hacer el uso de esta herramienta";
                            MG33.TipoImagen = 1000;
                            MG33.ShowDialog();
                            return;
                        }
                    }

                    //SMS
                    if (e.ColumnIndex == 5) 
                    {
                        if (Conexion.ConectionDictionary["Recordatorios"] == "A")
                        {
                            if (D.Rows[e.RowIndex].Cells[1].Value.ToString() != "") //Admision
                            {
                                SMS(Convert.ToInt32(D.Rows[e.RowIndex].Cells[1].Value.ToString()), "SMSAgenda"); //Admision
                                return;
                            }
                            else
                            {
                                Comunes.MensajesGeneral MG = new Comunes.MensajesGeneral();
                                MG.Mensaje = "No es posible enviar un recordatorio de citas ya que no hay paciente agendado aqui";
                                MG.TipoImagen = 1000;
                                MG.ShowDialog();
                                return;
                            }
                        }
                        else
                        {
                            Comunes.MensajesGeneral MG3 = new Comunes.MensajesGeneral();
                            MG3.Mensaje = "Su licencia no permite hacer el uso de esta herramienta";
                            MG3.TipoImagen = 1000;
                            MG3.ShowDialog();
                            return;
                        }
                    }

                    if (e.ColumnIndex == 7) //NOVEDADES
                    {
                        if (D.Rows[e.RowIndex].Cells[7].Value.ToString() != "")
                        {
                            Comunes.MensajesGeneral MG = new Comunes.MensajesGeneral();
                            MG.Mensaje = D.Rows[e.RowIndex].Cells[7].Value.ToString();
                            MG.TipoImagen = 3;
                            MG.ShowDialog();
                            return;
                        }
                    }

                    string Admision = D.Rows[e.RowIndex].Cells[1].Value.ToString(); //Admision

                    if (D.Rows[e.RowIndex].Cells[2].Value.ToString() != "") //Paciente
                    {
                        string Estado = repositorioHorario.consularAdmisionEstado(Convert.ToInt32(Admision));                    
                       
                        if (Estado != "0")
                        {
                            if (Estado == "A")
                            {
                                ConsultaAdmision f = new ConsultaAdmision(Admision);
                                f.ShowDialog();
                            }
                            else if (Estado == "P")
                            {
                                DatosCita f = new DatosCita(Convert.ToInt32(Admision), "Agenda");
                                f.ShowDialog();
                            }
                            else if (Estado == "H")
                            {
                                DatosCita f = new DatosCita(Convert.ToInt32(Admision), "Agenda");
                                f.ShowDialog();
                            }
                            else if (Estado == "B")
                            {
                                DialogResult result = MessageBox.Show("¿Desea desbloquear este espacio?", "Zamenis Health - Agendas", MessageBoxButtons.YesNo);

                                if (result == DialogResult.Yes)
                                {
                                    
                                    repositorioHorario.desbloquearEspacio(Comunes.Contenedor.UsuarioLogueado, Convert.ToInt32(Admision));
                                                                        

                                    calZamenis1.IdProf = this.IdProf;
                                    calZamenis1.Max15 = this.Max15;
                                    calZamenis1.Cargar(); 
                                }
                            }
                            else
                            {
                                MensajesGeneral m = new MensajesGeneral();
                                m.TipoImagen = 1000;
                                m.Mensaje = "Estado desconocido en la posicion seleccionada de la grilla";
                                m.ShowDialog();
                            }
                        }
                        else
                        {                            
                            (int CodProf, string TipoBod) DatoProf = repositorioBodegas.ProfesionalId(comboBox1.Text);
                            
                            CXN_CIA IdCia =  repositorioCompañias.getPrestadorbyName(comboBox2.Text);
                                                      
                            if (gridRowDataDictionary.ContainsKey(e.RowIndex))
                            {
                                GridRowData rowData = gridRowDataDictionary[e.RowIndex];

                                AgendarCita Agenda_Cita = new AgendarCita(DatoProf.CodProf, //Profesional
                                                                      calZamenis1.txtDia.Text, //Nombre del dia
                                                                      Convert.ToDateTime(D.Rows[e.RowIndex].Cells[0].Value.ToString()), //Hora Hora
                                                                      DatoProf.TipoBod, // Tipo bodega
                                                                      rowData.IDE.ToString(), //Ide Hora
                                                                      IdCia.Com_Identificador, //Prestador
                                                                      Convert.ToDateTime(calZamenis1.txtFecha.Text), //Fecha
                                                                      rowData.Habilita.ToString()); //Verde o Rojo Horario

                                Agenda_Cita.ShowDialog();
                            }
                        }
                    }
                    else
                    {
                        (int CodProf, string TipoBod) DatoProf = repositorioBodegas.ProfesionalId(comboBox1.Text);
                        
                        CXN_CIA IdCia =  repositorioCompañias.getPrestadorbyName(comboBox2.Text);
                        
                        if (gridRowDataDictionary.ContainsKey(e.RowIndex))
                        {
                            GridRowData rowData = gridRowDataDictionary[e.RowIndex];

                            AgendarCita Agenda_Cita = new AgendarCita(DatoProf.CodProf, //Profesional
                                                                      calZamenis1.txtDia.Text, //Nombre del dia
                                                                      Convert.ToDateTime(D.Rows[e.RowIndex].Cells[0].Value.ToString()), //Hora Hora
                                                                      DatoProf.TipoBod, // Tipo bodega
                                                                      rowData.IDE.ToString(), //Ide Hora
                                                                      IdCia.Com_Identificador, //Prestador
                                                                      Convert.ToDateTime(calZamenis1.txtFecha.Text), //Fecha
                                                                      rowData.Habilita.ToString()); //Verde o Rojo Horario

                            Agenda_Cita.ShowDialog();
                        }
                    }
                }
            }
            catch
            {
                D.ClearSelection();
            }
            finally
            {
                isHandlingClick = false;
            }
        }
        void EncuestaCUbtn(object sender, EventArgs e)
        {
            try
            {
                Extras.EncuestasQR q = new EncuestasQR(Convert.ToInt32(Adm_Selected));
                q.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        void ChangeTipoMG_PN(object sender, EventArgs e) 
        {
            
                repositorioHorario.updateTipoCitaMG("N", Convert.ToInt32(this.Adm_Selected), "S"); RechargeTrueCheck();
            
        }      
        void ChangeTipoMG_IP(object sender, EventArgs e) 
        {
            
                repositorioHorario.updateTipoCitaMG("A", Convert.ToInt32(this.Adm_Selected), "S"); RechargeTrueCheck();
            
        }
        void ChangeTipoMG_RI(object sender, EventArgs e) 
        {
            
                repositorioHorario.updateTipoCitaMG("R", Convert.ToInt32(this.Adm_Selected), ""); RechargeTrueCheck();
            
        }
        void ChangeTipoMG_CA(object sender, EventArgs e) 
        {
            
                repositorioHorario.updateTipoCitaMG("C", Convert.ToInt32(this.Adm_Selected), ""); RechargeTrueCheck();
            
        }
        void ChangeTipoMG_CT(object sender, EventArgs e) 
        {
            
                repositorioHorario.updateTipoCitaMG("Z", Convert.ToInt32(this.Adm_Selected), ""); RechargeTrueCheck();
            
        }
        void ChangeTipoMG_SA(object sender, EventArgs e) 
        {
            
                repositorioHorario.updateTipoCitaMG("E", Convert.ToInt32(this.Adm_Selected), ""); RechargeTrueCheck();
            
        }
        void dataGridView1_MouseWheel(object sender, MouseEventArgs e)
        {
            try
            {
                int currentIndex = this.D.FirstDisplayedScrollingRowIndex;
                int scrollLines = SystemInformation.MouseWheelScrollLines;

                if (e.Delta > 0)
                {
                    this.D.FirstDisplayedScrollingRowIndex
                        = Math.Max(0, currentIndex - scrollLines);
                }
                else if (e.Delta < 0)
                {
                    this.D.FirstDisplayedScrollingRowIndex
                        = currentIndex + scrollLines;
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
        #endregion Grilla
        public void RechargeTrueCheck()
        {
            try
            {
                calZamenis1.IdProf = this.IdProf;
                calZamenis1.Max15 = this.Max15;
                calZamenis1.diasBloq = this.diasBloq;

                
                    calZamenis1.calendarFestivo = repositorioAgendaC.getCalendario();
                    calZamenis1.getListaBloqueos = repositorioAgendaC.CargarListBlocked(this.IdProf);
                    calZamenis1.getlistasLunes = repositorioDisponibilidad.getHorariosHabilitados(this.IdProf, "Lunes");
                    calZamenis1.getlistasMartes = repositorioDisponibilidad.getHorariosHabilitados(this.IdProf, "Martes");
                    calZamenis1.getlistasMiercoles = repositorioDisponibilidad.getHorariosHabilitados(this.IdProf, "Miércoles");
                    calZamenis1.getlistasJueves = repositorioDisponibilidad.getHorariosHabilitados(this.IdProf, "Jueves");
                    calZamenis1.getlistasViernes = repositorioDisponibilidad.getHorariosHabilitados(this.IdProf, "Viernes");
                    calZamenis1.getlistasSabado = repositorioDisponibilidad.getHorariosHabilitados(this.IdProf, "Sábado");
                    calZamenis1.getlistasDomingo = repositorioDisponibilidad.getHorariosHabilitados(this.IdProf, "Domingo");

                    calZamenis1.getCantDiaLunes = repositorioAgendaC.getOcupados(Convert.ToDateTime(DateTime.Now.Date), this.IdProf);
                    calZamenis1.getCantDiaMartes = repositorioAgendaC.getOcupados(Convert.ToDateTime(DateTime.Now.Date), this.IdProf);
                    calZamenis1.getCantDiaMiercoles = repositorioAgendaC.getOcupados(Convert.ToDateTime(DateTime.Now.Date), this.IdProf);
                    calZamenis1.getCantDiaJueves = repositorioAgendaC.getOcupados(Convert.ToDateTime(DateTime.Now.Date), this.IdProf);
                    calZamenis1.getCantDiaViernes = repositorioAgendaC.getOcupados(Convert.ToDateTime(DateTime.Now.Date), this.IdProf);
                    calZamenis1.getCantDiaSabado = repositorioAgendaC.getOcupados(Convert.ToDateTime(DateTime.Now.Date), this.IdProf);
                    calZamenis1.getCantDiaDomingo = repositorioAgendaC.getOcupados(Convert.ToDateTime(DateTime.Now.Date), this.IdProf);
                                

                calZamenis1.Cargar(); //Llena el calendario
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
        void ExcluirIdesBloqueadosRange()
        {
            try
            {
                int indexrow = 0;                

                foreach (DataGridViewRow row in D.Rows)
                {
                    if (gridRowDataDictionary.ContainsKey(indexrow))
                    {
                        GridRowData rowData = gridRowDataDictionary[indexrow];

                        if (rowData.BLOQUEOS != null && Convert.ToInt32(rowData.BLOQUEOS) > 0)
                        {
                            int Conteo = Convert.ToInt32(rowData.BLOQUEOS);

                            for (int i = 1; i < Conteo; i++)
                            {
                                int currentIndex = row.Index;

                                if (currentIndex < D.Rows.Count - 1)
                                {
                                    DataGridViewRow nextRow = D.Rows[currentIndex + i];

                                    if (nextRow.Cells["Paciente"].Value == null || nextRow.Cells["Paciente"].Value.ToString() == "")
                                    {
                                        D.Rows[currentIndex + i].Visible = false;
                                    }
                                }
                            }
                        }
                    }

                    indexrow++;
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
        void ExcluirIdesHoraDoblePaciente()
        {
            try
            {
                if (this.listaDobleEspacio != null)
                {
                    foreach (string i in this.listaDobleEspacio)
                    {
                        foreach (DataGridViewRow row in D.Rows)
                        {
                            if (gridRowDataDictionary.ContainsKey(row.Index))
                            {
                                GridRowData rowData = gridRowDataDictionary[row.Index];

                                if (rowData.IDE != null && rowData.IDE.ToString() == i)
                                {
                                    int currentIndex = row.Index;

                                    if (currentIndex < D.Rows.Count - 1)
                                    {
                                        DataGridViewRow nextRow = D.Rows[currentIndex + 1];

                                        if (nextRow.Cells["Paciente"].Value == null || nextRow.Cells["Paciente"].Value.ToString() == "")
                                        {
                                            D.Rows[currentIndex + 1].Visible = false;
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
        private void Agenda_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyData == Keys.F5)
                {
                    Busqueda B = new Busqueda(calZamenis1.dTPCalendar.Value);
                    B.ShowDialog();
                }
                if (e.KeyData == Keys.Escape)
                {
                    if (calZamenis1.Visible == true)
                    {
                        if (calZamenis1.Anclar == false)
                        {
                            calZamenis1.Visible = false;
                        }
                    }
                    else
                    {
                        Cierra_Agenda();
                    }                    
                }

                if (e.KeyData == Keys.F12)
                {
                    if (textBox1.Visible == true)
                    {
                        this.TipoConsultaBarras = 1;

                        textBox1.Text = "";
                        textBox1.Focus();
                        textBox1.Text = "";

                        if (textBox1.Focus() == true)
                        {
                            label52.BackColor = Color.DarkGreen;
                            label52.Text = "Consulta por Admision";
                        }
                        else
                        {
                            label52.BackColor = Color.RoyalBlue;
                            label52.Text = "Agenda Medica - Zamenis Health";
                        }
                    }
                    else
                    {
                        this.TipoConsultaBarras = 0;
                    }
                }

                if (e.KeyData == Keys.F11)
                {
                    if (textBox1.Visible == true)
                    {
                        this.TipoConsultaBarras = 2;

                        textBox1.Text = "";
                        textBox1.Focus();
                        textBox1.Text = "";

                        if (textBox1.Focus() == true)
                        {
                            label52.BackColor = Color.DarkGreen;
                            label52.Text = "Consulta por Documento";
                        }
                        else
                        {
                            label52.BackColor = Color.RoyalBlue;
                            label52.Text = "Agenda Medica - Zamenis Health";
                        }
                    }
                    else
                    {
                        this.TipoConsultaBarras = 0;
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
        private void label18_Click(object sender, EventArgs e)
        {
            ActivarProfesionales f = new ActivarProfesionales();
            f.ShowDialog();
        }
        private void Send_Mailer(int Admision)
        {
            try
            {
                Comunes.MensajesGeneral MG = new Comunes.MensajesGeneral();

                CXN_HORARIO _datosCitaforEmail = repositorioHorario.DatosforMailSMS(Admision);                 
                if (_datosCitaforEmail != null)
                {
                    if (_datosCitaforEmail.Hor_Estado != "A")
                    {
                        MG.Mensaje = "Para enviar un recordatorio de citas el estado de la cita no puede estar consumido, " +
                                        "debe estar en color negro el estado de la cita";
                        MG.TipoImagen = 0;
                        MG.ShowDialog();
                        return;
                    }

                    string Hoy = Convert.ToDateTime(DateTime.Now).ToString(Conexion.ConectionDictionary["Format_Fecha"]);
                    string FCita = Convert.ToDateTime(_datosCitaforEmail.Hor_Pac_Fecha_Cita).ToString(Conexion.ConectionDictionary["Format_Fecha"]);

                    if (Convert.ToDateTime(FCita) < Convert.ToDateTime(Hoy))
                    {
                        MG.Mensaje = "Para enviar un recordatorio de citas el estado de la cita no puede estar consumido, " +
                                        "debe estar en color negro el estado de la cita";
                        MG.TipoImagen = 0;
                        MG.ShowDialog();
                        return;
                    }

                    var valida = repositorioPacientes.ValidaEmail(_datosCitaforEmail.Hor_Observacion);
                    if (valida != true)
                    {
                        MG.Mensaje = "El formato del correo del paciente es incorrecto";
                        MG.TipoImagen = 1000;
                        MG.ShowDialog();
                        return;
                    }


                    CXN_EMAIL getFirstEmail = RepoEmail.FirstEmail();
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
                        
                        MG.TipoImagen = 0;
                        MG.Mensaje = enviarAPI.ToString() + " ... HECHO ...";
                        MG.ShowDialog();
                    }                   
                    else
                    {
                        MG.TipoImagen = 1000;
                        MG.Mensaje = "No fue posible crear el Google Calendar, no se envio el mensaje";
                        MG.ShowDialog();
                    }
                }
                else
                {
                    MG.Mensaje = "No hay datos suficientes para enviar recordatorio de citas";
                    MG.TipoImagen = 1000;
                    MG.ShowDialog();
                    return;
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
                            bool _updateCel = false;

                            
                                _updateCel = repositorioPacientes.ActualizarCelular(texto, Admision);
                            

                            if (_updateCel != true)
                            {
                                Comunes.MensajesGeneral MG = new Comunes.MensajesGeneral();
                                MG.Mensaje = "El numero digitado no es valido";
                                MG.TipoImagen = 1000;
                                MG.ShowDialog();
                            }
                            else
                            {
                                Comunes.MensajesGeneral MG = new Comunes.MensajesGeneral();
                                MG.Mensaje = "Actualizado con exito";
                                MG.TipoImagen = 3;
                                MG.ShowDialog();
                            }
                        }
                        else
                        {
                            Comunes.MensajesGeneral MG = new Comunes.MensajesGeneral();
                            MG.Mensaje = "El numero digitado no es valido";
                            MG.TipoImagen = 1000;
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
        private async void SMS(int Admision, string Tipo)
        {
            try
            {
                Comunes.MensajesGeneral MG = new Comunes.MensajesGeneral();

                CXN_HORARIO _datosCitaSMS =  repositorioHorario.DatosforMailSMS(Admision);                
               
                if (_datosCitaSMS != null)
                {
                    if (_datosCitaSMS.Hor_Estado != "A")
                    {
                        MG.Mensaje = "Para enviar un recordatorio de citas el estado de la cita no puede estar consumido, " +
                            "debe estar en color negro el estado de la cita";
                        MG.TipoImagen = 1000;
                        MG.ShowDialog();
                        return;
                    }

                    DateTime Hoy = DateTime.Now.Date;

                    string Celular = _datosCitaSMS.Hor_RegAtn;
                    DateTime Fecha = Convert.ToDateTime(_datosCitaSMS.Hor_Pac_Fecha_Cita);
                    DateTime Hora = Convert.ToDateTime(_datosCitaSMS.Hor_Pac_Hora_Cita);

                    if (Convert.ToDateTime(Fecha.ToString(Conexion.ConectionDictionary["Format_Fecha"])) < Convert.ToDateTime(Hoy.ToString(Conexion.ConectionDictionary["Format_Fecha"])))
                    {
                        MG.Mensaje = "Para enviar un recordatorio de citas el estado de la cita no puede estar consumido, " +
                            "debe estar en color negro el estado de la cita";
                        MG.TipoImagen = 1000;
                        MG.ShowDialog();
                        return;
                    }

                    bool ValidaCel = repositorioPacientes.ValidaCelular(Celular);
                    if (ValidaCel != true)
                    {
                        MG.Mensaje = "El numero de celular del paciente contiene caracteres NO numericos, " +
                            "actualize el numero y vuelva a intentar -> " + Celular;
                        MG.TipoImagen = 1000;
                        MG.ShowDialog();

                        ActualizarCel(Convert.ToInt32(_datosCitaSMS.Hor_Pac_Id));
                        return;
                    }

                    string Mensaje_SMS = "Cita agendada dia " + Convert.ToDateTime(Fecha).ToString(Conexion.ConectionDictionary["Format_Fecha"]) +
                                             " hora " + Convert.ToDateTime(Hora).ToString("H:mm tt") + " en " + _datosCitaSMS.Com_Nombre_SMS + " Tel " + _datosCitaSMS.Com_Telefono_SMS + " Dir " + _datosCitaSMS.Com_Direccion +
                                             " favor asista 20 minutos antes";

                    if (Tipo == "SMSAgenda")
                    {
                        CXN_CIA getCantSMS = repositorioCompañias.getPrestadorbyCode(_datosCitaSMS.Hor_Pac_Cia);
                        if (getCantSMS.Com_SMS <= 0)
                        {
                            MG.Mensaje = "No hay mensajes disponibles, recargue su cuenta";
                            MG.TipoImagen = 1000;
                            MG.ShowDialog();
                            return;
                        }

                        Dictionary<string, string> gtData = repoHelisa.Claves("ShortLinks", _datosCitaSMS.Hor_Pac_Cia);

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

                        /*ShortLink.SDK.Clases.RequestShortLink Rsl = new ShortLink.SDK.Clases.RequestShortLink
                        {
                            CodeUser = Datos.CodeUserSDK,
                            User = Datos.UserSDK,
                            Pass = Datos.PassSDK,
                            UrlOriginal = Mensaje_SMS
                        };*/

                        //ShortLink.SDK.ShortLinks Sl = new ShortLink.SDK.ShortLinks();
                        ShortLink.SDK.EnviarSMS sms = new ShortLink.SDK.EnviarSMS();
                        var res = await sms.EnviarMensaje(s);
                        //ResponseShortLinks res = await Sl.CrearLinkCorto(Rsl);

                        Log(Admision, res, Mensaje_SMS, Celular);

                        MG.Mensaje = "Su mensaje de texto ha sido enviado al numero de celular: " + Celular + " exitosamente";
                        MG.TipoImagen = 2;
                        MG.ShowDialog();
                    }

                    if (Tipo == "Personaliza")
                    {
                        Extras.SMSPersonaliza f = new Extras.SMSPersonaliza(Admision, Mensaje_SMS, Celular, _datosCitaSMS.Hor_Pac_Cia);
                        f.ShowDialog();
                    }
                }
                else
                {
                    MG.Mensaje = "Hay un inconveniente con esta admision, posiblemente no esta en estado Agendado";
                    MG.TipoImagen = 1000;
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

        void J_MED(object sender, EventArgs e)
        {
            try
            {
                if (Adm_Selected == 0)
                {
                    Comunes.MensajesGeneral MG = new Comunes.MensajesGeneral();
                    MG.Mensaje = "No hay cita asignada aqui";
                    MG.TipoImagen = 1000;
                    MG.ShowDialog();
                    return;
                }

                (string TipoId, string IdNum) _idNum = ("","");
               
                
                    _idNum = repositorioFechasAgendaa.getIdbyAdmision(Adm_Selected);
                
                
                if (_idNum.TipoId != "0" && _idNum.IdNum != "0")
                {
                    HistoriasClinicas.Historia_JM_Completar_Create H = new HistoriasClinicas.Historia_JM_Completar_Create(_idNum.IdNum.ToString());
                    H.ShowDialog();
                }
                else
                {
                    Comunes.MensajesGeneral MG = new Comunes.MensajesGeneral();
                    MG.Mensaje = "No hay cita asignada aqui";
                    MG.TipoImagen = 1000;
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
        void Ticket_Print(object sender, EventArgs e)
        {
            try
            {
                PrintTickets pT = new PrintTickets(this.Adm_Selected);
                pT.ShowDialog();
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
        void BloqRange(object sender, EventArgs e)
        {
            try
            {
                string texto = Microsoft.VisualBasic.Interaction.InputBox(
                       "Digite la cantidad de espacios a bloquear: ",
                       "Bloqueo de espacios");

                if (texto != "")
                {
                    int numericValue;

                    bool isNumber = int.TryParse(texto, out numericValue);

                    if (isNumber == false || numericValue == 0)
                    {
                        MensajesGeneral M = new MensajesGeneral();
                        M.Mensaje = "El valor digitado no es valido";
                        M.TipoImagen = 1000;
                        M.ShowDialog();
                        return;
                    }

                    int conteo = 0;

                    foreach (DataGridViewRow row in D.Rows)
                    {
                        conteo++;
                    }

                    if (conteo < Convert.ToInt32(texto))
                    {
                        MensajesGeneral M2 = new MensajesGeneral();
                        M2.Mensaje = "No es posible bloquear mas espacios de los que hay en la posision que ha solicitado";
                        M2.TipoImagen = 1000;
                        M2.ShowDialog();
                        return;
                    }
                    else
                    {
                        Bloqueo(texto.ToString() + " ESPACIOS BLOQUEADOS DESDE RECEPCION", Convert.ToInt32(texto));
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
        void Bloqueo(string Razon, int CantidadEspacios)
        {
            try
            {
                string Observa = "--> Bloqueado por: " + Comunes.Contenedor.UsuarioLogueado + " - " + calZamenis1.txtDia.Text;

                (int CodProf, string TipoBod) DatoProf = (0, "");

                
                    DatoProf = repositorioBodegas.ProfesionalId(comboBox1.Text);
                                

                CXN_HORARIO H = new CXN_HORARIO
                {
                    Hor_Estado = "B",
                    Hor_Observacion = Observa,
                    Hor_Pac_Cup = "",
                    Hor_Pac_Sal = "",
                    Hor_Vales = "",
                    Hor_Pac_Modalidad = "",
                    Hor_Pac_Fecha_Cita = Convert.ToDateTime(calZamenis1.dTPCalendar.Value.Date),
                    Hor_Pac_Id_Hora = this.idIDEHOUR,
                    Hor_Pac_Hora_Cita = Convert.ToDateTime(this.idHOUR), //hora cita
                    Hor_Pac_Bod = IdProf,
                    Hor_Pac_Tipo_Serv = DatoProf.TipoBod,
                    Hor_Pac_Ase = 88,
                    Hor_Imp_Age = Razon.ToString(),
                    Hor_Pac_Cia = IdCom,
                    Hor_Pac_Id = 1,
                    Hor_Pac_UsrGraba = Comunes.Contenedor.UsuarioLogueado,
                    Hor_BloqEspaces = CantidadEspacios,
                    Hor_GrupoServicios = "",
                    Hor_Regimen = ""
                };

                int createBloq = 0;

                
                    createBloq = repositorioHorario.AgendarPaciente(H);
                                

                if (createBloq <= 0)
                {
                    MensajesGeneral MG = new MensajesGeneral();
                    MG.Mensaje = "No se logro bloquear el espacio";
                    MG.TipoImagen = 1000;
                    MG.ShowDialog();
                    return;
                }

                RechargeTrueCheck();
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
        void O_Generated(object sender, EventArgs e)
        {
            try
            {
                O_GeneratedP P = new O_GeneratedP(Adm_Selected);
                P.ShowDialog();
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

        #region //hoja de firmas

        void Firmas_Print(object sender, EventArgs e)
        {
            try
            {
                CXN_CIA DatNombre = new CXN_CIA();

                
                    DatNombre = repositorioCompañias.getPrestadorbyName(comboBox2.Text);
                               
                
                if (DatNombre != null)
                {           
                    List<FirmasR> lista = new List<FirmasR>();

                    FirmasR F = new FirmasR
                    {
                        Com_Nombre = DatNombre.Com_Nombre,
                        Com_Direccion = DatNombre.Com_Direccion,
                        Com_Telefono = DatNombre.Com_Telefono,
                        Com_Logo = DatNombre.Com_Logo
                    };

                    if (this.FirmasAutoComplete == true)
                    {
                        (int Cantidad, string Clase) getImpAuto = (0, "");

                        
                            getImpAuto = repositorioAgendaC.GenerarImprentaAutomatica(Adm_Selected);
                        
                        
                        if (getImpAuto.Cantidad != 0 && getImpAuto.Clase != "")
                        {
                            
                                lista = repositorioReportes.Firmas_Print(Adm_Selected, F, true);
                                                        
                            
                            modelo = new List<FirmasR>();

                            int Cantidad = getImpAuto.Cantidad;
                            int BajaMedia = 1;
                            int Alta = 2;

                            if (getImpAuto.Clase == "A") { Cantidad = Cantidad + Alta; }
                            if (getImpAuto.Clase == "B" || getImpAuto.Clase == "M" || getImpAuto.Clase == "C") { Cantidad = Cantidad + BajaMedia; }

                            for (int i = 0; i <= Cantidad; i++)
                            {
                                foreach (FirmasR G in lista)
                                {
                                    modelo.Add(new FirmasR
                                    {
                                        Com_RIP = i,
                                        PacienteNombre = G.PacienteNombre,
                                        PacienteAseguradora = G.PacienteAseguradora,
                                        PacienteIdentificacion = G.PacienteIdentificacion,
                                        PacienteTelefono = G.PacienteTelefono,
                                        EmpresaNombre = G.EmpresaNombre,
                                        EmpresaDireccion = G.EmpresaDireccion,
                                        EmpresaTelefono = G.EmpresaTelefono,
                                        Logo = G.Logo,
                                        PacienteDireccion = G.PacienteDireccion, //profesional
                                        Com_Email = G.Com_Email, //fase de fibromialgia
                                        Com_Nombre_SMS = G.Com_Nombre_SMS,
                                        Com_Direccion = G.Com_Direccion, //cantidad de sesiones
                                        Com_UsuarioGraba = G.Com_UsuarioGraba, //Curacion 
                                        Com_Resolucion = G.Com_Resolucion //Consulta
                                    });

                                    break;
                                }
                            }

                            if (getImpAuto.Clase == "A")
                            {
                                int operDivDifAlta = getImpAuto.Cantidad / 2;

                                FirmasR item = modelo[0];
                                item.Com_UsuarioGraba = "CONSULTA";
                                item = modelo[operDivDifAlta];
                                item.Com_UsuarioGraba = "CONSULTA";
                                item = modelo[Cantidad];
                                item.Com_UsuarioGraba = "CONSULTA";
                            }
                            else
                            {
                                FirmasR item = modelo[0];
                                item.Com_UsuarioGraba = "CONSULTA";
                                item = modelo[Cantidad];
                                item.Com_UsuarioGraba = "CONSULTA";

                                if (getImpAuto.Cantidad == 1)
                                {
                                    modelo.RemoveAt(modelo.Count - 1);
                                }
                            }

                            if (modelo != null)
                            {
                                Thread thread = new Thread(M2);
                                thread.SetApartmentState(ApartmentState.STA); // Configura el subproceso en STA
                                thread.Start();
                            }
                            else
                            {
                                MessageBox.Show("No se puede imprimir una hoja de firmas en este momento para esta seleccion", "No hay hoja de firmas", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        else
                        {
                            modelo = new List<FirmasR>();
                         
                            
                                modelo = repositorioReportes.Firmas_Print(Adm_Selected, F, false);
                                                                                   

                            if (modelo != null)
                            {
                                Thread thread = new Thread(M);
                                thread.SetApartmentState(ApartmentState.STA); // Configura el subproceso en STA
                                thread.Start();
                            }
                            else
                            {
                                MessageBox.Show("No se puede imprimir una hoja de firmas en este momento para esta seleccion", "No hay hoja de firmas", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                    else
                    {
                        modelo = new List<FirmasR>();

                        
                            modelo = repositorioReportes.Firmas_Print(Adm_Selected, F, false);
                                                

                        if (modelo != null)
                        {
                            Thread thread = new Thread(M);
                            thread.SetApartmentState(ApartmentState.STA); // Configura el subproceso en STA
                            thread.Start();
                        }
                        else
                        {
                            MessageBox.Show("No se puede imprimir una hoja de firmas en este momento para esta seleccion", "No hay hoja de firmas", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
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
        void Firmas_Print2(object sender, EventArgs e)
        {
            try
            {
                CXN_CIA DatNombre =  repositorioCompañias.getPrestadorbyName(comboBox2.Text);            

                if (DatNombre != null)
                {
                    List<FirmasR> lista = new List<FirmasR>();

                    FirmasR F = new FirmasR
                    {
                        Com_Nombre = DatNombre.Com_Nombre,
                        Com_Direccion = DatNombre.Com_Direccion,
                        Com_Telefono = DatNombre.Com_Telefono,
                        Com_Logo = DatNombre.Com_Logo
                    };

                    modelo = new List<FirmasR>();
                    modelo = repositorioReportes.Firmas_Print(Adm_Selected, F, false);
                                        

                    if (modelo != null)
                    {
                        Thread thread = new Thread(M);
                        thread.SetApartmentState(ApartmentState.STA); // Configura el subproceso en STA
                        thread.Start();
                    }
                    else
                    {
                        MessageBox.Show("No se puede imprimir una hoja de firmas en este momento para esta seleccion", "No hay hoja de firmas", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
        void Firmas_Print_TF_Void(object sender, EventArgs e)
        {
            try
            {
                try
                {
                    CXN_CIA DatNombre = new CXN_CIA();

                    
                        DatNombre = repositorioCompañias.getPrestadorbyName(comboBox2.Text);
                    

                    if (DatNombre != null)
                    {
                        string fase = Microsoft.VisualBasic.Interaction.InputBox(
                                "Digite el numero de la fase",
                                "Impresion de Hoja de Firmas",
                                    "");
                        if (fase == "1")
                        {
                            FirmasR F = new FirmasR
                            {
                                Com_Nombre = DatNombre.Com_Nombre,
                                Com_Direccion = DatNombre.Com_Direccion,
                                Com_Telefono = DatNombre.Com_Telefono,
                                Com_Logo = DatNombre.Com_Logo,
                                Com_Email = fase.ToString(),
                                Com_Nombre_SMS = DatNombre.Com_Nombre_SMS
                            };

                            modelo = new List<FirmasR>();

                            
                                modelo = repositorioReportes.Firmas_Print(Adm_Selected, F, false);
                            
                            
                            if (modelo != null)
                            {
                                Thread thread = new Thread(M_TF);
                                thread.SetApartmentState(ApartmentState.STA);
                                thread.Start();
                            }
                            else
                            {
                                MessageBox.Show("No se puede imprimir una hoja de firmas en este momento para esta seleccion", "No hay hoja de firmas", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        else if (fase == "2")
                        {
                            FirmasR F = new FirmasR
                            {
                                Com_Nombre = DatNombre.Com_Nombre,
                                Com_Direccion = DatNombre.Com_Direccion,
                                Com_Telefono = DatNombre.Com_Telefono,
                                Com_Logo = DatNombre.Com_Logo,
                                Com_Email = fase.ToString(),
                                Com_Nombre_SMS = DatNombre.Com_Nombre_SMS
                            };

                            modelo = new List<FirmasR>();
                            
                            
                                modelo = repositorioReportes.Firmas_Print(Adm_Selected, F, false);
                            
                            
                            if (modelo != null)
                            {
                                Thread thread = new Thread(M_TF2);
                                thread.SetApartmentState(ApartmentState.STA);
                                thread.Start();
                            }
                            else
                            {
                                MessageBox.Show("No se puede imprimir una hoja de firmas en este momento para esta seleccion", "No hay hoja de firmas", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        else
                        {
                            MensajesGeneral MG = new MensajesGeneral();
                            MG.TipoImagen = 1000;
                            MG.Mensaje = "El valor digitado es incorrecto, solo puede ser 1 o 2";
                            MG.ShowDialog();
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
        void Firmas_Print_TO_Void(object sender, EventArgs e)
        {
            try
            {
                CXN_CIA DatNombre = new CXN_CIA();

                
                    DatNombre = repositorioCompañias.getPrestadorbyName(comboBox2.Text);
                

                if (DatNombre != null)
                {
                    string fase = Microsoft.VisualBasic.Interaction.InputBox(
                            "Digite el numero de la fase",
                            "Impresion de Hoja de Firmas",
                                "");
                    if (fase == "1")
                    {
                        FirmasR F = new FirmasR
                        {
                            Com_Nombre = DatNombre.Com_Nombre,
                            Com_Direccion = DatNombre.Com_Direccion,
                            Com_Telefono = DatNombre.Com_Telefono,
                            Com_Logo = DatNombre.Com_Logo,
                            Com_Email = fase.ToString(),
                            Com_Nombre_SMS = DatNombre.Com_Nombre_SMS
                        };

                        modelo = new List<FirmasR>();

                        
                            modelo = repositorioReportes.Firmas_Print(Adm_Selected, F, false);
                        
                        
                        if (modelo != null)
                        {
                            Thread thread = new Thread(M_TO);
                            thread.SetApartmentState(ApartmentState.STA);
                            thread.Start();
                        }
                        else
                        {
                            MessageBox.Show("No se puede imprimir una hoja de firmas en este momento para esta seleccion", "No hay hoja de firmas", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else if (fase == "2")
                    {
                        FirmasR F = new FirmasR
                        {
                            Com_Nombre = DatNombre.Com_Nombre,
                            Com_Direccion = DatNombre.Com_Direccion,
                            Com_Telefono = DatNombre.Com_Telefono,
                            Com_Logo = DatNombre.Com_Logo,
                            Com_Email = fase.ToString(),
                            Com_Nombre_SMS = DatNombre.Com_Nombre_SMS
                        };

                        modelo = new List<FirmasR>();
                        
                        
                            modelo = repositorioReportes.Firmas_Print(Adm_Selected, F, false);
                        
                        
                        if (modelo != null)
                        {
                            Thread thread = new Thread(M_TO2);
                            thread.SetApartmentState(ApartmentState.STA);
                            thread.Start();
                        }
                        else
                        {
                            MessageBox.Show("No se puede imprimir una hoja de firmas en este momento para esta seleccion", "No hay hoja de firmas", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        MensajesGeneral MG = new MensajesGeneral();
                        MG.TipoImagen = 1000;
                        MG.Mensaje = "El valor digitado es incorrecto, solo puede ser 1 o 2";
                        MG.ShowDialog();
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
        void Firmas_Print_PS_Void(object sender, EventArgs e)
        {
            try
            {
                CXN_CIA DatNombre = new CXN_CIA();

                
                    DatNombre = repositorioCompañias.getPrestadorbyName(comboBox2.Text);
                
                
                if (DatNombre != null)
                {
                    string fase = Microsoft.VisualBasic.Interaction.InputBox(
                            "Digite el numero de la fase",
                            "Impresion de Hoja de Firmas",
                                "");
                    if (fase == "1")
                    {
                        FirmasR F = new FirmasR
                        {
                            Com_Nombre = DatNombre.Com_Nombre,
                            Com_Direccion = DatNombre.Com_Direccion,
                            Com_Telefono = DatNombre.Com_Telefono,
                            Com_Logo = DatNombre.Com_Logo,
                            Com_Email = fase.ToString(),
                            Com_Nombre_SMS = DatNombre.Com_Nombre_SMS
                        };

                        modelo = new List<FirmasR>();
                        
                        
                            modelo = repositorioReportes.Firmas_Print(Adm_Selected, F, false);
                        
                                                
                        if (modelo != null)
                        {
                            Thread thread = new Thread(M_PS);
                            thread.SetApartmentState(ApartmentState.STA);
                            thread.Start();
                        }
                        else
                        {
                            MessageBox.Show("No se puede imprimir una hoja de firmas en este momento para esta seleccion", "No hay hoja de firmas", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else if (fase == "2")
                    {
                        FirmasR F = new FirmasR
                        {
                            Com_Nombre = DatNombre.Com_Nombre,
                            Com_Direccion = DatNombre.Com_Direccion,
                            Com_Telefono = DatNombre.Com_Telefono,
                            Com_Logo = DatNombre.Com_Logo,
                            Com_Email = fase.ToString(),
                            Com_Nombre_SMS = DatNombre.Com_Nombre_SMS
                        };

                        modelo = new List<FirmasR>();
                        
                        
                            modelo = repositorioReportes.Firmas_Print(Adm_Selected, F, false);
                        
                        
                        if (modelo != null)
                        {
                            Thread thread = new Thread(M_PS2);
                            thread.SetApartmentState(ApartmentState.STA);
                            thread.Start();
                        }
                        else
                        {
                            MessageBox.Show("No se puede imprimir una hoja de firmas en este momento para esta seleccion", "No hay hoja de firmas", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        MensajesGeneral MG = new MensajesGeneral();
                        MG.TipoImagen = 1000;
                        MG.Mensaje = "El valor digitado es incorrecto, solo puede ser 1 o 2";
                        MG.ShowDialog();
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
        void M()
        {
            try
            {
                ConfigForm.GenerarReportViewer("DataSet_Firmas",
               "ZamenisHealth.Reportes.Firmas.rdlc",
               modelo);
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
        void M2()
        {
            try
            {
                ConfigForm.GenerarReportViewer("DataSet_Firmas",
               "ZamenisHealth.Reportes.Firmas2AutoComplete.rdlc",
               modelo);

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
        void M_PS()
        {
            try
            {
                ConfigForm.GenerarReportViewer("DataSet_Firmas",
               "ZamenisHealth.Reportes.RDLC_Firmas_PS.rdlc",
               modelo);

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
        void M_PS2()
        {
            try
            {
                ConfigForm.GenerarReportViewer("DataSet_Firmas",
               "ZamenisHealth.Reportes.RDLC_Firmas_PS2.rdlc",
               modelo);

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
        void M_TF()
        {
            try
            {
                ConfigForm.GenerarReportViewer("DataSet_Firmas",
               "ZamenisHealth.Reportes.RDLC_Firmas_TF.rdlc",
               modelo);

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
        void M_TF2()
        {
            try
            {
                ConfigForm.GenerarReportViewer("DataSet_Firmas",
               "ZamenisHealth.Reportes.RDLC_Firmas_TF2.rdlc",
               modelo);

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
        void M_TO()
        {
            try
            {
                ConfigForm.GenerarReportViewer("DataSet_Firmas",
               "ZamenisHealth.Reportes.RDLC_Firmas_TO.rdlc",
               modelo);

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
        void M_TO2()
        {
            try
            {
                ConfigForm.GenerarReportViewer("DataSet_Firmas",
               "ZamenisHealth.Reportes.RDLC_Firmas_TO2.rdlc",
               modelo);

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

        #endregion //Hoja de Firmas

        void E_SMS(object sender, EventArgs e)
        {
            //cambiar texto SMS
            try
            {
                if (Adm_Selected == 0)
                {
                    Comunes.MensajesGeneral MG = new Comunes.MensajesGeneral();
                    MG.Mensaje = "No hay admision aqui disponible";
                    MG.TipoImagen = 1000;
                    MG.ShowDialog();

                    return;
                }
                SMS(Adm_Selected, "Personaliza");
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
        void MoverAgendas(object sender, EventArgs e)
        {
            try
            {
                if (Adm_Selected == 0)
                {
                    Comunes.MensajesGeneral MG = new Comunes.MensajesGeneral();
                    MG.Mensaje = "No hay cita asignada aqui";
                    MG.TipoImagen = 1000;
                    MG.ShowDialog();
                    return;
                }

                Extras.MoverAgenda MA = new Extras.MoverAgenda(Adm_Selected);
                MA.ShowDialog();
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
        void RCitas(object sender, EventArgs e)
        {
            try
            {
                if (Adm_Selected == 0)
                {
                    Comunes.MensajesGeneral MG = new Comunes.MensajesGeneral();
                    MG.Mensaje = "No hay cita asignada aqui";
                    MG.TipoImagen = 1000;
                    MG.ShowDialog();
                    return;
                }

                Extras.CitasMailPDF F = new Extras.CitasMailPDF(Adm_Selected);
                F.ShowDialog();
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
        void CopyCC(object sender, EventArgs e)
        {
            try
            {
                Thread thread = new Thread(CopyDocument);
                thread.SetApartmentState(ApartmentState.STA); // Configura el subproceso en STA
                thread.Start();
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
        void CopyDocument()
        {
            try
            {
                if (Adm_Selected == 0)
                {
                    Comunes.MensajesGeneral MG = new Comunes.MensajesGeneral();
                    MG.Mensaje = "No hay cita asignada aqui";
                    MG.TipoImagen = 1000;
                    MG.ShowDialog();
                    return;
                }

                (string TipoId, string IdNum) _idNum = ("", "");

                
                    _idNum = repositorioFechasAgendaa.getIdbyAdmision(Adm_Selected);
                

                if (_idNum.TipoId != "0" && _idNum.IdNum != "0")
                {
                    Clipboard.SetText(_idNum.IdNum.ToString());
                }
                else
                {
                    Comunes.MensajesGeneral MG = new Comunes.MensajesGeneral();
                    MG.Mensaje = "No se logro copiar";
                    MG.TipoImagen = 1000;
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
        void CopyName(object sender, EventArgs e)
        {
            try
            {
                Thread thread = new Thread(CopyName);
                thread.SetApartmentState(ApartmentState.STA); // Configura el subproceso en STA
                thread.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show("CopyName ARgs: " + ex.Message);
                Comunes.MensajesGeneral MG = new Comunes.MensajesGeneral();
                MG.Mensaje = "No se logro copiar";
                MG.TipoImagen = 1000;
                MG.ShowDialog();
            }
        }
        void CopyName()
        {
            try
            {
                if (Adm_Selected == 0)
                {
                    Comunes.MensajesGeneral MG = new Comunes.MensajesGeneral();
                    MG.Mensaje = "No hay cita asignada aqui";
                    MG.TipoImagen = 1000;
                    MG.ShowDialog();
                    return;
                }

                Clipboard.SetText(name_Selected.ToString());
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

                Comunes.MensajesGeneral MG = new Comunes.MensajesGeneral();
                MG.Mensaje = "No se logro copiar";
                MG.TipoImagen = 1000;
                MG.ShowDialog();
            }
        }
        void A_Admi(object sender, EventArgs e)
        {
            try
            {
                otrosDatosPacienteHorario DatosAdmision = new otrosDatosPacienteHorario();

                Comunes.MensajesGeneral MG = new Comunes.MensajesGeneral();
                if (Adm_Selected == 0)
                {
                    MG.Mensaje = "No hay admision aqui disponible";
                    MG.TipoImagen = 1000;
                    MG.ShowDialog();
                    return;
                }

                string _estadoAdmision =  repositorioHorario.consularAdmisionEstado(Adm_Selected);

                if (_estadoAdmision != "")
                {
                    if (_estadoAdmision == "A")
                    {
                        MG.Mensaje = "Esta admision no se puede anular porque nunca se admisiono";
                        MG.TipoImagen = 1000;
                        MG.ShowDialog();
                        return;
                    }
                    if (_estadoAdmision == "C")
                    {
                        MG.Mensaje = "Esta admision no se puede anular porque esta cancelada";
                        MG.TipoImagen = 1000;
                        MG.ShowDialog();
                        return;
                    }
                    if (_estadoAdmision == "H")
                    {
                        MG.Mensaje = "Esta admision no se puede anular porque ya cuenta con una historia hecha";
                        MG.TipoImagen = 1000;
                        MG.ShowDialog();
                        return;
                    }
                    if (_estadoAdmision == "P")
                    {
                        bool isOpenedAdmition = repositorioHorario.OpenAdmition(Adm_Selected);

                        if (isOpenedAdmition == true)
                        {
                            DatosAdmision = repositorioAgendaC.cargarAdmision(Adm_Selected, "'P','H','A'");

                            if (DatosAdmision != null)
                            {
                                repositorioOrdenes.consumirAutorizacion(DatosAdmision.Hor_Pac_Id, DatosAdmision.Hor_Autoriza, "N");
                            }

                            if (string.IsNullOrEmpty(DatosAdmision.Hor_DocFEModeradorCUFE))
                            {
                                repositorioHorario.anularAdmision(Adm_Selected, Contenedor.UsuarioLogueado);
                            }
                            else
                            {
                                MessageBox.Show("La admision se anulo exitosamente pero el recibo de caja no se ha anulado ya que este cuenta con una factura electronica asociada", 
                                    "Hecho con novedades", 
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            
                            repositorioRcCaja.anularRcCaja(Adm_Selected);
                            fDigitales.EliminarFirma(Adm_Selected);

                            calZamenis1.IdProf = this.IdProf;
                            calZamenis1.Max15 = this.Max15;
                            calZamenis1.Cargar();
                            fecha();

                            MG.Mensaje = "Admision y firma anuladas correctamente";
                            MG.TipoImagen = 3;
                            MG.ShowDialog();
                        }
                        else
                        {
                            MG.Mensaje = "No es posible anular esta admision debido a que el profesional ya abrio el historial, solicite al profesional que cancele " +
                                "la historia e intente nuevamente anular esta admision";
                            MG.TipoImagen = 1000;
                            MG.ShowDialog();
                        }
                    }
                }
                else
                {
                    MG.Mensaje = "El numero de admision digitado no existe";
                    MG.TipoImagen = 1000;
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
        void C_Asist(object sender, EventArgs e)
        {
            try
            {
                Comunes.MensajesGeneral MG = new Comunes.MensajesGeneral();
                if (Adm_Selected == 0)
                {
                    MG.Mensaje = "No hay admision aqui disponible";
                    MG.TipoImagen = 1000;
                    MG.ShowDialog();
                    return;
                }

                Extras.CAsistencia C = new Extras.CAsistencia(Adm_Selected);
                C.ApartmentState = ApartmentState.STA;
                C.ShowDialog();
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
        void RC_Print(object sender, EventArgs e)
        {
            try
            {
                Comunes.MensajesGeneral MG = new Comunes.MensajesGeneral();
                if (Adm_Selected == 0)
                {
                    MG.Mensaje = "No hay admision aqui disponible";
                    MG.TipoImagen = 1000;
                    MG.ShowDialog();
                    return;
                }

                DialogResult result = MessageBox.Show("Seleccione SI para imprrimir recibo de caja termico, seleccione NO para imprimir recibo de caja tamaño carta",
                                                "Zamenis Health - Impresion de Recibos",
                                                MessageBoxButtons.YesNo,
                                                MessageBoxIcon.Question);

                int getLastad = repositorioRcCaja.getLastAdmition(Adm_Selected);
                if (getLastad > 0)
                {
                    Exporta = repositorioRcCaja.ReciboRpt(getLastad);

                    if (result == DialogResult.Yes)
                    {
                        if (Exporta != null)
                        {
                            Thread thread = new Thread(MCaja2);
                            thread.SetApartmentState(ApartmentState.STA); // Configura el subproceso en STA
                            thread.Start();
                        }
                        else
                        {
                            MG.Mensaje = "Hubo un inconveniente con este recibo, ingrese por copias recepcion o consulte el administrador del sistema";
                            MG.TipoImagen = 1000;
                            MG.ShowDialog();
                        }
                    }

                    if (result == DialogResult.No)
                    {
                        if (Exporta != null)
                        {
                            Thread thread = new Thread(MCaja);
                            thread.SetApartmentState(ApartmentState.STA); // Configura el subproceso en STA
                            thread.Start();
                        }
                        else
                        {
                            MG.Mensaje = "Hubo un inconveniente con este recibo, ingrese por copias recepcion o consulte el administrador del sistema";
                            MG.TipoImagen = 1000;
                            MG.ShowDialog();
                        }
                    }
                }    
                else
                {
                    MG.Mensaje = "Hubo un inconveniente con este recibo, ingrese por copias recepcion o consulte el administrador del sistema";
                    MG.TipoImagen = 1000;
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
        void MCaja2()
        {
            try
            {
                ConfigForm.GenerarReportViewer("ReciboCajaDataset",
               "ZamenisHealth.Reportes.RDLC_RcCajaImpTermica.rdlc",
               Exporta);

            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        void MCaja()
        {
            try
            {
                ConfigForm.GenerarReportViewer("ReciboCajaDataset",
               "ZamenisHealth.Reportes.RDLC_RcCaja.rdlc",
               Exporta);

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
        void A_PRE(object sender, EventArgs e)
        {
            try
            {
                Comunes.MensajesGeneral MG = new Comunes.MensajesGeneral();
                if (Adm_Selected == 0)
                {
                    MG.Mensaje = "No hay admision aqui disponible";
                    MG.TipoImagen = 1000;
                    MG.ShowDialog();
                    return;
                }

                (string TipoId, string IdNum) _idNum = ("", "");

                
                    _idNum = repositorioFechasAgendaa.getIdbyAdmision(Adm_Selected);
                

                if (_idNum.IdNum != "0" && _idNum.TipoId != "0")
                {
                    Asistencia f = new ZamenisHealth.Recepcion.Asistencia( _idNum.IdNum )
                    {
                        StartPosition = FormStartPosition.CenterScreen,
                        FormBorderStyle = FormBorderStyle.FixedSingle,
                        AutoScroll = false                        
                    };

                    f.ShowDialog();
                }
                else
                {
                    MG.Mensaje = "No hay admision aqui disponible";
                    MG.TipoImagen = 1000;
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
        private void toolStripLabel3_Click(object sender, EventArgs e)
        {
            Asistencia f = new Asistencia
            {
                StartPosition = FormStartPosition.CenterScreen,
                FormBorderStyle = FormBorderStyle.FixedSingle,
                AutoScroll = false
            };
            f.ShowDialog();
        }
        private void toolStripLabel4_Click(object sender, EventArgs e)
        {
            Extras.PrintAgendas printAgendas = new Extras.PrintAgendas();
            printAgendas.ShowDialog();
        }
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            Ventas.BuscarPaciente P = new Ventas.BuscarPaciente();
            P.ShowDialog();
        }
        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            Comunes.MensajeroSend M = new Comunes.MensajeroSend();
            M.ShowDialog();
        }
        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            ReportesRec reportesRec = new ReportesRec();
            reportesRec.ShowDialog();
        }
        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            Extras.BloqueoDias bloqueoDias = new Extras.BloqueoDias();
            bloqueoDias.ShowDialog();
        }       
        private void label22_Click(object sender, EventArgs e)
        {
            Extras.OrdenesPendientes P = new Extras.OrdenesPendientes();
            P.ShowDialog();
        }        
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Cierra_Agenda();
        }
        private void toolStripButton2_Click_1(object sender, EventArgs e)
        {
            Extras.O_GeneratedP O = new Extras.O_GeneratedP(0);
            O.listView1.Size = new Size(956, 344);
            O.listView1.Location = new Point(12, 116);
            O.ShowDialog();
        }
        private void textBox1_Leave(object sender, EventArgs e)
        {
            label52.BackColor = Color.RoyalBlue;
            label52.Text = "Agenda Medica - Zamenis Health";
        }
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.listaSaleConsulta != null)
                {
                    listaSaleConsulta.Clear();
                }

                DateTime hoy = DateTime.Now.Date;
                listaSaleConsulta = new Dictionary<int, string>();
                
                
                    listaSaleConsulta = repositorioHorario.getSaleConsultas(Convert.ToDateTime(hoy), "L");
                                

                string pacSalieron = "";

                if (listaSaleConsulta != null)
                {
                    foreach (string pacSalido in this.listaSaleConsulta.Values)
                    {
                        pacSalieron = pacSalieron + pacSalido.ToString() + "\n\r";
                    }

                    TimerSalidas.Stop();

                    MensajesGeneral MG = new MensajesGeneral();
                    MG.Mensaje = pacSalieron;
                    MG.TipoImagen = 0;
                    MG.ShowDialog();

                    TimerSalidas.Start();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void comboBox1_Leave(object sender, EventArgs e)
        {
            try
            {
                if (this.accionarCombos == true)
                {
                    CXN_BODEGAS getBod = new CXN_BODEGAS();

                    
                        getBod = repositorioBodegas.getDatosName(comboBox1.Text);
                    

                    this.IdProf = getBod.Bod_Numero;
                    this.tipoBod = getBod.Bod_Tipo;

                    calZamenis1.IdProf = this.IdProf;
                    calZamenis1.Max15 = this.Max15;
                    calZamenis1.diasBloq = this.diasBloq;

                    
                        calZamenis1.calendarFestivo = repositorioAgendaC.getCalendario();
                        calZamenis1.getListaBloqueos = repositorioAgendaC.CargarListBlocked(this.IdProf);
                        calZamenis1.getlistasLunes = repositorioDisponibilidad.getHorariosHabilitados(this.IdProf, "Lunes");
                        calZamenis1.getlistasMartes = repositorioDisponibilidad.getHorariosHabilitados(this.IdProf, "Martes");
                        calZamenis1.getlistasMiercoles = repositorioDisponibilidad.getHorariosHabilitados(this.IdProf, "Miércoles");
                        calZamenis1.getlistasJueves = repositorioDisponibilidad.getHorariosHabilitados(this.IdProf, "Jueves");
                        calZamenis1.getlistasViernes = repositorioDisponibilidad.getHorariosHabilitados(this.IdProf, "Viernes");
                        calZamenis1.getlistasSabado = repositorioDisponibilidad.getHorariosHabilitados(this.IdProf, "Sábado");
                        calZamenis1.getlistasDomingo = repositorioDisponibilidad.getHorariosHabilitados(this.IdProf, "Domingo");

                        calZamenis1.getCantDiaLunes = repositorioAgendaC.getOcupados(Convert.ToDateTime(DateTime.Now.Date), this.IdProf);
                        calZamenis1.getCantDiaMartes = repositorioAgendaC.getOcupados(Convert.ToDateTime(DateTime.Now.Date), this.IdProf);
                        calZamenis1.getCantDiaMiercoles = repositorioAgendaC.getOcupados(Convert.ToDateTime(DateTime.Now.Date), this.IdProf);
                        calZamenis1.getCantDiaJueves = repositorioAgendaC.getOcupados(Convert.ToDateTime(DateTime.Now.Date), this.IdProf);
                        calZamenis1.getCantDiaViernes = repositorioAgendaC.getOcupados(Convert.ToDateTime(DateTime.Now.Date), this.IdProf);
                        calZamenis1.getCantDiaSabado = repositorioAgendaC.getOcupados(Convert.ToDateTime(DateTime.Now.Date), this.IdProf);
                        calZamenis1.getCantDiaDomingo = repositorioAgendaC.getOcupados(Convert.ToDateTime(DateTime.Now.Date), this.IdProf);
                    

                    calZamenis1.Cargar();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void comboBox2_SelectedValueChanged(object sender, EventArgs e)
        {
            richTextBox1.Focus();
        }
        private void comboBox2_Leave(object sender, EventArgs e)
        {
            try
            {
                if (this.accionarCombos == true)
                {
                    CXN_CIA C = new CXN_CIA();

                    
                        repositorioCompañias.getPrestadorbyName(comboBox2.Text);
                    

                    this.IdCom = C.Com_Identificador;

                    calZamenis1.IdProf = this.IdProf;
                    calZamenis1.Max15 = this.Max15;
                    calZamenis1.diasBloq = this.diasBloq;

                    
                        calZamenis1.calendarFestivo = repositorioAgendaC.getCalendario();
                        calZamenis1.getListaBloqueos = repositorioAgendaC.CargarListBlocked(this.IdProf);
                        calZamenis1.getlistasLunes = repositorioDisponibilidad.getHorariosHabilitados(this.IdProf, "Lunes");
                        calZamenis1.getlistasMartes = repositorioDisponibilidad.getHorariosHabilitados(this.IdProf, "Martes");
                        calZamenis1.getlistasMiercoles = repositorioDisponibilidad.getHorariosHabilitados(this.IdProf, "Miércoles");
                        calZamenis1.getlistasJueves = repositorioDisponibilidad.getHorariosHabilitados(this.IdProf, "Jueves");
                        calZamenis1.getlistasViernes = repositorioDisponibilidad.getHorariosHabilitados(this.IdProf, "Viernes");
                        calZamenis1.getlistasSabado = repositorioDisponibilidad.getHorariosHabilitados(this.IdProf, "Sábado");
                        calZamenis1.getlistasDomingo = repositorioDisponibilidad.getHorariosHabilitados(this.IdProf, "Domingo");

                        calZamenis1.getCantDiaLunes = repositorioAgendaC.getOcupados(Convert.ToDateTime(DateTime.Now.Date), this.IdProf);
                        calZamenis1.getCantDiaMartes = repositorioAgendaC.getOcupados(Convert.ToDateTime(DateTime.Now.Date), this.IdProf);
                        calZamenis1.getCantDiaMiercoles = repositorioAgendaC.getOcupados(Convert.ToDateTime(DateTime.Now.Date), this.IdProf);
                        calZamenis1.getCantDiaJueves = repositorioAgendaC.getOcupados(Convert.ToDateTime(DateTime.Now.Date), this.IdProf);
                        calZamenis1.getCantDiaViernes = repositorioAgendaC.getOcupados(Convert.ToDateTime(DateTime.Now.Date), this.IdProf);
                        calZamenis1.getCantDiaSabado = repositorioAgendaC.getOcupados(Convert.ToDateTime(DateTime.Now.Date), this.IdProf);
                        calZamenis1.getCantDiaDomingo = repositorioAgendaC.getOcupados(Convert.ToDateTime(DateTime.Now.Date), this.IdProf);
                    

                    calZamenis1.Cargar();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void label9_MouseMove(object sender, MouseEventArgs e)
        {
            calZamenis1.Visible = true;
        }   
        private void label9_MouseMove(object sender, EventArgs e)
        {
            calZamenis1.Visible = true;
        }
        private void label9_Click(object sender, EventArgs e)
        {
            calZamenis1.Visible = true;
        }
        private void textoAgendaDia_Click(object sender, EventArgs e)
        {
            calZamenis1.Visible = true;
        }
        private void panel3_MouseDown(object sender, MouseEventArgs e)
        {
            ConfigForm.ReleaseCapturing();
            ConfigForm.SendMessageMove(this.Handle, 0x112, 0xf012, 0);
        }
        private void toolStripButton7_Click(object sender, EventArgs e)
        {            
            planos.RNV(Convert.ToDateTime(textoAgendaFecha.Text), Convert.ToDateTime(textoAgendaFecha.Text));
        }
        private void comboBox1_SelectedValueChanged(object sender, EventArgs e)
        {
            richTextBox1.Focus();
        }               
        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                MensajesGeneral MG = new MensajesGeneral();

                if (e.KeyData == Keys.Enter)
                {
                    if (textBox1.Enabled == true)
                    {
                        if (this.TipoConsultaBarras == 1)
                        {
                            int _admTem = Convert.ToInt32(textBox1.Text);
                            string Estado = "";
                            
                            
                                Estado = repositorioHorario.consularAdmisionEstado(_admTem);
                                                        

                            if (Estado != "0")
                            {
                                if (Estado == "A")
                                {
                                    ConsultaAdmision f = new ConsultaAdmision(_admTem.ToString());
                                    f.ShowDialog();
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
                                    MG.TipoImagen = 0;
                                    MG.Mensaje = "El estado de esta admision es CITA CANCELADA";
                                    MG.ShowDialog();
                                }
                                else
                                {
                                    MG.TipoImagen = 1000;
                                    MG.Mensaje = "El estado de esta admision no permite su consumo";
                                    MG.ShowDialog();
                                }
                            }
                            else
                            {
                                MG.TipoImagen = 1000;
                                MG.Mensaje = "No hay resultados para esta admision";
                                MG.ShowDialog();
                            }
                        }
                        else if (this.TipoConsultaBarras == 2)
                        {
                            Busqueda B = new Busqueda(calZamenis1.dTPCalendar.Value, textBox1.Text);
                            B.ShowDialog();
                        }
                        else
                        {
                            richTextBox1.Focus();
                        }                        

                        richTextBox1.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void TimerSalidas_Tick(object sender, EventArgs e)
        {
            try
            {
                if (this.listaSaleConsulta != null)
                {
                    listaSaleConsulta.Clear();
                }

                DateTime hoy = DateTime.Now.Date;
                listaSaleConsulta = new Dictionary<int, string>();

                
                    listaSaleConsulta = repositorioHorario.getSaleConsultas(Convert.ToDateTime(hoy), "A");
                
              
                string pacSalieron = "";

                if (listaSaleConsulta != null)
                {
                    foreach (string pacSalido in this.listaSaleConsulta.Values)
                    {
                        pacSalieron = pacSalieron + pacSalido.ToString() + "\n\r";
                    }

                    TimerSalidas.Stop();

                    MensajesGeneral MG = new MensajesGeneral();
                    MG.Mensaje = "Los siguientes pacientes ya han salido de consulta con el medico: \n\r \n\r" +
                        pacSalieron;
                    MG.TipoImagen = 0;
                    MG.ShowDialog();

                    foreach (int pacSalido in this.listaSaleConsulta.Keys)
                    {
                        
                            repositorioHorario.updateSALIERONCONSULTA(pacSalido);
                                                
                    }

                    TimerSalidas.Start();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        } 
    }
}
