using Domain;
using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Data;
using System.Windows.Forms;
using ZamenisHealth.AdminSystem.Material;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.AdminSystem
{
    public partial class Productos : Forma
    {
        private static readonly IInventario repoInventario = new MInventario();
        private static readonly IAseguradoras repoAseguradoras = new MAseguradoras();

        private MensajesGeneral MG;

        private int Ase;

        DataTable dt = new DataTable();
        DataColumn POS;
        DataColumn InvId;
        DataColumn InvCod;
        DataColumn InvItem;
        DataColumn InvPrecio;

        public Productos()
        {
            InitializeComponent();
        }
        
        private void Productos_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Productos";
            LogoMain.Image = Properties.Resources.Splash;
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";

            gridZH1.dataGridView1.CellClick += DataGridView1_CellClick;

            ToolStripButton btnGenerar = new ToolStripButton();
            btnGenerar = createToolButton("Crear");
            MenuLateral.Items.Add(btnGenerar);
            btnGenerar.Click += button1_Click;

            var ases = repoAseguradoras.getAseguradoras();
            if (ases != null)
            {
                foreach (var i in ases)
                {
                    comboBox6.Items.Add(i.Ase_Descripcion);
                }

                comboBox6.SelectedIndex = 0;                
            }

            Cargar_Grid();
        }
        void button1_Click(object sender, EventArgs e)
        {
            EdicionProductos W = new EdicionProductos(0, Ase);
            W.ShowDialog();
        }
        private void DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                int Posision = Convert.ToInt32(gridZH1.dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString());
                EdicionProductos W = new EdicionProductos(Posision, 0);
                W.ShowDialog();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        private void Encabezados()
        {
            try
            {
                gridZH1.dataGridView1.DataSource = null;
                dt = new DataTable();
                InvId = dt.Columns.Add("InvId", typeof(int));
                InvCod = dt.Columns.Add("Codigo", typeof(string));
                InvItem = dt.Columns.Add("Item", typeof(string));
                InvPrecio = dt.Columns.Add("Valor", typeof(string));
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void Cargar_Grid()
        {
            try
            {
                var getProdAll = repoInventario.getAllProducts(Ase);
                if (getProdAll != null)
                {
                    Encabezados();

                    foreach (var i in getProdAll)
                    {
                        DataRow row = dt.NewRow();

                        row["InvId"] = i.InvId.ToString();
                        row["Codigo"] = i.InvCod.ToString();
                        row["Item"] = i.InvItem.ToString();
                        row["Valor"] = "$ " + Convert.ToInt32(i.InvPrecio).ToString("N0");

                        dt.Rows.Add(row);
                        dt.AcceptChanges();
                    }

                    gridZH1.dataGridView1.DataSource = dt;
                    gridZH1.dataGridView1.Columns["InvId"].Visible = false;
                }
                else
                {
                    Encabezados();
                    MG = new MensajesGeneral();
                    MG.Mensaje = "No hay productos creados aun en sistema";
                    MG.TipoImagen = 0;
                    MG.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void comboBox6_SelectedIndexChanged(object sender, EventArgs e)
        {
            Ase = repoAseguradoras.getInfoFromAsebyName(comboBox6.Text).Ase_Identificador;
            Cargar_Grid();
        }
    }
}
