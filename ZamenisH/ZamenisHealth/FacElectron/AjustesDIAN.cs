using Domain.CXN;
using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Windows.Forms;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.FacElectron
{
    public partial class AjustesDIAN : Forma
    {
        private readonly IFacElectron facElectron;
        private int Cia;
        private MensajesGeneral MG;
        bool edit = false;

        public AjustesDIAN(int cia)
        {
            InitializeComponent();
            Cia = cia;
            SoloNumeros(textBox1);
            SoloNumeros(textBox2);
            facElectron = new MFacElectron();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {                
                if (string.IsNullOrEmpty(textBox1.Text) || string.IsNullOrEmpty(textBox2.Text))
                {
                    edit = false;
                    MG = new MensajesGeneral();
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "Debe diligenciar los dos campos";
                    MG.ShowDialog();
                    return;
                }

                CXN_FACTURA getFacCorrecta = facElectron.GetFacZam(Cia, Convert.ToInt32(textBox2.Text));
                if (getFacCorrecta == null) 
                {
                    edit = false;
                    MG = new MensajesGeneral();
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "No existe factura correcta";
                    MG.ShowDialog();
                }
                else
                {
                    CXN_FACTURA getFacErroneo = facElectron.GetFacZam(Cia, Convert.ToInt32(textBox1.Text));
                    if (getFacErroneo == null)
                    {
                        edit = false;
                        MG = new MensajesGeneral();
                        MG.TipoImagen = 1000;
                        MG.Mensaje = "No existe factura erronea";
                        MG.ShowDialog();
                    }
                    else
                    {
                        if (getFacCorrecta.Fac_Fecha == new DateTime(2000, 01, 01).Date)
                        {
                            edit = false;
                            MG = new MensajesGeneral();
                            MG.TipoImagen = 1000;
                            MG.Mensaje = "El numero zamenis correcto no esta homologado como factura electronica";
                            MG.ShowDialog();
                        }
                        else
                        {
                            edit = true;
                            textBox10.Text = getFacCorrecta.Homologo;
                            textBox9.Text = getFacCorrecta.Cufe;
                            textBox8.Text = Convert.ToDateTime(getFacCorrecta.Hora).ToString("HH:mm:ss tt");
                            textBox7.Text = getFacCorrecta.Fac_Res;

                            textBox3.Text = getFacErroneo.Homologo;
                            textBox4.Text = getFacErroneo.Cufe;
                            textBox5.Text = Convert.ToDateTime(getFacErroneo.Hora) == new DateTime(2000,01,01) ? "" : Convert.ToDateTime(getFacErroneo.Hora).ToString("HH:mm:ss tt");
                            textBox6.Text = getFacErroneo.Fac_Res;

                            textBox1.Enabled = false;
                            textBox2.Enabled = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                edit = false;
                MessageBox.Show(ex.Message);
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (edit == false) 
                {
                    MG = new MensajesGeneral();
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "No es posible cambiar estados ya que no hay resultados validos con los criterios de busqueda ingresados";
                    MG.ShowDialog();
                    return;
                }

                CXN_FACTURA getFacCorrecta = facElectron.GetFacZam(Cia, Convert.ToInt32(textBox2.Text));
                getFacCorrecta.Fac_Num_Fac = Convert.ToInt32(textBox1.Text);

                bool editar = facElectron.UpdateDatosDIAN(getFacCorrecta); //escribe sobre la factura erronea los datos de la correcta
                if (editar == false)
                {
                    MG = new MensajesGeneral();
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "No se logro editar los datos DIAN";
                    MG.ShowDialog();
                }
                else
                {
                    CXN_FACTURA f = new CXN_FACTURA
                    {
                        Fac_Num_Fac = Convert.ToInt32(textBox2.Text),
                        Cufe = null,
                        Fac_Res = "NO APLICA RESOLUCION ES ORDEN DE PEDIDO"
                    };

                    bool editarAntigua = facElectron.UpdateDatosDIAN(f); //limpia la factura con los antigus datos y la deja abierta nuevameante para homologar
                    if (editarAntigua == false)
                    {
                        MG = new MensajesGeneral();
                        MG.TipoImagen = 3;
                        MG.Mensaje = "Los datos se actualizaron con exito sobre la factura erronea, pero los datos de la antigua factura no se " +
                            "lograron editar, contacte al administrador para formatearlos y entregue este numero.  Documento Zamenis a formatear: " + textBox2.Text;
                        MG.ShowDialog();
                    }
                    else
                    {
                        MG = new MensajesGeneral();
                        MG.TipoImagen = 3;
                        MG.Mensaje = "Datos editados con exito";
                        MG.ShowDialog();
                    }                    

                    this.Dispose();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void AjustesDIAN_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Ajustes DIAN";
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";
            LogoMain.Image = Properties.Resources.Splash;

            ToolStripButton btnSearch = new ToolStripButton();
            btnSearch = createToolButton("Consultar");
            MenuLateral.Items.Add(btnSearch);
            btnSearch.Click += button1_Click;

            ToolStripButton btnGrabar = new ToolStripButton();
            btnGrabar = createToolButton("Grabar");
            MenuLateral.Items.Add(btnGrabar);
            btnGrabar.Click += button2_Click;
        }
    }
}
