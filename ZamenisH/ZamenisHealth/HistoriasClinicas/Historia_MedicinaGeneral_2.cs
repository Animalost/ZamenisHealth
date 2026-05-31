using Domain;
using Domain.CXN;
using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using ZamenisHealth.Clases;
using ZamenisHealth.Comunes;
using ZamenisHealth.Recepcion.Extras;

namespace ZamenisHealth.HistoriasClinicas
{
    public partial class Historia_MedicinaGeneral_2 : Forma2
    {
        private static readonly IReportes IReports = new MReportes();
        private static readonly IBodegas IBodegas = new MBodegas();
        private static readonly IFirmasDigitales IFirmasDigitales = new MFirmasDigitales();

        private int Admision, PacId;
        private byte[] Firma, FirmaMed;
        private MensajesGeneral MG;

        public Historia_MedicinaGeneral_2(int _admision, int pacid)
        {
            InitializeComponent();
            this.Admision = _admision;
            this.PacId = pacid;
        }

        private void Historia_MedicinaGeneral_2_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Documentos Clínicos";
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";

          /*  if (IReports.ActasMedicos(this.Admision) != null)
            {
                listaReport = new List<CXN_CIA>();
                listaReport = IReports.ActasMedicos(this.Admision);
            }*/

            //FIRMA MEDICO
            CXN_BODEGAS Bod = IBodegas.getDatosUser(Contenedor.UsuarioLogueado);
            FirmaMed = Convert.FromBase64String(Bod.Bod_Firma);           
        }
        Rectangle ObtenerAreaFirma(Bitmap imagen)
        {
            int minX = imagen.Width;
            int minY = imagen.Height;
            int maxX = 0;
            int maxY = 0;

            for (int y = 0; y < imagen.Height; y++)
            {
                for (int x = 0; x < imagen.Width; x++)
                {
                    Color pixel = imagen.GetPixel(x, y);

                    if (pixel.R < 250 || pixel.G < 250 || pixel.B < 250)
                    {
                        if (x < minX) minX = x;
                        if (y < minY) minY = y;
                        if (x > maxX) maxX = x;
                        if (y > maxY) maxY = y;
                    }
                }
            }

            if (maxX <= minX || maxY <= minY)
                return new Rectangle(0, 0, imagen.Width, imagen.Height);

            return new Rectangle(minX, minY, maxX - minX, maxY - minY);
        }
        Bitmap AjustarFirma(Bitmap original)
        {
            Rectangle area = ObtenerAreaFirma(original);

            Bitmap nueva = new Bitmap(area.Width, area.Height);

            using (Graphics g = Graphics.FromImage(nueva))
            {
                g.DrawImage(original,
                    new Rectangle(0, 0, nueva.Width, nueva.Height),
                    area,
                    GraphicsUnit.Pixel);
            }

            return nueva;
        }
        private void Exportar(int scope)
        {
            try
            {
                List<CXN_CIA> f = IReports.ActasMedicos(this.Admision);
                if (f != null)
                {
                    CXN_FIRMASDIGITALES_MED getDoc = IFirmasDigitales.getFirmas_MED(scope);
                    if (getDoc != null)
                    {
                        foreach (CXN_CIA c in f)
                        {
                            c.FechaBase = getDoc.Fecha;
                            c.FirmaMed = getDoc.FirmaMedico;
                            c.FirmaPac = getDoc.FirmaPaciente;
                        }

                        switch (comboBox1.Text)
                        {
                            case "Consentimiento Informado":
                                ConfigForm.GenerarReportViewer("Data_ActasMedicas", "ZamenisHealth.Reportes.RDLC_ActaConsentimiento.rdlc", f);
                                return;

                            case "Acta de Ingreso":
                                ConfigForm.GenerarReportViewer("Data_ActasMedicas", "ZamenisHealth.Reportes.RDLC_ActaIngreso.rdlc", f);
                                break;

                            case "Acta de Salida":
                                ConfigForm.GenerarReportViewer("Data_ActasMedicas", "ZamenisHealth.Reportes.RDLC_ActaEgreso.rdlc", f);
                                break;

                            default:
                                MG = new MensajesGeneral()
                                {
                                    Mensaje = "Documento hecho, pero no se logro exportar, consulte en reportes de documentacion",
                                    TipoImagen = 1000
                                };

                                MG.ShowDialog();
                                break;
                        }
                    }
                    else
                    {
                        MG = new MensajesGeneral()
                        {
                            Mensaje = "Documento hecho, pero no se logro exportar, consulte en reportes de documentacion",
                            TipoImagen = 1000
                        };

                        MG.ShowDialog();
                    }                    
                }
                else
                {
                    MG = new MensajesGeneral()
                    {
                        Mensaje = "Documento hecho, pero no se logro exportar, consulte en reportes de documentacion",
                        TipoImagen = 1000
                    };

                    MG.ShowDialog();
                }

                this.Dispose();
                this.Close();               
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void boton1_Click(object sender, EventArgs e)
        {
            FirmaDigital fD = new FirmaDigital(Admision, PacId, "Historia_MedicinaGeneral_2");
            fD.ShowDialog();
        }
        private void boton2_Click(object sender, EventArgs e)
        {
            Historia_MedicinaGeneral_2_2 H = new Historia_MedicinaGeneral_2_2();
            H.ShowDialog();
        }
        private void boton3_Click(object sender, EventArgs e)
        {
            try
            {
                if (Firma != null)
                {
                    //FIRMA PACIENTE
                    PictureBox pic = new PictureBox();
                    using (MemoryStream ms = new MemoryStream(Firma))
                    {
                        pic.Image = Image.FromStream(ms);
                    }

                    Bitmap firmaOriginal;

                    using (MemoryStream ms2 = new MemoryStream())
                    {
                        pic.Image.Save(ms2, ImageFormat.Png);
                        ms2.Position = 0;
                        firmaOriginal = new Bitmap(ms2);
                    }

                    Bitmap firmaRecortada = AjustarFirma(firmaOriginal);
                    byte[] FirmaFinalPaciente = null;

                    using (MemoryStream ms3 = new MemoryStream())
                    {
                        firmaRecortada.Save(ms3, ImageFormat.Png);
                        ms3.Position = 0;
                        FirmaFinalPaciente = ms3.ToArray();
                    }

                    if (FirmaFinalPaciente == null)
                    {
                        MG = new MensajesGeneral()
                        {
                            Mensaje = "No hay firma paciente para grabar",
                            TipoImagen = 1000
                        };

                        MG.ShowDialog();
                        return;
                    }

                    CXN_FIRMASDIGITALES_MED cXN_FIRMASDIGITALES_MED = new CXN_FIRMASDIGITALES_MED()
                    {
                        Admision = Admision,
                        Paciente = PacId,
                        Fecha = DateTime.Now.Date,
                        FirmaMedico = FirmaMed,
                        FirmaPaciente = FirmaFinalPaciente,
                        Tipo = comboBox1.Text,
                        Usuario = Contenedor.UsuarioLogueado
                    };

                    int grabar = IFirmasDigitales.InsertSign_Med(cXN_FIRMASDIGITALES_MED);
                    if (grabar <= 0)
                    {
                        MG = new MensajesGeneral()
                        {
                            Mensaje = "No se logro grabar la firma",
                            TipoImagen = 1000
                        };

                        MG.ShowDialog();
                    }
                    else
                    {
                        MG = new MensajesGeneral()
                        {
                            Mensaje = "Hecho",
                            TipoImagen = 3
                        };

                        MG.ShowDialog();

                        Exportar(grabar);
                    }
                }
                else
                {
                    MG = new MensajesGeneral()
                    {
                        Mensaje = "No hay firma asociada para grabar el documento",
                        TipoImagen = 1000
                    };

                    MG.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public void setFirma(byte[] firma)
        {
            Firma = firma;
        }
    }
}
