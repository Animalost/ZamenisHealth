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

namespace ZamenisHealth.INV.Consultorios
{
    public partial class MasivoConsultorios : Forma
    {
        private IBodegas repositorioBodegas;
        private ISubBodegas subbodegas;
        private IHomologos repoHomologos;
        private IProductos repoProductos;
        private IBodegaPrincipal principal;
        private OpenFileDialog openFileDialog;
        private MensajesGeneral MG;

        private int CodeBodega;

        public MasivoConsultorios()
        {
            InitializeComponent();
            subbodegas = new MSubBodegas();
            repositorioBodegas = new MBodegas();
            repoHomologos = new MHomologos();
            repoProductos = new MProductos();
            principal = new MBodegaPrincipal();
        }

        private void MasivoConsultorios_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Masivo";
            LogoMain.Image = Properties.Resources.Splash;
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";

            ToolStripButton btnSeleccionar;
            ToolStripButton btnImportar;

            btnSeleccionar = new ToolStripButton();
            btnSeleccionar = createToolButton("Seleccionar Archivo");
            MenuLateral.Items.Add(btnSeleccionar);
            btnSeleccionar.Click += toolStripButton2_Click;

            btnImportar = new ToolStripButton();
            btnImportar = createToolButton("Importar");
            MenuLateral.Items.Add(btnImportar);
            btnImportar.Click += toolStripButton3_Click;

            CargarProfesionales();
        }

