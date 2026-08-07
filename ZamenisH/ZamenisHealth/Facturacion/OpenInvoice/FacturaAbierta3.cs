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
using ZamenisHealth.Clases;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.Facturacion
{
    public partial class FacturaAbierta3 : Forma
    {
        private readonly IPacientes repoPacientes = new MPacientes();
        private readonly IFacturacion repoFacturacion = new MFacturacion();
        private readonly ICompañia repoCia = new MCompañia();
        private readonly ICargos repoCargos = new MCargos();
        private readonly IFacElectron repoFacelectron = new MFacElectron();

        private MensajesGeneral MG;
        private List<CXN_CARGOS> C;

        public FacturaAbierta3(List<CXN_CARGOS> c)
        {
            InitializeComponent();
            this.C = c;
            ConfigForm.SoloNumeros(textBox11);
            ConfigForm.SoloNumeros(textBox7);
            ConfigForm.SoloNumeros(textBox8);
            ConfigForm.SoloNumeros(textBox2);
            ConfigForm.SoloNumeros(textBox9);
        }
        void CargarMediosPago()
        {
            try
            {
                List<CXN_MEDIOSPAGO> getMedios = repoFacelectron.ListaMediosPago();
                if (getMedios != null)
                {
                    comboBox5.Items.Clear();

                    foreach (CXN_MEDIOSPAGO m in getMedios)
                    {
                        comboBox5.Items.Add(m.Medio);
                    }

                    comboBox5.Text = "Consignación bancaria";
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void FacturaAbierta3_Load(object sender, EventArgs e)
        {
            CargarMediosPago();
            Titulo.Text = "Facturacion Abierta";
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";
            LogoMain.Image = Properties.Resources.Splash;

            ToolStripButton btnGrabar = new ToolStripButton();
            btnGrabar = createToolButton("Grabar");
            MenuLateral.Items.Add(btnGrabar);
            btnGrabar.Click += button3_Click;

            ConfigForm.SoloNumeros(textBox7);
            ConfigForm.SoloNumeros(textBox8);
            ConfigForm.SoloNumeros(textBox2);
            ConfigForm.SoloNumeros(textBox9);

            List<string> Regi = repoPacientes.ListaRegimen();
            if (Regi != null)
            {
                foreach (string r in Regi)
                {
                    comboBox4.Items.Add(r);
                }
            }

            CXN_PACIENTES Paciente = repoPacientes.LlamarPacientebyId(C[0].Car_Pac);
            textBox12.Text = Paciente.Pac_Email;

            CXN_CIA CO = repoCia.getPrestadorbyCode(Convert.ToInt32(C[0].Car_Cia));
            if (CO != null)
            {
                textBox6.Text = CO.Com_Cod_Prestador_2.ToString();
            }

            comboBox3.Items.Add("02 - Presupuesto máximo");
            comboBox3.Items.Add("03 - Prima EPS / EOC, no asegurados SOAT");
            comboBox3.Items.Add("04 - Cobertura Póliza SOAT");
            comboBox3.Items.Add("05 - Cobertura ARL");
            comboBox3.Items.Add("06 - Cobertura ADRES");
            comboBox3.Items.Add("07 - Cobertura Salud Pública");
            comboBox3.Items.Add("08 - Cobertura entidad territorial, recursos de oferta");
            comboBox3.Items.Add("09 - Urgencias población migrante");
            comboBox3.Items.Add("10 - Plan complementario en salud");
            comboBox3.Items.Add("11 - Plan medicina prepagada");
            comboBox3.Items.Add("12 - Pólizas en salud");
            comboBox3.Items.Add("13 - Cobertura Régimen Especial o Excepción");
            comboBox3.Items.Add("14 - Cobertura Fondo Nacional de Salud de las Personas Privadas de la Libertad");
            comboBox3.Items.Add("15 - Particular");
            comboBox3.Items.Add("16 - Plan de beneficios en Salud dinanciado con UPC contributivo");
            comboBox3.Items.Add("17 - Plan de beneficios en Salud dinanciado con UPC subsidiado");
        }
        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(textBox11.Text))
                {
                    MG = new MensajesGeneral();
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "Debe escribir el tiempo de vencimiento de la factura";
                    MG.ShowDialog();
                    return;
                }

                DialogResult result = MessageBox.Show("¿Desea generar este documento? " + comboBox1.Text,
                                                "Zamenis Health - Cancelar Factura",
                                                MessageBoxButtons.YesNo,
                                                MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                   if (string.IsNullOrEmpty(textBox7.Text) || string.IsNullOrEmpty(textBox8.Text) ||
                        string.IsNullOrEmpty(textBox2.Text) || string.IsNullOrEmpty(textBox9.Text))
                    {
                        MG = new MensajesGeneral();
                        MG.TipoImagen = 1000;
                        MG.Mensaje = "Debe diligenciar los campos de valores economicos, si no hay descuentos ingrese el 0 en cada recuadro";
                        MG.ShowDialog();
                        return;
                    }
                    if (string.IsNullOrEmpty(comboBox2.Text) || string.IsNullOrEmpty(comboBox3.Text) ||
                        string.IsNullOrEmpty(textBox6.Text) || string.IsNullOrEmpty(comboBox4.Text))
                    {
                        MG = new MensajesGeneral();
                        MG.TipoImagen = 1000;
                        MG.Mensaje = "Seleccione las modalidades del documento, regimen y codigo de prestador";
                        MG.ShowDialog();
                        return;
                    }

                    CXN_CARGOS DatosComplementarios = C[0];

                    CXN_CIA DataCia = repoCia.getPrestadorbyCode(DatosComplementarios.Car_Cia);
                    int NumeroDocumento = 0;
                    string Resolucion = "NO APLICA RESOLUCION ES ORDEN DE PEDIDO - FACTURA ABIERTA SIN SERVICIOS GENERADOS";
                    string TDoc = "";

                    switch (comboBox1.SelectedIndex)
                    {
                        case 0: //oprden de pedido
                            NumeroDocumento = DataCia.Com_OP;
                            TDoc = "OP";
                            break;
                        case 1: //factura
                            NumeroDocumento = DataCia.Com_Fac;
                            Resolucion = DataCia.Com_Resolucion + " - FACTURA ABIERTA SIN SERVICIOS GENERADOS";
                            TDoc = "FA";
                            break;
                        case 2: //Doc Eq
                            NumeroDocumento = DataCia.Com_DE;
                            Resolucion = DataCia.Com_Resolucion + " - FACTURA ABIERTA SIN SERVICIOS GENERADOS";
                            TDoc = "DE";
                            break;

                        default:
                            MG = new MensajesGeneral();
                            MG.TipoImagen = 1000;
                            MG.Mensaje = "Seleccione el tipo de documento que desea generar";
                            MG.ShowDialog();
                            return;                        
                    }

                    CXN_FACTURA F = new CXN_FACTURA
                    {
                        Fac_Num_Fac = NumeroDocumento,
                        Fac_Res = Resolucion,
                        Homologo = NumeroDocumento.ToString(),
                        Fac_Estado = "F",
                        Fac_Ase = Convert.ToInt32(DatosComplementarios.Car_Ase),
                        Fac_Cia = Convert.ToInt32(DatosComplementarios.Car_Cia),
                        Fac_Pac = Convert.ToInt32(DatosComplementarios.Car_Pac),
                        Fac_Fecha = Convert.ToDateTime(DateTime.Now.Date),
                        Fac_Fecha_Des = dateTimePicker1.Value.Date,
                        Fac_Fecha_Has = dateTimePicker2.Value.Date,
                        Fac_Num_Aut = textBox1.Text,
                        Fac_Descuento = textBox2.Text,
                        Fac_Observa = richTextBox1.Text,
                        Fac_Usr_Graba = Comunes.Contenedor.UsuarioLogueado,
                        Fac_ConSub = repoPacientes.Regimen(comboBox4.Text),
                        Fac_Tipo_Doc = TDoc,
                        VrCompartido = Convert.ToInt32(textBox8.Text),
                        Copago = Convert.ToInt32(textBox7.Text),
                        Anticipo = Convert.ToInt32(textBox9.Text),
                        CodPrestador = textBox6.Text,
                        ContratoPoliza = textBox5.Text,
                        Cobertura = comboBox3.Text,
                        ModPago = comboBox2.Text,
                        DiasVencimiento = Convert.ToInt32(textBox11.Text),
                        MetodoPago = comboBox6.Text,
                        MedioPago = comboBox5.Text,
                        PercentICA = label24.Text,
                        PercentFUENTE = label23.Text,
                        Num_Cruce = 0
                    };

                    bool GrabarDocumento = repoFacturacion.Graba_Factura_Orden(F);
                    if (GrabarDocumento == false)
                    {
                        MessageBox.Show("No se logro grabar la factura, vuelva a intentar",
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        return;
                    }
                    else
                    {
                        foreach (CXN_CARGOS car in C)
                        {
                            CXN_CARGOS c = new CXN_CARGOS
                            {
                                Car_Adm_Id = car.Car_Adm_Id,
                                Car_Pac = car.Car_Pac,
                                Car_Cia = car.Car_Cia,
                                Car_Ase = car.Car_Ase,
                                Car_Prof = car.Car_Prof,
                                Car_Fecha = car.Car_Fecha,
                                Car_Estado = car.Car_Estado,
                                Car_Tipo = car.Car_Tipo,
                                Car_Cod = car.Car_Cod,
                                Car_Tipo_Serv = car.Car_Tipo_Serv,
                                Car_Cant = car.Car_Cant,
                                Car_Val_Un = car.Car_Val_Un,
                                Car_Val_Tot = car.Car_Val_Tot,
                                Car_Item = car.Car_Item,
                                Car_Detalle = car.Car_Detalle,
                                Car_Dx1 = car.Car_Dx1,
                                Car_Dx2 = car.Car_Dx2,
                                Car_Dx3 = car.Car_Dx3,
                                Car_Regimen = car.Car_Regimen,
                                Car_Ambito = car.Car_Ambito,
                                Car_Finalidad = car.Car_Finalidad,
                                Car_Personal = car.Car_Personal,
                                Car_CExterna  = car.Car_CExterna,
                                Car_Finalidad_CO = car.Car_Finalidad_CO,
                                Car_Imp_Dx = car.Car_Imp_Dx
                            };

                            bool inserCargo = repoCargos.InsertarCargoHistorias(c);
                        }

                        bool GrabarCargo = repoFacturacion.Actualiza_Cargo_Facturado(F, "Todo", false);
                        if (GrabarCargo == false)
                        {
                            MessageBox.Show("No se logro actualizar los cargos de la factura, esta es una incidencia grave, NO vuelva a intentar " +
                                "contacte al desarrollador inmediatamente sin cerrar esta pantalla",
                                "Error Gave",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                            return;
                        }
                        else
                        {
                            int nuevoCons = F.Fac_Num_Fac + 1;
                            bool _updateCons = repoCia.ConsecutivoActualiza(Convert.ToInt32(F.Fac_Cia), TDoc, nuevoCons);
                            if (_updateCons == false)
                            {
                                MessageBox.Show("Su documento ha sido facturado con el consecutivo " + F.Fac_Num_Fac.ToString() +
                                    " pero no se ha actualizado corretamente el siguiente numero, antes de seguir facturando reporte este " +
                                    "incidente de inmediato al desarrollador de la aplicacion",
                                    "Error grave",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                                this.Dispose();
                                this.Close();
                                return;
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(textBox12.Text))
                                {
                                    if (repoPacientes.ValidaEmail(textBox12.Text) == true)
                                    {
                                        repoPacientes.Actualiza_Email(C[0].Car_Pac, textBox12.Text);
                                    }
                                }

                                MessageBox.Show("Hecho, numero de documento: " + F.Fac_Num_Fac.ToString(),
                                                "Facturado Exitoso",
                                                MessageBoxButtons.OK,
                                                MessageBoxIcon.Exclamation);

                                string R = repoPacientes.Regimen(comboBox1.Text);
                                repoPacientes.UpdateFac2(F.Fac_Pac, textBox5.Text, R);

                                DialogResult result2 = MessageBox.Show("¿Desea Imprimir el Documento?",
                                    "Zamenis Health - Facturacion",
                                    MessageBoxButtons.YesNo,
                                    MessageBoxIcon.Question);

                                if (result2 == DialogResult.Yes)
                                {
                                    //CODIGO PARA EXPORTAR FACTURA
                                    ExportarDocumento(Convert.ToInt32(F.Fac_Num_Fac), F.Fac_Cia, F.Fac_Tipo_Doc, "");
                                }
                                if (result2 == DialogResult.No)
                                {
                                    FacturaAbierta f2 = Application.OpenForms.OfType<FacturaAbierta>().LastOrDefault();
                                    f2.Dispose();
                                    f2.Close();

                                    this.Dispose();
                                    this.Close();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void ExportarDocumento(int Documento, int Compañia, string Tipo, string TReport)
        {
            try
            {
                List<FacturasR> GenerarDocumentoGrafico = repoFacturacion.Fac_Export(Documento, Compañia, Tipo);
                if (GenerarDocumentoGrafico == null)
                {
                    MG = new MensajesGeneral();
                    MG.TipoImagen = 0;
                    MG.Mensaje = "No se logro exportar el reporte o no hay datos en estas fechas";
                    MG.ShowDialog();
                    return;
                }

                Extras.TipoReportFactura R = new Extras.TipoReportFactura(null, GenerarDocumentoGrafico, false);
                R.ShowDialog();

                FacturaAbierta f2 = Application.OpenForms.OfType<FacturaAbierta>().LastOrDefault();
                f2.Dispose();
                f2.Close();

                this.Dispose();
                this.Close();
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void label23_DoubleClick(object sender, EventArgs e)
        {
            Facturacion.Extras.Retenciones retenciones = new Facturacion.Extras.Retenciones("Fuente", "SaludAbierta");
            retenciones.ShowDialog();
        }
        private void label24_DoubleClick(object sender, EventArgs e)
        {
            Facturacion.Extras.Retenciones retenciones = new Facturacion.Extras.Retenciones("Ica", "SaludAbierta");
            retenciones.ShowDialog();
        }
    }
}
