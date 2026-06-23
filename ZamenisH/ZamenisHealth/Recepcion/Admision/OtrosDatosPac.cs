using Domain.CXN;
using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ZamenisHealth.Comunes;
using ZamenisHealth.Medicina.Extras;

namespace ZamenisHealth.Recepcion.Admision
{
    public partial class OtrosDatosPac : Forma
    {
        private IPacientes oPacientes;
        private IGenerales oGenerales;

        private MensajesGeneral MG;
        private int PacienteId;

        public OtrosDatosPac(int pacienteId)
        {
            InitializeComponent();
            PacienteId = pacienteId;
            oPacientes = new MPacientes();
            oGenerales = new MGenerales();
        }

        private void OtrosDatosPac_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Datos adicionales del paciente";
            LogoMain.Image = Properties.Resources.Splash;
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";

            ToolStripButton btnGenerar = new ToolStripButton();
            btnGenerar = createToolButton("Grabar");
            MenuLateral.Items.Add(btnGenerar);
            btnGenerar.Click += button1_Click;

            CargarDisapacidades();
            CargarPaciente();
        }
        void CargarDisapacidades()
        {
            List<CXN_DISCAPACIDAD> lista = oGenerales.ListaDiscapacidades();
            if (lista != null)
            {
                foreach (CXN_DISCAPACIDAD i in lista)
                {
                    comboBox1.Items.Add(i.Discapacidad);
                }
            }
        }
        void button1_Click(object sender, EventArgs e) 
        {
            try
            {
                if (comboBox1.Text == "")
                {
                    MG = new MensajesGeneral()
                    {
                        Mensaje = "Debe seleccionar alguna discapacidad",
                        TipoImagen = 1000
                    };
                    MG.ShowDialog();
                    return;
                }

                if (checkBox1.Checked == false)
                {
                    if (string.IsNullOrEmpty(textBox15.Text))
                    {
                        MG = new MensajesGeneral()
                        {
                            Mensaje = "Debe seleccionar alguna ocupacion, si no tiene o no la encuentra, marque la casilla NO APARECE EN LA LISTA",
                            TipoImagen = 1000
                        };
                        MG.ShowDialog();
                        return;
                    }
                }

                CXN_PACIENTES Paciente = new CXN_PACIENTES()
                {
                    Pac_Id = PacienteId,
                    Pac_Acudiente = textBox1.Text,
                    Pac_Parentesco = textBox2.Text,
                    Pac_DireccionAcu = textBox3.Text,
                    Pac_TelefonoAcu = textBox4.Text,
                    Pac_CorreoAcu = textBox5.Text,
                    Discapacidad = comboBox1.Text,
                    Pac_Ocupacion = textBox15.Text,
                };

                oPacientes.Actualiza_Pac3(Paciente);

                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        void CargarPaciente()
        {
            try
            {
                CXN_PACIENTES Pac = oPacientes.LlamarPacientebyId(PacienteId);
                if (Pac != null)
                {
                    textBox1.Text = Pac.Pac_Acudiente;
                    textBox2.Text = Pac.Pac_Parentesco;
                    textBox3.Text = Pac.Pac_DireccionAcu;
                    textBox4.Text = Pac.Pac_TelefonoAcu;
                    textBox5.Text = Pac.Pac_CorreoAcu;
                    textBox15.Text = Pac.Pac_Ocupacion;

                    comboBox1.Text = Pac.Discapacidad;
                }
                else
                {
                    MG = new MensajesGeneral()
                    {
                        Mensaje = "No se logro cargar el paciente, ERROR",
                        TipoImagen = 1000
                    };
                    MG.ShowDialog();

                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void textBox15_DoubleClick(object sender, EventArgs e)
        {
            OcupacionPac ocupacionPac = new OcupacionPac(this);
            ocupacionPac.ShowDialog();
        }
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                textBox15.Text = "";
            }
            else
            {
                textBox15.Text = textBox15.Text;
            }
        }
    }
}
