using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using AForge.Video;
using AForge.Video.DirectShow;
using APIController.Images;
using Domain;
using Domain.CXN;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using ZamenisHealth.Clases;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.Medicina.Extras
{
    public partial class RegImagenesCamara : ConfigForm.BaseForm
    {
        private static readonly IBodegas repoBodegas = new MBodegas();
        private static readonly IImagenes repoImagenes = new MImagenes();
        private static readonly ILogin repoLogin = new MLogin();
        private static readonly IConfSystem repoConfSystem = new MConfSystem();
        private ImagesController oController;

        private FilterInfoCollection MisDispositivos;
        private VideoCaptureDevice MiWebCam;
        private int Admision, Paciente;
        private bool Haydispositivos;

        public RegImagenesCamara(int _admision, int _paciente)
        {
            InitializeComponent();
            this.Admision = _admision;
            this.Paciente = _paciente;
            oController = new ImagesController();
        }

        private void InicioWebcam()
        {
            if (MiWebCam != null && MiWebCam.IsRunning)
            {
                MiWebCam.SignalToStop();
                MiWebCam = null;
                this.Dispose();
                this.Close();
            }
        }

        private void CerrarWebCam()
        {
            try
            {
                if (MiWebCam != null && MiWebCam.IsRunning)
                {
                    MiWebCam.SignalToStop();
                    MiWebCam = null;
                    this.Dispose();
                    this.Close();
                }
                else
                {
                    this.Dispose();
                    this.Close();
                }
            }
            catch
            {
                this.Dispose();
                this.Close();
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                InicioWebcam();
                int i = comboBox2.SelectedIndex;
                string NombreVideo = MisDispositivos[i].MonikerString;
                MiWebCam = new VideoCaptureDevice(NombreVideo);
                MiWebCam.NewFrame += new NewFrameEventHandler(Capturando);
                MiWebCam.Start();

                button6.Enabled = true;
                button5.Enabled = false;
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Cerrar();
        }

        private void Capturando(object sender, NewFrameEventArgs eventArgs)
        {
            try
            {
                Bitmap imagen = (Bitmap)eventArgs.Frame.Clone();
                pictureBox3.Image = imagen;
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        void Cerrar()
        {
            CerrarWebCam();

            RegImagenes f29 = Application.OpenForms.OfType<RegImagenes>().SingleOrDefault();
            f29.CerrarPanel();

            this.Dispose();
            this.Close();
        }

        private void RegImagenesCamara_Load(object sender, EventArgs e)
        {
            Titulo.Visible = false;
            ImageClose.Visible = false;

            CargaDispositivos();
        }

        //GRABAR EN DISCO
        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
                Comunes.MensajesGeneral MG = new MensajesGeneral();

                if (Admision <= 0)
                {
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "No hay admision seleccionada";
                    MG.ShowDialog();

                    Cerrar();

                    return;
                }

                var Bod = repoLogin.getUser(Comunes.Contenedor.UsuarioLogueado);
                if (Bod == null || Bod.Log_Fotos != "A")
                {
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "Su usuario no tiene acceso a los registros medicos debido que usted no registra como profesional";
                    MG.ShowDialog();

                    Cerrar();

                    return;
                }

                if (MiWebCam != null && MiWebCam.IsRunning)
                {
                    pictureBox2.Image = pictureBox3.Image;
                    pictureBox2.Visible = true;
                }

                DialogResult result = MessageBox.Show("Desea grabar esta imagen?",
                    "Zamenis Health - Imagenes",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    var getCodUs = repoBodegas.getDatosUser(Contenedor.UsuarioLogueado);
                    if (getCodUs != null)
                    {
                        ImageConverter imgCon = new ImageConverter();
                        var Conversion = (byte[])imgCon.ConvertTo(pictureBox2.Image, typeof(byte[]));
                        string codingb64 = Convert.ToBase64String(Conversion);
                        DateTime Hoy = DateTime.Now;

                        Dictionary<string, string> getData = Conexion.Conection();
                        string cadena = getData["Conexion"];

                        CXN_IMAGENES I = new CXN_IMAGENES
                        {
                            Ima_Adm = Admision,
                            Ima_Med = getCodUs.Bod_Numero,
                            Ima_Pac = Paciente,
                            Ima_Nota = textBox4.Text,
                            Ima_Fecha = Convert.ToDateTime(Hoy),
                            Pac_Usr_Web = cadena,
                            Pac_Categoria = codingb64,
                            Ima_Ruta = repoConfSystem.getListado()["ImagenesSistema"].ToString().Trim()
                        };

                        //Generar token de autorizacion
                        bool Log = oController.Loguear(Program.URLApiConexion, Contenedor.UsuarioLogueado, cadena);
                        if (Log == false)
                        {
                            MG = new MensajesGeneral();
                            MG.Mensaje = "No se logro conectar al servicio API Web, intente nuevamente o mas tarde";
                            MG.TipoImagen = 1000;
                            MG.ShowDialog();
                            return;
                        }

                        int _inserta = oController.insertImageAPI(I, cadena);
                        if (_inserta >= 1)
                        {
                            pictureBox2.Image.Dispose();
                            pictureBox2.Image = null;
                            pictureBox2.Visible = false;
                            richTextBox1.Text = "";

                            MG.TipoImagen = 3;
                            MG.Mensaje = "Imagen Agregada";
                            MG.ShowDialog();

                            Cerrar();                            
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
                        MG.Mensaje = "Su usuario no es profesional para agregar imagenes";
                        MG.ShowDialog();

                        Cerrar();
                    }
                }
                if (result == DialogResult.No)
                {
                    pictureBox2.Image = null;
                    pictureBox2.Visible = false;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
                Cerrar();
            }
        }

        //GRABAR EN BD
        /* private void button6_Click(object sender, EventArgs e)
         {
             try
             {
                 Comunes.MensajesGeneral MG = new MensajesGeneral();

                 if (Admision <= 0)
                 {
                     MG.TipoImagen = 1000;
                     MG.Mensaje = "No hay admision seleccionada";
                     MG.ShowDialog();

                     Cerrar();

                     return;
                 }

                 var Bod = repoLogin.getUser(Comunes.Contenedor.UsuarioLogueado);
                 if (Bod == null || Bod.Log_Fotos != "A")
                 {
                     MG.TipoImagen = 1000;
                     MG.Mensaje = "Su usuario no tiene acceso a los registros medicos debido que usted no registra como profesional";
                     MG.ShowDialog();

                     Cerrar();

                     return;
                 }

                 if (MiWebCam != null && MiWebCam.IsRunning)
                 {
                     pictureBox2.Image = pictureBox3.Image;
                     pictureBox2.Visible = true;
                 }

                 DialogResult result = MessageBox.Show("Desea grabar esta imagen?",
                     "Zamenis Health - Imagenes",
                     MessageBoxButtons.YesNo,
                     MessageBoxIcon.Question);

                 if (result == DialogResult.Yes)
                 {
                     var getCodUs = repoBodegas.getDatosUser(Contenedor.UsuarioLogueado);
                     if (getCodUs != null)
                     {
                         ImageConverter imgCon = new ImageConverter();
                         var Conversion = (byte[])imgCon.ConvertTo(pictureBox2.Image, typeof(byte[]));
                         DateTime Hoy = DateTime.Now;

                         CXN_IMAGENES I = new CXN_IMAGENES
                         {
                             Ima_Adm = Admision,
                             Ima_Med = getCodUs.Bod_Numero,
                             Ima_Pac = Paciente,
                             Ima_Nota = textBox4.Text,
                             Ima_Grafica = Conversion,
                             Ima_Fecha = Convert.ToDateTime(Hoy)
                         };

                         bool _inserta = repoImagenes.insertImage(I);
                         if (_inserta != true)
                         {
                             MG.TipoImagen = 1000;
                             MG.Mensaje = "No logro ser insertada la imagen";
                             MG.ShowDialog();
                         }
                         else
                         {
                             pictureBox2.Image.Dispose();
                             pictureBox2.Image = null;
                             pictureBox2.Visible = false;
                             richTextBox1.Text = "";

                             MG.TipoImagen = 3;
                             MG.Mensaje = "Imagen Agregada";
                             MG.ShowDialog();

                             Cerrar();
                         }
                     }
                     else
                     {
                         MG.TipoImagen = 1000;
                         MG.Mensaje = "Su usuario no es profesional para agregar imagenes";
                         MG.ShowDialog();

                         Cerrar();
                     }
                 }
                 if (result == DialogResult.No)
                 {
                     pictureBox2.Image = null;
                     pictureBox2.Visible = false;
                 }
             }
             catch (Exception ex)
             {
                 TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
                 Cerrar();
             }
         }*/

        private void CargaDispositivos()
        {
            try
            {
                MisDispositivos = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                if (MisDispositivos.Count > 0)
                {
                    Haydispositivos = true;
                    for (int i = 0; i < MisDispositivos.Count; i++)
                    {
                        comboBox2.Items.Add(MisDispositivos[i].Name.ToString());
                        comboBox2.Text = MisDispositivos[0].Name.ToString();
                    }
                }
                else
                {
                    Haydispositivos = false;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
    }
}
