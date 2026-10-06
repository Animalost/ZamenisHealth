using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Data;
using System.Windows.Forms;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.Medicina
{
    public partial class AprobarSalidas : Forma
    {
        private IBodegas repositorioBodegas;
        private IMedicinaGeneral repoMGeneral;

        DataTable dt;
        DataColumn Admision;
        DataColumn Fecha;
        DataColumn Paciente;
        DataColumn Profesional;
        DataColumn Estado;

        public AprobarSalidas()
        {
            InitializeComponent();
            repositorioBodegas = new MBodegas();
            repoMGeneral = new MMedicinaGeneral();
        }

        private void AprobarSalidas_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Aprobar Salidas";
            LogoMain.Image = Properties.Resources.Splash;
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";

            gridZH1.CeldaHeight = true;
            CargarSalidas();

            ToolStripButton btnRecagar = new ToolStripButton();
            btnRecagar = createToolButton("Recagar");
            MenuLateral.Items.Add(btnRecagar);
            btnRecagar.Click += btnRecagar_Click;

            var esmedico = repositorioBodegas.EsProfesional("Medico", Comunes.Contenedor.UsuarioLogueado);
            if (esmedico != true)
            {
                MessageBox.Show("Su usuario no es tipo medico, no puede autorizar solicitudes de salida",
                    "Acceso Denegado!!!",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                this.Dispose();
                this.Close();
                return;
            }

            gridZH1.dataGridView1.CellClick += DataGridView1_CellClick;
        }
        private void DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                int Cod = Convert.ToInt32(gridZH1.dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString());
                string estado = gridZH1.dataGridView1.Rows[e.RowIndex].Cells[4].Value.ToString();

                if (estado != "APROBADO")
                {
                    AprobarSalidas2 A = new AprobarSalidas2(Cod);
                    A.ShowDialog();
                }                
                else
                {
                    MensajesGeneral MG = new MensajesGeneral()
                    {
                        Mensaje = "Esta solicitud ya fue aprobada",
                        TipoImagen = 3
                    };
                    MG.ShowDialog();
                }

                gridZH1.dataGridView1.ClearSelection();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        void btnRecagar_Click(object sender, EventArgs e)
        {
            CargarSalidas();
        }
        void Encabezados()
        {
            gridZH1.dataGridView1.DataSource = null;
            dt = new DataTable();
            Admision = dt.Columns.Add("Admision", typeof(int));
            Fecha = dt.Columns.Add("Fecha", typeof(string));
            Paciente = dt.Columns.Add("Paciente", typeof(string));
            Profesional = dt.Columns.Add("Profesional", typeof(string));
            Estado = dt.Columns.Add("Estado", typeof(string)); 
        }
        public void CargarSalidas()
        {
            try
            {
                var _lista = repoMGeneral.GetSalidasEnfermeria();
                if (_lista != null)
                {
                    Encabezados();

                    foreach (var i in _lista)
                    {
                        DataRow row = dt.NewRow();

                        row[Admision] = i.Hor_Id;
                        row[Fecha] = i.Hor_Pac_Fecha_Cita.ToString("yyyy-MM-dd");
                        row[Paciente] = i.Hor_Imp_Age.ToString();
                        row[Profesional] = i.Com_Cod_Prestador;
                        row[Estado] = i.Hor_Estado.ToString();

                        dt.Rows.Add(row);
                        dt.AcceptChanges();
                    }
 
                    gridZH1.dataGridView1.DataSource = dt;
                    gridZH1.dataGridView1.ClearSelection();
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
    }
}
