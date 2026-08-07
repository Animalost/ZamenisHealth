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
using System.Threading.Tasks;
using System.Windows.Forms;
using Tulpep.NotificationWindow;
using ZamenisHealth.Clases;
using ZamenisHealth.Comunes;
using ZamenisHealth.Recepcion.AgendaDiaria;
using ZamenisHealth.Recepcion.Extras;

namespace ZamenisHealth.Recepcion
{
    public partial class Admisiones : ConfigForm.BaseForm
    {
        private static readonly IPacientes repositorioPacientes = new MPacientes();
        private static readonly IAseguradoras repositorioAseguradora = new MAseguradoras();
        private static readonly IAgenda repositorioHorario = new MAgenda();
        private static readonly IConvenios repositorioConvenios = new MConvenios();
        private static readonly IPacientes repositorioIPacientes = new MPacientes();
        private static readonly IZonas repositorioZonas = new MZonas();
        private static readonly IAgendaC repositorioFechasAgendaa = new MAgendaC();
        private static readonly IOrdenes repositorioOrdenes = new MOrdenes();
        private static readonly IConfSystem RepoConfSystem = new MConfSystem();

        private int Admi, Paciente, Bodega_Uno;
        private string inputBoxRetardo, Tserv, Pac_Sal;
        List<Modelo> modelo;
        List<FirmasR> modelos;
        private bool FirmasAutoComplete, RequiereVales;
        private DataTable dt;
        Comunes.MensajesGeneral MG = new Comunes.MensajesGeneral();
        public Admisiones(int _admi)
        {
            InitializeComponent();
            this.Admi = _admi;
            
            ConfigForm.MoverForma(label52, this);
            ConfigForm.MoverForma(panel4, this);
        }
 
