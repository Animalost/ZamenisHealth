using Domain;
using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Windows.Forms;
using ZamenisHealth.Clases;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.Recepcion.Extras
{
    public partial class PrintAgendas : Forma
    {        
        private static readonly IBodegas repoBodegas = new MBodegas();
        private static readonly IPacientes repoPacientes = new MPacientes();

        MensajesGeneral MG;
        string Bod_CXM, Namereport, Namedatasource;
        List<MReportes.PrntAgendas> ExportarXPAC;
        Thread thread;

        private ToolStripButton btnGenerar;

        public PrintAgendas()
        {
            InitializeComponent();
        }

        private void CargarDocumentos()
        {
            List<string> ListaDocs = repoPacientes.ListaDocs();
           
            if (ListaDocs != null)
            {
                foreach (var i in ListaDocs)
                {
                    comboBox3.Items.Add(i);
                }
            }
        }
        private void PrintAgendas_Load(object sender, EventArgs e)
        {
            try
            {
                

                Titulo.Text = "Agendas";
                LogoMain.Image = Properties.Resources.Splash;

                btnGenerar = new ToolStripButton();
                btnGenerar = createToolButton("Generar");
                MenuLateral.Items.Add(btnGenerar);
                btnGenerar.Click += button1_Click;

                DataTable ListaProfesionales =  repoBodegas.Profesionales2("Todos");                

                if (ListaProfesionales == null)
                {
                    MG = new MensajesGeneral();
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "Error cargando profesionales";
                    MG.ShowDialog();
                    return;
                }
                else
                {
                    groupBox1.Location = new System.Drawing.Point(150, 259);
                    groupBox3.Location = new System.Drawing.Point(150, 259);

                    comboBox2.DisplayMember = "Bod_Responsable";
                    comboBox2.DataSource = ListaProfesionales;
                    CargarDocumentos();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string Busca_Med = "";
                ExportarXPAC = new List<MReportes.PrntAgendas>();

                switch (comboBox1.Text)
                {
                    case "Reporte de Citas por Paciente":
                        Busca_Med = label4.Text.TrimEnd(',');

                        
                            ExportarXPAC = MReportes.PrntAgendas.Genera_CitasXPac_Report(textBox1.Text,
                                                                           dateTimePicker1.Value.Date,
                                                                           dateTimePicker2.Value.Date);
                                        
                      
                        if (ExportarXPAC == null) 
                        {
                            MG = new MensajesGeneral();
                            MG.TipoImagen = 1000;
                            MG.Mensaje = "No hay datos para exportar";
                            MG.ShowDialog();
                            return;
                        }

                        Namedatasource = "DataSet_CitasGenerales";
                        Namereport = "ZamenisHealth.Reportes.RDLC_CitasXPAC.rdlc";
                        thread = new Thread(M);
                        thread.SetApartmentState(ApartmentState.STA); // Configura el subproceso en STA
                        thread.Start();
                        break;

                    case "Reporte de Citas Mixto":
                        if (label4.Text == "") { MessageBox.Show("Debe agregar al menos un profesional"); return; }
                        Busca_Med = label4.Text.TrimEnd(',');

                        
                            ExportarXPAC = MReportes.PrntAgendas.Genera_Export_Citas("Mixto",
                                                       dateTimePicker1.Value.Date, Busca_Med);
                        
                       
                        if (ExportarXPAC == null)
                        {
                            MG = new MensajesGeneral();
                            MG.TipoImagen = 1000;
                            MG.Mensaje = "No hay datos para exportar";
                            MG.ShowDialog();
                            return;
                        }

                        Namedatasource = "DataSet_CitasGenerales";
                        Namereport = "ZamenisHealth.Reportes.RDLC_CitasGeneral.rdlc";
                        thread = new Thread(M);
                        thread.SetApartmentState(ApartmentState.STA); // Configura el subproceso en STA
                        thread.Start();
                        break;

                    case "Reporte de Citas de Todos los Medicos":
                        Busca_Med = label4.Text.TrimEnd(',');

                        
                            ExportarXPAC = MReportes.PrntAgendas.Genera_Export_Citas("Total",
                            dateTimePicker1.Value.Date, Busca_Med);
                        
                        
                        if (ExportarXPAC == null)
                        {
                            MG = new MensajesGeneral();
                            MG.TipoImagen = 1000;
                            MG.Mensaje = "No hay datos para exportar";
                            MG.ShowDialog();
                            return;
                        }

                        Namedatasource = "DataSet_CitasGenerales";
                        Namereport = "ZamenisHealth.Reportes.RDLC_CitasGeneral.rdlc";
                        thread = new Thread(M);
                        thread.SetApartmentState(ApartmentState.STA); // Configura el subproceso en STA
                        thread.Start();
                        break;

                    default:
                        MG = new MensajesGeneral();
                        MG.TipoImagen = 1000;
                        MG.Mensaje = "Seleccione tipo de reporte";
                        MG.ShowDialog();
                        break;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        void M()
        {
            try
            {
                ConfigForm.GenerarReportViewer(Namedatasource, Namereport, ExportarXPAC);
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                switch (comboBox1.Text)
                {
                    case "Reporte de Citas por Paciente":
                        label3.Visible = true;
                        dateTimePicker2.Visible = true;
                        label2.Visible = true;
                        dateTimePicker1.Visible = true;
                        groupBox1.Visible = false;
                        groupBox3.Visible = true;
                        btnGenerar.Visible = true;
                        break;

                    case "Reporte de Citas Mixto":
                        label3.Visible = false;
                        dateTimePicker2.Visible = false;
                        label2.Visible = true;
                        dateTimePicker1.Visible = true;
                        groupBox1.Visible = true;
                        groupBox3.Visible = false;
                        btnGenerar.Visible = true;
                        break;

                    case "Reporte de Citas de Todos los Medicos":
                        label3.Visible = false;
                        dateTimePicker2.Visible = false;
                        label2.Visible = true;
                        dateTimePicker1.Visible = true;
                        groupBox1.Visible = false;
                        groupBox3.Visible = false;
                        btnGenerar.Visible = true;
                        break;

                    default:
                        label3.Visible = false;
                        dateTimePicker2.Visible = false;
                        label2.Visible = false;
                        dateTimePicker1.Visible = false;
                        groupBox1.Visible = false;
                        groupBox3.Visible = false;
                        btnGenerar.Visible = false;
                        break;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                label4.Text = label4.Text + Bod_CXM + ",";
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void button4_Click(object sender, EventArgs e)
        {
            Bod_CXM = "";
            label4.Text = "";
        }
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                var IdProf = repoBodegas.getDatosName(comboBox2.Text);
                if (IdProf == null)
                {
                    MG = new MensajesGeneral();
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "Error en este profesional";
                    MG.ShowDialog();
                    this.Dispose();
                    this.Close();
                    return;
                }
                Bod_CXM = IdProf.Bod_Numero.ToString();
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void textBox1_DoubleClick(object sender, EventArgs e)
        {
            Comunes.BuscarPacientes buscarPacientes = new Comunes.BuscarPacientes();
            buscarPacientes.Tipo_Busca_Pac = "ExpAgenda";
            buscarPacientes.ShowDialog();
        }
    }
}
