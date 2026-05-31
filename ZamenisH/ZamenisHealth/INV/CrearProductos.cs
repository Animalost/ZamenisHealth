using Domain;
using Domain.CXN;
using Domain.INV;
using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using Persistence.INV.Interfaces;
using Persistence.INV.Metodos;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ZamenisHealth.Comunes;
using ZamenisHealth.INV;

namespace ZamenisHealth.Inventario
{
    public partial class CrearProductos : Forma
    {
        private static readonly IProveedores repoProv = new MProveedores();
        private static readonly IProductos repoProductos = new MProductos();
        private static readonly ICompañia repoCia = new MCompañia();

        private int CodPrestador;
        private int CodProveedor;

        private ToolTip toolRecharge;
        private MensajesGeneral MG;

        public CrearProductos()
        {
            InitializeComponent();
        }

        private void InvPpal2_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Crear Producto";
            LogoMain.Image = Properties.Resources.Splash;
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";

            ToolStripButton btnCreateProd;
            ToolStripButton btnEditProd;
            ToolStripButton btnCreateProv;

            btnCreateProd = new ToolStripButton();
            btnCreateProd = createToolButton("Crear Producto");
            MenuLateral.Items.Add(btnCreateProd);
            btnCreateProd.Click += toolStripButton3_Click;

            btnEditProd = new ToolStripButton();
            btnEditProd = createToolButton("Editar Producto");
            MenuLateral.Items.Add(btnEditProd);
            btnEditProd.Click += toolStripButton4_Click;

            btnCreateProv = new ToolStripButton();
            btnCreateProv = createToolButton("Crear Proveedor");
            MenuLateral.Items.Add(btnCreateProv);
            btnCreateProv.Click += toolStripButton2_Click;

            CargarProveedores();
            CargarPrestadores();

            toolRecharge = new ToolTip();
            toolRecharge.SetToolTip(pictureBox1, "Recargar Lista de Proveedores");
            toolRecharge.ShowAlways = true;
        }

        void CargarProveedores()
        {
            comboBox1.Items.Clear();

            List<CXN_PROVEEDORES> getLista = repoProv.getListadoProvs();
            if (getLista != null)
            {
                foreach (CXN_PROVEEDORES i in getLista)
                {
                    comboBox1.Items.Add(i.Nombre);
                }

                comboBox1.SelectedIndex = 0;
            }
        }

        void CargarPrestadores()
        {
            List<CXN_CIA> getLista = repoCia.getAllCompañias();
            if (getLista != null)
            {
                foreach (CXN_CIA i in getLista)
                {
                    comboBox3.Items.Add(i.Com_Nombre);
                }

                comboBox3.SelectedIndex = 0;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                this.CodProveedor = repoProv.getCodProvbyName(comboBox1.Text);
            }
            catch (Exception ex) 
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                this.CodPrestador = repoCia.getPrestadorbyName(comboBox3.Text).Com_Identificador;
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(textBox1.Text) || string.IsNullOrEmpty(textBox2.Text) || string.IsNullOrEmpty(textBox3.Text))
                {
                    MG = new MensajesGeneral();
                    MG.Mensaje = "Por favor complete todos los campos obligatorios.";
                    MG.TipoImagen = 1000;
                    MG.ShowDialog();
                }
                else
                {
                    INV_PRODUCTOS P = new INV_PRODUCTOS
                    {
                        CodigoInterno = textBox1.Text,
                        CodigoProveedor = textBox2.Text,
                        Nombre = textBox3.Text,
                        Observacion = textBox4.Text,
                        Proveedor = this.CodProveedor,
                        Prestador = this.CodPrestador
                    };

                    INV_PRODUCTOS result = repoProductos.GetProducto(P);
                    if (result != null)
                    {
                        MG = new MensajesGeneral();
                        MG.Mensaje = "El producto que intenta crear ya existe en el sistema para este proveedor y prestador";
                        MG.TipoImagen = 1000;
                        MG.ShowDialog();
                    }
                    else
                    {
                        int save = repoProductos.CrearProducto(P);
                        if (save >= 1)
                        {
                            MG = new MensajesGeneral();
                            MG.Mensaje = "El producto se ha creado exitosamente.";
                            MG.TipoImagen = 3;
                            MG.ShowDialog();

                            Limpiar();
                        }
                        else
                        {
                            MG = new MensajesGeneral();
                            MG.Mensaje = "No se logro crear el producto";
                            MG.TipoImagen = 1000;
                            MG.ShowDialog();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        void Limpiar()
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            Proveedores P = new Proveedores();
            P.ShowDialog();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            CargarProveedores();
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            CrearProductos2 crearProductos2 = new CrearProductos2(CodPrestador, CodProveedor);
            crearProductos2.ShowDialog();
        }
    }
}
