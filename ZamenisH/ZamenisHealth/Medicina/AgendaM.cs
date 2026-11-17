using Domain;
using Domain.CXN;

using FormAndControls;

using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;

using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

using Tulpep.NotificationWindow;

using ZamenisHealth.Clases;
using ZamenisHealth.Clases.Controles;
using ZamenisHealth.Comunes;
using ZamenisHealth.HistoriasClinicas;
using ZamenisHealth.Medicina.VirtualMedic;
using ZamenisHealth.Recepcion.Extras;

using Color = System.Drawing.Color;

namespace ZamenisHealth.Medicina
{
    public partial class AgendaM : Forma
    {
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, bool wParam, int lParam);
        private const int WM_SETREDRAW = 0x000B;

        private static readonly IAgendaC repoAgendaMedicaConsultas = new MAgendaC();
        private static readonly IAgendaProfesionales repoAgendaProfesionales = new MAgendaProfesionales();
        private static readonly IDisponibilidad repoDisponibilidad = new MDisponibilidad();
        private static readonly IBodegas repoBodegas = new MBodegas();
        private static readonly ICompañia repoCompañia = new MCompañia();
        private static readonly IConfSystem repoConfSystem = new MConfSystem();
        private static readonly IConvenios repoConvenios = new MConvenios();

        int Med, Cia;
        private bool Colorimetria;
        private bool MedGen, ToolTipCitas;

        private ToolStripButton btnVideollamadaH;
        private TooltipEnriquecido tooltipAgenda;
        private Dictionary<int, CXN_HORARIO> DicDatos;

        DataTable dt;
        DataColumn IDE;
        DataColumn Habilita;
        DataColumn Hora;
        DataColumn Admision;
        DataColumn Paciente;
        DataColumn Paciente_Est;
        DataColumn Modalidad;
        DataColumn Enfermero;
        DataColumn InPaquete;
        DataColumn PacSal;
        DataColumn Aseguradora;
        DataColumn Arrastra;
        DataColumn Colores;
        DataColumn CodePaciente;

