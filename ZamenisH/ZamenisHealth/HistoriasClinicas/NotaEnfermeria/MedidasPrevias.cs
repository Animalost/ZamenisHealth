using Domain.CXN;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using static ZamenisHealth.Clases.ConfigForm;

namespace ZamenisHealth.HistoriasClinicas.NotaEnfermeria
{
    public partial class MedidasPrevias : BaseForm
    {
        private readonly INotasCuracion repoNotasCuracion;
        private int Paciente, Admision;
        private DataTable dt;
        private DataColumn POS;
        private DataColumn Id;
        private DataColumn Largo;
        private DataColumn Ancho;
        private DataColumn Profundidad;
        private DataColumn Total;
        private DataColumn Localizacion;

        public MedidasPrevias(int paciente, int admision)
        {
            InitializeComponent();
            Paciente = paciente;
            Admision = admision;
            repoNotasCuracion = new MNotasCuracion();
        }

        private void MedidasPrevias_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Medidas Previas";
            CargarMedidasPrevias();
        }
        void Encabezados()
        {
            dt = new DataTable();
            POS = dt.Columns.Add("POS", typeof(int));
            Id = dt.Columns.Add("Id", typeof(int));
            Largo = dt.Columns.Add("Largo", typeof(string));
            Ancho = dt.Columns.Add("Ancho", typeof(string));
            Profundidad = dt.Columns.Add("Profundidad", typeof(string));
            Total = dt.Columns.Add("Total", typeof(string));
            Localizacion = dt.Columns.Add("Localizacion", typeof(string));
        }
        void Estilos(DataGridView D, DataTable t)
        {
            D.EnableHeadersVisualStyles = false;
            D.ScrollBars = ScrollBars.Both;

            D.DataSource = t;

            D.Columns["Largo"].Width = 100;
            D.Columns["Ancho"].Width = 100;
            D.Columns["Profundidad"].Width = 100;
            D.Columns["Total"].Width = 100;
            D.Columns["Localizacion"].Width = 600;

            D.ColumnHeadersDefaultCellStyle.Font = new Font(D.Font, FontStyle.Bold);
            D.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 8, FontStyle.Bold);
            D.ColumnHeadersDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#bfdbff");
            D.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;
            D.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;

            D.Columns["Largo"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["Ancho"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["Profundidad"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["Total"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["Localizacion"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

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

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int PosSelected = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString());

            MedidasPrevias2 medidasPrevias2 = new MedidasPrevias2(PosSelected);
            medidasPrevias2.ShowDialog();
        }

        void CargarMedidasPrevias()
        {
            try
            {
                List<CXN_NOTASMED> _lista = repoNotasCuracion.LoadHistorialHeridas(Paciente);
                if (_lista != null)
                {
                    Encabezados();

                    int Contador = 1;

                    foreach (CXN_NOTASMED i in _lista)
                    {
                        DataRow row = dt.NewRow();

                        row["POS"] = Contador;
                        row["Id"] = i.Id;
                        row["Largo"] = i.Largo.ToString();
                        row["Ancho"] = i.Ancho.ToString();
                        row["Profundidad"] = i.Profundidad.ToString();
                        row["Total"] = i.Total.ToString();
                        row["Localizacion"] = i.txtLocalizacion.ToString();

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
                MessageBox.Show("Error al cargar las medidas previas: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
