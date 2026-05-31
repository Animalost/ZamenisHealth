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
using System.Threading.Tasks;
using System.Windows.Forms;
using Tulpep.NotificationWindow;
using ZamenisHealth.Clases;
using ZamenisHealth.Comunes;
using ZamenisHealth.Properties;
using ZamenisHealth.Recepcion.Extras;

namespace ZamenisHealth.Recepcion.Admision
{
    public partial class AdmisionesCuraciones : Forma2
    {
        private IAgendaC agendaController;
        private IAgenda horarioController;
        private IConvenios conveniosController;
        private IPacientes pacientesController;
        private IAseguradoras aseguradorasController;
        private IZonas zonasController;
        private int Admision, PacId, Ase;
        private DateTime _fechas;

        private bool Bonos, Pendiente, InicioAutoriza;
        private string CodeMun, CodeDep;
        private DataTable dt, dt2;
        private MensajesGeneral MG;

        private DataColumn POS;
        private DataColumn ADM;
        private DataColumn FECHA;
        private DataColumn PROFESIONAL;
        private DataColumn ESTADO;
        private DataColumn SESIONES;

        public AdmisionesCuraciones(int admision)
        {
            InitializeComponent();
            this.Admision = admision;

            SoloNumeros(textBox10);
            SoloNumeros(textBox9);
            SoloNumeros(textBox2);

            agendaController = new MAgendaC();
            conveniosController = new MConvenios();
            pacientesController = new MPacientes();
            aseguradorasController = new MAseguradoras();
            zonasController = new MZonas();
            horarioController = new MAgenda();
        }

