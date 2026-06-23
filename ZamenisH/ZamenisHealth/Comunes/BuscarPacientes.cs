using Domain;
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
using ZamenisHealth.FrontFHIR.VisorZamenis;
using ZamenisHealth.Medicina.DocumentosWEB;
using ZamenisHealth.Medicina.OrdenesExtra;

namespace ZamenisHealth.Comunes
{
    public partial class BuscarPacientes : Forma2
    {
        private static readonly IPacientes repositorioPacientes = new MPacientes();

        public string Tipo_Busca_Pac;
        DataTable dt = new DataTable();
        DataColumn POS;
        DataColumn Tipo;
        DataColumn Identificacion;
        DataColumn Pacientes;

        public BuscarPacientes()
        {
            InitializeComponent();            
        }

        public BuscarPacientes(string _tipo_Busca_Pac)
        {
            InitializeComponent();            
            this.Tipo_Busca_Pac = _tipo_Busca_Pac;
        }

        private void BuscarPacientes_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Buscar Paciente";
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";
            gridZH1.dataGridView1.CellClick += dataGridView1_CellClick;
            gridZH1.CeldaHeight = true;
        }
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            Buscar();
        }
        private void Encabezados()
        {
            gridZH1.dataGridView1.DataSource = null;
            dt = new DataTable();
            POS = dt.Columns.Add("POS", typeof(int));
            Tipo = dt.Columns.Add("Tipo", typeof(string));
            Identificacion = dt.Columns.Add("Identificacion", typeof(string));
            Pacientes = dt.Columns.Add("Paciente", typeof(string));
        }
        void Estilos()
        {
            gridZH1.dataGridView1.DataSource = dt;
            gridZH1.dataGridView1.Columns["POS"].Visible = false;
        }
        private void Buscar()
        {
            try
            {
                if (string.IsNullOrEmpty(textBox1.Text))
                {
                    MensajesGeneral MG = new MensajesGeneral
                    {
                        Mensaje = "Debe diligenciar al menos el campo de apellido",
                        TipoImagen = 1000
                    };
                    MG.ShowDialog();
                }
                else
                {
                    List<CXN_PACIENTES> _DatosPacientes = repositorioPacientes.LlamarPacienteDOCSimilares(textBox1.Text, textBox2.Text);
                    if (_DatosPacientes != null)
                    {
                        Encabezados();
                        int Contador = 1;

                        foreach (var i in _DatosPacientes)
                        {
                            DataRow row = dt.NewRow();

                            row["POS"] = Contador;
                            row["Tipo"] = i.Pac_TipoId.ToString();
                            row["Identificacion"] = i.Pac_IdNum.ToString().Trim();
                            row["Paciente"] = i.Pac_PrimerN.ToString();

                            dt.Rows.Add(row);
                            dt.AcceptChanges();

                            Contador = Contador + 1;
                        }

                        Contador = 1;
                        Estilos();
                    }
                    else
                    {
                        MensajesGeneral MG = new MensajesGeneral
                        {
                            Mensaje = "No se encontraron coincidencias con los criterios de busqueda ingresados",
                            TipoImagen = 1000
                        };
                        MG.ShowDialog();

                        Encabezados();
                    }
                }                
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void BuscarPacientes_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyData == Keys.F5)
                {
                    Buscar();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            textBox1.CharacterCasing = CharacterCasing.Upper;
        }
        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            textBox2.CharacterCasing = CharacterCasing.Upper;
        }        
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                string TipoId = gridZH1.dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
                string Identidad = gridZH1.dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString();

                switch (Tipo_Busca_Pac)
                {
                    case "Pacientes":
                        Recepcion.CrearEditarPaciente f1 = Application.OpenForms.OfType<Recepcion.CrearEditarPaciente>().LastOrDefault();
                        f1.comboBox1.Text =TipoId;
                        f1.textBox1.Text =Identidad;
                        this.Dispose();
                        this.Close();
                        break;

                    case "BusquedaGeneral":
                        ConfigContenedor.DatosPac f2 = Application.OpenForms.OfType<ConfigContenedor.DatosPac>().LastOrDefault();
                        f2.comboBox1.Text =TipoId;
                        f2.textBox1.Text =Identidad;
                        this.Dispose();
                        this.Close();
                        break;

                    case "AgendarCita":
                        Recepcion.AgendarCita f4 = Application.OpenForms.OfType<Recepcion.AgendarCita>().LastOrDefault();
                        f4.comboBox1.Text =TipoId;
                        f4.textBox1.Text =Identidad;
                        this.Dispose();
                        this.Close();
                        break;

                    case "Asistencia":
                        Recepcion.Asistencia f5 = Application.OpenForms.OfType<Recepcion.Asistencia>().LastOrDefault();
                        f5.textBox1.Text = Identidad;
                        this.Dispose();
                        this.Close();
                        break;

                    case "ExpAgenda":
                        Recepcion.Extras.PrintAgendas f6 = Application.OpenForms.OfType<Recepcion.Extras.PrintAgendas>().LastOrDefault();
                        f6.comboBox3.Text =TipoId;
                        f6.textBox1.Text =Identidad;
                        this.Dispose();
                        this.Close();
                        break;

                    case "BusVen":
                        Recepcion.Ventas.BuscarPaciente f7 = Application.OpenForms.OfType<Recepcion.Ventas.BuscarPaciente>().LastOrDefault();
                        f7.textBox1.Text = Identidad;
                        this.Dispose();
                        this.Close();
                        break;

                    case "Cotizaciones":
                        Recepcion.Particulares f14 = Application.OpenForms.OfType<Recepcion.Particulares>().LastOrDefault();
                        f14.comboBox1.Text =TipoId;
                        f14.textBox61.Text =Identidad;
                        this.Dispose();
                        this.Close();
                        break;

                    case "Cotiza_Hist":
                        Recepcion.Extras.ParticularesPrevios f15 = Application.OpenForms.OfType<Recepcion.Extras.ParticularesPrevios>().LastOrDefault();
                        f15.comboBox1.Text =TipoId;
                        f15.textBox1.Text =Identidad;
                        this.Dispose();
                        this.Close();
                        break;

                    case "CMANE":
                        Medicina.CambioManejoEnfermero f16 = Application.OpenForms.OfType<Medicina.CambioManejoEnfermero>().LastOrDefault();
                        f16.comboBox1.Text =TipoId;
                        f16.textBox1.Text =Identidad;
                        this.Dispose();
                        this.Close();
                        break;

                    case "NotaAcla":
                        HistoriasClinicas.NotasAclaratorias f17 = Application.OpenForms.OfType<HistoriasClinicas.NotasAclaratorias>().LastOrDefault();
                        f17.comboBox1.Text =TipoId;
                        f17.textBox1.Text =Identidad;
                        this.Dispose();
                        this.Close();
                        break;

                    case "VerImagen":
                        Medicina.VerImagenes f18 = Application.OpenForms.OfType<Medicina.VerImagenes>().LastOrDefault();
                        f18.comboBox1.Text =TipoId;
                        f18.textBox1.Text =Identidad;
                        this.Dispose();
                        this.Close();
                        break;

                    case "OrdenesS":
                        Medicina.OrdenesMedicas f20 = Application.OpenForms.OfType<Medicina.OrdenesMedicas>().LastOrDefault();
                        f20.comboBox1.Text =TipoId;
                        f20.textBox1.Text =Identidad;
                        this.Dispose();
                        this.Close();
                        break;

                    case "Facturacion":
                        Facturacion.Facturar f22 = Application.OpenForms.OfType<Facturacion.Facturar>().LastOrDefault();
                        f22.comboBox1.Text =TipoId;
                        f22.textBox1.Text =Identidad;
                        this.Dispose();
                        this.Close();
                        break;

                    case "Formatos":
                        AdminSystem.Formatos f23 = Application.OpenForms.OfType<AdminSystem.Formatos>().LastOrDefault();
                        f23.comboBox4.Text =TipoId;
                        f23.textBox9.Text =Identidad;
                        this.Dispose();
                        this.Close();
                        break;

                    case "Fac_Per":
                        Gerencia f24 = Application.OpenForms.OfType<Gerencia>().LastOrDefault();
                        f24.comboBox1.Text =TipoId;
                        f24.textBox1.Text =Identidad;
                        this.Dispose();
                        this.Close();
                        break;

                    case "FirmaDocs":
                        Medicina.FirmaHistorias f26 = Application.OpenForms.OfType<Medicina.FirmaHistorias>().LastOrDefault();
                        f26.comboBox2.Text =TipoId;
                        f26.textBox1.Text =Identidad;
                        this.Dispose();
                        this.Close();
                        break;

                    case "CompleteJM":
                        HistoriasClinicas.Historia_JM_Completar f27 = Application.OpenForms.OfType<HistoriasClinicas.Historia_JM_Completar>().LastOrDefault();
                        f27.setDatos(TipoId, Identidad);
                        this.Dispose();
                        this.Close();
                        break;

                    case "Encuesta":
                        Fibromialgia.Encuestas.Menu f28 = Application.OpenForms.OfType<Fibromialgia.Encuestas.Menu>().LastOrDefault();
                        f28.setDoc(TipoId, Identidad);
                        this.Dispose();
                        this.Close();
                        break;

                    case "InfFibroGen":
                        Fibromialgia.Informes.InformesPrograma f29 = Application.OpenForms.OfType<Fibromialgia.Informes.InformesPrograma>().LastOrDefault();
                        f29.setDoc(TipoId, Identidad);
                        this.Dispose();
                        this.Close();
                        break;

                    case "CANCELAWEB":
                        Recepcion.Extras.CancelacionesWEB f30 = Application.OpenForms.OfType<Recepcion.Extras.CancelacionesWEB>().LastOrDefault();
                        f30.setDatos(TipoId, Identidad);
                        this.Dispose();
                        this.Close();
                        break;

                    case "HistorialMedico":
                        Medicina.Historial_Medico_1 f32 = Application.OpenForms.OfType<Medicina.Historial_Medico_1>().LastOrDefault();
                        f32.setDoc(TipoId, Identidad);
                        this.Dispose();
                        this.Close();
                        break;

                    case "Agenda2":
                        Recepcion.Extras.Busqueda f33 = Application.OpenForms.OfType<Recepcion.Extras.Busqueda >().LastOrDefault();
                        f33.setDoc(Identidad);
                        this.Dispose();
                        this.Close();
                        break;

                    case "INGSAL":
                        Medicina.EstInformesMG f34 = Application.OpenForms.OfType<Medicina.EstInformesMG>().LastOrDefault();
                        f34.setSelection(Identidad);
                        this.Dispose();
                        this.Close();
                        break;

                    case "UploadPDF":
                        Medicina.UploadHistory f35 = Application.OpenForms.OfType<Medicina.UploadHistory>().LastOrDefault();
                        f35.setSelection(Identidad);
                        this.Dispose();
                        this.Close();
                        break;

                    case "FacturacionAbierta":
                        Facturacion.FacturaAbierta f36 = Application.OpenForms.OfType<Facturacion.FacturaAbierta>().LastOrDefault();
                        f36.setSelection(Identidad);
                        this.Dispose();
                        this.Close();
                        break;

                    case "ConsentimientosWEB":
                        MenuDocWeb f37 = Application.OpenForms.OfType<MenuDocWeb>().LastOrDefault();
                        f37.setDoc(Identidad);
                        this.Dispose();
                        this.Close();
                        break;

                    case "FHIR":
                        VerRDA f38 = Application.OpenForms.OfType<VerRDA>().LastOrDefault();
                        f38.setDoc(TipoId, Identidad);
                        this.Dispose();
                        this.Close();
                        break;

                    case "OrdenesFHIRExtra":
                        CrearOrdenExtra f39 = Application.OpenForms.OfType<CrearOrdenExtra>().LastOrDefault();
                        f39.setDoc(Identidad);
                        this.Dispose();
                        this.Close();
                        break;
                        

                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
