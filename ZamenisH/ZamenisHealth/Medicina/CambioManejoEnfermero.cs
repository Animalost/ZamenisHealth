using Domain;

using FormAndControls;

using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;

using System;
using System.Data;
using System.Windows.Forms;

using ZamenisHealth.Clases;
using ZamenisHealth.Clases.Controles;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.Medicina
{
    public partial class CambioManejoEnfermero : Forma
    {
        private static readonly ICompañia repositorioCompañias = new MCompañia();
        private static readonly IBodegas repositorioBodegas = new MBodegas();
        private static readonly IPacientes repositorioPacientes = new MPacientes();
        private static readonly ICManejo repositorioCMan = new MCManenejo();
        private static readonly IReportes repositorioReportes = new MReportes();

        int Cia = 0;
        int Pac_Id = 0;

        private DataTable dt;
        private DataColumn POS;
        private DataColumn Id;
        private DataColumn UsuarioG;
        private DataColumn Fecha;

        private ToolStripButton btnGrabar, btnBuscar;

        public CambioManejoEnfermero()
        {
            InitializeComponent();          
        }

        private void CambioManejoEnfermero_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Cambios de Manejo";
            LogoMain.Image = Properties.Resources.Splash;

            btnBuscar = new ToolStripButton();
            btnBuscar = createToolButton("Buscar");
            MenuLateral.Items.Add(btnBuscar);
            btnBuscar.Click += button1_Click;

            btnGrabar = new ToolStripButton();
            btnGrabar = createToolButton("Grabar");
            MenuLateral.Items.Add(btnGrabar);
            btnGrabar.Click += toolStripButton2_Click;
            btnGrabar.Enabled = false;

            gridZH1.dataGridView1.CellDoubleClick += dataGridView1_CellDoubleClick;
            gridZH1.CeldaHeight = true;

            CargarDocumentos();

            var Cias = repositorioCompañias.getAllCompañias();
            if (Cias != null)
            {
                foreach (var i in Cias)
                {
                    comboBox2.Items.Add(i.Com_Nombre);
                }

                comboBox2.SelectedIndex = 0;
            }


            var ValidaUsuario = repositorioBodegas.EsProfesional("Enfermero", Comunes.Contenedor.UsuarioLogueado);
            if (ValidaUsuario != true)
            {
                MessageBox.Show("Su usuario no es tipo enfermero, no puede continuar",
                    "Acceso Denegado!!!",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                this.Dispose();
                this.Close();
            }
        }
        public void CargarDocumentos()
        {
            var ListaDocs = repositorioPacientes.ListaDocs();
            if (ListaDocs != null)
            {
                foreach (var i in ListaDocs)
                {
                    comboBox1.Items.Add(i);
                }
            }
        }
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            var IdCia = repositorioCompañias.getPrestadorbyName(comboBox2.Text);
            Cia = IdCia.Com_Identificador;
        }
        private void textBox1_DoubleClick(object sender, EventArgs e)
        {
            Comunes.BuscarPacientes P = new Comunes.BuscarPacientes("CMANE");
            P.ShowDialog();
        }
        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            textBox2.CharacterCasing = CharacterCasing.Upper;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            BuscarPac();
        }
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            try
            {
                Comunes.MensajesGeneral MG = new Comunes.MensajesGeneral();

                if (Pac_Id == 0 || Cia == 0 || textBox2.Text == "")
                {
                    MG.Mensaje = "No hay paciente seleccionado o no ha registrado ninguna informacion en el campo de texto, revise la informacion";
                    MG.TipoImagen = 1000;
                    MG.ShowDialog();
                    return;
                }

                Domain.CXN.CXN_CMAN C = new Domain.CXN.CXN_CMAN();
                C.Cam_IdPac = Pac_Id;
                C.Cam_Descripcion = textBox2.Text;
                C.Cam_Estado = "G";
                C.Cam_UsrGenera = Comunes.Contenedor.UsuarioLogueado;
                C.Cam_Cia = Cia;

                var _insertCambio = repositorioCMan.insertarCambioManejo(C);
                if (_insertCambio != true)
                {
                    MG.Mensaje = "No se logro insertar el cambio";
                    MG.TipoImagen = 1000;
                    MG.ShowDialog();
                }
                else
                {
                    MG.Mensaje = "Cambio registrado correctamente";
                    MG.TipoImagen = 3;
                    MG.ShowDialog();
                }

                this.Dispose();
                this.Close();
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void BuscarPac()
        {
            try
            {
                var DatPac = repositorioPacientes.LlamarPacienteDOC(comboBox1.Text, textBox1.Text);
                if (DatPac == null)
                {
                    Comunes.MensajesGeneral MG = new Comunes.MensajesGeneral
                    {
                        Mensaje = "Inconveniente con este paciente, no se puede cargar datos",
                        TipoImagen = 1000
                    };
                    MG.ShowDialog();
                    return;
                }

                label4.Text = DatPac.Pac_PrimerN + " " + DatPac.Pac_SegundoN + " " + DatPac.Pac_PrimerA + " " + DatPac.Pac_SegundoA;
                Pac_Id = Convert.ToInt32(DatPac.Pac_Id);
                btnBuscar.Enabled = false;
                btnGrabar.Enabled = true;
                comboBox1.Enabled = false;
                textBox1.Enabled = false;

                Consultar_Manejos(Pac_Id);

            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        void Consultar_Manejos(int _Pac_Id)
        {
            var datos = repositorioCMan.getManejosxPaciente(_Pac_Id);
            if (datos != null)
            {
                Encabezados();

                panel1.Visible = true;
                int Contador = 1;

                foreach (var i in datos)
                {
                    DataRow row = dt.NewRow();

                    row[POS] = Contador;
                    row[Id] = i.Cam_Id.ToString();
                    row[UsuarioG] = i.Cam_UsrGenera.ToString();
                    row[Fecha] = Convert.ToDateTime(i.Cam_Fecha).ToString(Conexion.ConectionDictionary["Format_Fecha"]);

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
                panel1.Visible = false;
            }
        }
        void Estilos(DataGridView D, DataTable t)
        {
            D.DataSource = t;
            D.Columns["POS"].Visible = false;          
        }
        void Encabezados()
        {
            gridZH1.dataGridView1.DataSource = null;

            dt = new DataTable();
            POS = dt.Columns.Add("POS", typeof(int));
            Id = dt.Columns.Add("Id", typeof(int));
            UsuarioG = dt.Columns.Add("Usuario", typeof(string));
            Fecha = dt.Columns.Add("Fecha", typeof(DateTime));
        }
        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                Comunes.MensajesGeneral MG = new Comunes.MensajesGeneral();

                var H_HCCMAN = repositorioReportes.CambiosManejo(Convert.ToInt32(gridZH1.dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString()));
                if (H_HCCMAN == null)
                {
                    MG.Mensaje = "No se logro exportar, posiblemente halla una falla al exportar o la historia no existe";
                    MG.TipoImagen = 1000;
                    MG.ShowDialog();
                    return;
                }

                ConfigForm.GenerarReportViewer("DataSet_CMAN",
                                             "ZamenisHealth.Reportes.RDLC_CMAN.rdlc",
                                             H_HCCMAN);
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            panel1.Visible = false;
        }
    }
}
