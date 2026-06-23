using Domain.CXN;
using FormAndControls;
using Microsoft.Reporting.WinForms;
using Persistence.CXN;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ZamenisHealth.Comunes;

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

        private int ConsecutivoCierre;

        //CAJA
        int ValTotalEfectivo = 0;
        int ValTotalTC = 0;
        int ValTotalTD = 0;
        int ValTotalNequi = 0;
        int ValTotalDaviplata = 0;
        int ValTotalOtras = 0;

        //VENTAS
        int ValTotalEfectivoV = 0;
        int ValTotalTCV = 0;
        int ValTotalTDV = 0;
        int ValTotalNequiV = 0;
        int ValTotalDaviplataV = 0;
        int ValTotalOtrasV = 0;

        //PARTICULARES
        int ValTotalEfectivoP = 0;
        int ValTotalTCP = 0;
        int ValTotalTDP = 0;
        int ValTotalNequiP = 0;
        int ValTotalDaviplataP = 0;
        int ValTotalOtrasP = 0;

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
                (Dictionary<string, int> Ventas, 
                    Dictionary<string, int> Caja, 
                    Dictionary<string, int> Particulares) ingresos = iCIerresCaja.getIngresos(Cia, Desde, Hasta);            

                if (ingresos.Caja != null)
                {
                    if (ingresos.Caja.TryGetValue("Efectivo", out int valorE))
                    {
                        ValTotalEfectivo = valorE;
                    }
                    if (ingresos.Caja.TryGetValue("Tarjeta Credito", out int valorTC))
                    {
                        ValTotalTC = valorTC;
                    }
                    if (ingresos.Caja.TryGetValue("Tarjeta Debito", out int valorTD))
                    {
                        ValTotalTD = valorTD;
                    }
                    if (ingresos.Caja.TryGetValue("Nequi", out int valorN))
                    {
                        ValTotalNequi = valorN;
                    }
                    if (ingresos.Caja.TryGetValue("Daviplata", out int valorD))
                    {
                        ValTotalDaviplata = valorD;
                    }
                    if (ingresos.Caja.TryGetValue("Otras Billeteras", out int valorOB))
                    {
                        ValTotalOtras = valorOB;
                    }

                    label14.Text = $" $ {Convert.ToInt32(ValTotalEfectivo).ToString("N0")}";
                    label13.Text = $" $ {Convert.ToInt32(ValTotalTC).ToString("N0")}";
                    label12.Text = $" $ {Convert.ToInt32(ValTotalTD).ToString("N0")}";
                    label11.Text = $" $ {Convert.ToInt32(ValTotalNequi).ToString("N0")}";
                    label10.Text = $" $ {Convert.ToInt32(ValTotalDaviplata).ToString("N0")}";
                    label19.Text = $" $ {Convert.ToInt32(ValTotalOtras).ToString("N0")}";
                }

                label47.Text = Convert.ToInt32(ValTotalEfectivo + ValTotalTC + ValTotalTD + ValTotalNequi + ValTotalDaviplata + ValTotalOtras).ToString("N0");

                if (ingresos.Ventas != null)
                {
                    if (ingresos.Ventas.TryGetValue("Efectivo", out int valorE))
                    {
                        ValTotalEfectivoV = valorE;
                    }
                    if (ingresos.Ventas.TryGetValue("Tarjeta Credito", out int valorTC))
                    {
                        ValTotalTCV = valorTC;
                    }
                    if (ingresos.Ventas.TryGetValue("Tarjeta Debito", out int valorTD))
                    {
                        ValTotalTDV = valorTD;
                    }
                    if (ingresos.Ventas.TryGetValue("Nequi", out int valorN))
                    {
                        ValTotalNequiV = valorN;
                    }
                    if (ingresos.Ventas.TryGetValue("Daviplata", out int valorD))
                    {
                        ValTotalDaviplataV = valorD;
                    }
                    if (ingresos.Ventas.TryGetValue("Otras Billeteras", out int valorOB))
                    {
                        ValTotalOtrasV = valorOB;
                    }

                    label22.Text = $" $ {Convert.ToInt32(ValTotalEfectivoV).ToString("N0")}";
                    label21.Text = $" $ {Convert.ToInt32(ValTotalTCV).ToString("N0")}";
                    label20.Text = $" $ {Convert.ToInt32(ValTotalTDV).ToString("N0")}";
                    label19.Text = $" $ {Convert.ToInt32(ValTotalNequiV).ToString("N0")}";
                    label18.Text = $" $ {Convert.ToInt32(ValTotalDaviplataV).ToString("N0")}";
                    label17.Text = $" $ {Convert.ToInt32(ValTotalOtrasV).ToString("N0")}";
                }

                label15.Text = Convert.ToInt32(ValTotalEfectivoV + ValTotalTCV + ValTotalTDV + ValTotalNequiV + ValTotalDaviplataV + ValTotalOtrasV).ToString("N0");

                if (ingresos.Particulares != null)
                {
                    if (ingresos.Particulares.TryGetValue("Efectivo", out int valorE))
                    {
                        ValTotalEfectivoP = valorE;
                    }
                    if (ingresos.Particulares.TryGetValue("Tarjeta Credito", out int valorTC))
                    {
                        ValTotalTCP = valorTC;
                    }
                    if (ingresos.Particulares.TryGetValue("Tarjeta Debito", out int valorTD))
                    {
                        ValTotalTDP = valorTD;
                    }
                    if (ingresos.Particulares.TryGetValue("Nequi", out int valorN))
                    {
                        ValTotalNequiP = valorN;
                    }
                    if (ingresos.Particulares.TryGetValue("Daviplata", out int valorD))
                    {
                        ValTotalDaviplataP = valorD;
                    }
                    if (ingresos.Particulares.TryGetValue("Otras Billeteras", out int valorOB))
                    {
                        ValTotalOtrasP = valorOB;
                    }

                    label39.Text = $" $ {Convert.ToInt32(ValTotalEfectivoP).ToString("N0")}";
                    label38.Text = $" $ {Convert.ToInt32(ValTotalTCP).ToString("N0")}";
                    label36.Text = $" $ {Convert.ToInt32(ValTotalTDP).ToString("N0")}";
                    label35.Text = $" $ {Convert.ToInt32(ValTotalNequiP).ToString("N0")}";
                    label34.Text = $" $ {Convert.ToInt32(ValTotalDaviplataP).ToString("N0")}";
                    label33.Text = $" $ {Convert.ToInt32(ValTotalOtrasP).ToString("N0")}";
                }

                label30.Text = Convert.ToInt32(ValTotalEfectivoP + ValTotalTCP + ValTotalTDP + ValTotalNequiP + ValTotalDaviplataP + ValTotalOtrasP).ToString("N0");

                label49.Text = "0";
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
                int Sumatoria =  ValTotalEfectivo + ValTotalTC + ValTotalTD + ValTotalNequi + ValTotalDaviplata + ValTotalOtras +
                                 ValTotalEfectivoV + ValTotalTCV + ValTotalTDV + ValTotalNequiV + ValTotalDaviplataV + ValTotalOtrasV +
                                 ValTotalEfectivoP + ValTotalTCP + ValTotalTDP + ValTotalNequiP + ValTotalDaviplataP + ValTotalOtrasP;

                int Resta = string.IsNullOrEmpty(textBox16.Text) ? 0 : Convert.ToInt32(textBox16.Text);
                int Resta2 = string.IsNullOrEmpty(textBox15.Text) ? 0 : Convert.ToInt32(textBox15.Text);
                int Resta3 = string.IsNullOrEmpty(textBox14.Text) ? 0 : Convert.ToInt32(textBox14.Text);
                int Resta4 = string.IsNullOrEmpty(textBox13.Text) ? 0 : Convert.ToInt32(textBox13.Text);
                int Resta5 = string.IsNullOrEmpty(textBox7.Text) ? 0 : Convert.ToInt32(textBox7.Text);

                int SumRestaTemp = Resta + Resta2 + Resta3 + Resta4 + Resta5;

                label49.Text = Convert.ToInt32(SumRestaTemp).ToString("N0");
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
                    List<CXN_REPORTECAJA2> Resultado = new List<CXN_REPORTECAJA2>();

                    (Dictionary<string, int> Ventas,
                     Dictionary<string, int> Caja,
                     Dictionary<string, int> Particulares) ingresos = iCIerresCaja.getIngresos(Cia, Desde, Hasta);

                    if (ingresos.Ventas != null || ingresos.Caja != null || ingresos.Particulares != null)
                    {                        
                        if (ingresos.Caja != null)
                        {
                            if (ingresos.Caja.TryGetValue("Efectivo", out int valorE))
                            {
                                Resultado.Add(new CXN_REPORTECAJA2
                                {
                                    Consecutivo = ConsecutivoCierre,
                                    Usuario = Contenedor.UsuarioLogueado,
                                    Tipo = "CAJA",
                                    Desde = Desde,
                                    Hasta = Hasta,
                                    Generacion = DateTime.Now.Date,
                                    Clase = "Efectivo",
                                    Valor = valorE,
                                    Estado = "H",
                                    Observacion = richTextBox1.Text,
                                    Compañia = Cia
                                });
                            }
                            if (ingresos.Caja.TryGetValue("Tarjeta Credito", out int valorTC))
                            {
                                Resultado.Add(new CXN_REPORTECAJA2
                                {
                                    Consecutivo = ConsecutivoCierre,
                                    Usuario = Contenedor.UsuarioLogueado,
                                    Tipo = "CAJA",
                                    Desde = Desde,
                                    Hasta = Hasta,
                                    Generacion = DateTime.Now.Date,
                                    Clase = "Tarjeta Credito",
                                    Valor = valorTC,
                                    Estado = "H",
                                    Observacion = richTextBox1.Text,
                                    Compañia = Cia
                                });
                            }
                            if (ingresos.Caja.TryGetValue("Tarjeta Debito", out int valorTD))
                            {
                                Resultado.Add(new CXN_REPORTECAJA2
                                {
                                    Consecutivo = ConsecutivoCierre,
                                    Usuario = Contenedor.UsuarioLogueado,
                                    Tipo = "CAJA",
                                    Desde = Desde,
                                    Hasta = Hasta,
                                    Generacion = DateTime.Now.Date,
                                    Clase = "Tarjeta Debito",
                                    Valor = valorTD,
                                    Estado = "H",
                                    Observacion = richTextBox1.Text,
                                    Compañia = Cia
                                });
                            }
                            if (ingresos.Caja.TryGetValue("Nequi", out int valorN))
                            {
                                Resultado.Add(new CXN_REPORTECAJA2
                                {
                                    Consecutivo = ConsecutivoCierre,
                                    Usuario = Contenedor.UsuarioLogueado,
                                    Tipo = "CAJA",
                                    Desde = Desde,
                                    Hasta = Hasta,
                                    Generacion = DateTime.Now.Date,
                                    Clase = "Nequi",
                                    Valor = valorN,
                                    Estado = "H",
                                    Observacion = richTextBox1.Text,
                                    Compañia = Cia
                                });
                            }
                            if (ingresos.Caja.TryGetValue("Daviplata", out int valorD))
                            {
                                Resultado.Add(new CXN_REPORTECAJA2
                                {
                                    Consecutivo = ConsecutivoCierre,
                                    Usuario = Contenedor.UsuarioLogueado,
                                    Tipo = "CAJA",
                                    Desde = Desde,
                                    Hasta = Hasta,
                                    Generacion = DateTime.Now.Date,
                                    Clase = "Daviplata",
                                    Valor = valorD,
                                    Estado = "H",
                                    Observacion = richTextBox1.Text,
                                    Compañia = Cia
                                });
                            }
                            if (ingresos.Caja.TryGetValue("Otras Billeteras", out int valorOB))
                            {
                                Resultado.Add(new CXN_REPORTECAJA2
                                {
                                    Consecutivo = ConsecutivoCierre,
                                    Usuario = Contenedor.UsuarioLogueado,
                                    Tipo = "CAJA",
                                    Desde = Desde,
                                    Hasta = Hasta,
                                    Generacion = DateTime.Now.Date,
                                    Clase = "Otras Billeteras",
                                    Valor = valorOB,
                                    Estado = "H",
                                    Observacion = richTextBox1.Text,
                                    Compañia = Cia
                                });
                            }
                        }

                        if (ingresos.Ventas != null)
                        {
                            if (ingresos.Ventas.TryGetValue("Efectivo", out int valorE))
                            {
                                Resultado.Add(new CXN_REPORTECAJA2
                                {
                                    Consecutivo = ConsecutivoCierre,
                                    Usuario = Contenedor.UsuarioLogueado,
                                    Tipo = "VENTAS",
                                    Desde = Desde,
                                    Hasta = Hasta,
                                    Generacion = DateTime.Now.Date,
                                    Clase = "Efectivo",
                                    Valor = valorE,
                                    Estado = "H",
                                    Observacion = richTextBox1.Text,
                                    Compañia = Cia
                                });
                            }
                            if (ingresos.Ventas.TryGetValue("Tarjeta Credito", out int valorTC))
                            {
                                Resultado.Add(new CXN_REPORTECAJA2
                                {
                                    Consecutivo = ConsecutivoCierre,
                                    Usuario = Contenedor.UsuarioLogueado,
                                    Tipo = "VENTAS",
                                    Desde = Desde,
                                    Hasta = Hasta,
                                    Generacion = DateTime.Now.Date,
                                    Clase = "Tarjeta Credito",
                                    Valor = valorTC,
                                    Estado = "H",
                                    Observacion = richTextBox1.Text,
                                    Compañia = Cia
                                });
                            }
                            if (ingresos.Ventas.TryGetValue("Tarjeta Debito", out int valorTD))
                            {
                                Resultado.Add(new CXN_REPORTECAJA2
                                {
                                    Consecutivo = ConsecutivoCierre,
                                    Usuario = Contenedor.UsuarioLogueado,
                                    Tipo = "VENTAS",
                                    Desde = Desde,
                                    Hasta = Hasta,
                                    Generacion = DateTime.Now.Date,
                                    Clase = "Tarjeta Debito",
                                    Valor = valorTD,
                                    Estado = "H",
                                    Observacion = richTextBox1.Text,
                                    Compañia = Cia
                                });
                            }
                            if (ingresos.Ventas.TryGetValue("Nequi", out int valorN))
                            {
                                Resultado.Add(new CXN_REPORTECAJA2
                                {
                                    Consecutivo = ConsecutivoCierre,
                                    Usuario = Contenedor.UsuarioLogueado,
                                    Tipo = "VENTAS",
                                    Desde = Desde,
                                    Hasta = Hasta,
                                    Generacion = DateTime.Now.Date,
                                    Clase = "Nequi",
                                    Valor = valorN,
                                    Estado = "H",
                                    Observacion = richTextBox1.Text,
                                    Compañia = Cia
                                });
                            }
                            if (ingresos.Ventas.TryGetValue("Daviplata", out int valorD))
                            {
                                Resultado.Add(new CXN_REPORTECAJA2
                                {
                                    Consecutivo = ConsecutivoCierre,
                                    Usuario = Contenedor.UsuarioLogueado,
                                    Tipo = "VENTAS",
                                    Desde = Desde,
                                    Hasta = Hasta,
                                    Generacion = DateTime.Now.Date,
                                    Clase = "Daviplata",
                                    Valor = valorD,
                                    Estado = "H",
                                    Observacion = richTextBox1.Text,
                                    Compañia = Cia
                                });
                            }
                            if (ingresos.Ventas.TryGetValue("Otras Billeteras", out int valorOB))
                            {
                                Resultado.Add(new CXN_REPORTECAJA2
                                {
                                    Consecutivo = ConsecutivoCierre,
                                    Usuario = Contenedor.UsuarioLogueado,
                                    Tipo = "VENTAS",
                                    Desde = Desde,
                                    Hasta = Hasta,
                                    Generacion = DateTime.Now.Date,
                                    Clase = "Otras Billeteras",
                                    Valor = valorOB,
                                    Estado = "H",
                                    Observacion = richTextBox1.Text,
                                    Compañia = Cia
                                });
                            }
                        }

                        if (ingresos.Particulares != null)
                        {
                            if (ingresos.Particulares.TryGetValue("Efectivo", out int valorE))
                            {
                                Resultado.Add(new CXN_REPORTECAJA2
                                {
                                    Consecutivo = ConsecutivoCierre,
                                    Usuario = Contenedor.UsuarioLogueado,
                                    Tipo = "PARTICULARES",
                                    Desde = Desde,
                                    Hasta = Hasta,
                                    Generacion = DateTime.Now.Date,
                                    Clase = "Efectivo",
                                    Valor = valorE,
                                    Estado = "H",
                                    Observacion = richTextBox1.Text,
                                    Compañia = Cia
                                });
                            }
                            if (ingresos.Particulares.TryGetValue("Tarjeta Credito", out int valorTC))
                            {
                                Resultado.Add(new CXN_REPORTECAJA2
                                {
                                    Consecutivo = ConsecutivoCierre,
                                    Usuario = Contenedor.UsuarioLogueado,
                                    Tipo = "PARTICULARES",
                                    Desde = Desde,
                                    Hasta = Hasta,
                                    Generacion = DateTime.Now.Date,
                                    Clase = "Tarjeta Credito",
                                    Valor = valorTC,
                                    Estado = "H",
                                    Observacion = richTextBox1.Text,
                                    Compañia = Cia
                                });
                            }
                            if (ingresos.Particulares.TryGetValue("Tarjeta Debito", out int valorTD))
                            {
                                Resultado.Add(new CXN_REPORTECAJA2
                                {
                                    Consecutivo = ConsecutivoCierre,
                                    Usuario = Contenedor.UsuarioLogueado,
                                    Tipo = "PARTICULARES",
                                    Desde = Desde,
                                    Hasta = Hasta,
                                    Generacion = DateTime.Now.Date,
                                    Clase = "Tarjeta Debito",
                                    Valor = valorTD,
                                    Estado = "H",
                                    Observacion = richTextBox1.Text,
                                    Compañia = Cia
                                });
                            }
                            if (ingresos.Particulares.TryGetValue("Nequi", out int valorN))
                            {
                                Resultado.Add(new CXN_REPORTECAJA2
                                {
                                    Consecutivo = ConsecutivoCierre,
                                    Usuario = Contenedor.UsuarioLogueado,
                                    Tipo = "PARTICULARES",
                                    Desde = Desde,
                                    Hasta = Hasta,
                                    Generacion = DateTime.Now.Date,
                                    Clase = "Nequi",
                                    Valor = valorN,
                                    Estado = "H",
                                    Observacion = richTextBox1.Text,
                                    Compañia = Cia
                                });
                            }
                            if (ingresos.Particulares.TryGetValue("Daviplata", out int valorD))
                            {
                                Resultado.Add(new CXN_REPORTECAJA2
                                {
                                    Consecutivo = ConsecutivoCierre,
                                    Usuario = Contenedor.UsuarioLogueado,
                                    Tipo = "PARTICULARES",
                                    Desde = Desde,
                                    Hasta = Hasta,
                                    Generacion = DateTime.Now.Date,
                                    Clase = "Daviplata",
                                    Valor = valorD,
                                    Estado = "H",
                                    Observacion = richTextBox1.Text,
                                    Compañia = Cia
                                });
                            }
                            if (ingresos.Particulares.TryGetValue("Otras Billeteras", out int valorOB))
                            {
                                Resultado.Add(new CXN_REPORTECAJA2
                                {
                                    Consecutivo = ConsecutivoCierre,
                                    Usuario = Contenedor.UsuarioLogueado,
                                    Tipo = "PARTICULARES",
                                    Desde = Desde,
                                    Hasta = Hasta,
                                    Generacion = DateTime.Now.Date,
                                    Clase = "Otras Billeteras",
                                    Valor = valorOB,
                                    Estado = "H",
                                    Observacion = richTextBox1.Text,
                                    Compañia = Cia
                                });
                            }
                        }

                        #region EGRESOS
                        if (!string.IsNullOrEmpty(textBox21.Text) && !string.IsNullOrEmpty(textBox16.Text))
                        {
                            Resultado.Add(new CXN_REPORTECAJA2
                            {
                                Consecutivo = ConsecutivoCierre,
                                Usuario = Contenedor.UsuarioLogueado,
                                Tipo = "EGRESOS",
                                Desde = Desde,
                                Hasta = Hasta,
                                Generacion = DateTime.Now.Date,
                                Clase = "Efectivo",
                                Valor = Convert.ToInt32(textBox16.Text),
                                Estado = "H",
                                Observacion = textBox21.Text,
                                Compañia = Cia
                            });
                        }
                        if (!string.IsNullOrEmpty(textBox20.Text) && !string.IsNullOrEmpty(textBox15.Text))
                        {
                            Resultado.Add(new CXN_REPORTECAJA2
                            {
                                Consecutivo = ConsecutivoCierre,
                                Usuario = Contenedor.UsuarioLogueado,
                                Tipo = "EGRESOS",
                                Desde = Desde,
                                Hasta = Hasta,
                                Generacion = DateTime.Now.Date,
                                Clase = "Efectivo",
                                Valor = Convert.ToInt32(textBox15.Text),
                                Estado = "H",
                                Observacion = textBox20.Text,
                                Compañia = Cia
                            });
                        }
                        if (!string.IsNullOrEmpty(textBox19.Text) && !string.IsNullOrEmpty(textBox14.Text))
                        {
                            Resultado.Add(new CXN_REPORTECAJA2
                            {
                                Consecutivo = ConsecutivoCierre,
                                Usuario = Contenedor.UsuarioLogueado,
                                Tipo = "EGRESOS",
                                Desde = Desde,
                                Hasta = Hasta,
                                Generacion = DateTime.Now.Date,
                                Clase = "Efectivo",
                                Valor = Convert.ToInt32(textBox14.Text),
                                Estado = "H",
                                Observacion = textBox19.Text,
                                Compañia = Cia
                            });
                        }
                        if (!string.IsNullOrEmpty(textBox18.Text) && !string.IsNullOrEmpty(textBox13.Text))
                        {
                            Resultado.Add(new CXN_REPORTECAJA2
                            {
                                Consecutivo = ConsecutivoCierre,
                                Usuario = Contenedor.UsuarioLogueado,
                                Tipo = "EGRESOS",
                                Desde = Desde,
                                Hasta = Hasta,
                                Generacion = DateTime.Now.Date,
                                Clase = "Efectivo",
                                Valor = Convert.ToInt32(textBox13.Text),
                                Estado = "H",
                                Observacion = textBox18.Text,
                                Compañia = Cia
                            });
                        }
                        if (!string.IsNullOrEmpty(textBox17.Text) && !string.IsNullOrEmpty(textBox7.Text))
                        {
                            Resultado.Add(new CXN_REPORTECAJA2
                            {
                                Consecutivo = ConsecutivoCierre,
                                Usuario = Contenedor.UsuarioLogueado,
                                Tipo = "EGRESOS",
                                Desde = Desde,
                                Hasta = Hasta,
                                Generacion = DateTime.Now.Date,
                                Clase = "Efectivo",
                                Valor = Convert.ToInt32(textBox7.Text),
                                Estado = "H",
                                Observacion = textBox17.Text,
                                Compañia = Cia
                            });
                        }
                        #endregion
                    
                        foreach (CXN_REPORTECAJA2 i in Resultado)
                        {
                            CXN_REPORTECAJA2 R = new CXN_REPORTECAJA2()
                            {
                                Clase = i.Clase,
                                Consecutivo = i.Consecutivo,
                                Desde = i.Desde,
                                Estado = i.Estado,
                                Generacion = i.Generacion,
                                Hasta = i.Hasta,
                                Observacion = i.Observacion,
                                Tipo = i.Tipo,
                                Usuario = i.Usuario,
                                Valor = i.Valor,
                                Compañia = i.Compañia
                            };

                            iCIerresCaja.GrabarReporte2(R);
                        }

                        MG = new MensajesGeneral()
                        {
                            Mensaje = "Reporte " + ConsecutivoCierre.ToString() + " generado exitosamente",
                            TipoImagen = 3
                        };

                        MG.ShowDialog();

                        iCompañia.ConsecutivoActualiza(Cia, "CIERRES", ConsecutivoCierre + 1);
                        iCIerresCaja.ActualizarNumCruce(ConsecutivoCierre, Cia, Convert.ToDateTime(Desde), Convert.ToDateTime(Hasta));

                        List<CXN_REPORTECAJA2> getReport = iCIerresCaja.GetReport2(ConsecutivoCierre.ToString(), Cia);
                        if (getReport != null)
                        {
                            int TotalIngreso = getReport.Where(x => x.Tipo != "EGRESOS" && x.Clase == "Efectivo").Sum(x => x.Valor);
                            int TotalEgreso = getReport.Where(x => x.Tipo == "EGRESOS").Sum(x => x.Valor);
                            int Entregar = TotalIngreso - TotalEgreso;

                            foreach (var i in getReport)
                            {
                                i.Estado = i.Estado == "H" ? "VIGENTE" : "ANULADO";
                                i.IdRC = Entregar;
                                i.ObservacionGeneral = getReport[0].Observacion;
                            }

                            List<CXN_REPORTECAJA2> Caja = getReport.Where(x => x.Tipo == "CAJA").ToList();
                            List<CXN_REPORTECAJA2> Ventas = getReport.Where(x => x.Tipo == "VENTAS").ToList();
                            List<CXN_REPORTECAJA2> Particulares = getReport.Where(x => x.Tipo == "PARTICULARES").ToList();
                            List<CXN_REPORTECAJA2> Egresos = getReport.Where(x => x.Tipo == "EGRESOS").ToList();

                            Reportes.Maestro maestro = new Reportes.Maestro();
                            maestro.Universal.LocalReport.DataSources.Clear();

                            maestro.Universal.LocalReport.DataSources.Add(new ReportDataSource("DataSet_CierresEncabezado", getReport));
                            maestro.Universal.LocalReport.DataSources.Add(new ReportDataSource("DataSet_CierresCaja", Caja));
                            maestro.Universal.LocalReport.DataSources.Add(new ReportDataSource("DataSet_CierresVentas", Ventas));
                            maestro.Universal.LocalReport.DataSources.Add(new ReportDataSource("DataSet_CierresParticulares", Particulares));
                            maestro.Universal.LocalReport.DataSources.Add(new ReportDataSource("DataSet_CierresEgresos", Egresos));

                            maestro.Universal.LocalReport.ReportEmbeddedResource = "ZamenisHealth.Reportes.CierreCaja2.rdlc";
                            maestro.Universal.SetDisplayMode(DisplayMode.PrintLayout);
                            maestro.Universal.ZoomMode = ZoomMode.Percent;
                            maestro.Universal.ZoomPercent = 100;
                            maestro.Universal.LocalReport.EnableExternalImages = true;
                            maestro.Universal.Font = new Font("Arial", 8);
                            maestro.Universal.RefreshReport();
                            maestro.Universal.Visible = true;
                            maestro.Universal.Dock = System.Windows.Forms.DockStyle.Fill;
                            maestro.ShowDialog();                          
                        }
                        else
                        {
                            MG = new MensajesGeneral();
                            MG.Mensaje = "No se logro exportar el reporte";
                            MG.TipoImagen = 3;
                            MG.ShowDialog();
                        }
                    }
                    else
                    {
                        MG = new MensajesGeneral()
                        {
                            Mensaje = "No hay datos validos para generar un cierre de caja"
                        };
                        MG.ShowDialog();
                    }
                }

                this.Close();
            }
            catch (Exception ex)
            { 
                Console.WriteLine(ex.Message); 
            }
        }
    }
}
