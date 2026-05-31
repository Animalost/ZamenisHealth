using Domain;
using Domain.CXN;
using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.IO;
using System.Windows.Forms;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.Facturacion
{
    public partial class RIPS_Docker_3 : Forma2
    {
        private static readonly IHomologos repoHomologos = new MHomologos();
        private static readonly IFacturacion repoFacturacion = new MFacturacion();

        private Comunes.MensajesGeneral MG;
        private OpenFileDialog openFileDialog;

        public RIPS_Docker_3()
        {
            InitializeComponent();
        }

        private void RIPS_Docker_3_Load(object sender, EventArgs e)
        {
            this.Titulo.Text = "CUV Masivo";
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";
        }

        private void button1_Click(object sender, EventArgs e)
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

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                string texto = "FACTURA|CUV|PRESTADOR|DETALLE\r";

                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    string FH = Convert.ToString(row.Cells["FACTURA"].Value); //FACTURA ELECTRONICA
                    string CUV = Convert.ToString(row.Cells["CUV"].Value); //CODIGO CUV
                    string CIA = Convert.ToString(row.Cells["PRESTADOR"].Value); //CODIGO CUV

                    if (string.IsNullOrEmpty(FH) || string.IsNullOrEmpty(CUV) || string.IsNullOrEmpty(CIA)) 
                    {
                        continue;
                    }

                    CXN_FACTURA factura = repoFacturacion.getFacElectronica(FH);
                    if (factura == null)
                    {
                        texto = texto + FH + "|" + CUV + "|" + CIA + "|Factura electronica no existe\r";
                    }
                    else
                    {
                        if (factura.Fac_Cia == Convert.ToInt32(CIA))
                        {
                            if (!string.IsNullOrEmpty(factura.CUV))
                            {
                                texto = texto + FH + "|" + CUV + "|" + CIA + "|Factura ya tiene CUV " + factura.CUV.Trim() + "\r";
                            }
                            else
                            {
                                repoFacturacion.UpdateCUV(FH, CUV);
                                texto = texto + FH + "|" + CUV + "|" + CIA + "|CUV actualizado correctamente\r";
                            }
                        }
                        else
                        {
                            texto = texto + FH + "|" + CUV + "|" + CIA + "|Factura no pertenece a la compañia " + factura.Fac_Cia.ToString() + "\r";
                        }                       
                    }
                }

                FileStream Query = new FileStream("C:/Cxn/Reportes/CUV_Resultados.txt", FileMode.Append, FileAccess.Write);
                StreamWriter Escriba = new StreamWriter(Query);
                Escriba.Write(texto);
                Escriba.WriteLine();
                Escriba.Flush();
                Escriba.Close();

                MG = new Comunes.MensajesGeneral
                {
                    Mensaje = "Terminado, verifique el resultado en la carpeta Reportes",
                    TipoImagen = 3 
                };

                MG.ShowDialog();
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
    }
}
