using APIController.Images;
using Domain;
using Domain.CXN;
using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.Medicina.Extras
{
    public partial class RegImagenesSubir : Forma
    {
        private readonly ImagesController imagesController;

        private static readonly ILogin repoLogin = new MLogin();
        private static readonly IBodegas repoBodegas = new MBodegas();
        private static readonly IConfSystem repoConfSystem = new MConfSystem();

        private int Admision, Paciente;
        private string Imagen_Ruta, Ruta, StringImagenB64, Nota;

        private float rotationAngle = 0;
        private Image recoveryImage;
        private bool isSearch;

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                Comunes.MensajesGeneral MG = new Comunes.MensajesGeneral();

                if (Ruta == "")
                {
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "No hay ruta de image";
                    MG.ShowDialog();

                    Cierra();
                    return;
                }

                if (Admision <= 0)
                {
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "No hay admision seleccionada";
                    MG.ShowDialog();

                    Cierra();
                    return;
                }

                var Bod = repoLogin.getUser(Comunes.Contenedor.UsuarioLogueado);
                if (Bod == null)
                {
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "Su usuario no tiene acceso a los registros medicos debido que usted no registra como profesional";
                    MG.ShowDialog();

                    Cierra();
                }
                else
                {
                    if (Bod.Log_Fotos == "A")
                    {
                        var getCodUs = repoBodegas.getDatosUser(Comunes.Contenedor.UsuarioLogueado);
                        if (getCodUs != null)
                        {
                            Byte[] bytes = File.ReadAllBytes(Ruta);
                            string codingb64 = Convert.ToBase64String(bytes);

                            DateTime Hoy = DateTime.Now;

                            Dictionary<string, string> getData = Conexion.Conection();
                            string cadena = getData["Conexion"];

                            CXN_IMAGENES I = new CXN_IMAGENES
                            {
                                Ima_Adm = Convert.ToInt32(Admision),
                                Ima_Med = getCodUs.Bod_Numero,
                                Ima_Pac = Paciente,
                                Ima_Nota = textBox3.Text,
                                Ima_Fecha = Convert.ToDateTime(Hoy),
                                Pac_Usr_Web = cadena,
                                Pac_Categoria = codingb64, 
                                Ima_Ruta = repoConfSystem.getListado()["ImagenesSistema"].ToString().Trim()
                            };                            

                            //Generar token de autorizacion
                            bool Log = imagesController.Loguear(Program.URLApiConexion, Contenedor.UsuarioLogueado, cadena);
                            if (Log == false)
                            {
                                MG = new MensajesGeneral();
                                MG.Mensaje = "No se logro conectar al servicio API Web, intente nuevamente o mas tarde";
                                MG.TipoImagen = 1000;
                                MG.ShowDialog();
                                return;
                            }                            

                            int _inserta = imagesController.insertImageAPI(I, cadena);
                            if (_inserta >= 1)
                            {
                                pictureBox1.Image.Dispose();
                                pictureBox1.Image = null;
                                textBox3.Text = "";
                                Ruta = "";
                                richTextBox1.Text = "";

                                MG.TipoImagen = 3;
                                MG.Mensaje = "Agregado";
                                MG.ShowDialog();
                            }
                            else
                            {
                                MG.TipoImagen = 1000;
                                MG.Mensaje = "No logro ser insertada la imagen";
                                MG.ShowDialog();
                            }
                        }
                        else
                        {
                            MG.TipoImagen = 1000;
                            MG.Mensaje = "Su usuario no tiene acceso a los registros medicos debido que usted no registra como profesional";
                            MG.ShowDialog();

                            Cierra();
                        }
                    }
                    else
                    {
                        MG.TipoImagen = 1000;
                        MG.Mensaje = "Su usuario no tiene acceso a los registros medicos debido que usted no registra como profesional";
                        MG.ShowDialog();

                        Cierra();
                    }
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        void Cierra()
        {
            RegImagenes f29 = Application.OpenForms.OfType<RegImagenes>().SingleOrDefault();
            f29.CerrarPanel();

            this.Dispose();
            this.Close();
        }
        private void label8_Click(object sender, EventArgs e)
        {
            if (recoveryImage != null)
            {
                rotationAngle += 90; // Incrementar el ángulo de rotación
                pictureBox1.Image = RotateImage(recoveryImage, rotationAngle);
            }
            else
            {
                MessageBox.Show("No hay ninguna imagen cargada para rotar.");
            }
        }
        private Image RotateImage(Image img, float angle)
        {
            try
            {
                Bitmap rotatedBitmap = new Bitmap(img.Width, img.Height);

                using (Graphics g = Graphics.FromImage(rotatedBitmap))
                {
                    g.TranslateTransform((float)img.Width / 2, (float)img.Height / 2);
                    g.RotateTransform(angle);
                    g.TranslateTransform(-(float)img.Width / 2, -(float)img.Height / 2);
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.DrawImage(img, new Point(0, 0));
                }

                return rotatedBitmap;
            }
            catch (Exception ex)
            {
                MessageBox.Show("No fue posible girar la imagen: " + ex.Message);
                return null;
            }            
        }
        void ShowImageInPictureBox(string imageBase64)
        {
            try
            {
                byte[] imageBytes = Convert.FromBase64String(imageBase64);

                using (MemoryStream ms = new MemoryStream(imageBytes))
                {
                    recoveryImage = Image.FromStream(ms);
                    pictureBox1.Image = System.Drawing.Image.FromStream(ms);
                };
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void RegImagenesSubir_Load(object sender, EventArgs e)
        {
            if (isSearch == true)
            {
                Titulo.Text = "Ver Imagenes";
                LogoMain.Image = Properties.Resources.Splash;

                ToolStripButton btnRotar = new ToolStripButton();
                btnRotar = createToolButton("Rotar Imagen");
                MenuLateral.Items.Add(btnRotar);
                btnRotar.Click += btnRotate_Click;

                ToolStripButton btnDescargar = new ToolStripButton();
                btnDescargar = createToolButton("Descargar Imagen");
                MenuLateral.Items.Add(btnDescargar);
                btnDescargar.Click += btnDownload_Click;

                textBox3.Text = Nota;

                textBox3.ReadOnly = true;
                label8.Visible = false;

                ShowImageInPictureBox(StringImagenB64);
            }
            else
            {
                Titulo.Text = "Subir Imagenes";

                LogoMain.Image = Properties.Resources.Splash;

                ToolStripButton btnSelImage = new ToolStripButton();
                btnSelImage = createToolButton("Seleccionar Imagen");
                MenuLateral.Items.Add(btnSelImage);
                btnSelImage.Click += button2_Click;

                ToolStripButton btnGrabar = new ToolStripButton();
                btnGrabar = createToolButton("Grabar Imagen");
                MenuLateral.Items.Add(btnGrabar);
                btnGrabar.Click += button3_Click;
            }            
        }
        void btnDownload_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime H = DateTime.Now.Date;
                SaveFileDialog Guardar = new SaveFileDialog();
                Guardar.FileName = "Imagen - " + Convert.ToDateTime(H).ToString("dd-MM-yyyy");
                Guardar.Filter = "JPEG(*.JPG)|*.JPG|BMP(*.BMP)|*.BMP";

                Bitmap thumbBMP = new Bitmap(pictureBox1.Image);

                Guardar.ShowDialog();

                using (MemoryStream memory = new MemoryStream())
                {
                    using (FileStream fs = new FileStream(Guardar.FileName, FileMode.Create, FileAccess.ReadWrite))
                    {
                        thumbBMP.Save(memory, ImageFormat.Jpeg);
                        byte[] bytes = memory.ToArray();
                        fs.Write(bytes, 0, bytes.Length);
                    }
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        void btnRotate_Click(object sender, EventArgs e)
        {
            rotationAngle += 90;
            pictureBox1.Image = RotateImage(recoveryImage, rotationAngle);
        }
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog BuscarImagen = new OpenFileDialog();
                BuscarImagen.Filter = "Archivos de Imagen|*.jpg;*.jpeg;*.png;*.bmp;*.gif;*.tif;*.tiff";
                BuscarImagen.FileName = "";
                BuscarImagen.InitialDirectory = "C:\\";
                BuscarImagen.FileName = Imagen_Ruta;
                if (BuscarImagen.ShowDialog() == DialogResult.OK)
                {
                    Imagen_Ruta = BuscarImagen.FileName;
                    String Direccion = BuscarImagen.FileName;
                    recoveryImage = Image.FromFile(Direccion);
                    pictureBox1.Image = recoveryImage; 
                    pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                    // textBox2.Text = BuscarImagen.FileName.Substring(Direccion.LastIndexOf("\\") + 1);
                    Ruta = Imagen_Ruta;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        public RegImagenesSubir(int _admision, int _paciente, bool isSearch, string stringImagenB64, string nota)
        {
            InitializeComponent();
            this.Admision = _admision;
            this.Paciente = _paciente;

            imagesController = new ImagesController();
            this.isSearch = isSearch;
            StringImagenB64 = stringImagenB64;
            Nota = nota;
        }
        public RegImagenesSubir(string Base64)
        {
            InitializeComponent();
            StringImagenB64 = Base64;
            CargarImagen();
        }
        void CargarImagen()
        {
            try
            {
                isSearch = true;
                ShowImageInPictureBox(StringImagenB64);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
