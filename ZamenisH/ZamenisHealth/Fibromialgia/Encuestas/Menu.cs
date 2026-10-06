using Domain;
using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using Persistence.Fibromialgia.Interfaces;
using Persistence.Fibromialgia.Metodos;
using System;
using System.Data;
using System.Windows.Forms;
using ZamenisHealth.Clases;
using ZamenisHealth.Comunes;
using ZamenisHealth.Fibromialgia.Encuestas.Informe;

namespace ZamenisHealth.Fibromialgia.Encuestas
{
    public partial class Menu : Forma
    {
        private static readonly IPacientes repoPac = new MPacientes();
        private static readonly IEncuestas repoEncuestas = new MEncuestas();
        private static readonly IExport repoExport = new MExport();

        private string DOC, TDOC;
        private int Pos_Selected;
        private ToolStripButton button3;
        private ToolStripButton button4;
        private ToolStripButton button5;
        private ToolStripButton button6;

        private MensajesGeneral MG;

        public Menu()
        {
            InitializeComponent();
        }

        private void Menu_Load(object sender, EventArgs e)
        {
            try
            {
                LogoMain.Image = Properties.Resources.Splash;
                SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";
                Titulo.Text = "Cuestionarios Fibromialgia";

                gridZH1.CeldaHeight = true;
                Encabezados();
                gridZH1.dataGridView1.CellDoubleClick += DataGridView1_CellDoubleClick;

                button3 = new ToolStripButton();
                button3 = createToolButton("Crear FIQ y SF36");
                MenuLateral.Items.Add(button3);
                button3.Click += button3_Click;

                button4 = new ToolStripButton();
                button4 = createToolButton("Crear WPI y IGS");
                MenuLateral.Items.Add(button4);
                button4.Click += button4_Click;

                button5 = new ToolStripButton();
                button5 = createToolButton("Encuesta Satisfaccion");
                MenuLateral.Items.Add(button5);
                button5.Click += button5_Click;

                button6 = new ToolStripButton();
                button6 = createToolButton("Informe Encuestas");
                MenuLateral.Items.Add(button6);
                button6.Click += button6_Click;

                button3.Enabled = false;
                button4.Enabled = false;
                button5.Enabled = false;
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        public void setDoc(string _tdoc, string _doc)
        {
            this.TDOC = _tdoc;
            this.DOC = _doc;

            textBox1.Text = this.DOC;
        }

        DataTable dt = new DataTable();
        DataColumn Codigo;
        DataColumn Fecha;
        DataColumn Paciente;
        DataColumn Encuesta;
        DataColumn Realiza;
        DataColumn Estado;

        void Encabezados()
        {
            gridZH1.dataGridView1.DataSource = null;
            dt = new DataTable();
            Codigo = dt.Columns.Add("Codigo", typeof(string));
            Fecha = dt.Columns.Add("Fecha", typeof(string));            
            Paciente = dt.Columns.Add("Paciente", typeof(string));
            Encuesta = dt.Columns.Add("Encuesta", typeof(string));
            Realiza = dt.Columns.Add("Realiza", typeof(string));
            Estado = dt.Columns.Add("Estado", typeof(string));
        }
        void getEncuesta1XPaciente()
        {
            try
            {
                var getE1XPac = repoEncuestas.getPreviosXPacEncuesta1(textBox1.Text);
                if (getE1XPac != null)
                {
                    Encabezados();

                    foreach (var i in getE1XPac)
                    {
                        DataRow row = dt.NewRow();

                        row[Codigo] = i.Id.ToString();
                        row[Fecha] = Convert.ToDateTime(i.FechaEncuesta).ToString(Conexion.ConectionDictionary["Format_Fecha"]);
                        row[Paciente] = i.UsuarioCambia.ToString();
                        row[Encuesta] = "1";
                        row[Realiza] = i.UsuarioRegistra.ToString();
                        row[Estado] = i.Estado.ToString() == "V" ? "Vigente" : "Anulada";

                        dt.Rows.Add(row);
                        dt.AcceptChanges();
                    }

                    gridZH1.dataGridView1.DataSource = dt;
                }
                else
                {
                    Encabezados();
                }

                button3.Enabled = true;
                button4.Enabled = true;
                button5.Enabled = true;
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        void getEncuesta2XPaciente()
        {
            try
            {
                var getE2XPac = repoEncuestas.getPreviosXPacEncuesta2(textBox1.Text);
                if (getE2XPac != null)
                {
                    Encabezados();

                    foreach (var i in getE2XPac)
                    {
                        DataRow row = dt.NewRow();

                        row[Codigo] = i.Id.ToString();
                        row[Fecha] = Convert.ToDateTime(i.FechaEncuesta).ToString(Conexion.ConectionDictionary["Format_Fecha"]);
                        row[Paciente] = i.UsuarioCambia.ToString();
                        row[Encuesta] = "2";
                        row[Realiza] = i.UsuarioRegistra.ToString();
                        row[Estado] = i.Estado.ToString() == "V" ? "Vigente" : "Anulada";

                        dt.Rows.Add(row);
                        dt.AcceptChanges();
                    }

                    gridZH1.dataGridView1.DataSource = dt;
                }
                else
                {
                    Encabezados();
                }

                button3.Enabled = true;
                button4.Enabled = true;
                button5.Enabled = true;
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        void getEncuesta3XPaciente()
        {
            try
            {
                var getE3XPac = repoEncuestas.getPreviosXPacEncuesta3(textBox1.Text);
                if (getE3XPac != null)
                {
                    Encabezados();

                    foreach (var i in getE3XPac)
                    {
                        DataRow row = dt.NewRow();

                        row[Codigo] = i.Id.ToString();
                        row[Fecha] = Convert.ToDateTime(i.FechaEncuesta).ToString(Conexion.ConectionDictionary["Format_Fecha"]);
                        row[Paciente] = i.UsuarioCambia.ToString();
                        row[Encuesta] = "3";
                        row[Realiza] = i.UsuarioRegistra.ToString();
                        row[Estado] = i.Estado.ToString() == "V" ? "Vigente" : "Anulada";

                        dt.Rows.Add(row);
                        dt.AcceptChanges();
                    }

                    gridZH1.dataGridView1.DataSource = dt;
                }
                else
                {
                    Encabezados();
                }

                button3.Enabled = true;
                button4.Enabled = true;
                button5.Enabled = true;
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            Busquedas();
        }
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            Busquedas();
        }
        void Busquedas()
        {
            try
            {
                MensajesGeneral MG = new MensajesGeneral();

                if (string.IsNullOrEmpty(textBox1.Text))
                {
                    button3.Enabled = false;
                    button4.Enabled = false;
                    button5.Enabled = false;

                    MG.TipoImagen = 1000;
                    MG.Mensaje = "Debe buscar un paciente";
                    MG.ShowDialog();
                    return;
                }

                switch (comboBox2.SelectedIndex)
                {
                    case 1:
                        getEncuesta1XPaciente();
                        break;

                    case 2:
                        getEncuesta2XPaciente();
                        break;

                    case 3:
                        getEncuesta3XPaciente();
                        break;

                    default:
                        button3.Enabled = false;
                        button4.Enabled = false;
                        button5.Enabled = false;

                        MG.TipoImagen = 1000;
                        MG.Mensaje = "Seleccione una encuesta";
                        MG.ShowDialog();
                        break;
                }

            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        void Encuesta1General()
        {
            var getE1General = repoEncuestas.getPreviosGeneralEncuesta1();
            if (getE1General != null)
            {
                Encabezados();

                foreach (var i in getE1General)
                {
                    DataRow row = dt.NewRow();

                    row[Codigo] = i.Id.ToString();
                    row[Fecha] = Convert.ToDateTime(i.FechaEncuesta).ToString(Conexion.ConectionDictionary["Format_Fecha"]);
                    row[Paciente] = i.UsuarioCambia.ToString();
                    row[Encuesta] = "1";
                    row[Realiza] = i.UsuarioRegistra.ToString();
                    row[Estado] = i.Estado.ToString();

                    dt.Rows.Add(row);
                    dt.AcceptChanges();
                }

                gridZH1.dataGridView1.DataSource = dt;
            }
            else
            {
                Encabezados();
            }
        }
        void Encuesta2General()
        {
            var getE2General = repoEncuestas.getPreviosGeneralEncuesta2();
            if (getE2General != null)
            {
                Encabezados();

                foreach (var i in getE2General)
                {
                    DataRow row = dt.NewRow();

                    row[Codigo] = i.Id.ToString();
                    row[Fecha] = Convert.ToDateTime(i.FechaEncuesta).ToString(Conexion.ConectionDictionary["Format_Fecha"]);
                    row[Paciente] = i.UsuarioCambia.ToString();
                    row[Encuesta] = "2";
                    row[Realiza] = i.UsuarioRegistra.ToString();
                    row[Estado] = i.Estado.ToString();

                    dt.Rows.Add(row);
                    dt.AcceptChanges();
                }

                gridZH1.dataGridView1.DataSource = dt;
            }
            else
            {
                Encabezados();
            }
        }
        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                MensajesGeneral MG = new MensajesGeneral();

                switch (comboBox2.SelectedIndex)
                {
                    case 1:
                        Encuesta1General();
                        break;

                    case 2:
                        Encuesta2General();
                        break;

                    case 3:
                        Encuesta3General();
                        break;

                    default:
                        MG.TipoImagen = 1000;
                        MG.Mensaje = "Seleccione una encuesta";
                        MG.ShowDialog();
                        break;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void DataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                switch (gridZH1.dataGridView1.Rows[e.RowIndex].Cells[3].Value.ToString())
                {
                    case "1":
                        var E1 = repoExport.ExportarEncuesta1(Convert.ToInt32(gridZH1.dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString()));
                        if (E1 != null)
                        {
                            ConfigForm.GenerarReportViewer("DataSetEncuesta1", "ZamenisHealth.Reportes.Fibro.Encuesta1.rdlc", E1);
                            return;
                        }

                        break;

                    case "2":
                        var E2 = repoExport.ExportarEncuesta2(Convert.ToInt32(gridZH1.dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString()));
                        if (E2 != null)
                        {
                            ConfigForm.GenerarReportViewer("DataSetEncuesta2", "ZamenisHealth.Reportes.Fibro.Encuesta2.rdlc", E2);
                            return;
                        }

                        break;

                    case "3":
                        var E3 = repoExport.ExportarEncuesta3(Convert.ToInt32(gridZH1.dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString()));
                        if (E3 != null)
                        {
                            ConfigForm.GenerarReportViewer("DataSetEncuesta3", "ZamenisHealth.Reportes.Fibro.Encuesta3.rdlc", E3);
                            return;
                        }

                        break;

                    default:
                        MG.TipoImagen = 1000;
                        MG.Mensaje = "Informe con inconvenientes";
                        MG.ShowDialog();
                        break;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            MensajesGeneral MG = new MensajesGeneral();

            int val = searchPac();
            if (val <= 0)
            {
                MG.TipoImagen = 1000;
                MG.Mensaje = "El documento digitado no existe";
                MG.ShowDialog();
                return;
            }

            Encuesta1 encuesta1 = new Encuesta1(val);
            encuesta1.ShowDialog();
        }
        int searchPac()
        {
            try
            {
                var getPac = repoPac.LlamarPacienteNumDoc(textBox1.Text);
                if (getPac != null)
                {
                    return getPac.Pac_Id;
                }

                return 0;
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
                return 0;
            }
        }
        private void button4_Click(object sender, EventArgs e)
        {
            int val = searchPac();
            if (val <= 0)
            {
                MG.TipoImagen = 1000;
                MG.Mensaje = "El documento digitado no existe";
                MG.ShowDialog();
                return;
            }

            Encuesta2 E = new Encuesta2(val);
            E.ShowDialog();
        }
        private void button5_Click(object sender, EventArgs e)
        {
            int val = searchPac();
            if (val <= 0)
            {
                MG.TipoImagen = 1000;
                MG.Mensaje = "El documento digitado no existe";
                MG.ShowDialog();
                return;
            }

            Encuesta3 E = new Encuesta3(val);
            E.ShowDialog();
        }
        private void button6_Click(object sender, EventArgs e)
        {
            InformesEncuestas I = new InformesEncuestas();
            I.ShowDialog();
        }
        private void textBox1_DoubleClick(object sender, EventArgs e)
        {
            BuscarPacientes P = new BuscarPacientes("Encuesta");
            P.ShowDialog();
        }
        void Encuesta3General()
        {
            var getE3General = repoEncuestas.getPreviosGeneralEncuesta3();
            if (getE3General != null)
            {
                Encabezados();

                foreach (var i in getE3General)
                {
                    DataRow row = dt.NewRow();

                    row[Codigo] = i.Id.ToString();
                    row[Fecha] = Convert.ToDateTime(i.FechaEncuesta).ToString(Conexion.ConectionDictionary["Format_Fecha"]);
                    row[Paciente] = i.UsuarioCambia.ToString();
                    row[Encuesta] = "3";
                    row[Realiza] = i.UsuarioRegistra.ToString();
                    row[Estado] = i.Estado.ToString();

                    dt.Rows.Add(row);
                    dt.AcceptChanges();
                }

                gridZH1.dataGridView1.DataSource = dt;
            }
            else
            {
                Encabezados();
            }
        }
    }
}
