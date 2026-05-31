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

namespace ZamenisHealth.Medicina.OrdenesExtra
{
    public partial class CrearOrdenExtra : Forma
    {
        private IPacientes pacientesController;
        private ICompañia compañiaController;
        private IMedicinaGeneral medicinaController;
        private IFisiatria fisiatriaController;
        private IBodegas bodegasController;

        private MensajesGeneral MG;
        private int Paciente;

        DataTable dt;
        DataColumn POS;
        DataColumn Admision;
        DataColumn Fecha;
        DataColumn Profesional;

        ToolStripButton btnBuscar;
        private string TServ;

        public CrearOrdenExtra(string tServ)
        {
            InitializeComponent();
            pacientesController = new MPacientes();
            compañiaController = new MCompañia();
            medicinaController = new MMedicinaGeneral();
            fisiatriaController = new MFisiatria();
            bodegasController = new MBodegas();
            TServ = tServ;
        }

        private void CrearOrdenExtra_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Medicamentos Extra";
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";
            CargarPrestador();

            ToolStripButton btnRecMedica = new ToolStripButton();
            btnRecMedica = createToolButton("Recomendaciones");
            MenuLateral.Items.Add(btnRecMedica);
            btnRecMedica.Click += button1_Click;

            btnBuscar = new ToolStripButton();
            btnBuscar = createToolButton("Buscar");
            MenuLateral.Items.Add(btnBuscar);
            btnBuscar.Click += boton1_Click;
        }
        void button1_Click(object sender, EventArgs e)
        {
            OrdenesMedicas ordenesMedicas = new OrdenesMedicas(TServ);
            ordenesMedicas.ShowDialog();
        }
        void CargarPrestador()
        {
            List<CXN_CIA> getcias = compañiaController.getAllCompañias();
            if (getcias != null)
            {
                foreach (var i in getcias)
                {
                    comboBox1.Items.Add(i.Com_Nombre);
                }

                comboBox1.SelectedIndex = 0;
            }
        }
        private void boton1_Click(object sender, EventArgs e)
        {
            CXN_PACIENTES P = pacientesController.LlamarPacienteNumDoc(textBox1.Text.Trim());
            if (P != null)
            {
                Paciente = P.Pac_Id;

                ActualizarPaciente A = new ActualizarPaciente(Paciente);
                A.ShowDialog();

                textBox1.Enabled = false;
                btnBuscar.Enabled = false;

                ConsultarHistorias();
            }
            else
            {
                MG = new MensajesGeneral()
                {
                    Mensaje = "El documento no existe",
                    TipoImagen = 1000
                };
                MG.ShowDialog();
            }
        }
        void ConsultarHistorias()
        {
            try
            {
                int CodeBod = bodegasController.getDatosUser(Contenedor.UsuarioLogueado).Bod_Numero;
                List<CXN_HCMG> lista = new List<CXN_HCMG>();

                if (TServ == "MG")
                {
                    lista = medicinaController.ListaUltimasCitas(Paciente, DateTime.Now.Date, CodeBod);
                }
                if (TServ == "FI")
                {
                    lista = fisiatriaController.ListaUltimasCitas(Paciente, DateTime.Now.Date, CodeBod);
                }
                
                if (lista != null)
                {
                    Encabezados();
                    int Contador = 1;

                    foreach (CXN_HCMG i in lista)
                    {
                        DataRow row = dt.NewRow();

                        row[POS] = Contador;
                        row[Admision] = i.HC_Adm;
                        row[Fecha] = i.HC_Fecha.ToString("yyyy-MM-dd");
                        row[Profesional] = i.HC_IMC;

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
                MessageBox.Show(ex.Message);
            }
        }
        void Encabezados()
        {
            dataGridView1.DataSource = null;
            dt = new DataTable();
            POS = dt.Columns.Add("POS", typeof(int));
            Admision = dt.Columns.Add("Admision", typeof(int));
            Fecha = dt.Columns.Add("Fecha", typeof(string));
            Profesional = dt.Columns.Add("Profesional", typeof(string));
        }
        void Estilos(DataGridView D, DataTable t)
        {
            D.EnableHeadersVisualStyles = false;
            D.ScrollBars = ScrollBars.Both;

            D.DataSource = t;

            D.ColumnHeadersDefaultCellStyle.Font = new Font(D.Font, FontStyle.Bold);
            D.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 8, FontStyle.Bold);
            D.ColumnHeadersDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#bfdbff");
            D.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;

            D.Columns["Admision"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["Fecha"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["Profesional"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            D.Columns["POS"].Visible = false;

            D.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells + 10;
            D.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            D.DefaultCellStyle.WrapMode = DataGridViewTriState.True;

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

            dataGridView1.ClearSelection();
        }
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int Adm = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString());
            CrearOrdenExtra2 o2 = new CrearOrdenExtra2(TServ, Adm, Paciente);
            o2.ShowDialog();

            dataGridView1.ClearSelection();
        }

        private void textBox1_DoubleClick(object sender, EventArgs e)
        {
            BuscarPacientes a = new BuscarPacientes("OrdenesFHIRExtra");
            a.ShowDialog();
        }
        public void setDoc(string Doc)
        {
            textBox1.Text = Doc;
        }
    }
}
