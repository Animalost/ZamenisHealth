using Domain;
using Domain.CXN;
using Domain.CXN_ADJUNTOS;
using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using Persistence.CXN_ADJUNTOS.Interfaces;
using Persistence.CXN_ADJUNTOS.Metodos;
using System;
using System.IO;
using System.Windows.Forms;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.Medicina
{
    public partial class UploadHistory : Forma
    {
        private static readonly IPacientes repoPac = new MPacientes();
        private static readonly IAdjuntos repoAdjuntos = new MAdjuntos();

        private MensajesGeneral MG;
        private int CodePac;

        public UploadHistory()
        {
            InitializeComponent();
            
        }

        private void textBox1_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                Comunes.BuscarPacientes P = new BuscarPacientes("UploadPDF");
                P.Show();
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        public void setSelection(string Doc)
        {
            textBox1.Text = Doc.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (textBox1.Text == "")
                {
                    MG = new MensajesGeneral();
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "";
                    MG.ShowDialog();
                    return;
                }

                CXN_PACIENTES P = repoPac.LlamarPacienteNumDoc(textBox1.Text);
                if (P != null)
                {
                    this.CodePac = P.Pac_Id;
                    label3.Text = P.Pac_PrimerA + " " + P.Pac_SegundoA + " " + P.Pac_PrimerN + " " + P.Pac_SegundoN;
                }
                else
                {
                    this.CodePac = 0;
                    label3.Text = "Paciente No Encontrado";
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        label4.Text = openFileDialog.FileName;
                    }
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            label4.Text = "";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.CodePac <= 0)
                {
                    MG = new MensajesGeneral();
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "No hay paciente seleccionado";
                    MG.ShowDialog();
                    return;
                }

                if (label4.Text == "")
                {
                    MG = new MensajesGeneral();
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "No hay archivo seleccionado";
                    MG.ShowDialog();
                    return;
                }

                if (comboBox1.SelectedIndex == 0)
                {
                    MG = new MensajesGeneral();
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "No hay tipo de historia seleccionada";
                    MG.ShowDialog();
                    return;
                }

                if (comboBox2.SelectedIndex == 0)
                {
                    MG = new MensajesGeneral();
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "No hay clase de documento seleccionado";
                    MG.ShowDialog();
                    return;
                }

                string tHis = "";

                switch (comboBox1.Text)
                {
                    case "Curacion de Heridas":
                        tHis = "CU";
                        break;
                    case "Medicina General":
                        tHis = "MG";
                        break;
                    case "Fisiatria":
                        tHis = "FI";
                        break;
                    case "Terapia Fisica":
                        tHis = "TF";
                        break;
                    case "Terapia Ocupacional":
                        tHis = "TO";
                        break;
                    case "Psicologia":
                        tHis = "PS";
                        break;
                    default:
                        MG = new MensajesGeneral();
                        MG.TipoImagen = 1000;
                        MG.Mensaje = "No hay tipo de historia seleccionada";
                        MG.ShowDialog();
                        return;
                }

                byte[] pdfBytes = ConvertirPDFABinario(label4.Text);

                Adj_Archivos aA = new Adj_Archivos
                {
                    Adj_Fecha = dateTimePicker1.Value,
                    Adj_Observacion = textBox2.Text,
                    Adj_Paciente = this.CodePac,
                    Adj_Tipo = tHis.ToString(),
                    Adj_Usr_Graba = Contenedor.UsuarioLogueado,
                    Adj_Archivo = pdfBytes,
                    Adj_Clase = comboBox2.Text
                };

                int savePDFBinary = repoAdjuntos.uploadFile(aA);
                if (savePDFBinary <= 0)
                {
                    MG = new MensajesGeneral();
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "No se logro grabar el adjunto, consulte el log de transacciones";
                    MG.ShowDialog();
                }
                else
                {
                    MG = new MensajesGeneral();
                    MG.TipoImagen = 3;
                    MG.Mensaje = "Archivo subido con numero unico de identificador: " + savePDFBinary.ToString();
                    MG.ShowDialog();

                    label4.Text = "";
                    textBox2.Text = "";
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        static byte[] ConvertirPDFABinario(string filePath)
        {
            return File.ReadAllBytes(filePath);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Dispose();
            this.Close();
        }

        private void UploadHistory_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Subir Registros PDF";

            LogoMain.Image = Properties.Resources.Splash;

            ToolStripButton btnBuscar = new ToolStripButton();
            btnBuscar = createToolButton("Buscar");
            MenuLateral.Items.Add(btnBuscar);
            btnBuscar.Click += button1_Click;
        }
    }
}
