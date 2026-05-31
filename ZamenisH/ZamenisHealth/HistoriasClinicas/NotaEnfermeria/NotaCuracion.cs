using Domain;
using Domain.CXN;
using FormAndControls;
using FormAndControls.Controles;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZamenisHealth.Clases;
using ZamenisHealth.Comunes;
using ZamenisHealth.HistoriasClinicas.Extras;
using ZamenisHealth.Medicina.DocumentosWEB;
using Color = System.Drawing.Color;

namespace ZamenisHealth.HistoriasClinicas.NotaEnfermeria
{
    public partial class NotaCuracion : Forma
    {
        private readonly IAgenda repoAgendaMedica;
        private readonly IAgendaC repoAgendaMedicaConsultas;
        private readonly IConvenios repoConvenios;
        private readonly IAseguradoras repoAseguradoras;
        private readonly ICIE10 repoCIE10;
        private readonly INotasCuracion repoNotasCuracion;
        private readonly IPacientes repoPacs;
        private readonly IConfSystem repoConfSystem;
        private readonly ICondiciones repoCond;
        private readonly IEncuestasSatis repoEncuestasSatis;
        private readonly IRIPS repoRIPS;
        private readonly ICargos repoCargos;
        private readonly IMedicinaGeneral repoMedicinaGeneral;
        private readonly ICambiosSolicitados repoCambiosSolicitados;

        private int Admision, Paciente, Cia, Ase, Prof, Valor;
        private string arrastra, CUP, TSERV, Reg_RIP, CMANID, CMANTID, PACSAL, Serv;
        private DateTime Fecha_Serv;
        private bool Mayus;

        private MensajesGeneral MG;
        private CondicionesP c;

        private DataTable dt;
        private DataColumn POS;
        private DataColumn Id;
        private DataColumn Largo;
        private DataColumn Ancho;
        private DataColumn Profundidad;
        private DataColumn Total;
        private DataColumn Localizacion;

        public NotaCuracion(int admision)
        {
            InitializeComponent();
            Admision = admision;
            repoAgendaMedica = new MAgenda();
            repoAgendaMedicaConsultas = new MAgendaC();
            repoConvenios = new MConvenios();
            repoAseguradoras = new MAseguradoras();
            repoCIE10 = new MCIE10();
            repoNotasCuracion = new MNotasCuracion();
            repoPacs = new MPacientes();
            repoConfSystem = new MConfSystem();
            repoCond = new MCondiciones();
            repoEncuestasSatis = new MEncuestasSatis();
            repoRIPS = new MRIPS();
            repoCargos = new MCargos();
            repoMedicinaGeneral = new MMedicinaGeneral();
            repoCambiosSolicitados = new MCambiosSolicitados();
        }

        private void NotaCuracion_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Notas de Curacion";
            LogoMain.Image = Properties.Resources.Splash;
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";
            ImageClose.Visible = false;
            ImageMinimize.Visible = false;

            setButtons();

