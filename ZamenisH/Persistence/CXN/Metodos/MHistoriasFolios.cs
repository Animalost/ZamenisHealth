using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.draw;
using Persistence.CXN.Interfaces;

namespace Persistence.CXN.Metodos
{
    public class MHistoriasFolios : IHistoriasFolios
    {
        private static readonly IGenerales repoGen = new MGenerales();
        private static readonly ICIE10 repoCIE10 = new MCIE10();

        public int Pac_Id { get; set; }
        public DateTime Desde { get; set; }
        public DateTime Hasta { get; set; }
        int Contador;
        string Name_Paciente;
        public void Cons()
        {
            try
            {
                Contador = Contador + 1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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
        public void Unificar(string Name_Pac)
        {
            try
            { //el total es la variable inicio
                string Ruta = "C:\\CXN\\Reportes\\";
                int Comienzo = 1;
                string ArchivoFinal = Name_Pac + ".pdf";
                int Pos = 0;
                Contador = Contador + 1; // importante sumar uno  para que agarre el ultimo pdf
                string[] lstFiles = new string[Contador];

                foreach (int number in EvenSequence(Comienzo, Contador))
                {
                    lstFiles[Pos] = Ruta + number + ".pdf";
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
                    MessageBox.Show("Unificado, busque en C: CXN Reportes", "Hecho", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        void IHistoriasFolios.Genera_Export_HCMG(int Pac_Id, DateTime Desde, DateTime Hasta)
        {
            try
            {
                Dictionary<string,string> getData = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getData["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    String Cargar_Hora = "SELECT Com_Nombre,  Com_Identificacion, Com_Direccion, Com_Telefono, Bod_Responsable, Bod_Reg_Med, HC_Pac, Pac_TipoId, Pac_IdNum, Pac_Direccion, Pac_FechaNto, Pac_Sexo, HC_Edad, Pac_Telefono, HC_SubPat, HC_Patologia, " +
                                         " Ase_Descripcion, Pac_Email, HC_Ocupacion, Pac_Acudiente, Pac_Parentesco, Pac_DireccionAcu, Pac_TelefonoAcu, Pac_CorreoAcu, HC_Vez, HC_Fecha, HC_MotivoC, HC_EnfA, HC_Neurologico, HC_Cardiovascular, " +
                                         " HC_Gastrourinario, HC_Piel, HC_Respiratorio, HC_Gastrointestinal, HC_Osteomuscular, HC_AntQui, HC_AntFam, HC_AntPat, HC_AntFarma, HC_AntAle, HC_Hematolin, HC_Nota_Acl, HC_Presart, HC_FreCar, " +
                                         " HC_FreRes, HC_Temp, HC_Peso, HC_IMC, HC_Altura, HC_ITB, HC_AparienciaG, HC_EstadoEmo, HC_EstadoNut, HC_GradoC, HC_ActEje, HC_TipoLes, HC_DescHer, HC_CantHer, HC_Bolsillo, HC_Longitud, HC_Ancho, " +
                                         " HC_Profundidad, HC_NumCav, HC_ConsCant,HC_TejCom,  HC_CaracTej, HC_Exudado, HC_SignosInf, HC_PielCirc, HC_TamañoHP, HC_Analisis, HC_PManejo, HC_ProtoInst, HC_Dx1T, HC_Dx2, HC_Dx3, HC_PruebasDiag, HC_Complicacion, " +
                                         " HC_Dx1, Bod_Firma, Bod_Responsable, HC_Dolor, HC_Estado, ' ' AS Vacio, HC_PacId, Pac_Id, HC_Ase, Ase_Identificador, HC_Com, Com_Identificador, HC_Prof, Bod_Numero, HC_Adm, " +
                                         " 'Motivo de Consulta' as MCONS, 'Enfermedad Actual' as ENFAC, 'Revision a Sistemas' as RSIST, 'Neurologico' as NEURO, 'Cardiovascular' as CARDIO, 'Gastrourinario' as GASTROU, 'OsteoMuscular' as OSTEO, " +
                                         " 'GastroIntestinal' as GASTROI, 'Piel' as PIEL, 'Respiratorio' as RESPI, 'ANTECEDENTES' as ANT, 'Quirurgicos' as QUI, 'Familiares' as FAMI, 'Patologicos' as PATO, 'Farmacologicos' as FARMA, " +
                                         " 'Alergicos' AS ALER, 'Hematologico y Linfatico' HEMA, 'Examen Fisico' AS EXAF, 'Presion Arterial' AS PRES, 'Frecuencia Cardiaca' AS FRECC, 'Frec. Respiratoria' AS FRECR, 'Temperatura' AS TEMP,  " +
                                         " 'Peso' AS PESO, 'IMC' AS IMC, 'Talla' AS Talla, 'Indice Tobillo Brazo' AS ITB, 'Apariencia General' AS APAG, 'Estado Emocional' AS ESTE, 'Estado Nutricional' AS ESTN, 'Grado de Cuidado' AS GCUI, " +
                                         " 'Actividad y Ejercicio' AS ACEJ, 'Tipo de Lesion' AS TLES, 'Descripcion de la Herida' AS DESH, 'Consistencia y Cantidad de Exudado' AS CONC, 'Tejidos Comprometidos' AS TCOM, " +
                                         " 'Caracteristicas del Tejido' AS CTEJ, 'Exudado' AS EXUD, 'Signos de Infeccion' AS SINF, 'Piel Circundante' AS PCIR, 'Tamaño' AS TAMA, " +
                                         " 'PLAN DE MANEJO' AS PMAN, 'Analisis' AS ANAL, 'Plan de Manejo' AS PMAN2, 'Diagnostico' AS DIAG, 'Pruebas Diagnosticas Complementarias' AS PDC, 'Complicaciones Durante el Tratamiento en esta Institucion' AS CTRA, 'Notas Aclaratorias' AS NOTAA, 'Firma y Sello' AS FIRMA, HC_Epidemia " +
                                         " FROM CXN_HCMG " +
                                         " INNER JOIN CXN_PACIENTES ON CXN_HCMG.HC_PacId = CXN_PACIENTES.Pac_Id " +
                                         " INNER JOIN CXN_ASEGURADORA ON CXN_HCMG.HC_Ase = CXN_ASEGURADORA.Ase_Identificador " +
                                         " INNER JOIN CXN_CIA ON CXN_HCMG.HC_Com = CXN_CIA.Com_Identificador " +
                                         " INNER JOIN CXN_BODEGAS ON CXN_HCMG.HC_Prof = CXN_BODEGAS.Bod_Numero " +
                                         " WHERE CXN_HCMG.HC_Pacid = '" + Pac_Id + "' " +
                                         " AND CXN_HCMG.HC_Fecha BETWEEN '" + Convert.ToDateTime(Desde).ToString(getData["Format_Fecha"]) + "' AND '" + Convert.ToDateTime(Hasta).ToString(getData["Format_Fecha"]) + "'";
                    SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con);
                    SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader());
                    if (Lectura_Hora.HasRows)
                    {
                        while (Lectura_Hora.Read() == true)
                        {
                            var Diag1 = repoCIE10.BuscaDX(Lectura_Hora["HC_Dx1"].ToString());
                            var Diag2 = repoCIE10.BuscaDX(Lectura_Hora["HC_Dx2"].ToString());
                            var Diag3 = repoCIE10.BuscaDX(Lectura_Hora["HC_Dx3"].ToString());

                            Cons();
                            /////////////////////////////////
                            Document doc = new Document();
                            PdfWriter.GetInstance(doc, new FileStream("C:\\CXN\\Reportes\\" + Contador + ".pdf", FileMode.Create));
                            doc.Open();

                            Paragraph title = new Paragraph();
                            Paragraph title1 = new Paragraph();
                            Paragraph title2 = new Paragraph();
                            Paragraph title3 = new Paragraph();
                            Paragraph title4 = new Paragraph();
                            Paragraph title5 = new Paragraph();
                            Paragraph title6 = new Paragraph();

                            /////////////////////////////////ENCABEZADO PRIMERA PAGINA
                            title.Font = FontFactory.GetFont(FontFactory.TIMES, 8f, BaseColor.BLACK);
                            title.Add("Historia Clinica");
                            title.Alignment = Element.ALIGN_CENTER;

                            doc.Add(title);
                            doc.Add(new Paragraph("  "));

                            //---------------------TABLA ENCABEZADO                        
                            PdfPTable table1 = new PdfPTable(4);
                            table1.HorizontalAlignment = Element.ALIGN_LEFT;
                            table1.SetTotalWidth(new float[] { 65, 220, 65, 120 });
                            table1.LockedWidth = (true);

                            //Primera fila
                            PdfPCell cella = new PdfPCell(new Phrase("Institucion: ", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.BOLD)));
                            cella.Colspan = 1;
                            cella.Border = 0;
                            table1.AddCell(cella);

                            PdfPCell cell2a = new PdfPCell(new Phrase(Lectura_Hora["Com_Nombre"].ToString(), FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            cell2a.Colspan = 1;
                            cell2a.Border = 0;
                            table1.AddCell(cell2a);

                            PdfPCell C77a = new PdfPCell(new Phrase(" ", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            C77a.Border = 0;
                            table1.AddCell(C77a);

                            PdfPCell C77a3 = new PdfPCell(new Phrase(" ", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            C77a3.Border = 0;
                            table1.AddCell(C77a3);

                            //segunda fila

                            PdfPCell cell3a = new PdfPCell(new Phrase("Nit: ", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.BOLD)));
                            cell3a.Colspan = 1;
                            cell3a.Border = 0;
                            table1.AddCell(cell3a);

                            PdfPCell cell2aa = new PdfPCell(new Phrase(Lectura_Hora["Com_Identificacion"].ToString(), FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            cell2aa.Colspan = 1;
                            cell2aa.Border = 0;
                            table1.AddCell(cell2aa);

                            PdfPCell cell2aat = new PdfPCell(new Phrase("Telefono: ", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.BOLD)));
                            cell2aat.Colspan = 1;
                            cell2aat.Border = 0;
                            table1.AddCell(cell2aat);

                            PdfPCell cell2aatt = new PdfPCell(new Phrase(Lectura_Hora["Com_Telefono"].ToString(), FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            cell2aatt.Colspan = 1;
                            cell2aatt.Border = 0;
                            table1.AddCell(cell2aatt);

                            //tercera fila

                            PdfPCell cella3 = new PdfPCell(new Phrase("Direccion: ", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.BOLD)));
                            cella3.Colspan = 1;
                            cella3.Border = 0;
                            table1.AddCell(cella3);

                            PdfPCell cell2a3 = new PdfPCell(new Phrase(Lectura_Hora["Com_Direccion"].ToString(), FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            cell2a3.Colspan = 1;
                            cell2a3.Border = 0;
                            table1.AddCell(cell2a3);

                            PdfPCell C77a33 = new PdfPCell(new Phrase(" ", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            C77a33.Border = 0;
                            table1.AddCell(C77a33);

                            PdfPCell C77a334 = new PdfPCell(new Phrase(" ", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            C77a334.Border = 0;
                            table1.AddCell(C77a334);

                            doc.Add(table1);

                            //---------------------FIN TABLA ENCABEZADO

                            doc.Add(new Paragraph(" ", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));

                            //---------------------SEGUNDA TABLA ENCABZADO
                            PdfPTable table2 = new PdfPTable(4);
                            table2.HorizontalAlignment = Element.ALIGN_LEFT;
                            table2.SetTotalWidth(new float[] { 65, 220, 65, 120 });
                            table2.LockedWidth = (true);

                            //Primera fila
                            PdfPCell cell1_A = new PdfPCell(new Phrase("Doctor: ", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.BOLD)));
                            cell1_A.Colspan = 1;
                            cell1_A.Border = 0;
                            table2.AddCell(cell1_A);

                            PdfPCell cell2_A = new PdfPCell(new Phrase(Lectura_Hora["Bod_Responsable"].ToString(), FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            cell2_A.Colspan = 1;
                            cell2_A.Border = 0;
                            table2.AddCell(cell2_A);

                            PdfPCell cell3_A = new PdfPCell(new Phrase(" ", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            cell3_A.Border = 0;
                            table2.AddCell(cell3_A);

                            PdfPCell cell4_A = new PdfPCell(new Phrase(" ", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            cell4_A.Border = 0;
                            table2.AddCell(cell4_A);

                            //segunda fila
                            PdfPCell cell1_A2 = new PdfPCell(new Phrase("Registro Medico: ", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.BOLD)));
                            cell1_A2.Colspan = 1;
                            cell1_A2.Border = 0;
                            table2.AddCell(cell1_A2);

                            PdfPCell cell2_A2 = new PdfPCell(new Phrase(Lectura_Hora["Bod_Reg_Med"].ToString(), FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            cell2_A2.Colspan = 1;
                            cell2_A2.Border = 0;
                            table2.AddCell(cell2_A2);

                            PdfPCell cell3_A2 = new PdfPCell(new Phrase("Admision: ", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.BOLD)));
                            cell3_A2.Colspan = 1;
                            cell3_A2.Border = 0;
                            table2.AddCell(cell3_A2);

                            PdfPCell cell4_A2 = new PdfPCell(new Phrase(Lectura_Hora["HC_Adm"].ToString(), FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            cell4_A2.Colspan = 1;
                            cell4_A2.Border = 0;
                            table2.AddCell(cell4_A2);

                            doc.Add(table2);

                            //---------------------FIN SEGUNDA TABLA ENCABEZADO

                            doc.Add(new Paragraph(" ", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));

                            //---------------------TERCERA TABLA ENCABEZADO

                            PdfPTable table3 = new PdfPTable(4);
                            table3.HorizontalAlignment = Element.ALIGN_LEFT;
                            table3.SetTotalWidth(new float[] { 100, 220, 65, 120 });
                            table3.LockedWidth = (true);

                            //Primera fila
                            PdfPCell cell1_A4 = new PdfPCell(new Phrase("Datos del Paciente", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.BOLD)));
                            cell1_A4.Colspan = 1;
                            cell1_A4.Border = 0;
                            table3.AddCell(cell1_A4);

                            PdfPCell cell3_A4 = new PdfPCell(new Phrase(" ", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            cell3_A4.Border = 0;
                            table3.AddCell(cell3_A4);

                            PdfPCell cell4_A4 = new PdfPCell(new Phrase(" ", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            cell4_A4.Border = 0;
                            table3.AddCell(cell4_A4);

                            PdfPCell cell5_A4 = new PdfPCell(new Phrase(" ", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            cell5_A4.Border = 0;
                            table3.AddCell(cell5_A4);

                            //segunda linea

                            PdfPCell cell1_A55 = new PdfPCell(new Phrase("Nombre ", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.BOLD)));
                            cell1_A55.Colspan = 1;
                            cell1_A55.Border = 0;
                            table3.AddCell(cell1_A55);

                            PdfPCell cell1_A5 = new PdfPCell(new Phrase(Lectura_Hora["HC_Pac"].ToString(), FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            cell1_A5.Colspan = 1;
                            cell1_A5.Border = 0;
                            table3.AddCell(cell1_A5);

                            PdfPCell cell3_A55 = new PdfPCell(new Phrase("Edad ", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.BOLD)));
                            cell3_A55.Colspan = 1;
                            cell3_A55.Border = 0;
                            table3.AddCell(cell3_A55);

                            PdfPCell cell4_A5 = new PdfPCell(new Phrase(Lectura_Hora["HC_Edad"].ToString(), FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            cell4_A5.Colspan = 1;
                            cell4_A5.Border = 0;
                            table3.AddCell(cell4_A5);

                            //tercera linea

                            PdfPCell cell1_A53 = new PdfPCell(new Phrase("Identificacion ", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.BOLD)));
                            cell1_A53.Colspan = 1;
                            cell1_A53.Border = 0;
                            table3.AddCell(cell1_A53);

                            PdfPCell cell1_A54 = new PdfPCell(new Phrase(Lectura_Hora["Pac_TipoId"].ToString() + " " + Lectura_Hora["Pac_IdNum"].ToString(), FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            cell1_A54.Colspan = 1;
                            cell1_A54.Border = 0;
                            table3.AddCell(cell1_A54);

                            PdfPCell cell1_A56 = new PdfPCell(new Phrase("Telefono ", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.BOLD)));
                            cell1_A56.Colspan = 1;
                            cell1_A56.Border = 0;
                            table3.AddCell(cell1_A56);

                            PdfPCell cell1_A57 = new PdfPCell(new Phrase(Lectura_Hora["Pac_Telefono"].ToString(), FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            cell1_A57.Colspan = 1;
                            cell1_A57.Border = 0;
                            table3.AddCell(cell1_A57);

                            //cuarta linea

                            PdfPCell cell4_A53 = new PdfPCell(new Phrase("Direccion ", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.BOLD)));
                            cell4_A53.Colspan = 1;
                            cell4_A53.Border = 0;
                            table3.AddCell(cell4_A53);

                            PdfPCell cell5_A53 = new PdfPCell(new Phrase(Lectura_Hora["Pac_Direccion"].ToString(), FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            cell5_A53.Colspan = 1;
                            cell5_A53.Border = 0;
                            table3.AddCell(cell5_A53);

                            PdfPCell cell6_A53 = new PdfPCell(new Phrase("Aseguradora ", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.BOLD)));
                            cell6_A53.Colspan = 1;
                            cell6_A53.Border = 0;
                            table3.AddCell(cell6_A53);

                            PdfPCell cell7_A53 = new PdfPCell(new Phrase(Lectura_Hora["Ase_Descripcion"].ToString(), FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            cell7_A53.Colspan = 1;
                            cell7_A53.Border = 0;
                            table3.AddCell(cell7_A53);

                            //quinta linea

                            PdfPCell cell4_A533 = new PdfPCell(new Phrase("Fecha Nto", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.BOLD)));
                            cell4_A533.Colspan = 1;
                            cell4_A533.Border = 0;
                            table3.AddCell(cell4_A533);

                            PdfPCell cell4_A534 = new PdfPCell(new Phrase(Convert.ToDateTime(Lectura_Hora["Pac_FechaNto"]).ToString(getData["Format_Fecha"]), FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            cell4_A534.Colspan = 1;
                            cell4_A534.Border = 0;
                            table3.AddCell(cell4_A534);

                            PdfPCell cell4_A535 = new PdfPCell(new Phrase("Correo ", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.BOLD)));
                            cell4_A535.Colspan = 1;
                            cell4_A535.Border = 0;
                            table3.AddCell(cell4_A535);

                            PdfPCell cell4_A536 = new PdfPCell(new Phrase(Lectura_Hora["Pac_Email"].ToString(), FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            cell4_A536.Colspan = 1;
                            cell4_A536.Border = 0;
                            table3.AddCell(cell4_A536);

                            //sexta linea

                            PdfPCell cell4_A666 = new PdfPCell(new Phrase("Genero ", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.BOLD)));
                            cell4_A666.Colspan = 1;
                            cell4_A666.Border = 0;
                            table3.AddCell(cell4_A666);

                            PdfPCell cell4_A6666 = new PdfPCell(new Phrase(Lectura_Hora["Pac_Sexo"].ToString(), FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            cell4_A6666.Colspan = 1;
                            cell4_A6666.Border = 0;
                            table3.AddCell(cell4_A6666);

                            PdfPCell cell4_A6667 = new PdfPCell(new Phrase("Ocupacion ", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.BOLD)));
                            cell4_A6667.Colspan = 1;
                            cell4_A6667.Border = 0;
                            table3.AddCell(cell4_A6667);

                            PdfPCell cell4_A6668 = new PdfPCell(new Phrase(Lectura_Hora["HC_Ocupacion"].ToString(), FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            cell4_A6668.Colspan = 1;
                            cell4_A6668.Border = 0;
                            table3.AddCell(cell4_A6668);

                            doc.Add(table3);

                            //--------------------FIN TERCERA TABLA ENCABEZADO

                            doc.Add(new Paragraph(" ", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));

                            //--------------------CUARTA TABLA ENCABEZADO

                            PdfPTable table4 = new PdfPTable(4);
                            table4.HorizontalAlignment = Element.ALIGN_LEFT;
                            table4.SetTotalWidth(new float[] { 100, 220, 65, 120 });
                            table4.LockedWidth = (true);

                            //Primera fila
                            PdfPCell C1_L1_Tabla4 = new PdfPCell(new Phrase("Datos del Acudiente", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.BOLD)));
                            C1_L1_Tabla4.Colspan = 1;
                            C1_L1_Tabla4.Border = 0;
                            table4.AddCell(C1_L1_Tabla4);

                            PdfPCell C2_L1_Tabla4 = new PdfPCell(new Phrase(" ", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            C2_L1_Tabla4.Border = 0;
                            table4.AddCell(C2_L1_Tabla4);

                            PdfPCell C3_L1_Tabla4 = new PdfPCell(new Phrase(" ", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            C3_L1_Tabla4.Border = 0;
                            table4.AddCell(C3_L1_Tabla4);

                            PdfPCell C4_L1_Tabla4 = new PdfPCell(new Phrase(" ", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            C4_L1_Tabla4.Border = 0;
                            table4.AddCell(C4_L1_Tabla4);

                            //segunda linea

                            PdfPCell C1_L2_Tabla4 = new PdfPCell(new Phrase("Nombre ", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.BOLD)));
                            C1_L2_Tabla4.Colspan = 1;
                            C1_L2_Tabla4.Border = 0;
                            table4.AddCell(C1_L2_Tabla4);

                            PdfPCell C2_L2_Tabla4 = new PdfPCell(new Phrase(Lectura_Hora["Pac_Acudiente"].ToString(), FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            C2_L2_Tabla4.Colspan = 1;
                            C2_L2_Tabla4.Border = 0;
                            table4.AddCell(C2_L2_Tabla4);

                            PdfPCell C3_L2_Tabla4 = new PdfPCell(new Phrase("Telefono ", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.BOLD)));
                            C3_L2_Tabla4.Colspan = 1;
                            C3_L2_Tabla4.Border = 0;
                            table4.AddCell(C3_L2_Tabla4);

                            PdfPCell C4_L2_Tabla4 = new PdfPCell(new Phrase(Lectura_Hora["Pac_TelefonoAcu"].ToString(), FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            C4_L2_Tabla4.Colspan = 1;
                            C4_L2_Tabla4.Border = 0;
                            table4.AddCell(C4_L2_Tabla4);

                            //tercera linea

                            PdfPCell C1_L3_Tabla4 = new PdfPCell(new Phrase("Parentesco ", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.BOLD)));
                            C1_L3_Tabla4.Colspan = 1;
                            C1_L3_Tabla4.Border = 0;
                            table4.AddCell(C1_L3_Tabla4);

                            PdfPCell C2_L3_Tabla4 = new PdfPCell(new Phrase(Lectura_Hora["Pac_Parentesco"].ToString(), FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            C2_L3_Tabla4.Colspan = 1;
                            C2_L3_Tabla4.Border = 0;
                            table4.AddCell(C2_L3_Tabla4);

                            PdfPCell C3_L3_Tabla4 = new PdfPCell(new Phrase("Correo ", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.BOLD)));
                            C3_L3_Tabla4.Colspan = 1;
                            C3_L3_Tabla4.Border = 0;
                            table4.AddCell(C3_L3_Tabla4);

                            PdfPCell C4_L3_Tabla4 = new PdfPCell(new Phrase(Lectura_Hora["Pac_CorreoAcu"].ToString(), FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            C4_L3_Tabla4.Colspan = 1;
                            C4_L3_Tabla4.Border = 0;
                            table4.AddCell(C4_L3_Tabla4);

                            //cuarta linea

                            PdfPCell C1_L4_Tabla4 = new PdfPCell(new Phrase("Direccion ", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.BOLD)));
                            C1_L4_Tabla4.Colspan = 1;
                            C1_L4_Tabla4.Border = 0;
                            table4.AddCell(C1_L4_Tabla4);

                            PdfPCell C2_L4_Tabla4 = new PdfPCell(new Phrase(Lectura_Hora["Pac_DireccionAcu"].ToString(), FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            C2_L4_Tabla4.Colspan = 1;
                            C2_L4_Tabla4.Border = 0;
                            table4.AddCell(C2_L4_Tabla4);

                            PdfPCell C3_L4_Tabla4 = new PdfPCell(new Phrase(" ", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.BOLD)));
                            C3_L4_Tabla4.Colspan = 1;
                            C3_L4_Tabla4.Border = 0;
                            table4.AddCell(C3_L4_Tabla4);

                            PdfPCell C4_L4_Tabla4 = new PdfPCell(new Phrase(" ", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            C4_L4_Tabla4.Colspan = 1;
                            C4_L4_Tabla4.Border = 0;
                            table4.AddCell(C4_L4_Tabla4);

                            //quinta linea

                            PdfPCell C1_L5_Tabla4 = new PdfPCell(new Phrase("Tipo Consulta ", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.BOLD)));
                            C1_L5_Tabla4.Colspan = 1;
                            C1_L5_Tabla4.Border = 0;
                            table4.AddCell(C1_L5_Tabla4);

                            PdfPCell C2_L5_Tabla4 = new PdfPCell(new Phrase(Lectura_Hora["HC_Vez"].ToString(), FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            C2_L5_Tabla4.Colspan = 1;
                            C2_L5_Tabla4.Border = 0;
                            table4.AddCell(C2_L5_Tabla4);

                            PdfPCell C3_L5_Tabla4 = new PdfPCell(new Phrase("Fecha ", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.BOLD)));
                            C3_L5_Tabla4.Colspan = 1;
                            C3_L5_Tabla4.Border = 0;
                            table4.AddCell(C3_L5_Tabla4);

                            PdfPCell C4_L5_Tabla4 = new PdfPCell(new Phrase(Convert.ToDateTime(Lectura_Hora["HC_Fecha"]).ToString(getData["Format_Fecha"]), FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            C4_L5_Tabla4.Colspan = 1;
                            C4_L5_Tabla4.Border = 0;
                            table4.AddCell(C4_L5_Tabla4);

                            doc.Add(table4);

                            //--------------------FIN CUARTA TABLA ENCABEZADO

                            doc.Add(new Chunk(Environment.NewLine));

                            LineSeparator line = new LineSeparator(1f, 100f, BaseColor.BLACK, Element.ALIGN_LEFT, 1);
                            doc.Add(line);
                            /// FIN  ENCABEZADO PRIMERA PAGINA

                            title1.Font = FontFactory.GetFont(FontFactory.TIMES_BOLD, 8f, BaseColor.BLACK);
                            title1.Add("Motivo de Consulta");
                            title1.Alignment = Element.ALIGN_LEFT;
                            doc.Add(title1); doc.Add(new Paragraph(" ", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            doc.Add(new Paragraph(Lectura_Hora["HC_MotivoC"].ToString(), FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            doc.Add(new Chunk(Environment.NewLine));

                            title2.Font = FontFactory.GetFont(FontFactory.TIMES_BOLD, 8f, BaseColor.BLACK);
                            title2.Add("Enfermedad Actual");
                            title2.Alignment = Element.ALIGN_LEFT;
                            doc.Add(title2); doc.Add(new Paragraph(" ", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            doc.Add(new Paragraph(Lectura_Hora["HC_EnfA"].ToString(), FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            doc.Add(new Chunk(Environment.NewLine));

                            doc.Add(line);


                            //REVISION A SISTEMAS
                            title3.Font = FontFactory.GetFont(FontFactory.TIMES_BOLD, 8f, BaseColor.BLACK);
                            title3.Add("   REVISION A SISTEMAS");
                            title3.Alignment = Element.ALIGN_LEFT;
                            doc.Add(title3); doc.Add(new Paragraph(" ", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));

                            title4.Font = FontFactory.GetFont(FontFactory.TIMES_BOLD, 8f, BaseColor.BLACK);
                            title4.Add("Neurologico");
                            title4.Alignment = Element.ALIGN_LEFT;
                            doc.Add(title4); doc.Add(new Paragraph(" ", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            doc.Add(new Paragraph(Lectura_Hora["HC_Neurologico"].ToString(), FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            doc.Add(new Chunk(Environment.NewLine));

                            title5.Font = FontFactory.GetFont(FontFactory.TIMES_BOLD, 8f, BaseColor.BLACK);
                            title5.Add("CardioVascular");
                            title5.Alignment = Element.ALIGN_LEFT;
                            doc.Add(title5); doc.Add(new Paragraph(" ", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            doc.Add(new Paragraph(Lectura_Hora["HC_Cardiovascular"].ToString(), FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            doc.Add(new Chunk(Environment.NewLine));

                            title6.Font = FontFactory.GetFont(FontFactory.TIMES_BOLD, 8f, BaseColor.BLACK);
                            title6.Add("OsteoMuscular");
                            title6.Alignment = Element.ALIGN_LEFT;
                            doc.Add(title6); doc.Add(new Paragraph(" ", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            doc.Add(new Paragraph(Lectura_Hora["HC_OsteoMuscular"].ToString(), FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            doc.Add(new Chunk(Environment.NewLine));

                            Paragraph title7 = new Paragraph();
                            title7.Font = FontFactory.GetFont(FontFactory.TIMES_BOLD, 8f, BaseColor.BLACK);
                            title7.Add("GastroIntestinal");
                            title7.Alignment = Element.ALIGN_LEFT;
                            doc.Add(title7); doc.Add(new Paragraph(" ", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            doc.Add(new Paragraph(Lectura_Hora["HC_GastroIntestinal"].ToString(), FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            doc.Add(new Chunk(Environment.NewLine));

                            Paragraph title8 = new Paragraph();
                            title8.Font = FontFactory.GetFont(FontFactory.TIMES_BOLD, 8f, BaseColor.BLACK);
                            title8.Add("Piel");
                            title8.Alignment = Element.ALIGN_LEFT;
                            doc.Add(title8); doc.Add(new Paragraph(" ", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            doc.Add(new Paragraph(Lectura_Hora["HC_Piel"].ToString(), FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            doc.Add(new Chunk(Environment.NewLine));

                            Paragraph title9 = new Paragraph();
                            title9.Font = FontFactory.GetFont(FontFactory.TIMES_BOLD, 8f, BaseColor.BLACK);
                            title9.Add("Respiratorio");
                            title9.Alignment = Element.ALIGN_LEFT;
                            doc.Add(title9); doc.Add(new Paragraph(" ", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            doc.Add(new Paragraph(Lectura_Hora["HC_Respiratorio"].ToString(), FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            doc.Add(new Chunk(Environment.NewLine));

                            doc.Add(line);


                            //ANTECEDENTES
                            Paragraph title10 = new Paragraph();
                            title10.Font = FontFactory.GetFont(FontFactory.TIMES_BOLD, 8f, BaseColor.BLACK);
                            title10.Add("   ANTECEDENTES");
                            title10.Alignment = Element.ALIGN_LEFT;
                            doc.Add(title10); doc.Add(new Paragraph(" ", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));

                            Paragraph title11 = new Paragraph();
                            title11.Font = FontFactory.GetFont(FontFactory.TIMES_BOLD, 8f, BaseColor.BLACK);
                            title11.Add("Quirurgicos");
                            title11.Alignment = Element.ALIGN_LEFT;
                            doc.Add(title11); doc.Add(new Paragraph(" ", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            doc.Add(new Paragraph(Lectura_Hora["HC_AntQui"].ToString(), FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            doc.Add(new Chunk(Environment.NewLine));

                            Paragraph title12 = new Paragraph();
                            title12.Font = FontFactory.GetFont(FontFactory.TIMES_BOLD, 8f, BaseColor.BLACK);
                            title12.Add("Familiares");
                            title12.Alignment = Element.ALIGN_LEFT;
                            doc.Add(title12); doc.Add(new Paragraph(" ", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            doc.Add(new Paragraph(Lectura_Hora["HC_AntFam"].ToString(), FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            doc.Add(new Chunk(Environment.NewLine));

                            Paragraph title13 = new Paragraph();
                            title13.Font = FontFactory.GetFont(FontFactory.TIMES_BOLD, 8f, BaseColor.BLACK);
                            title13.Add("Patologicos");
                            title13.Alignment = Element.ALIGN_LEFT;
                            doc.Add(title13); doc.Add(new Paragraph(" ", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            doc.Add(new Paragraph(Lectura_Hora["HC_AntPat"].ToString(), FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            doc.Add(new Chunk(Environment.NewLine));

                            Paragraph title14 = new Paragraph();
                            title14.Font = FontFactory.GetFont(FontFactory.TIMES_BOLD, 8f, BaseColor.BLACK);
                            title14.Add("Farmacologicos");
                            title14.Alignment = Element.ALIGN_LEFT;
                            doc.Add(title14); doc.Add(new Paragraph(" ", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            doc.Add(new Paragraph(Lectura_Hora["HC_AntFarma"].ToString(), FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            doc.Add(new Chunk(Environment.NewLine));

                            Paragraph title15 = new Paragraph();
                            title15.Font = FontFactory.GetFont(FontFactory.TIMES_BOLD, 8f, BaseColor.BLACK);
                            title15.Add("Alergicos");
                            title15.Alignment = Element.ALIGN_LEFT;
                            doc.Add(title15); doc.Add(new Paragraph(" ", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            doc.Add(new Paragraph(Lectura_Hora["HC_AntAle"].ToString(), FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            doc.Add(new Chunk(Environment.NewLine));

                            Paragraph title16 = new Paragraph();
                            title16.Font = FontFactory.GetFont(FontFactory.TIMES_BOLD, 8f, BaseColor.BLACK);
                            title16.Add("Hematologico y Linfatico");
                            title16.Alignment = Element.ALIGN_LEFT;
                            doc.Add(title16); doc.Add(new Paragraph(" ", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            doc.Add(new Paragraph(Lectura_Hora["HC_Hematolin"].ToString(), FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            doc.Add(new Chunk(Environment.NewLine));

                            doc.Add(line);

                            //EXAMEN FISICO

                            Paragraph title17 = new Paragraph();
                            title17.Font = FontFactory.GetFont(FontFactory.TIMES_BOLD, 8f, BaseColor.BLACK);
                            title17.Add("EXAMEN FISICO");
                            title17.Alignment = Element.ALIGN_LEFT;
                            doc.Add(title17); doc.Add(new Paragraph(" ", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));

                            //QUINTA TABLA

                            PdfPTable table5 = new PdfPTable(4);
                            table5.HorizontalAlignment = Element.ALIGN_LEFT;
                            table5.SetTotalWidth(new float[] { 100, 170, 100, 72 });
                            table5.LockedWidth = (true);

                            //Primera fila
                            PdfPCell C1_L1_Tabla5 = new PdfPCell(new Phrase("Presion Arterial", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.BOLD)));
                            C1_L1_Tabla5.Colspan = 1;
                            C1_L1_Tabla5.Border = 0;
                            table5.AddCell(C1_L1_Tabla5);

                            PdfPCell C2_L1_Tabla5 = new PdfPCell(new Phrase(Lectura_Hora["HC_Presart"].ToString(), FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            C2_L1_Tabla5.Colspan = 1;
                            C2_L1_Tabla5.Border = 0;
                            table5.AddCell(C2_L1_Tabla5);

                            PdfPCell C3_L1_Tabla5 = new PdfPCell(new Phrase("Peso", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.BOLD)));
                            C3_L1_Tabla5.Colspan = 1;
                            C3_L1_Tabla5.Border = 0;
                            table5.AddCell(C3_L1_Tabla5);

                            PdfPCell C4_L1_Tabla5 = new PdfPCell(new Phrase(Lectura_Hora["HC_Peso"].ToString(), FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            C4_L1_Tabla5.Colspan = 1;
                            C4_L1_Tabla5.Border = 0;
                            table5.AddCell(C4_L1_Tabla5);

                            //segunda linea

                            PdfPCell C1_L2_Tabla5 = new PdfPCell(new Phrase("Frecuencia Cardiaca ", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.BOLD)));
                            C1_L2_Tabla5.Colspan = 1;
                            C1_L2_Tabla5.Border = 0;
                            table5.AddCell(C1_L2_Tabla5);

                            PdfPCell C2_L2_Tabla5 = new PdfPCell(new Phrase(Lectura_Hora["HC_Frecar"].ToString(), FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            C2_L2_Tabla5.Colspan = 1;
                            C2_L2_Tabla5.Border = 0;
                            table5.AddCell(C2_L2_Tabla5);

                            PdfPCell C3_L2_Tabla5 = new PdfPCell(new Phrase("IMC ", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.BOLD)));
                            C3_L2_Tabla5.Colspan = 1;
                            C3_L2_Tabla5.Border = 0;
                            table5.AddCell(C3_L2_Tabla5);

                            PdfPCell C4_L2_Tabla5 = new PdfPCell(new Phrase(Lectura_Hora["HC_IMC"].ToString(), FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            C4_L2_Tabla5.Colspan = 1;
                            C4_L2_Tabla5.Border = 0;
                            table5.AddCell(C4_L2_Tabla5);

                            //tercera linea

                            PdfPCell C1_L3_Tabla5 = new PdfPCell(new Phrase("Frecuencia Respiratoria ", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.BOLD)));
                            C1_L3_Tabla5.Colspan = 1;
                            C1_L3_Tabla5.Border = 0;
                            table5.AddCell(C1_L3_Tabla5);

                            PdfPCell C2_L3_Tabla5 = new PdfPCell(new Phrase(Lectura_Hora["HC_FreRes"].ToString(), FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            C2_L3_Tabla5.Colspan = 1;
                            C2_L3_Tabla5.Border = 0;
                            table5.AddCell(C2_L3_Tabla5);

                            PdfPCell C3_L3_Tabla5 = new PdfPCell(new Phrase("Talla ", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.BOLD)));
                            C3_L3_Tabla5.Colspan = 1;
                            C3_L3_Tabla5.Border = 0;
                            table5.AddCell(C3_L3_Tabla5);

                            PdfPCell C4_L3_Tabla5 = new PdfPCell(new Phrase(Lectura_Hora["HC_Altura"].ToString(), FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            C4_L3_Tabla5.Colspan = 1;
                            C4_L3_Tabla5.Border = 0;
                            table5.AddCell(C4_L3_Tabla5);

                            //cuarta linea

                            PdfPCell C1_L4_Tabla5 = new PdfPCell(new Phrase("Temperatura ", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.BOLD)));
                            C1_L4_Tabla5.Colspan = 1;
                            C1_L4_Tabla5.Border = 0;
                            table5.AddCell(C1_L4_Tabla5);

                            PdfPCell C2_L4_Tabla5 = new PdfPCell(new Phrase(Lectura_Hora["HC_Temp"].ToString(), FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            C2_L4_Tabla5.Colspan = 1;
                            C2_L4_Tabla5.Border = 0;
                            table5.AddCell(C2_L4_Tabla5);

                            PdfPCell C3_L4_Tabla5 = new PdfPCell(new Phrase("Indice Tobillo Brazo ", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.BOLD)));
                            C3_L4_Tabla5.Colspan = 1;
                            C3_L4_Tabla5.Border = 0;
                            table5.AddCell(C3_L4_Tabla5);

                            PdfPCell C4_L4_Tabla5 = new PdfPCell(new Phrase(Lectura_Hora["HC_ITB"].ToString(), FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            C4_L4_Tabla5.Colspan = 1;
                            C4_L4_Tabla5.Border = 0;
                            table5.AddCell(C4_L4_Tabla5);

                            //quinta linea

                            PdfPCell C1_L5_Tabla5 = new PdfPCell(new Phrase(" ", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.BOLD)));
                            C1_L5_Tabla5.Colspan = 1;
                            C1_L5_Tabla5.Border = 0;
                            table5.AddCell(C1_L5_Tabla5);

                            PdfPCell C2_L5_Tabla5 = new PdfPCell(new Phrase(" ", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            C2_L5_Tabla5.Colspan = 1;
                            C2_L5_Tabla5.Border = 0;
                            table5.AddCell(C2_L5_Tabla5);

                            PdfPCell C3_L5_Tabla5 = new PdfPCell(new Phrase(" ", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.BOLD)));
                            C3_L5_Tabla5.Colspan = 1;
                            C3_L5_Tabla5.Border = 0;
                            table5.AddCell(C3_L5_Tabla5);

                            PdfPCell C4_L5_Tabla5 = new PdfPCell(new Phrase(" ", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            C4_L5_Tabla5.Colspan = 1;
                            C4_L5_Tabla5.Border = 0;
                            table5.AddCell(C4_L5_Tabla5);

                            //sexta linea

                            PdfPCell C1_L5_Tabla6 = new PdfPCell(new Phrase("Apariencia General ", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.BOLD)));
                            C1_L5_Tabla6.Colspan = 1;
                            C1_L5_Tabla6.Border = 0;
                            table5.AddCell(C1_L5_Tabla6);

                            PdfPCell C2_L5_Tabla6 = new PdfPCell(new Phrase(Lectura_Hora["HC_AparienciaG"].ToString(), FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            C2_L5_Tabla6.Colspan = 1;
                            C2_L5_Tabla6.Border = 0;
                            table5.AddCell(C2_L5_Tabla6);

                            PdfPCell C3_L5_Tabla6 = new PdfPCell(new Phrase("Grado de Cuidado ", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.BOLD)));
                            C3_L5_Tabla6.Colspan = 1;
                            C3_L5_Tabla6.Border = 0;
                            table5.AddCell(C3_L5_Tabla6);

                            PdfPCell C4_L5_Tabla6 = new PdfPCell(new Phrase(Lectura_Hora["HC_GradoC"].ToString(), FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            C4_L5_Tabla6.Colspan = 1;
                            C4_L5_Tabla6.Border = 0;
                            table5.AddCell(C4_L5_Tabla6);

                            //septima linea

                            PdfPCell C1_L5_Tabla7 = new PdfPCell(new Phrase("Estado Emocional ", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.BOLD)));
                            C1_L5_Tabla7.Colspan = 1;
                            C1_L5_Tabla7.Border = 0;
                            table5.AddCell(C1_L5_Tabla7);

                            PdfPCell C2_L5_Tabla7 = new PdfPCell(new Phrase(Lectura_Hora["HC_EstadoEmo"].ToString(), FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            C2_L5_Tabla7.Colspan = 1;
                            C2_L5_Tabla7.Border = 0;
                            table5.AddCell(C2_L5_Tabla7);

                            PdfPCell C3_L5_Tabla7 = new PdfPCell(new Phrase("Actividad y Ejercicio ", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.BOLD)));
                            C3_L5_Tabla7.Colspan = 1;
                            C3_L5_Tabla7.Border = 0;
                            table5.AddCell(C3_L5_Tabla7);

                            PdfPCell C4_L5_Tabla7 = new PdfPCell(new Phrase(Lectura_Hora["HC_ActEje"].ToString(), FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            C4_L5_Tabla7.Colspan = 1;
                            C4_L5_Tabla7.Border = 0;
                            table5.AddCell(C4_L5_Tabla7);

                            //octava linea

                            PdfPCell C1_L5_Tabla8 = new PdfPCell(new Phrase("Estado Nutricional ", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.BOLD)));
                            C1_L5_Tabla8.Colspan = 1;
                            C1_L5_Tabla8.Border = 0;
                            table5.AddCell(C1_L5_Tabla8);

                            PdfPCell C2_L5_Tabla8 = new PdfPCell(new Phrase(Lectura_Hora["HC_EstadoNut"].ToString(), FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            C2_L5_Tabla8.Colspan = 1;
                            C2_L5_Tabla8.Border = 0;
                            table5.AddCell(C2_L5_Tabla8);

                            PdfPCell C3_L5_Tabla8 = new PdfPCell(new Phrase("Tipo Lesion ", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.BOLD)));
                            C3_L5_Tabla8.Colspan = 1;
                            C3_L5_Tabla8.Border = 0;
                            table5.AddCell(C3_L5_Tabla8);

                            PdfPCell C4_L5_Tabla8 = new PdfPCell(new Phrase(Lectura_Hora["HC_TipoLes"].ToString(), FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            C4_L5_Tabla8.Colspan = 1;
                            C4_L5_Tabla8.Border = 0;
                            table5.AddCell(C4_L5_Tabla8);

                            doc.Add(table5);

                            //FIN QUINTA TABLA

                            doc.Add(new Chunk(Environment.NewLine));
                            doc.Add(line);

                            //HERIDAS

                            Paragraph title18 = new Paragraph();
                            title18.Font = FontFactory.GetFont(FontFactory.TIMES_BOLD, 8f, BaseColor.BLACK);
                            title18.Add("   HERIDAS");
                            title18.Alignment = Element.ALIGN_LEFT;
                            doc.Add(title18); doc.Add(new Paragraph(" ", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));

                            Paragraph title19 = new Paragraph();
                            title19.Font = FontFactory.GetFont(FontFactory.TIMES_BOLD, 8f, BaseColor.BLACK);
                            title19.Add("Descripcion de la(s) herida(s) ");
                            title19.Alignment = Element.ALIGN_LEFT;
                            doc.Add(title19); doc.Add(new Paragraph(" ", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            doc.Add(new Paragraph(Lectura_Hora["HC_DescHer"].ToString(), FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            doc.Add(new Chunk(Environment.NewLine));

                            Paragraph title20 = new Paragraph();
                            title20.Font = FontFactory.GetFont(FontFactory.TIMES_BOLD, 8f, BaseColor.BLACK);
                            title20.Add("Consistencia y Cantidad de Exudado");
                            title20.Alignment = Element.ALIGN_LEFT;
                            doc.Add(title20); doc.Add(new Paragraph(" ", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            doc.Add(new Paragraph(Lectura_Hora["HC_ConsCant"].ToString(), FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            doc.Add(new Chunk(Environment.NewLine));

                            Paragraph title21 = new Paragraph();
                            title21.Font = FontFactory.GetFont(FontFactory.TIMES_BOLD, 8f, BaseColor.BLACK);
                            title21.Add("Tejidos Comprometidos");
                            title21.Alignment = Element.ALIGN_LEFT;
                            doc.Add(title21); doc.Add(new Paragraph(" ", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            doc.Add(new Paragraph(Lectura_Hora["HC_TejCom"].ToString(), FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            doc.Add(new Chunk(Environment.NewLine));

                            Paragraph title22 = new Paragraph();
                            title22.Font = FontFactory.GetFont(FontFactory.TIMES_BOLD, 8f, BaseColor.BLACK);
                            title22.Add("Caracteristicas del Tejido");
                            title22.Alignment = Element.ALIGN_LEFT;
                            doc.Add(title22); doc.Add(new Paragraph(" ", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            doc.Add(new Paragraph(Lectura_Hora["HC_CaracTej"].ToString(), FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            doc.Add(new Chunk(Environment.NewLine));

                            Paragraph title23 = new Paragraph();
                            title23.Font = FontFactory.GetFont(FontFactory.TIMES_BOLD, 8f, BaseColor.BLACK);
                            title23.Add("Exudado");
                            title23.Alignment = Element.ALIGN_LEFT;
                            doc.Add(title23); doc.Add(new Paragraph(" ", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            doc.Add(new Paragraph(Lectura_Hora["HC_Exudado"].ToString(), FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            doc.Add(new Chunk(Environment.NewLine));

                            Paragraph title24 = new Paragraph();
                            title24.Font = FontFactory.GetFont(FontFactory.TIMES_BOLD, 8f, BaseColor.BLACK);
                            title24.Add("Signos de Infeccion");
                            title24.Alignment = Element.ALIGN_LEFT;
                            doc.Add(title24); doc.Add(new Paragraph(" ", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            doc.Add(new Paragraph(Lectura_Hora["HC_SignosInf"].ToString(), FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            doc.Add(new Chunk(Environment.NewLine));

                            Paragraph title25 = new Paragraph();
                            title25.Font = FontFactory.GetFont(FontFactory.TIMES_BOLD, 8f, BaseColor.BLACK);
                            title25.Add("Piel Cicundante");
                            title25.Alignment = Element.ALIGN_LEFT;
                            doc.Add(title25); doc.Add(new Paragraph(" ", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            doc.Add(new Paragraph(Lectura_Hora["HC_PielCirc"].ToString(), FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            doc.Add(new Chunk(Environment.NewLine));

                            doc.Add(line);

                            //EGRESO

                            Paragraph title26 = new Paragraph();
                            title26.Font = FontFactory.GetFont(FontFactory.TIMES_BOLD, 8f, BaseColor.BLACK);
                            title26.Add("   EGRESO");
                            title26.Alignment = Element.ALIGN_LEFT;
                            doc.Add(title26); doc.Add(new Paragraph(" ", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));

                            Paragraph title27 = new Paragraph();
                            title27.Font = FontFactory.GetFont(FontFactory.TIMES_BOLD, 8f, BaseColor.BLACK);
                            title27.Add("Analisis ");
                            title27.Alignment = Element.ALIGN_LEFT;
                            doc.Add(title27); doc.Add(new Paragraph(" ", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            doc.Add(new Paragraph(Lectura_Hora["HC_Analisis"].ToString(), FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            doc.Add(new Chunk(Environment.NewLine));

                            Paragraph title28 = new Paragraph();
                            title28.Font = FontFactory.GetFont(FontFactory.TIMES_BOLD, 8f, BaseColor.BLACK);
                            title28.Add("Plan de Manejo");
                            title28.Alignment = Element.ALIGN_LEFT;
                            doc.Add(title28); doc.Add(new Paragraph(" ", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            doc.Add(new Paragraph(Lectura_Hora["HC_PManejo"].ToString(), FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            doc.Add(new Chunk(Environment.NewLine));

                            Paragraph title29 = new Paragraph();
                            title29.Font = FontFactory.GetFont(FontFactory.TIMES_BOLD, 8f, BaseColor.BLACK);
                            title29.Add("Diagnostico Principal");
                            title29.Alignment = Element.ALIGN_LEFT;
                            doc.Add(title29); doc.Add(new Paragraph(" ", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            doc.Add(new Paragraph(Lectura_Hora["HC_DX1"].ToString() + " - " + Diag1, FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            doc.Add(new Chunk(Environment.NewLine));

                            Paragraph title29A = new Paragraph();
                            title29A.Font = FontFactory.GetFont(FontFactory.TIMES_BOLD, 8f, BaseColor.BLACK);
                            title29A.Add("Diagnostico Relacionado 1");
                            title29A.Alignment = Element.ALIGN_LEFT;
                            doc.Add(title29A); doc.Add(new Paragraph(" ", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            doc.Add(new Paragraph(Lectura_Hora["HC_DX2"].ToString() + " - " + Diag2, FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            doc.Add(new Chunk(Environment.NewLine));

                            Paragraph title29B = new Paragraph();
                            title29B.Font = FontFactory.GetFont(FontFactory.TIMES_BOLD, 8f, BaseColor.BLACK);
                            title29B.Add("Diagnostico Relacionado 2");
                            title29B.Alignment = Element.ALIGN_LEFT;
                            doc.Add(title29B); doc.Add(new Paragraph(" ", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            doc.Add(new Paragraph(Lectura_Hora["HC_DX3"].ToString() + " - " + Diag3, FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            doc.Add(new Chunk(Environment.NewLine));

                            Paragraph title30 = new Paragraph();
                            title30.Font = FontFactory.GetFont(FontFactory.TIMES_BOLD, 8f, BaseColor.BLACK);
                            title30.Add("Pruebas Diagnosticas Complementarias");
                            title30.Alignment = Element.ALIGN_LEFT;
                            doc.Add(title30); doc.Add(new Paragraph(" ", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            doc.Add(new Paragraph(Lectura_Hora["HC_PruebasDiag"].ToString(), FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            doc.Add(new Chunk(Environment.NewLine));

                            Paragraph title31 = new Paragraph();
                            title31.Font = FontFactory.GetFont(FontFactory.TIMES_BOLD, 8f, BaseColor.BLACK);
                            title31.Add("Complicaciones durante el tratamiento en esta institucion");
                            title31.Alignment = Element.ALIGN_LEFT;
                            doc.Add(title31); doc.Add(new Paragraph(" ", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            doc.Add(new Paragraph(Lectura_Hora["HC_Complicacion"].ToString(), FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            doc.Add(new Chunk(Environment.NewLine));

                            Paragraph title32 = new Paragraph();
                            title32.Font = FontFactory.GetFont(FontFactory.TIMES_BOLD, 8f, BaseColor.BLACK);
                            title32.Add("Informacion de Control Epidemiologico");
                            title32.Alignment = Element.ALIGN_LEFT;
                            doc.Add(title32); doc.Add(new Paragraph(" ", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            doc.Add(new Paragraph(Lectura_Hora["HC_Epidemia"].ToString(), FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            doc.Add(new Chunk(Environment.NewLine));

                            Paragraph title33 = new Paragraph();
                            title33.Font = FontFactory.GetFont(FontFactory.TIMES_BOLD, 8f, BaseColor.BLACK);
                            title33.Add("Notas Aclaratorias");
                            title33.Alignment = Element.ALIGN_LEFT;
                            doc.Add(title33); doc.Add(new Paragraph(" ", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            doc.Add(new Paragraph(Lectura_Hora["HC_Nota_Acl"].ToString(), FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            doc.Add(new Chunk(Environment.NewLine));
                            doc.Add(new Chunk(Environment.NewLine));
                            doc.Add(new Chunk(Environment.NewLine));

                            //////// IMAGEN

                            string Bod_Firma1 = Lectura_Hora["Bod_Firma"].ToString(); //trae base64
                            Byte[] bytes = Convert.FromBase64String(Bod_Firma1); //convierte a bytes
                            MemoryStream stmBLOBData = new MemoryStream(bytes);

                            iTextSharp.text.Image imagen = null;
                            imagen = iTextSharp.text.Image.GetInstance(bytes);
                            imagen.BorderWidth = 0;
                            imagen.Alignment = Element.ALIGN_RIGHT;
                            float percentage = 0.0f;
                            percentage = 75 / imagen.Width;
                            imagen.ScalePercent(percentage * 100);
                            //imagen.SetAbsolutePosition(doc.PageSize.Width - 36f - 72f,
                            //                              doc.PageSize.Height - 36f - 72f);
                            doc.Add(imagen);
                            ///////

                            Paragraph title34 = new Paragraph();
                            title34.Font = FontFactory.GetFont(FontFactory.TIMES_BOLD, 8f, BaseColor.BLACK);
                            title34.Add("Firma y Sello");
                            title34.Alignment = Element.ALIGN_RIGHT;
                            doc.Add(title34);
                            doc.Add(new Chunk(Environment.NewLine));

                            doc.Add(new Chunk(Environment.NewLine));
                            doc.Add(new Chunk(Environment.NewLine));

                            //SEXTA TABLA

                            PdfPTable table6 = new PdfPTable(4);
                            table6.HorizontalAlignment = Element.ALIGN_CENTER;
                            table6.SetTotalWidth(new float[] { 100, 170, 100, 72 });
                            table6.LockedWidth = (true);


                            //Primera fila
                            PdfPCell C1_L1_table6 = new PdfPCell(new Phrase("Escala de dolor 1 a 10 ", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.BOLDITALIC)));
                            C1_L1_table6.Colspan = 1;
                            C1_L1_table6.Border = 1;
                            C1_L1_table6.UseVariableBorders = true;
                            C1_L1_table6.BorderWidthLeft = 1f;
                            C1_L1_table6.BorderWidthRight = 1f;
                            C1_L1_table6.BorderWidthTop = 1f;
                            C1_L1_table6.BorderWidthBottom = 1f;
                            C1_L1_table6.BorderColorLeft = BaseColor.BLACK;
                            C1_L1_table6.BorderColorRight = BaseColor.BLACK;
                            C1_L1_table6.BorderColorTop = BaseColor.BLACK;
                            C1_L1_table6.BorderColorBottom = BaseColor.BLACK;

                            table6.AddCell(C1_L1_table6);

                            PdfPCell C2_L1_Tabla6 = new PdfPCell(new Phrase(Lectura_Hora["HC_Dolor"].ToString(), FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.BOLDITALIC)));
                            C2_L1_Tabla6.Colspan = 1;
                            C2_L1_Tabla6.Border = 1;
                            C2_L1_Tabla6.UseVariableBorders = true;
                            C2_L1_Tabla6.BorderWidthLeft = 1f;
                            C2_L1_Tabla6.BorderWidthRight = 1f;
                            C2_L1_Tabla6.BorderWidthTop = 1f;
                            C2_L1_Tabla6.BorderWidthBottom = 1f;
                            C2_L1_Tabla6.BorderColorLeft = BaseColor.BLACK;
                            C2_L1_Tabla6.BorderColorRight = BaseColor.BLACK;
                            C2_L1_Tabla6.BorderColorTop = BaseColor.BLACK;
                            C2_L1_Tabla6.BorderColorBottom = BaseColor.BLACK;
                            table6.AddCell(C2_L1_Tabla6);

                            PdfPCell C3_L1_Tabla6 = new PdfPCell(new Phrase("Estado del Paciente ", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.BOLDITALIC)));
                            C3_L1_Tabla6.Colspan = 1;
                            C3_L1_Tabla6.Border = 1;
                            C3_L1_Tabla6.UseVariableBorders = true;
                            C3_L1_Tabla6.BorderWidthLeft = 1f;
                            C3_L1_Tabla6.BorderWidthRight = 1f;
                            C3_L1_Tabla6.BorderWidthTop = 1f;
                            C3_L1_Tabla6.BorderWidthBottom = 1f;
                            C3_L1_Tabla6.BorderColorLeft = BaseColor.BLACK;
                            C3_L1_Tabla6.BorderColorRight = BaseColor.BLACK;
                            C3_L1_Tabla6.BorderColorTop = BaseColor.BLACK;
                            C3_L1_Tabla6.BorderColorBottom = BaseColor.BLACK;
                            table6.AddCell(C3_L1_Tabla6);

                            PdfPCell C4_L1_Tabla6 = new PdfPCell(new Phrase(Lectura_Hora["HC_Estado"].ToString(), FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.BOLDITALIC)));
                            C4_L1_Tabla6.Colspan = 1;
                            C4_L1_Tabla6.Border = 1;
                            C4_L1_Tabla6.UseVariableBorders = true;
                            C4_L1_Tabla6.BorderWidthLeft = 1f;
                            C4_L1_Tabla6.BorderWidthRight = 1f;
                            C4_L1_Tabla6.BorderWidthTop = 1f;
                            C4_L1_Tabla6.BorderWidthBottom = 1f;
                            C4_L1_Tabla6.BorderColorLeft = BaseColor.BLACK;
                            C4_L1_Tabla6.BorderColorRight = BaseColor.BLACK;
                            C4_L1_Tabla6.BorderColorTop = BaseColor.BLACK;
                            C4_L1_Tabla6.BorderColorBottom = BaseColor.BLACK;
                            table6.AddCell(C4_L1_Tabla6);

                            //segunda linea

                            PdfPCell C1_L2_Tabla6 = new PdfPCell(new Phrase("Medico ", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.BOLDITALIC)));
                            C1_L2_Tabla6.Colspan = 1;
                            C1_L2_Tabla6.Border = 1;
                            C1_L2_Tabla6.UseVariableBorders = true;
                            C1_L2_Tabla6.BorderWidthLeft = 1f;
                            C1_L2_Tabla6.BorderWidthRight = 1f;
                            C1_L2_Tabla6.BorderWidthTop = 1f;
                            C1_L2_Tabla6.BorderWidthBottom = 1f;
                            C1_L2_Tabla6.BorderColorLeft = BaseColor.BLACK;
                            C1_L2_Tabla6.BorderColorRight = BaseColor.BLACK;
                            C1_L2_Tabla6.BorderColorTop = BaseColor.BLACK;
                            C1_L2_Tabla6.BorderColorBottom = BaseColor.BLACK;
                            table6.AddCell(C1_L2_Tabla6);

                            PdfPCell C2_L2_Tabla6 = new PdfPCell(new Phrase(Lectura_Hora["Bod_Responsable"].ToString(), FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.BOLDITALIC)));
                            C2_L2_Tabla6.Colspan = 1;
                            C2_L2_Tabla6.Border = 1;
                            C2_L2_Tabla6.UseVariableBorders = true;
                            C2_L2_Tabla6.BorderWidthLeft = 1f;
                            C2_L2_Tabla6.BorderWidthRight = 1f;
                            C2_L2_Tabla6.BorderWidthTop = 1f;
                            C2_L2_Tabla6.BorderWidthBottom = 1f;
                            C2_L2_Tabla6.BorderColorLeft = BaseColor.BLACK;
                            C2_L2_Tabla6.BorderColorRight = BaseColor.BLACK;
                            C2_L2_Tabla6.BorderColorTop = BaseColor.BLACK;
                            C2_L2_Tabla6.BorderColorBottom = BaseColor.BLACK;
                            table6.AddCell(C2_L2_Tabla6);

                            PdfPCell C3_L2_Tabla6 = new PdfPCell(new Phrase("Posoperatorio ", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.BOLDITALIC)));
                            C3_L2_Tabla6.Colspan = 1;
                            C3_L2_Tabla6.Border = 1;
                            C3_L2_Tabla6.UseVariableBorders = true;
                            C3_L2_Tabla6.BorderWidthLeft = 1f;
                            C3_L2_Tabla6.BorderWidthRight = 1f;
                            C3_L2_Tabla6.BorderWidthTop = 1f;
                            C3_L2_Tabla6.BorderWidthBottom = 1f;
                            C3_L2_Tabla6.BorderColorLeft = BaseColor.BLACK;
                            C3_L2_Tabla6.BorderColorRight = BaseColor.BLACK;
                            C3_L2_Tabla6.BorderColorTop = BaseColor.BLACK;
                            C3_L2_Tabla6.BorderColorBottom = BaseColor.BLACK;
                            table6.AddCell(C3_L2_Tabla6);

                            PdfPCell C4_L2_Tabla6 = new PdfPCell(new Phrase(Lectura_Hora["HC_Patologia"].ToString(), FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.BOLDITALIC)));
                            C4_L2_Tabla6.Colspan = 1;
                            C4_L2_Tabla6.Border = 1;
                            C4_L2_Tabla6.UseVariableBorders = true;
                            C4_L2_Tabla6.BorderWidthLeft = 1f;
                            C4_L2_Tabla6.BorderWidthRight = 1f;
                            C4_L2_Tabla6.BorderWidthTop = 1f;
                            C4_L2_Tabla6.BorderWidthBottom = 1f;
                            C4_L2_Tabla6.BorderColorLeft = BaseColor.BLACK;
                            C4_L2_Tabla6.BorderColorRight = BaseColor.BLACK;
                            C4_L2_Tabla6.BorderColorTop = BaseColor.BLACK;
                            C4_L2_Tabla6.BorderColorBottom = BaseColor.BLACK;
                            table6.AddCell(C4_L2_Tabla6);

                            doc.Add(table6);

                            //FIN TABLA 6

                            doc.Close();
                            Name_Paciente = Lectura_Hora["HC_Pac"].ToString();
                        }
                        Unificar(Name_Paciente);
                    }
                    else
                    {
                        MessageBox.Show("No fue posible exportar la historia clinica");
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        void IHistoriasFolios.Genera_Export_Notas(int Pac_Id, DateTime Desde, DateTime Hasta)
        {
            try
            {
                Dictionary<string,string> getData = Conexion.Conection();

                using (SqlConnection con = new SqlConnection(getData["Conexion"]))
                {
                    if (con != null && con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }
                    String Cargar_Hora2 = "SELECT Com_Nombre, Com_Direccion, Com_Telefono, Bod_Responsable, Pac_PrimerN, Pac_SegundoN, Pac_PrimerA, Pac_SegundoA, " +
                                          "Ase_Descripcion, Pac_TipoId, Pac_IdNum, Bod_Reg_Med, Pac_Sexo, Not_Edad, Pac_FechaNto, Hor_Pac_Fecha_Cita, Not_Nota, Not_Observa, " +
                                          "Not_Recomienda , Not_Adm, Not_Adherencia, Not_Epidemia, Not_NotaAcla, Car_Dx1, Car_Dx2, Car_Dx3  " +
                                          "FROM CXN_NOTAS " +
                                          "INNER JOIN CXN_HORARIO ON CXN_NOTAS.Not_Adm = CXN_HORARIO.Hor_id " +
                                          "INNER JOIN CXN_PACIENTES ON CXN_HORARIO.Hor_Pac_Id = CXN_PACIENTES.Pac_Id " +
                                          "INNER JOIN CXN_CIA ON CXN_HORARIO.Hor_Pac_Cia = CXN_CIA.Com_Identificador " +
                                          "INNER JOIN CXN_BODEGAS ON CXN_HORARIO.Hor_Pac_Bod = CXN_BODEGAS.Bod_Numero " +
                                          "INNER JOIN CXN_ASEGURADORA ON CXN_HORARIO.Hor_Pac_Ase = CXN_ASEGURADORA.Ase_Identificador " +
                                          "INNER JOIN CXN_CARGOS ON CXN_HORARIO.Hor_Id = CXN_CARGOS.Car_Adm_Id " +
                                          "WHERE CXN_NOTAS.Not_Pac = '" + Pac_Id + "' " +
                                          "AND CXN_CARGOS.Car_Tipo = 'Nota' " +
                                          "AND CXN_HORARIO.Hor_Pac_Fecha_Cita BETWEEN '" + Convert.ToDateTime(Desde).ToString(getData["Format_Fecha"]) + "' AND '" + Convert.ToDateTime(Hasta).ToString(getData["Format_Fecha"]) + "' " +
                                          "ORDER BY CXN_HORARIO.Hor_Pac_Fecha_Cita ASC";
                    SqlCommand Carga_Command2 = new SqlCommand(Cargar_Hora2, con);
                    SqlDataReader Lectura_Hora2 = (Carga_Command2.ExecuteReader());
                    if (Lectura_Hora2.HasRows)
                    {
                        while (Lectura_Hora2.Read() == true)
                        {
                            var Diag1 = repoCIE10.BuscaDX(Lectura_Hora2["Car_Dx1"].ToString());
                            var Diag2 = repoCIE10.BuscaDX(Lectura_Hora2["Car_Dx2"].ToString());
                            var Diag3 = repoCIE10.BuscaDX(Lectura_Hora2["Car_Dx3"].ToString());

                            Name_Paciente = Lectura_Hora2["Pac_PrimerN"].ToString() + " " + Lectura_Hora2["Pac_SegundoN"].ToString() + " " + Lectura_Hora2["Pac_PrimerA"].ToString() + " " + Lectura_Hora2["Pac_SegundoA"].ToString();
                            var QR = repoGen.CodifyQR(Name_Paciente + " -> ADMISION: " + Lectura_Hora2["Not_Adm"].ToString());
                            Cons();

                            Document doc = new Document();
                            PdfWriter.GetInstance(doc, new FileStream("C:\\CXN\\Reportes\\" + Contador + ".pdf", FileMode.Create));
                            doc.Open();

                            Paragraph title = new Paragraph();
                            Paragraph title1 = new Paragraph();
                            Paragraph title2 = new Paragraph();
                            Paragraph title3 = new Paragraph();
                            Paragraph title4 = new Paragraph();
                            Paragraph title5 = new Paragraph();
                            Paragraph title6 = new Paragraph();
                            Paragraph title7 = new Paragraph();
                            Paragraph title7Dx1 = new Paragraph();
                            Paragraph title7Dx2 = new Paragraph();
                            Paragraph title7Dx3 = new Paragraph();

                            //ENCABEZADO PRIMERA PAGINA
                            title.Font = FontFactory.GetFont(FontFactory.TIMES_BOLD, 8f, BaseColor.BLACK);
                            title.Add("Admision " + Lectura_Hora2["Not_Adm"].ToString() + " - NOTAS DE ENFERMERIA");
                            title.Alignment = Element.ALIGN_CENTER;

                            //////// IMAGEN
                            iTextSharp.text.Image imagen = iTextSharp.text.Image.GetInstance(repoGen.GetBytes(QR));
                            imagen.BorderWidth = 0;
                            imagen.Alignment = Element.ALIGN_RIGHT;
                            float percentage = 0.0f;
                            percentage = 75 / imagen.Width;
                            imagen.ScalePercent(percentage * 100);
                            imagen.SetAbsolutePosition(doc.PageSize.Width - 36f - 72f,
                                                            doc.PageSize.Height - 36f - 72f);
                            doc.Add(imagen);
                            ///////

                            doc.Add(title);
                            doc.Add(new Paragraph("  "));

                            Phrase folio = new Phrase(new Chunk("Empresa: ", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.BOLD)));
                            Phrase folio2 = new Phrase(new Chunk(Lectura_Hora2["Com_Nombre"].ToString(), FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            doc.Add(folio); doc.Add(folio2); doc.Add(new Chunk(Environment.NewLine));

                            Phrase folio3 = new Phrase(new Chunk("Direccion: ", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.BOLD)));
                            Phrase folio4 = new Phrase(new Chunk(Lectura_Hora2["Com_Direccion"].ToString(), FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            doc.Add(folio3); doc.Add(folio4); doc.Add(new Chunk(Environment.NewLine));

                            Phrase folio5 = new Phrase(new Chunk("Telefono: ", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.BOLD)));
                            Phrase folio6 = new Phrase(new Chunk(Lectura_Hora2["Com_Telefono"].ToString(), FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            doc.Add(folio5); doc.Add(folio6);
                            doc.Add(new Chunk(Environment.NewLine)); doc.Add(new Chunk(Environment.NewLine));

                            //TABLA ENCABEZADO                        
                            PdfPTable table1 = new PdfPTable(6);
                            table1.HorizontalAlignment = Element.ALIGN_LEFT;
                            table1.SetTotalWidth(new float[] { 65, 220, 65, 70, 40, 40 });
                            table1.LockedWidth = (true);

                            //Primera fila
                            PdfPCell cella = new PdfPCell(new Phrase("Paciente: ", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.BOLD)));
                            cella.Colspan = 1;
                            cella.Border = 0;
                            table1.AddCell(cella);

                            PdfPCell cell2a = new PdfPCell(new Phrase(Name_Paciente, FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            cell2a.Colspan = 1;
                            cell2a.Border = 0;
                            table1.AddCell(cell2a);

                            PdfPCell cell3a = new PdfPCell(new Phrase("Genero: ", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.BOLD)));
                            cell3a.Colspan = 1;
                            cell3a.Border = 0;
                            table1.AddCell(cell3a);

                            PdfPCell cell2aa = new PdfPCell(new Phrase(Lectura_Hora2["Pac_Sexo"].ToString(), FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            cell2aa.Colspan = 1;
                            cell2aa.Border = 0;
                            table1.AddCell(cell2aa);

                            PdfPCell cell3aa = new PdfPCell(new Phrase("Edad: ", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.BOLD)));
                            cell3aa.Colspan = 1;
                            cell3aa.Border = 0;
                            table1.AddCell(cell3aa);

                            PdfPCell cell2aaa = new PdfPCell(new Phrase(Lectura_Hora2["Not_Edad"].ToString(), FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            cell2aaa.Colspan = 1;
                            cell2aaa.Border = 0;
                            table1.AddCell(cell2aaa);

                            //segunda fila
                            PdfPCell cell3aaa = new PdfPCell(new Phrase("Identificacion: ", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.BOLD)));
                            cell3aaa.Colspan = 1;
                            cell3aaa.Border = 0;
                            table1.AddCell(cell3aaa);

                            PdfPCell cell2aaaa = new PdfPCell(new Phrase(Lectura_Hora2["Pac_TipoId"].ToString() + " " + Lectura_Hora2["Pac_IdNum"].ToString(), FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            cell2aaaa.Colspan = 1;
                            cell2aaaa.Border = 0;
                            table1.AddCell(cell2aaaa);

                            PdfPCell cell3aaaa = new PdfPCell(new Phrase("Fecha Nto: ", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.BOLD)));
                            cell3aaaa.Colspan = 1;
                            cell3aaaa.Border = 0;
                            table1.AddCell(cell3aaaa);

                            PdfPCell cell2aaaaa = new PdfPCell(new Phrase(Convert.ToDateTime(Lectura_Hora2["Pac_FechaNto"]).ToString(getData["Format_Fecha"]), FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            cell2aaaaa.Colspan = 1;
                            cell2aaaaa.Border = 0;
                            table1.AddCell(cell2aaaaa);

                            PdfPCell C77a = new PdfPCell(new Phrase(" ", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            C77a.Border = 0;
                            table1.AddCell(C77a);
                            PdfPCell C87a = new PdfPCell(new Phrase(" ", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.ITALIC)));
                            C87a.Border = 0;
                            table1.AddCell(C87a);

                            //tercera fila
                            PdfPCell cell3aaab = new PdfPCell(new Phrase("Aseguradora: ", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.BOLD)));
                            cell3aaab.Colspan = 1;
                            cell3aaab.Border = 0;
                            table1.AddCell(cell3aaab);

                            PdfPCell cell2aaaab = new PdfPCell(new Phrase(Lectura_Hora2["Ase_Descripcion"].ToString(), FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            cell2aaaab.Colspan = 1;
                            cell2aaaab.Border = 0;
                            table1.AddCell(cell2aaaab);

                            PdfPCell cell3aaaab = new PdfPCell(new Phrase("Fecha Admision: ", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.BOLD)));
                            cell3aaaab.Colspan = 1;
                            cell3aaaab.Border = 0;
                            table1.AddCell(cell3aaaab);

                            PdfPCell cell2aaaaab = new PdfPCell(new Phrase(Convert.ToDateTime(Lectura_Hora2["Hor_Pac_Fecha_Cita"]).ToString(getData["Format_Fecha"]), FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            cell2aaaaab.Colspan = 1;
                            cell2aaaaab.Border = 0;
                            table1.AddCell(cell2aaaaab);

                            PdfPCell C77ab = new PdfPCell(new Phrase(" ", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            C77ab.Border = 0;
                            table1.AddCell(C77ab);
                            PdfPCell C87ab = new PdfPCell(new Phrase(" ", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.ITALIC)));
                            C87ab.Border = 0;
                            table1.AddCell(C87ab);
                            doc.Add(table1);
                            //FIN TABAL ENCABEZADO

                            doc.Add(new Chunk(Environment.NewLine)); doc.Add(new Chunk(Environment.NewLine));

                            LineSeparator line = new LineSeparator(1f, 100f, BaseColor.BLACK, Element.ALIGN_LEFT, 1);
                            doc.Add(line);
                            /// FIN  ENCABEZADO PRIMERA PAGINA

                            title1.Font = FontFactory.GetFont(FontFactory.TIMES_BOLD, 8f, BaseColor.BLACK);
                            title1.Add("NOTA DE CURACION");
                            title1.Alignment = Element.ALIGN_CENTER;
                            doc.Add(title1); doc.Add(new Chunk(Environment.NewLine));

                            doc.Add(new Paragraph(Lectura_Hora2["Not_Nota"].ToString(), FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            doc.Add(new Chunk(Environment.NewLine));

                            title2.Font = FontFactory.GetFont(FontFactory.TIMES_BOLD, 8f, BaseColor.BLACK);
                            title2.Add("APOSITOS E INSUMOS USADOS EN LA CURACION");
                            title2.Alignment = Element.ALIGN_CENTER;
                            doc.Add(title2);
                            doc.Add(new Paragraph(" ", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));

                            PdfPTable table = new PdfPTable(3);
                            table.HorizontalAlignment = Element.ALIGN_CENTER;
                            table.SetTotalWidth(new float[] { 72, 350, 72 });
                            table.LockedWidth = (true);

                            PdfPCell cell = new PdfPCell(new Phrase("Codigo", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.BOLD)));
                            cell.Colspan = 1;
                            cell.Border = 0;
                            table.AddCell(cell);

                            PdfPCell cell2 = new PdfPCell(new Phrase("Item/Descripcion", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.BOLD)));
                            cell2.Colspan = 1;
                            cell2.Border = 0;
                            table.AddCell(cell2);

                            PdfPCell cell3 = new PdfPCell(new Phrase("Cantidad", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.BOLD)));
                            cell3.Colspan = 1;
                            cell3.Border = 0;
                            table.AddCell(cell3);

                            PdfPCell C77 = new PdfPCell(new Phrase(" ", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            C77.Border = 0;
                            table.AddCell(C77);
                            PdfPCell C87 = new PdfPCell(new Phrase(" ", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.ITALIC)));
                            C87.Border = 0;
                            table.AddCell(C87);
                            PdfPCell C97 = new PdfPCell(new Phrase(" ", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            C97.Border = 0;
                            table.AddCell(C97);

                            String Cargar_Hora = "SELECT Com_Nombre, Com_Direccion, Com_Telefono, Bod_Responsable, Pac_PrimerN, Pac_SegundoN, Pac_PrimerA, Pac_SegundoA, " +
                                          "Ase_Descripcion, Pac_TipoId, Pac_IdNum, Bod_Reg_Med, Pac_Sexo, Not_Edad, Pac_FechaNto, Hor_Pac_Fecha_Cita, Not_Nota, Not_Observa, " +
                                          "Not_Recomienda , Not_Adm, Car_Cod, Car_Item, Car_Detalle, Car_Cant, Not_Epidemia, Not_Adherencia, Not_NotaAcla " +
                                          "FROM CXN_NOTAS " +
                                          "INNER JOIN CXN_HORARIO ON CXN_NOTAS.Not_Adm = CXN_HORARIO.Hor_id " +
                                          "INNER JOIN CXN_PACIENTES ON CXN_HORARIO.Hor_Pac_Id = CXN_PACIENTES.Pac_Id " +
                                          "INNER JOIN CXN_CIA ON CXN_HORARIO.Hor_Pac_Cia = CXN_CIA.Com_Identificador " +
                                          "INNER JOIN CXN_BODEGAS ON CXN_HORARIO.Hor_Pac_Bod = CXN_BODEGAS.Bod_Numero " +
                                          "INNER JOIN CXN_ASEGURADORA ON CXN_HORARIO.Hor_Pac_Ase = CXN_ASEGURADORA.Ase_Identificador " +
                                          "INNER JOIN CXN_CARGOS ON CXN_HORARIO.Hor_Id = CXN_CARGOS.Car_Adm_Id " +
                                          "WHERE CXN_CARGOS.Car_Tipo IN ('Cargo','Nota') " +
                                          "AND CXN_CARGOS.Car_Adm_Id = '" + Lectura_Hora2["Not_Adm"].ToString() + "'";
                            SqlCommand Carga_Command = new SqlCommand(Cargar_Hora, con);
                            SqlDataReader Lectura_Hora = (Carga_Command.ExecuteReader());
                            {
                                while (Lectura_Hora.Read() == true)
                                {
                                    //Segunda fila
                                    PdfPCell cell4 = new PdfPCell(new Phrase(Lectura_Hora["Car_Cod"].ToString(), FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                                    cell4.Border = 0;
                                    table.AddCell(cell4);

                                    PdfPCell cell5 = new PdfPCell(new Phrase(
                                        Lectura_Hora["Car_Item"].ToString(),
                                        FontFactory.GetFont("Times New Roman",
                                        7,
                                        iTextSharp.text.Font.NORMAL)));
                                    cell5.Border = 0;
                                    table.AddCell(cell5);

                                    PdfPCell cell6 = new PdfPCell(new Phrase(Convert.ToInt32(Lectura_Hora["Car_Cant"]).ToString(), FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                                    cell6.Border = 0;
                                    table.AddCell(cell6);

                                    PdfPCell C7 = new PdfPCell(new Phrase("", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                                    C7.Border = 0;
                                    table.AddCell(C7);
                                    PdfPCell C8 = new PdfPCell(new Phrase(Lectura_Hora["Car_Detalle"].ToString(), FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.ITALIC)));
                                    C8.Border = 0;
                                    table.AddCell(C8);
                                    PdfPCell C9 = new PdfPCell(new Phrase("", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                                    C9.Border = 0;
                                    table.AddCell(C9);
                                }
                                doc.Add(table);
                                doc.NewPage(); //esto agrega una nueva hoja despues de  los apositos
                            }

                            //ENCABEZADO SEGUNDA PAGINA
                            doc.Add(imagen);
                            doc.Add(title);
                            doc.Add(new Paragraph("  "));
                            doc.Add(folio); doc.Add(folio2); doc.Add(new Chunk(Environment.NewLine));
                            doc.Add(folio3); doc.Add(folio4); doc.Add(new Chunk(Environment.NewLine));
                            doc.Add(folio5); doc.Add(folio6);
                            doc.Add(new Chunk(Environment.NewLine)); doc.Add(new Chunk(Environment.NewLine));

                            //Llamar Encabezado de Tabla
                            doc.Add(table1);

                            doc.Add(new Chunk(Environment.NewLine)); doc.Add(new Chunk(Environment.NewLine));
                            LineSeparator line2 = new LineSeparator(1f, 100f, BaseColor.BLACK, Element.ALIGN_LEFT, 1);
                            doc.Add(line2);
                            //FIN ENCABEZADO SEGUNDA PAGINA

                            title3.Font = FontFactory.GetFont(FontFactory.TIMES_BOLD, 8f, BaseColor.BLACK);
                            title3.Add("ADHERENCIA Y/O PERTINENCIA DE APOSITOS");
                            title3.Alignment = Element.ALIGN_CENTER;
                            doc.Add(title3);
                            doc.Add(new Chunk(Environment.NewLine));
                            doc.Add(new Paragraph(Lectura_Hora2["Not_Adherencia"].ToString(), FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            doc.Add(new Chunk(Environment.NewLine));

                            title4.Font = FontFactory.GetFont(FontFactory.TIMES_BOLD, 8f, BaseColor.BLACK);
                            title4.Add("INFORMACION EPIDEMIOLOGICA");
                            title4.Alignment = Element.ALIGN_CENTER;
                            doc.Add(title4);
                            doc.Add(new Chunk(Environment.NewLine));
                            doc.Add(new Paragraph(Lectura_Hora2["Not_Epidemia"].ToString(), FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            doc.Add(new Chunk(Environment.NewLine));

                            title5.Font = FontFactory.GetFont(FontFactory.TIMES_BOLD, 8f, BaseColor.BLACK);
                            title5.Add("OBSERVACIONES DE CURACION");
                            title5.Alignment = Element.ALIGN_CENTER;
                            doc.Add(title5);
                            doc.Add(new Chunk(Environment.NewLine));
                            doc.Add(new Paragraph(Lectura_Hora2["Not_Observa"].ToString(), FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            doc.Add(new Chunk(Environment.NewLine));

                            title6.Font = FontFactory.GetFont(FontFactory.TIMES_BOLD, 8f, BaseColor.BLACK);
                            title6.Add("RECOMENDACIONES Y/O CUIDADOS DE CURACION");
                            title6.Alignment = Element.ALIGN_CENTER;
                            doc.Add(title6);
                            doc.Add(new Chunk(Environment.NewLine));
                            doc.Add(new Paragraph(Lectura_Hora2["Not_Recomienda"].ToString(), FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            doc.Add(new Chunk(Environment.NewLine));

                            title7.Font = FontFactory.GetFont(FontFactory.TIMES_BOLD, 8f, BaseColor.BLACK);
                            title7.Add("NOTAS ACLARATORIAS A ESTA NOTA DE CURACION");
                            title7.Alignment = Element.ALIGN_CENTER;
                            doc.Add(title7);
                            doc.Add(new Chunk(Environment.NewLine));
                            doc.Add(new Paragraph(Lectura_Hora2["Not_NotaAcla"].ToString(), FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            doc.Add(new Chunk(Environment.NewLine));

                            //DX 
                            title7Dx1.Font = FontFactory.GetFont(FontFactory.TIMES_BOLD, 8f, BaseColor.BLACK);
                            title7Dx1.Add("Diagnostico Principal: ");
                            title7Dx1.Alignment = Element.ALIGN_LEFT;
                            doc.Add(title7Dx1);
                            doc.Add(new Paragraph(Lectura_Hora2["Car_Dx1"].ToString() + " - " + Diag1, FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            doc.Add(new Chunk(Environment.NewLine));

                            title7Dx2.Font = FontFactory.GetFont(FontFactory.TIMES_BOLD, 8f, BaseColor.BLACK);
                            title7Dx2.Add("Diagnostico Relacionado 1");
                            title7Dx2.Alignment = Element.ALIGN_LEFT;
                            doc.Add(title7Dx2);
                            doc.Add(new Paragraph(Lectura_Hora2["Car_Dx2"].ToString() + " - " + Diag2, FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            doc.Add(new Chunk(Environment.NewLine));

                            title7Dx3.Font = FontFactory.GetFont(FontFactory.TIMES_BOLD, 8f, BaseColor.BLACK);
                            title7Dx3.Add("Diagnostico Relacionado 2");
                            title7Dx3.Alignment = Element.ALIGN_LEFT;
                            doc.Add(title7Dx3);
                            doc.Add(new Paragraph(Lectura_Hora2["Car_Dx3"].ToString() + " - " + Diag3, FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.NORMAL)));
                            doc.Add(new Chunk(Environment.NewLine));
                            //DX FIN

                            doc.Add(new Chunk(Environment.NewLine)); doc.Add(new Chunk(Environment.NewLine));

                            Phrase Pie = new Phrase(new Chunk("Profesional: ", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.BOLD)));
                            doc.Add(Pie);
                            doc.Add(new Chunk(Environment.NewLine));
                            Phrase Pie2 = new Phrase(new Chunk(Lectura_Hora2["Bod_Responsable"].ToString(), FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.BOLD)));
                            doc.Add(Pie2);
                            doc.Add(new Chunk(Environment.NewLine));

                            Phrase Pie3 = new Phrase(new Chunk("Centro Medico: ", FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.BOLD)));
                            doc.Add(Pie3);
                            doc.Add(new Chunk(Environment.NewLine));
                            Phrase Pie4 = new Phrase(new Chunk(Lectura_Hora2["Com_Nombre"].ToString(), FontFactory.GetFont("Times New Roman", 7, iTextSharp.text.Font.BOLD)));
                            doc.Add(Pie4);

                            doc.Close();
                        }
                        MessageBox.Show("Generado");
                        Unificar(Name_Paciente);
                    }
                    else
                    {
                        MessageBox.Show("No hay historias de este paciente", "No se pudo generar PDF", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
