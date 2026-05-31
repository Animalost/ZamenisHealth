using Domain;
using Domain.CXN;
using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.Recepcion.Ventas
{
    public partial class Vender : Forma
    {
        private IVender oController;
        private int IdPac, IdCia;
        private CXN_PACIENTES P = new CXN_PACIENTES();
        private CXN_CIA C = new CXN_CIA();
        private MensajesGeneral MG;

        private int filaSeleccionada = -1;
        private int columnaSeleccionada = -1;

        public Vender(int idPac, int idCia)
        {
            InitializeComponent();
            IdPac = idPac;
            IdCia = idCia;
            oController = new MVender();
            SoloNumeros(textBox7);
            SoloNumeros(textBox8);
        }

        private void Vender_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Modulo de Ventas";
            LogoMain.Image = Properties.Resources.Splash;
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";

            ToolStripButton btnGenerar = new ToolStripButton();
            btnGenerar = createToolButton("Terminar");
            MenuLateral.Items.Add(btnGenerar);
            btnGenerar.Click += btnGenerar_Click;

            ToolStripButton btnHistorial = new ToolStripButton();
            btnHistorial = createToolButton("Historial");
            MenuLateral.Items.Add(btnHistorial);
            btnHistorial.Click += btnHistorial_Click;

            CargarDatos();
            CargarMediosPago();
            Grid();
        }
        void btnHistorial_Click(object sender, EventArgs e)
        {
            HistorialVentas h = new HistorialVentas(IdPac);
            h.ShowDialog();
        }
        void btnGenerar_Click(object sender, EventArgs e)
        {
            try
            {
                if (comboBox5.Text == "")
                {
                    MG = new MensajesGeneral()
                    {
                        Mensaje = "Seleccione medio de pago",
                        TipoImagen = 1000
                    };

                    MG.ShowDialog();
                }
                else if (comboBox2.Text == "")
                {
                    MG = new MensajesGeneral()
                    {
                        Mensaje = "Seleccione forma de pago",
                        TipoImagen = 1000
                    };

                    MG.ShowDialog();
                }
                else if (comboBox1.Text == "")
                {
                    MG = new MensajesGeneral()
                    {
                        Mensaje = "Seleccione metodo de pago",
                        TipoImagen = 1000
                    };

                    MG.ShowDialog();
                }
                else if (string.IsNullOrEmpty(textBox7.Text) == true)
                {
                    MG = new MensajesGeneral()
                    {
                        Mensaje = "Digite los dias de vencimiento de la factura",
                        TipoImagen = 1000
                    };

                    MG.ShowDialog();
                }
                else if (string.IsNullOrEmpty(textBox8.Text) == true)
                {
                    MG = new MensajesGeneral()
                    {
                        Mensaje = "Digite el porcentaje de descuento de la factura",
                        TipoImagen = 1000
                    };

                    MG.ShowDialog();
                }
                else if (IdPac == 0)
                {
                    MG = new MensajesGeneral()
                    {
                        Mensaje = "No ha seleccionado ningun cliente",
                        TipoImagen = 1000
                    };

                    MG.ShowDialog();
                }
                else if (IdCia == 0)
                {
                    MG = new MensajesGeneral()
                    {
                        Mensaje = "No ha seleccionado ningun prestador",
                        TipoImagen = 1000
                    };

                    MG.ShowDialog();
                }
                else
                {
                    if (textBox6.Text == "$ 0")
                    {
                        MG = new MensajesGeneral()
                        {
                            Mensaje = "No se puede facturar sin cargos agregados",
                            TipoImagen = 1000
                        };

                        MG.ShowDialog();
                    }
                    else
                    {
                        CXN_VENTAS_TEMP v = new CXN_VENTAS_TEMP()
                        {
                            Descuento = Convert.ToInt32(textBox8.Text),
                            Vencimiento = Convert.ToInt32(textBox7.Text),
                            IdCia = IdCia,
                            IdPac = IdPac,
                            RICA = label24.Text,
                            RFuente = label23.Text,
                            FormaPago = comboBox2.Text,
                            MedioPago = comboBox5.Text,
                            MetodoPago = comboBox1.Text,
                            Cargos = new List<CXN_VENTAS>()
                        };

                        foreach (DataGridViewRow i in dataGridView2.Rows)
                        {
                            if (!i.IsNewRow &&
                                    i.Cells["Codigo"].Value != null &&
                                    i.Cells["Item"].Value != null &&
                                    i.Cells["Cantidad"].Value != null &&
                                    i.Cells["Unitario"].Value != null &&
                                    i.Cells["Total"].Value != null)
                            {
                                v.Cargos.Add(new CXN_VENTAS
                                {
                                    Ven_Cod = i.Cells["Codigo"].Value.ToString(),
                                    Ven_Item = i.Cells["Item"].Value.ToString(),
                                    Ven_Cantidad = Convert.ToInt32(i.Cells["Cantidad"].Value.ToString()),
                                    Ven_Precio = Convert.ToInt32(i.Cells["Unitario"].Value.ToString()),
                                    Ven_Total = Convert.ToInt32(i.Cells["Total"].Value.ToString())
                                });
                            }                            
                        }

                        Terminar T = new Terminar(v);
                        T.ShowDialog();
                    }                    
                }                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        void CargarMediosPago()
        {
            try
            {
                List<CXN_MEDIOSPAGO> getMedios = oController.ListaMediosPago();
                if (getMedios != null)
                {
                    comboBox5.Items.Clear();

                    foreach (CXN_MEDIOSPAGO m in getMedios)
                    {
                        comboBox5.Items.Add(m.Medio);
                    }

                    comboBox5.Text = "Consignación bancaria";
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        void CargarDatos()
        {
            P = oController.LlamarPacientebyId(IdPac);
            C = oController.getPrestadorbyCode(IdCia);

            textBox5.Text = C.Com_Nombre.Trim();
            textBox1.Text = P.Pac_PrimerA + " " + P.Pac_SegundoA + " " + P.Pac_PrimerN + " " + P.Pac_SegundoN;
            textBox2.Text = P.Pac_TipoId + " " + P.Pac_IdNum;
            textBox3.Text = P.Pac_Telefono;
            textBox4.Text = P.Pac_Email;
        }
        private void dataGridView2_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                dataGridView2.CellValueChanged -= dataGridView2_CellValueChanged;

                if (dataGridView2.Rows[e.RowIndex].Cells[0].Value != null &&
                !string.IsNullOrEmpty(dataGridView2.Rows[e.RowIndex].Cells[0].Value.ToString()))
                {
                    CXN_INVENTARIO getdatCode = oController.getProductbyCode(dataGridView2.Rows[e.RowIndex].Cells[0].Value.ToString());
                    if (getdatCode != null)
                    {
                        dataGridView2.Rows[e.RowIndex].Cells[1].Value = getdatCode.InvItem;
                        dataGridView2.Rows[e.RowIndex].Cells[3].Value = Convert.ToInt32(getdatCode.InvPrecio);

                        if (dataGridView2.Rows[e.RowIndex].Cells[2].Value != null &&
                            !string.IsNullOrEmpty(dataGridView2.Rows[e.RowIndex].Cells[2].Value.ToString()) &&
                            dataGridView2.Rows[e.RowIndex].Cells[3].Value != null &&
                            !string.IsNullOrEmpty(dataGridView2.Rows[e.RowIndex].Cells[3].Value.ToString()))
                        {
                            dataGridView2.Rows[e.RowIndex].Cells[4].Value = Convert.ToInt32(dataGridView2.Rows[e.RowIndex].Cells[2].Value) * Convert.ToInt32(dataGridView2.Rows[e.RowIndex].Cells[3].Value);
                        }
                        else
                        {
                            //dataGridView2.Rows[e.RowIndex].Cells[3].Value = 0;
                            dataGridView2.Rows[e.RowIndex].Cells[4].Value = 0;
                        }
                    }
                    else
                    {
                        dataGridView2.Rows[e.RowIndex].Cells[1].Value = null;
                        //dataGridView2.Rows[e.RowIndex].Cells[3].Value = null;
                        dataGridView2.Rows[e.RowIndex].Cells[4].Value = null;
                    }
                }
                else
                {
                    dataGridView2.Rows[e.RowIndex].Cells[1].Value = null;
                    //dataGridView2.Rows[e.RowIndex].Cells[3].Value = null;
                    dataGridView2.Rows[e.RowIndex].Cells[4].Value = null;
                }

                int tot = 0;

                foreach (DataGridViewRow i in dataGridView2.Rows)
                {
                    if (i.Cells["Total"].Value != null)
                    {
                        tot = tot + Convert.ToInt32(i.Cells["Total"].Value);                        
                    }
                }

                textBox6.Text = "$ " + Convert.ToInt32(tot).ToString("N0");
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
            finally
            {
                // Reactivar el evento CellValueChanged después de modificar las celdas
                dataGridView2.CellValueChanged += dataGridView2_CellValueChanged;
            }
        }
        private void dataGridView2_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            dataGridView2.BeginEdit(true);
        }
        private void label23_DoubleClick(object sender, EventArgs e)
        {
            Facturacion.Extras.Retenciones retenciones = new Facturacion.Extras.Retenciones("Fuente", "Ventas");
            retenciones.ShowDialog();
        }
        private void label24_DoubleClick(object sender, EventArgs e)
        {
            Facturacion.Extras.Retenciones retenciones = new Facturacion.Extras.Retenciones("Ica", "Ventas");
            retenciones.ShowDialog();
        }
        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == 0) //Codigo
                {
                    filaSeleccionada = e.RowIndex;
                    columnaSeleccionada = e.ColumnIndex;

                    BuscarProducto bP = new BuscarProducto(this);
                    bP.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public void setValores(string Valor)
        {
            if (filaSeleccionada >= 0 && columnaSeleccionada >= 0)
            {
                dataGridView2.Rows[filaSeleccionada].Cells[columnaSeleccionada].Value = Valor;
            }

            this.BeginInvoke(new Action(() =>
            {
                textBox7.Focus();
            }));
        }

        void Grid()
        {
            int numberOfRows = 13; // Número de filas vacías a agregar
            for (int i = 0; i < numberOfRows; i++)
            {
                dataGridView2.Rows.Add();
            }
        }
    }
}
