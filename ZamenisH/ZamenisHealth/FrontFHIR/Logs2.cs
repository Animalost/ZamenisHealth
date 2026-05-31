using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.IO;
using System.Text.Json;
using System.Windows.Forms;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.FrontFHIR
{
    public partial class Logs2 : Forma2
    {
        private IFHIR fHIR;
        private int Posision;
        private MensajesGeneral MG;

        public Logs2(int posision)
        {
            InitializeComponent();
            Posision = posision;
            fHIR = new MFHIR();
        }

        private void Logs2_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Visor de Logs IHCE";
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";

            CargarResultado();
        }

        void CargarResultado()
        {
            string getLog = fHIR.GetLogFHIR(Posision);
           
            try
            {
                var jsonObj = JsonSerializer.Deserialize<object>(getLog);

                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };

                string jsonFormateado = JsonSerializer.Serialize(jsonObj, options);

                richTextBox1.Text = jsonFormateado;
            }
            catch
            {
                richTextBox1.Text = getLog;
            }
        }

        private void boton1_Click(object sender, EventArgs e)
        {
            try
            {
                FileStream Querys = new FileStream("C:/Cxn/Reportes/Log_RDA.json", FileMode.Append, FileAccess.Write);
                StreamWriter Escriba = new StreamWriter(Querys);

                Escriba.Write(richTextBox1.Text);
                Escriba.WriteLine();
                Escriba.Flush();
                Escriba.Close();

                MG = new MensajesGeneral()
                {
                    Mensaje = "Archivo guardado en C:/Cxn/Reportes/Log_RDA.json",
                    TipoImagen = 3
                };

                MG.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
