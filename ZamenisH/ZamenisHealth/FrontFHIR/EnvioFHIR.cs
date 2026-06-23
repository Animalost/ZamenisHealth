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
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZamenisHealth.Comunes;
using ZamenisHealth.FrontFHIR.RDAs;

using DataTable = System.Data.DataTable;
using Font = System.Drawing.Font;
using ScrollBars = System.Windows.Forms.ScrollBars;

namespace ZamenisHealth.FrontFHIR
{
    public partial class EnvioFHIR : Forma
    {
        private readonly ICompañia repoCia;
        private readonly IFHIR repoFHIR;
        private readonly IAgendaC repoAgeC;
        private readonly CreateToken repoCrearToken;
        private readonly IConfSystem repoConfSystem;
        private API_FHIR apiFHIR;

        private int CodePrestador;
        private CXN_CIA DatosPrestador;
        private MensajesGeneral MG;

        DataTable dt;
        DataColumn POS;
        DataColumn CheckForSend;
        DataColumn Admision;
        DataColumn Paciente;
        DataColumn RDAPaciente;
        DataColumn RDAAmbulatorio;
        DataColumn FechaRDAPaciente;
        DataColumn FechaRDAAmbulatorio;
        DataColumn PersonaRDAPaciente;
        DataColumn PersonaRDAAmbulatorio;

        ToolStripButton btnConsultar;
        ToolStripButton btnMarcarTodo;
        ToolStripButton btnDesMarcarTodo;
        ToolStripButton btnRDAPaciente;
        ToolStripButton btnRDACExterna;
       // ToolStripButton btnRDACuraciones;
       // ToolStripButton btnRDANotaAclaratoriaMG;
        ToolStripButton btnLog;

        //ToolStripButton btnSendRDAPaciente;
        //ToolStripButton btnSendRDAAmbulatorio;

        public EnvioFHIR()
        {
            InitializeComponent();
            repoCia = new MCompañia();
            repoFHIR = new MFHIR();
            repoAgeC = new MAgendaC();
            repoCrearToken = new EndPoint_Token();
            repoConfSystem = new MConfSystem();
            apiFHIR = new API_FHIR();
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

            //PRUEBAs
          /*  btnSendRDAPaciente = new ToolStripButton();
            btnSendRDAPaciente = createToolButton("RDA Paciente");
            MenuLateral.Items.Add(btnSendRDAPaciente);
            btnSendRDAPaciente.Click += SendRDAPaciente;

            btnSendRDAAmbulatorio = new ToolStripButton();
            btnSendRDAAmbulatorio = createToolButton("RDA Ambulatorio");
            MenuLateral.Items.Add(btnSendRDAAmbulatorio);
            btnSendRDAAmbulatorio.Click += SendRDAAmbulatorio;*/
            //FIN PRUEBAS

             btnRDACExterna = new ToolStripButton();
             btnRDACExterna = createToolButton("RDA C. Externa");
             MenuLateral.Items.Add(btnRDACExterna);
             btnRDACExterna.Click += toolStripButton7_Click;

          /*  btnRDACuraciones = new ToolStripButton();
            btnRDACuraciones = createToolButton("RDA Curaciones");
            MenuLateral.Items.Add(btnRDACuraciones);
            btnRDACuraciones.Click += toolStripButton9_Click;*/

          /*  btnRDANotaAclaratoriaMG = new ToolStripButton();
            btnRDANotaAclaratoriaMG = createToolButton("RDA N. Aclaratoria MG");
            MenuLateral.Items.Add(btnRDANotaAclaratoriaMG);
            btnRDANotaAclaratoriaMG.Click += toolStripButton8_Click;  */

            btnLog = new ToolStripButton();
            btnLog = createToolButton("Logs");
            MenuLateral.Items.Add(btnLog);
            btnLog.Click += btnLog_Click;

            dataGridView1.BorderStyle = BorderStyle.None;

            comboBox2.Items.Add("Medicina General");
            comboBox2.Items.Add("Fisiatria");
            //comboBox2.Items.Add("Enfermeria");
            //comboBox2.Items.Add("Notas Aclaratorias Medicina General");

            //FIN BLOQUEOS

            CargarCia();

            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
        }
     
