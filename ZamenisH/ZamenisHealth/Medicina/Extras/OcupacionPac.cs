using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace ZamenisHealth.Medicina.Extras
{
    public partial class OcupacionPac : Forma2
    {
        private static readonly IFHIR rFHIR = new MFHIR();
        private ActualizarPaciente actualizarPaciente;

        DataTable dt = new DataTable();
        DataColumn POS;
        DataColumn Ocupacion;

        public OcupacionPac(ActualizarPaciente _actualizarPaciente)
        {
            InitializeComponent();
            actualizarPaciente = _actualizarPaciente;
        }

        private void OcupacionPac_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Ocupaciones";
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";

            cargarOcupacion("");
        }
        void cargarOcupacion(string Ocupaciona)
        {
            List<string> lista = rFHIR.GetOcupaciones(Ocupaciona);
            if (lista != null)
            {
                Encabezados();

                int Contador = 1;

                foreach (string i in lista)
                {
                    DataRow row = dt.NewRow();

                    row[POS] = Contador;
                    row[Ocupacion] = i;

                    dt.Rows.Add(row);
                    dt.AcceptChanges();

                    Contador = Contador + 1;
                }

                Contador = 1;
                Estilos(dataGridView1, dt);
            }
            else
            {
                dataGridView1.DataSource = null;
                Encabezados();
            }
        }
        void Estilos(DataGridView D, DataTable t)
        {
            D.EnableHeadersVisualStyles = false;
            D.ScrollBars = ScrollBars.Both;

            D.DataSource = t;

            D.Columns["Ocupacion"].Width = 720;

            D.ColumnHeadersDefaultCellStyle.Font = new Font(D.Font, FontStyle.Bold);
            D.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 12, FontStyle.Bold);
            D.ColumnHeadersDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#bfdbff");
            D.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;
            D.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;

            D.Columns["Ocupacion"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["Ocupacion"].SortMode = DataGridViewColumnSortMode.NotSortable;

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
        void Encabezados()
        {
            dt = new DataTable();
            POS = dt.Columns.Add("POS", typeof(int));
            Ocupacion = dt.Columns.Add("Ocupacion", typeof(string));
        }
        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                string Cod = dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
                actualizarPaciente.textBox15.Text = Cod;

                this.Dispose();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }            
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            cargarOcupacion(textBox1.Text);
        }
    }
}
