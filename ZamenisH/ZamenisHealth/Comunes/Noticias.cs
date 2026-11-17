using Domain;
using Domain.CXN;

using FormAndControls;

using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Windows.Forms;

namespace ZamenisHealth.Comunes
{
    public partial class Noticias : Forma2
    {
        private readonly static IConfSystem repoSys = new MConfSystem();

        public Noticias()
        {
            InitializeComponent();
        
        }

        private async void Noticias_Load(object sender, EventArgs e)
        {
            try
            {
                Titulo.Text = "Noticias";
                SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";

                List<CXN_IMAGEN_SYSTEM> d = repoSys.getConfSystem();
                if (d != null)
                {
                    CXN_IMAGEN_SYSTEM d2 = d.FirstOrDefault(x => x.Tab_Nombre == "Noticia");
                    if (d2 != null)
                    {
                        using (HttpClient client = new HttpClient())
                        {
                            byte[] imagenBytes = await client.GetByteArrayAsync(d2.Tab_Config);

                            using (MemoryStream ms = new MemoryStream(imagenBytes))
                            {
                                using (Image imagen = Image.FromStream(ms))
                                {
                                    pictureBox1.Image = new Bitmap(imagen);
                                }
                            }
                        }

                        pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                    }
                }                                
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
    }
}
