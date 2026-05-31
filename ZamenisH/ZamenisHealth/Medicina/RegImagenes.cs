using Domain;
using Domain.CXN;
using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using ZamenisHealth.Comunes;
using ZamenisHealth.Medicina.Extras;

namespace ZamenisHealth.Medicina
{
    public partial class RegImagenes : Forma
    {
        private static readonly IPacientes repoPacientes = new MPacientes();
        private static readonly IImagenes repoImagenes = new MImagenes();

        int Paciente;

        private DataTable dt;
        private DataColumn POS;
        private DataColumn Admision;
        private DataColumn Fecha;
        private DataColumn Profesional;

        public RegImagenes()
        {
            InitializeComponent();
        }
        private void CargarDocumentos()
        {
            var ListaDocs = repoPacientes.ListaDocs();
            if (ListaDocs != null)
            {
                foreach (var i in ListaDocs)
                {
                    comboBox1.Items.Add(i);
                }
            }
        }
        private void Encabezados()
        {
            try
            {
                dt = new DataTable();
                POS = dt.Columns.Add("POS", typeof(int));
                Admision = dt.Columns.Add("Admision", typeof(int));
                Fecha = dt.Columns.Add("Fecha", typeof(DateTime));
                Profesional = dt.Columns.Add("Profesional", typeof(string));
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void RegImagenes_Load(object sender, EventArgs e)
        {
            

            Titulo.Text = "Imagenes";

            LogoMain.Image = Properties.Resources.Splash;

            ToolStripButton btnBuscar = new ToolStripButton();
            btnBuscar = createToolButton("Buscar");
            MenuLateral.Items.Add(btnBuscar);
            btnBuscar.Click += button1_Click;

            comboBox3.SelectedIndex = 1;
            CargarDocumentos();
        }    
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                var getHistorias = repoImagenes.getHistorias(comboBox1.Text, textBox1.Text);
                if (getHistorias != null)
                {
                    Encabezados();

                    int Contador = 1;

                    foreach (CXN_HORARIO i in getHistorias)
                    {
                        DataRow row = dt.NewRow();

                        row["POS"] = Contador;
                        row["Admision"] = i.Hor_Id.ToString();
                        row["Fecha"] = Convert.ToDateTime(i.Hor_Pac_Fecha_Cita).ToString(Conexion.ConectionDictionary["Format_Fecha"]);
                        row["Profesional"] = i.Hor_Observacion.ToString();

                        dt.Rows.Add(row);
                        dt.AcceptChanges();

                        Contador = Contador + 1;

                        Paciente = Convert.ToInt32(i.Hor_Pac_Id);
                    }

                    Contador = 1;
                    Estilos(dataGridView1, dt);
                }
                else
                {
                    Encabezados();

                    MessageBox.Show("Este paciente no tiene historias clinicas hechas para agregarle imagenes",
                        "Sin datos",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
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

            D.Columns["Admision"].Width = 120;
            D.Columns["Fecha"].Width = 150;
            D.Columns["Profesional"].Width = 445;

            D.ColumnHeadersDefaultCellStyle.Font = new Font(D.Font, FontStyle.Bold);
            D.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 8, FontStyle.Bold);
            D.ColumnHeadersDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#bfdbff");
            D.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;
            D.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;

            D.Columns["Admision"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["Fecha"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["Profesional"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            D.Columns["Admision"].SortMode = DataGridViewColumnSortMode.NotSortable;
            D.Columns["Fecha"].SortMode = DataGridViewColumnSortMode.NotSortable;
            D.Columns["Profesional"].SortMode = DataGridViewColumnSortMode.NotSortable;

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
        public void CerrarPanel()
        {
            panel1.Visible = false;
        }
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                label5.Text = dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();

                if (comboBox3.Text == "Subir una Imagen")
                {
                    panel1.Visible = true;
                    Extras.RegImagenesSubir s = new Extras.RegImagenesSubir(Convert.ToInt32(label5.Text), Paciente, false, "", "");
                    s.ShowDialog();
                }
                else if (comboBox3.Text == "Usar la WebCam")
                {
                    panel1.Visible = true;
                    Extras.RegImagenesCamara a = new RegImagenesCamara(Convert.ToInt32(label5.Text), Paciente);
                    a.ShowDialog();
                }
                else
                {
                    Comunes.MensajesGeneral MG = new MensajesGeneral();
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "Debe seleccionar un tipo de captura";
                    MG.ShowDialog();

                    comboBox3.Text = "";
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
    }
}