        void ValidarBarras()
        {
            try
            {
                Dictionary<string, string> getConfig = new Dictionary<string, string>();
                                    
                getConfig = RepoConfSystem.getListado();
                                
                
                if (getConfig != null)
                {                    
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
        private async void Admisiones_Load(object sender, EventArgs e)
        {
            try
            {
                Titulo.Visible = false;
                ImageClose.Visible = false;

                ValidarBarras();

                ConfigForm.SoloNumeros(textBox9);
                ConfigForm.SoloNumeros(textBox13);
                ConfigForm.SoloNumeros(textBox16);        
                
                textBox13.MaxLength = 10;

                List<string> getDocs = repositorioPacientes.ListaDocs();
                
                if (getDocs != null)
                {
                    comboBox6.Items.Clear();

                    foreach (string i in getDocs)
                    {
                        comboBox6.Items.Add(i.ToString());
                    }                    
                }

                otrosDatosPacienteHorario _datosAdmision =  repositorioFechasAgendaa.cargarAdmision(Admi, "'A'");
                
                if (_datosAdmision != null)
                {
                    this.Pac_Sal = _datosAdmision.Hor_Pac_Sal;

                    List<string> Servicios =  repositorioConvenios.CargarServicios(_datosAdmision.Hor_Pac_Tipo_Serv,
                                                                         _datosAdmision.Hor_Pac_Ase);
                    
                    if (Servicios != null)
                    {
                        comboBox1.Items.Clear();

                        foreach (var i in Servicios)
                        {
                            comboBox1.Items.Add(i); 
                        }
                    }
                    else
                    {
                        MG.Mensaje = "No se logro cargar los servicios de este pacientes, probablemente la aseguradora no tiene convenio con la clase de " +
                            "EPS o MP del paciente";
                        MG.TipoImagen = 1000;
                        MG.ShowDialog();
                        this.Dispose();
                        this.Close();
                        return;
                    }

                    CXN_CONVENIOS DatServ = repositorioConvenios.ServicioNombre(_datosAdmision.Hor_Pac_Cup,
                                                                      _datosAdmision.Hor_Pac_Ase,
                                                                      _datosAdmision.Hor_Pac_Tipo_Serv);  
                    

                    if (DatServ != null)
                    {
                        comboBox1.Text = DatServ.Con_Nombre;
                    }
                    else
                    {
                        MG.Mensaje = "No se logro cargar los servicios de este pacientes, probablemente la aseguradora no tiene convenio con la clase de " +
                            "EPS o MP del paciente al seleccionar algun servicio";
                        MG.TipoImagen = 1000;
                        MG.ShowDialog();
                        this.Dispose();
                        this.Close();
                        return;
                    }

                    CargarAseguradoras(_datosAdmision.Hor_Pac_Ase,
                                       _datosAdmision.Hor_Pac_Tipo_Serv);

                    CargarRegimen();

                    CXN_ASEGURADORA Aseguradora = new CXN_ASEGURADORA();
                    List<string> getPaises = new List<string>();
                    string NombreRegimen = "";
                    string Departamento = "";
                    string Municipio = "";                    

                        Aseguradora = repositorioAseguradora.getInfoFromAsebyCode(_datosAdmision.Hor_Pac_Ase);
                        NombreRegimen = repositorioPacientes.Carga_Regimen(_datosAdmision.Hor_Regimen);
                        Departamento = repositorioZonas.DepartamentoNombre(_datosAdmision.Pac_Dep_Cod);
                        Municipio = repositorioZonas.MunicipioNombre(_datosAdmision.Pac_Mun_Cod, _datosAdmision.Pac_Dep_Cod);
                        getPaises = repositorioIPacientes.getListPaises();
                    
                    
                    comboBox7.Text = Aseguradora.Ase_Descripcion.ToString();                   
                    comboBox3.Text = NombreRegimen;            
                    label21.Text = Departamento.ToString();
                    label23.Text = Municipio.ToString();
                    Paciente = _datosAdmision.Hor_Pac_Id;
                    Bodega_Uno = _datosAdmision.Hor_Pac_Bod;
                    textBox1.Text = Admi.ToString();
                    textBox2.Text = Convert.ToDateTime(_datosAdmision.Hor_Pac_Fecha_Cita).ToString(Conexion.ConectionDictionary["Format_Fecha"]);
                    textBox4.Text = Convert.ToDateTime(_datosAdmision.Hor_Pac_Hora_Cita).ToString("HH:mm tt");
                    textBox5.Text = _datosAdmision.Bod_Responsable;
                    textBox6.Text = _datosAdmision.Hor_Pac_Ase.ToString();
                    textBox7.Text = _datosAdmision.Com_Nombre;
                    textBox8.Text = _datosAdmision.Pac_TipoId + " " + _datosAdmision.Pac_IdNum.Trim();
                    textBox18.Text = _datosAdmision.Pac_PrimerN;
                    textBox19.Text = _datosAdmision.Pac_SegundoN;
                    textBox20.Text = _datosAdmision.Pac_PrimerA;
                    textBox21.Text = _datosAdmision.Pac_SegundoA;
                    textBox13.Text = _datosAdmision.Pac_Telefono;
                    textBox14.Text = _datosAdmision.Pac_TelefonoAux;
                    textBox12.Text = _datosAdmision.Pac_Email;
                    textBox15.Text = _datosAdmision.Hor_Observacion;
                    textBox17.Text = _datosAdmision.Pac_Contrato;
                    label22.Text = _datosAdmision.Pac_Dep_Cod;
                    label24.Text = _datosAdmision.Pac_Mun_Cod;
                    comboBox6.Text = _datosAdmision.Pac_TipoId;
                    comboBox11.Text = _datosAdmision.Pac_ECivilLoadAdmition;
                    textBox22.Text = _datosAdmision.Pac_CorreoLoadAdmition;
                    textBox23.Text = _datosAdmision.Pac_TelefonoLoadAdmition;
                    textBox24.Text = _datosAdmision.Pac_DireccionLoadAdmition;
                    textBox25.Text = _datosAdmision.Pac_ParentescoLoadAdmition;
                    textBox26.Text = _datosAdmision.Pac_AcudienteLoadAdmition;
                    dateTimePicker1.Text = Convert.ToDateTime(_datosAdmision.Pac_FechaNto).ToString(Conexion.ConectionDictionary["Format_Fecha"]);
                    Tserv = _datosAdmision.Hor_Pac_Tipo_Serv;

                    switch (_datosAdmision.Pac_Sexo)
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

                    comboBox5.Text = (_datosAdmision.Pac_Zona != "R" ? "Urbana" : "Rural");

                    if (_datosAdmision.Pac_Bonos == "A")
                    {
                        checkBox6.Checked = true;
                        RequiereVales = true;
                        MG.Mensaje = "Este paciente esta marcado que debe anexar vale, bono, pin segun corresponda";
                        MG.TipoImagen = 0;
                        MG.ShowDialog();
                    }
                    else
                    {
                        checkBox6.Checked = false;
                        RequiereVales = false;
                    }
                    
                    if (getPaises != null)
                    {
                        foreach (string pa in getPaises)
                        {
                            comboBox8.Items.Add(pa);
                        }

                        foreach (string pa in getPaises)
                        {
                            comboBox10.Items.Add(pa);
                        }
                    }

                    switch (_datosAdmision.Pac_Categoria)
                    {
                        case "A":
                            comboBox4.SelectedIndex = 0;
                            break;

                        case "B":
                            comboBox4.SelectedIndex = 1;
                            break;

                        case "C":
                            comboBox4.SelectedIndex = 2;
                            break;

                        case "Z":
                            comboBox4.SelectedIndex = 3;
                            break;

                        default:
                            comboBox4.SelectedIndex = 4;
                            break;
                    }

                    
                        comboBox8.Text = repositorioIPacientes.getNamePais(_datosAdmision.Pac_PaisOrigen); //pais origen
                        comboBox10.Text = repositorioIPacientes.getNamePais(_datosAdmision.Pac_PaisResidencia); //pais residenciua
                                       

                    //Verificar mas citas del dia
                    await CitasMismoDia(Paciente, Convert.ToDateTime(textBox2.Text), Bodega_Uno);
                    //Fin Verificar mas citas del dia

                    try
                    {
                        Dictionary<string, string> datos = new Dictionary<string, string>();

                        datos = repositorioFechasAgendaa.ConsultaDatosAutorizacionMedGen(Paciente, Convert.ToDateTime(_datosAdmision.Hor_Pac_Fecha_Cita));
                        
                         
                        if (datos != null && datos["Confirma"] == "1")
                        {
                            if (datos["Hor_Pac_Sal"] == "A")
                            {
                                checkBox2.Checked = true;
                                checkBox1.Checked = false;

                                textBox11.Text = datos["Autorizacion"];
                                textBox16.Text = datos["Cantidad"];
                                textBox3.Text = datos["Registro"];
                            }
                            else if (datos["Hor_Pac_Sal"] == "N")
                            {
                                checkBox1.Checked = true;
                                checkBox2.Checked = false;

                                textBox11.Text = datos["Autorizacion"];
                                textBox16.Text = datos["Cantidad"];
                                textBox3.Text = datos["Registro"];
                            }
                            else
                            {
                                checkBox2.Checked = false;
                                checkBox1.Checked = false;
                                textBox11.Text = "";
                                textBox16.Text = "";
                                textBox3.Text = "";
                            }
                        }
                        else
                        {
                            checkBox1.Checked = false;
                            textBox11.Text = "";
                            textBox16.Text = "";
                            textBox3.Text = "";
                        }

                        string canactual =  repositorioFechasAgendaa.Calcular2(Paciente, _datosAdmision.Hor_Pac_Tipo_Serv);
                        
                        if (canactual == "No es posible calcular sesion debido a que no hay registros de autorizaciones vigentes")
                        {
                            label12.Visible = false;
                        }
                        else
                        {
                            label12.Visible = true;
                        }

                        label12.Text = canactual.ToString();                       
                    }
                    catch
                    {
                        label12.Text = "ERROR INESPERADO EN SESIONES";
                        label12.Visible = false;
                    }

                    if (label21.Text == "")
                    {
                        label21.Text = "BOGOTÁ, D.C.";
                        label22.Text = "11";
                    }
                    if (label23.Text == "")
                    {
                        label23.Text = "BOGOTÁ, D.C.";
                        label24.Text = "001";
                    }
                    if (comboBox8.Text == "")
                    {
                        comboBox8.Text = "COLOMBIA";
                    }
                    if (comboBox10.Text == "")
                    {
                        comboBox10.Text = "COLOMBIA";
                    }
                }
                else
                {
                    toolStripButton2.Enabled = false;
                    MG.Mensaje = "Error en Esta admision";
                    MG.TipoImagen = 1000;
                    MG.ShowDialog();
                    this.Dispose();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        async Task<List<CXN_HORARIO>> CargarCitas(int idPac, DateTime fecha, int bodega)
        {
            return await Task.Run(() =>
            {
               return repositorioFechasAgendaa.ListarCitasXPaciente(idPac, fecha, bodega);                
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

                dataGridView1.Columns["POS"].Width = 0;
                dataGridView1.Columns["ADM"].Width = 0;
                dataGridView1.Columns["CitaProgramada"].Width = 850;

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

                dataGridView1.Columns["POS"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridView1.Columns["ADM"].SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridView1.Columns["CitaProgramada"].SortMode = DataGridViewColumnSortMode.NotSortable;

                dataGridView1.Columns["POS"].Visible = false;
                dataGridView1.Columns["ADM"].Visible = false;

                dataGridView1.ClearSelection();
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
        private void label21_DoubleClick(object sender, EventArgs e)
        {
            Extras.Zonas Z = new Extras.Zonas("Admisiones", "Departamento", "");
            Z.ShowDialog();
        }
        private void label23_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                Comunes.MensajesGeneral MG = new Comunes.MensajesGeneral();
                if (label22.Text == "")
                {
                    MG.Mensaje = "Debe seleccionar el departamento primero";
                    MG.TipoImagen = 0;
                    MG.ShowDialog();
                    return;
                }

                if (label21.Text == "")
                {
                    MG.Mensaje = "Debe seleccionar el departamento primero";
                    MG.TipoImagen = 0;
                    MG.ShowDialog();
                    return;
                }

                Extras.Zonas Z = new Extras.Zonas("Admisiones", "Municipio", label21.Text);
                Z.ShowDialog();
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void comboBox7_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                CXN_ASEGURADORA CodeAse = new CXN_ASEGURADORA();

                CodeAse = repositorioAseguradora.getInfoFromAsebyName(comboBox7.Text);
                

                textBox6.Text = CodeAse.Ase_Identificador.ToString();
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        void Cerrar()
        {
            this.Dispose();
            this.Close();
        }
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            try
            {
                Comunes.MensajesGeneral MG = new Comunes.MensajesGeneral();

                bool celular = repositorioIPacientes.ValidaCelular(textBox13.Text);
                if (celular != true)
                {
                    MG.Mensaje = "El numero de celular es incorrecto, este campo es oligatorio";
                    MG.TipoImagen = 1000;
                    MG.ShowDialog();
                    return;
                }

                if (textBox12.Text != "")
                {
                    bool email = repositorioIPacientes.ValidaEmail(textBox12.Text);
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

                if (textBox1.Text == "")
                {
                    MG.Mensaje = "No hay admision para procesar cierre este cuadro y vuelva a seleccionar una admision valida";
                    MG.TipoImagen = 1000;
                    MG.ShowDialog();
                    return;
                }
                if (label22.Text == "")
                {
                    MG.Mensaje = "Debe seleccionar un departamenteo de residencia del paciente";
                    MG.TipoImagen = 1000;
                    MG.ShowDialog();
                    return;
                }
                if (label24.Text == "")
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

                /*if (repositorioPacientes.ComprobarEdad(dateTimePicker1.Value, comboBox6.Text) == false)
                {
                    Comunes.MensajesGeneral M = new Comunes.MensajesGeneral();
                    M.Mensaje = "La fecha de nacimiento no coincide con el tipo de documento seleccionado, verifique los datos.  Para Mayores de 18 años no puede tener " +
                        "tipos de documentos de menores de edad y viceversa";
                    M.TipoImagen = 1000;
                    M.ShowDialog();
                    return;
                }*/

                int Canti = 0;
                string IniciaSesion = "N";

                if (checkBox1.Checked == true || checkBox2.Checked == true)
                {
                    if (textBox11.Text == "" || textBox16.Text == "")
                    {
                        MG.Mensaje = "Cuando selecciona la opcion de autorizacion se espera un numero de autorizacion y una cantidad de sesiones";
                        MG.TipoImagen = 1000;
                        MG.ShowDialog();
                        return;
                    }

                    Canti = Convert.ToInt32(textBox16.Text);
                    
                    if (Canti <= 0)
                    {
                        MG.Mensaje = "Cuando marca la opcion de autorizacion la cantidad no puede estar en blanco o ser menor o igual a 0";
                        MG.TipoImagen = 1000;
                        MG.ShowDialog();
                        return;
                    }            

                    IniciaSesion = "S";
                }

                //MODIFICAR ANTE CAMBIOS LEGALES
                if (checkBox3.Checked == true || checkBox4.Checked == true)
                {
                    if (comboBox1.Text == "CURACION COMPLEJIDAD ALTA")
                    {
                        Canti = 12;
                    }
                    else if (comboBox1.Text == "CURACION COMPLEJIDAD MEDIA")
                    {
                        Canti = 8;
                    }
                    else if (comboBox1.Text == "CURACION COMPLEJIDAD BAJA")
                    {
                        Canti = 6;
                    }
                    else if (comboBox1.Text == "CURACION DE LESION EN PIEL O TEJIDO CELULAR SUBCUTANEO SOD")
                    {
                        Canti = 1;
                    }
                }

                string Retardo = "";

                if (textBox9.Text != "")
                {
                    Retardo = Microsoft.VisualBasic.Interaction.InputBox(
                            "Retardo de Citas",
                            "Digite la razon del retardo del paciente",
                                "");
                }

                DateTime _fechas = new DateTime();

                if (Conexion.ConectionDictionary["Format_Fecha"] == "yyyy/MM/dd" || Conexion.ConectionDictionary["Format_Fecha"] == "yyyy-MM-dd")
                {
                    string D1 = repositorioFechasAgendaa.Extract(textBox2.Text, 4, 0);
                    string D2 = repositorioFechasAgendaa.Extract(textBox2.Text, 2, 5);
                    string D3 = repositorioFechasAgendaa.Extract(textBox2.Text, 2, 8);
                    _fechas = new DateTime(Convert.ToInt32(D1), Convert.ToInt32(D2), Convert.ToInt32(D3));
                }

                if (Conexion.ConectionDictionary["Format_Fecha"] == "dd/MM/yyyy" || Conexion.ConectionDictionary["Format_Fecha"] == "dd-MM-yyyy")
                {
                    string D1 = repositorioFechasAgendaa.Extract(textBox2.Text, 2, 0);
                    string D2 = repositorioFechasAgendaa.Extract(textBox2.Text, 2, 3);
                    string D3 = repositorioFechasAgendaa.Extract(textBox2.Text, 4, 6);
                    _fechas = new DateTime(Convert.ToInt32(D1), Convert.ToInt32(D2), Convert.ToInt32(D3));
                }

                string _consCitsDiaXPac = repositorioFechasAgendaa.consultarCitasMismoDia(Paciente, Bodega_Uno, Convert.ToDateTime(_fechas));                
                
                if (_consCitsDiaXPac != "")
                {
                    DialogResult result = MessageBox.Show("Este paciente tambien tiene otra cita el dia de hoy con " +
                           _consCitsDiaXPac + " ¿Desea admisionarlo realmente?",
                           "Admision de Pacientes",
                           MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {

                    }
                    if (result == DialogResult.No)
                    {
                        return;
                    }
                }

                string Tipo_Actualizacion = "";
                Actualiza_Pac(Tipo_Actualizacion);

                int getIdPac =  repositorioFechasAgendaa.getIdPacByAdmition(Admi, "A");                

                if (getIdPac != 0)
                {
                    CXN_CONVENIOS CUPServicio =  repositorioConvenios.ServicioCUP(comboBox1.Text, Convert.ToInt32(textBox6.Text));                    

                    DateTime Hora_Llega = DateTime.Now;
                    Hora_Llega = Convert.ToDateTime(Hora_Llega.ToString("HH:mm"));

                    CXN_HORARIO H = new CXN_HORARIO
                    {
                        Hor_Estado = "P",
                        Hor_Autoriza = textBox11.Text,
                        Hor_Valida = comboBox2.Text,
                        Hor_Pac_Ase = Convert.ToInt32(textBox6.Text),
                        Hor_Imp_Age = textBox20.Text + " " + textBox21.Text + " " + textBox18.Text + " " + textBox19.Text,
                        Hor_Regimen = repositorioPacientes.Regimen(comboBox3.Text),
                        Hor_Pac_Llegada = Convert.ToDateTime(Hora_Llega),
                        Hor_Usr_Admisiona = Comunes.Contenedor.UsuarioLogueado,
                        Hor_Pac_Minutos = textBox9.Text,
                        Hor_Pac_Razon = (Retardo == null ? "" : Retardo),
                        Hor_Pac_Cup = CUPServicio.Con_Id_Serv.ToString(),
                        Hor_RegAtn = textBox3.Text,  //registro de atencion
                        Hor_CantSesion = Canti,  //cantidad de sesiones
                        Hor_IniciaSesion = IniciaSesion, //Iniciar sesion
                        Hor_ValDerechos = textBox10.Text, //pines
                        Hor_Id = Admi,
                        Hor_Observacion = " ||| CITA ADMISIONADA POR " + Comunes.Contenedor.UsuarioLogueado,
                        Hor_Vales = checkBox6.Checked == true ? "S" : "N"
                    };

                    bool _updateCita =  repositorioHorario.updateCitaAdmisionar(H);                    

                    if (checkBox3.Checked == true) 
                    { 
                        repositorioHorario.PendientesChecked(Admi, "N");                                         
                    }

                    if (checkBox4.Checked == true) 
                    {
                        repositorioHorario.PendientesChecked(Admi, "A");    
                    }

                    if (checkBox2.Checked == true) 
                    {
                        repositorioHorario.InicioControlCuraciones(Admi, "A");                     
                    }

                    if (checkBox1.Checked == true) 
                    {
                        repositorioHorario.InicioControlCuraciones(Admi, "N");
                    }

                    int consultaMGmismodia = repositorioHorario.consularMGMismoDia(Paciente, Convert.ToDateTime(_fechas));                    

                    if (consultaMGmismodia > 0)
                    {
                        repositorioHorario.InicioControlCuraciones(Admi, "C");  
                    }

                    if (this.Pac_Sal == "Z")
                    {
                        repositorioHorario.InicioControlCuraciones(Admi, "Z");
                    }

                    Agendamiento f7 = Application.OpenForms.OfType<Agendamiento>().FirstOrDefault();
                    f7.EventoInicial();

                    //HOJA DE FIRMAS AUTOMATICA
                    /*if (checkBox1.Checked == true || checkBox2.Checked == true)
                    {
                        DialogResult resultprint = MessageBox.Show("¿Desea imprimir una hoja de firmas nueva?",
                        "Admision de Pacientes",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                        if (resultprint == DialogResult.Yes)
                        {
                            CXN_CIA DatNombre = repositorioCompañia.getPrestadorbyName(textBox7.Text);                            

                            modelo = new List<Modelo>();

                            string LogoCia = DatNombre.Com_Logo.ToString(); //trae base64
                            Byte[] bytes = Convert.FromBase64String(LogoCia); //convierte a bytes
                            MemoryStream stmBLOBData = new MemoryStream(bytes);
                            PictureBox pic = new PictureBox();
                            pic.Image = Image.FromStream(stmBLOBData);

                            if (this.FirmasAutoComplete == true)
                            {
                                if (!string.IsNullOrEmpty(textBox16.Text) && !string.IsNullOrEmpty(textBox11.Text))
                                {
                                    (int Cantidad, string Clase) getImpAuto = (0, "");

                                    
                                    getImpAuto = repositorioFechasAgendaa.GenerarImprentaAutomatica(Convert.ToInt32(textBox1.Text));
                                    

                                    if (getImpAuto.Cantidad != 0 && getImpAuto.Clase != "")
                                    {
                                        List<FirmasR> lista = new List<FirmasR>();

                                        FirmasR F = new FirmasR
                                        {
                                            Com_Nombre = DatNombre.Com_Nombre,
                                            Com_Direccion = DatNombre.Com_Direccion,
                                            Com_Telefono = DatNombre.Com_Telefono,
                                            Com_Logo = DatNombre.Com_Logo
                                        };

                                        
                                        lista = repositorioReportes.Firmas_Print(Convert.ToInt32(textBox1.Text), F, true);
                                                                                
                                        
                                        modelos = new List<FirmasR>();

                                        int Cantidad = getImpAuto.Cantidad;
                                        int BajaMedia = 1;
                                        int Alta = 2;

                                        if (getImpAuto.Clase == "A") { Cantidad = Cantidad + Alta; }
                                        if (getImpAuto.Clase == "B" || getImpAuto.Clase == "M" || getImpAuto.Clase == "C") { Cantidad = Cantidad + BajaMedia; }

                                        for (int i = 0; i <= Cantidad; i++)
                                        {
                                            foreach (FirmasR G in lista)
                                            {
                                                modelos.Add(new FirmasR
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

                                            FirmasR item = modelos[0];
                                            item.Com_UsuarioGraba = "CONSULTA";
                                            item = modelos[operDivDifAlta];
                                            item.Com_UsuarioGraba = "CONSULTA";
                                            item = modelos[Cantidad];
                                            item.Com_UsuarioGraba = "CONSULTA";
                                        }
                                        else
                                        {
                                            FirmasR item = modelos[0];
                                            item.Com_UsuarioGraba = "CONSULTA";
                                            item = modelos[Cantidad];
                                            item.Com_UsuarioGraba = "CONSULTA";

                                            if (getImpAuto.Cantidad == 1)
                                            {
                                                modelos.RemoveAt(modelos.Count - 1);
                                            }
                                        }

                                        if (modelos != null)
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
                                }
                                else
                                {
                                    modelo.Add(new Modelo
                                    {
                                        PacienteNombre = textBox20.Text + " " + textBox21.Text + " " + textBox18.Text + " " + textBox19.Text,
                                        PacienteIdentificacion = textBox8.Text,
                                        PacienteAseguradora = comboBox7.Text,
                                        PacienteTelefono = textBox11.Text, //autorizacion
                                        EmpresaNombre = textBox7.Text,
                                        EmpresaDireccion = DatNombre.Com_Direccion,
                                        EmpresaTelefono = DatNombre.Com_Telefono,
                                        Logo = repositorioGenerales.GetBytes(pic.Image), //Logo
                                        Com_Direccion = textBox16.Text
                                    });

                                    Thread thread = new Thread(M);
                                    thread.SetApartmentState(ApartmentState.STA); // Configura el subproceso en STA
                                    thread.Start();
                                }
                            } 
                            else
                            {
                                modelo.Add(new Modelo
                                {
                                    PacienteNombre = textBox20.Text + " " + textBox21.Text + " " + textBox18.Text + " " + textBox19.Text,
                                    PacienteIdentificacion = textBox8.Text,
                                    PacienteAseguradora = comboBox7.Text,
                                    PacienteTelefono = textBox11.Text, //autorizacion
                                    EmpresaNombre = textBox7.Text,
                                    EmpresaDireccion = DatNombre.Com_Direccion,
                                    EmpresaTelefono = DatNombre.Com_Telefono,
                                    Logo = repositorioGenerales.GetBytes(pic.Image), //Logo
                                    Com_Direccion = textBox16.Text
                                });

                                Thread thread = new Thread(M);
                                thread.SetApartmentState(ApartmentState.STA); // Configura el subproceso en STA
                                thread.Start();
                            }
                        }
                    }*/

                    if (checkBox3.Checked == true || checkBox4.Checked == true)
                    {
                        CXN_OPEND OP = new CXN_OPEND
                        {
                            OP_Adm = Admi,
                            OP_Estado = "P",
                            OP_Registra = Comunes.Contenedor.UsuarioLogueado
                        };
                        repositorioHorario.OPend(OP);                            
                    }

                    PopupNotifier Pop = PopUps.setPopUp(Properties.Resources2.comprobado,
                                                                Color.LightBlue,
                                                                "CITA ADMISIONADA",
                                                                Color.DarkBlue,
                                                                "Cita de " + H.Hor_Imp_Age + " admisionada exitosamente");

                    Pop.Popup();
                    
                    repositorioOrdenes.consumirAutorizacion(getIdPac, textBox11.Text, "S");                

                    if (checkBox6.Checked == true)
                    {
                        Extras.RcCaja RcCaja = new Extras.RcCaja(Admi);
                        RcCaja.ShowDialog();
                    }                    
                    else
                    {
                        if (Preferencias.TabletaFirmas == "A")
                        {
                            FirmaDigital firmaDigital = new FirmaDigital(Admi, getIdPac, "Admisiones");
                            firmaDigital.ShowDialog();
                        }
                    }                   

                    this.Dispose();
                    this.Close();
                }
                else
                {
                    MG.Mensaje = "No se pudo recuperar el estado de la admision";
                    MG.TipoImagen = 1000;
                    MG.ShowDialog();

                    this.Dispose();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        void M2()
        {
            try
            {
                ConfigForm.GenerarReportViewer("DataSet_Firmas",
                                               "ZamenisHealth.Reportes.Firmas2AutoComplete.rdlc",
                                               modelos);
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
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }        
        private void Admisiones_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyData == (Keys.Escape))
                {
                    Cerrar();
                }
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
                /*if (repositorioPacientes.ComprobarEdad(dateTimePicker1.Value, comboBox6.Text) == false)
                {
                    Comunes.MensajesGeneral M = new Comunes.MensajesGeneral();
                    M.Mensaje = "La fecha de nacimiento no coincide con el tipo de documento seleccionado, verifique los datos.  Para Mayores de 18 años no puede tener " +
                        "tipos de documentos de menores de edad y viceversa";
                    M.TipoImagen = 1000;
                    M.ShowDialog();
                    return;
                }*/

                string Tipo_Actualizacion = "Datos Actualizados del Paciente SIN ADMISIONAR!!!!!";
                ActualizaAdmision();
                Actualiza_Pac(Tipo_Actualizacion);
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void CargarRegimen()
        {
            try
            {
                List<string> CargaRegimen = new List<string>();
                
                CargaRegimen = repositorioPacientes.ListaRegimen();
                

                if (CargaRegimen != null)
                {
                    comboBox3.Items.Clear();

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
        private void CargarAseguradoras(int Ase, string TipoServ)
        {
            try
            {
                List<string> _listAse = new List<string>();

                
                _listAse = repositorioAseguradora.CargarAseguradorasXServ(TipoServ);
                

                if (_listAse != null)
                {
                    comboBox7.Items.Clear();

                    foreach (var i in _listAse)
                    {
                        comboBox7.Items.Add(i);
                    }
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked == true)
            {
                checkBox1.Checked = false;
                checkBox3.Checked = false;
                checkBox4.Checked = false;

                textBox11.Text = "";
                textBox16.Text = "";
            }
        }
        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox3.Checked == true)
            {
                checkBox2.Checked = false;
                checkBox1.Checked = false;
                checkBox4.Checked = false;
                textBox11.Text = "";
                textBox16.Text = "";
            }

            if (checkBox4.Checked == false && checkBox3.Checked == false)
            {
                textBox11.Text = "";
                textBox16.Text = "";
            }
        }
        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox4.Checked == true)
            {
                checkBox2.Checked = false;
                checkBox1.Checked = false;
                checkBox3.Checked = false;
                textBox11.Text = "";
                textBox16.Text = "";
            }

            if (checkBox4.Checked == false && checkBox3.Checked == false)
            {
                textBox11.Text = "";
                textBox16.Text = "";
            }
        }
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                checkBox2.Checked = false;
                checkBox3.Checked = false;
                checkBox4.Checked = false;

                textBox11.Text = "";
                textBox16.Text = "";
            }
        }
        private void label27_Click(object sender, EventArgs e)
        {
            ExpandeContraer(label27, panel5, 32, 143);
        }
        private void label28_Click(object sender, EventArgs e)
        {
            ExpandeContraer(label28, panel6, 35, 369);
        }
        private void label55_Click(object sender, EventArgs e)
        {
            ExpandeContraer(label55, panel7, 41, 186);
        }
        private void label57_Click(object sender, EventArgs e)
        {
            ExpandeContraer(label57, panel8, 34, 450);
        }
        private void label48_Click(object sender, EventArgs e)
        {
            ExpandeContraer(label48, panel1, 39, 321);
        }
        private void label52_Click(object sender, EventArgs e)
        {

        }
        private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {
                Admisiones A = new Admisiones(Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString()));

                this.Dispose();
                this.Close();
               
                A.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }          
        }
        private void pictureBox2_Click(object sender, EventArgs e)
        {
            Cerrar();
        }
        public void Actualiza_Pac(string Tipo)
        {
            try
            {
                Comunes.MensajesGeneral MG = new Comunes.MensajesGeneral();

                string regi = repositorioPacientes.Regimen(comboBox3.Text);

                string Sex = "";
                string Zona = "U";

                switch (comboBox9.SelectedIndex)
                {
                    case 0:
                        Sex = "M";
                        break;
                    case 1:
                        Sex = "F";
                        break;
                    case 2:
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
                    Pac_Dep_Cod = label22.Text,
                    Pac_Mun_Cod = label24.Text,
                    Pac_Aseguradora = Convert.ToInt32(textBox6.Text),
                    Pac_FechaNto = Convert.ToDateTime(dateTimePicker1.Value.Date),
                    Pac_Id = Paciente,
                    Pac_Zona = Zona,
                    Pac_TipoId = comboBox6.Text,
                    Pac_Contrato = textBox17.Text,
                    Pac_PaisOrigen = repositorioIPacientes.getCodePais(comboBox8.Text),
                    Pac_Residencia = repositorioIPacientes.getCodePais(comboBox10.Text),
                    Pac_ECivil = comboBox11.Text,
                    Pac_Acudiente = textBox26.Text,
                    Pac_Parentesco = textBox25.Text,
                    Pac_DireccionAcu = textBox24.Text,
                    Pac_TelefonoAcu = textBox23.Text,
                    Pac_CorreoAcu = textBox22.Text                   
                };
               

                switch (comboBox4.SelectedIndex)
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

                
                    repositorioPacientes.Actualiza_Pac(P);
                                

                if (Tipo != "")
                {
                    string Paci_Hori = textBox20.Text + " " + textBox21.Text + " " + textBox18.Text + " " + textBox19.Text;

                    //ACTUALIZAR EN TABLA HORARIO LA ASEGURADORA AQUI
                    
                    
                        repositorioHorario._updateAseHorario(Convert.ToInt32(textBox6.Text), Paci_Hori, Admi);
                    

                    MG.Mensaje = Tipo;
                    MG.TipoImagen = 3;
                    MG.ShowDialog();

                    this.Dispose();
                    this.Close();
                }

            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void ActualizaAdmision()
        {
            try
            {
                var CUPServicio = repositorioConvenios.ServicioCUP(comboBox1.Text,
                                                                   Convert.ToInt32(textBox6.Text));

                repositorioHorario.ActualizaAdmision(CUPServicio.Con_Id_Serv, Admi);
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
    }
}
