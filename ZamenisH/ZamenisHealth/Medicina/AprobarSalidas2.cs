using Domain.CXN;
using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Linq;
using System.Windows.Forms;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.Medicina
{
    public partial class AprobarSalidas2 : Forma2
    {
        private IMedicinaGeneral oMedGen;
        private IBodegas oBodegas;
        private int Admision, Bodega;
        private MensajesGeneral MG;

        public AprobarSalidas2(int admision)
        {
            InitializeComponent();
            this.Admision = admision;
            oMedGen = new MMedicinaGeneral();
            oBodegas = new MBodegas();
        }

        private void AprobarSalidas2_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Observaciones de Salida";
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";

            CargarObservacion();

            Bodega = oBodegas.getDatosUser(Contenedor.UsuarioLogueado).Bod_Numero;
        }
        void CargarObservacion()
        {
            try
            {
                CXN_HCMG get = oMedGen.getResumen(Admision);
                if (get == null) 
                {
                    MG = new MensajesGeneral()
                    {
                        Mensaje = "No hay relacion de historia",
                        TipoImagen = 3
                    };
                    MG.ShowDialog();

                    this.Dispose();
                    this.Close();
                }
                else
                {
                    richTextBox1.Text = get.HC_PManejo.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void boton2_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult result = MessageBox.Show("¿Desea eliminar esta solicitud de salida?",
                                                "Zamenis Health - Salidas por Enfermeria",
                                                MessageBoxButtons.YesNo,
                                                MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    oMedGen.EliminarSalida(Admision);

                    MG = new MensajesGeneral()
                    {
                        Mensaje = "Solicitud eliminada con exito",
                        TipoImagen = 3
                    };
                    MG.ShowDialog();

                    this.Dispose();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void boton1_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(richTextBox1.Text))
                {
                    MG = new MensajesGeneral()
                    {
                        Mensaje = "Debe digitar una observacion de salida",
                        TipoImagen = 3
                    };
                    MG.ShowDialog();
                }
                else
                {

                    int save = oMedGen.ActualizaSalidas(richTextBox1.Text, Admision, Bodega);
                    if (save > 0)
                    {
                        MG = new MensajesGeneral()
                        {
                            Mensaje = "Hecho.  SALIDA APROBADA",
                            TipoImagen = 3
                        };
                        MG.ShowDialog();

                        AprobarSalidas f1 = Application.OpenForms.OfType<AprobarSalidas>().SingleOrDefault();
                        f1.CargarSalidas();

                        oMedGen.ActualizaSalidas(Admision);

                        this.Dispose();
                        this.Close();
                    }
                    else
                    {
                        MG = new MensajesGeneral()
                        {
                            Mensaje = "No se logro grabar la observacion",
                            TipoImagen = 1000
                        };
                        MG.ShowDialog();
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
