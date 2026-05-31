using Domain;
using Domain.INV;
using FormAndControls;
using Persistence;
using Persistence.INV.Interfaces;
using Persistence.INV.Metodos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using ZamenisHealth.Clases;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.INV
{
    public partial class VerInventarioPpal : Forma
    {
        private IBodegaPrincipal bodegaPrincipal;
        private int CodePrestador;
        private MensajesGeneral MG;

        DataTable dt = new DataTable();
        DataColumn POS;
        DataColumn Nombre;
        DataColumn Lote;
        DataColumn Factura;
        DataColumn Cantidad;
        DataColumn Costo;
        DataColumn NombreProveedor;

        public VerInventarioPpal(int codePrestador)
        {
            InitializeComponent();
            CodePrestador = codePrestador;
            bodegaPrincipal = new MBodegaPrincipal();
        }

        private void VerInventarioPpal_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Ver Inventario";
            LogoMain.Image = Properties.Resources.Splash;
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";

            ToolStripButton btnExport;
            ToolStripButton btnMasivo;

            btnExport = new ToolStripButton();
            btnExport = createToolButton("Exportar");
            MenuLateral.Items.Add(btnExport);
            btnExport.Click += toolStripButton2_Click;

            btnMasivo = new ToolStripButton();
            btnMasivo = createToolButton("Masivo");
            MenuLateral.Items.Add(btnMasivo);
            btnMasivo.Click += toolStripButton3_Click;

            CargarInventario();
        }
        void Encabezados()
        {
            dataGridView1.DataSource = null;
            dt = new DataTable();
            POS = dt.Columns.Add("POS", typeof(int));
            Nombre = dt.Columns.Add("Nombre", typeof(string));
            Lote = dt.Columns.Add("Lote", typeof(string));
            Factura = dt.Columns.Add("Factura", typeof(string));
            Cantidad = dt.Columns.Add("Cantidad", typeof(int));
            Costo = dt.Columns.Add("Costo", typeof(int));
            NombreProveedor = dt.Columns.Add("NombreProveedor", typeof(string));
        }
        void CargarInventario()
        {
            try
            {
                List<INV_HISTORICOPPAL> _lista = bodegaPrincipal.LoadInventary(CodePrestador);
                if (_lista != null)
                {
                    Encabezados();

                    int Contador = 1;

                    foreach (INV_HISTORICOPPAL i in _lista)
                    {
                        DataRow row = dt.NewRow();

                        row["POS"] = Contador;
                        row["Nombre"] = i.Nombre.ToString();
                        row["Lote"] = i.Lote.ToString();
                        row["Factura"] = i.Factura.ToString();
                        row["Cantidad"] = i.Cantidad;
                        row["Costo"] = i.Costo;
                        row["NombreProveedor"] = i.CodigoProveedor.ToString();

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
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        void Estilos(DataGridView D, DataTable t)
        {
            D.EnableHeadersVisualStyles = false;
            D.ScrollBars = ScrollBars.Both;

            D.DataSource = t;

            D.Columns["Nombre"].Width = 450;
            D.Columns["Lote"].Width = 150;
            D.Columns["Factura"].Width = 150;
            D.Columns["Cantidad"].Width = 100;
            D.Columns["Costo"].Width = 150;
            D.Columns["NombreProveedor"].Width = 400;

            D.ColumnHeadersDefaultCellStyle.Font = new Font(D.Font, FontStyle.Bold);
            D.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 8, FontStyle.Bold);
            D.ColumnHeadersDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#bfdbff");
            D.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;
            D.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;

            D.Columns["Nombre"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["Lote"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["Factura"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["Cantidad"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["Costo"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["NombreProveedor"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

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
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            try
            {
                List<INV_HISTORICOPPAL> _lista = bodegaPrincipal.LoadInventary(CodePrestador);
                if (_lista != null)
                {
                    ConfigForm.GenerarReportViewer("DataSetInventarios", "ZamenisHealth.Reportes.INV_INVENTARIO.rdlc", _lista);
                }
                else
                {
                    MG = new MensajesGeneral();
                    MG.Mensaje = "No hay datos para exportar";
                    MG.TipoImagen = 3;
                    MG.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            MasivoPpal masivoPpal = new MasivoPpal();
            masivoPpal.ShowDialog();
        }
    }
}
