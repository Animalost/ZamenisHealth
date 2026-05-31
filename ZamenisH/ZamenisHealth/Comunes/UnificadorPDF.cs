using Domain;
using FormAndControls;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Persistence;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using ZamenisHealth.Clases;

namespace ZamenisHealth.Comunes
{
    public partial class UnificadorPDF : Forma
    {
        public UnificadorPDF()
        {
            InitializeComponent();
        }
        private void btnZamenis1_ButtonClick(object sender, EventArgs e)
        {
            this.Dispose();
            this.Close();

            UnificadorPDFMasivo f = new UnificadorPDFMasivo();
            f.ShowDialog();
        }
        private void btnZamenis4_ButtonClick(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        textBox1.Text = openFileDialog.FileName;
                    }
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void btnZamenis5_ButtonClick(object sender, EventArgs e)
        {
            try
            {
                int Total = Convert.ToInt32(textBox4.Text) + 1;
                int Inicio = Convert.ToInt32(textBox2.Text);
                string ArchivoFinal = textBox3.Text + ".pdf";
                int Pos = 0;

                string[] lstFiles = new string[Total];

                foreach (int number in EvenSequence(Inicio, Total))
                {
                    lstFiles[Pos] = textBox1.Text + number + ".pdf";
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
                    MessageBox.Show("Unificado, busque en C: CXN Reportes",
                        "Hecho",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
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
        private void UnificadorPDF_Load(object sender, EventArgs e)
        {
            try
            {
                Titulo.Text = "Unificador PDF";
                SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";
                LogoMain.Image = Properties.Resources.Splash;

                ToolStripButton btnMasivo = new ToolStripButton();
                btnMasivo = createToolButton("Masivo");
                MenuLateral.Items.Add(btnMasivo);
                btnMasivo.Click += btnZamenis1_ButtonClick;

                ToolStripButton btnSeleccionar = new ToolStripButton();
                btnSeleccionar = createToolButton("Seleccionar");
                MenuLateral.Items.Add(btnSeleccionar);
                btnSeleccionar.Click += btnZamenis4_ButtonClick;

                ToolStripButton btnUnificar = new ToolStripButton();
                btnUnificar = createToolButton("Unificar");
                MenuLateral.Items.Add(btnUnificar);
                btnUnificar.Click += btnZamenis5_ButtonClick;

                ConfigForm.SoloNumeros(textBox2);
                ConfigForm.SoloNumeros(textBox4);

                
                MessageBox.Show("Esta opcion esta diseñada con itextsharp en la version 5.5.13.0 la " +
                    "cual cuenta con una licencia libre tipo Affero GNU Public License",
                    "Aviso Importante de Licenciamiento",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
    }
}
