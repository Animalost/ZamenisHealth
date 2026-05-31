using Domain.CXN;
using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ZamenisHealth.Comunes;
using static APIController.Clases.IAs;
using Message = APIController.Clases.IAs.Message;

namespace ZamenisHealth.IAS
{
    public partial class Perplexity : Forma
    {
        private readonly IIAS ias;
        private readonly IHelisa repoHelisa;
        private string ApiKey;
        private ToolStripButton btnGrabar, btnHistorial;

        public Perplexity()
        {
            InitializeComponent();
            repoHelisa = new MHelisa();
            ias = new MIAS();
        }

        private void Perplexity_Load(object sender, EventArgs e)
        {
            Titulo.Text = "IA";
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";
            LogoMain.Image = Properties.Resources.Splash;

            btnGrabar = new ToolStripButton();
            btnGrabar = createToolButton("Enviar Pregunta");
            MenuLateral.Items.Add(btnGrabar);
            btnGrabar.Click += toolStripButton1_Click;

            btnHistorial = new ToolStripButton();
            btnHistorial = createToolButton("Historial");
            MenuLateral.Items.Add(btnHistorial);
            btnHistorial.Click += toolStripButton3_Click;

            CargarApiKey();
            
        }
        void CargarApiKey()
        {
            try
            {
                ApiKey = repoHelisa.Claves("Perplexity", 10)["IAaPIPerplexity"];
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar la API Key: " + ex.Message, "Zamenis Health", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private async void toolStripButton1_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(richTextBox1.Text))
                {
                    MessageBox.Show("Debe realizar una pregunta valida", "Zamenis Health", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                else
                {
                    PerplexityRequest perplexity = new PerplexityRequest
                    {
                        model = "sonar",
                        messages = new List<Message>
                        {
                            new Message
                            {
                                role = "user",
                                content = richTextBox1.Text
                            }
                        },
                        max_tokens = 800,
                        temperature = 0.2,
                        APIKey = ApiKey
                    };

                    btnGrabar.Enabled = false;
                    pictureBox1.Visible = true;

                    var respuesta = await APIController.Services.IAs.Perplexity.EnviarPerplexity(perplexity, Program.URLApiConexion);
                    string respuestaFormateada = respuesta.Replace("\\n", Environment.NewLine);                   

                    richTextBox2.Text = respuestaFormateada;

                    CXN_HISTORYIA cXN_HISTORYIA = new CXN_HISTORYIA
                    {
                        Consulta = richTextBox1.Text,
                        Respuesta = respuestaFormateada,
                        FechaHora = DateTime.Now,
                        Usuario = Contenedor.UsuarioLogueado,
                        IA = "Perplexity"                        
                    };

                    ias.Insertar(cXN_HISTORYIA);

                    btnGrabar.Enabled = true;
                    pictureBox1.Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Zamenis Health", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            Historial historial = new Historial();
            historial.ShowDialog();
        }
    }
}
