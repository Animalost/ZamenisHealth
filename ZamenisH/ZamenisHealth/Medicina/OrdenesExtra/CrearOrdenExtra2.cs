using APIFhir.Controlador;
using APIFhir.Servicio;
using Domain;
using Domain.CXN;
using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZamenisHealth.Comunes;
using ZamenisHealth.FrontFHIR.RDAs;
using ZamenisHealth.Medicina.OrdenesHistory;

namespace ZamenisHealth.Medicina.OrdenesExtra
{
    public partial class CrearOrdenExtra2 : Forma2
    {
        private IOrdenes ordenes;
        private IFHIR oController;
        private IPacientes pacientesController;
        private IAgendaC agendaController;
        private IAgenda horarioController;
        private IConvenios conveniosController;
        private ICargos cargosController;
        private IMedicinaGeneral medicinaController;
        private IReportes reportesController;
        private ICompañia compañiaController;
        private ICIE10 cie10Controller;
        private IConfSystem confController;
        private CreateToken tokenController;

        private CXN_PACIENTES DatosPaciente = new CXN_PACIENTES();
        private otrosDatosPacienteHorario DatosCita = new otrosDatosPacienteHorario();
        private MensajesGeneral MG;
        private string TServicio;
        private int Paciente, Admision, Compañia, NumOrdenIN, NumOrdenSE;

        private List<Ordenes> ExportarOM = new List<Ordenes>();
        private List<Ordenes> ExportarIN = new List<Ordenes>();
        private List<Ordenes> ExportarSE = new List<Ordenes>();

        public CrearOrdenExtra2(string tServicio, int admision, int paciente) //Especialidad MG o FI
        {
            InitializeComponent();
            TServicio = tServicio;
            Admision = admision;
            Paciente = paciente;

            ordenes = new MOrdenes();
            oController = new MFHIR();
            pacientesController = new MPacientes();
            agendaController = new MAgendaC();
            horarioController = new MAgenda();
            conveniosController = new MConvenios();
            cargosController = new MCargos();
            medicinaController = new MMedicinaGeneral();
            reportesController = new MReportes();
            compañiaController = new MCompañia();
            cie10Controller = new MCIE10();
            confController = new MConfSystem();
            tokenController = new EndPoint_Token();

            SoloNumeros(textBox2);
            SoloNumeros(textBox13);
            SoloNumeros(textBox16);
            SoloNumeros(textBox4);
        }

        private void CrearOrdenExtra2_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Creacion de Ordenes";
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";

            DatosPaciente = pacientesController.LlamarPacientebyId(Paciente);
            DatosCita = agendaController.cargarAdmision(Admision, "'H'");
            CargarUMM();
            CargarVia();
        }

