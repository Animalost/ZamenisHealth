using Domain.CXN;
using FormAndControls;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.PagosApp
{
    public partial class EstadoCuenta : Forma2
    {
        private readonly IPayments rPayments;
        private MensajesGeneral MG;

        DataTable dt;
        DataColumn Id;
        DataColumn Mes;
        DataColumn Año;
        DataColumn Estado;
        DataColumn Factura;
        DataColumn Pagar;
        DataColumn B64;

        public EstadoCuenta()
        {
            InitializeComponent();
            rPayments = new MPayments();
        }

        private void EstadoCuenta_Load(object sender, EventArgs e)
        {
            try
            {
                Titulo.Text = "Estado de Cuenta Zamenis Grille";

                List<Pagos> f = rPayments.getPagosHechos();
                if (f == null)
                {
                    Encabezados();
                    MG = new MensajesGeneral();
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "No se encontraron registros de pagos.";
                    MG.ShowDialog();

                    this.Dispose();
                    this.Close();
                }
                else
                {
                    Encabezados();

                    foreach (Pagos i in f)
                    {
                        DataRow row = dt.NewRow();

                        row["Id"] = i.Id;
                        row["Mes"] = i.Mes;
                        row["Año"] = i.Año;
                        row["Estado"] = i.Estado;
                        row["Factura"] = "Descargar";
                        row["Pagar"] = i.Link;
                        row["B64"] = i.Factura;

                        dt.Rows.Add(row);
                        dt.AcceptChanges();
                    }

                    Estilos(dataGridView1, dt);
                    dataGridView1.ClearSelection();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void Encabezados()
        {
            try
            {
                dt = new DataTable();
                Id = dt.Columns.Add("Id", typeof(int));
                Mes = dt.Columns.Add("Mes", typeof(string));
                Año = dt.Columns.Add("Año", typeof(int));
                Estado = dt.Columns.Add("Estado", typeof(string));
                Factura = dt.Columns.Add("Factura", typeof(string));
                Pagar = dt.Columns.Add("Pagar", typeof(string));
                B64 = dt.Columns.Add("B64", typeof(string));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        void Estilos(DataGridView D, DataTable t)
        {
            D.EnableHeadersVisualStyles = false;

            D.DataSource = t;

            D.Columns["Mes"].Width = 110;
            D.Columns["Año"].Width = 110;
            D.Columns["Estado"].Width = 110;
            D.Columns["Factura"].Width = 150;
            D.Columns["Pagar"].Width = 280;

            D.ColumnHeadersDefaultCellStyle.Font = new Font(D.Font, FontStyle.Bold);
            D.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 8, FontStyle.Bold);
            D.ColumnHeadersDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#bfdbff");
            D.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;
            D.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;

            D.Columns["Mes"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["Año"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["Estado"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["Factura"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["Pagar"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            D.Columns["Mes"].SortMode = DataGridViewColumnSortMode.NotSortable;
            D.Columns["Año"].SortMode = DataGridViewColumnSortMode.NotSortable;
            D.Columns["Estado"].SortMode = DataGridViewColumnSortMode.NotSortable;
            D.Columns["Factura"].SortMode = DataGridViewColumnSortMode.NotSortable;
            D.Columns["Pagar"].SortMode = DataGridViewColumnSortMode.NotSortable;

            D.Columns["Id"].Visible = false;
            D.Columns["B64"].Visible = false;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                string Estado = row.Cells["Estado"].Value.ToString();

                if (Estado == "PENDIENTE")
                {
                    row.DefaultCellStyle.BackColor = Color.LightGreen;
                    row.DefaultCellStyle.ForeColor = Color.Green;
                }
                else if (Estado == "PAGADO")
                {
                    row.DefaultCellStyle.BackColor = Color.LightBlue;
                    row.DefaultCellStyle.ForeColor = Color.Blue;
                }
                else
                {
                    row.DefaultCellStyle.BackColor = Color.White;
                    row.DefaultCellStyle.ForeColor = Color.Black;
                }
            }

            dataGridView1.ClearSelection();
        }

        private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == 4)
                {
                    string fac = dataGridView1.Rows[e.RowIndex].Cells[6].Value.ToString();
                    byte[] pdfBytes = Convert.FromBase64String(fac);

                    string tempFile = Path.Combine(Path.GetTempPath(), "Documento.pdf");
                    File.WriteAllBytes(tempFile, pdfBytes);

                    Payments2 P = new Payments2(tempFile);
                    P.ShowDialog();
                }
                else if (e.ColumnIndex == 5)
                {
                    string url = dataGridView1.Rows[e.RowIndex].Cells[5].Value.ToString();
                    if (!string.IsNullOrEmpty(url))
                    {
                        System.Diagnostics.Process.Start(url);
                    }
                }
                else
                {
                    return;
                }

                dataGridView1.ClearSelection();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
