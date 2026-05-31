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

using ZamenisHealth.Recepcion;
using ZamenisHealth.Recepcion.Ventas;

namespace ZamenisHealth.Facturacion.Extras
{
    public partial class Retenciones : Forma2
    {
        private readonly IImpuestos impuestos;

        private string TipoImpuesto;
        private string Forma;

        DataTable dt;
        DataColumn POS;
        DataColumn Concepto;
        DataColumn Tarifa;
        DataColumn Id;

        public Retenciones(string tipoImpuesto, string forma)
        {
            InitializeComponent();
            TipoImpuesto = tipoImpuesto;
            Forma = forma;

            impuestos = new MImpuestos();
        }

        private void Retenciones_Load(object sender, EventArgs e)
        {
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";
            if (this.TipoImpuesto == "Fuente")
            {
                Titulo.Text = "Retencion en la Fuente";
                CargarFuente();
            }
            if (this.TipoImpuesto == "Ica")
            {
                Titulo.Text = "Retencion del ICA";
                CargarIca();
            }
        }
        void Encabezados()
        {
            dataGridView1.DataSource = null;

            dt = new DataTable();
            POS = dt.Columns.Add("POS", typeof(int));
            Id = dt.Columns.Add("Id", typeof(int));
            Concepto = dt.Columns.Add("Concepto", typeof(string));
            Tarifa = dt.Columns.Add("Tarifa", typeof(string));
        }
        void CargarIca()
        {
            try
            {
                List<ICA2> getIca = impuestos.getICA();
                if (getIca != null)
                {
                    Encabezados();

                    int Contador = 1;

                    foreach (var i in getIca)
                    {
                        DataRow row = dt.NewRow();

                        row["POS"] = Contador;
                        row["Id"] = i.Id.ToString();
                        row["Concepto"] = i.Concepto.ToString();
                        row["Tarifa"] = i.ICA.ToString();

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
                MessageBox.Show(ex.Message);
            }
        }
        void CargarFuente()
        {
            try
            {
                List<FUENTE2> getFuente = impuestos.getFuente();
                if (getFuente != null)
                {
                    Encabezados();

                    int Contador = 1;

                    foreach (var i in getFuente)
                    {
                        DataRow row = dt.NewRow();

                        row["POS"] = Contador;
                        row["Id"] = i.Id.ToString();
                        row["Concepto"] = i.Concepto.ToString();
                        row["Tarifa"] = i.Retencion.ToString();

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
                MessageBox.Show(ex.Message);
            }
        }
        void Estilos(DataGridView D, DataTable t)
        {
            D.EnableHeadersVisualStyles = false;
            D.ScrollBars = ScrollBars.Both;

            D.DataSource = t;

            D.Columns["Concepto"].Width = 650;
            D.Columns["Tarifa"].Width = 150;

            D.ColumnHeadersDefaultCellStyle.Font = new Font(D.Font, FontStyle.Bold);
            D.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 8, FontStyle.Bold);
            D.ColumnHeadersDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#bfdbff");
            D.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;
            D.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;

            D.Columns["Concepto"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["Tarifa"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            D.Columns["Concepto"].SortMode = DataGridViewColumnSortMode.NotSortable;
            D.Columns["Tarifa"].SortMode = DataGridViewColumnSortMode.NotSortable;

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

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                string Tarifa = dataGridView1.Rows[e.RowIndex].Cells[3].Value.ToString();

                if (this.Forma == "Ventas" && this.TipoImpuesto == "Fuente")
                {
                    Vender fTemp = Application.OpenForms.OfType<Vender>().SingleOrDefault();
                    fTemp.label23.Text = Tarifa;
                }
                else if (this.Forma == "Ventas" && this.TipoImpuesto == "Ica")
                {
                    Vender fTemp = Application.OpenForms.OfType<Vender>().SingleOrDefault();
                    fTemp.label24.Text = Tarifa;
                }
                else if (this.Forma == "Salud" && this.TipoImpuesto == "Ica")
                {
                    Facturar2 fTemp = Application.OpenForms.OfType<Facturar2>().SingleOrDefault();
                    fTemp.label24.Text = Tarifa;
                }
                else if (this.Forma == "Salud" && this.TipoImpuesto == "Fuente")
                {
                    Facturar2 fTemp = Application.OpenForms.OfType<Facturar2>().SingleOrDefault();
                    fTemp.label23.Text = Tarifa;
                }
                else if (this.Forma == "SaludAbierta" && this.TipoImpuesto == "Ica")
                {
                    FacturaAbierta3 fTemp = Application.OpenForms.OfType<FacturaAbierta3>().SingleOrDefault();
                    fTemp.label24.Text = Tarifa;
                }
                else if (this.Forma == "SaludAbierta" && this.TipoImpuesto == "Fuente")
                {
                    FacturaAbierta3 fTemp = Application.OpenForms.OfType<FacturaAbierta3>().SingleOrDefault();
                    fTemp.label23.Text = Tarifa;
                }

                this.Dispose();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
