using Domain;
using Domain.CXN;
using FormAndControls;
using Microsoft.Reporting.WinForms;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using Persistence.CXN_ADJUNTOS.Interfaces;
using Persistence.CXN_ADJUNTOS.Metodos;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZamenisHealth.Clases;
using ZamenisHealth.Clases.Controles;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.Medicina
{
    public partial class Historial_Medico_2 : Forma2
    {
        private static readonly IReportes repoReportes = new MReportes();
        private static readonly IOrdenes repoOrdenes = new MOrdenes();
        private static readonly IAdjuntos repoAdjuntos = new MAdjuntos();

        FlowLayoutPanel flow;
        private readonly Font fuenteNormal = new Font("Microsoft Sans Serif", 10, FontStyle.Regular);
        private readonly Font fuenteNegrita = new Font("Microsoft Sans Serif", 10, FontStyle.Bold);
        private ToolTip toolBntSee = new ToolTip();

        private int Paciente;
        private string SeleccionReporte;
        private string TipoTerapia;
        private string TipoNodoGrupal;
        string NodoPrincipal2, TittleSelected2;

        List<HCMG> H_HCMGHC;
        List<ReportHCFI> H_HCFICompleto;
        List<ReportEVO> H_HCEVO;
        private MensajesGeneral MG;
        

        public Historial_Medico_2(int paciente)
        {
            InitializeComponent();
            this.Paciente = paciente;
        }
        
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            try
            {
                Verificador_Paciente();

                if (treeView1.SelectedNode != null)
                {
                    string TittleSelected = treeView1.SelectedNode.Text.ToString();
                    TittleSelected2 = treeView1.SelectedNode.Text.ToString();

                    if (e.Node.Parent != null)
                    {
                        string NodoPrincipal = e.Node.Parent.Text.ToString();
                        NodoPrincipal2 = e.Node.Parent.Text.ToString();

                        if (NodoPrincipal == "MEDICINA GENERAL")
                        {
                            switch (TittleSelected)
                            {
                                case "Historia Clinica Individual":
                                    SeleccionReporte = "HisMG";
                                    BuscarH(SeleccionReporte);
                                    panel1.Visible = false;
                                    break;

                                case "Medidas de Heridas":
                                    SeleccionReporte = "HisMGDiametro";
                                    BuscarH("HisMG");
                                    panel1.Visible = false;
                                    break;

                                case "Historia Clinica Agrupada":
                                    TipoNodoGrupal = "MEDGEN";
                                    SeleccionReporte = "";
                                    BuscarH("HisMG");
                                    panel1.Visible = true;
                                    break;

                                case "Ordenes Medicas Medicina General":
                                    SeleccionReporte = "MG";
                                    BuscarOM("MG");
                                    panel1.Visible = false;
                                    break;
                            }
                        }
                        else if (NodoPrincipal == "MEDICINA FISICA Y REHABILITACION")
                        {
                            switch (TittleSelected)
                            {
                                case "Historia Clinica Individual":
                                    SeleccionReporte = "HFI";
                                    BuscarH(SeleccionReporte);
                                    panel1.Visible = false;
                                    break;

                                case "Historia Clinica Agrupada":
                                    TipoNodoGrupal = "FISIATRIA";
                                    SeleccionReporte = "";
                                    BuscarH("HFI");
                                    panel1.Visible = true;
                                    break;

                                case "Ordenes Medicas Fisiatria":
                                    SeleccionReporte = "FI";
                                    BuscarOM("FI");
                                    panel1.Visible = false;
                                    break;
                            }
                        }
                        else if (NodoPrincipal == "PSIQUIATRIA")
                        {
                            switch (TittleSelected)
                            {
                                case "Historia Clinica":
                                    SeleccionReporte = "PSIQ";
                                    BuscarH(SeleccionReporte);
                                    panel1.Visible = false;
                                    break;

                                case "Seguimiento":
                                    SeleccionReporte = "PSIQ2";
                                    BuscarH(SeleccionReporte);
                                    panel1.Visible = false;
                                    break;

                                case "Evolucion":
                                    SeleccionReporte = "PSIQ3";
                                    BuscarH(SeleccionReporte);
                                    panel1.Visible = false;
                                    break;
                            }
                        }
                        else if (NodoPrincipal == "ENFERMERIA")
                        {
                            switch (TittleSelected)
                            {
                                case "Notas de Enfermeria":
                                    SeleccionReporte = "Notas";
                                    BuscarH(SeleccionReporte);
                                    panel1.Visible = false;
                                    break;

                                case "Notas Enfermeria Agrupada":
                                    TipoNodoGrupal = "NOTAS";
                                    SeleccionReporte = "";
                                    BuscarH("Notas");
                                    panel1.Visible = true;
                                    break;

                                case "Cambios de Manejo":
                                    SeleccionReporte = "CMan";
                                    BuscarH(SeleccionReporte);
                                    panel1.Visible = false;
                                    break;
                            }
                        }
                        else if (NodoPrincipal == "PSICOLOGIA")
                        {
                            switch (TittleSelected)
                            {
                                case "Historia Clinica Individual":
                                    SeleccionReporte = "PSI";
                                    BuscarH(SeleccionReporte);
                                    panel1.Visible = false;
                                    break;

                                case "Historia Clinica Agrupada":
                                    TipoNodoGrupal = "PSI";
                                    SeleccionReporte = "";
                                    BuscarH("HEVO");
                                    panel1.Visible = true;
                                    break;
                            }
                        }
                        else if (NodoPrincipal == "TERAPIA FISICA")
                        {
                            switch (TittleSelected)
                            {
                                case "Historia Clinica Individual":
                                    SeleccionReporte = "TF";
                                    BuscarH(SeleccionReporte);
                                    panel1.Visible = false;
                                    break;

                                case "Historia Clinica Agrupada":
                                    TipoNodoGrupal = "TF";
                                    SeleccionReporte = "";
                                    BuscarH("HEVO");
                                    panel1.Visible = true;
                                    break;
                            }
                        }
                        else if (NodoPrincipal == "TERAPIA OCUPACIONAL")
                        {
                            switch (TittleSelected)
                            {
                                case "Historia Clinica Individual":
                                    SeleccionReporte = "TO";
                                    BuscarH(SeleccionReporte);
                                    panel1.Visible = false;
                                    break;

                                case "Historia Clinica Agrupada":
                                    TipoNodoGrupal = "TO";
                                    SeleccionReporte = "";
                                    BuscarH("HEVO");
                                    panel1.Visible = true;
                                    break;
                            }
                        }
                        else if (NodoPrincipal == "EVOLUCIONES Y JUNTAS MEDICAS")
                        {
                            switch (TittleSelected)
                            {
                                case "Junta Medica 1":
                                    SeleccionReporte = "HJ1";
                                    BuscarH(SeleccionReporte);
                                    panel1.Visible = false;
                                    break;

                                case "Junta Medica 2":
                                    SeleccionReporte = "HJ2";
                                    BuscarH(SeleccionReporte);
                                    panel1.Visible = false;
                                    break;

                                case "Evoluciones":
                                    SeleccionReporte = "HEVO";
                                    BuscarH(SeleccionReporte);
                                    panel1.Visible = false;
                                    break;
                            }                            
                        }
                        else if (NodoPrincipal == "OTROS SOPORTES")
                        {
                            switch (TittleSelected)
                            {
                                case "Soportes Adicionales Cargados":
                                    SeleccionReporte = "Adjuntos";
                                    BuscarAdjuntos();
                                    panel1.Visible = false;
                                    break;
                            }
                        }
                        else
                        {
                            if (flow != null)
                                flow.Dispose();
                        }                        
                    }
                }
                else
                {
                    if (flow != null)
                        flow.Dispose();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                if (flow != null)
                    flow.Dispose();
            }
        }
        void ConfigTree()
        {
            try
            {                
                treeView1.CheckBoxes = false; 
                treeView1.FullRowSelect = true;
                treeView1.HideSelection = false;
                treeView1.Font = new Font("Arial", 11, FontStyle.Regular);

                #region  MEDICINA GENERAL
                TreeNode MedGen = new TreeNode("MEDICINA GENERAL");

                TreeNode MedGenHistory = new TreeNode("Historia Clinica Individual");
                TreeNode MedGenHistoryMedidas = new TreeNode("Medidas de Heridas");
                TreeNode MedGenHistoryTotal = new TreeNode("Historia Clinica Agrupada");
                TreeNode MedGenOrdenMedica = new TreeNode("Ordenes Medicas Medicina General");
                MedGen.Nodes.Add(MedGenHistory);
                MedGen.Nodes.Add(MedGenHistoryMedidas);
                MedGen.Nodes.Add(MedGenHistoryTotal);
                MedGen.Nodes.Add(MedGenOrdenMedica);

                treeView1.Nodes.Add(MedGen);
                #endregion

                #region  FISIATRIA
                TreeNode Fisi = new TreeNode("MEDICINA FISICA Y REHABILITACION");

                TreeNode FisiHistory = new TreeNode("Historia Clinica Individual");
                TreeNode FisiHistoryTotal = new TreeNode("Historia Clinica Agrupada");
                TreeNode FisiOrdenMedica = new TreeNode("Ordenes Medicas Fisiatria");
                Fisi.Nodes.Add(FisiHistory);
                Fisi.Nodes.Add(FisiHistoryTotal);
                Fisi.Nodes.Add(FisiOrdenMedica);

                treeView1.Nodes.Add(Fisi);
                #endregion

                #region  PSIQUIATRIA
                TreeNode psiq = new TreeNode("PSIQUIATRIA");

                TreeNode PsiqHistory = new TreeNode("Historia Clinica");
                TreeNode PsiqHistory2 = new TreeNode("Seguimiento");
                TreeNode PsiqHistory3 = new TreeNode("Evolucion");

                psiq.Nodes.Add(PsiqHistory);
                psiq.Nodes.Add(PsiqHistory2);
                psiq.Nodes.Add(PsiqHistory3);

                treeView1.Nodes.Add(psiq); 
                #endregion

                #region  NOTAS DE CURACION
                TreeNode Curacion = new TreeNode("ENFERMERIA");

                TreeNode CuracionHistory = new TreeNode("Notas de Enfermeria");
                TreeNode CuracionHistoryTotal = new TreeNode("Notas Enfermeria Agrupada");
                TreeNode CuracionCManejo = new TreeNode("Cambios de Manejo");

                Curacion.Nodes.Add(CuracionHistory);
                Curacion.Nodes.Add(CuracionHistoryTotal);
                Curacion.Nodes.Add(CuracionCManejo);

                treeView1.Nodes.Add(Curacion);
                #endregion

                #region PROGRAMA FIBROMIALGIA
                TreeNode Fibro = new TreeNode("PROGRAMA FIBROMIALGIA");

                #region  PSICOLOGIA
                TreeNode PSI = new TreeNode("PSICOLOGIA");

                TreeNode PSIHistory = new TreeNode("Historia Clinica Individual");
                TreeNode PSIHistoryTotal = new TreeNode("Historia Clinica Agrupada");

                PSI.Nodes.Add(PSIHistory);
                PSI.Nodes.Add(PSIHistoryTotal);

                Fibro.Nodes.Add(PSI);
                #endregion

                #region  TERAPIA FISICA
                TreeNode TFisica = new TreeNode("TERAPIA FISICA");

                TreeNode TFisicaHistory = new TreeNode("Historia Clinica Individual");
                TreeNode TFisicaHistoryTotal = new TreeNode("Historia Clinica Agrupada");

                TFisica.Nodes.Add(TFisicaHistory);
                TFisica.Nodes.Add(TFisicaHistoryTotal);

                Fibro.Nodes.Add(TFisica);
                #endregion

                #region  TERAPIA OCUPACIONAL
                TreeNode TOcupacional = new TreeNode("TERAPIA OCUPACIONAL");

                TreeNode TOcupacionalHistory = new TreeNode("Historia Clinica Individual");
                TreeNode TOcupacionalHistoryTotal = new TreeNode("Historia Clinica Agrupada");

                TOcupacional.Nodes.Add(TOcupacionalHistory);
                TOcupacional.Nodes.Add(TOcupacionalHistoryTotal);

                Fibro.Nodes.Add(TOcupacional);
                #endregion

                #region PROGRAMA
                TreeNode JMedica = new TreeNode("EVOLUCIONES Y JUNTAS MEDICAS");
                TreeNode JMedica1 = new TreeNode("Junta Medica 1");
                TreeNode JMedica2 = new TreeNode("Junta Medica 2");
                TreeNode Evol = new TreeNode("Evoluciones");

                JMedica.Nodes.Add(JMedica1);
                JMedica.Nodes.Add(JMedica2);
                JMedica.Nodes.Add(Evol);

                Fibro.Nodes.Add(JMedica);
                #endregion

                treeView1.Nodes.Add(Fibro);
                #endregion

                #region SOPORTES
                TreeNode soportes = new TreeNode("OTROS SOPORTES");

                TreeNode soportesHistory = new TreeNode("Soportes Adicionales Cargados");

                soportes.Nodes.Add(soportesHistory);

                treeView1.Nodes.Add(soportes);
                #endregion
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void Historial_Medico_2_Load(object sender, EventArgs e)
        {
            try
            {
                dateTimePicker3.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                dateTimePicker4.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

                treeView1.AfterSelect += treeView1_AfterSelect;
                ConfigTree();
                Titulo.Text = "Registros Medicos";
                SubTitulo.Text = Conexion.VersionApp;

                this.DoubleBuffered = true;
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }            
        }
        void Verificador_Paciente()
        {
            if (this.Paciente <= 0)
            {
                Comunes.MensajesGeneral MG = new Comunes.MensajesGeneral
                {
                    Mensaje = "No hay paciente seleccionado",
                    TipoImagen = 1000
                };
                MG.ShowDialog();

                this.Dispose();
                this.Close();
            }
        }
        async Task BuscarAdjuntos()
        {
            try
            {
                pictureBox1.Visible = true;
                pictureBox1.BringToFront();

                await Task.Delay(100);

                List<CXN_HORARIO> H = repoAdjuntos.getAdjuntos(this.Paciente);
                if (H != null)
                {
                    if (flow != null)
                    {
                        flow.Dispose();
                        flow = null;
                    }

                    flow = new FlowLayoutPanel();
                    flow.AutoScroll = true;
                    flow.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                    flow.BackColor = Color.WhiteSmoke;
                    flow.BorderStyle = BorderStyle.None;
                    flow.FlowDirection = FlowDirection.TopDown;
                    flow.Size = new Size(747, 581);
                    flow.Location = new Point(297, 77);
                    flow.WrapContents = false;
                    this.Controls.Add(flow);
                    flow.BringToFront();
                    flow.SuspendLayout();

                    flow.Visible = false;

                    try
                    {
                        int contador = 0;

                        foreach (var i in H)
                        {
                            CreaPanel(i.Hor_Id, i.Hor_Observacion, i.Com_Tipo_Doc, "ARCHIVO EXTRA SUBIDO", i.Hor_Pac_Fecha_Cita, "");

                            contador++;

                            if (contador % 10 == 0)
                            {
                                flow.ResumeLayout(true);
                                await Task.Delay(1);
                                flow.SuspendLayout();
                            }
                        }
                    }
                    finally
                    {
                        flow.ResumeLayout(true);
                        if (flow != null)
                            flow.Visible = true;
                        pictureBox1.SendToBack();
                        flow.BringToFront();
                    }
                }
                else
                {
                    if (flow != null)
                        flow.Dispose();
                        flow = null;
                        pictureBox1.SendToBack();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
                if (flow != null)
                    flow.Dispose();
                    flow = null;
                    pictureBox1.SendToBack();
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
        void TerapiasTotal()
        {
            try
            {
                string Tipo = this.TipoTerapia;

                H_HCEVO = new List<ReportEVO>();
                H_HCEVO = repoReportes.ReporteEvoluciones(this.Paciente,
                                                                          Convert.ToDateTime(dateTimePicker1.Value.Date),
                                                                          Convert.ToDateTime(dateTimePicker2.Value.Date),
                                                                          Tipo);
                if (H_HCEVO == null)
                {
                    MessageBox.Show("No se logro exportar, posiblemente halla una falla al exportar o la historia no existe",
                        "No se logro exportar",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation);
                    return;
                }

                Thread thread = new Thread(ExportRvSTATer);
                thread.SetApartmentState(ApartmentState.STA); // Configura el subproceso en STA
                thread.Start();
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        void ExportRvSTATer()
        {
            Form F = new Form();
            ReportViewer R = new ReportViewer();

            R.LocalReport.DataSources.Clear();
            R.LocalReport.DataSources.Add(new ReportDataSource("DataSet_HCEVO", H_HCEVO));
            R.LocalReport.ReportEmbeddedResource = "ZamenisHealth.Reportes.RDLC_HCEVO.rdlc";
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
        void FiTotal()
        {
            try
            {
                H_HCFICompleto = new List<ReportHCFI>();
                H_HCFICompleto = repoReportes.ReportefisiatriaCompleto(this.Paciente,
                                                                       dateTimePicker1.Value,
                                                                       dateTimePicker2.Value,
                                                                       Comunes.Contenedor.UsuarioLogueado);
                if (H_HCFICompleto == null)
                {
                    MG = new MensajesGeneral()
                    {
                        Mensaje = "No se logro exportar, posiblemente halla una falla al exportar o la historia no existe",
                        TipoImagen = 1000
                    };
                    
                    MG.ShowDialog();
                    return;
                }

                Thread thread = new Thread(ExportRvSTAFI);
                thread.SetApartmentState(ApartmentState.STA); // Configura el subproceso en STA
                thread.Start();
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        void ExportRvSTAFI()
        {
            Form F = new Form();
            ReportViewer R = new ReportViewer();

            R.LocalReport.DataSources.Clear();
            R.LocalReport.DataSources.Add(new ReportDataSource("DataSet_HCFI", H_HCFICompleto));
            R.LocalReport.ReportEmbeddedResource = "ZamenisHealth.Reportes.RDLC_HCFICompleto.rdlc";
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
        void MedGenTotal()
        {
            try
            {                
                H_HCMGHC = new List<HCMG>();
                H_HCMGHC = repoReportes.MedicinaGeneralCompleto(this.Paciente,
                                                                             dateTimePicker1.Value,
                                                                             dateTimePicker2.Value,
                                                                             Comunes.Contenedor.UsuarioLogueado);
                if (H_HCMGHC == null)
                {
                    MG = new MensajesGeneral()
                    {
                        Mensaje = "No se logro exportar, posiblemente halla una falla al exportar o la historia no existe",
                        TipoImagen = 1000
                    };
                    
                    MG.ShowDialog();
                    return;
                }                

                Thread thread = new Thread(ExportRvSTAMG);
                thread.SetApartmentState(ApartmentState.STA); // Configura el subproceso en STA
                thread.Start();                
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        void ExportRvSTAMG()
        {
            Form F = new Form();
            ReportViewer R = new ReportViewer();

            R.LocalReport.DataSources.Clear();
            R.LocalReport.DataSources.Add(new ReportDataSource("DataSet_HCMG", H_HCMGHC));
            R.LocalReport.ReportEmbeddedResource = "ZamenisHealth.Reportes.RDLC_HCMGCompleto.rdlc";
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
        void NotasTotal()
        {
            MensajesGeneral MG = new MensajesGeneral();
            try
            {
                var getRDLCMasivo = repoReportes.NotasMetodoRDLC(Convert.ToInt32(this.Paciente),
                                                                                            Convert.ToDateTime(dateTimePicker1.Value.Date),
                                                                                            Convert.ToDateTime(dateTimePicker2.Value.Date));
                if (getRDLCMasivo != null)
                {
                    int Contador = 1;
                    //Reportes.Maestro maestro = new Reportes.Maestro();
                    ReportViewer R = new ReportViewer();

                    int Total = getRDLCMasivo.Keys.Count;

                    while (Contador <= Total)
                    {                        
                        R.LocalReport.DataSources.Clear();
                        R.LocalReport.DataSources.Add(new ReportDataSource("DataSet_Notas", getRDLCMasivo[Contador]));
                        R.LocalReport.ReportEmbeddedResource = "ZamenisHealth.Reportes.RDLC_Notas.rdlc";                        
                        R.SetDisplayMode(DisplayMode.PrintLayout);
                        R.ZoomMode = ZoomMode.Percent;
                        R.ZoomPercent = 100;
                        R.Font = new Font("Arial", 7);
                        R.LocalReport.EnableExternalImages = true;
                        R.RefreshReport();
                        //maestro.Visible = true;
                        R.Dock = System.Windows.Forms.DockStyle.Fill;

                        byte[] bytes = R.LocalReport.Render("PDF");
                        FileStream fss = new FileStream("C:\\CXN\\Reportes\\" + Contador.ToString() + ".pdf", FileMode.Create);
                        fss.Write(bytes, 0, bytes.Length);
                        fss.Close();
                        Contador = Contador + 1;
                    }

                    string salida = "";
                    var dic = getRDLCMasivo[1];
                    foreach (var i in dic)
                    {
                        salida = i.PacienteNombre.ToString();
                        break;
                    }

                    Contador = Contador - 1;
                    Comunes.UnificadorPDFMasivo U = new Comunes.UnificadorPDFMasivo();
                    U.Unificar_Estructura(@"C:\CXN\Reportes\",
                                          1.ToString(),
                                          Total.ToString(),
                                          salida.ToString() + ".pdf");

                    R.Dispose();
                    
                    MG.TipoImagen = 3;
                    MG.Mensaje = "Generado en C CXN REPORTES";
                    MG.ShowDialog();
                }
                else
                {
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "No hay reportes en estas fechas";
                    MG.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        async Task BuscarOM(string TipoEs)
        {
            try
            {
                pictureBox1.Visible = true;
                pictureBox1.BringToFront();

                await Task.Delay(100);

                List<CXN_OM> getOM = repoOrdenes.getOrdenes(this.Paciente, TipoEs, dateTimePicker3.Value.Date, dateTimePicker4.Value.Date);
                if (getOM != null)
                {
                    if (flow != null)
                    {
                        flow.Dispose();
                        flow = null;
                    }

                    flow = new FlowLayoutPanel();
                    flow.AutoScroll = true;
                    flow.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                    flow.BackColor = Color.White;
                    flow.BorderStyle = BorderStyle.None;
                    flow.FlowDirection = FlowDirection.TopDown;
                    flow.Size = new Size(747, 581);
                    flow.Location = new Point(297, 77);
                    flow.WrapContents = false;
                    this.Controls.Add(flow);
                    flow.BringToFront();
                    flow.SuspendLayout();

                    flow.Visible = false;

                    try
                    {
                        int contador = 0;

                        foreach (var i in getOM)
                        {
                            Console.WriteLine(i.OM_Num);
                            CreaPanel(i.OM_Num, i.OM_Prof, i.OM_Tipo, i.OM_Clasificacion, i.OM_Fecha, i.OM_Cia.ToString());

                            contador++;

                            if (contador % 10 == 0)
                            {
                                flow.ResumeLayout(true);
                                await Task.Delay(1);
                                flow.SuspendLayout();
                            }
                        }
                    }
                    finally 
                    {
                        flow.ResumeLayout(true);
                        if (flow != null)
                            flow.Visible = true;
                            pictureBox1.SendToBack();
                            flow.BringToFront();
                    }
                }
                else
                {
                    if (flow != null)
                        flow.Dispose();
                        flow = null;
                        pictureBox1.SendToBack();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
                if (flow != null)
                    flow.Dispose();
                    flow = null;
                    pictureBox1.SendToBack();
            }
        }        
        private async Task BuscarH(string Tipo)
        {
            try
            {
                // Mostrar indicador de carga inmediatamente
                pictureBox1.Visible = true;
                pictureBox1.BringToFront();                

                // Permitir que Windows pinte el PictureBox
                await Task.Delay(100);

                // Consultar historias
                List<CXN_HORARIO> getHistorias =
                    await repoReportes.BuscarHistorias(this.Paciente, Tipo, dateTimePicker3.Value.Date, dateTimePicker4.Value.Date);

                if (flow != null)
                {
                    flow.Dispose();
                    flow = null;
                }

                if (getHistorias != null && getHistorias.Count > 0)
                {
                    flow = new FlowLayoutPanel();

                    flow.AutoScroll = true;
                    flow.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                    flow.BackColor = Color.White;
                    flow.BorderStyle = BorderStyle.None;
                    flow.FlowDirection = FlowDirection.TopDown;
                    flow.Size = new Size(747, 581);
                    flow.Location = new Point(297, 77);
                    flow.WrapContents = false;

                    this.Controls.Add(flow);

                    flow.SuspendLayout();

                    flow.Visible = false;

                    bool mostrarPaneles =
                        (Tipo == "HEVO" && SeleccionReporte != "") ||
                        (Tipo != "HEVO" && SeleccionReporte != "");

                    if (mostrarPaneles)
                    {
                        int contador = 0;

                        try
                        {
                            foreach (var i in getHistorias)
                            {
                                string junta = Tipo == "HEVO"
                                    ? i.Hor_Pac_Tipo_Serv
                                    : "";

                                CreaPanel(
                                    i.Hor_Id,
                                    i.Hor_Observacion,
                                    i.Hor_GrupoServicios,
                                    i.PacienteAseguradora,
                                    i.Hor_Pac_Fecha_Cita,
                                    junta
                                );

                                contador++;

                                // Cada 10 historias permitimos actualizar la interfaz
                                if (contador % 10 == 0)
                                {
                                    flow.ResumeLayout(true);

                                    await Task.Delay(1);

                                    flow.SuspendLayout();
                                }
                            }
                        }
                        finally
                        {
                            flow.ResumeLayout(true);
                        }

                        flow.BringToFront();
                        panel1.SendToBack();
                    }
                    else
                    {
                        flow.Dispose();
                        flow = null;

                        panel1.BringToFront();
                    }
                }
                else
                {
                    panel1.BringToFront();
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
            finally
            {
                pictureBox1.Visible = false;
                
                if (flow != null)
                    flow.Visible = true;
            }
        }        
        private async void button1_Click(object sender, EventArgs e)
        {
            try
            {
                Task oTask = null;
                Shows();

                switch (TipoNodoGrupal)
                {
                    case "NOTAS":
                        oTask = new Task(NotasTotal);
                        break;

                    case "MEDGEN":
                        oTask = new Task(MedGenTotal);
                        break;

                    case "FISIATRIA":
                        oTask = new Task(FiTotal);
                        break;

                    case "TF":
                        TipoTerapia = "Terapia Fisica";
                        oTask = new Task(TerapiasTotal);
                        break;

                    case "TO":
                        TipoTerapia = "Terapia Ocupacional";
                        oTask = new Task(TerapiasTotal);
                        break;

                    case "PSI":
                        TipoTerapia = "Psicologia";
                        oTask = new Task(TerapiasTotal);
                        break;

                    default:
                        panel1.Enabled = false;
                        break;
                }

                if (oTask != null)
                {
                    oTask.Start();
                    await oTask;
                    Hides();
                }
                else
                {
                    MG = new MensajesGeneral()
                    {
                        TipoImagen = 1000,
                        Mensaje = "Seleccione una opcion valida"
                    };
                    
                    MG.ShowDialog();
                }

                Hides();
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        void CreaPanel(int txtAdmition, string txtProfesional, string txtServicio, string txtAseguradora, DateTime txtFecha, string txtJunta)
        {
            try
            {
                PanelCSS panel = new PanelCSS();
                panel.BackColor = Color.White;
                panel.BorderStyle = BorderStyle.None;
                panel.Size = new Size(715, 138);

                flow.Controls.Add(panel);

                //Titulos
                Label labelAdmision = new Label();
                labelAdmision.Text = "Admisión";
                labelAdmision.Font = fuenteNormal;
                labelAdmision.AutoSize = true;
                labelAdmision.ForeColor = Color.Black;
                labelAdmision.Location = new Point(20, 21);
                panel.Controls.Add(labelAdmision);

                Label labelProfesional = new Label();
                labelProfesional.Text = "Profesional";
                labelProfesional.Font = fuenteNormal;
                labelProfesional.AutoSize = true;
                labelProfesional.ForeColor = Color.Black;
                labelProfesional.Location = new Point(20, 46);
                panel.Controls.Add(labelProfesional);

                Label labelServicio = new Label();
                labelServicio.Text = "Servicio";
                labelServicio.Font = fuenteNormal;
                labelServicio.AutoSize = true;
                labelServicio.ForeColor = Color.Black;
                labelServicio.Location = new Point(20, 70);
                panel.Controls.Add(labelServicio);

                Label labelAseguradora = new Label();
                labelAseguradora.Text = "Aseguradora";
                labelAseguradora.Font = fuenteNormal;
                labelAseguradora.AutoSize = true;
                labelAseguradora.ForeColor = Color.Black;
                labelAseguradora.Location = new Point(20, 95);
                panel.Controls.Add(labelAseguradora);

                Label labelFecha = new Label();
                labelFecha.Text = "Fecha";
                labelFecha.Font = fuenteNormal;
                labelFecha.AutoSize = true;
                labelFecha.ForeColor = Color.Black;
                labelFecha.Location = new Point(496, 21);
                panel.Controls.Add(labelFecha);

                //Botones
                Button botonVer = new Button();
                botonVer.BackColor = Color.White;
                botonVer.Cursor = Cursors.Hand;
                botonVer.Image = Properties.Resources.eye;
                botonVer.FlatStyle = FlatStyle.Standard;
                botonVer.Text = "";
                botonVer.Location = new Point(636, 88);
                botonVer.Size = new Size(59, 32);
                panel.Controls.Add(botonVer);

                toolBntSee.SetToolTip(botonVer, "Click para ver el documento");
                toolBntSee.ShowAlways = true;

                //VALORES
                Label labelAdmisionValor = new Label();
                labelAdmisionValor.Text = txtAdmition.ToString();
                labelAdmisionValor.Font = fuenteNegrita;
                labelAdmisionValor.AutoSize = true;
                labelAdmisionValor.ForeColor = Color.Green;
                labelAdmisionValor.Location = new Point(124, 21);
                panel.Controls.Add(labelAdmisionValor);

                Label labelProfesionalValor = new Label();
                labelProfesionalValor.Text = txtProfesional;
                labelProfesionalValor.Font = fuenteNegrita;
                labelProfesionalValor.AutoSize = true;
                labelProfesionalValor.ForeColor = Color.Blue;
                labelProfesionalValor.Location = new Point(124, 46);
                panel.Controls.Add(labelProfesionalValor);

                Label labelServicioValor = new Label();
                labelServicioValor.Text = txtServicio;
                labelServicioValor.Font = fuenteNegrita;
                labelServicioValor.AutoSize = true;
                labelServicioValor.ForeColor = Color.Blue;
                labelServicioValor.Location = new Point(124, 70);
                panel.Controls.Add(labelServicioValor);

                Label labelAseguradoraValor = new Label();
                labelAseguradoraValor.Text = txtAseguradora;
                labelAseguradoraValor.Font = fuenteNegrita;
                labelAseguradoraValor.AutoSize = false;
                labelAseguradoraValor.Size = new Size(516, 24);
                labelAseguradoraValor.ForeColor = Color.Blue;
                labelAseguradoraValor.Location = new Point(124, 95);
                panel.Controls.Add(labelAseguradoraValor);

                Label labelFechaValor = new Label();
                labelFechaValor.Text = txtFecha.ToString("yyyy-MM-dd");
                labelFechaValor.Font = fuenteNegrita;
                labelFechaValor.AutoSize = true;
                labelFechaValor.ForeColor = Color.Red;
                labelFechaValor.Location = new Point(566, 21);
                panel.Controls.Add(labelFechaValor);

                //JUNTAS
                Label labelEvolucionValor = new Label();
                labelEvolucionValor.Text = txtJunta;
                labelEvolucionValor.Font = fuenteNegrita;
                labelEvolucionValor.AutoSize = true;
                labelEvolucionValor.ForeColor = Color.Purple;
                labelEvolucionValor.Location = new Point(304, 21);
                panel.Controls.Add(labelEvolucionValor);                               

                botonVer.Click += (s, e) =>
                {
                    try
                    {
                        Exportar(txtAdmition, txtServicio, txtJunta);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                };

                botonVer.MouseHover += (e, s) =>
                {
                    panel.BackColor = Color.Thistle;
                };
                botonVer.MouseLeave += (e, s) =>
                {
                    panel.BackColor = Color.White;
                };
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }        
        void Exportar(int Admition, string Servicio, string Tipo)
        {
            try
            {
                Comunes.MensajesGeneral MG = new Comunes.MensajesGeneral();

                if (this.SeleccionReporte == "Adjuntos")
                {
                    byte[] _pdf = repoAdjuntos.getPDF(Admition);
                    if (_pdf != null)
                    {
                        byte[] nuevoPDFBytes = JoinImagesToPDF.ConvertirBinarioAPDF(_pdf);
                        JoinImagesToPDF.AbrirPDFDesdeBytes(nuevoPDFBytes);
                    }
                    else
                    {
                        MG.TipoImagen = 1000;
                        MG.Mensaje = "No se logro exportar el documento";
                        MG.ShowDialog();
                    }
                }
                else
                {
                    switch (SeleccionReporte)
                    {
                        case "MG":
                            Historial_Medico_3 H = new Historial_Medico_3(Admition,
                                                                          Servicio,
                                                                          "MG",
                                                                          Convert.ToInt32(Tipo));
                            H.ShowDialog();
                            return;

                        case "FI":
                            Historial_Medico_3 H2 = new Historial_Medico_3(Admition,
                                                                          Servicio,
                                                                          "FI",
                                                                          Convert.ToInt32(Tipo));
                            H2.ShowDialog();
                            return;

                        case "Notas":
                            Medicina.NotasReportPrevio A = new Medicina.NotasReportPrevio();
                            A.Adm_Nota_Export = Admition;
                            A.ShowDialog();
                            break;

                        case "HisMG":
                            var H_HCMG = repoReportes.MedicinaGeneral(Admition);
                            if (H_HCMG == null)
                            {
                                MG.Mensaje = "No se logro exportar, posiblemente halla una falla al exportar o la historia no existe";
                                MG.TipoImagen = 1000;
                                MG.ShowDialog();
                                return;
                            }

                            ConfigForm.GenerarReportViewer("DataSet_HCMG",
                                          "ZamenisHealth.Reportes.RDLC_HCMG.rdlc",
                                          H_HCMG);
                            break;

                        case "HisMGDiametro":
                            var H_HCMGDIA = repoReportes.DiametrosHeridas(Admition);
                            if (H_HCMGDIA == null)
                            {
                                MG.Mensaje = "No se logro exportar, posiblemente halla una falla al exportar o la historia no existe";
                                MG.TipoImagen = 1000;
                                MG.ShowDialog();
                                return;
                            }

                            ConfigForm.GenerarReportViewer("DataSet_HCMG", "ZamenisHealth.Reportes.RDLC_HCMGDIA.rdlc", H_HCMGDIA);

                            break;

                        case "CMan":
                            var H_HCCMAN = repoReportes.CambiosManejo(Admition);
                            if (H_HCCMAN == null)
                            {
                                MG.Mensaje = "No se logro exportar, posiblemente halla una falla al exportar o la historia no existe";
                                MG.TipoImagen = 1000;
                                MG.ShowDialog();
                                return;
                            }

                            ConfigForm.GenerarReportViewer("DataSet_CMAN", "ZamenisHealth.Reportes.RDLC_CMAN.rdlc", H_HCCMAN);

                            break;

                        case "HFI":
                            var H_HCFI = repoReportes.ReporteFisiatria(Admition);
                            if (H_HCFI == null)
                            {
                                MG.Mensaje = "No se logro exportar, posiblemente halla una falla al exportar o la historia no existe";
                                MG.TipoImagen = 1000;
                                MG.ShowDialog();
                                return;
                            }

                            ConfigForm.GenerarReportViewer("DataSet_HCFI", "ZamenisHealth.Reportes.RDLC_HCFI.rdlc", H_HCFI);

                            break;

                        case "PSIQ":
                            var H_HCPSIQ = repoReportes.ReportePsiquiatria(Admition);
                            if (H_HCPSIQ == null)
                            {
                                MG.Mensaje = "No se logro exportar, posiblemente halla una falla al exportar o la historia no existe";
                                MG.TipoImagen = 1000;
                                MG.ShowDialog();
                                return;
                            }

                            ConfigForm.GenerarReportViewer("DataSet_Psiquiatria", "ZamenisHealth.Reportes.RDLC_HCPsiquiatria.rdlc", H_HCPSIQ);

                            break;

                        case "PSIQ2":
                            var H_HCPSIQ2 = repoReportes.ReportePsiquiatria(Admition);
                            if (H_HCPSIQ2 == null)
                            {
                                MG.Mensaje = "No se logro exportar, posiblemente halla una falla al exportar o la historia no existe";
                                MG.TipoImagen = 1000;
                                MG.ShowDialog();
                                return;
                            }

                            ConfigForm.GenerarReportViewer("DataSet_Psiquiatria", "ZamenisHealth.Reportes.RDLC_HCPsiquiatria2.rdlc", H_HCPSIQ2);

                            break;

                        case "PSIQ3":
                            var H_HCPSIQ3 = repoReportes.ReportePsiquiatria(Admition);
                            if (H_HCPSIQ3 == null)
                            {
                                MG.Mensaje = "No se logro exportar, posiblemente halla una falla al exportar o la historia no existe";
                                MG.TipoImagen = 1000;
                                MG.ShowDialog();
                                return;
                            }

                            ConfigForm.GenerarReportViewer("DataSetPsiquiatria", "ZamenisHealth.Reportes.RDLC_HCPsiquiatria3.rdlc", H_HCPSIQ3);

                            break;

                        case "PSI":
                            var H_HCPSI = repoReportes.ReportePsicologia(Admition);
                            if (H_HCPSI == null)
                            {
                                MG.Mensaje = "No se logro exportar, posiblemente halla una falla al exportar o la historia no existe";
                                MG.TipoImagen = 1000;
                                MG.ShowDialog();
                                return;
                            }

                            ConfigForm.GenerarReportViewer("DataSet_HCPSI", "ZamenisHealth.Reportes.RDLC_HCPSI.rdlc", H_HCPSI);

                            break;

                        case "HEVO":
                            var H_HCEVO = repoReportes.ReporteEvoluciones(this.Paciente,
                                                                                      Admition,
                                                                                      Tipo);
                            if (H_HCEVO == null)
                            {
                                MG.Mensaje = "No se logro exportar, posiblemente halla una falla al exportar o la historia no existe";
                                MG.TipoImagen = 1000;
                                MG.ShowDialog();
                                return;
                            }

                            ConfigForm.GenerarReportViewer("DataSet_HCEVO", "ZamenisHealth.Reportes.RDLC_HCEVO.rdlc", H_HCEVO);

                            break;

                        case "HJ2":
                            var H_HCJUN2 = repoReportes.ReporteJuntas(Admition, "Junta2");
                            if (H_HCJUN2 == null)
                            {
                                MG.Mensaje = "No se logro exportar, posiblemente halla una falla al exportar o la historia no existe";
                                MG.TipoImagen = 1000;
                                MG.ShowDialog();
                                return;
                            }

                            ConfigForm.GenerarReportViewer("DataSet_JUNTAS", "ZamenisHealth.Reportes.RDLC_JUNTA2.rdlc", H_HCJUN2);

                            break;

                        case "HJ1":
                            var H_HCJUN1 = repoReportes.ReporteJuntas(Admition, "Junta1");
                            if (H_HCJUN1 == null)
                            {
                                MG.Mensaje = "No se logro exportar, posiblemente halla una falla al exportar o la historia no existe";
                                MG.TipoImagen = 1000;
                                MG.ShowDialog();
                                return;
                            }

                            ConfigForm.GenerarReportViewer("DataSet_JUNTAS", "ZamenisHealth.Reportes.RDLC_JUNTA1.rdlc", H_HCJUN1);

                            break;

                        case "TF":
                            var H_HCTF = repoReportes.ReporteTerapiaFisica(Admition);
                            if (H_HCTF == null)
                            {
                                MG.Mensaje = "No se logro exportar, posiblemente halla una falla al exportar o la historia no existe";
                                MG.TipoImagen = 1000;
                                MG.ShowDialog();
                                return;
                            }

                            ConfigForm.GenerarReportViewer("DataSet_HCTF", "ZamenisHealth.Reportes.RDLC_HCFT.rdlc", H_HCTF);

                            break;

                        case "TO":
                            var H_HCTO = repoReportes.ReporteTerapiaOcupacional(Admition);
                            if (H_HCTO == null)
                            {
                                MG.Mensaje = "No se logro exportar, posiblemente halla una falla al exportar o la historia no existe";
                                MG.TipoImagen = 1000;
                                MG.ShowDialog();
                                return;
                            }

                            ConfigForm.GenerarReportViewer("DataSet_HCTO", "ZamenisHealth.Reportes.RDLC_HCTO.rdlc", H_HCTO);

                            break;

                        case "Adjuntos":
                            return;

                        default:
                            MG.Mensaje = "Seleccione una opcion de reporte valida";
                            MG.TipoImagen = 1000;
                            MG.ShowDialog();
                            return;
                    }

                    if (SeleccionReporte == "HisMG")
                    {
                        DialogResult result = MessageBox.Show("¿Desea generar el reporte de medidas de esta admision?",
                                                      "Zamenis Health - Diametros de Heridas",
                                                      MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                        if (result == DialogResult.Yes)
                        {
                            var Diametros = repoReportes.DiametrosHeridas(Admition);
                            if (Diametros == null)
                            {
                                MG.Mensaje = "No se encontraron reportes de medidas ingresados por el medico";
                                MG.TipoImagen = 3;
                                MG.ShowDialog();
                                return;
                            }

                            ConfigForm.GenerarReportViewer("DataSet_HCMG", "ZamenisHealth.Reportes.RDLC_HCMGDIA.rdlc", Diametros);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        private void dateTimePicker3_ValueChanged(object sender, EventArgs e)
        {
            Filtrar();
        }
        private void dateTimePicker4_ValueChanged(object sender, EventArgs e)
        {
            Filtrar();
        }
        void Filtrar()
        {
            try
            {
                if (!string.IsNullOrEmpty(NodoPrincipal2) || !string.IsNullOrEmpty(TittleSelected2))
                {
                    if (NodoPrincipal2 == "MEDICINA GENERAL")
                    {
                        switch (TittleSelected2)
                        {
                            case "Historia Clinica Individual":
                                SeleccionReporte = "HisMG";
                                BuscarH(SeleccionReporte);
                                panel1.Visible = false;
                                break;

                            case "Medidas de Heridas":
                                SeleccionReporte = "HisMGDiametro";
                                BuscarH("HisMG");
                                panel1.Visible = false;
                                break;

                            case "Historia Clinica Agrupada":
                                TipoNodoGrupal = "MEDGEN";
                                SeleccionReporte = "";
                                BuscarH("HisMG");
                                panel1.Visible = true;
                                break;

                            case "Ordenes Medicas Medicina General":
                                SeleccionReporte = "MG";
                                BuscarOM("MG");
                                panel1.Visible = false;
                                break;
                        }
                    }
                    else if (NodoPrincipal2 == "MEDICINA FISICA Y REHABILITACION")
                    {
                        switch (TittleSelected2)
                        {
                            case "Historia Clinica Individual":
                                SeleccionReporte = "HFI";
                                BuscarH(SeleccionReporte);
                                panel1.Visible = false;
                                break;

                            case "Historia Clinica Agrupada":
                                TipoNodoGrupal = "FISIATRIA";
                                SeleccionReporte = "";
                                BuscarH("HFI");
                                panel1.Visible = true;
                                break;

                            case "Ordenes Medicas Fisiatria":
                                SeleccionReporte = "FI";
                                BuscarOM("FI");
                                panel1.Visible = false;
                                break;
                        }
                    }
                    else if (NodoPrincipal2 == "PSIQUIATRIA")
                    {
                        switch (TittleSelected2)
                        {
                            case "Historia Clinica":
                                SeleccionReporte = "PSIQ";
                                BuscarH(SeleccionReporte);
                                panel1.Visible = false;
                                break;

                            case "Seguimiento":
                                SeleccionReporte = "PSIQ2";
                                BuscarH(SeleccionReporte);
                                panel1.Visible = false;
                                break;

                            case "Evolucion":
                                SeleccionReporte = "PSIQ3";
                                BuscarH(SeleccionReporte);
                                panel1.Visible = false;
                                break;
                        }
                    }
                    else if (NodoPrincipal2 == "ENFERMERIA")
                    {
                        switch (TittleSelected2)
                        {
                            case "Notas de Enfermeria":
                                SeleccionReporte = "Notas";
                                BuscarH(SeleccionReporte);
                                panel1.Visible = false;
                                break;

                            case "Notas Enfermeria Agrupada":
                                TipoNodoGrupal = "NOTAS";
                                SeleccionReporte = "";
                                BuscarH("Notas");
                                panel1.Visible = true;
                                break;

                            case "Cambios de Manejo":
                                SeleccionReporte = "CMan";
                                BuscarH(SeleccionReporte);
                                panel1.Visible = false;
                                break;
                        }
                    }
                    else if (NodoPrincipal2 == "PSICOLOGIA")
                    {
                        switch (TittleSelected2)
                        {
                            case "Historia Clinica Individual":
                                SeleccionReporte = "PSI";
                                BuscarH(SeleccionReporte);
                                panel1.Visible = false;
                                break;

                            case "Historia Clinica Agrupada":
                                TipoNodoGrupal = "PSI";
                                SeleccionReporte = "";
                                BuscarH("HEVO");
                                panel1.Visible = true;
                                break;
                        }
                    }
                    else if (NodoPrincipal2 == "TERAPIA FISICA")
                    {
                        switch (TittleSelected2)
                        {
                            case "Historia Clinica Individual":
                                SeleccionReporte = "TF";
                                BuscarH(SeleccionReporte);
                                panel1.Visible = false;
                                break;

                            case "Historia Clinica Agrupada":
                                TipoNodoGrupal = "TF";
                                SeleccionReporte = "";
                                BuscarH("HEVO");
                                panel1.Visible = true;
                                break;
                        }
                    }
                    else if (NodoPrincipal2 == "TERAPIA OCUPACIONAL")
                    {
                        switch (TittleSelected2)
                        {
                            case "Historia Clinica Individual":
                                SeleccionReporte = "TO";
                                BuscarH(SeleccionReporte);
                                panel1.Visible = false;
                                break;

                            case "Historia Clinica Agrupada":
                                TipoNodoGrupal = "TO";
                                SeleccionReporte = "";
                                BuscarH("HEVO");
                                panel1.Visible = true;
                                break;
                        }
                    }
                    else if (NodoPrincipal2 == "EVOLUCIONES Y JUNTAS MEDICAS")
                    {
                        switch (TittleSelected2)
                        {
                            case "Junta Medica 1":
                                SeleccionReporte = "HJ1";
                                BuscarH(SeleccionReporte);
                                panel1.Visible = false;
                                break;

                            case "Junta Medica 2":
                                SeleccionReporte = "HJ2";
                                BuscarH(SeleccionReporte);
                                panel1.Visible = false;
                                break;

                            case "Evoluciones":
                                SeleccionReporte = "HEVO";
                                BuscarH(SeleccionReporte);
                                panel1.Visible = false;
                                break;
                        }
                    }
                    else if (NodoPrincipal2 == "OTROS SOPORTES")
                    {
                        switch (TittleSelected2)
                        {
                            case "Soportes Adicionales Cargados":
                                SeleccionReporte = "Adjuntos";
                                BuscarAdjuntos();
                                panel1.Visible = false;
                                break;
                        }
                    }
                    else
                    {
                        if (flow != null)
                            flow.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                if (flow != null)
                    flow.Dispose();
            }
        }
    }
}
