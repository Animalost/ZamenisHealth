using Domain;
using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using Persistence.Informes.Interfaces;
using Persistence.Informes.Methods;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZamenisHealth.Clases;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.Gerenciales
{
    public partial class ReportesGerenciales : Forma
    {
        private static readonly ICargos repositorioCargos = new MCargos();
        private static readonly IOrdenes repositorioOrdenes = new MOrdenes();
        private static readonly IPacientes repositorioPacientes = new MPacientes();
        private static readonly IReportes repositorioReportes = new MReportes();
        private static readonly IInformeEstadistico repoEst = new MInformeEstadistico();
        private static readonly IGenerales repoGenerales = new MGenerales();

        private MensajesGeneral MG;
        private string T;
        private ToolStripButton boton2;

        public ReportesGerenciales()
        {
            InitializeComponent();
        }

        private void ReportesGerenciales_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Reportes Gerencia";
            LogoMain.Image = Properties.Resources.Splash;
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";            

            boton2 = new ToolStripButton();
            boton2 = createToolButton("Generar");
            MenuLateral.Items.Add(boton2);
            boton2.Click += btnGenerar_Click;
        }
        void btnGenerar_Click(object sender, EventArgs e)
        {
            try
            {
                string Tipo_Rep = comboBox2.Text;
                switch (Tipo_Rep)
                {
                    case "Reporte de Recepcion":
                        Rpt_Rece_Ger();
                        break;

                    case "Reporte de Ingresos":
                        Rpt_Ing_Ger();
                        break;

                    case "Reporte Cargos Total":
                        Rpt_Car_Tot();
                        break;

                    case "Reporte de Atenciones de Profesionales":
                        Rpt_Atenciones();
                        break;

                    case "Reporte de Atenciones de Pacientes (Informe)":
                        Rpt_Atn_Pac();
                        break;

                    case "Reporte de Ordenes Medicas":
                        Rpt_Ordenes();
                        break;

                    case "Cargos por Paciente":
                        DialogResult result = MessageBox.Show("Seleccione SI para generar reporte por un paciente especifico, seleccione NO para generar " +
                            "reporte total de pacientes de acuerdo a las fechas seleccionadas",
                                                  "Zamenis Health - Reportes de Cargos",
                                                  MessageBoxButtons.YesNo,
                                                  MessageBoxIcon.Question);

                        if (result == DialogResult.Yes)
                        {
                            string Documento = Microsoft.VisualBasic.Interaction.InputBox(
                                "Digite el numero de documento del paciente sin puntos, ni comas " +
                                "Cargos por Pacientes",
                                "Cargos por Pacientes TXT",
                                    "");
                            if (Documento != "")
                            {
                                //Individual


                                repositorioCargos.Individual(dateTimePicker1.Value, dateTimePicker2.Value, Documento);


                                MG = new MensajesGeneral();
                                MG.TipoImagen = 3;
                                MG.Mensaje = "Generado en C: Cxn Reportes " + Documento + "-Informe.txt";
                                MG.ShowDialog();

                                return;
                            }
                        }
                        if (result == DialogResult.No)
                        {
                            //Total


                            repositorioCargos.Total(dateTimePicker1.Value, dateTimePicker2.Value);


                            MG = new MensajesGeneral();
                            MG.TipoImagen = 3;
                            MG.Mensaje = "Generado en C: Cxn Reportes CargosTotal-Informe.txt";
                            MG.ShowDialog();
                        }

                        break;

                    case "Reporte Cancelacion de Citas":
                        ReportCan();
                        break;

                    case "Reporte Recuperacion Pacientes":
                        this.T = "DelMes";
                        GeneraSalida();
                        break;

                    case "Reporte Salida Pacientes":
                        this.T = "Salidas";
                        GeneraSalida();
                        break;

                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        async void GeneraSalida()
        {
            Shows();
            Task oTask = new Task(RecPaciente);
            oTask.Start();
            await oTask;
            Hides();
        }
        void Shows()
        {
            pictureBox1.Visible = true;
            boton2.Enabled = false;
        }
        void Hides()
        {
            pictureBox1.Visible = false;
            boton2.Enabled = true;
        }
        void ReportCan()
        {
            try
            {
                repositorioPacientes.RptCancelaciones(dateTimePicker1.Value.Date, dateTimePicker2.Value.Date);

                MessageBox.Show("Generado en C CXN Reportes CANCELACIONES_.xls");
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void Rpt_Ordenes()
        {
            try
            {
                repositorioOrdenes.Rpt_Ordenes(dateTimePicker1.Value.Date, dateTimePicker2.Value.Date);

                MessageBox.Show("Generado en C CXN Reportes ORDENES");
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void Rpt_Atn_Pac()
        {
            try
            {
                repositorioPacientes.Rpt_Atenciones2(dateTimePicker1.Value.Date, dateTimePicker2.Value.Date);

                MessageBox.Show("Generado en C CXN Reportes Informe");
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void Rpt_Atenciones()
        {
            try
            {
                List<GerencialR> Export = repositorioReportes.Rpt_Atenciones(dateTimePicker1.Value.Date, dateTimePicker2.Value.Date);

                if (Export == null)
                {
                    MessageBox.Show("Sin Resultados", "Sin Resultados", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                ConfigForm.GenerarReportViewer("DataSet_Gerencial",
                                           "ZamenisHealth.Reportes.RDLC_Atenciones.rdlc",
                                           Export);

            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void Rpt_Ing_Ger()
        {
            try
            {
                repositorioReportes.Rpt_Ing_Ger(dateTimePicker1.Value, dateTimePicker2.Value);

                MessageBox.Show("Generado en C CXN Reportes Ingresos");
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void Rpt_Car_Tot()
        {
            try
            {
                repositorioCargos.Rpt_Car_Tot(dateTimePicker1.Value, dateTimePicker2.Value);
                MessageBox.Show("Generado en C CXN Reportes Cargos_");
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void Rpt_Rece_Ger()
        {
            try
            {
                repositorioReportes.Rpt_Rece_Ger(dateTimePicker1.Value, dateTimePicker2.Value);

                MessageBox.Show("Generado en C CXN Reportes");
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        void RecPaciente()
        {
            try
            {
                List<ExportInExcel> getLista = repoEst.RptRecPaciente(dateTimePicker1.Value.Date, dateTimePicker2.Value.Date, this.T);

                if (getLista != null)
                {
                    DataTable dt = new DataTable();
                    DataColumn FINGRESO = dt.Columns.Add("FINGRESO", typeof(string));
                    DataColumn PACIENTE = dt.Columns.Add("PACIENTE", typeof(string));
                    DataColumn DOCUMENTO = dt.Columns.Add("DOCUMENTO", typeof(string));
                    DataColumn DIAGNOSTICO = dt.Columns.Add("DIAGNOSTICO", typeof(string));
                    DataColumn COMPLEJIDADINICIAL = dt.Columns.Add("COMPLEJIDAD_INICIAL", typeof(string));
                    DataColumn CANTIDADHERIDAS = dt.Columns.Add("CANTIDAD_HERIDAS", typeof(string));
                    DataColumn TAMAÑOHERIDAS = dt.Columns.Add("TAMAÑO_HERIDAS", typeof(string));
                    DataColumn ZONAANATOMICADELESION = dt.Columns.Add("ZONA_ANATOMICA_DE_LESION", typeof(string));
                    DataColumn MANEJOINICIAL = dt.Columns.Add("MANEJO_INICIAL", typeof(string));
                    DataColumn DURACIONMANEJOENTIEMPO = dt.Columns.Add("DURACION_MANEJO_EN_TIEMPO", typeof(string));
                    DataColumn FECHADESALIDA = dt.Columns.Add("FECHA_DE_SALIDA", typeof(string));
                    DataColumn ULTIMOCAMBIODEMANEJOPORENFERMERO = dt.Columns.Add("ULTIMO_CAMBIO_DE_MANEJO_POR_ENFERMERO", typeof(string));
                    DataColumn RESULTADODEEPITELIOGANADOPORDIA = dt.Columns.Add("RESULTADO_DE_EPITELIO_GANADO_POR_DIA", typeof(string));
                    DataColumn PATOLOGIA = dt.Columns.Add("PATOLOGIA", typeof(string));
                    DataColumn INGRESOPATOLOGIA = dt.Columns.Add("INGRESOPATOLOGIA", typeof(string));
                    DataColumn SALIDAPATOLOGIA = dt.Columns.Add("SALIDAPATOLOGIA", typeof(string));
                    DataColumn PORCENTAJEDERECUPERACIONPORETIOLOGIA = dt.Columns.Add("PORCENTAJE_DE_RECUPERACION_POR_ETIOLOGIA", typeof(string));

                    foreach (ExportInExcel i in getLista)
                    {
                        double pRec = 0;
                        string ing = "0";
                        string sal = "0";

                        if (i.Dato1 != "")
                        {
                            if (i.Dato27.ContainsKey("INGRESO_" + Convert.ToDateTime(i.Dato1).ToString("MM-yyyy") + i.Dato15) == true)
                            {
                                if (i.Dato27.ContainsKey("SALIDA_" + Convert.ToDateTime(i.Dato1).ToString("MM-yyyy") + i.Dato15) == true)
                                {
                                    double ValIng = i.Dato27["INGRESO_" + Convert.ToDateTime(i.Dato1).ToString("MM-yyyy") + i.Dato15];
                                    double ValSal = i.Dato27["SALIDA_" + Convert.ToDateTime(i.Dato1).ToString("MM-yyyy") + i.Dato15];

                                    pRec = ((((((((ValIng - ValSal)) / ValIng)) * 100)) - 100)) * -1;

                                    ing = i.Dato27["INGRESO_" + Convert.ToDateTime(i.Dato1).ToString("MM-yyyy") + i.Dato15].ToString();
                                    sal = i.Dato27["SALIDA_" + Convert.ToDateTime(i.Dato1).ToString("MM-yyyy") + i.Dato15].ToString();
                                }
                                else
                                {
                                    ing = i.Dato27["INGRESO_" + Convert.ToDateTime(i.Dato1).ToString("MM-yyyy") + i.Dato15].ToString();
                                    sal = "0";
                                    pRec = 0;
                                }
                            }
                        }

                        DataRow row = dt.NewRow();

                        row["FINGRESO"] = i.Dato1;
                        row["PACIENTE"] = i.Dato2;
                        row["DOCUMENTO"] = i.Dato3;
                        row["DIAGNOSTICO"] = i.Dato4;
                        row["COMPLEJIDAD_INICIAL"] = i.Dato5;
                        row["CANTIDAD_HERIDAS"] = i.Dato6;
                        row["TAMAÑO_HERIDAS"] = i.Dato7;
                        row["ZONA_ANATOMICA_DE_LESION"] = i.Dato8;
                        row["MANEJO_INICIAL"] = i.Dato9;
                        row["DURACION_MANEJO_EN_TIEMPO"] = i.Dato10;
                        row["FECHA_DE_SALIDA"] = i.Dato11;
                        row["ULTIMO_CAMBIO_DE_MANEJO_POR_ENFERMERO"] = i.Dato12;
                        row["RESULTADO_DE_EPITELIO_GANADO_POR_DIA"] = i.Dato13;
                        row["PATOLOGIA"] = i.Dato15;
                        row["INGRESOPATOLOGIA"] = ing.ToString();
                        row["SALIDAPATOLOGIA"] = sal.ToString();
                        row["PORCENTAJE_DE_RECUPERACION_POR_ETIOLOGIA"] = pRec.ToString("N2");

                        dt.Rows.Add(row);
                        dt.AcceptChanges();
                    }

                    dataGridView1.DataSource = dt;
                    repoGenerales.ExportarGrilla(dataGridView1);

                    MessageBox.Show("Generado, si no puede visualizar el archivo, abra en Excel una hoja en blanco y vuelva a generar el reporte");
                }
                else
                {
                    MessageBox.Show("No hay resultados con estas fechas seleccionadas");
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox2.Text) 
            {
                case "Reporte Encuestas de Satisfaccion":
                    EncuestasSatis encuestasSatis = new EncuestasSatis();
                    encuestasSatis.ShowDialog();
                    break;

                case "Informe Profesional Gerencia de Atenciones":
                    InformeGerenciaAtenciones informeGerenciaAtenciones = new InformeGerenciaAtenciones();
                    informeGerenciaAtenciones.ShowDialog();
                    break;

                case "Informe Mensual Productividad":
                    InformeEnfermeria informeEnfermeria = new InformeEnfermeria();
                    informeEnfermeria.ShowDialog();
                    break;

                default:
                    break;
            }
        }
    }
}
