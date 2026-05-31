using Domain;
using Domain.CXN;
using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Windows.Forms;
using ZamenisHealth.Clases;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.Facturacion
{
    public partial class Facturar5Val : Forma
    {
        private static readonly ICargos repoCar = new MCargos();
        private int Posision_Val;

        public Facturar5Val(int _cargo, int _posision)
        {
            InitializeComponent();
            textBox1.Text = _cargo.ToString();
            this.Posision_Val = _posision;      
        }

        private void Facturar5Val_Load(object sender, EventArgs e)
        {
            try
            {
                Titulo.Text = "Cambiar Valores";
                SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";
                LogoMain.Image = Properties.Resources.Splash;

                ToolStripButton btnGrabar = new ToolStripButton();
                btnGrabar = createToolButton("Grabar");
                MenuLateral.Items.Add(btnGrabar);
                btnGrabar.Click += button1_Click;

                ConfigForm.SoloNumeros(textBox2);
                ConfigForm.SoloNumeros(textBox3);

                

                var getValores = repoCar.getValores(Posision_Val);
                if (getValores != null)
                {
                    textBox5.Text = getValores.Car_Item.ToString();
                    textBox2.Text = Convert.ToInt32(getValores.Car_Cant).ToString();
                    textBox3.Text = Convert.ToInt32(getValores.Car_Val_Un).ToString();

                    Calculo(Convert.ToInt32(getValores.Car_Cant), Convert.ToInt32(getValores.Car_Val_Un));
                }
                else
                {
                    textBox2.Text = "0";
                    textBox3.Text = "0";
                    textBox5.Text = "0";
                    textBox4.Text = "0";

                    MessageBox.Show("Hay un inconveniente con esta admision, cierre esta pantalla y vuelva a abrirla", "Incidencia", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void Calculo(int Val_Cant, int Val_Un)
        {
            try
            {
                textBox4.Text = (Val_Cant * Val_Un).ToString();
            }
            catch
            {
                textBox4.Text = "0";
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (textBox4.Text == "0") { MessageBox.Show("Los valores ingresados no son validos", "Error de Actualizacion", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
                if (textBox2.Text == "0") { MessageBox.Show("Los valores ingresados no son validos", "Error de Actualizacion", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
                if (textBox3.Text == "0") { MessageBox.Show("Los valores ingresados no son validos", "Error de Actualizacion", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

                if (textBox4.Text == "") { MessageBox.Show("Los valores ingresados no son validos", "Error de Actualizacion", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
                if (textBox2.Text == "") { MessageBox.Show("Los valores ingresados no son validos", "Error de Actualizacion", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
                if (textBox3.Text == "") { MessageBox.Show("Los valores ingresados no son validos", "Error de Actualizacion", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

                Calculo(Convert.ToInt32(textBox2.Text), Convert.ToInt32(textBox3.Text));

                CXN_CARGOS C = new CXN_CARGOS
                {
                    Car_Cant = Convert.ToInt32(textBox2.Text),
                    Car_Val_Un = Convert.ToInt32(textBox3.Text),
                    Car_Val_Tot = Convert.ToInt32(textBox4.Text),
                    Car_Id = Posision_Val
                };

                bool _update = repoCar.updateValores(C);
                if (_update != true)
                {
                    MessageBox.Show("No se logro actualizar", "Intente nuevamente");
                }
                else
                {
                    MessageBox.Show("Actualizado con Exito");
                    this.Dispose();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void textBox2_Leave(object sender, EventArgs e)
        {
            Calculo(Convert.ToInt32(textBox2.Text), Convert.ToInt32(textBox3.Text));
        }
        private void textBox3_Leave(object sender, EventArgs e)
        {
            Calculo(Convert.ToInt32(textBox2.Text), Convert.ToInt32(textBox3.Text));
        }
    }
}
