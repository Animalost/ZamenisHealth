using Domain;
using Domain.CXN;
using Domain.Fibromialgia;
using FormAndControls;
using Microsoft.Reporting.WinForms;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using Persistence.Fibromialgia.Interfaces;
using Persistence.Fibromialgia.Metodos;
using Persistence.Informes.Interfaces;
using Persistence.Informes.Methods;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.UI.DataVisualization.Charting;
using System.Windows.Forms;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.Gerenciales
{
    public partial class InformeEnfermeria : Forma
    {
        private static readonly ICompañia repositorioCompañias = new MCompañia();
        private static readonly IInformeEnfermeria repositorioInfEnf = new MInformeEnfermeria();
        private static readonly IBodegas repositorioBodegas = new MBodegas();
        private static readonly IGenerales repoGen = new MGenerales();
        private static readonly IInformeMensualFibromialgia rFibromialgia = new MInformeMensualFibromialgia();

        private List<ClaseReportsFibro> L1;
        private List<ClaseReportsFibro> L2;
        private List<ClaseReportsFibro> L3; 
        private List<ClaseReportsFibro> L4;

        private List<Domain.Informes.InformeEnfermeria> Ie;
        private MensajesGeneral MG;

        public InformeEnfermeria()
        {
            InitializeComponent();
        }
        private async void btnZamenis1_ButtonClick(object sender, EventArgs e)
        {
            try
            {
                MG = new MensajesGeneral();

                if (comboBox1.Text == "" || comboBox2.Text == "" || comboBox3.Text == "" || comboBox4.Text == "")
                {
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "Debe seleccionar una opcion de cada lista";
                    MG.ShowDialog();
                    return;
                }

                Task oTask = null;

                if (comboBox4.Text == "Curaciones")
                {
                    Shows();
                    oTask = new Task(GenerarInformeCuraciones);
                    oTask.Start();
                    await oTask;
                    Hides();
                }

                if (comboBox4.Text == "Medicina General")
                {
                    Shows();
                    oTask = new Task(GenerarInformeMedicinaGeneral);
                    oTask.Start();
                    await oTask;
                    Hides();
                }

                if (comboBox4.Text == "Fibromialgia")
                {
                    Shows();
                    oTask = new Task(GenerarInformeFibromialgia);
                    oTask.Start();
                    await oTask;
                    Hides();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void CargarCias()
        {
            List<CXN_CIA> cias = repositorioCompañias.getAllCompañias();
            if (cias != null)
            {
                foreach (var i in cias)
                {
                    comboBox3.Items.Add(i.Com_Nombre);
                }

                comboBox3.SelectedIndex = 0;
            }
        }

        private void InformeEnfermeria_Load(object sender, EventArgs e)
        {
            try
            {
                Titulo.Text = "Informe Enfermeria";
                SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";
                LogoMain.Image = Properties.Resources.Splash;

                ToolStripButton btnGenerar = new ToolStripButton();
                btnGenerar = createToolButton("Generar");
                MenuLateral.Items.Add(btnGenerar);
                btnGenerar.Click += btnZamenis1_ButtonClick;
                CargarCias();
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        
        void Shows()
        {
            pictureBox2.Visible = true;
        }

        void Hides()
        {
            pictureBox2.Visible = false;
        }

        void GenerarInformeMedicinaGeneral()
        { 
            try
            {
                PictureBox pic = new PictureBox();
                MG = new MensajesGeneral();

                CXN_CIA codCia = repositorioCompañias.getPrestadorbyName(comboBox3.Text);
                if (codCia != null)
                {
                    string logTemp = codCia.Com_Logo.ToString();
                    if (logTemp != null)
                    {
                        Byte[] bytes = Convert.FromBase64String(logTemp); //convierte a bytes
                        MemoryStream stmBLOBData = new MemoryStream(bytes);
                        pic.Image = Image.FromStream(stmBLOBData);
                    }
                }

                int cantAtendidosYearEneroGeneralMG = 0; int cantAtendidosYearFebreroGeneralMG = 0; int cantAtendidosYearMarzoGeneralMG = 0;
                int cantAtendidosYearAbrilGeneralMG = 0; int cantAtendidosYearMayoGeneralMG = 0; int cantAtendidosYearJunioGeneralMG = 0;
                int cantAtendidosYearAgostoGeneralMG = 0; int cantAtendidosYearSeptiembreGeneralMG = 0; int cantAtendidosYearOctubreGeneralMG = 0;
                int cantAtendidosYearNoviembreGeneralMG = 0; int cantAtendidosYearDiciembreGeneralMG = 0; int cantAtendidosYearJulioGeneralMG = 0;
                int SumatoriaYearGeneralMG = 0;
                string nameEnf = "";

                Ie = new List<Domain.Informes.InformeEnfermeria>();

                List<int> getMedicosAño = repositorioInfEnf.getMedicosYear(comboBox2.Text);
                if (getMedicosAño != null)
                {
                    foreach (int iM in getMedicosAño)
                    {
                        if (comboBox1.Text == "Enero")
                        {
                            cantAtendidosYearEneroGeneralMG = repositorioInfEnf.getCantidadAtendidosMG(iM, "Enero", comboBox2.Text);
                        }
                        else if (comboBox1.Text == "Febrero")
                        {
                            cantAtendidosYearEneroGeneralMG = repositorioInfEnf.getCantidadAtendidosMG(iM, "Enero", comboBox2.Text);
                            cantAtendidosYearFebreroGeneralMG = repositorioInfEnf.getCantidadAtendidosMG(iM, "Febrero", comboBox2.Text);
                        }
                        else if (comboBox1.Text == "Marzo")
                        {
                            cantAtendidosYearEneroGeneralMG = repositorioInfEnf.getCantidadAtendidosMG(iM, "Enero", comboBox2.Text);
                            cantAtendidosYearFebreroGeneralMG = repositorioInfEnf.getCantidadAtendidosMG(iM, "Febrero", comboBox2.Text);
                            cantAtendidosYearMarzoGeneralMG = repositorioInfEnf.getCantidadAtendidosMG(iM, "Marzo", comboBox2.Text);
                        }
                        else if (comboBox1.Text == "Abril")
                        {
                            cantAtendidosYearEneroGeneralMG = repositorioInfEnf.getCantidadAtendidosMG(iM, "Enero", comboBox2.Text);
                            cantAtendidosYearFebreroGeneralMG = repositorioInfEnf.getCantidadAtendidosMG(iM, "Febrero", comboBox2.Text);
                            cantAtendidosYearMarzoGeneralMG = repositorioInfEnf.getCantidadAtendidosMG(iM, "Marzo", comboBox2.Text);
                            cantAtendidosYearAbrilGeneralMG = repositorioInfEnf.getCantidadAtendidosMG(iM, "Abril", comboBox2.Text);
                        }
                        else if (comboBox1.Text == "Mayo")
                        {
                            cantAtendidosYearEneroGeneralMG = repositorioInfEnf.getCantidadAtendidosMG(iM, "Enero", comboBox2.Text);
                            cantAtendidosYearFebreroGeneralMG = repositorioInfEnf.getCantidadAtendidosMG(iM, "Febrero", comboBox2.Text);
                            cantAtendidosYearMarzoGeneralMG = repositorioInfEnf.getCantidadAtendidosMG(iM, "Marzo", comboBox2.Text);
                            cantAtendidosYearAbrilGeneralMG = repositorioInfEnf.getCantidadAtendidosMG(iM, "Abril", comboBox2.Text);
                            cantAtendidosYearMayoGeneralMG = repositorioInfEnf.getCantidadAtendidosMG(iM, "Mayo", comboBox2.Text);
                        }
                        else if (comboBox1.Text == "Junio")
                        {
                            cantAtendidosYearEneroGeneralMG = repositorioInfEnf.getCantidadAtendidosMG(iM, "Enero", comboBox2.Text);
                            cantAtendidosYearFebreroGeneralMG = repositorioInfEnf.getCantidadAtendidosMG(iM, "Febrero", comboBox2.Text);
                            cantAtendidosYearMarzoGeneralMG = repositorioInfEnf.getCantidadAtendidosMG(iM, "Marzo", comboBox2.Text);
                            cantAtendidosYearAbrilGeneralMG = repositorioInfEnf.getCantidadAtendidosMG(iM, "Abril", comboBox2.Text);
                            cantAtendidosYearMayoGeneralMG = repositorioInfEnf.getCantidadAtendidosMG(iM, "Mayo", comboBox2.Text);
                            cantAtendidosYearJunioGeneralMG = repositorioInfEnf.getCantidadAtendidosMG(iM, "Junio", comboBox2.Text);
                        }
                        else if (comboBox1.Text == "Julio")
                        {
                            cantAtendidosYearEneroGeneralMG = repositorioInfEnf.getCantidadAtendidosMG(iM, "Enero", comboBox2.Text);
                            cantAtendidosYearFebreroGeneralMG = repositorioInfEnf.getCantidadAtendidosMG(iM, "Febrero", comboBox2.Text);
                            cantAtendidosYearMarzoGeneralMG = repositorioInfEnf.getCantidadAtendidosMG(iM, "Marzo", comboBox2.Text);
                            cantAtendidosYearAbrilGeneralMG = repositorioInfEnf.getCantidadAtendidosMG(iM, "Abril", comboBox2.Text);
                            cantAtendidosYearMayoGeneralMG = repositorioInfEnf.getCantidadAtendidosMG(iM, "Mayo", comboBox2.Text);
                            cantAtendidosYearJunioGeneralMG = repositorioInfEnf.getCantidadAtendidosMG(iM, "Junio", comboBox2.Text);
                            cantAtendidosYearJulioGeneralMG = repositorioInfEnf.getCantidadAtendidosMG(iM, "Julio", comboBox2.Text);
                        }
                        else if (comboBox1.Text == "Agosto")
                        {
                            cantAtendidosYearEneroGeneralMG = repositorioInfEnf.getCantidadAtendidosMG(iM, "Enero", comboBox2.Text);
                            cantAtendidosYearFebreroGeneralMG = repositorioInfEnf.getCantidadAtendidosMG(iM, "Febrero", comboBox2.Text);
                            cantAtendidosYearMarzoGeneralMG = repositorioInfEnf.getCantidadAtendidosMG(iM, "Marzo", comboBox2.Text);
                            cantAtendidosYearAbrilGeneralMG = repositorioInfEnf.getCantidadAtendidosMG(iM, "Abril", comboBox2.Text);
                            cantAtendidosYearMayoGeneralMG = repositorioInfEnf.getCantidadAtendidosMG(iM, "Mayo", comboBox2.Text);
                            cantAtendidosYearJunioGeneralMG = repositorioInfEnf.getCantidadAtendidosMG(iM, "Junio", comboBox2.Text);
                            cantAtendidosYearJulioGeneralMG = repositorioInfEnf.getCantidadAtendidosMG(iM, "Julio", comboBox2.Text);
                            cantAtendidosYearAgostoGeneralMG = repositorioInfEnf.getCantidadAtendidosMG(iM, "Agosto", comboBox2.Text);
                        }
                        else if (comboBox1.Text == "Septiembre")
                        {
                            cantAtendidosYearEneroGeneralMG = repositorioInfEnf.getCantidadAtendidosMG(iM, "Enero", comboBox2.Text);
                            cantAtendidosYearFebreroGeneralMG = repositorioInfEnf.getCantidadAtendidosMG(iM, "Febrero", comboBox2.Text);
                            cantAtendidosYearMarzoGeneralMG = repositorioInfEnf.getCantidadAtendidosMG(iM, "Marzo", comboBox2.Text);
                            cantAtendidosYearAbrilGeneralMG = repositorioInfEnf.getCantidadAtendidosMG(iM, "Abril", comboBox2.Text);
                            cantAtendidosYearMayoGeneralMG = repositorioInfEnf.getCantidadAtendidosMG(iM, "Mayo", comboBox2.Text);
                            cantAtendidosYearJunioGeneralMG = repositorioInfEnf.getCantidadAtendidosMG(iM, "Junio", comboBox2.Text);
                            cantAtendidosYearJulioGeneralMG = repositorioInfEnf.getCantidadAtendidosMG(iM, "Julio", comboBox2.Text);
                            cantAtendidosYearAgostoGeneralMG = repositorioInfEnf.getCantidadAtendidosMG(iM, "Agosto", comboBox2.Text);
                            cantAtendidosYearSeptiembreGeneralMG = repositorioInfEnf.getCantidadAtendidosMG(iM, "Septiembre", comboBox2.Text);
                        }
                        else if (comboBox1.Text == "Octubre")
                        {
                            cantAtendidosYearEneroGeneralMG = repositorioInfEnf.getCantidadAtendidosMG(iM, "Enero", comboBox2.Text);
                            cantAtendidosYearFebreroGeneralMG = repositorioInfEnf.getCantidadAtendidosMG(iM, "Febrero", comboBox2.Text);
                            cantAtendidosYearMarzoGeneralMG = repositorioInfEnf.getCantidadAtendidosMG(iM, "Marzo", comboBox2.Text);
                            cantAtendidosYearAbrilGeneralMG = repositorioInfEnf.getCantidadAtendidosMG(iM, "Abril", comboBox2.Text);
                            cantAtendidosYearMayoGeneralMG = repositorioInfEnf.getCantidadAtendidosMG(iM, "Mayo", comboBox2.Text);
                            cantAtendidosYearJunioGeneralMG = repositorioInfEnf.getCantidadAtendidosMG(iM, "Junio", comboBox2.Text);
                            cantAtendidosYearJulioGeneralMG = repositorioInfEnf.getCantidadAtendidosMG(iM, "Julio", comboBox2.Text);
                            cantAtendidosYearAgostoGeneralMG = repositorioInfEnf.getCantidadAtendidosMG(iM, "Agosto", comboBox2.Text);
                            cantAtendidosYearSeptiembreGeneralMG = repositorioInfEnf.getCantidadAtendidosMG(iM, "Septiembre", comboBox2.Text);
                            cantAtendidosYearOctubreGeneralMG = repositorioInfEnf.getCantidadAtendidosMG(iM, "Octubre", comboBox2.Text);
                        }
                        else if (comboBox1.Text == "Noviembre")
                        {
                            cantAtendidosYearEneroGeneralMG = repositorioInfEnf.getCantidadAtendidosMG(iM, "Enero", comboBox2.Text);
                            cantAtendidosYearFebreroGeneralMG = repositorioInfEnf.getCantidadAtendidosMG(iM, "Febrero", comboBox2.Text);
                            cantAtendidosYearMarzoGeneralMG = repositorioInfEnf.getCantidadAtendidosMG(iM, "Marzo", comboBox2.Text);
                            cantAtendidosYearAbrilGeneralMG = repositorioInfEnf.getCantidadAtendidosMG(iM, "Abril", comboBox2.Text);
                            cantAtendidosYearMayoGeneralMG = repositorioInfEnf.getCantidadAtendidosMG(iM, "Mayo", comboBox2.Text);
                            cantAtendidosYearJunioGeneralMG = repositorioInfEnf.getCantidadAtendidosMG(iM, "Junio", comboBox2.Text);
                            cantAtendidosYearJulioGeneralMG = repositorioInfEnf.getCantidadAtendidosMG(iM, "Julio", comboBox2.Text);
                            cantAtendidosYearAgostoGeneralMG = repositorioInfEnf.getCantidadAtendidosMG(iM, "Agosto", comboBox2.Text);
                            cantAtendidosYearSeptiembreGeneralMG = repositorioInfEnf.getCantidadAtendidosMG(iM, "Septiembre", comboBox2.Text);
                            cantAtendidosYearOctubreGeneralMG = repositorioInfEnf.getCantidadAtendidosMG(iM, "Octubre", comboBox2.Text);
                            cantAtendidosYearNoviembreGeneralMG = repositorioInfEnf.getCantidadAtendidosMG(iM, "Noviembre", comboBox2.Text);
                        }
                        else if (comboBox1.Text == "Diciembre")
                        {
                            cantAtendidosYearEneroGeneralMG = repositorioInfEnf.getCantidadAtendidosMG(iM, "Enero", comboBox2.Text);
                            cantAtendidosYearFebreroGeneralMG = repositorioInfEnf.getCantidadAtendidosMG(iM, "Febrero", comboBox2.Text);
                            cantAtendidosYearMarzoGeneralMG = repositorioInfEnf.getCantidadAtendidosMG(iM, "Marzo", comboBox2.Text);
                            cantAtendidosYearAbrilGeneralMG = repositorioInfEnf.getCantidadAtendidosMG(iM, "Abril", comboBox2.Text);
                            cantAtendidosYearMayoGeneralMG = repositorioInfEnf.getCantidadAtendidosMG(iM, "Mayo", comboBox2.Text);
                            cantAtendidosYearJunioGeneralMG = repositorioInfEnf.getCantidadAtendidosMG(iM, "Junio", comboBox2.Text);
                            cantAtendidosYearJulioGeneralMG = repositorioInfEnf.getCantidadAtendidosMG(iM, "Julio", comboBox2.Text);
                            cantAtendidosYearAgostoGeneralMG = repositorioInfEnf.getCantidadAtendidosMG(iM, "Agosto", comboBox2.Text);
                            cantAtendidosYearSeptiembreGeneralMG = repositorioInfEnf.getCantidadAtendidosMG(iM, "Septiembre", comboBox2.Text);
                            cantAtendidosYearOctubreGeneralMG = repositorioInfEnf.getCantidadAtendidosMG(iM, "Octubre", comboBox2.Text);
                            cantAtendidosYearNoviembreGeneralMG = repositorioInfEnf.getCantidadAtendidosMG(iM, "Noviembre", comboBox2.Text);
                            cantAtendidosYearDiciembreGeneralMG = repositorioInfEnf.getCantidadAtendidosMG(iM, "Diciembre", comboBox2.Text);
                        }
                        
                        SumatoriaYearGeneralMG = cantAtendidosYearEneroGeneralMG + cantAtendidosYearFebreroGeneralMG + cantAtendidosYearMarzoGeneralMG +
                        cantAtendidosYearAbrilGeneralMG + cantAtendidosYearMayoGeneralMG + cantAtendidosYearJunioGeneralMG + cantAtendidosYearJulioGeneralMG +
                        cantAtendidosYearAgostoGeneralMG + cantAtendidosYearSeptiembreGeneralMG + cantAtendidosYearOctubreGeneralMG + cantAtendidosYearNoviembreGeneralMG +
                        cantAtendidosYearDiciembreGeneralMG;

                        nameEnf = repositorioBodegas.getDatosCode(iM).Bod_Responsable;

                        Ie.Add(new Domain.Informes.InformeEnfermeria 
                        {
                            Enfermero = nameEnf.ToString(),
                            cantAtendidosYearEneroMG = cantAtendidosYearEneroGeneralMG,
                            cantAtendidosYearFebreroMG = cantAtendidosYearFebreroGeneralMG,
                            cantAtendidosYearMarzoMG = cantAtendidosYearMarzoGeneralMG,
                            cantAtendidosYearAbrilMG = cantAtendidosYearAbrilGeneralMG,
                            cantAtendidosYearMayoMG = cantAtendidosYearMayoGeneralMG,
                            cantAtendidosYearJunioMG = cantAtendidosYearJunioGeneralMG,
                            cantAtendidosYearJulioMG = cantAtendidosYearJulioGeneralMG,
                            cantAtendidosYearAgostoMG = cantAtendidosYearAgostoGeneralMG,
                            cantAtendidosYearSeptiembreMG = cantAtendidosYearSeptiembreGeneralMG,
                            cantAtendidosYearOctubreMG = cantAtendidosYearOctubreGeneralMG,
                            cantAtendidosYearNoviembreMG = cantAtendidosYearNoviembreGeneralMG,
                            cantAtendidosYearDiciembreMG = cantAtendidosYearDiciembreGeneralMG,
                            SumatoriaYearMG = SumatoriaYearGeneralMG,

                            MesGenera = comboBox1.Text.ToUpper() + " DE " + comboBox2.Text,
                            Logo = repoGen.GetBytes(pic.Image),
                            UserGenera = Contenedor.UsuarioLogueado.ToUpper()
                        });
                    }

                    Thread thread = new Thread(ExportInformeMG);
                    thread.SetApartmentState(ApartmentState.STA); // Configura el subproceso en STA
                    thread.Start();
                }
                else
                {
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "No hay listado de enfermeros";
                    MG.ShowDialog();
                    return;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        void GenerarInformeFibromialgia()
        {
            try
            {
                List<ClaseReportsFibro> listaFinal1 = new List<ClaseReportsFibro>();
                List<ClaseReportsFibro> listaFinal4 = new List<ClaseReportsFibro>();

                List<int> codBodsTF = repositorioBodegas.FiltrarCodesByTipose("TF");
                List<int> codBodsTO = repositorioBodegas.FiltrarCodesByTipose("TO");
                List<int> codBodsPS = repositorioBodegas.FiltrarCodesByTipose("PS");

                List<int> codBods = new List<int>();

                if (codBodsTF != null)
                {
                    codBods.AddRange(codBodsTF);
                    
                    foreach (int i in codBodsTF)
                    {
                        (string Prof, int Cant) getCant = rFibromialgia.getCantidadByProfesional("TF", comboBox1.Text, Convert.ToInt32(comboBox2.Text), i);
                        if (getCant.Cant > 0)
                        {
                            listaFinal1.Add(new ClaseReportsFibro
                            {
                                Dato1 = getCant.Prof,
                                Dato2 = getCant.Cant.ToString()
                            });
                        }                          
                    }
                }
                if (codBodsTO != null)
                {
                    codBods.AddRange(codBodsTO);

                    foreach (int i in codBodsTO)
                    {
                        (string Prof, int Cant) getCant = rFibromialgia.getCantidadByProfesional("TO", comboBox1.Text, Convert.ToInt32(comboBox2.Text), i);
                        if (getCant.Cant > 0)
                        {
                            listaFinal1.Add(new ClaseReportsFibro
                            {
                                Dato1 = getCant.Prof,
                                Dato2 = getCant.Cant.ToString()
                            });
                        }
                    }
                }
                if (codBodsPS != null)
                {
                    codBods.AddRange(codBodsPS);

                    foreach (int i in codBodsPS)
                    {
                        (string Prof, int Cant) getCant = rFibromialgia.getCantidadByProfesional("PS", comboBox1.Text, Convert.ToInt32(comboBox2.Text), i);
                        if (getCant.Cant > 0)
                        {
                            listaFinal1.Add(new ClaseReportsFibro
                            {
                                Dato1 = getCant.Prof,
                                Dato2 = getCant.Cant.ToString()
                            });
                        }
                    }
                }

                if (codBods == null || codBods.Count <= 0)
                {
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "No hay listado de Fibromialgia";
                    MG.ShowDialog();
                }
                else
                {
                    PictureBox pic = new PictureBox();

                    CXN_CIA codCia = repositorioCompañias.getPrestadorbyName(comboBox3.Text);
                    if (codCia != null)
                    {
                        string logTemp = codCia.Com_Logo.ToString();
                        if (logTemp != null)
                        {
                            Byte[] bytes = Convert.FromBase64String(logTemp); //convierte a bytes

                            listaFinal4.Add(new ClaseReportsFibro { 
                                Dato1 = comboBox1.Text + " - " + comboBox2.Text, //Mes y año
                                Graph = bytes, //logo cia
                            });
                        }
                    }

                    //Total Atenciones Mes
                    int TotalTF = rFibromialgia.getCantidad("TF", comboBox1.Text, Convert.ToInt32(comboBox2.Text));
                    int TotalTO = rFibromialgia.getCantidad("TO", comboBox1.Text, Convert.ToInt32(comboBox2.Text));
                    int TotalPS = rFibromialgia.getCantidad("PS", comboBox1.Text, Convert.ToInt32(comboBox2.Text));

                    int TOTALATENDIDOSMES = TotalTF + TotalTO + TotalPS;

                    //Total Atenciones por Mes Profesional = DICCANTIDADXMESXPROF                    

                    //Numero de Pacientes Atendidos por MES
                    //Por Total
                    int TOTALPACMES = rFibromialgia.getCantidadPacientesMES(comboBox1.Text, Convert.ToInt32(comboBox2.Text), true);
                    //Por Mes
                    int TOTALCANTPACENERO = rFibromialgia.getCantidadPacientesMES("ENERO", Convert.ToInt32(comboBox2.Text), false);
                    int TOTALCANTPACFEBRERO = rFibromialgia.getCantidadPacientesMES("FEBRERO", Convert.ToInt32(comboBox2.Text), false);
                    int TOTALCANTPACMARZO = rFibromialgia.getCantidadPacientesMES("MARZO", Convert.ToInt32(comboBox2.Text), false);
                    int TOTALCANTPACABRIL = rFibromialgia.getCantidadPacientesMES("ABRIL", Convert.ToInt32(comboBox2.Text), false);
                    int TOTALCANTPACMAYO = rFibromialgia.getCantidadPacientesMES("MAYO", Convert.ToInt32(comboBox2.Text), false);
                    int TOTALCANTPACJUNIO = rFibromialgia.getCantidadPacientesMES("JUNIO", Convert.ToInt32(comboBox2.Text), false);
                    int TOTALCANTPACJULIO = rFibromialgia.getCantidadPacientesMES("JULIO", Convert.ToInt32(comboBox2.Text), false);
                    int TOTALCANTPACAGOSTO = rFibromialgia.getCantidadPacientesMES("AGOSTO", Convert.ToInt32(comboBox2.Text), false);
                    int TOTALCANTPACSEPTIEMBRE = rFibromialgia.getCantidadPacientesMES("SEPTIEMBRE", Convert.ToInt32(comboBox2.Text), false);
                    int TOTALCANTPACOCTUBRE = rFibromialgia.getCantidadPacientesMES("OCTUBRE", Convert.ToInt32(comboBox2.Text), false);
                    int TOTALCANTPACNOVIEMBRE = rFibromialgia.getCantidadPacientesMES("NOVIEMBRE", Convert.ToInt32(comboBox2.Text), false);
                    int TOTALCANTPACDICIEMBRE = rFibromialgia.getCantidadPacientesMES("DICIEMBRE+", Convert.ToInt32(comboBox2.Text), false);
                    
                    List<ClaseReportsFibro> listaFinal2 = new List<ClaseReportsFibro>();
                    List<ClaseReportsFibro> listaFinal3 = new List<ClaseReportsFibro>();

                    listaFinal1 = listaFinal1
                            .GroupBy(x => x.Dato1)
                            .Select(g => new ClaseReportsFibro
                            {
                                Dato1 = g.Key,
                                Dato2 = g.Sum(x => Convert.ToInt32(x.Dato2)).ToString()
                            })
                            .OrderByDescending(x => x.Dato2).ToList();

                    //Grafica Cantidad Atenciones por Consultorio
                    byte[] GRAFICA1 = rFibromialgia.GraficoCantidadPorProfesional(listaFinal1);

                    foreach (var i in listaFinal1)
                    {
                        i.Graph = GRAFICA1;
                        i.Dato15 = listaFinal1.Sum(x => Convert.ToInt32(x.Dato2)).ToString(); //Total Sumatoria por mes CANTIDAD DE SERVICIOS                                                    
                    }

                    listaFinal3.Add(new ClaseReportsFibro {
                        Dato1 = TOTALCANTPACENERO.ToString(), //Enero
                        Dato2 = TOTALCANTPACFEBRERO.ToString(), //Febrero
                        Dato3 = TOTALCANTPACMARZO.ToString(), //Marzo
                        Dato4 = TOTALCANTPACABRIL.ToString(), //Abril
                        Dato5 = TOTALCANTPACMAYO.ToString(), //Mayo
                        Dato6 = TOTALCANTPACJUNIO.ToString(), //Junio
                        Dato7 = TOTALCANTPACJULIO.ToString(), //Julio
                        Dato8 = TOTALCANTPACAGOSTO.ToString(), //Agosto
                        Dato9 = TOTALCANTPACSEPTIEMBRE.ToString(), //Septiuembre
                        Dato10 = TOTALCANTPACOCTUBRE.ToString(), //Octubre
                        Dato11 = TOTALCANTPACNOVIEMBRE.ToString(), //Noviembre
                        Dato12 = TOTALCANTPACDICIEMBRE.ToString(), //Diciembre
                        Dato13 = TOTALPACMES.ToString(), //Total pacientes por mes
                    });
                   

                    //FOREACH MAESTRO ES AQUI
                    //Total por servicio Mes
                    //Obtener los CUPS
                    List<(string Code, string Service)> servs = rFibromialgia.getServices();

                    servs = servs
                            .GroupBy(x => new { x.Code })
                            .Select(g => g.First())
                            .ToList();

                    List<ClaseReportsFibro> listaTempForGrafica2 = new List<ClaseReportsFibro>();

                    //Obtener Cantidad por Servicio
                    foreach (var i in servs)
                    {
                        int cant = rFibromialgia.getCantidadByServ(i.Code, comboBox1.Text, Convert.ToInt32(comboBox2.Text));

                        if (cant > 0)
                        {
                            listaFinal2.Add(new ClaseReportsFibro
                            {
                                Dato1 = cant.ToString(), // Total atendidos
                                Dato2 = i.Service
                            });

                            listaTempForGrafica2.Add(new ClaseReportsFibro
                            {
                                Dato1 = i.Service,
                                Dato2 = cant.ToString()
                            });
                        }                        
                    }

                    listaFinal2 = listaFinal2.OrderByDescending(x => Convert.ToInt32(x.Dato1)).ToList();
                    byte[] GRAFICA2 = rFibromialgia.GraficoCantidadPorProfesional(listaTempForGrafica2);

                    foreach (var i in listaFinal2)
                    {
                        i.Graph = GRAFICA2;
                    }

                    //EXPORTAR
                    setListasFibro(listaFinal1, listaFinal2, listaFinal3, listaFinal4);

                    Thread thread = new Thread(ExportInformeFib);
                    thread.SetApartmentState(ApartmentState.STA); // Configura el subproceso en STA
                    thread.Start();                                        
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        void setListasFibro(List<ClaseReportsFibro> l1, List<ClaseReportsFibro> l2, List<ClaseReportsFibro> l3, List<ClaseReportsFibro> l4)
        {
            L1 = new List<ClaseReportsFibro>();
            L2 = new List<ClaseReportsFibro>();
            L3 = new List<ClaseReportsFibro>();
            L4 = new List<ClaseReportsFibro>();

            L1 = l1; L2 = l2; L3 = l3; L4 = l4;
        }

        void ExportInformeFib()
        {
            try
            {
                Form F = new Form();
                ReportViewer R = new ReportViewer();

                R.LocalReport.DataSources.Clear();
                R.LocalReport.DataSources.Add(new ReportDataSource("DataSetFibroGeneral", L1));
                R.LocalReport.DataSources.Add(new ReportDataSource("DataSetFibroGeneral2", L2));
                R.LocalReport.DataSources.Add(new ReportDataSource("DataSetFibroGeneral3", L3));
                R.LocalReport.DataSources.Add(new ReportDataSource("DataSetFibroGeneral4", L4));
                R.LocalReport.ReportEmbeddedResource = "ZamenisHealth.Reportes.Fibro.InformeGeneral.InformeGeneral.rdlc";
                R.SetDisplayMode(DisplayMode.PrintLayout);
                R.ZoomMode = ZoomMode.Percent;
                R.ZoomPercent = 100;
                R.LocalReport.EnableExternalImages = true;
                R.Font = new Font("Arial", 7);
                R.RefreshReport();
                R.Visible = true;
                R.Dock = System.Windows.Forms.DockStyle.Fill;
                F.Controls.Add(R);
                F.WindowState = FormWindowState.Maximized;
                //F.System.Threading.ApartmentState state = System.Threading.ApartmentState.STA;
                //F.System.Threading.Thread.CurrentThread.SetApartmentState(state);
                Application.Run(F);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }           
        }

        void GenerarInformeCuraciones()
        {
            try
            {
                List<int> codBods = repositorioBodegas.FiltrarCodesByTipose("CU");
                if (codBods == null)
                {
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "No hay listado de enfermeros";
                    MG.ShowDialog();
                    return;
                }

                PictureBox pic = new PictureBox();

                CXN_CIA codCia = repositorioCompañias.getPrestadorbyName(comboBox3.Text);
                if (codCia != null)
                {
                    string logTemp = codCia.Com_Logo.ToString();
                    if (logTemp != null)
                    {
                        Byte[] bytes = Convert.FromBase64String(logTemp); //convierte a bytes
                        MemoryStream stmBLOBData = new MemoryStream(bytes);                       
                        pic.Image = Image.FromStream(stmBLOBData);
                    }
                }

                Ie = new List<Domain.Informes.InformeEnfermeria>();

                int SumatoriaCantAtendidos = repositorioInfEnf.getCantidadAtendidos2(comboBox1.Text, comboBox2.Text, "CU");

                string nameEnf = "";
                int cantAtendidos = 0;

                int CantPatologiaVasculares = repositorioInfEnf.getCantidadPatologia(comboBox1.Text, comboBox2.Text, "Vasculares");
                int CantPatologiaPosOperatorio = repositorioInfEnf.getCantidadPatologia(comboBox1.Text, comboBox2.Text, "PosOperatorio");
                int CantPatologiaZonadePresion = repositorioInfEnf.getCantidadPatologia(comboBox1.Text, comboBox2.Text, "Zona de Presion");
                int CantPatologiaTrauma = repositorioInfEnf.getCantidadPatologia(comboBox1.Text, comboBox2.Text, "Trauma");
                int CantPatologiaQuemado = repositorioInfEnf.getCantidadPatologia(comboBox1.Text, comboBox2.Text, "Quemado");
                int CantPatologiaOtros = repositorioInfEnf.getCantidadPatologia2(comboBox1.Text, comboBox2.Text, "Otros");                  
                int CantPatologiaPieDiabetico = repositorioInfEnf.getCantidadPatologia(comboBox1.Text, comboBox2.Text, "PieDiabetico");

                byte[] getGraph2 = Grafico2(CantPatologiaVasculares, CantPatologiaPosOperatorio, CantPatologiaZonadePresion,
                    CantPatologiaTrauma, CantPatologiaQuemado, CantPatologiaPieDiabetico, CantPatologiaOtros);

                //Patologias Anual
                Dictionary<string, int> getPatologiasVascularesAño = GetPatologiasMesAMes("Vasculares");
                Dictionary<string, int> getPatologiasPosOperatorioAño = GetPatologiasMesAMes("PosOperatorio");
                Dictionary<string, int> getPatologiasZPAño = GetPatologiasMesAMes("Zona de Presion");
                Dictionary<string, int> getPatologiasTraumaAño = GetPatologiasMesAMes("Trauma");
                Dictionary<string, int> getPatologiasQuemadoAño = GetPatologiasMesAMes("Quemado");
                Dictionary<string, int> getPatologiasOtrosAño = GetPatologiasMesAMes("Otros");
                Dictionary<string, int> getPatologiasPieDiabeticoAño = GetPatologiasMesAMes("PieDiabetico");

                #region //Patologias Anual

                int TotalPatologiaAnualVascular = 0;

                foreach (string pVA in getPatologiasVascularesAño.Keys)
                {
                    TotalPatologiaAnualVascular = TotalPatologiaAnualVascular + getPatologiasVascularesAño[pVA];
                }
                int TotalPatologiaAnualPos = 0;

                foreach (string pPA in getPatologiasPosOperatorioAño.Keys)
                {
                    TotalPatologiaAnualPos = TotalPatologiaAnualPos + getPatologiasPosOperatorioAño[pPA];
                }
                int TotalPatologiaAnualZP = 0;

                foreach (string pZPA in getPatologiasZPAño.Keys)
                {
                    TotalPatologiaAnualZP = TotalPatologiaAnualZP + getPatologiasZPAño[pZPA];
                }
                int TotalPatologiaAnualTX = 0;

                foreach (string pTXA in getPatologiasTraumaAño.Keys)
                {
                    TotalPatologiaAnualTX = TotalPatologiaAnualTX + getPatologiasTraumaAño[pTXA];
                }
                int TotalPatologiaAnualQX = 0;

                foreach (string pTXA in getPatologiasQuemadoAño.Keys)
                {
                    TotalPatologiaAnualQX = TotalPatologiaAnualQX + getPatologiasQuemadoAño[pTXA];
                }
                int TotalPatologiaAnualOtros = 0;

                foreach (string pOtA in getPatologiasOtrosAño.Keys)
                {
                    TotalPatologiaAnualOtros = TotalPatologiaAnualOtros + getPatologiasOtrosAño[pOtA];
                }
                int TotalPatologiaAnualPD = 0;

                foreach (string pDA in getPatologiasPieDiabeticoAño.Keys)
                {
                    TotalPatologiaAnualPD = TotalPatologiaAnualPD + getPatologiasPieDiabeticoAño[pDA];
                }

                int TotalAñoTodasPatologiasH = TotalPatologiaAnualVascular + TotalPatologiaAnualPos + TotalPatologiaAnualZP + TotalPatologiaAnualTX +
                    TotalPatologiaAnualQX + TotalPatologiaAnualOtros + TotalPatologiaAnualPD;

                int TotalPatologiaAnualEneroVertical = 0; int TotalPatologiaAnualFebreroVertical = 0; int TotalPatologiaAnualMarzoVertical = 0;
                int TotalPatologiaAnualAbrilVertical = 0; int TotalPatologiaAnualMayoVertical = 0; int TotalPatologiaAnualJunioVertical = 0;
                int TotalPatologiaAnualJulioVertical = 0; int TotalPatologiaAnualAgostoVertical = 0; int TotalPatologiaAnualSeptiembreVertical = 0;
                int TotalPatologiaAnualOctubreVertical = 0; int TotalPatologiaAnualNoviembreVertical = 0; int TotalPatologiaAnualDiciembreVertical = 0;

                if (comboBox1.Text == "Enero" || comboBox1.Text == "ENERO")
                {
                    TotalPatologiaAnualEneroVertical = getPatologiasVascularesAño["Enero"] + getPatologiasPosOperatorioAño["Enero"] +
                    getPatologiasZPAño["Enero"] + getPatologiasTraumaAño["Enero"] + getPatologiasQuemadoAño["Enero"] + getPatologiasOtrosAño["Enero"] +
                    getPatologiasPieDiabeticoAño["Enero"];
                }
                else if (comboBox1.Text == "Febrero" || comboBox1.Text == "FEBRERO")
                {
                    TotalPatologiaAnualEneroVertical = getPatologiasVascularesAño["Enero"] + getPatologiasPosOperatorioAño["Enero"] +
                    getPatologiasZPAño["Enero"] + getPatologiasTraumaAño["Enero"] + getPatologiasQuemadoAño["Enero"] + getPatologiasOtrosAño["Enero"] +
                    getPatologiasPieDiabeticoAño["Enero"];

                    TotalPatologiaAnualFebreroVertical = getPatologiasVascularesAño["Febrero"] + getPatologiasPosOperatorioAño["Febrero"] +
                    getPatologiasZPAño["Febrero"] + getPatologiasTraumaAño["Febrero"] + getPatologiasQuemadoAño["Febrero"] + getPatologiasOtrosAño["Febrero"] +
                    getPatologiasPieDiabeticoAño["Febrero"];
                }
                else if (comboBox1.Text == "Marzo" || comboBox1.Text == "MARZO")
                {
                    TotalPatologiaAnualEneroVertical = getPatologiasVascularesAño["Enero"] + getPatologiasPosOperatorioAño["Enero"] +
                    getPatologiasZPAño["Enero"] + getPatologiasTraumaAño["Enero"] + getPatologiasQuemadoAño["Enero"] + getPatologiasOtrosAño["Enero"] +
                    getPatologiasPieDiabeticoAño["Enero"];

                    TotalPatologiaAnualFebreroVertical = getPatologiasVascularesAño["Febrero"] + getPatologiasPosOperatorioAño["Febrero"] +
                    getPatologiasZPAño["Febrero"] + getPatologiasTraumaAño["Febrero"] + getPatologiasQuemadoAño["Febrero"] + getPatologiasOtrosAño["Febrero"] +
                    getPatologiasPieDiabeticoAño["Febrero"];

                    TotalPatologiaAnualMarzoVertical = getPatologiasVascularesAño["Marzo"] + getPatologiasPosOperatorioAño["Marzo"] +
                    getPatologiasZPAño["Marzo"] + getPatologiasTraumaAño["Marzo"] + getPatologiasQuemadoAño["Marzo"] + getPatologiasOtrosAño["Marzo"] +
                    getPatologiasPieDiabeticoAño["Marzo"];
                }
                else if (comboBox1.Text == "Abril" || comboBox1.Text == "ABRIL")
                {
                    TotalPatologiaAnualEneroVertical = getPatologiasVascularesAño["Enero"] + getPatologiasPosOperatorioAño["Enero"] +
                    getPatologiasZPAño["Enero"] + getPatologiasTraumaAño["Enero"] + getPatologiasQuemadoAño["Enero"] + getPatologiasOtrosAño["Enero"] +
                    getPatologiasPieDiabeticoAño["Enero"];

                    TotalPatologiaAnualFebreroVertical = getPatologiasVascularesAño["Febrero"] + getPatologiasPosOperatorioAño["Febrero"] +
                    getPatologiasZPAño["Febrero"] + getPatologiasTraumaAño["Febrero"] + getPatologiasQuemadoAño["Febrero"] + getPatologiasOtrosAño["Febrero"] +
                    getPatologiasPieDiabeticoAño["Febrero"];

                    TotalPatologiaAnualMarzoVertical = getPatologiasVascularesAño["Marzo"] + getPatologiasPosOperatorioAño["Marzo"] +
                    getPatologiasZPAño["Marzo"] + getPatologiasTraumaAño["Marzo"] + getPatologiasQuemadoAño["Marzo"] + getPatologiasOtrosAño["Marzo"] +
                    getPatologiasPieDiabeticoAño["Marzo"];

                    TotalPatologiaAnualAbrilVertical = getPatologiasVascularesAño["Abril"] + getPatologiasPosOperatorioAño["Abril"] +
                    getPatologiasZPAño["Abril"] + getPatologiasTraumaAño["Abril"] + getPatologiasQuemadoAño["Abril"] + getPatologiasOtrosAño["Abril"] +
                    getPatologiasPieDiabeticoAño["Abril"];
                }
                else if (comboBox1.Text == "Mayo" || comboBox1.Text == "MAYO")
                {
                    TotalPatologiaAnualEneroVertical = getPatologiasVascularesAño["Enero"] + getPatologiasPosOperatorioAño["Enero"] +
                    getPatologiasZPAño["Enero"] + getPatologiasTraumaAño["Enero"] + getPatologiasQuemadoAño["Enero"] + getPatologiasOtrosAño["Enero"] +
                    getPatologiasPieDiabeticoAño["Enero"];

                    TotalPatologiaAnualFebreroVertical = getPatologiasVascularesAño["Febrero"] + getPatologiasPosOperatorioAño["Febrero"] +
                    getPatologiasZPAño["Febrero"] + getPatologiasTraumaAño["Febrero"] + getPatologiasQuemadoAño["Febrero"] + getPatologiasOtrosAño["Febrero"] +
                    getPatologiasPieDiabeticoAño["Febrero"];

                    TotalPatologiaAnualMarzoVertical = getPatologiasVascularesAño["Marzo"] + getPatologiasPosOperatorioAño["Marzo"] +
                    getPatologiasZPAño["Marzo"] + getPatologiasTraumaAño["Marzo"] + getPatologiasQuemadoAño["Marzo"] + getPatologiasOtrosAño["Marzo"] +
                    getPatologiasPieDiabeticoAño["Marzo"];

                    TotalPatologiaAnualAbrilVertical = getPatologiasVascularesAño["Abril"] + getPatologiasPosOperatorioAño["Abril"] +
                    getPatologiasZPAño["Abril"] + getPatologiasTraumaAño["Abril"] + getPatologiasQuemadoAño["Abril"] + getPatologiasOtrosAño["Abril"] +
                    getPatologiasPieDiabeticoAño["Abril"];

                    TotalPatologiaAnualMayoVertical = getPatologiasVascularesAño["Mayo"] + getPatologiasPosOperatorioAño["Mayo"] +
                    getPatologiasZPAño["Mayo"] + getPatologiasTraumaAño["Mayo"] + getPatologiasQuemadoAño["Mayo"] + getPatologiasOtrosAño["Mayo"] +
                    getPatologiasPieDiabeticoAño["Mayo"];
                }
                else if (comboBox1.Text == "Junio" || comboBox1.Text == "JUNIO")
                {
                    TotalPatologiaAnualEneroVertical = getPatologiasVascularesAño["Enero"] + getPatologiasPosOperatorioAño["Enero"] +
                    getPatologiasZPAño["Enero"] + getPatologiasTraumaAño["Enero"] + getPatologiasQuemadoAño["Enero"] + getPatologiasOtrosAño["Enero"] +
                    getPatologiasPieDiabeticoAño["Enero"];

                    TotalPatologiaAnualFebreroVertical = getPatologiasVascularesAño["Febrero"] + getPatologiasPosOperatorioAño["Febrero"] +
                    getPatologiasZPAño["Febrero"] + getPatologiasTraumaAño["Febrero"] + getPatologiasQuemadoAño["Febrero"] + getPatologiasOtrosAño["Febrero"] +
                    getPatologiasPieDiabeticoAño["Febrero"];

                    TotalPatologiaAnualMarzoVertical = getPatologiasVascularesAño["Marzo"] + getPatologiasPosOperatorioAño["Marzo"] +
                    getPatologiasZPAño["Marzo"] + getPatologiasTraumaAño["Marzo"] + getPatologiasQuemadoAño["Marzo"] + getPatologiasOtrosAño["Marzo"] +
                    getPatologiasPieDiabeticoAño["Marzo"];

                    TotalPatologiaAnualAbrilVertical = getPatologiasVascularesAño["Abril"] + getPatologiasPosOperatorioAño["Abril"] +
                    getPatologiasZPAño["Abril"] + getPatologiasTraumaAño["Abril"] + getPatologiasQuemadoAño["Abril"] + getPatologiasOtrosAño["Abril"] +
                    getPatologiasPieDiabeticoAño["Abril"];

                    TotalPatologiaAnualMayoVertical = getPatologiasVascularesAño["Mayo"] + getPatologiasPosOperatorioAño["Mayo"] +
                    getPatologiasZPAño["Mayo"] + getPatologiasTraumaAño["Mayo"] + getPatologiasQuemadoAño["Mayo"] + getPatologiasOtrosAño["Mayo"] +
                    getPatologiasPieDiabeticoAño["Mayo"];

                    TotalPatologiaAnualJunioVertical = getPatologiasVascularesAño["Junio"] + getPatologiasPosOperatorioAño["Junio"] +
                    getPatologiasZPAño["Junio"] + getPatologiasTraumaAño["Junio"] + getPatologiasQuemadoAño["Junio"] + getPatologiasOtrosAño["Junio"] +
                    getPatologiasPieDiabeticoAño["Junio"];
                }
                else if (comboBox1.Text == "Julio" || comboBox1.Text == "JULIO")
                {
                    TotalPatologiaAnualEneroVertical = getPatologiasVascularesAño["Enero"] + getPatologiasPosOperatorioAño["Enero"] +
                    getPatologiasZPAño["Enero"] + getPatologiasTraumaAño["Enero"] + getPatologiasQuemadoAño["Enero"] + getPatologiasOtrosAño["Enero"] +
                    getPatologiasPieDiabeticoAño["Enero"];

                    TotalPatologiaAnualFebreroVertical = getPatologiasVascularesAño["Febrero"] + getPatologiasPosOperatorioAño["Febrero"] +
                    getPatologiasZPAño["Febrero"] + getPatologiasTraumaAño["Febrero"] + getPatologiasQuemadoAño["Febrero"] + getPatologiasOtrosAño["Febrero"] +
                    getPatologiasPieDiabeticoAño["Febrero"];

                    TotalPatologiaAnualMarzoVertical = getPatologiasVascularesAño["Marzo"] + getPatologiasPosOperatorioAño["Marzo"] +
                    getPatologiasZPAño["Marzo"] + getPatologiasTraumaAño["Marzo"] + getPatologiasQuemadoAño["Marzo"] + getPatologiasOtrosAño["Marzo"] +
                    getPatologiasPieDiabeticoAño["Marzo"];

                    TotalPatologiaAnualAbrilVertical = getPatologiasVascularesAño["Abril"] + getPatologiasPosOperatorioAño["Abril"] +
                    getPatologiasZPAño["Abril"] + getPatologiasTraumaAño["Abril"] + getPatologiasQuemadoAño["Abril"] + getPatologiasOtrosAño["Abril"] +
                    getPatologiasPieDiabeticoAño["Abril"];

                    TotalPatologiaAnualMayoVertical = getPatologiasVascularesAño["Mayo"] + getPatologiasPosOperatorioAño["Mayo"] +
                    getPatologiasZPAño["Mayo"] + getPatologiasTraumaAño["Mayo"] + getPatologiasQuemadoAño["Mayo"] + getPatologiasOtrosAño["Mayo"] +
                    getPatologiasPieDiabeticoAño["Mayo"];

                    TotalPatologiaAnualJunioVertical = getPatologiasVascularesAño["Junio"] + getPatologiasPosOperatorioAño["Junio"] +
                    getPatologiasZPAño["Junio"] + getPatologiasTraumaAño["Junio"] + getPatologiasQuemadoAño["Junio"] + getPatologiasOtrosAño["Junio"] +
                    getPatologiasPieDiabeticoAño["Junio"];

                    TotalPatologiaAnualJulioVertical = getPatologiasVascularesAño["Julio"] + getPatologiasPosOperatorioAño["Julio"] +
                    getPatologiasZPAño["Julio"] + getPatologiasTraumaAño["Julio"] + getPatologiasQuemadoAño["Julio"] + getPatologiasOtrosAño["Julio"] +
                    getPatologiasPieDiabeticoAño["Julio"];
                }
                else if (comboBox1.Text == "Agosto" || comboBox1.Text == "AGOSTO")
                {
                    TotalPatologiaAnualEneroVertical = getPatologiasVascularesAño["Enero"] + getPatologiasPosOperatorioAño["Enero"] +
                    getPatologiasZPAño["Enero"] + getPatologiasTraumaAño["Enero"] + getPatologiasQuemadoAño["Enero"] + getPatologiasOtrosAño["Enero"] +
                    getPatologiasPieDiabeticoAño["Enero"];

                    TotalPatologiaAnualFebreroVertical = getPatologiasVascularesAño["Febrero"] + getPatologiasPosOperatorioAño["Febrero"] +
                    getPatologiasZPAño["Febrero"] + getPatologiasTraumaAño["Febrero"] + getPatologiasQuemadoAño["Febrero"] + getPatologiasOtrosAño["Febrero"] +
                    getPatologiasPieDiabeticoAño["Febrero"];

                    TotalPatologiaAnualMarzoVertical = getPatologiasVascularesAño["Marzo"] + getPatologiasPosOperatorioAño["Marzo"] +
                    getPatologiasZPAño["Marzo"] + getPatologiasTraumaAño["Marzo"] + getPatologiasQuemadoAño["Marzo"] + getPatologiasOtrosAño["Marzo"] +
                    getPatologiasPieDiabeticoAño["Marzo"];

                    TotalPatologiaAnualAbrilVertical = getPatologiasVascularesAño["Abril"] + getPatologiasPosOperatorioAño["Abril"] +
                    getPatologiasZPAño["Abril"] + getPatologiasTraumaAño["Abril"] + getPatologiasQuemadoAño["Abril"] + getPatologiasOtrosAño["Abril"] +
                    getPatologiasPieDiabeticoAño["Abril"];

                    TotalPatologiaAnualMayoVertical = getPatologiasVascularesAño["Mayo"] + getPatologiasPosOperatorioAño["Mayo"] +
                    getPatologiasZPAño["Mayo"] + getPatologiasTraumaAño["Mayo"] + getPatologiasQuemadoAño["Mayo"] + getPatologiasOtrosAño["Mayo"] +
                    getPatologiasPieDiabeticoAño["Mayo"];

                    TotalPatologiaAnualJunioVertical = getPatologiasVascularesAño["Junio"] + getPatologiasPosOperatorioAño["Junio"] +
                    getPatologiasZPAño["Junio"] + getPatologiasTraumaAño["Junio"] + getPatologiasQuemadoAño["Junio"] + getPatologiasOtrosAño["Junio"] +
                    getPatologiasPieDiabeticoAño["Junio"];

                    TotalPatologiaAnualJulioVertical = getPatologiasVascularesAño["Julio"] + getPatologiasPosOperatorioAño["Julio"] +
                    getPatologiasZPAño["Julio"] + getPatologiasTraumaAño["Julio"] + getPatologiasQuemadoAño["Julio"] + getPatologiasOtrosAño["Julio"] +
                    getPatologiasPieDiabeticoAño["Julio"];

                    TotalPatologiaAnualAgostoVertical = getPatologiasVascularesAño["Agosto"] + getPatologiasPosOperatorioAño["Agosto"] +
                   getPatologiasZPAño["Agosto"] + getPatologiasTraumaAño["Agosto"] + getPatologiasQuemadoAño["Agosto"] + getPatologiasOtrosAño["Agosto"] +
                   getPatologiasPieDiabeticoAño["Agosto"];
                }
                else if (comboBox1.Text == "Septiembre" || comboBox1.Text == "SEPTIEMBRE")
                {
                    TotalPatologiaAnualEneroVertical = getPatologiasVascularesAño["Enero"] + getPatologiasPosOperatorioAño["Enero"] +
                    getPatologiasZPAño["Enero"] + getPatologiasTraumaAño["Enero"] + getPatologiasQuemadoAño["Enero"] + getPatologiasOtrosAño["Enero"] +
                    getPatologiasPieDiabeticoAño["Enero"];

                    TotalPatologiaAnualFebreroVertical = getPatologiasVascularesAño["Febrero"] + getPatologiasPosOperatorioAño["Febrero"] +
                    getPatologiasZPAño["Febrero"] + getPatologiasTraumaAño["Febrero"] + getPatologiasQuemadoAño["Febrero"] + getPatologiasOtrosAño["Febrero"] +
                    getPatologiasPieDiabeticoAño["Febrero"];

                    TotalPatologiaAnualMarzoVertical = getPatologiasVascularesAño["Marzo"] + getPatologiasPosOperatorioAño["Marzo"] +
                    getPatologiasZPAño["Marzo"] + getPatologiasTraumaAño["Marzo"] + getPatologiasQuemadoAño["Marzo"] + getPatologiasOtrosAño["Marzo"] +
                    getPatologiasPieDiabeticoAño["Marzo"];

                    TotalPatologiaAnualAbrilVertical = getPatologiasVascularesAño["Abril"] + getPatologiasPosOperatorioAño["Abril"] +
                    getPatologiasZPAño["Abril"] + getPatologiasTraumaAño["Abril"] + getPatologiasQuemadoAño["Abril"] + getPatologiasOtrosAño["Abril"] +
                    getPatologiasPieDiabeticoAño["Abril"];

                    TotalPatologiaAnualMayoVertical = getPatologiasVascularesAño["Mayo"] + getPatologiasPosOperatorioAño["Mayo"] +
                    getPatologiasZPAño["Mayo"] + getPatologiasTraumaAño["Mayo"] + getPatologiasQuemadoAño["Mayo"] + getPatologiasOtrosAño["Mayo"] +
                    getPatologiasPieDiabeticoAño["Mayo"];

                    TotalPatologiaAnualJunioVertical = getPatologiasVascularesAño["Junio"] + getPatologiasPosOperatorioAño["Junio"] +
                    getPatologiasZPAño["Junio"] + getPatologiasTraumaAño["Junio"] + getPatologiasQuemadoAño["Junio"] + getPatologiasOtrosAño["Junio"] +
                    getPatologiasPieDiabeticoAño["Junio"];

                    TotalPatologiaAnualJulioVertical = getPatologiasVascularesAño["Julio"] + getPatologiasPosOperatorioAño["Julio"] +
                    getPatologiasZPAño["Julio"] + getPatologiasTraumaAño["Julio"] + getPatologiasQuemadoAño["Julio"] + getPatologiasOtrosAño["Julio"] +
                    getPatologiasPieDiabeticoAño["Julio"];

                    TotalPatologiaAnualAgostoVertical = getPatologiasVascularesAño["Agosto"] + getPatologiasPosOperatorioAño["Agosto"] +
                    getPatologiasZPAño["Agosto"] + getPatologiasTraumaAño["Agosto"] + getPatologiasQuemadoAño["Agosto"] + getPatologiasOtrosAño["Agosto"] +
                    getPatologiasPieDiabeticoAño["Agosto"];

                    TotalPatologiaAnualSeptiembreVertical = getPatologiasVascularesAño["Septiembre"] + getPatologiasPosOperatorioAño["Septiembre"] +
                    getPatologiasZPAño["Septiembre"] + getPatologiasTraumaAño["Septiembre"] + getPatologiasQuemadoAño["Septiembre"] + getPatologiasOtrosAño["Septiembre"] +
                    getPatologiasPieDiabeticoAño["Septiembre"];
                }
                else if (comboBox1.Text == "Octubre" || comboBox1.Text == "OCTUBRE")
                {
                    TotalPatologiaAnualEneroVertical = getPatologiasVascularesAño["Enero"] + getPatologiasPosOperatorioAño["Enero"] +
                    getPatologiasZPAño["Enero"] + getPatologiasTraumaAño["Enero"] + getPatologiasQuemadoAño["Enero"] + getPatologiasOtrosAño["Enero"] +
                    getPatologiasPieDiabeticoAño["Enero"];

                    TotalPatologiaAnualFebreroVertical = getPatologiasVascularesAño["Febrero"] + getPatologiasPosOperatorioAño["Febrero"] +
                    getPatologiasZPAño["Febrero"] + getPatologiasTraumaAño["Febrero"] + getPatologiasQuemadoAño["Febrero"] + getPatologiasOtrosAño["Febrero"] +
                    getPatologiasPieDiabeticoAño["Febrero"];

                    TotalPatologiaAnualMarzoVertical = getPatologiasVascularesAño["Marzo"] + getPatologiasPosOperatorioAño["Marzo"] +
                    getPatologiasZPAño["Marzo"] + getPatologiasTraumaAño["Marzo"] + getPatologiasQuemadoAño["Marzo"] + getPatologiasOtrosAño["Marzo"] +
                    getPatologiasPieDiabeticoAño["Marzo"];

                    TotalPatologiaAnualAbrilVertical = getPatologiasVascularesAño["Abril"] + getPatologiasPosOperatorioAño["Abril"] +
                    getPatologiasZPAño["Abril"] + getPatologiasTraumaAño["Abril"] + getPatologiasQuemadoAño["Abril"] + getPatologiasOtrosAño["Abril"] +
                    getPatologiasPieDiabeticoAño["Abril"];

                    TotalPatologiaAnualMayoVertical = getPatologiasVascularesAño["Mayo"] + getPatologiasPosOperatorioAño["Mayo"] +
                    getPatologiasZPAño["Mayo"] + getPatologiasTraumaAño["Mayo"] + getPatologiasQuemadoAño["Mayo"] + getPatologiasOtrosAño["Mayo"] +
                    getPatologiasPieDiabeticoAño["Mayo"];

                    TotalPatologiaAnualJunioVertical = getPatologiasVascularesAño["Junio"] + getPatologiasPosOperatorioAño["Junio"] +
                    getPatologiasZPAño["Junio"] + getPatologiasTraumaAño["Junio"] + getPatologiasQuemadoAño["Junio"] + getPatologiasOtrosAño["Junio"] +
                    getPatologiasPieDiabeticoAño["Junio"];

                    TotalPatologiaAnualJulioVertical = getPatologiasVascularesAño["Julio"] + getPatologiasPosOperatorioAño["Julio"] +
                    getPatologiasZPAño["Julio"] + getPatologiasTraumaAño["Julio"] + getPatologiasQuemadoAño["Julio"] + getPatologiasOtrosAño["Julio"] +
                    getPatologiasPieDiabeticoAño["Julio"];

                    TotalPatologiaAnualAgostoVertical = getPatologiasVascularesAño["Agosto"] + getPatologiasPosOperatorioAño["Agosto"] +
                    getPatologiasZPAño["Agosto"] + getPatologiasTraumaAño["Agosto"] + getPatologiasQuemadoAño["Agosto"] + getPatologiasOtrosAño["Agosto"] +
                    getPatologiasPieDiabeticoAño["Agosto"];

                    TotalPatologiaAnualSeptiembreVertical = getPatologiasVascularesAño["Septiembre"] + getPatologiasPosOperatorioAño["Septiembre"] +
                    getPatologiasZPAño["Septiembre"] + getPatologiasTraumaAño["Septiembre"] + getPatologiasQuemadoAño["Septiembre"] + getPatologiasOtrosAño["Septiembre"] +
                    getPatologiasPieDiabeticoAño["Septiembre"];

                    TotalPatologiaAnualOctubreVertical = getPatologiasVascularesAño["Octubre"] + getPatologiasPosOperatorioAño["Octubre"] +
                    getPatologiasZPAño["Octubre"] + getPatologiasTraumaAño["Octubre"] + getPatologiasQuemadoAño["Octubre"] + getPatologiasOtrosAño["Octubre"] +
                    getPatologiasPieDiabeticoAño["Octubre"];
                }
                else if (comboBox1.Text == "Noviembre" || comboBox1.Text == "NOVIEMBRE")
                {
                    TotalPatologiaAnualEneroVertical = getPatologiasVascularesAño["Enero"] + getPatologiasPosOperatorioAño["Enero"] +
                    getPatologiasZPAño["Enero"] + getPatologiasTraumaAño["Enero"] + getPatologiasQuemadoAño["Enero"] + getPatologiasOtrosAño["Enero"] +
                    getPatologiasPieDiabeticoAño["Enero"];

                    TotalPatologiaAnualFebreroVertical = getPatologiasVascularesAño["Febrero"] + getPatologiasPosOperatorioAño["Febrero"] +
                    getPatologiasZPAño["Febrero"] + getPatologiasTraumaAño["Febrero"] + getPatologiasQuemadoAño["Febrero"] + getPatologiasOtrosAño["Febrero"] +
                    getPatologiasPieDiabeticoAño["Febrero"];

                    TotalPatologiaAnualMarzoVertical = getPatologiasVascularesAño["Marzo"] + getPatologiasPosOperatorioAño["Marzo"] +
                    getPatologiasZPAño["Marzo"] + getPatologiasTraumaAño["Marzo"] + getPatologiasQuemadoAño["Marzo"] + getPatologiasOtrosAño["Marzo"] +
                    getPatologiasPieDiabeticoAño["Marzo"];

                    TotalPatologiaAnualAbrilVertical = getPatologiasVascularesAño["Abril"] + getPatologiasPosOperatorioAño["Abril"] +
                    getPatologiasZPAño["Abril"] + getPatologiasTraumaAño["Abril"] + getPatologiasQuemadoAño["Abril"] + getPatologiasOtrosAño["Abril"] +
                    getPatologiasPieDiabeticoAño["Abril"];

                    TotalPatologiaAnualMayoVertical = getPatologiasVascularesAño["Mayo"] + getPatologiasPosOperatorioAño["Mayo"] +
                    getPatologiasZPAño["Mayo"] + getPatologiasTraumaAño["Mayo"] + getPatologiasQuemadoAño["Mayo"] + getPatologiasOtrosAño["Mayo"] +
                    getPatologiasPieDiabeticoAño["Mayo"];

                    TotalPatologiaAnualJunioVertical = getPatologiasVascularesAño["Junio"] + getPatologiasPosOperatorioAño["Junio"] +
                    getPatologiasZPAño["Junio"] + getPatologiasTraumaAño["Junio"] + getPatologiasQuemadoAño["Junio"] + getPatologiasOtrosAño["Junio"] +
                    getPatologiasPieDiabeticoAño["Junio"];

                    TotalPatologiaAnualJulioVertical = getPatologiasVascularesAño["Julio"] + getPatologiasPosOperatorioAño["Julio"] +
                    getPatologiasZPAño["Julio"] + getPatologiasTraumaAño["Julio"] + getPatologiasQuemadoAño["Julio"] + getPatologiasOtrosAño["Julio"] +
                    getPatologiasPieDiabeticoAño["Julio"];

                    TotalPatologiaAnualAgostoVertical = getPatologiasVascularesAño["Agosto"] + getPatologiasPosOperatorioAño["Agosto"] +
                    getPatologiasZPAño["Agosto"] + getPatologiasTraumaAño["Agosto"] + getPatologiasQuemadoAño["Agosto"] + getPatologiasOtrosAño["Agosto"] +
                    getPatologiasPieDiabeticoAño["Agosto"];

                    TotalPatologiaAnualSeptiembreVertical = getPatologiasVascularesAño["Septiembre"] + getPatologiasPosOperatorioAño["Septiembre"] +
                    getPatologiasZPAño["Septiembre"] + getPatologiasTraumaAño["Septiembre"] + getPatologiasQuemadoAño["Septiembre"] + getPatologiasOtrosAño["Septiembre"] +
                    getPatologiasPieDiabeticoAño["Septiembre"];

                    TotalPatologiaAnualOctubreVertical = getPatologiasVascularesAño["Octubre"] + getPatologiasPosOperatorioAño["Octubre"] +
                    getPatologiasZPAño["Octubre"] + getPatologiasTraumaAño["Octubre"] + getPatologiasQuemadoAño["Octubre"] + getPatologiasOtrosAño["Octubre"] +
                    getPatologiasPieDiabeticoAño["Octubre"];

                    TotalPatologiaAnualNoviembreVertical = getPatologiasVascularesAño["Noviembre"] + getPatologiasPosOperatorioAño["Noviembre"] +
                    getPatologiasZPAño["Noviembre"] + getPatologiasTraumaAño["Noviembre"] + getPatologiasQuemadoAño["Noviembre"] + getPatologiasOtrosAño["Noviembre"] +
                    getPatologiasPieDiabeticoAño["Noviembre"];
                }
                else if (comboBox1.Text == "Diciembre" || comboBox1.Text == "DICIEMBRE")
                {
                    TotalPatologiaAnualEneroVertical = getPatologiasVascularesAño["Enero"] + getPatologiasPosOperatorioAño["Enero"] +
                    getPatologiasZPAño["Enero"] + getPatologiasTraumaAño["Enero"] + getPatologiasQuemadoAño["Enero"] + getPatologiasOtrosAño["Enero"] +
                    getPatologiasPieDiabeticoAño["Enero"];

                    TotalPatologiaAnualFebreroVertical = getPatologiasVascularesAño["Febrero"] + getPatologiasPosOperatorioAño["Febrero"] +
                    getPatologiasZPAño["Febrero"] + getPatologiasTraumaAño["Febrero"] + getPatologiasQuemadoAño["Febrero"] + getPatologiasOtrosAño["Febrero"] +
                    getPatologiasPieDiabeticoAño["Febrero"];

                    TotalPatologiaAnualMarzoVertical = getPatologiasVascularesAño["Marzo"] + getPatologiasPosOperatorioAño["Marzo"] +
                    getPatologiasZPAño["Marzo"] + getPatologiasTraumaAño["Marzo"] + getPatologiasQuemadoAño["Marzo"] + getPatologiasOtrosAño["Marzo"] +
                    getPatologiasPieDiabeticoAño["Marzo"];

                    TotalPatologiaAnualAbrilVertical = getPatologiasVascularesAño["Abril"] + getPatologiasPosOperatorioAño["Abril"] +
                    getPatologiasZPAño["Abril"] + getPatologiasTraumaAño["Abril"] + getPatologiasQuemadoAño["Abril"] + getPatologiasOtrosAño["Abril"] +
                    getPatologiasPieDiabeticoAño["Abril"];

                    TotalPatologiaAnualMayoVertical = getPatologiasVascularesAño["Mayo"] + getPatologiasPosOperatorioAño["Mayo"] +
                    getPatologiasZPAño["Mayo"] + getPatologiasTraumaAño["Mayo"] + getPatologiasQuemadoAño["Mayo"] + getPatologiasOtrosAño["Mayo"] +
                    getPatologiasPieDiabeticoAño["Mayo"];

                    TotalPatologiaAnualJunioVertical = getPatologiasVascularesAño["Junio"] + getPatologiasPosOperatorioAño["Junio"] +
                    getPatologiasZPAño["Junio"] + getPatologiasTraumaAño["Junio"] + getPatologiasQuemadoAño["Junio"] + getPatologiasOtrosAño["Junio"] +
                    getPatologiasPieDiabeticoAño["Junio"];

                    TotalPatologiaAnualJulioVertical = getPatologiasVascularesAño["Julio"] + getPatologiasPosOperatorioAño["Julio"] +
                    getPatologiasZPAño["Julio"] + getPatologiasTraumaAño["Julio"] + getPatologiasQuemadoAño["Julio"] + getPatologiasOtrosAño["Julio"] +
                    getPatologiasPieDiabeticoAño["Julio"];

                    TotalPatologiaAnualAgostoVertical = getPatologiasVascularesAño["Agosto"] + getPatologiasPosOperatorioAño["Agosto"] +
                    getPatologiasZPAño["Agosto"] + getPatologiasTraumaAño["Agosto"] + getPatologiasQuemadoAño["Agosto"] + getPatologiasOtrosAño["Agosto"] +
                    getPatologiasPieDiabeticoAño["Agosto"];

                    TotalPatologiaAnualSeptiembreVertical = getPatologiasVascularesAño["Septiembre"] + getPatologiasPosOperatorioAño["Septiembre"] +
                    getPatologiasZPAño["Septiembre"] + getPatologiasTraumaAño["Septiembre"] + getPatologiasQuemadoAño["Septiembre"] + getPatologiasOtrosAño["Septiembre"] +
                    getPatologiasPieDiabeticoAño["Septiembre"];

                    TotalPatologiaAnualOctubreVertical = getPatologiasVascularesAño["Octubre"] + getPatologiasPosOperatorioAño["Octubre"] +
                    getPatologiasZPAño["Octubre"] + getPatologiasTraumaAño["Octubre"] + getPatologiasQuemadoAño["Octubre"] + getPatologiasOtrosAño["Octubre"] +
                    getPatologiasPieDiabeticoAño["Octubre"];

                    TotalPatologiaAnualNoviembreVertical = getPatologiasVascularesAño["Noviembre"] + getPatologiasPosOperatorioAño["Noviembre"] +
                    getPatologiasZPAño["Noviembre"] + getPatologiasTraumaAño["Noviembre"] + getPatologiasQuemadoAño["Noviembre"] + getPatologiasOtrosAño["Noviembre"] +
                    getPatologiasPieDiabeticoAño["Noviembre"];

                    TotalPatologiaAnualDiciembreVertical = getPatologiasVascularesAño["Diciembre"] + getPatologiasPosOperatorioAño["Diciembre"] +
                    getPatologiasZPAño["Diciembre"] + getPatologiasTraumaAño["Diciembre"] + getPatologiasQuemadoAño["Diciembre"] + getPatologiasOtrosAño["Diciembre"] +
                    getPatologiasPieDiabeticoAño["Diciembre"];
                }             

                int TOTALPATOLOGIASANUAL = TotalPatologiaAnualEneroVertical + TotalPatologiaAnualFebreroVertical + TotalPatologiaAnualMarzoVertical +
                    TotalPatologiaAnualAbrilVertical + TotalPatologiaAnualMayoVertical + TotalPatologiaAnualJunioVertical + TotalPatologiaAnualJulioVertical +
                    TotalPatologiaAnualAgostoVertical + TotalPatologiaAnualSeptiembreVertical + TotalPatologiaAnualOctubreVertical +
                    TotalPatologiaAnualNoviembreVertical + TotalPatologiaAnualDiciembreVertical;

                double TotalEneroPatologiaPOPVertical = 0; double TotalFebreroPatologiaPOPVertical = 0; double TotalMarzoPatologiaPOPVertical = 0;
                double TotalAbrilPatologiaPOPVertical = 0; double TotalMayoPatologiaPOPVertical = 0; double TotalJunioPatologiaPOPVertical = 0;
                double TotalJulioPatologiaPOPVertical = 0; double TotalAgostoPatologiaPOPVertical = 0; double TotalSeptiembrePatologiaPOPVertical = 0;
                double TotalOctubrePatologiaPOPVertical = 0; double TotalNoviembrePatologiaPOPVertical = 0; double TotalDiciembrePatologiaPOPVertical = 0;

                double TotalEneroPatologiaZPVertical = 0; double TotalFebreroPatologiaZPVertical = 0; double TotalMarzoPatologiaZPVertical = 0;
                double TotalAbrilPatologiaZPVertical = 0; double TotalMayoPatologiaZPVertical = 0; double TotalJunioPatologiaZPVertical = 0;
                double TotalJulioPatologiaZPVertical = 0; double TotalAgostoPatologiaZPVertical = 0; double TotalSeptiembrePatologiaZPVertical = 0;
                double TotalOctubrePatologiaZPVertical = 0; double TotalNoviembrePatologiaZPVertical = 0; double TotalDiciembrePatologiaZPVertical = 0;

                double TotalEneroPatologiaPDVertical = 0; double TotalFebreroPatologiaPDVertical = 0; double TotalMarzoPatologiaPDVertical = 0;
                double TotalAbrilPatologiaPDVertical = 0; double TotalMayoPatologiaPDVertical = 0; double TotalJunioPatologiaPDVertical = 0;
                double TotalJulioPatologiaPDVertical = 0; double TotalAgostoPatologiaPDVertical = 0; double TotalSeptiembrePatologiaPDVertical = 0;
                double TotalOctubrePatologiaPDVertical = 0; double TotalNoviembrePatologiaPDVertical = 0; double TotalDiciembrePatologiaPDVertical = 0;

                double TotalEneroPatologiaQXVertical = 0; double TotalFebreroPatologiaQXVertical = 0; double TotalMarzoPatologiaQXVertical = 0;
                double TotalAbrilPatologiaQXVertical = 0; double TotalMayoPatologiaQXVertical = 0; double TotalJunioPatologiaQXVertical = 0;
                double TotalJulioPatologiaQXVertical = 0; double TotalAgostoPatologiaQXVertical = 0; double TotalSeptiembrePatologiaQXVertical = 0;
                double TotalOctubrePatologiaQXVertical = 0; double TotalNoviembrePatologiaQXVertical = 0; double TotalDiciembrePatologiaQXVertical = 0;

                double TotalEneroPatologiaTXVertical = 0; double TotalFebreroPatologiaTXVertical = 0; double TotalMarzoPatologiaTXVertical = 0;
                double TotalAbrilPatologiaTXVertical = 0; double TotalMayoPatologiaTXVertical = 0; double TotalJunioPatologiaTXVertical = 0;
                double TotalJulioPatologiaTXVertical = 0; double TotalAgostoPatologiaTXVertical = 0; double TotalSeptiembrePatologiaTXVertical = 0;
                double TotalOctubrePatologiaTXVertical = 0; double TotalNoviembrePatologiaTXVertical = 0; double TotalDiciembrePatologiaTXVertical = 0;

                double TotalEneroPatologiaUVVertical = 0; double TotalFebreroPatologiaUVVertical = 0; double TotalMarzoPatologiaUVVertical = 0;
                double TotalAbrilPatologiaUVVertical = 0; double TotalMayoPatologiaUVVertical = 0; double TotalJunioPatologiaUVVertical = 0;
                double TotalJulioPatologiaUVVertical = 0; double TotalAgostoPatologiaUVVertical = 0; double TotalSeptiembrePatologiaUVVertical = 0;
                double TotalOctubrePatologiaUVVertical = 0; double TotalNoviembrePatologiaUVVertical = 0; double TotalDiciembrePatologiaUVVertical = 0;

                double TotalEneroPatologiaOtrosVertical = 0; double TotalFebreroPatologiaOtrosVertical = 0; double TotalMarzoPatologiaOtrosVertical = 0;
                double TotalAbrilPatologiaOtrosVertical = 0; double TotalMayoPatologiaOtrosVertical = 0; double TotalJunioPatologiaOtrosVertical = 0;
                double TotalJulioPatologiaOtrosVertical = 0; double TotalAgostoPatologiaOtrosVertical = 0; double TotalSeptiembrePatologiaOtrosVertical = 0;
                double TotalOctubrePatologiaOtrosVertical = 0; double TotalNoviembrePatologiaOtrosVertical = 0; double TotalDiciembrePatologiaOtrosVertical = 0;

                if (comboBox1.Text == "Enero" || comboBox1.Text == "ENERO")
                {
                    TotalEneroPatologiaPOPVertical = (TotalPatologiaAnualEneroVertical <= 0 ? 0 : ((getPatologiasPosOperatorioAño["Enero"] * 100)) / TotalPatologiaAnualEneroVertical);
                    TotalEneroPatologiaZPVertical = (TotalPatologiaAnualEneroVertical <= 0 ? 0 : ((getPatologiasZPAño["Enero"] * 100)) / TotalPatologiaAnualEneroVertical);
                    TotalEneroPatologiaPDVertical = (TotalPatologiaAnualEneroVertical <= 0 ? 0 : ((getPatologiasPieDiabeticoAño["Enero"] * 100)) / TotalPatologiaAnualEneroVertical);
                    TotalEneroPatologiaQXVertical = (TotalPatologiaAnualEneroVertical <= 0 ? 0 : ((getPatologiasQuemadoAño["Enero"] * 100)) / TotalPatologiaAnualEneroVertical);
                    TotalEneroPatologiaTXVertical = (TotalPatologiaAnualEneroVertical <= 0 ? 0 : ((getPatologiasTraumaAño["Enero"] * 100)) / TotalPatologiaAnualEneroVertical);
                    TotalEneroPatologiaUVVertical = (TotalPatologiaAnualEneroVertical <= 0 ? 0 : ((getPatologiasVascularesAño["Enero"] * 100)) / TotalPatologiaAnualEneroVertical);
                    TotalEneroPatologiaOtrosVertical = (TotalPatologiaAnualEneroVertical <= 0 ? 0 : ((getPatologiasOtrosAño["Enero"] * 100)) / TotalPatologiaAnualEneroVertical);
                }
                else if (comboBox1.Text == "Febrero" || comboBox1.Text == "FEBRERO")
                {
                    TotalEneroPatologiaPOPVertical = (TotalPatologiaAnualEneroVertical <= 0 ? 0 : ((getPatologiasPosOperatorioAño["Enero"] * 100)) / TotalPatologiaAnualEneroVertical);
                    TotalEneroPatologiaZPVertical = (TotalPatologiaAnualEneroVertical <= 0 ? 0 : ((getPatologiasZPAño["Enero"] * 100)) / TotalPatologiaAnualEneroVertical);
                    TotalEneroPatologiaPDVertical = (TotalPatologiaAnualEneroVertical <= 0 ? 0 : ((getPatologiasPieDiabeticoAño["Enero"] * 100)) / TotalPatologiaAnualEneroVertical);
                    TotalEneroPatologiaQXVertical = (TotalPatologiaAnualEneroVertical <= 0 ? 0 : ((getPatologiasQuemadoAño["Enero"] * 100)) / TotalPatologiaAnualEneroVertical);
                    TotalEneroPatologiaTXVertical = (TotalPatologiaAnualEneroVertical <= 0 ? 0 : ((getPatologiasTraumaAño["Enero"] * 100)) / TotalPatologiaAnualEneroVertical);
                    TotalEneroPatologiaUVVertical = (TotalPatologiaAnualEneroVertical <= 0 ? 0 : ((getPatologiasVascularesAño["Enero"] * 100)) / TotalPatologiaAnualEneroVertical);
                    TotalEneroPatologiaOtrosVertical = (TotalPatologiaAnualEneroVertical <= 0 ? 0 : ((getPatologiasOtrosAño["Enero"] * 100)) / TotalPatologiaAnualEneroVertical);

                    TotalFebreroPatologiaPOPVertical = (TotalPatologiaAnualFebreroVertical <= 0 ? 0 : ((getPatologiasPosOperatorioAño["Febrero"] * 100)) / TotalPatologiaAnualFebreroVertical);
                    TotalFebreroPatologiaZPVertical = (TotalPatologiaAnualFebreroVertical <= 0 ? 0 : ((getPatologiasZPAño["Febrero"] * 100)) / TotalPatologiaAnualFebreroVertical);
                    TotalFebreroPatologiaPDVertical = (TotalPatologiaAnualFebreroVertical <= 0 ? 0 : ((getPatologiasPieDiabeticoAño["Febrero"] * 100)) / TotalPatologiaAnualFebreroVertical);
                    TotalFebreroPatologiaQXVertical = (TotalPatologiaAnualFebreroVertical <= 0 ? 0 : ((getPatologiasQuemadoAño["Febrero"] * 100)) / TotalPatologiaAnualFebreroVertical);
                    TotalFebreroPatologiaTXVertical = (TotalPatologiaAnualFebreroVertical <= 0 ? 0 : ((getPatologiasTraumaAño["Febrero"] * 100)) / TotalPatologiaAnualFebreroVertical);
                    TotalFebreroPatologiaUVVertical = (TotalPatologiaAnualFebreroVertical <= 0 ? 0 : ((getPatologiasVascularesAño["Febrero"] * 100)) / TotalPatologiaAnualFebreroVertical);
                    TotalFebreroPatologiaOtrosVertical = (TotalPatologiaAnualFebreroVertical <= 0 ? 0 : ((getPatologiasOtrosAño["Febrero"] * 100)) / TotalPatologiaAnualFebreroVertical);
                }
                else if (comboBox1.Text == "Marzo" || comboBox1.Text == "MARZO")
                {
                    TotalEneroPatologiaPOPVertical = (TotalPatologiaAnualEneroVertical <= 0 ? 0 : ((getPatologiasPosOperatorioAño["Enero"] * 100)) / TotalPatologiaAnualEneroVertical);
                    TotalEneroPatologiaZPVertical = (TotalPatologiaAnualEneroVertical <= 0 ? 0 : ((getPatologiasZPAño["Enero"] * 100)) / TotalPatologiaAnualEneroVertical);
                    TotalEneroPatologiaPDVertical = (TotalPatologiaAnualEneroVertical <= 0 ? 0 : ((getPatologiasPieDiabeticoAño["Enero"] * 100)) / TotalPatologiaAnualEneroVertical);
                    TotalEneroPatologiaQXVertical = (TotalPatologiaAnualEneroVertical <= 0 ? 0 : ((getPatologiasQuemadoAño["Enero"] * 100)) / TotalPatologiaAnualEneroVertical);
                    TotalEneroPatologiaTXVertical = (TotalPatologiaAnualEneroVertical <= 0 ? 0 : ((getPatologiasTraumaAño["Enero"] * 100)) / TotalPatologiaAnualEneroVertical);
                    TotalEneroPatologiaUVVertical = (TotalPatologiaAnualEneroVertical <= 0 ? 0 : ((getPatologiasVascularesAño["Enero"] * 100)) / TotalPatologiaAnualEneroVertical);
                    TotalEneroPatologiaOtrosVertical = (TotalPatologiaAnualEneroVertical <= 0 ? 0 : ((getPatologiasOtrosAño["Enero"] * 100)) / TotalPatologiaAnualEneroVertical);

                    TotalFebreroPatologiaPOPVertical = (TotalPatologiaAnualFebreroVertical <= 0 ? 0 : ((getPatologiasPosOperatorioAño["Febrero"] * 100)) / TotalPatologiaAnualFebreroVertical);
                    TotalFebreroPatologiaZPVertical = (TotalPatologiaAnualFebreroVertical <= 0 ? 0 : ((getPatologiasZPAño["Febrero"] * 100)) / TotalPatologiaAnualFebreroVertical);
                    TotalFebreroPatologiaPDVertical = (TotalPatologiaAnualFebreroVertical <= 0 ? 0 : ((getPatologiasPieDiabeticoAño["Febrero"] * 100)) / TotalPatologiaAnualFebreroVertical);
                    TotalFebreroPatologiaQXVertical = (TotalPatologiaAnualFebreroVertical <= 0 ? 0 : ((getPatologiasQuemadoAño["Febrero"] * 100)) / TotalPatologiaAnualFebreroVertical);
                    TotalFebreroPatologiaTXVertical = (TotalPatologiaAnualFebreroVertical <= 0 ? 0 : ((getPatologiasTraumaAño["Febrero"] * 100)) / TotalPatologiaAnualFebreroVertical);
                    TotalFebreroPatologiaUVVertical = (TotalPatologiaAnualFebreroVertical <= 0 ? 0 : ((getPatologiasVascularesAño["Febrero"] * 100)) / TotalPatologiaAnualFebreroVertical);
                    TotalFebreroPatologiaOtrosVertical = (TotalPatologiaAnualFebreroVertical <= 0 ? 0 : ((getPatologiasOtrosAño["Febrero"] * 100)) / TotalPatologiaAnualFebreroVertical);

                    TotalMarzoPatologiaPOPVertical = (TotalPatologiaAnualMarzoVertical <= 0 ? 0 : ((getPatologiasPosOperatorioAño["Marzo"] * 100)) / TotalPatologiaAnualMarzoVertical);
                    TotalMarzoPatologiaZPVertical = (TotalPatologiaAnualMarzoVertical <= 0 ? 0 : ((getPatologiasZPAño["Marzo"] * 100)) / TotalPatologiaAnualMarzoVertical);
                    TotalMarzoPatologiaPDVertical = (TotalPatologiaAnualMarzoVertical <= 0 ? 0 : ((getPatologiasPieDiabeticoAño["Marzo"] * 100)) / TotalPatologiaAnualMarzoVertical);
                    TotalMarzoPatologiaQXVertical = (TotalPatologiaAnualMarzoVertical <= 0 ? 0 : ((getPatologiasQuemadoAño["Marzo"] * 100)) / TotalPatologiaAnualMarzoVertical);
                    TotalMarzoPatologiaTXVertical = (TotalPatologiaAnualMarzoVertical <= 0 ? 0 : ((getPatologiasTraumaAño["Marzo"] * 100)) / TotalPatologiaAnualMarzoVertical);
                    TotalMarzoPatologiaUVVertical = (TotalPatologiaAnualMarzoVertical <= 0 ? 0 : ((getPatologiasVascularesAño["Marzo"] * 100)) / TotalPatologiaAnualMarzoVertical);
                    TotalMarzoPatologiaOtrosVertical = (TotalPatologiaAnualMarzoVertical <= 0 ? 0 : ((getPatologiasOtrosAño["Marzo"] * 100)) / TotalPatologiaAnualMarzoVertical);
                }
                else if (comboBox1.Text == "Abril" || comboBox1.Text == "ABRIL")
                {
                    TotalEneroPatologiaPOPVertical = (TotalPatologiaAnualEneroVertical <= 0 ? 0 : ((getPatologiasPosOperatorioAño["Enero"] * 100)) / TotalPatologiaAnualEneroVertical);
                    TotalEneroPatologiaZPVertical = (TotalPatologiaAnualEneroVertical <= 0 ? 0 : ((getPatologiasZPAño["Enero"] * 100)) / TotalPatologiaAnualEneroVertical);
                    TotalEneroPatologiaPDVertical = (TotalPatologiaAnualEneroVertical <= 0 ? 0 : ((getPatologiasPieDiabeticoAño["Enero"] * 100)) / TotalPatologiaAnualEneroVertical);
                    TotalEneroPatologiaQXVertical = (TotalPatologiaAnualEneroVertical <= 0 ? 0 : ((getPatologiasQuemadoAño["Enero"] * 100)) / TotalPatologiaAnualEneroVertical);
                    TotalEneroPatologiaTXVertical = (TotalPatologiaAnualEneroVertical <= 0 ? 0 : ((getPatologiasTraumaAño["Enero"] * 100)) / TotalPatologiaAnualEneroVertical);
                    TotalEneroPatologiaUVVertical = (TotalPatologiaAnualEneroVertical <= 0 ? 0 : ((getPatologiasVascularesAño["Enero"] * 100)) / TotalPatologiaAnualEneroVertical);
                    TotalEneroPatologiaOtrosVertical = (TotalPatologiaAnualEneroVertical <= 0 ? 0 : ((getPatologiasOtrosAño["Enero"] * 100)) / TotalPatologiaAnualEneroVertical);

                    TotalFebreroPatologiaPOPVertical = (TotalPatologiaAnualFebreroVertical <= 0 ? 0 : ((getPatologiasPosOperatorioAño["Febrero"] * 100)) / TotalPatologiaAnualFebreroVertical);
                    TotalFebreroPatologiaZPVertical = (TotalPatologiaAnualFebreroVertical <= 0 ? 0 : ((getPatologiasZPAño["Febrero"] * 100)) / TotalPatologiaAnualFebreroVertical);
                    TotalFebreroPatologiaPDVertical = (TotalPatologiaAnualFebreroVertical <= 0 ? 0 : ((getPatologiasPieDiabeticoAño["Febrero"] * 100)) / TotalPatologiaAnualFebreroVertical);
                    TotalFebreroPatologiaQXVertical = (TotalPatologiaAnualFebreroVertical <= 0 ? 0 : ((getPatologiasQuemadoAño["Febrero"] * 100)) / TotalPatologiaAnualFebreroVertical);
                    TotalFebreroPatologiaTXVertical = (TotalPatologiaAnualFebreroVertical <= 0 ? 0 : ((getPatologiasTraumaAño["Febrero"] * 100)) / TotalPatologiaAnualFebreroVertical);
                    TotalFebreroPatologiaUVVertical = (TotalPatologiaAnualFebreroVertical <= 0 ? 0 : ((getPatologiasVascularesAño["Febrero"] * 100)) / TotalPatologiaAnualFebreroVertical);
                    TotalFebreroPatologiaOtrosVertical = (TotalPatologiaAnualFebreroVertical <= 0 ? 0 : ((getPatologiasOtrosAño["Febrero"] * 100)) / TotalPatologiaAnualFebreroVertical);

                    TotalMarzoPatologiaPOPVertical = (TotalPatologiaAnualMarzoVertical <= 0 ? 0 : ((getPatologiasPosOperatorioAño["Marzo"] * 100)) / TotalPatologiaAnualMarzoVertical);
                    TotalMarzoPatologiaZPVertical = (TotalPatologiaAnualMarzoVertical <= 0 ? 0 : ((getPatologiasZPAño["Marzo"] * 100)) / TotalPatologiaAnualMarzoVertical);
                    TotalMarzoPatologiaPDVertical = (TotalPatologiaAnualMarzoVertical <= 0 ? 0 : ((getPatologiasPieDiabeticoAño["Marzo"] * 100)) / TotalPatologiaAnualMarzoVertical);
                    TotalMarzoPatologiaQXVertical = (TotalPatologiaAnualMarzoVertical <= 0 ? 0 : ((getPatologiasQuemadoAño["Marzo"] * 100)) / TotalPatologiaAnualMarzoVertical);
                    TotalMarzoPatologiaTXVertical = (TotalPatologiaAnualMarzoVertical <= 0 ? 0 : ((getPatologiasTraumaAño["Marzo"] * 100)) / TotalPatologiaAnualMarzoVertical);
                    TotalMarzoPatologiaUVVertical = (TotalPatologiaAnualMarzoVertical <= 0 ? 0 : ((getPatologiasVascularesAño["Marzo"] * 100)) / TotalPatologiaAnualMarzoVertical);
                    TotalMarzoPatologiaOtrosVertical = (TotalPatologiaAnualMarzoVertical <= 0 ? 0 : ((getPatologiasOtrosAño["Marzo"] * 100)) / TotalPatologiaAnualMarzoVertical);

                    TotalAbrilPatologiaPOPVertical = (TotalPatologiaAnualAbrilVertical <= 0 ? 0 : ((getPatologiasPosOperatorioAño["Abril"] * 100)) / TotalPatologiaAnualAbrilVertical);
                    TotalAbrilPatologiaZPVertical = (TotalPatologiaAnualAbrilVertical <= 0 ? 0 : ((getPatologiasZPAño["Abril"] * 100)) / TotalPatologiaAnualAbrilVertical);
                    TotalAbrilPatologiaPDVertical = (TotalPatologiaAnualAbrilVertical <= 0 ? 0 : ((getPatologiasPieDiabeticoAño["Abril"] * 100)) / TotalPatologiaAnualAbrilVertical);
                    TotalAbrilPatologiaQXVertical = (TotalPatologiaAnualAbrilVertical <= 0 ? 0 : ((getPatologiasQuemadoAño["Abril"] * 100)) / TotalPatologiaAnualAbrilVertical);
                    TotalAbrilPatologiaTXVertical = (TotalPatologiaAnualAbrilVertical <= 0 ? 0 : ((getPatologiasTraumaAño["Abril"] * 100)) / TotalPatologiaAnualAbrilVertical);
                    TotalAbrilPatologiaUVVertical = (TotalPatologiaAnualAbrilVertical <= 0 ? 0 : ((getPatologiasVascularesAño["Abril"] * 100)) / TotalPatologiaAnualAbrilVertical);
                    TotalAbrilPatologiaOtrosVertical = (TotalPatologiaAnualAbrilVertical <= 0 ? 0 : ((getPatologiasOtrosAño["Abril"] * 100)) / TotalPatologiaAnualAbrilVertical);
                }
                else if (comboBox1.Text == "Mayo" || comboBox1.Text == "MAYO")
                {
                    TotalEneroPatologiaPOPVertical = (TotalPatologiaAnualEneroVertical <= 0 ? 0 : ((getPatologiasPosOperatorioAño["Enero"] * 100)) / TotalPatologiaAnualEneroVertical);
                    TotalEneroPatologiaZPVertical = (TotalPatologiaAnualEneroVertical <= 0 ? 0 : ((getPatologiasZPAño["Enero"] * 100)) / TotalPatologiaAnualEneroVertical);
                    TotalEneroPatologiaPDVertical = (TotalPatologiaAnualEneroVertical <= 0 ? 0 : ((getPatologiasPieDiabeticoAño["Enero"] * 100)) / TotalPatologiaAnualEneroVertical);
                    TotalEneroPatologiaQXVertical = (TotalPatologiaAnualEneroVertical <= 0 ? 0 : ((getPatologiasQuemadoAño["Enero"] * 100)) / TotalPatologiaAnualEneroVertical);
                    TotalEneroPatologiaTXVertical = (TotalPatologiaAnualEneroVertical <= 0 ? 0 : ((getPatologiasTraumaAño["Enero"] * 100)) / TotalPatologiaAnualEneroVertical);
                    TotalEneroPatologiaUVVertical = (TotalPatologiaAnualEneroVertical <= 0 ? 0 : ((getPatologiasVascularesAño["Enero"] * 100)) / TotalPatologiaAnualEneroVertical);
                    TotalEneroPatologiaOtrosVertical = (TotalPatologiaAnualEneroVertical <= 0 ? 0 : ((getPatologiasOtrosAño["Enero"] * 100)) / TotalPatologiaAnualEneroVertical);

                    TotalFebreroPatologiaPOPVertical = (TotalPatologiaAnualFebreroVertical <= 0 ? 0 : ((getPatologiasPosOperatorioAño["Febrero"] * 100)) / TotalPatologiaAnualFebreroVertical);
                    TotalFebreroPatologiaZPVertical = (TotalPatologiaAnualFebreroVertical <= 0 ? 0 : ((getPatologiasZPAño["Febrero"] * 100)) / TotalPatologiaAnualFebreroVertical);
                    TotalFebreroPatologiaPDVertical = (TotalPatologiaAnualFebreroVertical <= 0 ? 0 : ((getPatologiasPieDiabeticoAño["Febrero"] * 100)) / TotalPatologiaAnualFebreroVertical);
                    TotalFebreroPatologiaQXVertical = (TotalPatologiaAnualFebreroVertical <= 0 ? 0 : ((getPatologiasQuemadoAño["Febrero"] * 100)) / TotalPatologiaAnualFebreroVertical);
                    TotalFebreroPatologiaTXVertical = (TotalPatologiaAnualFebreroVertical <= 0 ? 0 : ((getPatologiasTraumaAño["Febrero"] * 100)) / TotalPatologiaAnualFebreroVertical);
                    TotalFebreroPatologiaUVVertical = (TotalPatologiaAnualFebreroVertical <= 0 ? 0 : ((getPatologiasVascularesAño["Febrero"] * 100)) / TotalPatologiaAnualFebreroVertical);
                    TotalFebreroPatologiaOtrosVertical = (TotalPatologiaAnualFebreroVertical <= 0 ? 0 : ((getPatologiasOtrosAño["Febrero"] * 100)) / TotalPatologiaAnualFebreroVertical);

                    TotalMarzoPatologiaPOPVertical = (TotalPatologiaAnualMarzoVertical <= 0 ? 0 : ((getPatologiasPosOperatorioAño["Marzo"] * 100)) / TotalPatologiaAnualMarzoVertical);
                    TotalMarzoPatologiaZPVertical = (TotalPatologiaAnualMarzoVertical <= 0 ? 0 : ((getPatologiasZPAño["Marzo"] * 100)) / TotalPatologiaAnualMarzoVertical);
                    TotalMarzoPatologiaPDVertical = (TotalPatologiaAnualMarzoVertical <= 0 ? 0 : ((getPatologiasPieDiabeticoAño["Marzo"] * 100)) / TotalPatologiaAnualMarzoVertical);
                    TotalMarzoPatologiaQXVertical = (TotalPatologiaAnualMarzoVertical <= 0 ? 0 : ((getPatologiasQuemadoAño["Marzo"] * 100)) / TotalPatologiaAnualMarzoVertical);
                    TotalMarzoPatologiaTXVertical = (TotalPatologiaAnualMarzoVertical <= 0 ? 0 : ((getPatologiasTraumaAño["Marzo"] * 100)) / TotalPatologiaAnualMarzoVertical);
                    TotalMarzoPatologiaUVVertical = (TotalPatologiaAnualMarzoVertical <= 0 ? 0 : ((getPatologiasVascularesAño["Marzo"] * 100)) / TotalPatologiaAnualMarzoVertical);
                    TotalMarzoPatologiaOtrosVertical = (TotalPatologiaAnualMarzoVertical <= 0 ? 0 : ((getPatologiasOtrosAño["Marzo"] * 100)) / TotalPatologiaAnualMarzoVertical);

                    TotalAbrilPatologiaPOPVertical = (TotalPatologiaAnualAbrilVertical <= 0 ? 0 : ((getPatologiasPosOperatorioAño["Abril"] * 100)) / TotalPatologiaAnualAbrilVertical);
                    TotalAbrilPatologiaZPVertical = (TotalPatologiaAnualAbrilVertical <= 0 ? 0 : ((getPatologiasZPAño["Abril"] * 100)) / TotalPatologiaAnualAbrilVertical);
                    TotalAbrilPatologiaPDVertical = (TotalPatologiaAnualAbrilVertical <= 0 ? 0 : ((getPatologiasPieDiabeticoAño["Abril"] * 100)) / TotalPatologiaAnualAbrilVertical);
                    TotalAbrilPatologiaQXVertical = (TotalPatologiaAnualAbrilVertical <= 0 ? 0 : ((getPatologiasQuemadoAño["Abril"] * 100)) / TotalPatologiaAnualAbrilVertical);
                    TotalAbrilPatologiaTXVertical = (TotalPatologiaAnualAbrilVertical <= 0 ? 0 : ((getPatologiasTraumaAño["Abril"] * 100)) / TotalPatologiaAnualAbrilVertical);
                    TotalAbrilPatologiaUVVertical = (TotalPatologiaAnualAbrilVertical <= 0 ? 0 : ((getPatologiasVascularesAño["Abril"] * 100)) / TotalPatologiaAnualAbrilVertical);
                    TotalAbrilPatologiaOtrosVertical = (TotalPatologiaAnualAbrilVertical <= 0 ? 0 : ((getPatologiasOtrosAño["Abril"] * 100)) / TotalPatologiaAnualAbrilVertical);

                    TotalMayoPatologiaPOPVertical = (TotalPatologiaAnualMayoVertical <= 0 ? 0 : ((getPatologiasPosOperatorioAño["Mayo"] * 100)) / TotalPatologiaAnualMayoVertical);
                    TotalMayoPatologiaZPVertical = (TotalPatologiaAnualMayoVertical <= 0 ? 0 : ((getPatologiasZPAño["Mayo"] * 100)) / TotalPatologiaAnualMayoVertical);
                    TotalMayoPatologiaPDVertical = (TotalPatologiaAnualMayoVertical <= 0 ? 0 : ((getPatologiasPieDiabeticoAño["Mayo"] * 100)) / TotalPatologiaAnualMayoVertical);
                    TotalMayoPatologiaQXVertical = (TotalPatologiaAnualMayoVertical <= 0 ? 0 : ((getPatologiasQuemadoAño["Mayo"] * 100)) / TotalPatologiaAnualMayoVertical);
                    TotalMayoPatologiaTXVertical = (TotalPatologiaAnualMayoVertical <= 0 ? 0 : ((getPatologiasTraumaAño["Mayo"] * 100)) / TotalPatologiaAnualMayoVertical);
                    TotalMayoPatologiaUVVertical = (TotalPatologiaAnualMayoVertical <= 0 ? 0 : ((getPatologiasVascularesAño["Mayo"] * 100)) / TotalPatologiaAnualMayoVertical);
                    TotalMayoPatologiaOtrosVertical = (TotalPatologiaAnualMayoVertical <= 0 ? 0 : ((getPatologiasOtrosAño["Mayo"] * 100)) / TotalPatologiaAnualMayoVertical);
                }
                else if (comboBox1.Text == "Junio" || comboBox1.Text == "JUNIO")
                {
                    TotalEneroPatologiaPOPVertical = (TotalPatologiaAnualEneroVertical <= 0 ? 0 : ((getPatologiasPosOperatorioAño["Enero"] * 100)) / TotalPatologiaAnualEneroVertical);
                    TotalEneroPatologiaZPVertical = (TotalPatologiaAnualEneroVertical <= 0 ? 0 : ((getPatologiasZPAño["Enero"] * 100)) / TotalPatologiaAnualEneroVertical);
                    TotalEneroPatologiaPDVertical = (TotalPatologiaAnualEneroVertical <= 0 ? 0 : ((getPatologiasPieDiabeticoAño["Enero"] * 100)) / TotalPatologiaAnualEneroVertical);
                    TotalEneroPatologiaQXVertical = (TotalPatologiaAnualEneroVertical <= 0 ? 0 : ((getPatologiasQuemadoAño["Enero"] * 100)) / TotalPatologiaAnualEneroVertical);
                    TotalEneroPatologiaTXVertical = (TotalPatologiaAnualEneroVertical <= 0 ? 0 : ((getPatologiasTraumaAño["Enero"] * 100)) / TotalPatologiaAnualEneroVertical);
                    TotalEneroPatologiaUVVertical = (TotalPatologiaAnualEneroVertical <= 0 ? 0 : ((getPatologiasVascularesAño["Enero"] * 100)) / TotalPatologiaAnualEneroVertical);
                    TotalEneroPatologiaOtrosVertical = (TotalPatologiaAnualEneroVertical <= 0 ? 0 : ((getPatologiasOtrosAño["Enero"] * 100)) / TotalPatologiaAnualEneroVertical);

                    TotalFebreroPatologiaPOPVertical = (TotalPatologiaAnualFebreroVertical <= 0 ? 0 : ((getPatologiasPosOperatorioAño["Febrero"] * 100)) / TotalPatologiaAnualFebreroVertical);
                    TotalFebreroPatologiaZPVertical = (TotalPatologiaAnualFebreroVertical <= 0 ? 0 : ((getPatologiasZPAño["Febrero"] * 100)) / TotalPatologiaAnualFebreroVertical);
                    TotalFebreroPatologiaPDVertical = (TotalPatologiaAnualFebreroVertical <= 0 ? 0 : ((getPatologiasPieDiabeticoAño["Febrero"] * 100)) / TotalPatologiaAnualFebreroVertical);
                    TotalFebreroPatologiaQXVertical = (TotalPatologiaAnualFebreroVertical <= 0 ? 0 : ((getPatologiasQuemadoAño["Febrero"] * 100)) / TotalPatologiaAnualFebreroVertical);
                    TotalFebreroPatologiaTXVertical = (TotalPatologiaAnualFebreroVertical <= 0 ? 0 : ((getPatologiasTraumaAño["Febrero"] * 100)) / TotalPatologiaAnualFebreroVertical);
                    TotalFebreroPatologiaUVVertical = (TotalPatologiaAnualFebreroVertical <= 0 ? 0 : ((getPatologiasVascularesAño["Febrero"] * 100)) / TotalPatologiaAnualFebreroVertical);
                    TotalFebreroPatologiaOtrosVertical = (TotalPatologiaAnualFebreroVertical <= 0 ? 0 : ((getPatologiasOtrosAño["Febrero"] * 100)) / TotalPatologiaAnualFebreroVertical);

                    TotalMarzoPatologiaPOPVertical = (TotalPatologiaAnualMarzoVertical <= 0 ? 0 : ((getPatologiasPosOperatorioAño["Marzo"] * 100)) / TotalPatologiaAnualMarzoVertical);
                    TotalMarzoPatologiaZPVertical = (TotalPatologiaAnualMarzoVertical <= 0 ? 0 : ((getPatologiasZPAño["Marzo"] * 100)) / TotalPatologiaAnualMarzoVertical);
                    TotalMarzoPatologiaPDVertical = (TotalPatologiaAnualMarzoVertical <= 0 ? 0 : ((getPatologiasPieDiabeticoAño["Marzo"] * 100)) / TotalPatologiaAnualMarzoVertical);
                    TotalMarzoPatologiaQXVertical = (TotalPatologiaAnualMarzoVertical <= 0 ? 0 : ((getPatologiasQuemadoAño["Marzo"] * 100)) / TotalPatologiaAnualMarzoVertical);
                    TotalMarzoPatologiaTXVertical = (TotalPatologiaAnualMarzoVertical <= 0 ? 0 : ((getPatologiasTraumaAño["Marzo"] * 100)) / TotalPatologiaAnualMarzoVertical);
                    TotalMarzoPatologiaUVVertical = (TotalPatologiaAnualMarzoVertical <= 0 ? 0 : ((getPatologiasVascularesAño["Marzo"] * 100)) / TotalPatologiaAnualMarzoVertical);
                    TotalMarzoPatologiaOtrosVertical = (TotalPatologiaAnualMarzoVertical <= 0 ? 0 : ((getPatologiasOtrosAño["Marzo"] * 100)) / TotalPatologiaAnualMarzoVertical);

                    TotalAbrilPatologiaPOPVertical = (TotalPatologiaAnualAbrilVertical <= 0 ? 0 : ((getPatologiasPosOperatorioAño["Abril"] * 100)) / TotalPatologiaAnualAbrilVertical);
                    TotalAbrilPatologiaZPVertical = (TotalPatologiaAnualAbrilVertical <= 0 ? 0 : ((getPatologiasZPAño["Abril"] * 100)) / TotalPatologiaAnualAbrilVertical);
                    TotalAbrilPatologiaPDVertical = (TotalPatologiaAnualAbrilVertical <= 0 ? 0 : ((getPatologiasPieDiabeticoAño["Abril"] * 100)) / TotalPatologiaAnualAbrilVertical);
                    TotalAbrilPatologiaQXVertical = (TotalPatologiaAnualAbrilVertical <= 0 ? 0 : ((getPatologiasQuemadoAño["Abril"] * 100)) / TotalPatologiaAnualAbrilVertical);
                    TotalAbrilPatologiaTXVertical = (TotalPatologiaAnualAbrilVertical <= 0 ? 0 : ((getPatologiasTraumaAño["Abril"] * 100)) / TotalPatologiaAnualAbrilVertical);
                    TotalAbrilPatologiaUVVertical = (TotalPatologiaAnualAbrilVertical <= 0 ? 0 : ((getPatologiasVascularesAño["Abril"] * 100)) / TotalPatologiaAnualAbrilVertical);
                    TotalAbrilPatologiaOtrosVertical = (TotalPatologiaAnualAbrilVertical <= 0 ? 0 : ((getPatologiasOtrosAño["Abril"] * 100)) / TotalPatologiaAnualAbrilVertical);

                    TotalMayoPatologiaPOPVertical = (TotalPatologiaAnualMayoVertical <= 0 ? 0 : ((getPatologiasPosOperatorioAño["Mayo"] * 100)) / TotalPatologiaAnualMayoVertical);
                    TotalMayoPatologiaZPVertical = (TotalPatologiaAnualMayoVertical <= 0 ? 0 : ((getPatologiasZPAño["Mayo"] * 100)) / TotalPatologiaAnualMayoVertical);
                    TotalMayoPatologiaPDVertical = (TotalPatologiaAnualMayoVertical <= 0 ? 0 : ((getPatologiasPieDiabeticoAño["Mayo"] * 100)) / TotalPatologiaAnualMayoVertical);
                    TotalMayoPatologiaQXVertical = (TotalPatologiaAnualMayoVertical <= 0 ? 0 : ((getPatologiasQuemadoAño["Mayo"] * 100)) / TotalPatologiaAnualMayoVertical);
                    TotalMayoPatologiaTXVertical = (TotalPatologiaAnualMayoVertical <= 0 ? 0 : ((getPatologiasTraumaAño["Mayo"] * 100)) / TotalPatologiaAnualMayoVertical);
                    TotalMayoPatologiaUVVertical = (TotalPatologiaAnualMayoVertical <= 0 ? 0 : ((getPatologiasVascularesAño["Mayo"] * 100)) / TotalPatologiaAnualMayoVertical);
                    TotalMayoPatologiaOtrosVertical = (TotalPatologiaAnualMayoVertical <= 0 ? 0 : ((getPatologiasOtrosAño["Mayo"] * 100)) / TotalPatologiaAnualMayoVertical);

                    TotalJunioPatologiaPOPVertical = (TotalPatologiaAnualJunioVertical <= 0 ? 0 : ((getPatologiasPosOperatorioAño["Junio"] * 100)) / TotalPatologiaAnualJunioVertical);
                    TotalJunioPatologiaZPVertical = (TotalPatologiaAnualJunioVertical <= 0 ? 0 : ((getPatologiasZPAño["Junio"] * 100)) / TotalPatologiaAnualJunioVertical);
                    TotalJunioPatologiaPDVertical = (TotalPatologiaAnualJunioVertical <= 0 ? 0 : ((getPatologiasPieDiabeticoAño["Junio"] * 100)) / TotalPatologiaAnualJunioVertical);
                    TotalJunioPatologiaQXVertical = (TotalPatologiaAnualJunioVertical <= 0 ? 0 : ((getPatologiasQuemadoAño["Junio"] * 100)) / TotalPatologiaAnualJunioVertical);
                    TotalJunioPatologiaTXVertical = (TotalPatologiaAnualJunioVertical <= 0 ? 0 : ((getPatologiasTraumaAño["Junio"] * 100)) / TotalPatologiaAnualJunioVertical);
                    TotalJunioPatologiaUVVertical = (TotalPatologiaAnualJunioVertical <= 0 ? 0 : ((getPatologiasVascularesAño["Junio"] * 100)) / TotalPatologiaAnualJunioVertical);
                    TotalJunioPatologiaOtrosVertical = (TotalPatologiaAnualJunioVertical <= 0 ? 0 : ((getPatologiasOtrosAño["Junio"] * 100)) / TotalPatologiaAnualJunioVertical);
                }
                else if (comboBox1.Text == "Julio" || comboBox1.Text == "JULIO")
                {
                    TotalEneroPatologiaPOPVertical = (TotalPatologiaAnualEneroVertical <= 0 ? 0 : ((getPatologiasPosOperatorioAño["Enero"] * 100)) / TotalPatologiaAnualEneroVertical);
                    TotalEneroPatologiaZPVertical = (TotalPatologiaAnualEneroVertical <= 0 ? 0 : ((getPatologiasZPAño["Enero"] * 100)) / TotalPatologiaAnualEneroVertical);
                    TotalEneroPatologiaPDVertical = (TotalPatologiaAnualEneroVertical <= 0 ? 0 : ((getPatologiasPieDiabeticoAño["Enero"] * 100)) / TotalPatologiaAnualEneroVertical);
                    TotalEneroPatologiaQXVertical = (TotalPatologiaAnualEneroVertical <= 0 ? 0 : ((getPatologiasQuemadoAño["Enero"] * 100)) / TotalPatologiaAnualEneroVertical);
                    TotalEneroPatologiaTXVertical = (TotalPatologiaAnualEneroVertical <= 0 ? 0 : ((getPatologiasTraumaAño["Enero"] * 100)) / TotalPatologiaAnualEneroVertical);
                    TotalEneroPatologiaUVVertical = (TotalPatologiaAnualEneroVertical <= 0 ? 0 : ((getPatologiasVascularesAño["Enero"] * 100)) / TotalPatologiaAnualEneroVertical);
                    TotalEneroPatologiaOtrosVertical = (TotalPatologiaAnualEneroVertical <= 0 ? 0 : ((getPatologiasOtrosAño["Enero"] * 100)) / TotalPatologiaAnualEneroVertical);

                    TotalFebreroPatologiaPOPVertical = (TotalPatologiaAnualFebreroVertical <= 0 ? 0 : ((getPatologiasPosOperatorioAño["Febrero"] * 100)) / TotalPatologiaAnualFebreroVertical);
                    TotalFebreroPatologiaZPVertical = (TotalPatologiaAnualFebreroVertical <= 0 ? 0 : ((getPatologiasZPAño["Febrero"] * 100)) / TotalPatologiaAnualFebreroVertical);
                    TotalFebreroPatologiaPDVertical = (TotalPatologiaAnualFebreroVertical <= 0 ? 0 : ((getPatologiasPieDiabeticoAño["Febrero"] * 100)) / TotalPatologiaAnualFebreroVertical);
                    TotalFebreroPatologiaQXVertical = (TotalPatologiaAnualFebreroVertical <= 0 ? 0 : ((getPatologiasQuemadoAño["Febrero"] * 100)) / TotalPatologiaAnualFebreroVertical);
                    TotalFebreroPatologiaTXVertical = (TotalPatologiaAnualFebreroVertical <= 0 ? 0 : ((getPatologiasTraumaAño["Febrero"] * 100)) / TotalPatologiaAnualFebreroVertical);
                    TotalFebreroPatologiaUVVertical = (TotalPatologiaAnualFebreroVertical <= 0 ? 0 : ((getPatologiasVascularesAño["Febrero"] * 100)) / TotalPatologiaAnualFebreroVertical);
                    TotalFebreroPatologiaOtrosVertical = (TotalPatologiaAnualFebreroVertical <= 0 ? 0 : ((getPatologiasOtrosAño["Febrero"] * 100)) / TotalPatologiaAnualFebreroVertical);

                    TotalMarzoPatologiaPOPVertical = (TotalPatologiaAnualMarzoVertical <= 0 ? 0 : ((getPatologiasPosOperatorioAño["Marzo"] * 100)) / TotalPatologiaAnualMarzoVertical);
                    TotalMarzoPatologiaZPVertical = (TotalPatologiaAnualMarzoVertical <= 0 ? 0 : ((getPatologiasZPAño["Marzo"] * 100)) / TotalPatologiaAnualMarzoVertical);
                    TotalMarzoPatologiaPDVertical = (TotalPatologiaAnualMarzoVertical <= 0 ? 0 : ((getPatologiasPieDiabeticoAño["Marzo"] * 100)) / TotalPatologiaAnualMarzoVertical);
                    TotalMarzoPatologiaQXVertical = (TotalPatologiaAnualMarzoVertical <= 0 ? 0 : ((getPatologiasQuemadoAño["Marzo"] * 100)) / TotalPatologiaAnualMarzoVertical);
                    TotalMarzoPatologiaTXVertical = (TotalPatologiaAnualMarzoVertical <= 0 ? 0 : ((getPatologiasTraumaAño["Marzo"] * 100)) / TotalPatologiaAnualMarzoVertical);
                    TotalMarzoPatologiaUVVertical = (TotalPatologiaAnualMarzoVertical <= 0 ? 0 : ((getPatologiasVascularesAño["Marzo"] * 100)) / TotalPatologiaAnualMarzoVertical);
                    TotalMarzoPatologiaOtrosVertical = (TotalPatologiaAnualMarzoVertical <= 0 ? 0 : ((getPatologiasOtrosAño["Marzo"] * 100)) / TotalPatologiaAnualMarzoVertical);

                    TotalAbrilPatologiaPOPVertical = (TotalPatologiaAnualAbrilVertical <= 0 ? 0 : ((getPatologiasPosOperatorioAño["Abril"] * 100)) / TotalPatologiaAnualAbrilVertical);
                    TotalAbrilPatologiaZPVertical = (TotalPatologiaAnualAbrilVertical <= 0 ? 0 : ((getPatologiasZPAño["Abril"] * 100)) / TotalPatologiaAnualAbrilVertical);
                    TotalAbrilPatologiaPDVertical = (TotalPatologiaAnualAbrilVertical <= 0 ? 0 : ((getPatologiasPieDiabeticoAño["Abril"] * 100)) / TotalPatologiaAnualAbrilVertical);
                    TotalAbrilPatologiaQXVertical = (TotalPatologiaAnualAbrilVertical <= 0 ? 0 : ((getPatologiasQuemadoAño["Abril"] * 100)) / TotalPatologiaAnualAbrilVertical);
                    TotalAbrilPatologiaTXVertical = (TotalPatologiaAnualAbrilVertical <= 0 ? 0 : ((getPatologiasTraumaAño["Abril"] * 100)) / TotalPatologiaAnualAbrilVertical);
                    TotalAbrilPatologiaUVVertical = (TotalPatologiaAnualAbrilVertical <= 0 ? 0 : ((getPatologiasVascularesAño["Abril"] * 100)) / TotalPatologiaAnualAbrilVertical);
                    TotalAbrilPatologiaOtrosVertical = (TotalPatologiaAnualAbrilVertical <= 0 ? 0 : ((getPatologiasOtrosAño["Abril"] * 100)) / TotalPatologiaAnualAbrilVertical);

                    TotalMayoPatologiaPOPVertical = (TotalPatologiaAnualMayoVertical <= 0 ? 0 : ((getPatologiasPosOperatorioAño["Mayo"] * 100)) / TotalPatologiaAnualMayoVertical);
                    TotalMayoPatologiaZPVertical = (TotalPatologiaAnualMayoVertical <= 0 ? 0 : ((getPatologiasZPAño["Mayo"] * 100)) / TotalPatologiaAnualMayoVertical);
                    TotalMayoPatologiaPDVertical = (TotalPatologiaAnualMayoVertical <= 0 ? 0 : ((getPatologiasPieDiabeticoAño["Mayo"] * 100)) / TotalPatologiaAnualMayoVertical);
                    TotalMayoPatologiaQXVertical = (TotalPatologiaAnualMayoVertical <= 0 ? 0 : ((getPatologiasQuemadoAño["Mayo"] * 100)) / TotalPatologiaAnualMayoVertical);
                    TotalMayoPatologiaTXVertical = (TotalPatologiaAnualMayoVertical <= 0 ? 0 : ((getPatologiasTraumaAño["Mayo"] * 100)) / TotalPatologiaAnualMayoVertical);
                    TotalMayoPatologiaUVVertical = (TotalPatologiaAnualMayoVertical <= 0 ? 0 : ((getPatologiasVascularesAño["Mayo"] * 100)) / TotalPatologiaAnualMayoVertical);
                    TotalMayoPatologiaOtrosVertical = (TotalPatologiaAnualMayoVertical <= 0 ? 0 : ((getPatologiasOtrosAño["Mayo"] * 100)) / TotalPatologiaAnualMayoVertical);

                    TotalJunioPatologiaPOPVertical = (TotalPatologiaAnualJunioVertical <= 0 ? 0 : ((getPatologiasPosOperatorioAño["Junio"] * 100)) / TotalPatologiaAnualJunioVertical);
                    TotalJunioPatologiaZPVertical = (TotalPatologiaAnualJunioVertical <= 0 ? 0 : ((getPatologiasZPAño["Junio"] * 100)) / TotalPatologiaAnualJunioVertical);
                    TotalJunioPatologiaPDVertical = (TotalPatologiaAnualJunioVertical <= 0 ? 0 : ((getPatologiasPieDiabeticoAño["Junio"] * 100)) / TotalPatologiaAnualJunioVertical);
                    TotalJunioPatologiaQXVertical = (TotalPatologiaAnualJunioVertical <= 0 ? 0 : ((getPatologiasQuemadoAño["Junio"] * 100)) / TotalPatologiaAnualJunioVertical);
                    TotalJunioPatologiaTXVertical = (TotalPatologiaAnualJunioVertical <= 0 ? 0 : ((getPatologiasTraumaAño["Junio"] * 100)) / TotalPatologiaAnualJunioVertical);
                    TotalJunioPatologiaUVVertical = (TotalPatologiaAnualJunioVertical <= 0 ? 0 : ((getPatologiasVascularesAño["Junio"] * 100)) / TotalPatologiaAnualJunioVertical);
                    TotalJunioPatologiaOtrosVertical = (TotalPatologiaAnualJunioVertical <= 0 ? 0 : ((getPatologiasOtrosAño["Junio"] * 100)) / TotalPatologiaAnualJunioVertical);

                    TotalJulioPatologiaPOPVertical = (TotalPatologiaAnualJulioVertical <= 0 ? 0 : ((getPatologiasPosOperatorioAño["Julio"] * 100)) / TotalPatologiaAnualJulioVertical);
                    TotalJulioPatologiaZPVertical = (TotalPatologiaAnualJulioVertical <= 0 ? 0 : ((getPatologiasZPAño["Julio"] * 100)) / TotalPatologiaAnualJulioVertical);
                    TotalJulioPatologiaPDVertical = (TotalPatologiaAnualJulioVertical <= 0 ? 0 : ((getPatologiasPieDiabeticoAño["Julio"] * 100)) / TotalPatologiaAnualJulioVertical);
                    TotalJulioPatologiaQXVertical = (TotalPatologiaAnualJulioVertical <= 0 ? 0 : ((getPatologiasQuemadoAño["Julio"] * 100)) / TotalPatologiaAnualJulioVertical);
                    TotalJulioPatologiaTXVertical = (TotalPatologiaAnualJulioVertical <= 0 ? 0 : ((getPatologiasTraumaAño["Julio"] * 100)) / TotalPatologiaAnualJulioVertical);
                    TotalJulioPatologiaUVVertical = (TotalPatologiaAnualJulioVertical <= 0 ? 0 : ((getPatologiasVascularesAño["Julio"] * 100)) / TotalPatologiaAnualJulioVertical);
                    TotalJulioPatologiaOtrosVertical = (TotalPatologiaAnualJulioVertical <= 0 ? 0 : ((getPatologiasOtrosAño["Julio"] * 100)) / TotalPatologiaAnualJulioVertical);
                }
                else if (comboBox1.Text == "Agosto" || comboBox1.Text == "AGOSTO")
                {
                    TotalEneroPatologiaPOPVertical = (TotalPatologiaAnualEneroVertical <= 0 ? 0 : ((getPatologiasPosOperatorioAño["Enero"] * 100)) / TotalPatologiaAnualEneroVertical);
                    TotalEneroPatologiaZPVertical = (TotalPatologiaAnualEneroVertical <= 0 ? 0 : ((getPatologiasZPAño["Enero"] * 100)) / TotalPatologiaAnualEneroVertical);
                    TotalEneroPatologiaPDVertical = (TotalPatologiaAnualEneroVertical <= 0 ? 0 : ((getPatologiasPieDiabeticoAño["Enero"] * 100)) / TotalPatologiaAnualEneroVertical);
                    TotalEneroPatologiaQXVertical = (TotalPatologiaAnualEneroVertical <= 0 ? 0 : ((getPatologiasQuemadoAño["Enero"] * 100)) / TotalPatologiaAnualEneroVertical);
                    TotalEneroPatologiaTXVertical = (TotalPatologiaAnualEneroVertical <= 0 ? 0 : ((getPatologiasTraumaAño["Enero"] * 100)) / TotalPatologiaAnualEneroVertical);
                    TotalEneroPatologiaUVVertical = (TotalPatologiaAnualEneroVertical <= 0 ? 0 : ((getPatologiasVascularesAño["Enero"] * 100)) / TotalPatologiaAnualEneroVertical);
                    TotalEneroPatologiaOtrosVertical = (TotalPatologiaAnualEneroVertical <= 0 ? 0 : ((getPatologiasOtrosAño["Enero"] * 100)) / TotalPatologiaAnualEneroVertical);

                    TotalFebreroPatologiaPOPVertical = (TotalPatologiaAnualFebreroVertical <= 0 ? 0 : ((getPatologiasPosOperatorioAño["Febrero"] * 100)) / TotalPatologiaAnualFebreroVertical);
                    TotalFebreroPatologiaZPVertical = (TotalPatologiaAnualFebreroVertical <= 0 ? 0 : ((getPatologiasZPAño["Febrero"] * 100)) / TotalPatologiaAnualFebreroVertical);
                    TotalFebreroPatologiaPDVertical = (TotalPatologiaAnualFebreroVertical <= 0 ? 0 : ((getPatologiasPieDiabeticoAño["Febrero"] * 100)) / TotalPatologiaAnualFebreroVertical);
                    TotalFebreroPatologiaQXVertical = (TotalPatologiaAnualFebreroVertical <= 0 ? 0 : ((getPatologiasQuemadoAño["Febrero"] * 100)) / TotalPatologiaAnualFebreroVertical);
                    TotalFebreroPatologiaTXVertical = (TotalPatologiaAnualFebreroVertical <= 0 ? 0 : ((getPatologiasTraumaAño["Febrero"] * 100)) / TotalPatologiaAnualFebreroVertical);
                    TotalFebreroPatologiaUVVertical = (TotalPatologiaAnualFebreroVertical <= 0 ? 0 : ((getPatologiasVascularesAño["Febrero"] * 100)) / TotalPatologiaAnualFebreroVertical);
                    TotalFebreroPatologiaOtrosVertical = (TotalPatologiaAnualFebreroVertical <= 0 ? 0 : ((getPatologiasOtrosAño["Febrero"] * 100)) / TotalPatologiaAnualFebreroVertical);

                    TotalMarzoPatologiaPOPVertical = (TotalPatologiaAnualMarzoVertical <= 0 ? 0 : ((getPatologiasPosOperatorioAño["Marzo"] * 100)) / TotalPatologiaAnualMarzoVertical);
                    TotalMarzoPatologiaZPVertical = (TotalPatologiaAnualMarzoVertical <= 0 ? 0 : ((getPatologiasZPAño["Marzo"] * 100)) / TotalPatologiaAnualMarzoVertical);
                    TotalMarzoPatologiaPDVertical = (TotalPatologiaAnualMarzoVertical <= 0 ? 0 : ((getPatologiasPieDiabeticoAño["Marzo"] * 100)) / TotalPatologiaAnualMarzoVertical);
                    TotalMarzoPatologiaQXVertical = (TotalPatologiaAnualMarzoVertical <= 0 ? 0 : ((getPatologiasQuemadoAño["Marzo"] * 100)) / TotalPatologiaAnualMarzoVertical);
                    TotalMarzoPatologiaTXVertical = (TotalPatologiaAnualMarzoVertical <= 0 ? 0 : ((getPatologiasTraumaAño["Marzo"] * 100)) / TotalPatologiaAnualMarzoVertical);
                    TotalMarzoPatologiaUVVertical = (TotalPatologiaAnualMarzoVertical <= 0 ? 0 : ((getPatologiasVascularesAño["Marzo"] * 100)) / TotalPatologiaAnualMarzoVertical);
                    TotalMarzoPatologiaOtrosVertical = (TotalPatologiaAnualMarzoVertical <= 0 ? 0 : ((getPatologiasOtrosAño["Marzo"] * 100)) / TotalPatologiaAnualMarzoVertical);

                    TotalAbrilPatologiaPOPVertical = (TotalPatologiaAnualAbrilVertical <= 0 ? 0 : ((getPatologiasPosOperatorioAño["Abril"] * 100)) / TotalPatologiaAnualAbrilVertical);
                    TotalAbrilPatologiaZPVertical = (TotalPatologiaAnualAbrilVertical <= 0 ? 0 : ((getPatologiasZPAño["Abril"] * 100)) / TotalPatologiaAnualAbrilVertical);
                    TotalAbrilPatologiaPDVertical = (TotalPatologiaAnualAbrilVertical <= 0 ? 0 : ((getPatologiasPieDiabeticoAño["Abril"] * 100)) / TotalPatologiaAnualAbrilVertical);
                    TotalAbrilPatologiaQXVertical = (TotalPatologiaAnualAbrilVertical <= 0 ? 0 : ((getPatologiasQuemadoAño["Abril"] * 100)) / TotalPatologiaAnualAbrilVertical);
                    TotalAbrilPatologiaTXVertical = (TotalPatologiaAnualAbrilVertical <= 0 ? 0 : ((getPatologiasTraumaAño["Abril"] * 100)) / TotalPatologiaAnualAbrilVertical);
                    TotalAbrilPatologiaUVVertical = (TotalPatologiaAnualAbrilVertical <= 0 ? 0 : ((getPatologiasVascularesAño["Abril"] * 100)) / TotalPatologiaAnualAbrilVertical);
                    TotalAbrilPatologiaOtrosVertical = (TotalPatologiaAnualAbrilVertical <= 0 ? 0 : ((getPatologiasOtrosAño["Abril"] * 100)) / TotalPatologiaAnualAbrilVertical);

                    TotalMayoPatologiaPOPVertical = (TotalPatologiaAnualMayoVertical <= 0 ? 0 : ((getPatologiasPosOperatorioAño["Mayo"] * 100)) / TotalPatologiaAnualMayoVertical);
                    TotalMayoPatologiaZPVertical = (TotalPatologiaAnualMayoVertical <= 0 ? 0 : ((getPatologiasZPAño["Mayo"] * 100)) / TotalPatologiaAnualMayoVertical);
                    TotalMayoPatologiaPDVertical = (TotalPatologiaAnualMayoVertical <= 0 ? 0 : ((getPatologiasPieDiabeticoAño["Mayo"] * 100)) / TotalPatologiaAnualMayoVertical);
                    TotalMayoPatologiaQXVertical = (TotalPatologiaAnualMayoVertical <= 0 ? 0 : ((getPatologiasQuemadoAño["Mayo"] * 100)) / TotalPatologiaAnualMayoVertical);
                    TotalMayoPatologiaTXVertical = (TotalPatologiaAnualMayoVertical <= 0 ? 0 : ((getPatologiasTraumaAño["Mayo"] * 100)) / TotalPatologiaAnualMayoVertical);
                    TotalMayoPatologiaUVVertical = (TotalPatologiaAnualMayoVertical <= 0 ? 0 : ((getPatologiasVascularesAño["Mayo"] * 100)) / TotalPatologiaAnualMayoVertical);
                    TotalMayoPatologiaOtrosVertical = (TotalPatologiaAnualMayoVertical <= 0 ? 0 : ((getPatologiasOtrosAño["Mayo"] * 100)) / TotalPatologiaAnualMayoVertical);

                    TotalJunioPatologiaPOPVertical = (TotalPatologiaAnualJunioVertical <= 0 ? 0 : ((getPatologiasPosOperatorioAño["Junio"] * 100)) / TotalPatologiaAnualJunioVertical);
                    TotalJunioPatologiaZPVertical = (TotalPatologiaAnualJunioVertical <= 0 ? 0 : ((getPatologiasZPAño["Junio"] * 100)) / TotalPatologiaAnualJunioVertical);
                    TotalJunioPatologiaPDVertical = (TotalPatologiaAnualJunioVertical <= 0 ? 0 : ((getPatologiasPieDiabeticoAño["Junio"] * 100)) / TotalPatologiaAnualJunioVertical);
                    TotalJunioPatologiaQXVertical = (TotalPatologiaAnualJunioVertical <= 0 ? 0 : ((getPatologiasQuemadoAño["Junio"] * 100)) / TotalPatologiaAnualJunioVertical);
                    TotalJunioPatologiaTXVertical = (TotalPatologiaAnualJunioVertical <= 0 ? 0 : ((getPatologiasTraumaAño["Junio"] * 100)) / TotalPatologiaAnualJunioVertical);
                    TotalJunioPatologiaUVVertical = (TotalPatologiaAnualJunioVertical <= 0 ? 0 : ((getPatologiasVascularesAño["Junio"] * 100)) / TotalPatologiaAnualJunioVertical);
                    TotalJunioPatologiaOtrosVertical = (TotalPatologiaAnualJunioVertical <= 0 ? 0 : ((getPatologiasOtrosAño["Junio"] * 100)) / TotalPatologiaAnualJunioVertical);

                    TotalJulioPatologiaPOPVertical = (TotalPatologiaAnualJulioVertical <= 0 ? 0 : ((getPatologiasPosOperatorioAño["Julio"] * 100)) / TotalPatologiaAnualJulioVertical);
                    TotalJulioPatologiaZPVertical = (TotalPatologiaAnualJulioVertical <= 0 ? 0 : ((getPatologiasZPAño["Julio"] * 100)) / TotalPatologiaAnualJulioVertical);
                    TotalJulioPatologiaPDVertical = (TotalPatologiaAnualJulioVertical <= 0 ? 0 : ((getPatologiasPieDiabeticoAño["Julio"] * 100)) / TotalPatologiaAnualJulioVertical);
                    TotalJulioPatologiaQXVertical = (TotalPatologiaAnualJulioVertical <= 0 ? 0 : ((getPatologiasQuemadoAño["Julio"] * 100)) / TotalPatologiaAnualJulioVertical);
                    TotalJulioPatologiaTXVertical = (TotalPatologiaAnualJulioVertical <= 0 ? 0 : ((getPatologiasTraumaAño["Julio"] * 100)) / TotalPatologiaAnualJulioVertical);
                    TotalJulioPatologiaUVVertical = (TotalPatologiaAnualJulioVertical <= 0 ? 0 : ((getPatologiasVascularesAño["Julio"] * 100)) / TotalPatologiaAnualJulioVertical);
                    TotalJulioPatologiaOtrosVertical = (TotalPatologiaAnualJulioVertical <= 0 ? 0 : ((getPatologiasOtrosAño["Julio"] * 100)) / TotalPatologiaAnualJulioVertical);

                    TotalAgostoPatologiaPOPVertical = (TotalPatologiaAnualAgostoVertical <= 0 ? 0 : ((getPatologiasPosOperatorioAño["Agosto"] * 100)) / TotalPatologiaAnualAgostoVertical);
                    TotalAgostoPatologiaZPVertical = (TotalPatologiaAnualAgostoVertical <= 0 ? 0 : ((getPatologiasZPAño["Agosto"] * 100)) / TotalPatologiaAnualAgostoVertical);
                    TotalAgostoPatologiaPDVertical = (TotalPatologiaAnualAgostoVertical <= 0 ? 0 : ((getPatologiasPieDiabeticoAño["Agosto"] * 100)) / TotalPatologiaAnualAgostoVertical);
                    TotalAgostoPatologiaQXVertical = (TotalPatologiaAnualAgostoVertical <= 0 ? 0 : ((getPatologiasQuemadoAño["Agosto"] * 100)) / TotalPatologiaAnualAgostoVertical);
                    TotalAgostoPatologiaTXVertical = (TotalPatologiaAnualAgostoVertical <= 0 ? 0 : ((getPatologiasTraumaAño["Agosto"] * 100)) / TotalPatologiaAnualAgostoVertical);
                    TotalAgostoPatologiaUVVertical = (TotalPatologiaAnualAgostoVertical <= 0 ? 0 : ((getPatologiasVascularesAño["Agosto"] * 100)) / TotalPatologiaAnualAgostoVertical);
                    TotalAgostoPatologiaOtrosVertical = (TotalPatologiaAnualAgostoVertical <= 0 ? 0 : ((getPatologiasOtrosAño["Agosto"] * 100)) / TotalPatologiaAnualAgostoVertical);
                }
                else if (comboBox1.Text == "Septiembre" || comboBox1.Text == "SEPTIEMBRE")
                {
                    TotalEneroPatologiaPOPVertical = (TotalPatologiaAnualEneroVertical <= 0 ? 0 : ((getPatologiasPosOperatorioAño["Enero"] * 100)) / TotalPatologiaAnualEneroVertical);
                    TotalEneroPatologiaZPVertical = (TotalPatologiaAnualEneroVertical <= 0 ? 0 : ((getPatologiasZPAño["Enero"] * 100)) / TotalPatologiaAnualEneroVertical);
                    TotalEneroPatologiaPDVertical = (TotalPatologiaAnualEneroVertical <= 0 ? 0 : ((getPatologiasPieDiabeticoAño["Enero"] * 100)) / TotalPatologiaAnualEneroVertical);
                    TotalEneroPatologiaQXVertical = (TotalPatologiaAnualEneroVertical <= 0 ? 0 : ((getPatologiasQuemadoAño["Enero"] * 100)) / TotalPatologiaAnualEneroVertical);
                    TotalEneroPatologiaTXVertical = (TotalPatologiaAnualEneroVertical <= 0 ? 0 : ((getPatologiasTraumaAño["Enero"] * 100)) / TotalPatologiaAnualEneroVertical);
                    TotalEneroPatologiaUVVertical = (TotalPatologiaAnualEneroVertical <= 0 ? 0 : ((getPatologiasVascularesAño["Enero"] * 100)) / TotalPatologiaAnualEneroVertical);
                    TotalEneroPatologiaOtrosVertical = (TotalPatologiaAnualEneroVertical <= 0 ? 0 : ((getPatologiasOtrosAño["Enero"] * 100)) / TotalPatologiaAnualEneroVertical);

                    TotalFebreroPatologiaPOPVertical = (TotalPatologiaAnualFebreroVertical <= 0 ? 0 : ((getPatologiasPosOperatorioAño["Febrero"] * 100)) / TotalPatologiaAnualFebreroVertical);
                    TotalFebreroPatologiaZPVertical = (TotalPatologiaAnualFebreroVertical <= 0 ? 0 : ((getPatologiasZPAño["Febrero"] * 100)) / TotalPatologiaAnualFebreroVertical);
                    TotalFebreroPatologiaPDVertical = (TotalPatologiaAnualFebreroVertical <= 0 ? 0 : ((getPatologiasPieDiabeticoAño["Febrero"] * 100)) / TotalPatologiaAnualFebreroVertical);
                    TotalFebreroPatologiaQXVertical = (TotalPatologiaAnualFebreroVertical <= 0 ? 0 : ((getPatologiasQuemadoAño["Febrero"] * 100)) / TotalPatologiaAnualFebreroVertical);
                    TotalFebreroPatologiaTXVertical = (TotalPatologiaAnualFebreroVertical <= 0 ? 0 : ((getPatologiasTraumaAño["Febrero"] * 100)) / TotalPatologiaAnualFebreroVertical);
                    TotalFebreroPatologiaUVVertical = (TotalPatologiaAnualFebreroVertical <= 0 ? 0 : ((getPatologiasVascularesAño["Febrero"] * 100)) / TotalPatologiaAnualFebreroVertical);
                    TotalFebreroPatologiaOtrosVertical = (TotalPatologiaAnualFebreroVertical <= 0 ? 0 : ((getPatologiasOtrosAño["Febrero"] * 100)) / TotalPatologiaAnualFebreroVertical);

                    TotalMarzoPatologiaPOPVertical = (TotalPatologiaAnualMarzoVertical <= 0 ? 0 : ((getPatologiasPosOperatorioAño["Marzo"] * 100)) / TotalPatologiaAnualMarzoVertical);
                    TotalMarzoPatologiaZPVertical = (TotalPatologiaAnualMarzoVertical <= 0 ? 0 : ((getPatologiasZPAño["Marzo"] * 100)) / TotalPatologiaAnualMarzoVertical);
                    TotalMarzoPatologiaPDVertical = (TotalPatologiaAnualMarzoVertical <= 0 ? 0 : ((getPatologiasPieDiabeticoAño["Marzo"] * 100)) / TotalPatologiaAnualMarzoVertical);
                    TotalMarzoPatologiaQXVertical = (TotalPatologiaAnualMarzoVertical <= 0 ? 0 : ((getPatologiasQuemadoAño["Marzo"] * 100)) / TotalPatologiaAnualMarzoVertical);
                    TotalMarzoPatologiaTXVertical = (TotalPatologiaAnualMarzoVertical <= 0 ? 0 : ((getPatologiasTraumaAño["Marzo"] * 100)) / TotalPatologiaAnualMarzoVertical);
                    TotalMarzoPatologiaUVVertical = (TotalPatologiaAnualMarzoVertical <= 0 ? 0 : ((getPatologiasVascularesAño["Marzo"] * 100)) / TotalPatologiaAnualMarzoVertical);
                    TotalMarzoPatologiaOtrosVertical = (TotalPatologiaAnualMarzoVertical <= 0 ? 0 : ((getPatologiasOtrosAño["Marzo"] * 100)) / TotalPatologiaAnualMarzoVertical);

                    TotalAbrilPatologiaPOPVertical = (TotalPatologiaAnualAbrilVertical <= 0 ? 0 : ((getPatologiasPosOperatorioAño["Abril"] * 100)) / TotalPatologiaAnualAbrilVertical);
                    TotalAbrilPatologiaZPVertical = (TotalPatologiaAnualAbrilVertical <= 0 ? 0 : ((getPatologiasZPAño["Abril"] * 100)) / TotalPatologiaAnualAbrilVertical);
                    TotalAbrilPatologiaPDVertical = (TotalPatologiaAnualAbrilVertical <= 0 ? 0 : ((getPatologiasPieDiabeticoAño["Abril"] * 100)) / TotalPatologiaAnualAbrilVertical);
                    TotalAbrilPatologiaQXVertical = (TotalPatologiaAnualAbrilVertical <= 0 ? 0 : ((getPatologiasQuemadoAño["Abril"] * 100)) / TotalPatologiaAnualAbrilVertical);
                    TotalAbrilPatologiaTXVertical = (TotalPatologiaAnualAbrilVertical <= 0 ? 0 : ((getPatologiasTraumaAño["Abril"] * 100)) / TotalPatologiaAnualAbrilVertical);
                    TotalAbrilPatologiaUVVertical = (TotalPatologiaAnualAbrilVertical <= 0 ? 0 : ((getPatologiasVascularesAño["Abril"] * 100)) / TotalPatologiaAnualAbrilVertical);
                    TotalAbrilPatologiaOtrosVertical = (TotalPatologiaAnualAbrilVertical <= 0 ? 0 : ((getPatologiasOtrosAño["Abril"] * 100)) / TotalPatologiaAnualAbrilVertical);

                    TotalMayoPatologiaPOPVertical = (TotalPatologiaAnualMayoVertical <= 0 ? 0 : ((getPatologiasPosOperatorioAño["Mayo"] * 100)) / TotalPatologiaAnualMayoVertical);
                    TotalMayoPatologiaZPVertical = (TotalPatologiaAnualMayoVertical <= 0 ? 0 : ((getPatologiasZPAño["Mayo"] * 100)) / TotalPatologiaAnualMayoVertical);
                    TotalMayoPatologiaPDVertical = (TotalPatologiaAnualMayoVertical <= 0 ? 0 : ((getPatologiasPieDiabeticoAño["Mayo"] * 100)) / TotalPatologiaAnualMayoVertical);
                    TotalMayoPatologiaQXVertical = (TotalPatologiaAnualMayoVertical <= 0 ? 0 : ((getPatologiasQuemadoAño["Mayo"] * 100)) / TotalPatologiaAnualMayoVertical);
                    TotalMayoPatologiaTXVertical = (TotalPatologiaAnualMayoVertical <= 0 ? 0 : ((getPatologiasTraumaAño["Mayo"] * 100)) / TotalPatologiaAnualMayoVertical);
                    TotalMayoPatologiaUVVertical = (TotalPatologiaAnualMayoVertical <= 0 ? 0 : ((getPatologiasVascularesAño["Mayo"] * 100)) / TotalPatologiaAnualMayoVertical);
                    TotalMayoPatologiaOtrosVertical = (TotalPatologiaAnualMayoVertical <= 0 ? 0 : ((getPatologiasOtrosAño["Mayo"] * 100)) / TotalPatologiaAnualMayoVertical);

                    TotalJunioPatologiaPOPVertical = (TotalPatologiaAnualJunioVertical <= 0 ? 0 : ((getPatologiasPosOperatorioAño["Junio"] * 100)) / TotalPatologiaAnualJunioVertical);
                    TotalJunioPatologiaZPVertical = (TotalPatologiaAnualJunioVertical <= 0 ? 0 : ((getPatologiasZPAño["Junio"] * 100)) / TotalPatologiaAnualJunioVertical);
                    TotalJunioPatologiaPDVertical = (TotalPatologiaAnualJunioVertical <= 0 ? 0 : ((getPatologiasPieDiabeticoAño["Junio"] * 100)) / TotalPatologiaAnualJunioVertical);
                    TotalJunioPatologiaQXVertical = (TotalPatologiaAnualJunioVertical <= 0 ? 0 : ((getPatologiasQuemadoAño["Junio"] * 100)) / TotalPatologiaAnualJunioVertical);
                    TotalJunioPatologiaTXVertical = (TotalPatologiaAnualJunioVertical <= 0 ? 0 : ((getPatologiasTraumaAño["Junio"] * 100)) / TotalPatologiaAnualJunioVertical);
                    TotalJunioPatologiaUVVertical = (TotalPatologiaAnualJunioVertical <= 0 ? 0 : ((getPatologiasVascularesAño["Junio"] * 100)) / TotalPatologiaAnualJunioVertical);
                    TotalJunioPatologiaOtrosVertical = (TotalPatologiaAnualJunioVertical <= 0 ? 0 : ((getPatologiasOtrosAño["Junio"] * 100)) / TotalPatologiaAnualJunioVertical);

                    TotalJulioPatologiaPOPVertical = (TotalPatologiaAnualJulioVertical <= 0 ? 0 : ((getPatologiasPosOperatorioAño["Julio"] * 100)) / TotalPatologiaAnualJulioVertical);
                    TotalJulioPatologiaZPVertical = (TotalPatologiaAnualJulioVertical <= 0 ? 0 : ((getPatologiasZPAño["Julio"] * 100)) / TotalPatologiaAnualJulioVertical);
                    TotalJulioPatologiaPDVertical = (TotalPatologiaAnualJulioVertical <= 0 ? 0 : ((getPatologiasPieDiabeticoAño["Julio"] * 100)) / TotalPatologiaAnualJulioVertical);
                    TotalJulioPatologiaQXVertical = (TotalPatologiaAnualJulioVertical <= 0 ? 0 : ((getPatologiasQuemadoAño["Julio"] * 100)) / TotalPatologiaAnualJulioVertical);
                    TotalJulioPatologiaTXVertical = (TotalPatologiaAnualJulioVertical <= 0 ? 0 : ((getPatologiasTraumaAño["Julio"] * 100)) / TotalPatologiaAnualJulioVertical);
                    TotalJulioPatologiaUVVertical = (TotalPatologiaAnualJulioVertical <= 0 ? 0 : ((getPatologiasVascularesAño["Julio"] * 100)) / TotalPatologiaAnualJulioVertical);
                    TotalJulioPatologiaOtrosVertical = (TotalPatologiaAnualJulioVertical <= 0 ? 0 : ((getPatologiasOtrosAño["Julio"] * 100)) / TotalPatologiaAnualJulioVertical);

                    TotalAgostoPatologiaPOPVertical = (TotalPatologiaAnualAgostoVertical <= 0 ? 0 : ((getPatologiasPosOperatorioAño["Agosto"] * 100)) / TotalPatologiaAnualAgostoVertical);
                    TotalAgostoPatologiaZPVertical = (TotalPatologiaAnualAgostoVertical <= 0 ? 0 : ((getPatologiasZPAño["Agosto"] * 100)) / TotalPatologiaAnualAgostoVertical);
                    TotalAgostoPatologiaPDVertical = (TotalPatologiaAnualAgostoVertical <= 0 ? 0 : ((getPatologiasPieDiabeticoAño["Agosto"] * 100)) / TotalPatologiaAnualAgostoVertical);
                    TotalAgostoPatologiaQXVertical = (TotalPatologiaAnualAgostoVertical <= 0 ? 0 : ((getPatologiasQuemadoAño["Agosto"] * 100)) / TotalPatologiaAnualAgostoVertical);
                    TotalAgostoPatologiaTXVertical = (TotalPatologiaAnualAgostoVertical <= 0 ? 0 : ((getPatologiasTraumaAño["Agosto"] * 100)) / TotalPatologiaAnualAgostoVertical);
                    TotalAgostoPatologiaUVVertical = (TotalPatologiaAnualAgostoVertical <= 0 ? 0 : ((getPatologiasVascularesAño["Agosto"] * 100)) / TotalPatologiaAnualAgostoVertical);
                    TotalAgostoPatologiaOtrosVertical = (TotalPatologiaAnualAgostoVertical <= 0 ? 0 : ((getPatologiasOtrosAño["Agosto"] * 100)) / TotalPatologiaAnualAgostoVertical);

                    TotalSeptiembrePatologiaPOPVertical = (TotalPatologiaAnualSeptiembreVertical <= 0 ? 0 : ((getPatologiasPosOperatorioAño["Septiembre"] * 100)) / TotalPatologiaAnualSeptiembreVertical);
                    TotalSeptiembrePatologiaZPVertical = (TotalPatologiaAnualSeptiembreVertical <= 0 ? 0 : ((getPatologiasZPAño["Septiembre"] * 100)) / TotalPatologiaAnualSeptiembreVertical);
                    TotalSeptiembrePatologiaPDVertical = (TotalPatologiaAnualSeptiembreVertical <= 0 ? 0 : ((getPatologiasPieDiabeticoAño["Septiembre"] * 100)) / TotalPatologiaAnualSeptiembreVertical);
                    TotalSeptiembrePatologiaQXVertical = (TotalPatologiaAnualSeptiembreVertical <= 0 ? 0 : ((getPatologiasQuemadoAño["Septiembre"] * 100)) / TotalPatologiaAnualSeptiembreVertical);
                    TotalSeptiembrePatologiaTXVertical = (TotalPatologiaAnualSeptiembreVertical <= 0 ? 0 : ((getPatologiasTraumaAño["Septiembre"] * 100)) / TotalPatologiaAnualSeptiembreVertical);
                    TotalSeptiembrePatologiaUVVertical = (TotalPatologiaAnualSeptiembreVertical <= 0 ? 0 : ((getPatologiasVascularesAño["Septiembre"] * 100)) / TotalPatologiaAnualSeptiembreVertical);
                    TotalSeptiembrePatologiaOtrosVertical = (TotalPatologiaAnualSeptiembreVertical <= 0 ? 0 : ((getPatologiasOtrosAño["Septiembre"] * 100)) / TotalPatologiaAnualSeptiembreVertical);
                }
                else if (comboBox1.Text == "Octubre" || comboBox1.Text == "OCTUBRE")
                {
                    TotalEneroPatologiaPOPVertical = (TotalPatologiaAnualEneroVertical <= 0 ? 0 : ((getPatologiasPosOperatorioAño["Enero"] * 100)) / TotalPatologiaAnualEneroVertical);
                    TotalEneroPatologiaZPVertical = (TotalPatologiaAnualEneroVertical <= 0 ? 0 : ((getPatologiasZPAño["Enero"] * 100)) / TotalPatologiaAnualEneroVertical);
                    TotalEneroPatologiaPDVertical = (TotalPatologiaAnualEneroVertical <= 0 ? 0 : ((getPatologiasPieDiabeticoAño["Enero"] * 100)) / TotalPatologiaAnualEneroVertical);
                    TotalEneroPatologiaQXVertical = (TotalPatologiaAnualEneroVertical <= 0 ? 0 : ((getPatologiasQuemadoAño["Enero"] * 100)) / TotalPatologiaAnualEneroVertical);
                    TotalEneroPatologiaTXVertical = (TotalPatologiaAnualEneroVertical <= 0 ? 0 : ((getPatologiasTraumaAño["Enero"] * 100)) / TotalPatologiaAnualEneroVertical);
                    TotalEneroPatologiaUVVertical = (TotalPatologiaAnualEneroVertical <= 0 ? 0 : ((getPatologiasVascularesAño["Enero"] * 100)) / TotalPatologiaAnualEneroVertical);
                    TotalEneroPatologiaOtrosVertical = (TotalPatologiaAnualEneroVertical <= 0 ? 0 : ((getPatologiasOtrosAño["Enero"] * 100)) / TotalPatologiaAnualEneroVertical);

                    TotalFebreroPatologiaPOPVertical = (TotalPatologiaAnualFebreroVertical <= 0 ? 0 : ((getPatologiasPosOperatorioAño["Febrero"] * 100)) / TotalPatologiaAnualFebreroVertical);
                    TotalFebreroPatologiaZPVertical = (TotalPatologiaAnualFebreroVertical <= 0 ? 0 : ((getPatologiasZPAño["Febrero"] * 100)) / TotalPatologiaAnualFebreroVertical);
                    TotalFebreroPatologiaPDVertical = (TotalPatologiaAnualFebreroVertical <= 0 ? 0 : ((getPatologiasPieDiabeticoAño["Febrero"] * 100)) / TotalPatologiaAnualFebreroVertical);
                    TotalFebreroPatologiaQXVertical = (TotalPatologiaAnualFebreroVertical <= 0 ? 0 : ((getPatologiasQuemadoAño["Febrero"] * 100)) / TotalPatologiaAnualFebreroVertical);
                    TotalFebreroPatologiaTXVertical = (TotalPatologiaAnualFebreroVertical <= 0 ? 0 : ((getPatologiasTraumaAño["Febrero"] * 100)) / TotalPatologiaAnualFebreroVertical);
                    TotalFebreroPatologiaUVVertical = (TotalPatologiaAnualFebreroVertical <= 0 ? 0 : ((getPatologiasVascularesAño["Febrero"] * 100)) / TotalPatologiaAnualFebreroVertical);
                    TotalFebreroPatologiaOtrosVertical = (TotalPatologiaAnualFebreroVertical <= 0 ? 0 : ((getPatologiasOtrosAño["Febrero"] * 100)) / TotalPatologiaAnualFebreroVertical);

                    TotalMarzoPatologiaPOPVertical = (TotalPatologiaAnualMarzoVertical <= 0 ? 0 : ((getPatologiasPosOperatorioAño["Marzo"] * 100)) / TotalPatologiaAnualMarzoVertical);
                    TotalMarzoPatologiaZPVertical = (TotalPatologiaAnualMarzoVertical <= 0 ? 0 : ((getPatologiasZPAño["Marzo"] * 100)) / TotalPatologiaAnualMarzoVertical);
                    TotalMarzoPatologiaPDVertical = (TotalPatologiaAnualMarzoVertical <= 0 ? 0 : ((getPatologiasPieDiabeticoAño["Marzo"] * 100)) / TotalPatologiaAnualMarzoVertical);
                    TotalMarzoPatologiaQXVertical = (TotalPatologiaAnualMarzoVertical <= 0 ? 0 : ((getPatologiasQuemadoAño["Marzo"] * 100)) / TotalPatologiaAnualMarzoVertical);
                    TotalMarzoPatologiaTXVertical = (TotalPatologiaAnualMarzoVertical <= 0 ? 0 : ((getPatologiasTraumaAño["Marzo"] * 100)) / TotalPatologiaAnualMarzoVertical);
                    TotalMarzoPatologiaUVVertical = (TotalPatologiaAnualMarzoVertical <= 0 ? 0 : ((getPatologiasVascularesAño["Marzo"] * 100)) / TotalPatologiaAnualMarzoVertical);
                    TotalMarzoPatologiaOtrosVertical = (TotalPatologiaAnualMarzoVertical <= 0 ? 0 : ((getPatologiasOtrosAño["Marzo"] * 100)) / TotalPatologiaAnualMarzoVertical);

                    TotalAbrilPatologiaPOPVertical = (TotalPatologiaAnualAbrilVertical <= 0 ? 0 : ((getPatologiasPosOperatorioAño["Abril"] * 100)) / TotalPatologiaAnualAbrilVertical);
                    TotalAbrilPatologiaZPVertical = (TotalPatologiaAnualAbrilVertical <= 0 ? 0 : ((getPatologiasZPAño["Abril"] * 100)) / TotalPatologiaAnualAbrilVertical);
                    TotalAbrilPatologiaPDVertical = (TotalPatologiaAnualAbrilVertical <= 0 ? 0 : ((getPatologiasPieDiabeticoAño["Abril"] * 100)) / TotalPatologiaAnualAbrilVertical);
                    TotalAbrilPatologiaQXVertical = (TotalPatologiaAnualAbrilVertical <= 0 ? 0 : ((getPatologiasQuemadoAño["Abril"] * 100)) / TotalPatologiaAnualAbrilVertical);
                    TotalAbrilPatologiaTXVertical = (TotalPatologiaAnualAbrilVertical <= 0 ? 0 : ((getPatologiasTraumaAño["Abril"] * 100)) / TotalPatologiaAnualAbrilVertical);
                    TotalAbrilPatologiaUVVertical = (TotalPatologiaAnualAbrilVertical <= 0 ? 0 : ((getPatologiasVascularesAño["Abril"] * 100)) / TotalPatologiaAnualAbrilVertical);
                    TotalAbrilPatologiaOtrosVertical = (TotalPatologiaAnualAbrilVertical <= 0 ? 0 : ((getPatologiasOtrosAño["Abril"] * 100)) / TotalPatologiaAnualAbrilVertical);

                    TotalMayoPatologiaPOPVertical = (TotalPatologiaAnualMayoVertical <= 0 ? 0 : ((getPatologiasPosOperatorioAño["Mayo"] * 100)) / TotalPatologiaAnualMayoVertical);
                    TotalMayoPatologiaZPVertical = (TotalPatologiaAnualMayoVertical <= 0 ? 0 : ((getPatologiasZPAño["Mayo"] * 100)) / TotalPatologiaAnualMayoVertical);
                    TotalMayoPatologiaPDVertical = (TotalPatologiaAnualMayoVertical <= 0 ? 0 : ((getPatologiasPieDiabeticoAño["Mayo"] * 100)) / TotalPatologiaAnualMayoVertical);
                    TotalMayoPatologiaQXVertical = (TotalPatologiaAnualMayoVertical <= 0 ? 0 : ((getPatologiasQuemadoAño["Mayo"] * 100)) / TotalPatologiaAnualMayoVertical);
                    TotalMayoPatologiaTXVertical = (TotalPatologiaAnualMayoVertical <= 0 ? 0 : ((getPatologiasTraumaAño["Mayo"] * 100)) / TotalPatologiaAnualMayoVertical);
                    TotalMayoPatologiaUVVertical = (TotalPatologiaAnualMayoVertical <= 0 ? 0 : ((getPatologiasVascularesAño["Mayo"] * 100)) / TotalPatologiaAnualMayoVertical);
                    TotalMayoPatologiaOtrosVertical = (TotalPatologiaAnualMayoVertical <= 0 ? 0 : ((getPatologiasOtrosAño["Mayo"] * 100)) / TotalPatologiaAnualMayoVertical);

                    TotalJunioPatologiaPOPVertical = (TotalPatologiaAnualJunioVertical <= 0 ? 0 : ((getPatologiasPosOperatorioAño["Junio"] * 100)) / TotalPatologiaAnualJunioVertical);
                    TotalJunioPatologiaZPVertical = (TotalPatologiaAnualJunioVertical <= 0 ? 0 : ((getPatologiasZPAño["Junio"] * 100)) / TotalPatologiaAnualJunioVertical);
                    TotalJunioPatologiaPDVertical = (TotalPatologiaAnualJunioVertical <= 0 ? 0 : ((getPatologiasPieDiabeticoAño["Junio"] * 100)) / TotalPatologiaAnualJunioVertical);
                    TotalJunioPatologiaQXVertical = (TotalPatologiaAnualJunioVertical <= 0 ? 0 : ((getPatologiasQuemadoAño["Junio"] * 100)) / TotalPatologiaAnualJunioVertical);
                    TotalJunioPatologiaTXVertical = (TotalPatologiaAnualJunioVertical <= 0 ? 0 : ((getPatologiasTraumaAño["Junio"] * 100)) / TotalPatologiaAnualJunioVertical);
                    TotalJunioPatologiaUVVertical = (TotalPatologiaAnualJunioVertical <= 0 ? 0 : ((getPatologiasVascularesAño["Junio"] * 100)) / TotalPatologiaAnualJunioVertical);
                    TotalJunioPatologiaOtrosVertical = (TotalPatologiaAnualJunioVertical <= 0 ? 0 : ((getPatologiasOtrosAño["Junio"] * 100)) / TotalPatologiaAnualJunioVertical);

                    TotalJulioPatologiaPOPVertical = (TotalPatologiaAnualJulioVertical <= 0 ? 0 : ((getPatologiasPosOperatorioAño["Julio"] * 100)) / TotalPatologiaAnualJulioVertical);
                    TotalJulioPatologiaZPVertical = (TotalPatologiaAnualJulioVertical <= 0 ? 0 : ((getPatologiasZPAño["Julio"] * 100)) / TotalPatologiaAnualJulioVertical);
                    TotalJulioPatologiaPDVertical = (TotalPatologiaAnualJulioVertical <= 0 ? 0 : ((getPatologiasPieDiabeticoAño["Julio"] * 100)) / TotalPatologiaAnualJulioVertical);
                    TotalJulioPatologiaQXVertical = (TotalPatologiaAnualJulioVertical <= 0 ? 0 : ((getPatologiasQuemadoAño["Julio"] * 100)) / TotalPatologiaAnualJulioVertical);
                    TotalJulioPatologiaTXVertical = (TotalPatologiaAnualJulioVertical <= 0 ? 0 : ((getPatologiasTraumaAño["Julio"] * 100)) / TotalPatologiaAnualJulioVertical);
                    TotalJulioPatologiaUVVertical = (TotalPatologiaAnualJulioVertical <= 0 ? 0 : ((getPatologiasVascularesAño["Julio"] * 100)) / TotalPatologiaAnualJulioVertical);
                    TotalJulioPatologiaOtrosVertical = (TotalPatologiaAnualJulioVertical <= 0 ? 0 : ((getPatologiasOtrosAño["Julio"] * 100)) / TotalPatologiaAnualJulioVertical);

                    TotalAgostoPatologiaPOPVertical = (TotalPatologiaAnualAgostoVertical <= 0 ? 0 : ((getPatologiasPosOperatorioAño["Agosto"] * 100)) / TotalPatologiaAnualAgostoVertical);
                    TotalAgostoPatologiaZPVertical = (TotalPatologiaAnualAgostoVertical <= 0 ? 0 : ((getPatologiasZPAño["Agosto"] * 100)) / TotalPatologiaAnualAgostoVertical);
                    TotalAgostoPatologiaPDVertical = (TotalPatologiaAnualAgostoVertical <= 0 ? 0 : ((getPatologiasPieDiabeticoAño["Agosto"] * 100)) / TotalPatologiaAnualAgostoVertical);
                    TotalAgostoPatologiaQXVertical = (TotalPatologiaAnualAgostoVertical <= 0 ? 0 : ((getPatologiasQuemadoAño["Agosto"] * 100)) / TotalPatologiaAnualAgostoVertical);
                    TotalAgostoPatologiaTXVertical = (TotalPatologiaAnualAgostoVertical <= 0 ? 0 : ((getPatologiasTraumaAño["Agosto"] * 100)) / TotalPatologiaAnualAgostoVertical);
                    TotalAgostoPatologiaUVVertical = (TotalPatologiaAnualAgostoVertical <= 0 ? 0 : ((getPatologiasVascularesAño["Agosto"] * 100)) / TotalPatologiaAnualAgostoVertical);
                    TotalAgostoPatologiaOtrosVertical = (TotalPatologiaAnualAgostoVertical <= 0 ? 0 : ((getPatologiasOtrosAño["Agosto"] * 100)) / TotalPatologiaAnualAgostoVertical);

                    TotalSeptiembrePatologiaPOPVertical = (TotalPatologiaAnualSeptiembreVertical <= 0 ? 0 : ((getPatologiasPosOperatorioAño["Septiembre"] * 100)) / TotalPatologiaAnualSeptiembreVertical);
                    TotalSeptiembrePatologiaZPVertical = (TotalPatologiaAnualSeptiembreVertical <= 0 ? 0 : ((getPatologiasZPAño["Septiembre"] * 100)) / TotalPatologiaAnualSeptiembreVertical);
                    TotalSeptiembrePatologiaPDVertical = (TotalPatologiaAnualSeptiembreVertical <= 0 ? 0 : ((getPatologiasPieDiabeticoAño["Septiembre"] * 100)) / TotalPatologiaAnualSeptiembreVertical);
                    TotalSeptiembrePatologiaQXVertical = (TotalPatologiaAnualSeptiembreVertical <= 0 ? 0 : ((getPatologiasQuemadoAño["Septiembre"] * 100)) / TotalPatologiaAnualSeptiembreVertical);
                    TotalSeptiembrePatologiaTXVertical = (TotalPatologiaAnualSeptiembreVertical <= 0 ? 0 : ((getPatologiasTraumaAño["Septiembre"] * 100)) / TotalPatologiaAnualSeptiembreVertical);
                    TotalSeptiembrePatologiaUVVertical = (TotalPatologiaAnualSeptiembreVertical <= 0 ? 0 : ((getPatologiasVascularesAño["Septiembre"] * 100)) / TotalPatologiaAnualSeptiembreVertical);
                    TotalSeptiembrePatologiaOtrosVertical = (TotalPatologiaAnualSeptiembreVertical <= 0 ? 0 : ((getPatologiasOtrosAño["Septiembre"] * 100)) / TotalPatologiaAnualSeptiembreVertical);

                    TotalOctubrePatologiaPOPVertical = (TotalPatologiaAnualOctubreVertical <= 0 ? 0 : ((getPatologiasPosOperatorioAño["Octubre"] * 100)) / TotalPatologiaAnualOctubreVertical);
                    TotalOctubrePatologiaZPVertical = (TotalPatologiaAnualOctubreVertical <= 0 ? 0 : ((getPatologiasZPAño["Octubre"] * 100)) / TotalPatologiaAnualOctubreVertical);
                    TotalOctubrePatologiaPDVertical = (TotalPatologiaAnualOctubreVertical <= 0 ? 0 : ((getPatologiasPieDiabeticoAño["Octubre"] * 100)) / TotalPatologiaAnualOctubreVertical);
                    TotalOctubrePatologiaQXVertical = (TotalPatologiaAnualOctubreVertical <= 0 ? 0 : ((getPatologiasQuemadoAño["Octubre"] * 100)) / TotalPatologiaAnualOctubreVertical);
                    TotalOctubrePatologiaTXVertical = (TotalPatologiaAnualOctubreVertical <= 0 ? 0 : ((getPatologiasTraumaAño["Octubre"] * 100)) / TotalPatologiaAnualOctubreVertical);
                    TotalOctubrePatologiaUVVertical = (TotalPatologiaAnualOctubreVertical <= 0 ? 0 : ((getPatologiasVascularesAño["Octubre"] * 100)) / TotalPatologiaAnualOctubreVertical);
                    TotalOctubrePatologiaOtrosVertical = (TotalPatologiaAnualOctubreVertical <= 0 ? 0 : ((getPatologiasOtrosAño["Octubre"] * 100)) / TotalPatologiaAnualOctubreVertical);
                }
                else if (comboBox1.Text == "Noviembre" || comboBox1.Text == "NOVIEMBRE")
                {
                    TotalEneroPatologiaPOPVertical = (TotalPatologiaAnualEneroVertical <= 0 ? 0 : ((getPatologiasPosOperatorioAño["Enero"] * 100)) / TotalPatologiaAnualEneroVertical);
                    TotalEneroPatologiaZPVertical = (TotalPatologiaAnualEneroVertical <= 0 ? 0 : ((getPatologiasZPAño["Enero"] * 100)) / TotalPatologiaAnualEneroVertical);
                    TotalEneroPatologiaPDVertical = (TotalPatologiaAnualEneroVertical <= 0 ? 0 : ((getPatologiasPieDiabeticoAño["Enero"] * 100)) / TotalPatologiaAnualEneroVertical);
                    TotalEneroPatologiaQXVertical = (TotalPatologiaAnualEneroVertical <= 0 ? 0 : ((getPatologiasQuemadoAño["Enero"] * 100)) / TotalPatologiaAnualEneroVertical);
                    TotalEneroPatologiaTXVertical = (TotalPatologiaAnualEneroVertical <= 0 ? 0 : ((getPatologiasTraumaAño["Enero"] * 100)) / TotalPatologiaAnualEneroVertical);
                    TotalEneroPatologiaUVVertical = (TotalPatologiaAnualEneroVertical <= 0 ? 0 : ((getPatologiasVascularesAño["Enero"] * 100)) / TotalPatologiaAnualEneroVertical);
                    TotalEneroPatologiaOtrosVertical = (TotalPatologiaAnualEneroVertical <= 0 ? 0 : ((getPatologiasOtrosAño["Enero"] * 100)) / TotalPatologiaAnualEneroVertical);

                    TotalFebreroPatologiaPOPVertical = (TotalPatologiaAnualFebreroVertical <= 0 ? 0 : ((getPatologiasPosOperatorioAño["Febrero"] * 100)) / TotalPatologiaAnualFebreroVertical);
                    TotalFebreroPatologiaZPVertical = (TotalPatologiaAnualFebreroVertical <= 0 ? 0 : ((getPatologiasZPAño["Febrero"] * 100)) / TotalPatologiaAnualFebreroVertical);
                    TotalFebreroPatologiaPDVertical = (TotalPatologiaAnualFebreroVertical <= 0 ? 0 : ((getPatologiasPieDiabeticoAño["Febrero"] * 100)) / TotalPatologiaAnualFebreroVertical);
                    TotalFebreroPatologiaQXVertical = (TotalPatologiaAnualFebreroVertical <= 0 ? 0 : ((getPatologiasQuemadoAño["Febrero"] * 100)) / TotalPatologiaAnualFebreroVertical);
                    TotalFebreroPatologiaTXVertical = (TotalPatologiaAnualFebreroVertical <= 0 ? 0 : ((getPatologiasTraumaAño["Febrero"] * 100)) / TotalPatologiaAnualFebreroVertical);
                    TotalFebreroPatologiaUVVertical = (TotalPatologiaAnualFebreroVertical <= 0 ? 0 : ((getPatologiasVascularesAño["Febrero"] * 100)) / TotalPatologiaAnualFebreroVertical);
                    TotalFebreroPatologiaOtrosVertical = (TotalPatologiaAnualFebreroVertical <= 0 ? 0 : ((getPatologiasOtrosAño["Febrero"] * 100)) / TotalPatologiaAnualFebreroVertical);

                    TotalMarzoPatologiaPOPVertical = (TotalPatologiaAnualMarzoVertical <= 0 ? 0 : ((getPatologiasPosOperatorioAño["Marzo"] * 100)) / TotalPatologiaAnualMarzoVertical);
                    TotalMarzoPatologiaZPVertical = (TotalPatologiaAnualMarzoVertical <= 0 ? 0 : ((getPatologiasZPAño["Marzo"] * 100)) / TotalPatologiaAnualMarzoVertical);
                    TotalMarzoPatologiaPDVertical = (TotalPatologiaAnualMarzoVertical <= 0 ? 0 : ((getPatologiasPieDiabeticoAño["Marzo"] * 100)) / TotalPatologiaAnualMarzoVertical);
                    TotalMarzoPatologiaQXVertical = (TotalPatologiaAnualMarzoVertical <= 0 ? 0 : ((getPatologiasQuemadoAño["Marzo"] * 100)) / TotalPatologiaAnualMarzoVertical);
                    TotalMarzoPatologiaTXVertical = (TotalPatologiaAnualMarzoVertical <= 0 ? 0 : ((getPatologiasTraumaAño["Marzo"] * 100)) / TotalPatologiaAnualMarzoVertical);
                    TotalMarzoPatologiaUVVertical = (TotalPatologiaAnualMarzoVertical <= 0 ? 0 : ((getPatologiasVascularesAño["Marzo"] * 100)) / TotalPatologiaAnualMarzoVertical);
                    TotalMarzoPatologiaOtrosVertical = (TotalPatologiaAnualMarzoVertical <= 0 ? 0 : ((getPatologiasOtrosAño["Marzo"] * 100)) / TotalPatologiaAnualMarzoVertical);

                    TotalAbrilPatologiaPOPVertical = (TotalPatologiaAnualAbrilVertical <= 0 ? 0 : ((getPatologiasPosOperatorioAño["Abril"] * 100)) / TotalPatologiaAnualAbrilVertical);
                    TotalAbrilPatologiaZPVertical = (TotalPatologiaAnualAbrilVertical <= 0 ? 0 : ((getPatologiasZPAño["Abril"] * 100)) / TotalPatologiaAnualAbrilVertical);
                    TotalAbrilPatologiaPDVertical = (TotalPatologiaAnualAbrilVertical <= 0 ? 0 : ((getPatologiasPieDiabeticoAño["Abril"] * 100)) / TotalPatologiaAnualAbrilVertical);
                    TotalAbrilPatologiaQXVertical = (TotalPatologiaAnualAbrilVertical <= 0 ? 0 : ((getPatologiasQuemadoAño["Abril"] * 100)) / TotalPatologiaAnualAbrilVertical);
                    TotalAbrilPatologiaTXVertical = (TotalPatologiaAnualAbrilVertical <= 0 ? 0 : ((getPatologiasTraumaAño["Abril"] * 100)) / TotalPatologiaAnualAbrilVertical);
                    TotalAbrilPatologiaUVVertical = (TotalPatologiaAnualAbrilVertical <= 0 ? 0 : ((getPatologiasVascularesAño["Abril"] * 100)) / TotalPatologiaAnualAbrilVertical);
                    TotalAbrilPatologiaOtrosVertical = (TotalPatologiaAnualAbrilVertical <= 0 ? 0 : ((getPatologiasOtrosAño["Abril"] * 100)) / TotalPatologiaAnualAbrilVertical);

                    TotalMayoPatologiaPOPVertical = (TotalPatologiaAnualMayoVertical <= 0 ? 0 : ((getPatologiasPosOperatorioAño["Mayo"] * 100)) / TotalPatologiaAnualMayoVertical);
                    TotalMayoPatologiaZPVertical = (TotalPatologiaAnualMayoVertical <= 0 ? 0 : ((getPatologiasZPAño["Mayo"] * 100)) / TotalPatologiaAnualMayoVertical);
                    TotalMayoPatologiaPDVertical = (TotalPatologiaAnualMayoVertical <= 0 ? 0 : ((getPatologiasPieDiabeticoAño["Mayo"] * 100)) / TotalPatologiaAnualMayoVertical);
                    TotalMayoPatologiaQXVertical = (TotalPatologiaAnualMayoVertical <= 0 ? 0 : ((getPatologiasQuemadoAño["Mayo"] * 100)) / TotalPatologiaAnualMayoVertical);
                    TotalMayoPatologiaTXVertical = (TotalPatologiaAnualMayoVertical <= 0 ? 0 : ((getPatologiasTraumaAño["Mayo"] * 100)) / TotalPatologiaAnualMayoVertical);
                    TotalMayoPatologiaUVVertical = (TotalPatologiaAnualMayoVertical <= 0 ? 0 : ((getPatologiasVascularesAño["Mayo"] * 100)) / TotalPatologiaAnualMayoVertical);
                    TotalMayoPatologiaOtrosVertical = (TotalPatologiaAnualMayoVertical <= 0 ? 0 : ((getPatologiasOtrosAño["Mayo"] * 100)) / TotalPatologiaAnualMayoVertical);

                    TotalJunioPatologiaPOPVertical = (TotalPatologiaAnualJunioVertical <= 0 ? 0 : ((getPatologiasPosOperatorioAño["Junio"] * 100)) / TotalPatologiaAnualJunioVertical);
                    TotalJunioPatologiaZPVertical = (TotalPatologiaAnualJunioVertical <= 0 ? 0 : ((getPatologiasZPAño["Junio"] * 100)) / TotalPatologiaAnualJunioVertical);
                    TotalJunioPatologiaPDVertical = (TotalPatologiaAnualJunioVertical <= 0 ? 0 : ((getPatologiasPieDiabeticoAño["Junio"] * 100)) / TotalPatologiaAnualJunioVertical);
                    TotalJunioPatologiaQXVertical = (TotalPatologiaAnualJunioVertical <= 0 ? 0 : ((getPatologiasQuemadoAño["Junio"] * 100)) / TotalPatologiaAnualJunioVertical);
                    TotalJunioPatologiaTXVertical = (TotalPatologiaAnualJunioVertical <= 0 ? 0 : ((getPatologiasTraumaAño["Junio"] * 100)) / TotalPatologiaAnualJunioVertical);
                    TotalJunioPatologiaUVVertical = (TotalPatologiaAnualJunioVertical <= 0 ? 0 : ((getPatologiasVascularesAño["Junio"] * 100)) / TotalPatologiaAnualJunioVertical);
                    TotalJunioPatologiaOtrosVertical = (TotalPatologiaAnualJunioVertical <= 0 ? 0 : ((getPatologiasOtrosAño["Junio"] * 100)) / TotalPatologiaAnualJunioVertical);

                    TotalJulioPatologiaPOPVertical = (TotalPatologiaAnualJulioVertical <= 0 ? 0 : ((getPatologiasPosOperatorioAño["Julio"] * 100)) / TotalPatologiaAnualJulioVertical);
                    TotalJulioPatologiaZPVertical = (TotalPatologiaAnualJulioVertical <= 0 ? 0 : ((getPatologiasZPAño["Julio"] * 100)) / TotalPatologiaAnualJulioVertical);
                    TotalJulioPatologiaPDVertical = (TotalPatologiaAnualJulioVertical <= 0 ? 0 : ((getPatologiasPieDiabeticoAño["Julio"] * 100)) / TotalPatologiaAnualJulioVertical);
                    TotalJulioPatologiaQXVertical = (TotalPatologiaAnualJulioVertical <= 0 ? 0 : ((getPatologiasQuemadoAño["Julio"] * 100)) / TotalPatologiaAnualJulioVertical);
                    TotalJulioPatologiaTXVertical = (TotalPatologiaAnualJulioVertical <= 0 ? 0 : ((getPatologiasTraumaAño["Julio"] * 100)) / TotalPatologiaAnualJulioVertical);
                    TotalJulioPatologiaUVVertical = (TotalPatologiaAnualJulioVertical <= 0 ? 0 : ((getPatologiasVascularesAño["Julio"] * 100)) / TotalPatologiaAnualJulioVertical);
                    TotalJulioPatologiaOtrosVertical = (TotalPatologiaAnualJulioVertical <= 0 ? 0 : ((getPatologiasOtrosAño["Julio"] * 100)) / TotalPatologiaAnualJulioVertical);

                    TotalAgostoPatologiaPOPVertical = (TotalPatologiaAnualAgostoVertical <= 0 ? 0 : ((getPatologiasPosOperatorioAño["Agosto"] * 100)) / TotalPatologiaAnualAgostoVertical);
                    TotalAgostoPatologiaZPVertical = (TotalPatologiaAnualAgostoVertical <= 0 ? 0 : ((getPatologiasZPAño["Agosto"] * 100)) / TotalPatologiaAnualAgostoVertical);
                    TotalAgostoPatologiaPDVertical = (TotalPatologiaAnualAgostoVertical <= 0 ? 0 : ((getPatologiasPieDiabeticoAño["Agosto"] * 100)) / TotalPatologiaAnualAgostoVertical);
                    TotalAgostoPatologiaQXVertical = (TotalPatologiaAnualAgostoVertical <= 0 ? 0 : ((getPatologiasQuemadoAño["Agosto"] * 100)) / TotalPatologiaAnualAgostoVertical);
                    TotalAgostoPatologiaTXVertical = (TotalPatologiaAnualAgostoVertical <= 0 ? 0 : ((getPatologiasTraumaAño["Agosto"] * 100)) / TotalPatologiaAnualAgostoVertical);
                    TotalAgostoPatologiaUVVertical = (TotalPatologiaAnualAgostoVertical <= 0 ? 0 : ((getPatologiasVascularesAño["Agosto"] * 100)) / TotalPatologiaAnualAgostoVertical);
                    TotalAgostoPatologiaOtrosVertical = (TotalPatologiaAnualAgostoVertical <= 0 ? 0 : ((getPatologiasOtrosAño["Agosto"] * 100)) / TotalPatologiaAnualAgostoVertical);

                    TotalSeptiembrePatologiaPOPVertical = (TotalPatologiaAnualSeptiembreVertical <= 0 ? 0 : ((getPatologiasPosOperatorioAño["Septiembre"] * 100)) / TotalPatologiaAnualSeptiembreVertical);
                    TotalSeptiembrePatologiaZPVertical = (TotalPatologiaAnualSeptiembreVertical <= 0 ? 0 : ((getPatologiasZPAño["Septiembre"] * 100)) / TotalPatologiaAnualSeptiembreVertical);
                    TotalSeptiembrePatologiaPDVertical = (TotalPatologiaAnualSeptiembreVertical <= 0 ? 0 : ((getPatologiasPieDiabeticoAño["Septiembre"] * 100)) / TotalPatologiaAnualSeptiembreVertical);
                    TotalSeptiembrePatologiaQXVertical = (TotalPatologiaAnualSeptiembreVertical <= 0 ? 0 : ((getPatologiasQuemadoAño["Septiembre"] * 100)) / TotalPatologiaAnualSeptiembreVertical);
                    TotalSeptiembrePatologiaTXVertical = (TotalPatologiaAnualSeptiembreVertical <= 0 ? 0 : ((getPatologiasTraumaAño["Septiembre"] * 100)) / TotalPatologiaAnualSeptiembreVertical);
                    TotalSeptiembrePatologiaUVVertical = (TotalPatologiaAnualSeptiembreVertical <= 0 ? 0 : ((getPatologiasVascularesAño["Septiembre"] * 100)) / TotalPatologiaAnualSeptiembreVertical);
                    TotalSeptiembrePatologiaOtrosVertical = (TotalPatologiaAnualSeptiembreVertical <= 0 ? 0 : ((getPatologiasOtrosAño["Septiembre"] * 100)) / TotalPatologiaAnualSeptiembreVertical);

                    TotalOctubrePatologiaPOPVertical = (TotalPatologiaAnualOctubreVertical <= 0 ? 0 : ((getPatologiasPosOperatorioAño["Octubre"] * 100)) / TotalPatologiaAnualOctubreVertical);
                    TotalOctubrePatologiaZPVertical = (TotalPatologiaAnualOctubreVertical <= 0 ? 0 : ((getPatologiasZPAño["Octubre"] * 100)) / TotalPatologiaAnualOctubreVertical);
                    TotalOctubrePatologiaPDVertical = (TotalPatologiaAnualOctubreVertical <= 0 ? 0 : ((getPatologiasPieDiabeticoAño["Octubre"] * 100)) / TotalPatologiaAnualOctubreVertical);
                    TotalOctubrePatologiaQXVertical = (TotalPatologiaAnualOctubreVertical <= 0 ? 0 : ((getPatologiasQuemadoAño["Octubre"] * 100)) / TotalPatologiaAnualOctubreVertical);
                    TotalOctubrePatologiaTXVertical = (TotalPatologiaAnualOctubreVertical <= 0 ? 0 : ((getPatologiasTraumaAño["Octubre"] * 100)) / TotalPatologiaAnualOctubreVertical);
                    TotalOctubrePatologiaUVVertical = (TotalPatologiaAnualOctubreVertical <= 0 ? 0 : ((getPatologiasVascularesAño["Octubre"] * 100)) / TotalPatologiaAnualOctubreVertical);
                    TotalOctubrePatologiaOtrosVertical = (TotalPatologiaAnualOctubreVertical <= 0 ? 0 : ((getPatologiasOtrosAño["Octubre"] * 100)) / TotalPatologiaAnualOctubreVertical);

                    TotalNoviembrePatologiaPOPVertical = (TotalPatologiaAnualNoviembreVertical <= 0 ? 0 : ((getPatologiasPosOperatorioAño["Noviembre"] * 100)) / TotalPatologiaAnualNoviembreVertical);
                    TotalNoviembrePatologiaZPVertical = (TotalPatologiaAnualNoviembreVertical <= 0 ? 0 : ((getPatologiasZPAño["Noviembre"] * 100)) / TotalPatologiaAnualNoviembreVertical);
                    TotalNoviembrePatologiaPDVertical = (TotalPatologiaAnualNoviembreVertical <= 0 ? 0 : ((getPatologiasPieDiabeticoAño["Noviembre"] * 100)) / TotalPatologiaAnualNoviembreVertical);
                    TotalNoviembrePatologiaQXVertical = (TotalPatologiaAnualNoviembreVertical <= 0 ? 0 : ((getPatologiasQuemadoAño["Noviembre"] * 100)) / TotalPatologiaAnualNoviembreVertical);
                    TotalNoviembrePatologiaTXVertical = (TotalPatologiaAnualNoviembreVertical <= 0 ? 0 : ((getPatologiasTraumaAño["Noviembre"] * 100)) / TotalPatologiaAnualNoviembreVertical);
                    TotalNoviembrePatologiaUVVertical = (TotalPatologiaAnualNoviembreVertical <= 0 ? 0 : ((getPatologiasVascularesAño["Noviembre"] * 100)) / TotalPatologiaAnualNoviembreVertical);
                    TotalNoviembrePatologiaOtrosVertical = (TotalPatologiaAnualNoviembreVertical <= 0 ? 0 : ((getPatologiasOtrosAño["Noviembre"] * 100)) / TotalPatologiaAnualNoviembreVertical);
                }
                else if (comboBox1.Text == "Diciembre" || comboBox1.Text == "DICIEMBRE")
                {
                    TotalEneroPatologiaPOPVertical = (TotalPatologiaAnualEneroVertical <= 0 ? 0 : ((getPatologiasPosOperatorioAño["Enero"] * 100)) / TotalPatologiaAnualEneroVertical);
                    TotalEneroPatologiaZPVertical = (TotalPatologiaAnualEneroVertical <= 0 ? 0 : ((getPatologiasZPAño["Enero"] * 100)) / TotalPatologiaAnualEneroVertical);
                    TotalEneroPatologiaPDVertical = (TotalPatologiaAnualEneroVertical <= 0 ? 0 : ((getPatologiasPieDiabeticoAño["Enero"] * 100)) / TotalPatologiaAnualEneroVertical);
                    TotalEneroPatologiaQXVertical = (TotalPatologiaAnualEneroVertical <= 0 ? 0 : ((getPatologiasQuemadoAño["Enero"] * 100)) / TotalPatologiaAnualEneroVertical);
                    TotalEneroPatologiaTXVertical = (TotalPatologiaAnualEneroVertical <= 0 ? 0 : ((getPatologiasTraumaAño["Enero"] * 100)) / TotalPatologiaAnualEneroVertical);
                    TotalEneroPatologiaUVVertical = (TotalPatologiaAnualEneroVertical <= 0 ? 0 : ((getPatologiasVascularesAño["Enero"] * 100)) / TotalPatologiaAnualEneroVertical);
                    TotalEneroPatologiaOtrosVertical = (TotalPatologiaAnualEneroVertical <= 0 ? 0 : ((getPatologiasOtrosAño["Enero"] * 100)) / TotalPatologiaAnualEneroVertical);

                    TotalFebreroPatologiaPOPVertical = (TotalPatologiaAnualFebreroVertical <= 0 ? 0 : ((getPatologiasPosOperatorioAño["Febrero"] * 100)) / TotalPatologiaAnualFebreroVertical);
                    TotalFebreroPatologiaZPVertical = (TotalPatologiaAnualFebreroVertical <= 0 ? 0 : ((getPatologiasZPAño["Febrero"] * 100)) / TotalPatologiaAnualFebreroVertical);
                    TotalFebreroPatologiaPDVertical = (TotalPatologiaAnualFebreroVertical <= 0 ? 0 : ((getPatologiasPieDiabeticoAño["Febrero"] * 100)) / TotalPatologiaAnualFebreroVertical);
                    TotalFebreroPatologiaQXVertical = (TotalPatologiaAnualFebreroVertical <= 0 ? 0 : ((getPatologiasQuemadoAño["Febrero"] * 100)) / TotalPatologiaAnualFebreroVertical);
                    TotalFebreroPatologiaTXVertical = (TotalPatologiaAnualFebreroVertical <= 0 ? 0 : ((getPatologiasTraumaAño["Febrero"] * 100)) / TotalPatologiaAnualFebreroVertical);
                    TotalFebreroPatologiaUVVertical = (TotalPatologiaAnualFebreroVertical <= 0 ? 0 : ((getPatologiasVascularesAño["Febrero"] * 100)) / TotalPatologiaAnualFebreroVertical);
                    TotalFebreroPatologiaOtrosVertical = (TotalPatologiaAnualFebreroVertical <= 0 ? 0 : ((getPatologiasOtrosAño["Febrero"] * 100)) / TotalPatologiaAnualFebreroVertical);

                    TotalMarzoPatologiaPOPVertical = (TotalPatologiaAnualMarzoVertical <= 0 ? 0 : ((getPatologiasPosOperatorioAño["Marzo"] * 100)) / TotalPatologiaAnualMarzoVertical);
                    TotalMarzoPatologiaZPVertical = (TotalPatologiaAnualMarzoVertical <= 0 ? 0 : ((getPatologiasZPAño["Marzo"] * 100)) / TotalPatologiaAnualMarzoVertical);
                    TotalMarzoPatologiaPDVertical = (TotalPatologiaAnualMarzoVertical <= 0 ? 0 : ((getPatologiasPieDiabeticoAño["Marzo"] * 100)) / TotalPatologiaAnualMarzoVertical);
                    TotalMarzoPatologiaQXVertical = (TotalPatologiaAnualMarzoVertical <= 0 ? 0 : ((getPatologiasQuemadoAño["Marzo"] * 100)) / TotalPatologiaAnualMarzoVertical);
                    TotalMarzoPatologiaTXVertical = (TotalPatologiaAnualMarzoVertical <= 0 ? 0 : ((getPatologiasTraumaAño["Marzo"] * 100)) / TotalPatologiaAnualMarzoVertical);
                    TotalMarzoPatologiaUVVertical = (TotalPatologiaAnualMarzoVertical <= 0 ? 0 : ((getPatologiasVascularesAño["Marzo"] * 100)) / TotalPatologiaAnualMarzoVertical);
                    TotalMarzoPatologiaOtrosVertical = (TotalPatologiaAnualMarzoVertical <= 0 ? 0 : ((getPatologiasOtrosAño["Marzo"] * 100)) / TotalPatologiaAnualMarzoVertical);

                    TotalAbrilPatologiaPOPVertical = (TotalPatologiaAnualAbrilVertical <= 0 ? 0 : ((getPatologiasPosOperatorioAño["Abril"] * 100)) / TotalPatologiaAnualAbrilVertical);
                    TotalAbrilPatologiaZPVertical = (TotalPatologiaAnualAbrilVertical <= 0 ? 0 : ((getPatologiasZPAño["Abril"] * 100)) / TotalPatologiaAnualAbrilVertical);
                    TotalAbrilPatologiaPDVertical = (TotalPatologiaAnualAbrilVertical <= 0 ? 0 : ((getPatologiasPieDiabeticoAño["Abril"] * 100)) / TotalPatologiaAnualAbrilVertical);
                    TotalAbrilPatologiaQXVertical = (TotalPatologiaAnualAbrilVertical <= 0 ? 0 : ((getPatologiasQuemadoAño["Abril"] * 100)) / TotalPatologiaAnualAbrilVertical);
                    TotalAbrilPatologiaTXVertical = (TotalPatologiaAnualAbrilVertical <= 0 ? 0 : ((getPatologiasTraumaAño["Abril"] * 100)) / TotalPatologiaAnualAbrilVertical);
                    TotalAbrilPatologiaUVVertical = (TotalPatologiaAnualAbrilVertical <= 0 ? 0 : ((getPatologiasVascularesAño["Abril"] * 100)) / TotalPatologiaAnualAbrilVertical);
                    TotalAbrilPatologiaOtrosVertical = (TotalPatologiaAnualAbrilVertical <= 0 ? 0 : ((getPatologiasOtrosAño["Abril"] * 100)) / TotalPatologiaAnualAbrilVertical);

                    TotalMayoPatologiaPOPVertical = (TotalPatologiaAnualMayoVertical <= 0 ? 0 : ((getPatologiasPosOperatorioAño["Mayo"] * 100)) / TotalPatologiaAnualMayoVertical);
                    TotalMayoPatologiaZPVertical = (TotalPatologiaAnualMayoVertical <= 0 ? 0 : ((getPatologiasZPAño["Mayo"] * 100)) / TotalPatologiaAnualMayoVertical);
                    TotalMayoPatologiaPDVertical = (TotalPatologiaAnualMayoVertical <= 0 ? 0 : ((getPatologiasPieDiabeticoAño["Mayo"] * 100)) / TotalPatologiaAnualMayoVertical);
                    TotalMayoPatologiaQXVertical = (TotalPatologiaAnualMayoVertical <= 0 ? 0 : ((getPatologiasQuemadoAño["Mayo"] * 100)) / TotalPatologiaAnualMayoVertical);
                    TotalMayoPatologiaTXVertical = (TotalPatologiaAnualMayoVertical <= 0 ? 0 : ((getPatologiasTraumaAño["Mayo"] * 100)) / TotalPatologiaAnualMayoVertical);
                    TotalMayoPatologiaUVVertical = (TotalPatologiaAnualMayoVertical <= 0 ? 0 : ((getPatologiasVascularesAño["Mayo"] * 100)) / TotalPatologiaAnualMayoVertical);
                    TotalMayoPatologiaOtrosVertical = (TotalPatologiaAnualMayoVertical <= 0 ? 0 : ((getPatologiasOtrosAño["Mayo"] * 100)) / TotalPatologiaAnualMayoVertical);

                    TotalJunioPatologiaPOPVertical = (TotalPatologiaAnualJunioVertical <= 0 ? 0 : ((getPatologiasPosOperatorioAño["Junio"] * 100)) / TotalPatologiaAnualJunioVertical);
                    TotalJunioPatologiaZPVertical = (TotalPatologiaAnualJunioVertical <= 0 ? 0 : ((getPatologiasZPAño["Junio"] * 100)) / TotalPatologiaAnualJunioVertical);
                    TotalJunioPatologiaPDVertical = (TotalPatologiaAnualJunioVertical <= 0 ? 0 : ((getPatologiasPieDiabeticoAño["Junio"] * 100)) / TotalPatologiaAnualJunioVertical);
                    TotalJunioPatologiaQXVertical = (TotalPatologiaAnualJunioVertical <= 0 ? 0 : ((getPatologiasQuemadoAño["Junio"] * 100)) / TotalPatologiaAnualJunioVertical);
                    TotalJunioPatologiaTXVertical = (TotalPatologiaAnualJunioVertical <= 0 ? 0 : ((getPatologiasTraumaAño["Junio"] * 100)) / TotalPatologiaAnualJunioVertical);
                    TotalJunioPatologiaUVVertical = (TotalPatologiaAnualJunioVertical <= 0 ? 0 : ((getPatologiasVascularesAño["Junio"] * 100)) / TotalPatologiaAnualJunioVertical);
                    TotalJunioPatologiaOtrosVertical = (TotalPatologiaAnualJunioVertical <= 0 ? 0 : ((getPatologiasOtrosAño["Junio"] * 100)) / TotalPatologiaAnualJunioVertical);

                    TotalJulioPatologiaPOPVertical = (TotalPatologiaAnualJulioVertical <= 0 ? 0 : ((getPatologiasPosOperatorioAño["Julio"] * 100)) / TotalPatologiaAnualJulioVertical);
                    TotalJulioPatologiaZPVertical = (TotalPatologiaAnualJulioVertical <= 0 ? 0 : ((getPatologiasZPAño["Julio"] * 100)) / TotalPatologiaAnualJulioVertical);
                    TotalJulioPatologiaPDVertical = (TotalPatologiaAnualJulioVertical <= 0 ? 0 : ((getPatologiasPieDiabeticoAño["Julio"] * 100)) / TotalPatologiaAnualJulioVertical);
                    TotalJulioPatologiaQXVertical = (TotalPatologiaAnualJulioVertical <= 0 ? 0 : ((getPatologiasQuemadoAño["Julio"] * 100)) / TotalPatologiaAnualJulioVertical);
                    TotalJulioPatologiaTXVertical = (TotalPatologiaAnualJulioVertical <= 0 ? 0 : ((getPatologiasTraumaAño["Julio"] * 100)) / TotalPatologiaAnualJulioVertical);
                    TotalJulioPatologiaUVVertical = (TotalPatologiaAnualJulioVertical <= 0 ? 0 : ((getPatologiasVascularesAño["Julio"] * 100)) / TotalPatologiaAnualJulioVertical);
                    TotalJulioPatologiaOtrosVertical = (TotalPatologiaAnualJulioVertical <= 0 ? 0 : ((getPatologiasOtrosAño["Julio"] * 100)) / TotalPatologiaAnualJulioVertical);

                    TotalAgostoPatologiaPOPVertical = (TotalPatologiaAnualAgostoVertical <= 0 ? 0 : ((getPatologiasPosOperatorioAño["Agosto"] * 100)) / TotalPatologiaAnualAgostoVertical);
                    TotalAgostoPatologiaZPVertical = (TotalPatologiaAnualAgostoVertical <= 0 ? 0 : ((getPatologiasZPAño["Agosto"] * 100)) / TotalPatologiaAnualAgostoVertical);
                    TotalAgostoPatologiaPDVertical = (TotalPatologiaAnualAgostoVertical <= 0 ? 0 : ((getPatologiasPieDiabeticoAño["Agosto"] * 100)) / TotalPatologiaAnualAgostoVertical);
                    TotalAgostoPatologiaQXVertical = (TotalPatologiaAnualAgostoVertical <= 0 ? 0 : ((getPatologiasQuemadoAño["Agosto"] * 100)) / TotalPatologiaAnualAgostoVertical);
                    TotalAgostoPatologiaTXVertical = (TotalPatologiaAnualAgostoVertical <= 0 ? 0 : ((getPatologiasTraumaAño["Agosto"] * 100)) / TotalPatologiaAnualAgostoVertical);
                    TotalAgostoPatologiaUVVertical = (TotalPatologiaAnualAgostoVertical <= 0 ? 0 : ((getPatologiasVascularesAño["Agosto"] * 100)) / TotalPatologiaAnualAgostoVertical);
                    TotalAgostoPatologiaOtrosVertical = (TotalPatologiaAnualAgostoVertical <= 0 ? 0 : ((getPatologiasOtrosAño["Agosto"] * 100)) / TotalPatologiaAnualAgostoVertical);

                    TotalSeptiembrePatologiaPOPVertical = (TotalPatologiaAnualSeptiembreVertical <= 0 ? 0 : ((getPatologiasPosOperatorioAño["Septiembre"] * 100)) / TotalPatologiaAnualSeptiembreVertical);
                    TotalSeptiembrePatologiaZPVertical = (TotalPatologiaAnualSeptiembreVertical <= 0 ? 0 : ((getPatologiasZPAño["Septiembre"] * 100)) / TotalPatologiaAnualSeptiembreVertical);
                    TotalSeptiembrePatologiaPDVertical = (TotalPatologiaAnualSeptiembreVertical <= 0 ? 0 : ((getPatologiasPieDiabeticoAño["Septiembre"] * 100)) / TotalPatologiaAnualSeptiembreVertical);
                    TotalSeptiembrePatologiaQXVertical = (TotalPatologiaAnualSeptiembreVertical <= 0 ? 0 : ((getPatologiasQuemadoAño["Septiembre"] * 100)) / TotalPatologiaAnualSeptiembreVertical);
                    TotalSeptiembrePatologiaTXVertical = (TotalPatologiaAnualSeptiembreVertical <= 0 ? 0 : ((getPatologiasTraumaAño["Septiembre"] * 100)) / TotalPatologiaAnualSeptiembreVertical);
                    TotalSeptiembrePatologiaUVVertical = (TotalPatologiaAnualSeptiembreVertical <= 0 ? 0 : ((getPatologiasVascularesAño["Septiembre"] * 100)) / TotalPatologiaAnualSeptiembreVertical);
                    TotalSeptiembrePatologiaOtrosVertical = (TotalPatologiaAnualSeptiembreVertical <= 0 ? 0 : ((getPatologiasOtrosAño["Septiembre"] * 100)) / TotalPatologiaAnualSeptiembreVertical);

                    TotalOctubrePatologiaPOPVertical = (TotalPatologiaAnualOctubreVertical <= 0 ? 0 : ((getPatologiasPosOperatorioAño["Octubre"] * 100)) / TotalPatologiaAnualOctubreVertical);
                    TotalOctubrePatologiaZPVertical = (TotalPatologiaAnualOctubreVertical <= 0 ? 0 : ((getPatologiasZPAño["Octubre"] * 100)) / TotalPatologiaAnualOctubreVertical);
                    TotalOctubrePatologiaPDVertical = (TotalPatologiaAnualOctubreVertical <= 0 ? 0 : ((getPatologiasPieDiabeticoAño["Octubre"] * 100)) / TotalPatologiaAnualOctubreVertical);
                    TotalOctubrePatologiaQXVertical = (TotalPatologiaAnualOctubreVertical <= 0 ? 0 : ((getPatologiasQuemadoAño["Octubre"] * 100)) / TotalPatologiaAnualOctubreVertical);
                    TotalOctubrePatologiaTXVertical = (TotalPatologiaAnualOctubreVertical <= 0 ? 0 : ((getPatologiasTraumaAño["Octubre"] * 100)) / TotalPatologiaAnualOctubreVertical);
                    TotalOctubrePatologiaUVVertical = (TotalPatologiaAnualOctubreVertical <= 0 ? 0 : ((getPatologiasVascularesAño["Octubre"] * 100)) / TotalPatologiaAnualOctubreVertical);
                    TotalOctubrePatologiaOtrosVertical = (TotalPatologiaAnualOctubreVertical <= 0 ? 0 : ((getPatologiasOtrosAño["Octubre"] * 100)) / TotalPatologiaAnualOctubreVertical);

                    TotalNoviembrePatologiaPOPVertical = (TotalPatologiaAnualNoviembreVertical <= 0 ? 0 : ((getPatologiasPosOperatorioAño["Noviembre"] * 100)) / TotalPatologiaAnualNoviembreVertical);
                    TotalNoviembrePatologiaZPVertical = (TotalPatologiaAnualNoviembreVertical <= 0 ? 0 : ((getPatologiasZPAño["Noviembre"] * 100)) / TotalPatologiaAnualNoviembreVertical);
                    TotalNoviembrePatologiaPDVertical = (TotalPatologiaAnualNoviembreVertical <= 0 ? 0 : ((getPatologiasPieDiabeticoAño["Noviembre"] * 100)) / TotalPatologiaAnualNoviembreVertical);
                    TotalNoviembrePatologiaQXVertical = (TotalPatologiaAnualNoviembreVertical <= 0 ? 0 : ((getPatologiasQuemadoAño["Noviembre"] * 100)) / TotalPatologiaAnualNoviembreVertical);
                    TotalNoviembrePatologiaTXVertical = (TotalPatologiaAnualNoviembreVertical <= 0 ? 0 : ((getPatologiasTraumaAño["Noviembre"] * 100)) / TotalPatologiaAnualNoviembreVertical);
                    TotalNoviembrePatologiaUVVertical = (TotalPatologiaAnualNoviembreVertical <= 0 ? 0 : ((getPatologiasVascularesAño["Noviembre"] * 100)) / TotalPatologiaAnualNoviembreVertical);
                    TotalNoviembrePatologiaOtrosVertical = (TotalPatologiaAnualNoviembreVertical <= 0 ? 0 : ((getPatologiasOtrosAño["Noviembre"] * 100)) / TotalPatologiaAnualNoviembreVertical);

                    TotalDiciembrePatologiaPOPVertical = (TotalPatologiaAnualDiciembreVertical <= 0 ? 0 : ((getPatologiasPosOperatorioAño["Diciembre"] * 100)) / TotalPatologiaAnualDiciembreVertical);
                    TotalDiciembrePatologiaZPVertical = (TotalPatologiaAnualDiciembreVertical <= 0 ? 0 : ((getPatologiasZPAño["Diciembre"] * 100)) / TotalPatologiaAnualDiciembreVertical);
                    TotalDiciembrePatologiaPDVertical = (TotalPatologiaAnualDiciembreVertical <= 0 ? 0 : ((getPatologiasPieDiabeticoAño["Diciembre"] * 100)) / TotalPatologiaAnualDiciembreVertical);
                    TotalDiciembrePatologiaQXVertical = (TotalPatologiaAnualDiciembreVertical <= 0 ? 0 : ((getPatologiasQuemadoAño["Diciembre"] * 100)) / TotalPatologiaAnualDiciembreVertical);
                    TotalDiciembrePatologiaTXVertical = (TotalPatologiaAnualDiciembreVertical <= 0 ? 0 : ((getPatologiasTraumaAño["Diciembre"] * 100)) / TotalPatologiaAnualDiciembreVertical);
                    TotalDiciembrePatologiaUVVertical = (TotalPatologiaAnualDiciembreVertical <= 0 ? 0 : ((getPatologiasVascularesAño["Diciembre"] * 100)) / TotalPatologiaAnualDiciembreVertical);
                    TotalDiciembrePatologiaOtrosVertical = (TotalPatologiaAnualDiciembreVertical <= 0 ? 0 : ((getPatologiasOtrosAño["Diciembre"] * 100)) / TotalPatologiaAnualDiciembreVertical);
                }
              
                #endregion

                int Cant4160 = repositorioInfEnf.getCantidadEdad(comboBox1.Text, comboBox2.Text, "41 a 60 años");
                int Cant6180 = repositorioInfEnf.getCantidadEdad(comboBox1.Text, comboBox2.Text, "61 a 80 años");
                int Cant2140 = repositorioInfEnf.getCantidadEdad(comboBox1.Text, comboBox2.Text, "21 a 40 años");
                int Cant80 = repositorioInfEnf.getCantidadEdad(comboBox1.Text, comboBox2.Text, "> a 80 años");
                int Cant020 = repositorioInfEnf.getCantidadEdad(comboBox1.Text, comboBox2.Text, "0 a 20 años");

                byte[] getGraph3 = Grafico3(Cant4160, Cant6180, Cant2140, Cant80, Cant020);

                List<int> getEnfermerosAño = repositorioInfEnf.getEnfermerosYear(comboBox2.Text);
                if (getEnfermerosAño == null)
                {
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "No hay resultados en enfermeria del año";
                    MG.ShowDialog();
                    return;
                }                

                int cantAtendidosYearEnero = 0; int cantAtendidosYearFebrero = 0; int cantAtendidosYearMarzo = 0; int cantAtendidosYearAbril = 0;
                int cantAtendidosYearMayo = 0; int cantAtendidosYearJunio = 0; int cantAtendidosYearJulio = 0; int cantAtendidosYearAgosto = 0;
                int cantAtendidosYearSeptiembre = 0; int cantAtendidosYearOctubre = 0; int cantAtendidosYearNoviembre = 0; int cantAtendidosYearDiciembre = 0;
                int SumatoriaYear = 0;

                int cantAtendidosYearEneroGeneral = 0; int cantAtendidosYearFebreroGeneral = 0; int cantAtendidosYearMarzoGeneral = 0;
                int cantAtendidosYearAbrilGeneral = 0; int cantAtendidosYearMayoGeneral = 0; int cantAtendidosYearJunioGeneral = 0;
                int cantAtendidosYearAgostoGeneral = 0; int cantAtendidosYearSeptiembreGeneral = 0; int cantAtendidosYearOctubreGeneral = 0;
                int cantAtendidosYearNoviembreGeneral = 0; int cantAtendidosYearDiciembreGeneral = 0; int cantAtendidosYearJulioGeneral = 0;
                int SumatoriaYearGeneral = 0;

                double cantAtendidosYearEneroPorcentual = 0; double cantAtendidosYearFebreroPorcentual = 0; double cantAtendidosYearMarzoPorcentual = 0;
                double cantAtendidosYearAbrilPorcentual = 0; double cantAtendidosYearMayoPorcentual = 0; double cantAtendidosYearJunioPorcentual = 0;
                double cantAtendidosYearJulioPorcentual = 0; double cantAtendidosYearAgostoPorcentual = 0; double cantAtendidosYearSeptiembrePorcentual = 0;
                double cantAtendidosYearOctubrePorcentual = 0; double cantAtendidosYearNoviembrePorcentual = 0; double cantAtendidosYearDiciembrePorcentual = 0;

                double cantAtendidosYearPromedio = 0;

                //SUMA TOTAL DEL MES DE TODOS
                if (comboBox1.Text == "Enero" || comboBox1.Text == "ENERO")
                {
                    cantAtendidosYearEneroGeneral = repositorioInfEnf.getCantidadAtendidos2("Enero", comboBox2.Text, "CU");
                }
                else if (comboBox1.Text == "Febrero" || comboBox1.Text == "FEBRERO")
                {
                    cantAtendidosYearEneroGeneral = repositorioInfEnf.getCantidadAtendidos2("Enero", comboBox2.Text, "CU");
                    cantAtendidosYearFebreroGeneral = repositorioInfEnf.getCantidadAtendidos2("Febrero", comboBox2.Text, "CU");
                }
                else if (comboBox1.Text == "Marzo" || comboBox1.Text == "MARZO")
                {
                    cantAtendidosYearEneroGeneral = repositorioInfEnf.getCantidadAtendidos2("Enero", comboBox2.Text, "CU");
                    cantAtendidosYearFebreroGeneral = repositorioInfEnf.getCantidadAtendidos2("Febrero", comboBox2.Text, "CU");
                    cantAtendidosYearMarzoGeneral = repositorioInfEnf.getCantidadAtendidos2("Marzo", comboBox2.Text, "CU");
                }
                else if (comboBox1.Text == "Abril" || comboBox1.Text == "ABRIL")
                {
                    cantAtendidosYearEneroGeneral = repositorioInfEnf.getCantidadAtendidos2("Enero", comboBox2.Text, "CU");
                    cantAtendidosYearFebreroGeneral = repositorioInfEnf.getCantidadAtendidos2("Febrero", comboBox2.Text, "CU");
                    cantAtendidosYearMarzoGeneral = repositorioInfEnf.getCantidadAtendidos2("Marzo", comboBox2.Text, "CU");
                    cantAtendidosYearAbrilGeneral = repositorioInfEnf.getCantidadAtendidos2("Abril", comboBox2.Text, "CU");
                }
                else if (comboBox1.Text == "Mayo" || comboBox1.Text == "MAYO")
                {
                    cantAtendidosYearEneroGeneral = repositorioInfEnf.getCantidadAtendidos2("Enero", comboBox2.Text, "CU");
                    cantAtendidosYearFebreroGeneral = repositorioInfEnf.getCantidadAtendidos2("Febrero", comboBox2.Text, "CU");
                    cantAtendidosYearMarzoGeneral = repositorioInfEnf.getCantidadAtendidos2("Marzo", comboBox2.Text, "CU");
                    cantAtendidosYearAbrilGeneral = repositorioInfEnf.getCantidadAtendidos2("Abril", comboBox2.Text, "CU");
                    cantAtendidosYearMayoGeneral = repositorioInfEnf.getCantidadAtendidos2("Mayo", comboBox2.Text, "CU");
                }
                else if (comboBox1.Text == "Junio" || comboBox1.Text == "JUNIO")
                {
                    cantAtendidosYearEneroGeneral = repositorioInfEnf.getCantidadAtendidos2("Enero", comboBox2.Text, "CU");
                    cantAtendidosYearFebreroGeneral = repositorioInfEnf.getCantidadAtendidos2("Febrero", comboBox2.Text, "CU");
                    cantAtendidosYearMarzoGeneral = repositorioInfEnf.getCantidadAtendidos2("Marzo", comboBox2.Text, "CU");
                    cantAtendidosYearAbrilGeneral = repositorioInfEnf.getCantidadAtendidos2("Abril", comboBox2.Text, "CU");
                    cantAtendidosYearMayoGeneral = repositorioInfEnf.getCantidadAtendidos2("Mayo", comboBox2.Text, "CU");
                    cantAtendidosYearJunioGeneral = repositorioInfEnf.getCantidadAtendidos2("Junio", comboBox2.Text, "CU");
                }
                else if (comboBox1.Text == "Julio" || comboBox1.Text == "JULIO")
                {
                    cantAtendidosYearEneroGeneral = repositorioInfEnf.getCantidadAtendidos2("Enero", comboBox2.Text, "CU");
                    cantAtendidosYearFebreroGeneral = repositorioInfEnf.getCantidadAtendidos2("Febrero", comboBox2.Text, "CU");
                    cantAtendidosYearMarzoGeneral = repositorioInfEnf.getCantidadAtendidos2("Marzo", comboBox2.Text, "CU");
                    cantAtendidosYearAbrilGeneral = repositorioInfEnf.getCantidadAtendidos2("Abril", comboBox2.Text, "CU");
                    cantAtendidosYearMayoGeneral = repositorioInfEnf.getCantidadAtendidos2("Mayo", comboBox2.Text, "CU");
                    cantAtendidosYearJunioGeneral = repositorioInfEnf.getCantidadAtendidos2("Junio", comboBox2.Text, "CU");
                    cantAtendidosYearJulioGeneral = repositorioInfEnf.getCantidadAtendidos2("Julio", comboBox2.Text, "CU");
                }
                else if (comboBox1.Text == "Agosto" || comboBox1.Text == "AGOSTO")
                {
                    cantAtendidosYearEneroGeneral = repositorioInfEnf.getCantidadAtendidos2("Enero", comboBox2.Text, "CU");
                    cantAtendidosYearFebreroGeneral = repositorioInfEnf.getCantidadAtendidos2("Febrero", comboBox2.Text, "CU");
                    cantAtendidosYearMarzoGeneral = repositorioInfEnf.getCantidadAtendidos2("Marzo", comboBox2.Text, "CU");
                    cantAtendidosYearAbrilGeneral = repositorioInfEnf.getCantidadAtendidos2("Abril", comboBox2.Text, "CU");
                    cantAtendidosYearMayoGeneral = repositorioInfEnf.getCantidadAtendidos2("Mayo", comboBox2.Text, "CU");
                    cantAtendidosYearJunioGeneral = repositorioInfEnf.getCantidadAtendidos2("Junio", comboBox2.Text, "CU");
                    cantAtendidosYearJulioGeneral = repositorioInfEnf.getCantidadAtendidos2("Julio", comboBox2.Text, "CU");
                    cantAtendidosYearAgostoGeneral = repositorioInfEnf.getCantidadAtendidos2("Agosto", comboBox2.Text, "CU");
                }
                else if (comboBox1.Text == "Septiembre" || comboBox1.Text == "SEPTIEMBRE")
                {
                    cantAtendidosYearEneroGeneral = repositorioInfEnf.getCantidadAtendidos2("Enero", comboBox2.Text, "CU");
                    cantAtendidosYearFebreroGeneral = repositorioInfEnf.getCantidadAtendidos2("Febrero", comboBox2.Text, "CU");
                    cantAtendidosYearMarzoGeneral = repositorioInfEnf.getCantidadAtendidos2("Marzo", comboBox2.Text, "CU");
                    cantAtendidosYearAbrilGeneral = repositorioInfEnf.getCantidadAtendidos2("Abril", comboBox2.Text, "CU");
                    cantAtendidosYearMayoGeneral = repositorioInfEnf.getCantidadAtendidos2("Mayo", comboBox2.Text, "CU");
                    cantAtendidosYearJunioGeneral = repositorioInfEnf.getCantidadAtendidos2("Junio", comboBox2.Text, "CU");
                    cantAtendidosYearJulioGeneral = repositorioInfEnf.getCantidadAtendidos2("Julio", comboBox2.Text, "CU");
                    cantAtendidosYearAgostoGeneral = repositorioInfEnf.getCantidadAtendidos2("Agosto", comboBox2.Text, "CU");
                    cantAtendidosYearSeptiembreGeneral = repositorioInfEnf.getCantidadAtendidos2("Septiembre", comboBox2.Text, "CU");
                }
                else if (comboBox1.Text == "Octubre" || comboBox1.Text == "OCTUBRE")
                {
                    cantAtendidosYearEneroGeneral = repositorioInfEnf.getCantidadAtendidos2("Enero", comboBox2.Text, "CU");
                    cantAtendidosYearFebreroGeneral = repositorioInfEnf.getCantidadAtendidos2("Febrero", comboBox2.Text, "CU");
                    cantAtendidosYearMarzoGeneral = repositorioInfEnf.getCantidadAtendidos2("Marzo", comboBox2.Text, "CU");
                    cantAtendidosYearAbrilGeneral = repositorioInfEnf.getCantidadAtendidos2("Abril", comboBox2.Text, "CU");
                    cantAtendidosYearMayoGeneral = repositorioInfEnf.getCantidadAtendidos2("Mayo", comboBox2.Text, "CU");
                    cantAtendidosYearJunioGeneral = repositorioInfEnf.getCantidadAtendidos2("Junio", comboBox2.Text, "CU");
                    cantAtendidosYearJulioGeneral = repositorioInfEnf.getCantidadAtendidos2("Julio", comboBox2.Text, "CU");
                    cantAtendidosYearAgostoGeneral = repositorioInfEnf.getCantidadAtendidos2("Agosto", comboBox2.Text, "CU");
                    cantAtendidosYearSeptiembreGeneral = repositorioInfEnf.getCantidadAtendidos2("Septiembre", comboBox2.Text, "CU");
                    cantAtendidosYearOctubreGeneral = repositorioInfEnf.getCantidadAtendidos2("Octubre", comboBox2.Text, "CU");
                }
                else if (comboBox1.Text == "Noviembre" || comboBox1.Text == "NOVIEMBRE")
                {
                    cantAtendidosYearEneroGeneral = repositorioInfEnf.getCantidadAtendidos2("Enero", comboBox2.Text, "CU");
                    cantAtendidosYearFebreroGeneral = repositorioInfEnf.getCantidadAtendidos2("Febrero", comboBox2.Text, "CU");
                    cantAtendidosYearMarzoGeneral = repositorioInfEnf.getCantidadAtendidos2("Marzo", comboBox2.Text, "CU");
                    cantAtendidosYearAbrilGeneral = repositorioInfEnf.getCantidadAtendidos2("Abril", comboBox2.Text, "CU");
                    cantAtendidosYearMayoGeneral = repositorioInfEnf.getCantidadAtendidos2("Mayo", comboBox2.Text, "CU");
                    cantAtendidosYearJunioGeneral = repositorioInfEnf.getCantidadAtendidos2("Junio", comboBox2.Text, "CU");
                    cantAtendidosYearJulioGeneral = repositorioInfEnf.getCantidadAtendidos2("Julio", comboBox2.Text, "CU");
                    cantAtendidosYearAgostoGeneral = repositorioInfEnf.getCantidadAtendidos2("Agosto", comboBox2.Text, "CU");
                    cantAtendidosYearSeptiembreGeneral = repositorioInfEnf.getCantidadAtendidos2("Septiembre", comboBox2.Text, "CU");
                    cantAtendidosYearOctubreGeneral = repositorioInfEnf.getCantidadAtendidos2("Octubre", comboBox2.Text, "CU");
                    cantAtendidosYearNoviembreGeneral = repositorioInfEnf.getCantidadAtendidos2("Noviembre", comboBox2.Text, "CU");
                }
                else if (comboBox1.Text == "Diciembre" || comboBox1.Text == "DICIEMBRE")
                {
                    cantAtendidosYearEneroGeneral = repositorioInfEnf.getCantidadAtendidos2("Enero", comboBox2.Text, "CU");
                    cantAtendidosYearFebreroGeneral = repositorioInfEnf.getCantidadAtendidos2("Febrero", comboBox2.Text, "CU");
                    cantAtendidosYearMarzoGeneral = repositorioInfEnf.getCantidadAtendidos2("Marzo", comboBox2.Text, "CU");
                    cantAtendidosYearAbrilGeneral = repositorioInfEnf.getCantidadAtendidos2("Abril", comboBox2.Text, "CU");
                    cantAtendidosYearMayoGeneral = repositorioInfEnf.getCantidadAtendidos2("Mayo", comboBox2.Text, "CU");
                    cantAtendidosYearJunioGeneral = repositorioInfEnf.getCantidadAtendidos2("Junio", comboBox2.Text, "CU");
                    cantAtendidosYearJulioGeneral = repositorioInfEnf.getCantidadAtendidos2("Julio", comboBox2.Text, "CU");
                    cantAtendidosYearAgostoGeneral = repositorioInfEnf.getCantidadAtendidos2("Agosto", comboBox2.Text, "CU");
                    cantAtendidosYearSeptiembreGeneral = repositorioInfEnf.getCantidadAtendidos2("Septiembre", comboBox2.Text, "CU");
                    cantAtendidosYearOctubreGeneral = repositorioInfEnf.getCantidadAtendidos2("Octubre", comboBox2.Text, "CU");
                    cantAtendidosYearNoviembreGeneral = repositorioInfEnf.getCantidadAtendidos2("Noviembre", comboBox2.Text, "CU");
                    cantAtendidosYearDiciembreGeneral = repositorioInfEnf.getCantidadAtendidos2("Diciembre", comboBox2.Text, "CU");
                }
                
                SumatoriaYearGeneral = cantAtendidosYearEneroGeneral + cantAtendidosYearFebreroGeneral + cantAtendidosYearMarzoGeneral +
                    cantAtendidosYearAbrilGeneral + cantAtendidosYearMayoGeneral + cantAtendidosYearJunioGeneral + cantAtendidosYearJulioGeneral +
                    cantAtendidosYearAgostoGeneral + cantAtendidosYearSeptiembreGeneral + cantAtendidosYearOctubreGeneral + cantAtendidosYearNoviembreGeneral +
                    cantAtendidosYearDiciembreGeneral;

                int graph1Total = 0;
                Dictionary<int, int> dicGraphics = new Dictionary<int, int>();
                Dictionary<int, string> dicGraphics2 = new Dictionary<int, string>();
                int Contadorgraph = 0;
                string nameProfGraph = "";

                int CantPacEneroCU = 0; int CantPacFebreroCU = 0; int CantPacMarzoCU = 0; int CantPacAbrilCU = 0;
                int CantPacMayoCU = 0; int CantPacJunioCU = 0; int CantPacJulioCU = 0; int CantPacAgostoCU = 0;
                int CantPacSeptiembreCU = 0; int CantPacOctubreCU = 0; int CantPacNoviembreCU = 0; int CantPacDiciembreCU = 0;

                foreach (int graphCant in getEnfermerosAño)
                {
                    //Grafico1;
                    graph1Total = repositorioInfEnf.getCantidadAtendidos(graphCant, comboBox1.Text, comboBox2.Text);
                    nameProfGraph = repositorioBodegas.getDatosCode(graphCant).Bod_Responsable;
                    dicGraphics.Add(Contadorgraph, graph1Total);
                    dicGraphics2.Add(Contadorgraph, nameProfGraph);
                    Contadorgraph++;
                }

                byte[] getGraph1 = Grafico1(dicGraphics, dicGraphics2, "NUMERO DE CURACIONES POR CONSULTORIO");

                foreach (int eYear in getEnfermerosAño)
                {
                    //SUMA TOTAL DELMES POR ENFERMERO
                    cantAtendidosYearEnero = repositorioInfEnf.getCantidadAtendidos(eYear, "Enero", comboBox2.Text);
                    cantAtendidosYearFebrero = repositorioInfEnf.getCantidadAtendidos(eYear, "Febrero", comboBox2.Text);
                    cantAtendidosYearMarzo = repositorioInfEnf.getCantidadAtendidos(eYear, "Marzo", comboBox2.Text);
                    cantAtendidosYearAbril = repositorioInfEnf.getCantidadAtendidos(eYear, "Abril", comboBox2.Text);
                    cantAtendidosYearMayo = repositorioInfEnf.getCantidadAtendidos(eYear, "Mayo", comboBox2.Text);
                    cantAtendidosYearJunio = repositorioInfEnf.getCantidadAtendidos(eYear, "Junio", comboBox2.Text);
                    cantAtendidosYearJulio = repositorioInfEnf.getCantidadAtendidos(eYear, "Julio", comboBox2.Text);
                    cantAtendidosYearAgosto = repositorioInfEnf.getCantidadAtendidos(eYear, "Agosto", comboBox2.Text);
                    cantAtendidosYearSeptiembre = repositorioInfEnf.getCantidadAtendidos(eYear, "Septiembre", comboBox2.Text);
                    cantAtendidosYearOctubre = repositorioInfEnf.getCantidadAtendidos(eYear, "Octubre", comboBox2.Text);
                    cantAtendidosYearNoviembre = repositorioInfEnf.getCantidadAtendidos(eYear, "Noviembre", comboBox2.Text);
                    cantAtendidosYearDiciembre = repositorioInfEnf.getCantidadAtendidos(eYear, "Diciembre", comboBox2.Text);

                    if (comboBox1.Text == "Enero" || comboBox1.Text == "ENERO")
                    {
                        cantAtendidosYearFebrero = 0; cantAtendidosYearMarzo = 0; cantAtendidosYearAbril = 0;
                        cantAtendidosYearMayo = 0; cantAtendidosYearJunio = 0; cantAtendidosYearJulio = 0;
                        cantAtendidosYearAgosto = 0; cantAtendidosYearSeptiembre = 0; cantAtendidosYearOctubre = 0;
                        cantAtendidosYearNoviembre = 0; cantAtendidosYearDiciembre = 0;
                    }
                    else if (comboBox1.Text == "Febrero" || comboBox1.Text == "FEBRERO")
                    {
                        cantAtendidosYearMarzo = 0; cantAtendidosYearAbril = 0;
                        cantAtendidosYearMayo = 0; cantAtendidosYearJunio = 0; cantAtendidosYearJulio = 0;
                        cantAtendidosYearAgosto = 0; cantAtendidosYearSeptiembre = 0; cantAtendidosYearOctubre = 0;
                        cantAtendidosYearNoviembre = 0; cantAtendidosYearDiciembre = 0;
                    }
                    else if (comboBox1.Text == "Marzo" || comboBox1.Text == "MARZO")
                    {
                        cantAtendidosYearAbril = 0;
                        cantAtendidosYearMayo = 0; cantAtendidosYearJunio = 0; cantAtendidosYearJulio = 0;
                        cantAtendidosYearAgosto = 0; cantAtendidosYearSeptiembre = 0; cantAtendidosYearOctubre = 0;
                        cantAtendidosYearNoviembre = 0; cantAtendidosYearDiciembre = 0;
                    }
                    else if (comboBox1.Text == "Abril" || comboBox1.Text == "ABRIL")
                    {
                        cantAtendidosYearMayo = 0; cantAtendidosYearJunio = 0; cantAtendidosYearJulio = 0;
                        cantAtendidosYearAgosto = 0; cantAtendidosYearSeptiembre = 0; cantAtendidosYearOctubre = 0;
                        cantAtendidosYearNoviembre = 0; cantAtendidosYearDiciembre = 0;
                    }
                    else if (comboBox1.Text == "Mayo" || comboBox1.Text == "MAYO")
                    {
                        cantAtendidosYearJunio = 0; cantAtendidosYearJulio = 0;
                        cantAtendidosYearAgosto = 0; cantAtendidosYearSeptiembre = 0; cantAtendidosYearOctubre = 0;
                        cantAtendidosYearNoviembre = 0; cantAtendidosYearDiciembre = 0;
                    }
                    else if (comboBox1.Text == "Junio" || comboBox1.Text == "JUNIO")
                    {
                        cantAtendidosYearJulio = 0;
                        cantAtendidosYearAgosto = 0; cantAtendidosYearSeptiembre = 0; cantAtendidosYearOctubre = 0;
                        cantAtendidosYearNoviembre = 0; cantAtendidosYearDiciembre = 0;
                    }
                    else if (comboBox1.Text == "Julio" || comboBox1.Text == "JULIO")
                    {
                        cantAtendidosYearAgosto = 0; cantAtendidosYearSeptiembre = 0; cantAtendidosYearOctubre = 0;
                        cantAtendidosYearNoviembre = 0; cantAtendidosYearDiciembre = 0;
                    }
                    else if (comboBox1.Text == "Agosto" || comboBox1.Text == "AGOSTO")
                    {
                        cantAtendidosYearSeptiembre = 0; cantAtendidosYearOctubre = 0;
                        cantAtendidosYearNoviembre = 0; cantAtendidosYearDiciembre = 0;
                    }
                    else if (comboBox1.Text == "Septiembre" || comboBox1.Text == "SEPTIEMBRE")
                    {
                        cantAtendidosYearOctubre = 0;
                        cantAtendidosYearNoviembre = 0; cantAtendidosYearDiciembre = 0;
                    }
                    else if (comboBox1.Text == "Octubre" || comboBox1.Text == "OCTUBRE")
                    {
                        cantAtendidosYearNoviembre = 0; cantAtendidosYearDiciembre = 0;
                    }
                    else if (comboBox1.Text == "Noviembre" || comboBox1.Text == "NOVIEMBRE")
                    {
                        cantAtendidosYearDiciembre = 0;
                    }

                    SumatoriaYear = cantAtendidosYearEnero + cantAtendidosYearFebrero + cantAtendidosYearMarzo + cantAtendidosYearAbril +
                        cantAtendidosYearMayo + cantAtendidosYearJunio + cantAtendidosYearJulio + cantAtendidosYearAgosto + cantAtendidosYearSeptiembre +
                        cantAtendidosYearOctubre + cantAtendidosYearNoviembre + cantAtendidosYearDiciembre;

                    if (comboBox1.Text == "Enero" || comboBox1.Text == "ENERO")
                    {
                        cantAtendidosYearEneroPorcentual = ((double)((cantAtendidosYearEnero * 100)) / SumatoriaYear);
                        cantAtendidosYearFebreroPorcentual = 0;
                        cantAtendidosYearMarzoPorcentual = 0;
                        cantAtendidosYearAbrilPorcentual = 0;
                        cantAtendidosYearMayoPorcentual = 0;
                        cantAtendidosYearJunioPorcentual = 0;
                        cantAtendidosYearJulioPorcentual = 0;
                        cantAtendidosYearAgostoPorcentual = 0;
                        cantAtendidosYearSeptiembrePorcentual = 0;
                        cantAtendidosYearOctubrePorcentual = 0;
                        cantAtendidosYearNoviembrePorcentual = 0;
                        cantAtendidosYearDiciembrePorcentual = 0;
                    }
                    else if (comboBox1.Text == "Febrero" || comboBox1.Text == "FEBRERO")
                    {
                        cantAtendidosYearEneroPorcentual = ((double)((cantAtendidosYearEnero * 100)) / SumatoriaYear);
                        cantAtendidosYearFebreroPorcentual = ((double)((cantAtendidosYearFebrero * 100)) / SumatoriaYear);
                        cantAtendidosYearMarzoPorcentual = 0;
                        cantAtendidosYearAbrilPorcentual = 0;
                        cantAtendidosYearMayoPorcentual = 0;
                        cantAtendidosYearJunioPorcentual = 0;
                        cantAtendidosYearJulioPorcentual = 0;
                        cantAtendidosYearAgostoPorcentual = 0;
                        cantAtendidosYearSeptiembrePorcentual = 0;
                        cantAtendidosYearOctubrePorcentual = 0;
                        cantAtendidosYearNoviembrePorcentual = 0;
                        cantAtendidosYearDiciembrePorcentual = 0;
                    }
                    else if (comboBox1.Text == "Marzo" || comboBox1.Text == "MARZO")
                    {
                        cantAtendidosYearEneroPorcentual = ((double)((cantAtendidosYearEnero * 100)) / SumatoriaYear);
                        cantAtendidosYearFebreroPorcentual = ((double)((cantAtendidosYearFebrero * 100)) / SumatoriaYear);
                        cantAtendidosYearMarzoPorcentual = ((double)((cantAtendidosYearMarzo * 100)) / SumatoriaYear);
                        cantAtendidosYearAbrilPorcentual = 0;
                        cantAtendidosYearMayoPorcentual = 0;
                        cantAtendidosYearJunioPorcentual = 0;
                        cantAtendidosYearJulioPorcentual = 0;
                        cantAtendidosYearAgostoPorcentual = 0;
                        cantAtendidosYearSeptiembrePorcentual = 0;
                        cantAtendidosYearOctubrePorcentual = 0;
                        cantAtendidosYearNoviembrePorcentual = 0;
                        cantAtendidosYearDiciembrePorcentual = 0;
                    }
                    else if (comboBox1.Text == "Abril" || comboBox1.Text == "ABRIL")
                    {
                        cantAtendidosYearEneroPorcentual = ((double)((cantAtendidosYearEnero * 100)) / SumatoriaYear);
                        cantAtendidosYearFebreroPorcentual = ((double)((cantAtendidosYearFebrero * 100)) / SumatoriaYear);
                        cantAtendidosYearMarzoPorcentual = ((double)((cantAtendidosYearMarzo * 100)) / SumatoriaYear);
                        cantAtendidosYearAbrilPorcentual = ((double)((cantAtendidosYearAbril * 100)) / SumatoriaYear);
                        cantAtendidosYearMayoPorcentual = 0;
                        cantAtendidosYearJunioPorcentual = 0;
                        cantAtendidosYearJulioPorcentual = 0;
                        cantAtendidosYearAgostoPorcentual = 0;
                        cantAtendidosYearSeptiembrePorcentual = 0;
                        cantAtendidosYearOctubrePorcentual = 0;
                        cantAtendidosYearNoviembrePorcentual = 0;
                        cantAtendidosYearDiciembrePorcentual = 0;
                    }
                    else if (comboBox1.Text == "Mayo" || comboBox1.Text == "MAYO")
                    {
                        cantAtendidosYearEneroPorcentual = ((double)((cantAtendidosYearEnero * 100)) / SumatoriaYear);
                        cantAtendidosYearFebreroPorcentual = ((double)((cantAtendidosYearFebrero * 100)) / SumatoriaYear);
                        cantAtendidosYearMarzoPorcentual = ((double)((cantAtendidosYearMarzo * 100)) / SumatoriaYear);
                        cantAtendidosYearAbrilPorcentual = ((double)((cantAtendidosYearAbril * 100)) / SumatoriaYear);
                        cantAtendidosYearMayoPorcentual = ((double)((cantAtendidosYearMayo * 100)) / SumatoriaYear);
                        cantAtendidosYearJunioPorcentual = 0;
                        cantAtendidosYearJulioPorcentual = 0;
                        cantAtendidosYearAgostoPorcentual = 0;
                        cantAtendidosYearSeptiembrePorcentual = 0;
                        cantAtendidosYearOctubrePorcentual = 0;
                        cantAtendidosYearNoviembrePorcentual = 0;
                        cantAtendidosYearDiciembrePorcentual = 0;
                    }
                    else if (comboBox1.Text == "Junio" || comboBox1.Text == "JUNIO")
                    {
                        cantAtendidosYearEneroPorcentual = ((double)((cantAtendidosYearEnero * 100)) / SumatoriaYear);
                        cantAtendidosYearFebreroPorcentual = ((double)((cantAtendidosYearFebrero * 100)) / SumatoriaYear);
                        cantAtendidosYearMarzoPorcentual = ((double)((cantAtendidosYearMarzo * 100)) / SumatoriaYear);
                        cantAtendidosYearAbrilPorcentual = ((double)((cantAtendidosYearAbril * 100)) / SumatoriaYear);
                        cantAtendidosYearMayoPorcentual = ((double)((cantAtendidosYearMayo * 100)) / SumatoriaYear);
                        cantAtendidosYearJunioPorcentual = ((double)((cantAtendidosYearJunio * 100)) / SumatoriaYear);
                        cantAtendidosYearJulioPorcentual = 0;
                        cantAtendidosYearAgostoPorcentual = 0;
                        cantAtendidosYearSeptiembrePorcentual = 0;
                        cantAtendidosYearOctubrePorcentual = 0;
                        cantAtendidosYearNoviembrePorcentual = 0;
                        cantAtendidosYearDiciembrePorcentual = 0;
                    }
                    else if (comboBox1.Text == "Julio" || comboBox1.Text == "JULIO")
                    {
                        cantAtendidosYearEneroPorcentual = ((double)((cantAtendidosYearEnero * 100)) / SumatoriaYear);
                        cantAtendidosYearFebreroPorcentual = ((double)((cantAtendidosYearFebrero * 100)) / SumatoriaYear);
                        cantAtendidosYearMarzoPorcentual = ((double)((cantAtendidosYearMarzo * 100)) / SumatoriaYear);
                        cantAtendidosYearAbrilPorcentual = ((double)((cantAtendidosYearAbril * 100)) / SumatoriaYear);
                        cantAtendidosYearMayoPorcentual = ((double)((cantAtendidosYearMayo * 100)) / SumatoriaYear);
                        cantAtendidosYearJunioPorcentual = ((double)((cantAtendidosYearJunio * 100)) / SumatoriaYear);
                        cantAtendidosYearJulioPorcentual = ((double)((cantAtendidosYearJulio * 100)) / SumatoriaYear);
                        cantAtendidosYearAgostoPorcentual = 0;
                        cantAtendidosYearSeptiembrePorcentual = 0;
                        cantAtendidosYearOctubrePorcentual = 0;
                        cantAtendidosYearNoviembrePorcentual = 0;
                        cantAtendidosYearDiciembrePorcentual = 0;
                    }
                    else if (comboBox1.Text == "Agosto" || comboBox1.Text == "AGOSTO")
                    {
                        cantAtendidosYearEneroPorcentual = ((double)((cantAtendidosYearEnero * 100)) / SumatoriaYear);
                        cantAtendidosYearFebreroPorcentual = ((double)((cantAtendidosYearFebrero * 100)) / SumatoriaYear);
                        cantAtendidosYearMarzoPorcentual = ((double)((cantAtendidosYearMarzo * 100)) / SumatoriaYear);
                        cantAtendidosYearAbrilPorcentual = ((double)((cantAtendidosYearAbril * 100)) / SumatoriaYear);
                        cantAtendidosYearMayoPorcentual = ((double)((cantAtendidosYearMayo * 100)) / SumatoriaYear);
                        cantAtendidosYearJunioPorcentual = ((double)((cantAtendidosYearJunio * 100)) / SumatoriaYear);
                        cantAtendidosYearJulioPorcentual = ((double)((cantAtendidosYearJulio * 100)) / SumatoriaYear);
                        cantAtendidosYearAgostoPorcentual = ((double)((cantAtendidosYearAgosto * 100)) / SumatoriaYear);
                        cantAtendidosYearSeptiembrePorcentual = 0;
                        cantAtendidosYearOctubrePorcentual = 0;
                        cantAtendidosYearNoviembrePorcentual = 0;
                        cantAtendidosYearDiciembrePorcentual = 0;
                    }
                    else if (comboBox1.Text == "Septiembre" || comboBox1.Text == "SEPTIEMBRE")
                    {
                        cantAtendidosYearEneroPorcentual = ((double)((cantAtendidosYearEnero * 100)) / SumatoriaYear);
                        cantAtendidosYearFebreroPorcentual = ((double)((cantAtendidosYearFebrero * 100)) / SumatoriaYear);
                        cantAtendidosYearMarzoPorcentual = ((double)((cantAtendidosYearMarzo * 100)) / SumatoriaYear);
                        cantAtendidosYearAbrilPorcentual = ((double)((cantAtendidosYearAbril * 100)) / SumatoriaYear);
                        cantAtendidosYearMayoPorcentual = ((double)((cantAtendidosYearMayo * 100)) / SumatoriaYear);
                        cantAtendidosYearJunioPorcentual = ((double)((cantAtendidosYearJunio * 100)) / SumatoriaYear);
                        cantAtendidosYearJulioPorcentual = ((double)((cantAtendidosYearJulio * 100)) / SumatoriaYear);
                        cantAtendidosYearAgostoPorcentual = ((double)((cantAtendidosYearAgosto * 100)) / SumatoriaYear);
                        cantAtendidosYearSeptiembrePorcentual = ((double)((cantAtendidosYearSeptiembre * 100)) / SumatoriaYear);
                        cantAtendidosYearOctubrePorcentual = 0;
                        cantAtendidosYearNoviembrePorcentual = 0;
                        cantAtendidosYearDiciembrePorcentual = 0;
                    }
                    else if (comboBox1.Text == "Octubre" || comboBox1.Text == "OCTUBRE")
                    {
                        cantAtendidosYearEneroPorcentual = ((double)((cantAtendidosYearEnero * 100)) / SumatoriaYear);
                        cantAtendidosYearFebreroPorcentual = ((double)((cantAtendidosYearFebrero * 100)) / SumatoriaYear);
                        cantAtendidosYearMarzoPorcentual = ((double)((cantAtendidosYearMarzo * 100)) / SumatoriaYear);
                        cantAtendidosYearAbrilPorcentual = ((double)((cantAtendidosYearAbril * 100)) / SumatoriaYear);
                        cantAtendidosYearMayoPorcentual = ((double)((cantAtendidosYearMayo * 100)) / SumatoriaYear);
                        cantAtendidosYearJunioPorcentual = ((double)((cantAtendidosYearJunio * 100)) / SumatoriaYear);
                        cantAtendidosYearJulioPorcentual = ((double)((cantAtendidosYearJulio * 100)) / SumatoriaYear);
                        cantAtendidosYearAgostoPorcentual = ((double)((cantAtendidosYearAgosto * 100)) / SumatoriaYear);
                        cantAtendidosYearSeptiembrePorcentual = ((double)((cantAtendidosYearSeptiembre * 100)) / SumatoriaYear);
                        cantAtendidosYearOctubrePorcentual = ((double)((cantAtendidosYearOctubre * 100)) / SumatoriaYear);
                        cantAtendidosYearNoviembrePorcentual = 0;
                        cantAtendidosYearDiciembrePorcentual = 0;
                    }
                    else if (comboBox1.Text == "Noviembre" || comboBox1.Text == "NOVIEMBRE")
                    {
                        cantAtendidosYearEneroPorcentual = ((double)((cantAtendidosYearEnero * 100)) / SumatoriaYear);
                        cantAtendidosYearFebreroPorcentual = ((double)((cantAtendidosYearFebrero * 100)) / SumatoriaYear);
                        cantAtendidosYearMarzoPorcentual = ((double)((cantAtendidosYearMarzo * 100)) / SumatoriaYear);
                        cantAtendidosYearAbrilPorcentual = ((double)((cantAtendidosYearAbril * 100)) / SumatoriaYear);
                        cantAtendidosYearMayoPorcentual = ((double)((cantAtendidosYearMayo * 100)) / SumatoriaYear);
                        cantAtendidosYearJunioPorcentual = ((double)((cantAtendidosYearJunio * 100)) / SumatoriaYear);
                        cantAtendidosYearJulioPorcentual = ((double)((cantAtendidosYearJulio * 100)) / SumatoriaYear);
                        cantAtendidosYearAgostoPorcentual = ((double)((cantAtendidosYearAgosto * 100)) / SumatoriaYear);
                        cantAtendidosYearSeptiembrePorcentual = ((double)((cantAtendidosYearSeptiembre * 100)) / SumatoriaYear);
                        cantAtendidosYearOctubrePorcentual = ((double)((cantAtendidosYearOctubre * 100)) / SumatoriaYear);
                        cantAtendidosYearNoviembrePorcentual = ((double)((cantAtendidosYearNoviembre * 100)) / SumatoriaYear);
                        cantAtendidosYearDiciembrePorcentual = 0;
                    }
                    else if (comboBox1.Text == "Diciembre" || comboBox1.Text == "DICIEMBRE")
                    {
                        cantAtendidosYearEneroPorcentual = ((double)((cantAtendidosYearEnero * 100)) / SumatoriaYear);
                        cantAtendidosYearFebreroPorcentual = ((double)((cantAtendidosYearFebrero * 100)) / SumatoriaYear);
                        cantAtendidosYearMarzoPorcentual = ((double)((cantAtendidosYearMarzo * 100)) / SumatoriaYear);
                        cantAtendidosYearAbrilPorcentual = ((double)((cantAtendidosYearAbril * 100)) / SumatoriaYear);
                        cantAtendidosYearMayoPorcentual = ((double)((cantAtendidosYearMayo * 100)) / SumatoriaYear);
                        cantAtendidosYearJunioPorcentual = ((double)((cantAtendidosYearJunio * 100)) / SumatoriaYear);
                        cantAtendidosYearJulioPorcentual = ((double)((cantAtendidosYearJulio * 100)) / SumatoriaYear);
                        cantAtendidosYearAgostoPorcentual = ((double)((cantAtendidosYearAgosto * 100)) / SumatoriaYear);
                        cantAtendidosYearSeptiembrePorcentual = ((double)((cantAtendidosYearSeptiembre * 100)) / SumatoriaYear);
                        cantAtendidosYearOctubrePorcentual = ((double)((cantAtendidosYearOctubre * 100)) / SumatoriaYear);
                        cantAtendidosYearNoviembrePorcentual = ((double)((cantAtendidosYearNoviembre * 100)) / SumatoriaYear);
                        cantAtendidosYearDiciembrePorcentual = ((double)((cantAtendidosYearDiciembre * 100)) / SumatoriaYear);
                    }

                    double TOTALPATOLOGIASUVHORIZONTAL = ((double)((TotalEneroPatologiaUVVertical + TotalFebreroPatologiaUVVertical + TotalMarzoPatologiaUVVertical +
                        TotalAbrilPatologiaUVVertical + TotalMayoPatologiaUVVertical + TotalJunioPatologiaUVVertical + TotalJulioPatologiaUVVertical +
                        TotalAgostoPatologiaUVVertical + TotalSeptiembrePatologiaUVVertical + TotalOctubrePatologiaUVVertical +
                        TotalNoviembrePatologiaUVVertical + TotalDiciembrePatologiaUVVertical))) / 12;

                    double TOTALPATOLOGIASPDHORIZONTAL = ((double)((TotalEneroPatologiaPDVertical + TotalFebreroPatologiaPDVertical + TotalMarzoPatologiaPDVertical +
                        TotalAbrilPatologiaPDVertical + TotalMayoPatologiaPDVertical + TotalJunioPatologiaPDVertical + TotalJulioPatologiaPDVertical +
                        TotalAgostoPatologiaPDVertical + TotalSeptiembrePatologiaPDVertical + TotalOctubrePatologiaPDVertical +
                        TotalNoviembrePatologiaPDVertical + TotalDiciembrePatologiaPDVertical))) / 12;

                    double TOTALPATOLOGIASQXHORIZONTAL = ((double)((TotalEneroPatologiaQXVertical + TotalFebreroPatologiaQXVertical + TotalMarzoPatologiaQXVertical +
                        TotalAbrilPatologiaQXVertical + TotalMayoPatologiaQXVertical + TotalJunioPatologiaQXVertical + TotalJulioPatologiaQXVertical +
                        TotalAgostoPatologiaQXVertical + TotalSeptiembrePatologiaQXVertical + TotalOctubrePatologiaQXVertical +
                        TotalNoviembrePatologiaQXVertical + TotalDiciembrePatologiaQXVertical))) / 12;

                    double TOTALPATOLOGIASTXHORIZONTAL = ((double)((TotalEneroPatologiaTXVertical + TotalFebreroPatologiaTXVertical + TotalMarzoPatologiaTXVertical +
                        TotalAbrilPatologiaTXVertical + TotalMayoPatologiaTXVertical + TotalJunioPatologiaTXVertical + TotalJulioPatologiaTXVertical +
                        TotalAgostoPatologiaTXVertical + TotalSeptiembrePatologiaTXVertical + TotalOctubrePatologiaTXVertical +
                        TotalNoviembrePatologiaTXVertical + TotalDiciembrePatologiaTXVertical))) / 12;

                    double TOTALPATOLOGIASOTROSHORIZONTAL = ((double)((TotalEneroPatologiaOtrosVertical + TotalFebreroPatologiaOtrosVertical + TotalMarzoPatologiaOtrosVertical +
                        TotalAbrilPatologiaOtrosVertical + TotalMayoPatologiaOtrosVertical + TotalJunioPatologiaOtrosVertical + TotalJulioPatologiaOtrosVertical +
                        TotalAgostoPatologiaOtrosVertical + TotalSeptiembrePatologiaOtrosVertical + TotalOctubrePatologiaOtrosVertical +
                        TotalNoviembrePatologiaOtrosVertical + TotalDiciembrePatologiaOtrosVertical))) / 12;

                    double TOTALPATOLOGIASZPHORIZONTAL = ((double)((TotalEneroPatologiaZPVertical + TotalFebreroPatologiaZPVertical + TotalMarzoPatologiaZPVertical +
                        TotalAbrilPatologiaZPVertical + TotalMayoPatologiaZPVertical + TotalJunioPatologiaZPVertical + TotalJulioPatologiaZPVertical +
                        TotalAgostoPatologiaZPVertical + TotalSeptiembrePatologiaZPVertical + TotalOctubrePatologiaZPVertical +
                        TotalNoviembrePatologiaZPVertical + TotalDiciembrePatologiaZPVertical))) / 12;

                    double TOTALPATOLOGIASPOPHORIZONTAL = ((double)((TotalEneroPatologiaPOPVertical + TotalFebreroPatologiaPOPVertical + TotalMarzoPatologiaPOPVertical +
                        TotalAbrilPatologiaPOPVertical + TotalMayoPatologiaPOPVertical + TotalJunioPatologiaPOPVertical + TotalJulioPatologiaPOPVertical +
                        TotalAgostoPatologiaPOPVertical + TotalSeptiembrePatologiaPOPVertical + TotalOctubrePatologiaPOPVertical +
                        TotalNoviembrePatologiaPOPVertical + TotalDiciembrePatologiaPOPVertical))) / 12;

                    //REVISAR ESTE ES EL PORCENTUAL TOTAL PROMEDIO DE ENFERMEROS
                    cantAtendidosYearPromedio = (cantAtendidosYearEneroPorcentual + cantAtendidosYearFebreroPorcentual + cantAtendidosYearMarzoPorcentual +
                        cantAtendidosYearAbrilPorcentual + cantAtendidosYearMayoPorcentual + cantAtendidosYearJunioPorcentual + cantAtendidosYearJulioPorcentual +
                        cantAtendidosYearAgostoPorcentual + cantAtendidosYearSeptiembrePorcentual + cantAtendidosYearOctubrePorcentual +
                        cantAtendidosYearNoviembrePorcentual + cantAtendidosYearDiciembrePorcentual) / 12;

                    cantAtendidos = repositorioInfEnf.getCantidadAtendidos(eYear, comboBox1.Text, comboBox2.Text);
                    nameEnf = repositorioBodegas.getDatosCode(eYear).Bod_Responsable;

                    if (comboBox1.Text == "Enero" || comboBox1.Text == "ENERO")
                    {
                        CantPacEneroCU = repositorioInfEnf.getCantPacsForMonth("Enero", comboBox2.Text, "CU");
                    }
                    if (comboBox1.Text == "Febrero" || comboBox1.Text == "FEBRERO")
                    {
                        CantPacEneroCU = repositorioInfEnf.getCantPacsForMonth("Enero", comboBox2.Text, "CU");
                        CantPacFebreroCU = repositorioInfEnf.getCantPacsForMonth("Febrero", comboBox2.Text, "CU");
                    }
                    if (comboBox1.Text == "Marzo" || comboBox1.Text == "MARZO")
                    {
                        CantPacEneroCU = repositorioInfEnf.getCantPacsForMonth("Enero", comboBox2.Text, "CU");
                        CantPacFebreroCU = repositorioInfEnf.getCantPacsForMonth("Febrero", comboBox2.Text, "CU");
                        CantPacMarzoCU = repositorioInfEnf.getCantPacsForMonth("Marzo", comboBox2.Text, "CU");
                    }
                    if (comboBox1.Text == "Abril" || comboBox1.Text == "ABRIL")
                    {
                        CantPacEneroCU = repositorioInfEnf.getCantPacsForMonth("Enero", comboBox2.Text, "CU");
                        CantPacFebreroCU = repositorioInfEnf.getCantPacsForMonth("Febrero", comboBox2.Text, "CU");
                        CantPacMarzoCU = repositorioInfEnf.getCantPacsForMonth("Marzo", comboBox2.Text, "CU");
                        CantPacAbrilCU = repositorioInfEnf.getCantPacsForMonth("Abril", comboBox2.Text, "CU");
                    }
                    if (comboBox1.Text == "Mayo" || comboBox1.Text == "MAYO")
                    {
                        CantPacEneroCU = repositorioInfEnf.getCantPacsForMonth("Enero", comboBox2.Text, "CU");
                        CantPacFebreroCU = repositorioInfEnf.getCantPacsForMonth("Febrero", comboBox2.Text, "CU");
                        CantPacMarzoCU = repositorioInfEnf.getCantPacsForMonth("Marzo", comboBox2.Text, "CU");
                        CantPacAbrilCU = repositorioInfEnf.getCantPacsForMonth("Abril", comboBox2.Text, "CU");
                        CantPacMayoCU = repositorioInfEnf.getCantPacsForMonth("Mayo", comboBox2.Text, "CU");
                    }
                    if (comboBox1.Text == "Junio" || comboBox1.Text == "JUNIO")
                    {
                        CantPacEneroCU = repositorioInfEnf.getCantPacsForMonth("Enero", comboBox2.Text, "CU");
                        CantPacFebreroCU = repositorioInfEnf.getCantPacsForMonth("Febrero", comboBox2.Text, "CU");
                        CantPacMarzoCU = repositorioInfEnf.getCantPacsForMonth("Marzo", comboBox2.Text, "CU");
                        CantPacAbrilCU = repositorioInfEnf.getCantPacsForMonth("Abril", comboBox2.Text, "CU");
                        CantPacMayoCU = repositorioInfEnf.getCantPacsForMonth("Mayo", comboBox2.Text, "CU");
                        CantPacJunioCU = repositorioInfEnf.getCantPacsForMonth("Junio", comboBox2.Text, "CU");
                    }
                    if (comboBox1.Text == "Julio" || comboBox1.Text == "JULIO")
                    {
                        CantPacEneroCU = repositorioInfEnf.getCantPacsForMonth("Enero", comboBox2.Text, "CU");
                        CantPacFebreroCU = repositorioInfEnf.getCantPacsForMonth("Febrero", comboBox2.Text, "CU");
                        CantPacMarzoCU = repositorioInfEnf.getCantPacsForMonth("Marzo", comboBox2.Text, "CU");
                        CantPacAbrilCU = repositorioInfEnf.getCantPacsForMonth("Abril", comboBox2.Text, "CU");
                        CantPacMayoCU = repositorioInfEnf.getCantPacsForMonth("Mayo", comboBox2.Text, "CU");
                        CantPacJunioCU = repositorioInfEnf.getCantPacsForMonth("Junio", comboBox2.Text, "CU");
                        CantPacJulioCU = repositorioInfEnf.getCantPacsForMonth("Julio", comboBox2.Text, "CU");
                    }
                    if (comboBox1.Text == "Agosto" || comboBox1.Text == "AGOSTO")
                    {
                        CantPacEneroCU = repositorioInfEnf.getCantPacsForMonth("Enero", comboBox2.Text, "CU");
                        CantPacFebreroCU = repositorioInfEnf.getCantPacsForMonth("Febrero", comboBox2.Text, "CU");
                        CantPacMarzoCU = repositorioInfEnf.getCantPacsForMonth("Marzo", comboBox2.Text, "CU");
                        CantPacAbrilCU = repositorioInfEnf.getCantPacsForMonth("Abril", comboBox2.Text, "CU");
                        CantPacMayoCU = repositorioInfEnf.getCantPacsForMonth("Mayo", comboBox2.Text, "CU");
                        CantPacJunioCU = repositorioInfEnf.getCantPacsForMonth("Junio", comboBox2.Text, "CU");
                        CantPacJulioCU = repositorioInfEnf.getCantPacsForMonth("Julio", comboBox2.Text, "CU");
                        CantPacAgostoCU = repositorioInfEnf.getCantPacsForMonth("Agosto", comboBox2.Text, "CU");
                    }
                    if (comboBox1.Text == "Septiembre" || comboBox1.Text == "SEPTIEMBRE")
                    {
                        CantPacEneroCU = repositorioInfEnf.getCantPacsForMonth("Enero", comboBox2.Text, "CU");
                        CantPacFebreroCU = repositorioInfEnf.getCantPacsForMonth("Febrero", comboBox2.Text, "CU");
                        CantPacMarzoCU = repositorioInfEnf.getCantPacsForMonth("Marzo", comboBox2.Text, "CU");
                        CantPacAbrilCU = repositorioInfEnf.getCantPacsForMonth("Abril", comboBox2.Text, "CU");
                        CantPacMayoCU = repositorioInfEnf.getCantPacsForMonth("Mayo", comboBox2.Text, "CU");
                        CantPacJunioCU = repositorioInfEnf.getCantPacsForMonth("Junio", comboBox2.Text, "CU");
                        CantPacJulioCU = repositorioInfEnf.getCantPacsForMonth("Julio", comboBox2.Text, "CU");
                        CantPacAgostoCU = repositorioInfEnf.getCantPacsForMonth("Agosto", comboBox2.Text, "CU");
                        CantPacSeptiembreCU = repositorioInfEnf.getCantPacsForMonth("Septiembre", comboBox2.Text, "CU");
                    }
                    if (comboBox1.Text == "Octubre" || comboBox1.Text == "OCTUBRE")
                    {
                        CantPacEneroCU = repositorioInfEnf.getCantPacsForMonth("Enero", comboBox2.Text, "CU");
                        CantPacFebreroCU = repositorioInfEnf.getCantPacsForMonth("Febrero", comboBox2.Text, "CU");
                        CantPacMarzoCU = repositorioInfEnf.getCantPacsForMonth("Marzo", comboBox2.Text, "CU");
                        CantPacAbrilCU = repositorioInfEnf.getCantPacsForMonth("Abril", comboBox2.Text, "CU");
                        CantPacMayoCU = repositorioInfEnf.getCantPacsForMonth("Mayo", comboBox2.Text, "CU");
                        CantPacJunioCU = repositorioInfEnf.getCantPacsForMonth("Junio", comboBox2.Text, "CU");
                        CantPacJulioCU = repositorioInfEnf.getCantPacsForMonth("Julio", comboBox2.Text, "CU");
                        CantPacAgostoCU = repositorioInfEnf.getCantPacsForMonth("Agosto", comboBox2.Text, "CU");
                        CantPacSeptiembreCU = repositorioInfEnf.getCantPacsForMonth("Septiembre", comboBox2.Text, "CU");
                        CantPacOctubreCU = repositorioInfEnf.getCantPacsForMonth("Octubre", comboBox2.Text, "CU");
                    }
                    if (comboBox1.Text == "Noviembre" || comboBox1.Text == "NOVIEMBRE")
                    {
                        CantPacEneroCU = repositorioInfEnf.getCantPacsForMonth("Enero", comboBox2.Text, "CU");
                        CantPacFebreroCU = repositorioInfEnf.getCantPacsForMonth("Febrero", comboBox2.Text, "CU");
                        CantPacMarzoCU = repositorioInfEnf.getCantPacsForMonth("Marzo", comboBox2.Text, "CU");
                        CantPacAbrilCU = repositorioInfEnf.getCantPacsForMonth("Abril", comboBox2.Text, "CU");
                        CantPacMayoCU = repositorioInfEnf.getCantPacsForMonth("Mayo", comboBox2.Text, "CU");
                        CantPacJunioCU = repositorioInfEnf.getCantPacsForMonth("Junio", comboBox2.Text, "CU");
                        CantPacJulioCU = repositorioInfEnf.getCantPacsForMonth("Julio", comboBox2.Text, "CU");
                        CantPacAgostoCU = repositorioInfEnf.getCantPacsForMonth("Agosto", comboBox2.Text, "CU");
                        CantPacSeptiembreCU = repositorioInfEnf.getCantPacsForMonth("Septiembre", comboBox2.Text, "CU");
                        CantPacOctubreCU = repositorioInfEnf.getCantPacsForMonth("Octubre", comboBox2.Text, "CU");
                        CantPacNoviembreCU = repositorioInfEnf.getCantPacsForMonth("Noviembre", comboBox2.Text, "CU");
                    }
                    if (comboBox1.Text == "Diciembre" || comboBox1.Text == "DICIEMBRE")
                    {
                        CantPacEneroCU = repositorioInfEnf.getCantPacsForMonth("Enero", comboBox2.Text, "CU");
                        CantPacFebreroCU = repositorioInfEnf.getCantPacsForMonth("Febrero", comboBox2.Text, "CU");
                        CantPacMarzoCU = repositorioInfEnf.getCantPacsForMonth("Marzo", comboBox2.Text, "CU");
                        CantPacAbrilCU = repositorioInfEnf.getCantPacsForMonth("Abril", comboBox2.Text, "CU");
                        CantPacMayoCU = repositorioInfEnf.getCantPacsForMonth("Mayo", comboBox2.Text, "CU");
                        CantPacJunioCU = repositorioInfEnf.getCantPacsForMonth("Junio", comboBox2.Text, "CU");
                        CantPacJulioCU = repositorioInfEnf.getCantPacsForMonth("Julio", comboBox2.Text, "CU");
                        CantPacAgostoCU = repositorioInfEnf.getCantPacsForMonth("Agosto", comboBox2.Text, "CU");
                        CantPacSeptiembreCU = repositorioInfEnf.getCantPacsForMonth("Septiembre", comboBox2.Text, "CU");
                        CantPacOctubreCU = repositorioInfEnf.getCantPacsForMonth("Octubre", comboBox2.Text, "CU");
                        CantPacNoviembreCU = repositorioInfEnf.getCantPacsForMonth("Noviembre", comboBox2.Text, "CU");
                        CantPacDiciembreCU = repositorioInfEnf.getCantPacsForMonth("Diciembre", comboBox2.Text, "CU");
                    }
                   
                    int SumatoriaPacsYear = repositorioInfEnf.getCantPacsForYear(comboBox1.Text, comboBox2.Text, "CU");
                    double PromedioCurByPac = SumatoriaYearGeneral / SumatoriaPacsYear;
                    int salida = 0;

                    Ie.Add(new Domain.Informes.InformeEnfermeria
                    {
                        Enfermero = nameEnf.ToString(),
                        CantidadEnfermero = cantAtendidos,
                        SumatoriaCantAtendidos = SumatoriaCantAtendidos,
                        PorcentajeEnfermeroAtendidos = ((double)((cantAtendidos * 100)) / SumatoriaCantAtendidos),
                        CantPatologiaVasculares = CantPatologiaVasculares,
                        CantPatologiaPosOperatorio = CantPatologiaPosOperatorio,
                        CantPatologiaZonadePresion = CantPatologiaZonadePresion,
                        CantPatologiaTrauma = CantPatologiaTrauma,
                        CantPatologiaQuemado = CantPatologiaQuemado,
                        CantPatologiaOtros = CantPatologiaOtros,
                        CantPatologiaPieDiabetico = CantPatologiaPieDiabetico,
                        Cant4160 = Cant4160,
                        Cant6180 = Cant6180,
                        Cant2140 = Cant2140,
                        Cant80 = Cant80,
                        Cant020 = Cant020,
                        cantAtendidosYearEnero = cantAtendidosYearEnero,
                        cantAtendidosYearFebrero = cantAtendidosYearFebrero,
                        cantAtendidosYearMarzo = cantAtendidosYearMarzo,
                        cantAtendidosYearAbril = cantAtendidosYearAbril,
                        cantAtendidosYearMayo = cantAtendidosYearMayo,
                        cantAtendidosYearJunio = cantAtendidosYearJunio,
                        cantAtendidosYearJulio = cantAtendidosYearJulio,
                        cantAtendidosYearAgosto = cantAtendidosYearAgosto,
                        cantAtendidosYearSeptiembre = cantAtendidosYearSeptiembre,
                        cantAtendidosYearOctubre = cantAtendidosYearOctubre,
                        cantAtendidosYearNoviembre = cantAtendidosYearNoviembre,
                        cantAtendidosYearDiciembre = cantAtendidosYearDiciembre,
                        SumatoriaYear = SumatoriaYear,
                        cantAtendidosYearEneroGeneral = cantAtendidosYearEneroGeneral,
                        cantAtendidosYearFebreroGeneral = cantAtendidosYearFebreroGeneral,
                        cantAtendidosYearMarzoGeneral = cantAtendidosYearMarzoGeneral,
                        cantAtendidosYearAbrilGeneral = cantAtendidosYearAbrilGeneral,
                        cantAtendidosYearMayoGeneral = cantAtendidosYearMayoGeneral,
                        cantAtendidosYearJunioGeneral = cantAtendidosYearJunioGeneral,
                        cantAtendidosYearJulioGeneral = cantAtendidosYearJulioGeneral,
                        cantAtendidosYearAgostoGeneral = cantAtendidosYearAgostoGeneral,
                        cantAtendidosYearSeptiembreGeneral = cantAtendidosYearSeptiembreGeneral,
                        cantAtendidosYearOctubreGeneral = cantAtendidosYearOctubreGeneral,
                        cantAtendidosYearNoviembreGeneral = cantAtendidosYearNoviembreGeneral,
                        cantAtendidosYearDiciembreGeneral = cantAtendidosYearDiciembreGeneral,
                        SumatoriaYearGeneral = SumatoriaYearGeneral,
                        cantAtendidosYearEneroPorcentual = cantAtendidosYearEneroPorcentual,
                        cantAtendidosYearFebreroPorcentual = cantAtendidosYearFebreroPorcentual,
                        cantAtendidosYearMarzoPorcentual = cantAtendidosYearMarzoPorcentual,
                        cantAtendidosYearAbrilPorcentual = cantAtendidosYearAbrilPorcentual,
                        cantAtendidosYearMayoPorcentual = cantAtendidosYearMayoPorcentual,
                        cantAtendidosYearJunioPorcentual = cantAtendidosYearJunioPorcentual,
                        cantAtendidosYearJulioPorcentual = cantAtendidosYearJulioPorcentual,
                        cantAtendidosYearAgostoPorcentual = cantAtendidosYearAgostoPorcentual,
                        cantAtendidosYearSeptiembrePorcentual = cantAtendidosYearSeptiembrePorcentual,
                        cantAtendidosYearOctubrePorcentual = cantAtendidosYearOctubrePorcentual,
                        cantAtendidosYearNoviembrePorcentual = cantAtendidosYearNoviembrePorcentual,
                        cantAtendidosYearDiciembrePorcentual = cantAtendidosYearDiciembrePorcentual,
                        MesGenera = comboBox1.Text.ToUpper() + " DE " + comboBox2.Text,
                        Logo = repoGen.GetBytes(pic.Image),
                        cantAtendidosYearPromedio = cantAtendidosYearPromedio,
                        UserGenera = Contenedor.UsuarioLogueado.ToUpper(),
                        Grafico1 = getGraph1,
                        CantPacEneroCU = CantPacEneroCU,
                        CantPacFebreroCU = CantPacFebreroCU,
                        CantPacMarzoCU = CantPacMarzoCU,
                        CantPacAbrilCU = CantPacAbrilCU,
                        CantPacMayoCU = CantPacMayoCU,
                        CantPacJunioCU = CantPacJunioCU,
                        CantPacJulioCU = CantPacJulioCU,
                        CantPacAgostoCU = CantPacAgostoCU,
                        CantPacSeptiembreCU = CantPacSeptiembreCU,
                        CantPacOctubreCU = CantPacOctubreCU,
                        CantPacNoviembreCU = CantPacNoviembreCU,
                        CantPacDiciembreCU = CantPacDiciembreCU,
                        SumatoriaPacsYear = SumatoriaPacsYear,
                        PromedioCurByPac = PromedioCurByPac,
                        Grafico2 = getGraph2,
                        Grafico3 = getGraph3,
                        Grafico4 = Grafico4(),

                        CantPatologiaEneroPOP = (getPatologiasPosOperatorioAño.TryGetValue("Enero", out salida) == true ? getPatologiasPosOperatorioAño["Enero"] : 0),
                        CantPatologiaFebreroPOP = (getPatologiasPosOperatorioAño.TryGetValue("Febrero", out salida) == true ? getPatologiasPosOperatorioAño["Febrero"] : 0),
                        CantPatologiaMarzoPOP = (getPatologiasPosOperatorioAño.TryGetValue("Marzo", out salida) == true ? getPatologiasPosOperatorioAño["Marzo"] : 0),
                        CantPatologiaAbrilPOP = (getPatologiasPosOperatorioAño.TryGetValue("Abril", out salida) == true ? getPatologiasPosOperatorioAño["Abril"] : 0),
                        CantPatologiaMayoPOP = (getPatologiasPosOperatorioAño.TryGetValue("Mayo", out salida) == true ? getPatologiasPosOperatorioAño["Mayo"] : 0),
                        CantPatologiaJunioPOP = (getPatologiasPosOperatorioAño.TryGetValue("Junio", out salida) == true ? getPatologiasPosOperatorioAño["Junio"] : 0),
                        CantPatologiaJulioPOP = (getPatologiasPosOperatorioAño.TryGetValue("Julio", out salida) == true ? getPatologiasPosOperatorioAño["Julio"] : 0),
                        CantPatologiaAgostoPOP = (getPatologiasPosOperatorioAño.TryGetValue("Agosto", out salida) == true ? getPatologiasPosOperatorioAño["Agosto"] : 0),
                        CantPatologiaSeptiembrePOP = (getPatologiasPosOperatorioAño.TryGetValue("Septiembre", out salida) == true ? getPatologiasPosOperatorioAño["Septiembre"] : 0),
                        CantPatologiaOctubrePOP = (getPatologiasPosOperatorioAño.TryGetValue("Octubre", out salida) == true ? getPatologiasPosOperatorioAño["Octubre"] : 0),
                        CantPatologiaNoviembrePOP = (getPatologiasPosOperatorioAño.TryGetValue("Noviembre", out salida) == true ? getPatologiasPosOperatorioAño["Noviembre"] : 0),
                        CantPatologiaDiciembrePOP = (getPatologiasPosOperatorioAño.TryGetValue("Diciembre", out salida) == true ? getPatologiasPosOperatorioAño["Diciembre"] : 0),

                        CantPatologiaEneroZP = (getPatologiasZPAño.TryGetValue("Enero", out salida) == true ? getPatologiasZPAño["Enero"] : 0),
                        CantPatologiaFebreroZP = (getPatologiasZPAño.TryGetValue("Febrero", out salida) == true ? getPatologiasZPAño["Febrero"] : 0),
                        CantPatologiaMarzoZP = (getPatologiasZPAño.TryGetValue("Marzo", out salida) == true ? getPatologiasZPAño["Marzo"] : 0),
                        CantPatologiaAbrilZP = (getPatologiasZPAño.TryGetValue("Abril", out salida) == true ? getPatologiasZPAño["Abril"] : 0),
                        CantPatologiaMayoZP = (getPatologiasZPAño.TryGetValue("Mayo", out salida) == true ? getPatologiasZPAño["Mayo"] : 0),
                        CantPatologiaJunioZP = (getPatologiasZPAño.TryGetValue("Junio", out salida) == true ? getPatologiasZPAño["Junio"] : 0),
                        CantPatologiaJulioZP = (getPatologiasZPAño.TryGetValue("Julio", out salida) == true ? getPatologiasZPAño["Julio"] : 0),
                        CantPatologiaAgostoZP = (getPatologiasZPAño.TryGetValue("Agosto", out salida) == true ? getPatologiasZPAño["Agosto"] : 0),
                        CantPatologiaSeptiembreZP = (getPatologiasZPAño.TryGetValue("Septiembre", out salida) == true ? getPatologiasZPAño["Septiembre"] : 0),
                        CantPatologiaOctubreZP = (getPatologiasZPAño.TryGetValue("Octubre", out salida) == true ? getPatologiasZPAño["Octubre"] : 0),
                        CantPatologiaNoviembreZP = (getPatologiasZPAño.TryGetValue("Noviembre", out salida) == true ? getPatologiasZPAño["Noviembre"] : 0),
                        CantPatologiaDiciembreZP = (getPatologiasZPAño.TryGetValue("Diciembre", out salida) == true ? getPatologiasZPAño["Diciembre"] : 0),

                        CantPatologiaEneroPD = (getPatologiasPieDiabeticoAño.TryGetValue("Enero", out salida) == true ? getPatologiasPieDiabeticoAño["Enero"] : 0),
                        CantPatologiaFebreroPD = (getPatologiasPieDiabeticoAño.TryGetValue("Febrero", out salida) == true ? getPatologiasPieDiabeticoAño["Febrero"] : 0),
                        CantPatologiaMarzoPD = (getPatologiasPieDiabeticoAño.TryGetValue("Marzo", out salida) == true ? getPatologiasPieDiabeticoAño["Marzo"] : 0),
                        CantPatologiaAbrilPD = (getPatologiasPieDiabeticoAño.TryGetValue("Abril", out salida) == true ? getPatologiasPieDiabeticoAño["Abril"] : 0),
                        CantPatologiaMayoPD = (getPatologiasPieDiabeticoAño.TryGetValue("Mayo", out salida) == true ? getPatologiasPieDiabeticoAño["Mayo"] : 0),
                        CantPatologiaJunioPD = (getPatologiasPieDiabeticoAño.TryGetValue("Junio", out salida) == true ? getPatologiasPieDiabeticoAño["Junio"] : 0),
                        CantPatologiaJulioPD = (getPatologiasPieDiabeticoAño.TryGetValue("Julio", out salida) == true ? getPatologiasPieDiabeticoAño["Julio"] : 0),
                        CantPatologiaAgostoPD = (getPatologiasPieDiabeticoAño.TryGetValue("Agosto", out salida) == true ? getPatologiasPieDiabeticoAño["Agosto"] : 0),
                        CantPatologiaSeptiembrePD = (getPatologiasPieDiabeticoAño.TryGetValue("Septiembre", out salida) == true ? getPatologiasPieDiabeticoAño["Septiembre"] : 0),
                        CantPatologiaOctubrePD = (getPatologiasPieDiabeticoAño.TryGetValue("Octubre", out salida) == true ? getPatologiasPieDiabeticoAño["Octubre"] : 0),
                        CantPatologiaNoviembrePD = (getPatologiasPieDiabeticoAño.TryGetValue("Noviembre", out salida) == true ? getPatologiasPieDiabeticoAño["Noviembre"] : 0),
                        CantPatologiaDiciembrePD = (getPatologiasPieDiabeticoAño.TryGetValue("Diciembre", out salida) == true ? getPatologiasPieDiabeticoAño["Diciembre"] : 0),

                        CantPatologiaEneroQX = (getPatologiasQuemadoAño.TryGetValue("Enero", out salida) == true ? getPatologiasQuemadoAño["Enero"] : 0),
                        CantPatologiaFebreroQX = (getPatologiasQuemadoAño.TryGetValue("Febrero", out salida) == true ? getPatologiasQuemadoAño["Febrero"] : 0),
                        CantPatologiaMarzoQX = (getPatologiasQuemadoAño.TryGetValue("Marzo", out salida) == true ? getPatologiasQuemadoAño["Marzo"] : 0),
                        CantPatologiaAbrilQX = (getPatologiasQuemadoAño.TryGetValue("Abril", out salida) == true ? getPatologiasQuemadoAño["Abril"] : 0),
                        CantPatologiaMayoQX = (getPatologiasQuemadoAño.TryGetValue("Mayo", out salida) == true ? getPatologiasQuemadoAño["Mayo"] : 0),
                        CantPatologiaJunioQX = (getPatologiasQuemadoAño.TryGetValue("Junio", out salida) == true ? getPatologiasQuemadoAño["Junio"] : 0),
                        CantPatologiaJulioQX = (getPatologiasQuemadoAño.TryGetValue("Julio", out salida) == true ? getPatologiasQuemadoAño["Julio"] : 0),
                        CantPatologiaAgostoQX = (getPatologiasQuemadoAño.TryGetValue("Agosto", out salida) == true ? getPatologiasQuemadoAño["Agosto"] : 0),
                        CantPatologiaSeptiembreQX = (getPatologiasQuemadoAño.TryGetValue("Septiembre", out salida) == true ? getPatologiasQuemadoAño["Septiembre"] : 0),
                        CantPatologiaOctubreQX = (getPatologiasQuemadoAño.TryGetValue("Octubre", out salida) == true ? getPatologiasQuemadoAño["Octubre"] : 0),
                        CantPatologiaNoviembreQX = (getPatologiasQuemadoAño.TryGetValue("Noviembre", out salida) == true ? getPatologiasQuemadoAño["Noviembre"] : 0),
                        CantPatologiaDiciembreQX = (getPatologiasQuemadoAño.TryGetValue("Diciembre", out salida) == true ? getPatologiasQuemadoAño["Diciembre"] : 0),

                        CantPatologiaEneroTX = (getPatologiasTraumaAño.TryGetValue("Enero", out salida) == true ? getPatologiasTraumaAño["Enero"] : 0),
                        CantPatologiaFebreroTX = (getPatologiasTraumaAño.TryGetValue("Febrero", out salida) == true ? getPatologiasTraumaAño["Febrero"] : 0),
                        CantPatologiaMarzoTX = (getPatologiasTraumaAño.TryGetValue("Marzo", out salida) == true ? getPatologiasTraumaAño["Marzo"] : 0),
                        CantPatologiaAbrilTX = (getPatologiasTraumaAño.TryGetValue("Abril", out salida) == true ? getPatologiasTraumaAño["Abril"] : 0),
                        CantPatologiaMayoTX = (getPatologiasTraumaAño.TryGetValue("Mayo", out salida) == true ? getPatologiasTraumaAño["Mayo"] : 0),
                        CantPatologiaJunioTX = (getPatologiasTraumaAño.TryGetValue("Junio", out salida) == true ? getPatologiasTraumaAño["Junio"] : 0),
                        CantPatologiaJulioTX = (getPatologiasTraumaAño.TryGetValue("Julio", out salida) == true ? getPatologiasTraumaAño["Julio"] : 0),
                        CantPatologiaAgostoTX = (getPatologiasTraumaAño.TryGetValue("Agosto", out salida) == true ? getPatologiasTraumaAño["Agosto"] : 0),
                        CantPatologiaSeptiembreTX = (getPatologiasTraumaAño.TryGetValue("Septiembre", out salida) == true ? getPatologiasTraumaAño["Septiembre"] : 0),
                        CantPatologiaOctubreTX = (getPatologiasTraumaAño.TryGetValue("Octubre", out salida) == true ? getPatologiasTraumaAño["Octubre"] : 0),
                        CantPatologiaNoviembreTX = (getPatologiasTraumaAño.TryGetValue("Noviembre", out salida) == true ? getPatologiasTraumaAño["Noviembre"] : 0),
                        CantPatologiaDiciembreTX = (getPatologiasTraumaAño.TryGetValue("Diciembre", out salida) == true ? getPatologiasTraumaAño["Diciembre"] : 0),

                        CantPatologiaEneroUV = (getPatologiasVascularesAño.TryGetValue("Enero", out salida) == true ? getPatologiasVascularesAño["Enero"] : 0),
                        CantPatologiaFebreroUV = (getPatologiasVascularesAño.TryGetValue("Febrero", out salida) == true ? getPatologiasVascularesAño["Febrero"] : 0),
                        CantPatologiaMarzoUV = (getPatologiasVascularesAño.TryGetValue("Marzo", out salida) == true ? getPatologiasVascularesAño["Marzo"] : 0),
                        CantPatologiaAbrilUV = (getPatologiasVascularesAño.TryGetValue("Abril", out salida) == true ? getPatologiasVascularesAño["Abril"] : 0),
                        CantPatologiaMayoUV = (getPatologiasVascularesAño.TryGetValue("Mayo", out salida) == true ? getPatologiasVascularesAño["Mayo"] : 0),
                        CantPatologiaJunioUV = (getPatologiasVascularesAño.TryGetValue("Junio", out salida) == true ? getPatologiasVascularesAño["Junio"] : 0),
                        CantPatologiaJulioUV = (getPatologiasVascularesAño.TryGetValue("Julio", out salida) == true ? getPatologiasVascularesAño["Julio"] : 0),
                        CantPatologiaAgostoUV = (getPatologiasVascularesAño.TryGetValue("Agosto", out salida) == true ? getPatologiasVascularesAño["Agosto"] : 0),
                        CantPatologiaSeptiembreUV = (getPatologiasVascularesAño.TryGetValue("Septiembre", out salida) == true ? getPatologiasVascularesAño["Septiembre"] : 0),
                        CantPatologiaOctubreUV = (getPatologiasVascularesAño.TryGetValue("Octubre", out salida) == true ? getPatologiasVascularesAño["Octubre"] : 0),
                        CantPatologiaNoviembreUV = (getPatologiasVascularesAño.TryGetValue("Noviembre", out salida) == true ? getPatologiasVascularesAño["Noviembre"] : 0),
                        CantPatologiaDiciembreUV = (getPatologiasVascularesAño.TryGetValue("Diciembre", out salida) == true ? getPatologiasVascularesAño["Diciembre"] : 0),

                        CantPatologiaEneroOtros = (getPatologiasOtrosAño.TryGetValue("Enero", out salida) == true ? getPatologiasOtrosAño["Enero"] : 0),
                        CantPatologiaFebreroOtros = (getPatologiasOtrosAño.TryGetValue("Febrero", out salida) == true ? getPatologiasOtrosAño["Febrero"] : 0),
                        CantPatologiaMarzoOtros = (getPatologiasOtrosAño.TryGetValue("Marzo", out salida) == true ? getPatologiasOtrosAño["Marzo"] : 0),
                        CantPatologiaAbrilOtros = (getPatologiasOtrosAño.TryGetValue("Abril", out salida) == true ? getPatologiasOtrosAño["Abril"] : 0),
                        CantPatologiaMayoOtros = (getPatologiasOtrosAño.TryGetValue("Mayo", out salida) == true ? getPatologiasOtrosAño["Mayo"] : 0),
                        CantPatologiaJunioOtros = (getPatologiasOtrosAño.TryGetValue("Junio", out salida) == true ? getPatologiasOtrosAño["Junio"] : 0),
                        CantPatologiaJulioOtros = (getPatologiasOtrosAño.TryGetValue("Julio", out salida) == true ? getPatologiasOtrosAño["Julio"] : 0),
                        CantPatologiaAgostoOtros = (getPatologiasOtrosAño.TryGetValue("Agosto", out salida) == true ? getPatologiasOtrosAño["Agosto"] : 0),
                        CantPatologiaSeptiembreOtros = (getPatologiasOtrosAño.TryGetValue("Septiembre", out salida) == true ? getPatologiasOtrosAño["Septiembre"] : 0),
                        CantPatologiaOctubreOtros = (getPatologiasOtrosAño.TryGetValue("Octubre", out salida) == true ? getPatologiasOtrosAño["Octubre"] : 0),
                        CantPatologiaNoviembreOtros = (getPatologiasOtrosAño.TryGetValue("Noviembre", out salida) == true ? getPatologiasOtrosAño["Noviembre"] : 0),
                        CantPatologiaDiciembreOtros = (getPatologiasOtrosAño.TryGetValue("Diciembre", out salida) == true ? getPatologiasOtrosAño["Diciembre"] : 0),

                        //Horizontales
                        TotalPatologiaAnualVascular = TotalPatologiaAnualVascular,
                        TotalPatologiaAnualPos = TotalPatologiaAnualPos,
                        TotalPatologiaAnualZP = TotalPatologiaAnualZP,
                        TotalPatologiaAnualTX = TotalPatologiaAnualTX,
                        TotalPatologiaAnualQX = TotalPatologiaAnualQX,
                        TotalPatologiaAnualOtros = TotalPatologiaAnualOtros,
                        TotalPatologiaAnualPD = TotalPatologiaAnualPD,

                        //Total Curaiones de todo todo todo
                        TotalAñoTodasPatologiasH = TotalAñoTodasPatologiasH,

                        //Total del Mes de manera vertical
                        TotalPatologiaAnualEneroVertical = TotalPatologiaAnualEneroVertical,
                        TotalPatologiaAnualFebreroVertical = TotalPatologiaAnualFebreroVertical,
                        TotalPatologiaAnualMarzoVertical = TotalPatologiaAnualMarzoVertical,
                        TotalPatologiaAnualAbrilVertical = TotalPatologiaAnualAbrilVertical,
                        TotalPatologiaAnualMayoVertical = TotalPatologiaAnualMayoVertical,
                        TotalPatologiaAnualJunioVertical = TotalPatologiaAnualJunioVertical,
                        TotalPatologiaAnualJulioVertical = TotalPatologiaAnualJulioVertical,
                        TotalPatologiaAnualAgostoVertical = TotalPatologiaAnualAgostoVertical,
                        TotalPatologiaAnualSeptiembreVertical = TotalPatologiaAnualSeptiembreVertical,
                        TotalPatologiaAnualOctubreVertical = TotalPatologiaAnualOctubreVertical,
                        TotalPatologiaAnualNoviembreVertical = TotalPatologiaAnualNoviembreVertical,
                        TotalPatologiaAnualDiciembreVertical = TotalPatologiaAnualDiciembreVertical,

                        //Doubles porcentajes verticales
                        TotalEneroPatologiaPOPVertical = TotalEneroPatologiaPOPVertical,
                        TotalEneroPatologiaZPVertical = TotalEneroPatologiaZPVertical,
                        TotalEneroPatologiaPDVertical = TotalEneroPatologiaPDVertical,
                        TotalEneroPatologiaQXVertical = TotalEneroPatologiaQXVertical,
                        TotalEneroPatologiaTXVertical = TotalEneroPatologiaTXVertical,
                        TotalEneroPatologiaUVVertical = TotalEneroPatologiaUVVertical,
                        TotalEneroPatologiaOtrosVertical = TotalEneroPatologiaOtrosVertical,

                        TotalFebreroPatologiaPOPVertical = TotalFebreroPatologiaPOPVertical,
                        TotalFebreroPatologiaZPVertical = TotalFebreroPatologiaZPVertical,
                        TotalFebreroPatologiaPDVertical = TotalFebreroPatologiaPDVertical,
                        TotalFebreroPatologiaQXVertical = TotalFebreroPatologiaQXVertical,
                        TotalFebreroPatologiaTXVertical = TotalFebreroPatologiaTXVertical,
                        TotalFebreroPatologiaUVVertical = TotalFebreroPatologiaUVVertical,
                        TotalFebreroPatologiaOtrosVertical = TotalFebreroPatologiaOtrosVertical,

                        TotalMarzoPatologiaPOPVertical = TotalMarzoPatologiaPOPVertical,
                        TotalMarzoPatologiaZPVertical = TotalMarzoPatologiaZPVertical,
                        TotalMarzoPatologiaPDVertical = TotalMarzoPatologiaPDVertical,
                        TotalMarzoPatologiaQXVertical = TotalMarzoPatologiaQXVertical,
                        TotalMarzoPatologiaTXVertical = TotalMarzoPatologiaTXVertical,
                        TotalMarzoPatologiaUVVertical = TotalMarzoPatologiaUVVertical,
                        TotalMarzoPatologiaOtrosVertical = TotalMarzoPatologiaOtrosVertical,

                        TotalAbrilPatologiaPOPVertical = TotalAbrilPatologiaPOPVertical,
                        TotalAbrilPatologiaZPVertical = TotalAbrilPatologiaZPVertical,
                        TotalAbrilPatologiaPDVertical = TotalAbrilPatologiaPDVertical,
                        TotalAbrilPatologiaQXVertical = TotalAbrilPatologiaQXVertical,
                        TotalAbrilPatologiaTXVertical = TotalAbrilPatologiaTXVertical,
                        TotalAbrilPatologiaUVVertical = TotalAbrilPatologiaUVVertical,
                        TotalAbrilPatologiaOtrosVertical = TotalAbrilPatologiaOtrosVertical,

                        TotalMayoPatologiaPOPVertical = TotalMayoPatologiaPOPVertical,
                        TotalMayoPatologiaZPVertical = TotalMayoPatologiaZPVertical,
                        TotalMayoPatologiaPDVertical = TotalMayoPatologiaPDVertical,
                        TotalMayoPatologiaQXVertical = TotalMayoPatologiaQXVertical,
                        TotalMayoPatologiaTXVertical = TotalMayoPatologiaTXVertical,
                        TotalMayoPatologiaUVVertical = TotalMayoPatologiaUVVertical,
                        TotalMayoPatologiaOtrosVertical = TotalMayoPatologiaOtrosVertical,

                        TotalJunioPatologiaPOPVertical = TotalJunioPatologiaPOPVertical,
                        TotalJunioPatologiaZPVertical = TotalJunioPatologiaZPVertical,
                        TotalJunioPatologiaPDVertical = TotalJunioPatologiaPDVertical,
                        TotalJunioPatologiaQXVertical = TotalJunioPatologiaQXVertical,
                        TotalJunioPatologiaTXVertical = TotalJunioPatologiaTXVertical,
                        TotalJunioPatologiaUVVertical = TotalJunioPatologiaUVVertical,
                        TotalJunioPatologiaOtrosVertical = TotalJunioPatologiaOtrosVertical,

                        TotalJulioPatologiaPOPVertical = TotalJulioPatologiaPOPVertical,
                        TotalJulioPatologiaZPVertical = TotalJulioPatologiaZPVertical,
                        TotalJulioPatologiaPDVertical = TotalJulioPatologiaPDVertical,
                        TotalJulioPatologiaQXVertical = TotalJulioPatologiaQXVertical,
                        TotalJulioPatologiaTXVertical = TotalJulioPatologiaTXVertical,
                        TotalJulioPatologiaUVVertical = TotalJulioPatologiaUVVertical,
                        TotalJulioPatologiaOtrosVertical = TotalJulioPatologiaOtrosVertical,

                        TotalAgostoPatologiaPOPVertical = TotalAgostoPatologiaPOPVertical,
                        TotalAgostoPatologiaZPVertical = TotalAgostoPatologiaZPVertical,
                        TotalAgostoPatologiaPDVertical = TotalAgostoPatologiaPDVertical,
                        TotalAgostoPatologiaQXVertical = TotalAgostoPatologiaQXVertical,
                        TotalAgostoPatologiaTXVertical = TotalAgostoPatologiaTXVertical,
                        TotalAgostoPatologiaUVVertical = TotalAgostoPatologiaUVVertical,
                        TotalAgostoPatologiaOtrosVertical = TotalAgostoPatologiaOtrosVertical,

                        TotalSeptiembrePatologiaPOPVertical = TotalSeptiembrePatologiaPOPVertical,
                        TotalSeptiembrePatologiaZPVertical = TotalSeptiembrePatologiaZPVertical,
                        TotalSeptiembrePatologiaPDVertical = TotalSeptiembrePatologiaPDVertical,
                        TotalSeptiembrePatologiaQXVertical = TotalSeptiembrePatologiaQXVertical,
                        TotalSeptiembrePatologiaTXVertical = TotalSeptiembrePatologiaTXVertical,
                        TotalSeptiembrePatologiaUVVertical = TotalSeptiembrePatologiaUVVertical,
                        TotalSeptiembrePatologiaOtrosVertical = TotalSeptiembrePatologiaOtrosVertical,

                        TotalOctubrePatologiaPOPVertical = TotalOctubrePatologiaPOPVertical,
                        TotalOctubrePatologiaZPVertical = TotalOctubrePatologiaZPVertical,
                        TotalOctubrePatologiaPDVertical = TotalOctubrePatologiaPDVertical,
                        TotalOctubrePatologiaQXVertical = TotalOctubrePatologiaQXVertical,
                        TotalOctubrePatologiaTXVertical = TotalOctubrePatologiaTXVertical,
                        TotalOctubrePatologiaUVVertical = TotalOctubrePatologiaUVVertical,
                        TotalOctubrePatologiaOtrosVertical = TotalOctubrePatologiaOtrosVertical,

                        TotalNoviembrePatologiaPOPVertical = TotalNoviembrePatologiaPOPVertical,
                        TotalNoviembrePatologiaZPVertical = TotalNoviembrePatologiaZPVertical,
                        TotalNoviembrePatologiaPDVertical = TotalNoviembrePatologiaPDVertical,
                        TotalNoviembrePatologiaQXVertical = TotalNoviembrePatologiaQXVertical,
                        TotalNoviembrePatologiaTXVertical = TotalNoviembrePatologiaTXVertical,
                        TotalNoviembrePatologiaUVVertical = TotalNoviembrePatologiaUVVertical,
                        TotalNoviembrePatologiaOtrosVertical = TotalNoviembrePatologiaOtrosVertical,

                        TotalDiciembrePatologiaPOPVertical = TotalDiciembrePatologiaPOPVertical,
                        TotalDiciembrePatologiaZPVertical = TotalDiciembrePatologiaZPVertical,
                        TotalDiciembrePatologiaPDVertical = TotalDiciembrePatologiaPDVertical,
                        TotalDiciembrePatologiaQXVertical = TotalDiciembrePatologiaQXVertical,
                        TotalDiciembrePatologiaTXVertical = TotalDiciembrePatologiaTXVertical,
                        TotalDiciembrePatologiaUVVertical = TotalDiciembrePatologiaUVVertical,
                        TotalDiciembrePatologiaOtrosVertical = TotalDiciembrePatologiaOtrosVertical,
                        TOTALPATOLOGIASANUAL = TOTALPATOLOGIASANUAL,

                        TOTALPATOLOGIASUVHORIZONTAL = TOTALPATOLOGIASUVHORIZONTAL,
                        TOTALPATOLOGIASPOPHORIZONTAL = TOTALPATOLOGIASPOPHORIZONTAL,
                        TOTALPATOLOGIASZPHORIZONTAL = TOTALPATOLOGIASZPHORIZONTAL,
                        TOTALPATOLOGIASPDHORIZONTAL = TOTALPATOLOGIASPDHORIZONTAL,
                        TOTALPATOLOGIASQXHORIZONTAL = TOTALPATOLOGIASQXHORIZONTAL,
                        TOTALPATOLOGIASTXHORIZONTAL = TOTALPATOLOGIASTXHORIZONTAL,
                        TOTALPATOLOGIASOTROSHORIZONTAL = TOTALPATOLOGIASOTROSHORIZONTAL
                    });
                }

                Thread thread = new Thread(ExportInforme);
                thread.SetApartmentState(ApartmentState.STA); // Configura el subproceso en STA
                thread.Start();
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        Dictionary<string, int> GetPatologiasMesAMes(string Patologia)
        {
            try
            {
                Dictionary<string, int> dicPat = new Dictionary<string, int>();

                if (Patologia != "Otros")
                {
                    foreach (string m in ObtenerListaDeMeses())
                    {
                        int CantPatologia = repositorioInfEnf.getCantidadPatologia(m, comboBox2.Text, Patologia);
                        dicPat.Add(m, CantPatologia);
                    }
                }
                else
                {
                    foreach (string m in ObtenerListaDeMeses())
                    {
                        int CantPatologia = repositorioInfEnf.getCantidadPatologia2(m, comboBox2.Text, Patologia);
                        dicPat.Add(m, CantPatologia);
                    }
                }

                return dicPat;
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
                return null;
            }
        }

        Byte[] Grafico4()
        {
            try
            {
                Chart chart = new Chart
                {
                    Width = 800,
                    Height = 600,
                    RenderType = RenderType.ImageTag,
                    AntiAliasing = AntiAliasingStyles.All,
                    TextAntiAliasingQuality = TextAntiAliasingQuality.High
                };

                chart.Titles.Add("CUADRO COMPARATIVO DE CURACIONES");
                chart.Titles[0].Font = new System.Drawing.Font("Arial Narrow", 10f);

                chart.ChartAreas.Add("");
                chart.ChartAreas[0].AxisX.Title = "";
                chart.ChartAreas[0].AxisY.Title = "Nivel";
                chart.ChartAreas[0].AxisX.TitleFont = new System.Drawing.Font("Arial Narrow", 10f);
                chart.ChartAreas[0].AxisX.LabelStyle.Font = new System.Drawing.Font("Arial Narrow", 10f);
                chart.ChartAreas[0].AxisX.LabelStyle.Angle = -90;
                chart.ChartAreas[0].BackColor = Color.White;

                chart.Series.Clear();

                List<int> ObtenerListaDePersonas = repositorioInfEnf.getEnfermerosYear(comboBox2.Text);

                foreach (int persona in ObtenerListaDePersonas)
                {
                    string nameProf = repositorioBodegas.getDatosCode(persona).Bod_Responsable;
                    Series series = new Series(nameProf);
                    series.ChartType = SeriesChartType.StackedBar;

                    // Agregar puntos para cada mes
                    foreach (string mes in ObtenerListaDeMeses())
                    {
                        int _tempCant = repositorioInfEnf.getCantidadAtendidos(persona, mes, comboBox2.Text);
                        series.Points.AddXY(mes, _tempCant);
                    }

                    chart.Series.Add(series);
                }

                var ms = new System.IO.MemoryStream();
                chart.SaveImage(ms, ChartImageFormat.Jpeg);
                byte[] pdfBytes = ms.ToArray();
                return pdfBytes;
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
                return null;
            }
        }

        List<string> ObtenerListaDeMeses()
        {
            List<string> L = new List<string>();

            if (comboBox1.Text == "Enero")
            {
                L.Add("Enero");
            }
            else if (comboBox1.Text == "Febrero")
            {
                L.Add("Enero");
                L.Add("Febrero");
            }
            else if (comboBox1.Text == "Marzo")
            {
                L.Add("Enero");
                L.Add("Febrero");
                L.Add("Marzo");
            }
            else if (comboBox1.Text == "Abril")
            {
                L.Add("Enero");
                L.Add("Febrero");
                L.Add("Marzo");
                L.Add("Abril");
            }
            else if (comboBox1.Text == "Mayo")
            {
                L.Add("Enero");
                L.Add("Febrero");
                L.Add("Marzo");
                L.Add("Abril");
                L.Add("Mayo");
            }
            else if (comboBox1.Text == "Junio")
            {
                L.Add("Enero");
                L.Add("Febrero");
                L.Add("Marzo");
                L.Add("Abril");
                L.Add("Mayo");
                L.Add("Junio");
            }
            else if (comboBox1.Text == "Julio")
            {
                L.Add("Enero");
                L.Add("Febrero");
                L.Add("Marzo");
                L.Add("Abril");
                L.Add("Mayo");
                L.Add("Junio");
                L.Add("Julio");
            }
            else if (comboBox1.Text == "Agosto")
            {
                L.Add("Enero");
                L.Add("Febrero");
                L.Add("Marzo");
                L.Add("Abril");
                L.Add("Mayo");
                L.Add("Junio");
                L.Add("Julio");
                L.Add("Agosto");
            }
            else if (comboBox1.Text == "Septiembre")
            {
                L.Add("Enero");
                L.Add("Febrero");
                L.Add("Marzo");
                L.Add("Abril");
                L.Add("Mayo");
                L.Add("Junio");
                L.Add("Julio");
                L.Add("Agosto");
                L.Add("Septiembre");
            }
            else if (comboBox1.Text == "Octubre")
            {
                L.Add("Enero");
                L.Add("Febrero");
                L.Add("Marzo");
                L.Add("Abril");
                L.Add("Mayo");
                L.Add("Junio");
                L.Add("Julio");
                L.Add("Agosto");
                L.Add("Septiembre");
                L.Add("Octubre");
            }
            else if (comboBox1.Text == "Noviembre")
            {
                L.Add("Enero");
                L.Add("Febrero");
                L.Add("Marzo");
                L.Add("Abril");
                L.Add("Mayo");
                L.Add("Junio");
                L.Add("Julio");
                L.Add("Agosto");
                L.Add("Septiembre");
                L.Add("Octubre");
                L.Add("Noviembre");
            }
            else if (comboBox1.Text == "Diciembre")
            {
                L.Add("Enero");
                L.Add("Febrero");
                L.Add("Marzo");
                L.Add("Abril");
                L.Add("Mayo");
                L.Add("Junio");
                L.Add("Julio");
                L.Add("Agosto");
                L.Add("Septiembre");
                L.Add("Octubre");
                L.Add("Noviembre");
                L.Add("Diciembre");
            }           

            return L;
        }

        Byte[] Grafico3(int C4160, int C6180, int C2140, int C80, int C020)
        {
            try
            {
                Chart chart = new Chart
                {
                    Width = 800,
                    Height = 600,
                    RenderType = RenderType.ImageTag,
                    AntiAliasing = AntiAliasingStyles.All,
                    TextAntiAliasingQuality = TextAntiAliasingQuality.High
                };

                chart.Titles.Add("RANGOS DE EDAD");
                chart.Titles[0].Font = new System.Drawing.Font("Arial Narrow", 10f);

                chart.ChartAreas.Add("");
                chart.ChartAreas[0].AxisX.Title = "";
                chart.ChartAreas[0].AxisY.Title = "Nivel";
                chart.ChartAreas[0].AxisX.TitleFont = new System.Drawing.Font("Arial Narrow", 10f);
                chart.ChartAreas[0].AxisX.LabelStyle.Font = new System.Drawing.Font("Arial Narrow", 10f);
                chart.ChartAreas[0].AxisX.LabelStyle.Angle = -90;
                chart.ChartAreas[0].BackColor = Color.White;

                chart.Series.Add("");
                chart.Series[0].ChartType = SeriesChartType.Column;

                chart.Series[0].Points.AddXY("0 A 20" + "   " + C020.ToString(), C020);
                chart.Series[0].Points.AddXY("21 A 40" + "   " + C2140.ToString(), C2140);
                chart.Series[0].Points.AddXY("41 A 60" + "   " + C4160.ToString(), C4160);
                chart.Series[0].Points.AddXY("61 A 80" + "   " + C6180.ToString(), C6180);
                chart.Series[0].Points.AddXY("> DE 80" + "   " + C80.ToString(), C80);

                var ms = new System.IO.MemoryStream();
                chart.SaveImage(ms, ChartImageFormat.Jpeg);
                byte[] pdfBytes = ms.ToArray();
                return pdfBytes;
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
                return null;
            }
        }

        Byte[] Grafico2(int Vas, int Pos, int ZP, int TX, int QX, int Pie, int Otros)
        {
            try
            {
                Chart chart = new Chart
                {
                    Width = 800,
                    Height = 600,
                    RenderType = RenderType.ImageTag,
                    AntiAliasing = AntiAliasingStyles.All,
                    TextAntiAliasingQuality = TextAntiAliasingQuality.High
                };

                chart.Titles.Add("ATENCION SEGUN ETIOLOGIA");
                chart.Titles[0].Font = new System.Drawing.Font("Arial Narrow", 10f);

                chart.ChartAreas.Add("");
                chart.ChartAreas[0].AxisX.Title = "";
                chart.ChartAreas[0].AxisY.Title = "Nivel";
                chart.ChartAreas[0].AxisX.TitleFont = new System.Drawing.Font("Arial Narrow", 10f);
                chart.ChartAreas[0].AxisX.LabelStyle.Font = new System.Drawing.Font("Arial Narrow", 10f);
                chart.ChartAreas[0].AxisX.LabelStyle.Angle = -90;
                chart.ChartAreas[0].BackColor = Color.White;

                chart.Series.Add("");
                chart.Series[0].ChartType = SeriesChartType.Column;

                chart.Series[0].Points.AddXY("Vasculares" + "   " + Vas.ToString(), Vas);
                chart.Series[0].Points.AddXY("PosOperatorios" + "   " + Pos.ToString(), Pos);
                chart.Series[0].Points.AddXY("Zona de Presion" + "   " + ZP.ToString(), ZP);
                chart.Series[0].Points.AddXY("Trauma" + "   " + TX.ToString(), TX);
                chart.Series[0].Points.AddXY("Quemadura" + "   " + QX.ToString(), QX);
                chart.Series[0].Points.AddXY("Pie Diabetico" + "   " + Pie.ToString(), Pie);
                chart.Series[0].Points.AddXY("Otros" + "   " + Otros.ToString(), Otros);

                var ms = new System.IO.MemoryStream();
                chart.SaveImage(ms, ChartImageFormat.Jpeg);
                byte[] pdfBytes = ms.ToArray();
                return pdfBytes;
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
                return null;
            }
        }

        Byte[] Grafico1(Dictionary<int, int> datos, Dictionary<int, string> datos2, string Titulo)
        {
            try
            {
                Chart chart = new Chart
                {
                    Width = 800,
                    Height = 600,
                    RenderType = RenderType.ImageTag,
                    AntiAliasing = AntiAliasingStyles.All,
                    TextAntiAliasingQuality = TextAntiAliasingQuality.High
                };

                chart.Titles.Add(Titulo);
                chart.Titles[0].Font = new System.Drawing.Font("Arial Narrow", 10f);

                chart.ChartAreas.Add("");
                chart.ChartAreas[0].AxisX.Title = "";
                chart.ChartAreas[0].AxisY.Title = "Nivel";
                chart.ChartAreas[0].AxisX.TitleFont = new System.Drawing.Font("Arial Narrow", 10f);
                chart.ChartAreas[0].AxisX.LabelStyle.Font = new System.Drawing.Font("Arial Narrow", 10f);
                chart.ChartAreas[0].AxisX.LabelStyle.Angle = -90;
                chart.ChartAreas[0].BackColor = Color.White;

                chart.Series.Add("");
                chart.Series[0].ChartType = SeriesChartType.Column;

                for (int i = 0; i < datos.Count; i++)
                {
                    if (Convert.ToInt32(datos[i]) >= 1)
                    {
                        chart.Series[0].Points.AddXY(datos2[i].ToString() + "   " + datos[i].ToString(), datos[i]);
                    }                    
                }

                var ms = new System.IO.MemoryStream();
                chart.SaveImage(ms, ChartImageFormat.Jpeg);
                byte[] pdfBytes = ms.ToArray();
                return pdfBytes;
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
                return null;
            }
        }

        void ExportInformeMG()
        {
            Form F = new Form();
            ReportViewer R = new ReportViewer();

            R.LocalReport.DataSources.Clear();
            R.LocalReport.DataSources.Add(new ReportDataSource("DataSet_Informes", Ie));
            R.LocalReport.ReportEmbeddedResource = "ZamenisHealth.Reportes.RDLC_InformeEnfermeriaMG.rdlc";
            R.SetDisplayMode(DisplayMode.PrintLayout);
            R.ZoomMode = ZoomMode.Percent;
            R.ZoomPercent = 100;
            R.LocalReport.EnableExternalImages = true;
            R.Font = new Font("Arial", 7);
            R.RefreshReport();
            R.Visible = true;
            R.Dock = System.Windows.Forms.DockStyle.Fill;
            F.Controls.Add(R);
            F.WindowState = FormWindowState.Maximized;
            //F.System.Threading.ApartmentState state = System.Threading.ApartmentState.STA;
            //F.System.Threading.Thread.CurrentThread.SetApartmentState(state);
            Application.Run(F);
        }

        void ExportInforme()
        {
            Form F = new Form();
            ReportViewer R = new ReportViewer();

            R.LocalReport.DataSources.Clear();
            R.LocalReport.DataSources.Add(new ReportDataSource("DataSet_Informes", Ie));
            R.LocalReport.ReportEmbeddedResource = "ZamenisHealth.Reportes.RDLC_InformeEnfermeria.rdlc";
            R.SetDisplayMode(DisplayMode.PrintLayout);
            R.ZoomMode = ZoomMode.Percent;
            R.ZoomPercent = 100;
            R.LocalReport.EnableExternalImages = true;
            R.Font = new Font("Arial", 7);
            R.RefreshReport();
            R.Visible = true;
            R.Dock = System.Windows.Forms.DockStyle.Fill;
            F.Controls.Add(R);
            F.WindowState = FormWindowState.Maximized;
            //F.System.Threading.ApartmentState state = System.Threading.ApartmentState.STA;
            //F.System.Threading.Thread.CurrentThread.SetApartmentState(state);
            Application.Run(F);
        }
    }
}
