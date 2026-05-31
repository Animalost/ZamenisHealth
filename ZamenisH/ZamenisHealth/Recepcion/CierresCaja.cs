using Domain.CXN;
using FormAndControls;
using Persistence.CXN;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ZamenisHealth.Clases;
using ZamenisHealth.Comunes;
using ZamenisHealth.INV.Consultorios;

namespace ZamenisHealth.Recepcion
{
    public partial class CierresCaja : Forma
    {
        private readonly ICIerresCaja iCIerresCaja = new MCierresCaja();
        private readonly ICompañia iCompañia = new MCompañia();

        private MensajesGeneral MG;
        private DateTime Desde;
        private DateTime Hasta;
        private int Cia;

        private int CienMil = 100000;
        private int CincuentaMil = 50000;
        private int VeinteMil = 20000;
        private int DiezMil = 10000;
        private int CincoMil = 5000;
        private int DosMil = 2000;
        private int Mil = 1000;

        private int mMil = 1000;
        private int mQuinientos = 500;
        private int mDoscientos = 200;
        private int mCien = 100;
        private int mCincuenta = 50;

        private int ConsecutivoCierre;

        public CierresCaja(DateTime desde, DateTime hasta, int cia)
        {
            InitializeComponent();
            Desde = desde;
            Hasta = hasta;
            Cia = cia;

            SoloNumeros(textBox16);
            SoloNumeros(textBox15);
            SoloNumeros(textBox14);
            SoloNumeros(textBox13);
            SoloNumeros(textBox7);

            SoloNumeros(textBox1);
            SoloNumeros(textBox2);
            SoloNumeros(textBox3);
            SoloNumeros(textBox4);
            SoloNumeros(textBox5);
            SoloNumeros(textBox6);

            SoloNumeros(textBox12);
            SoloNumeros(textBox11);
            SoloNumeros(textBox10);
            SoloNumeros(textBox9);
            SoloNumeros(textBox8);
        }

        private void CierresCaja_Load(object sender, EventArgs e)
        {
            this.Titulo.Text = "Cierres de Caja";
            LogoMain.Image = Properties.Resources.Splash;

            ToolStripButton btnGenerar = new ToolStripButton();
            btnGenerar = createToolButton("Generar");
            MenuLateral.Items.Add(btnGenerar);
            btnGenerar.Click += button1_Click;

            label1.Text = "Cierre de Caja desde " + Desde.ToString("dd/MM/yyyy") + " hasta " + Hasta.ToString("dd/MM/yyyy");
            CargarValores();
            CargarCons();
        }
        void CargarCons() 
        { 
            ConsecutivoCierre = iCompañia.getPrestadorbyCode(Cia).Com_Cierres;
        }

