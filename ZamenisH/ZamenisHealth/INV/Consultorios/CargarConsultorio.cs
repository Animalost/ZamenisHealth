using Domain.INV;
using FormAndControls;
using Persistence;
using Persistence.INV.Interfaces;
using Persistence.INV.Metodos;
using System;
using System.Linq;
using System.Windows.Forms;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.INV.Consultorios
{
    public partial class CargarConsultorio : Forma
    {
        private IProductos productos;
        private IBodegaPrincipal bodegaP;
        private ISubBodegas bodegaS;
        private int CodeBodega, PosisionInvPpal, CodProveedor, Cod_Prestador, PosisionInvSub, CodigoProductoTableProducto, IVA, Costo;
        private MensajesGeneral MG;


        public CargarConsultorio(int codeBodega, int posisionInvPpal)
        {
            InitializeComponent();
            PosisionInvPpal = posisionInvPpal;
            CodeBodega = codeBodega;
            productos = new MProductos();
            bodegaP = new MBodegaPrincipal();
            bodegaS = new MSubBodegas();
        }
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            try
            {                
                if (Convert.ToInt32(textBox6.Text) < Convert.ToInt32(textBox7.Text))
                {
                    MG = new MensajesGeneral();
                    MG.Mensaje = "No hay cantidad suficiente en la Bodega Principal para este producto";
                    MG.TipoImagen = 1000;
                    MG.ShowDialog();
                }
                else
                {
                    InvConsultorio f7 = Application.OpenForms.OfType<InvConsultorio>().LastOrDefault();

                    if (CodeBodega == 88 || CodeBodega == 888 || CodeBodega == 8888)
                    {
                        //Ventas y  otros
                        int CantidadNuevaPpal = Convert.ToInt32(textBox6.Text) - Convert.ToInt32(textBox7.Text);

                        string ValorGuarda = "";

                        switch (CodeBodega)
                        {
                            case 88:
                                ValorGuarda = "VENTAS - " + Contenedor.UsuarioLogueado;
                                break;
                            case 888:
                                ValorGuarda = "PRESTAMO - " + Contenedor.UsuarioLogueado;
                                break;
                            case 8888:
                                ValorGuarda = "DOMICILIOS - " + Contenedor.UsuarioLogueado;
                                break;
                            default:
                                ValorGuarda = "SALIDA - " + Contenedor.UsuarioLogueado;
                                break;
                        }

                        INV_INVENTARIOPPAL invPpal = new INV_INVENTARIOPPAL
                        {
                            Cantidad = CantidadNuevaPpal,
                            Id = PosisionInvPpal,
                        };

                        bool actPpal = bodegaP.ActualizarCantidad(invPpal, Contenedor.UsuarioLogueado, false);
                        if (actPpal == false)
                        {
                            MG = new MensajesGeneral();
                            MG.Mensaje = "Se ha gestionado el inventario de la subBodega pero no se actualizo el inventario principal";
                            MG.TipoImagen = 1000;
                            MG.ShowDialog();
                        }
                        else
                        {
                            INV_SALIDASSUB Is = new INV_SALIDASSUB
                            {
                                Bodega = 1000,
                                Cantidad = Convert.ToInt32(textBox7.Text),
                                CodPpal = PosisionInvPpal,
                                Fecha = DateTime.Now.Date,
                                Usuario = ValorGuarda
                            };

                            bodegaS.IngresarSalidaSub(Is);

                            f7.CargarGrilla();

                            this.Dispose();
                            this.Close();
                        }
                    }
                    else
                    {
                        //Bodegas 

                        int CantidadNuevaSub = Convert.ToInt32(textBox5.Text) + Convert.ToInt32(textBox7.Text);
                        int CantidadNuevaPpal = Convert.ToInt32(textBox6.Text) - Convert.ToInt32(textBox7.Text);

                        INV_INVENTARIOBODEGAS invSub = new INV_INVENTARIOBODEGAS
                        {
                            Bodega = CodeBodega,
                            Cantidad = CantidadNuevaSub,
                            CodInvPpal = CodigoProductoTableProducto,
                            Id = PosisionInvSub,
                            IVA = IVA,
                            Costo = Costo,
                            Lote = textBox2.Text,
                            Factura = textBox4.Text
                        };

                        if (PosisionInvSub == 0)
                        {
                            //insert
                            bool insertar = bodegaS.IngresarNuevo(invSub);
                            if (insertar == true)
                            {
                                MG = new MensajesGeneral();
                                MG.Mensaje = "Hecho";
                                MG.TipoImagen = 3;
                                MG.ShowDialog();
                            }
                            else
                            {
                                MG = new MensajesGeneral();
                                MG.Mensaje = "No se logro agregar al inventario";
                                MG.TipoImagen = 1000;
                                MG.ShowDialog();
                            }
                        }
                        else
                        {
                            //update
                            bool update = bodegaS.ActualizarCantidad(invSub);
                            if (update == true)
                            {
                                MG = new MensajesGeneral();
                                MG.Mensaje = "Hecho";
                                MG.TipoImagen = 3;
                                MG.ShowDialog();
                            }
                            else
                            {
                                MG = new MensajesGeneral();
                                MG.Mensaje = "No se logro actualizar al inventario";
                                MG.TipoImagen = 1000;
                                MG.ShowDialog();
                            }
                        }

                        //update ppal
                        INV_INVENTARIOPPAL invPpal = new INV_INVENTARIOPPAL
                        {
                            Cantidad = CantidadNuevaPpal,
                            Id = PosisionInvPpal,
                        };

                        bool actPpal = bodegaP.ActualizarCantidad(invPpal, Contenedor.UsuarioLogueado, false);
                        if (actPpal == false)
                        {
                            MG = new MensajesGeneral();
                            MG.Mensaje = "Se ha gestionado el inventario de la subBodega pero no se actualizo el inventario principal";
                            MG.TipoImagen = 1000;
                            MG.ShowDialog();
                        }

                        INV_SALIDASSUB Is = new INV_SALIDASSUB
                        {
                            Bodega = CodeBodega,
                            Cantidad = Convert.ToInt32(textBox7.Text),
                            CodPpal = PosisionInvPpal,
                            Fecha = DateTime.Now.Date,
                            Usuario = Contenedor.UsuarioLogueado
                        };

                        bodegaS.IngresarSalidaSub(Is);
                        
                        f7.CargarGrilla();

                        this.Dispose();
                        this.Close();
                    }                    
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void CargarConsultorio_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Cargar Consultorios";
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";
            LogoMain.Image = Properties.Resources.Splash;

            ToolStripButton btnCargar = new ToolStripButton();
            btnCargar = createToolButton("Cargar");
            MenuLateral.Items.Add(btnCargar);
            btnCargar.Click += toolStripButton2_Click;

            CargarProductoPpal();

            if (CodeBodega != 88)
            {
                CargarProductoSub();
            }   
            else
            {
                textBox5.Text = "0";
            }
        }
        void CargarProductoSub()
        {
            try
            {
                var datosSub = bodegaS.GetInventarySubByBodPosProd(CodeBodega, PosisionInvPpal);
                PosisionInvSub = datosSub.PosisionSub;
                textBox5.Text = datosSub.CantidadSub.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Zamenis Health", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        void CargarProductoPpal()
        {
            try
            {
                INV_HISTORICOPPAL ClaseProductos = bodegaP.GetInventaryPpalByPos(PosisionInvPpal); 
                if (ClaseProductos != null)
                {
                    textBox1.Text = ClaseProductos.Nombre;
                    textBox2.Text = ClaseProductos.Lote;
                    textBox3.Text = "$ " + Convert.ToInt32(ClaseProductos.Costo).ToString("N0");
                    textBox4.Text = ClaseProductos.Factura;
                    textBox6.Text = ClaseProductos.Cantidad.ToString();

                    CodigoProductoTableProducto = ClaseProductos.CodProducto;
                    CodProveedor = ClaseProductos.Proveedor;
                    Cod_Prestador = ClaseProductos.Prestador;
                    Costo = Convert.ToInt32(ClaseProductos.Costo);
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
    }
}
