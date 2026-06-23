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
using ZamenisHealth.Comunes;
using ZamenisHealth.FrontFHIR.RDAs;
using ZamenisHealth.HistoriasClinicas.Extras;
using ZamenisHealth.Medicina.OrdenesHistory;

namespace ZamenisHealth.HistoriasClinicas
{
    public partial class Historia_Fisiatria : Forma
    {
        private static readonly IPacientes repoPacientes = new MPacientes();
        private static readonly IFisiatria repoFisiatria = new MFisiatria();
        private static readonly ICargos repoCargos = new MCargos();
        private static readonly IRIPS repoRIPS = new MRIPS();
        private static readonly IAgenda repoAgendaMedica = new MAgenda();
        private static readonly IConvenios repoConvenios = new MConvenios();
        private static readonly IAgendaC repoAgendaMedicaConsultas = new MAgendaC();
        private static readonly ICondiciones repoCond = new MCondiciones();
        private static readonly ICIE10 repoCIE10 = new MCIE10();
        private static readonly IAseguradoras repoAseguradoras = new MAseguradoras();
        private static readonly IMenu repoMenu = new MMenu();
        private static readonly IConfSystem repoConfSystem = new MConfSystem();
        private static readonly IAntecedentesGlobales repoAntGen = new MAntecedentesGlobales();
        private static readonly IFHIR repoFHIR = new MFHIR();
        private static readonly CreateToken repoToken = new EndPoint_Token();

        private MensajesGeneral MG;
        private Extras.CondicionesP c;
        public int Admision;
        public bool Retoma;
        DateTime Fecha_Serv;
        int Paciente, Cia, Ase, Prof, Valor;
        public int edad;

        DataTable dt;
        DataColumn POS;
        DataColumn Id;
        DataColumn Parentesco;
        DataColumn Codigo;
        DataColumn Descripcion;

        DataTable dt2, dt3, dt4;
        DataColumn POS2, POS3, POS4;
        DataColumn Id2, Id3, Id4;
        DataColumn Parentesco2, Parentesco3, Alergia4;
        DataColumn Codigo2, Codigo3, Codigo4;
        DataColumn Descripcion2, Descripcion3, Observacion4;

        private ToolStripButton btn1, btn2, btn3, btn4, btn5, btn6, btn7;
        string CUP, TSERV, Reg_RIP, CMANTID, CMANID, Serv;

