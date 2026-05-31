using Domain;
using Domain.CXN;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Linq;
using System.Windows.Forms;
using ZamenisHealth.Clases;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.HistoriasClinicas
{
    public partial class Historia_MedicinaGeneral_4 : ConfigForm.BaseForm
    {
        private static readonly IInventario repoInv = new MInventario();
        private CXN_INVENTARIO DatosCita;

        public Historia_MedicinaGeneral_4(CXN_INVENTARIO dato)
        {
            InitializeComponent();
            this.DatosCita = dato;

            
        }

        private void Historia_MedicinaGeneral_4_Load(object sender, EventArgs e)
        {
            Titulo.Visible = false;
            ImageClose.Visible = false;

            ConfigForm.SoloNumeros(textBox1);

            label2.Text = this.DatosCita.InvItem;
            label3.Text = "$ " + this.DatosCita.InvPrecio.ToString("N0");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Dispose();
            this.Close();
        }


        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (textBox1.Text == "" || Convert.ToInt32(textBox1.Text) <= 0)
                {
                    MensajesGeneral MG = new MensajesGeneral();
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "";
                    MG.ShowDialog();
                    return;
                }

                CXN_INVENTARIO dat = repoInv.ConsultarValor2(label2.Text, 99);

                Historia_MedicinaGeneral_3 f2 = Application.OpenForms.OfType<Historia_MedicinaGeneral_3>().SingleOrDefault();
                f2.AddItem(dat, Convert.ToInt32(textBox1.Text));

                this.Dispose();
                this.Close();
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
    }
}
