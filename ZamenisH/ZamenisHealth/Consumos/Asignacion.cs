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
    public partial class Asignacion : Forma
    {
        private IAsignacion oAsignacion;

        DataTable dt;
        DataColumn Consultorio;
        DataColumn Nombre;
        DataColumn Estado;

        public Asignacion()
        {
            InitializeComponent();
            oAsignacion = new MAsignacion();
        }

        private void Asignacion_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Asignacion de Bodegas";
            LogoMain.Image = Properties.Resources.Splash;
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";

            gridZH1.CeldaHeight = true;
            CargarBodegas();

            gridZH1.dataGridView1.CellClick += DataGridView1_CellClick;

            ToolStripButton btnCrear = new ToolStripButton();
            btnCrear = createToolButton("Crear Nueva");
            MenuLateral.Items.Add(btnCrear);
            btnCrear.Click += btnCrear_Click;

            ToolStripButton btnConsumos = new ToolStripButton();
            btnConsumos = createToolButton("Consumos");
            MenuLateral.Items.Add(btnConsumos);
            btnConsumos.Click += btnConsumos_Click;

            ToolStripButton btnAnular = new ToolStripButton();
            btnAnular = createToolButton("Anular");
            MenuLateral.Items.Add(btnConsumos);
            btnAnular.Click += btnAnular_Click;

            ToolStripButton btnExportarCompleto = new ToolStripButton();
            btnExportarCompleto = createToolButton("Exportar Contable");
            MenuLateral.Items.Add(btnExportarCompleto);
            btnExportarCompleto.Click += btnExportarCompleto_Click;

            ToolStripButton btnProductos = new ToolStripButton();
            btnProductos = createToolButton("Productos");
            MenuLateral.Items.Add(btnProductos);
            btnProductos.Click += btnProductos_Click;
        }
        void btnProductos_Click(object sender, EventArgs e)
        {
            Products P = new Products();
            P.ShowDialog();
        }
        void btnExportarCompleto_Click(object sender, EventArgs e)
        {
            VerMovimientosCompleto VmC = new VerMovimientosCompleto();
            VmC.ShowDialog();
        }
        void btnAnular_Click(object sender, EventArgs e)
        {
            VerMovimientosBasico Vm = new VerMovimientosBasico(true);
            Vm.ShowDialog();
        }
        void btnConsumos_Click(object sender, EventArgs e)
        {
            Movimientos C = new Movimientos();
            C.ShowDialog();
        }
        void btnCrear_Click(object sender, EventArgs e)
        {
            CrearBodega C = new CrearBodega(0);
            C.ShowDialog();
        }
        private void DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                int n = Convert.ToInt32(gridZH1.dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString());

                CrearBodega C = new CrearBodega(n);
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
            Consultorio = dt.Columns.Add("Consultorio", typeof(int));
            Nombre = dt.Columns.Add("Nombre", typeof(string));
            Estado = dt.Columns.Add("Estado", typeof(string));
        }
        public void CargarBodegas()
        {
            try
            {
                List<CON_ASIGNACION> bods = oAsignacion.GetBodegas();
                if (bods != null)
                {
                    Encabezados();

                    foreach (var i in bods)
                    {
                        DataRow row = dt.NewRow();

                        row[Consultorio] = i.Asi_Number;
                        row[Nombre] = i.Asi_Name.ToString();
                        row[Estado] = i.Asi_Status == true ? "ACTIVA" : "INACTIVA";

                        dt.Rows.Add(row);
                        dt.AcceptChanges();
                    }

                    gridZH1.dataGridView1.DataSource = dt;
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
    }
}
