using Domain;
using Domain.CXN;
using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using ZamenisHealth.Clases;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.Recepcion.Extras
{
    public partial class PrintAgendas : Forma
    {        
        private static readonly IBodegas repoBodegas = new MBodegas();
        private static readonly IPacientes repoPacientes = new MPacientes();

        MensajesGeneral MG;
        string Namereport, Namedatasource;
        List<MReportes.PrntAgendas> ExportarXPAC;
        Thread thread;

        private ToolStripButton btnGenerar;

        DataTable dt;
        DataColumn CheckForSend;
        DataColumn Bodega;
        DataColumn Profesional;
        DataColumn Tipo;

        public PrintAgendas()
        {
            InitializeComponent();
        }

        void Encabezados()
        {
            gridZH1.dataGridView1.DataSource = null;
            dt = new DataTable();

            CheckForSend = dt.Columns.Add("✔", typeof(bool));
            Bodega = dt.Columns.Add("Bodega", typeof(int));
            Profesional = dt.Columns.Add("Profesional", typeof(string));
            Tipo = dt.Columns.Add("Tipo", typeof(string));
        }
        void CargarGrilla()
        {
            try
            {
                List<CXN_BODEGAS> listaMeds = repoBodegas.GetAllProfesionales("Todos");
                if (listaMeds != null)
                {
                    listaMeds = listaMeds.OrderBy(x => x.Bod_Tipo).ToList();
                    Encabezados();

                    foreach (CXN_BODEGAS i in listaMeds)
                    {
                        DataRow row = dt.NewRow();

                        row["✔"] = false;
                        row["Bodega"] = i.Bod_Numero;
                        row["Profesional"] = i.Bod_Responsable;
                        row["Tipo"] = i.Bod_Tipo == "CU" ? "CURACIONES" :
                            i.Bod_Tipo == "MG" ? "MEDICINA GENERAL" :
                            i.Bod_Tipo == "FI" ? "FISIATRIA" :
                            i.Bod_Tipo == "PS" ? "PSICOLOGIA" :
                            i.Bod_Tipo == "TO" ? "TERAPIA OCUPACIONAL" :
                            i.Bod_Tipo == "TF" ? "TERAPIA FISICA" :
                            "DESCONOCIDO";

                        dt.Rows.Add(row);
                        dt.AcceptChanges();
                    }

                    Estilos(gridZH1.dataGridView1, dt);                    
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
        void Estilos(DataGridView D, DataTable t)
        {
            D.EnableHeadersVisualStyles = false;
            D.ScrollBars = ScrollBars.Both;

            D.DataSource = t;            

            D.ReadOnly = false;
            D.Columns["✔"].ReadOnly = false;

            D.Columns["Bodega"].ReadOnly = true;
            D.Columns["Profesional"].ReadOnly = true;            

            gridZH1.dataGridView1.ClearSelection();
        }
        private void PrintAgendas_Load(object sender, EventArgs e)
        {
            try
            {
                Titulo.Text = "Agendas";
                LogoMain.Image = Properties.Resources.Splash;

                ToolStripButton btMarcar = new ToolStripButton();
                btMarcar = createToolButton("Marcar Todos");
                MenuLateral.Items.Add(btMarcar);
                btMarcar.Click += buttonMarcar_Click;

                ToolStripButton btDesMarcar = new ToolStripButton();
                btDesMarcar = createToolButton("Desmarcar Todos");
                MenuLateral.Items.Add(btDesMarcar);
                btDesMarcar.Click += buttonDesMarcar_Click;

                btnGenerar = new ToolStripButton();
                btnGenerar = createToolButton("Generar");
                MenuLateral.Items.Add(btnGenerar);
                btnGenerar.Click += button1_Click;

                gridZH1.CeldaHeight = true;
                groupBox3.Location = new System.Drawing.Point(142, 152);

                CargarDocumentos();
                CargarGrilla();
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        void CargarDocumentos()
        {
            var docs = repoPacientes.ListaDocs();
            foreach (string d in docs)
            {
                comboBox3.Items.Add(d);
            }
            comboBox3.SelectedIndex = 0;
        }
        private void buttonMarcar_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in gridZH1.dataGridView1.Rows)
            {
                object valor = row.Cells["✔"].Value;
                bool isChecked = valor != null && Convert.ToBoolean(valor);

                if (!isChecked)
                {
                    row.Cells["✔"].Value = true;
                }
            }
        }
        private void buttonDesMarcar_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in gridZH1.dataGridView1.Rows)
            {
                object valor = row.Cells["✔"].Value;
                bool isChecked = valor != null && Convert.ToBoolean(valor);

                if (isChecked)
                {
                    row.Cells["✔"].Value = false;
                }
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (panel1.Visible == true)
                {
                    gridZH1.dataGridView1.EndEdit();
                    List<string> listaProfesionales = new List<string>();

                    foreach (DataGridViewRow row in gridZH1.dataGridView1.Rows)
                    {
                        object valor = row.Cells["✔"].Value;
                        bool isChecked = valor != null && Convert.ToBoolean(valor);

                        if (isChecked)
                        {
                            listaProfesionales.Add(row.Cells["Bodega"].Value.ToString());
                        }
                    }

                    if (listaProfesionales.Count == 0)
                    {
                        MG = new MensajesGeneral
                        {
                            Mensaje = "No ha seleccionado ningun profesional",
                            TipoImagen = 1000
                        };
                        MG.ShowDialog();
                        return;
                    }

                    string selectedProfesionales = string.Join(",", listaProfesionales);

                    ExportarXPAC = new List<MReportes.PrntAgendas>();
                    ExportarXPAC = MReportes.PrntAgendas.Genera_Export_Citas("Mixto", dateTimePicker1.Value.Date, selectedProfesionales);

                    if (ExportarXPAC == null)
                    {
                        MG = new MensajesGeneral();
                        MG.TipoImagen = 1000;
                        MG.Mensaje = "No hay datos para exportar";
                        MG.ShowDialog();
                    }
                    else
                    {
                        Namedatasource = "DataSet_CitasGenerales";
                        Namereport = "ZamenisHealth.Reportes.RDLC_CitasGeneral.rdlc";
                        thread = new Thread(M);
                        thread.SetApartmentState(ApartmentState.STA); // Configura el subproceso en STA
                        thread.Start();
                    }
                }
                else if (groupBox3.Visible == true)
                {
                    ExportarXPAC = MReportes.PrntAgendas.Genera_CitasXPac_Report(textBox1.Text,
                                                                   dateTimePicker1.Value.Date,
                                                                   dateTimePicker2.Value.Date);


                    if (ExportarXPAC == null)
                    {
                        MG = new MensajesGeneral();
                        MG.TipoImagen = 1000;
                        MG.Mensaje = "No hay datos para exportar";
                        MG.ShowDialog();
                    }
                    else
                    {
                        Namedatasource = "DataSet_CitasGenerales";
                        Namereport = "ZamenisHealth.Reportes.RDLC_CitasXPAC.rdlc";
                        thread = new Thread(M);
                        thread.SetApartmentState(ApartmentState.STA); // Configura el subproceso en STA
                        thread.Start();
                    }                    
                }
                else
                {
                    MG = new MensajesGeneral();
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "Seleccione tipo de reporte valido";
                    MG.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }            
        }
        void M()
        {
            try
            {
                ConfigForm.GenerarReportViewer(Namedatasource, Namereport, ExportarXPAC);
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void boton3_Click(object sender, EventArgs e)
        {
            try
            {
                label2.Text = "Seleccione Fecha";
                panel1.Visible = true;
                groupBox3.Visible = false;
                label2.Visible = true;
                dateTimePicker1.Visible = true;

                dateTimePicker2.Visible = false;
                label3.Visible = false;

                gridZH1.dataGridView1.Columns["✔"].Width = 50;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }           
        }
        private void boton4_Click(object sender, EventArgs e)
        {
            label2.Text = "Seleccione Fecha Desde";
            panel1.Visible = false;
            groupBox3.Visible = true;

            label2.Visible = true;
            dateTimePicker1.Visible = true;
            dateTimePicker2.Visible = true;
            label3.Visible = true;
        }
        private void textBox1_DoubleClick(object sender, EventArgs e)
        {
            Comunes.BuscarPacientes buscarPacientes = new Comunes.BuscarPacientes();
            buscarPacientes.Tipo_Busca_Pac = "ExpAgenda";
            buscarPacientes.ShowDialog();
        }
    }
}
