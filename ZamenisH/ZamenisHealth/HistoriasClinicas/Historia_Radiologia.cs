using Domain;
using Domain.CXN;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ZamenisHealth.Comunes;
using ZamenisHealth.HistoriasClinicas.Extras;
using static ZamenisHealth.Clases.ConfigForm;

namespace ZamenisHealth.HistoriasClinicas
{
    public partial class Historia_Radiologia : BaseForm
    {
        private static readonly ICondiciones repoCond = new MCondiciones();
        private static readonly IAgenda repoAgendaMedica = new MAgenda();
        private static readonly IAgendaC repoAgendaMedicaConsultas = new MAgendaC();
        private static readonly IConfSystem repoConfSystem = new MConfSystem();
        private static readonly IConvenios repoConvenios = new MConvenios();
        private static readonly IAseguradoras repoAseguradoras = new MAseguradoras();
        private static readonly IMenu repoMenu = new MMenu();
        private static readonly IPacientes repoPacientes = new MPacientes();
        private static readonly ICargos repoCargos = new MCargos();
        private static readonly IRadiologia repoRadio = new MRadiologia();
        private static readonly IRIPS repoRIPS = new MRIPS();
        private static readonly ICIE10 repoCIE10 = new MCIE10();

        private Extras.CondicionesP c;

        private MensajesGeneral MG;
        private int Paciente, Admision, Cia, Ase, Prof, Valor;
        public bool Retoma;
        private DateTime Fecha_Serv;
        private string CUP, TSERV, Reg_RIP, CMANTID, CMANID, Serv, TID, IDNUM;

        public Historia_Radiologia(int admision)
        {
            InitializeComponent();
            Admision = admision;
        }

        private void Historia_Radiologia_Load(object sender, EventArgs e)
        {
            try
            {
                this.Titulo.Visible = false;
                this.ImageClose.Visible = false;

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
                comboBox7.Text = (DatosAdmision.Pac_Sexo == "M" ? "Masculino" : DatosAdmision.Pac_Sexo == "F" ? "Femenino" : "Intersexual");

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

                TID = DatosAdmision.Pac_TipoId.ToString();
                IDNUM = DatosAdmision.Pac_IdNum.ToString();

                DateTime nacimiento = Convert.ToDateTime(DatosAdmision.Pac_FechaNto);
                int edad = DateTime.Today.AddTicks(-nacimiento.Ticks).Year - 1;
                textBox4.Text = edad.ToString();

                var Aseg = repoAseguradoras.getInfoFromAsebyCode(DatosAdmision.Hor_Pac_Ase);
                textBox6.Text = Aseg.Ase_Descripcion.ToString();

                var getCausa = repoMenu.getCausaExterna();
                if (getCausa != null)
                {
                    foreach (var i in getCausa)
                    {
                        comboBox6.Items.Add(i);
                    }
                }

                ActualizarPAC();

                if (Retoma == true)
                {
                    RecuperarIngresado();
                }

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
                MessageBox.Show(ex.Message);
            }           
        }

