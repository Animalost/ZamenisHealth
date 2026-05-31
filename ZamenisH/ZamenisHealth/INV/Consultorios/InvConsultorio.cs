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

namespace ZamenisHealth.INV.Consultorios
{
    public partial class InvConsultorio : Forma
    {
        private static readonly ICompañia repoCia = new MCompañia();
        private static readonly IBodegaPrincipal repoBodegas = new MBodegaPrincipal();

        private int CodeBodega, CodPrestador, CodProveedor;
        private bool Filtrar = false;
        private ToolStripButton btnInventario;

        DataTable dt = new DataTable();
        DataColumn POS;
        DataColumn IdInvPpal;
        DataColumn CodigoInterno;
        DataColumn CodigoProveedor;
        DataColumn Nombre;
        DataColumn Lote;
        DataColumn Factura;
        DataColumn Cantidad;
        DataColumn Costo;

        public InvConsultorio(int codeBodega)
        {
            InitializeComponent();
            CodeBodega = codeBodega;
        }
        void Encabezados()
        {
            dataGridView1.DataSource = null;
            dt = new DataTable();
            POS = dt.Columns.Add("POS", typeof(int));
            IdInvPpal = dt.Columns.Add("IdInvPpal", typeof(int));
            CodigoInterno = dt.Columns.Add("CodigoInterno", typeof(string));
            CodigoProveedor = dt.Columns.Add("CodigoProveedor", typeof(string));
            Nombre = dt.Columns.Add("Nombre", typeof(string));
            Lote = dt.Columns.Add("Lote", typeof(string));
            Factura = dt.Columns.Add("Factura", typeof(string));
            Cantidad = dt.Columns.Add("Cantidad", typeof(int));
            Costo = dt.Columns.Add("Costo", typeof(int));
        }

        private void InvConsultorio_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Inventarios";
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";
            LogoMain.Image = Properties.Resources.Splash;

            btnInventario = new ToolStripButton();
            btnInventario = createToolButton("Ver Inventarios");
            MenuLateral.Items.Add(btnInventario);
            btnInventario.Click += toolStripButton2_Click;

            ToolStripButton btnProds = new ToolStripButton();
            btnProds = createToolButton("Productos");
            MenuLateral.Items.Add(btnProds);
            btnProds.Click += toolStripButton3_Click;

            Filtrar = true;

            CargarPrestadores();
            CargarGrilla();

            Filtrar = true;

            if (CodeBodega == 88)
            {
                btnInventario.Visible = false;
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
        public void CargarGrilla()
        {
            try
            {
                if (Filtrar == true)
                {
                    INV_PRODUCTOS inv = new INV_PRODUCTOS
                    {
                        Nombre = textBox1.Text.Trim(),
                        Prestador = CodPrestador,
                        Proveedor = CodProveedor
                    };

                    List<INV_HISTORICOPPAL> _lista = repoBodegas.GetInventary(inv);
                    if (_lista != null)
                    {
                        Encabezados();

                        int Contador = 1;

                        foreach (INV_HISTORICOPPAL i in _lista)
                        {
                            DataRow row = dt.NewRow();

                            row["POS"] = Contador;
                            row["IdInvPpal"] = i.Id;
                            row["CodigoInterno"] = i.CodigoInterno.ToString();
                            row["CodigoProveedor"] = i.CodigoProveedor.ToString();
                            row["Nombre"] = i.Nombre.ToString();
                            row["Lote"] = i.Lote.ToString();
                            row["Factura"] = i.Factura.ToString();
                            row["Cantidad"] = i.Cantidad.ToString();
                            row["Costo"] = i.Costo.ToString();

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
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar la grilla: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        void Estilos(DataGridView D, DataTable t)
        {
            D.EnableHeadersVisualStyles = false;
            D.ScrollBars = ScrollBars.Both;

            D.DataSource = t;

            D.Columns["CodigoInterno"].Width = 150;
            D.Columns["CodigoProveedor"].Width = 150;
            D.Columns["Nombre"].Width = 400;
            D.Columns["Lote"].Width = 150;
            D.Columns["Factura"].Width = 150;
            D.Columns["Cantidad"].Width = 150;
            D.Columns["Costo"].Width = 200;

            D.ColumnHeadersDefaultCellStyle.Font = new Font(D.Font, FontStyle.Bold);
            D.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 8, FontStyle.Bold);
            D.ColumnHeadersDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#bfdbff");
            D.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;
            D.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;

            D.Columns["CodigoInterno"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["CodigoProveedor"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["Nombre"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["Lote"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["Factura"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["Cantidad"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["Costo"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            D.Columns["POS"].Visible = false;
            D.Columns["IdInvPpal"].Visible = false;

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
        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            CrearProductos crearProductos = new CrearProductos();
            crearProductos.ShowDialog();
        }
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            VerInventario ver = new VerInventario(CodeBodega, false);
            ver.ShowDialog();
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            CargarGrilla();
        }
        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            int posInvPpal = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString());
            CargarConsultorio cargarConsultorio = new CargarConsultorio(CodeBodega, posInvPpal);
            cargarConsultorio.ShowDialog();
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
    }
}
