using Domain;
using Persistence;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Compression;
using ZamenisHealth.Clases;
using System.Drawing;
using FormAndControls;

namespace ZamenisHealth.Comunes
{
    public partial class CompresorZIP : Forma2
    {
        public CompresorZIP()
        {
            InitializeComponent();

            ConfigForm.GraficarControl(button1, 1, Color.Red);
            ConfigForm.GraficarControl(button1, 2, Color.Red);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.InitialDirectory = @"C:\";
                    //openFileDialog.Filter = "Archivos PDF (*.pdf)|*.pdf"; 

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        string folderPath = Path.GetDirectoryName(openFileDialog.FileName);
                        textBox1.Text = folderPath;
                    }
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        async void button1_Click(object sender, EventArgs e)
        {
            Task oTask = null;
            oTask = new Task(Comprimir);

            if (oTask != null)
            {
                button1.Enabled = false;
                button2.Enabled = false;
                Shows();
                oTask.Start();
                await oTask;
                Hides();
                button1.Enabled = true;
                button2.Enabled = true;
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

        void Comprimir()
        {
            try
            {
                string[] carpetas = Directory.GetDirectories(textBox1.Text);

                foreach (string carpeta in carpetas)
                {
                    string nombreCarpeta = Path.GetFileName(carpeta);
                    string zipPath = @"C:\CXN\Reportes\" + nombreCarpeta + ".zip";
                    ZipFile.CreateFromDirectory(carpeta, zipPath);
                }                

                MensajesGeneral MG = new MensajesGeneral();
                MG.TipoImagen = 3;
                MG.Mensaje = "Se han comprimido los archivos";
                MG.ShowDialog();
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void CompresorZIP_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Compresor ZIP"; SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";
        }
    }
}
