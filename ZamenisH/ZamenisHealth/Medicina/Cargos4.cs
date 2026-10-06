using Domain;
using Domain.CONSUMOS;
using FormAndControls;
using Persistence;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.Medicina
{
    public partial class Cargos4 : Forma2
    {
        private List<CON_PRODUCTOS> Lista;

        DataTable dt;
        DataColumn IdProducto;
        DataColumn CodigoInterno;
        DataColumn Producto;

        public Cargos4(List<CON_PRODUCTOS> lista)
        {
            InitializeComponent();
            Lista = lista;
        }

        private void Cargos4_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Seleccione producto asociado de la lista";
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";

            ImageClose.Visible = false;
            ImageMinimize.Visible = false;

            gridZH1.CeldaHeight = true;
            gridZH1.dataGridView1.CellClick += DataGridView1_CellClick;

            CargarGrilla();
        }
        void Encabezados()
        {
            dt = new DataTable();
            IdProducto = dt.Columns.Add("IdProducto", typeof(int));
            CodigoInterno = dt.Columns.Add("Codigo Interno", typeof(string));
            Producto = dt.Columns.Add("Producto", typeof(string));            
        }
        void CargarGrilla()
        {
            try
            {                
                if (Lista != null)
                {
                    Encabezados();

                    int Contador = 1;

                    foreach (var i in Lista)
                    {
                        DataRow row = dt.NewRow();

                        row[IdProducto] = Contador;
                        row[CodigoInterno] = i.Con_Prod_Cod_Interno.ToString();
                        row[Producto] = i.Con_Prod_Name.ToString();

                        dt.Rows.Add(row);
                        dt.AcceptChanges();
                    }

                    Estilos(gridZH1.dataGridView1, dt);
                }
                else
                {
                    gridZH1.dataGridView1.DataSource = null;
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
            D.DataSource = t;
            D.Columns["IdProducto"].Visible = false;
        }
        private void DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                int Pos = Convert.ToInt32(gridZH1.dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString());
                if (Pos <= 0)
                {
                    MensajesGeneral MG = new MensajesGeneral()
                    {
                        Mensaje = "Seleccion invalida, vuelva a intentar",
                        TipoImagen = 1000
                    };

                    MG.ShowDialog();
                }
                else
                {
                    Cargos2 f1 = Application.OpenForms.OfType<Cargos2>().LastOrDefault();
                    f1.IdProducto = Pos;

                    this.Close();
                }                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
