using Domain;
using Domain.CXN;
using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.Recepcion.Extras
{
    public partial class CrearCliente : Forma
    {
        private readonly static IPacientes repositorioPacientes = new MPacientes();

        private string TDOC_CLI;
        private string DOC_CLI;

        public CrearCliente(string TDOC, string DOC)
        {
            InitializeComponent();
            this.TDOC_CLI = TDOC;
            this.DOC_CLI = DOC;
        }

        private void CargarDocumentos()
        {
            List<string> ListaDocs = new List<string>();

            
                ListaDocs = repositorioPacientes.ListaDocs();
            

            if (ListaDocs != null)
            {
                foreach (var i in ListaDocs)
                {
                    comboBox1.Items.Add(i);
                }
            }
        }

        private void CrearCliente_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Crear Cliente";
            LogoMain.Image = Properties.Resources.Splash;

            ToolStripButton btnGrabar = new ToolStripButton();
            btnGrabar = createToolButton("Crear Cliente");
            MenuLateral.Items.Add(btnGrabar);
            btnGrabar.Click += button1_Click;

            
            CargarDocumentos();
            textBox6.MaxLength = 10;
            comboBox1.Text = TDOC_CLI;
            textBox5.Text = DOC_CLI;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (textBox1.Text == "") { MessageBox.Show("Debe escribir el primer nombre"); return; }
                if (textBox3.Text == "") { MessageBox.Show("Debe escribir el primer apellido"); return; }
                if (comboBox1.Text == "") { MessageBox.Show("Debe seleccionar el tipo de documento"); return; }
                if (textBox5.Text == "") { MessageBox.Show("Debe escribir el numero de documento"); return; }

                if (textBox7.Text != "")
                {
                    var ConfirmaEmail = repositorioPacientes.ValidaEmail(textBox7.Text);
                    if (ConfirmaEmail != true)
                    {
                        MessageBox.Show("El email no esta bien escrito, si el cliente no posee email deje el campo en blanco",
                                        "No se puede continuar",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Error);
                        return;
                    }
                }

                if (textBox6.Text != "")
                {
                    var ConfirmaCelular = repositorioPacientes.ValidaCelular(textBox6.Text);
                    if (ConfirmaCelular != true)
                    {
                        MessageBox.Show("El celular no esta bien escrito, si el cliente no posee celular deje el campo en blanco",
                                        "No se puede continuar",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Error);
                        return;
                    }
                }

                CXN_PACIENTES crea_Paciente = new CXN_PACIENTES
                {
                    Pac_PrimerN = textBox1.Text,
                    Pac_SegundoN = textBox2.Text,
                    Pac_PrimerA = textBox3.Text,
                    Pac_SegundoA = textBox4.Text,
                    Pac_Telefono = textBox6.Text,
                    Pac_Email = textBox7.Text,
                    Pac_Aseguradora = 99,
                    Pac_TipoId = comboBox1.Text,
                    Pac_IdNum = textBox5.Text.Trim(),
                    Pac_Categoria = "N",
                    Pac_Sexo = "M",
                    IdentidadGenero = "04"
                };
                

                bool _crearcliente = repositorioPacientes.CrearClientes(crea_Paciente);                

                if (_crearcliente != true)
                {
                    MessageBox.Show("No se logro crear el cliente, error inesperado",
                    "Hecho",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
                }
                else
                {
                    MessageBox.Show("Cliente creado con exito, ahora puede hacer la venta",
                       "Hecho",
                       MessageBoxButtons.OK,
                       MessageBoxIcon.Exclamation);

                    Recepcion.Ventas.BuscarPaciente f1 = Application.OpenForms.OfType<Recepcion.Ventas.BuscarPaciente>().SingleOrDefault();
                    f1.Buscar();

                    this.Dispose();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void textBox6_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                if (char.IsSeparator(e.KeyChar))
                {
                    e.Handled = true;
                }
                else
                {
                    e.Handled = false;
                }

                if (!(char.IsNumber(e.KeyChar)) && (e.KeyChar != (char)Keys.Back))
                {
                    e.Handled = true;
                    return;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void textBox5_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)32)
            {
                e.Handled = true;
            }
        }
    }
}