        public AgendaM(bool _MedGen)
        {
            InitializeComponent();
            this.MedGen = _MedGen;
        }
        private void AgendaM_Load(object sender, EventArgs e)
        {
            try
            {
                Titulo.Text = "Agenda Diaria";
                LogoMain.Image = Properties.Resources.Splash;
                SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";

                ToolTipCitas = Preferencias.DatosCitaMouseMove == "A" ? true : false;

                gridZH1.ElimnarAutosize();
                gridZH1.dataGridView1.CellMouseClick += dataGridView1_CellMouseClick;
                gridZH1.dataGridView1.CellMouseEnter += DataGridView1_CellMouseEnter1;
                gridZH1.dataGridView1.CellMouseLeave += DataGridView1_MouseLeave;
                //gridZH1.CeldaHeight = true;
                checkBox1.Checked = true;

                typeof(DataGridView).InvokeMember("DoubleBuffered",
                   BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty,
                   null, gridZH1.dataGridView1, new object[] { true });

                Botones();

                var ValidaMed = repoBodegas.getDatosUser(Comunes.Contenedor.UsuarioLogueado);
                if (ValidaMed != null)
                {
                    var isMedic = repoDisponibilidad.getHorariosByCodeMed(ValidaMed.Bod_Numero);
                    if (isMedic != null)
                    {
                        Cargacia();

                        Colorimetria = (Preferencias.Colorimetria == "A" ? true : false);
                        if (this.Colorimetria == false)
                        {
                            label6.Visible = false;
                            label7.Visible = false;
                            label21.Visible = false;
                        }

                        if (Conexion.ConectionDictionary["Videoconferencia"] == "A")
                        {
                            List<CXN_IMAGEN_SYSTEM> d = repoConfSystem.getConfSystem();
                            if (d != null)
                            {
                                CXN_IMAGEN_SYSTEM d2 = d.FirstOrDefault(x => x.Tab_Nombre == "Videoconferencia");
                                if (d2 != null)
                                {
                                    if (d2.Tab_Clave == "A")
                                    {
                                        btnVideollamadaH.Visible = true;
                                    }
                                    else
                                    {
                                        btnVideollamadaH.Visible = false;
                                    }
                                }
                                else
                                {
                                    btnVideollamadaH.Visible = false;
                                }
                            }
                            else
                            {
                                btnVideollamadaH.Visible = false;
                            }                            
                        }

                        Med = ValidaMed.Bod_Numero;
                        Cargar_Agenda();
                    }
                    else
                    {
                        MessageBox.Show("Su usuario no es tipo medico, no puede validar agendas medicas",
                                               "Error",
                                               MessageBoxButtons.OK,
                                               MessageBoxIcon.Error);
                        this.Dispose();
                        this.Close();
                    }

                    if (checkBox1.Checked == true) { timer3.Enabled = true; }

                    if (ToolTipCitas == false)
                    {
                        gridZH1.dataGridView1.CellMouseEnter -= DataGridView1_CellMouseEnter1;
                    }
                }
                else
                {
                    MessageBox.Show("Su usuario no es tipo medico, no puede validar agendas medicas",
                       "Error",
                       MessageBoxButtons.OK,
                       MessageBoxIcon.Error);
                    this.Dispose();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void btnZamenis1_ButtonClick(object sender, EventArgs e)
        {
            Historial_Medico_1 RM = new Historial_Medico_1();
            RM.ShowDialog();
        }
        private void btnZamenis2_ButtonClick(object sender, EventArgs e)
        {
            Comunes.MensajeroSend M = new Comunes.MensajeroSend();
            M.ShowDialog();
        }
        private void btnZamenis3_ButtonClick(object sender, EventArgs e)
        {
            Cargar_Agenda();
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var NCia = repoCompañia.getPrestadorbyName(comboBox1.Text);
            Cia = NCia.Com_Identificador;
            Cargar_Agenda();
        }
        void Botones()
        {
            ToolStripButton btnHistorial = new ToolStripButton();
            btnHistorial = createToolButton("Historial");
            MenuLateral.Items.Add(btnHistorial);
            btnHistorial.Click += btnZamenis1_ButtonClick;

            ToolStripButton btnMensajero = new ToolStripButton();
            btnMensajero = createToolButton("Mensajero");
            MenuLateral.Items.Add(btnMensajero);
            btnMensajero.Click += btnZamenis2_ButtonClick;

            btnVideollamadaH = new ToolStripButton();
            btnVideollamadaH = createToolButton("Videollamada Historial");
            MenuLateral.Items.Add(btnVideollamadaH);
            btnVideollamadaH.Click += toolStripButton4_Click;

            ToolStripButton btnUpdateAgenda = new ToolStripButton();
            btnUpdateAgenda = createToolButton("Actualizar Agenda");
            MenuLateral.Items.Add(btnUpdateAgenda);
            btnUpdateAgenda.Click += btnZamenis3_ButtonClick;
        }
        private void AgendaM_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyData == Keys.F5)
                {
                    Cargar_Agenda();
                }
                if (e.KeyData == Keys.Escape)
                {
                    this.Dispose();
                    this.Close();
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
                timer1.Enabled = false;
            }
            catch (Exception ex)
            {
                timer1.Enabled = false;
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void timer2_Tick(object sender, EventArgs e)
        {
            try
            {
                Cargar_Agenda();
            }
            catch
            {
                timer2.Enabled = false;
                MessageBox.Show("El servicio de actualizacion automatica de la agenda medica se ha detenido, por favor cierre la agenda y vuelva a abrirla " +
                    "o si lo desea puede seguir actualizando la agenda cuando se admisione un paciente con la tecla F5", "Error de Internet", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                Cargar_Agenda();
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void Selection_Active(string AdmString)
        {
            try
            {               
                if (!string.IsNullOrEmpty(AdmString))
                {
                    string Estado = repoAgendaProfesionales.getEstadoCita(Convert.ToInt32(AdmString));

                    int Adm = Convert.ToInt32(AdmString);
                    string _createHistory = repoAgendaMedicaConsultas.CrearHistorias(Adm, Comunes.Contenedor.UsuarioLogueado);
                    if (_createHistory == "JM")
                    {
                        Historia_JM_Completar_Seleccion forma = new Historia_JM_Completar_Seleccion(Adm);
                        forma.ShowDialog();
                    }
                    else
                    {
                        if (Estado == "A")
                        {
                            DatosCita f = new DatosCita(Convert.ToInt32(AdmString), "AgendaM-A");
                            f.ShowDialog();

                            timer1.Enabled = true;
                        }
                        else if (Estado == "H")
                        {
                            DatosCita f = new DatosCita(Convert.ToInt32(AdmString), "AgendaM-A");
                            f.ShowDialog();

                            timer1.Enabled = true;
                        }
                        else if (Estado == "B")
                        {
                            timer1.Enabled = true;
                            MessageBox.Show("Este espacio esta bloquedo en su agenda, consulte con la Recepcion",
                                    "Bloqueado",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Exclamation);
                        }
                        else if (Estado == "P")
                        {
                            DialogResult result = MessageBox.Show("¿Desea crear esta historia clinica?",
                                              "Zamenis Health - Creacion de historias",
                                              MessageBoxButtons.YesNo,
                                              MessageBoxIcon.Question);

                            if (result == DialogResult.Yes)
                            {
                                HistoriasClinicas.Historia_NotaEnfermeria NE = new HistoriasClinicas.Historia_NotaEnfermeria();
                                HistoriasClinicas.Historia_JefeEnfermeria NE2 = new HistoriasClinicas.Historia_JefeEnfermeria();

                                switch (_createHistory.ToString())
                                {
                                    case "CURACION":
                                        NE.Admision = Adm;
                                        NE.ShowDialog();                                                                              
                                        break;

                                    case "NOBOSS":
                                        MessageBox.Show("No es posible acceder a este modulo, su usuario no es tipo Enfermero Jefe",
                                        "Acceso Denegado!!!",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Error);
                                        break;

                                    case "BOSS":
                                        NE2.Admision = Adm;
                                        NE2.ShowDialog();
                                        break;

                                    case "MEDGEN":
                                        HistoriasClinicas.Historia_MedicinaGeneral MG = new HistoriasClinicas.Historia_MedicinaGeneral();
                                        MG.Admision = Adm;
                                        MG.Retoma = false;
                                        MG.ShowDialog();
                                        break;

                                    case "FISIATRIA":
                                        HistoriasClinicas.Historia_Fisiatria FI = new HistoriasClinicas.Historia_Fisiatria();
                                        FI.Admision = Adm;
                                        FI.Retoma = false;
                                        FI.ShowDialog();
                                        break;

                                    case "EVOLUCION":
                                        HistoriasClinicas.Historia_Evoluciones EVO = new HistoriasClinicas.Historia_Evoluciones();
                                        EVO.Admision = Adm;
                                        EVO.ShowDialog();
                                        break;

                                    case "HTO":
                                        HistoriasClinicas.Historia_TerOcupacional Hto = new HistoriasClinicas.Historia_TerOcupacional();
                                        Hto.Admision = Adm;
                                        Hto.ShowDialog();
                                        break;

                                    case "HTF":
                                        HistoriasClinicas.Historia_TerFisica Htf = new HistoriasClinicas.Historia_TerFisica();
                                        Htf.Admision = Adm;
                                        Htf.ShowDialog();
                                        break;

                                    case "HPSI":
                                        HistoriasClinicas.Historia_Psicologia Hpsi = new HistoriasClinicas.Historia_Psicologia();
                                        Hpsi.Admision = Adm;
                                        Hpsi.ShowDialog();
                                        break;

                                    case "PSIQUIATRIA":
                                        MessageBox.Show("Esta historia de Psiquiatria debe elaborarse via WEB", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        break;

                                    case "PSIQUIATRIA2":
                                        MessageBox.Show("Esta historia de Psiquiatria debe elaborarse via WEB", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        break;

                                    case "PSIQUIATRIA3":
                                        MessageBox.Show("Esta historia de Psiquiatria debe elaborarse via WEB", "Informacion", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        break;

                                    default:
                                        timer1.Enabled = true;
                                        break;
                                }
                            }
                            if (result == DialogResult.No)
                            {
                                timer1.Enabled = true;
                            }
                        }
                        else
                        {
                            timer1.Enabled = true;
                        }
                    }                                        
                }
                else
                {
                    timer1.Enabled = true;
                }

                gridZH1.dataGridView1.ClearSelection();
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
                timer1.Enabled = true;
            }
        }
        private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {
                gridZH1.dataGridView1.ClearSelection();

                int Adm_Selected = Convert.ToInt32(gridZH1.dataGridView1.Rows[e.RowIndex].Cells[3].Value.ToString());
      
                if (e.Button == MouseButtons.Left)
                {
                    Selection_Active(Adm_Selected.ToString());
                }

                gridZH1.dataGridView1.ClearSelection();
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        void Pops()
        {
            try
            {
                string pacs = "";

                foreach (DataGridViewRow lvw in gridZH1.dataGridView1.Rows)
                {
                    if (gridZH1.dataGridView1.Rows[lvw.Index].Cells[5].Value.ToString() == "P")
                    {
                        if (checkBox1.Checked == true)
                        {
                            pacs = pacs + "\n\r" + gridZH1.dataGridView1.Rows[lvw.Index].Cells[4].Value.ToString();                            
                        }
                    }
                }

                if (pacs != "")
                {
                    PopupNotifier P = PopUps.setPopUp(Properties.Resources2.comprobado,
                                                          Color.LightGreen,
                                                          "PACIENTES ADMISIONADOS",
                                                          Color.DarkGreen,
                                                          pacs.ToString());
                    P.Popup();
                }               
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
                checkBox1.Checked = false;
            }
        }
        private void DataGridView1_MouseLeave(object sender, EventArgs e)
        {
            if (tooltipAgenda != null)
            {
                tooltipAgenda.Close();
                tooltipAgenda.Dispose();
                tooltipAgenda = null;
            }
        }
        private void DataGridView1_CellMouseEnter1(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                var grid = gridZH1.dataGridView1;

                if (e.RowIndex < 0 || e.ColumnIndex < 0)
                    return;

                object valor = grid.Rows[e.RowIndex].Cells[3].Value;

                if (valor == null || valor == DBNull.Value)
                    return;

                if (!int.TryParse(valor.ToString(), out int id))
                    return;

                if (DicDatos == null ||
                    !DicDatos.TryGetValue(id, out CXN_HORARIO data) ||
                    data == null)
                    return;

                // Cerrar el tooltip anterior.
                if (tooltipAgenda != null)
                {
                    tooltipAgenda.Close();
                    tooltipAgenda.Dispose();
                }

                // Crear el tooltip enriquecido.
                tooltipAgenda = new TooltipEnriquecido();

                tooltipAgenda.AgregarLinea(
                   "ADMISIÓN:",
                   Convert.ToString(data.Hor_AdmOpnened));

                tooltipAgenda.AgregarLinea(
                    "PACIENTE:",
                    Convert.ToString(data.Hor_Imp_Age));

                tooltipAgenda.AgregarLinea(
                   "USUARIO ASIGNA:",
                   Convert.ToString(data.Hor_Pac_UsrGraba));

                tooltipAgenda.AgregarLinea(
                   "USUARIO ADMISIONA:",
                   Convert.ToString(data.Hor_Usr_Admisiona));

                tooltipAgenda.AgregarLinea("", "");

                tooltipAgenda.AgregarLinea(
                    "SERVICIO:",
                    Convert.ToString(data.Hor_GrupoServicios));

                tooltipAgenda.AgregarLinea(
                    "ASEGURADORA:",
                    Convert.ToString(data.PacienteAseguradora));

                tooltipAgenda.AgregarLinea(
                   "TELEFONOS:",
                   Convert.ToString(data.DatosPaciente.Pac_Telefono + " - " + data.DatosPaciente.Pac_TelefonoAux));

                tooltipAgenda.AgregarLinea(
                   "IDENTIFICACION:",
                   Convert.ToString(data.DatosPaciente.Pac_TipoId + " " + data.DatosPaciente.Pac_IdNum));

                // Posición del cursor.
                Point posicion = Cursor.Position;
                posicion.Offset(15, 15);

                tooltipAgenda.MostrarEn(posicion);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public void Cargar_Agenda()
        {
            try
            {
                textBox1.Text = Convert.ToDateTime(dateTimePicker1.Value.Date).ToString(Conexion.ConectionDictionary["Format_Fecha"]);
                textBox2.Text = Convert.ToDateTime(dateTimePicker1.Value.Date).ToString("dddd");

                //llamar metodo de grilla
                CXN_HORARIO H = new CXN_HORARIO
                {
                    Hor_Pac_Bod = Med,
                    Hor_Pac_Cia = Cia,
                    Hor_Pac_Fecha_Cita = Convert.ToDateTime(textBox1.Text),
                    Hor_Observacion = textBox2.Text
                };

                SendMessage(gridZH1.dataGridView1.Handle, WM_SETREDRAW, false, 0);                

                List<CXN_HORARIO> getAgenda = repoAgendaProfesionales.Carga_Agenda(H);
                if (getAgenda != null)
                {
                    Encabezados();
                    DicDatos = new Dictionary<int, CXN_HORARIO>();

                    foreach (var i in getAgenda)
                    {
                        string EnfCitasMG = "";
                        string serviceName = ""; 
                        
                        if (!string.IsNullOrEmpty(i.Hor_Pac_Cup))
                        {
                            serviceName = repoConvenios.ServicioNombre(i.Hor_Pac_Cup, 
                                                                       i.Hor_Pac_Ase, 
                                                                       i.Hor_Pac_Tipo_Serv)?.Con_Nombre ?? "";
                        }

                        if (!string.IsNullOrEmpty(i.Hor_Estado))
                        {
                            if (i.Hor_Estado != "B")
                            {
                                DicDatos.Add(Convert.ToInt32(i.Hor_RegAtn), new CXN_HORARIO
                                {
                                    Hor_AdmOpnened = i.Hor_RegAtn,
                                    Hor_Imp_Age = i.Hor_Imp_Age,
                                    PacienteAseguradora = i.PacienteAseguradora,
                                    Hor_GrupoServicios = serviceName,
                                    Hor_Usr_Admisiona = i.Hor_Usr_Admisiona,
                                    Hor_Pac_UsrGraba = i.Hor_Pac_UsrGraba,
                                    DatosPaciente = new CXN_PACIENTES
                                    {
                                        Pac_Telefono = i.PacienteTelefono,
                                        Pac_TelefonoAux = i.Com_Telefono,
                                        Pac_TipoId = i.Com_Identificacion,
                                        Pac_IdNum = i.PacienteIdentificacion,
                                    }
                                });
                            }
                        }

                        if (this.MedGen == true)
                        {
                            if (i.Hor_Pac_Id != 0)
                            {
                                EnfCitasMG = (repoAgendaProfesionales.getEnfCitaMG(i.Hor_Pac_Id, dateTimePicker1.Value.Date) == "" ? "N/A" : repoAgendaProfesionales.getEnfCitaMG(i.Hor_Pac_Id, dateTimePicker1.Value.Date));
                            }

                            DataRow row = dt.NewRow();

                            row["IDE"] = i.Hor_Pac_Id_Hora.ToString();
                            row["Habilita"] = i.Hor_Autoriza.ToString();
                            row["Hora"] = Convert.ToDateTime(i.Hor_Pac_Hora).ToString("HH:mm tt");
                            row["Admision"] = i.Hor_RegAtn.ToString();
                            row["Paciente"] = i.Hor_Imp_Age.ToString();
                            row["Paciente_Est"] = i.Hor_Estado.ToString();
                            row["Modalidad"] = i.Hor_Pac_Modalidad.ToString();
                            row["Enfermero"] = EnfCitasMG.ToString();
                            row["InPaquete"] = i.Hor_IniciaSesion.ToString();
                            row["PacSal"] = i.Hor_Pac_Sal.ToString();
                            row["Aseguradora"] = i.PacienteAseguradora.ToString();
                            row["Arrastra"] = i.Hor_ArrastraHistoria.ToString();
                            row["Colores"] = i.Hor_Color.ToString();
                            row["CodePaciente"] = i.Hor_Pac_Id.ToString();
                            
                            dt.Rows.Add(row);
                            dt.AcceptChanges();
                        }
                        else
                        {
                            DataRow row = dt.NewRow();

                            row["IDE"] = i.Hor_Pac_Id_Hora.ToString();
                            row["Habilita"] = i.Hor_Autoriza.ToString();                           
                            row["Hora"] = Convert.ToDateTime(i.Hor_Pac_Hora).ToString("HH:mm tt");
                            row["Admision"] = i.Hor_RegAtn.ToString();
                            row["Paciente"] = i.Hor_Imp_Age.ToString();
                            row["Paciente_Est"] = i.Hor_Estado.ToString();
                            row["Modalidad"] = i.Hor_Pac_Modalidad.ToString();
                            row["Aseguradora"] = i.PacienteAseguradora.ToString();
                            row["InPaquete"] = i.Hor_IniciaSesion.ToString();
                            row["PacSal"] = i.Hor_Pac_Sal.ToString();
                            row["Arrastra"] = i.Hor_ArrastraHistoria.ToString();
                            row["Colores"] = i.Hor_Color.ToString();
                            row["CodePaciente"] = i.Hor_Pac_Id.ToString();

                            dt.Rows.Add(row);
                            dt.AcceptChanges();
                        }
                    }

                    Estilos(gridZH1.dataGridView1, dt);

                    //Colores Agenda
                    foreach (DataGridViewRow row in gridZH1.dataGridView1.Rows)
                    {
                        string Estado = gridZH1.dataGridView1.Rows[row.Index].Cells[5].Value.ToString();
                        string EstadoHora = gridZH1.dataGridView1.Rows[row.Index].Cells[1].Value.ToString();

                        if (EstadoHora == "A")
                        {
                            row.Cells[2].Style.BackColor = Color.LightGray;
                            row.Cells[3].Style.BackColor = Color.LightGray;
                            row.Cells[2].Style.ForeColor = Color.Green;
                        }
                        else 
                        {
                            row.Cells[2].Style.BackColor = Color.LightGray;
                            row.Cells[3].Style.BackColor = Color.LightGray;
                            row.Cells[2].Style.ForeColor = Color.Red;
                        }

                        if (this.Colorimetria == true)
                        {
                            if (Estado == "P")
                            {
                                string colorFila = row.Cells["Colores"].Value.ToString();

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

                                    case "T": //Cierra Orden
                                        row.Cells["Paciente"].Style.BackColor = Color.DarkBlue;
                                        row.Cells["Paciente"].Style.ForeColor = Color.White;
                                        break;

                                    default: //Controles  //Control Normal
                                        row.Cells["Paciente"].Style.BackColor = Color.LightGreen;
                                        row.Cells["Paciente"].Style.ForeColor = Color.Green;
                                        break;
                                }
                            }
                            else if (Estado == "A")
                            {
                                row.Cells[4].Style.BackColor = Color.White;
                                row.Cells[6].Style.BackColor = Color.White;
                                row.Cells[7].Style.BackColor = Color.White;
                                row.Cells[4].Style.ForeColor = Color.Black;
                                row.Cells[6].Style.ForeColor = Color.Black;
                                row.Cells[7].Style.ForeColor = Color.Black;

                                if (this.MedGen == true)
                                {
                                    row.Cells[10].Style.BackColor = Color.White;
                                    row.Cells[10].Style.ForeColor = Color.Black;
                                }
                            }
                            else if (Estado == "B")
                            {
                                row.Cells[4].Style.BackColor = Color.Orange;
                                row.Cells[6].Style.BackColor = Color.Orange;
                                row.Cells[7].Style.BackColor = Color.Orange;
                                row.Cells[4].Style.ForeColor = Color.Red;
                                row.Cells[6].Style.ForeColor = Color.Red;
                                row.Cells[7].Style.ForeColor = Color.Red;

                                if (this.MedGen == true)
                                {
                                    row.Cells[10].Style.BackColor = Color.Orange;
                                    row.Cells[10].Style.ForeColor = Color.Red;
                                }
                            }
                            else if (Estado == "H")
                            {
                                row.Cells[4].Style.BackColor = Color.LightBlue;
                                row.Cells[6].Style.BackColor = Color.LightBlue;
                                row.Cells[7].Style.BackColor = Color.LightBlue;
                                row.Cells[4].Style.ForeColor = Color.Blue;
                                row.Cells[6].Style.ForeColor = Color.Blue;
                                row.Cells[7].Style.ForeColor = Color.Blue;

                                if (this.MedGen == true)
                                {
                                    row.Cells[10].Style.BackColor = Color.LightBlue;
                                    row.Cells[10].Style.ForeColor = Color.Blue;
                                }
                            }
                        } //termina colorimetria
                        else
                        {
                            if (Estado == "P")
                            {
                                row.Cells[4].Style.BackColor = Color.LightGreen;
                                row.Cells[6].Style.BackColor = Color.LightGreen;
                                row.Cells[7].Style.BackColor = Color.LightGreen;
                                row.Cells[4].Style.ForeColor = Color.Green;
                                row.Cells[6].Style.ForeColor = Color.Green;
                                row.Cells[7].Style.ForeColor = Color.Green;

                                if (this.MedGen == true)
                                {
                                    row.Cells[10].Style.BackColor = Color.LightGreen;
                                    row.Cells[10].Style.ForeColor = Color.Green;
                                }
                            }
                            if (Estado == "A")
                            {
                                row.Cells[4].Style.BackColor = Color.White;
                                row.Cells[6].Style.BackColor = Color.White;
                                row.Cells[7].Style.BackColor = Color.White;
                                row.Cells[4].Style.ForeColor = Color.Black;
                                row.Cells[6].Style.ForeColor = Color.Black;
                                row.Cells[7].Style.ForeColor = Color.Black;

                                if (this.MedGen == true)
                                {
                                    row.Cells[10].Style.BackColor = Color.White;
                                    row.Cells[10].Style.ForeColor = Color.Black;
                                }
                            }
                            if (Estado == "B")
                            {
                                row.Cells[4].Style.BackColor = Color.Orange;
                                row.Cells[6].Style.BackColor = Color.Orange;
                                row.Cells[7].Style.BackColor = Color.Orange;
                                row.Cells[4].Style.ForeColor = Color.Red;
                                row.Cells[6].Style.ForeColor = Color.Red;
                                row.Cells[7].Style.ForeColor = Color.Red;

                                if (this.MedGen == true)
                                {
                                    row.Cells[10].Style.BackColor = Color.Orange;
                                    row.Cells[10].Style.ForeColor = Color.Red;
                                }
                            }
                            if (Estado == "H")
                            {
                                row.Cells[4].Style.BackColor = Color.LightBlue;
                                row.Cells[6].Style.BackColor = Color.LightBlue;
                                row.Cells[7].Style.BackColor = Color.LightBlue;
                                row.Cells[4].Style.ForeColor = Color.Blue;
                                row.Cells[6].Style.ForeColor = Color.Blue;
                                row.Cells[7].Style.ForeColor = Color.Blue;

                                if (this.MedGen == true)
                                {
                                    row.Cells[10].Style.BackColor = Color.LightBlue;
                                    row.Cells[10].Style.ForeColor = Color.Blue;
                                }
                            }
                        }
                    }
                }
                else
                {
                    Encabezados();
                }

                SendMessage(gridZH1.dataGridView1.Handle, WM_SETREDRAW, true, 0);
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        void Estilos(DataGridView D, DataTable t)
        { 
            try
            {
                D.Font = new Font("Arial", 9, FontStyle.Regular);
                D.EnableHeadersVisualStyles = false;
                D.ScrollBars = ScrollBars.Both;

                D.DataSource = t;

                if (ToolTipCitas == false)
                {
                    D.Columns["Modalidad"].Visible = true;
                    D.Columns["Aseguradora"].Visible = true;
                   
                }
                else
                {
                    D.Columns["Modalidad"].Visible = false;
                    D.Columns["Aseguradora"].Visible = false;
                    D.Columns["Paciente"].Width = 470;
                }

                if (this.MedGen == true)
                {
                    D.Columns["IDE"].Width = 0;
                    D.Columns["Habilita"].Width = 0;
                    D.Columns["Hora"].Width = 110;
                    D.Columns["Admision"].Width = 100;
                    //D.Columns["Paciente"].Width = 470;
                    D.Columns["Paciente_Est"].Width = 0;
                    D.Columns["Modalidad"].Width = 100;
                    D.Columns["Enfermero"].Width = 470;
                    D.Columns["InPaquete"].Width = 0;
                    D.Columns["PacSal"].Width = 0;
                    D.Columns["Aseguradora"].Width = 440;

                    D.Columns["Enfermero"].SortMode = DataGridViewColumnSortMode.NotSortable;
                    D.Columns["Enfermero"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

                    if (ToolTipCitas == false)
                    {
                        D.Columns["Paciente"].Width = 370;
                    }
                    else
                    {
                        D.Columns["Paciente"].Width = 470;
                    }
                }
                else
                {
                    D.Columns["IDE"].Width = 0;
                    D.Columns["Habilita"].Width = 0;
                    D.Columns["Hora"].Width = 110;
                    D.Columns["Admision"].Width = 100;
                    //D.Columns["Paciente"].Width = 940;
                    D.Columns["Paciente_Est"].Width = 0;
                    D.Columns["Modalidad"].Width = 100;
                    D.Columns["Aseguradora"].Width = 440;
                    D.Columns["InPaquete"].Width = 0;
                    D.Columns["PacSal"].Width = 0;

                    if (ToolTipCitas == false)
                    {
                        D.Columns["Paciente"].Width = 370;
                    }
                    else
                    {
                        D.Columns["Paciente"].Width = 940;
                    }
                }

                D.ColumnHeadersDefaultCellStyle.Font = new Font(D.Font, FontStyle.Bold);
                D.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 8, FontStyle.Bold);
                D.ColumnHeadersDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#bfdbff");
                D.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;
                D.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;

                D.Columns["Hora"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                D.Columns["Admision"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                D.Columns["Paciente"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                D.Columns["Paciente_Est"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                D.Columns["Modalidad"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                D.Columns["InPaquete"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                D.Columns["PacSal"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                D.Columns["Aseguradora"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

                D.Columns["Hora"].SortMode = DataGridViewColumnSortMode.NotSortable;
                D.Columns["Admision"].SortMode = DataGridViewColumnSortMode.NotSortable;
                D.Columns["Paciente"].SortMode = DataGridViewColumnSortMode.NotSortable;
                D.Columns["Paciente_Est"].SortMode = DataGridViewColumnSortMode.NotSortable;
                D.Columns["Modalidad"].SortMode = DataGridViewColumnSortMode.NotSortable;
                D.Columns["InPaquete"].SortMode = DataGridViewColumnSortMode.NotSortable;
                D.Columns["PacSal"].SortMode = DataGridViewColumnSortMode.NotSortable;
                D.Columns["Aseguradora"].SortMode = DataGridViewColumnSortMode.NotSortable;

                D.Columns["IDE"].Visible = false;
                D.Columns["Habilita"].Visible = false;
                D.Columns["Paciente_Est"].Visible = false;
                D.Columns["InPaquete"].Visible = false;
                D.Columns["PacSal"].Visible = false;
                D.Columns["Arrastra"].Visible = false;
                D.Columns["Colores"].Visible = false;
                D.Columns["CodePaciente"].Visible = false;               

                D.ClearSelection();
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        void Encabezados()
        {
            gridZH1.dataGridView1.DataSource = null;
            dt = new DataTable();

            if (this.MedGen == true)
            {
                IDE = dt.Columns.Add("IDE", typeof(string));
                Habilita = dt.Columns.Add("Habilita", typeof(string));
                Hora = dt.Columns.Add("Hora", typeof(string));
                Admision = dt.Columns.Add("Admision", typeof(string));
                Paciente = dt.Columns.Add("Paciente", typeof(string));
                Paciente_Est = dt.Columns.Add("Paciente_Est", typeof(string));
                Modalidad = dt.Columns.Add("Modalidad", typeof(string));
                Enfermero = dt.Columns.Add("Enfermero", typeof(string));
                InPaquete = dt.Columns.Add("InPaquete", typeof(string));
                PacSal = dt.Columns.Add("PacSal", typeof(string));
                Aseguradora = dt.Columns.Add("Aseguradora", typeof(string));
                Arrastra = dt.Columns.Add("Arrastra", typeof(string));
                Colores = dt.Columns.Add("Colores", typeof(string)); //12
                CodePaciente = dt.Columns.Add("CodePaciente", typeof(string));
            }
            else
            {
                IDE = dt.Columns.Add("IDE", typeof(string));
                Habilita = dt.Columns.Add("Habilita", typeof(string));
                Hora = dt.Columns.Add("Hora", typeof(string));
                Admision = dt.Columns.Add("Admision", typeof(string));
                Paciente = dt.Columns.Add("Paciente", typeof(string));
                Paciente_Est = dt.Columns.Add("Paciente_Est", typeof(string));
                Modalidad = dt.Columns.Add("Modalidad", typeof(string));
                Aseguradora = dt.Columns.Add("Aseguradora", typeof(string));
                InPaquete = dt.Columns.Add("InPaquete", typeof(string));
                PacSal = dt.Columns.Add("PacSal", typeof(string));
                Arrastra = dt.Columns.Add("Arrastra", typeof(string));
                Colores = dt.Columns.Add("Colores", typeof(string)); //11
                CodePaciente = dt.Columns.Add("CodePaciente", typeof(string));
            }            
        }
        private void timer3_Tick(object sender, EventArgs e)
        {
            try
            {
                if (checkBox1.Checked == true)
                {
                    Pops();
                }                
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
                checkBox1.Checked = false;
            }
        }
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true ) { timer3.Enabled = true; }
            if (checkBox1.Checked ==false ) { timer3.Enabled = false; }
        }
        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            try
            {
                MensajesGeneral MG = new MensajesGeneral();

                if (Conexion.ConectionDictionary["Videoconferencia"] == "A")
                {
                    List<CXN_IMAGEN_SYSTEM> d = repoConfSystem.getConfSystem();
                    if (d != null)
                    {
                        CXN_IMAGEN_SYSTEM d2 = d.FirstOrDefault(x => x.Tab_Nombre == "Videoconferencia");
                        if (d2 != null)
                        {
                            if (d2.Tab_Clave == "A")
                            {
                                HistorialTwilio historialTwilio = new HistorialTwilio();
                                historialTwilio.ShowDialog();
                            }
                            else
                            {
                                MG.Mensaje = "El permiso no esta autorizado";
                                MG.TipoImagen = 1000;
                                MG.ShowDialog();
                            }
                        }
                        else
                        {
                            MG.Mensaje = "El permiso no esta autorizado";
                            MG.TipoImagen = 1000;
                            MG.ShowDialog();
                        }
                    }
                    else
                    {
                        MG.Mensaje = "El permiso no esta autorizado";
                        MG.TipoImagen = 1000;
                        MG.ShowDialog();
                    }                    
                }
                else
                {
                    MG.Mensaje = "El permiso no esta autorizado";
                    MG.TipoImagen = 1000;
                    MG.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void AgendaM_MouseLeave(object sender, EventArgs e)
        {
            if (tooltipAgenda != null)
            {
                tooltipAgenda.Close();
                tooltipAgenda.Dispose();
                tooltipAgenda = null;
            }
        }
        private void gridZH1_MouseLeave(object sender, EventArgs e)
        {
            if (tooltipAgenda != null)
            {
                tooltipAgenda.Close();
                tooltipAgenda.Dispose();
                tooltipAgenda = null;
            }
        }
        private void Cargacia()
        {
            var cias = repoCompañia.getAllCompañias();
            if (cias != null)
            {
                foreach (var i in cias)
                {
                    comboBox1.Items.Add(i.Com_Nombre);
                }

                comboBox1.SelectedIndex = 0;
            }
        }
    }
}
