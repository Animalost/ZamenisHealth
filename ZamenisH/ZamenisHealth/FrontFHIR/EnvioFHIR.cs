using APIController.Services.FHIR_IHCE;
using APIFhir.Controlador;
using APIFhir.Servicio;
using Domain.CXN;
using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZamenisHealth.Comunes;
using ZamenisHealth.FrontFHIR.RDAs;
using DataTable = System.Data.DataTable;
using ScrollBars = System.Windows.Forms.ScrollBars;

namespace ZamenisHealth.FrontFHIR
{
    public partial class EnvioFHIR : Forma
    {
        private readonly ICompañia repoCia;
        private readonly IFHIR repoFHIR;
        private readonly IAgendaC repoAgeC;
        private readonly CreateToken repoCrearToken;

        private int CodePrestador;
        private CXN_CIA DatosPrestador;
        private MensajesGeneral MG;

        DataTable dt;
        DataColumn POS;
        DataColumn CheckForSend;
        DataColumn Admision;
        DataColumn Paciente;
        DataColumn FechaRDAPaciente;
        DataColumn FechaRDAAmbulatorio;
        DataColumn PersonaRDAPaciente;
        DataColumn PersonaRDAAmbulatorio;

        ToolStripButton btnConsultar;
        ToolStripButton btnMarcarTodo;
        ToolStripButton btnDesMarcarTodo;
        ToolStripButton btnRDAPaciente;
        ToolStripButton btnRDACExterna;
        ToolStripButton btnLog;