        void CargarValores()
        {
            try
            {
                Dictionary<string, int> ingresos = iCIerresCaja.getIngresos(Cia, Desde, Hasta);
                if (ingresos != null)
                {
                    label14.Text = ingresos.ContainsKey("Efectivo") ? "$ " + ingresos["Efectivo"].ToString("N2") : "$ 0";
                    label13.Text = ingresos.ContainsKey("Tarjeta Credito") ? "$ " + ingresos["Tarjeta Credito"].ToString("N2") : "$ 0";
                    label12.Text = ingresos.ContainsKey("Tarjeta Debito") ? "$ " + ingresos["Tarjeta Debito"].ToString("N2") : "$ 0";
                    label11.Text = ingresos.ContainsKey("Nequi") ? "$ " + ingresos["Nequi"].ToString("N2") : "$ 0";
                    label10.Text = ingresos.ContainsKey("Daviplata") ? "$ " + ingresos["Daviplata"].ToString("N2") : "$ 0";
                    label9.Text = ingresos.ContainsKey("Otras Billeteras") ? "$ " + ingresos["Otras Billeteras"].ToString("N2") : "$ 0";
                    label47.Text = ingresos.ContainsKey("Total") ? "$ " + ingresos["Total"].ToString("N2") : "$ 0";

                    label59.Text = label47.Text;
                }
                else
                {
                    MG = new MensajesGeneral();
                    MG.Mensaje = "No hay ventas en este rango de fechas";
                    MG.TipoImagen = 3;
                    MG.ShowDialog();

                    this.Dispose();
                    this.Dispose();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los valores: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void SumaEgresos()
        {
            try
            {
                int Valor1 = string.IsNullOrEmpty(textBox16.Text) ? 0 : Convert.ToInt32(textBox16.Text);
                int Valor2 = string.IsNullOrEmpty(textBox15.Text) ? 0 : Convert.ToInt32(textBox15.Text);
                int Valor3 = string.IsNullOrEmpty(textBox14.Text) ? 0 : Convert.ToInt32(textBox14.Text);
                int Valor4 = string.IsNullOrEmpty(textBox13.Text) ? 0 : Convert.ToInt32(textBox13.Text);
                int Valor5 = string.IsNullOrEmpty(textBox7.Text) ? 0 : Convert.ToInt32(textBox7.Text);

                label49.Text = Valor1 + Valor2 + Valor3 + Valor4 + Valor5 > 0
                    ? "$ " + (Valor1 + Valor2 + Valor3 + Valor4 + Valor5).ToString("N2")
                    : "$ 0";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                label49.Text = "$ 0";
            }
        }

        private void textBoxEgreso_TextChanged(object sender, EventArgs e)
        {
            SumaEgresos();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                int Valor = string.IsNullOrEmpty(textBox1.Text) ? 0 : Convert.ToInt32(textBox1.Text);
                label17.Text = "$ " + (Valor * CienMil).ToString("N2");
                SumaBilletes();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al procesar el texto: " + ex.Message);
                label17.Text = "$ 0";
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            try
            {
                int Valor = string.IsNullOrEmpty(textBox2.Text) ? 0 : Convert.ToInt32(textBox2.Text);
                label16.Text = "$ " + (Valor * CincuentaMil).ToString("N2");
                SumaBilletes();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al procesar el texto: " + ex.Message);
                label16.Text = "$ 0";
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            try
            {
                int Valor = string.IsNullOrEmpty(textBox3.Text) ? 0 : Convert.ToInt32(textBox3.Text);
                label15.Text = "$ " + (Valor * VeinteMil).ToString("N2");
                SumaBilletes();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al procesar el texto: " + ex.Message);
                label15.Text = "$ 0";
            }
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            try
            {
                int Valor = string.IsNullOrEmpty(textBox4.Text) ? 0 : Convert.ToInt32(textBox4.Text);
                label30.Text = "$ " + (Valor * DiezMil).ToString("N2");
                SumaBilletes();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al procesar el texto: " + ex.Message);
                label30.Text = "$ 0";
            }
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            try
            {
                int Valor = string.IsNullOrEmpty(textBox5.Text) ? 0 : Convert.ToInt32(textBox5.Text);
                label29.Text = "$ " + (Valor * CincoMil).ToString("N2");
                SumaBilletes();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al procesar el texto: " + ex.Message);
                label29.Text = "$ 0";
            }
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            try
            {
                int Valor = string.IsNullOrEmpty(textBox6.Text) ? 0 : Convert.ToInt32(textBox6.Text);
                label28.Text = "$ " + (Valor * DosMil).ToString("N2");
                SumaBilletes();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al procesar el texto: " + ex.Message);
                label28.Text = "$ 0";
            }
        }

        private void textBox22_TextChanged(object sender, EventArgs e)
        {
            try
            {
                int Valor = string.IsNullOrEmpty(textBox22.Text) ? 0 : Convert.ToInt32(textBox22.Text);
                label53.Text = "$ " + (Valor * Mil).ToString("N2");
                SumaBilletes();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al procesar el texto: " + ex.Message);
                label53.Text = "$ 0";
            }
        }
        void SumaBilletes()
        {
            try
            {
                int Valor1 = string.IsNullOrEmpty(textBox1.Text) ? 0 : Convert.ToInt32(textBox1.Text);
                int Valor2 = string.IsNullOrEmpty(textBox2.Text) ? 0 : Convert.ToInt32(textBox2.Text);
                int Valor3 = string.IsNullOrEmpty(textBox3.Text) ? 0 : Convert.ToInt32(textBox3.Text);
                int Valor4 = string.IsNullOrEmpty(textBox4.Text) ? 0 : Convert.ToInt32(textBox4.Text);
                int Valor5 = string.IsNullOrEmpty(textBox5.Text) ? 0 : Convert.ToInt32(textBox5.Text);
                int Valor6 = string.IsNullOrEmpty(textBox6.Text) ? 0 : Convert.ToInt32(textBox6.Text);
                int Valor7 = string.IsNullOrEmpty(textBox22.Text) ? 0 : Convert.ToInt32(textBox22.Text);

                int Vr1 = Valor1 * CienMil;
                int Vr2 = Valor2 * CincuentaMil;
                int Vr3 = Valor3 * VeinteMil;
                int Vr4 = Valor4 * DiezMil;
                int Vr5 = Valor5 * CincoMil;
                int Vr6 = Valor6 * DosMil;
                int Vr7 = Valor7 * Mil;

                label55.Text = Vr1 + Vr2 + Vr3 + Vr4 + Vr5 + Vr6 + Vr7 > 0
                    ? "$ " + (Vr1 + Vr2 + Vr3 + Vr4 + Vr5 + Vr6 + Vr7).ToString("N2")
                    : "$ 0";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                label55.Text = "$ 0";
            }
        }

        private void textBox12_TextChanged(object sender, EventArgs e)
        {
            try
            {
                int Valor = string.IsNullOrEmpty(textBox12.Text) ? 0 : Convert.ToInt32(textBox12.Text);
                label36.Text = "$ " + (Valor * mMil).ToString("N2");
                SumaMonedas();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al procesar el texto: " + ex.Message);
                label36.Text = "$ 0";
            }
        }

        private void textBox11_TextChanged(object sender, EventArgs e)
        {
            try
            {
                int Valor = string.IsNullOrEmpty(textBox11.Text) ? 0 : Convert.ToInt32(textBox11.Text);
                label35.Text = "$ " + (Valor * mQuinientos).ToString("N2");
                SumaMonedas();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al procesar el texto: " + ex.Message);
                label35.Text = "$ 0";
            }
        }

        private void textBox10_TextChanged(object sender, EventArgs e)
        {
            try
            {
                int Valor = string.IsNullOrEmpty(textBox10.Text) ? 0 : Convert.ToInt32(textBox10.Text);
                label34.Text = "$ " + (Valor * mDoscientos).ToString("N2");
                SumaMonedas();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al procesar el texto: " + ex.Message);
                label34.Text = "$ 0";
            }
        }

        private void textBox9_TextChanged(object sender, EventArgs e)
        {
            try
            {
                int Valor = string.IsNullOrEmpty(textBox9.Text) ? 0 : Convert.ToInt32(textBox9.Text);
                label33.Text = "$ " + (Valor * mCien).ToString("N2");
                SumaMonedas();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al procesar el texto: " + ex.Message);
                label33.Text = "$ 0";
            }
        }

        private void textBox8_TextChanged(object sender, EventArgs e)
        {
            try
            {
                int Valor = string.IsNullOrEmpty(textBox8.Text) ? 0 : Convert.ToInt32(textBox8.Text);
                label32.Text = "$ " + (Valor * mCincuenta).ToString("N2");
                SumaMonedas();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al procesar el texto: " + ex.Message);
                label32.Text = "$ 0";
            }
        }
        void SumaMonedas()
        {
            try
            {
                int Valor1 = string.IsNullOrEmpty(textBox12.Text) ? 0 : Convert.ToInt32(textBox12.Text);
                int Valor2 = string.IsNullOrEmpty(textBox11.Text) ? 0 : Convert.ToInt32(textBox11.Text);
                int Valor3 = string.IsNullOrEmpty(textBox10.Text) ? 0 : Convert.ToInt32(textBox10.Text);
                int Valor4 = string.IsNullOrEmpty(textBox9.Text) ? 0 : Convert.ToInt32(textBox9.Text);
                int Valor5 = string.IsNullOrEmpty(textBox8.Text) ? 0 : Convert.ToInt32(textBox8.Text);

                int Vr1 = Valor1 * mMil;
                int Vr2 = Valor2 * mQuinientos;
                int Vr3 = Valor3 * mDoscientos;
                int Vr4 = Valor4 * mCien;
                int Vr5 = Valor5 * mCincuenta;

                label57.Text = Vr1 + Vr2 + Vr3 + Vr4 + Vr5 > 0
                    ? "$ " + (Vr1 + Vr2 + Vr3 + Vr4 + Vr5 ).ToString("N2")
                    : "$ 0";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                label57.Text = "$ 0";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult result = MessageBox.Show("¿Desea crear este informe?",
                                                 "Zamenis Health - Informe de cierre de caja",
                                                 MessageBoxButtons.YesNo,
                                                 MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    Dictionary<string, int> ingresos = iCIerresCaja.getIngresos(Cia, Desde, Hasta);
                    if (ingresos != null)
                    {
                        CXN_REPORTECAJA C = new CXN_REPORTECAJA
                        {
                            BCien = 100000,
                            BCienCantidad = (string.IsNullOrEmpty(textBox1.Text) ? 0 : Convert.ToInt32(textBox1.Text)),
                            BCienValor = CienMil * (string.IsNullOrEmpty(textBox1.Text) ? 0 : Convert.ToInt32(textBox1.Text)),
                            BCincuenta = 50000,
                            BCincuentaCantidad = (string.IsNullOrEmpty(textBox2.Text) ? 0 : Convert.ToInt32(textBox2.Text)),
                            BCincuentaValor = CincuentaMil * (string.IsNullOrEmpty(textBox2.Text) ? 0 : Convert.ToInt32(textBox2.Text)),
                            BVeinte = 20000,
                            BVeinteCantidad = (string.IsNullOrEmpty(textBox3.Text) ? 0 : Convert.ToInt32(textBox3.Text)),
                            BVeinteValor = VeinteMil * (string.IsNullOrEmpty(textBox3.Text) ? 0 : Convert.ToInt32(textBox3.Text)),
                            BDiez = 10000,
                            BDiezCantidad = (string.IsNullOrEmpty(textBox4.Text) ? 0 : Convert.ToInt32(textBox4.Text)),
                            BDiezValor = DiezMil * (string.IsNullOrEmpty(textBox4.Text) ? 0 : Convert.ToInt32(textBox4.Text)),
                            BCinco = 5000,
                            BCincoCantidad = (string.IsNullOrEmpty(textBox5.Text) ? 0 : Convert.ToInt32(textBox5.Text)),
                            BCincoValor = CincoMil * (string.IsNullOrEmpty(textBox5.Text) ? 0 : Convert.ToInt32(textBox5.Text)),
                            BDosMil = 2000,
                            BDosMilCantidad = (string.IsNullOrEmpty(textBox6.Text) ? 0 : Convert.ToInt32(textBox6.Text)),
                            BDosMilValor = DosMil * (string.IsNullOrEmpty(textBox6.Text) ? 0 : Convert.ToInt32(textBox6.Text)),
                            BMil = 1000,
                            BMilCantidad = (string.IsNullOrEmpty(textBox22.Text) ? 0 : Convert.ToInt32(textBox22.Text)),
                            BMilValor = Mil * (string.IsNullOrEmpty(textBox22.Text) ? 0 : Convert.ToInt32(textBox22.Text)),

                            TipoPagoEfectivo = "Efectivo",
                            TipoPagoEfectivoValor = (string.IsNullOrEmpty(label14.Text) ? 0 : Convert.ToInt32(Numero(label14.Text))),
                            TipoPagoTC = "Tarjeta Credito",
                            TipoPagoTCValor = (string.IsNullOrEmpty(label13.Text) ? 0 : Convert.ToInt32(Numero(label13.Text))),
                            TipoPagoDB = "Tarjeta Debito",
                            TipoPagoDBValor = (string.IsNullOrEmpty(label12.Text) ? 0 : Convert.ToInt32(Numero(label12.Text))),
                            TipoPagoNequi = "Nequi",
                            TipoPagoNequiValor = (string.IsNullOrEmpty(label11.Text) ? 0 : Convert.ToInt32(Numero(label11.Text))),
                            TipoPagoDaviplata = "Daviplata",
                            TipoPagoDaviplataValor = (string.IsNullOrEmpty(label10.Text) ? 0 : Convert.ToInt32(Numero(label10.Text))),
                            TipoPagoOtraBilletera = "Otras Billeteras",
                            TipoPagoOtraBilleteraValor = (string.IsNullOrEmpty(label9.Text) ? 0 : Convert.ToInt32(Numero(label9.Text))),

                            MMil = mMil,
                            MMilCantidad = (string.IsNullOrEmpty(textBox12.Text) ? 0 : Convert.ToInt32(textBox12.Text)),
                            MMilValor = mMil * (string.IsNullOrEmpty(textBox12.Text) ? 0 : Convert.ToInt32(textBox12.Text)),
                            MQuinientos = mQuinientos,
                            MQuinientosCantidad = (string.IsNullOrEmpty(textBox11.Text) ? 0 : Convert.ToInt32(textBox11.Text)),
                            MQuinientosValor = mMil * (string.IsNullOrEmpty(textBox11.Text) ? 0 : Convert.ToInt32(textBox11.Text)),
                            MDoscientos = mDoscientos,
                            MDoscientosCantidad = (string.IsNullOrEmpty(textBox10.Text) ? 0 : Convert.ToInt32(textBox10.Text)),
                            MDoscientosValor = mMil * (string.IsNullOrEmpty(textBox10.Text) ? 0 : Convert.ToInt32(textBox10.Text)),
                            MCien = mCien,
                            MCienCantidad = (string.IsNullOrEmpty(textBox9.Text) ? 0 : Convert.ToInt32(textBox9.Text)),
                            MCienValor = mMil * (string.IsNullOrEmpty(textBox9.Text) ? 0 : Convert.ToInt32(textBox9.Text)),
                            MCincuenta = mCincuenta,
                            MCincuentaCantidad = (string.IsNullOrEmpty(textBox8.Text) ? 0 : Convert.ToInt32(textBox8.Text)),
                            MCincuentaValor = mMil * (string.IsNullOrEmpty(textBox8.Text) ? 0 : Convert.ToInt32(textBox8.Text)),

                            EgresoRazon1 = textBox21.Text,
                            EgresoValor1 = (string.IsNullOrEmpty(textBox16.Text) ? 0 : Convert.ToInt32(Numero(textBox16.Text))),
                            EgresoRazon2 = textBox20.Text,
                            EgresoValor2 = (string.IsNullOrEmpty(textBox15.Text) ? 0 : Convert.ToInt32(Numero(textBox15.Text))),
                            EgresoRazon3 = textBox19.Text,
                            EgresoValor3 = (string.IsNullOrEmpty(textBox14.Text) ? 0 : Convert.ToInt32(Numero(textBox14.Text))),
                            EgresoRazon4 = textBox18.Text,
                            EgresoValor4 = (string.IsNullOrEmpty(textBox13.Text) ? 0 : Convert.ToInt32(Numero(textBox13.Text))),
                            EgresoRazon5 = textBox17.Text,
                            EgresoValor5 = (string.IsNullOrEmpty(textBox7.Text) ? 0 : Convert.ToInt32(Numero(textBox7.Text))),

                            Consecutivo = ConsecutivoCierre.ToString(),
                            Cia = Cia,
                            Desde = Convert.ToDateTime(Desde),
                            Hasta = Convert.ToDateTime(Hasta),
                            Observacion = richTextBox1.Text,
                            Usuario = Contenedor.UsuarioLogueado
                        };

                        iCIerresCaja.GrabarReporte(C);
                        iCompañia.ConsecutivoActualiza(Cia, "CIERRES", ConsecutivoCierre + 1);
                        iCIerresCaja.ActualizarNumCruce(ConsecutivoCierre, Cia, Convert.ToDateTime(Desde), Convert.ToDateTime(Hasta));

                        List<CXN_REPORTECAJA> getReport = iCIerresCaja.GetReport(ConsecutivoCierre.ToString());
                        if (getReport != null) 
                        {                   
                            ConfigForm.GenerarReportViewer("DataSet_CierreCaja", 
                                "ZamenisHealth.Reportes.CierreCaja.rdlc", 
                                getReport);

                            this.Dispose();
                            this.Close();
                        }
                        else
                        {
                            MG = new MensajesGeneral();
                            MG.Mensaje = "No se logro exportar el reporte";
                            MG.TipoImagen = 3;
                            MG.ShowDialog();
                        }
                    }
                }
            }
            catch (Exception ex)
            { 
                Console.WriteLine(ex.Message); 
            }
        }

        int Numero (string tNumero)
        {
            try
            {
                tNumero = tNumero.Replace("$", "").Trim();
                int indiceComa = tNumero.IndexOf(',');
                if (indiceComa != -1)
                    tNumero = tNumero.Substring(0, indiceComa);
                tNumero = tNumero.Replace(".", "");
                return int.Parse(tNumero);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 0;
            }           
        }
    }
}
