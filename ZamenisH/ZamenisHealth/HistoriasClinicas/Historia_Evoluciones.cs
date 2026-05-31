using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Domain;
using Domain.CXN;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using ZamenisHealth.Clases;
using ZamenisHealth.Comunes;
using ZamenisHealth.HistoriasClinicas.Extras;

namespace ZamenisHealth.HistoriasClinicas
{
    public partial class Historia_Evoluciones : ConfigForm.BaseForm
    {
        private static readonly IEvolucionesFibromialgia repositorioEvoluciones = new MEvolucionesFibromialgia();
        private static readonly IAgendaC repoAgendaMedicaConsultas = new MAgendaC();
        private static readonly ICargos repositorioCargos = new MCargos();
        private static readonly IGenerales repositorioGenerales = new MGenerales();
        private static readonly IConvenios repositorioConvenios = new MConvenios();
        private static readonly IPacientes repoPacs = new MPacientes();
        private static readonly IAgenda repoAgendaMedica = new MAgenda();
        private static readonly IConvenios repoConvenios = new MConvenios();
        private static readonly IConfSystem repoConfSystem = new MConfSystem();
        private static readonly ICondiciones repoCond = new MCondiciones();

        private Extras.CondicionesP c;
        public int Admision;
        DateTime Fecha_Serv;
        int Paciente, Cia, Ase, Prof, Valor, bSession;

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            try
            {
                if (textBox7.Text == "") { MessageBox.Show("Digite numero de sesion"); return; }
                if (comboBox1.Text == "") { MessageBox.Show("Seleccione fase"); return; }
                if (textBox8.Text == "") { MessageBox.Show("Digite Hora inicial de la sesion"); return; }
                if (textBox10.Text == "") { MessageBox.Show("Seleccione codigo CIE10"); return; }
                if (textBox9.Text == "") { MessageBox.Show("Digite observacion de evolucion"); return; }
                if (textBox5.Text == "") { MessageBox.Show("No hay admision"); return; }                

                DialogResult result = MessageBox.Show("¿Desea grabar esta evolucion?",
                                                  "Zamenis Health",
                                                  MessageBoxButtons.YesNo,
                                                  MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    CXN_EVOFIB E = new CXN_EVOFIB
                    {
                        Evo_Pac = Paciente,
                        Evo_Ase = Ase,
                        Evo_Cia = Cia,
                        Evo_Med = Comunes.Contenedor.UsuarioLogueado,
                        Evo_Fecha = Fecha_Serv,
                        Evo_Edad = textBox4.Text,
                        Evo_Sesion = textBox7.Text,
                        Evo_Tipo = textBox6.Text,
                        Evo_Evo = textBox9.Text,
                        Evo_Hora = textBox8.Text,
                        Evo_Fase = comboBox1.Text,
                        Evo_Adm = Admision
                    };

                    bool insertEvo = repositorioEvoluciones.saveEvolution(E);
                    if (insertEvo != true)
                    {
                        MessageBox.Show("No se logro grabar la historia clinica, revise los datos",
                           "Incompleto",
                           MessageBoxButtons.OK,
                           MessageBoxIcon.Exclamation);
                        return;
                    }
                    else
                    {
                        CXN_CARGOS C = new CXN_CARGOS
                        {
                            Car_Adm_Id = Admision,
                            Car_Pac = Paciente,
                            Car_Cia = Cia,
                            Car_Ase = Ase,
                            Car_Prof = Prof,
                            Car_Fecha = Fecha_Serv,
                            Car_Estado = "G",
                            Car_Tipo = "Nota",
                            Car_Cod = CUP,
                            Car_Tipo_Serv = TSERV,
                            Car_Cant = 1,
                            Car_Val_Un = Valor,
                            Car_Val_Tot = Valor,
                            Car_Item = Serv,
                            Car_Detalle = textBox10.Text,
                            Car_Dx1 = textBox10.Text,
                            Car_Dx2 = "",
                            Car_Dx3 = "",
                            Car_Regimen = Reg_RIP,
                            Car_Ambito = 0,
                            Car_Finalidad = 0,
                            Car_Personal = 0,
                            Car_CExterna = 0,
                            Car_Finalidad_CO = 0,
                            Car_Imp_Dx = 0
                        };

                        bool inserCargo = repositorioCargos.InsertarCargoHistorias(C);
                        if (inserCargo != true)
                        {
                            MessageBox.Show("No se logro guardar el cargo economico en el registro de valores a cobrar en la factura",
                               "No se puede continuar",
                               MessageBoxButtons.OK,
                               MessageBoxIcon.Exclamation);
                            return;
                        }
                        else
                        {
                            repoAgendaMedica.ConsumirAdmision(Admision);

                            Medicina.RIPSHistory for_RIPS = new Medicina.RIPSHistory();
                            for_RIPS.Adm_Cargo = Convert.ToInt32(Admision);
                            for_RIPS.ShowDialog();

                            repoAgendaMedica.Graba_Hora_Salida(Admision);
                            Medicina.AgendaM f2 = Application.OpenForms.OfType<Medicina.AgendaM>().LastOrDefault();
                            f2.Cargar_Agenda();

                            this.Dispose();
                            this.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        
        private void textBox10_DoubleClick(object sender, EventArgs e)
        {
            Medicina.CIE10 C10 = new Medicina.CIE10("Evolucion");
            C10.ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            textBox10.Text = "";
            textBox11.Text = "";
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult result = MessageBox.Show("¿Desea cancelar esta historia? Al hacerlo no guardara ningun dato", "Zamenis_Health", MessageBoxButtons.YesNo);

                if (result == DialogResult.Yes)
                {
                    repoAgendaMedica.OpenAdmition(Admision, "N");
                    this.Dispose();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void Historia_Evoluciones_Load(object sender, EventArgs e)
        {
            try
            {
                Titulo.Visible = false;
                ImageClose.Visible = false;

                
                repoAgendaMedica.Graba_Hora_Atencion(Admision);
                repoAgendaMedica.OpenAdmition(Admision, "S");

                var DatosAdmision = repoAgendaMedicaConsultas.cargarAdmision(Admision, "'P','H','A'");
                if (DatosAdmision == null)
                {
                    MessageBox.Show("Error en esta admision",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    this.Dispose();
                    this.Close();
                    return;
                }                

                Paciente = DatosAdmision.Hor_Pac_Id;
                Cia = DatosAdmision.Hor_Pac_Cia;
                Ase = DatosAdmision.Hor_Pac_Ase;
                Prof = DatosAdmision.Hor_Pac_Bod;
                CUP = DatosAdmision.Hor_Pac_Cup;
                TSERV = DatosAdmision.Hor_Pac_Tipo_Serv;
                Fecha_Serv = Convert.ToDateTime(DatosAdmision.Hor_Pac_Fecha_Cita);
                Reg_RIP = DatosAdmision.Hor_Regimen;
                CMANID = DatosAdmision.Pac_IdNum.ToString();
                CMANTID = DatosAdmision.Pac_TipoId.ToString();

                var VAl = repoConvenios.ServicioNombre(CUP, Ase, TSERV);
                if (VAl == null)
                {
                    MessageBox.Show("Error grave cargardo valor del servicio, vuelva a ingresar a la admision",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    this.Dispose();
                    this.Close();
                    return;
                }

                Valor = VAl.Con_Valor;
                Serv = VAl.Con_Nombre;

                textBox5.Text = Admision.ToString();
                textBox1.Text = DatosAdmision.Hor_Imp_Age.ToString();
                textBox3.Text = Convert.ToDateTime(DatosAdmision.Pac_FechaNto).ToString(Conexion.ConectionDictionary["Format_Fecha"]);
                textBox2.Text = DatosAdmision.Pac_TipoId.ToString() + " " + DatosAdmision.Pac_IdNum.ToString();

                DateTime nacimiento = Convert.ToDateTime(DatosAdmision.Pac_FechaNto);
                int edad = DateTime.Today.AddTicks(-nacimiento.Ticks).Year - 1;
                textBox4.Text = edad.ToString();

                switch (TSERV)
                {
                    case "TO":
                        textBox6.Text = "Terapia Ocupacional";
                        break;
                    case "TF":
                        textBox6.Text = "Terapia Fisica";
                        break;
                    case "PS":
                        textBox6.Text = "Psicologia";
                        break;
                    default:
                        textBox6.Text = "Error";
                        break;
                }

                CXN_EVOFIB E = new CXN_EVOFIB
                {
                    Evo_Pac = DatosAdmision.Hor_Pac_Id,
                    Evo_Fecha = DatosAdmision.Hor_Pac_Fecha_Cita,
                    Evo_Tipo = textBox6.Text
                };

                var datosContadores = repositorioEvoluciones.getContador(E);
                if (datosContadores != null)
                {
                    bSession = Convert.ToInt32(datosContadores.Evo_Sesion) + 1;
                    bFase = datosContadores.Evo_Fase;
                }
                else
                {
                    bSession = 1;
                    bFase = "";
                }

                textBox7.Text = Convert.ToInt32(bSession).ToString();
                comboBox1.Text = bFase;

                var VAl2 = repositorioConvenios.ServicioNombre(CUP, DatosAdmision.Hor_Pac_Ase, DatosAdmision.Hor_Pac_Tipo_Serv);
                if (VAl2 != null)
                {
                    var frontServiceSugerence = repositorioGenerales.SugerenciaServicio(DatosAdmision.Hor_Pac_Id);
                    if (frontServiceSugerence != null)
                    {
                        textBox12.Text = frontServiceSugerence["Cup"];
                        textBox13.Text = frontServiceSugerence["Servicio"];
                    }
                    else
                    {
                        textBox12.Text = CUP;
                        textBox13.Text = VAl2.Con_Nombre;
                    }
                }
                else
                {
                    textBox13.Text = "Error cargando nombre del servicio";
                }

                ActualizarPAC();

                string getRecos = repoConfSystem.getListado()["Recomendaciones"];
                if (getRecos == "A")
                {
                    Extras.Recomendaciones r = new Extras.Recomendaciones(DatosAdmision.Hor_Pac_Id);
                    r.ShowDialog();

                    CargarOpcionesRecomendaciones();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        void CargarOpcionesRecomendaciones()
        {
            try
            {
                List<string> getConditions = repoCond.getCondiciones(this.Paciente);
                if (getConditions != null)
                {
                    string resultado = getConditions.Find(x => x == "CAIDA");
                    if (resultado != null)
                    {
                        CaidaTxt.BackColor = Color.DodgerBlue;
                        CaidaTxt.ForeColor = Color.Navy;
                    }
                    else
                    {
                        CaidaTxt.BackColor = Color.White;
                        CaidaTxt.ForeColor = Color.Black;
                    }

                    resultado = getConditions.Find(x => x == "INFECCION");
                    if (resultado != null)
                    {
                        infeccionTxt.BackColor = Color.LightGreen;
                        infeccionTxt.ForeColor = Color.Green;
                    }
                    else
                    {
                        infeccionTxt.BackColor = Color.White;
                        infeccionTxt.ForeColor = Color.Black;
                    }

                    resultado = getConditions.Find(x => x == "DETERIORO DE LA PIEL");
                    if (resultado != null)
                    {
                        deterioroTxt.BackColor = Color.Violet;
                        deterioroTxt.ForeColor = Color.DarkViolet;
                    }
                    else
                    {
                        deterioroTxt.BackColor = Color.White;
                        deterioroTxt.ForeColor = Color.Black;
                    }

                    resultado = getConditions.Find(x => x == "ALERGIA");
                    if (resultado != null)
                    {
                        alergiaTxt.BackColor = Color.Red;
                        alergiaTxt.ForeColor = Color.White;
                    }
                    else
                    {
                        alergiaTxt.BackColor = Color.White;
                        alergiaTxt.ForeColor = Color.Black;
                    }

                    resultado = getConditions.Find(x => x == "DIFICULTAD DE COMUNICACION");
                    if (resultado != null)
                    {
                        dificultadTxt.BackColor = Color.Yellow;
                        dificultadTxt.ForeColor = Color.Black;
                    }
                    else
                    {
                        dificultadTxt.BackColor = Color.White;
                        dificultadTxt.ForeColor = Color.Black;
                    }

                    resultado = getConditions.Find(x => x == "PACIENTE PSIQUIATRICO");
                    if (resultado != null)
                    {
                        psiquiatricoTxt.BackColor = Color.DarkViolet;
                        psiquiatricoTxt.ForeColor = Color.Thistle;
                    }
                    else
                    {
                        psiquiatricoTxt.BackColor = Color.White;
                        psiquiatricoTxt.ForeColor = Color.Black;
                    }

                    resultado = getConditions.Find(x => x == "MAYOR DE 70 AÑOS");
                    if (resultado != null)
                    {
                        mayorTxt.BackColor = Color.DarkTurquoise;
                        mayorTxt.ForeColor = Color.Blue;
                    }
                    else
                    {
                        mayorTxt.BackColor = Color.White;
                        mayorTxt.ForeColor = Color.Black;
                    }

                    resultado = getConditions.Find(x => x == "PACIENTE DIFICIL");
                    if (resultado != null)
                    {
                        dificilTxt.BackColor = Color.Orange;
                        dificilTxt.ForeColor = Color.AntiqueWhite;
                    }
                    else
                    {
                        dificilTxt.BackColor = Color.White;
                        dificilTxt.ForeColor = Color.Black;
                    }

                    resultado = getConditions.Find(x => x == "MEDICO LO REQUIERE");
                    if (resultado != null)
                    {
                        requiereTxt.BackColor = Color.Gray;
                        requiereTxt.ForeColor = Color.White;
                    }
                    else
                    {
                        requiereTxt.BackColor = Color.White;
                        requiereTxt.ForeColor = Color.Black;
                    }
                }
                else
                {
                    CaidaTxt.BackColor = Color.White;
                    CaidaTxt.ForeColor = Color.Black;
                    infeccionTxt.BackColor = Color.White;
                    infeccionTxt.ForeColor = Color.Black;
                    alergiaTxt.BackColor = Color.White;
                    alergiaTxt.ForeColor = Color.Black;
                    deterioroTxt.BackColor = Color.White;
                    deterioroTxt.ForeColor = Color.Black;
                    psiquiatricoTxt.BackColor = Color.White;
                    psiquiatricoTxt.ForeColor = Color.Black;
                    dificultadTxt.BackColor = Color.White;
                    dificultadTxt.ForeColor = Color.Black;
                    dificilTxt.BackColor = Color.White;
                    dificilTxt.ForeColor = Color.Black;
                    mayorTxt.BackColor = Color.White;
                    mayorTxt.ForeColor = Color.Black;
                    requiereTxt.BackColor = Color.White;
                    requiereTxt.ForeColor = Color.Black;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException
                {
                    FechaHora = DateTime.Now,
                    Error = ex.Message,
                    Formulario = this.Name,
                    Metodo = OverridesExtern.GetCurrentMethodName(),
                    Usuario = Contenedor.UsuarioLogueado
                };

                OverridesExtern.GenerarTXTException(T);
            }
        }

        void openConditions(string Tipo)
        {
            c = new CondicionesP(Tipo, this.Paciente);
            c.ShowDialog();

            CargarOpcionesRecomendaciones();
        }

        private void CaidaTxt_Click(object sender, EventArgs e)
        {
            openConditions("CAIDA");
        }

        private void infeccionTxt_Click(object sender, EventArgs e)
        {
            openConditions("INFECCION");
        }

        private void deterioroTxt_Click(object sender, EventArgs e)
        {
            openConditions("DETERIORO DE LA PIEL");
        }

        private void alergiaTxt_Click(object sender, EventArgs e)
        {
            Alergias c = new Alergias("ALERGIA", this.Paciente);
            c.ShowDialog();
            CargarOpcionesRecomendaciones();
        }

        private void dificultadTxt_Click(object sender, EventArgs e)
        {
            openConditions("DIFICULTAD DE COMUNICACION");
        }

        private void psiquiatricoTxt_Click(object sender, EventArgs e)
        {
            openConditions("PACIENTE PSIQUIATRICO");
        }

        private void mayorTxt_Click(object sender, EventArgs e)
        {
            openConditions("MAYOR DE 70 AÑOS");
        }

        private void dificilTxt_Click(object sender, EventArgs e)
        {
            openConditions("PACIENTE DIFICIL");
        }

        private void requiereTxt_Click(object sender, EventArgs e)
        {
            openConditions("MEDICO LO REQUIERE");
        }

        private void ActualizarPAC()
        {
            Medicina.ActualizarPaciente Historia_Edita_Paciente = new Medicina.ActualizarPaciente(Paciente);
            Historia_Edita_Paciente.ShowDialog();
        }
        

        private void textBox9_TextChanged(object sender, EventArgs e)
        {
            textBox9.CharacterCasing = CharacterCasing.Upper;
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            ActualizarPAC();
        }

        string CUP, TSERV, Reg_RIP, CMANTID, CMANID, Serv, bFase;

        public Historia_Evoluciones()
        {
            InitializeComponent();
            
            ConfigForm.MoverForma(panel3, this);
        }
    }
}