        public Historia_Fisiatria()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox50.Text = "";
            textBox49.Text = "";
        }
        private void button3_Click(object sender, EventArgs e)
        {
            textBox48.Text = "";
            textBox47.Text = "";
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
        private void button5_Click(object sender, EventArgs e)
        {
            textBox59.Text = "";
        }
        private void textBox52_DoubleClick(object sender, EventArgs e)
        {
            Medicina.CIE10 MC = new Medicina.CIE10("HCFI1");
            MC.ShowDialog();
        }
        private void textBox50_DoubleClick(object sender, EventArgs e)
        {
            Medicina.CIE10 MC = new Medicina.CIE10("HCFI2");
            MC.ShowDialog();
        }
        private void textBox48_DoubleClick(object sender, EventArgs e)
        {
            Medicina.CIE10 MC = new Medicina.CIE10("HCFI3");
            MC.ShowDialog();
        }
        private void toolStripLabel1_Click(object sender, EventArgs e)
        {
            try
            {

                if (Paciente == 0) { MessageBox.Show("No hay paciente"); return; }
                if (textBox7.Text == "") { MessageBox.Show("Motivo de Consulta"); return; }
                if (textBox8.Text == "") { MessageBox.Show("Enfermedad Actual"); return; }
                if (textBox9.Text == "") { MessageBox.Show("Revise la pestaña consulta"); return; }
                if (textBox10.Text == "") { MessageBox.Show("Revise la pestaña consulta"); return; }
                if (textBox11.Text == "") { MessageBox.Show("Revise la pestaña consulta"); return; }
                if (textBox12.Text == "") { MessageBox.Show("Revise la pestaña consulta"); return; }
                if (textBox13.Text == "") { MessageBox.Show("Revise la pestaña consulta"); return; }
                if (textBox14.Text == "") { MessageBox.Show("Revise la pestaña consulta"); return; }
                if (textBox15.Text == "") { MessageBox.Show("Revise la pestaña consulta"); return; }
                if (textBox16.Text == "") { MessageBox.Show("Revise la pestaña consulta"); return; }
                if (textBox17.Text == "") { MessageBox.Show("Revise pestaña antecedentes"); return; }
                if (textBox18.Text == "") { MessageBox.Show("Revise pestaña antecedentes"); return; }
                if (textBox19.Text == "") { MessageBox.Show("Revise pestaña antecedentes"); return; }
                if (textBox20.Text == "") { MessageBox.Show("Revise pestaña antecedentes"); return; }
                if (textBox35.Text == "") { MessageBox.Show("Revise pestaña antecedentes"); return; }
                if (textBox21.Text == "") { MessageBox.Show("Revise pestaña antecedentes"); return; }
                if (textBox22.Text == "") { MessageBox.Show("Revise pestaña antecedentes"); return; }
                if (textBox23.Text == "") { MessageBox.Show("Revise pestaña antecedentes"); return; }
                if (textBox24.Text == "") { MessageBox.Show("Revise pestaña antecedentes"); return; }
                if (textBox25.Text == "") { MessageBox.Show("Revise pestaña antecedentes"); return; }
                //if (textBox26.Text == "") { MessageBox.Show("Revise pestaña antecedentes"); return; }
                if (textBox27.Text == "") { MessageBox.Show("Revise pestaña antecedentes"); return; }
                //if (textBox28.Text == "") { MessageBox.Show("Revise pestaña antecedentes"); return; }
                if (textBox29.Text == "") { MessageBox.Show("Revise pestaña antecedentes"); return; }
                //if (textBox30.Text == "") { MessageBox.Show("Revise pestaña antecedentes"); return; }
                //if (textBox33.Text == "") { MessageBox.Show("Revise pestaña antecedentes"); return; }
                if (textBox34.Text == "") { MessageBox.Show("Revise pestaña antecedentes"); return; }
                if (comboBox4.Text == "") { MessageBox.Show("Revise pestaña antecedentes"); return; }
                if (comboBox1.Text == "") { MessageBox.Show("Revise pestaña antecedentes"); return; }
                if (textBox31.Text == "") { MessageBox.Show("Revise pestaña examen fisico"); return; }
                if (textBox32.Text == "") { MessageBox.Show("Revise pestaña examen fisico"); return; }
                //if (textBox33.Text == "") { MessageBox.Show("Revise pestaña examen fisico"); return; }
                if (textBox34.Text == "") { MessageBox.Show("Revise pestaña examen fisico"); return; }
                if (textBox35.Text == "") { MessageBox.Show("Revise pestaña examen fisico"); return; }
                if (textBox36.Text == "") { MessageBox.Show("Revise pestaña examen fisico"); return; }
                if (textBox37.Text == "") { MessageBox.Show("Revise pestaña examen fisico"); return; }
                if (textBox38.Text == "") { MessageBox.Show("Revise pestaña examen fisico"); return; }
                if (textBox39.Text == "") { MessageBox.Show("Revise pestaña examen fisico"); return; }
                if (textBox40.Text == "") { MessageBox.Show("Revise pestaña examen fisico"); return; }
                //if (textBox41.Text == "") { MessageBox.Show("Revise pestaña examen fisico"); return; }
                //if (textBox42.Text == "") { MessageBox.Show("Revise pestaña examen fisico"); return; }
                //if (textBox43.Text == "") { MessageBox.Show("Revise pestaña examen fisico"); return; }
                if (textBox44.Text == "") { MessageBox.Show("Revise pestaña examen fisico"); return; }
                if (textBox45.Text == "") { MessageBox.Show("Revise pestaña examen fisico"); return; }
                if (textBox46.Text == "") { MessageBox.Show("Revise pestaña examen fisico"); return; }
                if (textBox52.Text == "") { MessageBox.Show("Revise diagnostico principal"); return; }
                if (comboBox2.Text == "") { MessageBox.Show("Revise diagnostico principal"); return; }
                if (textBox56.Text == "") { MessageBox.Show("Revise pestaña Egreso"); return; }
                if (textBox57.Text == "") { MessageBox.Show("Revise pestaña Egreso"); return; }
                if (textBox58.Text == "") { MessageBox.Show("Revise pestaña Egreso"); return; }
                if (textBox59.Text == "") { MessageBox.Show("Revise pestaña Antecedentes, debe agregar un reporte de enfermedades infeciosas"); return; }

                DialogResult result = MessageBox.Show("¿Desea Egresar esta historia?, estos datos ya no se podran editar y se dara " +
                "cierre final. " +
                    "Egresar Atencion",
                    "Zamenis Health",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    CXN_HCFI H = new CXN_HCFI
                    {
                        HC_Pac = textBox1.Text,
                        HC_Pacid = Paciente,
                        HC_Fecha = Fecha_Serv,
                        HC_Prof = Prof,
                        HC_Cia = Cia,
                        HC_Ase = Ase,
                        HC_FechaNto = Convert.ToDateTime(textBox3.Text),
                        HC_Edad = textBox4.Text,
                        HC_Cant = 1,
                        HC_MotCons = textBox7.Text,
                        HC_EnfAct = textBox8.Text,
                        HC_Neurologico = textBox9.Text,
                        HC_Mental = textBox10.Text,
                        HC_OrgSent = textBox11.Text,
                        HC_Respiratorio = textBox12.Text,
                        HC_Cardiovascular = textBox13.Text,
                        HC_GastroI = textBox14.Text,
                        HC_GenitoU = textBox15.Text,
                        HC_OsteoM = textBox16.Text,
                        HC_PielFan = textBox18.Text,
                        HC_Hematolin = textBox17.Text,
                        HC_Ant = textBox19.Text,
                        HC_Presart = textBox21.Text,
                        HC_Peso = textBox22.Text,
                        HC_Frecar = textBox23.Text,
                        HC_Talla = textBox24.Text,
                        HC_FrecResp = textBox25.Text,
                        HC_IMC = textBox34.Text,
                        //HC_Temp = textBox33.Text,
                        //HC_PerimetroC = textBox30.Text,
                        HC_EstCons = textBox29.Text,
                        //HC_PerimetroA = textBox28.Text,
                        HC_Glasshow = textBox27.Text,
                        //HC_Embriaguez = textBox26.Text,
                        HC_ObservaFis = textBox35.Text,
                        HC_ObservaNeu = textBox31.Text,
                        HC_Cabeza = textBox32.Text,
                        HC_Orl = textBox36.Text,
                        //HC_Genitales = textBox43.Text,
                        HC_Abdomen = textBox40.Text,
                        //HC_Ombligo = textBox41.Text,
                        //HC_Ano = textBox42.Text,
                        HC_Torax = textBox38.Text,
                        HC_Extremidades = textBox44.Text,
                        HC_Cuello = textBox37.Text,
                        HC_Pulmonar = textBox39.Text,
                        HC_DX1 = textBox52.Text,
                        HC_DX2 = textBox50.Text,
                        HC_DX3 = textBox48.Text,
                        HC_Analisis = textBox57.Text,
                        HC_Egreso = textBox56.Text,
                        HC_PManejo = textBox58.Text,
                        HC_RH = comboBox4.Text,
                        HC_EAV = comboBox1.Text,
                        HC_Acudiente = textBox20.Text,
                        HC_NotaDX1 = textBox53.Text,
                        HC_NotaDX2 = textBox54.Text,
                        HC_NotaDX3 = textBox55.Text,
                        HC_ImpDX1 = comboBox2.Text,
                        HC_ImpDX2 = comboBox3.Text,
                        HC_ImpDX3 = comboBox5.Text,
                        HC_OsteoMUS = textBox45.Text,
                        HC_Causa_Externa = comboBox6.Text,
                        HC_Piel2 = textBox46.Text,
                        HC_Epidemia = textBox59.Text,
                        HC_Adm = Admision
                    };

                    repoPacientes.SexAndDate(Paciente, (comboBox7.Text == "Masculino" ? "M" : comboBox7.Text == "Femenino" ? "F" : "I"), Convert.ToDateTime(textBox3.Text));

                    if (Retoma == true) //ACTUALIZA HISTORIA
                    {
                        bool updateHistoria = repoFisiatria.ActualizaHCFI(H);
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
                        bool createHistoria = repoFisiatria.GrabaHCFI(H);
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
                DialogResult result = MessageBox.Show("¿Desea guardar esta historia sin egresar? " +
                        "Historia Pendiente",
                        "Zamenis Health",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    CXN_HCFI H = new CXN_HCFI
                    {
                        HC_Pac = textBox1.Text,
                        HC_Pacid = Paciente,
                        HC_Fecha = Fecha_Serv,
                        HC_Prof = Prof,
                        HC_Cia = Cia,
                        HC_Ase = Ase,
                        HC_FechaNto = Convert.ToDateTime(textBox3.Text),
                        HC_Edad = textBox4.Text,
                        HC_Cant = 0,
                        HC_MotCons = textBox7.Text,
                        HC_EnfAct = textBox8.Text,
                        HC_Neurologico = textBox9.Text,
                        HC_Mental = textBox10.Text,
                        HC_OrgSent = textBox11.Text,
                        HC_Respiratorio = textBox12.Text,
                        HC_Cardiovascular = textBox13.Text,
                        HC_GastroI = textBox14.Text,
                        HC_GenitoU = textBox15.Text,
                        HC_OsteoM = textBox16.Text,
                        HC_PielFan = textBox18.Text,
                        HC_Hematolin = textBox17.Text,
                        HC_Ant = textBox19.Text,
                        HC_Presart = textBox21.Text,
                        HC_Peso = textBox22.Text,
                        HC_Frecar = textBox23.Text,
                        HC_Talla = textBox24.Text,
                        HC_FrecResp = textBox25.Text,
                        HC_IMC = textBox34.Text,
                        //HC_Temp = textBox33.Text,
                        //HC_PerimetroC = textBox30.Text,
                        HC_EstCons = textBox29.Text,
                        //HC_PerimetroA = textBox28.Text,
                        HC_Glasshow = textBox27.Text,
                        //HC_Embriaguez = textBox26.Text,
                        HC_ObservaFis = textBox35.Text,
                        HC_ObservaNeu = textBox31.Text,
                        HC_Cabeza = textBox32.Text,
                        HC_Orl = textBox36.Text,
                        //HC_Genitales = textBox43.Text,
                        HC_Abdomen = textBox40.Text,
                        //HC_Ombligo = textBox41.Text,
                        //HC_Ano = textBox42.Text,
                        HC_Torax = textBox38.Text,
                        HC_Extremidades = textBox44.Text,
                        HC_Cuello = textBox37.Text,
                        HC_Pulmonar = textBox39.Text,
                        HC_DX1 = textBox52.Text,
                        HC_DX2 = textBox50.Text,
                        HC_DX3 = textBox48.Text,
                        HC_Analisis = textBox57.Text,
                        HC_Egreso = textBox56.Text,
                        HC_PManejo = textBox58.Text,
                        HC_RH = comboBox4.Text,
                        HC_EAV = comboBox1.Text,
                        HC_Acudiente = textBox20.Text,
                        HC_NotaDX1 = textBox53.Text,
                        HC_NotaDX2 = textBox54.Text,
                        HC_NotaDX3 = textBox55.Text,
                        HC_ImpDX1 = comboBox2.Text,
                        HC_ImpDX2 = comboBox3.Text,
                        HC_ImpDX3 = comboBox5.Text,
                        HC_OsteoMUS = textBox45.Text,
                        HC_Causa_Externa = comboBox6.Text,
                        HC_Piel2 = textBox46.Text,
                        HC_Epidemia = textBox59.Text,
                        HC_Adm = Admision
                    };

                    if (Retoma == true) //ACTUALIZA HISTORIA
                    {
                        bool updateHistoria = repoFisiatria.ActualizaHCFI(H);
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
                        bool createHistoria = repoFisiatria.GrabaHCFI(H);
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
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void toolStripLabel4_Click(object sender, EventArgs e)
        {
            Medicina.Historial_Medico_1 RM = new Medicina.Historial_Medico_1();
            RM.ShowDialog();
        }
        private void toolStripLabel3_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime Hoy = DateTime.Now;

                var getlast = repoFisiatria.getLastHistory(Paciente, Hoy);
                if (getlast != null)
                {
                    label69.Text = "Se ha recuperado la ultima historia con fecha: " + Convert.ToDateTime(getlast.HC_Fecha).ToString("dd-MM-yyyy");
                    textBox7.Text = getlast.HC_MotCons.ToString();
                    textBox8.Text = getlast.HC_EnfAct.ToString();

                    textBox9.Text = getlast.HC_Neurologico.ToString();
                    textBox10.Text = getlast.HC_Mental.ToString();
                    textBox11.Text = getlast.HC_OrgSent.ToString();
                    textBox12.Text = getlast.HC_Respiratorio.ToString();
                    textBox13.Text = getlast.HC_Cardiovascular.ToString();
                    textBox14.Text = getlast.HC_GastroI.ToString();
                    textBox15.Text = getlast.HC_GenitoU.ToString();
                    textBox16.Text = getlast.HC_OsteoM.ToString();
                    textBox17.Text = getlast.HC_Hematolin.ToString();
                    textBox18.Text = getlast.HC_PielFan.ToString();

                    textBox19.Text = getlast.HC_Ant.ToString();
                    textBox59.Text = getlast.HC_Epidemia.ToString();

                    textBox20.Text = getlast.HC_Acudiente.ToString();
                    textBox21.Text = getlast.HC_Presart.ToString();
                    textBox22.Text = getlast.HC_Peso.ToString();
                    textBox23.Text = getlast.HC_Frecar.ToString();
                    textBox24.Text = getlast.HC_Talla.ToString();
                    textBox25.Text = getlast.HC_FrecResp.ToString();

                    //textBox30.Text = getlast.HC_PerimetroC.ToString();
                    textBox29.Text = getlast.HC_EstCons.ToString();
                    //textBox28.Text = getlast.HC_PerimetroA.ToString();
                    textBox27.Text = getlast.HC_Glasshow.ToString();
                    //textBox26.Text = getlast.HC_Embriaguez.ToString();
                    textBox34.Text = getlast.HC_IMC.ToString();
                    //textBox33.Text = getlast.HC_Temp.ToString();
                    comboBox4.Text = getlast.HC_RH.ToString();
                    comboBox1.Text = getlast.HC_EAV.ToString();

                    textBox35.Text = getlast.HC_ObservaFis.ToString();
                    textBox31.Text = getlast.HC_ObservaNeu.ToString();
                    textBox32.Text = getlast.HC_Cabeza.ToString();
                    textBox36.Text = getlast.HC_Orl.ToString();
                    textBox37.Text = getlast.HC_Cuello.ToString();
                    textBox38.Text = getlast.HC_Torax.ToString();
                    textBox39.Text = getlast.HC_Pulmonar.ToString();
                    textBox40.Text = getlast.HC_Abdomen.ToString();
                    //textBox41.Text = getlast.HC_Ombligo.ToString();
                    //textBox42.Text = getlast.HC_Ano.ToString();
                    //textBox43.Text = getlast.HC_Genitales.ToString();
                    textBox44.Text = getlast.HC_Extremidades.ToString();
                    textBox45.Text = getlast.HC_OsteoMUS.ToString();
                    textBox46.Text = getlast.HC_Piel2.ToString();

                    textBox52.Text = getlast.HC_DX1.ToString();
                    textBox50.Text = getlast.HC_DX2.ToString();
                    textBox48.Text = getlast.HC_DX3.ToString();
                    comboBox2.Text = getlast.HC_ImpDX1.ToString();
                    comboBox3.Text = getlast.HC_ImpDX2.ToString();
                    comboBox5.Text = getlast.HC_ImpDX3.ToString();
                    textBox53.Text = getlast.HC_NotaDX1.ToString();
                    textBox54.Text = getlast.HC_NotaDX2.ToString();
                    textBox55.Text = getlast.HC_NotaDX3.ToString();

                    var DX = repoCIE10.BuscaDX(textBox52.Text);
                    textBox51.Text = DX.ToString();
                    DX = repoCIE10.BuscaDX(textBox50.Text);
                    textBox49.Text = DX.ToString();
                    DX = repoCIE10.BuscaDX(textBox48.Text);
                    textBox47.Text = DX.ToString();

                    comboBox6.Text = getlast.HC_Causa_Externa.ToString();

                    textBox56.Text = getlast.HC_Egreso.ToString();
                    textBox57.Text = getlast.HC_Analisis.ToString();
                    textBox58.Text = getlast.HC_PManejo.ToString();
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
                    textBox16.Text = "";
                    textBox17.Text = "";
                    textBox18.Text = "";

                    textBox19.Text = "";
                    textBox59.Text = "";

                    textBox20.Text = "";
                    textBox21.Text = "";
                    textBox22.Text = "";
                    textBox23.Text = "";
                    textBox24.Text = "";
                    textBox25.Text = "";

                    //textBox30.Text = "";
                    textBox29.Text = "";
                    //textBox28.Text = "";
                    textBox27.Text = "";
                    //textBox26.Text = "";
                    textBox34.Text = "";
                    //textBox33.Text = "";

                    textBox35.Text = "";
                    textBox31.Text = "";
                    textBox32.Text = "";
                    textBox36.Text = "";
                    textBox37.Text = "";
                    textBox38.Text = "";
                    textBox39.Text = "";
                    textBox40.Text = "";
                    //textBox41.Text = "";
                    //textBox42.Text = "";
                    //textBox43.Text = "";
                    textBox44.Text = "";
                    textBox45.Text = "";
                    textBox46.Text = "";

                    textBox52.Text = "";
                    textBox50.Text = "";
                    textBox48.Text = "";
                    textBox53.Text = "";
                    textBox54.Text = "";
                    textBox55.Text = "";

                    textBox56.Text = "";
                    textBox57.Text = "";
                    textBox58.Text = "";

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
        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            ActualizarPAC();
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
        private void Historia_Fisiatria_Load(object sender, EventArgs e)
        {
            try
            {
                Titulo.Text = "Medicina Fisica y Rehabilitacion";
                LogoMain.Image = Properties.Resources.Splash;
                SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";

                btn1 = new ToolStripButton();
                btn1 = createToolButton("Datos Pacientes");
                MenuLateral.Items.Add(btn1);
                btn1.Click += toolStripButton3_Click;

                btn2 = new ToolStripButton();
                btn2 = createToolButton("Ver Historias");
                MenuLateral.Items.Add(btn2);
                btn2.Click += toolStripLabel4_Click;

                if (repoConfSystem.getListado()["IHCE"] == "A")
                {
                    ToolStripButton btnVerIHCE = new ToolStripButton();
                    btnVerIHCE = createToolButton("Ver IHCE");
                    MenuLateral.Items.Add(btnVerIHCE);
                    btnVerIHCE.Click += btnVerIHCE_Click;
                }                

                btn3 = new ToolStripButton();
                btn3 = createToolButton("Recuperar ultima HC");
                MenuLateral.Items.Add(btn3);
                btn3.Click += toolStripLabel3_Click;

                btn4 = new ToolStripButton();
                btn4 = createToolButton("Guardar y Egresar");
                MenuLateral.Items.Add(btn4);
                btn4.Click += toolStripLabel1_Click;

                btn5 = new ToolStripButton();
                btn5 = createToolButton("Guardar sin Egresar");
                MenuLateral.Items.Add(btn5);
                btn5.Click += toolStripLabel2_Click;

                btn6 = new ToolStripButton();
                btn6 = createToolButton("Crear Ordenes");
                MenuLateral.Items.Add(btn6);
                btn6.Click += toolStripLabel5_Click;

                btn7 = new ToolStripButton();
                btn7 = createToolButton("Cancelar");
                MenuLateral.Items.Add(btn7);
                btn7.Click += toolStripLabel6_Click;

                Titulo.Visible = false;
                ImageClose.Visible = false;

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

                DateTime nacimiento = Convert.ToDateTime(DatosAdmision.Pac_FechaNto);
                edad = DateTime.Today.AddTicks(-nacimiento.Ticks).Year - 1;
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
                CargarAntFHIR();                
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        void btnVerIHCE_Click(object sender, EventArgs e)
        {
            FrontFHIR.VisorZamenis.VerRDA ass = new FrontFHIR.VisorZamenis.VerRDA(false, CMANTID, CMANID);
            ass.ShowDialog();
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
        private void RecuperarIngresado()
        {
            try
            {
                DateTime Hoy = DateTime.Now;

                var getlast = repoFisiatria.restoreHistory(Admision);
                if (getlast != null)
                {
                    textBox7.Text = getlast.HC_MotCons.ToString();
                    textBox8.Text = getlast.HC_EnfAct.ToString();

                    textBox9.Text = getlast.HC_Neurologico.ToString();
                    textBox10.Text = getlast.HC_Mental.ToString();
                    textBox11.Text = getlast.HC_OrgSent.ToString();
                    textBox12.Text = getlast.HC_Respiratorio.ToString();
                    textBox13.Text = getlast.HC_Cardiovascular.ToString();
                    textBox14.Text = getlast.HC_GastroI.ToString();
                    textBox15.Text = getlast.HC_GenitoU.ToString();
                    textBox16.Text = getlast.HC_OsteoM.ToString();
                    textBox17.Text = getlast.HC_Hematolin.ToString();
                    textBox18.Text = getlast.HC_PielFan.ToString();

                    textBox19.Text = getlast.HC_Ant.ToString();
                    textBox59.Text = getlast.HC_Epidemia.ToString();

                    textBox20.Text = getlast.HC_Acudiente.ToString();
                    textBox21.Text = getlast.HC_Presart.ToString();
                    textBox22.Text = getlast.HC_Peso.ToString();
                    textBox23.Text = getlast.HC_Frecar.ToString();
                    textBox24.Text = getlast.HC_Talla.ToString();
                    textBox25.Text = getlast.HC_FrecResp.ToString();

                    //textBox30.Text = getlast.HC_PerimetroC.ToString();
                    textBox29.Text = getlast.HC_EstCons.ToString();
                    //textBox28.Text = getlast.HC_PerimetroA.ToString();
                    textBox27.Text = getlast.HC_Glasshow.ToString();
                    //textBox26.Text = getlast.HC_Embriaguez.ToString();
                    textBox34.Text = getlast.HC_IMC.ToString();
                    //textBox33.Text = getlast.HC_Temp.ToString();
                    comboBox4.Text = getlast.HC_RH.ToString();
                    comboBox1.Text = getlast.HC_EAV.ToString();

                    textBox35.Text = getlast.HC_ObservaFis.ToString();
                    textBox31.Text = getlast.HC_ObservaNeu.ToString();
                    textBox32.Text = getlast.HC_Cabeza.ToString();
                    textBox36.Text = getlast.HC_Orl.ToString();
                    textBox37.Text = getlast.HC_Cuello.ToString();
                    textBox38.Text = getlast.HC_Torax.ToString();
                    textBox39.Text = getlast.HC_Pulmonar.ToString();
                    textBox40.Text = getlast.HC_Abdomen.ToString();
                    //textBox41.Text = getlast.HC_Ombligo.ToString();
                    //textBox42.Text = getlast.HC_Ano.ToString();
                    //textBox43.Text = getlast.HC_Genitales.ToString();
                    textBox44.Text = getlast.HC_Extremidades.ToString();
                    textBox45.Text = getlast.HC_OsteoMUS.ToString();
                    textBox46.Text = getlast.HC_Piel2.ToString();

                    textBox52.Text = getlast.HC_DX1.ToString();
                    textBox50.Text = getlast.HC_DX2.ToString();
                    textBox48.Text = getlast.HC_DX3.ToString();
                    comboBox2.Text = getlast.HC_ImpDX1.ToString();
                    comboBox3.Text = getlast.HC_ImpDX2.ToString();
                    comboBox5.Text = getlast.HC_ImpDX3.ToString();
                    textBox53.Text = getlast.HC_NotaDX1.ToString();
                    textBox54.Text = getlast.HC_NotaDX2.ToString();
                    textBox55.Text = getlast.HC_NotaDX3.ToString();

                    var DX = repoCIE10.BuscaDX(textBox52.Text);
                    textBox51.Text = DX.ToString();
                    DX = repoCIE10.BuscaDX(textBox50.Text);
                    textBox49.Text = DX.ToString();
                    DX = repoCIE10.BuscaDX(textBox48.Text);
                    textBox47.Text = DX.ToString();

                    comboBox6.Text = getlast.HC_Causa_Externa.ToString();

                    textBox56.Text = getlast.HC_Egreso.ToString();
                    textBox57.Text = getlast.HC_Analisis.ToString();
                    textBox58.Text = getlast.HC_PManejo.ToString();
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
                    textBox16.Text = "";
                    textBox17.Text = "";
                    textBox18.Text = "";

                    textBox19.Text = "";
                    textBox59.Text = "";

                    textBox20.Text = "";
                    textBox21.Text = "";
                    textBox22.Text = "";
                    textBox23.Text = "";
                    textBox24.Text = "";
                    textBox25.Text = "";

                    //textBox30.Text = "";
                    textBox29.Text = "";
                    //textBox28.Text = "";
                    textBox27.Text = "";
                    //textBox26.Text = "";
                    textBox34.Text = "";
                    //textBox33.Text = "";

                    textBox35.Text = "";
                    textBox31.Text = "";
                    textBox32.Text = "";
                    textBox36.Text = "";
                    textBox37.Text = "";
                    textBox38.Text = "";
                    textBox39.Text = "";
                    textBox40.Text = "";
                    //textBox41.Text = "";
                    //textBox42.Text = "";
                    //textBox43.Text = "";
                    textBox44.Text = "";
                    textBox45.Text = "";
                    textBox46.Text = "";

                    textBox52.Text = "";
                    textBox50.Text = "";
                    textBox48.Text = "";
                    textBox53.Text = "";
                    textBox54.Text = "";
                    textBox55.Text = "";

                    textBox56.Text = "";
                    textBox57.Text = "";
                    textBox58.Text = "";

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

        #region FHIR
        private async void Historia_Fisiatria_FormClosed(object sender, FormClosedEventArgs e)
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
        #endregion
        private async void timer1_Tick_1(object sender, EventArgs e)
        {
            RDAConsultaExterna rdaCE = new RDAConsultaExterna();
            await rdaCE.RadicarRDA(Admision, "Fisiatria");

            timer1.Enabled = false;
        }

        private void boton3_Click(object sender, EventArgs e)
        {
            Alergias c = new Alergias("ALERGIA", this.Paciente);
            c.ShowDialog();
            CargarOpcionesRecomendaciones();
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
        
        #region ALERGIAS INSITE
        private void boton4_Click(object sender, EventArgs e)
        {
            try
            {
                if (comboBox9.Text == "")
                {
                    MessageBox.Show("Seleccione un tipo de alergia",
                                    "Zamenis Health - Condiciones Especiales de Pacientes",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning);
                    return;
                }
                if (string.IsNullOrEmpty(textBox66.Text))
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
                        Alergia = comboBox9.Text,
                        Observacion = textBox66.Text,
                        CodeFHIR = comboBox9.Text == "Medicamento" ? "01" :
                                     comboBox9.Text == "Alimento" ? "02" :
                                     comboBox9.Text == "Sustancia del ambiente" ? "03" :
                                     comboBox9.Text == "Sustancia que entran en contacto con la piel" ? "04" :
                                     comboBox9.Text == "Picadura de insectos" ? "05" : "06"
                    };

                    repoCond.createCondicionesInSite(p);

                    CargarGrillaINSITE();
                    textBox66.Text = "";

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
        private void boton5_Click(object sender, EventArgs e)
        {
            CargarGrillaINSITE();
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
        #endregion

        #region ANETECEDENTES
        private void boton1_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(textBox63.Text) || string.IsNullOrEmpty(textBox62.Text))
                {
                    MessageBox.Show("Debe ingresar un codigo y una descripcion para el antecedente",
                        "Faltan datos",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation);
                }
                else
                {
                    bool BuscarExistente = repoAntGen.BuscaAntecedente(Paciente, textBox63.Text, "PROPIO");
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
                            CIECod = textBox63.Text,
                            CieDesc = textBox62.Text,
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

                        textBox63.Text = "";
                        textBox62.Text = "";

                        CargarAntFHIR();
                    }
                }
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
                if (string.IsNullOrEmpty(textBox65.Text) || string.IsNullOrEmpty(textBox64.Text))
                {
                    MessageBox.Show("Debe ingresar un codigo y una descripcion para el antecedente",
                        "Faltan datos",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation);
                }
                else
                {
                    bool BuscarExistente = repoAntGen.BuscaAntecedente(Paciente, textBox65.Text, "FARMACOLOGICO");
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
                            CIECod = textBox65.Text,
                            CieDesc = textBox64.Text,
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

                        textBox65.Text = "";
                        textBox64.Text = "";

                        CargarAntFHIR();
                    }
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        void CargarAntFHIR()
        {
            AntecedentesFamiliares();
            AntecedentesPatologicos();
            AntecedentesFarmacologicos();
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
        private void textBox63_DoubleClick(object sender, EventArgs e)
        {
            Medicina.CIE10 MC = new Medicina.CIE10("HCFIANTPAT");
            MC.ShowDialog();
        }
        private void textBox65_DoubleClick(object sender, EventArgs e)
        {
            Medicina.OrdenesMedicasM2 MC = new Medicina.OrdenesMedicasM2("ANTFIPROPIO");
            MC.ShowDialog();
        }
        #endregion

        private void dificilTxt_Click(object sender, EventArgs e)
        {
            openConditions("PACIENTE DIFICIL");
        }
        private void textBox61_Click(object sender, EventArgs e)
        {
            Medicina.CIE10 MC = new Medicina.CIE10("HCFIANTFAM");
            MC.ShowDialog();
        }
        private void button16_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(textBox61.Text) || string.IsNullOrEmpty(textBox60.Text) || comboBox8.Text == "")
                {
                    MessageBox.Show("Debe ingresar un codigo y una descripcion para el antecedente y un parentesco",
                        "Faltan datos",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation);
                }
                else
                {
                    bool BuscarExistente = repoAntGen.BuscaAntecedente(Paciente, textBox61.Text, comboBox8.Text);
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
                            CIECod = textBox61.Text,
                            CieDesc = textBox60.Text,
                            Parentesco = comboBox8.Text,
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
        private void requiereTxt_Click(object sender, EventArgs e)
        {
            openConditions("MEDICO LO REQUIERE");
        }
        private void toolStripLabel5_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox52.Text) || string.IsNullOrEmpty(textBox51.Text))
            {
                MG = new MensajesGeneral()
                {
                    Mensaje = "Debe ingresar al menos un diagnostico principal para poder generar ordenes medicas",
                    TipoImagen = 1000
                };
                MG.ShowDialog();
                return;
            }

            TipoOrdenMedica tipoOrdenMedica = new TipoOrdenMedica("FISIATRIA", Admision);
            tipoOrdenMedica.ShowDialog();
        }       
    }
}
