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
    public partial class ProductsEdition : Forma
    {
        private IProdsConsumo oProdsConsumo;
        private int Posision;
        private MensajesGeneral MG;

        public ProductsEdition(int posision)
        {
            InitializeComponent();
            this.Posision = posision;
            oProdsConsumo = new MProdsConsumo();
        }

        private void ProductsEdition_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Edicion de Productos";
            LogoMain.Image = Properties.Resources.Splash;
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";

            ToolStripButton btnGrabar = new ToolStripButton();
            btnGrabar = createToolButton("Grabar");
            MenuLateral.Items.Add(btnGrabar);
            btnGrabar.Click += btnGrabar_Click;

            if (Posision > 0)
            {
                CargarProducto();
            }
        }
        void CargarProducto()
        {
            CON_PRODUCTOS get = oProdsConsumo.GetProductoById(Posision);
            if (get == null) 
            {
                MG = new MensajesGeneral()
                {
                    Mensaje = "No se logro cargar el producto",
                    TipoImagen = 1000
                };
                MG.ShowDialog();

                this.Close();
            }
            else
            {
                textBox1.Text = get.Con_Prod_Cod_Interno;
                textBox2.Text = get.Con_Prod_Cod_Externo;
                textBox3.Text = get.Con_Prod_Name;
                comboBox1.Text = get.Con_Prod_Status == true ? "ACTIVO" : "INACTIVO";
            }
        }
        void btnGrabar_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(textBox1.Text) || string.IsNullOrEmpty(textBox2.Text) ||
                    string.IsNullOrEmpty(textBox3.Text) || comboBox1.Text == "")
                {
                    MG = new MensajesGeneral()
                    {
                        Mensaje = "Debe diligenciar todos los campos",
                        TipoImagen = 1000
                    };
                    MG.ShowDialog();
                }
                else
                {
                    CON_PRODUCTOS P = new CON_PRODUCTOS
                    {
                        Con_Prod_Name = textBox3.Text,
                        Con_Prod_Cod_Interno = textBox1.Text,
                        Con_Prod_Cod_Externo = textBox2.Text,
                        Con_Prod_Status = comboBox1.Text == "ACTIVO" ? true : false,
                        Con_Prod_Id = Posision
                    };

                    if (Posision > 0)
                    {
                        //edit
                        bool save = oProdsConsumo.UpdateProducto(P);
                        if (save == true)
                        {
                            MG = new MensajesGeneral()
                            {
                                Mensaje = "Hecho",
                                TipoImagen = 3
                            };
                            MG.ShowDialog();

                            Products f1 = Application.OpenForms.OfType<Products>().SingleOrDefault();
                            f1.CargarProductos();

                            this.Close();
                        }
                        else
                        {
                            MG = new MensajesGeneral()
                            {
                                Mensaje = "No se logro actualizar el producto",
                                TipoImagen = 1000
                            };
                            MG.ShowDialog();
                        }
                    }
                    else
                    {
                        CON_PRODUCTOS get = oProdsConsumo.GetProductoByCodeExtern(textBox2.Text);
                        if (get != null)
                        {
                            MG = new MensajesGeneral()
                            {
                                Mensaje = "El codigo externo elegido ya existe, elija otro",
                                TipoImagen = 1000
                            };
                            MG.ShowDialog();
                        }
                        else
                        {
                            //insert
                            bool save = oProdsConsumo.CreaProducto(P);
                            if (save == true)
                            {
                                MG = new MensajesGeneral()
                                {
                                    Mensaje = "Hecho",
                                    TipoImagen = 3
                                };
                                MG.ShowDialog();

                                Products f1 = Application.OpenForms.OfType<Products>().SingleOrDefault();
                                f1.CargarProductos();

                                this.Close();
                            }
                            else
                            {
                                MG = new MensajesGeneral()
                                {
                                    Mensaje = "No se logro crear el producto",
                                    TipoImagen = 1000
                                };
                                MG.ShowDialog();
                            }
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
