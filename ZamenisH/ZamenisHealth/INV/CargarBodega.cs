using Domain;
using Domain.CXN;
using Domain.INV;
using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using Persistence.INV.Interfaces;
using Persistence.INV.Metodos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using ZamenisHealth.Comunes;
using ZamenisHealth.Inventario;

namespace ZamenisHealth.INV
{
    public partial class CargarBodega : Forma
    {
        private static readonly IProveedores repoProv = new MProveedores();
        private static readonly IProductos repoProductos = new MProductos();
        private static readonly ICompañia repoCia = new MCompañia();
        private int CodPrestador, CodProveedor;

        DataTable dt = new DataTable();
        DataColumn POS;
        DataColumn Id;
        DataColumn CodigoInterno;
        DataColumn CodigoProveedor;
        DataColumn Nombre;
        DataColumn Observacion;

        public CargarBodega()
        {
            InitializeComponent();
        }

        private void CargarBodega_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Cargar Bodegas";
            LogoMain.Image = Properties.Resources.Splash;
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";

            ToolStripButton btnVerPpal;
            ToolStripButton btnCreateProveedor;
            ToolStripButton btnCreateProd;
            ToolStripButton btnEdirProd;
            ToolStripButton btnConsultorios;
            ToolStripButton btnReportes;

            btnVerPpal = new ToolStripButton();
            btnVerPpal = createToolButton("Ver Inv. Principal");
            MenuLateral.Items.Add(btnVerPpal);
            btnVerPpal.Click += toolStripButton6_Click;

            btnCreateProveedor = new ToolStripButton();
            btnCreateProveedor = createToolButton("Crear Proveedor");
            MenuLateral.Items.Add(btnCreateProveedor);
            btnCreateProveedor.Click += toolStripButton2_Click;

            btnCreateProd = new ToolStripButton();
            btnCreateProd = createToolButton("Crear Producto");
            MenuLateral.Items.Add(btnCreateProd);
            btnCreateProd.Click += toolStripButton3_Click;

            btnEdirProd = new ToolStripButton();
            btnEdirProd = createToolButton("Editar Producto");
            MenuLateral.Items.Add(btnEdirProd);
            btnEdirProd.Click += toolStripButton4_Click;

            btnConsultorios = new ToolStripButton();
            btnConsultorios = createToolButton("Consultorios");
            MenuLateral.Items.Add(btnConsultorios);
            btnConsultorios.Click += toolStripButton5_Click;

            btnReportes = new ToolStripButton();
            btnReportes = createToolButton("Reportes");
            MenuLateral.Items.Add(btnReportes);
            btnReportes.Click += toolStripButton7_Click;

            CargarProveedores();
            CargarPrestadores();

            INV_PRODUCTOS P = new INV_PRODUCTOS
            {
                Prestador = CodPrestador,
                Proveedor = CodProveedor
            };

            CargarProductos("", P);
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            CrearProductos crearProductos = new CrearProductos();
            crearProductos.ShowDialog();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            Proveedores P = new Proveedores();
            P.ShowDialog();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Dispose();
            this.Close();
        }
        void CargarProveedores()
        {
            comboBox2.Items.Clear();

            List<CXN_PROVEEDORES> getLista = repoProv.getListadoProvs();
            if (getLista != null)
            {
                foreach (CXN_PROVEEDORES i in getLista)
                {
                    comboBox2.Items.Add(i.Nombre);
                }

                comboBox2.SelectedIndex = 0;
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                this.CodProveedor = repoProv.getCodProvbyName(comboBox2.Text);
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                this.CodPrestador = repoCia.getPrestadorbyName(comboBox1.Text).Com_Identificador;
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        void CargarPrestadores()
        {
            List<CXN_CIA> getLista = repoCia.getAllCompañias();
            if (getLista != null)
            {
                foreach (CXN_CIA i in getLista)
                {
                    comboBox1.Items.Add(i.Com_Nombre);
                }

                comboBox1.SelectedIndex = 0;
            }
        }
        void Encabezados()
        {
            dataGridView1.DataSource = null;
            dt = new DataTable();
            POS = dt.Columns.Add("POS", typeof(int));
            Id = dt.Columns.Add("Id", typeof(int));
            CodigoInterno = dt.Columns.Add("CodigoInterno", typeof(string));
            CodigoProveedor = dt.Columns.Add("CodigoProveedor", typeof(string));
            Nombre = dt.Columns.Add("Nombre", typeof(string));
            Observacion = dt.Columns.Add("Observacion", typeof(string));
        }

        public void CargarProductos(string Filtro, INV_PRODUCTOS P)
        {
            try
            {
                List<INV_PRODUCTOS> _lista = repoProductos.CargarProductos(Filtro, P);
                if (_lista != null)
                {
                    Encabezados();

                    int Contador = 1;

                    foreach (INV_PRODUCTOS i in _lista)
                    {
                        DataRow row = dt.NewRow();

                        row["POS"] = Contador;
                        row["Id"] = i.Id;
                        row["CodigoInterno"] = i.CodigoInterno.ToString();
                        row["CodigoProveedor"] = i.CodigoProveedor.ToString();
                        row["Nombre"] = i.Nombre.ToString();
                        row["Observacion"] = i.Observacion.ToString();

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

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            INV_PRODUCTOS P = new INV_PRODUCTOS
            {
                Prestador = CodPrestador,
                Proveedor = CodProveedor
            };

            CargarProductos(textBox1.Text.Trim(), P);
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                int Pos = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString());
                CargarBodega2 cargarBodega2 = new CargarBodega2(Pos);
                cargarBodega2.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            Consultorios.Consultorios C = new Consultorios.Consultorios();
            C.ShowDialog();
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            CrearProductos2 crearProductos2 = new CrearProductos2(CodPrestador, CodProveedor);
            crearProductos2.ShowDialog();
        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            VerInventarioPpal verInventarioPpal = new VerInventarioPpal(CodPrestador);
            verInventarioPpal.ShowDialog();
        }

        private void toolStripButton7_Click(object sender, EventArgs e)
        {
            ReportesINV rI = new ReportesINV();
            rI.ShowDialog();
        }

        void Estilos(DataGridView D, DataTable t)
        {
            D.EnableHeadersVisualStyles = false;
            D.ScrollBars = ScrollBars.Both;

            D.DataSource = t;

            D.Columns["CodigoInterno"].Width = 100;
            D.Columns["CodigoProveedor"].Width = 120;
            D.Columns["Nombre"].Width = 400;
            D.Columns["Observacion"].Width = 150;

            D.ColumnHeadersDefaultCellStyle.Font = new Font(D.Font, FontStyle.Bold);
            D.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 8, FontStyle.Bold);
            D.ColumnHeadersDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#bfdbff");
            D.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;
            D.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;

            D.Columns["CodigoInterno"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["CodigoProveedor"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["Nombre"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["Observacion"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

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
    }
}
