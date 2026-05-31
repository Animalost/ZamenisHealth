using Domain.CXN;
using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Windows.Forms;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.Gerenciales
{
    public partial class Cruces : Forma
    {
        private ICruces repoCruces;

        private MensajesGeneral MG;
        private int PosTable = 0;

        public Cruces()
        {
            InitializeComponent();
            repoCruces = new MCruces();
        }

        private void Cruces_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Cierre de Caja";
            LogoMain.Image = Properties.Resources.Splash;
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";

            ToolStripButton btnBuscar;

            btnBuscar = new ToolStripButton();
            btnBuscar = createToolButton("Buscar");
            MenuLateral.Items.Add(btnBuscar);
            btnBuscar.Click += btnBuscar_Click;
        }
        void btnBuscar_Click(object sender, EventArgs e) 
        { 
            try
            {
                if (comboBox1.Text == "")
                {
                    panel1.Visible = false;
                    PosTable = 0;

                    MG = new MensajesGeneral
                    {
                        Mensaje = "Debe seleccionar el area a cruzar",
                        TipoImagen = 1000
                    };

                    MG.ShowDialog();
                }
                else if (string.IsNullOrEmpty(textBox1.Text))
                {
                    panel1.Visible = false;
                    PosTable = 0;

                    MG = new MensajesGeneral
                    {
                        Mensaje = "Debe digitar el numero de factura electronica",
                        TipoImagen = 1000
                    };

                    MG.ShowDialog();
                }
                else
                {                    
                    switch(comboBox1.Text)
                    {
                        case "Ventas Recepcion":
                            VentasRec();
                            break;

                        case "Recaudo de Vales y/o Bonos":
                            RecaudoBonos();
                            break;

                        case "Particulares":
                            Particulares();
                            break;

                        default:
                            panel1.Visible = false;
                            PosTable = 0;

                            MG = new MensajesGeneral()
                            {
                                Mensaje = "Seleccion Invalida",
                                TipoImagen = 1000
                            };

                            MG.ShowDialog();
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        void VentasRec()
        {
            try
            {
                CXN_VENTAS ventas = repoCruces.GetVentaRecepcion(textBox1.Text.Trim());
                if (ventas != null)
                {
                    if (ventas.Grafica == "X")
                    {
                        textBox2.Text = ventas.Ven_Factura.ToString().Trim();
                        textBox3.Text = Convert.ToDateTime(ventas.Ven_Fecha).ToString("yyyy-MM-dd");
                        textBox4.Text = ventas.Ven_Usr_Graba.ToString().Trim();
                        textBox5.Text = ventas.Ven_Item.ToString().Trim();
                        PosTable = ventas.Ven_Id;

                        panel1.Visible = true;
                    }
                    else
                    {
                        panel1.Visible = false;
                        PosTable = 0;

                        MG = new MensajesGeneral
                        {
                            Mensaje = $"Esta factura electronica ya fue cruzada",
                            TipoImagen = 3
                        };

                        MG.ShowDialog();
                    }                       
                }
                else
                {
                    panel1.Visible = false;
                    PosTable = 0;

                    MG = new MensajesGeneral
                    {
                        Mensaje = $"Esta factura electronica no existe para { comboBox1.Text }",
                        TipoImagen = 1000
                    };

                    MG.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        void RecaudoBonos()
        {
            try
            {
                CXN_HORARIO bonos = repoCruces.GetRecaudosBonos(textBox1.Text.Trim());
                if (bonos != null)
                {
                    if (bonos.Hor_ValDerechos == "X")
                    {
                        textBox2.Text = bonos.Hor_Id.ToString().Trim();
                        textBox3.Text = Convert.ToDateTime(bonos.Hor_Pac_Fecha_Cita).ToString("yyyy-MM-dd");
                        textBox4.Text = bonos.Hor_Imp_Age.ToString().Trim();
                        textBox5.Text = bonos.Hor_Pac_Tipo_Serv.ToString().Trim();
                        PosTable = bonos.Hor_Id;

                        panel1.Visible = true;
                    }
                    else
                    {
                        panel1.Visible = false;
                        PosTable = 0;

                        MG = new MensajesGeneral
                        {
                            Mensaje = $"Esta factura electronica ya fue cruzada",
                            TipoImagen = 3
                        };

                        MG.ShowDialog();
                    }                   
                }
                else
                {
                    panel1.Visible = false;
                    PosTable = 0;

                    MG = new MensajesGeneral
                    {
                        Mensaje = $"Esta factura electronica no existe para {comboBox1.Text}",
                        TipoImagen = 1000
                    };

                    MG.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        void Particulares()
        {
            try
            {
                CXN_FACTURA bonos = repoCruces.Particulares(textBox1.Text.Trim());
                if (bonos != null)
                {
                    if (bonos.QRCufe == "X")
                    {
                        textBox2.Text = bonos.Fac_Id.ToString().Trim();
                        textBox3.Text = Convert.ToDateTime(bonos.Fac_Fecha).ToString("yyyy-MM-dd");
                        textBox4.Text = bonos.Fac_Usr_Graba.ToString().Trim();
                        textBox5.Text = bonos.FacResNumeracion.ToString().Trim();
                        PosTable = bonos.Fac_Id;

                        panel1.Visible = true;
                    }
                    else
                    {
                        panel1.Visible = false;
                        PosTable = 0;

                        MG = new MensajesGeneral
                        {
                            Mensaje = $"Esta factura electronica ya fue cruzada",
                            TipoImagen = 3
                        };

                        MG.ShowDialog();
                    }                   
                }
                else
                {
                    panel1.Visible = false;
                    PosTable = 0;

                    MG = new MensajesGeneral
                    {
                        Mensaje = $"Esta factura electronica no existe para {comboBox1.Text}",
                        TipoImagen = 1000
                    };

                    MG.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void boton1_Click(object sender, EventArgs e)
        {
            try
            {
                switch(comboBox1.Text)
                {
                    case "Ventas Recepcion":
                        Cruzar("Ventas");
                        break;

                    case "Recaudo de Vales y/o Bonos":
                        Cruzar("Bonos");
                        break;

                    case "Particulares":
                        Cruzar("Particulares");
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        void Cruzar(string Tabla)
        {
            try
            {
                bool update = repoCruces.Cruzar(PosTable, Tabla, Contenedor.UsuarioLogueado);
                if (update == true)
                {
                    panel1.Visible = false;
                    PosTable = 0;

                    MG = new MensajesGeneral
                    {
                        Mensaje = "Hecho",
                        TipoImagen = 3
                    };

                    MG.ShowDialog();
                }
                else
                {
                    MG = new MensajesGeneral
                    {
                        Mensaje = "No se logro actualizar, intente mas tarde",
                        TipoImagen = 1000
                    };

                    MG.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
