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

namespace ZamenisHealth.HistoriasClinicas.Extras
{
    public partial class AprobarSolicitudes : Forma2
    {
        private readonly ICambiosSolicitados repoCambios = new MCambiosSolicitados();

        private MensajesGeneral MG;

        DataTable dt = new DataTable();
        DataColumn POS;
        DataColumn Id;
        DataColumn Cup;
        DataColumn Servicio;
        DataColumn Fecha;
        DataColumn Solicitante;
        DataColumn Admision;

        public AprobarSolicitudes()
        {
            InitializeComponent();
        }

        private void AprobarSolicitudes_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Aprobar solicitudes de cambios de complejidad";
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";
            CargarSolicitudes();
        }

        void CargarSolicitudes()
        {
            try
            {
                List<CXN_CAMBIOSSOLICITADOS> CS = repoCambios.GetCambios();
                if (CS == null)
                {
                    MG = new MensajesGeneral();
                    MG.Mensaje = "No hay cambios pendientes";
                    MG.TipoImagen = 1000;
                    MG.ShowDialog();

                    this.Dispose();
                    this.Close();
                }
                else
                {
                    Encabezados();

                    int Contador = 1;

                    foreach (CXN_CAMBIOSSOLICITADOS i in CS)
                    {
                        DataRow row = dt.NewRow();

                        row["POS"] = Contador;
                        row["Id"] = i.Id;
                        row["Cup"] = i.CupComplejidad.ToString();
                        row["Servicio"] = i.ServComplejidad.ToString();
                        row["Fecha"] = Convert.ToDateTime(i.Fecha).ToString("yyyy-MM-dd");
                        row["Solicitante"] = i.Solicitante.ToString();
                        row["Admision"] = i.AdmisionOfertante.ToString();

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
            }
        }
        void Estilos(DataGridView D, DataTable t)
        {
            D.EnableHeadersVisualStyles = false;
            D.ScrollBars = ScrollBars.Both;

            D.DataSource = t;

            D.Columns["Cup"].Width = 80;
            D.Columns["Servicio"].Width = 400;
            D.Columns["Fecha"].Width = 80;
            D.Columns["Solicitante"].Width = 100;

            D.ColumnHeadersDefaultCellStyle.Font = new Font(D.Font, FontStyle.Bold);
            D.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 8, FontStyle.Bold);
            D.ColumnHeadersDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#bfdbff");
            D.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;
            D.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;

            D.Columns["Cup"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["Servicio"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["Fecha"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["Solicitante"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            D.Columns["Cup"].SortMode = DataGridViewColumnSortMode.NotSortable;
            D.Columns["Servicio"].SortMode = DataGridViewColumnSortMode.NotSortable;
            D.Columns["Fecha"].SortMode = DataGridViewColumnSortMode.NotSortable;
            D.Columns["Solicitante"].SortMode = DataGridViewColumnSortMode.NotSortable;

            D.Columns["POS"].Visible = false;
            D.Columns["Id"].Visible = false;
            D.Columns["Admision"].Visible = false;

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
            Cup = dt.Columns.Add("Cup", typeof(string));
            Servicio = dt.Columns.Add("Servicio", typeof(string));
            Fecha = dt.Columns.Add("Fecha", typeof(string));
            Solicitante = dt.Columns.Add("Solicitante", typeof(string));
            Admision = dt.Columns.Add("Admision", typeof(int));
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int Posision = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString());
            int admision = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[6].Value.ToString());

            AprobarSolicitudes2 A = new AprobarSolicitudes2(Posision, admision);
            A.ShowDialog();
        }

        public void setCargarSolicitudes()
        {
            CargarSolicitudes();
        }
    }
}
