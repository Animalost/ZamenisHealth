using iTextSharp.text.pdf;
using iTextSharp.text;
using System;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Persistence;
using Domain;
using FormAndControls;

namespace ZamenisHealth.Comunes
{
    public partial class UnificadorPDFMasivo : Forma
    {
        public UnificadorPDFMasivo()
        {
            InitializeComponent();
        }
        private void btnZamenis1_ButtonClick(object sender, EventArgs e)
        {
            MessageBox.Show("Ruta \t Inicial \t Final \t Salida \t \n\r " +
                           "Ruta del Pdf \t Posision Inicial \t Ultimo Archivo \t Nombre archivo.pdf \t \n\r",
               "Instruccion de cabeceras del archivo de Excel",
               MessageBoxButtons.OK,
               MessageBoxIcon.Information);
        }
        private void btnZamenis2_ButtonClick(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    Filter = "Excel | *.xls;*.xlsx;",
                    Title = "Seleccionar Archivo"
                };

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    dataGridView1.DataSource = ImportarDatos(openFileDialog.FileName);
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
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    string Ruta = Convert.ToString(row.Cells["Ruta"].Value);
                    string Inicial = Convert.ToString(row.Cells["Inicial"].Value);
                    string Final = Convert.ToString(row.Cells["Final"].Value);
                    string Salida = Convert.ToString(row.Cells["Salida"].Value);

                    if (Ruta == "") { MessageBox.Show("Hay espacios vacios en la columna Ruta, verifique su archivo"); return; }
                    if (Inicial == "") { MessageBox.Show("Hay espacios vacios en la columna Inicial, verifique su archivo"); return; }
                    if (Final == "") { MessageBox.Show("Hay espacios vacios en la columna Final, verifique su archivo"); return; }
                    if (Salida == "") { MessageBox.Show("Hay espacios vacios en la columna Salida, verifique su archivo"); return; }

                    Unificar_Estructura(Ruta,
                                        Inicial,
                                        Final,
                                        Salida);
                }
                MessageBox.Show(@"Unificado, busque en C:\CXN\Reportes", "Hecho", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void UnificadorPDFMasivo_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Unificador PDF";
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";
            LogoMain.Image = Properties.Resources.Splash;

            ToolStripButton btnInstruccion = new ToolStripButton();
            btnInstruccion = createToolButton("Instruccion");
            MenuLateral.Items.Add(btnInstruccion);
            btnInstruccion.Click += btnZamenis1_ButtonClick;

            ToolStripButton btnsubir = new ToolStripButton();
            btnsubir = createToolButton("Subir");
            MenuLateral.Items.Add(btnsubir);
            btnsubir.Click += btnZamenis2_ButtonClick;

            ToolStripButton btnUnir = new ToolStripButton();
            btnUnir = createToolButton("Unir");
            MenuLateral.Items.Add(btnUnir);
            btnUnir.Click += btnZamenis3_ButtonClick;

            
        }        
        DataView ImportarDatos(string nombrearchivo)
        {
            string conexion = string.Format("Provider = Microsoft.ACE.OLEDB.12.0; Data Source = {0}; Extended Properties = 'Excel 12.0;'", nombrearchivo);

            OleDbConnection conector = new OleDbConnection(conexion);

            conector.Open();

            OleDbCommand consulta = new OleDbCommand("select * from [Hoja1$]", conector);

            OleDbDataAdapter adaptador = new OleDbDataAdapter
            {
                SelectCommand = consulta
            };

            DataSet ds = new DataSet();

            adaptador.Fill(ds);

            conector.Close();

            return ds.Tables[0].DefaultView;
        }
        public void Unificar_Estructura(string ruta,
                                         string inicial,
                                         string final,
                                         string salida)
        {
            try
            {
                int Total = Convert.ToInt32(final) + 1;
                int Inicio = Convert.ToInt32(inicial);
                string ArchivoFinal = salida;
                int Pos = 0;

                string[] lstFiles = new string[Total];

                foreach (int number in EvenSequence(Inicio, Total))
                {
                    lstFiles[Pos] = ruta + number + ".pdf";
                    Pos = Pos + 1;
                }

                PdfReader reader = null;
                Document sourceDocument = null;
                PdfCopy pdfCopyProvider = null;
                PdfImportedPage importedPage;
                string outputPdfPath = @"C:/Cxn/Reportes/" + ArchivoFinal;

                sourceDocument = new Document();
                pdfCopyProvider = new PdfCopy(sourceDocument, new System.IO.FileStream(outputPdfPath, System.IO.FileMode.Create));

                //Open the output file
                sourceDocument.Open();

                try
                {
                    for (int f = 0; f < lstFiles.Length - 1; f++)
                    {
                        int pages = get_pageCcount(lstFiles[f]);

                        reader = new PdfReader(lstFiles[f]);
                        PdfReader.unethicalreading = true;

                        for (int i = 1; i <= pages; i++)
                        {
                            importedPage = pdfCopyProvider.GetImportedPage(reader, i);
                            pdfCopyProvider.AddPage(importedPage);
                        }

                        reader.Close();
                    }

                    sourceDocument.Close();
                }
                catch (Exception ex)

                {
                    Log(ruta, salida, ex.Message);
                    //throw ex;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void Log(string RUTA,
                         string SALIDA,
                         string ERROR)
        {
            try
            {
                DateTime Hoy = DateTime.Now;

                FileStream Query = new FileStream("C:/Cxn/Reportes/Log_" + Convert.ToDateTime(Hoy).ToString("dd-MM-yyyy") + ".txt", FileMode.Append, FileAccess.Write);
                StreamWriter Escriba = new StreamWriter(Query);

                Escriba.Write("RUTA" + "," + "SALIDA" + "," + "ERROR");
                Escriba.WriteLine();
                Escriba.Flush();

                Escriba.Write(RUTA + ",");
                Escriba.Write(SALIDA + ",");
                Escriba.Write(ERROR);
                Escriba.WriteLine();
                Escriba.Flush();
                Escriba.Close();
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        public static System.Collections.Generic.IEnumerable<int>
          EvenSequence(int firstNumber, int lastNumber)
        {
            // Yield even numbers in the range.
            for (int number = firstNumber; number <= lastNumber; number++)
            {
                if (number % 1 == 0)
                {
                    yield return number;
                }
            }
        }
        public int get_pageCcount(string file)
        {
            using (StreamReader sr = new StreamReader(File.OpenRead(file)))
            {
                Regex regex = new Regex(@"/Type\s*/Page[^s]");
                MatchCollection matches = regex.Matches(sr.ReadToEnd());

                return matches.Count;
            }
        }
    }
}
