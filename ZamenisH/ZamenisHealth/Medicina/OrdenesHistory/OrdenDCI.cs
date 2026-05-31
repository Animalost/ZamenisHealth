using Domain.CXN;
using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using ZamenisHealth.Clases;
using ZamenisHealth.Comunes;
using ZamenisHealth.HistoriasClinicas;

namespace ZamenisHealth.Medicina.OrdenesHistory
{
    public partial class OrdenDCI : Forma2
    {
        private IOrdenes ordenes;
        private ICompañia cia;
        private IAgendaC agendaC;
        private IFHIR oController;

        private string Forma;
        private MensajesGeneral MG;
        private int Admision;

        public OrdenDCI(string forma, int admision)
        {
            InitializeComponent();
            ordenes = new MOrdenes();
            cia = new MCompañia();
            agendaC = new MAgendaC();
            oController = new MFHIR();

            SoloNumeros(textBox2);
            SoloNumeros(textBox13);
            SoloNumeros(textBox16);

            this.Forma = forma;
            Admision = admision;
        }

        private void OrdenDCI_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Medicamentos";
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";

            CargarUMM();
            CargarVia();
        }
        void CargarVia()
        {
            List<string> lista = oController.GetConsumos();
            foreach (string v in lista)
            {
                comboBox5.Items.Add(v);
            }
        }
        void CargarUMM()
        {
            List<string> lista = ordenes.GetUMM();
            if (lista != null)
            {
                foreach (string item in lista) 
                {
                    comboBox1.Items.Add(item);
                }
                
                comboBox1.Text = "mg";
            }
        }

        private void textBox3_DoubleClick(object sender, EventArgs e)
        {
            OrdenesMedicasM2 ordenesMedicasM2 = new OrdenesMedicasM2("RECETAADMITION");
            ordenesMedicasM2.ShowDialog();
        }

