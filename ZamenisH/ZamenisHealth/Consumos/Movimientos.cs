using Domain.CONSUMOS;
using FormAndControls;
using Persistence;
using Persistence.CONSUMOS.Interfaces;
using Persistence.CONSUMOS.Metodos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.Consumos
{
    public partial class Movimientos : Forma
    {
        private IAsignacion oAsignacion;
        private IProdsConsumo oProdConsumo;

        DataTable dt;
        DataColumn CodigoInterno;
        DataColumn Nombre;
        DataColumn Id;

        private MensajesGeneral MG;

        public Movimientos()
        {
            InitializeComponent();
            oAsignacion = new MAsignacion();
            oProdConsumo = new MProdsConsumo();
        }

        private void Movimientos_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Registro de Consumos Diarios";
            LogoMain.Image = Properties.Resources.Splash;
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";

            gridZH1.CeldaHeight = true;
            CargarBodegas();

            ToolStripButton btnReport = new ToolStripButton();
            btnReport = createToolButton("Ver Consumos");
            MenuLateral.Items.Add(btnReport);
            btnReport.Click += btnReport_Click;

            gridZH1.dataGridView1.CellClick += DataGridView1_CellClick;
        }

        private void DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (comboBox1.Text == "")
                {
                    MG = new MensajesGeneral()
                    {
                        Mensaje = "Debe seleccionar una bodega a la cual se hara el movimiento",
                        TipoImagen = 1000
                    };
                    MG.ShowDialog();
                }
                else
                {
                    int n = Convert.ToInt32(gridZH1.dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString());

                    MoverProducto M = new MoverProducto(n, comboBox1.Text);
                    M.ShowDialog();
                }                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        void btnReport_Click(object sender, EventArgs e)
        {
            VerMovimientosBasico Mb = new VerMovimientosBasico(false);
            Mb.ShowDialog();
        }
        void CargarBodegas()
        {
            List<CON_ASIGNACION> getBods = oAsignacion.GetBodegas();
            if (getBods != null)
            {
                getBods = getBods.Where(x => x.Asi_Status == true).ToList();

                foreach (CON_ASIGNACION i in getBods)
                {
                    comboBox1.Items.Add(i.Asi_Name);
                }

                comboBox1.SelectedItem = 0;
                CargarProductos();
            }
        }
        void CargarProductos()
        {
            List<CON_PRODUCTOS> bods = oProdConsumo.GetProductos(textBox1.Text);
            if (bods != null)
            {
                bods = bods.Where(x => x.Con_Prod_Status == true).ToList();

                Encabezados();

                foreach (var i in bods)
                {
                    DataRow row = dt.NewRow();

                    row[Id] = i.Con_Prod_Id;
                    row[CodigoInterno] = i.Con_Prod_Cod_Interno.ToString();
                    row[Nombre] = i.Con_Prod_Name.ToString();

                    dt.Rows.Add(row);
                    dt.AcceptChanges();
                }

                gridZH1.dataGridView1.DataSource = dt;
                gridZH1.dataGridView1.Columns["Id"].Visible = false;
            }
            else
            {
                Encabezados();
            }
        }
        void Encabezados()
        {
            gridZH1.dataGridView1.DataSource = null;
            dt = new DataTable();
            Id = dt.Columns.Add("Id", typeof(int));
            CodigoInterno = dt.Columns.Add("Codigo Interno", typeof(string));
            Nombre = dt.Columns.Add("Nombre del Producto", typeof(string));
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            CargarProductos();
        }
    }
}
