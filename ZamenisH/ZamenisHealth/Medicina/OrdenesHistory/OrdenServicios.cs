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
    public partial class OrdenServicios : Forma2
    {
        private IAgendaC agendaC = new MAgendaC();
        private ICompañia repositorioCompañias = new MCompañia();
        private IOrdenes repositorioOrdenes = new MOrdenes();

        private string Forma;
        private MensajesGeneral MG;
        private int Admision;

        public OrdenServicios(string forma, int admision)
        {
            InitializeComponent();
            this.Forma = forma;
            Admision = admision;
            SoloNumeros(textBox3);
        }

        private void OrdenServicios_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Orden de Servicios";
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";
        }
        private void textBox1_DoubleClick(object sender, EventArgs e)
        {
            ListaServicios listaServicios = new ListaServicios(this);
            listaServicios.ShowDialog();
        }
        private void dataGridView1_CellDoubleClick(object sender, System.Windows.Forms.DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var confirm = MessageBox.Show("¿Eliminar este servicio?", "Confirmar", MessageBoxButtons.YesNo);

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
                if (!string.IsNullOrEmpty(textBox1.Text) && !string.IsNullOrEmpty(textBox2.Text) && 
                    !string.IsNullOrEmpty(textBox3.Text) && textBox3.Text != "0")
                {
                    dataGridView1.Rows.Add(textBox1.Text, textBox2.Text, textBox3.Text, checkBox1.Checked == true ? "S" : "N");
                    textBox1.Text = "";
                    textBox2.Text = "";
                }
                else
                {
                    MG = new MensajesGeneral()
                    {
                        Mensaje = "Debe agregar un servicio y una cantidad",
                        TipoImagen = 1000
                    };
                    MG.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void boton1_Click(object sender, EventArgs e)
        {
            try
            {
                var confirm = MessageBox.Show("¿Desea generar esta orden de servicios?", "Confirmar", MessageBoxButtons.YesNo);

                if (confirm == DialogResult.Yes)
                {
                    if (dataGridView1.Rows.Cast<DataGridViewRow>().Any(r => !r.IsNewRow))
                    {
                        string Especialidad = "";
                        string Planillar = "N";
                        var datosAdm = agendaC.cargarAdmision(Admision, "'P','H','A'");
                        var NumOrden = repositorioCompañias.getPrestadorbyCode(datosAdm.Hor_Pac_Cia);

                        foreach (DataGridViewRow fila in dataGridView1.Rows)
                        {
                            if (!fila.IsNewRow)
                            {
                                string cup = fila.Cells["CUP"].Value?.ToString();
                                string servicio = fila.Cells["SERVICIO"].Value?.ToString();
                                string canti = fila.Cells["CANTIDAD"].Value?.ToString();
                                string bilateral = fila.Cells["BILATERAL"].Value?.ToString();

                                Historia_MedicinaGeneral f1 = null;
                                Historia_Fisiatria f2 = null;

                                if (Forma == "MEDGEN")
                                {
                                    f1 = Application.OpenForms.OfType<Historia_MedicinaGeneral>().LastOrDefault();
                                    Especialidad = "MG";
                                    Planillar = "S";
                                }
                                else if (Forma == "FISIATRIA")
                                {
                                    f2 = Application.OpenForms.OfType<Historia_Fisiatria>().LastOrDefault();
                                    Especialidad = "FI";
                                    Planillar = "N";
                                }

                                CXN_OM OM = new CXN_OM
                                {
                                    OM_Pac = datosAdm.Hor_Pac_Id,
                                    OM_Ase = datosAdm.Hor_Pac_Ase,
                                    OM_Cia = datosAdm.Hor_Pac_Cia,
                                    OM_Prof = Comunes.Contenedor.UsuarioLogueado,
                                    OM_Desc = $"{cup} - {servicio}{Environment.NewLine + Environment.NewLine} CANTIDAD: {canti}",
                                    OM_DX1 = Forma == "MEDGEN" ? f1.textBox39.Text : Forma == "FISIATRIA" ? f2.textBox52.Text : "",
                                    OM_DX2 = Forma == "MEDGEN" ? f1.textBox37.Text : Forma == "FISIATRIA" ? f2.textBox50.Text : "",
                                    OM_DX3 = Forma == "MEDGEN" ? f1.textBox35.Text : Forma == "FISIATRIA" ? f2.textBox48.Text : "",
                                    OM_DX1T = Forma == "MEDGEN" ? f1.textBox38.Text : Forma == "FISIATRIA" ? f2.textBox51.Text : "",
                                    OM_DX2T = Forma == "MEDGEN" ? f1.textBox36.Text : Forma == "FISIATRIA" ? f2.textBox49.Text : "",
                                    OM_DX3T = Forma == "MEDGEN" ? f1.textBox34.Text : Forma == "FISIATRIA" ? f2.textBox47.Text : "",
                                    OM_Edad = Forma == "MEDGEN" ? f1.edad.ToString() : Forma == "FISIATRIA" ? f2.edad.ToString() : "",
                                    OM_Genero = datosAdm.Pac_Sexo == "M" ? "Masculino" : datosAdm.Pac_Sexo == "F" ? "Femenino" : "Indeterminado",
                                    OM_Direccion = datosAdm.PacienteDireccion,
                                    OM_Telefono = datosAdm.Pac_Telefono,
                                    OM_Num = NumOrden.Com_OM,
                                    OM_Firma = "SI",
                                    OM_TEspecialidad = Especialidad,
                                    OM_Clasificacion = "ORDEN DE SERVICIOS",
                                    OM_FHIR_INC = "",
                                    OM_Dias = 0,
                                    OM_Planillar = Planillar,
                                    OM_Bilateral = bilateral
                                };

                                repositorioOrdenes.CrearOrden(OM);

                                if (cup == "1005434" || cup == "1005435" || cup == "1005436" ||
                                    cup == "1005434." || cup == "1005435." || cup == "1005436.")
                                {
                                    cup = "869501";
                                    servicio = "CURACIÓN DE LESIÓN EN PIEL O TEJIDO CELULAR SUBCUTÁNEO";
                                }

                                CXN_ORDENESFHIR cXN_ORDENESFHIR = new CXN_ORDENESFHIR
                                {
                                    Admision = Admision,
                                    CUP = cup,
                                    Servicio = servicio,
                                    Tipo = "ORDEN DE SERVICIOS",
                                    Paciente = datosAdm.Hor_Pac_Id,
                                    DX1 = OM.OM_DX1,
                                    DX2 = OM.OM_DX2,
                                    DX3 = OM.OM_DX3,
                                    Medico = Comunes.Contenedor.UsuarioLogueado
                                };

                                repositorioOrdenes.CrearOrdenFHIR(cXN_ORDENESFHIR);
                            }
                        }
                        int NuevoNumero = Convert.ToInt32(NumOrden.Com_OM) + 1;
                        bool _updateCons = repositorioCompañias.ConsecutivoActualiza(datosAdm.Hor_Pac_Cia, "OM", NuevoNumero);

                        var Exportar = repositorioOrdenes.Generar_OrdenMedica(Convert.ToInt32(NumOrden.Com_OM), datosAdm.Hor_Pac_Cia, Comunes.Contenedor.UsuarioLogueado);
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
                        else
                        {
                            if (Especialidad == "FI")
                            {
                                ConfigForm.GenerarReportViewer("Dataset_OM","ZamenisHealth.Reportes.RDLC_OrdenesServicios.rdlc",Exportar);
                            }
                            else if (Especialidad == "MG" || Especialidad == "RA")
                            {
                                OrdenesMedicasT OT = new OrdenesMedicasT(datosAdm.Hor_Pac_Cia,
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
                    else
                    {
                        MG = new MensajesGeneral()
                        {
                            Mensaje = "Debe agregar al menos un servicio",
                            TipoImagen = 1000
                        };
                        MG.ShowDialog();
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
