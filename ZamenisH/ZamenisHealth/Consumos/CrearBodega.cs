using Domain.CONSUMOS;
using FormAndControls;
using Persistence;
using Persistence.CONSUMOS.Interfaces;
using Persistence.CONSUMOS.Metodos;
using System;
using System.Linq;
using System.Windows.Forms;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.Consumos
{
    public partial class CrearBodega : Forma2
    {
        private IAsignacion oAsignacion;
        private int NumberBod;
        private MensajesGeneral MG;

        public CrearBodega(int numberBodega)
        {
            InitializeComponent();
            NumberBod = numberBodega;
            oAsignacion = new MAsignacion();
            SoloNumeros(textBox1);
        }

        private void CrearBodega_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Edicion de Bodegas";
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";

            if (NumberBod > 0)
            {
                CON_ASIGNACION getBod = oAsignacion.GetBodega(NumberBod);
                if (getBod != null)
                {
                    textBox1.Text = getBod.Asi_Number.ToString();
                    textBox2.Text = getBod.Asi_Name.ToString();
                    comboBox1.Text = getBod.Asi_Status == true ? "ACTIVA" : "INACTIVA";

                    textBox1.Enabled = false;
                }
                else
                {
                    MG = new MensajesGeneral()
                    {
                        Mensaje = "No se logro cargar datos de la bodega",
                        TipoImagen = 1000
                    };
                    MG.ShowDialog();

                    this.Close();
                }
            }
        }
        private void boton1_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(textBox1.Text) || string.IsNullOrEmpty(textBox2.Text) || comboBox1.Text == "")
                {
                    MG = new MensajesGeneral()
                    {
                        Mensaje = "Debe diligenciar todos los campos",
                        TipoImagen = 1000
                    };
                    MG.ShowDialog();
                }
                else if (textBox1.Text == "0")
                {
                    MG = new MensajesGeneral()
                    {
                        Mensaje = "el numero de bodega no puede ser 0",
                        TipoImagen = 1000
                    };
                    MG.ShowDialog();
                }
                else
                {
                    CON_ASIGNACION C = new CON_ASIGNACION
                    {
                        Asi_Name = textBox2.Text,
                        Asi_Status = comboBox1.Text == "ACTIVA" ? true : false,
                        Asi_Number = Convert.ToInt32(textBox1.Text)
                    };

                    if (NumberBod == 0)
                    {
                        CON_ASIGNACION data = oAsignacion.GetBodega(C.Asi_Number);
                        if (data != null)
                        {
                            MG = new MensajesGeneral()
                            {
                                Mensaje = "Este numero de bodega elegido ya esta en uso con " + data.Asi_Name + " elija otro codigo",
                                TipoImagen = 1000
                            };
                            MG.ShowDialog();
                        }
                        else
                        {
                            //insert
                            bool graba = oAsignacion.CreaBodega(C);
                            if (graba == true)
                            {
                                MG = new MensajesGeneral()
                                {
                                    Mensaje = "Bodega Crada con Exito",
                                    TipoImagen = 3
                                };
                                MG.ShowDialog();

                                Asignacion f1 = Application.OpenForms.OfType<Asignacion>().SingleOrDefault();
                                f1.CargarBodegas();

                                this.Close();
                            }
                            else
                            {
                                MG = new MensajesGeneral()
                                {
                                    Mensaje = "No se logro crear la bodega",
                                    TipoImagen = 1000
                                };
                                MG.ShowDialog();
                            }
                        }
                    }
                    else
                    {
                        //edit
                        bool graba = oAsignacion.UpdateBodega(C);
                        if (graba == true)
                        {
                            MG = new MensajesGeneral()
                            {
                                Mensaje = "Bodega Modificada con Exito",
                                TipoImagen = 3
                            };
                            MG.ShowDialog();

                            Asignacion f1 = Application.OpenForms.OfType<Asignacion>().SingleOrDefault();
                            f1.CargarBodegas();

                            this.Close();
                        }
                        else
                        {
                            MG = new MensajesGeneral()
                            {
                                Mensaje = "No se logro modificar la bodega",
                                TipoImagen = 1000
                            };
                            MG.ShowDialog();
                        }
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
