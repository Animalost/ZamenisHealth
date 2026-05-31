using Domain;
using Domain.CXN;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Drawing;
using System.Windows.Forms;
using ZamenisHealth.Clases;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.AdminSystem
{
    public partial class Proveedores : ConfigForm.BaseForm
    {
        private readonly static IProveedores repoProv = new MProveedores();
        private readonly static IReportes repoReportes = new MReportes();

        public Proveedores()
        {
            InitializeComponent();            
        }

        private void Proveedores_Load(object sender, EventArgs e)
        {
            try
            {
                Titulo.Text = "Proveedores";
                ConfigForm.SoloNumeros(textBox1);
                ConfigForm.SoloNumeros(textBox5);
                ConfigForm.SoloNumeros(textBox4);

                
                panel1.Location = new Point(6, 80);
                panel2.Location = new Point(6, 80);
                panel3.Location = new Point(6, 80);
                panel1.Size = new Size(846, 419);
                panel2.Size = new Size(846, 419);
                panel3.Size = new Size(846, 419);
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Blanco();
            panel1.Visible = false;
        }
        private void Blanco()
        {
            textBox1.Text = "";
            textBox9.Text = "";
            textBox10.Text = "";
            textBox11.Text = "";
            textBox12.Text = "";
            textBox13.Text = "";
            textBox14.Text = "";
        }

        void EncabezadosLv1()
        {
            listView1.Clear();
            listView1.View = View.Details;
            listView1.GridLines = true;
            listView1.FullRowSelect = true;
            listView1.Columns.Add("Codigo", 100, HorizontalAlignment.Left);
            listView1.Columns.Add("Proveedor", 450, HorizontalAlignment.Left);
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            try
            {
                panel2.Visible = false;
                panel3.Visible = false;
                textBox1.Enabled = true;
                Blanco();

                var getProvs = repoProv.getListadoProvs();
                if (getProvs != null)
                {
                    panel1.Visible = true;
                    EncabezadosLv1();

                    foreach (var i in getProvs)
                    {
                        listView1.Items.Add(new ListViewItem(new string[]
                        {
                            i.Codigo.ToString(),
                            i.Nombre.ToString()
                        }));
                    }
                }
                else
                {
                    EncabezadosLv1();
                    panel1.Visible = false;
                    MessageBox.Show("No hay proveedores creados hasta el momento");
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void listView1_Click(object sender, EventArgs e)
        {
            try
            {
                var getProv = repoProv.getProvByCode(Convert.ToInt32(listView1.SelectedItems[0].SubItems[0].Text));
                if (getProv != null)
                {
                    textBox1.Text = listView1.SelectedItems[0].SubItems[0].Text;
                    textBox9.Text = getProv.Nombre;
                    textBox10.Text = getProv.Identificacion;
                    textBox11.Text = getProv.Telefono;
                    textBox12.Text = getProv.Responsable;
                    textBox13.Text = getProv.Email;
                    textBox14.Text = getProv.Direccion;
                    textBox1.Enabled = false;
                }
                else
                {
                    Blanco();
                    MessageBox.Show("Error interno vuelva a intentar", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private static bool VAlidarExistente(int Cod)
        {
            try
            {
                var getProv = repoProv.getProvByCode(Convert.ToInt32(Cod));
                if (getProv != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return true;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                if (textBox1.Text == "")
                {
                    MessageBox.Show("No ha escogido ningun proveedor para editar",
                    "Faltan Datos",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
                    return;
                }
                if (textBox9.Text == "")
                {
                    MessageBox.Show("Debe escribir un nombre de proveedor",
                        "Faltan Datos",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
                    return;
                }

                CXN_PROVEEDORES P = new CXN_PROVEEDORES
                {
                    Nombre = textBox9.Text,
                    Identificacion = textBox10.Text,
                    Telefono = textBox11.Text,
                    Direccion = textBox14.Text,
                    Email = textBox13.Text,
                    Responsable = textBox12.Text,
                    Codigo = Convert.ToInt32(textBox1.Text)
                };

                Comunes.MensajesGeneral MG = new Comunes.MensajesGeneral();

                if (textBox1.Enabled == false)
                {
                    bool _updateP = repoProv.updateProveedor(P);
                    if (_updateP != true)
                    {
                        MG.TipoImagen = 1000;
                        MG.Mensaje = "No se logro actualizar el proveedor";
                        MG.ShowDialog();
                    }
                    else
                    {
                        Blanco();
                        panel1.Visible = false;

                        MG.TipoImagen = 3;
                        MG.Mensaje = "Proveedor gestionado";
                        MG.ShowDialog();
                    }
                }

                if (textBox1.Enabled == true)
                {
                    var ValidaExistente = VAlidarExistente(Convert.ToInt32(textBox1.Text));
                    if (ValidaExistente == true)
                    {
                        MessageBox.Show("Debe escoger otro codigo para crear proveedores",
                            "Repetido, " + ValidaExistente.ToString(),
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Exclamation);
                        return;
                    }

                    bool _createP = repoProv.createProveedor(P);
                    if (_createP != true)
                    {
                        MG.TipoImagen = 1000;
                        MG.Mensaje = "No se logro crear el proveedor";
                        MG.ShowDialog();
                    }
                    else
                    {
                        Blanco();
                        panel1.Visible = false;

                        MG.TipoImagen = 3;
                        MG.Mensaje = "Proveedor gestionado";
                        MG.ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            panel2.Visible = true;
            panel1.Visible = false;
            panel3.Visible = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            panel2.Visible = false;
        }
        private void EncabezadoList2()
        {
            listView2.Clear();
            listView2.View = View.Details;
            listView2.GridLines = true;
            listView2.FullRowSelect = true;
            listView2.Columns.Add("Fecha", 80, HorizontalAlignment.Left);
            listView2.Columns.Add("Proveedor", 250, HorizontalAlignment.Left);
            listView2.Columns.Add("Compañia", 250, HorizontalAlignment.Left);
            listView2.Columns.Add("Orden", 80, HorizontalAlignment.Left);
            listView2.Columns.Add("Estado", 80, HorizontalAlignment.Left);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                var getPed = repoProv.getPedidos(dateTimePicker1.Value.Date, dateTimePicker2.Value.Date);
                if (getPed != null)
                {
                    EncabezadoList2();

                    foreach (var i in getPed)
                    {
                        listView2.Items.Add(new ListViewItem(new string[]
                        {
                             Convert.ToDateTime(i.Fecha).ToString(Conexion.ConectionDictionary["Format_Fecha"]),
                             i.ObservacionG.ToString(),
                             i.Codigo_Prest.ToString(),
                             i.Num_Orden.ToString(),
                             i.Estado.ToString()
                        }));
                    }
                }
                else
                {
                    EncabezadoList2();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void listView2_Click(object sender, EventArgs e)
        {
            try
            {
                var Exporta = repoReportes.ExportaOrdenCompra(Convert.ToInt32(listView2.SelectedItems[0].SubItems[3].Text));
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

            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void Encabezadoslist3()
        {
            listView3.Clear();
            listView3.View = View.Details;
            listView3.GridLines = true;
            listView3.FullRowSelect = true;
            listView3.Columns.Add("Posision", 0, HorizontalAlignment.Left);
            listView3.Columns.Add("Codigo Proveedor", 80, HorizontalAlignment.Left);
            listView3.Columns.Add("Item", 250, HorizontalAlignment.Left);
            listView3.Columns.Add("Codigo Interno", 80, HorizontalAlignment.Left);
            listView3.Columns.Add("Valor Sin Iva", 80, HorizontalAlignment.Left);
            listView3.Columns.Add("Proveedor", 250, HorizontalAlignment.Left);
            listView3.Columns.Add("Consumo", 80, HorizontalAlignment.Left);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Blanco3();
            panel3.Visible = false;
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            try
            {
                panel3.Visible = true;
                panel1.Visible = false;
                panel2.Visible = false;

                var getInv = repoProv.getInventario();
                if (getInv != null)
                {
                    Encabezadoslist3();

                    foreach (var i in getInv)
                    {
                        listView3.Items.Add(new ListViewItem(new string[]
                        {
                             i.Id.ToString(),
                             i.Codigo_Pro.ToString(),
                             i.Item.ToString(),
                             i.Cod_institucion.ToString(),
                             i.Valor_SinIva.ToString(),
                             i.Identificador_Pro.ToString(),
                             i.Tipo.ToString()
                        }));
                    }
                }
                else
                {
                    Encabezadoslist3();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void Blanco3()
        {
            textBox8.Text = "";
            textBox7.Text = "";
            textBox6.Text = "";
            textBox5.Text = "";
            textBox4.Text = "";
            textBox3.Text = "";
            textBox2.Text = "";
            textBox4.Enabled = true;
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            this.Dispose();
            this.Close();

            Proveedores2 f = new Proveedores2();
            f.ShowDialog();
        }
      
        private void listView3_Click(object sender, EventArgs e)
        {
            try
            {
                var getProdId = repoProv.getProdById(Convert.ToInt32(listView3.SelectedItems[0].SubItems[0].Text));
                if (getProdId != null)
                {
                    textBox8.Text = getProdId.Codigo_Pro.ToString();
                    textBox7.Text = getProdId.Item.ToString();
                    textBox6.Text = getProdId.Cod_institucion.ToString();
                    textBox5.Text = getProdId.Valor_SinIva.ToString();
                    textBox4.Text = getProdId.Identificador_Pro.ToString();
                    textBox3.Text = getProdId.Tipo.ToString();
                    textBox2.Text = listView3.SelectedItems[0].SubItems[0].Text.ToString();
                    textBox4.Enabled = false;
                }
                else
                {
                    Blanco3();
                    MessageBox.Show("Error interno vuelva a intentar", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                Comunes.MensajesGeneral MG = new Comunes.MensajesGeneral();

                Inventario_Proveedores P = new Inventario_Proveedores
                {
                    Codigo_Pro = textBox8.Text,
                    Item = textBox7.Text,
                    Cod_institucion = textBox6.Text,
                    Valor_SinIva = textBox5.Text,
                    Tipo = textBox3.Text,
                    Id = Convert.ToInt32(textBox2.Text)
                };

                bool _updateInv = repoProv.updateInventario(P);
                if (_updateInv != true)
                {
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "No se logro actualizar";
                    MG.ShowDialog();
                }
                else
                {
                    MG.TipoImagen = 3;
                    MG.Mensaje = "Actualizado con exito";
                    MG.ShowDialog();

                    Blanco3();
                    panel3.Visible = false;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            try
            {
                var ValidaProExi = VAlidarExistente(Convert.ToInt32(textBox4.Text));
                if (ValidaProExi == false)
                {
                    MessageBox.Show("El codigo de proveedor no existe, validelo en la opcion correspondiente de " +
                        "Crear/Editar Proveedores",
                        "Proveedor No Encontrado",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }

                Comunes.MensajesGeneral MG = new Comunes.MensajesGeneral();

                Inventario_Proveedores P = new Inventario_Proveedores
                {
                    Codigo_Pro = textBox8.Text,
                    Item = textBox7.Text,
                    Cod_institucion = textBox6.Text,
                    Valor_SinIva = textBox5.Text,
                    Identificador_Pro = Convert.ToInt32(textBox4.Text),
                    Tipo = textBox3.Text
                };

                bool _createInv = repoProv.createInventario(P);
                if (_createInv != true)
                {
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "No se logro crear";
                    MG.ShowDialog();
                }
                else
                {
                    MG.TipoImagen = 3;
                    MG.Mensaje = "creado con exito";
                    MG.ShowDialog();

                    Blanco3();
                    panel3.Visible = false;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

    }
}
