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

namespace ZamenisHealth.Recepcion.Ventas
{
    public partial class BuscarProducto : Forma2
    {
        private IVender oController;

        DataTable dt;
        DataColumn POS;
        DataColumn Codigo;
        DataColumn Producto;
        DataColumn Valor;

        Vender _formulario;

        public BuscarProducto(Vender formulario)
        {
            InitializeComponent();
            oController = new MVender();
            _formulario = formulario;
        }

        private void BuscarProducto_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Buscar Producto";
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";

            textBox1.TextChanged += textBox1_textChanged;

            CargarProducto();
        }
        void Encabezados()
        {
            dt = new DataTable();
            POS = dt.Columns.Add("POS", typeof(int));
            Codigo = dt.Columns.Add("Codigo", typeof(string));
            Producto = dt.Columns.Add("Producto", typeof(string));
            Valor = dt.Columns.Add("Valor", typeof(int));
        }

        void textBox1_textChanged(object sender, EventArgs e)
        {
            CargarProducto();
        }

        void CargarProducto()
        {
            try
            {
                List<CXN_INVENTARIO> _lista = oController.getProductbyName(textBox1.Text.Trim());
                if (_lista != null)
                {
                    Encabezados();

                    int Contador = 1;

                    foreach (CXN_INVENTARIO i in _lista)
                    {
                        DataRow row = dt.NewRow();

                        row[POS] = Contador;
                        row[Codigo] = i.InvCod.ToString();
                        row[Producto] = i.InvItem.ToString();
                        row[Valor] = Convert.ToInt32(i.InvPrecio);

                        dt.Rows.Add(row);
                        dt.AcceptChanges();

                        Contador = Contador + 1;
                    }

                    Contador = 1;
                    Estilos(dataGridView1, dt);
                }
                else
                {
                    dataGridView1.DataSource = null;
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

            D.Columns["Codigo"].Width = 100;
            D.Columns["Producto"].Width = 480;
            D.Columns["Valor"].Width = 100;

            D.ColumnHeadersDefaultCellStyle.Font = new Font(D.Font, FontStyle.Bold);
            D.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 8, FontStyle.Bold);
            D.ColumnHeadersDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#bfdbff");
            D.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;
            D.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;

            D.Columns["Codigo"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["Producto"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["Valor"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

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

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0)
                {
                    _formulario.setValores(dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString());
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }   
        }
    }
}
