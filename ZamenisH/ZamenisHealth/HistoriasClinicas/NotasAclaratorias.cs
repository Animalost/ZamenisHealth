using Domain;
using Domain.CXN;
using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Data;
using System.Windows.Forms;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.HistoriasClinicas
{
    public partial class NotasAclaratorias : Forma
    {
        private static readonly INotasAclaratorias repoNotasAclaratorias = new MNotasAclaratorias();
        private static readonly IPacientes repoPacientes = new MPacientes();
        private static readonly ICManejo repoManejo = new MCManenejo();
        private static readonly ILogin repositorioLogin = new MLogin();
        private static readonly IAgenda repoAgenda = new MAgenda();

        public string TipoNota;

        DataTable dt;
        DataColumn POS;
        DataColumn Admision;
        DataColumn Fecha;
        DataColumn Profesional;

        private ToolStripButton toolStripButton5;
        private ToolStripButton toolStripButton4;
        private MensajesGeneral MG;

        public NotasAclaratorias()
        {
            InitializeComponent();
        }
        void Encabezados()
        {
            gridZH1.dataGridView1.DataSource = null;
            dt = new DataTable();
            POS = dt.Columns.Add("POS", typeof(int));
            Admision = dt.Columns.Add("Admision", typeof(int));
            Fecha = dt.Columns.Add("Fecha", typeof(DateTime));
            Profesional = dt.Columns.Add("Profesional", typeof(string));
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
        void CargarBotones()
        {
            toolStripButton4 = new ToolStripButton();
            toolStripButton4 = createToolButton("Buscar");
            MenuLateral.Items.Add(toolStripButton4);
            toolStripButton4.Click += button1_Click;

            toolStripButton5 = new ToolStripButton();
            toolStripButton5 = createToolButton("Grabar");
            MenuLateral.Items.Add(toolStripButton5);
            toolStripButton5.Click += toolStripButton2_Click;
            toolStripButton5.Enabled = false;
        }
        private void NotasAclaratorias_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Notas Aclaratorias";
            LogoMain.Image = Properties.Resources.Splash;
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";

            panel1.Size = new System.Drawing.Size(811, 589);
            panel1.Location = new System.Drawing.Point(140, 54);

            gridZH1.CeldaHeight = true;
            gridZH1.dataGridView1.CellClick += dataGridView1_CellClick;

            CargarBotones();
            CargarDocumentos();

            var imageJefe = repositorioLogin.getUser(Contenedor.UsuarioLogueado);
            if (imageJefe == null)
            {
                MG.Mensaje = "No se logro validar su usuario para la consulta de notas aclaratorias.  Error general";
                MG.TipoImagen = 1000;
                MG.ShowDialog();

                this.Dispose();
                this.Close();
            }
            else
            {
                if (imageJefe.Log_NotasAcla == "A")
                {
                    return;
                }
                else
                {
                    MG = new MensajesGeneral()
                    {
                        Mensaje = "Su usuario no tiene permiso para realizar notas aclaratorias",
                        TipoImagen = 1000
                    };
                    
                    MG.ShowDialog();

                    this.Dispose();
                    this.Close();
                }
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                switch (TipoNota)
                {
                    case "Curaciones":
                        NotasMedicas(TipoNota);
                        break;

                    case "MedGen":
                        NotasMedicas(TipoNota);
                        break;

                    case "Fisiatria":
                        NotasMedicas(TipoNota);
                        break;

                    default:
                        MG = new MensajesGeneral()
                        {
                            Mensaje = "Hubo un error inesperado, cierre esta pantalla y vuelva a ingresar",
                            TipoImagen = 1000
                        };

                        MG.ShowDialog();
                        break;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void NotasMedicas(string Tipo)
        {
            try
            {
                var getNotas = repoNotasAclaratorias.NotasMedicas(comboBox1.Text, textBox1.Text, Tipo);
                if (getNotas != null)
                {
                    Encabezados();
                    int Contador = 1;

                    foreach (var i in getNotas)
                    {
                        DataRow row = dt.NewRow();

                        row[POS] = Contador;
                        row[Admision] = i.Hor_Id;
                        row[Fecha] = Convert.ToDateTime(i.Hor_Pac_Fecha_Cita).ToString(Conexion.ConectionDictionary["Format_Fecha"]);
                        row[Profesional] = i.Com_Nombre.ToString();

                        dt.Rows.Add(row);
                        dt.AcceptChanges();

                        Contador = Contador + 1;

                        label4.Text = i.Hor_Imp_Age.ToString();
                    }

                    Estilos(gridZH1.dataGridView1, dt);
                }
                else
                {
                    label4.Text = "Sin resultados";
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

            D.Columns["Admision"].Width = 120;
            D.Columns["Fecha"].Width = 120;
            D.Columns["Profesional"].Width = 500;

            D.Columns["POS"].Visible = false;
        }
        private void textBox1_DoubleClick(object sender, EventArgs e)
        {
            Comunes.BuscarPacientes P = new Comunes.BuscarPacientes();
            P.Tipo_Busca_Pac = "NotaAcla";
            P.ShowDialog();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            toolStripButton4.Enabled = false;
            panel1.Visible = false;
            richTextBox1.Text = "";
            label7.Text = "";
        }
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            try
            {
                if (richTextBox1.Text == "") { MessageBox.Show("Diligencie una nota aclaratoria", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
                if (label7.Text == "") { MessageBox.Show("Seleccione una Admision a la cual registrar la nota aclaratoria", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

                DialogResult result = MessageBox.Show("Desea registrar esta nota aclaratoria?",
                    "Zamenis Health - Edicion de Historias Clinicas",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    var Graba1 = repoManejo.GrabarManejo(TipoNota, Convert.ToInt32(label7.Text), richTextBox1.Text, Comunes.Contenedor.UsuarioLogueado);
                    if (Graba1 != true)
                    {
                        MG = new MensajesGeneral()
                        {
                            Mensaje = "No se logro agrgar la nota aclaratoria a la historia clinica",
                            TipoImagen = 1000
                        };

                        MG.ShowDialog();
                        return;
                    }

                    CXN_HISTORIAL_ADMISION HistorialAdmision = new CXN_HISTORIAL_ADMISION
                    {
                        Admision = Convert.ToInt32(label7.Text),
                        Fecha = DateTime.Now,
                        Usuario = Contenedor.UsuarioLogueado,
                        Gestion = "NOTA ACLARATORIA CREADA"
                    };

                    repoAgenda.AddHistorialAdmision(HistorialAdmision);

                    MG = new MensajesGeneral()
                    {
                        Mensaje = "Nota aclaratoria agregada con exito",
                        TipoImagen = 3
                    };

                    MG.ShowDialog();

                    this.Dispose();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            toolStripButton5.Enabled = true;
            panel1.Visible = true;
            label7.Text = gridZH1.dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
            richTextBox1.Text = "";
        }
    }
}