        private void boton1_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(textBox3.Text) || string.IsNullOrEmpty(textBox1.Text) ||
                    string.IsNullOrEmpty(textBox2.Text) || string.IsNullOrEmpty(textBox15.Text) ||
                    string.IsNullOrEmpty(textBox13.Text) || string.IsNullOrEmpty(textBox16.Text) ||
                    comboBox1.Text == "" || comboBox2.Text == "" || comboBox3.Text == "" ||
                    comboBox4.Text == "" || comboBox5.Text == "")
                {
                    MG = new MensajesGeneral()
                    {
                        Mensaje = "Debe rellenar todos los campos",
                        TipoImagen = 1000
                    };

                    MG.ShowDialog();
                }
                else
                {
                    dataGridView1.Rows.Add(textBox3.Text,
                                           textBox1.Text,
                                           textBox15.Text,
                                           comboBox5.Text,
                                           textBox2.Text,
                                           comboBox3.Text,
                                           textBox13.Text,
                                           comboBox1.Text,
                                           textBox16.Text,
                                           comboBox2.Text,
                                           comboBox4.Text,
                                           textBox17.Text);

                    textBox3.Text = "";
                    textBox1.Text = "";
                    textBox15.Text = "";
                    comboBox5.Text = "";
                    textBox2.Text = "";
                    comboBox3.Text = "";
                    textBox13.Text = "";
                    comboBox1.Text = "";
                    textBox16.Text = "";
                    comboBox2.Text = "";
                    comboBox4.Text = "";
                    textBox17.Text = "";
                }
            }
            catch (Exception ex) 
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var confirm = MessageBox.Show("¿Eliminar este medicamento?", "Confirmar", MessageBoxButtons.YesNo);

                if (confirm == DialogResult.Yes)
                {
                    dataGridView1.Rows.RemoveAt(e.RowIndex);
                }
            }
        }

        private void boton2_Click(object sender, EventArgs e)
        {
            try
            {
                var confirm = MessageBox.Show("¿Desea generar esta orden de servicios?", "Confirmar", MessageBoxButtons.YesNo);

                if (confirm == DialogResult.Yes)
                {
                    otrosDatosPacienteHorario dataAdmi = agendaC.cargarAdmision(Admision, "'H','P','A'");
                    Historia_MedicinaGeneral f1 = null;
                    Historia_Fisiatria f2 = null;
                    CXN_CIA NumOrden = cia.getPrestadorbyCode(dataAdmi.Hor_Pac_Cia);
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

                    if (dataGridView1.Rows.Cast<DataGridViewRow>().Any(r => !r.IsNewRow))
                    {
                        foreach (DataGridViewRow fila in dataGridView1.Rows)
                        {
                            if (!fila.IsNewRow)
                            {
                                string Medicamento = fila.Cells["Medicamento"].Value?.ToString();
                                string CodMedicamento = fila.Cells["CodMedicamento"].Value?.ToString();
                                string Presentacion = fila.Cells["Presentacion"].Value?.ToString();
                                string Via = fila.Cells["Via"].Value?.ToString();
                                string Cada = fila.Cells["Cada"].Value?.ToString();
                                string FrecAdmi = fila.Cells["FrecAdmi"].Value?.ToString();
                                string Cantidad = fila.Cells["Cantidad"].Value?.ToString();
                                string UMM = fila.Cells["UMM"].Value?.ToString();
                                string Duracion = fila.Cells["Duracion"].Value?.ToString();
                                string Tiempo = fila.Cells["Tiempo"].Value?.ToString();
                                string TipoTecnologia = fila.Cells["TipoTecnologia"].Value?.ToString();
                                string Observacion = fila.Cells["Observacion"].Value?.ToString();

                                CXN_OM OM = new CXN_OM
                                {
                                    OM_Duracion = Duracion + " " + Tiempo,
                                    OM_Cantidad = Cantidad + " " + UMM,
                                    OM_Via = Via,
                                    OM_Presentacion = Presentacion,
                                    OM_Detalle = Observacion,
                                    OM_Firma = "SI",
                                    OM_Medicamento = Medicamento,
                                    OM_Direccion = dataAdmi.PacienteDireccion,
                                    OM_Telefono = dataAdmi.Pac_Telefono,
                                    OM_Genero = dataAdmi.Pac_Sexo == "M" ? "Masculino" : "Femenino",
                                    OM_Tecnologia = TipoTecnologia,

                                    OM_Pac = dataAdmi.Hor_Pac_Id,
                                    OM_Ase = dataAdmi.Hor_Pac_Ase,
                                    OM_Cia = dataAdmi.Hor_Pac_Cia,
                                    OM_Prof = Comunes.Contenedor.UsuarioLogueado,
                                    OM_Desc = "",
                                    OM_DX1 = f1.textBox39.Text,
                                    OM_DX2 = f1.textBox37.Text,
                                    OM_DX3 = f1.textBox35.Text,
                                    OM_DX1T = f1.textBox38.Text,
                                    OM_DX2T = f1.textBox36.Text,
                                    OM_DX3T = f1.textBox34.Text,
                                    OM_Edad = f1.edad.ToString(),
                                    OM_Num = NumOrden.Com_OM,
                                    OM_TEspecialidad = Especialidad,
                                    OM_Clasificacion = "ORDEN DE MEDICAMENTOS",
                                    OM_Tipo = "M"
                                };

                                ordenes.CrearOrdenM(OM);

                                CXN_ORDENESFHIR cXN_ORDENESFHIR = new CXN_ORDENESFHIR
                                {
                                    Admision = Admision,
                                    CUP = "", 
                                    Servicio = "", 
                                    Tipo = "ORDEN DE MEDICAMENTOS",
                                    Paciente = dataAdmi.Hor_Pac_Id,
                                    DX1 = f1.textBox39.Text,
                                    DX2 = f1.textBox37.Text,
                                    DX3 = f1.textBox35.Text,
                                    Medico = Comunes.Contenedor.UsuarioLogueado,

                                    Medicamento = Medicamento,
                                    CodMedicamento = CodMedicamento,
                                    Via = Via,
                                    Cada = Cada,
                                    FrecAdmi = FrecAdmi,
                                    Cantidad = Cantidad,
                                    UMM = UMM,
                                    Duracion = Duracion,
                                    Tiempo = Tiempo,
                                    TipoTecnologia = TipoTecnologia,
                                    Observacion = Observacion,
                                };

                                ordenes.CrearOrdenFHIRMED(cXN_ORDENESFHIR);
                            }                            
                        }

                        int NuevoNumero = Convert.ToInt32(NumOrden.Com_OM) + 1;
                        cia.ConsecutivoActualiza(dataAdmi.Hor_Pac_Cia, "OM", NuevoNumero);

                        var Exportar = ordenes.Genera_Orden_Medicamento(Convert.ToInt32(NumOrden.Com_OM), dataAdmi.Hor_Pac_Cia, Contenedor.UsuarioLogueado);
                        if (Exportar == null)
                        {
                            MessageBox.Show("La orden de medicamentos se genero pero no se logro exportar, ingrese por la opcion " +
                                "de busqueda de ordenes medicas",
                                "Inconsistencia",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Exclamation);
                            this.Dispose();
                            this.Close();
                            return;
                        }

                        ConfigForm.GenerarReportViewer("DataSet_OM", "ZamenisHealth.Reportes.RDLC_OrdenesMedicamentos.rdlc", Exportar);

                        this.Dispose();
                        this.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
