using Domain;
using Domain.CXN;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZamenisHealth.Clases;
using ZamenisHealth.Comunes;
using ZamenisHealth.HistoriasClinicas.Extras;

namespace ZamenisHealth.HistoriasClinicas
{
    public partial class Historia_JefeEnfermeria : ConfigForm.BaseForm
    {
        private static readonly IAseguradoras repoAseguradoras = new MAseguradoras();
        private static readonly IAgendaC repoAgendaMedicaConsultas = new MAgendaC();
        private static readonly IAgenda repoAgendaMedica = new MAgenda();
        private static readonly IConvenios repoConvenios = new MConvenios();
        private static readonly INotasCuracion repoNotasCuracion = new MNotasCuracion();
        private static readonly ICargos repoCargos = new MCargos();
        private static readonly ICIE10 repoCIE10 = new MCIE10();
        private static readonly IRIPS repoRIPS = new MRIPS();
        private static readonly ICondiciones repoCond = new MCondiciones();
        private static readonly IConfSystem repoConfSystem = new MConfSystem();

        private Extras.CondicionesP c;
        public int Admision;
        DateTime Fecha_Serv;
        int Paciente, Cia, Ase, Prof, Valor;

        private void toolStripButton8_Click(object sender, EventArgs e)
        {
            try
            {
                Task.Run(() =>
                {
                    this.Invoke((Action)(() =>
                    {
                        Medicina.ResumenHCMGNotas historia_MG_Resumen = new Medicina.ResumenHCMGNotas();
                        historia_MG_Resumen.Paci_Resumen_His = Paciente;
                        historia_MG_Resumen.Show();
                    }));
                });         
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            Medicina.Historial_Medico_1 RM = new Medicina.Historial_Medico_1();
            RM.ShowDialog();
        }

        private void toolStripButton10_Click(object sender, EventArgs e)
        {
            ActualizarPAC();
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

        private void textBox6_Click(object sender, EventArgs e)
        {
            try
            {
                Medicina.CIE10 CIE10_Historias = new Medicina.CIE10("DX1_NotaEJ");
                CIE10_Historias.ShowDialog();
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            try
            {
                if (richTextBox1.Text == "") { MessageBox.Show("Debe diligenciar un texto claro como control medico"); return; }
                if (textBox6.Text == "") { MessageBox.Show("Debe diligenciar el DX1, haga click en el recuadro gris del DX1 y seleccione un diagnostico"); return; }

                DialogResult result = MessageBox.Show("Una vez guardada esta estadistica, no se podran deshacer cambios. ¿Realmente desea guardar?",
                                                 "Zamenis Health - Enfermeria Estadisticas",
                                                 MessageBoxButtons.YesNo,
                                                 MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    var impresion = repoRIPS.TipoRipCargo(textBox12.Text, "IMPDX");

                    CXN_CARGOS C = new CXN_CARGOS
                    {
                        Car_Adm_Id = Admision,
                        Car_Pac = Paciente,
                        Car_Cia = Cia,
                        Car_Ase = Ase,
                        Car_Prof = Prof,
                        Car_Fecha = Fecha_Serv,
                        Car_Estado = "G",
                        Car_Tipo = "Historia",
                        Car_Cod = CUP,
                        Car_Tipo_Serv = TSERV,
                        Car_Cant = 1,
                        Car_Val_Un = Valor,
                        Car_Val_Tot = Valor,
                        Car_Item = Serv,
                        Car_Detalle = textBox13.Text,
                        Car_Dx1 = textBox6.Text,
                        Car_Dx2 = textBox8.Text,
                        Car_Dx3 = textBox9.Text,
                        Car_Regimen = Reg_RIP,
                        Car_Ambito = 0,
                        Car_Finalidad = 0,
                        Car_Personal = 0,
                        Car_CExterna = 0,
                        Car_Finalidad_CO = 0,
                        Car_Imp_Dx = impresion
                    };

                    bool inserCargo = repoCargos.InsertarCargoHistorias(C);
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
                        CXN_NOTAS N = new CXN_NOTAS
                        {
                            Not_Pac = Paciente,
                            Not_Med = Prof,
                            Not_Ase = Ase,
                            Not_Fecha = Fecha_Serv,
                            Not_Cia = Cia,
                            Not_Edad = textBox4.Text,
                            Not_Nota = richTextBox1.Text,
                            Not_Recomienda = "CONTROL ENFERMERA JEFE",
                            Not_Observa = "CONTROL ENFERMERA JEFE",
                            Not_Cup = CUP,
                            Not_Cant = 1,
                            Not_Patologia = textBox13.Text,
                            Not_Adm = Admision,
                            Not_Epidemia = "CONTROL ENFERMERA JEFE",
                            Not_CaracTej = "",
                            Not_Adherencia = richTextBox1.Text,
                        };

                        bool insertNota = repoNotasCuracion.GrabarNota(N);
                        if (inserCargo != true)
                        {
                            MessageBox.Show("No se logro guardar la nota de enfermeria, revise la informacion y vuelva a intentar, " +
                                          "si sigue presentando este inconveniente contacte al administrador del sistema",
                                          "No se puede continuar",
                                          MessageBoxButtons.OK,
                                          MessageBoxIcon.Exclamation);
                            return;
                        }
                        else
                        {
                            repoAgendaMedica.ConsumirAdmision(Admision);
                            repoAgendaMedica.Graba_Hora_Salida(Admision);

                            MessageBox.Show("Grabado exitosamente",
                                            "Hecho",
                                             MessageBoxButtons.OK,
                                             MessageBoxIcon.Information);

                            Medicina.RIPSHistory for_RIPS = new Medicina.RIPSHistory();
                            for_RIPS.Adm_Cargo = Convert.ToInt32(Admision);
                            for_RIPS.ShowDialog();

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
       
        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult result = MessageBox.Show("¿Desea salir de esta nota de curacion? si sale de esta nota ningun dato " +
                                                      "quedara grabado",
                                                      "Zamenis Health - Cancelar Curacion!!!!",
                                                 MessageBoxButtons.YesNo,
                                                 MessageBoxIcon.Question);

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

        private void Historia_JefeEnfermeria_Load(object sender, EventArgs e)
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

                var As = repoAseguradoras.getInfoFromAsebyCode(Ase);
                textBox17.Text = As.Ase_Descripcion.ToString();

                Valor = VAl.Con_Valor;
                Serv = VAl.Con_Nombre;

                textBox1.Text = Admision.ToString();
                textBox2.Text = DatosAdmision.Hor_Imp_Age.ToString();
                textBox3.Text = Convert.ToDateTime(DatosAdmision.Pac_FechaNto).ToString(Conexion.ConectionDictionary["Format_Fecha"]);
                textBox5.Text = DatosAdmision.Pac_TipoId.ToString() + " " + DatosAdmision.Pac_IdNum.ToString();

                DateTime nacimiento = Convert.ToDateTime(DatosAdmision.Pac_FechaNto);
                int edad = DateTime.Today.AddTicks(-nacimiento.Ticks).Year - 1;
                textBox4.Text = edad.ToString();

                Diagnosticos(DatosAdmision.Hor_Pac_Id);
                var DX = repoCIE10.BuscaDX(textBox6.Text);
                textBox7.Text = DX.ToString();
                DX = repoCIE10.BuscaDX(textBox8.Text);
                textBox10.Text = DX.ToString();
                DX = repoCIE10.BuscaDX(textBox9.Text);
                textBox11.Text = DX.ToString();

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
        private void ActualizarPAC()
        {
            Medicina.ActualizarPaciente Historia_Edita_Paciente = new Medicina.ActualizarPaciente(Paciente);
            Historia_Edita_Paciente.ShowDialog();
        }        

        private void Diagnosticos(int Paciente)
        {
            try
            {
                var getDX = repoNotasCuracion.Diagnosticos(Paciente);
                if (getDX != null)
                {
                    textBox6.Text = getDX["DX1"];
                    textBox8.Text = getDX["DX2"];
                    textBox9.Text = getDX["DX3"];
                    textBox12.Text = getDX["Impresion"];
                    textBox13.Text = getDX["Patologia"];
                }
                else
                {
                    textBox6.Text = "";
                    textBox8.Text = "";
                    textBox9.Text = "";
                    textBox12.Text = "";
                    textBox13.Text = "N/A";
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
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

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            Medicina.CambioManejoEnfermero CME = new Medicina.CambioManejoEnfermero();
            CME.CargarDocumentos();
            CME.comboBox1.Text = CMANTID;
            CME.textBox1.Text = CMANID;
            CME.comboBox1.Enabled = false;
            CME.textBox1.Enabled = false;
            CME.ShowDialog();
        }

        private void toolStripButton12_Click(object sender, EventArgs e)
        {
            Comunes.MensajeroSend S = new Comunes.MensajeroSend();
            S.ShowDialog();
        }

        string CUP, TSERV, Reg_RIP, CMANTID, CMANID, Serv;
        public Historia_JefeEnfermeria()
        {
            InitializeComponent();
            
            ConfigForm.MoverForma(TittleLbl, this);
        }
    }
}
