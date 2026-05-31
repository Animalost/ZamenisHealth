using Domain;
using Domain.CXN;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Windows.Forms;
using ZamenisHealth.Clases;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.AdminSystem
{
    public partial class Proveedores2 : ConfigForm.BaseForm
    {
        private static readonly IProveedores repoProv = new MProveedores();
        private static readonly ICompañia repoCompañia = new MCompañia();
        private static readonly IReportes repoReportes = new MReportes();

        int Num_Ord, Cia, Cod_Pro;

        private void Proveedores2_Load(object sender, EventArgs e)
        {
            try
            {
                Titulo.Text = "Proveedores";
                ConfigForm.SoloNumeros(textBox2);

                Titulo.Visible = false;
                ImageClose.Visible = false;

                

                var getProvs = repoProv.getListadoProvs();
                if (getProvs != null)
                {
                    foreach (var i in getProvs)
                    {
                        comboBox1.Items.Add(i.Nombre);
                    }

                    comboBox1.SelectedIndex = 0;
                }


                var Cias = repoCompañia.getAllCompañias();
                if (Cias != null)
                {
                    foreach (var i in Cias)
                    {
                        comboBox2.Items.Add(i.Com_Nombre);
                    }

                    comboBox2.SelectedIndex = 0;
                }

                var NO = repoCompañia.getPrestadorbyCode(Cia);
                if (NO == null)
                {
                    MessageBox.Show("Error en numero de orden de pedido",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    this.Dispose();
                    this.Close();

                    Proveedores f = new Proveedores();
                    f.ShowDialog();
                    return;
                }

                Num_Ord = Convert.ToInt32(NO.Com_PedPro);
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult result = MessageBox.Show("Al salir se eliminaran los productos cargados, " +
                       "¿Desea eliminar esta orden?",
                       "Zamenis_Health", MessageBoxButtons.YesNo);

                if (result == DialogResult.Yes)
                {
                    repoProv.deleteProds(Num_Ord);

                    this.Dispose();
                    this.Close();

                    Proveedores f = new Proveedores();
                    f.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                Comunes.MensajesGeneral MG = new Comunes.MensajesGeneral();

                if (Cod_Pro == 0) { MessageBox.Show("Debe diligenciar y/o seleccionar los campos"); return; }
                if (Cia == 0) { MessageBox.Show("Debe diligenciar y/o seleccionar los campos"); return; }
                if (textBox1.Text == "") { MessageBox.Show("Debe diligenciar y/o seleccionar los campos"); return; }
                if (textBox2.Text == "") { MessageBox.Show("Debe diligenciar y/o seleccionar los campos"); return; }
                if (label6.Text == "") { MessageBox.Show("Debe diligenciar y/o seleccionar los campos"); return; }
                if (label7.Text == "") { MessageBox.Show("Debe diligenciar y/o seleccionar los campos"); return; }
                if (label8.Text == "") { MessageBox.Show("Debe diligenciar y/o seleccionar los campos"); return; }

                DateTime Hoy = DateTime.Now;
                DateTime FECHA = Convert.ToDateTime(Hoy);

                string Observa = "";
                if (textBox3.Text == "")
                {
                    Observa = "Sin Observaciones";
                }
                else
                {
                    Observa = textBox3.Text;
                }

                CXN_PEDIDOS P = new CXN_PEDIDOS
                {
                    Cod_Pro = Cod_Pro,
                    Cod_Ins = Cia.ToString(),
                    Item = label6.Text,
                    Fecha = FECHA,
                    Num_Pedido = Num_Ord.ToString(),
                    Usuario = Comunes.Contenedor.UsuarioLogueado,
                    Estado = "P",
                    Cod_ItemI = label7.Text,
                    Cantidad = Convert.ToInt32(textBox2.Text),
                    Cod_Item_Pro = textBox1.Text,
                    Tipo = label8.Text,
                    Observacion = Observa
                };

                bool addP = repoProv.AddProducto(P);
                if (addP != true)
                {
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "No se logro agregar el producto a la orden";
                    MG.ShowDialog();
                }
                else
                {
                    Carga_Grid();

                    MessageBox.Show("Agregado", "Agregado", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    textBox1.Text = "";
                    textBox2.Text = "";
                    label6.Text = "";
                    label7.Text = "";
                    label8.Text = "";
                    textBox3.Text = "";
                    Observa = "";
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void Encabezados()
        {
            listView1.Clear();
            listView1.View = View.Details;
            listView1.GridLines = true;
            listView1.FullRowSelect = true;
            listView1.Columns.Add("Id", 0, HorizontalAlignment.Left);
            listView1.Columns.Add("Codigo", 80, HorizontalAlignment.Left);
            listView1.Columns.Add("Item", 250, HorizontalAlignment.Left);
            listView1.Columns.Add("Cantidad", 80, HorizontalAlignment.Left);
        }

        private void Carga_Grid()
        {
            try
            {
                var get_Added = repoProv.getProdAdded(Convert.ToInt32(Num_Ord));
                if (get_Added != null)
                {
                    Encabezados();
                    toolStripButton3.Enabled = true;

                    foreach (var i in get_Added)
                    {
                        listView1.Items.Add(new ListViewItem(new string[]
                        {
                            i.Id.ToString(),
                            i.Cod_ItemI.ToString(),
                            i.Item.ToString(),
                            i.Cantidad.ToString()
                        }));
                    }
                }
                else
                {
                    toolStripButton3.Enabled = false;
                    Encabezados();
                }
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
                Comunes.MensajesGeneral MG = new Comunes.MensajesGeneral();

                DateTime Hoy = DateTime.Now;
                DateTime FECHA = Convert.ToDateTime(Hoy);

                string ObservacionG = textBox4.Text;

                CXN_PEDIDOSF f = new CXN_PEDIDOSF
                {
                    Codigo_Prest = Cia.ToString(),
                    Codigo_Provee = Cod_Pro,
                    Num_Orden = Num_Ord.ToString(),
                    Fecha = FECHA,
                    Estado = "G",
                    ObservacionG = ObservacionG
                };

                bool insertP = repoProv.InsertarPedido(f);
                if (insertP != true)
                {
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "No se ha logrado crear el pedido";
                    MG.ShowDialog();
                }
                else
                {
                    int Nue = Convert.ToInt32(Num_Ord) + 1;
                    var Act = repoCompañia.ConsecutivoActualiza(Cia, "PEDPRO", Nue);
                    if (Act == false)
                    {
                        MessageBox.Show("No se logro actualizar el consecutivo de ordenes de pedido, valide este numero antes de crear otra orden",
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }

                    MessageBox.Show("Orden " + Num_Ord + " creada con exito!!",
                        "Hecho",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation);

                    Exporta_Orden();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void Exporta_Orden()
        {
            try
            {
                var Exporta = repoReportes.ExportaOrdenCompra(Convert.ToInt32(Num_Ord));
                if (Exporta == null)
                {
                    MessageBox.Show("No se logro exportar la orden",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }

                ConfigForm.GenerarReportViewer("DataSet_OrdenC",
              "ZamenisHealth.Reportes.RDLC_OrdenC.rdlc",
              Exporta);

                this.Dispose();
                this.Close();

                Proveedores f = new Proveedores();
                f.ShowDialog();
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyData == Keys.Enter)
                {

                    var getProdu = repoProv.getProducto(textBox1.Text, Cod_Pro);
                    if (getProdu != null)
                    {
                        button1.Enabled = true;
                        label6.Text = getProdu.Item.ToString();
                        label7.Text = getProdu.Cod_institucion.ToString();
                        label8.Text = getProdu.Tipo.ToString();
                    }
                    else
                    {
                        label6.Text = "";
                        label7.Text = "";
                        label8.Text = "";

                        DialogResult result = MessageBox.Show("Producto no existe con este proveedor, ¿desea crearlo?",
                            "Zamenis Health - Ordenes de Compra",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question);

                        if (result == DialogResult.Yes)
                        {
                            Proveedores3 proveedores3 = new Proveedores3();
                            proveedores3.button1.Enabled = true;
                            proveedores3.ShowDialog();
                        }
                        if (result == DialogResult.No)
                        {
                            label6.Text = "";
                            label7.Text = "";
                            label8.Text = "";
                            button1.Enabled = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                Comunes.MensajesGeneral MG = new Comunes.MensajesGeneral();

                bool delPos = repoProv.deleteProdFromOrder(Convert.ToInt32(listView1.SelectedItems[0].SubItems[0].Text));
                if (delPos != true)
                {
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "No se ha podido eleiminar este producto de la orden";
                    MG.ShowDialog();
                }
                else
                {
                    Carga_Grid();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                Comunes.MensajesGeneral MG = new Comunes.MensajesGeneral();

                int getCodP = repoProv.getCodProvbyName(comboBox1.Text);
                if (getCodP == 0)
                {
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "Este proveedor presenta inconvenientes";
                    MG.ShowDialog();

                    this.Dispose();
                    this.Close();

                    Proveedores f = new Proveedores();
                    f.ShowDialog();
                }
                else
                {
                    Cod_Pro = Convert.ToInt32(getCodP);
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                var P = repoCompañia.getPrestadorbyName(comboBox2.Text);
                Cia = P.Com_Identificador;
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        public Proveedores2()
        {
            InitializeComponent();
            
        }        
    }
}
