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
    public partial class Proveedores3 : ConfigForm.BaseForm
    {
        private static readonly IProveedores repoProv = new MProveedores();

        public string Code_Ac_Pro = "";
        int Codes;
        public Proveedores3()
        {
            InitializeComponent();          
        }
        
        private void Proveedores3_Load(object sender, EventArgs e)
        {
            try
            {
                Titulo.Text = "Proveedores";
                ConfigForm.SoloNumeros(textBox4);

                
                Proveedores_List();

                if (Code_Ac_Pro != "")
                {
                    var getProducto = repoProv.getProdById(Convert.ToInt32(Code_Ac_Pro));
                    if (getProducto != null)
                    {
                        button2.Enabled = true;
                        textBox1.Text = getProducto.Codigo_Pro.ToString();
                        textBox3.Text = getProducto.Cod_institucion.ToString();
                        textBox4.Text = Convert.ToInt32(getProducto.Valor_SinIva).ToString();

                        Ver_Proveedor(Convert.ToInt32(getProducto.Identificador_Pro));

                        textBox6.Text = getProducto.Tipo.ToString();
                    }
                    else
                    {
                        button2.Enabled = false;
                        MessageBox.Show("No se logro recuperar el articulo, vuelva a intentar");
                    }
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void Consulta_Codigo()
        {
            try
            {
                var getProveedor = repoProv.getCodProvbyName(comboBox1.Text);
                if (getProveedor == 0)
                {
                    MessageBox.Show("Error fatal de Proveedor contacte al desarrollador", "Error");
                    this.Dispose();
                    this.Close();                    
                }
                else
                {
                    Codes = Convert.ToInt32(getProveedor);
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
                Consulta_Codigo();

                var getProdProveedor = repoProv.getProducto(textBox1.Text, Codes);
                if (getProdProveedor != null)
                {
                    MessageBox.Show("Este articulo ya existe con este proveedor");
                }
                else
                {
                    if (textBox2.Text == "") { MessageBox.Show("Debe diligenciar todos los campos"); return; }
                    if (textBox3.Text == "") { MessageBox.Show("Debe diligenciar todos los campos"); return; }
                    if (textBox4.Text == "") { MessageBox.Show("Debe diligenciar todos los campos"); return; }
                    if (textBox6.Text == "") { MessageBox.Show("Debe diligenciar todos los campos"); return; }
                    if (textBox1.Text == "") { MessageBox.Show("Debe diligenciar todos los campos"); return; }

                    Inventario_Proveedores P = new Inventario_Proveedores
                    {
                        Codigo_Pro = textBox1.Text,
                        Item = textBox2.Text,
                        Cod_institucion = textBox3.Text,
                        Valor_SinIva = textBox4.Text,
                        Identificador_Pro = Codes,
                        Tipo = textBox6.Text
                    };

                    bool createProd = repoProv.createInventario(P);
                    if (createProd != true)
                    {
                        MessageBox.Show("No se logro crear el producto", "Error interno", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        MessageBox.Show("Creado con exito!!");

                        this.Dispose();
                        this.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void Proveedores_List()
        {
            try
            {
                var getProv = repoProv.getListadoProvs();
                if (getProv != null)
                {
                    foreach (var i in getProv)
                    {
                        comboBox1.Items.Add(i.Nombre);
                    }

                    comboBox1.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void Ver_Proveedor(int Code)
        {
            try
            {
                string getCodProv = repoProv.getNameProvbCode(Code);
                if (getCodProv == "")
                {
                    MessageBox.Show("Error fatal de Proveedor contacte al desarrollador", "Error");
                    this.Dispose();
                    this.Close();
                }
                else
                {
                    comboBox1.Text = getCodProv.ToString();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (textBox2.Text == "") { MessageBox.Show("Debe diligenciar todos los campos"); return; }
                if (textBox3.Text == "") { MessageBox.Show("Debe diligenciar todos los campos"); return; }
                if (textBox4.Text == "") { MessageBox.Show("Debe diligenciar todos los campos"); return; }
                if (textBox6.Text == "") { MessageBox.Show("Debe diligenciar todos los campos"); return; }
                if (textBox1.Text == "") { MessageBox.Show("Debe diligenciar todos los campos"); return; }

                Inventario_Proveedores P = new Inventario_Proveedores
                {
                    Item = textBox2.Text,
                    Cod_institucion = textBox3.Text,
                    Valor_SinIva = textBox4.Text,
                    Tipo = textBox6.Text,
                    Id = Convert.ToInt32(Code_Ac_Pro)
                };

                bool update = repoProv.updateInventario2(P);
                if (update != true)
                {
                    MessageBox.Show("No se logro actualizar");
                }
                else
                {
                    MessageBox.Show("Editado con exito!!");
                    this.Dispose();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
    }
}
