using Domain.CXN;
using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.Facturacion
{
    public partial class ReportesPagos : Forma
    {
        private IReportesPagos oController;
        
        private ToolStripButton btnGenerar;
        private int idCia, idAse;
        private MensajesGeneral MG;

        public ReportesPagos()
        {
            InitializeComponent();
            oController = new MReportesPagos();
        }

        private void ReportesPagos_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Reporte de Pagos";
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";
            LogoMain.Image = Properties.Resources.Splash;

            btnGenerar = new ToolStripButton();
            btnGenerar = createToolButton("Generar Reporte");
            MenuLateral.Items.Add(btnGenerar);
            btnGenerar.Click += btnGenerar_Click;

            foreach (string i in Años())
            {
                comboBox2.Items.Add(i);
            }
            foreach (string i2 in Meses())
            {
                comboBox3.Items.Add(i2);
            }

            CargarAseguradoras();
            CargarPrestadores();
        }
        void CargarAseguradoras()
        {
            List<CXN_ASEGURADORA> getLis = oController.getAseguradoras();
            if (getLis != null)
            {
                foreach (CXN_ASEGURADORA i in getLis)
                {
                    comboBox4.Items.Add(i.Ase_Descripcion);
                }
            }
        }
        void CargarPrestadores()
        {
            List<CXN_CIA> getLis = oController.getAllCompañias();
            if (getLis != null)
            {
                foreach (CXN_CIA i in getLis)
                {
                    comboBox1.Items.Add(i.Com_Nombre);
                }
            }
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            idCia = oController.getPrestadorbyName(comboBox1.Text).Com_Identificador;
        }
        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            idAse = oController.getInfoFromAsebyName(comboBox4.Text).Ase_Identificador;
        }
        async void btnGenerar_Click(object sender, EventArgs e) 
        { 
            try
            {
                if (comboBox1.Text == "" || comboBox2.Text == "" ||
                    comboBox3.Text == "" || comboBox4.Text == "")
                {
                    MG = new MensajesGeneral()
                    {
                        Mensaje = "Debe seleccionar una opcion de cada item",
                        TipoImagen = 1000
                    };

                    MG.ShowDialog();
                }
                else
                {
                    pictureBox1.Visible = true;
                    label5.Visible = true;
                    btnGenerar.Enabled = false;

                    DateTime desde = new DateTime(Convert.ToInt32(comboBox2.Text), getMonthNumber(comboBox3.Text), 1);
                    DateTime hasta = new DateTime(Convert.ToInt32(comboBox2.Text), getMonthNumber(comboBox3.Text), getMonthLastDay(comboBox3.Text));

                    CXN_PAGOSASEGURADORAS p = new CXN_PAGOSASEGURADORAS()
                    {
                        Desde = desde,
                        Hasta = hasta,
                        Prestador = idCia,
                        Aseguradora = idAse
                    };

                    List<CXN_PAGOSASEGURADORAS> getListado = await oController.ReportePagos(p);
                    if (getListado != null)
                    {
                        string texto = "PRESTADOR|ASEGURADORA|EXTRACTO|FECHA DE PAGO|FACTURA|CONCEPTO|VALOR";
                        string nameFile = @"C:\Cxn\Reportes\Pagos_" + DateTime.Now.ToString("yyyy-MM-dd HH mm ss tt") + ".xls";

                        FileStream Query = new FileStream(nameFile, FileMode.Append, FileAccess.Write);
                        StreamWriter Escriba = new StreamWriter(Query);

                        Escriba.Write("PRESTADOR|ASEGURADORA|EXTRACTO|FECHA DE PAGO|FACTURA|CONCEPTO|VALOR");
                        Escriba.WriteLine();
                        Escriba.Flush();

                        foreach (CXN_PAGOSASEGURADORAS i in getListado)
                        {
                            Escriba.Write(i.PrestadorName.Trim() + "|");
                            Escriba.Write(i.AseguradoraName.Trim() + "|");
                            Escriba.Write(i.Extracto.Trim() + "|");
                            Escriba.Write(i.FechaPago.ToString("yyyy-MM-dd").Trim() + "|");
                            Escriba.Write(i.Factura.Trim() + "|");
                            Escriba.Write(i.Concepto.Trim() + "|");
                            Escriba.Write(Convert.ToInt32(i.Valor).ToString("N0").Trim());

                            Escriba.WriteLine();
                            Escriba.Flush();
                        }

                        pictureBox1.Visible = false;
                        label5.Visible = false;
                        btnGenerar.Enabled = true;

                        MG = new MensajesGeneral()
                        {
                            Mensaje = "Generado",
                            TipoImagen = 3
                        };

                        MG.ShowDialog();

                        if (File.Exists(nameFile))
                        {
                            Process.Start(new ProcessStartInfo
                            {
                                FileName = nameFile,
                                UseShellExecute = true
                            });
                        }
                        else
                        {
                            MG = new MensajesGeneral()
                            {
                                Mensaje = "El archivo no existe",
                                TipoImagen = 1000
                            };

                            MG.ShowDialog();
                        }
                    }
                    else
                    {
                        MG = new MensajesGeneral()
                        {
                            Mensaje = "No hay datos para este periodo seleccionado",
                            TipoImagen = 3
                        };

                        MG.ShowDialog();
                    }
                }
            }
            catch (Exception ex) 
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
