using Domain.CXN;
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

namespace ZamenisHealth.INV.Consultorios
{
    public partial class Consultorios : Forma
    {
        private MensajesGeneral MG;
        private IBodegas repoBodegas;
        private IBodegaPrincipal principal;

        DataTable dt = new DataTable();
        DataColumn POS;
        DataColumn CodigoBodega;
        DataColumn Profesional;

        public Consultorios()
        {
            InitializeComponent();
            repoBodegas = new MBodegas();
            principal = new MBodegaPrincipal();
        }

        private void Consultorios_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Consultorios";
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";
            LogoMain.Image = Properties.Resources.Splash;

            ToolStripButton btnCharge = new ToolStripButton();
            btnCharge = createToolButton("Cargar Consultorio");
            MenuLateral.Items.Add(btnCharge);
            btnCharge.Click += toolStripButton2_Click;

            ToolStripButton btnMasivo = new ToolStripButton();
            btnMasivo = createToolButton("Masivo");
            MenuLateral.Items.Add(btnMasivo);
            btnMasivo.Click += toolStripButton3_Click;

            ToolStripButton btnHistorial = new ToolStripButton();
            btnHistorial = createToolButton("Historial");
            MenuLateral.Items.Add(btnHistorial);
            btnHistorial.Click += toolStripButton4_Click;

            CargarBodegas();
        }

        internal class Items
        {
            public string Codigo { get; set; }
            public string Profesional { get; set; }
        }

        void CargarBodegas()
        {
            try
            {
                List<CXN_BODEGAS> bodegas = principal.LoadConsultorios();
                if (bodegas != null)
                {
                    List<Items> I = new List<Items>();

                    foreach (CXN_BODEGAS bodega in bodegas)
                    {
                        I.Add(new Items
                        {
                            Codigo = bodega.Bod_Numero.ToString(),
                            Profesional = bodega.Bod_Responsable
                        });
                    }

                    CargarGrilla(I);
                }
                else
                {
                    MG = new MensajesGeneral();
                    MG.Mensaje = "No hay bodegas disponibles";
                    MG.TipoImagen = 1000;
                    MG.ShowDialog();

                    this.Dispose();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void CargarGrilla(List<Items> _lista)
        {
            try
            {
                if (_lista != null)
                {
                    Encabezados();

                    int Contador = 1;

                    foreach (Items i in _lista)
                    {
                        DataRow row = dt.NewRow();

                        row["POS"] = Contador;
                        row["CodigoBodega"] = Convert.ToInt32(i.Codigo);
                        row["Profesional"] = i.Profesional.ToString();

                        dt.Rows.Add(row);
                        dt.AcceptChanges();

                        Contador = Contador + 1;
                    }

                    DataRow row2 = dt.NewRow();

                    row2["POS"] = Contador;
                    row2["CodigoBodega"] = Convert.ToInt32(88);
                    row2["Profesional"] = "VENTAS";

                    dt.Rows.Add(row2);
                    dt.AcceptChanges();

                    Contador = Contador + 1;

                    row2 = dt.NewRow();

                    row2["POS"] = Contador;
                    row2["CodigoBodega"] = Convert.ToInt32(888);
                    row2["Profesional"] = "PRESTAMO";

                    dt.Rows.Add(row2);
                    dt.AcceptChanges();

                    Contador = Contador + 1;

                    row2 = dt.NewRow();

                    row2["POS"] = Contador;
                    row2["CodigoBodega"] = Convert.ToInt32(8888);
                    row2["Profesional"] = "DOMICILIOS";

                    dt.Rows.Add(row2);
                    dt.AcceptChanges();

                    Contador = Contador + 1;

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
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        void Estilos(DataGridView D, DataTable t)
        {
            D.EnableHeadersVisualStyles = false;
            D.ScrollBars = ScrollBars.Both;

            D.DataSource = t;

            D.Columns["CodigoBodega"].Width = 150;
            D.Columns["Profesional"].Width = 600;

            D.ColumnHeadersDefaultCellStyle.Font = new Font(D.Font, FontStyle.Bold);
            D.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 8, FontStyle.Bold);
            D.ColumnHeadersDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#bfdbff");
            D.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;
            D.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;

            D.Columns["CodigoBodega"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["Profesional"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

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

        void Encabezados()
        {
            dataGridView1.DataSource = null;
            dt = new DataTable();
            POS = dt.Columns.Add("POS", typeof(int));
            CodigoBodega = dt.Columns.Add("CodigoBodega", typeof(int));
            Profesional = dt.Columns.Add("Profesional", typeof(string));
        }
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            CargarBodegas();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Dispose();
            this.Close();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int code = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString());

            InvConsultorio l = new InvConsultorio(code);
            l.ShowDialog();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            MasivoConsultorios masivoConsultorios = new MasivoConsultorios();
            masivoConsultorios.ShowDialog();
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            Historial historial = new Historial();
            historial.ShowDialog();
        }
    }
}
