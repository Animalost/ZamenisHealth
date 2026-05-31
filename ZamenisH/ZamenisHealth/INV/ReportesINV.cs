using Domain.INV;
using FormAndControls;
using Persistence;
using Persistence.INV.Interfaces;
using Persistence.INV.Metodos;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ZamenisHealth.Clases;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.INV
{
    public partial class ReportesINV : Forma
    {
        private IProductos productos;
        private MensajesGeneral MG;

        public ReportesINV()
        {
            InitializeComponent();
            productos = new MProductos();
        }

        private void ReportesINV_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Reportes";
            LogoMain.Image = Properties.Resources.Splash;
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";

            ToolStripButton btnIngresos;
            ToolStripButton btnSalidas;

            btnIngresos = new ToolStripButton();
            btnIngresos = createToolButton("Ingresos");
            MenuLateral.Items.Add(btnIngresos);
            btnIngresos.Click += button1_Click;

            btnSalidas = new ToolStripButton();
            btnSalidas = createToolButton("Salidas");
            MenuLateral.Items.Add(btnSalidas);
            btnSalidas.Click += toolStripButton2_Click;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (comboBox1.Text == "" || comboBox2.Text == "")
                {
                    MG = new MensajesGeneral();
                    MG.Mensaje = "Debe seleccionar mes y año";
                    MG.TipoImagen = 1000;
                    MG.ShowDialog();
                }
                else
                {
                    DateTime Desde = new DateTime(Convert.ToInt32(comboBox2.Text), getMonthNumber(comboBox1.Text), 1);
                    DateTime Hasta = new DateTime(Convert.ToInt32(comboBox2.Text), getMonthNumber(comboBox1.Text), getMonthLastDay(comboBox1.Text));

                    List<ReportsINV> ingresos = productos.ReporteIngresos(Desde, Hasta);
                    if (ingresos != null)
                    {
                        ConfigForm.GenerarReportViewer("DataSet_INV",
                                                       "ZamenisHealth.Reportes.RDLC_INV_ING_SAL.rdlc",
                                                       ingresos);
                    }
                    else
                    {
                        MG = new MensajesGeneral();
                        MG.Mensaje = "No hay datos para este periodo seleccionado";
                        MG.TipoImagen = 3;
                        MG.ShowDialog();
                    }
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
                if (comboBox1.Text == "" || comboBox2.Text == "")
                {
                    MG = new MensajesGeneral();
                    MG.Mensaje = "Debe seleccionar mes y año";
                    MG.TipoImagen = 1000;
                    MG.ShowDialog();
                }
                else
                {
                    DateTime Desde = new DateTime(Convert.ToInt32(comboBox2.Text), getMonthNumber(comboBox1.Text), 1);
                    DateTime Hasta = new DateTime(Convert.ToInt32(comboBox2.Text), getMonthNumber(comboBox1.Text), getMonthLastDay(comboBox1.Text));

                    List<ReportsINV> ingresos = productos.ReporteSalidas(Desde, Hasta);
                    if (ingresos != null)
                    {
                        ConfigForm.GenerarReportViewer("DataSet_INV",
                                                       "ZamenisHealth.Reportes.RDLC_INV_ING_SAL.rdlc",
                                                       ingresos);
                    }
                    else
                    {
                        MG = new MensajesGeneral();
                        MG.Mensaje = "No hay datos para este periodo seleccionado";
                        MG.TipoImagen = 3;
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
