using Domain.CXN;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using ZamenisHealth.Comunes;
using static ZamenisHealth.Clases.ConfigForm;

namespace ZamenisHealth.HistoriasClinicas.Extras
{
    public partial class MedidasEnferemeria : BaseForm
    {
        private readonly INotasCuracion repoNotas = new MNotasCuracion();
        private int Paciente;
        private MensajesGeneral MG;

        DataTable dt = new DataTable();
        DataColumn POS;
        DataColumn Fecha;
        DataColumn Admision;
        DataColumn Profesional;

        DataTable dt2 = new DataTable();
        DataColumn Id;
        DataColumn Herida;
        DataColumn Ancho;
        DataColumn Largo;
        DataColumn Profundidad;
        DataColumn Area;

        public MedidasEnferemeria(int paciente)
        {
            InitializeComponent();
            Paciente = paciente;
        }

        private void MedidasEnferemeria_Load(object sender, EventArgs e)
        {
            try
            {
                Titulo.Text = "Medidas Enfermeria";

                List<CXN_NOTASMED> lista = repoNotas.VerCitas(Paciente);
                if (lista == null)
                {
                    Encabezados();

                    MG = new MensajesGeneral();
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "No hay registros de medidas aun para este paciente, consulte la nota general por la opcion Historis Clinicas";
                    MG.ShowDialog();

                    this.Dispose();
                    this.Close();
                }
                else
                {
                    Encabezados();

                    int Contador = 1;

                    foreach (CXN_NOTASMED i in lista)
                    {
                        DataRow row = dt.NewRow();

                        row["POS"] = Contador;
                        row["Fecha"] = Convert.ToDateTime(i.txtGranulacion).ToString("yyyy-MM-dd");
                        row["Admision"] = Convert.ToInt32(i.Admision);
                        row["Profesional"] = i.Evolucion.ToString();

                        dt.Rows.Add(row);
                        dt.AcceptChanges();

                        Contador = Contador + 1;
                    }

                    Contador = 1;
                    Estilos(dataGridView1, dt);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

                this.Dispose();
                this.Close();
            }           
        }
        void Estilos(DataGridView D, DataTable t)
        {
            D.EnableHeadersVisualStyles = false;
            D.ScrollBars = ScrollBars.Both;

            D.DataSource = t;

            D.Columns["Fecha"].Width = 80;
            D.Columns["Admision"].Width = 80;
            D.Columns["Profesional"].Width = 600;

            D.ColumnHeadersDefaultCellStyle.Font = new Font(D.Font, FontStyle.Bold);
            D.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 8, FontStyle.Bold);
            D.ColumnHeadersDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#bfdbff");
            D.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;
            D.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;

            D.Columns["Fecha"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["Admision"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["Profesional"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            D.Columns["Fecha"].SortMode = DataGridViewColumnSortMode.NotSortable;
            D.Columns["Admision"].SortMode = DataGridViewColumnSortMode.NotSortable;
            D.Columns["Profesional"].SortMode = DataGridViewColumnSortMode.NotSortable;

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
            Fecha = dt.Columns.Add("Fecha", typeof(string));
            Admision = dt.Columns.Add("Admision", typeof(int));
            Profesional = dt.Columns.Add("Profesional", typeof(string));
        }

        void Encabezados2()
        {
            dt2 = new DataTable();
            Id = dt2.Columns.Add("Id", typeof(int));
            Herida = dt2.Columns.Add("Herida", typeof(int));
            Ancho = dt2.Columns.Add("Ancho", typeof(string));
            Largo = dt2.Columns.Add("Largo", typeof(string));
            Profundidad = dt2.Columns.Add("Profundidad", typeof(string));
            Area = dt2.Columns.Add("Area", typeof(string));
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                int adm = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString());

                List<CXN_NOTASMED> lista = repoNotas.LoadHeridas(adm);
                if (lista == null)
                {
                    Encabezados2();

                    MG = new MensajesGeneral();
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "No se grabaron medidas en esta admision";
                    MG.ShowDialog();
                }
                else
                {
                    Encabezados2();

                    int Contador = 1;

                    foreach (CXN_NOTASMED i in lista)
                    {
                        DataRow row = dt2.NewRow();

                        row["Id"] = i.Id;
                        row["Herida"] = Contador;
                        row["Ancho"] = i.Ancho.ToString();
                        row["Largo"] = i.Largo.ToString();
                        row["Profundidad"] = i.Profundidad.ToString();
                        row["Area"] = i.Total.ToString();

                        dt2.Rows.Add(row);
                        dt2.AcceptChanges();

                        Contador = Contador + 1;
                    }

                    Contador = 1;
                    Estilos2(dataGridView2, dt2);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        void Estilos2(DataGridView D, DataTable t)
        {
            D.EnableHeadersVisualStyles = false;
            D.ScrollBars = ScrollBars.Both;

            D.DataSource = t;

            D.Columns["Herida"].Width = 100;
            D.Columns["Ancho"].Width = 150;
            D.Columns["Largo"].Width = 150;
            D.Columns["Profundidad"].Width = 150;
            D.Columns["Area"].Width = 150;

            D.ColumnHeadersDefaultCellStyle.Font = new Font(D.Font, FontStyle.Bold);
            D.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 8, FontStyle.Bold);
            D.ColumnHeadersDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#bfdbff");
            D.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;
            D.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;

            D.Columns["Herida"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["Ancho"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["Largo"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["Profundidad"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["Area"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            D.Columns["Herida"].SortMode = DataGridViewColumnSortMode.NotSortable;
            D.Columns["Ancho"].SortMode = DataGridViewColumnSortMode.NotSortable;
            D.Columns["Largo"].SortMode = DataGridViewColumnSortMode.NotSortable;
            D.Columns["Profundidad"].SortMode = DataGridViewColumnSortMode.NotSortable;
            D.Columns["Area"].SortMode = DataGridViewColumnSortMode.NotSortable;

            D.Columns["Id"].Visible = false;

            foreach (DataGridViewRow row in D.Rows)
            {
                int Numero = Convert.ToInt32(row.Cells["Herida"].Value.ToString());

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

        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                int IdSelected = Convert.ToInt32(dataGridView2.Rows[e.RowIndex].Cells[0].Value.ToString());

                CXN_NOTASMED NM = repoNotas.VerPosisionHerida(IdSelected);
                if (NM != null) 
                {
                    MedidasEnfermeria2 ME2 = new MedidasEnfermeria2(NM);
                    ME2.ShowDialog();
                }
                else
                {
                    MG = new MensajesGeneral();
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "No se logro recuperar el catalogo de la herida";
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
