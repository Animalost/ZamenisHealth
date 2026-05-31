using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Windows.Forms;

namespace ZamenisHealth.Comunes.Extras
{
    public partial class PatronAsign : Forma2
    {
        private static readonly ILogin repoLogin = new MLogin();
        private MensajesGeneral mensaje;

        public PatronAsign()
        {
            InitializeComponent();
        }

        private void PatronAsign_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Asignacion de Patron de Acceso";
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";

            ImageMinimize.Visible = false;
            ImageClose.Visible = false;
            recoveryPass1.MouseUp += RecoveryPass1_MouseUp;
        }
        private void RecoveryPass1_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                DialogResult result = MessageBox.Show("¿Desea guardar este patron?",
                                                 "Patron de clave",
                                                 MessageBoxButtons.YesNo,
                                                 MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    bool save = repoLogin.updatePatron(Contenedor.UsuarioLogueado, recoveryPass1.PatronMarcado.Trim());

                    if (save == true)
                    {
                        recoveryPass1.PatronMarcado = "";
                        mensaje = new MensajesGeneral();
                        mensaje.Mensaje = "Su patron ha sido almacenado con exito";
                        mensaje.TipoImagen = 3;
                        mensaje.ShowDialog();

                        this.Dispose();
                        this.Close();
                    }
                    else
                    {
                        recoveryPass1.PatronMarcado = "";
                        mensaje = new MensajesGeneral();
                        mensaje.Mensaje = "No se logro grabar su patron";
                        mensaje.TipoImagen = 1000;
                        mensaje.ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
