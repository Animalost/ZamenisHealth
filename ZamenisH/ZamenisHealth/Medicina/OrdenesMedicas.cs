using Domain;
using Domain.CXN;
using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Windows.Forms;
using ZamenisHealth.Clases;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.Medicina
{
    public partial class OrdenesMedicas : Forma
    {
        private static readonly IBodegas repositorioBodegas = new MBodegas();
        private static readonly ICompañia repositorioCompañias = new MCompañia();
        private static readonly IPacientes repositorioPacientess = new MPacientes();
        private static readonly ICIE10 repositorioCIE10 = new MCIE10();
        private static readonly IOrdenes repositorioOrdenes = new MOrdenes();

        int Paciente = 0;
        int Aseguradora = 0;
        public string TID = "";
        public string ID = "";
        private string _tipo_Especialidad;
        
        private ToolStripButton btnBuscar;
        private ToolStripButton btnSave;
        private ToolStripButton btnClose;

        public OrdenesMedicas(string TipoEspecialidad)
        {
            InitializeComponent();
            this._tipo_Especialidad = TipoEspecialidad;

            ConfigForm.SoloNumeros(textBox13);
        }
        public void CargarDocumentos()
        {
            var ListaDocs = repositorioPacientess.ListaDocs();
            if (ListaDocs != null)
            {
                foreach (var i in ListaDocs)
                {
                    comboBox1.Items.Add(i);
                }
            }
        }
        private void OrdenesMedicas_Load(object sender, EventArgs e)
        {
            ConfigForm.SoloNumeros(textBox5);

            Titulo.Text = "Crear ordenes medicas";
            LogoMain.Image = Properties.Resources.Splash;
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";

            ImageClose.Visible = false;
            ImageMinimize.Visible = false;

            btnBuscar = new ToolStripButton();
            btnBuscar = createToolButton("Buscar");
            MenuLateral.Items.Add(btnBuscar);
            btnBuscar.Click += button1_Click;

            btnSave = new ToolStripButton();
            btnSave = createToolButton("Grabar");
            MenuLateral.Items.Add(btnSave);
            btnSave.Click += toolStripLabel1_Click;
            btnSave.Enabled = false;

            btnClose = new ToolStripButton();
            btnClose = createToolButton("Cancelar");
            MenuLateral.Items.Add(btnClose);
            btnClose.Click += toolStripLabel2_Click;

            comboBox6.SelectedIndex = 0;
            
            var esmedico = repositorioBodegas.EsProfesional("Medico", Comunes.Contenedor.UsuarioLogueado);
            if (esmedico != true)
            {
                MessageBox.Show("Su usuario no es tipo medico, no puede crear ordenes medicas",
                    "Acceso Denegado!!!",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                this.Dispose();
                this.Close();
                return;
            }

            textBox5.MaxLength = 10;
            var pres = repositorioCompañias.getAllCompañias();
            if (pres != null)
            {
                foreach (var i in pres)
                {
                    comboBox2.Items.Add(i.Com_Nombre);
                }

                comboBox2.SelectedIndex = 0;
            }

            CargarDocumentos();
            checkBox1.BringToFront();
        }
        private void toolStripLabel2_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("¿Desea canelar esta orden medica?",
                                                "Zamenis Health - Cancelar Orden Medica",
                                                MessageBoxButtons.YesNo,
                                                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                this.Dispose();
                this.Close();
            }            
        }
        private void textBox1_DoubleClick(object sender, EventArgs e)
        {
            Comunes.BuscarPacientes P = new Comunes.BuscarPacientes("OrdenesS");
            P.ShowDialog();
        }
        private void button5_Click(object sender, EventArgs e)
        {
            textBox7.Text = "";
            textBox10.Text = "";
        }
        private void button6_Click(object sender, EventArgs e)
        {
            textBox8.Text = "";
            textBox11.Text = "";
        }
        private void button7_Click(object sender, EventArgs e)
        {
            textBox9.Text = "";
            textBox12.Text = "";
        }
        public void BuscarPaciente()
        {
            try
            {
                var DatosPaciente = repositorioPacientess.LlamarPacienteDOC(comboBox1.Text, textBox1.Text);
                if (DatosPaciente != null)
                {
                    panel1.Enabled = true;
                    Paciente = Convert.ToInt32(DatosPaciente.Pac_Id);
                    Aseguradora = Convert.ToInt32(DatosPaciente.Pac_Aseguradora);
                    textBox2.Text = DatosPaciente.Pac_PrimerN + " " + DatosPaciente.Pac_SegundoN + " " + DatosPaciente.Pac_PrimerA + " " + DatosPaciente.Pac_SegundoA;
                    textBox4.Text = DatosPaciente.Pac_Direccion;
                    textBox5.Text = DatosPaciente.Pac_Telefono;

                    switch (DatosPaciente.Pac_Sexo)
                    {
                        case "M":
                            comboBox3.Text = "Masculino";
                            break;

                        case "F":
                            comboBox3.Text = "Femenino";
                            break;

                        case "I":
                            comboBox3.Text = "Intersexual";
                            break;

                        default:
                            comboBox3.Text = "";
                            break;
                    }

                    DateTime nacimiento = Convert.ToDateTime(DatosPaciente.Pac_FechaNto);
                    int edad = DateTime.Today.AddTicks(-nacimiento.Ticks).Year - 1;
                    textBox6.Text = edad.ToString();

                    CargaDX();

                    btnBuscar.Enabled = false;
                    btnSave.Enabled = true;
                }
                else
                {
                    panel1.Enabled = false;
                    MessageBox.Show("Documento digitado no existe", "Sin Resultados", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            BuscarPaciente();
        }
        private void CargaDX()
        {
            try
            {
                var Dx = repositorioCIE10.CargaDX(Paciente);
                if (Dx != null)
                {
                    textBox7.Text = Dx.OM_DX1;
                    textBox8.Text = Dx.OM_DX2;
                    textBox9.Text = Dx.OM_DX3;

                    textBox10.Text = repositorioCIE10.BuscaDX(Dx.OM_DX1);
                    textBox11.Text = repositorioCIE10.BuscaDX(Dx.OM_DX2);
                    textBox12.Text = repositorioCIE10.BuscaDX(Dx.OM_DX3);
                }
                else
                {
                    textBox7.Text = "";
                    textBox8.Text = "";
                    textBox9.Text = "";

                    textBox10.Text = "";
                    textBox11.Text = "";
                    textBox12.Text = "";
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void textBox7_Click(object sender, EventArgs e)
        {
            Medicina.CIE10 C = new CIE10("DX1_COrdenS");
            C.ShowDialog();
        }
        private void textBox8_Click(object sender, EventArgs e)
        {
            Medicina.CIE10 C = new CIE10("DX2_COrdenS");
            C.ShowDialog();
        }
        private void textBox9_Click(object sender, EventArgs e)
        {
            Medicina.CIE10 C = new CIE10("DX3_COrdenS");
            C.ShowDialog();
        }
        private void label5_Click(object sender, EventArgs e)
        {
            MessageBox.Show("La compañia es el logo y los datos de la empresa que genera la orden medica, " +
            "esto es diferente al medico que genera la orden medica",
            "Aviso Importante",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
            return;
        }
        private void toolStripLabel1_Click(object sender, EventArgs e)
        {
            try
            {
                if (textBox3.Text == "") { MessageBox.Show("Debe diligenciar un texto para la orden medica"); return; }
                if (Paciente == 0) { MessageBox.Show("Debe seleccionar un paciente"); return; }
                if (Aseguradora == 0) { MessageBox.Show("El paciente no tiene aseguradora"); return; }
                if (textBox6.Text == "") { MessageBox.Show("Debe diligenciar la edad"); return; }
                if (textBox4.Text == "") { MessageBox.Show("Debe diligenciar la direccion"); return; }
                if (comboBox3.Text == "") { MessageBox.Show("Seleccione genero"); return; }
                if (textBox7.Text == "") { MessageBox.Show("Debe haber al menos un diagnostico principal"); return; }
                if (textBox10.Text == "") { MessageBox.Show("Debe haber al menos un diagnostico principal"); return; }
                if (comboBox5.Text == "") { MessageBox.Show("Seleccione la clasificacion de la orden medica"); return; }
                if (textBox13.Visible == true && string.IsNullOrEmpty(textBox13.Text)) { MessageBox.Show("Debe escribir los dias de la incapacidad medica"); return; }

                var celular = repositorioPacientess.ValidaCelular(textBox5.Text);
                if (celular != true)
                {
                    MessageBox.Show("El numero de celular es incorrecto, este campo es oligatorio", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var Cia = repositorioCompañias.getPrestadorbyName(comboBox2.Text);

                var NumOrden = repositorioCompañias.getPrestadorbyCode(Convert.ToInt32(Cia.Com_Identificador));
                if (NumOrden == null)
                {
                    MessageBox.Show("Error en numero de orden a asignar", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string confirmaFirma = "NO";

                if (checkBox1.Checked == true)
                {
                    confirmaFirma = "SI";
                }

                CXN_OM OM = new CXN_OM
                {
                    OM_Pac = Paciente,
                    OM_Ase = Aseguradora,
                    OM_Cia = Cia.Com_Identificador,
                    OM_Prof = Comunes.Contenedor.UsuarioLogueado,
                    OM_Desc = textBox3.Text,
                    OM_DX1 = textBox7.Text,
                    OM_DX2 = textBox8.Text,
                    OM_DX3 = textBox9.Text,
                    OM_DX1T = textBox10.Text,
                    OM_DX2T = textBox11.Text,
                    OM_DX3T = textBox12.Text,
                    OM_Edad = textBox6.Text,
                    OM_Genero = comboBox3.Text,
                    OM_Direccion = textBox4.Text,
                    OM_Telefono = textBox5.Text,
                    OM_Num = NumOrden.Com_OM,
                    OM_Firma = confirmaFirma,
                    OM_TEspecialidad = this._tipo_Especialidad,
                    OM_Clasificacion = comboBox5.Text,
                    OM_FHIR_INC = comboBox6.Visible == true ? comboBox6.Text == "Nueva" ? "01" : "02" : "",
                    OM_Dias = textBox13.Visible == true ? Convert.ToInt32(textBox13.Text) : 0
                };

                if (this._tipo_Especialidad == "MG")
                {
                    DialogResult result = MessageBox.Show("¿Desea agregar esta orden medica a la planilla de autorizaciones?",
                                                          "Zamenis Health - Planillas",
                                                          MessageBoxButtons.YesNo,
                                                          MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        OM.OM_Planillar = "S";
                    }
                }

                bool grabarOrden = repositorioOrdenes.CrearOrden(OM);
                if (grabarOrden != true)
                {
                    MessageBox.Show("No se logro generar la orden medica",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }

                int NuevoNumero = Convert.ToInt32(OM.OM_Num) + 1;
                bool _updateCons = repositorioCompañias.ConsecutivoActualiza(OM.OM_Cia, "OM", NuevoNumero);

                if (_updateCons != true)
                {
                    MessageBox.Show("La orden medica se genero con el numero " + OM.OM_Num.ToString() + " pero no se logro actualizar el consecutivo " +
                        "informe a la administracion antes de continuar con la generacion de otra orden medica",
                        "Generado con Inconsistencia",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation);
                    this.Dispose();
                    this.Close();
                    return;
                }

                var Exportar = repositorioOrdenes.Generar_OrdenMedica(Convert.ToInt32(NumOrden.Com_OM), Cia.Com_Identificador, Comunes.Contenedor.UsuarioLogueado);
                if (Exportar == null)
                {
                    MessageBox.Show("La orden medica se genero pero no se logro exportar, ingrese por la opcion " +
                        "de busqueda de ordenes medicas",
                        "Inconsistencia",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation);
                    this.Dispose();
                    this.Close();
                    return;
                }

                if (this._tipo_Especialidad == "FI")
                {
                    ConfigForm.GenerarReportViewer("Dataset_OM",
               "ZamenisHealth.Reportes.RDLC_OrdenesServicios.rdlc",
               Exportar);

                }
                else if (this._tipo_Especialidad == "MG" || this._tipo_Especialidad == "RA")
                {                    
                    OrdenesMedicasT OT = new OrdenesMedicasT(Cia.Com_Identificador,
                                                             Convert.ToInt32(NumOrden.Com_OM),
                                                             Comunes.Contenedor.UsuarioLogueado);

                    OT.ShowDialog();
                }
                else
                {
                    MessageBox.Show("Error inesperado, no se logro imprimir la orden medica pero si ha quedado guardada, genere una copia", "Error inesperado", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                this.Dispose();
                this.Close();
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox5.Text == "INCAPACIDAD MEDICA")
            {
                label1.Visible = true;
                comboBox6.Visible = true;
                label8.Visible = true;
                textBox13.Visible = true;
            }
            else
            {
                label1.Visible = false;
                comboBox6.Visible = false;
                label8.Visible = false;
                textBox13.Visible = false;
            }
        }
    }
}
