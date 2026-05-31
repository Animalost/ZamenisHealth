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
using ZamenisHealth.Comunes;

namespace ZamenisHealth.IAS
{
    public partial class Historial : Forma
    {
        private readonly IIAS ias;

        DataTable dt = new DataTable();
        DataColumn POS;
        DataColumn Pregunta;
        DataColumn Fecha;
        DataColumn Id;

        public Historial()
        {
            ias = new MIAS();
            InitializeComponent();
        }

        private void Historial_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Historial";
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";
            LogoMain.Image = Properties.Resources.Splash;

            ToolStripButton btnRefresh = new ToolStripButton();
            btnRefresh = createToolButton("Refrescar");
            MenuLateral.Items.Add(btnRefresh);
            btnRefresh.Click += toolStripButton1_Click;

            modernPanel1.Location = new Point(143, 73);
            modernPanel1.Size = new Size(690, 493);
            CargarHistorial();
        }
        void CargarHistorial()
        {
            try
            {
                List<CXN_HISTORYIA> _lista = ias.Historial(Contenedor.UsuarioLogueado);
                if (_lista != null)
                {
                    Encabezados();

                    int Contador = 1;

                    foreach (CXN_HISTORYIA i in _lista)
                    {
                        DataRow row = dt.NewRow();

                        row["POS"] = Contador;
                        row["Id"] = Convert.ToInt32(i.Id);
                        row["Fecha"] = Convert.ToDateTime(i.FechaHora).ToString();
                        row["Pregunta"] = i.Consulta.ToString();
                        

                        dt.Rows.Add(row);
                        dt.AcceptChanges();

                        Contador = Contador + 1;
                    }

                    Contador = 1;
                    Estilos(dataGridView1, dt);
                }
                else
                {                    
                    Encabezados();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar el historial: " + ex.Message, "Zamenis Health", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        void Encabezados()
        {
            dataGridView1.DataSource = null;
            dt = new DataTable();
            POS = dt.Columns.Add("POS", typeof(int));
            Id = dt.Columns.Add("Id", typeof(int));
            Fecha = dt.Columns.Add("Fecha", typeof(string));
            Pregunta = dt.Columns.Add("Pregunta", typeof(string));           
        }
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            CargarHistorial();
        }
        void Estilos(DataGridView D, DataTable t)
        {
            D.EnableHeadersVisualStyles = false;
            D.ScrollBars = ScrollBars.Both;

            D.DataSource = t;

            D.Columns["Id"].Width = 80;
            D.Columns["Fecha"].Width = 600;
            D.Columns["Pregunta"].Width = 600;

            D.ColumnHeadersDefaultCellStyle.Font = new Font(D.Font, FontStyle.Bold);
            D.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 8, FontStyle.Bold);
            D.ColumnHeadersDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#bfdbff");
            D.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;
            D.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;

            D.Columns["Id"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["Fecha"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["Pregunta"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            D.Columns["Id"].SortMode = DataGridViewColumnSortMode.NotSortable;
            D.Columns["Fecha"].SortMode = DataGridViewColumnSortMode.NotSortable;
            D.Columns["Pregunta"].SortMode = DataGridViewColumnSortMode.NotSortable;

            D.Columns["POS"].Visible = false;

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
        private void button1_Click(object sender, EventArgs e)
        {
            modernPanel1.Visible = false;
        }
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                int posision = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString());
                CXN_HISTORYIA historia = ias.Historial(posision);
                if (historia != null)
                {
                    richTextBox1.Text = historia.Consulta.ToString();
                    richTextBox2.Text = historia.Respuesta.ToString();
                    modernPanel1.Visible = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al seleccionar el registro: " + ex.Message, "Zamenis Health", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
