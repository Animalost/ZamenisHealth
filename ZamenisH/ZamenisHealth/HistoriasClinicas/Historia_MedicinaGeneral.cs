using APIFhir.Controlador;
using APIFhir.Servicio;
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
using ZamenisHealth.Clases;
using ZamenisHealth.Comunes;
using ZamenisHealth.FrontFHIR.RDAs;
using ZamenisHealth.HistoriasClinicas.Extras;
using ZamenisHealth.Medicina.OrdenesHistory;

namespace ZamenisHealth.HistoriasClinicas
{
    public partial class Historia_MedicinaGeneral : Forma
    {
        private static readonly IAgendaC repoAgendaMedicaConsultas = new MAgendaC();
        private static readonly IPacientes repoPacientes = new MPacientes();
        private static readonly IAgenda repositorioAgendaMedica = new MAgenda();
        private static readonly INotasCuracion repositorioNotasCuracion = new MNotasCuracion();
        private static readonly IMedicinaGeneral repositorioMedicinaGeneral = new MMedicinaGeneral();
        private static readonly IConvenios repositorioConvenios = new MConvenios();
        private static readonly ICargos repositorioCargos = new MCargos();
        private static readonly IRIPS repositorioRIPS = new MRIPS();
        private static readonly IMenu repositorioNMenu = new MMenu();
        private static readonly ICIE10 repositorioCIE10 = new MCIE10();
        private static readonly IPacientes repoPacs = new MPacientes();
        private static readonly IConfSystem repoConfSystem = new MConfSystem();
        private static readonly ICondiciones repoCond = new MCondiciones();
        private static readonly IAntecedentesGlobales repoAntGen = new MAntecedentesGlobales();
        private static readonly IFHIR repoFHIR = new MFHIR();
        private static readonly CreateToken repoToken = new EndPoint_Token();

        private CondicionesP c;
        public int Admision,edad;
        public bool Retoma;
        DateTime Fecha_Serv;
        int Paciente, Cia, Ase, Prof, Valor, ValorCatalogo;
        private string INGSAL = "";
        string CUP, TSERV, Reg_RIP, CMANTID, CMANID, Serv, UrlEvento;

        private ToolStripButton btnDatosPaciente, btnGuardarEgreso, btnGuardarNoEgreso,
                                btnVerHistorias, btnVerIHCE, btnVerImagenes, btnCrearOrdenes,
                                btnTraerUltimo, btnMensajero, btnFormatos, btnCotizaciones,
                                btnMedidasEnfermeria, btnAprobarSolictudes, btnCancelar;

        DataTable dt, dt2, dt3, dt4;
        DataColumn POS, POS2, POS3, POS4;
        DataColumn Id, Id2, Id3, Id4;
        DataColumn Parentesco, Parentesco2, Parentesco3, Alergia4;
        DataColumn Codigo, Codigo2, Codigo3, Codigo4;
        DataColumn Descripcion, Descripcion2, Descripcion3, Observacion4;

        private MensajesGeneral MG;