        private void RecuperarIngresado()
        {
            try
            {
                DateTime Hoy = DateTime.Now;

                CXN_HCRADIOLOGIA getlast = repoRadio.Ingresado(Admision);
                if (getlast != null)
                {
                    label69.Text = "Se ha recuperado la ultima historia con fecha: " + Convert.ToDateTime(getlast.Fecha).ToString("dd-MM-yyyy");
                    textBox7.Text = getlast.MotivoConsulta.ToString();
                    textBox8.Text = getlast.EnfermedadActual.ToString();

                    textBox9.Text = getlast.EvolucionSintomas.ToString();
                    textBox10.Text = getlast.AntecedentesRelevantes.ToString();
                    textBox11.Text = getlast.MedicamentosActuales.ToString();

                    textBox12.Text = getlast.AntecedentesRenales.ToString();
                    textBox13.Text = getlast.AntecedentesCardioVasculares.ToString();
                    textBox14.Text = getlast.Embarazo.ToString();
                    textBox15.Text = getlast.ImplantesMetalicos.ToString();

                    textBox20.Text = getlast.MenorEdad.ToString();
                    textBox21.Text = getlast.Presion.ToString();
                    textBox27.Text = getlast.GlassHow.ToString();
                    textBox22.Text = getlast.Peso.ToString();

                    textBox24.Text = getlast.Talla.ToString();
                    textBox25.Text = getlast.FResp.ToString();
                    textBox34.Text = getlast.IMC.ToString();
                    comboBox4.Text = getlast.RH.ToString();

                    textBox29.Text = getlast.Conciencia.ToString();
                    textBox23.Text = getlast.FCar.ToString();
                    textBox35.Text = getlast.ObservacionExaMedico.ToString();

                    textBox16.Text = getlast.TipoEstudio.ToString();
                    textBox17.Text = getlast.MedioContraste.ToString();
                    textBox18.Text = getlast.ReaccionAdversa.ToString();
                    textBox31.Text = getlast.Tecnica.ToString();

                    textBox19.Text = getlast.Hallazgos.ToString();

                    textBox52.Text = getlast.DX1.ToString();
                    textBox50.Text = getlast.DX2.ToString();
                    textBox48.Text = getlast.DX3.ToString();
                    textBox51.Text = !string.IsNullOrEmpty(getlast.DX1.ToString()) ? repoCIE10.BuscaDX(getlast.DX1.ToString()) : "";
                    textBox49.Text = !string.IsNullOrEmpty(getlast.DX2.ToString()) ? repoCIE10.BuscaDX(getlast.DX2.ToString()) : "";
                    textBox47.Text = !string.IsNullOrEmpty(getlast.DX3.ToString()) ? repoCIE10.BuscaDX(getlast.DX3.ToString()) : "";

                    textBox53.Text = getlast.NotaDX1.ToString();
                    textBox54.Text = getlast.NotaDX2.ToString();
                    textBox55.Text = getlast.NotaDX3.ToString();

                    comboBox2.Text = getlast.ImpDX1.ToString();
                    comboBox3.Text = getlast.ImpDX2.ToString();
                    comboBox5.Text = getlast.ImpDX3.ToString();

                    textBox56.Text = getlast.EstudioComplementario.ToString();
                    textBox57.Text = getlast.ControlSeguimiento.ToString();
                }
                else
                {
                    textBox7.Text = "";
                    textBox8.Text = "";

                    textBox9.Text = "";
                    textBox10.Text = "";
                    textBox11.Text = "";

                    textBox12.Text = "";
                    textBox13.Text = "";
                    textBox14.Text = "";
                    textBox15.Text = "";

                    textBox20.Text = "";
                    textBox21.Text = "";
                    textBox22.Text = "";

                    textBox24.Text = "";
                    textBox25.Text = "";
                    textBox34.Text = "";
                    comboBox4.Text = "";

                    textBox29.Text = "";
                    textBox23.Text = "";
                    textBox35.Text = "";

                    textBox16.Text = "";
                    textBox17.Text = "";
                    textBox18.Text = "";
                    textBox31.Text = "";

                    textBox19.Text = "";

                    textBox52.Text = "";
                    textBox50.Text = "";
                    textBox48.Text = "";
                    textBox51.Text = "";
                    textBox49.Text = "";
                    textBox47.Text = "";

                    textBox53.Text = "";
                    textBox54.Text = "";
                    textBox55.Text = "";

                    comboBox2.Text = "";
                    comboBox3.Text = "";
                    comboBox5.Text = "";

                    textBox56.Text = "";
                    textBox57.Text = "";

                    MessageBox.Show("No se encontro registros ingresados de este paciente",
                        "Paciente Nuevo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        void openConditions(string Tipo)
        {
            c = new CondicionesP(Tipo, this.Paciente);
            c.ShowDialog();

            CargarOpcionesRecomendaciones();
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

        private void toolStripLabel2_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult result = MessageBox.Show("¿Desea guardar esta historia sin egresar? " +
                        "Historia Pendiente",
                        "Zamenis Health",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    CXN_HCRADIOLOGIA cXN_HCRADIOLOGIA = new CXN_HCRADIOLOGIA
                    {
                        PacId = this.Paciente,
                        PacienteNombre = textBox1.Text,
                        PacienteTipoId = TID,
                        PacienteId = IDNUM,
                        MotivoConsulta = textBox7.Text,
                        EnfermedadActual = textBox8.Text,
                        EvolucionSintomas = textBox9.Text,
                        AntecedentesRelevantes = textBox10.Text,
                        MedicamentosActuales = textBox11.Text,
                        AntecedentesRenales = textBox12.Text,
                        AntecedentesCardioVasculares = textBox13.Text,
                        Embarazo = textBox14.Text,
                        ImplantesMetalicos = textBox15.Text,
                        MenorEdad = string.IsNullOrEmpty(textBox20.Text) ? "MISMO PACIENTE" : textBox20.Text,
                        Presion = textBox21.Text,
                        Peso = textBox22.Text,
                        GlassHow = textBox27.Text,
                        Talla = textBox24.Text,
                        FResp = textBox25.Text,
                        FCar = textBox23.Text,
                        RH = comboBox4.Text,
                        Conciencia = textBox29.Text,
                        IMC = textBox34.Text,
                        ObservacionExaMedico = textBox35.Text,
                        TipoEstudio = textBox16.Text,
                        MedioContraste = textBox17.Text,
                        ReaccionAdversa = textBox18.Text,
                        Tecnica = textBox31.Text,
                        Hallazgos = textBox19.Text,
                        DX1 = textBox52.Text,
                        DX2 = textBox50.Text,
                        DX3 = textBox48.Text,
                        NotaDX1 = textBox53.Text,
                        NotaDX2 = textBox54.Text,
                        NotaDX3 = textBox55.Text,
                        CausaExterna = comboBox6.Text,
                        ImpDX1 = comboBox2.Text,
                        ImpDX2 = comboBox3.Text,
                        ImpDX3 = comboBox5.Text,
                        EstudioComplementario = textBox56.Text,
                        ControlSeguimiento = textBox57.Text,
                        Compañia = this.Cia,
                        Aseguradora = this.Ase,
                        Medico = this.Prof,
                        Fecha = DateTime.Now.Date,
                        HCAdm = this.Admision,
                        HCCant = 0
                    };

                    if (Retoma == true) //ACTUALIZA HISTORIA
                    {
                        bool updateHistoria = repoRadio.ActualizaHistoria(cXN_HCRADIOLOGIA);
                        if (updateHistoria != true)
                        {
                            MessageBox.Show("No se logro actualizar la historia clinica, revise los datos",
                                "Incompleto",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Exclamation);
                            return;
                        }
                    }
                    else if (Retoma == false) //INSERTA HISTORIA
                    {
                        bool createHistoria = repoRadio.InsertarHistoria(cXN_HCRADIOLOGIA);
                        if (createHistoria != true)
                        {
                            MessageBox.Show("No se logro grabar la historia clinica, revise los datos",
                               "Incompleto",
                               MessageBoxButtons.OK,
                               MessageBoxIcon.Exclamation);
                            return;
                        }

                        Medicina.AgendaM f2 = Application.OpenForms.OfType<Medicina.AgendaM>().LastOrDefault();
                        f2.Cargar_Agenda();
                    }

                    repoAgendaMedica.ConsumirAdmision(Admision);

                    MessageBox.Show("Se ha grabado la historia pero NO SE HA EGRESADO.  Por favor retome la historia lo " +
                        "mas pronto posible para darle cierre efectivo",
                        "Hecho",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation);

                    this.Dispose();
                    this.Close();
                }               
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void toolStripLabel3_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime Hoy = DateTime.Now;

                CXN_HCRADIOLOGIA getlast = repoRadio.getLastHistory(Paciente, Hoy);
                if (getlast != null)
                {
                    label69.Text = "Se ha recuperado la ultima historia con fecha: " + Convert.ToDateTime(getlast.Fecha).ToString("dd-MM-yyyy");
                    textBox7.Text = getlast.MotivoConsulta.ToString();
                    textBox8.Text = getlast.EnfermedadActual.ToString();

                    textBox9.Text = getlast.EvolucionSintomas.ToString();
                    textBox10.Text = getlast.AntecedentesRelevantes.ToString();
                    textBox11.Text = getlast.MedicamentosActuales.ToString();
                   
                    textBox12.Text = getlast.AntecedentesRenales.ToString();
                    textBox13.Text = getlast.AntecedentesCardioVasculares.ToString();
                    textBox14.Text = getlast.Embarazo.ToString();
                    textBox15.Text = getlast.ImplantesMetalicos.ToString();
                   
                    textBox20.Text = getlast.Presion.ToString();
                    textBox21.Text = getlast.Peso.ToString();
                    textBox22.Text = getlast.GlassHow.ToString();

                    textBox24.Text = getlast.Talla.ToString();
                    textBox25.Text = getlast.FResp.ToString();
                    textBox34.Text = getlast.IMC.ToString();
                    comboBox4.Text = getlast.RH.ToString();

                    textBox29.Text = getlast.Conciencia.ToString();
                    textBox23.Text = getlast.FCar.ToString();                    
                    textBox35.Text = getlast.ObservacionExaMedico.ToString();
                    
                    textBox16.Text = getlast.TipoEstudio.ToString();
                    textBox17.Text = getlast.MedioContraste.ToString();
                    textBox18.Text = getlast.ReaccionAdversa.ToString();
                    textBox31.Text = getlast.Tecnica.ToString();

                    textBox19.Text = getlast.Hallazgos.ToString();
                   
                    textBox52.Text = getlast.DX1.ToString();
                    textBox50.Text = getlast.DX2.ToString();
                    textBox48.Text = getlast.DX3.ToString();
                    textBox51.Text = !string.IsNullOrEmpty(getlast.DX1.ToString()) ? repoCIE10.BuscaDX(getlast.DX1.ToString()) : "" ;
                    textBox49.Text = !string.IsNullOrEmpty(getlast.DX2.ToString()) ? repoCIE10.BuscaDX(getlast.DX2.ToString()) : "";
                    textBox47.Text = !string.IsNullOrEmpty(getlast.DX3.ToString()) ? repoCIE10.BuscaDX(getlast.DX3.ToString()) : "";

                    textBox53.Text = getlast.NotaDX1.ToString();
                    textBox54.Text = getlast.NotaDX2.ToString();
                    textBox55.Text = getlast.NotaDX3.ToString();
                    
                    comboBox2.Text = getlast.ImpDX1.ToString();
                    comboBox3.Text = getlast.ImpDX2.ToString();
                    comboBox5.Text = getlast.ImpDX3.ToString();
                    
                    textBox56.Text = getlast.EstudioComplementario.ToString();
                    textBox57.Text = getlast.ControlSeguimiento.ToString();
                }
                else
                {
                    textBox7.Text = "";
                    textBox8.Text = "";

                    textBox9.Text = "";
                    textBox10.Text = "";
                    textBox11.Text = "";

                    textBox12.Text = "";
                    textBox13.Text = "";
                    textBox14.Text = "";
                    textBox15.Text = "";

                    textBox20.Text = "";
                    textBox21.Text = "";
                    textBox22.Text = "";

                    textBox24.Text = "";
                    textBox25.Text = "";
                    textBox34.Text = "";
                    comboBox4.Text = "";

                    textBox29.Text = "";
                    textBox23.Text = "";
                    textBox35.Text = "";

                    textBox16.Text = "";
                    textBox17.Text = "";
                    textBox18.Text = "";
                    textBox31.Text = "";

                    textBox19.Text = "";

                    textBox52.Text = "";
                    textBox50.Text = "";
                    textBox48.Text = "";
                    textBox51.Text = "";
                    textBox49.Text = "";
                    textBox47.Text = "";

                    textBox53.Text = "";
                    textBox54.Text = "";
                    textBox55.Text = "";

                    comboBox2.Text = "";
                    comboBox3.Text = "";
                    comboBox5.Text = "";

                    textBox56.Text = "";
                    textBox57.Text = "";

                    MessageBox.Show("No se encontro registros ingresados de este paciente",
                        "Paciente Nuevo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation);
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            textBox52.Text = "";
            textBox51.Text = "";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox50.Text = "";
            textBox49.Text = "";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            textBox47.Text = "";
            textBox48.Text = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                double Valor1, Valor2, Res;
                Valor1 = Convert.ToDouble(textBox24.Text);
                Valor2 = Convert.ToDouble(textBox22.Text);

                Res = Valor2 / (Valor1 * Valor1);
                textBox34.Text = Convert.ToDouble(Res).ToString("N2");
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void textBox48_DoubleClick(object sender, EventArgs e)
        {
            Medicina.CIE10 MC = new Medicina.CIE10("HCRADIO3");
            MC.ShowDialog();
        }

        private void textBox50_DoubleClick(object sender, EventArgs e)
        {
            Medicina.CIE10 MC = new Medicina.CIE10("HCRADIO2");
            MC.ShowDialog();
        }

        private void textBox52_DoubleClick(object sender, EventArgs e)
        {
            Medicina.CIE10 MC = new Medicina.CIE10("HCRADIO1");
            MC.ShowDialog();
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

        private void toolStripLabel5_Click(object sender, EventArgs e)
        {
            MessageBox.Show("En desarrollo...");
        }

        private void toolStripLabel4_Click(object sender, EventArgs e)
        {
            Medicina.Historial_Medico_1 RM = new Medicina.Historial_Medico_1();
            RM.ShowDialog();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            ActualizarPAC();
        }

        void ActualizarPAC()
        {
            Medicina.ActualizarPaciente Historia_Edita_Paciente = new Medicina.ActualizarPaciente(Paciente);
            Historia_Edita_Paciente.ShowDialog();
        }

        private void toolStripLabel1_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(textBox7.Text) || string.IsNullOrEmpty(textBox8.Text))
                {
                    MG = new MensajesGeneral();
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "Debe diligenciar Motivo de Consulta y Enfermedad Actual";
                    MG.ShowDialog();
                    return;
                }

                if (comboBox7.Text == "")
                {
                    MG = new MensajesGeneral();
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "Debe seleccionar sexo del paciente";
                    MG.ShowDialog();
                    return;
                }

                if (string.IsNullOrEmpty(textBox9.Text) || string.IsNullOrEmpty(textBox10.Text) || string.IsNullOrEmpty(textBox11.Text))
                {
                    MG = new MensajesGeneral();
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "Debe diligenciar los campos de Antecedentes Clinicos Relevantes";
                    MG.ShowDialog();
                    return;
                }

                if (string.IsNullOrEmpty(textBox12.Text) || string.IsNullOrEmpty(textBox13.Text) || string.IsNullOrEmpty(textBox14.Text) || string.IsNullOrEmpty(textBox15.Text))
                {
                    MG = new MensajesGeneral();
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "Debe diligenciar los campos de Antecedentes Importantes para el Estudio";
                    MG.ShowDialog();
                    return;
                }
                if (string.IsNullOrEmpty(textBox21.Text) || string.IsNullOrEmpty(textBox22.Text) || string.IsNullOrEmpty(textBox27.Text) || string.IsNullOrEmpty(textBox24.Text) ||
                    string.IsNullOrEmpty(textBox25.Text) || string.IsNullOrEmpty(textBox34.Text) || string.IsNullOrEmpty(textBox29.Text) || string.IsNullOrEmpty(textBox23.Text) || 
                    comboBox4.Text == "")
                {
                    MG = new MensajesGeneral();
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "Debe diligenciar los campos de Examen Fisico";
                    MG.ShowDialog();
                    return;
                }
                if (string.IsNullOrEmpty(textBox16.Text) || string.IsNullOrEmpty(textBox17.Text) || string.IsNullOrEmpty(textBox18.Text) || string.IsNullOrEmpty(textBox31.Text))
                {
                    MG = new MensajesGeneral();
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "Debe diligenciar los campos de Detalles de procedimiento radiologico";
                    MG.ShowDialog();
                    return;
                }
                if (string.IsNullOrEmpty(textBox19.Text))
                {
                    MG = new MensajesGeneral();
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "Debe diligenciar los hallazgos";
                    MG.ShowDialog();
                    return;
                }
                if (string.IsNullOrEmpty(textBox51.Text) || string.IsNullOrEmpty(textBox52.Text) || comboBox2.Text == "")
                {
                    MG = new MensajesGeneral();
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "Debe diligenciar al menos el primer diagnostico y la impresion diagnostica";
                    MG.ShowDialog();
                    return;
                }
                if (comboBox6.Text == "")
                {
                    MG = new MensajesGeneral();
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "Debe seleccionar la Causa Externa";
                    MG.ShowDialog();
                    return;
                }
                if (string.IsNullOrEmpty(textBox56.Text) || string.IsNullOrEmpty(textBox57.Text))
                {
                    MG = new MensajesGeneral();
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "Debe diligenciar las recomendaciones";
                    MG.ShowDialog();
                    return;
                }

                CXN_HCRADIOLOGIA cXN_HCRADIOLOGIA = new CXN_HCRADIOLOGIA
                {
                    PacId = this.Paciente,
                    PacienteNombre = textBox1.Text,
                    PacienteTipoId = TID,
                    PacienteId = IDNUM,
                    MotivoConsulta = textBox7.Text,
                    EnfermedadActual = textBox8.Text,
                    EvolucionSintomas = textBox9.Text,
                    AntecedentesRelevantes = textBox10.Text,
                    MedicamentosActuales = textBox11.Text,
                    AntecedentesRenales = textBox12.Text,
                    AntecedentesCardioVasculares = textBox13.Text,
                    Embarazo = textBox14.Text,
                    ImplantesMetalicos = textBox15.Text,
                    MenorEdad = string.IsNullOrEmpty(textBox20.Text) ? "MISMO PACIENTE" : textBox20.Text,
                    Presion = textBox21.Text,
                    Peso = textBox22.Text,
                    GlassHow = textBox27.Text,
                    Talla = textBox24.Text,
                    FResp = textBox25.Text,
                    FCar = textBox23.Text,
                    RH = comboBox4.Text,
                    Conciencia = textBox29.Text,
                    IMC = textBox34.Text,
                    ObservacionExaMedico = textBox35.Text,
                    TipoEstudio = textBox16.Text,
                    MedioContraste = textBox17.Text,
                    ReaccionAdversa = textBox18.Text,
                    Tecnica = textBox31.Text,
                    Hallazgos = textBox19.Text,
                    DX1 = textBox52.Text,
                    DX2 = textBox50.Text,
                    DX3 = textBox48.Text,
                    NotaDX1 = textBox53.Text,
                    NotaDX2 = textBox54.Text,
                    NotaDX3 = textBox55.Text,
                    CausaExterna = comboBox6.Text,
                    ImpDX1 = comboBox2.Text,
                    ImpDX2 = comboBox3.Text,
                    ImpDX3 = comboBox5.Text,
                    EstudioComplementario = textBox56.Text,
                    ControlSeguimiento = textBox57.Text,
                    Compañia = this.Cia,
                    Aseguradora = this.Ase,
                    Medico = this.Prof,
                    Fecha = DateTime.Now.Date,
                    HCAdm = this.Admision,
                    HCCant = 1
                };

                repoPacientes.SexAndDate(Paciente, (comboBox7.Text == "Masculino" ? "M" : comboBox7.Text == "Femenino" ? "F" : "I"), Convert.ToDateTime(textBox3.Text));
              
                if (Retoma == true) //ACTUALIZA HISTORIA
                {
                    bool updateHistoria = repoRadio.ActualizaHistoria(cXN_HCRADIOLOGIA);
                    if (updateHistoria != true)
                    {
                        MessageBox.Show("No se logro actualizar la historia clinica, revise los datos",
                            "Incompleto",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Exclamation);
                        return;
                    }
                }
                else 
                {
                    bool createHistoria = repoRadio.InsertarHistoria(cXN_HCRADIOLOGIA);
                    if (createHistoria != true)
                    {
                        MessageBox.Show("No se logro grabar la historia clinica, revise los datos",
                           "Incompleto",
                           MessageBoxButtons.OK,
                           MessageBoxIcon.Exclamation);
                        return;
                    }

                    Medicina.AgendaM f2 = Application.OpenForms.OfType<Medicina.AgendaM>().LastOrDefault();
                    f2.Cargar_Agenda();
                }

                var impresion = repoRIPS.TipoRipCargo(comboBox2.Text, "IMPDX");

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
                    Car_Detalle = textBox52.Text,
                    Car_Item = Serv,
                    Car_Dx1 = textBox52.Text,
                    Car_Dx2 = textBox50.Text,
                    Car_Dx3 = textBox48.Text,
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

                Medicina.RIPSHistory for_RIPS = new Medicina.RIPSHistory();
                for_RIPS.Adm_Cargo = Convert.ToInt32(Admision);
                for_RIPS.ShowDialog();

                repoAgendaMedica.ConsumirAdmision(Admision);
                repoAgendaMedica.Graba_Hora_Salida(Admision);

                this.Dispose();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void toolStripLabel6_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult result = MessageBox.Show("¿Desea cancelar esta historia? Al hacerlo no guardara ningun dato", "Zamenis_Health", MessageBoxButtons.YesNo);

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

        public void setDX(string cieCod, string cieServ, string Pos)
        {
            if (Pos == "1")
            {
                textBox52.Text = cieCod.Trim();
                textBox51.Text = cieServ.Trim();
            }
            else if (Pos == "2")
            {
                textBox50.Text = cieCod.Trim();
                textBox49.Text = cieServ.Trim();
            }
            else if (Pos == "3")
            {
                textBox48.Text = cieCod.Trim();
                textBox47.Text = cieServ.Trim();
            }
            else
            {
                MessageBox.Show("Posision Invalida");
            }            
        }
    }
}
