using Domain;
using Domain.CXN;
using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using ZamenisHealth.Clases;
using ZamenisHealth.Comunes;
using ZamenisHealth.Recepcion.Admision;
using ZamenisHealth.Recepcion.Extras;

namespace ZamenisHealth.Recepcion.AgendaDiaria
{
    public partial class MenuOpcionesAgenda : Forma2
    {
        private IAgenda repoAgenda2;
        private IAgendaC repoAgenda;
        private ICompañia repoCompañia;
        private IReportes repoReportes;
        private IFirmasDigitales repoFirmas;
        private IPacientes repoPacientes;
        private IBodegas repoBodegas;

        private string Admision, IdHora, Hora;
        private otrosDatosPacienteHorario getCita;
        private int CodePrestador, CodeProfesional, CodePaciente;
        private DateTime FechaSeleccionada;

        private ToolTip ColorInicio, ColorNuevo, ColorControlMedico, ColorControlNormal;

        private MensajesGeneral MG;
        Agendamiento f7 = Application.OpenForms.OfType<Agendamiento>().SingleOrDefault();

        public MenuOpcionesAgenda(string admision, string idHora, int codePrestador, string hora, int codeProfesional)
        {
            InitializeComponent();
            Admision = admision;
            IdHora = idHora;
            CodePrestador = codePrestador;
            Hora = hora;
            CodeProfesional = codeProfesional;

            repoAgenda2 = new MAgenda();
            repoAgenda = new MAgendaC();
            repoCompañia = new MCompañia();
            repoReportes = new MReportes();
            repoFirmas = new MFirmasDigitales();
            repoPacientes = new MPacientes();
            repoBodegas = new MBodegas();
            CodePrestador = codePrestador;
        }

