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

namespace ZamenisHealth.Recepcion.Extras
{
    public partial class Precios : Forma
    {
        private static readonly IInventario repositorioInventario = new MInventario();

        private ToolStripButton btnHistorico, btnExportar;
        public ToolStripButton btnCotizaciones = new ToolStripButton();
        private MensajesGeneral MG;

        DataTable dt;
        DataColumn POS;
        DataColumn Codigo;
        DataColumn Item;
        DataColumn Valor;

        public Precios()
        {
            InitializeComponent();
        }

        private void Precios_Load(object sender, EventArgs e)
        {
            

            Titulo.Text = "Precios";
            LogoMain.Image = Properties.Resources.Splash;
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";

            btnCotizaciones = new ToolStripButton();
            btnCotizaciones = createToolButton("Cotizaciones");
            MenuLateral.Items.Add(btnCotizaciones);
            btnCotizaciones.Click += btnZamenis1_ButtonClick;

            btnHistorico = new ToolStripButton();
            btnHistorico = createToolButton("Historico");
            MenuLateral.Items.Add(btnHistorico);
            btnHistorico.Click += btnZamenis2_ButtonClick;

            btnExportar = new ToolStripButton();
            btnExportar = createToolButton("Exportar");
            MenuLateral.Items.Add(btnExportar);
            btnExportar.Click += btnZamenis3_ButtonClick;
        }
        private void btnZamenis1_ButtonClick(object sender, EventArgs e)
        {
            Particulares P = new Particulares();
            P.ShowDialog();
        }
        private void btnZamenis2_ButtonClick(object sender, EventArgs e)
        {
            ParticularesPrevios P = new ParticularesPrevios();
            P.ShowDialog();
        }
        private void btnZamenis3_ButtonClick(object sender, EventArgs e)
        {
            try
            {
                string _generar = repositorioInventario.listaPrecios(88);
                if (_generar == "1")
                {
                    MG = new MensajesGeneral()
                    {
                        Mensaje = "Generado en C CXN Reportes Lista_Precios.txt",
                        TipoImagen = 3
                    };

                    MG.ShowDialog();
                }
                else
                {
                    MG = new MensajesGeneral()
                    {
                        Mensaje = _generar,
                        TipoImagen = 0
                    };

                    MG.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void btnZamenis4_ButtonClick(object sender, EventArgs e)
        {
            this.Dispose();
            this.Close();
        }


        void Encabezados()
        {
            dataGridView1.DataSource = null;

            dt = new DataTable();
            POS = dt.Columns.Add("POS", typeof(int));
            Codigo = dt.Columns.Add("Codigo", typeof(string));
            Item = dt.Columns.Add("Item", typeof(string));
            Valor = dt.Columns.Add("Valor", typeof(string));
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                List<CXN_INVENTARIO> _list = repositorioInventario.getAllElements(88, textBox1.Text);
                if (_list != null)
                {
                    Encabezados();

                    int Contador = 1;

                    foreach (CXN_INVENTARIO i in _list)
                    {
                        DataRow row = dt.NewRow();

                        row[POS] = Contador;
                        row[Codigo] = i.InvCod.ToString();
                        row[Item] = i.InvItem.ToString();
                        row[Valor] = "$ " + Convert.ToInt32(i.InvPrecio).ToString("N0");

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
            D.Columns["Item"].Width = 600;
            D.Columns["Valor"].Width = 120;

            D.ColumnHeadersDefaultCellStyle.Font = new Font(D.Font, FontStyle.Bold);
            D.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 8, FontStyle.Bold);
            D.ColumnHeadersDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#bfdbff");
            D.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;
            D.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;

            D.Columns["Codigo"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["Item"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
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
    }
}