        public Historia_MedicinaGeneral()
        {
            InitializeComponent();  
        }
        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            Comunes.MensajeroSend MS = new Comunes.MensajeroSend();
            MS.ShowDialog();
        }
        private void CargarOpcionesBase()
        {
            try
            {
                var getMenus = repositorioNMenu.getMenus();
                if (getMenus != null)
                {
                    comboBox11.Items.Add("--- Sin Seleccion ---");
                    comboBox10.Items.Add("--- Sin Seleccion ---");
                    comboBox9.Items.Add("--- Sin Seleccion ---");
                    comboBox8.Items.Add("--- Sin Seleccion ---");
                    comboBox7.Items.Add("--- Sin Seleccion ---");
                    comboBox5.Items.Add("--- Sin Seleccion ---");

                    comboBox12.Items.Add("--- Sin Seleccion ---");

                    comboBox20.Items.Add("--- Sin Seleccion ---");
                    comboBox19.Items.Add("--- Sin Seleccion ---");
                    comboBox18.Items.Add("--- Sin Seleccion ---");
                    comboBox17.Items.Add("--- Sin Seleccion ---");
                    comboBox16.Items.Add("--- Sin Seleccion ---");
                    comboBox15.Items.Add("--- Sin Seleccion ---");
                    comboBox13.Items.Add("--- Sin Seleccion ---");
                    comboBox14.Items.Add("--- Sin Seleccion ---");

                    comboBox23.Items.Add("--- Sin Seleccion ---");

                    foreach (var i1 in getMenus) 
                    { 
                        if (!string.IsNullOrEmpty(i1.Grado))
                        {
                            comboBox11.Items.Add(i1.Grado.ToString());
                        }                        
                    }
                    foreach (var i2 in getMenus) 
                    {
                        if (!string.IsNullOrEmpty(i2.TipoLesion))
                        {
                            comboBox10.Items.Add(i2.TipoLesion.ToString());
                        }                        
                    }
                    foreach (var i3 in getMenus) 
                    {
                        if (!string.IsNullOrEmpty (i3.Emocional))
                        {
                            comboBox9.Items.Add(i3.Emocional.ToString());
                        }                        
                    }
                    foreach (var i4 in getMenus) 
                    {
                        if (!string.IsNullOrEmpty(i4.ActEjer))
                        {
                            comboBox8.Items.Add(i4.ActEjer.ToString());
                        }                        
                    }
                    foreach (var i5 in getMenus) 
                    {
                        if (!string.IsNullOrEmpty(i5.Vez))
                        {
                            comboBox7.Items.Add(i5.Vez.ToString());
                        }                        
                    }
                    foreach (var i6 in getMenus) 
                    {
                        if (!string.IsNullOrEmpty(i6.Apariencia))
                        {
                            comboBox5.Items.Add(i6.Apariencia.ToString());
                        }                        
                    }
                    foreach (var i7 in getMenus) 
                    {
                        if (!string.IsNullOrEmpty(i7.Nutricional))
                        {
                            comboBox12.Items.Add(i7.Nutricional.ToString());
                        }                        
                    }
                    foreach (var i8 in getMenus) 
                    {
                        if (!string.IsNullOrEmpty(i8.Tej_Com))
                        {
                            comboBox20.Items.Add(i8.Tej_Com.ToString());
                        }                        
                    }
                    foreach (var i9 in getMenus) 
                    {
                        if (!string.IsNullOrEmpty(i9.Carac_Tej))
                        {
                            comboBox19.Items.Add(i9.Carac_Tej.ToString());
                        }                        
                    }
                    foreach (var i10 in getMenus) 
                    {
                        if (!string.IsNullOrEmpty(i10.Sig_Inf))
                        {
                            comboBox18.Items.Add(i10.Sig_Inf.ToString());
                        }                        
                    }
                    foreach (var i11 in getMenus) 
                    {
                        if (!string.IsNullOrEmpty(i11.Exudado))
                        {
                            comboBox16.Items.Add(i11.Exudado.ToString());
                        }                        
                    }
                    foreach (var i12 in getMenus) 
                    {
                        if (!string.IsNullOrEmpty(i12.ConsCant))
                        {
                            comboBox15.Items.Add(i12.ConsCant.ToString());
                        }                        
                    }
                    foreach (var i13 in getMenus) 
                    {
                        if (!string.IsNullOrEmpty(i13.Estado))
                        {
                            comboBox13.Items.Add(i13.Estado.ToString());
                        }                        
                    }
                    foreach (var i14 in getMenus) 
                    {
                        if (!string.IsNullOrEmpty(i14.Dolor))
                        {
                            comboBox14.Items.Add(i14.Dolor.ToString());
                        }
                    }
                    foreach (var i15 in getMenus) 
                    {
                        if (!string.IsNullOrEmpty(i15.Patologia))
                        {
                            comboBox23.Items.Add(i15.Patologia.ToString());
                        }                        
                    }
                    foreach (var i17 in getMenus) 
                    {
                        if (!string.IsNullOrEmpty(i17.Piel_Circ))
                        {
                            comboBox17.Items.Add(i17.Piel_Circ.ToString());
                        }                        
                    }

                    comboBox5.Text = "--- Sin Seleccion ---";

                    comboBox12.Text = "--- Sin Seleccion ---";

                    comboBox20.Text = "--- Sin Seleccion ---";
                    comboBox19.Text = "--- Sin Seleccion ---";
                    comboBox18.Text = "--- Sin Seleccion ---";
                    comboBox17.Text = "--- Sin Seleccion ---";
                    comboBox16.Text = "--- Sin Seleccion ---";
                    comboBox15.Text = "--- Sin Seleccion ---";
                    comboBox13.Text = "--- Sin Seleccion ---";
                    comboBox14.Text = "--- Sin Seleccion ---";

                    comboBox23.Text = "--- Sin Seleccion ---";

                }
                else
                {
                    MessageBox.Show("Inconveniente en listado de menus", "Error desconocido", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Dispose();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void toolStripLabel5_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox39.Text) || string.IsNullOrEmpty(textBox38.Text))
            {
                MG = new MensajesGeneral()
                {
                    Mensaje = "Debe ingresar al menos un diagnostico principal para poder generar ordenes medicas",
                    TipoImagen = 1000
                };
                MG.ShowDialog();
                return;
            }

            TipoOrdenMedica tipoOrdenMedica = new TipoOrdenMedica("MEDGEN", Admision);
            tipoOrdenMedica.ShowDialog();
        }
        private void comboBox23_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (comboBox23.Text == "PosOperatorio")
                {
                    var getPP = repositorioNMenu.getPosPatologia();
                    if (getPP != null)
                    {
                        comboBox22.Visible = true;
                        label67.Visible = true;
                        comboBox22.DataSource = null;
                        comboBox22.Items.Clear();
                        comboBox22.Items.Add("");

                        comboBox22.Parent = panel1;
                        //comboBox22.Location = new Point(389, 2640);

                        foreach (var i in getPP)
                        {
                            comboBox22.Items.Add(i);
                        }

                        comboBox22.Text = "";
                    }
                    else
                    {
                        comboBox22.Items.Add("Sin datos relacionados");
                    }
                }
                else
                {
                    if (comboBox23.Text == "Vasculares")
                    {
                        var getPP = repositorioNMenu.getPosPatologia2();
                        if (getPP != null)
                        {
                            comboBox22.Visible = true;
                            label67.Visible = true;
                            comboBox22.DataSource = null;
                            comboBox22.Items.Clear();
                            comboBox22.Items.Add("");

                            comboBox22.Parent = panel1;
                            //comboBox22.Location = new Point(389, 2640);

                            foreach (var i in getPP)
                            {
                                comboBox22.Items.Add(i);
                            }

                            comboBox22.Text = "";
                        }
                        else
                        {
                            comboBox22.Items.Add("Sin datos relacionados");
                        }
                    }
                    else
                    {
                        comboBox22.Visible = false;
                        comboBox22.Text = "";
                        label67.Visible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void button11_Click(object sender, EventArgs e)
        {
            textBox35.Text = "";
            textBox34.Text = "";
        }
        private void button5_Click(object sender, EventArgs e)
        {
            textBox10.Text = "";
        }
        private void toolStripLabel7_Click(object sender, EventArgs e)
        {
            Medicina.VerImagenes VI = new Medicina.VerImagenes();
            VI.ShowDialog();
        }
        private void toolStripLabel3_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime f = DateTime.Now.Date;

                var getTemporal = repositorioMedicinaGeneral.getLastHistory(Paciente, f);
                if (getTemporal != null)
                {
                    textBox7.Text = getTemporal.HC_MotivoC.ToString();
                    textBox8.Text = getTemporal.HC_EnfA.ToString();
                    textBox9.Text = getTemporal.HC_PruebasDiag.ToString();

                    textBox18.Text = getTemporal.HC_Neurologico.ToString();
                    textBox17.Text = getTemporal.HC_Respiratorio.ToString();
                    textBox16.Text = getTemporal.HC_Cardiovascular.ToString();
                    textBox15.Text = getTemporal.HC_GastroIntestinal.ToString();
                    textBox14.Text = getTemporal.HC_GastroUrinario.ToString();
                    //textBox11.Text = getTemporal.HC_Ocupacion.ToString();
                    textBox12.Text = getTemporal.HC_Piel.ToString();
                    textBox13.Text = getTemporal.HC_OsteoMuscular.ToString();

                    textBox24.Text = getTemporal.HC_AntFam.ToString();
                    textBox23.Text = getTemporal.HC_AntPat.ToString();
                    textBox22.Text = getTemporal.HC_AntFarma.ToString();
                    textBox21.Text = getTemporal.HC_AntQui.ToString();
                    textBox20.Text = getTemporal.HC_AntAle.ToString();
                    textBox19.Text = getTemporal.HC_Hematolin.ToString();

                    comboBox12.Text = getTemporal.HC_EstadoNut.ToString();
                    textBox32.Text = getTemporal.HC_Presart.ToString();
                    textBox31.Text = getTemporal.HC_Frecar.ToString();
                    textBox29.Text = getTemporal.HC_Temp.ToString();
                    textBox30.Text = getTemporal.HC_FreRes.ToString();
                    textBox28.Text = getTemporal.HC_Peso.ToString();
                    textBox27.Text = getTemporal.HC_Altura.ToString();
                    textBox25.Text = getTemporal.HC_IMC.ToString();
                    textBox26.Text = getTemporal.HC_ITB.ToString();

                    comboBox20.Text = getTemporal.HC_TejCom.ToString();
                    comboBox19.Text = getTemporal.HC_CaracTej.ToString();
                    comboBox18.Text = getTemporal.HC_SignosInf.ToString();
                    comboBox17.Text = getTemporal.HC_PielCirc.ToString();
                    comboBox16.Text = getTemporal.HC_Exudado.ToString();
                    comboBox15.Text = getTemporal.HC_ConsCant.ToString();
                    comboBox14.Text = getTemporal.HC_Estado.ToString();
                    comboBox13.Text = getTemporal.HC_Dolor.ToString();
                    textBox33.Text = getTemporal.HC_DescHer.ToString();
                    label62.Text = getTemporal.HC_TejCom.ToString();
                    label60.Text = getTemporal.HC_CaracTej.ToString();
                    label58.Text = getTemporal.HC_SignosInf.ToString();
                    label56.Text = getTemporal.HC_PielCirc.ToString();

                    comboBox24.Text = getTemporal.HC_Imp_Dx.ToString();
                    comboBox23.Text = getTemporal.HC_Patologia.ToString();
                    comboBox21.Text = getTemporal.HC_RH.ToString();
                    textBox38.Text = getTemporal.HC_DX1T.ToString();
                    textBox42.Text = getTemporal.HC_Analisis.ToString();
                    textBox41.Text = getTemporal.HC_PManejo.ToString();
                    textBox40.Text = getTemporal.HC_Complicacion.ToString();
                    textBox39.Text = getTemporal.HC_DX1.ToString();
                    textBox37.Text = getTemporal.HC_DX2.ToString();
                    textBox35.Text = getTemporal.HC_DX3.ToString();

                    var DX = repositorioCIE10.BuscaDX(textBox39.Text);
                    textBox38.Text = DX.ToString();
                    DX = repositorioCIE10.BuscaDX(textBox37.Text);
                    textBox36.Text = DX.ToString();
                    DX = repositorioCIE10.BuscaDX(textBox35.Text);
                    textBox34.Text = DX.ToString();
                }
                else
                {
                    textBox7.Text = "";
                    textBox8.Text = "";
                    textBox9.Text = "";

                    textBox18.Text = "";
                    textBox17.Text = "";
                    textBox16.Text = "";
                    textBox15.Text = "";
                    textBox14.Text = "";
                    //textBox11.Text = "";
                    textBox12.Text = "";
                    textBox13.Text = "";

                    textBox24.Text = "";
                    textBox23.Text = "";
                    textBox22.Text = "";
                    textBox21.Text = "";
                    textBox20.Text = "";
                    textBox19.Text = "";

                    textBox32.Text = "";
                    textBox31.Text = "";
                    textBox29.Text = "";
                    textBox30.Text = "";
                    textBox28.Text = "";
                    textBox27.Text = "";
                    textBox25.Text = "";
                    textBox26.Text = "";

                    comboBox20.Text = "";
                    comboBox19.Text = "";
                    comboBox18.Text = "";
                    comboBox17.Text = "";
                    comboBox16.Text = "";
                    comboBox15.Text = "";
                    comboBox14.Text = "";
                    comboBox13.Text = "";
                    label62.Text = "";
                    label60.Text = "";
                    label58.Text = "";
                    label56.Text = "";

                    comboBox24.Text = "";
                    comboBox23.Text = "";
                    comboBox22.Text = "";
                    comboBox21.Text = "";
                    textBox38.Text = "";
                    textBox42.Text = "";
                    textBox41.Text = "";
                    textBox40.Text = "";
                    textBox39.Text = "";
                    textBox37.Text = "";
                    textBox35.Text = "";

                    MessageBox.Show("No se encontro registros ingresados de este paciente correspondientes a esta admision",
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
        private void toolStripLabel1_Click(object sender, EventArgs e)
        {
            MostrarPanel2Servicios();            
        }
        private async void GrabarHistoria() 
        {
            try
            {
                if (textBox7.Text == "") { MessageBox.Show("No hay motivo de consulta"); return; }
                if (textBox8.Text == "") { MessageBox.Show("No hay enfermedad actual"); return; }
                if (textBox10.Text == "") { MessageBox.Show("Diligencie informacion de control epidemiologico"); return; }
                if (textBox9.Text == "") { MessageBox.Show("Diligencie pruebas diagnosticas complementarias"); return; }
                                            
                if (comboBox11.Text == "") { MessageBox.Show("No hay grado de cuidado"); return; }
                if (comboBox10.Text == "") { MessageBox.Show("No hay tipo de lesion"); return; }
                if (comboBox8.Text == "") { MessageBox.Show("No hay actividad y ejercicio"); return; }
                if (comboBox7.Text == "") { MessageBox.Show("No hay vez registrada"); return; }
                if (comboBox9.Text == "") { MessageBox.Show("No hay estado emocional"); return; }
                if (comboBox5.Text == "") { MessageBox.Show("No hay apariencia fisica"); return; }
                if (comboBox11.Text == "--- Sin Seleccion ---") { MessageBox.Show("No hay grado de cuidado"); return; }
                if (comboBox10.Text == "--- Sin Seleccion ---") { MessageBox.Show("No hay tipo de lesion"); return; }
                if (comboBox8.Text == "--- Sin Seleccion ---") { MessageBox.Show("No hay actividad y ejercicio"); return; }
                if (comboBox7.Text == "--- Sin Seleccion ---") { MessageBox.Show("No hay vez registrada"); return; }
                if (comboBox9.Text == "--- Sin Seleccion ---") { MessageBox.Show("No hay estado emocional"); return; }
                if (comboBox5.Text == "--- Sin Seleccion ---") { MessageBox.Show("No hay apariencia fisica"); return; }

                if (textBox18.Text == "") { MessageBox.Show("No hay registro neurologico ingresado"); return; }
                if (textBox17.Text == "") { MessageBox.Show("No hay registro respiratorio ingresado"); return; }
                if (textBox16.Text == "") { MessageBox.Show("No hay registro cardiovascular ingresado"); return; }
                if (textBox15.Text == "") { MessageBox.Show("No hay registro gastrointestinal ingresado"); return; }
                if (textBox14.Text == "") { MessageBox.Show("No hay registro gastrourinario ingresado"); return; }
                if (textBox13.Text == "") { MessageBox.Show("No hay registro osteomuscular ingresado"); return; }
                if (textBox12.Text == "") { MessageBox.Show("No hay registro de piel ingresado"); return; }
                if (textBox11.Text == "") { MessageBox.Show("No hay registro de ocupacion ingresado"); return; }

                if (textBox24.Text == "") { MessageBox.Show("No hay registro de antecedentes"); return; }
                if (textBox23.Text == "") { MessageBox.Show("No hay registro de antecedentes"); return; }
                if (textBox22.Text == "") { MessageBox.Show("No hay registro de antecedentes"); return; }
                if (textBox21.Text == "") { MessageBox.Show("No hay registro de antecedentes"); return; }
                if (textBox20.Text == "") { MessageBox.Show("No hay registro de antecedentes"); return; }
                if (textBox19.Text == "") { MessageBox.Show("No hay registro de antecedentes"); return; }

                if (textBox32.Text == "") { MessageBox.Show("No hay registro de revision a sistemas ingresado"); return; }
                if (textBox31.Text == "") { MessageBox.Show("No hay registro de revision a sistemas ingresado"); return; }
                if (textBox30.Text == "") { MessageBox.Show("No hay registro de revision a sistemas ingresado"); return; }
                if (textBox29.Text == "") { MessageBox.Show("No hay registro de revision a sistemas ingresado"); return; }
                if (textBox28.Text == "") { MessageBox.Show("No hay registro de revision a sistemas ingresado"); return; }
                if (textBox27.Text == "") { MessageBox.Show("No hay registro de revision a sistemas ingresado"); return; }
                if (textBox25.Text == "") { MessageBox.Show("No hay registro de revision a sistemas ingresado"); return; }
                if (textBox26.Text == "") { MessageBox.Show("No hay registro de revision a sistemas ingresado"); return; }
                if (comboBox12.Text == "") { MessageBox.Show("No hay registro estado nutricional ingresado"); return; }
                if (comboBox12.Text == "--- Sin Seleccion ---") { MessageBox.Show("No hay registro estado nutricional ingresado"); return; }

                if (textBox33.Text == "") { MessageBox.Show("No hay registro de descripcion de heridas ingresado"); return; }
                if (label62.Text == "") { MessageBox.Show("No hay registro de tejidos comprometidos ingresado"); return; }
                if (label60.Text == "") { MessageBox.Show("No hay registro de caracteristicas de heridas ingresado"); return; }
                if (label58.Text == "") { MessageBox.Show("No hay registro de signosde infeccion de heridas ingresado"); return; }
                if (label56.Text == "") { MessageBox.Show("No hay registro de piel circundante ingresado"); return; }

                if (comboBox15.Text == "") { MessageBox.Show("No hay registro de consistencia y cantidad de heridas ingresado"); return; }
                if (comboBox14.Text == "") { MessageBox.Show("No hay registro de estado de heridas ingresado"); return; }
                if (comboBox13.Text == "") { MessageBox.Show("No hay registro de estado de dolor de heridas ingresado"); return; }
                if (comboBox16.Text == "") { MessageBox.Show("No hay registro de exudado ingresado"); return; }
                if (comboBox15.Text == "--- Sin Seleccion ---") { MessageBox.Show("No hay registro de consistencia y cantidad de heridas ingresado"); return; }
                if (comboBox14.Text == "--- Sin Seleccion ---") { MessageBox.Show("No hay registro de estado de heridas ingresado"); return; }
                if (comboBox13.Text == "--- Sin Seleccion ---") { MessageBox.Show("No hay registro de estado de dolor de heridas ingresado"); return; }
                if (comboBox16.Text == "--- Sin Seleccion ---") { MessageBox.Show("No hay registro de exudado ingresado"); return; }

                if (textBox42.Text == "") { MessageBox.Show("No hay registro de analisis ingresado"); return; }
                if (textBox40.Text == "") { MessageBox.Show("No hay registro de complicacion ingresado"); return; }
                if (textBox41.Text == "") { MessageBox.Show("No hay registro de plan de manejo ingresado"); return; }

                if (textBox39.Text == "") { MessageBox.Show("No hay registro de diagnosticos principales ingresado"); return; }
                if (textBox38.Text == "") { MessageBox.Show("No hay registro de diagnostico principal ingresado"); return; }

                if (comboBox24.Text == "") { MessageBox.Show("No hay registro de impresiones diagnosticas registradas"); return; }
                if (comboBox23.Text == "") { MessageBox.Show("No hay registro de patologias"); return; }
                if (comboBox21.Text == "") { MessageBox.Show("No hay registro de RH"); return; }
                if (comboBox24.Text == "--- Sin Seleccion ---") { MessageBox.Show("No hay registro de impresiones diagnosticas registradas"); return; }
                if (comboBox23.Text == "--- Sin Seleccion ---") { MessageBox.Show("No hay registro de patologias"); return; }
                if (comboBox21.Text == "--- Sin Seleccion ---") { MessageBox.Show("No hay registro de RH"); return; }

                if (comboBox25.Text == "") { MessageBox.Show("Seleccione el tipo de cita en la parte superior de la historia"); return; }

                if (comboBox1.SelectedIndex == 3) { MessageBox.Show("Seleccione el sexo del paciente"); return; }
                if (dateTimePicker1.Value.Date == DateTime.Now.Date) { MessageBox.Show("Seleccione la fecha de nacimiento del paciente"); return; }

                if (textBox24.Text != "SIN ANTECEDENTES".Trim())
                {
                    List<CXN_ANTECEDENTESFAMILIARES> getAntFam = repoAntGen.ObtenerAntecedentes(Paciente);
                    if (getAntFam == null)
                    {
                        MG = new MensajesGeneral()
                        {
                            Mensaje = "En la seccion de Antecedentes Familiares ha puesto que si tiene algunos antecedentes pero no le ha registrado " +
                            "los codigos CUP relacionados en la misma seccion, debe ingresar el antecedente de manera obligatoria de acuerdo a la " +
                            "resolucion 1888 de 2025 Historia Clinica Electronica en Colombia - IHCE.  1 CUP por cada antecedente, no omita ninguno.",
                            TipoImagen = 1000
                        };

                        MG.ShowDialog();

                        return;
                    }
                }

                DialogResult result = MessageBox.Show("¿Desea Egresar esta historia?, Si debe generar ordenes medicas de cualquier tipo " +
                    "incluyendo medicamentos, estan deben ser generadas antes de egresar la historia" +
                        "Egresar Atencion",
                        "Zamenis Health",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {                   
                    CXN_HCMG H = new CXN_HCMG
                    {
                        HC_Pac = textBox1.Text,
                        HC_Prof = Prof,
                        HC_Ase = Ase,
                        HC_Com = Cia,
                        HC_Pacid = Paciente,
                        HC_Edad = edad.ToString(),
                        HC_FechaNto = Convert.ToDateTime(dateTimePicker1.Value.Date),
                        HC_MotivoC = textBox7.Text,
                        HC_EnfA = textBox8.Text,
                        HC_GradoC = comboBox11.Text,
                        HC_TipoLes = comboBox10.Text,
                        HC_ActEje = comboBox8.Text,
                        HC_Vez = comboBox25.Text,
                        HC_Neurologico = textBox18.Text,
                        HC_Cardiovascular = textBox16.Text,
                        HC_GastroIntestinal = textBox15.Text,
                        HC_GastroUrinario = textBox14.Text,
                        HC_OsteoMuscular = textBox13.Text,
                        HC_Piel = textBox12.Text,
                        HC_Ocupacion = textBox11.Text,
                        HC_AparienciaG = comboBox5.Text,
                        HC_EstadoEmo = comboBox9.Text,
                        HC_EstadoNut = comboBox12.Text,
                        HC_Exudado = comboBox16.Text,
                        HC_Presart = textBox32.Text,
                        HC_Frecar = textBox31.Text,
                        HC_FreRes = textBox30.Text,
                        HC_Temp = textBox29.Text,
                        HC_Peso = textBox28.Text,
                        HC_Altura = textBox27.Text,
                        HC_IMC = textBox25.Text,
                        HC_ITB = textBox26.Text,
                        HC_DescHer = textBox33.Text,
                        HC_TejCom = label62.Text,
                        HC_CaracTej = label60.Text,
                        HC_SignosInf = label58.Text,
                        HC_PielCirc = label56.Text,
                        HC_ConsCant = comboBox15.Text,
                        HC_Estado = comboBox13.Text,
                        HC_Dolor = comboBox14.Text,
                        HC_Analisis = textBox42.Text,
                        HC_Complicacion = textBox40.Text,
                        HC_PruebasDiag = textBox9.Text,
                        HC_ProtoInst = "N/A",
                        HC_PManejo = textBox41.Text,
                        HC_DX1 = textBox39.Text,
                        HC_DX2 = textBox37.Text,
                        HC_DX3 = textBox35.Text,
                        HC_DX1T = textBox38.Text,
                        HC_AntFam = textBox24.Text,
                        HC_AntPat = textBox23.Text,
                        HC_AntQui = textBox21.Text,
                        HC_AntAle = textBox20.Text,
                        HC_AntFarma = textBox22.Text,
                        HC_Hematolin = textBox19.Text,
                        HC_Patologia = comboBox23.Text,
                        HC_Fecha = Fecha_Serv,
                        HC_Cant = 1,
                        HC_RH = comboBox21.Text,
                        HC_SubPat = comboBox22.Text,
                        HC_Imp_Dx = comboBox24.Text,
                        HC_Respiratorio = textBox17.Text,
                        HC_Epidemia = textBox10.Text,
                        HC_ServCatalogo = textBox51.Text,
                        HC_CupCatalogo = textBox50.Text,
                        HC_Adm = Admision,
                        HC_TipoINGSAL = INGSAL.ToString()
                    };

                    if (Retoma == true) //ACTUALIZA HISTORIA
                    {
                        var Actualiza = repositorioMedicinaGeneral.ActualizaHCMG(H);
                        if (Actualiza != true)
                        {
                            MessageBox.Show("No se logro actualizar la historia clinica, revise los datos",
                                "Incompleto",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Exclamation);
                            return;
                        }
                    }

                    if (Retoma == false) //INSERTA HISTORIA
                    {
                        var Graba = repositorioMedicinaGeneral.GrabaHCMG(H);
                        if (Graba != true)
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

                    var impresion = repositorioRIPS.TipoRipCargo(comboBox24.Text, "IMPDX");

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
                        Car_Detalle = textBox39.Text,
                        Car_Item = Serv,
                        Car_Dx1 = textBox39.Text,
                        Car_Dx2 = textBox37.Text,
                        Car_Dx3 = textBox35.Text,
                        Car_Ambito = 0,
                        Car_Personal = 0,
                        Car_CExterna = 0,
                        Car_Finalidad = 0,
                        Car_Finalidad_CO = 0, //motivo                                           
                        Car_Imp_Dx = impresion,
                        Car_Regimen = Reg_RIP
                    };

                    bool insertarCargo = repositorioCargos.InsertarCargoHistorias(C);
                    if (insertarCargo == false)
                    {
                        MessageBox.Show("No se logro guardar el cargo economico en el registro de valores a cobrar en la factura, " +
                            "su historia quedo resgitrada pero reporte este incidente a la recepcion con la admision: " + Admision,
                            "Advertencia!!!",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }

                    repositorioAgendaMedica.ConsumirAdmision(Admision);


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
                    if (checkBox3.Checked == true)
                    {
                        repoPacs.PacEspecial(Paciente, "S");
                    }
                    if (checkBox3.Checked == false)
                    {
                        repoPacs.PacEspecial(Paciente, "N");
                    }

                    repoPacs.SexAndDate(Paciente, (comboBox1.SelectedIndex == 0 ? "M" : comboBox1.SelectedIndex == 1 ? "F" : "I"), dateTimePicker1.Value.Date);                    

                    Medicina.RIPSHistory for_RIPS = new Medicina.RIPSHistory();
                    for_RIPS.Adm_Cargo = Convert.ToInt32(Admision);
                    for_RIPS.ShowDialog();

                    repositorioAgendaMedica.Graba_Hora_Salida(Admision);

                    this.Dispose();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void label41_Click(object sender, EventArgs e)
        {
            MessageBox.Show("El IMC se calcula automaticamente, sin embargo la operacion es: \n\r " +
                          "(IMC = peso [kg]/ estatura [m2]) \n\r \n\r debe hacer CLICK en el recuadro gris para calcular. \n\r " +
                          "Puede pasar el Mouse por encima del recuadro de peso y altura para ver una ayuda",
                          "Instruccion",
                          MessageBoxButtons.OK,
                          MessageBoxIcon.Information);
            return;
        }
        private void label40_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Hola.  De acuerdo al IMC Obtenido puedes calcular el Estado Nutricional de la siguiente manera: \n\r \n\r" +
                           " IMC < 18.5 = Bajo Peso \n\r " +
                           " IMC Entre 18.6 y 24.9 = Normal \n\r " +
                           " IMC Entre 25 y 29.9 = Sobrepeso \n\r " +
                           " IMC Entre 30 y 34.9 = Obesidad Moderada \n\r " +
                           " IMC Entre 35 = 39.9 = Obesidad Severa \n\r " +
                           " IMC > 40 = Obesidad Muy Severa \n\r",
                           "Instruccion",
                           MessageBoxButtons.OK,
                           MessageBoxIcon.Information);
        }
        private void textBox25_Click(object sender, EventArgs e)
        {
            try
            {
                double Valor1, Valor2, Res;
                Valor1 = Convert.ToDouble(textBox27.Text);
                Valor2 = Convert.ToDouble(textBox28.Text);

                Res = Valor2 / (Valor1 * Valor1);
                textBox25.Text = Convert.ToDouble(Res).ToString("N2");

            }
            catch
            {
                textBox25.Text = "0";
            }
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
                    string INGSAL = "";

                    switch (comboBox26.Text)
                    {
                        case "Ingreso":
                            INGSAL = "I";
                            break;

                        case "Salida":
                            INGSAL = "S";
                            break;

                        case "N/A":
                            INGSAL = "";
                            break;

                        default:
                            INGSAL = "";
                            return;
                    }
                    CXN_HCMG H = new CXN_HCMG
                    {
                        HC_Pac = textBox1.Text,
                        HC_Prof = Prof,
                        HC_Ase = Ase,
                        HC_Com = Cia,
                        HC_Pacid = Paciente,
                        HC_Edad = edad.ToString(),
                        HC_FechaNto = Convert.ToDateTime(dateTimePicker1.Value.Date),
                        HC_MotivoC = textBox7.Text,
                        HC_EnfA = textBox8.Text,
                        HC_GradoC = comboBox11.Text,
                        HC_TipoLes = comboBox10.Text,
                        HC_ActEje = comboBox8.Text,
                        HC_Vez = comboBox25.Text,
                        HC_Neurologico = textBox18.Text,
                        HC_Cardiovascular = textBox16.Text,
                        HC_GastroIntestinal = textBox15.Text,
                        HC_GastroUrinario = textBox14.Text,
                        HC_OsteoMuscular = textBox13.Text,
                        HC_Piel = textBox12.Text,
                        HC_Ocupacion = textBox11.Text,
                        HC_AparienciaG = comboBox5.Text,
                        HC_EstadoEmo = comboBox9.Text,
                        HC_EstadoNut = comboBox12.Text,
                        HC_Exudado = comboBox16.Text,
                        HC_Presart = textBox32.Text,
                        HC_Frecar = textBox31.Text,
                        HC_FreRes = textBox30.Text,
                        HC_Temp = textBox29.Text,
                        HC_Peso = textBox28.Text,
                        HC_Altura = textBox27.Text,
                        HC_IMC = textBox25.Text,
                        HC_ITB = textBox26.Text,
                        HC_DescHer = textBox33.Text,
                        HC_TejCom = label62.Text,
                        HC_CaracTej = label60.Text,
                        HC_SignosInf = label58.Text,
                        HC_PielCirc = label56.Text,
                        HC_ConsCant = comboBox15.Text,
                        HC_Estado = comboBox13.Text,
                        HC_Dolor = comboBox14.Text,
                        HC_Analisis = textBox42.Text,
                        HC_Complicacion = textBox40.Text,
                        HC_PruebasDiag = textBox9.Text,
                        HC_ProtoInst = "N/A",
                        HC_PManejo = textBox41.Text,
                        HC_DX1 = textBox39.Text,
                        HC_DX2 = textBox39.Text,
                        HC_DX3 = textBox39.Text,
                        HC_DX1T = textBox38.Text,
                        HC_AntFam = textBox24.Text,
                        HC_AntPat = textBox23.Text,
                        HC_AntQui = textBox21.Text,
                        HC_AntAle = textBox20.Text,
                        HC_AntFarma = textBox22.Text,
                        HC_Hematolin = textBox19.Text,
                        HC_Patologia = comboBox23.Text,
                        HC_Fecha = Fecha_Serv,
                        HC_Cant = 0,
                        HC_RH = comboBox21.Text,
                        HC_SubPat = comboBox22.Text,
                        HC_Imp_Dx = comboBox24.Text,
                        HC_Respiratorio = textBox17.Text,
                        HC_Epidemia = textBox10.Text,
                        HC_ServCatalogo = textBox51.Text,
                        HC_CupCatalogo = textBox50.Text,
                        HC_Adm = Admision,
                        HC_TipoINGSAL = INGSAL.ToString()
                    };

                    if (Retoma == true) //ACTUALIZA HISTORIA
                    {
                        var Actualiza = repositorioMedicinaGeneral.ActualizaHCMG(H);
                        if (Actualiza != true)
                        {
                            MessageBox.Show("No se logro actualizar la historia clinica, revise los datos",
                                "Incompleto",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Exclamation);
                            return;
                        }
                    }

                    if (Retoma == false) //INSERTA HISTORIA
                    {
                        var Graba = repositorioMedicinaGeneral.GrabaHCMG(H);
                        if (Graba != true)
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

                    repositorioAgendaMedica.ConsumirAdmision(Admision);

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
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void textBox39_DoubleClick(object sender, EventArgs e)
        {
            Medicina.CIE10 MC = new Medicina.CIE10("HCMG1");
            MC.ShowDialog();
        }
        private void textBox37_DoubleClick(object sender, EventArgs e)
        {
            Medicina.CIE10 MC = new Medicina.CIE10("HCMG2");
            MC.ShowDialog();
        }
        private void textBox35_DoubleClick(object sender, EventArgs e)
        {
            Medicina.CIE10 MC = new Medicina.CIE10("HCMG3");
            MC.ShowDialog();
        }
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            ActualizarPAC();
        }   
        private void button10_Click(object sender, EventArgs e)
        {
            label62.Text = label62.Text + ", " + comboBox20.Text;
        }
        private void button4_Click(object sender, EventArgs e)
        {
            label60.Text = label60.Text + ", " + comboBox19.Text;
        }
        private void button6_Click(object sender, EventArgs e)
        {
            label58.Text = label58.Text + ", " + comboBox18.Text;
        }
        private void button8_Click(object sender, EventArgs e)
        {
            label56.Text = label56.Text + ", " + comboBox17.Text;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            label62.Text = "";
        }
        private void button3_Click(object sender, EventArgs e)
        {
            label60.Text = "";
        }
        private void button1_Click(object sender, EventArgs e)
        {
            label58.Text = "";
        }
        private void button7_Click(object sender, EventArgs e)
        {
            label56.Text = "";
        }
        private void button13_Click(object sender, EventArgs e)
        {
            textBox39.Text = "";
            textBox38.Text = "";
        }
        private void button12_Click(object sender, EventArgs e)
        {
            textBox37.Text = "";
            textBox36.Text = "";
        }
        private void toolStripLabel4_Click(object sender, EventArgs e)
        {
            Medicina.Historial_Medico_1 RM = new Medicina.Historial_Medico_1();
            RM.ShowDialog();
        }     
        void MostrarPanel2Servicios()
        {
            try
            {
                if (repoConfSystem.getListado()["MedicinaGeneralEstadistica"] == "A")
                {
                    CatalogodePacientes();
                    CargarServiciosLista();

                    panel2.Size = new Size(1044, 768);
                    panel2.Location = new Point(140, 55);

                    var VAl = repositorioConvenios.ServicioNombre(CUP, Ase, TSERV);
                    if (VAl == null)
                    {
                        MessageBox.Show("Error grave cargardo valor del servicio, vuelva a ingresar a la admision",
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        return;
                    }

                    Serv = VAl.Con_Nombre;
                    CUP = VAl.Con_Id_Serv;
                    textBox49.Text = Serv.ToString();
                    textBox48.Text = CUP.ToString();
                    panel2.Visible = true;
                }
                else
                {
                    GrabarHistoria();
                }                    
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void listView2_Click(object sender, EventArgs e)
        {
            textBox49.Text = listView2.SelectedItems[0].SubItems[1].Text;
            textBox48.Text = listView2.SelectedItems[0].SubItems[0].Text;
            Valor = Convert.ToInt32(listView2.SelectedItems[0].SubItems[2].Text);
        }
        private void button9_Click(object sender, EventArgs e)
        {
            try
            {
                switch (comboBox26.Text)
                {
                    case "Ingreso":
                        INGSAL = "I";
                        break;

                    case "Salida":
                        INGSAL = "S";
                        break;

                    case "N/A":
                        INGSAL = "";
                        break;

                    default:
                        MessageBox.Show("Debe seleccionar el tipo de cita Ingreso, Salida o N/A");
                        return;
                }

                if (textBox50.Text == "" || textBox51.Text == "" || textBox48.Text == "" || textBox49.Text == "" || comboBox26.Text == "")
                {
                    MessageBox.Show("Debe catalogar al paciente", "Falta Catalogo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    CUP = textBox48.Text;
                    Serv = textBox49.Text;

                    CXN_CARGOS C = new CXN_CARGOS
                    {
                        Car_Cod = CUP,
                        Car_Item = Serv,
                        Car_Val_Un = Valor,
                        Car_Val_Tot = Valor,
                        Car_Adm_Id = Admision,
                        Car_Tipo = "Historia"
                    };

                    repositorioNotasCuracion.ActualizarServCuracion(C, Comunes.Contenedor.UsuarioLogueado);
                    panel2.Visible = false;

                    GrabarHistoria();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void toolStripLabel6_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult result = MessageBox.Show("Desea cancelar esta historia? Al hacerlo no guardara ningun dato", "Zamenis_Health", MessageBoxButtons.YesNo);

                if (result == DialogResult.Yes)
                {
                    repositorioAgendaMedica.OpenAdmition(Admision, "N");
                    this.Dispose();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void EncabezadosLV3()
        {
            listView3.Clear();
            listView3.View = View.Details;
            listView3.GridLines = true;
            listView3.FullRowSelect = true;
            listView3.Columns.Add("Codigo", 80, HorizontalAlignment.Left);
            listView3.Columns.Add("Servicio", 400, HorizontalAlignment.Left);
            listView3.Columns.Add("Valor", 80, HorizontalAlignment.Left);
        }
        private void listView3_Click(object sender, EventArgs e)
        {
            textBox51.Text = listView3.SelectedItems[0].SubItems[1].Text;
            textBox50.Text = listView3.SelectedItems[0].SubItems[0].Text;
            ValorCatalogo = Convert.ToInt32(listView3.SelectedItems[0].SubItems[2].Text);
        }
        private void CatalogodePacientes()
        {
            try
            {
                var getConvenios = repositorioConvenios.getServicesXAseServ(Ase, "CU");
                if (getConvenios != null)
                {
                    EncabezadosLV3();

                    foreach (var i in getConvenios)
                    {
                        listView3.Items.Add(new ListViewItem(new string[]
                          {
                                i.Con_Id_Serv.ToString(),
                                i.Con_Nombre.ToString(),
                                Convert.ToInt32(i.Con_Valor).ToString()
                          }));
                    }
                }
                else
                {
                    EncabezadosLV3();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, " - Lista Servicios", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void EncabezadosLV2()
        {
            listView2.Clear();
            listView2.View = View.Details;
            listView2.GridLines = true;
            listView2.FullRowSelect = true;
            listView2.Columns.Add("Codigo", 80, HorizontalAlignment.Left);
            listView2.Columns.Add("Servicio", 400, HorizontalAlignment.Left);
            listView2.Columns.Add("Valor", 80, HorizontalAlignment.Left);
        }
        private void CargarServiciosLista()
        {
            try
            {
                var getConvenios = repositorioConvenios.getServicesXAseServ(Ase, "MG");
                if (getConvenios != null)
                {
                    EncabezadosLV2();

                    foreach (var i in getConvenios)
                    {
                        listView2.Items.Add(new ListViewItem(new string[]
                          {
                                i.Con_Id_Serv.ToString(),
                                i.Con_Nombre.ToString(),
                                Convert.ToInt32(i.Con_Valor).ToString()
                          }));
                    }
                }
                else
                {
                    EncabezadosLV2();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, " - Lista Servicios", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        void CargarBotones()
        {
            Titulo.Text = "Historia Clinica Electronica - Medicina General - Consulta Externa";
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";
            LogoMain.Image = Properties.Resources.Splash;

            btnDatosPaciente = new ToolStripButton();
            btnDatosPaciente = createToolButton("Datos Paciente");
            MenuLateral.Items.Add(btnDatosPaciente);
            btnDatosPaciente.Click += toolStripButton1_Click;

            btnGuardarEgreso = new ToolStripButton();
            btnGuardarEgreso = createToolButton("Guardar y Egresar");
            MenuLateral.Items.Add(btnGuardarEgreso);
            btnGuardarEgreso.Click += toolStripLabel1_Click;

            btnGuardarNoEgreso = new ToolStripButton();
            btnGuardarNoEgreso = createToolButton("Guardar SIN Egresar");
            MenuLateral.Items.Add(btnGuardarNoEgreso);
            btnGuardarNoEgreso.Click += toolStripLabel2_Click;

            btnVerHistorias = new ToolStripButton();
            btnVerHistorias = createToolButton("Ver Historias");
            MenuLateral.Items.Add(btnVerHistorias);
            btnVerHistorias.Click += toolStripLabel4_Click;

            if (repoConfSystem.getListado()["IHCE"] == "A")
            {
                btnVerIHCE = new ToolStripButton();
                btnVerIHCE = createToolButton("Ver IHCE");
                MenuLateral.Items.Add(btnVerIHCE);
                btnVerIHCE.Click += toolStripButton8_Click;
            }           

            btnVerImagenes = new ToolStripButton();
            btnVerImagenes = createToolButton("Ver Imagenes");
            MenuLateral.Items.Add(btnVerImagenes);
            btnVerImagenes.Click += toolStripLabel7_Click;

            btnCrearOrdenes = new ToolStripButton();
            btnCrearOrdenes = createToolButton("Crear Ordenes");
            MenuLateral.Items.Add(btnCrearOrdenes);
            btnCrearOrdenes.Click += toolStripLabel5_Click;

            btnTraerUltimo = new ToolStripButton();
            btnTraerUltimo = createToolButton("Traer Historia");
            MenuLateral.Items.Add(btnTraerUltimo);
            btnTraerUltimo.Click += toolStripLabel3_Click;

            btnMensajero = new ToolStripButton();
            btnMensajero = createToolButton("Mensajero");
            MenuLateral.Items.Add(btnMensajero);
            btnMensajero.Click += toolStripButton3_Click;

            btnFormatos = new ToolStripButton();
            btnFormatos = createToolButton("Formatos");
            MenuLateral.Items.Add(btnFormatos);
            btnFormatos.Click += toolStripButton4_Click;

            btnCotizaciones = new ToolStripButton();
            btnCotizaciones = createToolButton("Cotizaciones");
            MenuLateral.Items.Add(btnCotizaciones);
            btnCotizaciones.Click += toolStripButton6_Click;

            btnMedidasEnfermeria = new ToolStripButton();
            btnMedidasEnfermeria = createToolButton("Medidas Heridas");
            MenuLateral.Items.Add(btnMedidasEnfermeria);
            btnMedidasEnfermeria.Click += toolStripButton2_Click;

            btnAprobarSolictudes = new ToolStripButton();
            btnAprobarSolictudes = createToolButton("Aprobar Solicitudes");
            MenuLateral.Items.Add(btnAprobarSolictudes);
            btnAprobarSolictudes.Click += toolStripButton5_Click;

            btnCancelar = new ToolStripButton();
            btnCancelar = createToolButton("Cancelar");
            MenuLateral.Items.Add(btnCancelar);
            btnCancelar.Click += toolStripLabel6_Click;
        }
        private void Historia_MedicinaGeneral_Load(object sender, EventArgs e)
        {
            try
            {
                var data = repoConfSystem.getListado()["EVENTOSADVERSOS"];
                if (data != "X")
                {
                    UrlEvento = data;
                    linkLabel1.Visible = true;
                }


                ImageClose.Visible = false;
                ImageMinimize.Visible = false;

                
                repositorioAgendaMedica.Graba_Hora_Atencion(Admision);
                repositorioAgendaMedica.OpenAdmition(Admision, "S");

                CargarBotones();

                comboBox22.Parent = panel1;
                //comboBox22.Location = new Point(389, 2640);

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

                if (Preferencias.TCPIP != "A")
                {
                    button14.Visible = false;
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

                var VAl = repositorioConvenios.ServicioNombre(CUP, Ase, TSERV);
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

                textBox48.Text = CUP.ToString();
                textBox49.Text = Serv.ToString();
                textBox5.Text = Admision.ToString();
                textBox1.Text = DatosAdmision.Hor_Imp_Age.ToString();
                dateTimePicker1.Value = Convert.ToDateTime(DatosAdmision.Pac_FechaNto);
                textBox2.Text = DatosAdmision.Pac_TipoId.ToString() + " " + DatosAdmision.Pac_IdNum.ToString();
                textBox52.Text = DatosAdmision.Hor_Observacion;
                textBox11.Text = DatosAdmision.Pac_Ocupacion;

                string consultarCitasEnfermeriaHoy = repositorioAgendaMedica.SearchCuracionForMG(Paciente, Convert.ToDateTime(DatosAdmision.Hor_Pac_Fecha_Cita), Prof);
                if (consultarCitasEnfermeriaHoy != "")
                {
                    textBox52.Text = textBox52.Text +
                        "\n\r" +
                        "Este paciente tiene cita hoy mismo con: " + consultarCitasEnfermeriaHoy;
                }

                DateTime nacimiento = Convert.ToDateTime(DatosAdmision.Pac_FechaNto);
                edad = DateTime.Today.AddTicks(-nacimiento.Ticks).Year - 1;
                textBox4.Text = edad.ToString() + " años";

                switch (DatosAdmision.Pac_Sexo)
                {
                    case "M":
                        comboBox1.SelectedIndex = 0;
                        break;

                    case "F":
                        comboBox1.SelectedIndex = 1;
                        break;

                    case "I":
                        comboBox1.SelectedIndex = 2;
                        break;

                    default:
                        comboBox1.SelectedIndex = 3;
                        break;
                }
          
                CargarOpcionesBase();
                ActualizarPAC();
                CargarAntFHIR();

                var DatoPac = repoPacientes.LlamarPacientebyId(Paciente);
                if (DatoPac.Pac_Doble != "S")
                {
                    checkBox1.Checked = false;
                }
                else
                {
                    checkBox1.Checked = true;
                }

                if (DatoPac.Pac_2VXS != "S")
                {
                    checkBox2.Checked = false;
                }
                else
                {
                    checkBox2.Checked = true;
                }

                if (Retoma == true)
                {
                    RecuperarIngresado();
                }

                AbrirFormEnPanel(new Extras.MedidasMG(Admision, Paciente));           

                string getRecos = repoConfSystem.getListado()["Recomendaciones"];
                if (getRecos == "A")
                {
                    Extras.Recomendaciones r = new Extras.Recomendaciones(DatosAdmision.Hor_Pac_Id);
                    r.ShowDialog();

                    CargarOpcionesRecomendaciones();
                }
                
                if (repoConfSystem.getListado()["RellenarVaciosMG"] == "A")
                {
                    string textoARellenar = repoConfSystem.getListado()["RellenarVaciosMG1"];

                    textBox18.Text = textoARellenar;
                    textBox17.Text = textoARellenar;
                    textBox16.Text = textoARellenar;
                    textBox15.Text = textoARellenar;
                    textBox14.Text = textoARellenar;
                    textBox13.Text = textoARellenar;

                    textBox24.Text = "NIEGA";
                    textBox23.Text = "NIEGA";
                    textBox22.Text = "NIEGA";
                    textBox21.Text = "NIEGA"; 
                    textBox20.Text = "NIEGA"; 
                    textBox19.Text = "NIEGA";

                    textBox40.Text = "NINGUNA";
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }    
        private void ActualizarPAC()
        {
            Medicina.ActualizarPaciente Historia_Edita_Paciente = new Medicina.ActualizarPaciente(Paciente);
            Historia_Edita_Paciente.ShowDialog();
        }
        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            Historia_MedicinaGeneral_2 MG = new Historia_MedicinaGeneral_2(this.Admision, this.Paciente);
            MG.ShowDialog();
        }
        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            Historia_MedicinaGeneral_3 H = new Historia_MedicinaGeneral_3(Paciente, Cia);
            H.ShowDialog();
        }
        private void button14_Click(object sender, EventArgs e)
        {
            try
            {
                MensajesGeneral MG = new MensajesGeneral();

                bool f = repositorioAgendaMedica.addSALECONSULTA(this.Admision);
                if (f == true)
                {                    
                    MG.Mensaje = "La notificacion ha sido enviada a recepcion";
                    MG.TipoImagen = 3;
                    MG.ShowDialog();
                }
                else
                {
                    MG.Mensaje = "La notificacion no pudo ser enviada a la recepcion, vuelva a intentar";
                    MG.TipoImagen = 1000;
                    MG.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                DateTime nacimiento = Convert.ToDateTime(dateTimePicker1.Value.Date);
                edad = DateTime.Today.AddTicks(-nacimiento.Ticks).Year - 1;
                textBox4.Text = edad.ToString() + " años";

                repoPacs.SexAndDate(Paciente, (comboBox1.SelectedIndex == 0 ? "M" : comboBox1.SelectedIndex == 1 ? "F" : "I"), dateTimePicker1.Value.Date);
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
        private void CaidaTxt_Click(object sender, EventArgs e)
        {
            openConditions("CAIDA");
        }
        private void infeccionTxt_Click(object sender, EventArgs e)
        {
            openConditions("INFECCION");
        }
        private void textBox11_DoubleClick(object sender, EventArgs e)
        {
            CXN_PACIENTES datPac = repoPacientes.LlamarPacientebyId(Paciente);
            textBox11.Text = datPac.Pac_Ocupacion;
        }
        private void dataGridView4_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                int Posision = Convert.ToInt32(dataGridView4.Rows[e.RowIndex].Cells[1].Value.ToString());
                DialogResult result = MessageBox.Show("¿Desea eliminar esta Alergia?" +
                       "Alergias detectadas por usted",
                       "Zamenis Health",
                       MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    repoCond.deleteINSITE(Posision);
                    CargarGrillaINSITE();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }           
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(UrlEvento);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void boton5_Click(object sender, EventArgs e)
        {
            CargarGrillaINSITE();
        }

        private void button17_Click(object sender, EventArgs e)
        {
            panel2.Visible = false;
        }
        private void boton3_Click(object sender, EventArgs e)
        {
            Alergias c = new Alergias("ALERGIA", this.Paciente);
            c.ShowDialog();
            CargarOpcionesRecomendaciones();
        }
        private void boton4_Click(object sender, EventArgs e)
        {
            try
            {
                if (comboBox3.Text == "")
                {
                    MessageBox.Show("Seleccione un tipo de alergia",
                                    "Zamenis Health - Condiciones Especiales de Pacientes",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning);
                    return;
                }
                if (string.IsNullOrEmpty(textBox47.Text))
                {
                    MessageBox.Show("Describa la alergia",
                                    "Zamenis Health - Condiciones Especiales de Pacientes",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning);
                    return;
                }

                DialogResult result = MessageBox.Show("¿Desea agregar esta condicion?",
                                                 "Zamenis Health - Condiciones Especiales de Pacientes",
                                                 MessageBoxButtons.YesNo,
                                                 MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    CXN_ALERGIASINSITE p = new CXN_ALERGIASINSITE
                    {
                        Paciente = Paciente,
                        Admision = Admision,
                        Alergia = comboBox3.Text,
                        Observacion = textBox47.Text,
                        CodeFHIR = comboBox3.Text == "Medicamento" ? "01" :
                                     comboBox3.Text == "Alimento" ? "02" :
                                     comboBox3.Text == "Sustancia del ambiente" ? "03" :
                                     comboBox3.Text == "Sustancia que entran en contacto con la piel" ? "04" :
                                     comboBox3.Text == "Picadura de insectos" ? "05" : "06"
                    };

                    repoCond.createCondicionesInSite(p);

                    CargarGrillaINSITE();
                    textBox47.Text = "";

                    MG = new MensajesGeneral()
                    {
                        TipoImagen = 3,
                        Mensaje = "Agregado con exito"
                    };
                    
                    MG.ShowDialog();
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
        void Encabezados()
        {
            dataGridView4.DataSource = null;
            dt4 = new DataTable();
            Id4 = dt4.Columns.Add("Id", typeof(int));
            POS4 = dt4.Columns.Add("POS", typeof(int));
            Codigo4 = dt4.Columns.Add("Codigo", typeof(string));
            Alergia4 = dt4.Columns.Add("Alergia", typeof(string));
            Observacion4 = dt4.Columns.Add("Observacion", typeof(string));
        }
        void Estilos()
        {
            dataGridView4.RowHeadersVisible = false;
            dataGridView4.EnableHeadersVisualStyles = false;
            dataGridView4.ScrollBars = ScrollBars.Both;

            dataGridView4.DataSource = dt4;

            dataGridView4.Columns["Codigo"].Width = 100;
            dataGridView4.Columns["Alergia"].Width = 200;
            dataGridView4.Columns["Observacion"].Width = 600;
            dataGridView4.Font = new Font("Arial", 10);

            dataGridView4.ColumnHeadersDefaultCellStyle.Font = new Font(dataGridView1.Font, FontStyle.Bold);
            dataGridView4.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 10, FontStyle.Bold);
            dataGridView4.ColumnHeadersDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#bfdbff");
            dataGridView4.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;
            dataGridView4.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;

            dataGridView4.Columns["Codigo"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView4.Columns["Alergia"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView4.Columns["Observacion"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView4.Columns["POS"].Visible = false;
            dataGridView4.Columns["Id"].Visible = false;

            foreach (DataGridViewRow row in dataGridView4.Rows)
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
        void CargarGrillaINSITE()
        {
            try
            {
                Encabezados();

                List<CXN_ALERGIASINSITE> H = repoCond.getCondicionesINSITE(Admision);
                if (H != null)
                {
                    int Contador = 1;

                    foreach (var h in H)
                    {
                        DataRow row = dt4.NewRow();

                        row["POS"] = Contador;
                        row["Id"] = h.Id;
                        row["Codigo"] = h.CodeFHIR;
                        row["Alergia"] = h.Alergia.ToString();
                        row["Observacion"] = h.Observacion;

                        dt4.Rows.Add(row);
                        dt4.AcceptChanges();

                        Contador = Contador + 1;
                    }

                    Contador = 1;
                    Estilos();
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
        private void button15_Click(object sender, EventArgs e)
        {
            textBox24.Text = "SIN ANTECEDENTES".Trim();
        }
        private void textBox24_Leave(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(textBox24.Text))
                {
                    textBox24.Text = "SIN ANTECEDENTES".Trim();
                }
                else if (textBox24.Text.Contains("SIN ANTECEDENTES"))
                {
                    textBox24.Text = "SIN ANTECEDENTES".Trim();
                }
            }
            catch 
            {
                textBox24.Text = "SIN ANTECEDENTES".Trim();
            }
        }
        private void psiquiatricoTxt_Click(object sender, EventArgs e)
        {
            openConditions("PACIENTE PSIQUIATRICO");
        }
        private void mayorTxt_Click(object sender, EventArgs e)
        {
            openConditions("MAYOR DE 70 AÑOS");
        }
        private async void Historia_MedicinaGeneral_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                if (repoConfSystem.getListado()["IHCE"] == "A")
                {
                    await Autoriza();
                    CrearBundlePaciente(Admision);
                }              
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        private async void timer1_Tick(object sender, EventArgs e)
        {
            RDAConsultaExterna rdaCE = new RDAConsultaExterna();
            await rdaCE.RadicarRDA(Admision, "Medicina General");

            timer1.Enabled = false;
        }
        private void dificilTxt_Click(object sender, EventArgs e)
        {
            openConditions("PACIENTE DIFICIL");
        }
        private void requiereTxt_Click(object sender, EventArgs e)
        {
            openConditions("MEDICO LO REQUIERE");
        }
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            MedidasEnferemeria M = new MedidasEnferemeria(Paciente);
            M.ShowDialog();
        }
        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            AprobarSolicitudes A = new AprobarSolicitudes();
            A.ShowDialog();
        }
        #region ANTECEDENTES
        //FAMILIARES PACIENTE
        private void textBox6_Click(object sender, EventArgs e)
        {
            Medicina.CIE10 MC = new Medicina.CIE10("HCMGANTFAM");
            MC.ShowDialog();
        }
        private void button16_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(textBox6.Text) || string.IsNullOrEmpty(textBox3.Text) || comboBox2.Text == "")
                {
                    MessageBox.Show("Debe ingresar un codigo y una descripcion para el antecedente y un parentesco",
                        "Faltan datos",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation);
                }
                else
                {
                    bool BuscarExistente = repoAntGen.BuscaAntecedente(Paciente, textBox6.Text, comboBox2.Text);
                    if (BuscarExistente == true)
                    {
                        MessageBox.Show("El antecedente ya se encuentra registrado para este paciente",
                            "Existente",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Exclamation);
                    }
                    else
                    {
                        CXN_ANTECEDENTESFAMILIARES A = new CXN_ANTECEDENTESFAMILIARES
                        {
                            Paciente = Paciente,
                            CIECod = textBox6.Text,
                            CieDesc = textBox3.Text,
                            Parentesco = comboBox2.Text,
                            Fecha = DateTime.Now.Date,
                            Usuario = Contenedor.UsuarioLogueado,
                            Tipo = ""
                        };

                        repoAntGen.InsertarAntecedente(A);

                        MessageBox.Show("El antecedente ha sido registrado",
                            "Creado",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Exclamation);

                        textBox6.Text = "";
                        textBox3.Text = "";

                        CargarAntFHIR();
                    }
                }                
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        //PATOLOGICOS PACIENTE
        private void textBox44_DoubleClick(object sender, EventArgs e)
        {
            Medicina.CIE10 MC = new Medicina.CIE10("HCMGANTPAT");
            MC.ShowDialog();
        }
        private void boton1_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(textBox44.Text) || string.IsNullOrEmpty(textBox43.Text))
                {
                    MessageBox.Show("Debe ingresar un codigo y una descripcion para el antecedente",
                        "Faltan datos",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation);
                }
                else
                {
                    bool BuscarExistente = repoAntGen.BuscaAntecedente(Paciente, textBox44.Text, "PROPIO");
                    if (BuscarExistente == true)
                    {
                        MessageBox.Show("El antecedente ya se encuentra registrado para este paciente",
                            "Existente",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Exclamation);
                    }
                    else
                    {
                        CXN_ANTECEDENTESFAMILIARES A = new CXN_ANTECEDENTESFAMILIARES
                        {
                            Paciente = Paciente,
                            CIECod = textBox44.Text,
                            CieDesc = textBox43.Text,
                            Parentesco = "PROPIO",
                            Fecha = DateTime.Now.Date,
                            Usuario = Contenedor.UsuarioLogueado,
                            Tipo = "PATOLOGICO"
                        };

                        repoAntGen.InsertarAntecedente(A);

                        MessageBox.Show("El antecedente ha sido registrado",
                            "Creado",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Exclamation);

                        textBox44.Text = "";
                        textBox43.Text = "";

                        CargarAntFHIR();
                    }
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        //FARMACOLOGICOS PACIENTE
        private void textBox46_DoubleClick(object sender, EventArgs e)
        {
            Medicina.OrdenesMedicasM2 MC = new Medicina.OrdenesMedicasM2("ANTPROPIO");
            MC.ShowDialog();
        }
        private void boton2_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(textBox46.Text) || string.IsNullOrEmpty(textBox45.Text))
                {
                    MessageBox.Show("Debe ingresar un codigo y una descripcion para el antecedente",
                        "Faltan datos",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation);
                }
                else
                {
                    bool BuscarExistente = repoAntGen.BuscaAntecedente(Paciente, textBox46.Text, "FARMACOLOGICO");
                    if (BuscarExistente == true)
                    {
                        MessageBox.Show("El antecedente ya se encuentra registrado para este paciente",
                            "Existente",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Exclamation);
                    }
                    else
                    {
                        CXN_ANTECEDENTESFAMILIARES A = new CXN_ANTECEDENTESFAMILIARES
                        {
                            Paciente = Paciente,
                            CIECod = textBox46.Text,
                            CieDesc = textBox45.Text,
                            Parentesco = "PROPIO",
                            Fecha = DateTime.Now.Date,
                            Usuario = Contenedor.UsuarioLogueado,
                            Tipo = "FARMACOLOGICO"                            
                        };

                        repoAntGen.InsertarAntecedente(A);

                        MessageBox.Show("El antecedente ha sido registrado",
                            "Creado",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Exclamation);

                        textBox46.Text = "";
                        textBox45.Text = "";

                        CargarAntFHIR();
                    }
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        void EncabezadosAnt()
        {
            dataGridView1.DataSource = null;
            dt = new DataTable();
            POS = dt.Columns.Add("POS", typeof(int));
            Id = dt.Columns.Add("Id", typeof(int));
            Parentesco = dt.Columns.Add("Parentesco", typeof(string));
            Codigo = dt.Columns.Add("Codigo", typeof(string));
            Descripcion = dt.Columns.Add("Descripcion", typeof(string));
        }
        void EncabezadosPatologicos()
        {
            dataGridView2.DataSource = null;
            dt2 = new DataTable();
            POS2 = dt2.Columns.Add("POS", typeof(int));
            Id2 = dt2.Columns.Add("Id", typeof(int));
            Parentesco2 = dt2.Columns.Add("Parentesco", typeof(string));
            Codigo2 = dt2.Columns.Add("Codigo", typeof(string));
            Descripcion2 = dt2.Columns.Add("Descripcion", typeof(string));
        }
        void EncabezadosFarmacologicos()
        {
            dataGridView3.DataSource = null;
            dt3 = new DataTable();
            POS3 = dt3.Columns.Add("POS", typeof(int));
            Id3 = dt3.Columns.Add("Id", typeof(int));
            Parentesco3 = dt3.Columns.Add("Parentesco", typeof(string));
            Codigo3 = dt3.Columns.Add("Codigo", typeof(string));
            Descripcion3 = dt3.Columns.Add("Descripcion", typeof(string));
        }
        void AntecedentesFamiliares()
        {
            try
            {
                List<CXN_ANTECEDENTESFAMILIARES> getAntFam = repoAntGen.ObtenerAntecedentes(Paciente);
                if (getAntFam != null)
                {
                    getAntFam = getAntFam.Where(x => x.Parentesco != "PROPIO").ToList();

                    EncabezadosAnt();

                    int Contador = 1;

                    foreach (CXN_ANTECEDENTESFAMILIARES i in getAntFam)
                    {
                        DataRow row = dt.NewRow();

                        row["POS"] = Contador;
                        row["Id"] = i.Id;
                        row["Parentesco"] = i.Parentesco.ToString();
                        row["Codigo"] = i.CIECod;
                        row["Descripcion"] = i.CieDesc;

                        dt.Rows.Add(row);
                        dt.AcceptChanges();

                        Contador = Contador + 1;
                    }

                    Contador = 1;
                    Estilos(dataGridView1, dt);
                }
                else
                {
                    EncabezadosAnt();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, " - Lista Antecedentes Familiares", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        void AntecedentesPatologicos()
        {
            try
            {
                List<CXN_ANTECEDENTESFAMILIARES> getAntFam = repoAntGen.ObtenerAntecedentes(Paciente);
                if (getAntFam != null)
                {
                    EncabezadosPatologicos();

                    int Contador = 1;

                    foreach (CXN_ANTECEDENTESFAMILIARES i in getAntFam)
                    {
                        if (i.Parentesco == "PROPIO" && i.Tipo == "PATOLOGICO")
                        {
                            DataRow row = dt2.NewRow();

                            row["POS"] = Contador;
                            row["Id"] = i.Id;
                            row["Parentesco"] = i.Parentesco.ToString();
                            row["Codigo"] = i.CIECod;
                            row["Descripcion"] = i.CieDesc;

                            dt2.Rows.Add(row);
                            dt2.AcceptChanges();

                            Contador = Contador + 1;
                        }                        
                    }

                    Contador = 1;
                    Estilos(dataGridView2, dt2);
                }
                else
                {
                    EncabezadosPatologicos();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, " - Lista Antecedentes Familiares", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        void AntecedentesFarmacologicos()
        {
            try
            {
                List<CXN_ANTECEDENTESFAMILIARES> getAntFam = repoAntGen.ObtenerAntecedentes(Paciente);
                if (getAntFam != null)
                {
                    EncabezadosFarmacologicos();

                    int Contador = 1;

                    foreach (CXN_ANTECEDENTESFAMILIARES i in getAntFam)
                    {
                        if (i.Parentesco == "PROPIO" && i.Tipo == "FARMACOLOGICO")
                        {
                            DataRow row = dt3.NewRow();

                            row["POS"] = Contador;
                            row["Id"] = i.Id;
                            row["Parentesco"] = i.Parentesco.ToString();
                            row["Codigo"] = i.CIECod;
                            row["Descripcion"] = i.CieDesc;

                            dt3.Rows.Add(row);
                            dt3.AcceptChanges();

                            Contador = Contador + 1;
                        }
                    }

                    Contador = 1;
                    Estilos(dataGridView3, dt3);
                }
                else
                {
                    EncabezadosFarmacologicos();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, " - Lista Antecedentes Familiares", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        void CargarAntFHIR()
        {
            AntecedentesFamiliares();
            AntecedentesPatologicos();
            AntecedentesFarmacologicos();
        }
        void Estilos(DataGridView D, DataTable t)
        {
            D.EnableHeadersVisualStyles = false;
            D.ScrollBars = ScrollBars.Both;

            D.DataSource = t;

            D.Columns["Parentesco"].Width = 150;
            D.Columns["Codigo"].Width = 100;
            D.Columns["Descripcion"].Width = 650;
            
            D.ColumnHeadersDefaultCellStyle.Font = new Font(D.Font, FontStyle.Bold);
            D.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 8, FontStyle.Bold);
            D.ColumnHeadersDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#bfdbff");
            D.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;
            D.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;

            D.Columns["Parentesco"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["Codigo"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["Descripcion"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;         

            D.Columns["POS"].Visible = false;
            D.Columns["Id"].Visible = false;

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
        //FAMILIARES
        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                DialogResult result = MessageBox.Show("¿Confirma que desea eliminar este antecedente?",
                                                "Zamenis Health - FHIR MinSalud",
                                                MessageBoxButtons.YesNoCancel,
                                                MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    repoAntGen.EliminarAntecedente(Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells["Id"].Value.ToString()));
                    CargarAntFHIR();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        //PATOLOGICOS
        private void dataGridView2_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                DialogResult result = MessageBox.Show("¿Confirma que desea eliminar este antecedente?",
                                                "Zamenis Health - FHIR MinSalud",
                                                MessageBoxButtons.YesNoCancel,
                                                MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    repoAntGen.EliminarAntecedente(Convert.ToInt32(dataGridView2.Rows[e.RowIndex].Cells["Id"].Value.ToString()));
                    CargarAntFHIR();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        //FARMACOLOGICOS
        private void dataGridView3_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                DialogResult result = MessageBox.Show("¿Confirma que desea eliminar este antecedente?",
                                                "Zamenis Health - FHIR MinSalud",
                                                MessageBoxButtons.YesNoCancel,
                                                MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    repoAntGen.EliminarAntecedente(Convert.ToInt32(dataGridView3.Rows[e.RowIndex].Cells["Id"].Value.ToString()));
                    CargarAntFHIR();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        #endregion AntFamGlobal
        private void toolStripButton8_Click(object sender, EventArgs e)
        {
            FrontFHIR.VisorZamenis.VerRDA ass = new FrontFHIR.VisorZamenis.VerRDA(false, CMANTID, CMANID);
            ass.ShowDialog();
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                repoPacs.SexAndDate(Paciente, (comboBox1.SelectedIndex == 0 ? "M" : comboBox1.SelectedIndex == 1 ? "F" : "I"), dateTimePicker1.Value.Date);
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }            
        }
        private void RecuperarIngresado()
        {
            try
            {
                var getTemporal = repositorioMedicinaGeneral.getIngresadoTemporal(Admision);
                if (getTemporal != null)
                {
                    textBox7.Text = getTemporal.HC_MotivoC.ToString();
                    textBox8.Text = getTemporal.HC_EnfA.ToString();
                    textBox9.Text = getTemporal.HC_PruebasDiag.ToString();

                    textBox18.Text = getTemporal.HC_Neurologico.ToString();
                    textBox17.Text = getTemporal.HC_Respiratorio.ToString();
                    textBox16.Text = getTemporal.HC_Cardiovascular.ToString();
                    textBox15.Text = getTemporal.HC_GastroIntestinal.ToString();
                    textBox14.Text = getTemporal.HC_GastroUrinario.ToString();
                   // textBox11.Text = getTemporal.HC_Ocupacion.ToString();
                    textBox12.Text = getTemporal.HC_Piel.ToString();
                    textBox13.Text = getTemporal.HC_OsteoMuscular.ToString();

                    textBox24.Text = getTemporal.HC_AntFam.ToString();
                    textBox23.Text = getTemporal.HC_AntPat.ToString();
                    textBox22.Text = getTemporal.HC_AntFarma.ToString();
                    textBox21.Text = getTemporal.HC_AntQui.ToString();
                    textBox20.Text = getTemporal.HC_AntAle.ToString();
                    textBox19.Text = getTemporal.HC_Hematolin.ToString();

                    comboBox12.Text = getTemporal.HC_EstadoNut.ToString();
                    textBox32.Text = getTemporal.HC_Presart.ToString();
                    textBox31.Text = getTemporal.HC_Frecar.ToString();
                    textBox29.Text = getTemporal.HC_Temp.ToString();
                    textBox30.Text = getTemporal.HC_FreRes.ToString();
                    textBox28.Text = getTemporal.HC_Peso.ToString();
                    textBox27.Text = getTemporal.HC_Altura.ToString();
                    textBox25.Text = getTemporal.HC_IMC.ToString();
                    textBox26.Text = getTemporal.HC_ITB.ToString();

                    comboBox20.Text = getTemporal.HC_TejCom.ToString();
                    comboBox19.Text = getTemporal.HC_CaracTej.ToString();
                    comboBox18.Text = getTemporal.HC_SignosInf.ToString();
                    comboBox17.Text = getTemporal.HC_PielCirc.ToString();
                    comboBox16.Text = getTemporal.HC_Exudado.ToString();
                    comboBox15.Text = getTemporal.HC_ConsCant.ToString();
                    comboBox14.Text = getTemporal.HC_Estado.ToString();
                    comboBox13.Text = getTemporal.HC_Dolor.ToString();
                    textBox33.Text = getTemporal.HC_DescHer.ToString();
                    label62.Text = getTemporal.HC_TejCom.ToString();
                    label60.Text = getTemporal.HC_CaracTej.ToString();
                    label58.Text = getTemporal.HC_SignosInf.ToString();
                    label56.Text = getTemporal.HC_PielCirc.ToString();

                    comboBox24.Text = getTemporal.HC_Imp_Dx.ToString();
                    comboBox23.Text = getTemporal.HC_Patologia.ToString();
                    comboBox21.Text = getTemporal.HC_RH.ToString();
                    textBox38.Text = getTemporal.HC_DX1T.ToString();
                    textBox42.Text = getTemporal.HC_Analisis.ToString();
                    textBox41.Text = getTemporal.HC_PManejo.ToString();
                    textBox40.Text = getTemporal.HC_Complicacion.ToString();
                    textBox39.Text = getTemporal.HC_DX1.ToString();
                    textBox37.Text = getTemporal.HC_DX2.ToString();
                    textBox35.Text = getTemporal.HC_DX3.ToString();

                    var DX = repositorioCIE10.BuscaDX(textBox39.Text);
                    textBox38.Text = DX.ToString();
                    DX = repositorioCIE10.BuscaDX(textBox37.Text);
                    textBox36.Text = DX.ToString();
                    DX = repositorioCIE10.BuscaDX(textBox35.Text);
                    textBox34.Text = DX.ToString();
                }
                else
                {
                    textBox7.Text = "";
                    textBox8.Text = "";
                    textBox9.Text = "";

                    textBox18.Text = "";
                    textBox17.Text = "";
                    textBox16.Text = "";
                    textBox15.Text = "";
                    textBox14.Text = "";
                    //textBox11.Text = "";
                    textBox12.Text = "";
                    textBox13.Text = "";

                    textBox24.Text = "";
                    textBox23.Text = "";
                    textBox22.Text = "";
                    textBox21.Text = "";
                    textBox20.Text = "";
                    textBox19.Text = "";

                    textBox32.Text = "";
                    textBox31.Text = "";
                    textBox29.Text = "";
                    textBox30.Text = "";
                    textBox28.Text = "";
                    textBox27.Text = "";
                    textBox25.Text = "";
                    textBox26.Text = "";

                    comboBox20.Text = "";
                    comboBox19.Text = "";
                    comboBox18.Text = "";
                    comboBox17.Text = "";
                    comboBox16.Text = "";
                    comboBox15.Text = "";
                    comboBox14.Text = "";
                    comboBox13.Text = "";
                    label62.Text = "";
                    label60.Text = "";
                    label58.Text = "";
                    label56.Text = "";

                    comboBox24.Text = "";
                    comboBox23.Text = "";
                    comboBox22.Text = "";
                    comboBox21.Text = "";
                    textBox38.Text = "";
                    textBox42.Text = "";
                    textBox41.Text = "";
                    textBox40.Text = "";
                    textBox39.Text = "";
                    textBox37.Text = "";
                    textBox35.Text = "";

                    MessageBox.Show("No se encontro registros ingresados de este paciente correspondientes a esta admision",
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
        private void AbrirFormEnPanel(object Formhijo)
        {
            if (this.panel3.Controls.Count > 0)
                this.panel3.Controls.RemoveAt(0);
            Form fh = Formhijo as Form;
            fh.TopLevel = false;
            fh.Dock = DockStyle.Fill;
            this.panel3.Controls.Add(fh);
            this.panel3.Tag = fh;
            panel3.Visible = true;
            fh.Show();
        }
        #region FHIR
        async Task Autoriza()
        {
            try
            {
                CXN_TOKENS_FHIR getToken = repoFHIR.RecuperarClaseToken(Cia);
                if (getToken == null)
                {
                    await repoToken.ObtenerTokenIHCE(Cia);                    
                }
                else if (DateTime.Now >= getToken.Fecha.AddHours(1))
                {
                    await repoToken.ObtenerTokenIHCE(Cia);
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
        async void CrearBundlePaciente(int Admision)
        {
            try
            {
                otrosDatosPacienteHorario datosCita = repoAgendaMedicaConsultas.cargarAdmision(Admision, "'H'");
                if (datosCita != null)
                {
                    RDAPaciente rda = new RDAPaciente();
                    rda.EnviarRDAPaciente(Admision);

                    timer1.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion DIN FHIR
    }
}
