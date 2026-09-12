using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Domain;
using Domain.CXN;
using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using ZamenisHealth.Comunes;
using ZamenisHealth.HistoriasClinicas.Extras;

namespace ZamenisHealth.HistoriasClinicas
{
    public partial class Historia_TerOcupacional : Forma
    {
        private static readonly ITerapiaOcupacional repoTO = new MTerapiaOcupacional();
        private static readonly IRIPS repoRIPS = new MRIPS();
        private static readonly ICargos repoCargos = new MCargos();
        private static readonly IAgenda repoAgendaMedica = new MAgenda();
        private static readonly IConvenios repoConvenios = new MConvenios();
        private static readonly ICondiciones repoCond = new MCondiciones();
        private static readonly IAgendaC repoAgendaMedicaConsultas = new MAgendaC();
        private static readonly IConfSystem repoConfSystem = new MConfSystem();

        private Extras.CondicionesP c;
        public int Admision;
        DateTime Fecha_Serv;
        int Paciente, Cia, Ase, Prof, Valor;
        string CUP, TSERV, Reg_RIP, CMANTID, CMANID, Serv;
     
        public Historia_TerOcupacional()
        {
            InitializeComponent();
        }

        private void toolStripLabel1_Click(object sender, EventArgs e)
        {
            try
            {
                if (textBox7.Text == "") { MessageBox.Show("Revise seccion Historia"); return; }
                if (textBox8.Text == "") { MessageBox.Show("Revise seccion Historia"); return; }
                if (textBox9.Text == "") { MessageBox.Show("Revise seccion Historia"); return; }
                if (textBox18.Text == "") { MessageBox.Show("Revise seccion Historia"); return; }

                if (textBox10.Text == "") { MessageBox.Show("Revise seccion Detalles"); return; }
                if (textBox11.Text == "") { MessageBox.Show("Revise seccion Detalles"); return; }
                if (textBox19.Text == "") { MessageBox.Show("Revise seccion Detalles"); return; }
                if (textBox20.Text == "") { MessageBox.Show("Revise seccion Detalles"); return; }
                if (textBox21.Text == "") { MessageBox.Show("Revise seccion Detalles"); return; }

                if (textBox12.Text == "") { MessageBox.Show("Revise seccion Func. MSup."); return; }
                if (textBox13.Text == "") { MessageBox.Show("Revise seccion Act. Vida Diaria 2"); return; }
                if (textBox16.Text == "") { MessageBox.Show("Revise seccion Tiempo Libre"); return; }
                if (textBox15.Text == "") { MessageBox.Show("Revise seccion Tiempo Libre"); return; }

                CXN_HCTO H = new CXN_HCTO
                {
                    HC_Pac = textBox1.Text,
                    HC_Cant = 1,
                    HC_Prof = Prof,
                    HC_Ase = Ase,
                    HC_Cia = Cia,
                    HC_Fecha = Fecha_Serv,
                    HC_Edad = textBox4.Text,
                    HC_PacId = Paciente,
                    HC_Adm = Admision,
                    HC_FechaNto = Convert.ToDateTime(textBox3.Text),
                    HC_CIE10 = textBox18.Text,
                    HC_DiagMedico = textBox17.Text,
                    HC_OcuAct = textBox19.Text,
                    HC_HisFam = textBox7.Text,
                    HC_HisOcu = textBox8.Text,
                    HC_HabRut = textBox9.Text,
                    HC_DiagOcu = textBox20.Text,
                    HC_PronoOcu = textBox21.Text,
                    HC_Acciones = textBox10.Text,
                    HC_AntLaboral = textBox11.Text,
                    MSD_MCP = comboBox1.Text,
                    MSD_MCA = comboBox2.Text,
                    MSI_MCP = comboBox4.Text,
                    MSI_MCA = comboBox3.Text,
                    MSD_MBP = comboBox8.Text,
                    MSD_MBA = comboBox7.Text,
                    MSI_MBP = comboBox6.Text,
                    MSI_MBA = comboBox5.Text,
                    MSD_MHP = comboBox12.Text,
                    MSD_MHA = comboBox11.Text,
                    MSI_MHP = comboBox10.Text,
                    MSI_MHA = comboBox9.Text,
                    MSD_MEP = comboBox16.Text,
                    MSD_MEA = comboBox15.Text,
                    MSI_MEP = comboBox14.Text,
                    MSI_MEA = comboBox13.Text,
                    MSD_MCIP = comboBox20.Text,
                    MSD_MCIA = comboBox19.Text,
                    MSI_MCIP = comboBox18.Text,
                    MSI_MCIA = comboBox17.Text,
                    MSD_MPP = comboBox24.Text,
                    MSD_MPA = comboBox23.Text,
                    MSI_MPP = comboBox22.Text,
                    MSI_MPA = comboBox21.Text,
                    MSD_MRP = comboBox28.Text,
                    MSD_MRA = comboBox27.Text,
                    MSI_MRP = comboBox26.Text,
                    MSI_MRA = comboBox25.Text,
                    MSD_MPIEP = comboBox32.Text,
                    MSD_MPIEA = comboBox31.Text,
                    MSI_MPIEP = comboBox30.Text,
                    MSI_MPIEA = comboBox29.Text,
                    MSD_AAP = comboBox64.Text,
                    MSD_AAA = comboBox63.Text,
                    MSI_AAP = comboBox62.Text,
                    MSI_AAA = comboBox61.Text,
                    MSD_AABP = comboBox60.Text,
                    MSD_AABA = comboBox59.Text,
                    MSI_AABP = comboBox58.Text,
                    MSI_AABA = comboBox57.Text,
                    MSD_AADP = comboBox56.Text,
                    MSD_AADA = comboBox55.Text,
                    MSI_AADP = comboBox54.Text,
                    MSI_AADA = comboBox53.Text,
                    MSD_AATP = comboBox52.Text,
                    MSD_AATA = comboBox51.Text,
                    MSI_AATP = comboBox50.Text,
                    MSI_AATA = comboBox49.Text,
                    MSD_AALP = comboBox68.Text,
                    MSD_AALA = comboBox67.Text,
                    MSI_AALP = comboBox66.Text,
                    MSI_AALA = comboBox65.Text,
                    MSD_AGAP = comboBox48.Text,
                    MSD_AGAA = comboBox47.Text,
                    MSI_AGAP = comboBox46.Text,
                    MSI_AGAA = comboBox45.Text,
                    MSD_AGACILP = comboBox44.Text,
                    MSD_AGACILA = comboBox43.Text,
                    MSI_AGACILP = comboBox42.Text,
                    MSI_AGACILA = comboBox41.Text,
                    MSD_AGAESFP = comboBox40.Text,
                    MSD_AGAESFA = comboBox39.Text,
                    MSI_AGAESFP = comboBox38.Text,
                    MSI_AGAESFA = comboBox37.Text,
                    MSD_PINFP = comboBox36.Text,
                    MSD_PINFA = comboBox35.Text,
                    MSI_PINFP = comboBox34.Text,
                    MSI_PINFA = comboBox33.Text,
                    MSD_PINTP = comboBox76.Text,
                    MSD_PINTA = comboBox75.Text,
                    MSI_PINTP = comboBox74.Text,
                    MSI_PINTA = comboBox73.Text,
                    MSD_PLATP = comboBox72.Text,
                    MSD_PLATA = comboBox71.Text,
                    MSI_PLATP = comboBox70.Text,
                    MSI_PLATA = comboBox69.Text,
                    HC_ObservaFMS = textBox12.Text,
                    HC_A_T1 = comboBox77.Text,
                    HC_A_T2 = comboBox78.Text,
                    HC_A_T3 = comboBox79.Text,
                    HC_A_T4 = comboBox82.Text,
                    HC_A_T5 = comboBox81.Text,
                    HC_A_T6 = comboBox80.Text,
                    HC_A_T7 = comboBox85.Text,
                    HC_A_T8 = comboBox84.Text,
                    HC_A_T9 = comboBox83.Text,
                    HC_A_T10 = comboBox86.Text,
                    HC_A_T11 = comboBox92.Text,
                    HC_A_T12 = comboBox91.Text,
                    HC_A_T13 = comboBox89.Text,
                    HC_A_T14 = comboBox88.Text,
                    HC_A_T15 = comboBox97.Text,
                    HC_A_T16 = comboBox96.Text,
                    HC_A_T17 = comboBox95.Text,
                    HC_A_T18 = comboBox94.Text,
                    HC_A_T19 = comboBox93.Text,
                    HC_A_T20 = comboBox90.Text,
                    HC_A_T21 = comboBox87.Text,
                    HC_A_T22 = comboBox98.Text,
                    HC_A_T23 = comboBox99.Text,
                    HC_A_T24 = comboBox100.Text,
                    HC_A_T25 = comboBox103.Text,
                    HC_A_T26 = comboBox102.Text,
                    HC_A_T27 = comboBox101.Text,
                    HC_A_T28 = comboBox106.Text,
                    HC_A_T29 = comboBox105.Text,
                    HC_A_T30 = comboBox104.Text,
                    HC_A_T31 = comboBox107.Text,
                    HC_A_T32 = comboBox111.Text,
                    HC_A_T33 = comboBox110.Text,
                    HC_A_T34 = comboBox109.Text,
                    HC_A_T35 = comboBox108.Text,
                    HC_A_ObservaVD = textBox13.Text,
                    HC_A_T37 = comboBox117.Text,
                    HC_A_T43 = comboBox116.Text,
                    HC_A_T38 = comboBox115.Text,
                    HC_A_T44 = comboBox114.Text,
                    HC_A_T39 = comboBox113.Text,
                    HC_A_T45 = comboBox112.Text,
                    HC_A_T40 = comboBox120.Text,
                    HC_A_T46 = comboBox119.Text,
                    HC_A_T41 = comboBox118.Text,
                    HC_A_T47 = comboBox123.Text,
                    HC_A_T42 = comboBox122.Text,
                    HC_A_T48 = comboBox121.Text,
                    HC_A_T50 = comboBox126.Text,
                    HC_A_T51 = comboBox125.Text,
                    HC_A_T52 = comboBox124.Text,
                    HC_A_T53 = textBox22.Text,
                    HC_A_T54 = comboBox129.Text,
                    HC_A_T55 = comboBox135.Text,
                    HC_A_T56 = comboBox134.Text,
                    HC_A_T57 = comboBox133.Text,
                    HC_A_T58 = comboBox132.Text,
                    HC_A_T59 = comboBox131.Text,
                    HC_A_T60 = comboBox130.Text,
                    HC_A_ObservaAVD = textBox16.Text,
                    HC_A_ObservaH = textBox14.Text
                };

                bool insert = repoTO.insertHistoria(H);
                if (insert != true)
                {
                    MessageBox.Show("No se logro insertar la historia",
                      "Error Inesperado",
                      MessageBoxButtons.OK,
                      MessageBoxIcon.Information);
                    return;
                }
                else
                {
                    int impresion = repoRIPS.TipoRipCargo(comboBox139.Text, "IMPDX");

                    CXN_CARGOS C = new CXN_CARGOS
                    {
                        Car_Adm_Id = Admision,
                        Car_Pac = Paciente,
                        Car_Cia = Cia,
                        Car_Ase = Ase,
                        Car_Prof = Prof,
                        Car_Fecha = Fecha_Serv,
                        Car_Estado = "G",
                        Car_Tipo = "Historia",
                        Car_Cod = CUP,
                        Car_Val_Tot = Valor,
                        Car_Val_Un = Valor,
                        Car_Tipo_Serv = TSERV,
                        Car_Cant = 1,
                        Car_Detalle = textBox18.Text,
                        Car_Item = Serv,
                        Car_Dx1 = textBox18.Text,
                        Car_Dx2 = "",
                        Car_Dx3 = "",
                        Car_Ambito = 0,
                        Car_Personal = 0,
                        Car_CExterna = 0,
                        Car_Finalidad = 0,
                        Car_Finalidad_CO = 0, //motivo                                           
                        Car_Imp_Dx = impresion,
                        Car_Regimen = Reg_RIP
                    };

                    bool insertarCargo = repoCargos.InsertarCargoHistorias(C);
                    if (insertarCargo == false)
                    {
                        MessageBox.Show("No se logro guardar el cargo economico en el registro de valores a cobrar en la factura, " +
                            "su historia quedo resgitrada pero reporte este incidente a la recepcion con la admision: " + Admision,
                            "Advertencia!!!",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }
                    else
                    {                        
                        repoAgendaMedica.ConsumirAdmision(Admision);
                        repoAgendaMedica.Graba_Hora_Salida(Admision);

                        Medicina.RIPSHistory for_RIPS = new Medicina.RIPSHistory();
                        for_RIPS.Adm_Cargo = Convert.ToInt32(Admision);
                        for_RIPS.ShowDialog();

                        this.Dispose();
                        this.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void toolStripLabel2_Click(object sender, EventArgs e)
        {
            try
            {
                var getlast = repoTO.getLastHistoy(Paciente);
                if (getlast != null)
                {
                    textBox18.Text = getlast.HC_CIE10;
                    textBox17.Text = getlast.HC_DiagMedico;
                    textBox19.Text = getlast.HC_OcuAct;
                    textBox7.Text = getlast.HC_HisFam;
                    textBox8.Text = getlast.HC_HisOcu;
                    textBox9.Text = getlast.HC_HabRut;
                    textBox10.Text = getlast.HC_Acciones;
                    textBox11.Text = getlast.HC_AntLaboral;
                    textBox20.Text = getlast.HC_DiagOcu;
                    textBox21.Text = getlast.HC_PronoOcu;

                    comboBox77.Text = getlast.HC_A_T1;
                    comboBox78.Text = getlast.HC_A_T2;
                    comboBox79.Text = getlast.HC_A_T3;

                    comboBox82.Text = getlast.HC_A_T4;
                    comboBox81.Text = getlast.HC_A_T5;
                    comboBox80.Text = getlast.HC_A_T6;
                    comboBox85.Text = getlast.HC_A_T7;
                    comboBox84.Text = getlast.HC_A_T8;
                    comboBox83.Text = getlast.HC_A_T9;
                    comboBox86.Text = getlast.HC_A_T10;

                    comboBox92.Text = getlast.HC_A_T11;
                    comboBox91.Text = getlast.HC_A_T12;
                    comboBox89.Text = getlast.HC_A_T13;
                    comboBox88.Text = getlast.HC_A_T14;

                    comboBox97.Text = getlast.HC_A_T15;
                    comboBox96.Text = getlast.HC_A_T16;
                    comboBox95.Text = getlast.HC_A_T17;
                    comboBox94.Text = getlast.HC_A_T18;
                    comboBox93.Text = getlast.HC_A_T19;
                    comboBox90.Text = getlast.HC_A_T20;
                    comboBox87.Text = getlast.HC_A_T21;
                    comboBox98.Text = getlast.HC_A_T22;
                    comboBox99.Text = getlast.HC_A_T23;
                    comboBox100.Text = getlast.HC_A_T24;

                    comboBox103.Text = getlast.HC_A_T25;
                    comboBox102.Text = getlast.HC_A_T26;
                    comboBox101.Text = getlast.HC_A_T27;

                    comboBox106.Text = getlast.HC_A_T28;
                    comboBox105.Text = getlast.HC_A_T29;
                    comboBox104.Text = getlast.HC_A_T30;
                    comboBox107.Text = getlast.HC_A_T31;

                    comboBox111.Text = getlast.HC_A_T32;
                    comboBox110.Text = getlast.HC_A_T33;
                    comboBox109.Text = getlast.HC_A_T34;
                    comboBox108.Text = getlast.HC_A_T35;

                    textBox13.Text = getlast.HC_A_ObservaVD;

                    comboBox117.Text = getlast.HC_A_T37;
                    comboBox115.Text = getlast.HC_A_T38;
                    comboBox113.Text = getlast.HC_A_T39;
                    comboBox120.Text = getlast.HC_A_T40;
                    comboBox122.Text = getlast.HC_A_T41;
                    comboBox118.Text = getlast.HC_A_T42;
                    comboBox116.Text = getlast.HC_A_T43;
                    comboBox114.Text = getlast.HC_A_T44;
                    comboBox112.Text = getlast.HC_A_T45;
                    comboBox119.Text = getlast.HC_A_T46;
                    comboBox123.Text = getlast.HC_A_T47;
                    comboBox121.Text = getlast.HC_A_T48;

                    textBox14.Text = getlast.HC_A_ObservaH;

                    comboBox126.Text = getlast.HC_A_T50;
                    comboBox125.Text = getlast.HC_A_T51;
                    comboBox124.Text = getlast.HC_A_T52;

                    textBox22.Text = getlast.HC_A_T53;
                    comboBox129.Text = getlast.HC_A_T54;
                    comboBox135.Text = getlast.HC_A_T55;
                    comboBox134.Text = getlast.HC_A_T56;
                    comboBox133.Text = getlast.HC_A_T57;
                    comboBox132.Text = getlast.HC_A_T58;
                    comboBox131.Text = getlast.HC_A_T59;
                    comboBox130.Text = getlast.HC_A_T60;

                    textBox16.Text = getlast.HC_A_ObservaAVD;
                    MessageBox.Show("Se ha recuperado el ultimo historial del paciente del dia: " + Convert.ToDateTime(getlast.HC_Fecha).ToString(Conexion.ConectionDictionary["Format_Fecha"]));
                }
                else
                {
                    MessageBox.Show("No hay antecedentes de este paciente");
                    return;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }                
        private void toolStripLabel4_Click(object sender, EventArgs e)
        {
            Medicina.Historial_Medico_1 RM = new Medicina.Historial_Medico_1();
            RM.ShowDialog();
        }
        private void toolStripLabel6_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult result = MessageBox.Show("Desea cancelar esta historia? Al hacerlo no guardara ningun dato", "Zamenis_Health", MessageBoxButtons.YesNo);

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
        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            ActualizarPAC();
        }
        void Botones()
        {
            ToolStripButton btnUpdateData = new ToolStripButton();
            btnUpdateData = createToolButton("Actualizar Datos");
            MenuLateral.Items.Add(btnUpdateData);
            btnUpdateData.Click += toolStripButton3_Click;

            ToolStripButton btnGenerar = new ToolStripButton();
            btnGenerar = createToolButton("Guardar");
            MenuLateral.Items.Add(btnGenerar);
            btnGenerar.Click += toolStripLabel1_Click;

            ToolStripButton btnHistory = new ToolStripButton();
            btnHistory = createToolButton("Historial");
            MenuLateral.Items.Add(btnHistory);
            btnHistory.Click += toolStripLabel4_Click;

            ToolStripButton btnLast = new ToolStripButton();
            btnLast = createToolButton("Traer Ultima");
            MenuLateral.Items.Add(btnLast);
            btnLast.Click += toolStripLabel2_Click;

            ToolStripButton btnCancel = new ToolStripButton();
            btnCancel = createToolButton("Cancelar");
            MenuLateral.Items.Add(btnCancel);
            btnCancel.Click += toolStripLabel6_Click;
        }
        private void Historia_TerOcupacional_Load(object sender, EventArgs e)
        {
            try
            {
                ImageClose.Visible = false;
                Titulo.Text = "Historia Clinica Terapia Ocupacional";
                LogoMain.Image = Properties.Resources.Splash;
                SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";

                Botones();

                repoAgendaMedica.Graba_Hora_Atencion(Admision);
                repoAgendaMedica.OpenAdmition(Admision, "S");

                var DatosAdmision = repoAgendaMedicaConsultas.cargarAdmision(Admision, "'P','H','A'");
                if (DatosAdmision == null)
                {
                    MessageBox.Show("Error en esta admision",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    this.Dispose();
                    this.Close();
                    return;
                }                

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

                var VAl = repoConvenios.ServicioNombre(CUP, Ase, TSERV);
                if (VAl == null)
                {
                    MessageBox.Show("Error grave cargardo valor del servicio, vuelva a ingresar a la admision",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    this.Dispose();
                    this.Close();
                    return;
                }

                Valor = VAl.Con_Valor;
                Serv = VAl.Con_Nombre;

                textBox5.Text = Admision.ToString();
                textBox1.Text = DatosAdmision.Hor_Imp_Age.ToString();
                textBox3.Text = Convert.ToDateTime(DatosAdmision.Pac_FechaNto).ToString(Conexion.ConectionDictionary["Format_Fecha"]);
                textBox2.Text = DatosAdmision.Pac_TipoId.ToString() + " " + DatosAdmision.Pac_IdNum.ToString();

                DateTime nacimiento = Convert.ToDateTime(DatosAdmision.Pac_FechaNto);
                int edad = DateTime.Today.AddTicks(-nacimiento.Ticks).Year - 1;
                textBox4.Text = edad.ToString();

                textBox6.Text = "Terapia Ocupacional";

                ActualizarPAC();

                string getRecos = repoConfSystem.getListado()["Recomendaciones"];
                if (getRecos == "A")
                {
                    Extras.Recomendaciones r = new Extras.Recomendaciones(DatosAdmision.Hor_Pac_Id);
                    r.ShowDialog();

                    CargarOpcionesRecomendaciones();
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
        private void ActualizarPAC()
        {
            Medicina.ActualizarPaciente Historia_Edita_Paciente = new Medicina.ActualizarPaciente(Paciente);
            Historia_Edita_Paciente.ShowDialog();
        }
        void openConditions(string Tipo)
        {
            c = new CondicionesP(Tipo, this.Paciente);
            c.ShowDialog();

            CargarOpcionesRecomendaciones();
        }
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
            Alergias c = new Alergias("ALERGIA", this.Paciente);
            c.ShowDialog();
            CargarOpcionesRecomendaciones();
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
        private void textBox18_DoubleClick(object sender, EventArgs e)
        {
            Medicina.CIE10 C10 = new Medicina.CIE10("HTO");
            C10.ShowDialog();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            textBox18.Text = "";
            textBox17.Text = "";
        }        
    }
}
