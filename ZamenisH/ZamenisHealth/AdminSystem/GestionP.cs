using Domain;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Drawing;
using System.Windows.Forms;
using ZamenisHealth.Clases;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.AdminSystem
{
    public partial class GestionP : ConfigForm.BaseForm
    {
        private static readonly IPacientes repoPacs = new MPacientes();

        int PACID;
        public GestionP()
        {
            InitializeComponent();
        }
        
        private void CargarDocumentos()
        {
            var ListaDocs = repoPacs.ListaDocs();
            if (ListaDocs != null)
            {
                comboBox1.Items.Add(ListaDocs);
            }
        }

        private void GestionP_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Gestion de Pacientes";
            
            CargarDocumentos();
        }
        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            panel1.Visible = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            panel1.Visible = false;
            panel2.Visible = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            panel2.Location = new Point(139, 89);
            panel2.Visible = true;
        }

        private void textBox1_DoubleClick(object sender, EventArgs e)
        {
            Comunes.BuscarPacientes buscarPacientes = new Comunes.BuscarPacientes();
            buscarPacientes.Tipo_Busca_Pac = "ArreglosPac";
            buscarPacientes.ShowDialog();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                var Datos = repoPacs.LlamarPacienteDOC(comboBox1.Text, textBox1.Text);
                if (Datos == null)
                {
                    button1.Enabled = false;
                    panel2.Visible = false;
                    PACID = 0;
                    MessageBox.Show("Paciente no existe", "No encontrado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                comboBox1.Enabled = false;
                textBox1.Enabled = false;
                panel2.Visible = false;
                PACID = Convert.ToInt32(Datos.Pac_Id);
                MessageBox.Show(Datos.Pac_PrimerA + " " + Datos.Pac_SegundoA + " " + Datos.Pac_PrimerN + " " + Datos.Pac_SegundoN);
                button1.Enabled = true;

                if (Datos.Pac_Doble != "S")
                {
                    checkBox1.Checked = false;
                }
                else
                {
                    checkBox1.Checked = true;
                }

                if (Datos.Pac_2VXS != "S")
                {
                    checkBox2.Checked = false;
                }
                else
                {
                    checkBox2.Checked = true;
                }

            }
            catch (Exception ex)
            {
                TXTException T = new TXTException
                {
                    FechaHora = DateTime.Now,
                    Error = ex.Message,
                    Formulario = this.Name,
                    Metodo = OverridesExtern.GetCurrentMethodName(),
                    Usuario = Contenedor.UsuarioLogueado
                };

                OverridesExtern.GenerarTXTException(T);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                repoPacs.VariasHeridas(PACID, "S");
            }
            if (checkBox1.Checked == false)
            {
                repoPacs.VariasHeridas(PACID, "N");
            }

            if (checkBox2.Checked == true)
            {
                repoPacs.VariosDias(PACID, "S");
            }
            if (checkBox2.Checked == false)
            {
                repoPacs.VariosDias(PACID, "N");
            }
            this.Dispose();
            this.Close();
        }
    }
}