            try
            {
                var DatosAdmision = repoAgendaMedicaConsultas.cargarAdmision(Admision, "'P','H','A'");
                if (DatosAdmision == null)
                {
                    MG = new MensajesGeneral();
                    MG.Mensaje = "Error al cargar la admision";
                    MG.TipoImagen = 1000;
                    MG.ShowDialog();

                    this.Dispose();
                    this.Close();                    
                }
                else
                {
                    arrastra = DatosAdmision.Hor_ArrastraHistoria;
                    Paciente = DatosAdmision.Hor_Pac_Id;
                    Cia = DatosAdmision.Hor_Pac_Cia;
                    Ase = DatosAdmision.Hor_Pac_Ase;
                    Prof = DatosAdmision.Hor_Pac_Bod;
                    CUP = DatosAdmision.Hor_Pac_Cup;
                    TSERV = DatosAdmision.Hor_Pac_Tipo_Serv;
                    Fecha_Serv = Convert.ToDateTime(DatosAdmision.Hor_Pac_Fecha_Cita);
                    Reg_RIP = DatosAdmision.Hor_Regimen;
                    CMANID = DatosAdmision.Pac_IdNum.ToString();
                    CMANTID = DatosAdmision.Pac_TipoId.ToString();
                    textBox23.Text = DatosAdmision.Hor_Observacion;
                    PACSAL = DatosAdmision.Hor_Pac_Sal;
                    comboBox1.Text = DatosAdmision.VIH;
                    comboBox2.Text = DatosAdmision.Hepatitis;

                    CXN_CONVENIOS VAl = repoConvenios.ServicioNombre(CUP, Ase, TSERV);
                    if (VAl == null)
                    {
                        MG = new MensajesGeneral();
                        MG.Mensaje = "Error grave cargardo valor del servicio, vuelva a ingresar a la admision";
                        MG.TipoImagen = 1000;
                        MG.ShowDialog();

                        this.Dispose();
                        this.Close();
                    }
                    else
                    {
                        repoAgendaMedica.Graba_Hora_Atencion(Admision);
                        repoAgendaMedica.OpenAdmition(Admision, "S");

                        CXN_ASEGURADORA As = repoAseguradoras.getInfoFromAsebyCode(Ase);
                        textBox17.Text = As.Ase_Descripcion.ToString();

                        Valor = VAl.Con_Valor;
                        Serv = VAl.Con_Nombre;

                        textBox20.Text = Serv.ToString();
                        textBox22.Text = CUP.ToString();
                        textBox1.Text = Admision.ToString();
                        textBox2.Text = DatosAdmision.Hor_Imp_Age.ToString();
                        textBox3.Text = Convert.ToDateTime(DatosAdmision.Pac_FechaNto).ToString(Conexion.ConectionDictionary["Format_Fecha"]);
                        textBox5.Text = DatosAdmision.Pac_TipoId.ToString() + " " + DatosAdmision.Pac_IdNum.ToString();

                        DateTime nacimiento = Convert.ToDateTime(DatosAdmision.Pac_FechaNto);
                        int edad = DateTime.Today.AddTicks(-nacimiento.Ticks).Year - 1;
                        textBox4.Text = edad.ToString();

                        Diagnosticos(DatosAdmision.Hor_Pac_Id);
                        var DX = repoCIE10.BuscaDX(textBox6.Text);
                        textBox7.Text = DX.ToString();
                        DX = repoCIE10.BuscaDX(textBox8.Text);
                        textBox10.Text = DX.ToString();
                        DX = repoCIE10.BuscaDX(textBox9.Text);
                        textBox11.Text = DX.ToString();

                        if (DatosAdmision.Hor_ArrastraHistoria == "S")
                        {
                            MG = new MensajesGeneral();
                            MG.Mensaje = "Este paciente es un control de mitad de paquete de Alta Complejidad";
                            MG.TipoImagen = 1000;
                            MG.ShowDialog();
                        }

                        CXN_PACIENTES DatoPac = repoPacs.LlamarPacientebyId(Paciente);
                        if (DatoPac.Pac_Doble == "S")
                        {
                            checkBox1.Checked = true;
                        }

                        if (DatoPac.Pac_2VXS == "S")
                        {
                            checkBox2.Checked = true;
                        }

                        if (DatoPac.Pac_Especial == "S")
                        {
                            checkBox4.Checked = true;
                            MensajesGeneral mg = new MensajesGeneral();
                            mg.TipoImagen = 0;
                            mg.Mensaje = "ATENCION!!! \n\r \n\r ESTE PACIENTE TIENE CONDICIONES Y TRATOS ESPECIALES, PROBABLEMENTE REQUIERA ACOMPAÑANTE EN CONSULTA";
                            mg.ShowDialog();
                        }

                        Dictionary<string, string> getConfig = repoConfSystem.getListado();

                        if (PACSAL == "A")
                        {
                            if (getConfig != null)
                            {
                                _ = getConfig["AutocompletarNotas"] == "A" ? checkBox3.Visible = true : checkBox3.Visible = false;
                            }
                        }
                        else
                        {
                            checkBox3.Visible = false;
                        }

                        Mayus = (repoConfSystem.getListado()["UpperCaseNotas"] == "A" ? true : false);

                        if (Mayus == true)
                        {
                            textBox14.CharacterCasing = CharacterCasing.Upper;
                            textBox15.CharacterCasing = CharacterCasing.Upper;
                            textBox16.CharacterCasing = CharacterCasing.Upper;
                        }
                        else
                        {
                            textBox14.CharacterCasing = CharacterCasing.Normal;
                            textBox15.CharacterCasing = CharacterCasing.Normal;
                            textBox16.CharacterCasing = CharacterCasing.Normal;
                        }

                        string getRecos = repoConfSystem.getListado()["Recomendaciones"];
                        if (getRecos == "A")
                        {
                            Extras.Recomendaciones r = new Extras.Recomendaciones(DatosAdmision.Hor_Pac_Id);
                            r.ShowDialog();

                            CargarOpcionesRecomendaciones();
                        }

                        string colorimetria = Preferencias.Colorimetria;
                        if (colorimetria == "A")
                        {
                            bool dataForSal = repoAgendaMedicaConsultas.ConsultarNavyEnfermeria(Paciente, Prof, Fecha_Serv);

                            if (dataForSal == true)
                            {
                                MG = new MensajesGeneral();
                                MG.TipoImagen = 3;
                                MG.Mensaje = "En esta cita el paciente tiene un control intermedio con Medicina General, por favor llame al medico";
                                MG.ShowDialog();
                            }
                        }

                        if (getConfig["Cargos"] == "A")
                        {
                            button2.Visible = true;
                        }

                        CargarServiciosCambiosComplejidad();
                        CargarServicioMedico();
                        CargarCantidades();
                        ActualizarPAC();
                        randomEncuestaSatisfaccion();
                        CargarMedidasEstaNota();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar la nota de curación: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        void setButtons()
        {
            ToolStripButton toolStripButton10 = new ToolStripButton();
            toolStripButton10 = createToolButton("Datos Pacientes");
            MenuLateral.Items.Add(toolStripButton10);
            toolStripButton10.Click += toolStripButton10_Click;

            ToolStripButton toolStripButton11 = new ToolStripButton();
            toolStripButton11 = createToolButton("Adherencia");
            MenuLateral.Items.Add(toolStripButton11);
            toolStripButton11.Click += toolStripButton11_Click;

            ToolStripButton toolStripButton9 = new ToolStripButton();
            toolStripButton9 = createToolButton("Documentos WEB");
            MenuLateral.Items.Add(toolStripButton9);
            toolStripButton9.Click += toolStripButton9_Click;

            ToolStripButton toolStripButton1 = new ToolStripButton();
            toolStripButton1 = createToolButton("Grabar");
            MenuLateral.Items.Add(toolStripButton1);
            toolStripButton1.Click += toolStripButton1_Click;

            ToolStripButton toolStripButton2 = new ToolStripButton();
            toolStripButton2 = createToolButton("Historia Clinica");
            MenuLateral.Items.Add(toolStripButton2);
            toolStripButton2.Click += toolStripButton2_Click;

            if (repoConfSystem.getListado()["IHCE"] == "A")
            {
                ToolStripButton toolStripButton3 = new ToolStripButton();
                toolStripButton3 = createToolButton("Visor IHCE");
                MenuLateral.Items.Add(toolStripButton3);
                toolStripButton3.Click += toolStripButton3_Click;
            }            

            ToolStripButton toolStripButton8 = new ToolStripButton();
            toolStripButton8 = createToolButton("Resumen Historia");
            MenuLateral.Items.Add(toolStripButton8);
            toolStripButton8.Click += toolStripButton8_Click;

            ToolStripButton toolStripButton6 = new ToolStripButton();
            toolStripButton6 = createToolButton("Cambios Manejo");
            MenuLateral.Items.Add(toolStripButton6);
            toolStripButton6.Click += toolStripButton6_Click;

            ToolStripButton toolStripButton5 = new ToolStripButton();
            toolStripButton5 = createToolButton("Traer Ultima");
            MenuLateral.Items.Add(toolStripButton5);
            toolStripButton5.Click += toolStripButton5_Click;

            ToolStripButton toolStripButton4 = new ToolStripButton();
            toolStripButton4 = createToolButton("Cargar Plantilla");
            MenuLateral.Items.Add(toolStripButton4);
            toolStripButton4.Click += toolStripButton4_Click;

            ToolStripButton toolStripButton7 = new ToolStripButton();
            toolStripButton7 = createToolButton("Tratamiento");
            MenuLateral.Items.Add(toolStripButton7);
            toolStripButton7.Click += toolStripButton7_Click;

            ToolStripButton toolStripButton12 = new ToolStripButton();
            toolStripButton12 = createToolButton("Mensajero");
            MenuLateral.Items.Add(toolStripButton12);
            toolStripButton12.Click += toolStripButton12_Click;

            ToolStripButton pictureBox1 = new ToolStripButton();
            pictureBox1 = createToolButton("Cancelar");
            MenuLateral.Items.Add(pictureBox1);
            pictureBox1.Click += pictureBox1_Click;
        }
        void CargarServiciosCambiosComplejidad()
        {
            try
            {
                comboBox3.Items.Add("NO APLICA");

                List<CXN_CONVENIOS> lista = repoConvenios.getConvenios(this.Ase);
                if (lista != null)
                {
                    foreach (CXN_CONVENIOS i in lista)
                    {
                        comboBox3.Items.Add(i.Con_Nombre.ToString());
                    }
                }

                comboBox3.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los servicios de cambios de complejidad: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void Diagnosticos(int Paciente)
        {
            try
            {
                var getDX = repoNotasCuracion.Diagnosticos(Paciente);
                if (getDX != null)
                {
                    textBox6.Text = getDX["DX1"];
                    textBox8.Text = getDX["DX2"];
                    textBox9.Text = getDX["DX3"];
                    textBox12.Text = getDX["Impresion"];
                    textBox13.Text = getDX["Patologia"];
                }
                else
                {
                    textBox6.Text = "";
                    textBox8.Text = "";
                    textBox9.Text = "";
                    textBox12.Text = "";
                    textBox13.Text = "N/A";
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult result = MessageBox.Show("¿Desea salir de esta nota de curacion? si sale de esta nota ningun dato " +
                                                      "quedara grabado",
                                                      "Zamenis Health - Cancelar Curacion!!!!",
                                                 MessageBoxButtons.YesNo,
                                                 MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    repoAgendaMedica.OpenAdmition(Admision, "N");
                    this.Dispose();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        void CargarOpcionesRecomendaciones()
        {
            try
            {
                List<string> getConditions = repoCond.getCondiciones(this.Paciente);
                if (getConditions != null)
                {
                    string resultado = getConditions.Find(x => x == "CAIDA");
                    if (resultado != null)
                    {
                        CaidaTxt.BackColor = Color.DodgerBlue;
                        CaidaTxt.ForeColor = Color.Navy;
                    }
                    else
                    {
                        CaidaTxt.BackColor = Color.White;
                        CaidaTxt.ForeColor = Color.Black;
                    }

                    resultado = getConditions.Find(x => x == "INFECCION");
                    if (resultado != null)
                    {
                        infeccionTxt.BackColor = Color.LightGreen;
                        infeccionTxt.ForeColor = Color.Green;
                    }
                    else
                    {
                        infeccionTxt.BackColor = Color.White;
                        infeccionTxt.ForeColor = Color.Black;
                    }

                    resultado = getConditions.Find(x => x == "DETERIORO DE LA PIEL");
                    if (resultado != null)
                    {
                        deterioroTxt.BackColor = Color.Violet;
                        deterioroTxt.ForeColor = Color.DarkViolet;
                    }
                    else
                    {
                        deterioroTxt.BackColor = Color.White;
                        deterioroTxt.ForeColor = Color.Black;
                    }

                    resultado = getConditions.Find(x => x == "ALERGIA");
                    if (resultado != null)
                    {
                        alergiaTxt.BackColor = Color.Red;
                        alergiaTxt.ForeColor = Color.White;
                    }
                    else
                    {
                        alergiaTxt.BackColor = Color.White;
                        alergiaTxt.ForeColor = Color.Black;
                    }

                    resultado = getConditions.Find(x => x == "DIFICULTAD DE COMUNICACION");
                    if (resultado != null)
                    {
                        dificultadTxt.BackColor = Color.Yellow;
                        dificultadTxt.ForeColor = Color.Black;
                    }
                    else
                    {
                        dificultadTxt.BackColor = Color.White;
                        dificultadTxt.ForeColor = Color.Black;
                    }

                    resultado = getConditions.Find(x => x == "PACIENTE PSIQUIATRICO");
                    if (resultado != null)
                    {
                        psiquiatricoTxt.BackColor = Color.DarkViolet;
                        psiquiatricoTxt.ForeColor = Color.Thistle;
                    }
                    else
                    {
                        psiquiatricoTxt.BackColor = Color.White;
                        psiquiatricoTxt.ForeColor = Color.Black;
                    }

                    resultado = getConditions.Find(x => x == "MAYOR DE 70 AÑOS");
                    if (resultado != null)
                    {
                        mayorTxt.BackColor = Color.DarkTurquoise;
                        mayorTxt.ForeColor = Color.Blue;
                    }
                    else
                    {
                        mayorTxt.BackColor = Color.White;
                        mayorTxt.ForeColor = Color.Black;
                    }

                    resultado = getConditions.Find(x => x == "PACIENTE DIFICIL");
                    if (resultado != null)
                    {
                        dificilTxt.BackColor = Color.Orange;
                        dificilTxt.ForeColor = Color.AntiqueWhite;
                    }
                    else
                    {
                        dificilTxt.BackColor = Color.White;
                        dificilTxt.ForeColor = Color.Black;
                    }

                    resultado = getConditions.Find(x => x == "MEDICO LO REQUIERE");
                    if (resultado != null)
                    {
                        requiereTxt.BackColor = Color.Gray;
                        requiereTxt.ForeColor = Color.White;
                    }
                    else
                    {
                        requiereTxt.BackColor = Color.White;
                        requiereTxt.ForeColor = Color.Black;
                    }
                }
                else
                {
                    CaidaTxt.BackColor = Color.White;
                    CaidaTxt.ForeColor = Color.Black;
                    infeccionTxt.BackColor = Color.White;
                    infeccionTxt.ForeColor = Color.Black;
                    alergiaTxt.BackColor = Color.White;
                    alergiaTxt.ForeColor = Color.Black;
                    deterioroTxt.BackColor = Color.White;
                    deterioroTxt.ForeColor = Color.Black;
                    psiquiatricoTxt.BackColor = Color.White;
                    psiquiatricoTxt.ForeColor = Color.Black;
                    dificultadTxt.BackColor = Color.White;
                    dificultadTxt.ForeColor = Color.Black;
                    dificilTxt.BackColor = Color.White;
                    dificilTxt.ForeColor = Color.Black;
                    mayorTxt.BackColor = Color.White;
                    mayorTxt.ForeColor = Color.Black;
                    requiereTxt.BackColor = Color.White;
                    requiereTxt.ForeColor = Color.Black;
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

        #region Condiciones
        private void CaidaTxt_Click(object sender, EventArgs e)
        {
            openConditions("CAIDA");
        }
        private void infeccionTxt_Click(object sender, EventArgs e)
        {
            openConditions("INFECCION");
        }
        private void deterioroTxt_Click(object sender, EventArgs e)
        {
            openConditions("DETERIORO DE LA PIEL");
        }
        private void alergiaTxt_Click(object sender, EventArgs e)
        {
            openConditions("ALERGIA");
        }
        private void dificultadTxt_Click(object sender, EventArgs e)
        {
            openConditions("DIFICULTAD DE COMUNICACION");
        }
        private void psiquiatricoTxt_Click(object sender, EventArgs e)
        {
            openConditions("PACIENTE PSIQUIATRICO");
        }
        private void mayorTxt_Click(object sender, EventArgs e)
        {
            openConditions("MAYOR DE 70 AÑOS");
        }
        private void dificilTxt_Click(object sender, EventArgs e)
        {
            openConditions("PACIENTE DIFICIL");
        }
        private void requiereTxt_Click(object sender, EventArgs e)
        {
            openConditions("MEDICO LO REQUIERE");
        }
        #endregion Condiciones

        private void toolStripButton10_Click(object sender, EventArgs e)
        {
            ActualizarPAC();
        }
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            Medicina.Historial_Medico_1 RM = new Medicina.Historial_Medico_1();
            RM.ShowDialog();
        }
        private void toolStripButton8_Click(object sender, EventArgs e)
        {
            try
            {
                Task.Run(() =>
                {
                    this.Invoke((Action)(() =>
                    {
                        Medicina.ResumenHCMGNotas historia_MG_Resumen = new Medicina.ResumenHCMGNotas();
                        historia_MG_Resumen.Paci_Resumen_His = Paciente;
                        historia_MG_Resumen.Show();
                    }));
                });
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            Medicina.CambioManejoEnfermero CME = new Medicina.CambioManejoEnfermero();
            CME.CargarDocumentos();
            CME.comboBox1.Text = CMANTID;
            CME.textBox1.Text = CMANID;
            CME.comboBox1.Enabled = false;
            CME.textBox1.Enabled = false;
            CME.ShowDialog();
        }
        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            try
            {
                var getLasnota = repoNotasCuracion.getLastNota(Paciente);
                if (getLasnota != null)
                {
                    textBox14.Text = getLasnota.Not_Nota.ToString();
                    textBox16.Text = getLasnota.Not_Recomienda.ToString();
                    //textBox15.Text = getLasnota.Not_Observa.ToString();
                }
                else
                {
                    MessageBox.Show("Este paciente no cuenta con curaciones previas, " +
                          "por lo tanto no es posible traer informacion, pero puede usar una de sus plantillas " +
                          "si las tiene definidas",
                          "Paciente al parecer nuevo",
                          MessageBoxButtons.OK,
                          MessageBoxIcon.Exclamation);
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void CargarServicioMedico()
        {
            try
            {
                var getInfo = repoAgendaMedica.SugerenciaServicio(Paciente);
                if (getInfo != null)
                {
                    textBox22.Text = getInfo["Cup"];
                    textBox20.Text = getInfo["Servicio"];
                }
                else
                {
                    textBox22.Text = "";
                    textBox20.Text = "";
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void toolStripButton7_Click(object sender, EventArgs e)
        {
            Lista_LLenar();
        }
        private void textBox6_Click(object sender, EventArgs e)
        {
            Medicina.CIE10 CIE10_Historias = new Medicina.CIE10("DX1_NotaCore");
            CIE10_Historias.ShowDialog();
        }
        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            Medicina.PlantillasEnfermeria Historia_Notas_Plantilla = new Medicina.PlantillasEnfermeria();
            Historia_Notas_Plantilla.Tipo_Plant = "NotasCore";
            Historia_Notas_Plantilla.textBox1.Enabled = false;
            Historia_Notas_Plantilla.textBox2.Enabled = false;
            Historia_Notas_Plantilla.textBox3.Enabled = false;
            Historia_Notas_Plantilla.btnGrabar.Enabled = false;
            Historia_Notas_Plantilla.dataGridView1.Enabled = true;
            Historia_Notas_Plantilla.ShowDialog();
        }
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            try
            {
                if (textBox14.Text == "")
                {
                    MG = new MensajesGeneral();
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "Debe diligenciar una nota";
                    MG.ShowDialog();
                    return;
                }
                if (textBox15.Text == "")
                {
                    MG = new MensajesGeneral();
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "Debe diligenciar una observacion";
                    MG.ShowDialog();
                    return;
                }
                if (textBox16.Text == "")
                {
                    MG = new MensajesGeneral();
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "Debe diligenciar una recomendacion";
                    MG.ShowDialog();
                    return;
                }
                if (textBox18.Text == "")
                {
                    MG = new MensajesGeneral();
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "Debe diligenciar una observacion de control epidemiologico";
                    MG.ShowDialog();
                    return;
                }

                if (richTextBox1.Text == "")
                {
                    MG = new MensajesGeneral();
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "Debe seleccionar los apositos usados en la curacion";
                    MG.ShowDialog();
                    return;
                }
                if (textBox6.Text == "")
                {
                    MG = new MensajesGeneral();
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "Debe diligenciar el Diagnostico Principal DX1, haga clic sobre el recuadro del codigo de diagnostico para seleccionar uno";
                    MG.ShowDialog();
                    return;
                }

                if (checkBox3.Visible == true && checkBox3.Checked == false)
                {
                    MG = new Comunes.MensajesGeneral();
                    MG.Mensaje = "Esta admision esta marcada como inicio de paquete, por lo tanto debe marcar la opcion de color rojo de traer historias clinicas";
                    MG.TipoImagen = 1000;
                    MG.ShowDialog();
                    return;
                }

                if (checkBox5.Checked == false)
                {
                    if (string.IsNullOrEmpty(textBox19.Text) || string.IsNullOrEmpty(textBox24.Text))
                    {
                        MG = new Comunes.MensajesGeneral();
                        MG.Mensaje = "Debe diligenciar los campos del acompañante y el telefono, en caso de no tener acompañante seleccione el check --Mismo Paciente--";
                        MG.TipoImagen = 1000;
                        MG.ShowDialog();
                        return;
                    }
                }

                if (textBox15.Text.Contains("-- AGREGAR DESCRIPCION AQUI --"))
                {
                    MG = new Comunes.MensajesGeneral();
                    MG.Mensaje = "Verifique el campo de observaciones ya que esta incorrecto";
                    MG.TipoImagen = 1000;
                    MG.ShowDialog();
                    return;
                }

                if (comboBox1.Text == "" || comboBox2.Text == "")
                {
                    MG = new Comunes.MensajesGeneral();
                    MG.Mensaje = "Debe seleccionar VIH y Hepatitis";
                    MG.TipoImagen = 1000;
                    MG.ShowDialog();
                    return;
                }

                if (comboBox3.Text != "NO APLICA" && string.IsNullOrEmpty(textBox25.Text))
                {
                    MG = new Comunes.MensajesGeneral();
                    MG.Mensaje = "Si marca un cambio de complejidad debe escribir el motivo por el cual lo cambia";
                    MG.TipoImagen = 1000;
                    MG.ShowDialog();
                    return;
                }

                DialogResult result = MessageBox.Show("Una vez guardada esta nota, no se podran deshacer cambios. ¿Realmente desea guardar?",
                                                 "Zamenis Health - Enfermeria",
                                                 MessageBoxButtons.YesNo,
                                                 MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    var impresion = repoRIPS.TipoRipCargo(textBox12.Text, "IMPDX");

                    CXN_CARGOS C = new CXN_CARGOS
                    {
                        Car_Adm_Id = Admision,
                        Car_Pac = Paciente,
                        Car_Cia = Cia,
                        Car_Ase = Ase,
                        Car_Prof = Prof,
                        Car_Fecha = Fecha_Serv,
                        Car_Estado = "G",
                        Car_Tipo = "Nota",
                        Car_Cod = CUP,
                        Car_Tipo_Serv = TSERV,
                        Car_Cant = 1,
                        Car_Val_Un = Valor,
                        Car_Val_Tot = Valor,
                        Car_Item = Serv,
                        Car_Detalle = textBox13.Text,
                        Car_Dx1 = textBox6.Text,
                        Car_Dx2 = textBox8.Text,
                        Car_Dx3 = textBox9.Text,
                        Car_Regimen = Reg_RIP,
                        Car_Ambito = 0,
                        Car_Finalidad = 0,
                        Car_Personal = 0,
                        Car_CExterna = 0,
                        Car_Finalidad_CO = 0,
                        Car_Imp_Dx = impresion
                    };

                    bool inserCargo = repoCargos.InsertarCargoHistorias(C);
                    if (inserCargo != true)
                    {
                        MessageBox.Show("No se logro guardar el cargo economico en el registro de valores a cobrar en la factura",
                           "No se puede continuar",
                           MessageBoxButtons.OK,
                           MessageBoxIcon.Exclamation);
                        return;
                    }
                    else
                    {
                        CXN_NOTAS N = new CXN_NOTAS
                        {
                            Not_Pac = Paciente,
                            Not_Med = Prof,
                            Not_Ase = Ase,
                            Not_Fecha = Fecha_Serv,
                            Not_Cia = Cia,
                            Not_Edad = textBox4.Text,
                            Not_Nota = textBox14.Text,
                            Not_Recomienda = textBox16.Text,
                            Not_Observa = textBox15.Text,
                            Not_Cup = CUP,
                            Not_Cant = 1,
                            Not_Patologia = textBox13.Text,
                            Not_Adm = Admision,
                            Not_Epidemia = textBox18.Text,
                            Not_CaracTej = "",
                            Not_Adherencia = richTextBox1.Text,
                            Not_Acompañante = (checkBox5.Checked == true ? textBox2.Text : textBox19.Text),
                            Not_Telefono = (checkBox5.Checked == true ? "N/A" : textBox24.Text)
                        };

                        bool insertNota = repoNotasCuracion.GrabarNota(N);
                        if (inserCargo != true)
                        {
                            MessageBox.Show("No se logro guardar la nota de enfermeria, revise la informacion y vuelva a intentar, " +
                                          "si sigue presentando este inconveniente contacte al administrador del sistema",
                                          "No se puede continuar",
                                          MessageBoxButtons.OK,
                                          MessageBoxIcon.Exclamation);
                            return;
                        }
                        else
                        {                                                     
                            repoAgendaMedica.ConsumirAdmision(Admision);

                            if (comboBox3.Text != "NO APLICA")
                            {
                                CXN_CAMBIOSSOLICITADOS cambio = new CXN_CAMBIOSSOLICITADOS
                                {
                                    CupComplejidad = repoConvenios.DatosServicioXNameAse(this.Ase, comboBox3.Text).Con_Id_Serv,
                                    ServComplejidad = comboBox3.Text,
                                    Solicitante = Contenedor.UsuarioLogueado,
                                    Fecha = DateTime.Now,
                                    Estado = "P",
                                    Motivo = textBox25.Text.ToUpper().Trim(),
                                    AdmisionOfertante = Admision
                                };

                                repoCambiosSolicitados.InsertSolicitud(cambio);
                            }                           

                            MessageBox.Show("Grabado exitosamente",
                                            "Hecho",
                                             MessageBoxButtons.OK,
                                             MessageBoxIcon.Information);

                            bool datoEstadis = repoConfSystem.getListado()["EstadisticaEnfermeria"] == "A" ? true : false;
                            if (datoEstadis == true)
                            {
                                Medicina.Estadistica estadistica = new Medicina.Estadistica();
                                estadistica.Admition = Admision;
                                estadistica.ShowDialog();
                            }

                            Medicina.RIPSHistory for_RIPS = new Medicina.RIPSHistory();
                            for_RIPS.Adm_Cargo = Convert.ToInt32(Admision);
                            for_RIPS.ShowDialog();

                            if (checkBox1.Checked == true)
                            {
                                repoPacs.VariasHeridas(Paciente, "S");
                            }
                            if (checkBox1.Checked == false)
                            {
                                repoPacs.VariasHeridas(Paciente, "N");
                            }
                            if (checkBox2.Checked == true)
                            {
                                repoPacs.VariosDias(Paciente, "S");
                            }
                            if (checkBox2.Checked == false)
                            {
                                repoPacs.VariosDias(Paciente, "N");
                            }
                            if (checkBox4.Checked == true)
                            {
                                repoPacs.PacEspecial(Paciente, "S");
                            }
                            if (checkBox4.Checked == false)
                            {
                                repoPacs.PacEspecial(Paciente, "N");
                            }

                            if (checkBox3.Visible == true && checkBox3.Checked == true)
                            {
                                InsertarHistoria();
                            }
                            else
                            {
                                if (this.arrastra == "S")
                                {
                                    InsertarHistoria();
                                }
                            }

                            if (listaNotasMed != null)
                            {
                                foreach (CXN_NOTASMED item in listaNotasMed)
                                {
                                    item.Admision = Admision;

                                    if (!string.IsNullOrEmpty(item.Evolucion) && !string.IsNullOrEmpty(item.txtLocalizacion) && !string.IsNullOrEmpty(item.selExudado))
                                    {
                                        item.Total = (decimal)item.Largo * (decimal)item.Ancho * (decimal)item.Profundidad;
                                        repoNotasCuracion.insertarHerida(item);
                                    }                                    
                                }
                            }                            

                            repoAgendaMedica.Graba_Hora_Salida(Admision);
                            Medicina.AgendaM f2 = Application.OpenForms.OfType<Medicina.AgendaM>().LastOrDefault();
                            f2.Cargar_Agenda();

                            repoPacs.setEnfermedades(Paciente, comboBox1.Text == "Positivo" ? "S" : "N", comboBox2.Text);

                            this.Dispose();
                            this.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void CargarCantidades()
        {
            try
            {
                textBox21.Text = repoAgendaMedicaConsultas.Calcular3(Paciente, TSERV);
            }
            catch
            {
                textBox21.Text = "Fallo en el calculo de sesiones, no hay registros de autorizaciones ingresados";
            }
        }
        private void button4_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = "No se usaron apositos en esta curacion";
            richTextBox1.Text = richTextBox1.Text;
        }
        private void button3_Click(object sender, EventArgs e)
        {
            textBox18.Text = "";
        }
        private void button1_Click(object sender, EventArgs e)
        {
            textBox8.Text = ""; textBox10.Text = "";
        }
        private void button5_Click(object sender, EventArgs e)
        {
            textBox9.Text = ""; textBox11.Text = "";
        }
        private void textBox8_Click(object sender, EventArgs e)
        {
            Medicina.CIE10 CIE10_Historias = new Medicina.CIE10("DX2_NotaCore");
            CIE10_Historias.ShowDialog();
        }
        private void textBox9_Click(object sender, EventArgs e)
        {
            Medicina.CIE10 CIE10_Historias = new Medicina.CIE10("DX3_NotaCore");
            CIE10_Historias.ShowDialog();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            Extras.CargosCuraciones cargosCuraciones = new CargosCuraciones(Admision);
            cargosCuraciones.ShowDialog();
        }
        private void toolStripButton9_Click(object sender, EventArgs e)
        {
            MenuDocWeb M = new MenuDocWeb();
            M.ShowDialog();
        }
        private void toolStripButton11_Click(object sender, EventArgs e)
        {
            Medicina.Adherencia historia_Notas_Adherencia = new Medicina.Adherencia("NotasCore");
            historia_Notas_Adherencia.ShowDialog();
            richTextBox1.Text = richTextBox1.Text;
        }
        private void toolStripButton12_Click(object sender, EventArgs e)
        {
            Comunes.MensajeroSend S = new Comunes.MensajeroSend();
            S.ShowDialog();
        }
        private void button7_Click(object sender, EventArgs e)
        {
            MedidasPrevias medidasPrevias = new MedidasPrevias(Paciente, Admision);
            medidasPrevias.ShowDialog();
        }
        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox3.Text != "NO APLICA")
            {
                label36.Visible = true;
                textBox25.Visible = true;
            }
            else
            {
                label36.Visible = false;
                textBox25.Visible = false;
            }
        }

        #region MEDIDAS
        int NumeroTextBox = 0;
        List<TextBox> LargoList = new List<TextBox>();
        List<TextBox> AnchoList = new List<TextBox>();
        List<TextBox> ProfundidadList = new List<TextBox>();
        List<TextBox> UbicacionList = new List<TextBox>();
        List<Boton> btnDetailsList = new List<Boton>();
        List<Boton> btnDeleteList = new List<Boton>();
        List<CXN_NOTASMED> listaNotasMed = new List<CXN_NOTASMED>();

        void ActualizarLeaveTextBox(object sender, EventArgs e)
        {
            try
            {
                TextBox txt = sender as TextBox;
                string pos = Regex.Replace(txt.Name, @"[^\d]", "");
                string letras = Regex.Replace(txt.Name, @"\d", "");

                CXN_NOTASMED retorno = listaNotasMed.Find(x => x.Id == Convert.ToInt32(pos));
                if (retorno != null)
                {
                    if (letras == "Largo")
                    {
                        retorno.Largo = decimal.Parse(txt.Text.Replace(",","."), CultureInfo.InvariantCulture);
                    }
                    else if (letras == "Ancho")
                    {
                        retorno.Ancho = decimal.Parse(txt.Text.Replace(",", "."), CultureInfo.InvariantCulture);
                    }
                    else if (letras == "Profundidad")
                    {
                        retorno.Profundidad = decimal.Parse(txt.Text.Replace(",", "."), CultureInfo.InvariantCulture);
                    }
                    else if (letras == "Ubicacion")
                    {
                        retorno.txtLocalizacion = txt.Text;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        void CreateControl(int X, int Y, string Name, string Tipo, int Numero)
        {
            if (Tipo == "Largo")
            {
                TextBox LargoTxt = new TextBox(); ;
                LargoTxt.Font = new Font("Arial", 12);
                LargoTxt.Size = new Size(175, 32);
                panel5.Controls.Add(LargoTxt);
                LargoTxt.Location = new Point(X, Y);
                LargoTxt.Name = Name; 
                LargoTxt.KeyPress += NumberDecimal;
                LargoTxt.Leave += ActualizarLeaveTextBox;
                
                panel6.Controls.Add(LargoTxt);
                LargoList.Add(LargoTxt);
            }
            else if (Tipo == "Ancho")
            {
                TextBox AnchoTxt = new TextBox(); ;
                AnchoTxt.Font = new Font("Arial", 12);
                AnchoTxt.Size = new Size(175, 32);
                panel5.Controls.Add(AnchoTxt);
                AnchoTxt.Location = new Point(X, Y);
                AnchoTxt.Name = Name;
                AnchoTxt.KeyPress += NumberDecimal;
                AnchoTxt.Leave += ActualizarLeaveTextBox;

                panel6.Controls.Add(AnchoTxt);
                AnchoList.Add(AnchoTxt);
            }
            else if (Tipo == "Profundidad")
            {
                TextBox ProfundidadTxt = new TextBox(); ;
                ProfundidadTxt.Font = new Font("Arial", 12);
                ProfundidadTxt.Size = new Size(175, 32);
                panel5.Controls.Add(ProfundidadTxt);
                ProfundidadTxt.Location = new Point(X, Y);
                ProfundidadTxt.Name = Name;
                ProfundidadTxt.KeyPress += NumberDecimal;
                ProfundidadTxt.Leave += ActualizarLeaveTextBox;

                panel6.Controls.Add(ProfundidadTxt);
                ProfundidadList.Add(ProfundidadTxt);
            }
            else if (Tipo == "Ubicacion")
            {
                TextBox UbicacionTxt = new TextBox(); ;
                UbicacionTxt.Font = new Font("Arial", 12);
                UbicacionTxt.Size = new Size(175, 32);
                panel5.Controls.Add(UbicacionTxt);
                UbicacionTxt.Location = new Point(X, Y);
                UbicacionTxt.Name = Name;
                UbicacionTxt.CharacterCasing = CharacterCasing.Upper;
                UbicacionTxt.Leave += ActualizarLeaveTextBox;

                panel6.Controls.Add(UbicacionTxt);
                UbicacionList.Add(UbicacionTxt);
            }
            else if (Tipo == "Boton")
            {
                Boton Botonbtn = new Boton(); ;
                Botonbtn.Font = new Font("Arial", 12);
                Botonbtn.Size = new Size(94, 32);
                panel5.Controls.Add(Botonbtn);
                Botonbtn.Location = new Point(X, Y);
                Botonbtn.Name = Name;
                Botonbtn.Text = "Detalle";
                Botonbtn.Tag = Numero;
                Botonbtn.Click += Botonbtn_Click;

                panel6.Controls.Add(Botonbtn);
                btnDetailsList.Add(Botonbtn);
            }
            else if (Tipo == "Boton2")
            {
                Boton Botonbtn2 = new Boton(); ;
                Botonbtn2.Font = new Font("Arial", 12);
                Botonbtn2.Size = new Size(94, 32);
                panel5.Controls.Add(Botonbtn2);
                Botonbtn2.Location = new Point(X, Y);
                Botonbtn2.Name = Name;
                Botonbtn2.Text = "Quitar";
                Botonbtn2.Tag = Numero;
                Botonbtn2.Click += BotonbtnDel_Click;

                panel6.Controls.Add(Botonbtn2);
                btnDeleteList.Add(Botonbtn2);
            }
        }
        private void button8_Click(object sender, EventArgs e)
        {      
            if (NumeroTextBox == 0)
            {
                CreateControl(9, 62, "Largo" + NumeroTextBox, "Largo", NumeroTextBox);
                CreateControl(199, 62, "Ancho" + NumeroTextBox, "Ancho", NumeroTextBox);
                CreateControl(389, 62, "Profundidad" + NumeroTextBox, "Profundidad", NumeroTextBox);
                CreateControl(579, 62, "Ubicacion" + NumeroTextBox, "Ubicacion", NumeroTextBox);
                CreateControl(755, 57, "Boton" + NumeroTextBox, "Boton", NumeroTextBox);
                CreateControl(874, 57, "Boton2" + NumeroTextBox, "Boton2", NumeroTextBox);

                NumeroTextBox++;
            }
            else
            {
                TextBox LargoTempTxt = LargoList[LargoList.Count - 1];
                TextBox AnchoTempTxt = AnchoList[AnchoList.Count - 1];
                TextBox ProfundidadTempTxt = ProfundidadList[ProfundidadList.Count - 1];
                TextBox UbicacionTempTxt = UbicacionList[UbicacionList.Count - 1];
                Boton BotonTempBtn = btnDetailsList[btnDetailsList.Count - 1];
                Boton BotonTempBtn2 = btnDeleteList[btnDeleteList.Count - 1];

                CreateControl(LargoTempTxt.Location.X, LargoTempTxt.Location.Y + 32, "Largo" + NumeroTextBox, "Largo", NumeroTextBox);
                CreateControl(AnchoTempTxt.Location.X, AnchoTempTxt.Location.Y + 32, "Ancho" + NumeroTextBox, "Ancho", NumeroTextBox);
                CreateControl(ProfundidadTempTxt.Location.X, ProfundidadTempTxt.Location.Y + 32, "Profundidad" + NumeroTextBox, "Profundidad", NumeroTextBox);
                CreateControl(UbicacionTempTxt.Location.X, UbicacionTempTxt.Location.Y + 32, "Ubicacion" + NumeroTextBox, "Ubicacion", NumeroTextBox);
                CreateControl(BotonTempBtn.Location.X, BotonTempBtn.Location.Y + 32, "Boton" + NumeroTextBox, "Boton", NumeroTextBox);
                CreateControl(BotonTempBtn2.Location.X, BotonTempBtn2.Location.Y + 32, "Boton2" + NumeroTextBox, "Boton2", NumeroTextBox);

                NumeroTextBox++;
            }            
        }
        private void BotonbtnDel_Click(object sender, EventArgs e)
        {
            try
            {
                Boton btn = (Boton)sender;
                int index = (int)btn.Tag;

                listaNotasMed.RemoveAll(x => x.Id == index);

                panel6.Controls.Remove(LargoList[index]);
                panel6.Controls.Remove(AnchoList[index]);
                panel6.Controls.Remove(ProfundidadList[index]);
                panel6.Controls.Remove(UbicacionList[index]);
                panel6.Controls.Remove(btnDetailsList[index]);
                panel6.Controls.Remove(btnDeleteList[index]);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void Botonbtn_Click(object sender, EventArgs e)
        {
            try
            {
                Boton btn = (Boton)sender;

                int index = (int)btn.Tag;

                string largo = LargoList[index].Text;
                string ancho = AnchoList[index].Text;
                string profundidad = ProfundidadList[index].Text;
                string ubicacion = UbicacionList[index].Text;

                if (string.IsNullOrEmpty(largo) || string.IsNullOrEmpty(ancho) ||
                    string.IsNullOrEmpty(profundidad) || string.IsNullOrEmpty(ubicacion))
                {
                    MG = new MensajesGeneral()
                    {
                        Mensaje = "Debe diligenciar Largo, Ancho y Profundidad",
                        TipoImagen = 1000
                    };

                    MG.ShowDialog();
                }
                else
                {
                    CXN_NOTASMED listTemp = new CXN_NOTASMED
                    {
                        Id = index,
                        Admision = Admision,
                        Ancho = decimal.Parse(ancho, CultureInfo.InvariantCulture),
                        Largo = decimal.Parse(largo, CultureInfo.InvariantCulture),
                        Profundidad = decimal.Parse(profundidad, CultureInfo.InvariantCulture),
                        txtLocalizacion = ubicacion.ToUpper().Trim()
                    };

                    CXN_NOTASMED buscaExistente = listaNotasMed.Find(x => x.Id == listTemp.Id);
                    if (buscaExistente != null) 
                    {
                        buscaExistente.Ancho = decimal.Parse(ancho, CultureInfo.InvariantCulture);
                        buscaExistente.Largo = decimal.Parse(largo, CultureInfo.InvariantCulture);
                        buscaExistente.Profundidad = decimal.Parse(profundidad, CultureInfo.InvariantCulture);
                        buscaExistente.txtLocalizacion = listTemp.txtLocalizacion;
                    }
                    else
                    {
                        listaNotasMed.Add(listTemp);
                    }

                    MedidasCuracion mC = new MedidasCuracion(listaNotasMed, listTemp.Id);
                    mC.ShowDialog();
                }                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }           
        }
        public void setDetalles(List<CXN_NOTASMED> detalle)
        {
            //listaNotasMed.Clear();
            listaNotasMed = detalle;
        }
        void NumberDecimal(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            if (e.KeyChar == '.' && ((TextBox)sender).Text.Contains('.'))
            {
                e.Handled = true;
            }
        }
        public void CargarMedidasEstaNota()
        {
            try
            {
                List<CXN_NOTASMED> _lista = repoNotasCuracion.LoadHeridas(Paciente, Admision);
                if (_lista != null)
                {
                    foreach (CXN_NOTASMED i in _lista)
                    {
                        i.Id = NumeroTextBox;

                        if (NumeroTextBox == 0)
                        {
                            CreateControl(9, 62, "Largo" + NumeroTextBox, "Largo", NumeroTextBox);
                            CreateControl(199, 62, "Ancho" + NumeroTextBox, "Ancho", NumeroTextBox);
                            CreateControl(389, 62, "Profundidad" + NumeroTextBox, "Profundidad", NumeroTextBox);
                            CreateControl(579, 62, "Ubicacion" + NumeroTextBox, "Ubicacion", NumeroTextBox);
                            CreateControl(755, 57, "Boton" + NumeroTextBox, "Boton", NumeroTextBox);
                            CreateControl(874, 57, "Boton2" + NumeroTextBox, "Boton2", NumeroTextBox);
                        }
                        else
                        {
                            TextBox LargoTempTxt = LargoList[LargoList.Count - 1];
                            TextBox AnchoTempTxt = AnchoList[AnchoList.Count - 1];
                            TextBox ProfundidadTempTxt = ProfundidadList[ProfundidadList.Count - 1];
                            TextBox UbicacionTempTxt = UbicacionList[UbicacionList.Count - 1];
                            Boton BotonTempBtn = btnDetailsList[btnDetailsList.Count - 1];
                            Boton BotonTempBtn2 = btnDeleteList[btnDeleteList.Count - 1];

                            CreateControl(LargoTempTxt.Location.X, LargoTempTxt.Location.Y + 32, "Largo" + NumeroTextBox, "Largo", NumeroTextBox);
                            CreateControl(AnchoTempTxt.Location.X, AnchoTempTxt.Location.Y + 32, "Ancho" + NumeroTextBox, "Ancho", NumeroTextBox);
                            CreateControl(ProfundidadTempTxt.Location.X, ProfundidadTempTxt.Location.Y + 32, "Profundidad" + NumeroTextBox, "Profundidad", NumeroTextBox);
                            CreateControl(UbicacionTempTxt.Location.X, UbicacionTempTxt.Location.Y + 32, "Ubicacion" + NumeroTextBox, "Ubicacion", NumeroTextBox);
                            CreateControl(BotonTempBtn.Location.X, BotonTempBtn.Location.Y + 32, "Boton" + NumeroTextBox, "Boton", NumeroTextBox);
                            CreateControl(BotonTempBtn2.Location.X, BotonTempBtn2.Location.Y + 32, "Boton2" + NumeroTextBox, "Boton2", NumeroTextBox);                            
                        }

                        LargoList[NumeroTextBox].Text = i.Largo.ToString(); 
                        AnchoList[NumeroTextBox].Text = i.Ancho.ToString();
                        ProfundidadList[NumeroTextBox].Text = i.Profundidad.ToString();
                        UbicacionList[NumeroTextBox].Text = i.txtLocalizacion.ToString();

                        listaNotasMed.Add(i);

                        NumeroTextBox++;
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
        #endregion FIN MEDIDAS
      
        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            FrontFHIR.VisorZamenis.VerRDA ass = new FrontFHIR.VisorZamenis.VerRDA(false, CMANTID, CMANID);
            ass.ShowDialog();
        }
        private void ActualizarPAC()
        {
            Medicina.ActualizarPaciente Historia_Edita_Paciente = new Medicina.ActualizarPaciente(Paciente);
            Historia_Edita_Paciente.ShowDialog();
        }
        string Capitalize(int Mes)
        {
            switch (Mes)
            {
                case 1:
                    return "Enero";
                case 2:
                    return "Febrero";
                case 3:
                    return "Marzo";
                case 4:
                    return "Abril";
                case 5:
                    return "Mayo";
                case 6:
                    return "Junio";
                case 7:
                    return "Julio";
                case 8:
                    return "Agosto";
                case 9:
                    return "Septiembre";
                case 10:
                    return "Octubre";
                case 11:
                    return "Noviembre";
                case 12:
                    return "Diciembre";
                default:
                    return "";
            }
        }
        void randomEncuestaSatisfaccion()
        {
            try
            {
                DateTime Hoy = DateTime.Now.Date;
                string Mes = Capitalize(Hoy.Month);

                if (repoConfSystem.getListado()["EncuestaSatisCU"] == "A")
                {
                    if (repoEncuestasSatis.getCantEncuestaCU("CU", Mes, Hoy.Year) == true)
                    {
                        CXN_CONFENCUESTA C = new CXN_CONFENCUESTA
                        {
                            Mes = Mes,
                            Año = Hoy.Year,
                            Servicio = "CU"
                        };

                        if (repoEncuestasSatis.getCantCitas(this.Paciente, C) == true)
                        {
                            if (repoEncuestasSatis.getCantEncuestasPaciente(this.Paciente, C) == true)
                            {
                                Random random = new Random();
                                int numero = random.Next(1, 2);
                                int numero2 = random.Next(1, 2);

                                if (numero == numero2)
                                {
                                    Extras.EncuestaSatisfaccion en = new Extras.EncuestaSatisfaccion(this.Admision);
                                    en.ShowDialog();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        void openConditions(string Tipo)
        {
            c = new CondicionesP(Tipo, this.Paciente);
            c.ShowDialog();

            CargarOpcionesRecomendaciones();
        }
        private void Lista_LLenar()
        {
            try
            {
                HistorialNotas H = new HistorialNotas(Paciente);
                H.ShowDialog();
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        void InsertarHistoria()
        {
            try
            {
                var getH = repoMedicinaGeneral.getLastHistoryToCopy(Paciente);
                if (getH != null)
                {
                    var getC = repoCargos.getLasCargoToCopy(Convert.ToInt32(getH.HC_Adm));
                    if (getC != null)
                    {
                        var getHO = repoAgendaMedica.getLastHorToCopy(Convert.ToInt32(getH.HC_Adm));
                        if (getHO != null)
                        {
                            DateTime HoraTexto = DateTime.Now;

                            CXN_HORARIO H = new CXN_HORARIO
                            {
                                Hor_Estado = "H",
                                Hor_Pac_Id = Convert.ToInt32(getHO.Hor_Pac_Id),
                                Hor_Pac_Bod = Convert.ToInt32(getHO.Hor_Pac_Bod),
                                Hor_Pac_Tipo_Serv = getHO.Hor_Pac_Tipo_Serv.ToString(),
                                Hor_Pac_Cia = Convert.ToInt32(getHO.Hor_Pac_Cia),
                                Hor_Pac_Ase = Convert.ToInt32(getHO.Hor_Pac_Ase),
                                Hor_Pac_Cup = getHO.Hor_Pac_Cup.ToString(),
                                Hor_Pac_UsrGraba = Comunes.Contenedor.UsuarioLogueado,
                                Hor_Imp_Age = getHO.Hor_Imp_Age.ToString(),
                                Hor_Pac_Fecha_Cita = Convert.ToDateTime(Fecha_Serv),
                                Hor_Pac_Id_Hora = "0000",
                                Hor_Observacion = "ARRASTRE AUTOMATICO NOTAS DE ENFERMERIA",
                                Hor_Pac_Sal = getHO.Hor_Pac_Sal.ToString(),
                                Hor_Vales = getHO.Hor_Vales.ToString(),
                                Hor_Pac_Modalidad = getHO.Hor_Pac_Modalidad.ToString(),
                                Hor_Pac_Hora_Cita = Convert.ToDateTime(HoraTexto), //hora cita
                                Hor_GrupoServicios = "01",
                                Hor_Regimen = getHO.Hor_Regimen.ToString()
                            };

                            int insertacita = repoAgendaMedica.AgendarPaciente(H);
                            if (insertacita >= 1)
                            {
                                int AdmiNew = repoAgendaMedica.getLastIDToCopy(H.Hor_Pac_Id);
                                if (AdmiNew != 0)
                                {
                                    CXN_HCMG Hcmg = new CXN_HCMG
                                    {
                                        HC_Pac = textBox2.Text,
                                        HC_Prof = Convert.ToInt32(getH.HC_Prof),
                                        HC_Ase = Convert.ToInt32(getH.HC_Ase),
                                        HC_Com = Convert.ToInt32(getH.HC_Com),
                                        HC_Pacid = Convert.ToInt32(getH.HC_Pacid),
                                        HC_Edad = textBox4.Text,
                                        HC_FechaNto = Convert.ToDateTime(textBox3.Text),
                                        HC_MotivoC = "Ingresa paciente a consulta para dar inicio al paquete. Se continúa con igual  manejo instaurado por el Medico General Institucional",
                                        HC_EnfA = getH.HC_EnfA.ToString(),
                                        HC_GradoC = getH.HC_GradoC.ToString(),
                                        HC_TipoLes = getH.HC_TipoLes.ToString(),
                                        HC_ActEje = getH.HC_ActEje.ToString(),
                                        HC_Vez = getH.HC_Vez.ToString(),
                                        HC_Neurologico = getH.HC_Neurologico.ToString(),
                                        HC_Cardiovascular = getH.HC_Cardiovascular.ToString(),
                                        HC_GastroIntestinal = getH.HC_GastroIntestinal.ToString(),
                                        HC_GastroUrinario = getH.HC_GastroUrinario.ToString(),
                                        HC_OsteoMuscular = getH.HC_OsteoMuscular.ToString(),
                                        HC_Piel = getH.HC_Piel.ToString(),
                                        HC_Ocupacion = getH.HC_Ocupacion.ToString(),
                                        HC_AparienciaG = getH.HC_AparienciaG.ToString(),
                                        HC_EstadoEmo = getH.HC_EstadoEmo.ToString(),
                                        HC_EstadoNut = getH.HC_EstadoNut.ToString(),
                                        HC_Exudado = getH.HC_Exudado.ToString(),
                                        HC_Presart = getH.HC_Presart.ToString(),
                                        HC_Frecar = getH.HC_Frecar.ToString(),
                                        HC_FreRes = getH.HC_FreRes.ToString(),
                                        HC_Temp = getH.HC_Temp.ToString(),
                                        HC_Peso = getH.HC_Peso.ToString(),
                                        HC_Altura = getH.HC_Altura.ToString(),
                                        HC_IMC = getH.HC_IMC.ToString(),
                                        HC_ITB = getH.HC_ITB.ToString(),
                                        HC_DescHer = getH.HC_DescHer.ToString(),
                                        HC_TejCom = getH.HC_TejCom.ToString(),
                                        HC_CaracTej = getH.HC_CaracTej.ToString(),
                                        HC_SignosInf = getH.HC_SignosInf.ToString(),
                                        HC_PielCirc = getH.HC_PielCirc.ToString(),
                                        HC_ConsCant = getH.HC_ConsCant.ToString(),
                                        HC_Estado = getH.HC_Estado.ToString(),
                                        HC_Dolor = getH.HC_Dolor.ToString(),
                                        HC_Analisis = getH.HC_Analisis.ToString(),
                                        HC_Complicacion = getH.HC_Complicacion.ToString(),
                                        HC_PruebasDiag = getH.HC_PruebasDiag.ToString(),
                                        HC_ProtoInst = getH.HC_ProtoInst.ToString(),
                                        HC_PManejo = getH.HC_PManejo.ToString(),
                                        HC_DX1 = getH.HC_DX1.ToString(),
                                        HC_DX2 = getH.HC_DX2.ToString(),
                                        HC_DX3 = getH.HC_DX3.ToString(),
                                        HC_DX1T = getH.HC_DX1T.ToString(),
                                        HC_AntFam = getH.HC_AntFam.ToString(),
                                        HC_AntPat = getH.HC_AntPat.ToString(),
                                        HC_AntQui = getH.HC_AntQui.ToString(),
                                        HC_AntAle = getH.HC_AntAle.ToString(),
                                        HC_AntFarma = getH.HC_AntFarma.ToString(),
                                        HC_Hematolin = getH.HC_Hematolin.ToString(),
                                        HC_Patologia = getH.HC_Patologia.ToString(),
                                        HC_Fecha = Fecha_Serv,
                                        HC_Cant = 1,
                                        HC_RH = getH.HC_RH.ToString(),
                                        HC_SubPat = getH.HC_SubPat.ToString(),
                                        HC_Imp_Dx = getH.HC_Imp_Dx.ToString(),
                                        HC_Respiratorio = getH.HC_Respiratorio.ToString(),
                                        HC_Epidemia = getH.HC_Epidemia.ToString(),
                                        HC_ServCatalogo = getH.HC_ServCatalogo.ToString(),
                                        HC_CupCatalogo = getH.HC_CupCatalogo.ToString(),
                                        HC_Adm = AdmiNew,
                                        HC_TipoINGSAL = ""
                                    };

                                    bool Graba = repoMedicinaGeneral.GrabaHCMG(Hcmg);
                                    if (Graba != true)
                                    {
                                        MessageBox.Show("La nota de curacion se registro correctamente sin novedades.  Pero como es un inicio de paquete no se encontro la ultima " +
                                        "historia clinica para copiar el registro de de inicio.  Por lo tanto se debera crear una historia clinica nueva por parte del medico general",
                                            "Incompleto",
                                            MessageBoxButtons.OK,
                                            MessageBoxIcon.Exclamation);
                                    }
                                    else
                                    {
                                        CXN_CARGOS CA = new CXN_CARGOS
                                        {
                                            Car_Adm_Id = AdmiNew,
                                            Car_Pac = Convert.ToInt32(getC.Car_Pac),
                                            Car_Cia = Convert.ToInt32(getC.Car_Cia),
                                            Car_Ase = Convert.ToInt32(getC.Car_Ase),
                                            Car_Prof = Convert.ToInt32(getC.Car_Prof),
                                            Car_Fecha = Fecha_Serv,
                                            Car_Estado = "G",
                                            Car_Tipo = "Historia",
                                            Car_Cod = getC.Car_Cod.ToString(),
                                            Car_Val_Tot = Convert.ToInt32(getC.Car_Val_Tot),
                                            Car_Val_Un = Convert.ToInt32(getC.Car_Val_Un),
                                            Car_Tipo_Serv = getC.Car_Tipo_Serv.ToString(),
                                            Car_Cant = 1,
                                            Car_Detalle = getC.Car_Detalle.ToString(),
                                            Car_Item = getC.Car_Item.ToString(),
                                            Car_Dx1 = getC.Car_Dx1.ToString(),
                                            Car_Dx2 = getC.Car_Dx2.ToString(),
                                            Car_Dx3 = getC.Car_Dx3.ToString(),
                                            Car_Ambito = Convert.ToInt32(getC.Car_Ambito),
                                            Car_Personal = Convert.ToInt32(getC.Car_Personal),
                                            Car_CExterna = Convert.ToInt32(getC.Car_CExterna),
                                            Car_Finalidad = Convert.ToInt32(getC.Car_Finalidad),
                                            Car_Finalidad_CO = Convert.ToInt32(getC.Car_Finalidad_CO), //motivo                                           
                                            Car_Imp_Dx = Convert.ToInt32(getC.Car_Imp_Dx),
                                            Car_Regimen = getC.Car_Regimen.ToString()
                                        };

                                        bool insertarCargo = repoCargos.InsertarCargoHistorias(CA);
                                        if (insertarCargo == false)
                                        {
                                            Comunes.MensajesGeneral MG = new Comunes.MensajesGeneral();
                                            MG.Mensaje = "No se logro guardar el cargo economico en el registro de valores a cobrar en la factura, " +
                                                "su historia quedo resgitrada pero reporte este incidente a la recepcion";
                                            MG.TipoImagen = 1000;
                                            MG.ShowDialog();
                                        }
                                        //EXITO
                                    }
                                }
                                else
                                {
                                    Comunes.MensajesGeneral MG = new Comunes.MensajesGeneral();
                                    MG.Mensaje = "No se logro recuperar el indice unico de historia clinica";
                                    MG.TipoImagen = 1000;
                                    MG.ShowDialog();
                                }
                            }
                            else
                            {
                                MessageBox.Show("La nota de curacion se registro correctamente sin novedades.  Pero como es un inicio de paquete no se encontro el ultimo registro del horario.",
                           "Incompleto",
                           MessageBoxButtons.OK,
                           MessageBoxIcon.Exclamation);
                            }
                        }
                        else
                        {
                            MessageBox.Show("La nota de curacion se registro correctamente sin novedades.  Pero como es un inicio de paquete no se encontro el ultimo registro del horario.",
                            "Incompleto",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Exclamation);
                        }
                    }
                    else
                    {
                        MessageBox.Show("La nota de curacion se registro correctamente sin novedades.  Pero como es un inicio de paquete no se encontro el ultimo " +
                        "cargo para copiar el registro de de inicio.  Reportelo inmediatamente a la administracion",
                            "Incompleto",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Exclamation);
                    }
                }
                else
                {
                    Comunes.MensajesGeneral MG = new Comunes.MensajesGeneral();
                    MG.Mensaje = "La nota de curacion se registro correctamente sin novedades.  Pero como es un inicio de paquete no se encontro la ultima " +
                        "historia clinica para copiar el registro de de inicio.  Por lo tanto se debera crear una historia clinica nueva por parte del medico general";
                    MG.TipoImagen = 1000;
                    MG.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
    }
}
