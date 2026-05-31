using FormAndControls;

using Persistence;

using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace ZamenisHealth.Comunes.Extras
{
    public partial class Manuales : Forma2
    {
        public Manuales()
        {
            InitializeComponent();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                AbrirPDF("Manual2");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al abrir el archivo PDF: {ex.Message}");
            }
        }

        void AbrirPDF(string Manual)
        {
            try
            {
                byte[] existFile = null;

                switch (Manual)
                {
                    case "Manual":
                        existFile = Properties.Resources.Manual_Zamenis_Health;
                        break;

                    case "Manual2":
                        existFile = Properties.Resources.Manual_V1_Zamenis_Health;
                        break;

                    case "Juntas":
                        existFile = Properties.Resources.Manual_Juntas_Medicas;
                        break;

                    case "IngresosMG":
                        existFile = Properties.Resources.Manual_Ingresos_Salidas_Medicina_General;
                        break;

                    case "SesionesCU":
                        existFile = Properties.Resources.Actualizacion_Citas_con_Sesiones;
                        break;

                    case "CargaPDF":
                        existFile = Properties.Resources.Manual_Carga_de_Archivos_PDF_al_sistema;
                        break;

                    case "Consentimientos":
                        existFile = Properties.Resources.Consentimientos;
                        break;

                    default:
                        MessageBox.Show("Manual no disponible");
                        return;
                }

                // Crear un MemoryStream con el contenido del PDF desde los recursos
                using (MemoryStream ms = new MemoryStream(existFile))
                {
                    // Crear un archivo temporal
                    string tempPath = Path.Combine(Path.GetTempPath(), "Archivo.pdf");

                    // Escribir el contenido del MemoryStream en el archivo temporal
                    using (FileStream fs = new FileStream(tempPath, FileMode.Create, FileAccess.Write))
                    {
                        ms.WriteTo(fs);
                    }

                    // Abrir el archivo temporal con la aplicación predeterminada
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = tempPath,
                        UseShellExecute = true // Asegura que se abra con la aplicación predeterminada
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al abrir el archivo PDF: {ex.Message}");
            }
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                AbrirPDF("Manual");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al abrir el archivo PDF: {ex.Message}");
            }
        }

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                AbrirPDF("Juntas");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al abrir el archivo PDF: {ex.Message}");
            }
        }

        private void linkLabel4_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                AbrirPDF("IngresosMG");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al abrir el archivo PDF: {ex.Message}");
            }
        }

        private void linkLabel5_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                AbrirPDF("SesionesCU");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al abrir el archivo PDF: {ex.Message}");
            }
        }

        private void linkLabel6_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                AbrirPDF("CargaPDF");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al abrir el archivo PDF: {ex.Message}");
            }
        }

        private void Manuales_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Manuales del Sistema";
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";
        }

        private void linkLabel7_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                AbrirPDF("Consentimientos");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al abrir el archivo PDF: {ex.Message}");
            }
        }
    }
}
