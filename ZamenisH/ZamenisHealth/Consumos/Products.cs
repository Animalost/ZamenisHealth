using Domain.CONSUMOS;
using FormAndControls;
using Persistence;
using Persistence.CONSUMOS.Interfaces;
using Persistence.CONSUMOS.Metodos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace ZamenisHealth.Consumos
{
    public partial class Products : Forma
    {
        private IProdsConsumo oProdsConsumo;

        DataTable dt;
        DataColumn Posision;
        DataColumn CodigoInterno;
        DataColumn CodigoExterno;
        DataColumn Producto;
        DataColumn Estado;

        public Products()
        {
            InitializeComponent();
            oProdsConsumo = new MProdsConsumo();
        }

        private void Products_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Productos de Consumos";
            LogoMain.Image = Properties.Resources.Splash;
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";

            gridZH1.CeldaHeight = true;
            CargarProductos();
            gridZH1.dataGridView1.CellClick += DataGridView1_CellClick;

            ToolStripButton btnCrear = new ToolStripButton();
            btnCrear = createToolButton("Crear");
            MenuLateral.Items.Add(btnCrear);
            btnCrear.Click += btnCrear_Click;
        }

        void btnCrear_Click(object sender, EventArgs e)
        {
            ProductsEdition C = new ProductsEdition(0);
            C.ShowDialog();
        }
        private void DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                int n = Convert.ToInt32(gridZH1.dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString());

                ProductsEdition C = new ProductsEdition(n);
                C.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        void Encabezados()
        {
            gridZH1.dataGridView1.DataSource = null;
            dt = new DataTable();
            Posision = dt.Columns.Add("Posision", typeof(int));
            CodigoInterno = dt.Columns.Add("Codigo Interno", typeof(string));
            CodigoExterno = dt.Columns.Add("Codigo Externo", typeof(string));
            Producto = dt.Columns.Add("Producto", typeof(string));
            Estado = dt.Columns.Add("Estado", typeof(string));
        }
        public void CargarProductos()
        {
            try
            {
                List<CON_PRODUCTOS> prods = oProdsConsumo.GetProductos(textBox1.Text);
                if (prods != null)
                {
                    Encabezados();

                    foreach (var i in prods)
                    {
                        DataRow row = dt.NewRow();

                        row[Posision] = i.Con_Prod_Id;
                        row[CodigoInterno] = i.Con_Prod_Cod_Interno.ToString();
                        row[CodigoExterno] = i.Con_Prod_Cod_Externo;
                        row[Producto] = i.Con_Prod_Name;
                        row[Estado] = i.Con_Prod_Status == true ? "ACTIVA" : "INACTIVA";

                        dt.Rows.Add(row);
                        dt.AcceptChanges();
                    }

                    gridZH1.dataGridView1.DataSource = dt;
                    gridZH1.dataGridView1.Columns["Posision"].Visible = false;
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
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            CargarProductos();
        }
    }
}
