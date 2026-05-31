using Domain;
using Domain.INV;
using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using Persistence.INV.Interfaces;
using Persistence.INV.Metodos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.INV
{
    public partial class MasivoPpal : Forma
    {
        private MensajesGeneral MG;
        private IBodegaPrincipal principal;
        private IHomologos repoHomologos;
        private IProductos repoProductos;
        private OpenFileDialog openFileDialog;

        public MasivoPpal()
        {
            InitializeComponent();
            principal = new MBodegaPrincipal();
            repoHomologos = new MHomologos();
            repoProductos = new MProductos();
        }

        private void MasivoPpal_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Importar";
            LogoMain.Image = Properties.Resources.Splash;
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";

            ToolStripButton btnSel;
            ToolStripButton btnImp;

            btnSel = new ToolStripButton();
            btnSel = createToolButton("Seleccionar Archivo");
            MenuLateral.Items.Add(btnSel);
            btnSel.Click += toolStripButton2_Click;

            btnImp = new ToolStripButton();
            btnImp = createToolButton("Importar");
            MenuLateral.Items.Add(btnImp);
            btnImp.Click += toolStripButton3_Click;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("¿Desea eliminar todo el inventario de la bodega principal?",
                                                "Zamenis Health - Atencion...",
                                                MessageBoxButtons.YesNo,
                                                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                Reset();
            }
        }
        void Reset()
        {
            try
            {
                bool resetPpal = principal.Reset();
                if (resetPpal == true)
                {
                    MG = new MensajesGeneral();
                    MG.Mensaje = "Inventario reseteado con exito";
                    MG.TipoImagen = 3;
                    MG.ShowDialog();                    
                }
                else
                {
                    MG = new MensajesGeneral();
                    MG.Mensaje = "No se logro resetear el inventario";
                    MG.TipoImagen = 1000;
                    MG.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        async void toolStripButton2_Click(object sender, EventArgs e)
        {
            try
            {
                pictureBox1.Visible = true;

                openFileDialog = new OpenFileDialog
                {
                    Filter = "Excel | *.xls;*.xlsx;",
                    Title = "Seleccionar Archivo"
                };

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    dataGridView1.DataSource = repoHomologos.ImportarDatos(openFileDialog.FileName);
                }

                pictureBox1.Visible = false;
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        internal class Resultados
        {
            public string Linea { get; set; }
            public string Observacion { get; set; }
            public string RFinal { get; set; }
        }
        async void toolStripButton3_Click(object sender, EventArgs e)
        {
            try
            {
                List<Resultados> R = new List<Resultados>();
                pictureBox1.Visible = true;

                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    string CodigoInterno = Convert.ToString(row.Cells["CodigoInterno"].Value);
                    string CodigoProveedor = Convert.ToString(row.Cells["CodigoProveedor"].Value);
                    string Cantidad = Convert.ToString(row.Cells["Cantidad"].Value);
                    string Lote = Convert.ToString(row.Cells["Lote"].Value);
                    string Factura = Convert.ToString(row.Cells["Factura"].Value);
                    string Proveedor = Convert.ToString(row.Cells["Proveedor"].Value);
                    string CostoUnitario = Convert.ToString(row.Cells["CostoUnitario"].Value);
                    string IvaUnitario = Convert.ToString(row.Cells["IvaUnitario"].Value);
                    string NombreProducto = Convert.ToString(row.Cells["NombreProducto"].Value);
                    string Prestador = Convert.ToString(row.Cells["Prestador"].Value);

                    if (CodigoInterno == "") { MessageBox.Show("Hay espacios vacios en la columna CodigoInterno, verifique su archivo"); return; }
                    if (CodigoProveedor == "") { MessageBox.Show("Hay espacios vacios en la columna CodigoProveedor, verifique su archivo"); return; }
                    if (Cantidad == "") { MessageBox.Show("Hay espacios vacios en la columna Cantidad, verifique su archivo"); return; }
                    if (Lote == "") { MessageBox.Show("Hay espacios vacios en la columna Lote, verifique su archivo"); return; }
                    if (Factura == "") { MessageBox.Show("Hay espacios vacios en la columna Factura, verifique su archivo"); return; }
                    if (Proveedor == "") { MessageBox.Show("Hay espacios vacios en la columna Proveedor, verifique su archivo"); return; }
                    if (CostoUnitario == "") { MessageBox.Show("Hay espacios vacios en la columna CostoUnitario, verifique su archivo"); return; }
                    if (IvaUnitario == "") { MessageBox.Show("Hay espacios vacios en la columna IvaUnitario, verifique su archivo"); return; }
                    if (NombreProducto == "") { MessageBox.Show("Hay espacios vacios en la columna NombreProducto, verifique su archivo"); return; }
                    if (Prestador == "") { MessageBox.Show("Hay espacios vacios en la columna Prestador, verifique su archivo"); return; }

                    //Consultar si existe el producto
                    INV_PRODUCTOS P = new INV_PRODUCTOS
                    {
                        CodigoInterno = CodigoInterno.Trim(),
                        CodigoProveedor = CodigoProveedor.Trim(),
                        Nombre = NombreProducto.ToUpper().Trim(),
                        Observacion = "N/A",
                        Proveedor = Convert.ToInt32(Proveedor),
                        Prestador = Convert.ToInt32(Prestador)
                    };

                    //Llenar la bodega principal
                    INV_INVENTARIOPPAL I = new INV_INVENTARIOPPAL
                    {
                        Cantidad = Convert.ToInt32(Cantidad),
                        IVA = Convert.ToInt32(IvaUnitario),
                        //CodProducto = CodigoInterno,
                        Costo = Convert.ToInt32(CostoUnitario),
                        Factura = Factura.Trim(),
                        //Id = ,
                        Lote = Lote.Trim()
                    };

                    INV_PRODUCTOS result = repoProductos.GetProducto(P);
                    if (result != null)
                    {
                       //Si existe
                       I.CodProducto = result.Id;
                    }
                    else
                    {
                        // No existe
                        int save = repoProductos.CrearProducto(P);
                        if (save >= 1)
                        {
                            I.CodProducto = save;
                        }
                        else
                        {
                            R.Add(new Resultados { 
                                Linea = CodigoInterno + "|" + CodigoProveedor + "|" + Cantidad + "|" + Lote + "|" + Factura
                                 + "|" + Proveedor + "|" + CostoUnitario + "|" + IvaUnitario + "|" + NombreProducto + "|" + Prestador,
                                Observacion = "NO SE LOGRO CREAR EL PRODUCTO",
                                RFinal = "ERROR"
                            });

                            continue;
                        }
                    }

                    //Consulto inventario principal a ver si existe el producto
                    INV_INVENTARIOPPAL cantPpal = principal.GetCantidad(I);
                    if (cantPpal != null)
                    {
                        //actualiza
                        int Nuevacantidad = cantPpal.Cantidad + Convert.ToInt32(Cantidad);

                        I.Cantidad = Nuevacantidad;
                        I.Id = cantPpal.Id;

                        bool actualizaExistente = principal.ActualizarCantidad(I, Contenedor.UsuarioLogueado, true);
                        if (actualizaExistente == true)
                        {
                            R.Add(new Resultados
                            {
                                Linea = CodigoInterno + "|" + CodigoProveedor + "|" + Cantidad + "|" + Lote + "|" + Factura
                               + "|" + Proveedor + "|" + CostoUnitario + "|" + IvaUnitario + "|" + NombreProducto + "|" + Prestador,
                                Observacion = "PRODUCTO EXISTENTE Y ACTUALIZADO EN EL INVENTARIO",
                                RFinal = "EXITOSO"
                            });
                        }
                        else
                        {
                            R.Add(new Resultados
                            {
                                Linea = CodigoInterno + "|" + CodigoProveedor + "|" + Cantidad + "|" + Lote + "|" + Factura
                              + "|" + Proveedor + "|" + CostoUnitario + "|" + IvaUnitario + "|" + NombreProducto + "|" + Prestador,
                                Observacion = "PRODUCTO EXISTENTE PERO NO SE LOGRO ACTUALIZAR EN EL INVENTARIO",
                                RFinal = "NOVEDAD"
                            });
                        }
                    }
                    else
                    {
                        //inserta
                        bool AgregarNuevo = principal.IngresarNuevo(I, Contenedor.UsuarioLogueado);
                        if (AgregarNuevo == true)
                        {
                            R.Add(new Resultados
                            {
                                Linea = CodigoInterno + "|" + CodigoProveedor + "|" + Cantidad + "|" + Lote + "|" + Factura
                                + "|" + Proveedor + "|" + CostoUnitario + "|" + IvaUnitario + "|" + NombreProducto + "|" + Prestador,
                                Observacion = "PRODUCTO CREADO CON EXITO E INSERTADO EN EL INVENTARIO", 
                                RFinal = "EXITOSO"
                            });
                        }
                        else
                        {
                            R.Add(new Resultados
                            {
                                Linea = CodigoInterno + "|" + CodigoProveedor + "|" + Cantidad + "|" + Lote + "|" + Factura
                                + "|" + Proveedor + "|" + CostoUnitario + "|" + IvaUnitario + "|" + NombreProducto + "|" + Prestador,
                                Observacion = "PRODUCTO CREADO CON EXITO PERO NO SE LOGRO INSERTAR EN EL INVENTARIO", 
                                RFinal = "NOVEDAD"
                            });
                        }
                    }
                }

                DateTime Hoy = DateTime.Now;
                FileStream Querys = new FileStream("C:/Cxn/Reportes/Resultado_Inventario_MasivoPrincipal_" + Convert.ToDateTime(Hoy).ToString("dd-MM-yyyy") + ".csv", FileMode.Append, FileAccess.Write);
                StreamWriter Escriba = new StreamWriter(Querys);
                string Texto = "LINEA|OBSERVACION|RESPUESTA \r";

                foreach (var item in R) 
                { 
                    Texto = Texto + item.Linea + "|" + item.Observacion + "|" + item.RFinal + "\r";
                }

                Escriba.Write(Texto);
                Escriba.WriteLine();
                Escriba.Flush();
                Escriba.Close();

                pictureBox1.Visible = false;

                MG = new MensajesGeneral();
                MG.Mensaje = "Terminado, consulte el resultado en la carpeta C:\\CXN\\Reportes\\Resultado_Inventario_MasivoPrincipal_*.csv";
                MG.TipoImagen = 3;
                MG.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
