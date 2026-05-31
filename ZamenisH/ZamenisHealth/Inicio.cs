using Domain;
using Domain.Licence;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Windows.Forms;
using ZamenisHealth.Clases;
using ZamenisHealth.Comunes;
using ZamenisHealth.Licence;

namespace ZamenisHealth
{
    public partial class Inicio : ConfigForm.BaseForm
    {
        private static readonly IGenerales repoGen = new MGenerales();

        private MensajesGeneral MG;

        public Inicio()
        {
            InitializeComponent();
        }       

        private void button1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        //Muestra la licencia de la aplicacion al dar doble click en el label
        private void label2_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                LicenceAdds L = new LicenceAdds("https://github.com/falahati/CircularProgressBar/blob/master/LICENSE", "CircProgressBar");
                L.ShowDialog();             
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        //Lanza todas las validaciones iniciales de la aplicacion para el funcionamiento correcto   
        private async void Inicio_Load(object sender, EventArgs e)
        {
            try
            {
                Titulo.Visible = false;
                ImageClose.Visible = false;

                //Trae el serial del registro de Windows en Base 64
                string s = LicenceZH.Serial.getSerial();
                ZhealthClass obj = LicenceZH.MySQLConection.ObtenerLicencia(s);
                if (obj == null)
                {
                    MG = new MensajesGeneral();
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "La licencia de la aplicacion no es valida, contacte a soporte";
                    MG.ShowDialog();
                    
                    Application.Exit();
                }
                else
                {
                    DateTime Hoy = DateTime.Now.Date;

                    if (Convert.ToDateTime(obj.Generales.Vencimiento) < Convert.ToDateTime(Hoy))
                    {
                        MG = new MensajesGeneral();
                        MG.TipoImagen = 1000;
                        MG.Mensaje = "Su licencia ha expirado el dia " + Convert.ToDateTime(obj.Generales.Vencimiento).ToString("yyyy-MM-dd");
                        MG.ShowDialog();

                        Application.Exit();
                    }
                    else
                    {
                        if (obj.Generales.Habilitar != "A")
                        {
                            MG = new MensajesGeneral();
                            MG.TipoImagen = 1000;
                            MG.Mensaje = "El sistema no esta habilitado en este PC.  Contacte al desarrollador";
                            MG.ShowDialog();

                            Application.Exit();
                        }
                        else
                        {
                            Conexion.ConectionInitialWEB(obj);

                            //Program.URLApiConexion = "http://localhost:5101";
                            Program.URLApiConexion = Conexion.ConectionDictionary["URLApi"];

                            this.Hide();
                            Comunes.Login FrmLogin = new Comunes.Login();
                            FrmLogin.ShowDialog();
                        }
                    }                   
                }                    
            }
            catch (Exception ex)
            {
                //SI hay algun error trae los datos del registro por defecto
                MG = new MensajesGeneral();
                MG.TipoImagen = 0;
                MG.Mensaje = ex.Message;
                MG.ShowDialog();

                Application.Exit();
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void Inicio_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F1)
            {
                ConfigForm.OpenIniFile();
            }
        }
    }
}