        void CargarProfesionales()
        {
            List<string> ListaProf = repositorioBodegas.Profesionales("ALL");

            if (ListaProf != null)
            {
                comboBox2.Items.Clear();

                foreach (var i in ListaProf)
                {
                    comboBox2.Items.Add(i.ToString());
                }

                comboBox2.SelectedIndex = 0;
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Dispose();
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("¿Desea eliminar todo el inventario de la bodega seleccionada?",
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
                bool resetPpal = subbodegas.Reset(CodeBodega);
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
        private void toolStripButton2_Click(object sender, EventArgs e)
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

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            CodeBodega = repositorioBodegas.getDatosName(comboBox2.Text).Bod_Numero;
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            try
            {
                List<Resultados> R = new List<Resultados>();
                pictureBox1.Visible = true;

                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    string CodigoProveedor = Convert.ToString(row.Cells["CodigoProveedor"].Value);
                    string Proveedor = Convert.ToString(row.Cells["Proveedor"].Value);
                    string Prestador = Convert.ToString(row.Cells["Prestador"].Value);
                    string Lote = Convert.ToString(row.Cells["Lote"].Value);
                    string Factura = Convert.ToString(row.Cells["Factura"].Value);
                    string CostoUnitario = Convert.ToString(row.Cells["CostoUnitario"].Value);
                    string CodigoInterno = Convert.ToString(row.Cells["CodigoInterno"].Value);
                    string Bodega = Convert.ToString(row.Cells["Bodega"].Value);
                    string Cantidad = Convert.ToString(row.Cells["Cantidad"].Value);                   


                    if (CodigoProveedor == "") { MessageBox.Show("Hay espacios vacios en la columna CodigoProveedor, verifique su archivo"); return; }
                    if (Proveedor == "") { MessageBox.Show("Hay espacios vacios en la columna Proveedor, verifique su archivo"); return; }
                    if (Prestador == "") { MessageBox.Show("Hay espacios vacios en la columna Prestador, verifique su archivo"); return; }
                    if (CodigoInterno == "") { MessageBox.Show("Hay espacios vacios en la columna CodigoInterno, verifique su archivo"); return; }                  
                    if (Cantidad == "") { MessageBox.Show("Hay espacios vacios en la columna Cantidad, verifique su archivo"); return; }
                    if (Lote == "") { MessageBox.Show("Hay espacios vacios en la columna Lote, verifique su archivo"); return; }
                    if (Factura == "") { MessageBox.Show("Hay espacios vacios en la columna Factura, verifique su archivo"); return; }                  
                    if (CostoUnitario == "") { MessageBox.Show("Hay espacios vacios en la columna CostoUnitario, verifique su archivo"); return; }
                    if (Bodega == "") { MessageBox.Show("Hay espacios vacios en la columna Bodega, verifique su archivo"); return; }
                  

                    //Consultar si existe el producto
                    INV_PRODUCTOS P = new INV_PRODUCTOS
                    {
                        CodigoProveedor = CodigoProveedor.Trim(),
                        Proveedor = Convert.ToInt32(Proveedor),
                        Prestador = Convert.ToInt32(Prestador)                        
                    };

                    //Llenar la bodega principal
                    INV_INVENTARIOPPAL I = new INV_INVENTARIOPPAL
                    {
                        Cantidad = Convert.ToInt32(Cantidad),
                        //IVA = Convert.ToInt32(IvaUnitario),
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
                        R.Add(new Resultados
                        {
                            Linea = CodigoProveedor + "|" + Proveedor + "|" + Prestador,
                            Observacion = "PRODUCTO NO EXISTE PARA EL PROVEEDOR Y PRESTADOR",
                            RFinal = "ERROR"
                        });

                        continue;
                    }

                    //Consulto inventario principal a ver si existe el producto
                    INV_INVENTARIOPPAL cantPpal = principal.GetCantidad(I);
                    if (cantPpal != null)
                    {
                        if (cantPpal.Cantidad < Convert.ToInt32(Cantidad))
                        {
                            R.Add(new Resultados
                            {
                                Linea = CodigoProveedor + "|" + Proveedor + "|" + Prestador,
                                Observacion = "LA CANTIDAD ACTUAL EN BODEGA PRINCIPAL ES INFERIOR A LA SOLICITADA",
                                RFinal = "ERROR"
                            });
                        }
                        else
                        {
                            //actualiza PRINCIPAL
                            int Nuevacantidad = cantPpal.Cantidad - Convert.ToInt32(Cantidad);

                            I.Cantidad = Nuevacantidad;

                            bool actualizaExistente = principal.ActualizarCantidad(I, Contenedor.UsuarioLogueado, true);
                            if (actualizaExistente == true)
                            {
                                //consultar si existe en subbodega
                                var cons = subbodegas.GetInventarySubByBodPosProd(Convert.ToInt32(Bodega), cantPpal.CodProducto);
                                if (cons.PosisionSub != 0)
                                {
                                    //actualiza cantidad
                                    int nuecantSub = cons.CantidadSub + Convert.ToInt32(Cantidad);

                                    INV_INVENTARIOBODEGAS IS = new INV_INVENTARIOBODEGAS
                                    {
                                        Cantidad = nuecantSub,
                                        Id = cons.PosisionSub
                                    };

                                    bool actualiza = subbodegas.ActualizarCantidad(IS);
                                    if (actualiza == true)
                                    {
                                        R.Add(new Resultados
                                        {
                                            Linea = CodigoProveedor + "|" + Proveedor + "|" + Prestador,
                                            Observacion = "ACTUALIZADO",
                                            RFinal = "EXITO"
                                        });
                                    }
                                    else
                                    {
                                        R.Add(new Resultados
                                        {
                                            Linea = CodigoProveedor + "|" + Proveedor + "|" + Prestador,
                                            Observacion = "SE HA DESCONTADO DE LA BODEGA PRINCIPAL PERO NO SE HA SUMADO A LA SUBBODEGA",
                                            RFinal = "NOVEDAD"
                                        });
                                    }
                                }
                                else
                                {
                                    //inserta cantidad
                                    INV_INVENTARIOBODEGAS ISInsert = new INV_INVENTARIOBODEGAS
                                    {
                                        CodInvPpal = cantPpal.CodProducto,
                                        Cantidad = Convert.ToInt32(Cantidad),
                                        Bodega = Convert.ToInt32(Bodega)
                                    };

                                    bool inserta = subbodegas.IngresarNuevo(ISInsert);
                                    if (inserta == true)
                                    {
                                        R.Add(new Resultados
                                        {
                                            Linea = CodigoProveedor + "|" + Proveedor + "|" + Prestador,
                                            Observacion = "INGRESADO",
                                            RFinal = "EXITO"
                                        });
                                    }
                                    else
                                    {
                                        R.Add(new Resultados
                                        {
                                            Linea = CodigoProveedor + "|" + Proveedor + "|" + Prestador,
                                            Observacion = "SE HA ESCONTADO DE LA BODEGA PRINCIPAL PERO NO SE HA INGRESADO A LA SUBBODEGA",
                                            RFinal = "NOVEDAD"
                                        });
                                    }
                                }
                            }
                            else
                            {
                                R.Add(new Resultados
                                {
                                    Linea = CodigoProveedor + "|" + Proveedor + "|" + Prestador,
                                    Observacion = "ERROR INTERNO, NO SE HA MOVIDO EL INVENTARIO",
                                    RFinal = "ERROR"
                                });
                            }
                        }                    
                    }
                    else
                    {
                        R.Add(new Resultados
                        {
                            Linea = CodigoProveedor + "|" + Proveedor + "|" + Prestador,
                            Observacion = "EL PRODUCTO NO EXISTE",
                            RFinal = "ERROR"
                        });
                    }
                }

                DateTime Hoy = DateTime.Now;
                FileStream Querys = new FileStream("C:/Cxn/Reportes/Resultado_Inventario_SubBodegas_" + Convert.ToDateTime(Hoy).ToString("dd-MM-yyyy") + ".csv", FileMode.Append, FileAccess.Write);
                StreamWriter Escriba = new StreamWriter(Querys);
                string Texto = "";

                foreach (var item in R)
                {
                    Texto = Texto + item.Linea + "|" + item.Observacion + "|" + item.RFinal;
                }

                Escriba.Write(Texto);
                Escriba.WriteLine();
                Escriba.Flush();
                Escriba.Close();

                pictureBox1.Visible = false;

                MG = new MensajesGeneral();
                MG.Mensaje = "Terminado, consulte el resultado en la carpeta C:\\CXN\\Reportes\\Resultado_Inventario_SubBodegas_*.csv";
                MG.TipoImagen = 3;
                MG.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        internal class Resultados
        {
            public string Linea { get; set; }
            public string Observacion { get; set; }
            public string RFinal { get; set; }
        }
    }
}
