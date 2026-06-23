using Domain.CXN;
using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.AdminSystem.Material
{
    public partial class EdicionProductos : Forma
    {
        private IInventario oInventario;
        private IAseguradoras oAseguradora;

        private MensajesGeneral MG;
        private int Posision, Aseguradora;
        private bool Nuevo;

        public EdicionProductos(int posision, int aseguradora)
        {
            InitializeComponent();
            Posision = posision;
            Aseguradora = aseguradora;

            oInventario = new MInventario();
            oAseguradora = new MAseguradoras();
        }

        private void EdicionProductos_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Edicion de Productos";
            LogoMain.Image = Properties.Resources.Splash;
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";

            ToolStripButton btnGenerar = new ToolStripButton();
            btnGenerar = createToolButton("Grabar");
            MenuLateral.Items.Add(btnGenerar);
            btnGenerar.Click += button1_Click;

            CargarAses();

            if (Posision == 0)
            {
                Nuevo = true;
            }
            else
            {
                Nuevo = false;
             
                CXN_INVENTARIO P = oInventario.getProductbyId(Posision);
                if (P != null)
                {
                    textBox1.Text = P.InvCod; textBox1.Enabled = false;
                    textBox2.Text = P.InvItem;
                    textBox3.Text = P.InvInvima;
                    textBox4.Text = Convert.ToInt32(P.InvPrecio).ToString();
                    textBox5.Text = P.InvDetalle;
                    comboBox1.Text = P.InvTipo;
                    comboBox2.Text = P.InvCobro;
                    comboBox3.Text = oAseguradora.getInfoFromAsebyCode(P.InvConvenio).Ase_Descripcion; comboBox3.Enabled = false;
                }
                else
                {
                    MG = new MensajesGeneral()
                    {
                        Mensaje = "No se logro cargar el producto seleccionado",
                        TipoImagen = 1000
                    };
                    MG.ShowDialog();

                    this.Close();
                }
            }
        }
        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            Aseguradora = oAseguradora.getInfoFromAsebyName(comboBox3.Text).Ase_Identificador;
        }
        void CargarAses()
        {
            List<CXN_ASEGURADORA> lista = oAseguradora.getAseguradoras();
            if (lista != null)
            {
                foreach (var i in lista)
                {
                    comboBox3.Items.Add(i.Ase_Descripcion);
                }

                comboBox3.SelectedIndex = 0;
            }
        }
        void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(textBox1.Text) || string.IsNullOrEmpty(textBox2.Text) || string.IsNullOrEmpty(textBox3.Text) 
                    || string.IsNullOrEmpty(textBox4.Text) || comboBox1.Text == "" || comboBox2.Text == "" || comboBox3.Text == "")
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
                    CXN_INVENTARIO I = new CXN_INVENTARIO()
                    {
                        InvCobro = comboBox2.Text,
                        InvCod = textBox1.Text,
                        InvId = Posision,
                        InvConvenio = Aseguradora,
                        InvDetalle = textBox5.Text,
                        InvUsrGraba = Contenedor.UsuarioLogueado,
                        InvInvima = textBox3.Text,
                        InvItem = textBox2.Text,
                        InvPrecio = Convert.ToInt32(textBox4.Text),
                        InvTipo = comboBox1.Text,
                        InvFechaCre = DateTime.Now.Date,
                        InvCodBar = textBox1.Text
                    };

                    if (Nuevo == true)
                    {
                        var Inv = oInventario.ConsultarValor(Aseguradora, textBox1.Text);
                        if (Inv.item != "")
                        {
                            MG = new MensajesGeneral()
                            {
                                Mensaje = "Este codigo de producto ya existe, debe elegir otro",
                                TipoImagen = 1000
                            };
                            MG.ShowDialog();
                        }
                        else
                        {
                            bool create = oInventario.CrearProducto(I);
                            if (create != true)
                            {
                                MG = new MensajesGeneral()
                                {
                                    Mensaje = "No se logro crear el producto",
                                    TipoImagen = 1000
                                };
                                MG.ShowDialog();
                            }
                            else
                            {
                                MG = new MensajesGeneral()
                                {
                                    Mensaje = "Creado con exito",
                                    TipoImagen = 3
                                };
                                MG.ShowDialog();

                                this.Close();
                            }
                        }
                    }
                    else
                    {
                        bool update = oInventario.updateProducto(I);
                        if (update != true)
                        {
                            MG = new MensajesGeneral()
                            {
                                Mensaje = "No se logro actualizar el producto",
                                TipoImagen = 1000
                            };
                            MG.ShowDialog();
                        }
                        else
                        {
                            MG = new MensajesGeneral()
                            {
                                Mensaje = "Actualizado con exito",
                                TipoImagen = 3
                            };
                            MG.ShowDialog();

                            this.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

