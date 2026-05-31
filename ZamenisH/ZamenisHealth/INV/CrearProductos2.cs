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
using ZamenisHealth.Comunes;

namespace ZamenisHealth.INV
{
    public partial class CrearProductos2 : Forma2
    {
        private IProductos Productos;
        private int CodPrestador;
        private int CodProveedor;

        DataTable dt = new DataTable();
        DataColumn POS;
        DataColumn Id;
        DataColumn CodigoInterno;
        DataColumn CodigoProveedor;
        DataColumn Nombre;
        DataColumn Observacion;

        public CrearProductos2(int codPrestador, int codProveedor)
        {
            InitializeComponent();
            CodPrestador = codPrestador;
            CodProveedor = codProveedor;
            Productos = new MProductos();
        }

        private void CrearProductos2_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Crear Productos";
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";

            INV_PRODUCTOS P = new INV_PRODUCTOS
            {
                Prestador = CodPrestador,
                Proveedor = CodProveedor
            };

            CargarProductos("", P);
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
                List<INV_PRODUCTOS> _lista = Productos.CargarProductos(Filtro, P);
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

        void Estilos(DataGridView D, DataTable t)
        {
            D.EnableHeadersVisualStyles = false;
            D.ScrollBars = ScrollBars.Both;

            D.DataSource = t;

            D.Columns["CodigoInterno"].Width = 150;
            D.Columns["CodigoProveedor"].Width = 150;
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
                CrearProductos3 crearProductos3 = new CrearProductos3(Pos);
                crearProductos3.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message); 
            }
        }
    }
}
