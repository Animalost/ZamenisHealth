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
    public partial class Historia_Psicologia : ConfigForm.BaseForm
    {
        private static readonly IAgendaC repoAgendaMedicaConsultas = new MAgendaC();
        private static readonly ICondiciones repoCond = new MCondiciones();
        private static readonly IAgenda repositorioAgendaMedica = new MAgenda();
        private static readonly IPsicologia repositorioPsicologia = new MPsicologia();
        private static readonly IConvenios repositorioConvenios = new MConvenios();
        private static readonly ICargos repositorioCargos = new MCargos();
        private static readonly IRIPS repositorioRIPS = new MRIPS();
        private static readonly IConfSystem repoConfSystem = new MConfSystem();

        private Extras.CondicionesP c;
        public int Admision;
        DateTime Fecha_Serv;
        int Paciente, Cia, Ase, Prof, Valor;

        private void button2_Click(object sender, EventArgs e)
        {
            textBox33.Text = "";
            textBox32.Text = "";
        }

        private void textBox37_DoubleClick(object sender, EventArgs e)
        {
            Medicina.CIE10 C10 = new Medicina.CIE10("PS1");
            C10.ShowDialog();
        }

        private void textBox35_DoubleClick(object sender, EventArgs e)
        {
            Medicina.CIE10 C10 = new Medicina.CIE10("PS2");
            C10.ShowDialog();
        }

        private void textBox33_DoubleClick(object sender, EventArgs e)
        {
            Medicina.CIE10 C10 = new Medicina.CIE10("PS3");
            C10.ShowDialog();
        }

        private void toolStripLabel2_Click(object sender, EventArgs e)
        {
            try
            {
                var getlast = repositorioPsicologia.getLastHistory(Paciente);
                if (getlast != null)
                {
                    textBox7.Text = getlast.HC_Patologico.ToString();
                    textBox8.Text = getlast.HC_Quirurgico.ToString();
                    textBox9.Text = getlast.HC_Farmacologico.ToString();
                    textBox10.Text = getlast.HC_Fracturas.ToString();
                    textBox11.Text = getlast.HC_Tox_Ale.ToString();
                    textBox12.Text = getlast.HC_Familiares.ToString();
                    textBox13.Text = getlast.HC_Psicologicos.ToString();
                    textBox14.Text = getlast.HC_Otros.ToString();
                    textBox15.Text = getlast.HC_Esc_Dol.ToString();
                    textBox16.Text = getlast.HC_Caracteristica.ToString();
                    textBox17.Text = getlast.HC_TEvolucion.ToString();
                    textBox18.Text = getlast.HC_Fisiatra.ToString();
                    textBox19.Text = getlast.HC_Psiquiatra.ToString();
                    textBox20.Text = getlast.HC_Remite.ToString();
                    comboBox1.Text = getlast.HC_RedApSoc.ToString();
                    comboBox2.Text = getlast.HC_FSoporte.ToString();
                    comboBox3.Text = getlast.HC_TFamilia.ToString();
                    comboBox4.Text = getlast.HC_TRelacion.ToString();
                    comboBox5.Text = getlast.HC_Estado.ToString();
                    comboBox6.Text = getlast.HC_TRelacion_2.ToString();
                    textBox21.Text = getlast.HC_OtroSoporte.ToString();
                    textBox22.Text = getlast.HC_ObSoporte.ToString();
                    textBox23.Text = getlast.HC_Sust.ToString();
                    textBox24.Text = getlast.HC_Compo.ToString();
                    textBox25.Text = getlast.HC_EstadoOtro.ToString();
                    textBox26.Text = getlast.HC_TiRelacion.ToString();
                    textBox27.Text = getlast.HC_Sust_2.ToString();
                    comboBox7.Text = getlast.HC_EstSex.ToString();
                    comboBox8.Text = getlast.HC_DolMol.ToString();
                    comboBox9.Text = getlast.HC_AutoEsq.ToString();
                    textBox28.Text = getlast.HC_Satis.ToString();
                    textBox29.Text = getlast.HC_ObservSex.ToString();
                    textBox30.Text = getlast.HC_ObseAuto.ToString();
                    textBox31.Text = getlast.HC_Suicida.ToString();
                    comboBox10.Text = getlast.HC_LabEst.ToString();
                    comboBox11.Text = getlast.HC_ApGen_1.ToString();
                    comboBox12.Text = getlast.HC_ApGen_2.ToString();
                    comboBox13.Text = getlast.HC_ApGen_3.ToString();
                    comboBox14.Text = getlast.HC_Cons.ToString();
                    comboBox15.Text = getlast.HC_Aten_1.ToString();
                    comboBox16.Text = getlast.HC_Aten_2.ToString();
                    textBox40.Text = getlast.HC_TiLab.ToString();
                    textBox41.Text = getlast.HC_ObLav.ToString();
                    textBox42.Text = getlast.HC_ObCons.ToString();
                    textBox43.Text = getlast.HC_ObAten.ToString();
                    comboBox17.Text = getlast.HC_Sue_1.ToString();
                    comboBox18.Text = getlast.HC_Sue_2.ToString();
                    comboBox19.Text = getlast.HC_Sue_4.ToString();
                    comboBox20.Text = getlast.HC_Sue_3.ToString();
                    textBox44.Text = getlast.HC_Orien_1.ToString();
                    textBox45.Text = getlast.HC_ObOrien.ToString();
                    textBox46.Text = getlast.HC_ObSue.ToString();
                    textBox47.Text = getlast.HC_ObSue_2.ToString();
                    textBox48.Text = getlast.HC_Riesgo.ToString();
                    textBox49.Text = getlast.HC_FacPro.ToString();
                    textBox50.Text = getlast.HC_ImpDiag.ToString();
                    textBox51.Text = getlast.HC_Pronostico.ToString();
                    textBox38.Text = getlast.HC_Paccion.ToString();
                    textBox39.Text = getlast.HC_Recomienda.ToString();
                    textBox37.Text = getlast.HC_CIE10.ToString();
                    textBox35.Text = getlast.HC_CIE10_2.ToString();
                    textBox33.Text = getlast.HC_CIE10_3.ToString();
                    textBox52.Text = getlast.HC_Ocupacion.ToString();
                    MessageBox.Show("Se ha recuperado el ultimo historial del paciente de la fecha: " + Convert.ToDateTime(getlast.HC_Fecha).ToString(Conexion.ConectionDictionary["Format_Fecha"]));
                }
                else
                {
                    MessageBox.Show("No hay antecedentes de este paciente");
                    return;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void toolStripLabel1_Click(object sender, EventArgs e)
        {
            try
            {
                if (textBox7.Text == "") { MessageBox.Show("Revise Antecedentes 1"); return; }
                if (textBox8.Text == "") { MessageBox.Show("Revise Antecedentes 1"); return; }
                if (textBox9.Text == "") { MessageBox.Show("Revise Antecedentes 1"); return; }
                if (textBox10.Text == "") { MessageBox.Show("Revise Antecedentes 2"); return; }
                if (textBox11.Text == "") { MessageBox.Show("Revise Antecedentes 2"); return; }
                if (textBox12.Text == "") { MessageBox.Show("Revise Antecedentes 2"); return; }
                if (textBox13.Text == "") { MessageBox.Show("Revise Antecedentes 3"); return; }
                if (textBox14.Text == "") { MessageBox.Show("Revise Antecedentes 3"); return; }
                if (textBox15.Text == "") { MessageBox.Show("Revise Antecedentes 3"); return; }
                if (textBox16.Text == "") { MessageBox.Show("Revise datos de ingreso"); return; }
                if (textBox17.Text == "") { MessageBox.Show("Revise datos de ingreso"); return; }
                if (textBox18.Text == "") { MessageBox.Show("Revise datos de ingreso"); return; }
                if (textBox19.Text == "") { MessageBox.Show("Revise datos de ingreso"); return; }
                if (textBox20.Text == "") { MessageBox.Show("Revise datos de ingreso"); return; }
                if (comboBox1.Text == "") { MessageBox.Show("Revise Areas de Ajuste"); return; }
                if (comboBox2.Text == "") { MessageBox.Show("Revise Areas de Ajuste"); return; }
                if (comboBox3.Text == "") { MessageBox.Show("Revise Areas de Ajuste"); return; }
                if (comboBox4.Text == "") { MessageBox.Show("Revise Areas de Ajuste"); return; }
                if (comboBox5.Text == "") { MessageBox.Show("Revise Areas de Ajuste"); return; }
                if (comboBox6.Text == "") { MessageBox.Show("Revise Areas de Ajuste"); return; }
                if (comboBox7.Text == "") { MessageBox.Show("Revise Areas de Ajuste 2"); return; }
                if (comboBox8.Text == "") { MessageBox.Show("Revise Areas de Ajuste 2"); return; }
                if (comboBox9.Text == "") { MessageBox.Show("Revise Areas de Ajuste 2"); return; }
                if (textBox31.Text == "") { MessageBox.Show("Revise Areas de Ajuste 2"); return; }
                if (comboBox10.Text == "") { MessageBox.Show("Revise Areas de Ajuste 3"); return; }
                if (comboBox11.Text == "") { MessageBox.Show("Revise Areas de Ajuste 3"); return; }
                if (comboBox12.Text == "") { MessageBox.Show("Revise Areas de Ajuste 3"); return; }
                if (comboBox13.Text == "") { MessageBox.Show("Revise Areas de Ajuste 3"); return; }
                if (comboBox14.Text == "") { MessageBox.Show("Revise Areas de Ajuste 3"); return; }
                if (comboBox15.Text == "") { MessageBox.Show("Revise Areas de Ajuste 3"); return; }
                if (comboBox16.Text == "") { MessageBox.Show("Revise Areas de Ajuste 3"); return; }
                if (textBox44.Text == "") { MessageBox.Show("Revise Areas de Ajuste 4"); return; }
                if (textBox45.Text == "") { MessageBox.Show("Revise Areas de Ajuste 4"); return; }
                if (comboBox17.Text == "") { MessageBox.Show("Revise Areas de Ajuste 4"); return; }
                if (comboBox18.Text == "") { MessageBox.Show("Revise Areas de Ajuste 4"); return; }
                if (comboBox19.Text == "") { MessageBox.Show("Revise Areas de Ajuste 4"); return; }
                if (comboBox20.Text == "") { MessageBox.Show("Revise Areas de Ajuste 4"); return; }
                if (textBox48.Text == "") { MessageBox.Show("Revise Factores"); return; }
                if (textBox49.Text == "") { MessageBox.Show("Revise Factores"); return; }
                if (textBox50.Text == "") { MessageBox.Show("Revise Factores"); return; }
                if (textBox51.Text == "") { MessageBox.Show("Revise Factores"); return; }
                if (textBox37.Text == "") { MessageBox.Show("Revise Diagnostico Principal"); return; }
                if (textBox38.Text == "") { MessageBox.Show("Revise pestaña diagnosticos"); return; }
                if (textBox39.Text == "") { MessageBox.Show("Revise pestaña diagnosticos"); return; }
                if (textBox52.Text == "") { MessageBox.Show("Revise Anteccedentes 3"); return; }

                DialogResult result = MessageBox.Show("Desea finalizar esta historia?", "Zamenis Health - Cerrar Historias", MessageBoxButtons.YesNo);

                if (result == DialogResult.Yes)
                {
                    CXN_HCPSI H = new CXN_HCPSI
                    {
                        HC_Pac = textBox1.Text,
                        HC_Cant = 1,
                        HC_Prof = Prof,
                        HC_Ase = Ase,
                        HC_Cia = Cia,
                        HC_Fecha = Fecha_Serv,
                        HC_Edad = textBox4.Text,
                        HC_Pac_id = Paciente,
                        HC_Adm = Admision,
                        HC_Ocupacion = textBox52.Text,
                        HC_CIE10 = textBox37.Text,
                        HC_CIE10_2 = textBox35.Text,
                        HC_CIE10_3 = textBox33.Text,
                        HC_Patologico = textBox7.Text,
                        HC_Quirurgico = textBox8.Text,
                        HC_Farmacologico = textBox9.Text,
                        HC_Fracturas = textBox10.Text,
                        HC_Tox_Ale = textBox11.Text,
                        HC_Familiares = textBox12.Text,
                        HC_Psicologicos = textBox13.Text,
                        HC_Otros = textBox14.Text,
                        HC_Esc_Dol = textBox15.Text,
                        HC_Caracteristica = textBox16.Text,
                        HC_TEvolucion = textBox17.Text,
                        HC_Fisiatra = textBox18.Text,
                        HC_Psiquiatra = textBox19.Text,
                        HC_Remite = textBox20.Text,
                        HC_RedApSoc = comboBox1.Text,
                        HC_FSoporte = comboBox2.Text,
                        HC_ObSoporte = textBox22.Text,
                        HC_OtroSoporte = textBox21.Text,
                        HC_TFamilia = comboBox3.Text,
                        HC_TRelacion = comboBox4.Text,
                        HC_Sust = textBox23.Text,
                        HC_Compo = textBox24.Text,
                        HC_Estado = comboBox5.Text,
                        HC_EstadoOtro = textBox25.Text,
                        HC_TRelacion_2 = comboBox6.Text,
                        HC_TiRelacion = textBox26.Text,
                        HC_Sust_2 = textBox27.Text,
                        HC_EstSex = comboBox7.Text,
                        HC_Satis = textBox28.Text,
                        HC_DolMol = comboBox8.Text,
                        HC_ObservSex = textBox29.Text,
                        HC_AutoEsq = comboBox9.Text,
                        HC_ObseAuto = textBox30.Text,
                        HC_Suicida = textBox31.Text,
                        HC_LabEst = comboBox10.Text,
                        HC_TiLab = textBox40.Text,
                        HC_ObLav = textBox41.Text,
                        HC_ApGen_1 = comboBox11.Text,
                        HC_ApGen_2 = comboBox12.Text,
                        HC_ApGen_3 = comboBox13.Text,
                        HC_Cons = comboBox14.Text,
                        HC_ObCons = textBox42.Text,
                        HC_Aten_1 = comboBox15.Text,
                        HC_Aten_2 = comboBox16.Text,
                        HC_ObAten = textBox43.Text,
                        HC_Orien_1 = textBox44.Text,
                        HC_ObOrien = textBox45.Text,
                        HC_Sue_1 = comboBox17.Text,
                        HC_Sue_2 = comboBox18.Text,
                        HC_Sue_3 = comboBox20.Text,
                        HC_Sue_4 = comboBox19.Text,
                        HC_ObSue = textBox46.Text,
                        HC_ObSue_2 = textBox47.Text,
                        HC_Riesgo = textBox47.Text,
                        HC_FacPro = textBox49.Text,
                        HC_ImpDiag = textBox50.Text,
                        HC_Pronostico = textBox51.Text,
                        HC_Paccion = textBox38.Text,
                        HC_Recomienda = textBox39.Text
                    };

                    bool saveH = repositorioPsicologia.saveHistory(H);
                    if (saveH != true)
                    {
                        MessageBox.Show("No se logro guardar la historia", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    else
                    {
                        var impresion = repositorioRIPS.TipoRipCargo(comboBox139.Text, "IMPDX");

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
                            Car_Val_Tot = Valor,
                            Car_Val_Un = Valor,
                            Car_Tipo_Serv = TSERV,
                            Car_Cant = 1,
                            Car_Detalle = textBox37.Text,
                            Car_Item = Serv,
                            Car_Dx1 = textBox37.Text,
                            Car_Dx2 = textBox35.Text,
                            Car_Dx3 = textBox33.Text,
                            Car_Ambito = 0,
                            Car_Personal = 0,
                            Car_CExterna = 0,
                            Car_Finalidad = 0,
                            Car_Finalidad_CO = 0, //motivo                                           
                            Car_Imp_Dx = impresion,
                            Car_Regimen = Reg_RIP
                        };

                        bool insertarCargo = repositorioCargos.InsertarCargoHistorias(C);
                        if (insertarCargo == false)
                        {
                            MessageBox.Show("No se logro guardar el cargo economico en el registro de valores a cobrar en la factura, " +
                                "su historia quedo resgitrada pero reporte este incidente a la recepcion con la admision: " + Admision,
                                "Advertencia!!!",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                        }
                    }

                    repositorioAgendaMedica.ConsumirAdmision(Admision);

                    Medicina.RIPSHistory for_RIPS = new Medicina.RIPSHistory();
                    for_RIPS.Adm_Cargo = Convert.ToInt32(Admision);
                    for_RIPS.ShowDialog();

                    Medicina.AgendaM f2 = Application.OpenForms.OfType<Medicina.AgendaM>().LastOrDefault();
                    f2.Cargar_Agenda();

                    repositorioAgendaMedica.Graba_Hora_Salida(Admision);

                    this.Dispose();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
       
        private void toolStripLabel6_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult result = MessageBox.Show("Desea cancelar esta historia? Al hacerlo no guardara ningun dato", "Zamenis_Health", MessageBoxButtons.YesNo);

                if (result == DialogResult.Yes)
                {
                    repositorioAgendaMedica.OpenAdmition(Admision, "N");
                    this.Dispose();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void toolStripLabel4_Click(object sender, EventArgs e)
        {
            Medicina.Historial_Medico_1 RM = new Medicina.Historial_Medico_1();
            RM.ShowDialog();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            ActualizarPAC();
        }

        private void Historia_Psicologia_Load(object sender, EventArgs e)
        {
            try
            {
                Titulo.Visible = false;
                ImageClose.Visible = false;

                
                repositorioAgendaMedica.Graba_Hora_Atencion(Admision);
                repositorioAgendaMedica.OpenAdmition(Admision, "S");

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

                var VAl = repositorioConvenios.ServicioNombre(CUP, Ase, TSERV);
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

                textBox6.Text = "Psicologia";

                ActualizarPAC();
                VideoLlamada();

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

        void VideoLlamada()
        {
            try
            {
                if (Conexion.ConectionDictionary["Videoconferencia"] == "A")
                {
                    if (repoConfSystem.getListado()["VideoconferenciaTwilio"] == "A")
                    {
                        toolStripButton2.Visible = true;
                    }
                    else
                    {
                        toolStripButton2.Visible = false;
                    }
                }   
                else
                {
                    toolStripButton2.Visible = false;
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

        private void ActualizarPAC()
        {
            Medicina.ActualizarPaciente Historia_Edita_Paciente = new Medicina.ActualizarPaciente(Paciente);
            Historia_Edita_Paciente.ShowDialog();
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

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            Medicina.DocumentosWEB.MenuDocWeb menuDocWeb = new Medicina.DocumentosWEB.MenuDocWeb();
            menuDocWeb.ShowDialog();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            Medicina.VirtualMedic.TwilioForm twilioForm = new Medicina.VirtualMedic.TwilioForm(Admision);
            twilioForm.ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            textBox35.Text = "";
            textBox34.Text = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox37.Text = "";
            textBox36.Text = "";
        }

        string CUP, TSERV, Reg_RIP, CMANTID, CMANID, Serv;
        public Historia_Psicologia()
        {
            InitializeComponent();

            
            ConfigForm.MoverForma(panel3, this);
        }
    }
}
