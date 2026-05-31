using Domain;
using Domain.CXN;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tulpep.NotificationWindow;
using ZamenisHealth.Clases;
using ZamenisHealth.Comunes;
using ZamenisHealth.Recepcion.Extras;

namespace ZamenisHealth.Recepcion
{
    public partial class AgendarCita : ConfigForm.BaseForm
    {
        private static readonly IConvenios repositorioConvenios = new MConvenios();
        private static readonly IAseguradoras repositorioAseguradoras = new MAseguradoras();
        private static readonly IBodegas repositorioBodegas = new MBodegas();
        private static readonly IPacientes repositorioPacientes = new MPacientes();
        private static readonly IAgendaC repositorioHorario = new MAgendaC();
        private static readonly IAgenda repositorioAgendar = new MAgenda();
        private static readonly IRIPS_Res2275_2023 IRIPS = new MRIPS_Res2275_2023();
        private static readonly IConfSystem repoConf = new MConfSystem();
        private static readonly ITipificador tipificador = new MTipificador();

        private int CodeProf;
        private string DiaNombre;
        private DateTime HoraTexto;
        private string TipoBod;
        private string IdHora;
        private DateTime FechaCita;
        private string HabilitaEspacio;
        private int CodePrest;

        private int ASESESIONS;
        private string TSERV;
        private DataTable dt;

        public static string Tipo_Serv = "";
        private MensajesGeneral MG;

        SpeechRecognitionEngine oSpeechRecognitionEngine = null;
        System.Speech.Synthesis.SpeechSynthesizer oSpeechSynthesizer = null;

        int Pac_Id;
        bool Nuevo;
        DateTime Hoy = DateTime.Now.Date;
        private int xPos = 0;

        MensajesGeneral MGmen;
        ToolTip toolTipServicio = new ToolTip();

        public AgendarCita(int _codeprof,
                           string _dianombre,
                           DateTime _horatexto,
                           string _tipobod,
                           string _idhora,
                           int _codprest,
                           DateTime _fechacita,
                           string _habilitar)
        {
            InitializeComponent();
            this.CodeProf = _codeprof;
            this.DiaNombre = _dianombre;
            this.HoraTexto = _horatexto;
            this.TipoBod = _tipobod;
            this.IdHora = _idhora;
            this.FechaCita = _fechacita;
            this.HabilitaEspacio = _habilitar;
            this.CodePrest = _codprest;

            ConfigForm.MoverForma(label52, this);
        }

        private void CargarDocumentos()
        {
            List<string> ListaDocs = repositorioPacientes.ListaDocs();
            
            if (ListaDocs != null)
            {
                foreach (var i in ListaDocs)
                {
                    comboBox1.Items.Add(i);
                }

                foreach (var i2 in ListaDocs)
                {
                    comboBox2.Items.Add(i2);
                }
            }
        }

        private void CargarRegimen()
        {
            List<string> ListaRegimen = repositorioPacientes.ListaRegimen();

            if (ListaRegimen != null) 
            {
                comboBox4.Items.Clear();

                foreach (string r in ListaRegimen)
                {
                    comboBox4.Items.Add(r);
                }
            }            
        }

        private void CargarServicio(int Ase)
        {
            comboBox5.DataSource = null;
            comboBox5.Items.Clear();

            List<string> ListaServicios =repositorioConvenios.CargarServicios(TipoBod, Ase);

            if (ListaServicios != null)
            {
                comboBox5.Items.Clear();

                foreach (var i in ListaServicios)
                {
                    comboBox5.Items.Add(i.ToString());
                }

                comboBox5.SelectedIndex = 0;
            }
        }

