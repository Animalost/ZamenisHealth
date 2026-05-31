using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Domain;
using Domain.CXN;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using ZamenisHealth.Clases;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.HistoriasClinicas
{
    public partial class Historia_JM_Completar : ConfigForm.BaseForm
    {
        private static readonly IPacientes repoPacs = new MPacientes();
        private static readonly IJuntas repoJuntas = new MJuntas();
        private static readonly IRIPS repoRIPS = new MRIPS();
        private static readonly ICargos repoCargos = new MCargos();
        private static readonly ICIE10 repoCIE10 = new MCIE10();
        private static readonly IBodegas repoBodegas = new MBodegas();
        private static readonly IAgenda repoAgendaMedica = new MAgenda();
        private static readonly IAgendaC repoAgendaMedicaC = new MAgendaC();
        private static readonly IConvenios repoConv = new MConvenios();

        private string TipoUsuario, Regimen;
        private MensajesGeneral MG;
        private int AdmiSelected;
        private bool PrimeraVez;
        private CXN_PACIENTES getPaciente;

        DataTable dt;
        DataColumn POS;
        DataColumn Admision;
        DataColumn Fecha;
        DataColumn Profesional;
        DataColumn Paciente;
        DataColumn Tipo;

        public Historia_JM_Completar(string _TipoUsuario)
        {
            InitializeComponent();
            this.TipoUsuario = _TipoUsuario;
            CargaDocs();           
            ConfigForm.MoverForma(panel3, this);
        }

        public Historia_JM_Completar(string _TipoUsuario, int Admi)
        {
            InitializeComponent();
            this.TipoUsuario = _TipoUsuario;
            CargaDocs();
            
            ConfigForm.MoverForma(panel3, this);

            otrosDatosPacienteHorario otherTemp = repoAgendaMedicaC.cargarAdmision(Admi, "'A','P','H'");
            if (otherTemp != null) 
            {
                this.AdmiSelected = Admi;
                comboBox1.Text = otherTemp.Pac_TipoId;
                textBox1.Text = otherTemp.Pac_IdNum;
               
                button1.Enabled = false;
                comboBox1.Enabled = false;
                textBox1.Enabled = false;
                dataGridView1.Enabled = false;
            }
            else
            {
                MG = new MensajesGeneral();
                MG.TipoImagen = 1000;
                MG.Mensaje = "No se logro cargar la admision";
                MG.ShowDialog();

                this.Dispose();
                this.Close();

                return;
            }
        }

        public void setDatos(string _tid, string _nid)
        {
            comboBox1.Text = _tid;
            textBox1.Text = _nid;
        }

        void CargaDocs()
        {
            var LD = repoPacs.ListaDocs();

            foreach (var i in LD)
            {
                comboBox1.Items.Add(i);
            }
        }

        private void Historia_JM_Completar_Load(object sender, EventArgs e)
        {
            Titulo.Visible = false;
            ImageClose.Visible = false;

            panel4.Enabled = false;
            panel5.Enabled = false;
            panel6.Enabled = false;
            panel7.Enabled = false;
            panel8.Enabled = false;
            panel9.Enabled = false;

            LoadProfesionales();

            if (button1.Enabled == false)
            {
                CargarUsuario();
                ClickData();
            }
        }

        void LoadProfesionales()
        {
            try
            {
                List<string> L1 = repoBodegas.getProfByTipo("FI");
                if (L1 != null)
                {
                    foreach (string i in L1)
                    {
                        comboBox6.Items.Add(i);
                    }
                }

                List<string> L2 = repoBodegas.getProfByTipo("TF");
                if (L2 != null)
                {
                    foreach (string i in L2)
                    {
                        comboBox2.Items.Add(i);
                    }
                }

                List<string> L3 = repoBodegas.getProfByTipo("TO");
                if (L3 != null)
                {
                    foreach (string i in L3)
                    {
                        comboBox3.Items.Add(i);
                    }
                }

                List<string> L4 = repoBodegas.getProfByTipo("PS");
                if (L4 != null)
                {
                    foreach (string i in L4)
                    {
                        comboBox4.Items.Add(i);
                    }
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Dispose();
            this.Close();
        }

        void Encabezados()
        {
            dataGridView1.DataSource = null;

            dt = new DataTable();
            POS = dt.Columns.Add("POS", typeof(int));
            Admision = dt.Columns.Add("Admision", typeof(int));
            Fecha = dt.Columns.Add("Fecha", typeof(DateTime));
            Profesional = dt.Columns.Add("Profesional", typeof(string));
            Paciente = dt.Columns.Add("Paciente", typeof(string));
            Tipo = dt.Columns.Add("Tipo", typeof(string));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                CargarUsuario();
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        void CargarUsuario()
        {
            try
            {
                List<CXN_HCJUNTAS> getJuntas = repoJuntas.getJuntasForComplete(comboBox1.Text, textBox1.Text, false);
                if (getJuntas != null)
                {
                    Encabezados();

                    int Contador = 1;

                    foreach (CXN_HCJUNTAS i in getJuntas)
                    {
                        DataRow row = dt.NewRow();

                        row["POS"] = Contador;
                        row["Admision"] = i.Jun_Adm.ToString();
                        row["Fecha"] = Convert.ToDateTime(i.Jun_Fecha).ToString(Conexion.ConectionDictionary["Format_Fecha"]);
                        row["Profesional"] = i.Jun_Con_FI.ToString();
                        row["Paciente"] = i.Jun_Observa.ToString();
                        row["Tipo"] = i.Jun_Tipo.ToString();

                        dt.Rows.Add(row);
                        dt.AcceptChanges();

                        Contador = Contador + 1;
                    }

                    Contador = 1;
                    Estilos(dataGridView1, dt);
                }
                else
                {
                    Encabezados();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        void Estilos(DataGridView D, DataTable t)
        {
            D.EnableHeadersVisualStyles = false;
            D.ScrollBars = ScrollBars.Both;

            D.DataSource = t;

            D.Columns["Admision"].Width = 80;
            D.Columns["Fecha"].Width = 80;
            D.Columns["Profesional"].Width = 400;
            D.Columns["Paciente"].Width = 400;
            D.Columns["Tipo"].Width = 80;

            D.ColumnHeadersDefaultCellStyle.Font = new Font(D.Font, FontStyle.Bold);
            D.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 8, FontStyle.Bold);
            D.ColumnHeadersDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#bfdbff");
            D.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;
            D.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;

            D.Columns["Admision"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["Fecha"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["Profesional"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["Paciente"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["Tipo"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            D.Columns["Admision"].SortMode = DataGridViewColumnSortMode.NotSortable;
            D.Columns["Fecha"].SortMode = DataGridViewColumnSortMode.NotSortable;
            D.Columns["Profesional"].SortMode = DataGridViewColumnSortMode.NotSortable;
            D.Columns["Paciente"].SortMode = DataGridViewColumnSortMode.NotSortable;
            D.Columns["Tipo"].SortMode = DataGridViewColumnSortMode.NotSortable;

            D.Columns["POS"].Visible = false;

            foreach (DataGridViewRow row in D.Rows)
            {
                int Numero = Convert.ToInt32(row.Cells["POS"].Value.ToString());

                if ((Numero % 2) == 0)
                {
                    row.DefaultCellStyle.BackColor = Color.Aquamarine;
                }
                else
                {
                    row.DefaultCellStyle.BackColor = Color.MediumAquamarine;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime FechaHora = DateTime.Now;

                if (comboBox7.Text == "")
                {
                    MG = new MensajesGeneral();
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "Debe seleccionar el tipo de junta";
                    MG.ShowDialog();
                    return;
                }

                if (richTextBox2.Text == "" || richTextBox3.Text == "")
                {
                    MG = new MensajesGeneral();
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "Debe completar todos los campos en blanco";
                    MG.ShowDialog();
                    return;
                }

                if (this.TipoUsuario != "TF")
                {
                    if (richTextBox6.Text == "")
                    {
                        MG = new MensajesGeneral();
                        MG.TipoImagen = 1000;
                        MG.Mensaje = "Debe completar todos los campos en blanco";
                        MG.ShowDialog();
                        return;
                    }
                }

                //pronostico
                if (panel9.Visible == true && string.IsNullOrEmpty(richTextBox7.Text))
                {
                    MG = new MensajesGeneral();
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "Debe completar todos los campos en blanco";
                    MG.ShowDialog();
                    return;
                }

                if (textBox2.Text == "0")
                {
                    MG = new MensajesGeneral();
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "La edad del paciente no puede ser 0";
                    MG.ShowDialog();
                    return;
                }

                if (comboBox5.Text == "")
                {
                    MG = new MensajesGeneral();
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "Debe completar la impresion diagnostica";
                    MG.ShowDialog();
                    return;
                }

                string Fisiatra = "";
                string Ter_Fis = "";
                string Ter_Ocu = "";
                string Psicologo = "";

                #region Combos de Profesionales
                if (comboBox6.Text == "OTRO")
                {
                    if (textBox22.Text == "") { MessageBox.Show("Digite nombre del fisiatra"); return; }
                    Fisiatra = textBox22.Text;
                }
                else
                {
                    if (comboBox6.Text == "") { MessageBox.Show("Seleccione fisiatra"); return; }
                    Fisiatra = comboBox6.Text;
                }

                if (comboBox2.Text == "OTRO")
                {
                    if (textBox23.Text == "") { MessageBox.Show("Digite nombre del fisioterapeuta"); return; }
                    Ter_Fis = textBox23.Text;
                }
                else
                {
                    if (comboBox2.Text == "") { MessageBox.Show("Seleccione fisioterapeuta"); return; }
                    Ter_Fis = comboBox2.Text;
                }

                if (comboBox3.Text == "OTRO")
                {
                    if (textBox24.Text == "") { MessageBox.Show("Digite nombre del terapeuta ocupacional"); return; }
                    Ter_Ocu = textBox24.Text;
                }
                else
                {
                    if (comboBox3.Text == "") { MessageBox.Show("Seleccione terapeuta ocupacional"); return; }
                    Ter_Ocu = comboBox3.Text;
                }

                if (comboBox4.Text == "OTRO")
                {
                    if (textBox25.Text == "") { MessageBox.Show("Digite nombre del psicologo"); return; }
                    Psicologo = textBox25.Text;
                }
                else
                {
                    if (comboBox4.Text == "") { MessageBox.Show("Seleccione psicologo"); return; }
                    Psicologo = comboBox4.Text;
                }

                #endregion

                otrosDatosPacienteHorario datoCita = repoAgendaMedicaC.cargarAdmision(AdmiSelected, "'A','P','H'");

                CXN_HCJUNTAS HCJM = new CXN_HCJUNTAS
                {
                    Jun_Fisiatra = Fisiatra,
                    Jun_Psicologo = Psicologo,
                    Jun_TO = Ter_Ocu,
                    Jun_TF = Ter_Fis,
                    Jun_CIE10 = textBox20.Text,
                    Jun_Edad = textBox2.Text,
                    Jun_Prox_Cita = dateTimePicker1.Value.Date,
                    Jun_Adm = AdmiSelected,
                    Jun_Tipo = comboBox7.Text,
                    Jun_Ase = datoCita.Hor_Pac_Ase,
                    Jun_Bodega = datoCita.Hor_Pac_Bod,
                    Jun_Cant = 1,
                    Jun_Cia = datoCita.Hor_Pac_Cia,
                    Jun_Fecha = datoCita.Hor_Pac_Fecha_Cita,
                    Jun_Pac = datoCita.Hor_Pac_Id                    
                };

                string nuevoTextoCierre = "";
                string nuevoTextoDiagnostico = "";
                string nuevoTextoPronostico = "";
                string nuevoTextoConducta = "";
                
                //Insertar
                if (this.PrimeraVez == true)
                {
                    if (this.TipoUsuario == "PS")
                    {
                        nuevoTextoCierre = richTextBox6.Text + " - Fecha de Agregado: " + Convert.ToDateTime(FechaHora).ToString("yyyy-MM-dd HH:mm:ss") + "\n\r";
                        nuevoTextoDiagnostico = richTextBox2.Text + " - Fecha de Agregado: " + Convert.ToDateTime(FechaHora).ToString("yyyy-MM-dd HH:mm:ss") + "\n\r";
                        nuevoTextoPronostico = richTextBox7.Text + " - Fecha de Agregado: " + Convert.ToDateTime(FechaHora).ToString("yyyy-MM-dd HH:mm:ss") + "\n\r";
                        nuevoTextoConducta = richTextBox3.Text + " - Fecha de Agregado: " + Convert.ToDateTime(FechaHora).ToString("yyyy-MM-dd HH:mm:ss") + "\n\r";

                        HCJM.Jun_Observa_PS = nuevoTextoCierre; //Jun_Observa_PS
                        HCJM.Jun_DX_PS = nuevoTextoDiagnostico; //Jun_DX_PS
                        HCJM.Jun_Pro_PS = nuevoTextoPronostico; //Jun_Pro_PS
                        HCJM.Jun_Con_PS = nuevoTextoConducta; //Jun_Con_PS
                        HCJM.Jun_Egresa = richTextBox6.Text; //

                        HCJM.Jun_Observa = "";
                        HCJM.Jun_DX_FI = "";
                        HCJM.Jun_Pro_FI = "";
                        HCJM.Jun_Con_FI = "";
                        HCJM.Jun_DX_TF = "";
                        HCJM.Jun_Pro_TF = "";
                        HCJM.Jun_Con_TF = "";
                        HCJM.Jun_Observa_TO = "";
                        HCJM.Jun_DX_TO = "";
                        HCJM.Jun_Pro_TO = "";
                        HCJM.Jun_Con_TO = "";
                    }

                    if (this.TipoUsuario == "TO")
                    {
                        nuevoTextoCierre = richTextBox6.Text + " - Fecha de Agregado: " + Convert.ToDateTime(FechaHora).ToString("yyyy-MM-dd HH:mm:ss") + "\n\r";
                        nuevoTextoDiagnostico = richTextBox2.Text + " - Fecha de Agregado: " + Convert.ToDateTime(FechaHora).ToString("yyyy-MM-dd HH:mm:ss") + "\n\r";
                        nuevoTextoPronostico = richTextBox7.Text + " - Fecha de Agregado: " + Convert.ToDateTime(FechaHora).ToString("yyyy-MM-dd HH:mm:ss") + "\n\r";
                        nuevoTextoConducta = richTextBox3.Text + " - Fecha de Agregado: " + Convert.ToDateTime(FechaHora).ToString("yyyy-MM-dd HH:mm:ss") + "\n\r";

                        HCJM.Jun_Observa_TO = nuevoTextoCierre; //Jun_Observa_TO
                        HCJM.Jun_DX_TO = nuevoTextoDiagnostico; //Jun_DX_TO
                        HCJM.Jun_Pro_TO = nuevoTextoPronostico; //Jun_Pro_TO
                        HCJM.Jun_Con_TO = nuevoTextoConducta; //Jun_Con_TO
                        HCJM.Jun_Egresa = nuevoTextoCierre;

                        HCJM.Jun_Observa = "";
                        HCJM.Jun_DX_FI = "";
                        HCJM.Jun_Pro_FI = "";
                        HCJM.Jun_Con_FI = "";
                        HCJM.Jun_DX_TF = "";
                        HCJM.Jun_Pro_TF = "";
                        HCJM.Jun_Con_TF = "";
                        HCJM.Jun_Observa_PS = "";
                        HCJM.Jun_DX_PS = "";
                        HCJM.Jun_Pro_PS = "";
                        HCJM.Jun_Con_PS = "";
                    }

                    if (this.TipoUsuario == "FI")
                    {
                        nuevoTextoCierre = richTextBox6.Text + " - Fecha de Agregado: " + Convert.ToDateTime(FechaHora).ToString("yyyy-MM-dd HH:mm:ss") + "\n\r";
                        nuevoTextoDiagnostico = richTextBox2.Text + " - Fecha de Agregado: " + Convert.ToDateTime(FechaHora).ToString("yyyy-MM-dd HH:mm:ss") + "\n\r";
                        nuevoTextoPronostico = "";
                        nuevoTextoConducta = richTextBox3.Text + " - Fecha de Agregado: " + Convert.ToDateTime(FechaHora).ToString("yyyy-MM-dd HH:mm:ss") + "\n\r";

                        HCJM.Jun_Observa = nuevoTextoCierre; //Jun_Observa
                        HCJM.Jun_DX_FI = nuevoTextoDiagnostico; //Jun_DX_FI
                        HCJM.Jun_Pro_FI = nuevoTextoPronostico; //Jun_Pro_FI
                        HCJM.Jun_Con_FI = nuevoTextoConducta; //Jun_Con_FI
                        HCJM.Jun_Egresa = richTextBox6.Text;

                        HCJM.Jun_Observa_TO = "";
                        HCJM.Jun_DX_TO = "";
                        HCJM.Jun_Pro_TO = "";
                        HCJM.Jun_Con_TO = "";
                        HCJM.Jun_DX_TF = "";
                        HCJM.Jun_Pro_TF = "";
                        HCJM.Jun_Con_TF = "";
                        HCJM.Jun_Observa_PS = "";
                        HCJM.Jun_DX_PS = "";
                        HCJM.Jun_Pro_PS = "";
                        HCJM.Jun_Con_PS = "";
                    }

                    if (this.TipoUsuario == "TF")
                    {
                        nuevoTextoDiagnostico = richTextBox2.Text + " - Fecha de Agregado: " + Convert.ToDateTime(FechaHora).ToString("yyyy-MM-dd HH:mm:ss") + "\n\r";
                        nuevoTextoPronostico = richTextBox7.Text + " - Fecha de Agregado: " + Convert.ToDateTime(FechaHora).ToString("yyyy-MM-dd HH:mm:ss") + "\n\r";
                        nuevoTextoConducta = richTextBox3.Text + " - Fecha de Agregado: " + Convert.ToDateTime(FechaHora).ToString("yyyy-MM-dd HH:mm:ss") + "\n\r";

                        HCJM.Jun_DX_TF = nuevoTextoDiagnostico; //Jun_DX_TF
                        HCJM.Jun_Pro_TF = nuevoTextoPronostico; //Jun_Pro_TF
                        HCJM.Jun_Con_TF = nuevoTextoConducta; //Jun_Con_TF
                        HCJM.Jun_Egresa = "";

                        HCJM.Jun_Observa = "";
                        HCJM.Jun_DX_FI = "";
                        HCJM.Jun_Pro_FI = "";
                        HCJM.Jun_Con_FI = "";
                        HCJM.Jun_DX_TO = "";
                        HCJM.Jun_Pro_TO = "";
                        HCJM.Jun_Con_TO = "";
                        HCJM.Jun_Observa_PS = "";
                        HCJM.Jun_DX_PS = "";
                        HCJM.Jun_Pro_PS = "";
                        HCJM.Jun_Con_PS = "";
                    }

                    bool grabarJunta = repoJuntas.InsertarJunta(HCJM);
                    if (grabarJunta == true)
                    {
                        var ambito = 0;
                        var personal = 0;
                        var causaexterna = 0;
                        var finalidad = 0;
                        var motivo = 0;
                        var impresion = 0;

                        CXN_CARGOS C = new CXN_CARGOS
                        {
                            Car_Adm_Id = AdmiSelected,
                            Car_Pac = datoCita.Hor_Pac_Id,
                            Car_Cia = datoCita.Hor_Pac_Cia,
                            Car_Ase = datoCita.Hor_Pac_Ase,
                            Car_Prof = datoCita.Hor_Pac_Bod,
                            Car_Fecha = Convert.ToDateTime(dateTimePicker1.Value.Date),
                            Car_Estado = "G",
                            Car_Tipo = "Historia",
                            Car_Cod = datoCita.Hor_Pac_Cup,
                            Car_Val_Tot = repoConv.ServicioNombre(datoCita.Hor_Pac_Cup, datoCita.Hor_Pac_Ase, "TF").Con_Valor,
                            Car_Val_Un = repoConv.ServicioNombre(datoCita.Hor_Pac_Cup, datoCita.Hor_Pac_Ase, "TF").Con_Valor,
                            Car_Tipo_Serv = "TF",
                            Car_Cant = 1,
                            Car_Detalle = "",
                            Car_Item = "DISCUSION JUNTA MEDICA",
                            Car_Dx1 = textBox20.Text,
                            Car_Dx2 = "",
                            Car_Dx3 = "",
                            Car_Ambito = ambito,
                            Car_Personal = personal,
                            Car_CExterna = causaexterna,
                            Car_Finalidad = finalidad,
                            Car_Finalidad_CO = motivo, //motivo                                           
                            Car_Imp_Dx = impresion,
                            Car_Regimen = "0"
                        };

                        bool insertarCargo = repoCargos.InsertarCargoHistorias(C);
                        if (insertarCargo == false)
                        {
                            MessageBox.Show("No se logro guardar el cargo economico en el registro de valores a cobrar en la factura, " +
                                "su historia quedo resgitrada pero reporte este incidente a la recepcion con la admision: " + AdmiSelected.ToString(),
                                "Advertencia!!!",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                        }
                        else
                        {
                            repoAgendaMedica.Graba_Hora_Salida(AdmiSelected);

                            Medicina.RIPSHistory for_RIPS = new Medicina.RIPSHistory();
                            for_RIPS.Adm_Cargo = AdmiSelected;
                            for_RIPS.ShowDialog();

                            MG = new MensajesGeneral();
                            MG.TipoImagen = 3;
                            MG.Mensaje = "Junta Creada";
                            MG.ShowDialog();

                            repoAgendaMedica.ConsumirAdmision(AdmiSelected);

                            this.Dispose();
                            this.Close();
                        }
                    }
                    else
                    {
                        MessageBox.Show("No se logro guardar la historia clinica, revise la informacion y verifique que todos los datos estan completos, " +
                               "No se ha logrado continuar",
                               "Advertencia!!!",
                               MessageBoxButtons.OK,
                               MessageBoxIcon.Information);
                        return;
                    }
                }

                //Actualizar
                if (this.PrimeraVez == false)
                {
                    CXN_HCJUNTAS juntaPrevia = repoJuntas.getJuntaCompleta(AdmiSelected);
                    if (juntaPrevia != null) 
                    {
                        if (this.TipoUsuario == "PS")
                        {
                            nuevoTextoCierre = juntaPrevia.Jun_Observa_PS + "\n\r" + richTextBox6.Text + " - Fecha de Agregado: " + Convert.ToDateTime(FechaHora).ToString("yyyy-MM-dd HH:mm:ss");
                            nuevoTextoDiagnostico = juntaPrevia.Jun_DX_PS + "\n\r" + richTextBox2.Text + " - Fecha de Agregado: " + Convert.ToDateTime(FechaHora).ToString("yyyy-MM-dd HH:mm:ss");
                            nuevoTextoPronostico = juntaPrevia.Jun_Pro_PS + "\n\r" + richTextBox7.Text + " - Fecha de Agregado: " + Convert.ToDateTime(FechaHora).ToString("yyyy-MM-dd HH:mm:ss");
                            nuevoTextoConducta = juntaPrevia.Jun_Con_PS + "\n\r" + richTextBox3.Text + " - Fecha de Agregado: " + Convert.ToDateTime(FechaHora).ToString("yyyy-MM-dd HH:mm:ss");

                            HCJM.Jun_Observa = nuevoTextoCierre; //Jun_Observa_PS
                            HCJM.Jun_DX_TF = nuevoTextoDiagnostico; //Jun_DX_PS
                            HCJM.Jun_Pro_TF = nuevoTextoPronostico; //Jun_Pro_PS
                            HCJM.Jun_Con_TF = nuevoTextoConducta; //Jun_Con_PS
                            HCJM.Jun_Egresa = richTextBox6.Text; //
                            HCJM.Jun_CIE10 = textBox20.Text; //Tipo par apoder actualizar las observaciones
                            HCJM.Jun_MedFirma = "PS";
                        }

                        if (this.TipoUsuario == "TO")
                        {
                            nuevoTextoCierre = juntaPrevia.Jun_Observa_TO + "\n\r" + richTextBox6.Text + " - Fecha de Agregado: " + Convert.ToDateTime(FechaHora).ToString("yyyy-MM-dd HH:mm:ss");
                            nuevoTextoDiagnostico = juntaPrevia.Jun_DX_TO + "\n\r" + richTextBox2.Text + " - Fecha de Agregado: " + Convert.ToDateTime(FechaHora).ToString("yyyy-MM-dd HH:mm:ss");
                            nuevoTextoPronostico = juntaPrevia.Jun_Pro_TO + "\n\r" + richTextBox7.Text + " - Fecha de Agregado: " + Convert.ToDateTime(FechaHora).ToString("yyyy-MM-dd HH:mm:ss");
                            nuevoTextoConducta = juntaPrevia.Jun_Con_TO + "\n\r" + richTextBox3.Text + " - Fecha de Agregado: " + Convert.ToDateTime(FechaHora).ToString("yyyy-MM-dd HH:mm:ss");

                            HCJM.Jun_Observa = nuevoTextoCierre; //Jun_Observa_TO
                            HCJM.Jun_DX_TF = nuevoTextoDiagnostico; //Jun_DX_TO
                            HCJM.Jun_Pro_TF = nuevoTextoPronostico; //Jun_Pro_TO
                            HCJM.Jun_Con_TF = nuevoTextoConducta; //Jun_Con_TO
                            HCJM.Jun_Egresa = richTextBox6.Text;
                            HCJM.Jun_CIE10 = textBox20.Text; //Tipo par apoder actualizar las observaciones
                            HCJM.Jun_MedFirma = "TO";
                        }

                        if (this.TipoUsuario == "FI")
                        {
                            nuevoTextoCierre = juntaPrevia.Jun_Observa + "\n\r" + richTextBox6.Text + " - Fecha de Agregado: " + Convert.ToDateTime(FechaHora).ToString("yyyy-MM-dd HH:mm:ss");
                            nuevoTextoDiagnostico = juntaPrevia.Jun_DX_FI + "\n\r" + richTextBox2.Text + " - Fecha de Agregado: " + Convert.ToDateTime(FechaHora).ToString("yyyy-MM-dd HH:mm:ss");
                            nuevoTextoPronostico = "";
                            nuevoTextoConducta = juntaPrevia.Jun_Con_FI + "\n\r" + richTextBox3.Text + " - Fecha de Agregado: " + Convert.ToDateTime(FechaHora).ToString("yyyy-MM-dd HH:mm:ss");

                            HCJM.Jun_Observa = nuevoTextoCierre; //Jun_Observa
                            HCJM.Jun_DX_TF = nuevoTextoDiagnostico; //Jun_DX_FI
                            HCJM.Jun_Pro_TF = nuevoTextoPronostico; //Jun_Pro_FI
                            HCJM.Jun_Con_TF = nuevoTextoConducta; //Jun_Con_FI
                            HCJM.Jun_Egresa = richTextBox6.Text;
                            HCJM.Jun_CIE10 = textBox20.Text; //Tipo par apoder actualizar las observaciones
                            HCJM.Jun_MedFirma = "FI";
                        }

                        if (this.TipoUsuario == "TF")
                        {
                            nuevoTextoDiagnostico = juntaPrevia.Jun_DX_TF + "\n\r" + richTextBox2.Text + " - Fecha de Agregado: " + Convert.ToDateTime(FechaHora).ToString("yyyy-MM-dd HH:mm:ss");
                            nuevoTextoPronostico = juntaPrevia.Jun_Pro_TF + "\n\r" + richTextBox7.Text + " - Fecha de Agregado: " + Convert.ToDateTime(FechaHora).ToString("yyyy-MM-dd HH:mm:ss");
                            nuevoTextoConducta = juntaPrevia.Jun_Con_TF + "\n\r" + richTextBox3.Text + " - Fecha de Agregado: " + Convert.ToDateTime(FechaHora).ToString("yyyy-MM-dd HH:mm:ss");

                            HCJM.Jun_DX_TF = nuevoTextoDiagnostico; //Jun_DX_TF
                            HCJM.Jun_Pro_TF = nuevoTextoPronostico; //Jun_Pro_TF
                            HCJM.Jun_Con_TF = nuevoTextoConducta; //Jun_Con_TF
                            HCJM.Jun_Egresa = "";
                            HCJM.Jun_CIE10 = textBox20.Text; //Tipo par apoder actualizar las observaciones
                            HCJM.Jun_MedFirma = "TF";
                        }

                        //Actualiza otros datos de la junta
                        repoJuntas.updateJuntaMedica(HCJM);

                        //Actualiza Observaciones
                        bool _updateObservations = repoJuntas.UpdateObservations(HCJM);
                        if (_updateObservations != true)
                        {
                            MG = new MensajesGeneral();
                            MG.TipoImagen = 1000;
                            MG.Mensaje = "No se ha logrado actualizar la junta";
                            MG.ShowDialog();
                            return;
                        }

                        repoAgendaMedica.ConsumirAdmision(AdmiSelected);

                        MG = new MensajesGeneral();
                        MG.TipoImagen = 3;
                        MG.Mensaje = "Junta Medica Actualizada con Exito";
                        MG.ShowDialog();

                        this.Dispose();
                        this.Close();
                    }
                    else
                    {
                        MG = new MensajesGeneral();
                        MG.TipoImagen = 1000;
                        MG.Mensaje = "No se logro atualizar la Junta Medica";
                        MG.ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        void LoadRipsPrevios()
        {
            try
            {
                CXN_CARGOS getRips = repoCargos.getRIPS(AdmiSelected);
                if (getRips != null)
                {
                    comboBox5.Text = repoRIPS.TipoRipCargo(getRips.Car_Imp_Dx, "IMPDX");
                }
                else
                {
                    comboBox5.Text = "";
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        List<string> getBods()
        {
            return repoBodegas.getBodegas();
        }

        private void textBox1_DoubleClick(object sender, EventArgs e)
        {
            Comunes.BuscarPacientes B = new Comunes.BuscarPacientes("CompleteJM");
            B.ShowDialog();
        }

        private void textBox20_DoubleClick(object sender, EventArgs e)
        {
            Medicina.CIE10 cIE10 = new Medicina.CIE10("JMComplete");
            cIE10.ShowDialog();
        }

        public void setDX(string DX, string DXt)
        {
            textBox20.Text = DX;
            textBox21.Text = DXt;
        }

        private void comboBox6_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (comboBox6.Text == "OTRO")
                {
                    label29.Visible = true;
                    textBox22.Visible = true;
                }
                else
                {
                    label29.Visible = false;
                    textBox22.Visible = false;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (comboBox2.Text == "OTRO")
                {
                    label30.Visible = true;
                    textBox23.Visible = true;
                }
                else
                {
                    label30.Visible = false;
                    textBox23.Visible = false;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (comboBox3.Text == "OTRO")
                {
                    label31.Visible = true;
                    textBox24.Visible = true;
                }
                else
                {
                    label31.Visible = false;
                    textBox24.Visible = false;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        void ClickData()
        {
            try
            {
                getPaciente = new CXN_PACIENTES();
                getPaciente = repoPacs.LlamarPacienteNumDoc(textBox1.Text);
                if (getPaciente != null)
                {
                    DateTime nacimiento = Convert.ToDateTime(getPaciente.Pac_FechaNto);
                    int edad = DateTime.Today.AddTicks(-nacimiento.Ticks).Year - 1;
                    textBox2.Text = edad.ToString();

                    this.Regimen = getPaciente.Pac_Regimen;
                }
                else
                {
                    textBox2.Text = "0";
                }

                panel4.Enabled = true;
                panel5.Enabled = true;
                panel6.Enabled = true;
                panel7.Enabled = true;
                panel8.Enabled = true;
                panel9.Enabled = true;

                CXN_HCJUNTAS getJunta = repoJuntas.getJuntaCompleta(AdmiSelected);
                if (getJunta != null)
                {
                    PrimeraVez = false;
                    comboBox7.Text = getJunta.Jun_Tipo;
                    LoadRipsPrevios();
                    comboBox7.Enabled = false;

                    if (getJunta.Jun_CIE10 != "")
                    {
                        textBox20.Text = getJunta.Jun_CIE10;
                        textBox21.Text = repoCIE10.BuscaDX(getJunta.Jun_CIE10);
                    }

                    List<string> list = getBods();
                    if (list != null)
                    {
                        var F = list.Find(x => x.Equals(getJunta.Jun_Fisiatra));
                        if (F != null) { comboBox6.Text = F.ToString(); } else { comboBox6.Text = "OTRO"; textBox22.Text = getJunta.Jun_Fisiatra; }

                        F = list.Find(x => x.Equals(getJunta.Jun_TF));
                        if (F != null) { comboBox2.Text = F.ToString(); } else { comboBox2.Text = "OTRO"; textBox23.Text = getJunta.Jun_TF; }

                        F = list.Find(x => x.Equals(getJunta.Jun_TO));
                        if (F != null) { comboBox3.Text = F.ToString(); } else { comboBox3.Text = "OTRO"; textBox24.Text = getJunta.Jun_TO; }

                        F = list.Find(x => x.Equals(getJunta.Jun_Psicologo));
                        if (F != null) { comboBox4.Text = F.ToString(); } else { comboBox4.Text = "OTRO"; textBox25.Text = getJunta.Jun_Psicologo; }
                    }

                    DateTime hoy = DateTime.Now.Date;
                    dateTimePicker1.Value = (getJunta.Jun_Prox_Cita != null ? Convert.ToDateTime(getJunta.Jun_Prox_Cita) : Convert.ToDateTime(hoy));

                    ///
                    if (getJunta.Jun_Tipo == "Junta1" && this.TipoUsuario != "FI")
                    {
                        //pronostico
                        panel9.Visible = true;
                    }
                    else
                    {
                        //pronostico
                        panel9.Visible = false;
                    }

                    if (this.TipoUsuario == "PS")
                    {
                        richTextBox5.Text = getJunta.Jun_Observa_PS;
                        richTextBox1.Text = getJunta.Jun_DX_PS;
                        richTextBox8.Text = getJunta.Jun_Pro_PS;
                        richTextBox4.Text = getJunta.Jun_Con_PS;
                    }
                    else if (this.TipoUsuario == "TO")
                    {
                        richTextBox5.Text = getJunta.Jun_Observa_TO;
                        richTextBox1.Text = getJunta.Jun_DX_TO;
                        richTextBox8.Text = getJunta.Jun_Pro_TO;
                        richTextBox4.Text = getJunta.Jun_Con_TO;
                    }
                    else if (this.TipoUsuario == "FI")
                    {
                        richTextBox5.Text = getJunta.Jun_Observa;
                        richTextBox1.Text = getJunta.Jun_DX_FI;
                        richTextBox4.Text = getJunta.Jun_Con_FI;
                    }
                    else if (this.TipoUsuario == "TF")
                    {
                        //Observaciones
                        panel7.Visible = false;
                        richTextBox1.Text = getJunta.Jun_DX_TF;
                        richTextBox4.Text = getJunta.Jun_Con_TF;
                        richTextBox8.Text = getJunta.Jun_Pro_TF;
                    }
                    else
                    {
                        MG = new MensajesGeneral();
                        MG.TipoImagen = 1000;
                        MG.Mensaje = "El rol de su usuario no corresponde al programa de Fibromialgia";
                        MG.ShowDialog();

                        panel4.Enabled = false;
                        panel5.Enabled = false;
                        panel6.Enabled = false;
                        panel7.Enabled = false;
                        panel8.Enabled = false;
                        panel9.Enabled = false;

                        richTextBox1.Text = "";
                        richTextBox2.Text = "";
                        richTextBox3.Text = "";
                        richTextBox4.Text = "";
                        richTextBox5.Text = "";
                        richTextBox6.Text = "";
                        richTextBox7.Text = "";
                        richTextBox8.Text = "";
                    }
                    ///                    
                }
                else
                {
                    PrimeraVez = true;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {
                AdmiSelected = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString());
                ClickData();
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void comboBox7_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox7.Text == "Junta1" && this.TipoUsuario != "FI")
            {
                //Pronostico
                panel9.Visible = true;
            }
            else
            {
                //Pronostico
                panel9.Visible = false;
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Dispose();
            this.Close();
        }

        private void label17_Click(object sender, EventArgs e)
        {
            ExpandeContraer(label17, panel2, 39, 293);
        }

        private void label18_Click(object sender, EventArgs e)
        {
            ExpandeContraer(label18, panel4, 39, 293);
        }

        private void label20_Click(object sender, EventArgs e)
        {
            ExpandeContraer(label20, panel5, 39, 114);
        }

        private void label22_Click(object sender, EventArgs e)
        {
            ExpandeContraer(label22, panel6, 39, 354);
        }

        private void label37_Click(object sender, EventArgs e)
        {
            ExpandeContraer(label37, panel7, 39, 354);
        }

        private void label40_Click(object sender, EventArgs e)
        {
            ExpandeContraer(label40, panel9, 39, 354);
        }

        private void label34_Click(object sender, EventArgs e)
        {
            ExpandeContraer(label34, panel8, 39, 354);
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (comboBox4.Text == "OTRO")
                {
                    label32.Visible = true;
                    textBox25.Visible = true;
                }
                else
                {
                    label32.Visible = false;
                    textBox25.Visible = false;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
    }
}
