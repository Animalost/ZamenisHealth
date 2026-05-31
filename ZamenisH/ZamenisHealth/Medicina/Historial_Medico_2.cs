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
using System.Data;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZamenisHealth.Clases;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.Medicina
{
    public partial class Historial_Medico_2 : Forma
    {
        private static readonly IReportes repoReportes = new MReportes();
        private static readonly IOrdenes repoOrdenes = new MOrdenes();
        private static readonly IAdjuntos repoAdjuntos = new MAdjuntos();

        DataTable dt = new DataTable();
        DataColumn POS;
        DataColumn Admision;
        DataColumn Pacientes;
        DataColumn Profesional;
        DataColumn Fecha;
        DataColumn Tipo;
        DataColumn Cia;
        DataColumn Clasificacion;

        private int Paciente;
        private string SeleccionReporte;
        private string TipoTerapia;
        List<HCMG> H_HCMGHC;
        List<CXN_HCRADIOLOGIA> cXN_HCRADIOLOGIA;
        List<ReportHCFI> H_HCFICompleto;
        List<ReportEVO> H_HCEVO;
        
        public Historial_Medico_2(int paciente)
        {
            InitializeComponent();
            this.Paciente = paciente;
        }
        private void btnZamenis1_ButtonClick(object sender, EventArgs e)
        {
            this.Dispose();
            this.Close();
        }

        private void Historial_Medico_2_Load(object sender, EventArgs e)
        {
            try
            {
                listBox1.Items.Add("Historia Clinica Medicina General");  //0
                listBox1.Items.Add("Medidas de Heridas"); //1
                listBox1.Items.Add("Historia Clinica Fisiatria");//2
                listBox1.Items.Add("Historia Clinica Terapia Fisica");//3
                listBox1.Items.Add("Historia Clinica Terapia Ocupacional");//4
                listBox1.Items.Add("Historia Clinica Psicologia"); //5
                listBox1.Items.Add("Historia Clinica Radiologia");//6
                listBox1.Items.Add("------------------------------------------");//7
                listBox1.Items.Add("Ordenes Medicina General");//8
                listBox1.Items.Add("Ordenes Fisiatria");//9
                listBox1.Items.Add("Ordenes Radiologia");//10
                listBox1.Items.Add("------------------------------------------");//11
                listBox1.Items.Add("Soportes Adjuntos");//12
                listBox1.Items.Add("------------------------------------------");//13
                listBox1.Items.Add("Notas de Enfermeria");//14
                listBox1.Items.Add("Cambios de Manejo Enfermeria");//15
                listBox1.Items.Add("Evoluciones Terapias");//16
                listBox1.Items.Add("Junta Medica 1");//17
                listBox1.Items.Add("Junta Medica 2");//18
                listBox1.Items.Add("------------------------------------------");//19
                listBox1.Items.Add("Compilado Notas de Enfermeria");//20
                listBox1.Items.Add("Compilado Historia Clinica Medicina General");//21
                listBox1.Items.Add("Compilado Historia Clinica Fisiatria");//22
                listBox1.Items.Add("Compilado Historia Clinica Terapia Fisica");//23
                listBox1.Items.Add("Compilado Historia Clinica Terapia Ocupacional");//24
                listBox1.Items.Add("Compilado Historia Clinica Psicologia");//25
                listBox1.Items.Add("Compilado Historia Clinica Radiologia");//26

                Titulo.Text = "Registros Medicos";
                LogoMain.Image = Properties.Resources.Splash;

                listBox1.ClearSelected();            
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

        void BuscarAdjuntos()
        {
            try
            {
                EncabezadosAdjuntos();

                List<CXN_HORARIO> H = repoAdjuntos.getAdjuntos(this.Paciente);
                if (H != null)
                {
                    int Contador = 1;

                    foreach (var h in H)
                    {
                        DataRow row = dt.NewRow();

                        row["POS"] = Contador;
                        row["Admision"] = Convert.ToInt32(h.Hor_Id);
                        row["Paciente"] = h.Hor_Imp_Age.ToString();
                        row["Profesional"] = h.Hor_Observacion.ToString();
                        row["Tipo"] = h.Com_Tipo_Doc.ToString();
                        row["Fecha"] = Convert.ToDateTime(h.Hor_Pac_Fecha_Cita).ToString(Conexion.ConectionDictionary["Format_Fecha"]);                      

                        dt.Rows.Add(row);
                        dt.AcceptChanges();

                        Contador = Contador + 1;
                    }

                    Contador = 1;
                    Estilos();
                }
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

        void RadiologiaTotal()
        {
            try
            {
                MensajesGeneral MG = new MensajesGeneral();

                cXN_HCRADIOLOGIA = new List<CXN_HCRADIOLOGIA>();
                cXN_HCRADIOLOGIA = repoReportes.ReporteRadiologiaCompleto(this.Paciente,
                                                                       dateTimePicker1.Value,
                                                                       dateTimePicker2.Value,
                                                                       Comunes.Contenedor.UsuarioLogueado);
                if (cXN_HCRADIOLOGIA == null)
                {
                    MG.Mensaje = "No se logro exportar, posiblemente halla una falla al exportar o la historia no existe";
                    MG.TipoImagen = 1000;
                    MG.ShowDialog();
                    return;
                }

                Thread thread = new Thread(ExportRvSTARA);
                thread.SetApartmentState(ApartmentState.STA); // Configura el subproceso en STA
                thread.Start();
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        void FiTotal()
        {
            try
            {
                MensajesGeneral MG = new MensajesGeneral();

                H_HCFICompleto = new List<ReportHCFI>();
                H_HCFICompleto = repoReportes.ReportefisiatriaCompleto(this.Paciente,
                                                                                        dateTimePicker1.Value,
                                                                                        dateTimePicker2.Value,
                                                                                        Comunes.Contenedor.UsuarioLogueado);
                if (H_HCFICompleto == null)
                {
                    MG.Mensaje = "No se logro exportar, posiblemente halla una falla al exportar o la historia no existe";
                    MG.TipoImagen = 1000;
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

        void ExportRvSTARA()//Historia radiologia grupada aqui
        {
            Form F = new Form();
            ReportViewer R = new ReportViewer();

            R.LocalReport.DataSources.Clear();
            R.LocalReport.DataSources.Add(new ReportDataSource("DataSetCompletoRa", cXN_HCRADIOLOGIA));
            R.LocalReport.ReportEmbeddedResource = "ZamenisHealth.Reportes.RDLC_RADIOLOGIACOMPLETO.rdlc";
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
                MensajesGeneral MG = new MensajesGeneral();

                H_HCMGHC = new List<HCMG>();
                H_HCMGHC = repoReportes.MedicinaGeneralCompleto(this.Paciente,
                                                                             dateTimePicker1.Value,
                                                                             dateTimePicker2.Value,
                                                                             Comunes.Contenedor.UsuarioLogueado);
                if (H_HCMGHC == null)
                {
                    MG.Mensaje = "No se logro exportar, posiblemente halla una falla al exportar o la historia no existe";
                    MG.TipoImagen = 1000;
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
            try
            {
                Comunes.MensajesGeneral MG = new Comunes.MensajesGeneral();

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

                        if (Preferencias.CuracionesCORE == "A" && getRDLCMasivo[Contador][0].listaMedidas != null)
                        {
                            R.LocalReport.DataSources.Add(new ReportDataSource("DataSet_NotasMed", getRDLCMasivo[Contador][0].listaMedidas));
                            R.LocalReport.ReportEmbeddedResource = "ZamenisHealth.Reportes.RDLC_NotasCore.rdlc";
                        }
                        else
                        {
                            R.LocalReport.ReportEmbeddedResource = "ZamenisHealth.Reportes.RDLC_Notas.rdlc";
                        }                        
                        
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

        void EncabezadosEvo()
        {
            dt = new DataTable();
            POS = dt.Columns.Add("POS", typeof(int));
            Admision = dt.Columns.Add("Admision", typeof(int));
            Pacientes = dt.Columns.Add("Paciente", typeof(string));
            Profesional = dt.Columns.Add("Profesional", typeof(string));
            Fecha = dt.Columns.Add("Fecha", typeof(DateTime));
            Tipo = dt.Columns.Add("Tipo", typeof(string));
        }

        void Encabezados()
        {
            dt = new DataTable();
            POS = dt.Columns.Add("POS", typeof(int));
            Admision = dt.Columns.Add("Admision", typeof(int));
            Pacientes = dt.Columns.Add("Paciente", typeof(string));
            Profesional = dt.Columns.Add("Profesional", typeof(string));
            Fecha = dt.Columns.Add("Fecha", typeof(DateTime));
        }

        void EncabezadosAdjuntos()
        {
            dt = new DataTable();
            POS = dt.Columns.Add("POS", typeof(int));
            Admision = dt.Columns.Add("Admision", typeof(int));
            Pacientes = dt.Columns.Add("Paciente", typeof(string));
            Profesional = dt.Columns.Add("Profesional", typeof(string));
            Tipo = dt.Columns.Add("Tipo", typeof(string));
            Fecha = dt.Columns.Add("Fecha", typeof(DateTime));
        }

        void EncabezadosOM()
        {
            dt = new DataTable();
            POS = dt.Columns.Add("POS", typeof(int));
            Admision = dt.Columns.Add("Admision", typeof(int));
            Pacientes = dt.Columns.Add("Paciente", typeof(string));
            Profesional = dt.Columns.Add("Profesional", typeof(string));
            Fecha = dt.Columns.Add("Fecha", typeof(DateTime));
            Tipo = dt.Columns.Add("Tipo", typeof(string));
            Cia = dt.Columns.Add("Cia", typeof(string));
            Clasificacion = dt.Columns.Add("Clasificacion", typeof(string));
        }

        void Estilos()
        {
            dataGridView1.EnableHeadersVisualStyles = false;
            dataGridView1.ScrollBars = ScrollBars.Both;

            dataGridView1.DataSource = dt;

            dataGridView1.Columns["Admision"].Width = 110;
            dataGridView1.Columns["Paciente"].Width = 350;
            dataGridView1.Columns["Profesional"].Width = 350;
            dataGridView1.Columns["Fecha"].Width = 115;
            dataGridView1.Font = new Font("Arial", 11);

            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font(dataGridView1.Font, FontStyle.Bold);
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Arial", 10, FontStyle.Bold);
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#bfdbff");
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.Blue;

            dataGridView1.Columns["Admision"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns["Paciente"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns["Profesional"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Columns["Fecha"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dataGridView1.Columns["Admision"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns["Paciente"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns["Profesional"].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns["Fecha"].SortMode = DataGridViewColumnSortMode.NotSortable;

            dataGridView1.Columns["POS"].Visible = false;

            if (SeleccionReporte == "MG" || SeleccionReporte == "FI")
            {
                dataGridView1.Columns["Cia"].Visible = false;
                dataGridView1.Columns["Clasificacion"].Width = 200;
            }

            //DataGridViewCellStyle style = new DataGridViewCellStyle();

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                int Numero = Convert.ToInt32(row.Cells["POS"].Value.ToString());

                if ((Numero % 2) == 0)
                {
                    row.DefaultCellStyle.BackColor = Color.Aquamarine;
                }
                else
                {
                    row.DefaultCellStyle.BackColor = Color.MediumAquamarine;
                }
            }
        }

        void BuscarOM(string TipoEs)
        {
            try
            {
                var getOM = repoOrdenes.getOrdenes(this.Paciente, TipoEs);
                if (getOM != null)
                {
                    EncabezadosOM();

                    int Contador = 1;

                    foreach (var i in getOM)
                    {
                        DataRow row = dt.NewRow();

                        row["POS"] = Contador;
                        row["Admision"] = Convert.ToInt32(i.OM_Num);
                        row["Paciente"] = i.OM_Detalle.ToString();
                        row["Profesional"] = i.OM_Prof.ToString();
                        row["Fecha"] = Convert.ToDateTime(i.OM_Fecha).ToString(Conexion.ConectionDictionary["Format_Fecha"]);
                        row["Tipo"] = i.OM_Tipo.ToString();
                        row["Cia"] = i.OM_Cia.ToString();
                        row["Clasificacion"] = i.OM_Clasificacion.ToString();

                        dt.Rows.Add(row);
                        dt.AcceptChanges();

                        Contador = Contador + 1;
                    }

                    Contador = 1;
                    Estilos();
                }
                else
                {
                    dataGridView1.DataSource = null;
                    EncabezadosOM();                    
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }        

        private void BuscarH(string Tipo)
        {
            try
            {
                Comunes.MensajesGeneral MG = new Comunes.MensajesGeneral();

                if (Tipo == "RADG")
                {
                    Tipo = "RA";
                }

                var getHistorias = repoReportes.BuscarHistorias(this.Paciente, Tipo);
                if (getHistorias != null)
                {
                    int Contador = 1;

                    if (Tipo == "HEVO")
                    {
                        EncabezadosEvo();

                        foreach (var i in getHistorias)
                        {
                            DataRow row = dt.NewRow();

                            row["POS"] = Contador;
                            row["Admision"] = Convert.ToInt32(i.Hor_Id);
                            row["Paciente"] = i.Hor_Imp_Age.ToString();
                            row["Profesional"] = i.Hor_Observacion.ToString();
                            row["Fecha"] = Convert.ToDateTime(i.Hor_Pac_Fecha_Cita).ToString(Conexion.ConectionDictionary["Format_Fecha"]);
                            row["Tipo"] = i.Hor_Pac_Tipo_Serv.ToString();

                            dt.Rows.Add(row);
                            dt.AcceptChanges();

                            Contador = Contador + 1;
                        }

                        Contador = 1;
                        Estilos();                        
                    }
                    else
                    {
                        Encabezados();

                        foreach (var i in getHistorias)
                        {
                            DataRow row = dt.NewRow();

                            row["POS"] = Contador;
                            row["Admision"] = Convert.ToInt32(i.Hor_Id);
                            row["Paciente"] = i.Hor_Imp_Age.ToString();
                            row["Profesional"] = i.Hor_Observacion.ToString();
                            row["Fecha"] = Convert.ToDateTime(i.Hor_Pac_Fecha_Cita).ToString(Conexion.ConectionDictionary["Format_Fecha"]);

                            dt.Rows.Add(row);
                            dt.AcceptChanges();

                            Contador = Contador + 1;
                        }

                        Contador = 1;
                        Estilos();                        
                    }

                    dataGridView1.ClearSelection();
                    panel1.Enabled = true;
                }
                else
                {
                    panel1.Enabled = false;
                    dataGridView1.DataSource = null;
                    Encabezados();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        
        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                Comunes.MensajesGeneral MG = new Comunes.MensajesGeneral();

                if (this.SeleccionReporte == "Adjuntos")
                {
                    int PosAd = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString());

                    //Exportar PDF desde Binario
                    if (e.ColumnIndex == 0 || e.ColumnIndex == 1 || e.ColumnIndex == 2 || e.ColumnIndex == 4 || e.ColumnIndex == 5)
                    {
                        byte[] _pdf = repoAdjuntos.getPDF(PosAd);
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
                    //Observacion
                    if (e.ColumnIndex == 3)
                    {
                        MG.TipoImagen = 0;
                        MG.Mensaje = dataGridView1.Rows[e.RowIndex].Cells[3].Value.ToString();
                        MG.ShowDialog();
                    }

                    return;
                }

                if (e.ColumnIndex == 0 || e.ColumnIndex == 1 || e.ColumnIndex == 2 || e.ColumnIndex == 3 ||
                    e.ColumnIndex == 4) 
                {
                    int Admition = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString());
                    //int _cia = 0;

                    switch (SeleccionReporte)
                    {
                        case "MG":
                            Historial_Medico_3 H = new Historial_Medico_3(Admition, 
                                                                          dataGridView1.Rows[e.RowIndex].Cells[5].Value.ToString(), 
                                                                          "MG",
                                                                           Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[6].Value.ToString()));
                            H.ShowDialog();
                            return;

                        case "FI":
                            Historial_Medico_3 H2 = new Historial_Medico_3(Admition,
                                                                          dataGridView1.Rows[e.RowIndex].Cells[5].Value.ToString(),
                                                                          "FI",
                                                                           Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[6].Value.ToString()));
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

                            ConfigForm.GenerarReportViewer("DataSet_HCMG",
                                          "ZamenisHealth.Reportes.RDLC_HCMGDIA.rdlc",
                                          H_HCMGDIA);

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

                            ConfigForm.GenerarReportViewer("DataSet_CMAN",
                                         "ZamenisHealth.Reportes.RDLC_CMAN.rdlc",
                                         H_HCCMAN);

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

                            ConfigForm.GenerarReportViewer("DataSet_HCFI",
                                       "ZamenisHealth.Reportes.RDLC_HCFI.rdlc",
                                       H_HCFI);

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

                            ConfigForm.GenerarReportViewer("DataSet_HCPSI",
                                       "ZamenisHealth.Reportes.RDLC_HCPSI.rdlc",
                                       H_HCPSI);

                            break;

                        case "RA":
                            var H_HCRA = repoReportes.RadiologiaReport(Admition);
                            if (H_HCRA == null)
                            {
                                MG.Mensaje = "No se logro exportar, posiblemente halla una falla al exportar o la historia no existe";
                                MG.TipoImagen = 1000;
                                MG.ShowDialog();
                                return;
                            }

                            ConfigForm.GenerarReportViewer("DataSetRadiologia",
                                          "ZamenisHealth.Reportes.RDLC_HCRADIOLOGIA.rdlc",
                                          H_HCRA);
                            return;

                        case "RADOM":
                            Historial_Medico_3 RADOM = new Historial_Medico_3(Admition,
                                                                           dataGridView1.Rows[e.RowIndex].Cells[5].Value.ToString(),
                                                                           "RA",
                                                                            Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[6].Value.ToString()));
                            RADOM.ShowDialog();
                            return;

                        case "HEVO":
                            var H_HCEVO = repoReportes.ReporteEvoluciones(this.Paciente,
                                                                                      Admition,
                                                                                      dataGridView1.Rows[e.RowIndex].Cells[5].Value.ToString());
                            if (H_HCEVO == null)
                            {
                                MG.Mensaje = "No se logro exportar, posiblemente halla una falla al exportar o la historia no existe";
                                MG.TipoImagen = 1000;
                                MG.ShowDialog();
                                return;
                            }

                            ConfigForm.GenerarReportViewer("DataSet_HCEVO",
                                      "ZamenisHealth.Reportes.RDLC_HCEVO.rdlc",
                                      H_HCEVO);

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

                            ConfigForm.GenerarReportViewer("DataSet_JUNTAS",
                                   "ZamenisHealth.Reportes.RDLC_JUNTA2.rdlc",
                                   H_HCJUN2);

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

                            ConfigForm.GenerarReportViewer("DataSet_JUNTAS",
                                   "ZamenisHealth.Reportes.RDLC_JUNTA1.rdlc",
                                   H_HCJUN1);

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

                            ConfigForm.GenerarReportViewer("DataSet_HCTF",
                                  "ZamenisHealth.Reportes.RDLC_HCFT.rdlc",
                                  H_HCTF);

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

                            ConfigForm.GenerarReportViewer("DataSet_HCTO",
                                 "ZamenisHealth.Reportes.RDLC_HCTO.rdlc",
                                 H_HCTO);

                            break;

                        case "Adjuntos":

                            return;

                        default:
                            MG.Mensaje = "Seleccione una opcion de reporte valida";
                            MG.TipoImagen = 1000;
                            MG.ShowDialog();
                            return;
                    }

                    /*if (SeleccionReporte != "Notas")
                    {
                        maestro.Universal.SetDisplayMode(DisplayMode.PrintLayout);
                        maestro.Universal.ZoomMode = ZoomMode.Percent;
                        maestro.Universal.ZoomPercent = 100;
                        maestro.Universal.LocalReport.EnableExternalImages = true;
                        maestro.Universal.Font = new Font("Arial", 8);
                        maestro.Universal.RefreshReport();
                        maestro.Universal.Visible = true;
                        maestro.Universal.Dock = System.Windows.Forms.DockStyle.Fill;
                        maestro.ShowDialog();
                    }*/

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

                            ConfigForm.GenerarReportViewer("DataSet_HCMG",
                                 "ZamenisHealth.Reportes.RDLC_HCMGDIA.rdlc",
                                 Diametros);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                Verificador_Paciente();

                switch (listBox1.SelectedIndex)
                {
                    case 0:
                        SeleccionReporte = "HisMG";
                        BuscarH(SeleccionReporte);
                        panel1.Visible = false;
                        break;

                    case 1:
                        SeleccionReporte = "HisMGDiametro";
                        BuscarH("HisMG");
                        panel1.Visible = false;
                        break;

                    case 2:
                        SeleccionReporte = "HFI";
                        BuscarH(SeleccionReporte);
                        panel1.Visible = false;
                        break;

                    case 3:
                        SeleccionReporte = "TF";
                        BuscarH(SeleccionReporte);
                        panel1.Visible = false;
                        break;

                    case 4:
                        SeleccionReporte = "TO";
                        BuscarH(SeleccionReporte);
                        panel1.Visible = false;
                        break;

                    case 5:
                        SeleccionReporte = "PSI";
                        BuscarH(SeleccionReporte);
                        panel1.Visible = false;
                        break;

                    case 6:
                        SeleccionReporte = "RA";
                        BuscarH(SeleccionReporte);
                        panel1.Visible = false;
                        break;

                    case 8: //OM Gen
                        SeleccionReporte = "MG";
                        BuscarOM("MG");
                        panel1.Visible = false;
                        break;

                    case 9: //OM Fisi
                        SeleccionReporte = "FI";
                        BuscarOM("FI");
                        panel1.Visible = false;
                        break;

                    case 10: //OM RAD
                        SeleccionReporte = "RADOM";
                        BuscarOM("RA");
                        panel1.Visible = false;
                        break;

                    case 12: //Adjuntos
                        SeleccionReporte = "Adjuntos";
                        BuscarAdjuntos();
                        panel1.Visible = false;
                        break;

                    case 14:
                        SeleccionReporte = "Notas";
                        BuscarH(SeleccionReporte);
                        panel1.Visible = false;
                        break;

                    case 15:
                        SeleccionReporte = "CMan";
                        BuscarH(SeleccionReporte);
                        panel1.Visible = false;
                        break;

                    case 16:
                        SeleccionReporte = "HEVO";
                        BuscarH(SeleccionReporte);
                        panel1.Visible = false;
                        break;

                    case 17:
                        SeleccionReporte = "HJ1";
                        BuscarH(SeleccionReporte);
                        panel1.Visible = false;
                        break;

                    case 18:
                        SeleccionReporte = "HJ2";
                        BuscarH(SeleccionReporte);
                        panel1.Visible = false;
                        break;

                    case 20:
                        SeleccionReporte = "";
                        BuscarH("Notas");
                        panel1.Visible = true;
                        break;

                    case 21:
                        SeleccionReporte = "";
                        BuscarH("HisMG");
                        panel1.Visible = true;
                        break;

                    case 22:
                        SeleccionReporte = "";
                        BuscarH("HFI");
                        panel1.Visible = true;
                        break;

                    case 23:
                        SeleccionReporte = "";
                        BuscarH("HEVO");
                        panel1.Visible = true;
                        break;

                    case 24:
                        SeleccionReporte = "";
                        BuscarH("HEVO");
                        panel1.Visible = true;
                        break;

                    case 25:
                        SeleccionReporte = "";
                        BuscarH("HEVO");
                        panel1.Visible = true;
                        break;

                    case 26:
                        SeleccionReporte = "";
                        BuscarH("RADG");
                        panel1.Visible = true;
                        break;

                    default:
                        listBox1.ClearSelected();
                        panel1.Visible = false;
                        break;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            try
            {
                Task oTask = null;
                Shows();

                switch (listBox1.SelectedIndex)
                {
                    case 20:
                        oTask = new Task(NotasTotal);
                        break;

                    case 21:
                        oTask = new Task(MedGenTotal);
                        break;

                    case 22:
                        oTask = new Task(FiTotal);
                        break;

                    case 23:
                        TipoTerapia = "Terapia Fisica";
                        oTask = new Task(TerapiasTotal);
                        break;

                    case 24:
                        TipoTerapia = "Terapia Ocupacional";
                        oTask = new Task(TerapiasTotal);
                        break;

                    case 25:
                        TipoTerapia = "Psicologia";
                        oTask = new Task(TerapiasTotal);
                        break;

                    case 26:
                        TipoTerapia = "Radiologia";
                        oTask = new Task(RadiologiaTotal);
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
                    MensajesGeneral MG = new MensajesGeneral();
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "Seleccione una opcion valida";
                    MG.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
    }
}