        private void AgendarCita_Load(object sender, EventArgs e)
        {
            try
            {
                this.Titulo.Visible = false;
                this.ImageClose.Visible = false;
                ConfigForm.SoloNumeros(textBox7);

                this.Size = new Size(944, 590);               

                ToolTip toolTip1 = new ToolTip();
                toolTip1.ShowAlways = true;
                toolTip1.SetToolTip(checkBox6, "Marque esta opcion en caso de que el paciente deba anexar vales de pago, bonos, efectivo o algun " +
                    "tipo de pago, si el usuario no paga; no marque esta opcion");
                toolTipServicio.ShowAlways = true;

                textBox7.MaxLength = 10;
                
                CargarDocumentos();
                CargarRegimen();

                if (this.TipoBod == "CU")
                {
                    checkBox7.Visible = true;
                }
                else
                {
                    checkBox7.Visible = false;
                }

                List<CXN_ASEGURADORA> CargarAse =  repositorioAseguradoras.getAseguradoras();
                
                if (CargarAse != null)
                {
                    comboBox3.Items.Clear();

                    foreach (var i in CargarAse)
                    {
                        comboBox3.Items.Add(i.Ase_Descripcion.ToString());
                    }

                    comboBox3.SelectedIndex = 0;
                }

                string ProfNombre =  repositorioBodegas.ProfesionalNombre(CodeProf);

                label4.Text = ProfNombre.ToString();
                label5.Text = this.HoraTexto.ToString("hh:mm tt");
                label6.Text = this.DiaNombre.ToString();
                label27.Text = Convert.ToDateTime(FechaCita).ToString(Conexion.ConectionDictionary["Format_Fecha"]);

                comboBox7.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void textBox1_DoubleClick(object sender, EventArgs e)
        {
            Comunes.BuscarPacientes buscarPacientes = new Comunes.BuscarPacientes();
            buscarPacientes.Tipo_Busca_Pac = "AgendarCita";
            buscarPacientes.ShowDialog();
        }

        private void Limpiar()
        {
            textBox3.Text = "";
            textBox4.Text = "";
            textBox5.Text = "";
            textBox6.Text = "";
            textBox7.Text = "";
            textBox8.Text = "";
            comboBox2.Text = "";
            textBox10.Text = "";
            dateTimePicker2.Value = Hoy;
            textBox11.Text = "";
            textBox9.Text = "";
            Pac_Id = 0;
        }

        private void Busca_Pac()
        {
            try
            {
                Comunes.MensajesGeneral MG1 = new Comunes.MensajesGeneral();
                Limpiar();

                CXN_PACIENTES Paciente =  repositorioPacientes.LlamarPacienteDOC(comboBox1.Text, textBox1.Text.Trim());
                
                if (Paciente == null)
                {
                    CXN_PACIENTES PacienteDoc =  repositorioPacientes.LlamarPacienteNumDoc(textBox1.Text.Trim());

                    if (PacienteDoc == null)
                    {
                        MG1.Mensaje = "El tipo y numero de documento digitados no existe";
                        MG1.TipoImagen = 1000;
                        MG1.ShowDialog();

                        /* CrearEditarPaciente crearEditarPaciente = new CrearEditarPaciente();
                         crearEditarPaciente.ShowDialog();*/

                        Extras.CrearPacienteFast PFast = new CrearPacienteFast(comboBox1.Text, textBox1.Text.Trim());
                        PFast.ShowDialog();
                        return;
                    }

                    Comunes.MensajesGeneral MG = new Comunes.MensajesGeneral
                    {
                        Mensaje = "El tipo y numero de documento no existen en sistema, pero el numero de documento si existe con otro tipo de " +
                        "documento " + PacienteDoc.Pac_TipoId.ToString() + ".  Se recomienda buscar nuevamente con este tipo de documento sugerido",
                        TipoImagen = 0
                    };
                    MG.ShowDialog();

                    DialogResult result = MessageBox.Show("¿Desea volver a buscar los datos con la sugerencia indicada anteriormente?",
                                                  "Gestion de pacientes",
                                                  MessageBoxButtons.YesNo,
                                                  MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        
                            Paciente = repositorioPacientes.LlamarPacienteNumDoc(textBox1.Text);
                        

                        CitasProximas(Paciente.Pac_Id);

                        comboBox1.Text = PacienteDoc.Pac_TipoId.ToString();
                        textBox3.Text = Paciente.Pac_PrimerN.ToString();
                        textBox4.Text = Paciente.Pac_PrimerA.ToString();
                        textBox5.Text = Paciente.Pac_SegundoN.ToString();
                        textBox6.Text = Paciente.Pac_SegundoA.ToString();
                        textBox7.Text = Paciente.Pac_Telefono.ToString();
                        textBox8.Text = Paciente.Pac_TelefonoAux.ToString();
                        comboBox2.Text = Paciente.Pac_TipoId.ToString();
                        textBox10.Text = Paciente.Pac_IdNum.ToString().Trim();
                        dateTimePicker2.Value = Convert.ToDateTime(Paciente.Pac_FechaNto);
                        textBox11.Text = Paciente.Pac_Direccion.ToString();
                        textBox9.Text = Paciente.Pac_Email.ToString();
                        Pac_Id = Convert.ToInt32(Paciente.Pac_Id);

                        checkBox4.Checked = (Paciente.Pac_Doble == "S" ? true : false);
                        checkBox5.Checked = (Paciente.Pac_2VXS == "S" ? true : false);
                        checkBox3.Checked = (Paciente.Pac_Especial == "S" ? true : false);

                        switch (Paciente.Pac_Sexo)
                        {
                            case "M":
                                comboBox9.SelectedIndex = 0;
                                break;
                            case "F":
                                comboBox9.SelectedIndex = 1;
                                break;
                            case "I":
                                comboBox9.SelectedIndex = 2;
                                break;
                            default:
                                comboBox9.SelectedIndex = 2;
                                break;
                        }

                        switch (Paciente.Pac_Categoria)
                        {
                            case "A":
                                comboBox8.SelectedIndex = 0;
                                break;
                            case "B":
                                comboBox8.SelectedIndex = 1;
                                break;
                            case "C":
                                comboBox8.SelectedIndex = 2;
                                break;
                            case "Z":
                                comboBox8.SelectedIndex = 3;
                                break;
                            default:
                                comboBox8.SelectedIndex = 4;
                                break;
                        }

                        string textoDesplazante = "";

                        if (checkBox4.Checked == true)
                        {
                            textoDesplazante = textoDesplazante + " - Paciente con multiples heridas";
                            panel2.Visible = true;
                            timer1.Start();
                        }
                        if (checkBox5.Checked == true)
                        {
                            textoDesplazante = textoDesplazante + " - Paciente de dos veces por semana";
                            panel2.Visible = true;
                            timer1.Start();
                        }
                        if (checkBox3.Checked == true)
                        {
                            textoDesplazante = textoDesplazante + " - PACIENTE DE TRATO ESPECIAL";
                            panel2.Visible = true;
                            timer1.Start();
                        }

                        string Regi1 = "";

                        
                            Regi1 = repositorioPacientes.Carga_Regimen(Paciente.Pac_Regimen);
                        

                        comboBox4.Text = Regi1.ToString();
                        CXN_ASEGURADORA Ase1 =  repositorioAseguradoras.getInfoFromAsebyCode(Paciente.Pac_Aseguradora);
                        
                        comboBox3.Text = Ase1.Ase_Descripcion.ToString();
                        this.ASESESIONS = Convert.ToInt32(Paciente.Pac_Aseguradora);

                        CargarServicio(Convert.ToInt32(Paciente.Pac_Aseguradora));
                        CargarPrevios();

                        comboBox1.Enabled = false;
                        comboBox5.Enabled = true;
                        textBox1.Enabled = false;
                        button1.Enabled = false;
                        toolStripButton2.Enabled = true;

                        try
                        {                           
                            repositorioHorario.setRecepcion("YES");

                            string canactual =  repositorioHorario.Calcular2(Convert.ToInt32(Pac_Id), this.TSERV);

                            if (canactual == "No es posible calcular sesion debido a que no hay registros de autorizaciones vigentes")
                            {
                                label30.Text = "SIN DATOS";

                            }
                            else
                            {
                                label30.Text = canactual.ToString();

                            }

                            if (repoConf.getListado()["MedicinaGeneralEstadistica"] == "A")
                            {
                                Dictionary<string, string> SGSEERV = new Dictionary<string, string>();

                                
                                    SGSEERV = repositorioHorario.SugerenciaServicio(Convert.ToInt32(Pac_Id));
                                

                                if (SGSEERV != null)
                                {
                                    toolTipServicio.SetToolTip(comboBox5, "Se sugiere asignar el servicio: " + SGSEERV["Servicio"].ToString().ToUpper());
                                    comboBox5.Text = SGSEERV["Servicio"].ToString();

                                    //AQUI IRIA ESA PARTE DE LAS CONSULTAS Y VERIFICAR
                                }
                                else
                                {
                                    toolTipServicio.SetToolTip(comboBox5, "No hay sugerencias disponibles");
                                }
                            }                            
                        }
                        catch
                        {
                            label30.Text = "ERROR INESPERADO EN SESIONES";
                        }

                        textBox2.Focus();
                    }

                    if (result == DialogResult.No)
                    {
                        CrearEditarPaciente crearEditarPaciente = new CrearEditarPaciente();
                        crearEditarPaciente.Size = new Size(619, 672);
                        crearEditarPaciente.StartPosition = FormStartPosition.CenterScreen;
                        crearEditarPaciente.FormBorderStyle = FormBorderStyle.FixedSingle;
                        crearEditarPaciente.AutoScroll = false;
                        crearEditarPaciente.ShowDialog();
                        return;
                    }
                    return;
                }

                CitasProximas(Paciente.Pac_Id);

                textBox3.Text = Paciente.Pac_PrimerN.ToString();
                textBox4.Text = Paciente.Pac_PrimerA.ToString();
                textBox5.Text = Paciente.Pac_SegundoN.ToString();
                textBox6.Text = Paciente.Pac_SegundoA.ToString();
                textBox7.Text = Paciente.Pac_Telefono.ToString();
                textBox8.Text = Paciente.Pac_TelefonoAux.ToString();
                comboBox2.Text = Paciente.Pac_TipoId.ToString();
                textBox10.Text = Paciente.Pac_IdNum.ToString().Trim();
                dateTimePicker2.Value = Convert.ToDateTime(Paciente.Pac_FechaNto);
                textBox11.Text = Paciente.Pac_Direccion.ToString();
                textBox9.Text = Paciente.Pac_Email.ToString();
                Pac_Id = Convert.ToInt32(Paciente.Pac_Id);

                checkBox4.Checked = (Paciente.Pac_Doble == "S" ? true : false);
                checkBox5.Checked = (Paciente.Pac_2VXS == "S" ? true : false);
                checkBox3.Checked = (Paciente.Pac_Especial == "S" ? true : false);

                switch (Paciente.Pac_Sexo)
                {
                    case "M":
                        comboBox9.SelectedIndex = 0;
                        break;
                    case "F":
                        comboBox9.SelectedIndex = 1;
                        break;
                    case "I":
                        comboBox9.SelectedIndex = 2;
                        break;
                    default:
                        comboBox9.SelectedIndex = 2;
                        break;
                }

                switch (Paciente.Pac_Categoria)
                {
                    case "A":
                        comboBox8.SelectedIndex = 0;
                        break;
                    case "B":
                        comboBox8.SelectedIndex = 1;
                        break;
                    case "C":
                        comboBox8.SelectedIndex = 2;
                        break;
                    case "Z":
                        comboBox8.SelectedIndex = 3;
                        break;
                    default:
                        comboBox8.SelectedIndex = 4;
                        break;
                }

                string textoDesplazante2 = "";

                if (checkBox4.Checked == true)
                {
                    textoDesplazante2 = textoDesplazante2 + " - Paciente con multiples heridas";
                    panel2.Visible = true;
                    timer1.Start();
                }
                if (checkBox5.Checked == true)
                {
                    textoDesplazante2 = textoDesplazante2 + " - Paciente de dos veces por semana";
                    panel2.Visible = true;
                    timer1.Start();
                }
                if (checkBox3.Checked == true)
                {
                    textoDesplazante2 = textoDesplazante2 + " - PACIENTE DE TRATO ESPECIAL";
                    panel2.Visible = true;
                    timer1.Start();
                }

                string Regi = "";

                
                    Regi = repositorioPacientes.Carga_Regimen(Paciente.Pac_Regimen);
                

                comboBox4.Text = Regi.ToString();

                CXN_ASEGURADORA Ase =  repositorioAseguradoras.getInfoFromAsebyCode(Paciente.Pac_Aseguradora);
                
                comboBox3.Text = Ase.Ase_Descripcion.ToString();
                ASESESIONS = Convert.ToInt32(Paciente.Pac_Aseguradora);

                CargarServicio(Convert.ToInt32(Paciente.Pac_Aseguradora));
                CargarPrevios();

                comboBox1.Enabled = false;
                comboBox5.Enabled = true;
                textBox1.Enabled = false;
                button1.Enabled = false;
                toolStripButton2.Enabled = true;

                try
                {                   
                    repositorioHorario.setRecepcion("YES");
                    string canactual =  repositorioHorario.Calcular2(Convert.ToInt32(Pac_Id), this.TSERV);
                    
                    if (canactual == "No es posible calcular sesion debido a que no hay registros de autorizaciones vigentes")
                    {
                        label30.Text = "Sin Contador Actualmente";
                    }
                    else
                    {
                        label30.Text = canactual.ToString();
                    }

                    string MedEstadiscica =  repoConf.getListado()["MedicinaGeneralEstadistica"];

                    if (repoConf.getListado()["MedicinaGeneralEstadistica"] == "A")
                    {
                        Dictionary<string, string> SGSEERV = new Dictionary<string, string>();

                        
                            SGSEERV = repositorioHorario.SugerenciaServicio(Convert.ToInt32(Pac_Id));
                        

                        if (SGSEERV != null)
                        {
                            toolTipServicio.SetToolTip(comboBox5, "Se sugiere asignar el servicio: " + SGSEERV["Servicio"].ToString().ToUpper());
                            comboBox5.Text = SGSEERV["Servicio"].ToString();
                        }
                        else
                        {
                            toolTipServicio.SetToolTip(comboBox5, "No hay sugerencias disponibles");
                        }
                    }                  
                }
                catch
                {
                    label30.Text = "ERROR INESPERADO EN SESIONES";
                }

                textBox2.Focus();
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        async void CitasProximas(int _pac_id)
        {
            try
            {
                DateTime Hoy = DateTime.Now.Date;

                List<CXN_HORARIO> getProximas = new List<CXN_HORARIO>();

                
                    getProximas = repositorioAgendar.CitasProximas(_pac_id, Convert.ToDateTime(Hoy));
                
                 
                if (getProximas != null)
                {
                    Task oTask = null;
                    oTask = new Task(Voz);

                    if (oTask != null)
                    {
                        oTask.Start();
                        AgendadosDia A = new AgendadosDia(getProximas);
                        A.ShowDialog();
                        await oTask;
                    }                    
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        void Voz()
        {
            try
            {
                cbVoice.Items.Clear();

                SpeechSynthesizer synth = new SpeechSynthesizer();

                foreach (var voice in synth.GetInstalledVoices())
                    cbVoice.Items.Add(voice.VoiceInfo.Name);
                cbVoice.SelectedIndex = 0;

                if (oSpeechSynthesizer == null)
                {
                    oSpeechSynthesizer = new System.Speech.Synthesis.SpeechSynthesizer();
                    oSpeechSynthesizer.SetOutputToDefaultAudioDevice();
                }

                if (cbVoice.Text != "")
                    oSpeechSynthesizer.SelectVoice(cbVoice.Text);

                oSpeechSynthesizer.Speak("ATENCION!, Este paciente ya cuenta con mas de una cita próxima agendada");
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void CargarPrevios()
        {
            try
            {
                List<CXN_HORARIO> _previos = new List<CXN_HORARIO>();

                
                    _previos = repositorioAgendar.CargarPrevios(Pac_Id);
                

                if (_previos != null)
                {
                    Nuevo = false;

                    this.Size = new Size(1288, 590);
                    
                    this.StartPosition = FormStartPosition.Manual;
                    this.Left = (Screen.PrimaryScreen.WorkingArea.Width - this.Width) / 2;
                    this.Top = (Screen.PrimaryScreen.WorkingArea.Height - this.Height) / 2;
                    dataGridView1.DataSource = null;

                    dt = new DataTable();
                    DataColumn POS;
                    DataColumn ADM;
                    DataColumn EST;
                    DataColumn DATOS;

                    dt = new DataTable();
                    POS = dt.Columns.Add("POS", typeof(int));
                    ADM = dt.Columns.Add("Admision", typeof(string));
                    EST = dt.Columns.Add("Estado", typeof(string));
                    DATOS = dt.Columns.Add("Datos", typeof(string));
                    int Contador = 1;

                    foreach (CXN_HORARIO i in _previos)
                    {
                        string Est;
                        switch (i.Hor_Estado)
                        {
                            case "C":
                                Est = "Cancelo";
                                break;
                            case "A":
                                Est = "No Asiste";
                                break;
                            case "H":
                                Est = "Asistio";
                                break;
                            case "P":
                                Est = "Pendiente";
                                break;
                            default:
                                Est = "Error";
                                break;
                        }


                        DataRow row = dt.NewRow();

                        row["POS"] = Contador;
                        row["Admision"] = i.Hor_Id.ToString();
                        row["Estado"] = Est;
                        row["Datos"] = "ADMISION: " + i.Hor_Id.ToString() + "\n" +
                                       "FECHA: " + Convert.ToDateTime(i.Hor_Pac_Fecha_Cita).ToString(Conexion.ConectionDictionary["Format_Fecha"]) + "\n" +
                                       "ESTADO: " + Est + "\n" +
                                       "SERVICIO: " + i.Com_Nombre.ToString() + "\n" +
                                       "PROFESIONAL: " + i.Hor_Observacion.ToString();

                        dt.Rows.Add(row);
                        dt.AcceptChanges();

                        Contador++;
                    }

                    Contador = 0;

                    Estilos();
                    dataGridView1.CellFormatting += DataGridView1_CellFormatting;
                    dataGridView1.CellClick += DataGridView1_CellClick;
                    dataGridView1.ClearSelection();                    
                }
                else
                {
                    Nuevo = true;
                    label10.Visible = true;
                    dataGridView1.Visible = false;
                    dataGridView1.DataSource = null;
                    this.Size = new Size(944, 590);              
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void DataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.Value != null && e.Value.GetType() == typeof(string))
            {
                e.Value = ((string)e.Value).Replace("\n", Environment.NewLine);
            }

            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                DataGridViewCell cell = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];
                cell.Style.WrapMode = DataGridViewTriState.True;
                dataGridView1.Rows[e.RowIndex].Height = 70;
            }

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (dataGridView1.Rows[row.Index].Cells[2].Value.ToString() == "Pendiente")
                {
                    dataGridView1.Rows[row.Index].Cells[1].Style.BackColor = Color.LightGreen;
                    dataGridView1.Rows[row.Index].Cells[3].Style.BackColor = Color.LightGreen;
                    dataGridView1.Rows[row.Index].Cells[1].Style.ForeColor = Color.DarkGreen;
                    dataGridView1.Rows[row.Index].Cells[3].Style.ForeColor = Color.DarkGreen;
                }
                else if (dataGridView1.Rows[row.Index].Cells[2].Value.ToString() == "Cancelo")
                {
                    dataGridView1.Rows[row.Index].Cells[1].Style.BackColor = Color.Orange;
                    dataGridView1.Rows[row.Index].Cells[3].Style.BackColor = Color.Orange;
                    dataGridView1.Rows[row.Index].Cells[1].Style.ForeColor = Color.Red;
                    dataGridView1.Rows[row.Index].Cells[3].Style.ForeColor = Color.Red;
                }
                else if (dataGridView1.Rows[row.Index].Cells[2].Value.ToString() == "Asistio")
                {
                    dataGridView1.Rows[row.Index].Cells[1].Style.BackColor = Color.LightBlue;
                    dataGridView1.Rows[row.Index].Cells[3].Style.BackColor = Color.LightBlue;
                    dataGridView1.Rows[row.Index].Cells[1].Style.ForeColor = Color.Blue;
                    dataGridView1.Rows[row.Index].Cells[3].Style.ForeColor = Color.Blue;
                }
                else if (dataGridView1.Rows[row.Index].Cells[2].Value.ToString() == "No Asiste")
                {
                    dataGridView1.Rows[row.Index].Cells[1].Style.BackColor = Color.White;
                    dataGridView1.Rows[row.Index].Cells[3].Style.BackColor = Color.White;
                    dataGridView1.Rows[row.Index].Cells[1].Style.ForeColor = Color.Black;
                    dataGridView1.Rows[row.Index].Cells[3].Style.ForeColor = Color.Black;
                }
                else
                {
                    dataGridView1.Rows[row.Index].Cells[1].Style.BackColor = Color.Red;
                    dataGridView1.Rows[row.Index].Cells[3].Style.BackColor = Color.Red;
                    dataGridView1.Rows[row.Index].Cells[1].Style.ForeColor = Color.White;
                    dataGridView1.Rows[row.Index].Cells[3].Style.ForeColor = Color.White;
                }
            }

            dataGridView1.ClearSelection();
        }

        private void DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                Comunes.MensajesGeneral MG1 = new Comunes.MensajesGeneral();
                string _message = "";

                
                    _message = repositorioAgendar.Observacioprevia(Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString()));
                
                
                if (_message != "")
                {
                    MG1.Mensaje = _message;
                    MG1.TipoImagen = 3;
                    MG1.ShowDialog();
                }
                else
                {
                    MG1.Mensaje = "No hay observacion para esta admision";
                    MG1.TipoImagen = 3;
                    MG1.ShowDialog();
                }

                dataGridView1.ClearSelection();
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }           
        }

        void Estilos()
        {
            try
            {
                dataGridView1.EnableHeadersVisualStyles = false;
                dataGridView1.ScrollBars = ScrollBars.Vertical;

                dataGridView1.DataSource = dt;

                dataGridView1.Columns["Admision"].Width = 80;
                dataGridView1.Columns["Datos"].Width = 230;

                dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font(dataGridView1.Font, FontStyle.Bold);
                dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 8, FontStyle.Bold);
                dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#bfdbff");
                dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;
                dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;

                dataGridView1.Columns["Admision"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView1.Columns["Datos"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

                dataGridView1.Columns["Admision"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridView1.Columns["Datos"].SortMode = DataGridViewColumnSortMode.NotSortable;

                dataGridView1.Columns["POS"].Visible = false;
                dataGridView1.Columns["Estado"].Visible = false;

                dataGridView1.ClearSelection();
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                Busca_Pac();
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void Cierra_Agenda()
        {
            this.Dispose();
            this.Close();
        }

        private void AgendarCita_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyData == Keys.Escape)
                {
                    Cierra_Agenda();
                }
                if (e.KeyData == Keys.F5)
                {
                    if (button1.Enabled == true)
                    {
                        Busca_Pac();
                    }
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            Cierra_Agenda();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            try
            {
                MGmen = new MensajesGeneral();

                var celular = repositorioPacientes.ValidaCelular(textBox7.Text);
                if (celular != true)
                {
                    MGmen.TipoImagen = 1000;
                    MGmen.Mensaje = "El numero de celular es incorrecto, este campo es oligatorio";
                    MGmen.ShowDialog();
                    return;
                }

                if (textBox9.Text != "")
                {
                    var email = repositorioPacientes.ValidaEmail(textBox9.Text);
                    if (email != true)
                    {
                        MGmen.TipoImagen = 1000;
                        MGmen.Mensaje = "El formato del correo es incorrecto, si no lo conoce deje esta casilla en blanco";
                        MGmen.ShowDialog();
                        return;
                    }
                }

                if (textBox3.Text == "")
                {
                    MGmen.TipoImagen = 1000;
                    MGmen.Mensaje = "Debe diligenciar primer nombre del paciente";
                    MGmen.ShowDialog();
                    return;
                }

                if (textBox4.Text == "")
                {
                    MGmen.TipoImagen = 1000;
                    MGmen.Mensaje = "Debe diligenciar primer apellido del paciente";
                    MGmen.ShowDialog();
                    return;
                }

                if (comboBox2.Text == "")
                {
                    MGmen.TipoImagen = 1000;
                    MGmen.Mensaje = "Debe seleccionar tipo de documento del paciente";
                    MGmen.ShowDialog();
                    return;
                }

                if (string.IsNullOrEmpty(comboBox7.Text))
                {
                    MGmen.TipoImagen = 1000;
                    MGmen.Mensaje = "Debe seleccionar el grupo de servicios";
                    MGmen.ShowDialog();
                    return;
                }

                if (textBox10.Text == "")
                {
                    MGmen.TipoImagen = 1000;
                    MGmen.Mensaje = "Debe diligenciar documento del paciente";
                    MGmen.ShowDialog();
                    return;
                }

                if (comboBox4.Text == "")
                {
                    MGmen.TipoImagen = 1000;
                    MGmen.Mensaje = "Debe seleccionar regimen del paciente";
                    MGmen.ShowDialog();
                    return;
                }

                if (comboBox5.Text == "")
                {
                    MGmen.TipoImagen = 1000;
                    MGmen.Mensaje = "Debe seleccionar un servicio a asignar";
                    MGmen.ShowDialog();
                    return;
                }

                if (comboBox6.Text == "")
                {
                    MGmen.TipoImagen = 1000;
                    MGmen.Mensaje = "Debe seleccionar la modalidad de la cita";
                    MGmen.ShowDialog();
                    return;
                }

                if (comboBox3.Text == "")
                {
                    MGmen.TipoImagen = 1000;
                    MGmen.Mensaje = "Debe seleccionar la aseguradora del paciente";
                    MGmen.ShowDialog();
                    return;
                }

                if (comboBox9.Text == "")
                {
                    MGmen.TipoImagen = 1000;
                    MGmen.Mensaje = "Debe seleccionar el sexo del paciente";
                    MGmen.ShowDialog();
                    return;
                }

                if (PacRules.ValidarRegimen(comboBox4.Text, comboBox8.Text) == false)
                {
                    MG = new MensajesGeneral();
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "Si el regimen es no afiliado, la categoria no puede ser A, B, C o Z.  Si el regimen es diferente a no afiliado, la categoria no puede ser no aplica";
                    MG.ShowDialog();
                    return;
                }

                if (comboBox8.Text == "")
                {
                    MGmen.TipoImagen = 1000;
                    MGmen.Mensaje = "Debe seleccionar una categoria";
                    MGmen.ShowDialog();
                    return;
                }

                DialogResult result = MessageBox.Show("¿Desea asignar la cita aqui?",
                                                      "Zamenis Health",
                                                      MessageBoxButtons.YesNo,
                                                      MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    DateTime _fechas = new DateTime();

                    if (Conexion.ConectionDictionary["Format_Fecha"] == "yyyy/MM/dd" || Conexion.ConectionDictionary["Format_Fecha"] == "yyyy-MM-dd")
                    {
                        string D1 = repositorioHorario.Extract(label27.Text, 4, 0);
                        string D2 = repositorioHorario.Extract(label27.Text, 2, 5);
                        string D3 = repositorioHorario.Extract(label27.Text, 2, 8);
                        _fechas = new DateTime(Convert.ToInt32(D1), Convert.ToInt32(D2), Convert.ToInt32(D3));
                    }

                    if (Conexion.ConectionDictionary["Format_Fecha"] == "dd/MM/yyyy" || Conexion.ConectionDictionary["Format_Fecha"] == "dd-MM-yyyy")
                    {
                        string D1 = repositorioHorario.Extract(label27.Text, 2, 0);
                        string D2 = repositorioHorario.Extract(label27.Text, 2, 3);
                        string D3 = repositorioHorario.Extract(label27.Text, 4, 6);
                        _fechas = new DateTime(Convert.ToInt32(D1), Convert.ToInt32(D2), Convert.ToInt32(D3));
                    }

                    List<CXN_HORARIO> _listaPrevios =  repositorioHorario.ListarCitasXPaciente(Pac_Id, Convert.ToDateTime(_fechas.ToString(Conexion.ConectionDictionary["Format_Fecha"])));
                    
                    if (_listaPrevios != null)
                    {
                        Extras.AgendadosDia agendadosDia = new Extras.AgendadosDia(_listaPrevios);
                        agendadosDia.ShowDialog();
                        ConfirmarCita();
                    }
                    else
                    {
                        Asignar_Cita();
                    }
                }
                if (result == DialogResult.No)
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void ConfirmarCita()
        {
            try
            {
                DialogResult result2 = MessageBox.Show("Debido a que este paciente ya cuenta con mas citas para este dia " +
                                    "¿aun asi desea asignar la cita de todas formas?",
                                    "Zamenis Health",
                                    MessageBoxButtons.YesNo,
                                    MessageBoxIcon.Question);

                if (result2 == DialogResult.Yes)
                {
                    Asignar_Cita();
                }

                if (result2 == DialogResult.No)
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        bool consultarEspacioRobado()
        {
            try
            {
                return repositorioHorario.EspacioRobado(this.CodePrest, this.CodeProf, this.IdHora, this.FechaCita);
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
                return false;
            }
        }

        private void FinalizaCita()
        {
            try
            {
                if (consultarEspacioRobado() == false)
                {
                    Comunes.MensajesGeneral MG1 = new Comunes.MensajesGeneral();
                    string Regi = repositorioPacientes.Regimen(comboBox4.Text);

                    CXN_ASEGURADORA idase = repositorioAseguradoras.getInfoFromAsebyName(comboBox3.Text);                                    

                    //ACTUALIZAR DATOS
                    CXN_PACIENTES P = new CXN_PACIENTES
                    {
                        Pac_PrimerN = textBox3.Text,
                        Pac_SegundoN = textBox5.Text,
                        Pac_PrimerA = textBox4.Text,
                        Pac_SegundoA = textBox6.Text,
                        Pac_Telefono = textBox7.Text,
                        Pac_TelefonoAux = textBox8.Text,
                        Pac_Email = textBox9.Text,
                        Pac_IdNum = textBox10.Text.Trim(),
                        Pac_TipoId = comboBox2.Text,
                        Pac_Direccion = textBox11.Text,
                        Pac_FechaNto = dateTimePicker2.Value.Date,
                        Pac_Id = Pac_Id,
                        Pac_Regimen = Regi,
                        Pac_Aseguradora = Convert.ToInt32(idase.Ase_Identificador),
                        Pac_2VXS = (checkBox5.Checked == true ? "S" : "N"),
                        Pac_Doble = (checkBox4.Checked == true ? "S" : "N"),
                        Pac_Especial = (checkBox3.Checked == true ? "S" : "N")
                    };

                    switch (comboBox9.SelectedIndex)
                    {
                        case 0:
                            P.Pac_Sexo = "M";
                            break;
                        case 1:
                            P.Pac_Sexo = "F";
                            break;
                        case 2:
                            P.Pac_Sexo = "I";
                            break;

                        default:
                            P.Pac_Sexo = "I";
                            return;
                    }

                    switch (comboBox8.SelectedIndex)
                    {
                        case 0:
                            P.Pac_Categoria = "A";
                            break;
                        case 1:
                            P.Pac_Categoria = "B";
                            break;
                        case 2:
                            P.Pac_Categoria = "C";
                            break;
                        case 3:
                            P.Pac_Categoria = "Z";
                            break;
                        case 4:
                            P.Pac_Categoria = "N";
                            break;

                        default:
                            MG = new MensajesGeneral();
                            MG.TipoImagen = 1000;
                            MG.Mensaje = "Seleccione una categoria";
                            MG.ShowDialog();
                            return;
                    }

                    
                    repositorioPacientes.ActualizarPaciente(P);
                                        

                    //AGENDA CITA

                    string Modalidad_Cita;
                    switch (comboBox6.Text)
                    {
                        case "Intramural":
                            Modalidad_Cita = "01";
                            break;

                        case "Extramural unidad móvil":
                            Modalidad_Cita = "02";
                            break;

                        case "Extramural domiciliaria":
                            Modalidad_Cita = "03";
                            break;

                        case "Extramural jornada de salud":
                            Modalidad_Cita = "04";
                            break;

                        case "Telemedicina interactiva":
                            Modalidad_Cita = "06";
                            break;

                        case "Telemedicina no interactiva":
                            Modalidad_Cita = "07";
                            break;

                        case "Telemedicina telexperticia":
                            Modalidad_Cita = "08";
                            break;

                        case "Telemedicina telemonitoreo":
                            Modalidad_Cita = "09";
                            break;

                        default:
                            Modalidad_Cita = "01";
                            break;
                    }

                    if (TipoBod == "MG")
                    {
                        Extras.TipoCitaMG TipoCitaMG = new Extras.TipoCitaMG();
                        TipoCitaMG.ShowDialog();
                    }
                    else
                    {
                        Tipo_Serv = "";
                    }

                    string Vales = "";
                    if (checkBox6.Checked == true)
                    {
                        Vales = "S";
                    }

                    if (checkBox6.Checked == false)
                    {
                        Vales = "N";
                    }

                    string Observa = "--> Cita asignada por: " + Comunes.Contenedor.UsuarioLogueado + " - " + textBox2.Text;

                    CXN_CONVENIOS CUP = repositorioConvenios.ServicioCUP(comboBox5.Text, idase.Ase_Identificador);                    

                    if (CUP == null)
                    {
                        MG1.Mensaje = "La aseguradora seleccionada no tiene convenio con el servicio seleccionado, cambie la aseguradora " +
                            "y vuelva a intentar";
                        MG1.TipoImagen = 1000;
                        MG1.ShowDialog();
                        return;
                    }

                    CXN_HORARIO H = new CXN_HORARIO
                    {
                        Hor_Estado = "A",
                        Hor_Pac_Id = Pac_Id,
                        Hor_Pac_Bod = CodeProf,
                        Hor_Pac_Tipo_Serv = TipoBod,
                        Hor_Pac_Cia = CodePrest,
                        Hor_Pac_Ase = Convert.ToInt32(idase.Ase_Identificador),
                        Hor_Pac_Cup = CUP.Con_Id_Serv.ToString(),
                        Hor_Pac_UsrGraba = Comunes.Contenedor.UsuarioLogueado,
                        Hor_Imp_Age = P.Pac_PrimerA + " " + P.Pac_SegundoA + " " + P.Pac_PrimerN + " " + P.Pac_SegundoN,
                        Hor_Pac_Fecha_Cita = Convert.ToDateTime(FechaCita),
                        Hor_Pac_Id_Hora = IdHora,
                        Hor_Pac_Hora_Cita = Convert.ToDateTime(HoraTexto), //hora cita
                        Hor_Observacion = Observa,
                        Hor_Pac_Sal = Tipo_Serv,
                        Hor_Vales = Vales,
                        Hor_Pac_Modalidad = Modalidad_Cita,
                        Hor_BloqEspaces = 0,
                        Hor_GrupoServicios = IRIPS.getCodeGrupoServicios(comboBox7.Text),
                        Hor_Regimen = P.Pac_Regimen,
                        Hor_ArrastraHistoria = checkBox7.Visible == false ? "N" : checkBox7.Checked == true ? "S" : "N",
                        Hor_AvisoCurInicio = checkBox8.Checked ? true : false
                    };

                    int admTemp = repositorioAgendar.AgendarPaciente(H);                    

                    Agenda f1 = Application.OpenForms.OfType<Agenda>().SingleOrDefault();

                    f1.RechargeTrueCheck();

                    if (Nuevo == true)
                    {
                        MG1.Mensaje = "Este paciente es nuevo, se ingresara a la Circular 016, a continuacion seleccione la fecha " +
                            "que el paciente solicito desde el principio";
                        MG1.TipoImagen = 0;
                        MG1.ShowDialog();

                        Extras.Circular016 Circular_016 = new Extras.Circular016(Pac_Id, Convert.ToDateTime(label27.Text));
                        Circular_016.ShowDialog();
                    }

                    PopupNotifier Pop = PopUps.setPopUp(Properties.Resources2.comprobado,
                                                                     Color.LightGreen,
                                                                     "CITA ASIGNADA",
                                                                     Color.DarkGreen,
                                                                     "Cita de " + H.Hor_Imp_Age + " asignada exitosamente");
                    Pop.Popup();

                    if (textBox2.Text != "")
                    {
                        
                            repositorioAgendar.updateObservaTemp(textBox2.Text, admTemp);
                                                
                    }

                    if (Preferencias.TicketCitas == "A")
                    {
                        PrintTickets pT = new PrintTickets(admTemp);
                        pT.ShowDialog();
                    }


                    CXN_TIPIFICADOR T = new CXN_TIPIFICADOR
                    {
                        Aseguradora = idase.Ase_Identificador,
                        Celular = textBox7.Text,
                        Email = textBox9.Text,
                        Fecha = DateTime.Now.Date,
                        Hora = DateTime.Now,
                        Gestion = textBox2.Text.Trim(),
                        NombreLlama = "DESDE AGENDA",
                        NumIdPaciente = textBox10.Text.Trim(),
                        TipoIdPaciente = comboBox2.Text,
                        RazonLlamada = "AGENDAMIENTO DE CITAS",
                        Usuario = Contenedor.UsuarioLogueado,
                        Ingreso = "DESDE AGENDA",
                        NombrePaciente = P.Pac_PrimerA + " " + P.Pac_SegundoA + " " + P.Pac_PrimerN + " " + P.Pac_SegundoN
                    };

                    bool crear = tipificador.Crea(T);
                    if (crear == false)
                    {
                        MG = new MensajesGeneral();
                        MG.Mensaje = "No se logro grabar el registro en el Tipificador, registrelo manualmente";
                        MG.TipoImagen = 1000;
                        MG.ShowDialog();
                    }

                    this.Dispose();
                    this.Close();
                }
                else
                {
                    MensajesGeneral MMG = new MensajesGeneral();
                    MMG.TipoImagen = 1000;
                    MMG.Mensaje = "Este espacio acaba de ser utilizado con otro paciente por parte de otra persona que usa la agenda";
                    MMG.ShowDialog();
                }                
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void Bloqueo(string Razon)
        {
            try
            {
                string Observa = "--> Bloqueado por: " + Comunes.Contenedor.UsuarioLogueado + " - " + textBox2.Text;

                CXN_HORARIO H = new CXN_HORARIO
                {
                    Hor_Estado = "B",
                    Hor_Observacion = Observa,
                    Hor_Pac_Cup = "",
                    Hor_Pac_Sal = "",
                    Hor_Vales = "",
                    Hor_Pac_Modalidad = "",
                    Hor_Pac_Fecha_Cita = Convert.ToDateTime(FechaCita),
                    Hor_Pac_Id_Hora = IdHora,
                    Hor_Pac_Hora_Cita = Convert.ToDateTime(HoraTexto), //hora cita
                    Hor_Pac_Bod = CodeProf,
                    Hor_Pac_Tipo_Serv = TipoBod,
                    Hor_Pac_Ase = 88,
                    Hor_Imp_Age = Razon.ToString(),
                    Hor_Pac_Cia = CodePrest,
                    Hor_Pac_Id = 1,
                    Hor_Pac_UsrGraba = Comunes.Contenedor.UsuarioLogueado,
                    Hor_GrupoServicios = "",
                    Hor_Regimen = ""
                };

                
                    repositorioAgendar.AgendarPaciente(H);
                                

                Agenda f1 = Application.OpenForms.OfType<Agenda>().SingleOrDefault();

                f1.RechargeTrueCheck();

                this.Dispose();
                this.Close();
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void Asignar_Cita()
        {
            if (HabilitaEspacio != "A")
            {
                DialogResult result = MessageBox.Show("Este espacio es de descanso del profesional. ¿Desea asignar la cita de todas formas?",
                                      "Zamenis Health - Asdignacion de citas",
                                      MessageBoxButtons.YesNo,
                                      MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    FinalizaCita();
                }
                if (result == DialogResult.No)
                {
                    return;
                }
            }
            else
            {
                FinalizaCita();
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            Bloqueo("ESPACIO BLOQUEADO DESDE RECEPCION");
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                string texto = Microsoft.VisualBasic.Interaction.InputBox(
                    "Por favor ingrese el motivo del Bloqueo de Agendas, si desea cancelar esta accion deje el cuadro en blanco y haga clic en Cancelar",
                    "Bloquear agendas");

                switch (texto)
                {
                    case "":
                        break;

                    default:
                        Bloqueo(texto);
                        break;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                if (this.Width == xPos)
                {
                    this.label11.Location = new System.Drawing.Point(0, 10);
                    xPos = 0;
                }
                else
                {
                    this.label11.Location = new System.Drawing.Point(xPos, 10);
                    xPos++;
                }
            }
            catch
            {
                timer1.Enabled = false;
            }
        }

        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                CXN_CONVENIOS TSERVICIO = new CXN_CONVENIOS();

                
                    TSERVICIO = repositorioConvenios.ServicioCUP(comboBox5.Text, ASESESIONS);
                

                TSERV = TSERVICIO.Con_Tipo_Serv;
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox3.Checked == true) 
            {
                if (this.Pac_Id > 0)
                {
                    HistoriasClinicas.Extras.Recomendaciones cP = new HistoriasClinicas.Extras.Recomendaciones(this.Pac_Id);
                    cP.ShowDialog();
                }                
            }
        }

        private void panel4_MouseDown(object sender, MouseEventArgs e)
        {
            ConfigForm.ReleaseCapturing();
            ConfigForm.SendMessageMove(this.Handle, 0x112, 0xf012, 0);
        }
    }
}
