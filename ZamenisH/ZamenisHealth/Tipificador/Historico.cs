using Domain.CXN;
using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using ZamenisHealth.Clases;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.Tipificador
{
    public partial class Historico : Forma
    {
        private ITipificador tipificador;

        DataTable dt = new DataTable();
        DataColumn POS;
        DataColumn Id;
        DataColumn PersonaLlama;
        DataColumn Paciente;
        DataColumn Fecha;
        DataColumn Hora;

        private MensajesGeneral MG;

        public Historico()
        {
            InitializeComponent();
            tipificador = new MTipificador();
        }

        private void Historico_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Historial";
            LogoMain.Image = Properties.Resources.Splash;
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";

            ToolStripButton btnFiltrar;
            ToolStripButton btnExportar;

            btnFiltrar = new ToolStripButton();
            btnFiltrar = createToolButton("Filtrar");
            MenuLateral.Items.Add(btnFiltrar);
            btnFiltrar.Click += toolStripButton1_Click;

            btnExportar = new ToolStripButton();
            btnExportar = createToolButton("Filtrar");
            MenuLateral.Items.Add(btnExportar);
            btnExportar.Click += toolStripButton3_Click;
        }
       
        void Estilos(DataGridView D, DataTable t)
        {
            D.EnableHeadersVisualStyles = false;
            D.ScrollBars = ScrollBars.Both;

            D.DataSource = t;

            D.Columns["PersonaLlama"].Width = 280;
            D.Columns["Paciente"].Width = 300;
            D.Columns["Fecha"].Width = 150;
            D.Columns["Hora"].Width = 150;

            D.ColumnHeadersDefaultCellStyle.Font = new Font(D.Font, FontStyle.Bold);
            D.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 8, FontStyle.Bold);
            D.ColumnHeadersDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#bfdbff");
            D.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;
            D.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;

            D.Columns["PersonaLlama"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["Paciente"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["Fecha"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["Hora"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

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

        void Encabezados()
        {
            dt = new DataTable();
            POS = dt.Columns.Add("POS", typeof(int));
            Id = dt.Columns.Add("Id", typeof(int));
            Fecha = dt.Columns.Add("Fecha", typeof(DateTime));
            Hora = dt.Columns.Add("Hora", typeof(DateTime));
            PersonaLlama = dt.Columns.Add("PersonaLlama", typeof(string));
            Paciente = dt.Columns.Add("Paciente", typeof(string));            
        }
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            try
            {
                if (comboBox1.Text == "" || comboBox2.Text == "")
                {
                    MG = new MensajesGeneral();
                    MG.Mensaje = "Debe seleccionar un mes y año para filtrar";
                    MG.TipoImagen = 1000;
                    MG.ShowDialog();
                }
                else
                {
                    DateTime desde = new DateTime(Convert.ToInt32(comboBox2.Text), getMonthNumber(comboBox1.Text), 1);
                    DateTime hasta = new DateTime(Convert.ToInt32(comboBox2.Text), getMonthNumber(comboBox1.Text), getMonthLastDay(comboBox1.Text));

                    List<CXN_TIPIFICADOR> reporte = tipificador.Reporte(desde, hasta);
                    if (reporte != null)
                    {
                        Encabezados();

                        int Contador = 1;

                        foreach (CXN_TIPIFICADOR i in reporte)
                        {
                            DataRow row = dt.NewRow();

                            row["POS"] = Contador;
                            row["Id"] = i.Id.ToString();
                            row["Fecha"] = Convert.ToDateTime(i.Fecha);
                            row["Hora"] = Convert.ToDateTime(i.Hora);
                            row["PersonaLlama"] = i.NombreLlama.ToString();
                            row["Paciente"] = i.NombrePaciente;

                            dt.Rows.Add(row);
                            dt.AcceptChanges();

                            Contador = Contador + 1;
                        }

                        Contador = 1;
                        Estilos(dataGridView1, dt);
                    }
                    else
                    {
                        MG = new MensajesGeneral();
                        MG.Mensaje = "No hay resultados para este mes y año seleccionados";
                        MG.TipoImagen = 3;
                        MG.ShowDialog();
                    }                   
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            try
            {
                if (comboBox1.Text == "" || comboBox2.Text == "")
                {
                    MG = new MensajesGeneral();
                    MG.Mensaje = "Debe seleccionar un mes y año para filtrar";
                    MG.TipoImagen = 1000;
                    MG.ShowDialog();
                }
                else
                {
                    DateTime desde = new DateTime(Convert.ToInt32(comboBox2.Text), getMonthNumber(comboBox1.Text), 1);
                    DateTime hasta = new DateTime(Convert.ToInt32(comboBox2.Text), getMonthNumber(comboBox1.Text), getMonthLastDay(comboBox1.Text));

                    List<CXN_TIPIFICADOR> reporte = tipificador.Reporte(desde, hasta);
                    if (reporte != null)
                    {
                        ConfigForm.GenerarReportViewer("DataSet_Tipificador", "ZamenisHealth.Reportes.RDLC_Tipificador.rdlc", reporte);
                    }
                    else
                    {
                        MG = new MensajesGeneral();
                        MG.Mensaje = "No hay resultados para este mes y año seleccionados para exportar";
                        MG.TipoImagen = 3;
                        MG.ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