        #region MEDICAMENTOS
        private void textBox3_DoubleClick(object sender, EventArgs e)
        {
            OrdenesMedicasM2 ordenesMedicasM2 = new OrdenesMedicasM2("RECETAEXTRA");
            ordenesMedicasM2.ShowDialog();
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
        #endregion

        #region SERVICIOS
        private void textBox8_DoubleClick(object sender, EventArgs e)
        {
            ListaServicios listaServicios = new ListaServicios(this);
            listaServicios.ShowDialog();
        }
        #endregion

        private async void boton4_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult result = MessageBox.Show("¿Desea generar estas ordenes con copia al Ministerio de Salud?",
                                                 "Zamenis Health - IHCE HL7 FHIR",
                                                 MessageBoxButtons.YesNo,
                                                 MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    await AgendarCitaNueva();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        async Task AgendarCitaNueva()
        {
            try
            {
                string Modalidad_Cita = "01"; //Intramural          
                string Vales = "N";
                string Observa = "--> Cita asignada por: " + Comunes.Contenedor.UsuarioLogueado + " - ORDENES EXTRAS, CITA NO REAL PARA GENERAR ORDENES MEDICAS";

                // 1. GRABAR CITA NUEVA
                CXN_HORARIO H = new CXN_HORARIO
                {
                    Hor_Estado = "H",
                    Hor_Pac_Id = Paciente,
                    Hor_Pac_Bod = DatosCita.Hor_Pac_Bod,
                    Hor_Pac_Tipo_Serv = TServicio,
                    Hor_Pac_Cia = DatosCita.Com_Identificador,
                    Hor_Pac_Ase = DatosCita.Hor_Pac_Ase,
                    Hor_Pac_Cup = DatosCita.Hor_Pac_Cup.ToString(),
                    Hor_Pac_UsrGraba = Comunes.Contenedor.UsuarioLogueado,
                    Hor_Imp_Age = DatosPaciente.Pac_PrimerA + " " + DatosPaciente.Pac_SegundoA + " " + DatosPaciente.Pac_PrimerN + " " + DatosPaciente.Pac_SegundoN,
                    Hor_Pac_Fecha_Cita = DateTime.Now.Date,
                    Hor_Pac_Id_Hora = "ZZZZ",
                    Hor_Pac_Hora_Cita = DateTime.Now, //hora cita
                    Hor_Observacion = Observa,
                    Hor_Pac_Sal = "",
                    Hor_Vales = Vales,
                    Hor_Pac_Modalidad = Modalidad_Cita,
                    Hor_BloqEspaces = 0,
                    Hor_GrupoServicios = "01",
                    Hor_Regimen = DatosPaciente.Pac_Regimen,
                    Hor_ArrastraHistoria = "N" 
                };

                int admTemp = horarioController.AgendarPaciente(H);
                if (admTemp == 0)
                {
                    MG = new MensajesGeneral()
                    {
                        Mensaje = "No se logro generar la cita nueva",
                        TipoImagen = 1000
                    };
                    MG.ShowDialog();
                }
                else
                {
                    CXN_CONVENIOS convenio = conveniosController.ServicioNombre(H.Hor_Pac_Cup, H.Hor_Pac_Ase, TServicio);
                    string DX1 = "";
                    string DX2 = "";
                    string DX3 = "";
                    List<HCMG> getLastHistory = new List<HCMG>();

                    if (TServicio == "MG")
                    {
                        getLastHistory = reportesController.MedicinaGeneral(Admision);
                        DX1 = getLastHistory[0].DX1Code;
                        DX2 = getLastHistory[0].DX2Code;
                        DX3 = getLastHistory[0].DX3Code;
                    }            

                    // 2. GRABAR CARGO
                    CXN_CARGOS C = new CXN_CARGOS
                    {
                        Car_Adm_Id = admTemp,
                        Car_Pac = Paciente,
                        Car_Cia = H.Hor_Pac_Cia,
                        Car_Ase = H.Hor_Pac_Ase,
                        Car_Prof = H.Hor_Pac_Bod,
                        Car_Fecha = H.Hor_Pac_Fecha_Cita,
                        Car_Estado = "F",
                        Car_Tipo = "Historia",
                        Car_Cod = H.Hor_Pac_Cup,
                        Car_Val_Tot = 0,
                        Car_Val_Un = 0,
                        Car_Tipo_Serv = TServicio,
                        Car_Cant = 1,
                        Car_Detalle = TServicio == "MG" ? "Vasculares" : "",
                        Car_Item = convenio.Con_Nombre,
                        Car_Dx1 = DX1,
                        Car_Dx2 = DX2,
                        Car_Dx3 = DX3,
                        Car_Ambito = 1,
                        Car_Personal = 2,
                        Car_CExterna = 38,
                        Car_Finalidad = 2,
                        Car_Finalidad_CO = 10, //motivo                                           
                        Car_Imp_Dx = 2,
                        Car_Regimen = "01"
                    };

                    bool insertarCargo = cargosController.InsertarCargoHistorias(C);
                    if (insertarCargo == false)
                    {
                        MG = new MensajesGeneral()
                        {
                            Mensaje = "Se genero la cita nueva pero no se logro agregar el cargo",
                            TipoImagen = 1000
                        };
                        MG.ShowDialog();
                    }
                    else
                    {
                        if (TServicio == "MG")
                        {
                            CXN_HCMG His = new CXN_HCMG
                            {
                                HC_Pac = getLastHistory[0].PacienteNombre,
                                HC_Prof = C.Car_Prof,
                                HC_Ase = C.Car_Ase,
                                HC_Com = C.Car_Cia,
                                HC_Pacid = Paciente,
                                HC_Edad = getLastHistory[0].Edad,
                                HC_FechaNto = Convert.ToDateTime(DatosPaciente.Pac_FechaNto.Date),

                                HC_MotivoC = getLastHistory[0].HC_MotivoC,
                                HC_EnfA = getLastHistory[0].HC_EnfA,
                                HC_GradoC = getLastHistory[0].HC_GradoC,
                                HC_TipoLes = getLastHistory[0].HC_TipoLes,
                                HC_ActEje = getLastHistory[0].HC_ActEje,
                                HC_Vez = getLastHistory[0].HC_Vez,
                                HC_Neurologico = getLastHistory[0].HC_Neurologico,
                                HC_Cardiovascular = getLastHistory[0].HC_Cardiovascular,
                                HC_GastroIntestinal = getLastHistory[0].HC_Gastrointestinal,
                                HC_GastroUrinario = getLastHistory[0].HC_Gastrourinario,
                                HC_OsteoMuscular = getLastHistory[0].HC_Osteomuscular,
                                HC_Piel = getLastHistory[0].HC_Piel,
                                HC_Ocupacion = getLastHistory[0].Ocupacion,
                                HC_AparienciaG = getLastHistory[0].HC_AparienciaG,
                                HC_EstadoEmo = getLastHistory[0].HC_EstadoEmo,
                                HC_EstadoNut = getLastHistory[0].HC_EstadoNut,
                                HC_Exudado = getLastHistory[0].HC_Exudado,
                                HC_Presart = getLastHistory[0].HC_Presart,
                                HC_Frecar = getLastHistory[0].HC_FreCar,
                                HC_FreRes = getLastHistory[0].HC_FreRes,
                                HC_Temp = getLastHistory[0].HC_Temp,
                                HC_Peso = getLastHistory[0].HC_Peso,
                                HC_Altura = getLastHistory[0].HC_Altura,
                                HC_IMC = getLastHistory[0].HC_IMC,
                                HC_ITB = getLastHistory[0].HC_ITB,
                                HC_DescHer = getLastHistory[0].HC_DescHer,
                                HC_TejCom = getLastHistory[0].HC_TejCom,
                                HC_CaracTej = getLastHistory[0].HC_CaracTej,
                                HC_SignosInf = getLastHistory[0].HC_SignosInf,
                                HC_PielCirc = getLastHistory[0].HC_PielCirc,
                                HC_ConsCant = getLastHistory[0].HC_ConsCant,
                                HC_Estado = getLastHistory[0].HC_Estado,
                                HC_Dolor = getLastHistory[0].HC_Dolor,
                                HC_Analisis = getLastHistory[0].HC_Analisis,
                                HC_Complicacion = getLastHistory[0].HC_Complicacion,
                                HC_PruebasDiag = getLastHistory[0].HC_PruebasDiag,
                                HC_ProtoInst = "N/A",
                                HC_PManejo = getLastHistory[0].HC_PManejo,
                                HC_DX1 = DX1,
                                HC_DX2 = DX2,
                                HC_DX3 = DX3,
                                HC_DX1T = DX1,
                                HC_AntFam = getLastHistory[0].HC_AntFam,
                                HC_AntPat = getLastHistory[0].HC_AntPat,
                                HC_AntQui = getLastHistory[0].HC_AntQui,
                                HC_AntAle = getLastHistory[0].HC_AntAle,
                                HC_AntFarma = getLastHistory[0].HC_AntFarma,
                                HC_Hematolin = getLastHistory[0].HC_Hematolin,
                                HC_Patologia = getLastHistory[0].HC_Patologia,
                                HC_Fecha = DateTime.Now.Date,
                                HC_Cant = 1,
                                HC_RH = "O+",
                                HC_SubPat = "",
                                HC_Imp_Dx = "CONFIRMADO NUEVO",
                                HC_Respiratorio = getLastHistory[0].HC_Respiratorio,
                                HC_Epidemia = getLastHistory[0].Epidemia,
                                HC_ServCatalogo = getLastHistory[0].HC_ServCatalogo,
                                HC_CupCatalogo = "",
                                HC_Adm = C.Car_Adm_Id,
                                HC_TipoINGSAL = ""
                            };

                            var Graba = medicinaController.GrabaHCMG(His);
                            if (Graba == false)
                            {
                                MG = new MensajesGeneral()
                                {
                                    Mensaje = "Se genero la cita nueva y el cargo pero no se logro generar la nueva historia",
                                    TipoImagen = 1000
                                };
                                MG.ShowDialog();
                                return;
                            }
                        }     
                        else if (TServicio == "FI")
                        {
                            
                        }

                        // 3. Generar Ordenes
                        GenerarOrdenMedicamentos(C.Car_Adm_Id, DX1, DX2, DX3, getLastHistory[0].Edad);
                        GenerarOrdenIncapacidad(C.Car_Adm_Id, DX1, DX2, DX3, getLastHistory[0].Edad);
                        GenerarOrdenServicios(C.Car_Adm_Id, DX1, DX2, DX3, getLastHistory[0].Edad);

                        // 4. Generar RDA Paciente
                        await RadicarRDA(C.Car_Adm_Id);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void boton3_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(textBox8.Text) && !string.IsNullOrEmpty(textBox7.Text) &&
                    !string.IsNullOrEmpty(textBox6.Text) && textBox6.Text != "0")
                {
                    dataGridView2.Rows.Add(textBox8.Text, textBox7.Text, textBox6.Text);
                    textBox8.Text = "";
                    textBox7.Text = "";
                    textBox6.Text = "1";
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
        private void dataGridView2_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var confirm = MessageBox.Show("¿Eliminar este servicio?", "Confirmar", MessageBoxButtons.YesNo);

                if (confirm == DialogResult.Yes)
                {
                    dataGridView2.Rows.RemoveAt(e.RowIndex);
                }
            }
        }
        async Task RadicarRDA(int Admition)
        {
            try
            {
                if (confController.getListado()["IHCE"] == "A")
                {
                    // 5. Generar Token MinSalud
                    await Autoriza();

                    // 5. Radicar RDA Paciente
                    RDAPaciente RDA = new RDAPaciente();
                    RDA.EnviarRDAPaciente(Admition);

                    // 5. Radicar RDA Consulta Ambulatoria Externa
                    RDAConsultaExterna RDACE = new RDAConsultaExterna();
                    if (TServicio == "MG")
                    {
                        await RDACE.RadicarRDA(Admition, "Medicina General");
                    }
                    if (TServicio == "FI")
                    {
                        await RDACE.RadicarRDA(Admition, "Fisiatria");
                    }                    
                }                

                MG = new MensajesGeneral()
                {
                    Mensaje = "Hecho",
                    TipoImagen = 3
                };

                MG.ShowDialog();

                ExportarGenerados E = new ExportarGenerados(ExportarOM, ExportarIN, ExportarSE, TServicio, Compañia, NumOrdenIN, NumOrdenSE);
                E.ShowDialog();

                this.Dispose();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        async Task Autoriza()
        {
            try
            {
                CXN_TOKENS_FHIR getToken = oController.RecuperarClaseToken(Compañia);
                if (getToken == null)
                {
                    await tokenController.ObtenerTokenIHCE(Compañia);
                }
                else if (DateTime.Now >= getToken.Fecha.AddHours(1))
                {
                    await tokenController.ObtenerTokenIHCE(Compañia);
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        void GenerarOrdenServicios(int Admition, string DX1, string DX2, string DX3, string Edad)
        {
            try
            {
                if (dataGridView2.Rows.Cast<DataGridViewRow>().Any(r => !r.IsNewRow))
                {
                    string Especialidad = "";
                    string Planillar = "N";
                    var datosAdm = agendaController.cargarAdmision(Admition, "'P','H','A'");
                    var NumOrden = compañiaController.getPrestadorbyCode(datosAdm.Hor_Pac_Cia);

                    foreach (DataGridViewRow fila in dataGridView2.Rows)
                    {
                        if (!fila.IsNewRow)
                        {
                            string cup = fila.Cells["CUP"].Value?.ToString();
                            string servicio = fila.Cells["SERVICIO"].Value?.ToString();
                            string canti = fila.Cells["CANT"].Value?.ToString();

                            if (TServicio == "MG")
                            {
                                Especialidad = "MG";
                                Planillar = "S";
                            }
                            else if (TServicio == "FI")
                            {
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
                                OM_DX1 = DX1,
                                OM_DX2 = DX2,
                                OM_DX3 = DX3,
                                OM_DX1T = cie10Controller.BuscaDX(DX1),
                                OM_DX2T = cie10Controller.BuscaDX(DX2),
                                OM_DX3T = cie10Controller.BuscaDX(DX1),
                                OM_Edad = Edad.ToString(),
                                OM_Genero = datosAdm.Pac_Sexo == "M" ? "Masculino" : datosAdm.Pac_Sexo == "F" ? "Femenino" : "Indeterminado",
                                OM_Direccion = datosAdm.PacienteDireccion,
                                OM_Telefono = datosAdm.Pac_Telefono,
                                OM_Num = NumOrden.Com_OM,
                                OM_Firma = "SI",
                                OM_TEspecialidad = Especialidad,
                                OM_Clasificacion = "ORDEN DE SERVICIOS",
                                OM_FHIR_INC = "",
                                OM_Dias = 0,
                                OM_Planillar = Planillar
                            };

                            ordenes.CrearOrden(OM);

                            if (cup == "1005434" || cup == "1005435" || cup == "1005436" ||
                                cup == "1005434." || cup == "1005435." || cup == "1005436.")
                            {
                                cup = "869501";
                                servicio = "CURACIÓN DE LESIÓN EN PIEL O TEJIDO CELULAR SUBCUTÁNEO";
                            }

                            CXN_ORDENESFHIR cXN_ORDENESFHIR = new CXN_ORDENESFHIR
                            {
                                Admision = Admition,
                                CUP = cup,
                                Servicio = servicio,
                                Tipo = "ORDEN DE SERVICIOS",
                                Paciente = datosAdm.Hor_Pac_Id,
                                DX1 = DX1,
                                DX2 = DX2,
                                DX3 = DX3,
                                Medico = Comunes.Contenedor.UsuarioLogueado
                            };

                            ordenes.CrearOrdenFHIR(cXN_ORDENESFHIR);
                        }
                    }
                    int NuevoNumero = Convert.ToInt32(NumOrden.Com_OM) + 1;
                    bool _updateCons = compañiaController.ConsecutivoActualiza(datosAdm.Hor_Pac_Cia, "OM", NuevoNumero);

                    ExportarSE = ordenes.Generar_OrdenMedica(Convert.ToInt32(NumOrden.Com_OM), datosAdm.Hor_Pac_Cia, Comunes.Contenedor.UsuarioLogueado);
                    Compañia = datosAdm.Hor_Pac_Cia;
                    NumOrdenSE = NumOrden.Com_OM;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        void GenerarOrdenIncapacidad(int Admition, string DX1, string DX2, string DX3, string Edad)
        {
            try
            {
                if (string.IsNullOrEmpty(textBox5.Text) || string.IsNullOrEmpty(textBox4.Text) || comboBox6.Text == "")
                {
                    return;
                }
                else
                {
                    otrosDatosPacienteHorario datos = agendaController.cargarAdmision(Admition, "'H','P','A'");
                    CXN_CIA NumOrden = compañiaController.getPrestadorbyCode(datos.Hor_Pac_Cia);
                    string Especialidad = "";

                    if (TServicio == "MG")
                    {
                        Especialidad = "MG";
                    }
                    else if (TServicio == "FI")
                    {
                        Especialidad = "FI";
                    }

                    CXN_OM OM = new CXN_OM
                    {
                        OM_Pac = datos.Hor_Pac_Id,
                        OM_Ase = datos.Hor_Pac_Ase,
                        OM_Cia = datos.Com_Identificador,
                        OM_Prof = Comunes.Contenedor.UsuarioLogueado,
                        OM_Desc = textBox5.Text,
                        OM_DX1 = DX1,
                        OM_DX2 = DX2,
                        OM_DX3 = DX3,
                        OM_DX1T = cie10Controller.BuscaDX(DX1),
                        OM_DX2T = cie10Controller.BuscaDX(DX2),
                        OM_DX3T = cie10Controller.BuscaDX(DX3),
                        OM_Edad = Edad.ToString(),
                        OM_Genero = datos.Pac_Sexo == "M" ? "Masculino" : "Femenino",
                        OM_Direccion = datos.PacienteDireccion,
                        OM_Telefono = datos.Pac_Telefono,
                        OM_Num = NumOrden.Com_OM,
                        OM_Firma = "SI",
                        OM_TEspecialidad = Especialidad,
                        OM_Clasificacion = "INCAPACIDAD MEDICA",
                        OM_FHIR_INC = comboBox6.Text == "Nueva" ? "01" : "02",
                        OM_Dias = Convert.ToInt32(textBox4.Text),
                        OM_Planillar = "N",
                        
                    };

                    bool grabarOrden = ordenes.CrearOrden(OM);
                    if (grabarOrden != true)
                    {
                        return;
                    }

                    int NuevoNumero = Convert.ToInt32(OM.OM_Num) + 1;
                    bool _updateCons = compañiaController.ConsecutivoActualiza(OM.OM_Cia, "OM", NuevoNumero);

                    if (_updateCons != true)
                    {
                        return;
                    }

                    CXN_ORDENESFHIR cXN_ORDENESFHIR = new CXN_ORDENESFHIR
                    {
                        Admision = Admition,
                        CUP = textBox4.Text, //dias en este caso
                        Servicio = comboBox6.Text, //prorroga o nueva en este caso
                        Tipo = "INCAPACIDAD MEDICA",
                        Paciente = datos.Hor_Pac_Id,
                        DX1 = DX1,
                        DX2 = DX2,
                        DX3 = DX3,
                        Medico = Comunes.Contenedor.UsuarioLogueado
                    };

                    ordenes.CrearOrdenFHIR(cXN_ORDENESFHIR);
                    ExportarIN = ordenes.Generar_OrdenMedica(Convert.ToInt32(NumOrden.Com_OM), datos.Hor_Pac_Cia, Comunes.Contenedor.UsuarioLogueado);
                    Compañia = datos.Hor_Pac_Cia;
                    NumOrdenIN = NumOrden.Com_OM;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        void GenerarOrdenMedicamentos(int Admition, string DX1, string DX2, string DX3, string Edad)
        {
            try
            {
                otrosDatosPacienteHorario dataAdmi = agendaController.cargarAdmision(Admition, "'H','P','A'");
                CXN_CIA NumOrden = compañiaController.getPrestadorbyCode(dataAdmi.Hor_Pac_Cia);
                string Especialidad = "";

                if (TServicio == "MG")
                {
                    Especialidad = "MG";
                }
                else if (TServicio == "FI")
                {
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
                                OM_DX1 = DX1,
                                OM_DX2 = DX2,
                                OM_DX3 = DX3,
                                OM_DX1T = cie10Controller.BuscaDX(DX1),
                                OM_DX2T = cie10Controller.BuscaDX(DX2),
                                OM_DX3T = cie10Controller.BuscaDX(DX3),
                                OM_Edad = Edad,
                                OM_Num = NumOrden.Com_OM,
                                OM_TEspecialidad = Especialidad,
                                OM_Clasificacion = "ORDEN DE MEDICAMENTOS",
                                OM_Tipo = "M"
                            };

                            ordenes.CrearOrdenM(OM);

                            CXN_ORDENESFHIR cXN_ORDENESFHIR = new CXN_ORDENESFHIR
                            {
                                Admision = Admition,
                                CUP = "",
                                Servicio = "",
                                Tipo = "ORDEN DE MEDICAMENTOS",
                                Paciente = dataAdmi.Hor_Pac_Id,
                                DX1 = DX1,
                                DX2 = DX2,
                                DX3 = DX3,
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
                    compañiaController.ConsecutivoActualiza(dataAdmi.Hor_Pac_Cia, "OM", NuevoNumero);
                    ExportarOM = ordenes.Genera_Orden_Medicamento(Convert.ToInt32(NumOrden.Com_OM), dataAdmi.Hor_Pac_Cia, Contenedor.UsuarioLogueado);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
