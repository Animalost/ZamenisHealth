using Domain.CXN;
using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ZamenisHealth.Comunes;
using ZamenisHealth.Recepcion.Extras;

namespace ZamenisHealth.Recepcion.Ventas
{
    public partial class BuscarPaciente : Forma
    {
        private IVender oController;

        private MensajesGeneral MG;
        private int IdPac, IdCia;

        public BuscarPaciente()
        {
            InitializeComponent();
            oController = new MVender();
            SoloNumeros(textBox2);
        }

        private void BuscarPaciente_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Busqueda de Cliente";
            LogoMain.Image = Properties.Resources.Splash;
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";

            ToolStripButton btnBuscar = new ToolStripButton();
            btnBuscar = createToolButton("Buscar");
            MenuLateral.Items.Add(btnBuscar);
            btnBuscar.Click += btnBuscar_Click;

            textBox2.MaxLength = 10;
            CargarCia();
        }

        void CargarCia()
        {
            List<CXN_CIA> lista = oController.getAllCompañias();
            if (lista != null)
            {
                foreach (CXN_CIA c in lista)
                {
                    comboBox1.Items.Add(c.Com_Nombre);
                }

                comboBox1.SelectedIndex = 0;
            }
        }

        void btnBuscar_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(textBox1.Text)) 
                {
                    MG = new MensajesGeneral()
                    {
                        Mensaje = "Debe diligenciar un documento valido",
                        TipoImagen = 1000
                    };

                    MG.ShowDialog();
                }
                else
                {
                    Buscar();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void Buscar()
        {
            CXN_PACIENTES P = oController.LlamarPacienteNumDoc(textBox1.Text.Trim());
            if (P != null)
            {
                textBox4.Text = P.Pac_PrimerA + " " +
                    P.Pac_SegundoA + " " +
                    P.Pac_PrimerN + " " +
                    P.Pac_SegundoN;
                textBox2.Text = P.Pac_Telefono;
                textBox3.Text = P.Pac_Email;

                IdPac = P.Pac_Id;
                boton1.Enabled = true;
            }
            else
            {
                boton1.Enabled = false;
                IdPac = 0;

                MG = new MensajesGeneral()
                {
                    Mensaje = "El numero de documento no existe, debe crear el cliente",
                    TipoImagen = 0
                };

                MG.ShowDialog();

                CrearCliente C = new CrearCliente("Cedula de Ciudadania", textBox1.Text);
                C.ShowDialog();
            }
        }

        private void boton1_Click(object sender, EventArgs e)
        {
            try
            {
                if (IdPac == 0)
                {
                    MG = new MensajesGeneral()
                    {
                        Mensaje = "El cliente no existe, haga click en Buscar para encontrarlo antes de continuar",
                        TipoImagen = 0
                    };

                    MG.ShowDialog();
                }
                else if (textBox2.TextLength != 10)
                {
                    MG = new MensajesGeneral()
                    {
                        Mensaje = "El numero de celular es invalido",
                        TipoImagen = 0
                    };

                    MG.ShowDialog();
                }
                else
                {
                    if (ValidaEmail(textBox3.Text) == false)
                    {
                        DialogResult result = MessageBox.Show("El formato del email es invalido, ¿seguro que desea continuar? " +
                            "Lo mas recomendable es digitar un correo valido para efectos de facturacion electronica",
                                                      "Zamenis Health - ATENCION!!!",
                                                      MessageBoxButtons.YesNo,
                                                      MessageBoxIcon.Question);

                        if (result == DialogResult.Yes)
                        {
                            CXN_PACIENTES P = new CXN_PACIENTES()
                            {
                                Pac_Telefono = textBox2.Text,
                                Pac_Email = textBox3.Text,
                                Pac_Id = IdPac
                            };

                            oController.ActualizarCliente(P);

                            Vender vender = new Vender(IdPac, IdCia);
                            vender.ShowDialog();

                            this.Close();
                        }
                    }
                    else
                    {
                        CXN_PACIENTES P = new CXN_PACIENTES()
                        {
                            Pac_Telefono = textBox2.Text,
                            Pac_Email = textBox3.Text,
                            Pac_Id = IdPac
                        };

                        oController.ActualizarCliente(P);

                        Vender vender = new Vender(IdPac, IdCia);
                        vender.ShowDialog();

                        this.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }            
        }

        private void textBox1_DoubleClick(object sender, EventArgs e)
        {
            BuscarPacientes buscarPacientes = new BuscarPacientes("BusVen");
            buscarPacientes.ShowDialog();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            IdCia = oController.getPrestadorbyName(comboBox1.Text).Com_Identificador;
        }
    }
}
