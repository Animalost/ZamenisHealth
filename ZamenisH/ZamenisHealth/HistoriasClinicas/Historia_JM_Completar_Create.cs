using Domain;
using Domain.CXN;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ZamenisHealth.Clases;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.HistoriasClinicas
{
    public partial class Historia_JM_Completar_Create : ConfigForm.BaseForm
    {
        private static readonly IPacientes repoPacientes = new MPacientes();
        private static readonly IBodegas repositorioBodegas = new MBodegas();
        private static readonly IAgenda repositorioAgendar = new MAgenda();
        private static readonly ICompañia repositorioCompañias = new MCompañia();
        private static readonly IConvenios repoConv = new MConvenios();
        private static readonly IJuntas repoJuntas = new MJuntas();
        private static readonly ICargos repoCargos = new MCargos();

        private MensajesGeneral MG;
        private string Documento, Regimen;
        private int CodPac;
        private int Ase;

        public Historia_JM_Completar_Create()
        {
            InitializeComponent();

            
            ConfigForm.MoverForma(label1, this);
        }

        public Historia_JM_Completar_Create(string documento)
        {
            InitializeComponent();
            this.Documento = documento;

            
            ConfigForm.MoverForma(label1, this);
        }

        private void Historia_JM_Completar_Create_Load(object sender, EventArgs e)
        {
            try
            {
                Titulo.Visible = false;
                ImageClose.Visible = false;

                if (this.Documento != null) { textBox1.Text = this.Documento; }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Dispose();
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                CXN_PACIENTES P = repoPacientes.LlamarPacienteNumDoc(textBox1.Text);
                if (P != null) 
                {
                    label3.Text = P.Pac_PrimerA + " " + P.Pac_SegundoA + " " + P.Pac_PrimerN + " " + P.Pac_SegundoN;
                    panel1.Visible = true;
                    button1.Enabled = false;
                    textBox1.Enabled = false;
                    CodPac = P.Pac_Id;
                    Ase = P.Pac_Aseguradora;
                    Regimen = P.Pac_Regimen;
                    GetProfesionales();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        void GetProfesionales()
        {
            try
            {
                List<string> ListaProf = repositorioBodegas.Profesionales("TerapeutasFisicas");
                if (ListaProf != null)
                {
                    foreach (string i in ListaProf)
                    {
                        listBox1.Items.Add(i.ToString());
                    }

                    listBox1.ClearSelected();

                    LoadPrestadores();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        void LoadPrestadores()
        {
            try
            {
                comboBox2.DataSource = null;
                comboBox2.Items.Clear();

                List<CXN_CIA> ListaCia = repositorioCompañias.getAllCompañias();
                if (ListaCia != null)
                {
                    foreach (CXN_CIA i in ListaCia)
                    {
                        comboBox2.Items.Add(i.Com_Nombre.ToString());
                    }

                    comboBox2.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (comboBox1.SelectedIndex <= 0)
                {
                    MG = new MensajesGeneral();
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "Debe seleccionar un terapeuta de la lista";
                    MG.ShowDialog();
                    return;
                }

                if (comboBox2.Text == "")
                {
                    MG = new MensajesGeneral();
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "Debe seleccionar un prestador de servicios";
                    MG.ShowDialog();
                    return;
                }

                if (comboBox3.Text == "")
                {
                    MG = new MensajesGeneral();
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "Debe seleccionar la modalidad de atencion de la cita";
                    MG.ShowDialog();
                    return;
                }

                if (this.CodPac <= 0)
                {
                    MG = new MensajesGeneral();
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "No hay paciente seleccionado o tiene inconvenientes";
                    MG.ShowDialog();
                    return;
                }

                string Modalidad_Cita;
                switch (comboBox3.Text)
                {
                    case "Intramural":
                        Modalidad_Cita = "01";
                        break;

                    case "Extramural unidad móvil":
                        Modalidad_Cita = "02";
                        break;

                    case "Extramural domiciliaria":
                        Modalidad_Cita = "03";
                        break;

                    case "Extramural jornada de salud":
                        Modalidad_Cita = "04";
                        break;

                    case "Telemedicina interactiva":
                        Modalidad_Cita = "06";
                        break;

                    case "Telemedicina no interactiva":
                        Modalidad_Cita = "07";
                        break;

                    case "Telemedicina telexperticia":
                        Modalidad_Cita = "08";
                        break;

                    case "Telemedicina telemonitoreo":
                        Modalidad_Cita = "09";
                        break;

                    default:
                        Modalidad_Cita = "01";
                        break;
                }

                DateTime Hoy = DateTime.Now;

                CXN_HORARIO H = new CXN_HORARIO
                {
                    Hor_Estado = "H",
                    Hor_Pac_Id = this.CodPac,
                    Hor_Pac_Bod = repositorioBodegas.getDatosName(listBox1.SelectedItem.ToString()).Bod_Numero,
                    Hor_Pac_Tipo_Serv = repositorioBodegas.getDatosName(listBox1.SelectedItem.ToString()).Bod_Tipo,
                    Hor_Pac_Cia = repositorioCompañias.getPrestadorbyName(comboBox2.Text).Com_Identificador,
                    Hor_Pac_Ase = this.Ase,
                    Hor_Pac_Cup = "890505",
                    Hor_Pac_UsrGraba = Comunes.Contenedor.UsuarioLogueado,
                    Hor_Imp_Age = label3.Text,
                    Hor_Pac_Fecha_Cita = Convert.ToDateTime(dateTimePicker1.Value.Date),
                    Hor_Pac_Id_Hora = "XXXX",
                    Hor_Observacion = "JUNTA MEDICA ASIGNADA POR: " + Comunes.Contenedor.UsuarioLogueado,
                    Hor_Pac_Sal = "",
                    Hor_Vales = "N",
                    Hor_Pac_Modalidad = Modalidad_Cita,
                    Hor_Pac_Hora_Cita = Convert.ToDateTime(dateTimePicker2.Value), //hora cita
                    Hor_GrupoServicios = "01",
                    
                    Hor_Regimen = this.Regimen,
                    Hor_Pac_Llegada = Convert.ToDateTime(Hoy),
                    Hor_Pac_Atendido = Convert.ToDateTime(Hoy),
                    Hor_Valida = "Validado en Validador de Derechos",
                    Hor_Usr_Admisiona = Contenedor.UsuarioLogueado,
                    Hor_CantSesion = 0,
                    Hor_IniciaSesion = "N",
                    Hor_AdmOpnened = "S"
                };


                int getKey = repositorioAgendar.AgendarPacienteJuntas(H);
                if (getKey >= 1)
                {
                    GrabarJunta(getKey);
                }
                else
                {
                    MG = new MensajesGeneral();
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "No fue posible asignar esta junta medica, contacte al administrador";
                    MG.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        void GrabarJunta(int KeyAgenda)
        {
            try
            {
                CXN_HCJUNTAS HC = new CXN_HCJUNTAS
                {
                    Jun_Pac = this.CodPac,
                    Jun_Cia = repositorioCompañias.getPrestadorbyName(comboBox2.Text).Com_Identificador,
                    Jun_Fecha = Convert.ToDateTime(dateTimePicker1.Value.Date),
                    Jun_Bodega = repositorioBodegas.getDatosName(listBox1.SelectedItem.ToString()).Bod_Numero,
                    Jun_Fisiatra = "",
                    Jun_Psicologo = "",
                    Jun_TF = repositorioBodegas.getDatosName(listBox1.SelectedItem.ToString()).Bod_Responsable,
                    Jun_TO = "",
                    Jun_Adm = KeyAgenda,
                    Jun_Ase = this.Ase,
                    Jun_DX_FI = "",
                    Jun_DX_TF = "",
                    Jun_DX_TO = "",
                    Jun_DX_PS = "",
                    Jun_Pro_FI = "N/A",
                    Jun_Con_FI = "",
                    Jun_Con_TF = "",
                    Jun_Con_TO = "",
                    Jun_Con_PS = "",
                    Jun_Observa = "",
                    Jun_Prox_Cita = dateTimePicker1.Value,
                    Jun_CIE10 = "",
                    Jun_Cant = 1,
                    Jun_Edad = ""
                };

                if (comboBox1.SelectedIndex == 1)
                {
                    HC.Jun_Pro_TF = "";
                    HC.Jun_Pro_TO = "";
                    HC.Jun_Pro_PS = "";
                    HC.Jun_Tipo = "Junta1";
                }

                if (comboBox1.SelectedIndex == 2)
                {
                    HC.Jun_Pro_TF = "";
                    HC.Jun_Pro_TO = "";
                    HC.Jun_Pro_PS = "";
                    HC.Jun_Tipo = "Junta2";
                }

                bool grabarJunta = repoJuntas.InsertarJunta(HC);
                if (grabarJunta == true)
                {
                    var ambito = 0;
                    var personal = 0;
                    var causaexterna = 0;
                    var finalidad = 0;
                    var motivo = 0;
                    var impresion = 0;

                    CXN_CARGOS C = new CXN_CARGOS
                    {
                        Car_Adm_Id = KeyAgenda,
                        Car_Pac = this.CodPac,
                        Car_Cia = repositorioCompañias.getPrestadorbyName(comboBox2.Text).Com_Identificador,
                        Car_Ase = this.Ase,
                        Car_Prof = repositorioBodegas.getDatosName(listBox1.SelectedItem.ToString()).Bod_Numero,
                        Car_Fecha = Convert.ToDateTime(dateTimePicker1.Value.Date),
                        Car_Estado = "G",
                        Car_Tipo = "Historia",
                        Car_Cod = "890505",
                        Car_Val_Tot = repoConv.ServicioNombre("890505", this.Ase, "TF").Con_Valor,
                        Car_Val_Un = repoConv.ServicioNombre("890505", this.Ase, "TF").Con_Valor,
                        Car_Tipo_Serv = "TF",
                        Car_Cant = 1,
                        Car_Detalle = "",
                        Car_Item = "DISCUSION JUNTA MEDICA",
                        Car_Dx1 = "",
                        Car_Dx2 = "",
                        Car_Dx3 = "",
                        Car_Ambito = ambito,
                        Car_Personal = personal,
                        Car_CExterna = causaexterna,
                        Car_Finalidad = finalidad,
                        Car_Finalidad_CO = motivo, //motivo                                           
                        Car_Imp_Dx = impresion,
                        Car_Regimen = "0"
                    };

                    bool insertarCargo = repoCargos.InsertarCargoHistorias(C);
                    if (insertarCargo == false)
                    {
                        MessageBox.Show("No se logro guardar el cargo economico en el registro de valores a cobrar en la factura, " +
                            "su historia quedo resgitrada pero reporte este incidente a la recepcion con la admision: " + KeyAgenda.ToString(),
                            "Advertencia!!!",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }

                    MG = new MensajesGeneral();
                    MG.TipoImagen = 3;
                    MG.Mensaje = "Junta Creada con el numero de Admision " + KeyAgenda.ToString();
                    MG.ShowDialog();

                    this.Dispose();
                    this.Close();

                }
                else
                {
                    MessageBox.Show("No se logro guardar la historia clinica, revise la informacion y verifique que todos los datos estan completos, " +
                           "No se ha logrado continuar",
                           "Advertencia!!!",
                           MessageBoxButtons.OK,
                           MessageBoxIcon.Information);
                    return;
                }                
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
    }
}
