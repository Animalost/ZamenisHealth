using Domain;

using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;

using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

using ZamenisHealth.Clases;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.HistoriasClinicas
{
    public partial class NotasAclaratorias : ConfigForm.BaseForm
    {
        private static readonly INotasAclaratorias repoNotasAclaratorias = new MNotasAclaratorias();
        private static readonly IPacientes repoPacientes = new MPacientes();
        private static readonly ICManejo repoManejo = new MCManenejo();
        private static readonly ILogin repositorioLogin = new MLogin();

        public string TipoNota;

        DataTable dt;
        DataColumn POS;
        DataColumn Admision;
        DataColumn Fecha;
        DataColumn Profesional;

        public NotasAclaratorias()
        {
            InitializeComponent();
            
            ConfigForm.MoverForma(label29, this);
            ConfigForm.MoverForma(panel3, this);
        }
        void Encabezados()
        {
            dataGridView1.DataSource = null;
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

        private void NotasAclaratorias_Load(object sender, EventArgs e)
        {
            Titulo.Visible = false;
            ImageClose.Visible = false;

            Comunes.MensajesGeneral MG = new MensajesGeneral();
            
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
                    MG.Mensaje = "Su usuario no tiene permiso para realizar notas aclaratorias";
                    MG.TipoImagen = 1000;
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

                    case "Radiologia":
                        NotasMedicas(TipoNota);
                        break;

                    default:
                        MessageBox.Show("Hubo un error inesperado, cierre esta pantalla y vuelva a ingresar",
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
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

                    Estilos(dataGridView1, dt);
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
            D.EnableHeadersVisualStyles = false;
            D.ScrollBars = ScrollBars.Both;

            D.DataSource = t;

            D.Columns["Admision"].Width = 120;
            D.Columns["Fecha"].Width = 120;
            D.Columns["Profesional"].Width = 500;

            D.ColumnHeadersDefaultCellStyle.Font = new Font(D.Font, FontStyle.Bold);
            D.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 8, FontStyle.Bold);
            D.ColumnHeadersDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#bfdbff");
            D.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;
            D.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;

            D.Columns["Admision"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            D.Columns["Fecha"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
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
                        MessageBox.Show("No se logro agrgar la nota aclaratoria a la historia clinica",
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        return;
                    }
                    MessageBox.Show("Nota aclaratoria agregada con exito",
                        "Hecho",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation);
                    this.Dispose();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Dispose();
            this.Close();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            toolStripButton5.Enabled = true;
            panel1.Visible = true;
            label7.Text = dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
            richTextBox1.Text = "";
        }
    }
}
