using Domain.CXN;
using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZamenisHealth.Clases;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.FacElectron
{
    public partial class MenuFacElectron : Forma2
    {
        private static readonly IFacElectron repoFacElectron = new MFacElectron();
        private static readonly ICompañia repoCia = new MCompañia();

        private MensajesGeneral MG;
        private int Cia;
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
        async Task GenerarReporte()
        {
            pictureBox1.Visible = true;

            try
            {

                DataTable dt = await repoFacElectron.getReportContableCompleto(Cia,
                                                                        "F",
                                                                        dateTimePicker1.Value.Date,
                                                                        dateTimePicker2.Value.Date);

                if (dt != null)
                {
                    string texto = "FECHA|TIPO DE DOCUMENTO|NUMERO DE DOCUMENTO|CUENTA|CONCEPTO|IDENTIDAD|CENTRO DE COSTO|VALOR|NATURALEZA|CLASE \r";
                    FileStream QueryTxt = new FileStream("C:/Cxn/Reportes/ReporteContable_" + DateTime.Now.ToString("yyyy-MM-dd HHmmss") + ".csv", FileMode.Append, FileAccess.Write);

                    foreach (DataRow fila in dt.Rows)
                    {
                        texto = texto +
                                fila["FECHA"].ToString() + "|" +
                                fila["TIPO DE DOCUMENTO"].ToString() + "|" +
                                fila["NUMERO DE DOCUMENTO"].ToString() + "|" +
                                fila["CUENTA"].ToString() + "|" +
                                fila["CONCEPTO"].ToString() + "|" +
                                fila["IDENTIDAD"].ToString() + "|" +
                                fila["CENTRO DE COSTO"].ToString() + "|" +
                                fila["VALOR"].ToString() + "|" +
                                fila["NATURALEZA"].ToString() + "|" +
                                fila["CLASE"].ToString() + "\r";
                    }

                    StreamWriter Escriba = new StreamWriter(QueryTxt);
                    Escriba.Write(texto);
                    Escriba.Close();

                    MG = new MensajesGeneral()
                    {
                        Mensaje = "Hecho",
                        TipoImagen = 3
                    };

                    MG.ShowDialog();
                }
                else
                {
                    MG = new MensajesGeneral()
                    {
                        Mensaje = "No hay datos para mostrar",
                        TipoImagen = 1000
                    };

                    MG.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            pictureBox1.Visible = false;
        }
        async void button1_Click(object sender, EventArgs e)
        {
            try
            {
                await GenerarReporte();            
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
