using Domain.CXN;

using FormAndControls;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using ZamenisHealth.Comunes;
using ZamenisHealth.Facturacion.Extras;

namespace ZamenisHealth.HistoriasClinicas.NotaEnfermeria
{
    public partial class MedidasCuracion : Forma
    {
        private CXN_NOTASMED listTemp;
        private MensajesGeneral MG;
        private List<CXN_NOTASMED> MedidasTotal;
        private int PosisionLista;

        public MedidasCuracion(List<CXN_NOTASMED> medidas, int posisionLista)
        {
            InitializeComponent();
            MedidasTotal = medidas;
            PosisionLista = posisionLista;
        }

        private void MedidasCuracion_Load(object sender, EventArgs e)
        {
            ImageClose.Visible = false;
            ImageMinimize.Visible = false;

            Titulo.Text = "Detalles de Herida";
            LogoMain.Image = Properties.Resources.Splash;

            ToolStripButton btnGrabar = new ToolStripButton();
            btnGrabar = createToolButton("Agregar Datos");
            MenuLateral.Items.Add(btnGrabar);
            btnGrabar.Click += toolStripButton11_Click;

            textBox5.KeyPress += NumberDecimal;
            textBox6.KeyPress += NumberDecimal;
            textBox7.KeyPress += NumberDecimal;
            textBox8.KeyPress += NumberDecimal;
            textBox9.KeyPress += NumberDecimal;
            textBox10.KeyPress += NumberDecimal;

            CargarMedida();
        }
        private void textBox10_TextChanged(object sender, EventArgs e)
        {
            if (textBox10.Text != "0")
            {
                textBox11.Visible = true;
                label13.Visible = true;
            }
            else
            {
                textBox11.Visible = false;
                label13.Visible = false;
            }
        }
        void CargarMedida()
        {
            try
            {
                CXN_NOTASMED Listado = MedidasTotal.Find(x => x.Id == PosisionLista);
                if (Listado != null)
                {
                    comboBox2.Text = Listado.Evolucion;
                    textBox13.Text = Listado.RazonNoEvolucion;
                    textBox10.Text = Listado.txtOtros;
                    textBox11.Text = Listado.RazonOtros;
                    textBox6.Text = Listado.txtFibrina;
                    textBox7.Text = Listado.txtNecroticoHumedo;
                    textBox8.Text = Listado.txtNecroticoSeco;
                    textBox9.Text = Listado.txtEpitelizacion;
                    textBox5.Text = Listado.txtGranulacion;
                    richTextBox1.Text = Listado.NovedadHerida;
                    comboBox3.Text = Listado.selExudado;
                    textBox4.Text = Listado.RazonExudado;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void toolStripButton11_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(textBox5.Text.Trim()) || string.IsNullOrEmpty(textBox6.Text.Trim()) || string.IsNullOrEmpty(textBox7.Text.Trim()) ||
                    string.IsNullOrEmpty(textBox8.Text.Trim()) || string.IsNullOrEmpty(textBox9.Text.Trim()) || string.IsNullOrEmpty(textBox10.Text.Trim()))
                {
                    MG = new MensajesGeneral();
                    MG.Mensaje = "Los campos de porcentaje son obligatorios.";
                    MG.TipoImagen = 1000;
                    MG.ShowDialog();
                }
                else if (textBox10.Text != "0" && string.IsNullOrEmpty(textBox11.Text.Trim()))
                {
                    MG = new MensajesGeneral();
                    MG.Mensaje = "El campo de Observaciones es obligatorio si el porcentaje de otro tejido es mayor a 0%.";
                    MG.TipoImagen = 1000;
                    MG.ShowDialog();
                }
                else if (comboBox2.Text == "Sin Evolucion" && string.IsNullOrEmpty(textBox13.Text.Trim()))
                {
                    MG = new MensajesGeneral();
                    MG.Mensaje = "El campo de Observaciones es obligatorio si no hay evolucion de la herida.";
                    MG.TipoImagen = 1000;
                    MG.ShowDialog();
                }
                else if (comboBox3.Text == "SI" && string.IsNullOrEmpty(textBox4.Text.Trim()))
                {
                    MG = new MensajesGeneral();
                    MG.Mensaje = "Si hay tunelizacion describa el grado orientacion de la herida en sentido de las manecillas del reloj";
                    MG.TipoImagen = 1000;
                    MG.ShowDialog();
                }
                else
                {
                    listTemp = new CXN_NOTASMED
                    {
                        Id = PosisionLista,
                        Evolucion = comboBox2.Text == "Sin Evolucion" ? "Sin Evolucion: " + textBox13.Text.ToUpper() : comboBox2.Text,
                        txtOtros = textBox10.Text + " -> " + textBox11.Text.ToUpper(),
                        txtFibrina = textBox6.Text,
                        txtNecroticoHumedo = textBox7.Text,
                        txtNecroticoSeco = textBox8.Text,
                        txtEpitelizacion = textBox9.Text,
                        txtGranulacion = textBox5.Text,
                        NovedadHerida = richTextBox1.Text,
                        selExudado = comboBox3.Text + " - " + textBox4.Text.Trim().ToUpper(),
                    };
                   
                    CXN_NOTASMED retorno = MedidasTotal.Find(x => x.Id == listTemp.Id);
                    retorno.Evolucion = comboBox2.Text;
                    retorno.RazonNoEvolucion = textBox13.Text.ToUpper();
                    retorno.txtOtros = textBox10.Text;
                    retorno.RazonOtros = textBox11.Text;
                    retorno.txtFibrina = textBox6.Text;
                    retorno.txtNecroticoHumedo = textBox7.Text;
                    retorno.txtNecroticoSeco = textBox8.Text;
                    retorno.txtEpitelizacion = textBox9.Text;
                    retorno.txtGranulacion = textBox5.Text;
                    retorno.NovedadHerida = richTextBox1.Text;
                    retorno.selExudado = comboBox3.Text;
                    retorno.RazonExudado = textBox4.Text.Trim().ToUpper();

                    NotaCuracion f1 = Application.OpenForms.OfType<NotaCuracion>().LastOrDefault();
                    f1.setDetalles(MedidasTotal);

                    this.Dispose();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Zamenis Health", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox2.Text == "En Evolucion" || comboBox2.Text == "Paciente Nuevo")
            {
                textBox13.Visible = false;
                label17.Visible = false;
            }
            else
            {
                textBox13.Visible = true;
                label17.Visible = true;
            }
        }
        void NumberDecimal(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true; 
            }

            if (e.KeyChar == '.' && ((TextBox)sender).Text.Contains('.'))
            {
                e.Handled = true; 
            }
        }
        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox3.Text == "SI")
            {
                label20.Visible = true;
                textBox4.Visible = true;
            }
            else
            {
                label20.Visible = false;
                textBox4.Visible = false;
                textBox4.Text = "";
            }
        }
    }
}
