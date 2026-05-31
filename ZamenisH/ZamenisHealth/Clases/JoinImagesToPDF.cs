using System;
using System.Collections.Generic;
using System.IO;
using Domain;
using Persistence;
using ZamenisHealth.Comunes;
using System.Diagnostics;

namespace ZamenisHealth.Clases
{
    public static class JoinImagesToPDF
    {
        /*public static void CreatePdfFromImages(List<byte[]> imageBytesList, string carpet, string fileName)
        {
            try
            {
                string folderPath = Path.Combine(@"C:\CXN\Reportes\", carpet);
                Directory.CreateDirectory(folderPath);

                string pdfFilePath = Path.Combine(folderPath, fileName + ".pdf");

                using (iText.Kernel.Pdf.PdfWriter writer = new iText.Kernel.Pdf.PdfWriter(pdfFilePath))
                using (iText.Kernel.Pdf.PdfDocument pdf = new iText.Kernel.Pdf.PdfDocument(writer))
                {
                    iText.Layout.Document document = new iText.Layout.Document(pdf);

                    foreach (byte[] imageBytes in imageBytesList)
                    {
                        iText.Layout.Element.Image itextImage;
                        using (MemoryStream ms = new MemoryStream(imageBytes))
                        using (Image<Rgba32> image = SixLabors.ImageSharp.Image.Load<Rgba32>(ms, out IImageFormat format))
                        {
                            iText.IO.Image.ImageData imageData = iText.IO.Image.ImageDataFactory.Create(imageBytes);
                            itextImage = new iText.Layout.Element.Image(imageData);
                            itextImage.SetRotationAngle(-Math.PI / 2);
                            itextImage.SetHorizontalAlignment(iText.Layout.Properties.HorizontalAlignment.CENTER);
                            itextImage.SetWidth(2100);
                            itextImage.SetHeight(2700);
                        }
                        document.Add(itextImage);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }*/

        public static void CreatePdfFromImages(List<byte[]> imageBytesList, string carpet, string fileName)
        {
            try
            {
                string folderPath = Path.Combine(@"C:\CXN\Reportes\", carpet);
                Directory.CreateDirectory(folderPath);

                string pdfFilePath = Path.Combine(folderPath, fileName + ".pdf");

                using (iText.Kernel.Pdf.PdfWriter writer = new iText.Kernel.Pdf.PdfWriter(pdfFilePath))
                using (iText.Kernel.Pdf.PdfDocument pdf = new iText.Kernel.Pdf.PdfDocument(writer))
                {
                    iText.Layout.Document document = new iText.Layout.Document(pdf);

                    foreach (byte[] imageBytes in imageBytesList)
                    {
                        // Crear imagen usando iText directamente
                        iText.IO.Image.ImageData imageData = iText.IO.Image.ImageDataFactory.Create(imageBytes);
                        iText.Layout.Element.Image itextImage = new iText.Layout.Element.Image(imageData);

                        // Rotar la imagen
                        itextImage.SetRotationAngle(-Math.PI / 2);

                        // Configurar alineación y tamaño
                        itextImage.SetHorizontalAlignment(iText.Layout.Properties.HorizontalAlignment.CENTER);
                        itextImage.SetWidth(2100);  // Ajustar el tamaño a tu preferencia
                        itextImage.SetHeight(2700); // Ajustar el tamaño a tu preferencia

                        // Agregar la imagen al documento
                        document.Add(itextImage);
                    }

                    // Cerrar el documento
                    document.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }


        [Obsolete]
        public static void CreatePdfFromBinary(byte[] imageBytesList, string carpet, string fileName)
        {
            string folderPath = @"C:\CXN\Reportes\" + carpet;
            string pdfFilePath = Path.Combine(folderPath, fileName + ".pdf");
            Directory.CreateDirectory(folderPath);
            File.WriteAllBytes(pdfFilePath, imageBytesList);
        }

        public static byte[] ConvertirBinarioAPDF(byte[] data)
        {
            using (MemoryStream stream = new MemoryStream(data))
            {
                return stream.ToArray();
            }
        }

        public static void AbrirPDFDesdeBytes(byte[] pdfBytes)
        {
            try
            {
                string tempFilePath = GuardarPDFComoTemporal(pdfBytes);
                AbrirPDFConVisorPredeterminado(tempFilePath);
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = "Metodo estatico: AbrirPDFDesdeBytes", Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        static string GuardarPDFComoTemporal(byte[] pdfBytes)
        {
            string tempFilePath = Path.GetTempFileName();
            File.WriteAllBytes(tempFilePath, pdfBytes);
            return tempFilePath;
        }
        static void AbrirPDFConVisorPredeterminado(string filePath)
        {
            Process.Start(filePath);
        }
    }
}
