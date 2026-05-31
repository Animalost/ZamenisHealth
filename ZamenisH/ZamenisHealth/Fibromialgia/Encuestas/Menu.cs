using System;
using System.Drawing;
using System.Windows.Forms;
using Domain;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using Persistence.Fibromialgia.Interfaces;
using Persistence.Fibromialgia.Metodos;
using ZamenisHealth.Clases;
using ZamenisHealth.Comunes;
using ZamenisHealth.Fibromialgia.Encuestas.Informe;

namespace ZamenisHealth.Fibromialgia.Encuestas
{
    public partial class Menu : ConfigForm.BaseForm
    {
        private static readonly IPacientes repoPac = new MPacientes();
        private static readonly IEncuestas repoEncuestas = new MEncuestas();
        private static readonly IExport repoExport = new MExport();

        private string DOC, TDOC;
        private int Pos_Selected;

        public Menu()
        {
            InitializeComponent();
        }

        private void Menu_Load(object sender, EventArgs e)
        {
            try
            {
                this.Titulo.Text = "Cuestionarios Fibromialgia";

                var getDocs = repoPac.ListaDocs();
                if (getDocs != null)
                {
                    foreach (var i in getDocs)
                    {
                        comboBox1.Items.Add(i);
                    }
                }

                Encabezados();
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

            comboBox1.Text = this.TDOC;
            textBox1.Text = this.DOC;
        }
        void Encabezados()
        {
            listView1.Clear();
            listView1.View = View.Details;
            listView1.GridLines = true;
            listView1.FullRowSelect = true;
            listView1.Columns.Add("Codigo", 80, HorizontalAlignment.Left);
            listView1.Columns.Add("Fecha", 80, HorizontalAlignment.Left);
            listView1.Columns.Add("Paciente", 250, HorizontalAlignment.Left);
            listView1.Columns.Add("Encuesta", 0, HorizontalAlignment.Left);
            listView1.Columns.Add("Realiza", 80, HorizontalAlignment.Left);
            listView1.Columns.Add("Estado", 80, HorizontalAlignment.Left);
        }

        void getEncuesta1XPaciente()
        {
            try
            {
                var getE1XPac = repoEncuestas.getPreviosXPacEncuesta1(comboBox1.Text, textBox1.Text);
                if (getE1XPac != null)
                {
                    Encabezados();

                    foreach (var i in getE1XPac)
                    {
                        listView1.Items.Add(new ListViewItem(new string[]
                        {
                            i.Id.ToString(),
                            Convert.ToDateTime(i.FechaEncuesta).ToString(Conexion.ConectionDictionary["Format_Fecha"]),
                            i.UsuarioCambia.ToString(),
                            "1",
                            i.UsuarioRegistra.ToString(),
                            i.Estado.ToString()
                        }));
                    }
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
                var getE2XPac = repoEncuestas.getPreviosXPacEncuesta2(comboBox1.Text, textBox1.Text);
                if (getE2XPac != null)
                {
                    Encabezados();

                    foreach (var i in getE2XPac)
                    {
                        listView1.Items.Add(new ListViewItem(new string[]
                        {
                            i.Id.ToString(),
                            Convert.ToDateTime(i.FechaEncuesta).ToString(Conexion.ConectionDictionary["Format_Fecha"]),
                            i.UsuarioCambia.ToString(),
                            "2",
                            i.UsuarioRegistra.ToString(),
                            i.Estado.ToString()
                        }));
                    }
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
                var getE3XPac = repoEncuestas.getPreviosXPacEncuesta3(comboBox1.Text, textBox1.Text);
                if (getE3XPac != null)
                {
                    Encabezados();

                    foreach (var i in getE3XPac)
                    {
                        listView1.Items.Add(new ListViewItem(new string[]
                        {
                            i.Id.ToString(),
                            Convert.ToDateTime(i.FechaEncuesta).ToString(Conexion.ConectionDictionary["Format_Fecha"]),
                            i.UsuarioCambia.ToString(),
                            "3",
                            i.UsuarioRegistra.ToString(),
                            i.Estado.ToString()
                        }));
                    }
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

                if (comboBox1.Text == "" || textBox1.Text == "")
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
                    listView1.Items.Add(new ListViewItem(new string[]
                    {
                            i.Id.ToString(),
                            Convert.ToDateTime(i.FechaEncuesta).ToString(Conexion.ConectionDictionary["Format_Fecha"]),
                            i.UsuarioCambia.ToString(),
                            "1",
                            i.UsuarioRegistra.ToString(),
                            i.Estado.ToString()
                    }));
                }
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
                    listView1.Items.Add(new ListViewItem(new string[]
                    {
                            i.Id.ToString(),
                            Convert.ToDateTime(i.FechaEncuesta).ToString(Conexion.ConectionDictionary["Format_Fecha"]),
                            i.UsuarioCambia.ToString(),
                            "2",
                            i.UsuarioRegistra.ToString(),
                            i.Estado.ToString()
                    }));
                }
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

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                MensajesGeneral MG = new MensajesGeneral();

                switch (listView1.SelectedItems[0].SubItems[3].Text)
                {
                    case "1":
                        var E1 = repoExport.ExportarEncuesta1(Convert.ToInt32(listView1.SelectedItems[0].SubItems[0].Text));
                        if (E1 != null)
                        {
                            ConfigForm.GenerarReportViewer("DataSetEncuesta1",
               "ZamenisHealth.Reportes.Fibro.Encuesta1.rdlc",
               E1);

                            return;
                        }

                        break;

                    case "2":
                        var E2 = repoExport.ExportarEncuesta2(Convert.ToInt32(listView1.SelectedItems[0].SubItems[0].Text));
                        if (E2 != null)
                        {
                            ConfigForm.GenerarReportViewer("DataSetEncuesta2",
               "ZamenisHealth.Reportes.Fibro.Encuesta2.rdlc",
               E2);
                            return;
                        }

                        break;

                    case "3":
                        var E3 = repoExport.ExportarEncuesta3(Convert.ToInt32(listView1.SelectedItems[0].SubItems[0].Text));
                        if (E3 != null)
                        {
                            ConfigForm.GenerarReportViewer("DataSetEncuesta3",
               "ZamenisHealth.Reportes.Fibro.Encuesta3.rdlc",
               E3);
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

        private void listView1_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Right)
                {
                    contextMenuStrip1.Visible = true;
                    contextMenuStrip1.Location = new Point(listView1.Location.X + 600, listView1.Location.Y + 100);

                    Pos_Selected = Convert.ToInt32(listView1.SelectedItems[0].SubItems[0].Text);

                    foreach (ListViewItem lvw in listView1.Items)
                    {
                        lvw.Selected = false;
                    }
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void excluirEncuestaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                MensajesGeneral MG = new MensajesGeneral();

                switch (comboBox2.SelectedIndex)
                {
                    case 1:
                        bool ex1 = repoEncuestas.ExcluirEncuesta(Pos_Selected, "Encuesta1", Contenedor.UsuarioLogueado);
                        if (ex1 != true)
                        {
                            MG.TipoImagen = 1000;
                            MG.Mensaje = "No se logro excluir esta encuesta";
                            MG.ShowDialog();
                        }
                        break;

                    case 2:
                        bool ex2 = repoEncuestas.ExcluirEncuesta(Pos_Selected, "Encuesta2", Contenedor.UsuarioLogueado);
                        if (ex2 != true)
                        {
                            MG.TipoImagen = 1000;
                            MG.Mensaje = "No se logro excluir esta encuesta";
                            MG.ShowDialog();
                        }
                        break;

                    case 3:
                        bool ex3 = repoEncuestas.ExcluirEncuesta(Pos_Selected, "Encuesta3", Contenedor.UsuarioLogueado);
                        if (ex3 != true)
                        {
                            MG.TipoImagen = 1000;
                            MG.Mensaje = "No se logro excluir esta encuesta";
                            MG.ShowDialog();
                        }
                        break;

                    default:
                        MG.TipoImagen = 1000;
                        MG.Mensaje = "Seleccione un tipo de encuesta para exluirla";
                        MG.ShowDialog();
                        return;
                }

                MG.TipoImagen = 3;
                MG.Mensaje = "Encuesta excluida correctamente, para volver a habilitar una encuesta despues de excluirla debe anunciarlo en la administracion";
                MG.ShowDialog();

                radioButton2.Checked = false;
                radioButton1.Checked = true;

                Busquedas();
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void label1_MouseDown(object sender, MouseEventArgs e)
        {

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
                var getPac = repoPac.LlamarPacienteDOC(comboBox1.Text, textBox1.Text);
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
            MensajesGeneral MG = new MensajesGeneral();

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
            MensajesGeneral MG = new MensajesGeneral();

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
                    listView1.Items.Add(new ListViewItem(new string[]
                    {
                            i.Id.ToString(),
                            Convert.ToDateTime(i.FechaEncuesta).ToString(Conexion.ConectionDictionary["Format_Fecha"]),
                            i.UsuarioCambia.ToString(),
                            "3",
                            i.UsuarioRegistra.ToString(),
                            i.Estado.ToString()
                    }));
                }
            }
            else
            {
                Encabezados();
            }
        }
    }
}