        #region RadicarRDA Paciente Nuevo
        async void SendRDAPaciente(object sender, EventArgs e)
        {
            try
            {
                //obtenerToken
                string data = repoConfSystem.getListado()["API_IHCE_Zamenis"];

                //DatosFHIR
                List<CXN_DATOS_FHIR> getDataConf = repoFHIR.ListaDatosConfFHIR(CodePrestador);
                if (getDataConf != null)
                {
                    ReceiveToken R = new ReceiveToken()
                    {
                        ClientID = getDataConf.FirstOrDefault(x => x.Llave == "ClientID").Valor,
                        ClientSecret = getDataConf.FirstOrDefault(x => x.Llave == "ClientSecret").Valor,
                        NitPrestador = repoCia.getPrestadorbyCode(CodePrestador).Com_Identificacion,
                        Scope = getDataConf.FirstOrDefault(x => x.Llave == "Scope").Valor,
                        TenantID = getDataConf.FirstOrDefault(x => x.Llave == "TenantID").Valor,
                        URLToken = getDataConf.FirstOrDefault(x => x.Llave == "URLToken").Valor
                    };

                    (bool Status, string Respuesta) res = await apiFHIR.GetToken(R, Program.URLApiConexion);
                    if (res.Status == false)
                    {
                        MG = new MensajesGeneral()
                        {
                            Mensaje = res.Respuesta,
                            TipoImagen = 1000
                        };
                        MG.ShowDialog();
                    }
                    else
                    {
                        CXN_TOKENS_FHIR T = new CXN_TOKENS_FHIR()
                        {
                            Fecha = DateTime.Now.Date,
                            Prestador = CodePrestador,
                            Token = res.Respuesta.ToString().Trim(),
                        };

                        bool inserta = repoFHIR.InsertarToken(T);
                        if (inserta == false)
                        {
                            MG = new MensajesGeneral()
                            {
                                Mensaje = "No se logro grabar el Token pero si se genero, contacte a soporte",
                                TipoImagen = 1000
                            };
                            MG.ShowDialog();
                        }                   
                        else
                        {
                            RadicarRDA.RDAPaciente_IHCE radRDAPaciente = new RadicarRDA.RDAPaciente_IHCE();
                            dataGridView1.EndEdit();

                            foreach (DataGridViewRow row in dataGridView1.Rows)
                            {
                                //bool isChecked = Convert.ToBoolean(row.Cells["CheckForSend"].Value);
                                object valor = row.Cells["CheckForSend"].Value;
                                bool isChecked = valor != null && Convert.ToBoolean(valor);

                                string rdapaciente = row.Cells["RDAPaciente"].Value.ToString();
                                string rdaambulatorio = row.Cells["RDAAmbulatorio"].Value.ToString();
                                int admision = Convert.ToInt32(row.Cells["Admision"].Value);

                                if (isChecked == true)
                                {
                                    if (string.IsNullOrEmpty(rdapaciente))
                                    {
                                        radRDAPaciente.EnviarRDAPaciente(admision);
                                    }
                                }
                            }

                            await Consultar();

                            MG = new MensajesGeneral();
                            MG.Mensaje = "Proceso de envio finalizado";
                            MG.TipoImagen = 3;
                            MG.ShowDialog();
                        }
                    }
                }
                else
                {
                    MG = new MensajesGeneral()
                    {
                        Mensaje = "No hay datos de configuracion del Ministerio de Salud, contacte al desarrollador del sistema",
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
        async void SendRDAAmbulatorio(object sender, EventArgs e)
        {
            try
            {
                //obtenerToken
                string data = repoConfSystem.getListado()["API_IHCE_Zamenis"];

                //DatosFHIR
                List<CXN_DATOS_FHIR> getDataConf = repoFHIR.ListaDatosConfFHIR(CodePrestador);
                if (getDataConf != null)
                {
                    ReceiveToken R = new ReceiveToken()
                    {
                        ClientID = getDataConf.FirstOrDefault(x => x.Llave == "ClientID").Valor,
                        ClientSecret = getDataConf.FirstOrDefault(x => x.Llave == "ClientSecret").Valor,
                        NitPrestador = repoCia.getPrestadorbyCode(CodePrestador).Com_Identificacion,
                        Scope = getDataConf.FirstOrDefault(x => x.Llave == "Scope").Valor,
                        TenantID = getDataConf.FirstOrDefault(x => x.Llave == "TenantID").Valor,
                        URLToken = getDataConf.FirstOrDefault(x => x.Llave == "URLToken").Valor
                    };

                    (bool Status, string Respuesta) res = await apiFHIR.GetToken(R, Program.URLApiConexion);
                    if (res.Status == false)
                    {
                        MG = new MensajesGeneral()
                        {
                            Mensaje = res.Respuesta,
                            TipoImagen = 1000
                        };
                        MG.ShowDialog();
                    }
                    else
                    {
                        CXN_TOKENS_FHIR T = new CXN_TOKENS_FHIR()
                        {
                            Fecha = DateTime.Now.Date,
                            Prestador = CodePrestador,
                            Token = res.Respuesta.ToString().Trim(),
                        };

                        bool inserta = repoFHIR.InsertarToken(T);
                        if (inserta == false)
                        {
                            MG = new MensajesGeneral()
                            {
                                Mensaje = "No se logro grabar el Token pero si se genero, contacte a soporte",
                                TipoImagen = 1000
                            };
                            MG.ShowDialog();
                        }
                        else
                        {
                            RadicarRDA.RDAAmbulatorio_IHCE radRDARDAAmbulatorio = new RadicarRDA.RDAAmbulatorio_IHCE();
                            dataGridView1.EndEdit();

                            foreach (DataGridViewRow row in dataGridView1.Rows)
                            {
                                //bool isChecked = Convert.ToBoolean(row.Cells["CheckForSend"].Value);
                                object valor = row.Cells["CheckForSend"].Value;
                                bool isChecked = valor != null && Convert.ToBoolean(valor);

                                string rdapaciente = row.Cells["RDAPaciente"].Value.ToString();
                                string rdaambulatorio = row.Cells["RDAAmbulatorio"].Value.ToString();
                                int admision = Convert.ToInt32(row.Cells["Admision"].Value);

                                if (isChecked == true)
                                {
                                    if (string.IsNullOrEmpty(rdaambulatorio))
                                    {
                                        radRDARDAAmbulatorio.EnviarRDAAmbulatorio(admision, comboBox2.Text);
                                    }
                                }
                            }

                            await Consultar();

                            MG = new MensajesGeneral();
                            MG.Mensaje = "Proceso de envio finalizado";
                            MG.TipoImagen = 3;
                            MG.ShowDialog();
                        }
                    }
                }
                else
                {
                    MG = new MensajesGeneral()
                    {
                        Mensaje = "No hay datos de configuracion del Ministerio de Salud, contacte al desarrollador del sistema",
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
        #endregion

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
        async Task Consultar()
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
                            await Filtrar("MG");
                            break;
                        case "Enfermeria":
                            await Filtrar("CU");
                            break;
                        case "Radiologia":
                            await Filtrar("RA");
                            break;
                        case "Psicologia":
                            await Filtrar("PS");
                            break;
                        case "Fisiatria":
                            await Filtrar("FI");
                            break;
                        case "Terapia Ocupacional":
                            await Filtrar("TO");
                            break;
                        case "Terapia Fisica":
                            await Filtrar("TF");
                            break;
                        case "Notas Aclaratorias Medicina General":
                            await Filtrar("NAMG");
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
        private async void toolStripButton1_Click(object sender, EventArgs e)
        {
            await Consultar();
        }
        void Estilos(DataGridView D, DataTable t)
        {
            D.EnableHeadersVisualStyles = false;
            D.ScrollBars = ScrollBars.Both;

            D.DataSource = t;

            D.ReadOnly = false;
            D.Columns["CheckForSend"].ReadOnly = false;

            D.Columns["CheckForSend"].Width = 40;
            D.Columns["Admision"].Width = 80;
            D.Columns["Paciente"].Width = 300;
            //D.Columns["RDAPaciente"].Width = 200;
            //D.Columns["RDAAmbulatorio"].Width = 200;
            D.Columns["FechaRDAPaciente"].Width = 120;
            D.Columns["FechaRDAAmbulatorio"].Width = 140;
            D.Columns["PersonaRDAPaciente"].Width = 150;
            D.Columns["PersonaRDAAmbulatorio"].Width = 150;

            D.Columns["Admision"].ReadOnly = true;
            D.Columns["Paciente"].ReadOnly = true;
            //D.Columns["RDAPaciente"].ReadOnly = true;
            //D.Columns["RDAAmbulatorio"].ReadOnly = true;
            D.Columns["FechaRDAPaciente"].ReadOnly = true;
            D.Columns["FechaRDAAmbulatorio"].ReadOnly = true;
            D.Columns["PersonaRDAPaciente"].ReadOnly = true;
            D.Columns["PersonaRDAAmbulatorio"].ReadOnly = true;

            D.ColumnHeadersDefaultCellStyle.Font = new Font(D.Font, FontStyle.Bold);
            D.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 8, FontStyle.Bold);
            D.ColumnHeadersDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#bfdbff");
            D.ColumnHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.Blue;
            D.ColumnHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.Blue;

            D.Columns["CheckForSend"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["Admision"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["Paciente"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            //D.Columns["RDAPaciente"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            //D.Columns["RDAAmbulatorio"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["FechaRDAPaciente"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["FechaRDAAmbulatorio"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["PersonaRDAPaciente"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["PersonaRDAAmbulatorio"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            D.Columns["POS"].Visible = false;
            D.Columns["RDAPaciente"].Visible = false;
            D.Columns["RDAAmbulatorio"].Visible = false;

            foreach (DataGridViewRow row in D.Rows)
            {
                int Numero = Convert.ToInt32(row.Cells["POS"].Value.ToString());

                if ((Numero % 2) == 0)
                {
                    row.DefaultCellStyle.BackColor = System.Drawing.Color.Aquamarine;
                }
                else
                {
                    row.DefaultCellStyle.BackColor = System.Drawing.Color.MediumAquamarine;
                }
            }

            dataGridView1.ClearSelection();
        }
        void Encabezados()
        {
            dataGridView1.DataSource = null;
            dt = new DataTable();
            POS = dt.Columns.Add("POS", typeof(int));
            CheckForSend = dt.Columns.Add("CheckForSend", typeof(bool));
            Admision = dt.Columns.Add("Admision", typeof(int));
            Paciente = dt.Columns.Add("Paciente", typeof(string));
            RDAPaciente = dt.Columns.Add("RDAPaciente", typeof(string));
            RDAAmbulatorio = dt.Columns.Add("RDAAmbulatorio", typeof(string));
            FechaRDAPaciente = dt.Columns.Add("FechaRDAPaciente", typeof(string));
            FechaRDAAmbulatorio = dt.Columns.Add("FechaRDAAmbulatorio", typeof(string));
            PersonaRDAPaciente = dt.Columns.Add("PersonaRDAPaciente", typeof(string));
            PersonaRDAAmbulatorio = dt.Columns.Add("PersonaRDAAmbulatorio", typeof(string));
        }
        async Task Filtrar(string Especialidad)
        {
            try
            {
                List<CXN_HORARIO> getRDA = await repoFHIR.FiltrarEspecialidad(dateTimePicker1.Value.Date, Especialidad);
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
                        row["RDAPaciente"] = i.Hor_ArrastraHistoria;
                        row["RDAAmbulatorio"] = i.Hor_AdmOpnened;
                        row["FechaRDAPaciente"] = i.Hor_Autoriza;
                        row["FechaRDAAmbulatorio"] = i.Hor_RegAtn;
                        row["PersonaRDAPaciente"] = i.Hor_Usr_Admisiona;
                        row["PersonaRDAAmbulatorio"] = i.PacienteAseguradora;

                        dt.Rows.Add(row);
                        dt.AcceptChanges();

                        Contador = Contador + 1;
                    }

                    Contador = 1;
                    Estilos(dataGridView1, dt);
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
                foreach (DataGridViewRow row in dataGridView1.Rows)
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
                foreach (DataGridViewRow row in dataGridView1.Rows)
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
            try
            {
                await Autoriza();
                dataGridView1.EndEdit();

                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    //bool isChecked = Convert.ToBoolean(row.Cells["CheckForSend"].Value);
                    object valor = row.Cells["CheckForSend"].Value;
                    bool isChecked = valor != null && Convert.ToBoolean(valor);

                    string rdapaciente = row.Cells["RDAPaciente"].Value.ToString();
                    string rdaambulatorio = row.Cells["RDAAmbulatorio"].Value.ToString();
                    int admision = Convert.ToInt32(row.Cells["Admision"].Value);    

                    if (isChecked == true)
                    {
                        if (string.IsNullOrEmpty(rdapaciente))
                        {
                            CrearBundlePaciente(admision);
                        }                        
                    }
                }

                await Consultar();

                MG = new MensajesGeneral();
                MG.Mensaje = "Proceso de envio finalizado";
                MG.TipoImagen = 3;
                MG.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
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
            try
            {
                await Autoriza();
                dataGridView1.EndEdit();

                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    //bool isChecked = Convert.ToBoolean(row.Cells["CheckForSend"].Value);
                    object valor = row.Cells["CheckForSend"].Value;
                    bool isChecked = valor != null && Convert.ToBoolean(valor);

                    string rdapaciente = row.Cells["RDAPaciente"].Value.ToString();
                    string rdaambulatorio = row.Cells["RDAAmbulatorio"].Value.ToString();
                    int admision = Convert.ToInt32(row.Cells["Admision"].Value);

                    if (isChecked == true)
                    {
                        if (string.IsNullOrEmpty(rdaambulatorio))
                        {
                            CrearBundleCE(admision);
                        }
                    }
                }

                await Consultar();

                MG = new MensajesGeneral();
                MG.Mensaje = "Proceso de envio finalizado";
                MG.TipoImagen = 3;
                MG.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private async void toolStripButton9_Click(object sender, EventArgs e)
        {
            try
            {
                await Autoriza();
                dataGridView1.EndEdit();

                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    //bool isChecked = Convert.ToBoolean(row.Cells["CheckForSend"].Value);
                    object valor = row.Cells["CheckForSend"].Value;
                    bool isChecked = valor != null && Convert.ToBoolean(valor);

                    string rdapaciente = row.Cells["RDAPaciente"].Value.ToString();
                    string rdaambulatorio = row.Cells["RDAAmbulatorio"].Value.ToString();
                    int admision = Convert.ToInt32(row.Cells["Admision"].Value);

                    if (isChecked == true)
                    {
                        if (string.IsNullOrEmpty(rdaambulatorio))
                        {
                            CrearBundleCuraciones(admision);
                        }
                    }
                }

                await Consultar();

                MG = new MensajesGeneral();
                MG.Mensaje = "Proceso de envio finalizado";
                MG.TipoImagen = 3;
                MG.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
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
        async void CrearBundleCuraciones(int Admision)
        {
            try
            {
                RDACuraciones rda = new RDACuraciones();
                await rda.RadicarRDA(Admision, comboBox2.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private async void toolStripButton8_Click(object sender, EventArgs e)
        {
            try
            {
                dataGridView1.EndEdit();

                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    //bool isChecked = Convert.ToBoolean(row.Cells["CheckForSend"].Value);
                    object valor = row.Cells["CheckForSend"].Value;
                    bool isChecked = valor != null && Convert.ToBoolean(valor);

                    string rdapaciente = row.Cells["RDAPaciente"].Value.ToString();
                    string rdaambulatorio = row.Cells["RDAAmbulatorio"].Value.ToString();
                    int admision = Convert.ToInt32(row.Cells["Admision"].Value);

                    if (isChecked == true)
                    {
                        if (!string.IsNullOrEmpty(rdaambulatorio))
                        {
                            await RadicarNAMG(admision);
                        }
                        else
                        {
                            MG = new MensajesGeneral()
                            {
                                Mensaje = $"La admision { admision.ToString() } no cuenta con un registro Composition de RDA Consulta Externa",
                                TipoImagen = 1000
                            };

                            MG.ShowDialog();
                        }
                    }
                }

                await Consultar();

                MG = new MensajesGeneral();
                MG.Mensaje = "Proceso de envio finalizado";
                MG.TipoImagen = 3;
                MG.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        async Task RadicarNAMG(int admision)
        {
            try
            {
                RDAs.RDANotaAclaratoria rda = new RDAs.RDANotaAclaratoria();
                await rda.EnviarRDA(admision);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
