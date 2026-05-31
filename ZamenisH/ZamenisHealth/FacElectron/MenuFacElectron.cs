using Domain.Contabilidad;
using Domain.CXN;

using FormAndControls;

using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

using ZamenisHealth.Clases;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.FacElectron
{
    public partial class MenuFacElectron : Forma2
    {
        private static readonly IFacElectron repoFacElectron = new MFacElectron();
        private static readonly ICompañia repoCia = new MCompañia();
        private static readonly ICuentas cuentas = new MCuentas();

        private MensajesGeneral MG;
        private int Cia;
        decimal baseRetencion = 1.10m;
        public MenuFacElectron()
        {
            InitializeComponent();
            ConfigForm.SoloNumeros(textBox1);
            ConfigForm.SoloNumeros(textBox4);
        }

        private void MenuFacElectron_Load(object sender, EventArgs e)
        {
            this.Titulo.Text = "Menu Facturacion Electronica";
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";
            CargarCompañias();
        }

        void CargarCompañias()
        {
            List<CXN_CIA> compañias = repoCia.getAllCompañias();
            if (compañias != null)
            {
                foreach (CXN_CIA c in compañias)
                {
                    comboBox1.Items.Add(c.Com_Nombre);
                }
                
                comboBox1.SelectedIndex = 0; // Selecciona la primera compañia por defecto
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            CleanInvoice cleanInvoice = new CleanInvoice();
            cleanInvoice.ShowDialog();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                Cia = repoCia.getPrestadorbyName(comboBox1.Text).Com_Identificador;

                CXN_CIA c = repoCia.getPrestadorbyCode(Cia);
                if (c != null)
                {
                    //Factura electronica
                    textBox1.Text = c.Com_Doc_Electron.ToString().Trim();
                    textBox2.Text = c.Com_Prefijo_Electron;
                    textBox3.Text = c.Com_Prefijo_Electron_NC;
                    textBox4.Text = c.Com_Doc_Electron_NC.ToString().Trim();
                    textBox5.Text = c.Com_Resolucion_Electron;
                    textBox6.Text = c.Com_Numeracion_Electron;
                    dateTimePicker3.Value = c.Com_Fecha_Electron;
                }
                else
                {
                    //Factura electronica
                    textBox1.Text = "";
                    textBox2.Text = "";
                    textBox3.Text = "";
                    textBox4.Text = "";
                    textBox5.Text = "";
                    textBox6.Text = "";
                    dateTimePicker3.Value = DateTime.Now.Date;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(textBox1.Text) || string.IsNullOrEmpty(textBox2.Text) || string.IsNullOrEmpty(textBox3.Text) || string.IsNullOrEmpty(textBox4.Text) || string.IsNullOrEmpty(textBox5.Text) || string.IsNullOrEmpty(textBox6.Text))
                {
                    MG = new MensajesGeneral();
                    MG.Mensaje = "Por favor, complete todos los campos antes de guardar.";
                    MG.TipoImagen = 1000; // Error
                    MG.ShowDialog();
                    return;
                }

                CXN_CIA c = new CXN_CIA
                {
                    Com_Identificador = Cia,
                    Com_Doc_Electron = Convert.ToInt32(textBox1.Text.Trim()),
                    Com_Prefijo_Electron = textBox2.Text.Trim(),
                    Com_Prefijo_Electron_NC = textBox3.Text.Trim(),
                    Com_Doc_Electron_NC = Convert.ToInt32(textBox4.Text.Trim()),
                    Com_Resolucion_Electron = textBox5.Text.Trim(),
                    Com_Numeracion_Electron = textBox6.Text.Trim(),
                    Com_Fecha_Electron = dateTimePicker3.Value.Date
                };

                bool result = repoCia.updateDataElectron(c);
                if (result == true)
                {
                    MG = new MensajesGeneral();
                    MG.Mensaje = "Datos actualizados correctamente.";
                    MG.TipoImagen = 3; // Éxito
                    MG.ShowDialog();
                }
                else
                {
                    MG = new MensajesGeneral();
                    MG.Mensaje = "Error al actualizar los datos. Por favor, intente nuevamente.";
                    MG.TipoImagen = 1000; // Error
                    MG.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        decimal ConvertirValor(decimal? valor)
        {
            return Math.Round(valor ?? 0m, 2);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                GenerarCuraciones();
                GenerarBonos();
                GenerarVentas();
                GenerarFibromialgia();
                GenerarNotasCredito();
             
                MG = new MensajesGeneral();
                MG.Mensaje = "Hecho";
                MG.TipoImagen = 3; // Error
                MG.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        void GenerarNotasCredito()
        {
            try
            {
                List<CXN_CUENTAS> getcuentas = cuentas.getCuentas();

                List<ReporteContable> getListaCaja = repoFacElectron.getReportContableNC(Cia, dateTimePicker1.Value.Date, dateTimePicker2.Value.Date, "Caja");
                if (getListaCaja != null)
                {
                    string texto = "FECHA|TIPO DE DOCUMENTO|NUMERO DE DOCUMENTO|CUENTA|CONCEPTO|IDENTIDAD|CENTRO DE COSTO|VALOR|NATURALEZA|CLASE\n";
                    FileStream QueryTxt = new FileStream("C:/Cxn/Reportes/ReporteContable_NC_BONOS_" + DateTime.Now.ToString("yyyy-MM-dd HHmmss") + ".csv", FileMode.Append, FileAccess.Write);

                    foreach (ReporteContable r in getListaCaja)
                    {
                        // Ingreso de Factura
                        texto = texto + Convert.ToDateTime(r.Fecha).ToString("yyyy-MM-dd") + "|" +
                            r.TipoDocumento + "|" +
                            r.NumeroDocumento + "|" +
                            getcuentas.FirstOrDefault(x => x.CuentaLlave == "Ingreso Nota Credito Bonos").CuentaCode + "|" +
                            getcuentas.FirstOrDefault(x => x.CuentaLlave == "Ingreso Nota Credito Bonos").CuentaName + "|" +
                            r.Identidad + "|" +
                            "|" +
                            ConvertirValor(r.Valor) + "|" +
                            getcuentas.FirstOrDefault(x => x.CuentaLlave == "Ingreso Nota Credito Bonos").Naturaleza + "|" +
                            "F" + "\n";

                        texto = texto + Convert.ToDateTime(r.Fecha).ToString("yyyy-MM-dd") + "|" +
                           r.TipoDocumento + "|" +
                           r.NumeroDocumento + "|" +
                           getcuentas.FirstOrDefault(x => x.CuentaLlave == "Ingreso Nota Credito Debito Bonos").CuentaCode + "|" +
                           getcuentas.FirstOrDefault(x => x.CuentaLlave == "Ingreso Nota Credito Debito Bonos").CuentaName + "|" +
                           r.Identidad + "|" +
                           "3" + "|" +
                           ConvertirValor(r.Valor) + "|" +
                           getcuentas.FirstOrDefault(x => x.CuentaLlave == "Ingreso Nota Credito Debito Bonos").Naturaleza + "|" +
                           "F" + "\n";

                        //Ingreso de Retenciones  Debito
                        texto = texto + Convert.ToDateTime(r.Fecha).ToString("yyyy-MM-dd") + "|" +
                           r.TipoDocumento + "|" +
                           r.NumeroDocumento + "|" +
                           getcuentas.FirstOrDefault(x => x.CuentaLlave == "Retencion Nota Credito Bonos").CuentaCode + "|" +
                           getcuentas.FirstOrDefault(x => x.CuentaLlave == "Retencion Nota Credito Bonos").CuentaName + " " + r.TipoDocumento + r.NumeroDocumento + "|" +
                           r.Identidad + "|" +
                           "" + "|" +
                           ConvertirValor(((r.Valor * baseRetencion)) / 100) + "|" +
                           getcuentas.FirstOrDefault(x => x.CuentaLlave == "Retencion Nota Credito Bonos").Naturaleza + "|" +
                           "F" + "\n";

                        //Ingreso de Retenciones  Credito
                        texto = texto + Convert.ToDateTime(r.Fecha).ToString("yyyy-MM-dd") + "|" +
                          r.TipoDocumento + "|" +
                          r.NumeroDocumento + "|" +
                          getcuentas.FirstOrDefault(x => x.CuentaLlave == "Autoretencion Nota Credito Bonos").CuentaCode + "|" +
                          getcuentas.FirstOrDefault(x => x.CuentaLlave == "Autoretencion Nota Credito Bonos").CuentaName + " " + r.TipoDocumento + r.NumeroDocumento + "|" +
                          r.Identidad + "|" +
                          "" + "|" +
                          ConvertirValor(((r.Valor * baseRetencion)) / 100) + "|" +
                          getcuentas.FirstOrDefault(x => x.CuentaLlave == "Autoretencion Nota Credito Bonos").Naturaleza + "|" +
                          "F" + "\n";                       
                    }

                    StreamWriter Escriba = new StreamWriter(QueryTxt);
                    Escriba.Write(texto);
                    Escriba.Close();
                }

                List<ReporteContable> getListaVentas = repoFacElectron.getReportContableNC(Cia, dateTimePicker1.Value.Date, dateTimePicker2.Value.Date, "VENTAS");
                if (getListaVentas != null)
                {
                    string texto = "FECHA|TIPO DE DOCUMENTO|NUMERO DE DOCUMENTO|CUENTA|CONCEPTO|IDENTIDAD|CENTRO DE COSTO|VALOR|NATURALEZA|CLASE\n";
                    FileStream QueryTxt = new FileStream("C:/Cxn/Reportes/ReporteContable_NC_VENTAS_" + DateTime.Now.ToString("yyyy-MM-dd HHmmss") + ".csv", FileMode.Append, FileAccess.Write);

                    foreach (ReporteContable r in getListaVentas)
                    {

                        // Ingreso de Factura
                        texto = texto + Convert.ToDateTime(r.Fecha).ToString("yyyy-MM-dd") + "|" +
                            r.TipoDocumento + "|" +
                            r.NumeroDocumento + "|" +
                            getcuentas.FirstOrDefault(x => x.CuentaLlave == "Ingreso Noa Credito Ventas").CuentaCode + "|" +
                            getcuentas.FirstOrDefault(x => x.CuentaLlave == "Ingreso Noa Credito Ventas").CuentaName + "|" +
                            r.Identidad + "|" +
                            "" + "|" +
                            ConvertirValor(r.Valor) + "|" +
                            getcuentas.FirstOrDefault(x => x.CuentaLlave == "Ingreso Noa Credito Ventas").Naturaleza + "|" +
                            "F" + "\n";

                        texto = texto + Convert.ToDateTime(r.Fecha).ToString("yyyy-MM-dd") + "|" +
                           r.TipoDocumento + "|" +
                           r.NumeroDocumento + "|" +
                           getcuentas.FirstOrDefault(x => x.CuentaLlave == "Ingreso Noa Credito Ventas Debito").CuentaCode + "|" +
                           getcuentas.FirstOrDefault(x => x.CuentaLlave == "Ingreso Noa Credito Ventas Debito").CuentaName + "|" +
                           r.Identidad + "|" +
                           "3" + "|" +
                           ConvertirValor(r.Valor) + "|" +
                           getcuentas.FirstOrDefault(x => x.CuentaLlave == "Ingreso Noa Credito Ventas Debito").Naturaleza + "|" +
                           "F" + "\n";

                        //Ingreso de Retenciones 
                        texto = texto + Convert.ToDateTime(r.Fecha).ToString("yyyy-MM-dd") + "|" +
                           r.TipoDocumento + "|" +
                           r.NumeroDocumento + "|" +
                           getcuentas.FirstOrDefault(x => x.CuentaLlave == "Retencion Nota Credito Ventas").CuentaCode + "|" +
                           getcuentas.FirstOrDefault(x => x.CuentaLlave == "Retencion Nota Credito Ventas").CuentaName + " " + r.TipoDocumento + r.NumeroDocumento + "|" +
                           r.Identidad + "|" +
                           "" + "|" +
                           ConvertirValor(((r.Valor * baseRetencion)) / 100) + "|" +
                           getcuentas.FirstOrDefault(x => x.CuentaLlave == "Retencion Nota Credito Ventas").Naturaleza + "|" +
                           "F" + "\n";

                        //Ingreso de Retenciones 
                        texto = texto + Convert.ToDateTime(r.Fecha).ToString("yyyy-MM-dd") + "|" +
                          r.TipoDocumento + "|" +
                          r.NumeroDocumento + "|" +
                          getcuentas.FirstOrDefault(x => x.CuentaLlave == "Autoretencion Nota Credito Ventas").CuentaCode + "|" +
                          getcuentas.FirstOrDefault(x => x.CuentaLlave == "Autoretencion Nota Credito Ventas").CuentaName + " " + r.TipoDocumento + r.NumeroDocumento + "|" +
                          r.Identidad + "|" +
                          "" + "|" +
                          ConvertirValor(((r.Valor * baseRetencion)) / 100) + "|" +
                          getcuentas.FirstOrDefault(x => x.CuentaLlave == "Autoretencion Nota Credito Ventas").Naturaleza + "|" +
                          "F" + "\n";                      
                    }

                    StreamWriter Escriba = new StreamWriter(QueryTxt);
                    Escriba.Write(texto);
                    Escriba.Close();
                }

                List<ReporteContable> getListaCuraciones = repoFacElectron.getReportContableNC(Cia, dateTimePicker1.Value.Date, dateTimePicker2.Value.Date, "Salud");
                if (getListaCuraciones != null)
                {
                    string texto = "FECHA|TIPO DE DOCUMENTO|NUMERO DE DOCUMENTO|CUENTA|CONCEPTO|IDENTIDAD|CENTRO DE COSTO|VALOR|NATURALEZA|CLASE\n";
                    FileStream QueryTxt = new FileStream("C:/Cxn/Reportes/ReporteContable_NC_CURACIONES_" + DateTime.Now.ToString("yyyy-MM-dd HHmmss") + ".csv", FileMode.Append, FileAccess.Write);

                    foreach (ReporteContable r in getListaCuraciones)
                    {
                        #region Ingreso de Factura
                        // Ingreso de Factura
                        texto = texto + Convert.ToDateTime(r.Fecha).ToString("yyyy-MM-dd") + "|" +
                            r.TipoDocumento + "|" +
                            r.NumeroDocumento + "|" +
                            getcuentas.FirstOrDefault(x => x.CuentaLlave == "Nota Credito Servicios").CuentaCode + "|" +
                            getcuentas.FirstOrDefault(x => x.CuentaLlave == "Nota Credito Servicios").CuentaName + "|" +
                            r.Identidad + "|" +
                            "" + "|" +
                            ConvertirValor(r.Valor - r.Descuentos) + "|" +
                            getcuentas.FirstOrDefault(x => x.CuentaLlave == "Nota Credito Servicios").Naturaleza + "|" +
                            "F" + "\n";

                        texto = texto + Convert.ToDateTime(r.Fecha).ToString("yyyy-MM-dd") + "|" +
                           r.TipoDocumento + "|" +
                           r.NumeroDocumento + "|" +
                           getcuentas.FirstOrDefault(x => x.CuentaLlave == "Nota Credito Servicios Debito").CuentaCode + "|" +
                           getcuentas.FirstOrDefault(x => x.CuentaLlave == "Nota Credito Servicios Debito").CuentaName + "|" +
                           r.Identidad + "|" +
                           "3" + "|" +
                           ConvertirValor(r.Valor - r.Descuentos) + "|" +
                           getcuentas.FirstOrDefault(x => x.CuentaLlave == "Nota Credito Servicios Debito").Naturaleza + "|" +
                           "F" + "\n";
                        #endregion

                        #region Ingreso descuentos
                        // Ingreso de Descuentos si los hay
                        if (r.Descuentos > 0)
                        {
                            texto = texto + Convert.ToDateTime(r.Fecha).ToString("yyyy-MM-dd") + "|" +
                           r.TipoDocumento + "|" +
                           r.NumeroDocumento + "|" +
                           getcuentas.FirstOrDefault(x => x.CuentaLlave == "Bonos Nota Credito Servicios").CuentaCode + "|" +
                           getcuentas.FirstOrDefault(x => x.CuentaLlave == "Bonos Nota Credito Servicios").CuentaName + " " + r.TipoDocumento + r.NumeroDocumento + "|" +
                           r.Identidad + "|" +
                           "" + "|" +
                           ConvertirValor(r.Descuentos) + "|" +
                           getcuentas.FirstOrDefault(x => x.CuentaLlave == "Bonos Nota Credito Servicios").Naturaleza + "|" +
                           "F" + "\n";
                        }
                        #endregion

                        #region Ingreso Retenciones
                        //Ingreso de Retenciones 1
                        texto = texto + Convert.ToDateTime(r.Fecha).ToString("yyyy-MM-dd") + "|" +
                           r.TipoDocumento + "|" +
                           r.NumeroDocumento + "|" +
                           getcuentas.FirstOrDefault(x => x.CuentaLlave == "Retencion Nota Credito Servicios").CuentaCode + "|" +
                           getcuentas.FirstOrDefault(x => x.CuentaLlave == "Retencion Nota Credito Servicios").CuentaName + " " + r.TipoDocumento + r.NumeroDocumento + "|" +
                           r.Identidad + "|" +
                           "" + "|" +
                           ConvertirValor(((((r.Valor)) * baseRetencion)) / 100) + "|" +
                           getcuentas.FirstOrDefault(x => x.CuentaLlave == "Retencion Nota Credito Servicios").Naturaleza + "|" +
                           "F" + "\n";

                        //Ingreso de Retenciones 2
                        texto = texto + Convert.ToDateTime(r.Fecha).ToString("yyyy-MM-dd") + "|" +
                          r.TipoDocumento + "|" +
                          r.NumeroDocumento + "|" +
                          getcuentas.FirstOrDefault(x => x.CuentaLlave == "Autoretencion Nota Credito Servicios").CuentaCode + "|" +
                          getcuentas.FirstOrDefault(x => x.CuentaLlave == "Autoretencion Nota Credito Servicios").CuentaName + " " + r.TipoDocumento + r.NumeroDocumento + "|" +
                          r.Identidad + "|" +
                          "" + "|" +
                          ConvertirValor(((((r.Valor)) * baseRetencion)) / 100) + "|" +
                          getcuentas.FirstOrDefault(x => x.CuentaLlave == "Autoretencion Nota Credito Servicios").Naturaleza + "|" +
                          "F" + "\n";
                        #endregion
                    }

                    StreamWriter Escriba = new StreamWriter(QueryTxt);
                    Escriba.Write(texto);
                    Escriba.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        void GenerarFibromialgia()
        {
            try
            {
                List<CXN_CUENTAS> getcuentas = cuentas.getCuentas(); 

                List<ReporteContable> getLista = repoFacElectron.getReportContable(Cia, dateTimePicker1.Value.Date, dateTimePicker2.Value.Date, "FIBROMIALGIA");
                if (getLista != null)
                {
                    string texto = "FECHA|TIPO DE DOCUMENTO|NUMERO DE DOCUMENTO|CUENTA|CONCEPTO|IDENTIDAD|CENTRO DE COSTO|VALOR|NATURALEZA|CLASE\n";
                    FileStream QueryTxt = new FileStream("C:/Cxn/Reportes/ReporteContable_FIBROMIALGIA_" + DateTime.Now.ToString("yyyy-MM-dd HHmmss") + ".csv", FileMode.Append, FileAccess.Write);

                    foreach (ReporteContable r in getLista)
                    { // Ingreso de Factura
                        texto = texto + Convert.ToDateTime(r.Fecha).ToString("yyyy-MM-dd") + "|" +
                 r.TipoDocumento + "|" +
                 r.NumeroDocumento + "|" +
                 getcuentas.FirstOrDefault(x => x.CuentaLlave == "Ingreso Factura Fibromialgia").CuentaCode + "|" +
                 getcuentas.FirstOrDefault(x => x.CuentaLlave == "Ingreso Factura Fibromialgia").CuentaName + "|" +
                 r.Identidad + "|" +
                 "04" + "|" +
                 ConvertirValor(r.Valor) + "|" +
                 getcuentas.FirstOrDefault(x => x.CuentaLlave == "Ingreso Factura Fibromialgia").Naturaleza + "|" +
                 "F" + "\n";

                        texto = texto + Convert.ToDateTime(r.Fecha).ToString("yyyy-MM-dd") + "|" +
                         r.TipoDocumento + "|" +
                         r.NumeroDocumento + "|" +
                         getcuentas.FirstOrDefault(x => x.CuentaLlave == "Ingreso Factura Fibromialgia Debito").CuentaCode + "|" +
                         getcuentas.FirstOrDefault(x => x.CuentaLlave == "Ingreso Factura Fibromialgia Debito").CuentaName + "|" +
                         r.Identidad + "|" +
                         "" + "|" +
                         ConvertirValor(r.Valor) + "|" +
                         getcuentas.FirstOrDefault(x => x.CuentaLlave == "Ingreso Factura Fibromialgia Debito").Naturaleza + "|" +
                         "F" + "\n";

                        //Ingreso de Retenciones 
                        texto = texto + Convert.ToDateTime(r.Fecha).ToString("yyyy-MM-dd") + "|" +
                         r.TipoDocumento + "|" +
                         r.NumeroDocumento + "|" +
                         getcuentas.FirstOrDefault(x => x.CuentaLlave == "Retencion Fibromialgia").CuentaCode + "|" +
                         getcuentas.FirstOrDefault(x => x.CuentaLlave == "Retencion Fibromialgia").CuentaName + " " + r.TipoDocumento + r.NumeroDocumento + "|" +
                         r.Identidad + "|" +
                         "" + "|" +
                         ConvertirValor(((r.Valor * baseRetencion)) / 100) + "|" +
                         getcuentas.FirstOrDefault(x => x.CuentaLlave == "Retencion Fibromialgia").Naturaleza + "|" +
                         "F" + "\n";

                        //Ingreso de Retenciones 
                        texto = texto + Convert.ToDateTime(r.Fecha).ToString("yyyy-MM-dd") + "|" +
                         r.TipoDocumento + "|" +
                         r.NumeroDocumento + "|" +
                         getcuentas.FirstOrDefault(x => x.CuentaLlave == "Autoretencion Fibromialgia").CuentaCode + "|" +
                         getcuentas.FirstOrDefault(x => x.CuentaLlave == "Autoretencion Fibromialgia").CuentaName + " " + r.TipoDocumento + r.NumeroDocumento + "|" +
                         r.Identidad + "|" +
                         "" + "|" +
                         ConvertirValor(((r.Valor * baseRetencion)) / 100) + "|" +
                         getcuentas.FirstOrDefault(x => x.CuentaLlave == "Autoretencion Fibromialgia").Naturaleza + "|" +
                         "F" + "\n";
                    }

                    StreamWriter Escriba = new StreamWriter(QueryTxt);
                    Escriba.Write(texto);
                    Escriba.Close();
                }                       
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        void GenerarVentas()
        {
            try
            {
                List<CXN_CUENTAS> getcuentas = cuentas.getCuentas();

                List<ReporteContable> getLista = repoFacElectron.getReportContable(Cia, dateTimePicker1.Value.Date, dateTimePicker2.Value.Date, "VENTAS");
                if (getLista != null)
                {
                    string texto = "FECHA|TIPO DE DOCUMENTO|NUMERO DE DOCUMENTO|CUENTA|CONCEPTO|IDENTIDAD|CENTRO DE COSTO|VALOR|NATURALEZA|CLASE\n";
                    FileStream QueryTxt = new FileStream("C:/Cxn/Reportes/ReporteContable_VENTAS_" + DateTime.Now.ToString("yyyy-MM-dd HHmmss") + ".csv", FileMode.Append, FileAccess.Write);

                    foreach (ReporteContable r in getLista)
                    {

                        // Ingreso de Factura
                        texto = texto + Convert.ToDateTime(r.Fecha).ToString("yyyy-MM-dd") + "|" +
                            r.TipoDocumento + "|" +
                            r.NumeroDocumento + "|" +
                            getcuentas.FirstOrDefault(x => x.CuentaLlave == "Venta Ingreso").CuentaCode + "|" +
                            getcuentas.FirstOrDefault(x => x.CuentaLlave == "Venta Ingreso").CuentaName + "|" +
                            r.Identidad + "|" +
                            "" + "|" +
                            ConvertirValor(r.Valor) + "|" +
                            getcuentas.FirstOrDefault(x => x.CuentaLlave == "Venta Ingreso").Naturaleza + "|" +
                            "F" + "\n";

                        //Ingreso de Retenciones 
                        texto = texto + Convert.ToDateTime(r.Fecha).ToString("yyyy-MM-dd") + "|" +
                           r.TipoDocumento + "|" +
                           r.NumeroDocumento + "|" +
                           getcuentas.FirstOrDefault(x => x.CuentaLlave == "Venta Retencion").CuentaCode + "|" +
                           getcuentas.FirstOrDefault(x => x.CuentaLlave == "Venta Retencion").CuentaName + " " + r.TipoDocumento + r.NumeroDocumento + "|" +
                           r.Identidad + "|" +
                           "" + "|" +
                           ConvertirValor(((r.Valor * baseRetencion)) / 100) + "|" +
                           getcuentas.FirstOrDefault(x => x.CuentaLlave == "Venta Retencion").Naturaleza + "|" +
                           "F" + "\n";

                        //Ingreso de Retenciones 
                        texto = texto + Convert.ToDateTime(r.Fecha).ToString("yyyy-MM-dd") + "|" +
                          r.TipoDocumento + "|" +
                          r.NumeroDocumento + "|" +
                          getcuentas.FirstOrDefault(x => x.CuentaLlave == "Venta Autoretencion").CuentaCode + "|" +
                          getcuentas.FirstOrDefault(x => x.CuentaLlave == "Venta Autoretencion").CuentaName + " " + r.TipoDocumento + r.NumeroDocumento + "|" +
                          r.Identidad + "|" +
                          "" + "|" +
                          ConvertirValor(((r.Valor * baseRetencion)) / 100) + "|" +
                          getcuentas.FirstOrDefault(x => x.CuentaLlave == "Venta Autoretencion").Naturaleza + "|" +
                          "F" + "\n";

                        texto = texto + Convert.ToDateTime(r.Fecha).ToString("yyyy-MM-dd") + "|" +
                           r.TipoDocumento + "|" +
                           r.NumeroDocumento + "|" +
                           getcuentas.FirstOrDefault(x => x.CuentaLlave == "Venta Ingreso Credito").CuentaCode + "|" +
                           getcuentas.FirstOrDefault(x => x.CuentaLlave == "Venta Ingreso Credito").CuentaName + "|" +
                           r.Identidad + "|" +
                           "03" + "|" +
                           ConvertirValor(r.Valor) + "|" +
                           getcuentas.FirstOrDefault(x => x.CuentaLlave == "Venta Ingreso Credito").Naturaleza + "|" +
                           "F" + "\n";
                    }

                    StreamWriter Escriba = new StreamWriter(QueryTxt);
                    Escriba.Write(texto);
                    Escriba.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        void GenerarCuraciones()
        {
            try
            {
                List<CXN_CUENTAS> getcuentas = cuentas.getCuentas();

                List<ReporteContable> getLista = repoFacElectron.getReportContable(Cia, dateTimePicker1.Value.Date, dateTimePicker2.Value.Date, "CURACIONES");
                if (getLista != null)
                {
                    string texto = "FECHA|TIPO DE DOCUMENTO|NUMERO DE DOCUMENTO|CUENTA|CONCEPTO|IDENTIDAD|CENTRO DE COSTO|VALOR|NATURALEZA|CLASE\n";
                    FileStream QueryTxt = new FileStream("C:/Cxn/Reportes/ReporteContable_CURACIONES_" + DateTime.Now.ToString("yyyy-MM-dd HHmmss") + ".csv", FileMode.Append, FileAccess.Write);

                    foreach (ReporteContable r in getLista)
                    {
                        #region Ingreso de Factura
                        // Ingreso de Factura
                        texto = texto + Convert.ToDateTime(r.Fecha).ToString("yyyy-MM-dd") + "|" +
                            r.TipoDocumento + "|" +
                            r.NumeroDocumento + "|" +
                            getcuentas.FirstOrDefault(x => x.CuentaLlave == "Ingreso Factura Curaciones").CuentaCode + "|" +
                            getcuentas.FirstOrDefault(x => x.CuentaLlave == "Ingreso Factura Curaciones").CuentaName + "|" +
                            r.Identidad + "|" +
                            "" + "|" +
                            ConvertirValor(r.Valor - r.Descuentos) + "|" +
                            getcuentas.FirstOrDefault(x => x.CuentaLlave == "Ingreso Factura Curaciones").Naturaleza + "|" +
                            "F" + "\n";
                        #endregion

                        #region Ingreso Bonos
                        // Bonos
                        if (r.Descuentos > 0)
                        {
                            texto = texto + Convert.ToDateTime(r.Fecha).ToString("yyyy-MM-dd") + "|" +
                           r.TipoDocumento + "|" +
                           r.NumeroDocumento + "|" +
                           getcuentas.FirstOrDefault(x => x.CuentaLlave == "Bonos Curaciones").CuentaCode + "|" +
                           getcuentas.FirstOrDefault(x => x.CuentaLlave == "Bonos Curaciones").CuentaName + " " + r.TipoDocumento + r.NumeroDocumento + "|" +
                           r.Identidad + "|" +
                           "" + "|" +
                           ConvertirValor(r.Descuentos) + "|" +
                           getcuentas.FirstOrDefault(x => x.CuentaLlave == "Bonos Curaciones").Naturaleza + "|" +
                           "F" + "\n";
                        }
                        #endregion

                        #region Ingreso Retenciones
                        //Ingreso de Retenciones 1
                        texto = texto + Convert.ToDateTime(r.Fecha).ToString("yyyy-MM-dd") + "|" +
                           r.TipoDocumento + "|" +
                           r.NumeroDocumento + "|" +
                           getcuentas.FirstOrDefault(x => x.CuentaLlave == "Retencion Curaciones").CuentaCode + "|" +
                           getcuentas.FirstOrDefault(x => x.CuentaLlave == "Retencion Curaciones").CuentaName + " " + r.TipoDocumento + r.NumeroDocumento + "|" +
                           r.Identidad + "|" +
                           "" + "|" +
                           ConvertirValor(((((r.Valor)) * baseRetencion)) / 100) + "|" +
                           getcuentas.FirstOrDefault(x => x.CuentaLlave == "Retencion Curaciones").Naturaleza + "|" +
                           "F" + "\n";                        

                        //Ingreso de Retenciones 2
                        texto = texto + Convert.ToDateTime(r.Fecha).ToString("yyyy-MM-dd") + "|" +
                          r.TipoDocumento + "|" +
                          r.NumeroDocumento + "|" +
                          getcuentas.FirstOrDefault(x => x.CuentaLlave == "Autoretencion Curaciones").CuentaCode + "|" +
                          getcuentas.FirstOrDefault(x => x.CuentaLlave == "Autoretencion Curaciones").CuentaName + " " + r.TipoDocumento + r.NumeroDocumento + "|" +
                          r.Identidad + "|" +
                          "" + "|" +
                          ConvertirValor(((((r.Valor)) * baseRetencion)) / 100) + "|" +
                          getcuentas.FirstOrDefault(x => x.CuentaLlave == "Autoretencion Curaciones").Naturaleza + "|" +
                          "F" + "\n";
                        #endregion

                        #region Insumos
                        //INSUMOS                            
                        CXN_CARGOS getCargosInsumos = repoFacElectron.getDetalleCargos(Cia, r.FacturaZamenis, "Cargos", "Insumo");
                        if (getCargosInsumos != null && getCargosInsumos.Car_Val_Tot > 0)
                        {
                            texto = texto + Convert.ToDateTime(r.Fecha).ToString("yyyy-MM-dd") + "|" +
                          r.TipoDocumento + "|" +
                          r.NumeroDocumento + "|" +
                          getcuentas.FirstOrDefault(x => x.CuentaLlave == "Insumos Curaciones").CuentaCode + "|" +
                          getcuentas.FirstOrDefault(x => x.CuentaLlave == "Insumos Curaciones").CuentaName + " " + r.TipoDocumento + r.NumeroDocumento + "|" +
                          r.Identidad + "|" +
                          "03" + "|" +
                          ConvertirValor(getCargosInsumos.Car_Val_Tot) + "|" +
                          getcuentas.FirstOrDefault(x => x.CuentaLlave == "Insumos Curaciones").Naturaleza + "|" +
                          "F" + "\n";
                        }
                        #endregion

                        #region Apositos

                        //APOSITOS
                        CXN_CARGOS getCargosAposito = repoFacElectron.getDetalleCargos(Cia, r.FacturaZamenis, "Cargos", "Aposito");
                        if (getCargosAposito != null && getCargosAposito.Car_Val_Tot > 0)
                        {
                            texto = texto + Convert.ToDateTime(r.Fecha).ToString("yyyy-MM-dd") + "|" +
                      r.TipoDocumento + "|" +
                      r.NumeroDocumento + "|" +
                      getcuentas.FirstOrDefault(x => x.CuentaLlave == "Apositos Curaciones").CuentaCode + "|" +
                      getcuentas.FirstOrDefault(x => x.CuentaLlave == "Apositos Curaciones").CuentaName + " " + r.TipoDocumento + r.NumeroDocumento + "|" +
                      r.Identidad + "|" +
                      "03" + "|" +
                      ConvertirValor(getCargosAposito.Car_Val_Tot) + "|" +
                      getcuentas.FirstOrDefault(x => x.CuentaLlave == "Apositos Curaciones").Naturaleza + "|" +
                      "F" + "\n";

                        }
                        #endregion

                        #region Servicios
                        //Ingreso de Curaciones si los hay
                        CXN_CARGOS getCargosCU = repoFacElectron.getDetalleCargos(Cia, r.FacturaZamenis, "Servicios", "CU");
                        if (getCargosCU != null && getCargosCU.Car_Val_Tot > 0)
                        {
                            texto = texto + Convert.ToDateTime(r.Fecha).ToString("yyyy-MM-dd") + "|" +
                             r.TipoDocumento + "|" +
                             r.NumeroDocumento + "|" +
                             getcuentas.FirstOrDefault(x => x.CuentaLlave == "Curaciones").CuentaCode + "|" +
                             getcuentas.FirstOrDefault(x => x.CuentaLlave == "Curaciones").CuentaName + " " + r.TipoDocumento + r.NumeroDocumento + "|" +
                             r.Identidad + "|" +
                             "03" + "|" +
                             ConvertirValor(getCargosCU.Car_Val_Tot) + "|" +
                             getcuentas.FirstOrDefault(x => x.CuentaLlave == "Curaciones").Naturaleza + "|" +
                             "F" + "\n";
                        }
                        //Ingreso de Consultas si los hay
                        CXN_CARGOS getCargosMG = repoFacElectron.getDetalleCargos(Cia, r.FacturaZamenis, "Servicios", "MG");
                        if (getCargosMG != null && getCargosMG.Car_Val_Tot > 0)
                        {
                            texto = texto + Convert.ToDateTime(r.Fecha).ToString("yyyy-MM-dd") + "|" +
                     r.TipoDocumento + "|" +
                     r.NumeroDocumento + "|" +
                     getcuentas.FirstOrDefault(x => x.CuentaLlave == "Consultas Curaciones").CuentaCode + "|" +
                     getcuentas.FirstOrDefault(x => x.CuentaLlave == "Consultas Curaciones").CuentaName + " " + r.TipoDocumento + r.NumeroDocumento + "|" +
                     r.Identidad + "|" +
                     "03" + "|" +
                     ConvertirValor(getCargosMG.Car_Val_Tot) + "|" +
                     getcuentas.FirstOrDefault(x => x.CuentaLlave == "Consultas Curaciones").Naturaleza + "|" +
                     "F" + "\n";                            
                        }
                        #endregion
                    }

                    StreamWriter Escriba = new StreamWriter(QueryTxt);
                    Escriba.Write(texto);
                    Escriba.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        void GenerarBonos()
        {
            try
            {
                List<CXN_CUENTAS> getcuentas = cuentas.getCuentas();

                List<ReporteContable> getLista = repoFacElectron.getReportContable(Cia, dateTimePicker1.Value.Date, dateTimePicker2.Value.Date, "BONOS");
                if (getLista != null)
                {
                    string texto = "FECHA|TIPO DE DOCUMENTO|NUMERO DE DOCUMENTO|CUENTA|CONCEPTO|IDENTIDAD|CENTRO DE COSTO|VALOR|NATURALEZA|CLASE\n";
                    FileStream QueryTxt = new FileStream("C:/Cxn/Reportes/ReporteContable_BONOS_" + DateTime.Now.ToString("yyyy-MM-dd HHmmss") + ".csv", FileMode.Append, FileAccess.Write);

                    foreach (ReporteContable r in getLista)
                    {
                        // Ingreso de Factura
                        texto = texto + Convert.ToDateTime(r.Fecha).ToString("yyyy-MM-dd") + "|" +
                            r.TipoDocumento + "|" +
                            r.NumeroDocumento + "|" +
                            getcuentas.FirstOrDefault(x => x.CuentaLlave == "Bonos General").CuentaCode + "|" +
                            getcuentas.FirstOrDefault(x => x.CuentaLlave == "Bonos General").CuentaName + "|" +
                            r.Identidad + "|" +
                            "|" +
                            ConvertirValor(r.Valor) + "|" +
                            getcuentas.FirstOrDefault(x => x.CuentaLlave == "Bonos General").Naturaleza + "|" +
                            "F" + "\n";

                        texto = texto + Convert.ToDateTime(r.Fecha).ToString("yyyy-MM-dd") + "|" +
                           r.TipoDocumento + "|" +
                           r.NumeroDocumento + "|" +
                           getcuentas.FirstOrDefault(x => x.CuentaLlave == "Bonos General Credito").CuentaCode + "|" +
                           getcuentas.FirstOrDefault(x => x.CuentaLlave == "Bonos General Credito").CuentaName + "|" +
                           r.Identidad + "|" +
                           "03" + "|" +
                           ConvertirValor(r.Valor) + "|" +
                           getcuentas.FirstOrDefault(x => x.CuentaLlave == "Bonos General Credito").Naturaleza + "|" +
                           "F" + "\n";

                        //Ingreso de Retenciones  Debito
                        texto = texto + Convert.ToDateTime(r.Fecha).ToString("yyyy-MM-dd") + "|" +
                           r.TipoDocumento + "|" +
                           r.NumeroDocumento + "|" +
                           getcuentas.FirstOrDefault(x => x.CuentaLlave == "Retencion Bonos").CuentaCode + "|" +
                           getcuentas.FirstOrDefault(x => x.CuentaLlave == "Retencion Bonos").CuentaName + " " + r.TipoDocumento + r.NumeroDocumento + "|" +
                           r.Identidad + "|" +
                           "" + "|" +
                           ConvertirValor(((r.Valor * baseRetencion)) / 100) + "|" +
                           getcuentas.FirstOrDefault(x => x.CuentaLlave == "Retencion Bonos").Naturaleza + "|" +
                           "F" + "\n";

                        //Ingreso de Retenciones  Credito
                        texto = texto + Convert.ToDateTime(r.Fecha).ToString("yyyy-MM-dd") + "|" +
                          r.TipoDocumento + "|" +
                          r.NumeroDocumento + "|" +
                          getcuentas.FirstOrDefault(x => x.CuentaLlave == "Autoretencion Bonos").CuentaCode + "|" +
                          getcuentas.FirstOrDefault(x => x.CuentaLlave == "Autoretencion Bonos").CuentaName + " " + r.TipoDocumento + r.NumeroDocumento + "|" +
                          r.Identidad + "|" +
                          "" + "|" +
                          ConvertirValor(((r.Valor * baseRetencion)) / 100) + "|" +
                          getcuentas.FirstOrDefault(x => x.CuentaLlave == "Autoretencion Bonos").Naturaleza + "|" +
                          "F" + "\n";
                    }

                    StreamWriter Escriba = new StreamWriter(QueryTxt);
                    Escriba.Write(texto);
                    Escriba.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                string TokenGenerado = repoFacElectron.GetTokenSaved(Cia).Trim();
                if (TokenGenerado == null)
                {
                    MessageBox.Show("No hay resultados para tokens generados, genere uno nuevo antes de emitir facturacion");
                    return;
                }

                GenerateXMLPDF.ReenviarPDFSalud(textBox7.Text, Cia, comboBox2.Text, TokenGenerado);

                MG = new MensajesGeneral();
                MG.Mensaje = "Hecho";
                MG.TipoImagen = 3;
                MG.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            AjustesDIAN ajustesDIAN = new AjustesDIAN(Cia);
            ajustesDIAN.ShowDialog();
        }
    }
}
