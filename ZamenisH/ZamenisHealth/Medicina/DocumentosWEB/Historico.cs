using Domain.CXN;
using FormAndControls;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using ZamenisHealth.Clases;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.Medicina.DocumentosWEB
{
    public partial class Historico : Forma
    {
        private readonly ICompañia repoCia = new MCompañia();
        private readonly IDocumentosWEB repoDocWEB = new MDocumentosWEB();
        private int Cia;
        private MensajesGeneral MG;

        DataTable dt;
        DataColumn POS;
        DataColumn Id;
        DataColumn Tipo;
        DataColumn Paciente;
        DataColumn FechaCreacion;
        DataColumn FechaFirma;

        public Historico()
        {
            InitializeComponent();
        }
        private void Historico_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Historico de Envios";
            LogoMain.Image = Properties.Resources.Splash;

            ToolStripButton btnBuscar = new ToolStripButton();
            btnBuscar = createToolButton("Buscar");
            MenuLateral.Items.Add(btnBuscar);
            btnBuscar.Click += button1_Click;

            gridZH1.dataGridView1.CellDoubleClick += dataGridView1_CellDoubleClick;
            gridZH1.CeldaHeight = true;

            CargarCia();
        }
        void CargarCia()
        {
            List<CXN_CIA> getCias = repoCia.getAllCompañias();
            foreach (CXN_CIA c in getCias)
            {
                comboBox2.Items.Add(c.Com_Nombre);
            }

            comboBox2.SelectedIndex = 0;
        }
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                CXN_CIA C = repoCia.getPrestadorbyName(comboBox2.Text);
                if (C != null)
                {
                    Cia = C.Com_Identificador;
                }
                else
                {
                    Cia = 0;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        void Busqueda(string Parametro)
        {
            try
            {
                List<ConsentimientosTemp> lista = repoDocWEB.GetHechos(Cia, Parametro);
                if (lista != null)
                {
                    Encabezados();

                    int Contador = 1;

                    foreach (ConsentimientosTemp i in lista)
                    {
                        DataRow row = dt.NewRow();

                        row["POS"] = Contador;
                        row["Id"] = i.Id.ToString();
                        row["Tipo"] = i.TipoConsentimiento.ToString();
                        row["Paciente"] = i.NombrePaciente.ToString();
                        row["FechaCreacion"] = Convert.ToDateTime(i.FechaCreacion);
                        row["FechaFirma"] = i.FechaFirma.ToString();

                        dt.Rows.Add(row);
                        dt.AcceptChanges();

                        Contador = Contador + 1;
                    }

                    Contador = 1;
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
        private void button1_Click(object sender, EventArgs e)
        {
            Busqueda(textBox1.Text.Trim());
        }
        void Encabezados()
        {
            gridZH1.dataGridView1.DataSource = null;
            dt = new DataTable();
            POS = dt.Columns.Add("POS", typeof(int));
            Id = dt.Columns.Add("Id", typeof(int));
            Tipo = dt.Columns.Add("Tipo", typeof(string));
            Paciente = dt.Columns.Add("Paciente", typeof(string));
            FechaCreacion = dt.Columns.Add("FechaCreacion", typeof(DateTime));
            FechaFirma = dt.Columns.Add("FechaFirma", typeof(string));
        }
        void Estilos(DataGridView D, DataTable t)
        {
            D.DataSource = t;
            D.Columns["POS"].Visible = false;
            D.Columns["Id"].Visible = false;
        }
        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                int Posision = Convert.ToInt32(gridZH1.dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString());
                List<ConsentimientosTemp> getReport = repoDocWEB.Export(Posision);
                if (getReport != null)
                {
                    foreach (ConsentimientosTemp i in getReport)
                    {
                        if (i.FechaFirma == "SIN FIRMAR")
                        {                           
                            MG = new MensajesGeneral();
                            MG.Mensaje = "Este consentimiento aun no esta firmado por el paciente";
                            MG.TipoImagen = 1000;
                            MG.ShowDialog();
                            return;
                        }                    
                    }

                    if (getReport[0].TipoConsentimiento == "Consentimiento Informado Curaciones")
                    {
                        ConfigForm.GenerarReportViewer("DataSetConsentimientos",
                                                       "ZamenisHealth.Reportes.RDLC_ConsentimientoWEB.rdlc",
                                                       getReport);
                    }
                    else if (getReport[0].TipoConsentimiento == "Consentimiento Informado Fibromialgia")
                    {
                        ConfigForm.GenerarReportViewer("DataSetConsentimientos",
                                                       "ZamenisHealth.Reportes.RDLC_ContentimientosWEBPSI.rdlc",
                                                       getReport);
                    }
                    else
                    {
                        MG = new MensajesGeneral();
                        MG.Mensaje = "Este consentimiento aun no esta firmado por el paciente";
                        MG.TipoImagen = 1000;
                        MG.ShowDialog();
                    }                    
                }
                else
                {
                    MG = new MensajesGeneral();
                    MG.Mensaje = "No se logro exportar el reporte";
                    MG.TipoImagen = 1000;
                    MG.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            Busqueda("");
        }
    }
}
