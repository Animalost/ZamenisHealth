using Domain;
using Domain.Fibromialgia;
using FormAndControls;
using Microsoft.Reporting.WinForms;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using Persistence.Fibromialgia.Interfaces;
using Persistence.Fibromialgia.Metodos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using ZamenisHealth.Clases;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.Fibromialgia.Encuestas.Informe
{
    public partial class InformesEncuestas : Forma2
    {
        private static readonly IPacientes repoPac = new MPacientes();
        private static readonly IEncuestasXPaciente repoEncuestasXPaciente = new MEncuestasXPaciente();
        private static readonly IConsolidadoSF36 repoConsolidadoSF36 = new MConsolidadoSF36();
        private static readonly IConsolidadoSF36Porcentual repoConsolidadoSF36Porcentual = new MConsolidadoSF36Porcentual();
        private static readonly IConsolidadoGeneral repoConsolidadoGeneral = new MConsolidadoGeneral();
        private static readonly IEncuestaSatisfaccion repoEncuestaSatisfaccion = new MEncuestaSatisfaccion();
        private static readonly IImpactoFibromialgia repoImpactoFibromialgia = new MImpactoFibromialgia();
        private static readonly ITFGeneral repoTFGeneral = new MTFGeneral();

        private int AdmitionChange = 0;
        private int Posision;
        private string TEncuesta, Status;

        DataTable dt;
        DataColumn Admision;
        DataColumn Fecha;
        DataColumn Paciente;
        DataColumn Tipo;

        private MensajesGeneral MG;

        public InformesEncuestas()
        {
            InitializeComponent();
        }

        private void InformesEncuestas_Load(object sender, EventArgs e)
        {
            try
            {
                SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";
                Titulo.Text = "Generacion de Informes Encuestas de Pacientes";

                gridZH1.CeldaHeight = true;
                gridZH1.dataGridView1.CellDoubleClick += GridZH1_CellDoubleClick;
                gridZH1.dataGridView1.CellMouseClick += GridZH1_CellMouseClick;
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        void CargarEncuestas()
        {
            try
            {
                gridZH1.dataGridView1.DataSource = null;
                DataTable dt2 = new DataTable();
                DataColumn IdEncuestas;
                DataColumn FechaEncuesta;
                DataColumn PacienteEncuesta;
                DataColumn TipoEncuesta;
                DataColumn UsuarioEncuesta;
                DataColumn EstadoEncuesta;

                List<ClaseReportsFibro> lista = repoEncuestasXPaciente.GetAllEncuestas(dateTimePicker1.Value.Date, dateTimePicker2.Value.Date, textBox1.Text);
                if (lista != null)
                {
                    IdEncuestas = dt2.Columns.Add("IdEncuestas", typeof(int));
                    FechaEncuesta = dt2.Columns.Add("Fecha", typeof(string));
                    PacienteEncuesta = dt2.Columns.Add("Paciente", typeof(string));
                    TipoEncuesta = dt2.Columns.Add("Tipo", typeof(string));
                    UsuarioEncuesta = dt2.Columns.Add("Usuario", typeof(string));
                    EstadoEncuesta = dt2.Columns.Add("Estado", typeof(string));

                    foreach (var i in lista)
                    {
                        DataRow row = dt2.NewRow();

                        row[IdEncuestas] = i.Admision;
                        row[FechaEncuesta] = Convert.ToDateTime(i.Fecha1).ToString(Conexion.ConectionDictionary["Format_Fecha"]);
                        row[PacienteEncuesta] = i.Dato1.ToString();
                        row[TipoEncuesta] = i.Dato2.ToString();
                        row[UsuarioEncuesta] = i.Dato3.ToString();
                        row[EstadoEncuesta] = i.Dato4.ToString() == "V" ? "Vigente" : "Anulada";

                        dt2.Rows.Add(row);
                        dt2.AcceptChanges();
                    }

                    gridZH1.dataGridView1.DataSource = dt2;
                    gridZH1.dataGridView1.Columns["IdEncuestas"].Visible = false;
                }
                else
                {
                    gridZH1.dataGridView1.DataSource = null;
                }              
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (listBox1.SelectedItem == null)
                {
                    MG = new MensajesGeneral()
                    {
                        Mensaje = "Seleccione una opcion valida",
                        TipoImagen = 1000
                    };

                    MG.ShowDialog();
                    return;
                }

                if (listBox1.SelectedItem.ToString() == "Informes de Encuestas por Paciente")
                {
                    if (string.IsNullOrEmpty(textBox1.Text))
                    {
                        MG = new MensajesGeneral();
                        MG.TipoImagen = 1000;
                        MG.Mensaje = "Seleccione tipo de documento del paciente y digite el numero de documento del paciente";
                        MG.ShowDialog();
                    }
                    else
                    {
                        //Obtener todas las encuestas
                        CargarEncuestas();

                        List<int> listaIdE1 = repoEncuestasXPaciente.ObtenerEncuestaXPaciente(Convert.ToDateTime(dateTimePicker1.Value.Date),
                                                                                              Convert.ToDateTime(dateTimePicker2.Value.Date),
                                                                                              textBox1.Text);
                        List<int> listaIdE2 = repoEncuestasXPaciente.ObtenerEncuesta2XPaciente(Convert.ToDateTime(dateTimePicker1.Value.Date),
                                                                                               Convert.ToDateTime(dateTimePicker2.Value.Date),
                                                                                               textBox1.Text);

                        if (listaIdE1 != null)
                        {
                            ClaseReportsFibro getInformeE1_2 = new ClaseReportsFibro();
                            ClaseReportsFibro getInformeE1_3 = new ClaseReportsFibro();
                            FIB_ENCUESTA2 getInformeE2_2 = new FIB_ENCUESTA2();
                            FIB_ENCUESTA2 getInformeE2_3 = new FIB_ENCUESTA2();

                            int E2_1 = 0;
                            int E2_2 = 0;
                            int E2_3 = 0;
                            
                            if (listaIdE2 != null)
                            {
                                E2_1 = listaIdE2[0];

                                if (listaIdE2.Count == 2)
                                {
                                    E2_2 = listaIdE2[1];
                                }
                                else if (listaIdE2.Count > 2)
                                {
                                    E2_2 = listaIdE2[1];
                                    E2_3 = listaIdE2[2];
                                }
                            }                            

                            //Encuesta 1
                            ClaseReportsFibro getInformeE1_1 = repoEncuestasXPaciente.getResultPacE1(listaIdE1[0]);
                            FIB_ENCUESTA2 getInformeE2_1 = repoEncuestasXPaciente.getResultE2(E2_1);

                            getInformeE1_1.Fecha2 = dateTimePicker1.Value.Date;
                            getInformeE1_1.Fecha3 = dateTimePicker2.Value.Date;

                            getInformeE1_1.Dato100 = getInformeE2_1.Pregunta1;
                            getInformeE1_1.Dato101 = getInformeE2_1.Pregunta2;
                            getInformeE1_1.Dato102 = getInformeE2_1.Pregunta3;
                            getInformeE1_1.Dato103 = getInformeE2_1.Pregunta4;
                            getInformeE1_1.Dato104 = getInformeE2_1.Pregunta5;
                            getInformeE1_1.Dato105 = getInformeE2_1.Pregunta6;
                            getInformeE1_1.Dato106 = getInformeE2_1.Pregunta7;
                            getInformeE1_1.Dato107 = getInformeE2_1.Pregunta8;
                            getInformeE1_1.Dato108 = getInformeE2_1.Pregunta9;
                            getInformeE1_1.Dato109 = getInformeE2_1.Pregunta10;
                            getInformeE1_1.Dato110 = getInformeE2_1.Pregunta11;
                            getInformeE1_1.Dato111 = getInformeE2_1.Pregunta12;
                            getInformeE1_1.Dato112 = getInformeE2_1.Pregunta13;
                            getInformeE1_1.Dato113 = getInformeE2_1.Pregunta14;
                            getInformeE1_1.Dato114 = getInformeE2_1.Pregunta15;
                            getInformeE1_1.Dato115 = getInformeE2_1.Pregunta16;
                            getInformeE1_1.Dato116 = getInformeE2_1.Pregunta17;
                            getInformeE1_1.Dato117 = getInformeE2_1.Pregunta18;
                            getInformeE1_1.Dato118 = getInformeE2_1.Pregunta19;
                            getInformeE1_1.Dato119 = getInformeE2_1.Fatiga.ToString();
                            getInformeE1_1.Dato120 = getInformeE2_1.Sueño.ToString();
                            getInformeE1_1.Dato121 = getInformeE2_1.Trastorno.ToString();
                            getInformeE1_1.Dato122 = getInformeE2_1.ResultadoP3.ToString();

                            if (listaIdE1.Count == 2)
                            {
                                //Encuesta 2
                                getInformeE1_2 = repoEncuestasXPaciente.getResultPacE1(listaIdE1[1]);
                                getInformeE2_2 = repoEncuestasXPaciente.getResultE2(E2_2);

                                getInformeE1_2.Dato100 = getInformeE2_2.Pregunta1;
                                getInformeE1_2.Dato101 = getInformeE2_2.Pregunta2;
                                getInformeE1_2.Dato102 = getInformeE2_2.Pregunta3;
                                getInformeE1_2.Dato103 = getInformeE2_2.Pregunta4;
                                getInformeE1_2.Dato104 = getInformeE2_2.Pregunta5;
                                getInformeE1_2.Dato105 = getInformeE2_2.Pregunta6;
                                getInformeE1_2.Dato106 = getInformeE2_2.Pregunta7;
                                getInformeE1_2.Dato107 = getInformeE2_2.Pregunta8;
                                getInformeE1_2.Dato108 = getInformeE2_2.Pregunta9;
                                getInformeE1_2.Dato109 = getInformeE2_2.Pregunta10;
                                getInformeE1_2.Dato110 = getInformeE2_2.Pregunta11;
                                getInformeE1_2.Dato111 = getInformeE2_2.Pregunta12;
                                getInformeE1_2.Dato112 = getInformeE2_2.Pregunta13;
                                getInformeE1_2.Dato113 = getInformeE2_2.Pregunta14;
                                getInformeE1_2.Dato114 = getInformeE2_2.Pregunta15;
                                getInformeE1_2.Dato115 = getInformeE2_2.Pregunta16;
                                getInformeE1_2.Dato116 = getInformeE2_2.Pregunta17;
                                getInformeE1_2.Dato117 = getInformeE2_2.Pregunta18;
                                getInformeE1_2.Dato118 = getInformeE2_2.Pregunta19;
                                getInformeE1_2.Dato119 = getInformeE2_2.Fatiga.ToString();
                                getInformeE1_2.Dato120 = getInformeE2_2.Sueño.ToString();
                                getInformeE1_2.Dato121 = getInformeE2_2.Trastorno.ToString();
                                getInformeE1_2.Dato122 = getInformeE2_2.ResultadoP3.ToString();
                            }
                            else if (listaIdE1.Count > 2)
                            {
                                //Encuesta 2
                                getInformeE1_2 = repoEncuestasXPaciente.getResultPacE1(listaIdE1[1]);
                                getInformeE2_2 = repoEncuestasXPaciente.getResultE2(E2_2);

                                getInformeE1_2.Dato100 = getInformeE2_2.Pregunta1;
                                getInformeE1_2.Dato101 = getInformeE2_2.Pregunta2;
                                getInformeE1_2.Dato102 = getInformeE2_2.Pregunta3;
                                getInformeE1_2.Dato103 = getInformeE2_2.Pregunta4;
                                getInformeE1_2.Dato104 = getInformeE2_2.Pregunta5;
                                getInformeE1_2.Dato105 = getInformeE2_2.Pregunta6;
                                getInformeE1_2.Dato106 = getInformeE2_2.Pregunta7;
                                getInformeE1_2.Dato107 = getInformeE2_2.Pregunta8;
                                getInformeE1_2.Dato108 = getInformeE2_2.Pregunta9;
                                getInformeE1_2.Dato109 = getInformeE2_2.Pregunta10;
                                getInformeE1_2.Dato110 = getInformeE2_2.Pregunta11;
                                getInformeE1_2.Dato111 = getInformeE2_2.Pregunta12;
                                getInformeE1_2.Dato112 = getInformeE2_2.Pregunta13;
                                getInformeE1_2.Dato113 = getInformeE2_2.Pregunta14;
                                getInformeE1_2.Dato114 = getInformeE2_2.Pregunta15;
                                getInformeE1_2.Dato115 = getInformeE2_2.Pregunta16;
                                getInformeE1_2.Dato116 = getInformeE2_2.Pregunta17;
                                getInformeE1_2.Dato117 = getInformeE2_2.Pregunta18;
                                getInformeE1_2.Dato118 = getInformeE2_2.Pregunta19;
                                getInformeE1_2.Dato119 = getInformeE2_2.Fatiga.ToString();
                                getInformeE1_2.Dato120 = getInformeE2_2.Sueño.ToString();
                                getInformeE1_2.Dato121 = getInformeE2_2.Trastorno.ToString();
                                getInformeE1_2.Dato122 = getInformeE2_2.ResultadoP3.ToString();

                                //Encuesta 3
                                getInformeE1_3 = repoEncuestasXPaciente.getResultPacE1(listaIdE1[2]);
                                getInformeE2_3 = repoEncuestasXPaciente.getResultE2(E2_3);

                                getInformeE1_3.Dato100 = getInformeE2_3.Pregunta1;
                                getInformeE1_3.Dato101 = getInformeE2_3.Pregunta2;
                                getInformeE1_3.Dato102 = getInformeE2_3.Pregunta3;
                                getInformeE1_3.Dato103 = getInformeE2_3.Pregunta4;
                                getInformeE1_3.Dato104 = getInformeE2_3.Pregunta5;
                                getInformeE1_3.Dato105 = getInformeE2_3.Pregunta6;
                                getInformeE1_3.Dato106 = getInformeE2_3.Pregunta7;
                                getInformeE1_3.Dato107 = getInformeE2_3.Pregunta8;
                                getInformeE1_3.Dato108 = getInformeE2_3.Pregunta9;
                                getInformeE1_3.Dato109 = getInformeE2_3.Pregunta10;
                                getInformeE1_3.Dato110 = getInformeE2_3.Pregunta11;
                                getInformeE1_3.Dato111 = getInformeE2_3.Pregunta12;
                                getInformeE1_3.Dato112 = getInformeE2_3.Pregunta13;
                                getInformeE1_3.Dato113 = getInformeE2_3.Pregunta14;
                                getInformeE1_3.Dato114 = getInformeE2_3.Pregunta15;
                                getInformeE1_3.Dato115 = getInformeE2_3.Pregunta16;
                                getInformeE1_3.Dato116 = getInformeE2_3.Pregunta17;
                                getInformeE1_3.Dato117 = getInformeE2_3.Pregunta18;
                                getInformeE1_3.Dato118 = getInformeE2_3.Pregunta19;
                                getInformeE1_3.Dato119 = getInformeE2_3.Fatiga.ToString();
                                getInformeE1_3.Dato120 = getInformeE2_3.Sueño.ToString();
                                getInformeE1_3.Dato121 = getInformeE2_3.Trastorno.ToString();
                                getInformeE1_3.Dato122 = getInformeE2_3.ResultadoP3.ToString();
                            }

                            //CREACION DATASETS
                            List<ClaseReportsFibro> E1 = new List<ClaseReportsFibro>();

                            E1.Add(new ClaseReportsFibro
                            {
                                Fecha1 = getInformeE1_1.Fecha1,
                                Fecha2 = getInformeE1_1.Fecha2,
                                Fecha3 = getInformeE1_1.Fecha3,
                                Dato1 = getInformeE1_1.Dato1 ?? "",

                                Dato10 = getInformeE1_1.Dato10 ?? "",
                                Dato2 = getInformeE1_1.Dato2 ?? "",
                                Dato3 = getInformeE1_1.Dato3 ?? "",
                                Dato4 = getInformeE1_1.Dato4 ?? "",
                                Dato5 = getInformeE1_1.Dato5 ?? "",
                                Dato6 = getInformeE1_1.Dato6 ?? "",
                                Dato7 = getInformeE1_1.Dato7 ?? "",
                                Dato8 = getInformeE1_1.Dato8 ?? "",
                                Dato9 = getInformeE1_1.Dato9 ?? "",

                                Dato11 = getInformeE1_1.Dato11 ?? "",
                                Dato12 = getInformeE1_1.Dato12 ?? "",

                                Dato36 = getInformeE1_1.Dato36 ?? "",

                                //E2
                                Dato13 = getInformeE2_1.Pregunta1 ?? "0",
                                Dato14 = getInformeE2_1.Pregunta2 ?? "0",
                                Dato15 = getInformeE2_1.Pregunta3 ?? "0",
                                Dato16 = getInformeE2_1.Pregunta4 ?? "0",
                                Dato17 = getInformeE2_1.Pregunta5 ?? "0",
                                Dato18 = getInformeE2_1.Pregunta6 ?? "0",
                                Dato19 = getInformeE2_1.Pregunta7 ?? "0",
                                Dato20 = getInformeE2_1.Pregunta8 ?? "0",
                                Dato21 = getInformeE2_1.Pregunta9 ?? "0",
                                Dato22 = getInformeE2_1.Pregunta10 ?? "0",
                                Dato23 = getInformeE2_1.Pregunta11 ?? "0",
                                Dato24 = getInformeE2_1.Pregunta12 ?? "0",
                                Dato25 = getInformeE2_1.Pregunta13 ?? "0",
                                Dato26 = getInformeE2_1.Pregunta14 ?? "0",
                                Dato27 = getInformeE2_1.Pregunta15 ?? "0",
                                Dato28 = getInformeE2_1.Pregunta16 ?? "0",
                                Dato29 = getInformeE2_1.Pregunta17 ?? "0",
                                Dato30 = getInformeE2_1.Pregunta18 ?? "0",
                                Dato31 = getInformeE2_1.Pregunta19 ?? "0",
                                Dato32 = (Convert.ToInt32(getInformeE2_1.Pregunta1) + Convert.ToInt32(getInformeE2_1.Pregunta2) + Convert.ToInt32(getInformeE2_1.Pregunta3) + Convert.ToInt32(getInformeE2_1.Pregunta4) +
                                              Convert.ToInt32(getInformeE2_1.Pregunta5) + Convert.ToInt32(getInformeE2_1.Pregunta6) + Convert.ToInt32(getInformeE2_1.Pregunta7) + Convert.ToInt32(getInformeE2_1.Pregunta8) +
                                              Convert.ToInt32(getInformeE2_1.Pregunta9) + Convert.ToInt32(getInformeE2_1.Pregunta10) + Convert.ToInt32(getInformeE2_1.Pregunta11) + Convert.ToInt32(getInformeE2_1.Pregunta12) +
                                              Convert.ToInt32(getInformeE2_1.Pregunta13) + Convert.ToInt32(getInformeE2_1.Pregunta14) + Convert.ToInt32(getInformeE2_1.Pregunta15) + Convert.ToInt32(getInformeE2_1.Pregunta16) +
                                              Convert.ToInt32(getInformeE2_1.Pregunta17) + Convert.ToInt32(getInformeE2_1.Pregunta18) + Convert.ToInt32(getInformeE2_1.Pregunta19)).ToString(),

                                Dato100 = getInformeE1_1.Dato100 ?? "",
                                Dato101 = getInformeE1_1.Dato101 ?? "",
                                Dato102 = getInformeE1_1.Dato102 ?? "",
                                Dato103 = getInformeE1_1.Dato103 ?? "",
                                Dato104 = getInformeE1_1.Dato104 ?? "",
                                Dato105 = getInformeE1_1.Dato105 ?? "",
                                Dato106 = getInformeE1_1.Dato106 ?? "",
                                Dato107 = getInformeE1_1.Dato107 ?? "",
                                Dato108 = getInformeE1_1.Dato108 ?? "",
                                Dato109 = getInformeE1_1.Dato109 ?? "",
                                Dato110 = getInformeE1_1.Dato110 ?? "",
                                Dato111 = getInformeE1_1.Dato111 ?? "",
                                Dato112 = getInformeE1_1.Dato112 ?? "",
                                Dato113 = getInformeE1_1.Dato113 ?? "",
                                Dato114 = getInformeE1_1.Dato114 ?? "",
                                Dato115 = getInformeE1_1.Dato115 ?? "",
                                Dato116 = getInformeE1_1.Dato116 ?? "",
                                Dato117 = getInformeE1_1.Dato117 ?? "",
                                Dato118 = getInformeE1_1.Dato118 ?? "",
                                Dato119 = getInformeE1_1.Dato119 ?? "",
                                Dato120 = getInformeE1_1.Dato120 ?? "",
                                Dato121 = getInformeE1_1.Dato121 ?? "",
                                Dato122 = getInformeE1_1.Dato122 ?? "",

                                Dato33 = getInformeE2_1.Fatiga.ToString(),
                                Dato34 = getInformeE2_1.Sueño.ToString(),
                                Dato35 = getInformeE2_1.Trastorno.ToString()
                            });

                            List<ClaseReportsFibro> E2 = new List<ClaseReportsFibro>();
                            E2.Add(new ClaseReportsFibro
                            {
                                Fecha1 = getInformeE1_2.Fecha1,
                                Dato1 = getInformeE1_2.Dato1 ?? "",

                                Dato10 = getInformeE1_2.Dato10 ?? "",
                                Dato2 = getInformeE1_2.Dato2 ?? "",
                                Dato3 = getInformeE1_2.Dato3 ?? "",
                                Dato4 = getInformeE1_2.Dato4 ?? "",
                                Dato5 = getInformeE1_2.Dato5 ?? "",
                                Dato6 = getInformeE1_2.Dato6 ?? "",
                                Dato7 = getInformeE1_2.Dato7 ?? "",
                                Dato8 = getInformeE1_2.Dato8 ?? "",
                                Dato9 = getInformeE1_2.Dato9 ?? "",

                                Dato11 = getInformeE1_2.Dato11 ?? "",
                                Dato12 = getInformeE1_2.Dato12 ?? "",

                                Dato36 = getInformeE1_2.Dato36 ?? "",

                                //E2
                                Dato13 = getInformeE2_2.Pregunta1 ?? "0",
                                Dato14 = getInformeE2_2.Pregunta2 ?? "0",
                                Dato15 = getInformeE2_2.Pregunta3 ?? "0",
                                Dato16 = getInformeE2_2.Pregunta4 ?? "0",
                                Dato17 = getInformeE2_2.Pregunta5 ?? "0",
                                Dato18 = getInformeE2_2.Pregunta6 ?? "0",
                                Dato19 = getInformeE2_2.Pregunta7 ?? "0",
                                Dato20 = getInformeE2_2.Pregunta8 ?? "0",
                                Dato21 = getInformeE2_2.Pregunta9 ?? "0",
                                Dato22 = getInformeE2_2.Pregunta10 ?? "0",
                                Dato23 = getInformeE2_2.Pregunta11 ?? "0",
                                Dato24 = getInformeE2_2.Pregunta12 ?? "0",
                                Dato25 = getInformeE2_2.Pregunta13 ?? "0",
                                Dato26 = getInformeE2_2.Pregunta14 ?? "0",
                                Dato27 = getInformeE2_2.Pregunta15 ?? "0",
                                Dato28 = getInformeE2_2.Pregunta16 ?? "0",
                                Dato29 = getInformeE2_2.Pregunta17 ?? "0",
                                Dato30 = getInformeE2_2.Pregunta18 ?? "0",
                                Dato31 = getInformeE2_2.Pregunta19 ?? "0",
                                Dato32 = (Convert.ToInt32(getInformeE2_2.Pregunta1) + Convert.ToInt32(getInformeE2_2.Pregunta2) + Convert.ToInt32(getInformeE2_2.Pregunta3) + Convert.ToInt32(getInformeE2_2.Pregunta4) +
                                              Convert.ToInt32(getInformeE2_2.Pregunta5) + Convert.ToInt32(getInformeE2_2.Pregunta6) + Convert.ToInt32(getInformeE2_2.Pregunta7) + Convert.ToInt32(getInformeE2_2.Pregunta8) +
                                              Convert.ToInt32(getInformeE2_2.Pregunta9) + Convert.ToInt32(getInformeE2_2.Pregunta10) + Convert.ToInt32(getInformeE2_2.Pregunta11) + Convert.ToInt32(getInformeE2_2.Pregunta12) +
                                              Convert.ToInt32(getInformeE2_2.Pregunta13) + Convert.ToInt32(getInformeE2_2.Pregunta14) + Convert.ToInt32(getInformeE2_2.Pregunta15) + Convert.ToInt32(getInformeE2_2.Pregunta16) +
                                              Convert.ToInt32(getInformeE2_2.Pregunta17) + Convert.ToInt32(getInformeE2_2.Pregunta18) + Convert.ToInt32(getInformeE2_2.Pregunta19)).ToString(),

                                Dato100 = getInformeE1_2.Dato100 ?? "",
                                Dato101 = getInformeE1_2.Dato101 ?? "",
                                Dato102 = getInformeE1_2.Dato102 ?? "",
                                Dato103 = getInformeE1_2.Dato103 ?? "",
                                Dato104 = getInformeE1_2.Dato104 ?? "",
                                Dato105 = getInformeE1_2.Dato105 ?? "",
                                Dato106 = getInformeE1_2.Dato106 ?? "",
                                Dato107 = getInformeE1_2.Dato107 ?? "",
                                Dato108 = getInformeE1_2.Dato108 ?? "",
                                Dato109 = getInformeE1_2.Dato109 ?? "",
                                Dato110 = getInformeE1_2.Dato110 ?? "",
                                Dato111 = getInformeE1_2.Dato111 ?? "",
                                Dato112 = getInformeE1_2.Dato112 ?? "",
                                Dato113 = getInformeE1_2.Dato113 ?? "",
                                Dato114 = getInformeE1_2.Dato114 ?? "",
                                Dato115 = getInformeE1_2.Dato115 ?? "",
                                Dato116 = getInformeE1_2.Dato116 ?? "",
                                Dato117 = getInformeE1_2.Dato117 ?? "",
                                Dato118 = getInformeE1_2.Dato118 ?? "",
                                Dato119 = getInformeE1_2.Dato119 ?? "",
                                Dato120 = getInformeE1_2.Dato120 ?? "",
                                Dato121 = getInformeE1_2.Dato121 ?? "",
                                Dato122 = getInformeE1_2.Dato122 ?? "",

                                Dato33 = getInformeE2_2.Fatiga.ToString(),
                                Dato34 = getInformeE2_2.Sueño.ToString(),
                                Dato35 = getInformeE2_2.Trastorno.ToString()
                            });

                            List<ClaseReportsFibro> E3 = new List<ClaseReportsFibro>();
                            E3.Add(new ClaseReportsFibro
                            {
                                Fecha1 = getInformeE1_3.Fecha1,
                                Dato1 = getInformeE1_3.Dato1 ?? "",

                                Dato10 = getInformeE1_3.Dato10 ?? "",
                                Dato2 = getInformeE1_3.Dato2 ?? "",
                                Dato3 = getInformeE1_3.Dato3 ?? "",
                                Dato4 = getInformeE1_3.Dato4 ?? "",
                                Dato5 = getInformeE1_3.Dato5 ?? "",
                                Dato6 = getInformeE1_3.Dato6 ?? "",
                                Dato7 = getInformeE1_3.Dato7 ?? "",
                                Dato8 = getInformeE1_3.Dato8 ?? "",
                                Dato9 = getInformeE1_3.Dato9 ?? "",

                                Dato11 = getInformeE1_3.Dato11 ?? "",
                                Dato12 = getInformeE1_3.Dato12 ?? "",

                                Dato36 = getInformeE1_3.Dato36 ?? "",

                                //E2
                                Dato13 = getInformeE2_3.Pregunta1 ?? "0",
                                Dato14 = getInformeE2_3.Pregunta2 ?? "0",
                                Dato15 = getInformeE2_3.Pregunta3 ?? "0",
                                Dato16 = getInformeE2_3.Pregunta4 ?? "0",
                                Dato17 = getInformeE2_3.Pregunta5 ?? "0",
                                Dato18 = getInformeE2_3.Pregunta6 ?? "0",
                                Dato19 = getInformeE2_3.Pregunta7 ?? "0",
                                Dato20 = getInformeE2_3.Pregunta8 ?? "0",
                                Dato21 = getInformeE2_3.Pregunta9 ?? "0",
                                Dato22 = getInformeE2_3.Pregunta10 ?? "0",
                                Dato23 = getInformeE2_3.Pregunta11 ?? "0",
                                Dato24 = getInformeE2_3.Pregunta12 ?? "0",
                                Dato25 = getInformeE2_3.Pregunta13 ?? "0",
                                Dato26 = getInformeE2_3.Pregunta14 ?? "0",
                                Dato27 = getInformeE2_3.Pregunta15 ?? "0",
                                Dato28 = getInformeE2_3.Pregunta16 ?? "0",
                                Dato29 = getInformeE2_3.Pregunta17 ?? "0",
                                Dato30 = getInformeE2_3.Pregunta18 ?? "0",
                                Dato31 = getInformeE2_3.Pregunta19 ?? "0",
                                Dato32 = (Convert.ToInt32(getInformeE2_3.Pregunta1) + Convert.ToInt32(getInformeE2_3.Pregunta2) + Convert.ToInt32(getInformeE2_3.Pregunta3) + Convert.ToInt32(getInformeE2_3.Pregunta4) +
                                              Convert.ToInt32(getInformeE2_3.Pregunta5) + Convert.ToInt32(getInformeE2_3.Pregunta6) + Convert.ToInt32(getInformeE2_3.Pregunta7) + Convert.ToInt32(getInformeE2_3.Pregunta8) +
                                              Convert.ToInt32(getInformeE2_3.Pregunta9) + Convert.ToInt32(getInformeE2_3.Pregunta10) + Convert.ToInt32(getInformeE2_3.Pregunta11) + Convert.ToInt32(getInformeE2_3.Pregunta12) +
                                              Convert.ToInt32(getInformeE2_3.Pregunta13) + Convert.ToInt32(getInformeE2_3.Pregunta14) + Convert.ToInt32(getInformeE2_3.Pregunta15) + Convert.ToInt32(getInformeE2_3.Pregunta16) +
                                              Convert.ToInt32(getInformeE2_3.Pregunta17) + Convert.ToInt32(getInformeE2_3.Pregunta18) + Convert.ToInt32(getInformeE2_3.Pregunta19)).ToString(),

                                Dato100 = getInformeE1_3.Dato100 ?? "",
                                Dato101 = getInformeE1_3.Dato101 ?? "",
                                Dato102 = getInformeE1_3.Dato102 ?? "",
                                Dato103 = getInformeE1_3.Dato103 ?? "",
                                Dato104 = getInformeE1_3.Dato104 ?? "",
                                Dato105 = getInformeE1_3.Dato105 ?? "",
                                Dato106 = getInformeE1_3.Dato106 ?? "",
                                Dato107 = getInformeE1_3.Dato107 ?? "",
                                Dato108 = getInformeE1_3.Dato108 ?? "",
                                Dato109 = getInformeE1_3.Dato109 ?? "",
                                Dato110 = getInformeE1_3.Dato110 ?? "",
                                Dato111 = getInformeE1_3.Dato111 ?? "",
                                Dato112 = getInformeE1_3.Dato112 ?? "",
                                Dato113 = getInformeE1_3.Dato113 ?? "",
                                Dato114 = getInformeE1_3.Dato114 ?? "",
                                Dato115 = getInformeE1_3.Dato115 ?? "",
                                Dato116 = getInformeE1_3.Dato116 ?? "",
                                Dato117 = getInformeE1_3.Dato117 ?? "",
                                Dato118 = getInformeE1_3.Dato118 ?? "",
                                Dato119 = getInformeE1_3.Dato119 ?? "",
                                Dato120 = getInformeE1_3.Dato120 ?? "",
                                Dato121 = getInformeE1_3.Dato121 ?? "",
                                Dato122 = getInformeE1_3.Dato122 ?? "",

                                Dato33 = getInformeE2_3.Fatiga.ToString(),
                                Dato34 = getInformeE2_3.Sueño.ToString(),
                                Dato35 = getInformeE2_3.Trastorno.ToString()
                            });

                            Reportes.Maestro maestro = new Reportes.Maestro();
                            maestro.Universal.LocalReport.DataSources.Clear();
                            maestro.Universal.LocalReport.DataSources.Add(new ReportDataSource("DataSetEncuestasPacientes", E1));
                            maestro.Universal.LocalReport.DataSources.Add(new ReportDataSource("DataSetEncuestasPacientes2", E2));
                            maestro.Universal.LocalReport.DataSources.Add(new ReportDataSource("DataSetEncuestasPacientes3", E3));
                            maestro.Universal.LocalReport.ReportEmbeddedResource = "ZamenisHealth.Reportes.Fibro.InformesEncuestas.ConsolidadoEncuestas.rdlc";
                            maestro.Universal.SetDisplayMode(DisplayMode.PrintLayout);
                            maestro.Universal.ZoomMode = ZoomMode.Percent;
                            maestro.Universal.ZoomPercent = 100;
                            maestro.Universal.LocalReport.EnableExternalImages = true;
                            maestro.Universal.Font = new Font("Arial", 8);
                            maestro.Universal.RefreshReport();
                            maestro.Universal.Visible = true;
                            maestro.Universal.Dock = System.Windows.Forms.DockStyle.Fill;
                            maestro.ShowDialog();
                        }
                        else
                        {
                            MG = new MensajesGeneral()
                            {
                                Mensaje = "No hay datos para exportar",
                                TipoImagen = 0
                            };

                            MG.ShowDialog();
                        }
                    }                    
                }
                else if (listBox1.SelectedItem.ToString() == "Informe Consolidado SF36")
                {
                    var getInforme = repoConsolidadoSF36.getInformeRDL(Convert.ToDateTime(dateTimePicker1.Value.Date),
                                                                                      Convert.ToDateTime(dateTimePicker2.Value.Date));
                    if (getInforme != null)
                    {
                        ConfigForm.GenerarReportViewer("DataSetConsolidadoSF36", "ZamenisHealth.Reportes.Fibro.InformesEncuestas.ConsolidadoSF36.rdlc", getInforme);
                    }
                    else
                    {
                        MG = new MensajesGeneral();
                        MG.TipoImagen = 1000;
                        MG.Mensaje = "No hay datos para exportar";
                        MG.ShowDialog();
                    }
                }
                else if (listBox1.SelectedItem.ToString() == "Informe Consolidado SF36 Porecentual")
                {
                    var getInforme = repoConsolidadoSF36Porcentual.getInformeRDLC(Convert.ToDateTime(dateTimePicker1.Value.Date),
                                                                                      Convert.ToDateTime(dateTimePicker2.Value.Date));
                    if (getInforme != null)
                    {
                        ConfigForm.GenerarReportViewer("DatasetSF36Porcentual", "ZamenisHealth.Reportes.Fibro.InformesEncuestas.ConsolidadoSF36Porcentual.rdlc", getInforme);
                    }
                    else
                    {
                        MG = new MensajesGeneral();
                        MG.TipoImagen = 1000;
                        MG.Mensaje = "No hay datos para exportar";
                        MG.ShowDialog();
                    }
                }
                else if (listBox1.SelectedItem.ToString() == "Informe Consolidado General")
                {
                    var getInforme = repoConsolidadoGeneral.generateReportRDLC(Convert.ToDateTime(dateTimePicker1.Value.Date),
                                                                                                     Convert.ToDateTime(dateTimePicker2.Value.Date));
                    if (getInforme != null)
                    {
                        ConfigForm.GenerarReportViewer("DatasetConsolidadoGeneral", "ZamenisHealth.Reportes.Fibro.InformesEncuestas.InformeFibro.rdlc", getInforme);
                    }
                    else
                    {
                        MG = new MensajesGeneral();
                        MG.TipoImagen = 1000;
                        MG.Mensaje = "No hay datos para exportar";
                        MG.ShowDialog();
                    }
                }
                else if (listBox1.SelectedItem.ToString() == "Informe Encuesta de Satisfaccion")
                {
                    var getInforme = repoEncuestaSatisfaccion.ExportarResGlobalGeneral(Convert.ToDateTime(dateTimePicker1.Value.Date),
                                                                                                     Convert.ToDateTime(dateTimePicker2.Value.Date));
                    if (getInforme != null)
                    {
                        ConfigForm.GenerarReportViewer("DataSetSatisfaccion", "ZamenisHealth.Reportes.Fibro.InformesEncuestas.EncuestaSatisfaccion.rdlc", getInforme);
                    }
                    else
                    {
                        MG = new MensajesGeneral();
                        MG.TipoImagen = 1000;
                        MG.Mensaje = "No hay datos para exportar";
                        MG.ShowDialog();
                    }
                }
                else if (listBox1.SelectedItem.ToString() == "Informe Porcentual Impacto de la Fibromialgia")
                {
                    var getInforme = repoImpactoFibromialgia.ExportarImpacto(Convert.ToDateTime(dateTimePicker1.Value.Date),
                                                                                          Convert.ToDateTime(dateTimePicker2.Value.Date));
                    if (getInforme != null)
                    {
                        ConfigForm.GenerarReportViewer("DataSetImpacto", "ZamenisHealth.Reportes.Fibro.InformesEncuestas.ImpactoFibromialgia.rdlc", getInforme);
                    }
                    else
                    {
                        MG = new MensajesGeneral();
                        MG.TipoImagen = 1000;
                        MG.Mensaje = "No hay datos para exportar";
                        MG.ShowDialog();
                    }
                }
                else if (listBox1.SelectedItem.ToString() == "Informe General Terapia Fisica")
                {
                    if (textBox1.Text == "")
                    {
                        MG = new MensajesGeneral();
                        MG.TipoImagen = 1000;
                        MG.Mensaje = "Seleccione tipo de documento del paciente y digite el numero de documento del paciente";
                        MG.ShowDialog();
                    }
                    else
                    {
                        var getPac = repoPac.LlamarPacienteNumDoc(textBox1.Text);
                        if (getPac != null)
                        {
                            DatosForInforme D = new DatosForInforme
                            {
                                Desde = Convert.ToDateTime(dateTimePicker1.Value.Date),
                                Hasta = Convert.ToDateTime(dateTimePicker2.Value.Date),
                                IDD = textBox1.Text,
                                Name = getPac.Pac_PrimerA + " " +
                                       getPac.Pac_SegundoA + " " +
                                       getPac.Pac_PrimerN + " " +
                                       getPac.Pac_SegundoN
                            };

                            var getInforme = repoTFGeneral.ExportarInforme1TF(D);
                            if (getInforme != null)
                            {
                                var getHistorys = repoTFGeneral.getAllTF(getPac.Pac_Id, Convert.ToDateTime(dateTimePicker1.Value.Date), Convert.ToDateTime(dateTimePicker2.Value.Date));
                                if (getHistorys != null)
                                {
                                    Encabezados();

                                    foreach (var i in getHistorys)
                                    {
                                        DataRow row = dt.NewRow();

                                        row[Admision] = i.HC_Adm.ToString();
                                        row[Fecha] = Convert.ToDateTime(i.HC_Fecha).ToString(Conexion.ConectionDictionary["Format_Fecha"]);
                                        row[Paciente] = i.HC_Pac.ToString();
                                        row[Tipo] = i.TipoHistoria.ToString();

                                        dt.Rows.Add(row);
                                        dt.AcceptChanges();
                                    }

                                    gridZH1.dataGridView1.DataSource = dt;
                                }
                                else
                                {
                                    Encabezados();
                                }

                                ConfigForm.GenerarReportViewer("DatasetInformeTF", "ZamenisHealth.Reportes.Fibro.InformesEncuestas.InformeTF.rdlc", getInforme);
                            }
                            else
                            {
                                Encabezados();

                                MG = new MensajesGeneral();
                                MG.TipoImagen = 1000;
                                MG.Mensaje = "No hay datos para exportar";
                                MG.ShowDialog();
                            }
                        }
                        else
                        {
                            MG = new MensajesGeneral();
                            MG.TipoImagen = 1000;
                            MG.Mensaje = "El documento digitado no existe";
                            MG.ShowDialog();
                        }
                    }                    
                }
                else
                {
                    MG = new MensajesGeneral();
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "No hay datos para exportar";
                    MG.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        void Encabezados()
        {
            gridZH1.dataGridView1.DataSource = null;
            dt = new DataTable();
            Admision = dt.Columns.Add("Admision", typeof(int));
            Fecha = dt.Columns.Add("Fecha", typeof(string));
            Paciente = dt.Columns.Add("Paciente", typeof(string));
            Tipo = dt.Columns.Add("Tipo", typeof(string));
        }
        void GridZH1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {
                if (listBox1.SelectedItem.ToString() == "Informes de Encuestas por Paciente")
                {
                    if (e.Button == MouseButtons.Right)
                    {
                        Posision = Convert.ToInt32(gridZH1.dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString());
                        TEncuesta = gridZH1.dataGridView1.Rows[e.RowIndex].Cells[3].Value.ToString();
                        Status = gridZH1.dataGridView1.Rows[e.RowIndex].Cells[5].Value.ToString();

                        Point posicionLocal = Cursor.Position;
                        contextMenuStrip1.Visible = true;
                        contextMenuStrip1.Location = new Point(posicionLocal.X, posicionLocal.Y);
                        contextMenuStrip1.Visible = true;
                    }
                }               
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        void GridZH1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0)
                {
                    AdmitionChange = Convert.ToInt32(gridZH1.dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString());
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }    
        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                MensajesGeneral Mg = new MensajesGeneral();

                if (AdmitionChange <= 0 || comboBox3.Text == "")
                {
                    Mg.TipoImagen = 1000;
                    Mg.Mensaje = "Debe seleccionar una historia para cambiar su estado y un estado de la lista desplegable";
                    Mg.ShowDialog();
                    return;
                }

                bool update = repoTFGeneral.updateTipoHistoria(AdmitionChange, comboBox3.Text);
                if (update != true)
                {
                    Mg.TipoImagen = 1000;
                    Mg.Mensaje = "No se logro actualizar la historia";
                    Mg.ShowDialog();
                }
                else
                {
                    Mg.TipoImagen = 3;
                    Mg.Mensaje = "Actualizado, haga click en buscar nuevamente para visualizar el cambio";
                    Mg.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void listBox1_Click(object sender, EventArgs e)
        {
            Encabezados();
        }
        private void cambiarEstadoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                bool changeStatus = false;

                if (Status == "Anulada")
                {
                    changeStatus = true;
                }

                bool actualizarEncuesta = repoEncuestasXPaciente.UpdateStatusEncuesta(Posision, changeStatus, TEncuesta);
                if (actualizarEncuesta == true)
                {
                    CargarEncuestas();

                    MG = new MensajesGeneral()
                    {
                        Mensaje = "Actualizado",
                        TipoImagen = 3
                    };

                    MG.ShowDialog();
                }
                else
                {
                    MG = new MensajesGeneral()
                    {
                        Mensaje = "No se logro actualiar la encuesta",
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
    }
}
