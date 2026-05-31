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
using System.Windows.Forms;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.INV
{
    public partial class Proveedores : Forma
    {
        private IProveedores proveedores;

        DataTable dt = new DataTable();
        DataColumn POS;
        DataColumn Codigo;
        DataColumn Nombre;

        public Proveedores()
        {
            InitializeComponent();
            proveedores = new MProveedores();
        }

        private void Proveedores_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Proveedores";
            LogoMain.Image = Properties.Resources.Splash;
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";

            ToolStripButton btnSave;

            btnSave = new ToolStripButton();
            btnSave = createToolButton("Crear Proveedor");
            MenuLateral.Items.Add(btnSave);
            btnSave.Click += toolStripButton2_Click;
            CargarGrilla();
        }
        void Encabezados()
        {
            dataGridView1.DataSource = null;
            dt = new DataTable();
            POS = dt.Columns.Add("POS", typeof(int));
            Codigo = dt.Columns.Add("Codigo", typeof(string));
            Nombre = dt.Columns.Add("Nombre", typeof(string));
        }
        public void CargarGrilla()
        {
            try
            {

                List<CXN_PROVEEDORES> _lista = proveedores.getListadoProvs();
                if (_lista != null)
                {
                    Encabezados();

                    int Contador = 1;

                    foreach (CXN_PROVEEDORES i in _lista)
                    {
                        DataRow row = dt.NewRow();

                        row["POS"] = Contador;
                        row["Codigo"] = i.Codigo.ToString();
                        row["Nombre"] = i.Nombre.ToString();

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

            D.Columns["Codigo"].Width = 150;
            D.Columns["Nombre"].Width = 600;

            D.ColumnHeadersDefaultCellStyle.Font = new Font(D.Font, FontStyle.Bold);
            D.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 8, FontStyle.Bold);
            D.ColumnHeadersDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#bfdbff");
            D.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;
            D.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;

            D.Columns["Codigo"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["Nombre"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

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
            Proveedores2 proveedores2 = new Proveedores2(0);
            proveedores2.ShowDialog();
        }
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            string Code = dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
            Proveedores2 proveedores2 = new Proveedores2(Convert.ToInt32(Code));
            proveedores2.ShowDialog();
        }
    }
}
