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
using System.Windows.Forms;
using ZamenisHealth.Comunes;
using ZamenisHealth.HistoriasClinicas;
using ZamenisHealth.Medicina.OrdenesExtra;
using ZamenisHealth.Medicina.OrdenesHistory;

namespace ZamenisHealth.Medicina
{
    public partial class OrdenesMedicasM2 : Forma2
    {
        private IMedicamentos repoMedi = new MMedicamentos();

        DataTable dt;
        DataColumn POS;
        DataColumn Codigo;
        DataColumn Medicamento;

        private string Forma;

        public OrdenesMedicasM2(string fForma)
        {
            InitializeComponent();
            Forma = fForma;
        }

        private void OrdenesMedicasM2_Load(object sender, EventArgs e)
        {
            Titulo.Text = "MEDICAMENTOS MINSALUD";
        }
        void Encabezados()
        {
            dt = new DataTable();
            POS = dt.Columns.Add("POS", typeof(int));
            Codigo = dt.Columns.Add("Codigo", typeof(string));
            Medicamento = dt.Columns.Add("Medicamento", typeof(string));
        }
        void Cargar(string Medi)
        {
            try
            {
                List<CXN_MEDICAMENTOS> _lista = repoMedi.PorDesc(textBox1.Text);

                if (_lista != null)
                {
                    Encabezados();

                    int Contador = 1;

                    foreach (CXN_MEDICAMENTOS i in _lista)
                    {
                        DataRow row = dt.NewRow();

                        row["POS"] = Contador;
                        row["Codigo"] = i.Codigo.ToString();
                        row["Medicamento"] = i.Medicamento.ToString();

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
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        void Estilos(DataGridView D, DataTable t)
        {
            D.EnableHeadersVisualStyles = false;
            D.ScrollBars = ScrollBars.Both;

            D.DataSource = t;

            D.Columns["Codigo"].Width = 80;
            D.Columns["Medicamento"].Width = 630;

            D.ColumnHeadersDefaultCellStyle.Font = new Font(D.Font, FontStyle.Bold);
            D.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 8, FontStyle.Bold);
            D.ColumnHeadersDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#bfdbff");
            D.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;
            D.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;

            D.Columns["Codigo"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["Medicamento"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            D.Columns["Codigo"].SortMode = DataGridViewColumnSortMode.NotSortable;
            D.Columns["Medicamento"].SortMode = DataGridViewColumnSortMode.NotSortable;

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

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (textBox1.Text.Length >= 4)
                {
                    Cargar(textBox1.Text);
                }
                else
                {
                    dataGridView1.DataSource = null;
                    Encabezados();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (Forma == "ANTPROPIO")
                {
                    Historia_MedicinaGeneral f1 = Application.OpenForms.OfType<Historia_MedicinaGeneral>().SingleOrDefault();
                    f1.textBox46.Text = dataGridView1.CurrentRow.Cells["Codigo"].Value.ToString();
                    f1.textBox45.Text = dataGridView1.CurrentRow.Cells["Medicamento"].Value.ToString();
                }
                else if (Forma == "RECETAADMITION")
                {
                    OrdenDCI f1 = Application.OpenForms.OfType<OrdenDCI>().SingleOrDefault();
                    f1.textBox1.Text = dataGridView1.CurrentRow.Cells["Codigo"].Value.ToString();
                    f1.textBox3.Text = dataGridView1.CurrentRow.Cells["Medicamento"].Value.ToString();
                }
                else if (Forma == "ANTFIPROPIO")
                {
                    Historia_Fisiatria f1 = Application.OpenForms.OfType<Historia_Fisiatria>().SingleOrDefault();
                    f1.textBox65.Text = dataGridView1.CurrentRow.Cells["Codigo"].Value.ToString();
                    f1.textBox64.Text = dataGridView1.CurrentRow.Cells["Medicamento"].Value.ToString();
                }
                else if (Forma == "RECETAEXTRA")
                {
                    CrearOrdenExtra2 f1 = Application.OpenForms.OfType<CrearOrdenExtra2>().SingleOrDefault();
                    f1.textBox1.Text = dataGridView1.CurrentRow.Cells["Codigo"].Value.ToString();
                    f1.textBox3.Text = dataGridView1.CurrentRow.Cells["Medicamento"].Value.ToString();
                }

                this.Dispose();
                this.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }
    }
}
