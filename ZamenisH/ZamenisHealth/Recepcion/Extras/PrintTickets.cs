using Domain;
using Domain.CXN;
using FormAndControls;
using Microsoft.Reporting.WinForms;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZamenisHealth.Comunes;
using ZXing;
using ZXing.Common;

namespace ZamenisHealth.Recepcion.Extras
{
    public partial class PrintTickets : Forma2
    {
        private static readonly IAgendaC repositorioFechasAgendaa = new MAgendaC();
        private static readonly IConfSystem repositorioConfiguracion = new MConfSystem();
        private static readonly IGenerales repositorioGenerales = new MGenerales();

        private int Admision;
        private MensajesGeneral MG;
        private otrosDatosPacienteHorario _datosAdmision;
        private List<otrosDatosPacienteHorario> _datosAdmisionReport;

        public PrintTickets(int _admision)
        {
            InitializeComponent();         
            this.Admision = _admision;
        }

        private void PrintTickets_Load(object sender, EventArgs e)
        {
            try
            {
                LoadPrinters();
                this.reportViewer1.RefreshReport();

                Titulo.Text = "Ticket de Citas";

                CargarReporte();
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private byte[] GenerarCodigo(string texto)
        {
            var writer = new BarcodeWriter
            {
                Format = BarcodeFormat.CODE_39,
                Options = new EncodingOptions
                {
                    Height = 80,
                    Width = 300,
                    Margin = 2
                }
            };

            Bitmap barcode = writer.Write(texto);

            using (MemoryStream ms = new MemoryStream())
            {
                barcode.Save(ms, ImageFormat.Png);
                return ms.ToArray();
            }
        }
        async void CargarReporte()
        {
            try
            {
                Shows();
                _datosAdmision = new otrosDatosPacienteHorario();
                _datosAdmision = await cargarAdmisionAsync(this.Admision, "'A','P','H'"); 

                if (_datosAdmision != null)
                {
                    //Barra Admision
                    byte[] adm = GenerarCodigo(this.Admision.ToString().Trim());

                    //Barra Documento
                    byte[] doc = GenerarCodigo(_datosAdmision.Pac_IdNum.ToString().Trim());

                    //Otros Datos
                    Byte[] bytes = Convert.FromBase64String(_datosAdmision.Com_Logo); //convierte a bytes
                    _datosAdmision.Logo = bytes;
                    _datosAdmision.Hor_Id = this.Admision;
                    _datosAdmision.ExtraLogo = adm;
                    _datosAdmision.ExtraLogo2 = doc;

                    string urlPQR = repositorioConfiguracion.getListado()["URLPQRSF"];
                    Image PQR = FormAndControls.ClasesExtra.Generales.GenerateQRCode(urlPQR);

                    _datosAdmisionReport = new List<otrosDatosPacienteHorario>();
                    _datosAdmisionReport.Add(new otrosDatosPacienteHorario
                    {
                        Hor_Pac_Fecha_Cita = Convert.ToDateTime(_datosAdmision.Hor_Pac_Fecha_Cita),
                        Hor_Pac_Hora_Cita = Convert.ToDateTime(_datosAdmision.Hor_Pac_Hora_Cita),
                        Hor_Id = Convert.ToInt32(_datosAdmision.Hor_Id),
                        Logo = _datosAdmision.Logo,
                        ExtraLogo = _datosAdmision.ExtraLogo,
                        ExtraLogo2 = _datosAdmision.ExtraLogo2,
                        Pac_IdNum = _datosAdmision.Pac_IdNum,
                        Hor_Imp_Age = _datosAdmision.Hor_Imp_Age,
                        Bod_Responsable = _datosAdmision.Bod_Responsable,
                        Hor_Pac_UsrGraba = _datosAdmision.Hor_Pac_UsrGraba,
                        Com_Nombre = _datosAdmision.Com_Nombre,
                        Com_Identificacion = _datosAdmision.Com_Identificacion,
                        Com_Telefono = _datosAdmision.Com_Telefono,
                        Com_Direccion = _datosAdmision.Com_Direccion,
                        HorObservaTemp = _datosAdmision.HorObservaTemp,
                        ImageEmail = repositorioGenerales.GetBytes(PQR),
                        Hor_Vales = _datosAdmision.Pac_Bonos == "A" ? "Recuerde que debe traer Cuota Moderadora el dia de su cita" : ""
                    });

                    reportViewer1.LocalReport.DataSources.Clear();
                    reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DatosAdmision", this._datosAdmisionReport));
                    reportViewer1.LocalReport.ReportEmbeddedResource = "ZamenisHealth.Reportes.RDLC_TicketCitas.rdlc";
                    reportViewer1.SetDisplayMode(DisplayMode.PrintLayout);
                    reportViewer1.ZoomMode = ZoomMode.Percent;
                    reportViewer1.ZoomPercent = 100;
                    reportViewer1.LocalReport.EnableExternalImages = true;
                    reportViewer1.Visible = true;
                    reportViewer1.RefreshReport();                   
                }
                else
                {
                    MG = new MensajesGeneral();
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "No se logro generar el ticket de citas";
                    MG.ShowDialog();

                    this.Dispose();
                    this.Close();
                }

                Hides();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void LoadPrinters()
        {
            try
            {
                using (PrintDialog printDialog = new PrintDialog())
                {
                    PrinterSettings.StringCollection printers = PrinterSettings.InstalledPrinters;

                    foreach (string printer in printers)
                    {
                        printDialog.PrinterSettings.PrinterName = printer;

                        comboBoxPrinters.Items.Add(printDialog.PrinterSettings.PrinterName);
                    }
                }

                if (comboBoxPrinters.Items.Count > 0)
                {
                    comboBoxPrinters.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }          
        }
        async Task<otrosDatosPacienteHorario> cargarAdmisionAsync(int admision, string estados)
        {
            return await Task.Run(() =>
            {
                return repositorioFechasAgendaa.cargarAdmision(admision, estados);
            });
        }
        void Shows()
        {
            pictureBox2.Visible = true;
        }
        void Hides()
        {
            pictureBox2.Visible = false;
        }
        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                reportViewer1.PrinterSettings.PrinterName = comboBoxPrinters.Text;
                reportViewer1.PrintDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }           
        }
    }
}
