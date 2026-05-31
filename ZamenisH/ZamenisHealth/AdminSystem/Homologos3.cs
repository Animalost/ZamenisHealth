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
    public partial class Homologos3 : ConfigForm.BaseForm
    {
        private static readonly IHomologos repoHomologos = new MHomologos();        
        private static readonly IFacturacion repoFacturacion = new MFacturacion();
        
        private OpenFileDialog openFileDialog;
        MensajesGeneral MG;

        public Homologos3()
        {
            InitializeComponent();
            
            btnZamenis1.ButtonClick += btnZamenis1_ButtonClick;
            btnZamenis2.ButtonClick += btnZamenis2_ButtonClick;
            btnZamenis3.ButtonClick += btnZamenis3_ButtonClick;

            btnZamenis1.captionBtn = "Subir Excel";
            btnZamenis1.tooltipBtn = "Subir masivamente datos a traves de una plantilla de Excel";

            btnZamenis2.captionBtn = "Comprobar";
            btnZamenis2.tooltipBtn = "Verificar archivo de Excel subido";

            btnZamenis3.captionBtn = "Homologar";
            btnZamenis3.tooltipBtn = "Grabar estos datos";
        }
        private void btnZamenis1_ButtonClick(object sender, EventArgs e)
        {
            try
            {
                openFileDialog = new OpenFileDialog
                {
                    Filter = "Excel | *.xls;*.xlsx;",
                    Title = "Seleccionar Archivo"
                };

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    dataGridView1.DataSource = repoHomologos.ImportarDatos(openFileDialog.FileName);
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void btnZamenis2_ButtonClick(object sender, EventArgs e)
        {
            try
            {
                if (comboBox1.SelectedIndex == 1)
                {
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        string FZ = Convert.ToString(row.Cells["FZ"].Value); //FACTURA ZAMENIS
                        string FH = Convert.ToString(row.Cells["FH"].Value); //FACTURA HELISA
                        string FF = Convert.ToString(row.Cells["FF"].Value); //FECHA DE FACTURA HELISA            
                        string FC = Convert.ToString(row.Cells["FC"].Value); //NUMERO DE COMPAÑIA
                        string CUFE = Convert.ToString(row.Cells["CUFE"].Value); //CUFE DE COMPAÑIA
                        string HORA = Convert.ToString(row.Cells["HORA"].Value); //HORA FACTURA
                        string RESOLUCION = Convert.ToString(row.Cells["RESOLUCION"].Value); //HORA FACTURA

                        if (FF == "") { MessageBox.Show("Hay espacios vacios en la columna FF, verifique su archivo"); return; }
                        if (FZ == "") { MessageBox.Show("Hay espacios vacios en la columna FZ, verifique su archivo"); return; }
                        if (FH == "") { MessageBox.Show("Hay espacios vacios en la columna FH, verifique su archivo"); return; }
                        if (FC == "") { MessageBox.Show("Hay espacios vacios en la columna FC, verifique su archivo"); return; }
                        if (CUFE == "") { MessageBox.Show("Hay espacios vacios en la columna CUFE, verifique su archivo"); return; }
                        if (HORA == "") { MessageBox.Show("Hay espacios vacios en la columna HORA, verifique su archivo"); return; }
                        if (RESOLUCION == "") { MessageBox.Show("Hay espacios vacios en la columna RESOLUCION, verifique su archivo"); return; }

                        Verificar_Factura(Convert.ToInt32(FZ),
                                          FH,
                                          FC,
                                          CUFE,
                                          HORA);
                    }

                    btnZamenis3.Enabled = true;
                }
                else if (comboBox1.SelectedIndex == 2)
                {
                    btnZamenis3.Enabled = true;
                }
                else if (comboBox1.SelectedIndex == 3) //RcCAja
                {
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        string Recibo = Convert.ToString(row.Cells["Recibo"].Value); //Admision
                        string Homologo = Convert.ToString(row.Cells["Homologo"].Value); //FACTURA HELISA
                        string CUFE = Convert.ToString(row.Cells["CUFE"].Value); //CUFE HELISA
                        string RES = Convert.ToString(row.Cells["Resolucion"].Value); //CUFE HELISA

                        if (Recibo == "") { MessageBox.Show("Hay espacios vacios en la columna Recibo, verifique su archivo"); return; }
                        if (Homologo == "") { MessageBox.Show("Hay espacios vacios en la columna Homologo, verifique su archivo"); return; }
                        if (CUFE == "") { MessageBox.Show("Hay espacios vacios en la columna CUFE, verifique su archivo"); return; }
                        if (RES == "") { MessageBox.Show("Hay espacios vacios en la columna Resolucion, verifique su archivo"); return; }

                        Verificar_Recibo(Convert.ToInt32(Recibo),
                                          Homologo);
                    }

                    btnZamenis3.Enabled = true;
                }
                else if (comboBox1.SelectedIndex == 4) //Inventario
                {
                    MessageBox.Show("Esta opcion no esta disponible");
                }
                else if (comboBox1.SelectedIndex == 5) //Pagos
                {
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        string FH = Convert.ToString(row.Cells["FH"].Value);
                        string FR = Convert.ToString(row.Cells["FR"].Value);
                        string VP = Convert.ToString(row.Cells["VP"].Value);
                        string CO = Convert.ToString(row.Cells["CO"].Value);
                        string FP = Convert.ToString(row.Cells["FP"].Value);
                        string RT = Convert.ToString(row.Cells["RT"].Value);

                        if (FH == "") { MessageBox.Show("Hay espacios vacios en la columna FH, verifique su archivo"); return; }
                        if (FR == "") { MessageBox.Show("Hay espacios vacios en la columna FR, verifique su archivo"); return; }
                        if (VP == "") { MessageBox.Show("Hay espacios vacios en la columna VP, verifique su archivo"); return; }
                        if (CO == "") { MessageBox.Show("Hay espacios vacios en la columna CO, verifique su archivo"); return; }
                        if (FP == "") { MessageBox.Show("Hay espacios vacios en la columna FP, verifique su archivo"); return; }
                        if (RT == "") { MessageBox.Show("Hay espacios vacios en la columna RT, verifique su archivo"); return; }

                        VerificarArchivoFacturas(FH, CO);
                    }

                    btnZamenis3.Enabled = true;
                }
                else
                {
                    MensajesGeneral MG = new MensajesGeneral();
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "Debe seleccionar un tipo de documento a homologar masivamente";
                    MG.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void btnZamenis3_ButtonClick(object sender, EventArgs e)
        {
            try
            {
                Comunes.MensajesGeneral MG = new Comunes.MensajesGeneral();

                if (comboBox1.SelectedIndex == 1)
                {
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        string FZ = Convert.ToString(row.Cells["FZ"].Value);
                        string FH = Convert.ToString(row.Cells["FH"].Value);
                        string FF = Convert.ToString(row.Cells["FF"].Value);
                        string FC = Convert.ToString(row.Cells["FC"].Value);
                        string CUFE = Convert.ToString(row.Cells["CUFE"].Value);
                        string HORA = Convert.ToString(row.Cells["HORA"].Value);
                        string RESOLUCION = Convert.ToString(row.Cells["RESOLUCION"].Value);

                        if (FF == "") { MessageBox.Show("Hay espacios vacios en la columna FF, verifique su archivo"); return; }
                        if (FZ == "") { MessageBox.Show("Hay espacios vacios en la columna FZ, verifique su archivo"); return; }
                        if (FH == "") { MessageBox.Show("Hay espacios vacios en la columna FH, verifique su archivo"); return; }
                        if (FC == "") { MessageBox.Show("Hay espacios vacios en la columna FC, verifique su archivo"); return; }
                        if (CUFE == "") { MessageBox.Show("Hay espacios vacios en la columna CUFE, verifique su archivo"); return; }
                        if (HORA == "") { MessageBox.Show("Hay espacios vacios en la columna HORA, verifique su archivo"); return; }
                        if (RESOLUCION == "") { MessageBox.Show("Hay espacios vacios en la columna RESOLUCION, verifique su archivo"); return; }

                        repoHomologos.ActualizarMasivo(FH, Convert.ToInt32(FZ), FC, Convert.ToDateTime(FF), CUFE, Convert.ToDateTime(HORA), RESOLUCION);
                    }

                    MG.TipoImagen = 3;
                    MG.Mensaje = "Homologado exitosamente!!!";
                    MG.ShowDialog();

                    this.Dispose();
                    this.Close();
                }
                else if (comboBox1.SelectedIndex == 2)
                {
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        string FZ = Convert.ToString(row.Cells["FZ"].Value);
                        string FH = Convert.ToString(row.Cells["FH"].Value);
                        string FF = Convert.ToString(row.Cells["FF"].Value);
                        string FC = Convert.ToString(row.Cells["FC"].Value);
                        string CUFE = Convert.ToString(row.Cells["CUFE"].Value);
                        string HORA = Convert.ToString(row.Cells["HORA"].Value);
                        string RESOLUCION = Convert.ToString(row.Cells["RESOLUCION"].Value);

                        if (FF == "") { MessageBox.Show("Hay espacios vacios en la columna FF, verifique su archivo"); return; }
                        if (FZ == "") { MessageBox.Show("Hay espacios vacios en la columna FZ, verifique su archivo"); return; }
                        if (FH == "") { MessageBox.Show("Hay espacios vacios en la columna FH, verifique su archivo"); return; }
                        if (FC == "") { MessageBox.Show("Hay espacios vacios en la columna FC, verifique su archivo"); return; }
                        if (CUFE == "") { MessageBox.Show("Hay espacios vacios en la columna CUFE, verifique su archivo"); return; }
                        if (HORA == "") { MessageBox.Show("Hay espacios vacios en la columna HORA, verifique su archivo"); return; }
                        if (RESOLUCION == "") { MessageBox.Show("Hay espacios vacios en la columna RESOLUCION, verifique su archivo"); return; }

                        repoHomologos.ActualizarMasivoRec(FH, Convert.ToInt32(FZ), FC, Convert.ToDateTime(FF), CUFE, Convert.ToDateTime(HORA), RESOLUCION);
                    }

                    MG.TipoImagen = 3;
                    MG.Mensaje = "Homologado exitosamente!!!";
                    MG.ShowDialog();

                    this.Dispose();
                    this.Close();
                }
                else if (comboBox1.SelectedIndex == 3)
                {
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        string Recibo = Convert.ToString(row.Cells["Recibo"].Value);
                        string Homologo = Convert.ToString(row.Cells["Homologo"].Value);
                        string CUFE = Convert.ToString(row.Cells["CUFE"].Value);
                        string RES = Convert.ToString(row.Cells["Resolucion"].Value);

                        if (Recibo == "") { MessageBox.Show("Hay espacios vacios en la columna Recibo, verifique su archivo"); return; }
                        if (Homologo == "") { MessageBox.Show("Hay espacios vacios en la columna Homologo, verifique su archivo"); return; }
                        if (CUFE == "") { MessageBox.Show("Hay espacios vacios en la columna CUFE, verifique su archivo"); return; }
                        if (RES == "") { MessageBox.Show("Hay espacios vacios en la columna Resolucion, verifique su archivo"); return; }

                        repoHomologos.ActualizarMasivoRecibo(Convert.ToInt32(Recibo), Homologo, CUFE, RES);
                    }

                    MG.TipoImagen = 3;
                    MG.Mensaje = "Homologado exitosamente!!!";
                    MG.ShowDialog();

                    this.Dispose();
                    this.Close();
                }
                else if (comboBox1.SelectedIndex == 4)
                {
                    MessageBox.Show("Esta opcion no esta disponible");
                }
                else if (comboBox1.SelectedIndex == 5) //Facturas
                {
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        string FH = Convert.ToString(row.Cells["FH"].Value);
                        string FR = Convert.ToString(row.Cells["FR"].Value);
                        string VP = Convert.ToString(row.Cells["VP"].Value);
                        string CO = Convert.ToString(row.Cells["CO"].Value);
                        string FP = Convert.ToString(row.Cells["FP"].Value);
                        string RT = Convert.ToString(row.Cells["RT"].Value);

                        if (FH == "") { MessageBox.Show("Hay espacios vacios en la columna FH, verifique su archivo"); return; }
                        if (FR == "") { MessageBox.Show("Hay espacios vacios en la columna FR, verifique su archivo"); return; }
                        if (VP == "") { MessageBox.Show("Hay espacios vacios en la columna VP, verifique su archivo"); return; }
                        if (CO == "") { MessageBox.Show("Hay espacios vacios en la columna CO, verifique su archivo"); return; }
                        if (FP == "") { MessageBox.Show("Hay espacios vacios en la columna FP, verifique su archivo"); return; }
                        if (RT == "") { MessageBox.Show("Hay espacios vacios en la columna RT, verifique su archivo"); return; }

                        CXN_PAGOS F = new CXN_PAGOS
                        {
                            FRadica = Convert.ToDateTime(FR),
                            VrPagado = Convert.ToInt32(VP),
                            FPago = Convert.ToDateTime(FP),
                            Fac_Cia = Convert.ToInt32(CO),
                            Homologo = FH,
                            RT = RT
                        };

                        repoFacturacion.updateDatosGlosas(F);
                    }

                    MG = new MensajesGeneral();
                    MG.TipoImagen = 3;
                    MG.Mensaje = "Operacion terminada, Consulte el log de transacciones";
                    MG.ShowDialog();
                }
                else
                {
                    MG = new MensajesGeneral();
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "Debe seleccionar un tipo de factura a homologar masivamente";
                    MG.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void Homologos3_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Homologos";
            
        }

        void VerificarArchivoFacturas(string FH, string CO)
        {
            try
            {
                if (repoFacturacion.existeHomologo(FH, Convert.ToInt32(CO)) != true)
                {
                    MG = new MensajesGeneral();
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "Homologo " + FH + " no encontrado";
                    MG.ShowDialog();
                    return;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void Verificar_Recibo(int Recibo,
                                      string Homologo)
        {
            try
            {
                Comunes.MensajesGeneral MG = new Comunes.MensajesGeneral();

                bool consultarFactura = repoHomologos.ConsultarRecibo(Recibo, Homologo);
                if (consultarFactura == true)
                {
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "La factura: " + Homologo + " ya existe con el recibo de caja " + Recibo.ToString();
                    MG.ShowDialog();
                    return;
                }                
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void Verificar_Factura(int FZ,
                                       string FH,
                                       string FC,
                                       string CUFE, 
                                       string HORA)
        {
            try
            {
                Comunes.MensajesGeneral MG = new Comunes.MensajesGeneral();

                bool consultarFactura = repoHomologos.ConsultarFactura(FZ, FC);
                if (consultarFactura != true)
                {
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "La factura: " + FZ + " no existe o esta anulada";
                    MG.ShowDialog();
                }
                else
                {
                    Verifica_Homologo(FH, FC);
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void Verifica_Homologo(string FH, string FC)
        {
            try
            {
                Comunes.MensajesGeneral MG = new Comunes.MensajesGeneral();

                bool consultarFactura = repoHomologos.Verifica_Homologo(FH, FC);
                if (consultarFactura == true)
                {
                    MG.TipoImagen = 1000;
                    MG.Mensaje = "El homologo: " + FH + " ya existe verifique su archivo y vuelva a subirlo";
                    MG.ShowDialog();
                    return;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }            
    }
}
