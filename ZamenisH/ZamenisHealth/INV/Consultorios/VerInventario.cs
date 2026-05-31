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
using ZamenisHealth.Clases;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.INV.Consultorios
{
    public partial class VerInventario : Forma
    {
        private ISubBodegas bodegasS;
        private int CodeBodega;
        private MensajesGeneral MG;
        private bool Editar;

        DataTable dt = new DataTable();
        DataColumn POS;
        DataColumn Id;
        DataColumn Codigo;
        DataColumn Nombre;
        DataColumn Cantidad;

        public VerInventario(int codeBodega, bool editar)
        {
            InitializeComponent();
            CodeBodega = codeBodega;
            bodegasS = new MSubBodegas();
            Editar = editar;
        }

        private void VerInventario_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Ver Inventario";
            LogoMain.Image = Properties.Resources.Splash;
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";

            ToolStripButton btnExportar;

            btnExportar = new ToolStripButton();
            btnExportar = createToolButton("Exportar");
            MenuLateral.Items.Add(btnExportar);
            btnExportar.Click += toolStripButton2_Click;

            CargarInventario();
            textBox1.TextChanged += TextBox1_TextChanged;
        }

        private void TextBox1_TextChanged(object sender, EventArgs e)
        {
            CargarInventario();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Dispose();
            this.Close();
        }
        void Encabezados()
        {
            dataGridView1.DataSource = null;
            dt = new DataTable();
            POS = dt.Columns.Add("POS", typeof(int));
            Id = dt.Columns.Add("Id", typeof(int));
            Codigo = dt.Columns.Add("Codigo", typeof(string));
            Nombre = dt.Columns.Add("Nombre", typeof(string));
            Cantidad = dt.Columns.Add("Cantidad", typeof(int));
        }
        public void CargarInventario()
        {
            try
            {
                List<INV_HISTORICOPPAL> _lista;

                if (string.IsNullOrEmpty(textBox1.Text))
                {
                    _lista = new List<INV_HISTORICOPPAL>();
                    _lista = bodegasS.LoadInventary(CodeBodega, "");
                }
                else
                {
                    _lista = new List<INV_HISTORICOPPAL>();
                    _lista = bodegasS.LoadInventary(CodeBodega, textBox1.Text);
                }
                
                if (_lista != null)
                {
                    Encabezados();

                    int Contador = 1;

                    foreach (INV_HISTORICOPPAL i in _lista)
                    {
                        DataRow row = dt.NewRow();

                        row["POS"] = Contador;
                        row["Id"] = i.Id;
                        row["Codigo"] = i.CodigoProveedor.ToString(); //es el interno en este caso
                        row["Nombre"] = i.Nombre.ToString();
                        row["Cantidad"] = i.Cantidad;

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

            D.Columns["Codigo"].Width = 120;
            D.Columns["Nombre"].Width = 470;
            D.Columns["Cantidad"].Width = 100;

            D.ColumnHeadersDefaultCellStyle.Font = new Font(D.Font, FontStyle.Bold);
            D.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 8, FontStyle.Bold);
            D.ColumnHeadersDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#bfdbff");
            D.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;
            D.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;

            D.Columns["Codigo"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["Nombre"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["Cantidad"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

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

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            try
            {
                List<INV_HISTORICOPPAL> _lista = bodegasS.LoadInventary(CodeBodega, "");
                if (_lista != null)
                {
                    ConfigForm.GenerarReportViewer("DataSetInventarios", "ZamenisHealth.Reportes.INV_INVENTARIO.rdlc", _lista);
                }
                else
                {
                    MG = new MensajesGeneral();
                    MG.Mensaje = "No hay datos para exportar";
                    MG.TipoImagen = 3;
                    MG.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (Editar == true)
            {
                int Posision = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString());
                string Nombre = dataGridView1.Rows[e.RowIndex].Cells[3].Value.ToString();

                GestionInventarios gestionInventarios = new GestionInventarios(Posision, Nombre);
                gestionInventarios.ShowDialog();
            }
        }
    }
}
