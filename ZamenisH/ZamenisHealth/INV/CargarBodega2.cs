using Domain.INV;
using FormAndControls;
using Persistence;
using Persistence.INV.Interfaces;
using Persistence.INV.Metodos;
using System;
using System.Linq;
using System.Windows.Forms;
using ZamenisHealth.Clases;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.INV
{
    public partial class CargarBodega2 : Forma
    {
        private IProductos productos;
        private IBodegaPrincipal bodegaP;
        private MensajesGeneral MG;
        private int Posision, Cod_Prestador, CodProveedor;

        public CargarBodega2(int posision)
        {
            InitializeComponent();
            Posision = posision;
            productos = new MProductos();
            bodegaP = new MBodegaPrincipal();

            ConfigForm.SoloNumeros(textBox6);
            ConfigForm.SoloNumeros(textBox7);
            ConfigForm.SoloNumeros(textBox8);
        }

        private void CargarBodega2_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Cargar Bodegas";
            LogoMain.Image = Properties.Resources.Splash;
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";

            ToolStripButton btnSave;

            btnSave = new ToolStripButton();
            btnSave = createToolButton("Guardar");
            MenuLateral.Items.Add(btnSave);
            btnSave.Click += toolStripButton3_Click;

            CargarProducto();
        }

        void CargarProducto()
        {
            try
            {
                INV_PRODUCTOS ClaseProductos = productos.GetProductobyId(Posision);
                if (ClaseProductos != null)
                {
                    textBox1.Text = ClaseProductos.Nombre;
                    textBox2.Text = ClaseProductos.CodigoInterno;
                    textBox3.Text = ClaseProductos.CodigoProveedor;

                    CodProveedor = ClaseProductos.Proveedor;
                    Cod_Prestador = ClaseProductos.Prestador;
                }
                else
                {
                    MG = new MensajesGeneral();
                    MG.Mensaje = "No se encontró el producto.";
                    MG.TipoImagen = 1000;
                    MG.ShowDialog();

                    this.Dispose();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Zamenis Health", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Dispose();
            this.Close();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(textBox4.Text) || string.IsNullOrEmpty(textBox5.Text) || string.IsNullOrEmpty(textBox6.Text) ||
                    string.IsNullOrEmpty(textBox7.Text) || string.IsNullOrEmpty(textBox8.Text))
                {
                    MG = new MensajesGeneral();
                    MG.Mensaje = "Debe completar todos los campos";
                    MG.TipoImagen = 1000;
                    MG.ShowDialog();
                }
                else
                {
                    INV_INVENTARIOPPAL ClaseInventario = new INV_INVENTARIOPPAL
                    {
                        Cantidad = Convert.ToInt32(textBox6.Text),
                        CodProducto = Posision,
                        Costo = Convert.ToInt32(textBox7.Text),
                        Factura = textBox5.Text,
                        Lote = textBox4.Text,
                        IVA = Convert.ToInt32(textBox8.Text)
                    };

                    INV_INVENTARIOPPAL iP = bodegaP.GetCantidad(ClaseInventario);
                    if (iP != null)
                    {
                        //Actualizar Cantidad
                        int CantidadNueva = iP.Cantidad + ClaseInventario.Cantidad;
                        ClaseInventario.Cantidad = CantidadNueva;
                        ClaseInventario.Id = iP.Id;

                        bool update = bodegaP.ActualizarCantidad(ClaseInventario, Contenedor.UsuarioLogueado, true);
                        if (update == true)
                        {
                            CargarBodega f7 = Application.OpenForms.OfType<CargarBodega>().LastOrDefault();

                            INV_PRODUCTOS ip = new INV_PRODUCTOS
                            {
                                Prestador = Cod_Prestador,
                                Proveedor = CodProveedor
                            };

                            f7.CargarProductos("", ip);

                            this.Dispose();
                            this.Close();
                        }
                        else                        
                        {
                            MG = new MensajesGeneral();
                            MG.Mensaje = "No se pudo actualizar la cantidad en inventario.";
                            MG.TipoImagen = 1000;
                            MG.ShowDialog();
                        }
                    }
                    else
                    {
                        //Crear Nuevo en Inventario
                        bool insert = bodegaP.IngresarNuevo(ClaseInventario, Contenedor.UsuarioLogueado);
                        if (insert == true)
                        {
                            CargarBodega f7 = Application.OpenForms.OfType<CargarBodega>().LastOrDefault();

                            INV_PRODUCTOS ip = new INV_PRODUCTOS
                            {
                                Prestador = Cod_Prestador,
                                Proveedor = CodProveedor
                            };

                            f7.CargarProductos("", ip);

                            this.Dispose();
                            this.Close();
                        }
                        else
                        {
                            MG = new MensajesGeneral();
                            MG.Mensaje = "No se pudo crear la cantidad en inventario.";
                            MG.TipoImagen = 1000;
                            MG.ShowDialog();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Zamenis Health", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