        private void MenuOpcionesAgenda_Load(object sender, EventArgs e)
        {
            try
            {
                Titulo.Text = "Opciones de Cita Medica";
                SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";

                if (Preferencias.TabletaFirmas != "A")
                {
                    panel7.Visible = false;
                }
                if (Preferencias.TicketCitas != "A")
                {
                    panel12.Visible = false;
                }
                if (Conexion.ConectionDictionary["Recordatorios"] != "A")
                {
                    panel14.Visible = false;
                }

                if (!string.IsNullOrEmpty(Admision))
                {
                    getCita = new otrosDatosPacienteHorario();
                    getCita = repoAgenda.cargarAdmision(Convert.ToInt32(Admision), "'P','H','A'");

                    if (getCita != null)
                    {
                        CodePaciente = getCita.Hor_Pac_Id;
                        FechaSeleccionada = getCita.Hor_Pac_Fecha_Cita;

                        switch (getCita.Hor_Pac_Tipo_Serv)
                        {
                            case "CU":
                                panel4.Visible = false;
                                panel2.Visible = false;
                                panel5.Visible = false;
                                break;

                            case "TF":
                                panel2.Visible = false;
                                panel5.Visible = false;
                                panel6.Visible = false;
                                panel7.Visible = false;
                                panel13.Visible = false;
                                panel22.Visible = false;
                                panel23.Visible = false;
                                panel24.Visible = false;
                                break;

                            case "TO":
                                panel4.Visible = false;
                                panel5.Visible = false;
                                panel6.Visible = false;
                                panel7.Visible = false;
                                panel13.Visible = false;
                                panel22.Visible = false;
                                panel23.Visible = false;
                                panel24.Visible = false;
                                break;

                            case "PS":
                                panel2.Visible = false;
                                panel4.Visible = false;
                                panel6.Visible = false;
                                panel7.Visible = false;
                                panel13.Visible = false;
                                panel22.Visible = false;
                                panel23.Visible = false;
                                panel24.Visible = false;
                                break;

                            case "MG":
                                panel4.Visible = false;
                                panel2.Visible = false;
                                panel5.Visible = false;
                                panel6.Visible = false;
                                panel7.Visible = false;
                                panel13.Visible = false;
                                break;
                        }

                        DateTime nacimiento = Convert.ToDateTime(getCita.Pac_FechaNto);
                        int edad = DateTime.Today.AddTicks(-nacimiento.Ticks).Year - 1;

                        textBox3.Text = Admision;
                        textBox2.Text = getCita.Bod_Responsable;
                        textBox1.Text = getCita.Hor_Imp_Age;
                        textBox4.Text = getCita.Pac_TipoId + " " + getCita.Pac_IdNum;
                        //textBox3.Text = Convert.ToDateTime(getCita.Hor_Pac_Fecha).ToString(Conexion.ConectionDictionary["Format_Fecha"]) + " - " + Convert.ToDateTime(getCita.Hor_Pac_Hora).ToString("HH:mm tt");
                        textBox5.Text = getCita.Hor_Pac_UsrGraba;
                        richTextBox1.Text = getCita.Hor_Observacion;
                        textBox6.Text = getCita.Pac_Telefono + " - " + getCita.Pac_TelefonoAux;
                        textBox7.Text = getCita.Pac_Email;
                        textBox9.Text = getCita.PacienteDireccion;
                        textBox10.Text = getCita.Com_Telefono_SMS;
                        textBox8.Text = getCita.Pac_FechaNto.ToString("yyyy-MM-dd") + " - " + edad.ToString() + " años";

                        DateTime H = DateTime.Now.Date;
                        DateTime fC = Convert.ToDateTime(getCita.Hor_Pac_Fecha_Cita);

                        if (Convert.ToDateTime(H).ToString("dd-MM-yyyy") != Convert.ToDateTime(fC).ToString("dd-MM-yyyy"))
                        {
                            MG = new MensajesGeneral()
                            {
                                TipoImagen = 0,
                                Mensaje = "Atencion!!! \n\r\r Esta admisionando una cita que no corresponde al dia de HOY calendario.  \n\r\r Esta cita es del dia " + Convert.ToDateTime(fC).ToString("dd-MM-yyyy")
                            };
                            MG.ShowDialog();
                        }

                        if (getCita.Hor_Estado == "H")
                        {
                            panel17.Visible = false;
                            panel20.Visible = false;
                            panel15.Visible = false;
                            panel19.Visible = false;
                            panel14.Visible = false;

                            panel22.Visible = false;
                            panel23.Visible = false;
                            panel24.Visible = false;
                        }
                        else if (getCita.Hor_Estado == "P")
                        {
                            panel17.Visible = false;
                            panel20.Visible = false;
                            panel19.Visible = false;
                            panel11.Visible = false;
                            panel14.Visible = false;

                            if (getCita.Hor_Pac_Tipo_Serv == "CU")
                            {
                                panel22.Visible = true;
                                panel23.Visible = true;
                                panel24.Visible = true;
                            }
                        }
                        else if (getCita.Hor_Estado == "A")
                        {
                            if (getCita.Hor_Pac_Tipo_Serv == "CU")
                            {
                                panel22.Visible = true;
                                panel23.Visible = true;
                                panel24.Visible = true;
                            }
                        }
                    }
                    else
                    {
                        MG = new MensajesGeneral()
                        {
                            Mensaje = "No se logro cargar la admision.  Error",
                            TipoImagen = 1000
                        };
                        MG.ShowDialog();

                        this.Close();
                    }
                }    
                else
                {
                    flowLayoutPanel1.Visible = false;
                    flowLayoutPanel2.Visible = false;

                    panel17.Visible = false;
                    panel16.Visible = false;
                    panel15.Visible = false;
                    panel19.Visible = false;
                    panel20.Visible = false;
                    panel21.Visible = false;
                    panel22.Visible = false;
                    panel23.Visible = false;
                    panel24.Visible = false;
                }

                ColorInicio = new ToolTip();
                ColorInicio.SetToolTip(button3, "COLOR CAFE");
                ColorInicio.ShowAlways = true;

                ColorNuevo = new ToolTip();
                ColorNuevo.SetToolTip(button4, "COLOR ROSA");
                ColorNuevo.ShowAlways = true;

                ColorControlMedico = new ToolTip();
                ColorControlMedico.SetToolTip(button2, "COLOR AMARILLO");
                ColorControlMedico.ShowAlways = true;

                ColorControlNormal = new ToolTip();
                ColorControlNormal.SetToolTip(button1, "COLOR VERDE");
                ColorControlNormal.ShowAlways = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void boton1_Click(object sender, EventArgs e)
        {
            try
            {
                Clipboard.SetText(getCita.Pac_IdNum.ToString().Trim());
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void boton15_Click(object sender, EventArgs e)
        {
            try
            {
                Clipboard.SetText(getCita.Pac_PrimerA.ToString().Trim() + " " +
                                  getCita.Pac_SegundoA.ToString().Trim() + " " +
                                  getCita.Pac_PrimerN.ToString().Trim() + " " +
                                  getCita.Pac_SegundoN.ToString().Trim());
                this.Close();
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
                CXN_CIA DatNombre = repoCompañia.getPrestadorbyCode(CodePrestador);
                List<FirmasR> modelo = new List<FirmasR>();

                if (DatNombre != null)
                {
                    string fase = Microsoft.VisualBasic.Interaction.InputBox(
                            "Digite el numero de la fase",
                            "Impresion de Hoja de Firmas",
                                "");
                    if (fase == "1")
                    {
                        FirmasR F = new FirmasR
                        {
                            Com_Nombre = DatNombre.Com_Nombre,
                            Com_Direccion = DatNombre.Com_Direccion,
                            Com_Telefono = DatNombre.Com_Telefono,
                            Com_Logo = DatNombre.Com_Logo,
                            Com_Email = fase.ToString(),
                            Com_Nombre_SMS = DatNombre.Com_Nombre_SMS
                        };

                        modelo = repoReportes.Firmas_Print(Convert.ToInt32(Admision), F, false);

                        if (modelo != null)
                        {
                            ConfigForm.GenerarReportViewer("DataSet_Firmas", "ZamenisHealth.Reportes.RDLC_Firmas_TF.rdlc", modelo);
                        }
                        else
                        {
                            MG = new MensajesGeneral()
                            {
                                TipoImagen = 1000,
                                Mensaje = "No se puede imprimir una hoja de firmas en este momento para esta seleccion"
                            };
                            MG.ShowDialog();
                        }
                    }
                    else if (fase == "2")
                    {
                        FirmasR F = new FirmasR
                        {
                            Com_Nombre = DatNombre.Com_Nombre,
                            Com_Direccion = DatNombre.Com_Direccion,
                            Com_Telefono = DatNombre.Com_Telefono,
                            Com_Logo = DatNombre.Com_Logo,
                            Com_Email = fase.ToString(),
                            Com_Nombre_SMS = DatNombre.Com_Nombre_SMS
                        };

                        modelo = repoReportes.Firmas_Print(Convert.ToInt32(Admision), F, false);


                        if (modelo != null)
                        {
                            ConfigForm.GenerarReportViewer("DataSet_Firmas", "ZamenisHealth.Reportes.RDLC_Firmas_TF2.rdlc", modelo);
                        }
                        else
                        {
                            MG = new MensajesGeneral()
                            {
                                TipoImagen = 1000,
                                Mensaje = "No se puede imprimir una hoja de firmas en este momento para esta seleccion.  No hay hoja de firmas"
                            };
                            MG.ShowDialog();
                        }
                    }
                    else
                    {
                        MG = new MensajesGeneral()
                        {
                            TipoImagen = 1000,
                            Mensaje = "El valor digitado es incorrecto, solo puede ser 1 o 2"
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
        private void boton4_Click(object sender, EventArgs e)
        {
            try
            {
                CXN_CIA DatNombre = new CXN_CIA();
                List<FirmasR> modelo = new List<FirmasR>();

                DatNombre = repoCompañia.getPrestadorbyCode(CodePrestador);

                if (DatNombre != null)
                {
                    string fase = Microsoft.VisualBasic.Interaction.InputBox(
                            "Digite el numero de la fase",
                            "Impresion de Hoja de Firmas",
                                "");
                    if (fase == "1")
                    {
                        FirmasR F = new FirmasR
                        {
                            Com_Nombre = DatNombre.Com_Nombre,
                            Com_Direccion = DatNombre.Com_Direccion,
                            Com_Telefono = DatNombre.Com_Telefono,
                            Com_Logo = DatNombre.Com_Logo,
                            Com_Email = fase.ToString(),
                            Com_Nombre_SMS = DatNombre.Com_Nombre_SMS
                        };

                        modelo = repoReportes.Firmas_Print(Convert.ToInt32(Admision), F, false);

                        if (modelo != null)
                        {
                            ConfigForm.GenerarReportViewer("DataSet_Firmas", "ZamenisHealth.Reportes.RDLC_Firmas_TO.rdlc", modelo);
                        }
                        else
                        {
                            MG = new MensajesGeneral()
                            {
                                TipoImagen = 1000,
                                Mensaje = "No se puede imprimir una hoja de firmas en este momento para esta seleccion"
                            };

                            MG.ShowDialog();
                        }
                    }
                    else if (fase == "2")
                    {
                        FirmasR F = new FirmasR
                        {
                            Com_Nombre = DatNombre.Com_Nombre,
                            Com_Direccion = DatNombre.Com_Direccion,
                            Com_Telefono = DatNombre.Com_Telefono,
                            Com_Logo = DatNombre.Com_Logo,
                            Com_Email = fase.ToString(),
                            Com_Nombre_SMS = DatNombre.Com_Nombre_SMS
                        };

                        modelo = repoReportes.Firmas_Print(Convert.ToInt32(Admision), F, false);

                        if (modelo != null)
                        {
                            ConfigForm.GenerarReportViewer("DataSet_Firmas", "ZamenisHealth.Reportes.RDLC_Firmas_TO2.rdlc", modelo);
                        }
                        else
                        {
                            MG = new MensajesGeneral()
                            {
                                TipoImagen = 1000,
                                Mensaje = "No se puede imprimir una hoja de firmas en este momento para esta seleccion"
                            };

                            MG.ShowDialog();
                        }
                    }
                    else
                    {
                        MG = new MensajesGeneral()
                        {
                            TipoImagen = 1000,
                            Mensaje = "El valor digitado es incorrecto, solo puede ser 1 o 2"
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
        private void boton5_Click(object sender, EventArgs e)
        {
            try
            {
                List<FirmasR> modelo = new List<FirmasR>();
                CXN_CIA DatNombre = new CXN_CIA();

                DatNombre = repoCompañia.getPrestadorbyCode(CodePrestador);

                if (DatNombre != null)
                {
                    string fase = Microsoft.VisualBasic.Interaction.InputBox(
                            "Digite el numero de la fase",
                            "Impresion de Hoja de Firmas",
                                "");
                    if (fase == "1")
                    {
                        FirmasR F = new FirmasR
                        {
                            Com_Nombre = DatNombre.Com_Nombre,
                            Com_Direccion = DatNombre.Com_Direccion,
                            Com_Telefono = DatNombre.Com_Telefono,
                            Com_Logo = DatNombre.Com_Logo,
                            Com_Email = fase.ToString(),
                            Com_Nombre_SMS = DatNombre.Com_Nombre_SMS
                        };

                        modelo = repoReportes.Firmas_Print(Convert.ToInt32(Admision), F, false);


                        if (modelo != null)
                        {
                            ConfigForm.GenerarReportViewer("DataSet_Firmas", "ZamenisHealth.Reportes.RDLC_Firmas_PS.rdlc", modelo);
                        }
                        else
                        {
                            MessageBox.Show("No se puede imprimir una hoja de firmas en este momento para esta seleccion", "No hay hoja de firmas", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else if (fase == "2")
                    {
                        FirmasR F = new FirmasR
                        {
                            Com_Nombre = DatNombre.Com_Nombre,
                            Com_Direccion = DatNombre.Com_Direccion,
                            Com_Telefono = DatNombre.Com_Telefono,
                            Com_Logo = DatNombre.Com_Logo,
                            Com_Email = fase.ToString(),
                            Com_Nombre_SMS = DatNombre.Com_Nombre_SMS
                        };

                        modelo = repoReportes.Firmas_Print(Convert.ToInt32(Admision), F, false);

                        if (modelo != null)
                        {
                            ConfigForm.GenerarReportViewer("DataSet_Firmas", "ZamenisHealth.Reportes.RDLC_Firmas_PS2.rdlc", modelo);
                        }
                        else
                        {
                            MG = new MensajesGeneral()
                            {
                                TipoImagen = 1000,
                                Mensaje = "No se puede imprimir una hoja de firmas en este momento para esta seleccion"
                            };

                            MG.ShowDialog();
                        }
                    }
                    else
                    {
                        MG = new MensajesGeneral()
                        {
                            TipoImagen = 1000,
                            Mensaje = "El valor digitado es incorrecto, solo puede ser 1 o 2"
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
        private void boton2_Click(object sender, EventArgs e)
        {
            try
            {
                CXN_CIA DatNombre = repoCompañia.getPrestadorbyCode(getCita.Com_Identificador);
                if (DatNombre != null)
                {
                    List<FirmasR> lista = new List<FirmasR>();

                    FirmasR F = new FirmasR
                    {
                        Com_Nombre = DatNombre.Com_Nombre,
                        Com_Direccion = DatNombre.Com_Direccion,
                        Com_Telefono = DatNombre.Com_Telefono,
                        Com_Logo = DatNombre.Com_Logo
                    };

                    List<FirmasR> modelo = new List<FirmasR>();
                    modelo = repoReportes.Firmas_Print(Convert.ToInt32(Admision), F, false);

                    if (modelo != null)
                    {
                        ConfigForm.GenerarReportViewer("DataSet_Firmas", "ZamenisHealth.Reportes.Firmas.rdlc", modelo);
                    }
                    else
                    {
                        MG = new MensajesGeneral()
                        {
                            Mensaje = "No se puede imprimir una hoja de firmas en este momento para esta seleccion",
                            TipoImagen = 1000
                        };
                        MG.ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void boton13_Click(object sender, EventArgs e)
        {
            try
            {
                if (getCita.Hor_Estado != "A")
                {
                    CXN_CIA DatNombre = repoCompañia.getPrestadorbyCode(getCita.Com_Identificador);

                    DateTime FechaLimite = getCita.Hor_Pac_Fecha_Cita;
                    List<(int Adm, string Aut, int Cant)> listAdmition = repoFirmas.getAdmitions(Convert.ToInt32(Admision), getCita.Com_Identificador, getCita.Hor_Pac_Fecha_Cita.Date);

                    if (listAdmition != null)
                    {
                        List<FirmasR> lista = new List<FirmasR>();
                        int Contador = 1;

                        foreach (var i in listAdmition)
                        {
                            otrosDatosPacienteHorario dataAdm = repoAgenda.cargarAdmision(i.Adm, "'H'");

                            CXN_FIRMASDIGITALES fTemp = repoFirmas.getFirmas(i.Adm);
                            byte[] firmaByte = null;

                            if (fTemp == null)
                            {
                                firmaByte = Convert.FromBase64String(repoFirmas.ImageNull());
                            }
                            else
                            {
                                using (MemoryStream ms = new MemoryStream(fTemp.Firma))
                                using (Bitmap bmp = new Bitmap(ms))
                                using (MemoryStream ms2 = new MemoryStream())
                                {
                                    bmp.Save(ms2, System.Drawing.Imaging.ImageFormat.Png);
                                    firmaByte = ms2.ToArray();
                                }
                            }

                            lista.Add(new FirmasR
                            {
                                PacienteNombre = getCita.Pac_PrimerA.ToString() + " " +
                                                 getCita.Pac_SegundoA.ToString() + " " +
                                                 getCita.Pac_PrimerN.ToString() + " " +
                                                 getCita.Pac_SegundoN.ToString(),
                                PacienteAseguradora = getCita.Com_Telefono_SMS.ToString(),
                                PacienteIdentificacion = getCita.Pac_TipoId.ToString() + " " +
                                                         getCita.Pac_IdNum.ToString(),
                                PacienteTelefono = getCita.Pac_Telefono.ToString(),
                                PacienteDireccion = getCita.PacienteDireccion.ToString(),

                                EmpresaNombre = DatNombre.Com_Nombre,
                                EmpresaDireccion = DatNombre.Com_Direccion,
                                Com_UsuarioGraba = DatNombre.Com_Tipo_Doc + " " + DatNombre.Com_Identificacion, //idd prestaddor
                                EmpresaTelefono = dataAdm.Com_Nombre_SMS.Contains("CONSULTA") ? "C" : Contador.ToString(),
                                Logo = Convert.FromBase64String(DatNombre.Com_Logo),

                                FechaBase = Convert.ToDateTime(dataAdm.Hor_Pac_Fecha_Cita),
                                FirmaByte = firmaByte,
                                Con_Nombre = dataAdm.Com_Nombre_SMS,
                                Com_Direccion = listAdmition[0].Aut, //autorizacion
                                Admision = i.Adm,
                                Cantidad = i.Cant
                            });

                            if (!dataAdm.Com_Nombre_SMS.Contains("CONSULTA"))
                            {
                                Contador++;
                            }
                        }

                        ConfigForm.GenerarReportViewer("DataSet_Firmas", "ZamenisHealth.Reportes.FirmasDigitales.rdlc", lista);
                    }
                }
                else
                {
                    MG = new MensajesGeneral()
                    {
                        Mensaje = "Para generar este documento la admision debe esta consumida",
                        TipoImagen = 0
                    };
                    MG.ShowDialog();
                }                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void boton8_Click(object sender, EventArgs e)
        {
            try
            {
                Asistencia f = new ZamenisHealth.Recepcion.Asistencia(getCita.Pac_IdNum)
                {
                    StartPosition = FormStartPosition.CenterScreen,
                    FormBorderStyle = FormBorderStyle.FixedSingle,
                    AutoScroll = false
                };

                f.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void boton9_Click(object sender, EventArgs e)
        {
            try
            {
                O_GeneratedP P = new O_GeneratedP(Convert.ToInt32(Admision));
                P.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void boton10_Click(object sender, EventArgs e)
        {
            try
            {
                if (getCita.Hor_Estado != "A")
                {
                    RecibosAsociados RA = new RecibosAsociados(Convert.ToInt32(Admision));
                    RA.ShowDialog();          
                }
                else
                {
                    MG = new MensajesGeneral()
                    {
                        Mensaje = "Para generar este documento la admision debe estar consumida y con registro de pago",
                        TipoImagen = 0
                    };
                    MG.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void boton11_Click(object sender, EventArgs e)
        {
            try
            {
                if (getCita.Hor_Estado == "H")
                {
                    Extras.CAsistencia C = new Extras.CAsistencia(Convert.ToInt32(Admision));
                    C.ShowDialog();
                }
                else
                {
                    MG = new MensajesGeneral()
                    {
                        Mensaje = "El certificado de asistencia solo esta disponible cuando la historia clinica ya este realizada",
                        TipoImagen = 0
                    };
                    MG.ShowDialog();
                }                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void boton12_Click(object sender, EventArgs e)
        {
            try
            {
                PrintTickets pT = new PrintTickets(Convert.ToInt32(Admision));
                pT.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void boton6_Click(object sender, EventArgs e)
        {
            try
            {
                if (getCita.Hor_Estado == "H" || getCita.Hor_Estado == "P")
                {
                    Extras.EncuestasQR q = new EncuestasQR(Convert.ToInt32(Admision));
                    q.ShowDialog();
                }
                else
                {
                    MG = new MensajesGeneral()
                    {
                        Mensaje = "PAra enviar la encuesta, esta admision debe esta consumida",
                        TipoImagen = 0
                    };
                    MG.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private async void boton14_Click(object sender, EventArgs e)
        {
            try
            {
                CXN_HORARIO _datosCitaSMS = repoAgenda2.DatosforMailSMS(Convert.ToInt32(Admision));

                if (_datosCitaSMS != null)
                {
                    if (_datosCitaSMS.Hor_Estado != "A")
                    {
                        MG = new MensajesGeneral()
                        {
                            Mensaje = "Para enviar un recordatorio de citas el estado de la cita no puede estar consumido, " +
                            "debe estar en color negro el estado de la cita",
                            TipoImagen = 1000
                        };
                       
                        MG.ShowDialog();
                    }
                    else
                    {
                        DateTime Hoy = DateTime.Now.Date;

                        string Celular = _datosCitaSMS.Hor_RegAtn;
                        DateTime Fecha = Convert.ToDateTime(_datosCitaSMS.Hor_Pac_Fecha_Cita);
                        DateTime Hora = Convert.ToDateTime(_datosCitaSMS.Hor_Pac_Hora_Cita);

                        if (Convert.ToDateTime(Fecha.ToString(Conexion.ConectionDictionary["Format_Fecha"])) < Convert.ToDateTime(Hoy.ToString(Conexion.ConectionDictionary["Format_Fecha"])))
                        {
                            MG = new MensajesGeneral()
                            {
                                Mensaje = "Para enviar un recordatorio de citas el estado de la cita no puede estar consumido, " +
                                "debe estar en color negro el estado de la cita",
                                TipoImagen = 1000
                            };
                           
                            MG.ShowDialog();
                        }
                        else
                        {
                            bool ValidaCel = repoPacientes.ValidaCelular(Celular);
                            if (ValidaCel != true)
                            {
                                MG = new MensajesGeneral()
                                {
                                    Mensaje = "El numero de celular del paciente contiene caracteres NO numericos, " +
                                    "actualize el numero y vuelva a intentar -> " + Celular,
                                    TipoImagen = 1000
                                };

                                MG.ShowDialog();

                                ActualizarCel(Convert.ToInt32(_datosCitaSMS.Hor_Pac_Id));
                            }
                            else
                            {
                                string Mensaje_SMS = "Cita agendada dia " + Convert.ToDateTime(Fecha).ToString(Conexion.ConectionDictionary["Format_Fecha"]) +
                                             " hora " + Convert.ToDateTime(Hora).ToString("H:mm tt") + " en " + _datosCitaSMS.Com_Nombre_SMS + " Tel " + _datosCitaSMS.Com_Telefono_SMS + " Dir " + _datosCitaSMS.Com_Direccion +
                                             " favor asista 20 minutos antes";

                                Extras.SMSPersonaliza f = new Extras.SMSPersonaliza(Convert.ToInt32(Admision), Mensaje_SMS, Celular, _datosCitaSMS.Hor_Pac_Cia);
                                f.ShowDialog();
                            }
                        }
                    }                                       
                }
                else
                {
                    MG.Mensaje = "Hay un inconveniente con esta admision, posiblemente no esta en estado Agendado";
                    MG.TipoImagen = 1000;
                    MG.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void ActualizarCel(int Admision)
        {
            try
            {
                string texto = Microsoft.VisualBasic.Interaction.InputBox(
                        "Digite el numero de celular a actualizar: ",
                        "Actualizacion de Celular de Paciente");

                if (texto != "")
                {
                    try
                    {
                        var _celBool = repoPacientes.ValidaCelular(texto);
                        if (_celBool == true)
                        {
                            bool _updateCel =  repoPacientes.ActualizarCelular(texto, Admision);

                            if (_updateCel != true)
                            {
                                MG = new MensajesGeneral()
                                {
                                    Mensaje = "El numero digitado no es valido",
                                    TipoImagen = 1000
                                };

                                MG.ShowDialog();
                            }
                            else
                            {
                                MG = new MensajesGeneral()
                                {
                                    Mensaje = "Actualizado con exito",
                                    TipoImagen = 3
                                };

                                MG.ShowDialog();
                            }
                        }
                        else
                        {
                            MG = new MensajesGeneral()
                            {
                                Mensaje = "El numero digitado no es valido",
                                TipoImagen = 1000
                            };

                            MG.ShowDialog();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void boton16_Click(object sender, EventArgs e)
        {
            try
            {
                Extras.CitasMailPDF F = new Extras.CitasMailPDF(Convert.ToInt32(Admision));
                F.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void boton17_Click(object sender, EventArgs e)
        {
            try
            {
                if (getCita.Hor_Estado != "A")
                {
                    MG = new MensajesGeneral()
                    {
                        Mensaje = "Para mover un paciente de una agenda a otra, la admision no puede estar consumida",
                        TipoImagen = 1000
                    };
                    MG.ShowDialog();
                }
                else
                {
                    Extras.MoverAgenda MA = new Extras.MoverAgenda(Convert.ToInt32(Admision));
                    MA.ShowDialog();
                }                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void boton18_Click(object sender, EventArgs e)
        {
            try
            {
                if (getCita.Hor_Estado == "P")
                {
                    repoAgenda2.anularAdmision(Convert.ToInt32(Admision), Contenedor.UsuarioLogueado);
                    f7.EventoInicial();
                    this.Close();
                }
                else
                {
                    MG = new MensajesGeneral()
                    {
                        Mensaje = "Esta admision no se puede anular porque ya cuenta con una historia hecha",
                        TipoImagen = 1000
                    };
                    MG.ShowDialog();
                }                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }            
        }
        private void boton20_Click(object sender, EventArgs e)
        {
            try
            {
                if (getCita.Hor_Pac_Tipo_Serv == "CU" || getCita.Hor_Pac_Tipo_Serv == "MG")
                {
                    AdmisionesCuraciones a = new AdmisionesCuraciones(Convert.ToInt32(Admision));
                    a.ShowDialog();
                }
                else
                {
                    Admisiones A = new Admisiones(Convert.ToInt32(Admision));

                    this.Dispose();
                    this.Close();

                    A.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void boton24_Click(object sender, EventArgs e)
        {
            repoAgenda2.UpdatecolorCita(Convert.ToDateTime(FechaSeleccionada), Convert.ToInt32(CodePaciente), "T");
            f7.EventoInicial();
            this.Close();
        }
        private void boton21_Click(object sender, EventArgs e)
        {
            repoAgenda2.UpdatecolorCita(Convert.ToDateTime(FechaSeleccionada), Convert.ToInt32(CodePaciente), "N");
            f7.EventoInicial();
            this.Close();
        }
        private void boton22_Click(object sender, EventArgs e)
        {
            repoAgenda2.UpdatecolorCita(Convert.ToDateTime(FechaSeleccionada), Convert.ToInt32(CodePaciente), "I");
            f7.EventoInicial();
            this.Close();
        }
        private void boton23_Click(object sender, EventArgs e)
        {
            repoAgenda2.UpdatecolorCita(Convert.ToDateTime(FechaSeleccionada), Convert.ToInt32(CodePaciente), "C");
            f7.EventoInicial();
            this.Close();
        }
        private void boton19_Click(object sender, EventArgs e)
        {
            if (getCita.Hor_Estado == "A")
            {
                NovedadesAdmision f = new NovedadesAdmision(Convert.ToInt32(Admision), "Cancela");
                f.ShowDialog();
                this.Close();
            }
            else
            {
                MG = new MensajesGeneral()
                {
                    Mensaje = "Para cancelar la cita, la admision no puede estar consumida",
                    TipoImagen = 1000
                };
                MG.ShowDialog();
            }                        
        }
        private void boton7_Click(object sender, EventArgs e)
        {
            try
            {                
                string texto = Microsoft.VisualBasic.Interaction.InputBox(
                       "Digite la cantidad de espacios a bloquear: ",
                       "Bloqueo de espacios");

                if (texto != "")
                {
                    int numericValue;

                    bool isNumber = int.TryParse(texto, out numericValue);

                    if (isNumber == false || numericValue == 0)
                    {
                        MG = new MensajesGeneral()
                        {
                            Mensaje = "El valor digitado no es valido",
                            TipoImagen = 1000
                        };
                        
                        MG.ShowDialog();
                    }
                    else
                    {
                        int conteo = 0;

                        foreach (DataGridViewRow row in f7.dataGridView1.Rows)
                        {
                            conteo++;
                        }

                        if (conteo < Convert.ToInt32(texto))
                        {
                            MG = new MensajesGeneral()
                            {
                                Mensaje = "No es posible bloquear mas espacios de los que hay en la posision que ha solicitado",
                                TipoImagen = 1000
                            };
                            
                            MG.ShowDialog();
                        }
                        else
                        {
                            Bloqueo(texto.ToString() + " ESPACIOS BLOQUEADOS DESDE RECEPCION", Convert.ToInt32(texto));
                        }
                    }                  
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        void Bloqueo(string Razon, int CantidadEspacios)
        {
            try
            {
                string Observa = "--> Bloqueado por: " + Comunes.Contenedor.UsuarioLogueado + " - " + f7.calendarHQ1.txtDia.Text;

                CXN_BODEGAS DatoProf = repoBodegas.getDatosCode(CodeProfesional);

                CXN_HORARIO H = new CXN_HORARIO
                {
                    Hor_Estado = "B",
                    Hor_Observacion = Observa,
                    Hor_Pac_Cup = "",
                    Hor_Pac_Sal = "",
                    Hor_Vales = "",
                    Hor_Pac_Modalidad = "",
                    Hor_Pac_Fecha_Cita = Convert.ToDateTime(f7.calendarHQ1.dTPCalendar.Value.Date),
                    Hor_Pac_Id_Hora = IdHora,
                    Hor_Pac_Hora_Cita = Convert.ToDateTime(Hora), //hora cita
                    Hor_Pac_Bod = CodeProfesional,
                    Hor_Pac_Tipo_Serv = DatoProf.Bod_Tipo,
                    Hor_Pac_Ase = 88,
                    Hor_Imp_Age = Razon.ToString(),
                    Hor_Pac_Cia = CodePrestador,
                    Hor_Pac_Id = 1,
                    Hor_Pac_UsrGraba = Comunes.Contenedor.UsuarioLogueado,
                    Hor_BloqEspaces = CantidadEspacios,
                    Hor_GrupoServicios = "",
                    Hor_Regimen = ""
                };

                int createBloq = repoAgenda2.AgendarPaciente(H);

                if (createBloq <= 0)
                {
                    MG = new MensajesGeneral()
                    {
                        Mensaje = "No se logro bloquear el espacio",
                        TipoImagen = 1000
                    };
                    
                    MG.ShowDialog();
                }
                else
                {
                    f7.EventoInicial();
                }                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
