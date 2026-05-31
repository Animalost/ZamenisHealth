using Domain.INV;
using FormAndControls;
using Persistence;
using Persistence.INV.Interfaces;
using Persistence.INV.Metodos;
using System;
using System.Windows.Forms;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.INV
{
    public partial class CrearProductos3 : Forma
    {
        private IProductos productos;
        private int Pos, CodPrestador, CodProveedor;

        private MensajesGeneral MG;

        public CrearProductos3(int pos)
        {
            InitializeComponent();
            Pos = pos;
            productos = new MProductos();
        }

        private void CrearProductos3_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Crear Producto";
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
                INV_PRODUCTOS getP = productos.GetProductobyId(Pos);
                if (getP != null)
                {
                    textBox1.Text = getP.CodigoInterno;
                    textBox2.Text = getP.CodigoProveedor;
                    textBox3.Text = getP.Nombre;
                    textBox4.Text = getP.Observacion;

                    this.CodPrestador = getP.Prestador;
                    this.CodProveedor = getP.Proveedor;
                }
                else
                {
                    MG = new MensajesGeneral();
                    MG.Mensaje = "No se pudo cargar el producto.";
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

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(textBox1.Text.Trim()) || string.IsNullOrEmpty(textBox2.Text.Trim()) || string.IsNullOrEmpty(textBox3.Text.Trim()))
                {
                    MG = new MensajesGeneral();
                    MG.Mensaje = "debe diligenciar los campos obligatorios";
                    MG.TipoImagen = 1000;
                    MG.ShowDialog();
                }
                else
                {
                    INV_PRODUCTOS P = new INV_PRODUCTOS
                    {
                        CodigoInterno = textBox1.Text.Trim(),
                        CodigoProveedor = textBox2.Text.Trim(),
                        Nombre = textBox3.Text.Trim(),
                        Observacion = textBox4.Text.Trim(),
                        Proveedor = this.CodProveedor,
                        Prestador = this.CodPrestador,
                        Id = this.Pos
                    };

                    INV_PRODUCTOS search = productos.GetProducto(P);
                    if (search != null) 
                    { 
                        if (search.Id != Pos)
                        {
                            MG = new MensajesGeneral();
                            MG.Mensaje = "El producto ya se encuentra registrado con este codigo de proveedor";
                            MG.TipoImagen = 1000;
                            MG.ShowDialog();
                        }
                        else
                        {
                            bool updated = productos.ActualizarProducto(P);
                            if (updated)
                            {
                                MG = new MensajesGeneral();
                                MG.Mensaje = "Producto actualizado correctamente.";
                                MG.TipoImagen = 3;
                                MG.ShowDialog();

                                this.Dispose();
                                this.Close();
                            }
                            else
                            {
                                MG = new MensajesGeneral();
                                MG.Mensaje = "No se pudo actualizar el producto.";
                                MG.TipoImagen = 1000;
                                MG.ShowDialog();
                            }
                        }
                    }
                    else
                    {
                        bool updated = productos.ActualizarProducto(P);
                        if (updated)
                        {
                            MG = new MensajesGeneral();
                            MG.Mensaje = "Producto actualizado correctamente.";
                            MG.TipoImagen = 3;
                            MG.ShowDialog();

                            this.Dispose();
                            this.Close();
                        }
                        else
                        {
                            MG = new MensajesGeneral();
                            MG.Mensaje = "No se pudo actualizar el producto.";
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
