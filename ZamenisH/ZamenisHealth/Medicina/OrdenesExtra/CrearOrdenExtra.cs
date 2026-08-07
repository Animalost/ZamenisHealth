using Domain.CXN;
using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Collections.Generic;
using System.Data;
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

            gridZH1.dataGridView1.CellClick += dataGridView1_CellClick;
            gridZH1.CeldaHeight = true;
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
                    Estilos(gridZH1.dataGridView1, dt);
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
            gridZH1.dataGridView1.DataSource = null;
            dt = new DataTable();
            POS = dt.Columns.Add("POS", typeof(int));
            Admision = dt.Columns.Add("Admision", typeof(int));
            Fecha = dt.Columns.Add("Fecha", typeof(string));
            Profesional = dt.Columns.Add("Profesional", typeof(string));
        }
        void Estilos(DataGridView D, DataTable t)
        {
            D.DataSource = t;
            D.Columns["POS"].Visible = false;
            gridZH1.dataGridView1.ClearSelection();
        }
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                int Adm = Convert.ToInt32(gridZH1.dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString());
                CrearOrdenExtra2 o2 = new CrearOrdenExtra2(TServ, Adm, Paciente);
                o2.ShowDialog();

                gridZH1.dataGridView1.ClearSelection();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }            
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
