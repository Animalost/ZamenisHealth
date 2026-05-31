using Domain.CXN;
using FormAndControls;
using Persistence;
using System;
using System.Linq;
using System.Windows.Forms;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.Facturacion.OpenInvoice
{
    public partial class Cantidades : Forma2
    {
        private CXN_CARGOS Car;
        private MensajesGeneral MG;

        public Cantidades(CXN_CARGOS c)
        {
            InitializeComponent();
            Car = c;

            SoloNumeros(textBox1);
            SoloNumeros(textBox2);
        }

        private void Cantidades_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Cantidades";
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";

            textBox1.Text = Convert.ToInt32(Car.Car_Val_Un).ToString("N0").Replace(".", "");
        }
        private void boton1_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(textBox1.Text) || string.IsNullOrEmpty(textBox2.Text))
                {
                    MG = new MensajesGeneral();
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "Debe diligenciar los dos campos de texto";
                    MG.ShowDialog();
                }
                else
                {
                    int ValTot = Convert.ToInt32(textBox1.Text) * Convert.ToInt32(textBox2.Text);

                    CXN_CARGOS C = new CXN_CARGOS
                    {
                        Car_Adm_Id = 0, //Primer Form
                        Car_Pac = 0,//Primer Form
                        Car_Cia = 0,//Primer Form
                        Car_Ase = Car.Car_Ase,
                        Car_Prof = 1000,
                        Car_Fecha = DateTime.Now.Date,
                        Car_Estado = "G",
                        Car_Tipo = Car.Car_Tipo,
                        Car_Cod = Car.Car_Cod,
                        Car_Tipo_Serv = Car.Car_Tipo_Serv,
                        Car_Cant = Convert.ToInt32(textBox2.Text),
                        Car_Val_Un = Convert.ToInt32(textBox1.Text),
                        Car_Val_Tot = ValTot,
                        Car_Item = Car.Car_Item,
                        Car_Detalle = Car.Car_Detalle,
                        Car_Dx1 = "L97X",
                        Car_Dx2 = "",
                        Car_Dx3 = "",
                        Car_Regimen = "",//Primer Form
                        Car_Ambito = 0,
                        Car_Finalidad = 0,
                        Car_Personal = 0,
                        Car_CExterna = 0,
                        Car_Finalidad_CO = 0,
                        Car_Imp_Dx = 0
                    };

                    FacturaAbierta f2 = Application.OpenForms.OfType<FacturaAbierta>().SingleOrDefault();
                    f2.setDataToFactura(C);

                    MG = new MensajesGeneral();
                    MG.TipoImagen = 3;
                    MG.Mensaje = "Agregado!!";
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
    }
}