        public EnvioFHIR()
        {
            InitializeComponent();
            repoCia = new MCompañia();
            repoFHIR = new MFHIR();
            repoAgeC = new MAgendaC();
            repoCrearToken = new EndPoint_Token();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Titulo.Text = "IHCE Ministerio de Salud Colombia";
            LogoMain.Image = Properties.Resources.Splash;
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";

            btnConsultar = new ToolStripButton();
            btnConsultar = createToolButton("Consultar");
            MenuLateral.Items.Add(btnConsultar);
            btnConsultar.Click += toolStripButton1_Click;

            btnMarcarTodo = new ToolStripButton();
            btnMarcarTodo = createToolButton("Marcar Todo");
            MenuLateral.Items.Add(btnMarcarTodo);
            btnMarcarTodo.Click += toolStripButton3_Click;

            btnDesMarcarTodo = new ToolStripButton();
            btnDesMarcarTodo = createToolButton("Desmarcar Todo");
            MenuLateral.Items.Add(btnDesMarcarTodo);
            btnDesMarcarTodo.Click += toolStripButton4_Click;

            btnRDAPaciente = new ToolStripButton();
            btnRDAPaciente = createToolButton("RDA Paciente");
            MenuLateral.Items.Add(btnRDAPaciente);
            btnRDAPaciente.Click += toolStripButton5_Click;

            btnRDACExterna = new ToolStripButton();
            btnRDACExterna = createToolButton("RDA C. Externa");
            MenuLateral.Items.Add(btnRDACExterna);
            btnRDACExterna.Click += toolStripButton7_Click;

            btnLog = new ToolStripButton();
            btnLog = createToolButton("Logs");
            MenuLateral.Items.Add(btnLog);
            btnLog.Click += btnLog_Click;

            gridZH1.CeldaHeight = true;
            gridZH1.dataGridView1.BorderStyle = BorderStyle.None;

            comboBox2.Items.Add("Medicina General");
            comboBox2.Items.Add("Fisiatria");

            CargarCia();

            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
        } 
        void btnLog_Click(object sender, EventArgs e)
        {
            Logs log = new Logs();
            log.ShowDialog();
        }
        void CargarCia()
        {
            try
            {
                List<CXN_CIA> getPrestadores = repoCia.getAllCompañias();
                if (getPrestadores == null)
                {
                    MG = new MensajesGeneral
                    {
                        Mensaje = "Error al cargar los prestadores",
                        TipoImagen = 1000
                    };
                    MG.ShowDialog();
                }
                else
                {
                    foreach (CXN_CIA i in getPrestadores)
                    {
                        comboBox1.Items.Add(i.Com_Nombre);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        void Consultar()
        {
            try
            {
                MG = new MensajesGeneral();

                if (CodePrestador == 0)
                {
                    MG.Mensaje = "Debe seleccionar un prestador";
                    MG.TipoImagen = 1000;
                    MG.ShowDialog();
                }
                else if (comboBox2.Text == "")
                {
                    MG.Mensaje = "Debe seleccionar un tipo de especialidad";
                    MG.TipoImagen = 1000;
                    MG.ShowDialog();
                }
                else
                {
                    pictureBox1.Visible = true;
                    btnConsultar.Enabled = false;

                    switch (comboBox2.Text)
                    {
                        case "Medicina General":
                            Filtrar("MG");
                            break;
                        case "Enfermeria":
                            Filtrar("CU");
                            break;
                        case "Radiologia":
                            Filtrar("RA");
                            break;
                        case "Psicologia":
                            Filtrar("PS");
                            break;
                        case "Fisiatria":
                            Filtrar("FI");
                            break;
                        case "Terapia Ocupacional":
                            Filtrar("TO");
                            break;
                        case "Terapia Fisica":
                            Filtrar("TF");
                            break;
                        case "Notas Aclaratorias Medicina General":
                            Filtrar("NAMG");
                            break;
                        default:
                            MG = new MensajesGeneral();
                            MG.Mensaje = "Especialidad no contemplada";
                            MG.TipoImagen = 1000;
                            MG.ShowDialog();
                            break;
                    }

                    pictureBox1.Visible = false;
                    btnConsultar.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            Consultar();
        }
        void Estilos(DataGridView D, DataTable t)
        {
            D.EnableHeadersVisualStyles = false;
            D.ScrollBars = ScrollBars.Both;

            D.DataSource = t;

            D.ReadOnly = false;
            D.Columns["CheckForSend"].ReadOnly = false;

            D.Columns["Admision"].ReadOnly = true;
            D.Columns["Paciente"].ReadOnly = true;
            //D.Columns["RDAPaciente"].ReadOnly = true;
            //D.Columns["RDAAmbulatorio"].ReadOnly = true;
            D.Columns["FechaRDAPaciente"].ReadOnly = true;
            D.Columns["FechaRDAAmbulatorio"].ReadOnly = true;
            D.Columns["PersonaRDAPaciente"].ReadOnly = true;
            D.Columns["PersonaRDAAmbulatorio"].ReadOnly = true;

            D.Columns["POS"].Visible = false;

            gridZH1.dataGridView1.ClearSelection();
        }
        void Encabezados()
        {
            gridZH1.dataGridView1.DataSource = null;
            dt = new DataTable();
            POS = dt.Columns.Add("POS", typeof(int));
            CheckForSend = dt.Columns.Add("CheckForSend", typeof(bool));
            Admision = dt.Columns.Add("Admision", typeof(int));
            Paciente = dt.Columns.Add("Paciente", typeof(string));
            FechaRDAPaciente = dt.Columns.Add("FechaRDAPaciente", typeof(string));
            FechaRDAAmbulatorio = dt.Columns.Add("FechaRDAAmbulatorio", typeof(string));
            PersonaRDAPaciente = dt.Columns.Add("PersonaRDAPaciente", typeof(string));
            PersonaRDAAmbulatorio = dt.Columns.Add("PersonaRDAAmbulatorio", typeof(string));
        }
        void Filtrar(string Especialidad)
        {
            try
            {
                List<CXN_HORARIO> getRDA = repoFHIR.FiltrarEspecialidad(dateTimePicker1.Value.Date, Especialidad);
                if (getRDA == null)
                {
                    Encabezados();
                }
                else
                {
                    Encabezados();

                    int Contador = 1;

                    foreach (CXN_HORARIO i in getRDA)
                    {
                        DataRow row = dt.NewRow();

                        row["POS"] = Contador;
                        row["CheckForSend"] = false;
                        row["Admision"] = i.Hor_Id;
                        row["Paciente"] = i.Hor_Imp_Age.ToString();
                        row["FechaRDAPaciente"] = i.Hor_Autoriza;
                        row["FechaRDAAmbulatorio"] = i.Hor_RegAtn;
                        row["PersonaRDAPaciente"] = i.Hor_Usr_Admisiona;
                        row["PersonaRDAAmbulatorio"] = i.PacienteAseguradora;

                        dt.Rows.Add(row);
                        dt.AcceptChanges();

                        Contador = Contador + 1;
                    }

                    Contador = 1;
                    Estilos(gridZH1.dataGridView1, dt);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                DatosPrestador = new CXN_CIA();
                DatosPrestador = repoCia.getPrestadorbyName(comboBox1.Text);
                if (DatosPrestador == null)
                {
                    MG = new MensajesGeneral
                    {
                        Mensaje = "Error al cargar los datos del prestador",
                        TipoImagen = 1000
                    };
                    MG.ShowDialog();
                }
                else
                {
                    CodePrestador = DatosPrestador.Com_Identificador;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (DataGridViewRow row in gridZH1.dataGridView1.Rows)
                {
                    object valor = row.Cells["CheckForSend"].Value;
                    bool isChecked = valor != null && Convert.ToBoolean(valor);

                    if (!isChecked)
                    {
                        row.Cells["CheckForSend"].Value = true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (DataGridViewRow row in gridZH1.dataGridView1.Rows)
                {
                    object valor = row.Cells["CheckForSend"].Value;
                    bool isChecked = valor != null && Convert.ToBoolean(valor);

                    if (isChecked)
                    {
                        row.Cells["CheckForSend"].Value = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private async void toolStripButton5_Click(object sender, EventArgs e)
        {
            pictureBox1.Visible = true;
            try
            {
                await Autoriza();
                gridZH1.dataGridView1.EndEdit();

                foreach (DataGridViewRow row in gridZH1.dataGridView1.Rows)
                {
                    object valor = row.Cells["CheckForSend"].Value;
                    bool isChecked = valor != null && Convert.ToBoolean(valor);

                    int admision = Convert.ToInt32(row.Cells["Admision"].Value);    

                    if (isChecked == true)
                    {
                        if (repoFHIR.VerificarEnvio(admision, "Paciente") == false)
                        {
                            CrearBundlePaciente(admision);
                        }                       
                    }
                }

                Consultar();

                MG = new MensajesGeneral();
                MG.Mensaje = "Proceso de envio finalizado";
                MG.TipoImagen = 3;
                MG.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            pictureBox1.Visible = false;
        }
        async void CrearBundlePaciente(int Admision)
        {
            try
            {
                otrosDatosPacienteHorario datosCita = repoAgeC.cargarAdmision(Admision, "'H'");
                if (datosCita == null)
                {
                    MG = new MensajesGeneral
                    {
                        Mensaje = "Error al cargar los datos de la admision " + Admision + " probablemente no tiene historia asociada aun",
                        TipoImagen = 1000
                    };
                    MG.ShowDialog();
                    return; //Admision no H
                }
                else
                {
                    FrontFHIR.RDAs.RDAPaciente rda = new FrontFHIR.RDAs.RDAPaciente();
                    rda.EnviarRDAPaciente(Admision);
                }                  
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }  
        async Task Autoriza()
        {
            try
            {
                if (comboBox1.Text == "" || CodePrestador == 0)
                {
                    MG = new MensajesGeneral();
                    MG.Mensaje = "Debe seleccionar un prestador";
                    MG.TipoImagen = 1000;
                    MG.ShowDialog();
                    return;
                }

                CXN_TOKENS_FHIR getToken = repoFHIR.RecuperarClaseToken(CodePrestador);
                if (getToken == null)
                {
                    await repoCrearToken.ObtenerTokenIHCE(CodePrestador);
                }
                else if (DateTime.Now >= getToken.Fecha.AddHours(1))
                {
                    await repoCrearToken.ObtenerTokenIHCE(CodePrestador);
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private async void toolStripButton7_Click(object sender, EventArgs e)
        {
            pictureBox1.Visible = true;
            try
            {                
                await Autoriza();
                gridZH1.dataGridView1.EndEdit();

                foreach (DataGridViewRow row in gridZH1.dataGridView1.Rows)
                {
                    object valor = row.Cells["CheckForSend"].Value;
                    bool isChecked = valor != null && Convert.ToBoolean(valor);

                    int admision = Convert.ToInt32(row.Cells["Admision"].Value);

                    if (isChecked == true)
                    {
                        if (repoFHIR.VerificarEnvio(admision, "CExterna") == false)
                        {
                            CrearBundleCE(admision);
                        }
                    }
                }

                Consultar();

                MG = new MensajesGeneral();
                MG.Mensaje = "Proceso de envio finalizado";
                MG.TipoImagen = 3;
                MG.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            pictureBox1.Visible = false;
        }
        async void CrearBundleCE(int Admision)
        {
            try
            {
                RDAConsultaExterna rda = new RDAConsultaExterna();
                await rda.RadicarRDA(Admision, comboBox2.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
