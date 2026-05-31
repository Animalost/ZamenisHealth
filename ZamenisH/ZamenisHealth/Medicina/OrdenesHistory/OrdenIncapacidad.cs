using Domain.CXN;
using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Linq;
using System.Windows.Forms;
using ZamenisHealth.Clases;
using ZamenisHealth.Comunes;
using ZamenisHealth.HistoriasClinicas;

namespace ZamenisHealth.Medicina.OrdenesHistory
{
    public partial class OrdenIncapacidad : Forma2
    {
        private IAgendaC datosAdm;
        private ICompañia datosCia;
        private IOrdenes ordenes;

        private string Forma;
        private MensajesGeneral MG;
        private int Admision;

        public OrdenIncapacidad(string forma, int admision)
        {
            InitializeComponent();
            this.Forma = forma;
            Admision = admision;
            SoloNumeros(textBox2);
            datosAdm = new MAgendaC();
            datosCia = new MCompañia();
            ordenes = new MOrdenes();
        }

        private void OrdenIncapacidad_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Incapacidades";
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";
        }

        private void boton1_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(textBox1.Text) || string.IsNullOrEmpty(textBox2.Text) || comboBox1.Text == "")
                {
                    MG = new MensajesGeneral()
                    {
                        Mensaje = "Debe diligencir todos los campos",
                        TipoImagen = 1000
                    };

                    MG.ShowDialog();
                }
                else 
                {
                    otrosDatosPacienteHorario datos = datosAdm.cargarAdmision(Admision, "'H','P','A'");
                    Historia_MedicinaGeneral f1 = null;
                    Historia_Fisiatria f2 = null;
                    CXN_CIA NumOrden = datosCia.getPrestadorbyCode(datos.Hor_Pac_Cia);
                    string Especialidad = "";

                    if (Forma == "MEDGEN")
                    {
                        f1 = Application.OpenForms.OfType<Historia_MedicinaGeneral>().LastOrDefault();
                        Especialidad = "MG";
                    }
                    else if (Forma == "FISIATRIA")
                    {
                        f2 = Application.OpenForms.OfType<Historia_Fisiatria>().LastOrDefault();
                        Especialidad = "FI";
                    }

                    CXN_OM OM = new CXN_OM
                    {
                        OM_Pac = datos.Hor_Pac_Id,
                        OM_Ase = datos.Hor_Pac_Ase,
                        OM_Cia = datos.Com_Identificador,
                        OM_Prof = Comunes.Contenedor.UsuarioLogueado,
                        OM_Desc = textBox1.Text,
                        OM_DX1 = f1.textBox39.Text,
                        OM_DX2 = f1.textBox37.Text,
                        OM_DX3 = f1.textBox35.Text,
                        OM_DX1T = f1.textBox38.Text,
                        OM_DX2T = f1.textBox36.Text,
                        OM_DX3T = f1.textBox34.Text,
                        OM_Edad = f1.edad.ToString(),
                        OM_Genero = datos.Pac_Sexo == "M" ? "Masculino" : "Femenino",
                        OM_Direccion = datos.PacienteDireccion,
                        OM_Telefono = datos.Pac_Telefono,
                        OM_Num = NumOrden.Com_OM,
                        OM_Firma = "SI",
                        OM_TEspecialidad = Especialidad,
                        OM_Clasificacion = "INCAPACIDAD MEDICA",
                        OM_FHIR_INC = comboBox1.Text == "Nueva" ? "01" : "02",
                        OM_Dias = Convert.ToInt32(textBox2.Text),
                        OM_Planillar = "N"
                    };

                    bool grabarOrden = ordenes.CrearOrden(OM);
                    if (grabarOrden != true)
                    {
                        MessageBox.Show("No se logro generar la orden medica",
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        return;
                    }

                    int NuevoNumero = Convert.ToInt32(OM.OM_Num) + 1;
                    bool _updateCons = datosCia.ConsecutivoActualiza(OM.OM_Cia, "OM", NuevoNumero);

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

                    CXN_ORDENESFHIR cXN_ORDENESFHIR = new CXN_ORDENESFHIR
                    {
                        Admision = Admision,
                        CUP = textBox2.Text, //dias en este caso
                        Servicio = comboBox1.Text, //prorroga o nueva en este caso
                        Tipo = "INCAPACIDAD MEDICA",
                        Paciente = datos.Hor_Pac_Id,
                        DX1 = f1.textBox39.Text,
                        DX2 = f1.textBox37.Text,
                        DX3 = f1.textBox35.Text,
                        Medico = Comunes.Contenedor.UsuarioLogueado
                    };

                    ordenes.CrearOrdenFHIR(cXN_ORDENESFHIR);

                    var Exportar = ordenes.Generar_OrdenMedica(Convert.ToInt32(NumOrden.Com_OM), datos.Hor_Pac_Cia, Comunes.Contenedor.UsuarioLogueado);
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

                    if (Especialidad == "FI")
                    {
                        ConfigForm.GenerarReportViewer("Dataset_OM", "ZamenisHealth.Reportes.RDLC_OrdenesServicios.rdlc", Exportar);
                    }
                    else if (Especialidad == "MG" || Especialidad == "RA")
                    {
                        OrdenesMedicasT OT = new OrdenesMedicasT(datos.Hor_Pac_Cia,
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
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
    }
}
