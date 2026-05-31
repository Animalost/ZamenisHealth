using System;
using System.Drawing;
using System.Windows.Forms;
using Domain;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using Persistence.Fibromialgia.Interfaces;
using Persistence.Fibromialgia.Metodos;
using ZamenisHealth.Clases;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.Fibromialgia.Informes
{
    public partial class InformesPrograma : ConfigForm.BaseForm
    {
        private static readonly IPacientes repoPac = new MPacientes();
        private static readonly IJuntas repoJuntas = new MJuntas();
        private static readonly IConsolidadoAIPEA repoConsolidadoAIPEA = new MConsolidadoAIPEA();

        private MensajesGeneral MG;

        public InformesPrograma()
        {
            InitializeComponent();
        }

        public void setDoc(string _tid, string _nid)
        {
            comboBox2.Text = _tid;
            textBox1.Text = _nid;
        }

        private void InformesJuntas_Load(object sender, EventArgs e)
        {
            try
            {
                this.Titulo.Text = "Configuracion de Generacion de Informes";

                var Docs = repoPac.ListaDocs();
                if (Docs != null)
                {
                    foreach (var i in Docs)
                    {
                        comboBox2.Items.Add(i);
                    }
                }

                panel1.Location = new Point(12, 92);
                panel2.Location = new Point(12, 92);
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            MG = new MensajesGeneral();

            switch (comboBox1.SelectedIndex)
            {
                case 1: //Egreso
                    panel1.Visible = true;
                    panel2.Visible = false;
                    break;

                case 2: //Incluir
                    panel1.Visible = true;
                    panel2.Visible = false;
                    break;

                case 3: //Informe
                    panel1.Visible = false;
                    panel2.Visible = true;
                    break;

                default:
                    panel2.Visible = false;
                    panel1.Visible = false;
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "Seleccione una opcion valida";
                    MG.ShowDialog();
                    break;
            }
        }
        void Encabezados()
        {
            listView1.Clear();
            listView1.View = View.Details;
            listView1.GridLines = true;
            listView1.FullRowSelect = true;
            listView1.Columns.Add("Admision", 80, HorizontalAlignment.Left);
            listView1.Columns.Add("Fecha", 80, HorizontalAlignment.Left);
            listView1.Columns.Add("Profesional", 250, HorizontalAlignment.Left);
            listView1.Columns.Add("Paciente", 250, HorizontalAlignment.Left);
            listView1.Columns.Add("Tipo", 80, HorizontalAlignment.Left);
            listView1.Columns.Add("Estado Junta", 80, HorizontalAlignment.Left);
        }

        void Encabezados2()
        {
            listView1.Clear();
            listView1.View = View.Details;
            listView1.GridLines = true;
            listView1.FullRowSelect = true;
            listView1.Columns.Add("IdPac", 0, HorizontalAlignment.Left);
            listView1.Columns.Add("Paciente", 300, HorizontalAlignment.Left);
            listView1.Columns.Add("Estado", 80, HorizontalAlignment.Left);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                MG = new MensajesGeneral();

                if (comboBox1.Text == "" || comboBox2.Text == "" || textBox1.Text == "")
                {
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "Seleccione las opciones de las listas desplegables y digite un documento de paciente valido";
                    MG.ShowDialog();
                    return;
                }
                else
                {
                    switch (comboBox1.SelectedIndex)
                    {
                        case 1:
                            var getListas = repoJuntas.getJuntasForComplete(comboBox2.Text, textBox1.Text, true);
                            if (getListas != null)
                            {
                                Encabezados();

                                foreach (var i in getListas)
                                {
                                    listView1.Items.Add(new ListViewItem(new string[]
                                    {
                                        i.Jun_Adm.ToString(),
                                        Convert.ToDateTime(i.Jun_Fecha).ToString(Conexion.ConectionDictionary["Format_Fecha"]),
                                        i.Jun_Con_FI.ToString(),
                                        i.Jun_Observa.ToString(),
                                        i.Jun_Tipo.ToString(),
                                        i.Jun_Egresa.ToString()
                                    }));
                                }
                            }
                            else
                            {
                                MG.TipoImagen = 1000;
                                MG.Mensaje = "No hay resultados para esta busqueda";
                                MG.ShowDialog();

                                Encabezados();
                            }
                            break;

                        case 2:
                            var getpac = repoPac.LlamarPacienteDOC(comboBox2.Text, textBox1.Text);
                            if (getpac != null)
                            {
                                Encabezados2();

                                listView1.Items.Add(new ListViewItem(new string[]
                                {
                                    getpac.Pac_Id.ToString(),
                                    getpac.Pac_PrimerA + " " + getpac.Pac_SegundoA + " " +
                                    getpac.Pac_PrimerN + " " + getpac.Pac_SegundoN,
                                    getpac.Pac_FibInf.ToString()
                                }));

                            }
                            else
                            {
                                MG.TipoImagen = 1000;
                                MG.Mensaje = "No hay resultados para esta busqueda";
                                MG.ShowDialog();

                                Encabezados2();
                            }

                            break;

                        default:
                            MG.TipoImagen = 1000;
                            MG.Mensaje = "La opcion inicial elegida ha cambiado";
                            MG.ShowDialog();
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void textBox1_DoubleClick(object sender, EventArgs e)
        {
            BuscarPacientes BP = new BuscarPacientes("InfFibroGen");
            BP.ShowDialog();
        }

        private void listView1_Click(object sender, EventArgs e)
        {
            try
            {
                MG = new MensajesGeneral();

                switch (comboBox1.SelectedIndex)
                {
                    case 1:
                        EgresoJuntas E = new EgresoJuntas(Convert.ToInt32(listView1.SelectedItems[0].SubItems[0].Text));
                        E.ShowDialog();
                        break;

                    case 2:

                        string est = (listView1.SelectedItems[0].SubItems[2].Text == "Incluido") ? "Excluir" : "Incluir";



                        DialogResult result = MessageBox.Show("¿Desea " + est + " este paciente para los informes de Fibromialgia?",
                                                              "Zamenis Health - Inclusion/Exclusion del Programa Fibromialgia",
                                                              MessageBoxButtons.YesNo,
                                                              MessageBoxIcon.Question);

                        if (result == DialogResult.Yes)
                        {
                            bool exIn = repoPac.UpdatePacFibro(Convert.ToInt32(listView1.SelectedItems[0].SubItems[0].Text),
                                                   listView1.SelectedItems[0].SubItems[2].Text);
                            if (exIn != true)
                            {
                                MG.TipoImagen = 1000;
                                MG.Mensaje = "No se logro " + est + " el paciente del informe";
                                MG.ShowDialog();
                            }
                            else
                            {
                                MG.TipoImagen = 3;
                                MG.Mensaje = "Hecho";
                                MG.ShowDialog();
                            }
                        }

                        break;

                    default:
                        MG.TipoImagen = 1000;
                        MG.Mensaje = "La opcion inicial elegida ha cambiado";
                        MG.ShowDialog();
                        break;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        int getMonth(string Month)
        {
            switch (Month)
            {
                case "ENERO":
                    return 1;
                case "FEBRERO":
                    return 1;
                case "MARZO":
                    return 1;
                case "ABRIL":
                    return 1;
                case "MAYO":
                    return 1;
                case "JUNIO":
                    return 1;
                case "JULIO":
                    return 1;
                case "AGOSTO":
                    return 1;
                case "SEPTIEMBRE":
                    return 1;
                case "OCTUBRE":
                    return 1;
                case "NOVIEMBRE":
                    return 1;
                case "DICIEMBRE":
                    return 1;
                default:
                    return 0;
            }
        }
        int getMonthLastDay(string Month)
        {
            switch (Month)
            {
                case "ENERO":
                    return 31;
                case "FEBRERO":
                    return 28;
                case "MARZO":
                    return 31;
                case "ABRIL":
                    return 30;
                case "MAYO":
                    return 31;
                case "JUNIO":
                    return 30;
                case "JULIO":
                    return 31;
                case "AGOSTO":
                    return 31;
                case "SEPTIEMBRE":
                    return 30;
                case "OCTUBRE":
                    return 31;
                case "NOVIEMBRE":
                    return 30;
                case "DICIEMBRE":
                    return 31;
                default:
                    return 0;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                MG = new MensajesGeneral();

                DateTime Desde = new DateTime(Convert.ToInt32(comboBox5.Text), getMonth(comboBox3.Text), 1);
                DateTime Hasta = new DateTime(Convert.ToInt32(comboBox5.Text), getMonth(comboBox3.Text), getMonthLastDay(comboBox3.Text));

                var getDatos = repoConsolidadoAIPEA.getEgresos(Desde.Date, Hasta.Date);
                if (getDatos != null)
                {
                    ConfigForm.GenerarReportViewer("DataSetAIPEA",
               "ZamenisHealth.Reportes.Fibro.InformeGeneral.Egresos.rdlc",
               getDatos);

                    return;
                }
                else
                {
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "No hay resultados para el reporte acorde a las fechas seleccionadas";
                    MG.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                MG = new MensajesGeneral();

                DateTime Desde = new DateTime(Convert.ToInt32(comboBox5.Text), getMonth(comboBox3.Text), 1);
                DateTime Hasta = new DateTime(Convert.ToInt32(comboBox5.Text), getMonth(comboBox3.Text), getMonthLastDay(comboBox3.Text));

                var getDatos = repoConsolidadoAIPEA.getPrevalentes(Desde.Date, Hasta.Date);
                if (getDatos != null)
                {
                    ConfigForm.GenerarReportViewer("DataSetAIPEA",
               "ZamenisHealth.Reportes.Fibro.InformeGeneral.Prevalentes.rdlc",
               getDatos);

                    return;
                }
                else
                {
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "No hay resultados para el reporte acorde a las fechas seleccionadas";
                    MG.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                MG = new MensajesGeneral();

                DateTime Desde = new DateTime(Convert.ToInt32(comboBox5.Text), getMonth(comboBox3.Text), 1);
                DateTime Hasta = new DateTime(Convert.ToInt32(comboBox5.Text), getMonth(comboBox3.Text), getMonthLastDay(comboBox3.Text));

                var getDatos = repoConsolidadoAIPEA.getActivosYbase(Desde.Date, Hasta.Date);
                if (getDatos != null)
                {
                    ConfigForm.GenerarReportViewer("DataSetAIPEA",
               "ZamenisHealth.Reportes.Fibro.InformeGeneral.ActivosBase.rdlc",
               getDatos);

                    return;
                }
                else
                {
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "No hay resultados para el reporte acorde a las fechas seleccionadas";
                    MG.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
                MG = new MensajesGeneral();

                DateTime Desde = new DateTime(Convert.ToInt32(comboBox5.Text), getMonth(comboBox3.Text), 1);
                DateTime Hasta = new DateTime(Convert.ToInt32(comboBox5.Text), getMonth(comboBox3.Text), getMonthLastDay(comboBox3.Text));

                var getDatos = repoConsolidadoAIPEA.getAdherencia(Desde.Date, Hasta.Date);
                if (getDatos != null)
                {
                    ConfigForm.GenerarReportViewer("DataSetAIPEA",
               "ZamenisHealth.Reportes.Fibro.InformeGeneral.Adherencia.rdlc",
               getDatos);

                    return;
                }
                else
                {
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "No hay resultados para el reporte acorde a las fechas seleccionadas";
                    MG.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            try
            {
                MG = new MensajesGeneral();

                DateTime Desde = new DateTime(Convert.ToInt32(comboBox5.Text), getMonth(comboBox3.Text), 1);
                DateTime Hasta = new DateTime(Convert.ToInt32(comboBox5.Text), getMonth(comboBox3.Text), getMonthLastDay(comboBox3.Text));

                var getDatos = repoConsolidadoAIPEA.getIngresos(Desde.Date, Hasta.Date);
                if (getDatos != null)
                {
                    ConfigForm.GenerarReportViewer("DataSetAIPEA",
               "ZamenisHealth.Reportes.Fibro.InformeGeneral.Ingresos.rdlc",
               getDatos);

                    return;
                }
                else
                {
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "No hay resultados para el reporte acorde a las fechas seleccionadas";
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