        private void AdmisionesCuraciones_Load(object sender, EventArgs e)
        {
            Titulo.Text = $"Admisiones {Admision.ToString()}";
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";           

            textBox13.MaxLength = 10;

            CargarPaises();
            CargarDocumentos();
            CargarAseguradoras();
            CargarRegimen();
            CargarDatosCita();

            foreach (Control c in this.Controls)
            {
                if (c is ComboBox combo)
                {
                    combo.MouseWheel += (s, e2) => ((HandledMouseEventArgs)e2).Handled = true;
                }
            }
            comboBox1.MouseWheel += CancelaScroll;
            comboBox2.MouseWheel += CancelaScroll;
            comboBox3.MouseWheel += CancelaScroll;
            comboBox4.MouseWheel += CancelaScroll;
            comboBox5.MouseWheel += CancelaScroll;
            comboBox6.MouseWheel += CancelaScroll;
            comboBox7.MouseWheel += CancelaScroll;
            comboBox8.MouseWheel += CancelaScroll;
            comboBox9.MouseWheel += CancelaScroll;
            comboBox10.MouseWheel += CancelaScroll;
            comboBox11.MouseWheel += CancelaScroll;
            comboBox12.MouseWheel += CancelaScroll;
        }
        void CancelaScroll(object sender, MouseEventArgs e)
        {
            ((HandledMouseEventArgs)e).Handled = true;
        }
        void CargarPaises()
        {
            try
            {
                List<string> CargaPaises = pacientesController.getListPaises();
                if (CargaPaises != null)
                {
                    foreach (string r in CargaPaises)
                    {
                        comboBox8.Items.Add(r);
                    }

                    foreach (string r2 in CargaPaises)
                    {
                        comboBox10.Items.Add(r2);
                    }
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        void CargarRegimen()
        {
            try
            {
                List<string> CargaRegimen = pacientesController.ListaRegimen();
                if (CargaRegimen != null)
                {
                    foreach (string r in CargaRegimen)
                    {
                        comboBox3.Items.Add(r);
                    }
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        void CargarAseguradoras()
        {
            List<CXN_ASEGURADORA> listado = aseguradorasController.getAseguradoras();
            if (listado != null)
            {
                foreach (var item in listado)
                {
                    comboBox7.Items.Add(item.Ase_Descripcion.ToString());
                }
            }
        }
        void CargarDocumentos()
        {
            List<string> listado = pacientesController.ListaDocs();
            if (listado != null)
            {
                foreach (string str in listado)
                {
                    comboBox6.Items.Add(str);
                }
            }
        }
        void CargarConvenios(int Ase)
        {
            List<string> listado = conveniosController.CargarServiciosxASE(Ase);
            if (listado != null)
            {
                foreach (string str in listado)
                {
                    comboBox1.Items.Add(str);
                }                
            }
        }
        async void CargarDatosCita()
        {
            try
            {
                MG = new MensajesGeneral();
                panel1.Size = new Size(735, 762);
                panel1.Location = new Point(1, 53);

                otrosDatosPacienteHorario getCita = agendaController.cargarAdmision(Admision, "'A'");
                if (getCita != null)
                {
                    if (getCita.Hor_AvisoCurInicio == true)
                    {
                        MG = new MensajesGeneral()
                        {
                            Mensaje = "Esta admision esta marcada como INICIO DE PAQUETE, debe consumir una autorizacion",
                            TipoImagen = 0
                        };
                        MG.ShowDialog();
                    }

                    _fechas = getCita.Hor_Pac_Fecha_Cita;
                    PacId = getCita.Hor_Pac_Id;
                    label3.Text = getCita.Com_Nombre;
                    label4.Text = getCita.Bod_Responsable;
                    label6.Text = Convert.ToDateTime(getCita.Hor_Pac_Fecha_Cita).ToString("yyyy-MM-dd") + " - " + Convert.ToDateTime(getCita.Hor_Pac_Hora_Cita).ToString("HH:mm tt");

                    CargarConvenios(getCita.Hor_Pac_Ase);

                    comboBox1.Text = conveniosController.ServicioNombre(getCita.Hor_Pac_Cup, getCita.Hor_Pac_Ase, getCita.Hor_Pac_Tipo_Serv).Con_Nombre;
                    textBox18.Text = getCita.PrimerNombre;
                    textBox19.Text = getCita.SegundoNombre;
                    textBox20.Text = getCita.PrimerApellido;
                    textBox21.Text = getCita.SegundoApellido;
                    comboBox6.Text = getCita.Pac_TipoId;
                    textBox8.Text = getCita.Pac_IdNum;
                    textBox13.Text = getCita.Pac_Telefono;
                    textBox14.Text = getCita.Pac_TelefonoAux;
                    dateTimePicker1.Value = Convert.ToDateTime(getCita.Pac_FechaNto);
                    comboBox9.Text = getCita.Pac_Sexo == "M" ? "Hombre" : getCita.Pac_Sexo == "F" ? "Mujer" : "Intersexual";
                    textBox12.Text = getCita.Com_Email;
                    comboBox11.Text = getCita.Pac_ECivilLoadAdmition;
                    comboBox7.Text = aseguradorasController.getInfoFromAsebyCode(getCita.Hor_Pac_Ase).Ase_Descripcion;
                    comboBox3.Text = pacientesController.Carga_Regimen(getCita.Hor_Regimen);
                    comboBox4.Text = getCita.Pac_Categoria == "A" ? "Categoria A" :
                                     getCita.Pac_Categoria == "B" ? "Categoria B" :
                                     getCita.Pac_Categoria == "C" ? "Categoria C" :
                                     getCita.Pac_Categoria == "Z" ? "Categoria Z" : "Z";
                    comboBox8.Text = pacientesController.getNamePais(getCita.Pac_PaisOrigen);
                    comboBox10.Text = pacientesController.getNamePais(getCita.Pac_PaisResidencia);
                    comboBox5.Text = getCita.Pac_Zona == "U" ? "Urbana" : "Rural";
                    textBox17.Text = getCita.Pac_Contrato;
                    label21.Text = zonasController.DepartamentoCodigo(getCita.Pac_Dep_Cod);
                    label23.Text = zonasController.DepartamentoCodigo(getCita.Pac_Mun_Cod);
                    CodeDep = getCita.Pac_Dep_Cod;
                    CodeMun = getCita.Pac_Mun_Cod;

                    Previos();

                    #region BONOS
                    if (getCita.Hor_Vales == "S")
                    {
                        Bonos = true;
                        pictureBox1.Image =  Resources2.comprobado;

                        MG.Mensaje = "Este paciente esta marcado que debe anexar vale, bono, pin segun corresponda";
                        MG.TipoImagen = 0;
                        MG.ShowDialog();
                    }
                    else
                    {
                        Bonos = false;
                        string ConfirmaValesUltimaCita = agendaController.UltimaAdmisionValidaVales(Admision, getCita.Hor_Pac_Id, getCita.Hor_Pac_Fecha_Cita);
                        if (ConfirmaValesUltimaCita == "S")
                        {
                            MG = new MensajesGeneral()
                            {
                                Mensaje = "Este paciente NO esta marcado para recaudo de vales o bonos, sin embargo la ultma cita asistida de este " +
                                "paciente si anexo vale/bono a su historial medico.  Por favor verifique si realmente requiere vale o firma"
                            };

                            MG.ShowDialog();
                        }
                    }
                    #endregion

                    #region DEP - MUN
                    if (string.IsNullOrEmpty(label21.Text))
                    {
                        label21.Text = "BOGOTÁ, D.C.";
                        CodeDep = "11";
                    }
                    if (string.IsNullOrEmpty(label23.Text))
                    {
                        label23.Text = "BOGOTÁ, D.C.";
                        CodeMun = "001";
                    }
                    #endregion

                    #region PAISES
                    if (comboBox8.Text == "")
                    {
                        comboBox8.Text = "COLOMBIA";
                    }
                    if (comboBox10.Text == "")
                    {
                        comboBox10.Text = "COLOMBIA";
                    }
                    #endregion

                    #region SESIONES
                    if (getCita.Hor_Pac_Tipo_Serv == "CU")
                    {
                        string canactual = agendaController.Calcular2(getCita.Hor_Pac_Id, getCita.Hor_Pac_Tipo_Serv);
                        if (canactual == "No es posible calcular sesion debido a que no hay registros de autorizaciones vigentes")
                        {
                            label27.Visible = false;
                        }
                        else
                        {
                            label27.Visible = true;
                        }

                        label27.Text = canactual.ToString();
                    }
                    else
                    {
                        label27.Visible = false;
                    }
                    #endregion

                    #region CITAS DEL MISMO DIA
                    await CitasMismoDia(getCita.Hor_Pac_Id, Convert.ToDateTime(getCita.Hor_Pac_Fecha_Cita), getCita.Hor_Pac_Bod);
                    #endregion

                    #region CONSULTAR AUTORIZACION
                    (string Autorizacion, string Cantidad, string Estado) Obtener = agendaController.CitaMismoDiaGetAutorizacion(PacId, Convert.ToDateTime(getCita.Hor_Pac_Fecha_Cita), getCita.Hor_Pac_Bod);
                    if (!string.IsNullOrEmpty(Obtener.Cantidad) && !string.IsNullOrEmpty(Obtener.Autorizacion))
                    {
                        if (Obtener.Autorizacion == "PENDIENTE INGRESAR")
                        {
                            textBox1.Text = "PENDIENTE INGRESAR";
                            textBox2.Text = "";

                            Pendiente = true;
                            pictureBox3.Image = Resources2.comprobado;

                            InicioAutoriza = true;

                            label37.Visible = true;
                            label43.Visible = true;
                            label44.Visible = true;
                            textBox1.Visible = true;
                            textBox2.Visible = true;
                            label46.Visible = true;
                            comboBox12.Visible = true;
                            pictureBox3.Visible = true;
                            pictureBox2.Image = Resources2.comprobado;
                        }
                        else
                        {
                            textBox1.Text = Obtener.Autorizacion;
                            textBox2.Text = Obtener.Cantidad;

                            InicioAutoriza = true;

                            label37.Visible = true;
                            label43.Visible = true;
                            label44.Visible = true;
                            textBox1.Visible = true;
                            textBox2.Visible = true;
                            label46.Visible = true;
                            comboBox12.Visible = true;
                            pictureBox3.Visible = true;
                            pictureBox2.Image = Resources2.comprobado;
                        }

                        if (Obtener.Estado == "N")
                        {
                            comboBox12.Text = "Paciente Nuevo";
                        }
                        else if (Obtener.Estado == "A")
                        {
                            comboBox12.Text = "Paciente Inicio Paquete";
                        }
                        else
                        {
                            comboBox12.Text = "";
                        }
                    }
                    #endregion
                }
                else
                {
                    MG = new MensajesGeneral()
                    {
                        Mensaje = "Esta admision no esta en un estado que permita Admisionar",
                        TipoImagen = 1000
                    };

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
        #region CITAS MISMO DIA
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

            dataGridView1.ClearSelection();
        }
        async Task<List<CXN_HORARIO>> CargarCitas(int idPac, DateTime fecha, int bodega)
        {
            return await Task.Run(() =>
            {
                return agendaController.ListarCitasXPaciente(idPac, fecha, bodega);
            });
        }
        async Task CitasMismoDia(int idPac, DateTime fecha, int bodega)
        {
            try
            {
                dataGridView1.DataSource = null;

                List<CXN_HORARIO> L = new List<CXN_HORARIO>();
                L = await CargarCitas(idPac, fecha, bodega);
                if (L != null)
                {
                    panel1.Visible = true;
                    dt = new DataTable();
                    DataColumn POS;
                    DataColumn ADM;
                    DataColumn CitaProgramada;

                    dt = new DataTable();
                    POS = dt.Columns.Add("POS", typeof(int));
                    ADM = dt.Columns.Add("ADM", typeof(int));
                    CitaProgramada = dt.Columns.Add("CitaProgramada", typeof(string));
                    int Contador = 1;

                    foreach (CXN_HORARIO i in L)
                    {
                        DataRow row = dt.NewRow();

                        row["POS"] = Contador;
                        row["Adm"] = i.Hor_Id;
                        row["CitaProgramada"] = "ADMISION: " + i.Hor_Id.ToString() + "\n" +
                            "FECHA: " + i.Hor_Pac_Fecha_Cita.ToString(Conexion.ConectionDictionary["Format_Fecha"]) + "\n" +
                            "HORA: " + i.Hor_Pac_Hora_Cita.ToString("hh:mm tt") + "\n" +
                            "PROFESIONAL: " + i.Hor_Observacion;

                        dt.Rows.Add(row);
                        dt.AcceptChanges();

                        Contador++;
                    }

                    Contador = 0;

                    Estilos();

                    dataGridView1.CellFormatting += DataGridView1_CellFormatting;
                    dataGridView1.ClearSelection();
                }
                else
                {
                    panel1.Visible = false;
                }
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
                dataGridView1.ScrollBars = ScrollBars.Both;
                dataGridView1.ColumnHeadersVisible = false;

                dataGridView1.DataSource = dt;

                dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font(dataGridView1.Font, FontStyle.Bold);
                dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 8, FontStyle.Bold);
                dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#bfdbff");
                dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;
                dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;

                dataGridView1.Font = new Font("Arial", 9, FontStyle.Bold);
                dataGridView1.ForeColor = Color.DarkBlue;

                dataGridView1.Columns["POS"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView1.Columns["ADM"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView1.Columns["CitaProgramada"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

                dataGridView1.Columns["POS"].Visible = false;
                dataGridView1.Columns["ADM"].Visible = false;

                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells + 10;
                dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
                dataGridView1.DefaultCellStyle.WrapMode = DataGridViewTriState.True;

                dataGridView1.ClearSelection();
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                AdmisionesCuraciones A = new AdmisionesCuraciones(Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString()));

                this.Dispose();
                this.Close();

                A.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion
        private void label21_DoubleClick(object sender, EventArgs e)
        {
            Extras.Zonas Z = new Extras.Zonas("AdmisionesCuraciones", "Departamento", "");
            Z.ShowDialog();
        }
        public void setCodeZSones(string Tipe, string Code)
        {
            if (Tipe == "Dep")
            {
                CodeDep = Code;
            }
            if (Tipe == "Mun")
            {
                CodeMun = Code;
            }
        }
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image == null)
            {
                Bonos = true;
                pictureBox1.Image = Resources2.comprobado;
            }
            else
            {
                Bonos = false;
                pictureBox1.Image = null;
            }
        }
        private void pictureBox2_Click(object sender, EventArgs e)
        {
            if (pictureBox2.Image == null)
            {
                InicioAutoriza = true;

                label37.Visible = true;
                label43.Visible = true;
                label44.Visible = true;
                textBox1.Visible = true;
                textBox2.Visible = true;
                label46.Visible = true;
                comboBox12.Visible = true;
                pictureBox3.Visible = true;
                pictureBox2.Image = Resources2.comprobado;
            }
            else
            {
                InicioAutoriza = false;

                label37.Visible = false;
                label43.Visible = false;
                label44.Visible = false;
                textBox1.Visible = false;
                textBox2.Visible = false;
                label46.Visible = false;
                comboBox12.Visible = false;
                pictureBox3.Visible = false;
                pictureBox2.Image = null;
            }
        }
        private void comboBox7_SelectedIndexChanged(object sender, EventArgs e)
        {
            Ase = aseguradorasController.getInfoFromAsebyName(comboBox7.Text).Ase_Identificador;
        }
        private void boton1_Click(object sender, EventArgs e)
        {
            panel1.Visible = false;
        }
        private void pictureBox3_Click(object sender, EventArgs e)
        {
            if (pictureBox3.Image == null)
            {
                Pendiente = true;
                pictureBox3.Image = Resources2.comprobado;
            }
            else
            {
                Pendiente = false;
                pictureBox3.Image = null;
            }
        }
        private void label23_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(label21.Text))
                {
                    MG.Mensaje = "Debe seleccionar el departamento primero";
                    MG.TipoImagen = 0;
                    MG.ShowDialog();
                    return;
                }

                Extras.Zonas Z = new Extras.Zonas("AdmisionesCuraciones", "Municipio", label21.Text);
                Z.ShowDialog();
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void boton2_Click(object sender, EventArgs e)
        {
            try
            {
                Comunes.MensajesGeneral MG = new Comunes.MensajesGeneral();

                bool celular = pacientesController.ValidaCelular(textBox13.Text);
                if (celular != true)
                {
                    MG.Mensaje = "El numero de celular es incorrecto, este campo es oligatorio";
                    MG.TipoImagen = 1000;
                    MG.ShowDialog();
                    return;
                }
                if (textBox12.Text != "")
                {
                    bool email = pacientesController.ValidaEmail(textBox12.Text);
                    if (email != true)
                    {
                        MG.Mensaje = "El formato del correo es incorrecto, si no lo conoce deje esta casilla en blanco";
                        MG.TipoImagen = 1000;
                        MG.ShowDialog();
                        return;
                    }
                }
                if (comboBox5.Text == "")
                {
                    MG.Mensaje = "Debe seleccionar la zona urbana del paciente";
                    MG.TipoImagen = 1000;
                    MG.ShowDialog();
                    return;
                }
                if (comboBox8.Text == "")
                {
                    MG.Mensaje = "Debe seleccionar el pais de nacimiento del paciente";
                    MG.TipoImagen = 1000;
                    MG.ShowDialog();
                    return;
                }
                if (comboBox10.Text == "")
                {
                    MG.Mensaje = "Debe seleccionar el pais de residencia del paciente";
                    MG.TipoImagen = 1000;
                    MG.ShowDialog();
                    return;
                }
                if (comboBox2.Text == "")
                {
                    MG.Mensaje = "Debe seleccionar Metodo Valida Si o No";
                    MG.TipoImagen = 1000;
                    MG.ShowDialog();
                    return;
                }
                if (comboBox3.Text == "")
                {
                    MG.Mensaje = "Debe seleccionar regimen del paciente";
                    MG.TipoImagen = 1000;
                    MG.ShowDialog();
                    return;
                }
                if (comboBox9.Text == "")
                {
                    MG.Mensaje = "Debe seleccionar sexo del paciente";
                    MG.TipoImagen = 1000;
                    MG.ShowDialog();
                    return;
                }
                if (string.IsNullOrEmpty(label21.Text))
                {
                    MG.Mensaje = "Debe seleccionar un departamento de residencia del paciente";
                    MG.TipoImagen = 1000;
                    MG.ShowDialog();
                    return;
                }
                if (string.IsNullOrEmpty(label23.Text))
                {
                    MG.Mensaje = "Debe seleccionar un municipio de residencia del paciente";
                    MG.TipoImagen = 1000;
                    MG.ShowDialog();
                    return;
                }
                if (textBox18.Text == "")
                {
                    MG.Mensaje = "Digite primer nombre del paciente";
                    MG.TipoImagen = 1000;
                    MG.ShowDialog();
                    return;
                }
                if (textBox20.Text == "")
                {
                    MG.Mensaje = "Digite primer apellido del paciente";
                    MG.TipoImagen = 1000;
                    MG.ShowDialog();
                    return;
                }
                if (comboBox4.Text == "")
                {
                    MG.Mensaje = "Seleccione Categoria del Paciente";
                    MG.TipoImagen = 1000;
                    MG.ShowDialog();
                    return;
                }
                if (PacRules.ValidarRegimen(comboBox3.Text, comboBox4.Text) == false)
                {
                    MG = new MensajesGeneral();
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "Si el regimen es no afiliado, la categoria no puede ser A, B, C o Z.  Si el regimen es diferente a no afiliado, la categoria no puede ser no aplica";
                    MG.ShowDialog();
                    return;
                }

                DateTime Hoy = DateTime.Now.Date;

                if (Convert.ToDateTime(dateTimePicker1.Value.ToString(Conexion.ConectionDictionary["Format_Fecha"])) >= Convert.ToDateTime(Hoy.ToString(Conexion.ConectionDictionary["Format_Fecha"])))
                {
                    MG.Mensaje = "La fecha de nacimiento no coincide con una edad real";
                    MG.TipoImagen = 1000;
                    MG.ShowDialog();
                    return;
                }

                int Canti = 0;
                string Autorizaciion = "";
                string IniciaSesion = "N";
                Console.WriteLine("SERVICIO 1: " + comboBox1.Text);

                #region 1 AUTORIZACIONES
                //Se marca la opcion de inicio de autorizacion
                if (InicioAutoriza == true)
                {
                    IniciaSesion = "S";
                    //Valido que sea inicio o nuevo
                    if (comboBox12.Text == "")
                    {
                        MG = new MensajesGeneral()
                        {
                            Mensaje = "Cuando selecciona la opcion de autorizacion se espera un inicio de paquete o paciente nuevo",
                            TipoImagen = 1000
                        };
                        MG.ShowDialog();
                        return;
                    }

                    //Se valida si esa pendiente la autorizacion pero es inicio
                    if (Pendiente == true)
                    {
                        if (comboBox1.Text == "CURACION COMPLEJIDAD ALTA")
                        {
                            Autorizaciion = "PENDIENTE INGRESAR";
                            Canti = 12;
                        }
                        else if (comboBox1.Text == "CURACION COMPLEJIDAD MEDIA")
                        {
                            Autorizaciion = "PENDIENTE INGRESAR";
                            Canti = 8;
                        }
                        else if (comboBox1.Text == "CURACION COMPLEJIDAD BAJA")
                        {
                            Autorizaciion = "PENDIENTE INGRESAR";
                            Canti = 6;
                        }
                        else if (comboBox1.Text == "CURACION DE LESION EN PIEL O TEJIDO CELULAR SUBCUTANEO SOD")
                        {
                            Autorizaciion = "PENDIENTE INGRESAR";
                            Canti = 1;
                        }
                    }
                    //si no esta pendiente
                    else
                    {
                        if (string.IsNullOrEmpty(textBox1.Text) || string.IsNullOrEmpty(textBox2.Text))
                        {
                            MG = new MensajesGeneral()
                            {
                                Mensaje = "Cuando selecciona la opcion de autorizacion se espera un numero de autorizacion y una cantidad de sesiones",
                                TipoImagen = 1000
                            };
                            MG.ShowDialog();
                            return;
                        }
                        if (textBox2.Text == "0")
                        {
                            MG = new MensajesGeneral()
                            {
                                Mensaje = "Cuando selecciona la opcion de autorizacion se espera una cantidad de sesiones mayor a cero",
                                TipoImagen = 1000
                            };
                            MG.ShowDialog();
                            return;
                        }

                        Canti = Convert.ToInt32(textBox2.Text);
                        Autorizaciion = textBox1.Text;
                    }
                }
                else
                {
                    //En autorizaciones 2 me pone "" como normal
                }
                #endregion

                string Retardo = "";

                if (textBox9.Text != "")
                {
                    Retardo = Microsoft.VisualBasic.Interaction.InputBox(
                            "Retardo de Citas",
                            "Digite la razon del retardo del paciente",
                                "");
                }

                Actualiza_Pac();

                Console.WriteLine("SERVICIO: " + comboBox1.Text);
                CXN_CONVENIOS CUPServicio = conveniosController.ServicioCUP(comboBox1.Text, Ase);
                if (CUPServicio == null)
                {
                    MG = new MensajesGeneral()
                    {
                        Mensaje = "Verifique la aseguradora ya que la actual no tiene convenio con el servicio seleccionado",
                        TipoImagen = 1000
                    };
                    MG.ShowDialog();
                    return;
                }

                DateTime Hora_Llega = DateTime.Now;
                Hora_Llega = Convert.ToDateTime(Hora_Llega.ToString("HH:mm"));

                CXN_HORARIO H = new CXN_HORARIO
                {
                    Hor_Estado = "P",
                    Hor_Autoriza = Autorizaciion,
                    Hor_Valida = comboBox2.Text,
                    Hor_Pac_Ase = Ase,
                    Hor_Imp_Age = textBox20.Text + " " + textBox21.Text + " " + textBox18.Text + " " + textBox19.Text,
                    Hor_Regimen = pacientesController.Regimen(comboBox3.Text),
                    Hor_Pac_Llegada = Convert.ToDateTime(Hora_Llega),
                    Hor_Usr_Admisiona = Comunes.Contenedor.UsuarioLogueado,
                    Hor_Pac_Minutos = textBox9.Text,
                    Hor_Pac_Razon = (Retardo == null ? "" : Retardo),
                    Hor_Pac_Cup = CUPServicio.Con_Id_Serv.ToString(),
                    Hor_RegAtn = textBox3.Text,  //registro de atencion
                    Hor_CantSesion = Canti,  //cantidad de sesiones
                    Hor_IniciaSesion = IniciaSesion, //Iniciar sesion
                    Hor_ValDerechos = textBox10.Text, //pines
                    Hor_Id = Admision,
                    Hor_Observacion = " ||| CITA ADMISIONADA POR " + Comunes.Contenedor.UsuarioLogueado,
                    Hor_Vales = Bonos == true ? "S" : "N"
                };

                bool _updateCita = horarioController.updateCitaAdmisionar(H);

                #region 2 AUTORIACIONES
                //Actualizo si es inicio o nuevo
                if (comboBox12.Text == "Paciente Nuevo" && comboBox12.Visible == true)
                {
                    horarioController.PendientesChecked(Admision, "N");
                }
                else if (comboBox12.Text == "Paciente Inicio Paquete" && comboBox12.Visible == true)
                {
                    horarioController.PendientesChecked(Admision, "A");
                }
                else
                {
                    horarioController.PendientesChecked(Admision, "");
                }

                //Poner autorizacion en MG si la hay
                if (InicioAutoriza == true)
                {
                    if (Pendiente == false)
                    {
                        int consultaMGmismodia = horarioController.consularMGMismoDia(PacId, Convert.ToDateTime(_fechas));
                        if (consultaMGmismodia > 0)
                        {
                            horarioController.ActualizarAutorizacionMG(Admision, textBox1.Text, Convert.ToInt32(textBox2.Text));
                        }
                    }
                }

                //Agregar a pendientes si esta pendiente true
                if (Pendiente == true)
                {
                    CXN_OPEND OP = new CXN_OPEND
                    {
                        OP_Adm = Admision,
                        OP_Estado = "P",
                        OP_Registra = Comunes.Contenedor.UsuarioLogueado
                    };
                    horarioController.OPend(OP);
                }
                #endregion

                Agenda f7 = Application.OpenForms.OfType<Agenda>().FirstOrDefault();
                f7.RechargeTrueCheck();

                PopupNotifier Pop = PopUps.setPopUp(Properties.Resources2.comprobado,
                                                            Color.LightBlue,
                                                            "CITA ADMISIONADA",
                                                            Color.DarkBlue,
                                                            "Cita de " + H.Hor_Imp_Age + " admisionada exitosamente");

                Pop.Popup();

                if (Bonos == true)
                {
                    Extras.RcCaja RcCaja = new Extras.RcCaja(Admision);
                    RcCaja.ShowDialog();
                }
                else
                {
                    if (Preferencias.TabletaFirmas == "A")
                    {
                        FirmaDigital firmaDigital = new FirmaDigital(Admision, PacId, "Admisiones");
                        firmaDigital.ShowDialog();
                    }
                    else
                    {
                        //aqui programar hoja autoomatica
                    }
                }

                ConsultaAdmision f = Application.OpenForms.OfType<ConsultaAdmision>().FirstOrDefault();
                f.Close();

                this.Dispose();
                this.Close();
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        public void Actualiza_Pac()
        {
            try
            {
                string regi = pacientesController.Regimen(comboBox3.Text);
                string Sex = "";
                string Zona = "U";

                switch (comboBox9.Text)
                {
                    case "Hombre":
                        Sex = "M";
                        break;
                    case "Mujer":
                        Sex = "F";
                        break;
                    case "Intersexual":
                        Sex = "I";
                        break;
                    default:
                        MG.TipoImagen = 1000;
                        MG.Mensaje = "Debe seleccionar sexo del paciente";
                        MG.ShowDialog();
                        return;
                }

                if (comboBox5.Text == "Rural")
                {
                    Zona = "R";
                }

                Ase = aseguradorasController.getInfoFromAsebyName(comboBox7.Text).Ase_Identificador;

                CXN_PACIENTES P = new CXN_PACIENTES
                {
                    Pac_Telefono = textBox13.Text,
                    Pac_TelefonoAux = textBox14.Text,
                    Pac_PrimerN = textBox18.Text,
                    Pac_SegundoN = textBox19.Text,
                    Pac_PrimerA = textBox20.Text,
                    Pac_SegundoA = textBox21.Text,
                    Pac_Email = textBox12.Text,
                    Pac_Regimen = regi,
                    Pac_Sexo = Sex,
                    Pac_Dep_Cod = CodeDep,
                    Pac_Mun_Cod = CodeMun,
                    Pac_Aseguradora = Ase,
                    Pac_FechaNto = Convert.ToDateTime(dateTimePicker1.Value.Date),
                    Pac_Id = PacId,
                    Pac_Zona = Zona,
                    Pac_TipoId = comboBox6.Text,
                    Pac_Contrato = textBox17.Text,
                    Pac_PaisOrigen = pacientesController.getCodePais(comboBox8.Text),
                    Pac_Residencia = pacientesController.getCodePais(comboBox10.Text),
                    Pac_ECivil = comboBox11.Text,
                    Pac_Categoria = comboBox4.Text == "Categoria A" ? "A" :
                                    comboBox4.Text == "Categoria B" ? "B" :
                                    comboBox4.Text == "Categoria C" ? "C" :
                                    comboBox4.Text == "Categoria Z" ? "Z" : "N"
                };              

                pacientesController.Actualiza_Pac2(P);

                string Paci_Hori = textBox20.Text + " " + textBox21.Text + " " + textBox18.Text + " " + textBox19.Text;
                horarioController._updateAseHorario(Ase, Paci_Hori, Admision);
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        void Previos()
        {
            try
            {
                List<CXN_HORARIO> _previos = horarioController.CargarPrevios(PacId);
                if (_previos != null)
                {
                    Encabezados();
                    int Contador = 1;

                    foreach (var i in _previos)
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


                        DataRow row = dt2.NewRow();

                        row[POS] = Contador;
                        row[ADM] = i.Hor_Id.ToString();
                        row[FECHA] = Convert.ToDateTime(i.Hor_Pac_Fecha_Cita).ToString(Conexion.ConectionDictionary["Format_Fecha"]);
                        row[PROFESIONAL] = i.Hor_Observacion.ToString();
                        row[ESTADO] = Est;
                        row[SESIONES] = string.IsNullOrEmpty(i.Hor_IniciaSesion) ? "" : 
                                        "Autorizacion: " + i.Hor_Autoriza + 
                                        Environment.NewLine +
                                        Environment.NewLine +
                                        "Cantidad: " + i.Hor_CantSesion.ToString();

                        dt2.Rows.Add(row);
                        dt2.AcceptChanges();

                        Contador++;
                    }

                    EstilosGrid2();                   
                }
                else
                {
                    Encabezados();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void DataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int Cod = Convert.ToInt32(dataGridView2.Rows[e.RowIndex].Cells[1].Value.ToString());

            DatosCita datosCita = new DatosCita(Admision, "");
            datosCita.ShowDialog();

            dataGridView2.ClearSelection();
        }
        void EstilosGrid2()
        {
            dataGridView2.DataSource = dt2;
            dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells + 10;
            dataGridView2.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dataGridView2.DefaultCellStyle.WrapMode = DataGridViewTriState.True;

            dataGridView2.Columns["POS"].Visible = false;

            foreach (DataGridViewRow row in dataGridView2.Rows)
            {
                string state = row.Cells["ESTADO"].Value.ToString();

                if (state == "Cancelo")
                {
                    row.DefaultCellStyle.BackColor = Color.Orange;
                    row.DefaultCellStyle.ForeColor = Color.Red;
                }
                else if (state == "Pendiente")
                {
                    row.DefaultCellStyle.BackColor = Color.LightGreen;
                    row.DefaultCellStyle.ForeColor = Color.Green;
                }
                else if (state == "Asistio")
                {
                    row.DefaultCellStyle.BackColor = Color.LightBlue;
                    row.DefaultCellStyle.ForeColor = Color.Blue;
                }
                else
                {
                    row.DefaultCellStyle.BackColor = Color.White;
                    row.DefaultCellStyle.ForeColor = Color.Black;
                }
            }

            dataGridView2.ClearSelection();
        }
        void Encabezados()
        {
            dataGridView2.DataSource = null;
            dt2 = new DataTable();
            POS = dt2.Columns.Add("POS", typeof(int));
            ADM = dt2.Columns.Add("ADM", typeof(int));
            FECHA = dt2.Columns.Add("FECHA", typeof(string));
            PROFESIONAL = dt2.Columns.Add("PROFESIONAL", typeof(string));
            ESTADO = dt2.Columns.Add("ESTADO", typeof(string));
            SESIONES = dt2.Columns.Add("SESIONES", typeof(string));
        }
    }
}
