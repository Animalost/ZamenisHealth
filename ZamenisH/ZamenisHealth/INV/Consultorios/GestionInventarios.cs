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
using System.Linq;
using System.Windows.Forms;

using ZamenisHealth.Clases;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.INV.Consultorios
{
    public partial class GestionInventarios : Forma
    {
        private ISubBodegas repoBodedas;
        private IBodegas repoConsultorios;
        private int PosisionSub, PosisionPpalProd, CodeBodegaEntrante;
        private string Item;
        private MensajesGeneral MG;

        public GestionInventarios(int posisionSub, string item)
        {
            InitializeComponent();
            PosisionSub = posisionSub;
            Item = item;
            repoBodedas = new MSubBodegas();
            repoConsultorios = new MBodegas();

            ConfigForm.SoloNumeros(textBox3);
        }

        private void GestionInventarios_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Movimientos";
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";
            LogoMain.Image = Properties.Resources.Splash;

            ToolStripButton btnPrestar = new ToolStripButton();
            btnPrestar = createToolButton("Prestar");
            MenuLateral.Items.Add(btnPrestar);
            btnPrestar.Click += toolStripButton2_Click;

                ToolStripButton btnConsumir = new ToolStripButton();
            btnConsumir = createToolButton("Consumir");
            MenuLateral.Items.Add(btnConsumir);
            btnConsumir.Click += toolStripButton3_Click;

            textBox1.Text = this.Item;
            CargarCantidad();
            CargarBodegas();
        }
        void CargarBodegas()
        {
            try
            {
                List<CXN_BODEGAS> getBods = repoConsultorios.Filtrar();
                if (getBods != null)
                {
                    foreach (CXN_BODEGAS i in getBods)
                    {
                        comboBox1.Items.Add(i.Bod_Responsable);
                    }

                    comboBox1.SelectedIndex = 0;
                }
                else
                {
                    MG = new MensajesGeneral();
                    MG.Mensaje = "No hay bodegas para hacer traslados o prestamos";
                    MG.TipoImagen = 3;
                    MG.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        void CargarCantidad()
        {
            var getcantActualSub = repoBodedas.GetInventarySubByBodPos(PosisionSub);
            textBox2.Text = getcantActualSub.Cantidad.ToString();
            PosisionPpalProd = getcantActualSub.PosIdTablePpal;
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            CodeBodegaEntrante = repoConsultorios.getDatosName(comboBox1.Text).Bod_Numero;
        }
        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(textBox3.Text))
                {
                    MG = new MensajesGeneral();
                    MG.Mensaje = "Debe diligenciar una cantidad valida";
                    MG.TipoImagen = 1000;
                    MG.ShowDialog();
                }
                else if (textBox3.Text == "0")
                {
                    MG = new MensajesGeneral();
                    MG.Mensaje = "Debe diligenciar una cantidad superior a 0";
                    MG.TipoImagen = 1000;
                    MG.ShowDialog();
                }
                else if (Convert.ToInt32(textBox2.Text) < Convert.ToInt32(textBox3.Text))
                {
                    MG = new MensajesGeneral();
                    MG.Mensaje = "La cantidad a descontar es superior a la disponible";
                    MG.TipoImagen = 1000;
                    MG.ShowDialog();
                }
                else
                {
                    int cantNue = Convert.ToInt32(textBox2.Text) - Convert.ToInt32(textBox3.Text);

                    INV_INVENTARIOBODEGAS I = new INV_INVENTARIOBODEGAS
                    {
                        Cantidad = cantNue,
                        Id = this.PosisionSub
                    };

                    bool update = repoBodedas.ActualizarCantidad(I);
                    if (update == true) 
                    {
                        MG = new MensajesGeneral();
                        MG.Mensaje = "Hecho";
                        MG.TipoImagen = 3;
                        MG.ShowDialog();

                        VerInventario f7 = Application.OpenForms.OfType<VerInventario>().LastOrDefault();
                        f7.CargarInventario();

                        this.Dispose();
                        this.Close();
                    }
                    else
                    {
                        MG = new MensajesGeneral();
                        MG.Mensaje = "No se logro consumir el producto";
                        MG.TipoImagen = 1000;
                        MG.ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(textBox3.Text))
                {
                    MG = new MensajesGeneral();
                    MG.Mensaje = "Debe diligenciar una cantidad valida";
                    MG.TipoImagen = 1000;
                    MG.ShowDialog();
                }
                else if (textBox3.Text == "0")
                {
                    MG = new MensajesGeneral();
                    MG.Mensaje = "Debe diligenciar una cantidad superior a 0";
                    MG.TipoImagen = 1000;
                    MG.ShowDialog();
                }
                else if (Convert.ToInt32(textBox2.Text) < Convert.ToInt32(textBox3.Text))
                {
                    MG = new MensajesGeneral();
                    MG.Mensaje = "La cantidad a descontar es superior a la disponible";
                    MG.TipoImagen = 1000;
                    MG.ShowDialog();
                }
                else
                {
                    //Nueva cantidad del consultorio saliente
                    int cantNueDesde = Convert.ToInt32(textBox2.Text) - Convert.ToInt32(textBox3.Text);

                    //obtener cantidad el consultorio entrante
                    var cantProdEntrante = repoBodedas.GetInventarySubByBodPosProd(CodeBodegaEntrante, PosisionPpalProd);
                    if (cantProdEntrante.PosisionSub == 0)
                    {
                        //insertar Entrante
                        INV_INVENTARIOBODEGAS I = new INV_INVENTARIOBODEGAS
                        {
                            Cantidad = Convert.ToInt32(textBox3.Text),
                            Bodega = CodeBodegaEntrante,
                            CodInvPpal = PosisionPpalProd
                        };

                        bool insertar = repoBodedas.IngresarNuevo(I);
                        if (insertar == false)
                        {
                            MG = new MensajesGeneral();
                            MG.Mensaje = "No se logro prestar el producto";
                            MG.TipoImagen = 1000;
                            MG.ShowDialog();
                            return;
                        }
                    }
                    else
                    {
                        //actualizar Entrante
                        INV_INVENTARIOBODEGAS I2 = new INV_INVENTARIOBODEGAS
                        {
                            Cantidad = cantProdEntrante.CantidadSub + Convert.ToInt32(textBox3.Text),
                            Id = cantProdEntrante.PosisionSub
                        };

                        bool actualizarentrante = repoBodedas.ActualizarCantidad(I2);
                        if (actualizarentrante == false)
                        {
                            MG = new MensajesGeneral();
                            MG.Mensaje = "No se logro prestar el producto";
                            MG.TipoImagen = 1000;
                            MG.ShowDialog();
                            return;
                        }
                    }

                    //descontar de la saliente
                    int cantNue = Convert.ToInt32(textBox2.Text) - Convert.ToInt32(textBox3.Text);

                    INV_INVENTARIOBODEGAS I3 = new INV_INVENTARIOBODEGAS
                    {
                        Cantidad = cantNue,
                        Id = this.PosisionSub
                    };

                    bool update = repoBodedas.ActualizarCantidad(I3);
                    if (update == true)
                    {
                        MG = new MensajesGeneral();
                        MG.Mensaje = "Hecho";
                        MG.TipoImagen = 3;
                        MG.ShowDialog();

                        VerInventario f7 = Application.OpenForms.OfType<VerInventario>().LastOrDefault();
                        f7.CargarInventario();

                        this.Dispose();
                        this.Close();
                    }
                    else
                    {
                        MG = new MensajesGeneral();
                        MG.Mensaje = "No se logro consumir el producto";
                        MG.TipoImagen = 1000;
                        MG.ShowDialog();
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
