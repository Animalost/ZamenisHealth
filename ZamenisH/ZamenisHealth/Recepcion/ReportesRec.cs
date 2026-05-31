using Domain;
using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Windows.Forms;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.Recepcion
{
    public partial class ReportesRec : Forma
    {
        private static readonly IPlanos repositorioReportRecepcion = new MPlanos();
        public ReportesRec()
        {
            InitializeComponent();           
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                switch (comboBox1.Text)
                {
                    case "Reporte de Inasistencias":
                        repositorioReportRecepcion.RI(dateTimePicker1.Value, dateTimePicker2.Value);
                        break;

                    case "Reporte de Cancelaciones":
                        repositorioReportRecepcion.CA(dateTimePicker1.Value, dateTimePicker2.Value);
                        break;

                    case "Reporte de Retardos":
                        repositorioReportRecepcion.RE(dateTimePicker1.Value, dateTimePicker2.Value);
                        break;

                    case "Reporte de Pacientes nuevos, controles, reingresos, altas de enfermeria":
                        repositorioReportRecepcion.ANCS(dateTimePicker1.Value, dateTimePicker2.Value);
                        break;

                    default:
                        break;
                }
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
                    case "Reporte de pacientes que no vuelven":
                        label2.Text = "Este reporte muestra los pacientes que no volvieron a curacion hace mas de 15 " +
                            "dias teniendo en cuenta la fecha de hoy.  SOLO CURACIONES.  Ahora seleccione el rango " +
                            "de fechas que desea consultar";
                        break;

                    case "Reporte de Inasistencias":
                        label2.Text = "Este reporte muestra lo que hallan ingresado como razon de los pacientes que no vinieron a su servicio " +
                            "de todas las especialidades, se genera un excel el cual se puede filtrar para ver los datos de interes";
                        break;

                    case "Reporte de Cancelaciones":
                        label2.Text = "Este reporte muestra lo que hallan ingresado como razon de los pacientes que cancelaron su servicio " +
                            "de todas las especialidades, se genera un excel el cual se puede filtrar para ver los datos de interes";
                        break;

                    case "Reporte de Retardos":
                        label2.Text = "Este reporte muestra lo que hallan ingresado como razon de los pacientes que llegaron tarde a su servicio " +
                            "de todas las especialidades, se genera un excel el cual se puede filtrar para ver los datos de interes";
                        break; //

                    case "Reporte de Pacientes nuevos, controles, reingresos, altas de enfermeria":
                        label2.Text = "Este reporte muestra lo que hallan ingresado como razon de cita cuando se asigna con el medico general " +
                            "de todas las especialidades, se genera un excel el cual se puede filtrar para ver los datos de interes";
                        break;

                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void ReportesRec_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Reportes Recepcion";
            LogoMain.Image = Properties.Resources.Splash;

            ToolStripButton btnGrabar = new ToolStripButton();
            btnGrabar = createToolButton("Generar");
            MenuLateral.Items.Add(btnGrabar);
            btnGrabar.Click += button1_Click;
        }
    }
}
