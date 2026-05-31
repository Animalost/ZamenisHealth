using Domain;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using ZamenisHealth.Facturacion;

namespace ZamenisHealth.Comunes.ConfigContenedor
{
    public partial class DatosPac : Form
    {
        private static readonly IPacientes repositorioPacientes = new MPacientes();
        private static readonly IAseguradoras repositorioAseguradora = new MAseguradoras();

        private int PacID;

        public DatosPac()
        {
            InitializeComponent();

            btnZamenis1.ButtonClick += btnZamenis1_ButtonClick;
            btnZamenis2.ButtonClick += btnZamenis2_ButtonClick;
            btnZamenis3.ButtonClick += btnZamenis3_ButtonClick;

            btnZamenis1.captionBtn = "Ver Datos";
            btnZamenis1.tooltipBtn = "Buscar los datos del paciente digitado";

            btnZamenis2.captionBtn = "Cerrar";
            btnZamenis2.tooltipBtn = "Cierra esta ventana";

            btnZamenis3.captionBtn = "Limpiar";
            btnZamenis3.tooltipBtn = "Limpiar Busqueda";
        }
        private void btnZamenis1_ButtonClick(object sender, EventArgs e)
        {
            try
            {
                if (textBox1.Text == "")
                {
                    MensajesGeneral MG = new MensajesGeneral();
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "Debe diligenciar un tipo de documento y un numero de documento";
                    MG.ShowDialog();
                    return;
                }

                var ResDatos = repositorioPacientes.LlamarPacienteDOC(comboBox1.Text, textBox1.Text);
                if (ResDatos != null)
                {
                    string Sexi;
                    switch (ResDatos.Pac_Sexo)
                    {
                        case "M":
                            Sexi = "Masculino";
                            break;

                        case "F":
                            Sexi = "Femenino";
                            break;

                        default:
                            Sexi = "Sin Definir";
                            break;
                    }

                    var cargareg = repositorioPacientes.Carga_Regimen(ResDatos.Pac_Regimen);
                    var Asegura = repositorioAseguradora.getInfoFromAsebyCode(ResDatos.Pac_Aseguradora);

                    richTextBox1.Text = "";
                    string Dato_PAC = "DATOS DEL PACIENTE" + "\n\r" +
                                      "Paciente: " + ResDatos.Pac_PrimerA + " " + ResDatos.Pac_SegundoA + " " + ResDatos.Pac_PrimerN + " " + ResDatos.Pac_SegundoN + "\n\r" +
                                      "Identificacion: " + ResDatos.Pac_TipoId + " " + ResDatos.Pac_IdNum + "\n\r" +
                                      "Telefonos: " + ResDatos.Pac_Telefono + " y " + ResDatos.Pac_TelefonoAux + "\n\r" +
                                      "Aseguradora: " + ResDatos.Pac_Aseguradora + " - " + Asegura.Ase_Descripcion + "\n\r" +
                                      "Direccion: " + ResDatos.Pac_Direccion + "\n\r" +
                                      "Email: " + ResDatos.Pac_Email + "\n\r" +
                                      "Fecha Nacimiento: " + Convert.ToDateTime(ResDatos.Pac_FechaNto).ToString(Conexion.ConectionDictionary["Format_Fecha"]) + "\n\r" +
                                      "Sexo: " + Sexi + "\n\r" +
                                      "Regimen: " + cargareg.ToString() + "\n\r" + "DATOS DEL ACUDIENTE" + "\n\r" +
                                      "Acudiente: " + ResDatos.Pac_Acudiente + "\n\r" +
                                      "Parentesco: " + ResDatos.Pac_Parentesco + "\n\r" +
                                      "Direccion: " + ResDatos.Pac_DireccionAcu + "\n\r" +
                                      "Telefono: " + ResDatos.Pac_TelefonoAcu + "\n\r" +
                                      "Correo: " + ResDatos.Pac_CorreoAcu + "\n\r" +
                                      "OTROS DATOS DE INTERES";
                    richTextBox1.Text = Dato_PAC;

                    this.PacID = ResDatos.Pac_Id;

                    checkBox1.Visible = true;
                    checkBox2.Visible = true;
                    checkBox3.Visible = true;

                    checkBox1.Checked = (ResDatos.Pac_2VXS == "S" ? true : false);
                    checkBox2.Checked = (ResDatos.Pac_Doble == "S" ? true : false);
                    checkBox3.Checked = (ResDatos.Pac_Especial == "S" ? true : false);
                }
                else
                {
                    checkBox1.Visible = false;
                    checkBox2.Visible = false;
                    checkBox3.Visible = false;

                    DialogResult result = MessageBox.Show("El tipo de documento y numero de documentos ingresados " +
                        "no existen en sistema. ¿Desea crearlo?",
                        "Zamenis Health",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        richTextBox1.Text = "";
                        ZamenisHealth.Recepcion.CrearEditarPaciente Crear_Paciente = new ZamenisHealth.Recepcion.CrearEditarPaciente();
                        Crear_Paciente.Size = new Size(619, 672);
                        Crear_Paciente.StartPosition = FormStartPosition.CenterScreen;
                        Crear_Paciente.FormBorderStyle = FormBorderStyle.FixedSingle;
                        Crear_Paciente.AutoScroll = false;
                        Crear_Paciente.ShowDialog();
                    }
                    if (result == DialogResult.No)
                    { }
                    richTextBox1.Text = "";
                    return;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void btnZamenis2_ButtonClick(object sender, EventArgs e)
        {
            Contenedor fTemp = Application.OpenForms.OfType<Contenedor>().LastOrDefault();
            fTemp.CierraPanel();
            this.Dispose(); 
            this.Close();
        }
        private void btnZamenis3_ButtonClick(object sender, EventArgs e)
        {
            richTextBox1.Text = "";
        }

        private void CargarDocumentos()
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

        private void textBox1_DoubleClick(object sender, EventArgs e)
        {
            BuscarPacientes buscarPacientes = new BuscarPacientes();
            buscarPacientes.Tipo_Busca_Pac = "BusquedaGeneral";
            buscarPacientes.ShowDialog();
        }

        private void DatosPac_Load(object sender, EventArgs e)
        {
            CargarDocumentos();
        }

        private void checkBox1_Click(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                repositorioPacientes.VariosDias(this.PacID, "S");
            }
            if (checkBox1.Checked == false)
            {
                repositorioPacientes.VariosDias(this.PacID, "N");
            }
        }

        private void checkBox2_Click(object sender, EventArgs e)
        {
            if (checkBox2.Checked == true)
            {
                repositorioPacientes.VariasHeridas(this.PacID, "S");
            }
            if (checkBox2.Checked == false)
            {
                repositorioPacientes.VariasHeridas(this.PacID, "N");
            }
        }
        private void checkBox3_Click(object sender, EventArgs e)
        {
            if (checkBox2.Checked == true)
            {
                repositorioPacientes.PacEspecial(this.PacID, "S");
            }
            if (checkBox2.Checked == false)
            {
                repositorioPacientes.PacEspecial(this.PacID, "N");
            }
        }
    }
}
