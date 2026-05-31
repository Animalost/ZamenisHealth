using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Domain;
using Domain.CXN;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using ZamenisHealth.Clases;
using ZamenisHealth.Comunes;
using ZamenisHealth.HistoriasClinicas.Extras;

namespace ZamenisHealth.HistoriasClinicas
{
    public partial class Historia_TerFisica : ConfigForm.BaseForm
    {
        private static readonly ITerapiaFisica repoTF = new MTerapiaFisica();
        private static readonly IRIPS repoRIPS = new MRIPS();
        private static readonly ICargos repoCargos = new MCargos();
        private static readonly IAgenda repoAgendaMedica = new MAgenda();
        private static readonly IConvenios repoConvenios = new MConvenios();
        private static readonly IAgendaC repoAgendaMedicaConsultas = new MAgendaC();
        private static readonly IConfSystem repoConfSystem = new MConfSystem();
        private static readonly ICondiciones repoCond = new MCondiciones();

        private Extras.CondicionesP c;
        public int Admision;
        DateTime Fecha_Serv;
        int Paciente, Cia, Ase, Prof, Valor;

        private void toolStripLabel1_Click(object sender, EventArgs e)
        {
            try
            {
                if (textBox7.Text == "") { MessageBox.Show("Revise seccion datos basicos"); return; }
                if (textBox8.Text == "") { MessageBox.Show("Revise seccion datos basicos"); return; }
                if (textBox9.Text == "") { MessageBox.Show("Revise seccion datos basicos"); return; }
                if (textBox10.Text == "") { MessageBox.Show("Revise seccion datos basicos"); return; }
                if (textBox11.Text == "") { MessageBox.Show("Revise diagnostico principal"); return; }
                if (textBox12.Text == "") { MessageBox.Show("Revise diagnostico principal"); return; }
                if (textBox13.Text == "") { MessageBox.Show("Revise seccion datos basicos"); return; }
                if (textBox14.Text == "") { MessageBox.Show("Revise seccion datos basicos"); return; }
                if (textBox15.Text == "") { MessageBox.Show("Revise seccion datos basicos"); return; }
                if (textBox16.Text == "") { MessageBox.Show("Revise seccion datos basicos"); return; }
                if (textBox17.Text == "") { MessageBox.Show("Revise seccion datos basicos"); return; }
                if (textBox24.Text == "") { MessageBox.Show("Revise seccion datos basicos"); return; }
                if (textBox25.Text == "") { MessageBox.Show("Revise seccion datos basicos"); return; }

                if (textBox18.Text == "") { MessageBox.Show("Revise seccion Antecedentes"); return; }
                if (textBox19.Text == "") { MessageBox.Show("Revise seccion Antecedentes"); return; }
                if (textBox20.Text == "") { MessageBox.Show("Revise seccion Antecedentes"); return; }
                if (textBox21.Text == "") { MessageBox.Show("Revise seccion Antecedentes"); return; }
                if (textBox22.Text == "") { MessageBox.Show("Revise seccion Antecedentes"); return; }
                if (textBox23.Text == "") { MessageBox.Show("Revise seccion Antecedentes"); return; }
                if (textBox26.Text == "") { MessageBox.Show("Revise seccion Antecedentes"); return; }

                if (textBox27.Text == "") { MessageBox.Show("Revise seccion Examen Fisico"); return; }
                if (textBox29.Text == "") { MessageBox.Show("Revise seccion Examen Fisico"); return; }
                if (textBox30.Text == "") { MessageBox.Show("Revise seccion Examen Fisico"); return; }
                if (textBox31.Text == "") { MessageBox.Show("Revise seccion Examen Fisico"); return; }
                if (comboBox1.Text == "") { MessageBox.Show("Revise seccion Examen Fisico"); return; }
                if (comboBox2.Text == "") { MessageBox.Show("Revise seccion Examen Fisico"); return; }
                if (comboBox3.Text == "") { MessageBox.Show("Revise seccion Examen Fisico"); return; }
                if (comboBox4.Text == "") { MessageBox.Show("Revise seccion Examen Fisico"); return; }
                if (comboBox8.Text == "") { MessageBox.Show("Revise seccion Examen Fisico"); return; }
                if (comboBox9.Text == "") { MessageBox.Show("Revise seccion Examen Fisico"); return; }
                if (textBox34.Text == "") { MessageBox.Show("Revise seccion Examen Fisico"); return; }
                if (textBox35.Text == "") { MessageBox.Show("Revise seccion Examen Fisico"); return; }
                if (textBox36.Text == "") { MessageBox.Show("Revise seccion Examen Fisico"); return; }
                if (textBox37.Text == "") { MessageBox.Show("Revise seccion Examen Fisico"); return; }
                if (textBox38.Text == "") { MessageBox.Show("Revise seccion Examen Fisico"); return; }
                if (textBox39.Text == "") { MessageBox.Show("Revise seccion Examen Fisico"); return; }
                if (textBox43.Text == "") { MessageBox.Show("Revise seccion Examen Fisico"); return; }
                if (textBox42.Text == "") { MessageBox.Show("Revise seccion Examen Fisico"); return; }
                if (textBox41.Text == "") { MessageBox.Show("Revise seccion Examen Fisico"); return; }
                if (textBox40.Text == "") { MessageBox.Show("Revise seccion Examen Fisico"); return; }
                if (textBox49.Text == "") { MessageBox.Show("Revise seccion Examen Fisico"); return; }
                if (textBox48.Text == "") { MessageBox.Show("Revise seccion Examen Fisico"); return; }
                if (textBox47.Text == "") { MessageBox.Show("Revise seccion Examen Fisico"); return; }
                if (textBox46.Text == "") { MessageBox.Show("Revise seccion Examen Fisico"); return; }
                if (textBox45.Text == "") { MessageBox.Show("Revise seccion Examen Fisico"); return; }
                if (textBox44.Text == "") { MessageBox.Show("Revise seccion Examen Fisico"); return; }
                if (textBox55.Text == "") { MessageBox.Show("Revise seccion Examen Fisico"); return; }
                if (textBox54.Text == "") { MessageBox.Show("Revise seccion Examen Fisico"); return; }
                if (textBox53.Text == "") { MessageBox.Show("Revise seccion Examen Fisico"); return; }
                if (textBox52.Text == "") { MessageBox.Show("Revise seccion Examen Fisico"); return; }
                if (textBox51.Text == "") { MessageBox.Show("Revise seccion Examen Fisico"); return; }
                if (textBox50.Text == "") { MessageBox.Show("Revise seccion Examen Fisico"); return; }

                if (textBox59.Text == "") { MessageBox.Show("Revise seccion Examen Fisico 2"); return; }
                if (textBox58.Text == "") { MessageBox.Show("Revise seccion Examen Fisico 2"); return; }
                if (textBox57.Text == "") { MessageBox.Show("Revise seccion Examen Fisico 2"); return; }
                if (textBox56.Text == "") { MessageBox.Show("Revise seccion Examen Fisico 2"); return; }
                if (textBox61.Text == "") { MessageBox.Show("Revise seccion Examen Fisico 2"); return; }
                if (textBox69.Text == "") { MessageBox.Show("Revise seccion Examen Fisico 2"); return; }
                if (textBox68.Text == "") { MessageBox.Show("Revise seccion Examen Fisico 2"); return; }
                if (textBox67.Text == "") { MessageBox.Show("Revise seccion Examen Fisico 2"); return; }
                if (textBox66.Text == "") { MessageBox.Show("Revise seccion Examen Fisico 2"); return; }
                if (textBox65.Text == "") { MessageBox.Show("Revise seccion Examen Fisico 2"); return; }
                if (textBox64.Text == "") { MessageBox.Show("Revise seccion Examen Fisico 2"); return; }
                if (textBox63.Text == "") { MessageBox.Show("Revise seccion Examen Fisico 2"); return; }
                if (textBox62.Text == "") { MessageBox.Show("Revise seccion Examen Fisico 2"); return; }
                if (textBox117.Text == "") { MessageBox.Show("Revise seccion Examen Fisico 2"); return; }
                if (textBox116.Text == "") { MessageBox.Show("Revise seccion Examen Fisico 2"); return; }
                if (textBox115.Text == "") { MessageBox.Show("Revise seccion Examen Fisico 2"); return; }
                if (textBox114.Text == "") { MessageBox.Show("Revise seccion Examen Fisico 2"); return; }
                if (textBox113.Text == "") { MessageBox.Show("Revise seccion Examen Fisico 2"); return; }
                if (textBox112.Text == "") { MessageBox.Show("Revise seccion Examen Fisico 2"); return; }
                if (textBox111.Text == "") { MessageBox.Show("Revise seccion Examen Fisico 2"); return; }
                if (textBox110.Text == "") { MessageBox.Show("Revise seccion Examen Fisico 2"); return; }
                if (textBox109.Text == "") { MessageBox.Show("Revise seccion Examen Fisico 2"); return; }
                if (textBox108.Text == "") { MessageBox.Show("Revise seccion Examen Fisico 2"); return; }
                if (textBox127.Text == "") { MessageBox.Show("Revise seccion Examen Fisico 2"); return; }
                if (textBox126.Text == "") { MessageBox.Show("Revise seccion Examen Fisico 2"); return; }
                if (textBox125.Text == "") { MessageBox.Show("Revise seccion Examen Fisico 2"); return; }
                if (textBox124.Text == "") { MessageBox.Show("Revise seccion Examen Fisico 2"); return; }
                if (textBox123.Text == "") { MessageBox.Show("Revise seccion Examen Fisico 2"); return; }
                if (textBox122.Text == "") { MessageBox.Show("Revise seccion Examen Fisico 2"); return; }
                if (textBox121.Text == "") { MessageBox.Show("Revise seccion Examen Fisico 2"); return; }
                if (textBox120.Text == "") { MessageBox.Show("Revise seccion Examen Fisico 2"); return; }
                if (textBox119.Text == "") { MessageBox.Show("Revise seccion Examen Fisico 2"); return; }
                if (textBox118.Text == "") { MessageBox.Show("Revise seccion Examen Fisico 2"); return; }
                if (textBox95.Text == "") { MessageBox.Show("Revise seccion Examen Fisico 2"); return; }
                if (textBox94.Text == "") { MessageBox.Show("Revise seccion Examen Fisico 2"); return; }
                if (textBox93.Text == "") { MessageBox.Show("Revise seccion Examen Fisico 2"); return; }
                if (textBox92.Text == "") { MessageBox.Show("Revise seccion Examen Fisico 2"); return; }
                if (textBox70.Text == "") { MessageBox.Show("Revise seccion Examen Fisico 2"); return; }
                if (textBox72.Text == "") { MessageBox.Show("Revise seccion Examen Fisico 2"); return; }
                if (textBox74.Text == "") { MessageBox.Show("Revise seccion Examen Fisico 2"); return; }
                if (textBox76.Text == "") { MessageBox.Show("Revise seccion Examen Fisico 2"); return; }
                if (textBox78.Text == "") { MessageBox.Show("Revise seccion Examen Fisico 2"); return; }
                if (textBox80.Text == "") { MessageBox.Show("Revise seccion Examen Fisico 2"); return; }
                if (textBox82.Text == "") { MessageBox.Show("Revise seccion Examen Fisico 2"); return; }
                if (textBox84.Text == "") { MessageBox.Show("Revise seccion Examen Fisico 2"); return; }
                if (textBox86.Text == "") { MessageBox.Show("Revise seccion Examen Fisico 2"); return; }
                if (textBox88.Text == "") { MessageBox.Show("Revise seccion Examen Fisico 2"); return; }
                if (textBox90.Text == "") { MessageBox.Show("Revise seccion Examen Fisico 2"); return; }
                if (textBox71.Text == "") { MessageBox.Show("Revise seccion Examen Fisico 2"); return; }
                if (textBox73.Text == "") { MessageBox.Show("Revise seccion Examen Fisico 2"); return; }
                if (textBox75.Text == "") { MessageBox.Show("Revise seccion Examen Fisico 2"); return; }
                if (textBox77.Text == "") { MessageBox.Show("Revise seccion Examen Fisico 2"); return; }
                if (textBox79.Text == "") { MessageBox.Show("Revise seccion Examen Fisico 2"); return; }
                if (textBox81.Text == "") { MessageBox.Show("Revise seccion Examen Fisico 2"); return; }
                if (textBox83.Text == "") { MessageBox.Show("Revise seccion Examen Fisico 2"); return; }
                if (textBox85.Text == "") { MessageBox.Show("Revise seccion Examen Fisico 2"); return; }
                if (textBox87.Text == "") { MessageBox.Show("Revise seccion Examen Fisico 2"); return; }
                if (textBox89.Text == "") { MessageBox.Show("Revise seccion Examen Fisico 2"); return; }
                if (textBox91.Text == "") { MessageBox.Show("Revise seccion Examen Fisico 2"); return; }
                if (textBox107.Text == "") { MessageBox.Show("Revise seccion Examen Fisico 2"); return; }
                if (textBox106.Text == "") { MessageBox.Show("Revise seccion Examen Fisico 2"); return; }
                if (textBox105.Text == "") { MessageBox.Show("Revise seccion Examen Fisico 2"); return; }
                if (textBox104.Text == "") { MessageBox.Show("Revise seccion Examen Fisico 2"); return; }
                if (textBox103.Text == "") { MessageBox.Show("Revise seccion Examen Fisico 2"); return; }
                if (textBox102.Text == "") { MessageBox.Show("Revise seccion Examen Fisico 2"); return; }
                if (textBox101.Text == "") { MessageBox.Show("Revise seccion Examen Fisico 2"); return; }
                if (textBox100.Text == "") { MessageBox.Show("Revise seccion Examen Fisico 2"); return; }
                if (textBox99.Text == "") { MessageBox.Show("Revise seccion Examen Fisico 2"); return; }
                if (textBox98.Text == "") { MessageBox.Show("Revise seccion Examen Fisico 2"); return; }
                if (textBox97.Text == "") { MessageBox.Show("Revise seccion Examen Fisico 2"); return; }
                if (textBox96.Text == "") { MessageBox.Show("Revise seccion Examen Fisico 2"); return; }

                if (comboBox10.Text == "") { MessageBox.Show("Revise seccion vista posterior"); return; }
                if (comboBox11.Text == "") { MessageBox.Show("Revise seccion vista posterior"); return; }
                if (comboBox12.Text == "") { MessageBox.Show("Revise seccion vista posterior"); return; }
                if (comboBox13.Text == "") { MessageBox.Show("Revise seccion vista posterior"); return; }
                if (comboBox14.Text == "") { MessageBox.Show("Revise seccion vista posterior"); return; }

                if (comboBox15.Text == "") { MessageBox.Show("Revise seccion vista anterior"); return; }
                if (comboBox16.Text == "") { MessageBox.Show("Revise seccion vista anterior"); return; }
                if (comboBox17.Text == "") { MessageBox.Show("Revise seccion vista anterior"); return; }
                if (comboBox18.Text == "") { MessageBox.Show("Revise seccion vista anterior"); return; }

                if (comboBox19.Text == "") { MessageBox.Show("Revise seccion vista lateral"); return; }
                if (comboBox20.Text == "") { MessageBox.Show("Revise seccion vista lateral"); return; }
                if (comboBox21.Text == "") { MessageBox.Show("Revise seccion vista lateral"); return; }
                if (comboBox22.Text == "") { MessageBox.Show("Revise seccion vista lateral"); return; }
                if (comboBox23.Text == "") { MessageBox.Show("Revise seccion vista lateral"); return; }
                if (comboBox24.Text == "") { MessageBox.Show("Revise seccion vista lateral"); return; }
                if (comboBox25.Text == "") { MessageBox.Show("Revise seccion vista lateral"); return; }

                if (comboBox29.Text == "") { MessageBox.Show("Revise seccion marcha 2"); return; }
                if (comboBox28.Text == "") { MessageBox.Show("Revise seccion marcha 2"); return; }
                if (comboBox27.Text == "") { MessageBox.Show("Revise seccion marcha 2"); return; }
                if (comboBox26.Text == "") { MessageBox.Show("Revise seccion marcha 2"); return; }
                if (comboBox30.Text == "") { MessageBox.Show("Revise seccion marcha 2"); return; }

                if (textBox128.Text == "") { MessageBox.Show("Revise seccion resumen"); return; }
                if (textBox129.Text == "") { MessageBox.Show("Revise seccion resumen"); return; }
                if (textBox130.Text == "") { MessageBox.Show("Revise seccion resumen"); return; }
                if (textBox131.Text == "") { MessageBox.Show("Revise seccion resumen"); return; }
                if (textBox132.Text == "") { MessageBox.Show("Revise seccion resumen"); return; }



                DateTime HC_Fecha = Fecha_Serv;

                CXN_HCTF H = new CXN_HCTF
                {
                    HC_Fecha = Fecha_Serv,
                    HC_Pac = textBox1.Text,
                    HC_Cant = 1,
                    HC_Prof = Prof,   //param4
                    HC_Ase = Ase,   //param5
                    HC_Cia = Cia,   //param6
                    HC_PacId = Paciente,   //param7
                    HC_Adm = Admision,   //param8
                    HC_FechaNto = Convert.ToDateTime(textBox3.Text),   //param9
                    HC_Edad = textBox4.Text,
                    HC_EstadoC = textBox7.Text,
                    HC_Hijos = textBox8.Text,
                    HC_Estudios = textBox10.Text,
                    HC_CIE10 = textBox11.Text,   //param5
                    HC_Dx = textBox12.Text,   //param6
                    HC_TiempoE = textBox13.Text,   //param7
                    HC_TratamientosP = textBox14.Text,   //param8
                    HC_ExaDiag = textBox15.Text,   //param9
                    HC_Ocupacion = textBox9.Text,
                    HC_MotivoCons = textBox16.Text,
                    HC_AntFam = textBox17.Text,
                    HC_AntPat = textBox24.Text,
                    HC_AntNeu = textBox25.Text,   //param5
                    HC_SalMen = textBox18.Text,   //param6
                    HC_OsteoMusc = textBox19.Text,   //param7
                    HC_Endocrino = textBox20.Text,   //param8
                    HC_Quir = textBox21.Text,   //param9
                    HC_Resp = textBox22.Text,
                    HC_Derma = textBox23.Text,
                    HC_Farma = textBox26.Text,
                    HC_DolorEIAN = textBox27.Text,
                    HC_Eva = comboBox1.Text,   //param5
                    HC_FrecDol = comboBox2.Text,   //param6
                    HC_TipoDol = comboBox3.Text,   //param7
                    HC_CaracDol = textBox133.Text,   //param8
                    HC_Text60 = textBox28.Text,   //param9
                    HC_SensiDol = textBox29.Text,
                    HC_SintAso = textBox30.Text,
                    HC_HabTox = textBox31.Text,
                    HC_Cual1 = textBox32.Text,   //param6
                    HC_Cual2 = textBox33.Text,   //param7
                    HC_CC1 = textBox34.Text,   //param8
                    HC_CC2 = textBox35.Text,   //param9
                    HC_CC3 = textBox36.Text,
                    HC_CC4 = textBox37.Text,
                    HC_CC5 = textBox38.Text,
                    HC_CC6 = textBox39.Text,
                    HC_CD1 = textBox43.Text,   //param5
                    HC_CD2 = textBox42.Text,   //param6
                    HC_CD3 = textBox41.Text,   //param7
                    HC_CD4 = textBox40.Text,   //param8
                    HC_H1 = textBox49.Text,   //param9
                    HC_H2 = textBox48.Text,
                    HC_H3 = textBox47.Text,
                    HC_H4 = textBox46.Text,
                    HC_H5 = textBox45.Text,
                    HC_H6 = textBox44.Text,   //param5
                    HC_C1 = textBox55.Text,   //param6
                    HC_C2 = textBox54.Text,   //param7
                    HC_C3 = textBox53.Text,   //param8
                    HC_C4 = textBox52.Text,   //aram9
                    HC_C5 = textBox51.Text,
                    HC_C6 = textBox50.Text,
                    HC_T1 = textBox59.Text,
                    HC_T2 = textBox58.Text,
                    HC_T3 = textBox57.Text,   //param5
                    HC_T4 = textBox56.Text,   //param6
                    HC_T5 = textBox61.Text,   //param7
                    HC_T6 = textBox60.Text,   //param8
                    HC_T7 = textBox69.Text,   //param9
                    HC_T8 = textBox68.Text,
                    HC_T9 = textBox67.Text,
                    HC_T10 = textBox66.Text,
                    HC_T11 = textBox65.Text,
                    HC_T12 = textBox64.Text,   //param5
                    HC_T13 = textBox63.Text,   //param6
                    HC_T14 = textBox62.Text,   //param7
                    HisExtCuI = textBox71.Text,   //param8
                    HisExtCuD = textBox70.Text,   //param9
                    HisEscalenosI = textBox73.Text,
                    HisEscalenosD = textBox72.Text,
                    HisTrapSupI = textBox75.Text,
                    HisTrapSupD = textBox74.Text,
                    HisTrapMedI = textBox77.Text,
                    HisTrapMedD = textBox76.Text,   //param8
                    HisTrapInfI = textBox79.Text,   //param9
                    HisTrapInfD = textBox78.Text,
                    HisSerAntI = textBox81.Text,
                    HisSerAntD = textBox80.Text,
                    HisPecMayI = textBox83.Text,
                    HisPecMayD = textBox82.Text,
                    HisRomI = textBox85.Text,   //param8
                    HisRomD = textBox84.Text,   //param9
                    HisAbSupI = textBox87.Text,
                    HisAbSupD = textBox86.Text,
                    HisAbdInfI = textBox89.Text,
                    HisAbdInfD = textBox88.Text,
                    HisOblicuoI = textBox91.Text,
                    HisOblicuoD = textBox90.Text,   //param8
                    HisExtDorsalI = textBox107.Text,   //param9
                    HisExtDorsalD = textBox106.Text,
                    HisExtLumbarI = textBox105.Text,
                    HisExtLumbarD = textBox104.Text,
                    HisGluMayI = textBox103.Text,
                    HisGluMayD = textBox102.Text,
                    HisGluMedI = textBox101.Text,   //param8
                    HisGluMedD = textBox100.Text,   //param9
                    HisCuadriI = textBox99.Text,
                    HisCuadriD = textBox98.Text,
                    HisIsquiI = textBox97.Text,
                    HisIsquiD = textBox96.Text,
                    HisGemeloI = textBox95.Text,
                    HisGemeloD = textBox94.Text,
                    HisTibAntI = textBox96.Text,
                    HisTibAntD = textBox92.Text,
                    HisEscaAntI = textBox117.Text,
                    HisEscaAntD = textBox116.Text,
                    HisEscaMedI = textBox115.Text,
                    HisEscaMedD = textBox114.Text,
                    HisEscaPosI = textBox113.Text,
                    HisEscaPosD = textBox112.Text,
                    HisECMI = textBox111.Text,
                    HisECMD = textBox110.Text,
                    HisCualLumI = textBox109.Text,
                    HisCualLumD = textBox108.Text,
                    HisCuadI = textBox127.Text,
                    HisCuadD = textBox126.Text,
                    HisIsquiI2 = textBox125.Text,
                    HisIsqui2D = textBox124.Text,
                    HisTensorI = textBox123.Text,
                    HisTensorD = textBox122.Text,
                    HisGastroI = textBox121.Text,
                    HisGastroD = textBox120.Text,
                    HisTAquilesI = textBox119.Text,
                    HisTAquilesD = textBox118.Text,
                    HisCabeza = comboBox15.Text,
                    HisHombros = comboBox16.Text,
                    HisBrazos = comboBox17.Text,
                    HisCadera = comboBox18.Text,
                    HisEscapula = comboBox10.Text,
                    HisColumna = comboBox11.Text,
                    HisCrestasIliacas = comboBox12.Text,
                    HisGluteos = comboBox13.Text,
                    HisFosa = comboBox14.Text,
                    HisDiagnostico = textBox128.Text,
                    HisPronostico = textBox129.Text,
                    HisAcciones = textBox130.Text,
                    HisConducta = textBox131.Text,
                    Marcha_Mec = textBox132.Text,
                    HisRodilla = comboBox29.Text,
                    HisRotula = comboBox28.Text,
                    HisTibia = comboBox27.Text,
                    HisTobillo = comboBox26.Text,
                    HisPie = comboBox30.Text,
                    HisCuello = comboBox19.Text,
                    HisEspaldaA = comboBox20.Text,
                    HisTorax = comboBox21.Text,
                    HisAbdomen = comboBox22.Text,
                    HisEspaldaB = comboBox23.Text,
                    HisPelvis = comboBox24.Text,
                    HisRodillas = comboBox25.Text
                };

                if (comboBox8.Text == "SI") { H.HC_ActFis = "X"; }
                if (comboBox8.Text == "NO") { H.HC_ActFis = ""; }
                if (comboBox9.Text == "SI") { H.HC_MedExt = "X"; }
                if (comboBox9.Text == "NO") { H.HC_MedExt = ""; }

                bool insert = repoTF.insertHistoria(H);
                if (insert != true)
                {
                    MessageBox.Show("No se logro insertar la historia", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                else
                {
                    int impresion = repoRIPS.TipoRipCargo(comboBox139.Text, "IMPDX");

                    CXN_CARGOS C = new CXN_CARGOS
                    {
                        Car_Adm_Id = Admision,
                        Car_Pac = Paciente,
                        Car_Cia = Cia,
                        Car_Ase = Ase,
                        Car_Prof = Prof,
                        Car_Fecha = Fecha_Serv,
                        Car_Estado = "G",
                        Car_Tipo = "Historia",
                        Car_Cod = CUP,
                        Car_Val_Tot = Valor,
                        Car_Val_Un = Valor,
                        Car_Tipo_Serv = TSERV,
                        Car_Cant = 1,
                        Car_Detalle = textBox11.Text,
                        Car_Item = Serv,
                        Car_Dx1 = textBox11.Text,
                        Car_Dx2 = "",
                        Car_Dx3 = "",
                        Car_Ambito = 0,
                        Car_Personal = 0,
                        Car_CExterna = 0,
                        Car_Finalidad = 0,
                        Car_Finalidad_CO = 0, //motivo                                           
                        Car_Imp_Dx = impresion,
                        Car_Regimen = Reg_RIP
                    };

                    bool insertarCargo = repoCargos.InsertarCargoHistorias(C);
                    if (insertarCargo == false)
                    {
                        MessageBox.Show("No se logro guardar el cargo economico en el registro de valores a cobrar en la factura, " +
                            "su historia quedo resgitrada pero reporte este incidente a la recepcion con la admision: " + Admision,
                            "Advertencia!!!",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }
                    else
                    {
                        repoAgendaMedica.ConsumirAdmision(Admision);
                        repoAgendaMedica.Graba_Hora_Salida(Admision);

                        Medicina.RIPSHistory for_RIPS = new Medicina.RIPSHistory();
                        for_RIPS.Adm_Cargo = Convert.ToInt32(Admision);
                        for_RIPS.ShowDialog();

                        Medicina.AgendaM f2 = Application.OpenForms.OfType<Medicina.AgendaM>().LastOrDefault();
                        f2.Cargar_Agenda();

                        this.Dispose();
                        this.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void toolStripLabel2_Click(object sender, EventArgs e)
        {
            try
            {
                var getLast = repoTF.getLastHistory(Paciente);
                if (getLast != null)
                {
                    textBox7.Text = getLast.HC_EstadoC.ToString();
                    textBox8.Text = getLast.HC_Hijos.ToString();
                    textBox9.Text = getLast.HC_Ocupacion.ToString();
                    textBox10.Text = getLast.HC_Estudios.ToString();
                    textBox11.Text = getLast.HC_CIE10.ToString();
                    textBox12.Text = getLast.HC_Dx.ToString();
                    textBox13.Text = getLast.HC_TiempoE.ToString();
                    textBox14.Text = getLast.HC_TratamientosP.ToString();
                    textBox15.Text = getLast.HC_ExaDiag.ToString();
                    textBox17.Text = getLast.HC_AntFam.ToString();
                    textBox24.Text = getLast.HC_AntPat.ToString();
                    textBox25.Text = getLast.HC_AntNeu.ToString();
                    textBox18.Text = getLast.HC_SalMen.ToString();
                    textBox19.Text = getLast.HC_OsteoMusc.ToString();
                    textBox20.Text = getLast.HC_Endocrino.ToString();
                    textBox21.Text = getLast.HC_Quir.ToString();
                    textBox22.Text = getLast.HC_Resp.ToString();
                    textBox23.Text = getLast.HC_Derma.ToString();
                    textBox26.Text = getLast.HC_Farma.ToString();
                    textBox27.Text = getLast.HC_DolorEIAN.ToString();
                    comboBox1.Text = getLast.HC_Eva.ToString();
                    comboBox2.Text = getLast.HC_FrecDol.ToString();
                    comboBox3.Text = getLast.HC_TipoDol.ToString();
                    if (getLast.HC_ActFis.ToString() != "X") { comboBox8.Text = "NO"; } else { comboBox8.Text = "SI"; }
                    if (getLast.HC_MedExt.ToString() != "X") { comboBox9.Text = "NO"; } else { comboBox9.Text = "SI"; }
                    textBox32.Text = getLast.HC_Cual1.ToString();
                    textBox33.Text = getLast.HC_Cual2.ToString();
                    textBox34.Text = getLast.HC_CC1.ToString();
                    textBox35.Text = getLast.HC_CC2.ToString();
                    textBox36.Text = getLast.HC_CC3.ToString();
                    textBox37.Text = getLast.HC_CC4.ToString();
                    textBox38.Text = getLast.HC_CC5.ToString();
                    textBox39.Text = getLast.HC_CC6.ToString();
                    textBox43.Text = getLast.HC_CD1.ToString();
                    textBox42.Text = getLast.HC_CD2.ToString();
                    textBox41.Text = getLast.HC_CD3.ToString();
                    textBox40.Text = getLast.HC_CD4.ToString();
                    textBox49.Text = getLast.HC_H1.ToString();
                    textBox48.Text = getLast.HC_H2.ToString();
                    textBox47.Text = getLast.HC_H3.ToString();
                    textBox46.Text = getLast.HC_H4.ToString();
                    textBox45.Text = getLast.HC_H5.ToString();
                    textBox44.Text = getLast.HC_H6.ToString();
                    textBox55.Text = getLast.HC_C1.ToString();
                    textBox54.Text = getLast.HC_C2.ToString();
                    textBox53.Text = getLast.HC_C3.ToString();
                    textBox52.Text = getLast.HC_C4.ToString();
                    textBox51.Text = getLast.HC_C5.ToString();
                    textBox50.Text = getLast.HC_C6.ToString();
                    textBox59.Text = getLast.HC_T1.ToString();
                    textBox58.Text = getLast.HC_T2.ToString();
                    textBox57.Text = getLast.HC_T3.ToString();
                    textBox56.Text = getLast.HC_T4.ToString();
                    textBox61.Text = getLast.HC_T5.ToString();
                    textBox60.Text = getLast.HC_T6.ToString();
                    textBox69.Text = getLast.HC_T7.ToString();
                    textBox68.Text = getLast.HC_T8.ToString();
                    textBox67.Text = getLast.HC_T9.ToString();
                    textBox66.Text = getLast.HC_T10.ToString();
                    textBox65.Text = getLast.HC_T11.ToString();
                    textBox64.Text = getLast.HC_T12.ToString();
                    textBox63.Text = getLast.HC_T13.ToString();
                    textBox62.Text = getLast.HC_T14.ToString();
                    textBox71.Text = getLast.HisExtCuI.ToString();
                    textBox70.Text = getLast.HisExtCuD.ToString();
                    textBox73.Text = getLast.HisEscalenosI.ToString();
                    textBox72.Text = getLast.HisEscalenosD.ToString();
                    textBox75.Text = getLast.HisTrapSupI.ToString();
                    textBox74.Text = getLast.HisTrapSupD.ToString();
                    textBox77.Text = getLast.HisTrapMedI.ToString();
                    textBox76.Text = getLast.HisTrapMedD.ToString();
                    textBox79.Text = getLast.HisTrapInfI.ToString();
                    textBox78.Text = getLast.HisTrapInfD.ToString();
                    textBox81.Text = getLast.HisSerAntI.ToString();
                    textBox80.Text = getLast.HisSerAntD.ToString();
                    textBox83.Text = getLast.HisPecMayI.ToString();
                    textBox82.Text = getLast.HisPecMayD.ToString();
                    textBox85.Text = getLast.HisRomI.ToString();
                    textBox84.Text = getLast.HisRomD.ToString();
                    textBox87.Text = getLast.HisAbSupI.ToString();
                    textBox86.Text = getLast.HisAbSupD.ToString();
                    textBox89.Text = getLast.HisAbdInfI.ToString();
                    textBox88.Text = getLast.HisAbdInfD.ToString();
                    textBox91.Text = getLast.HisOblicuoI.ToString();
                    textBox90.Text = getLast.HisOblicuoD.ToString();
                    textBox107.Text = getLast.HisExtDorsalI.ToString();
                    textBox106.Text = getLast.HisExtDorsalD.ToString();
                    textBox105.Text = getLast.HisExtLumbarI.ToString();
                    textBox104.Text = getLast.HisExtLumbarD.ToString();
                    textBox103.Text = getLast.HisGluMayI.ToString();
                    textBox102.Text = getLast.HisGluMayD.ToString();
                    textBox101.Text = getLast.HisGluMedI.ToString();
                    textBox100.Text = getLast.HisGluMedD.ToString();
                    textBox99.Text = getLast.HisCuadriI.ToString();
                    textBox98.Text = getLast.HisCuadriD.ToString();
                    textBox97.Text = getLast.HisIsquiI.ToString();
                    textBox96.Text = getLast.HisIsquiD.ToString();
                    textBox95.Text = getLast.HisGemeloI.ToString();
                    textBox94.Text = getLast.HisGemeloD.ToString();
                    textBox93.Text = getLast.HisTibAntI.ToString();
                    textBox92.Text = getLast.HisTibAntD.ToString();
                    textBox117.Text = getLast.HisEscaAntI.ToString();
                    textBox116.Text = getLast.HisEscaAntD.ToString();
                    textBox115.Text = getLast.HisEscaMedI.ToString();
                    textBox114.Text = getLast.HisEscaMedD.ToString();
                    textBox113.Text = getLast.HisEscaPosI.ToString();
                    textBox112.Text = getLast.HisEscaPosD.ToString();
                    textBox111.Text = getLast.HisECMI.ToString();
                    textBox110.Text = getLast.HisECMD.ToString();
                    textBox109.Text = getLast.HisCualLumI.ToString();
                    textBox108.Text = getLast.HisCualLumD.ToString();
                    textBox127.Text = getLast.HisCuadI.ToString();
                    textBox126.Text = getLast.HisCuadD.ToString();
                    textBox125.Text = getLast.HisIsquiI2.ToString();
                    textBox124.Text = getLast.HisIsqui2D.ToString();
                    textBox123.Text = getLast.HisTensorI.ToString();
                    textBox122.Text = getLast.HisTensorD.ToString();
                    textBox121.Text = getLast.HisGastroI.ToString();
                    textBox120.Text = getLast.HisGastroD.ToString();
                    textBox119.Text = getLast.HisTAquilesI.ToString();
                    textBox118.Text = getLast.HisTAquilesD.ToString();
                    textBox128.Text = getLast.HisDiagnostico.ToString();
                    textBox129.Text = getLast.HisPronostico.ToString();
                    textBox130.Text = getLast.HisAcciones.ToString();
                    textBox131.Text = getLast.HisConducta.ToString();
                    textBox132.Text = getLast.Marcha_Mec.ToString();
                    MessageBox.Show("Se ha recuperado el historial del ultimo registro del dia: " + Convert.ToDateTime(getLast.HC_Fecha.ToString()).ToString(Conexion.ConectionDictionary["Format_Fecha"]));
                }
                else
                {
                    MessageBox.Show("No hay historial previo de este paciente", "Sin historial medico", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            Medicina.Historial_Medico_1 RM = new Medicina.Historial_Medico_1();
            RM.ShowDialog();
        }

        private void toolStripLabel6_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult result = MessageBox.Show("Desea cancelar esta historia? Al hacerlo no guardara ningun dato", "Zamenis_Health", MessageBoxButtons.YesNo);

                if (result == DialogResult.Yes)
                {
                    repoAgendaMedica.OpenAdmition(Admision, "N");
                    this.Dispose();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void toolStripButton9_Click(object sender, EventArgs e)
        {
            ActualizarPAC();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            textBox133.Text = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox29.Text = "";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox30.Text = "";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            textBox31.Text = "";
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                textBox133.Text = textBox133.Text + ", " + comboBox4.Text;
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                textBox29.Text = textBox29.Text + ", " + comboBox5.Text;
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void comboBox6_SelectedIndexChanged(object sender, EventArgs e)
        {

            try
            {
                textBox30.Text = textBox30.Text + ", " + comboBox6.Text;
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void comboBox7_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                textBox31.Text = textBox31.Text + ", " + comboBox7.Text;
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void Historia_TerFisica_Load(object sender, EventArgs e)
        {
            try
            {
                Titulo.Visible = false;
                ImageClose.Visible = false;

                

                repoAgendaMedica.Graba_Hora_Atencion(Admision);
                repoAgendaMedica.OpenAdmition(Admision, "S");

                var DatosAdmision = repoAgendaMedicaConsultas.cargarAdmision(Admision, "'P','H','A'");
                if (DatosAdmision == null)
                {
                    MessageBox.Show("Error en esta admision",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    this.Dispose();
                    this.Close();
                    return;
                }                

                Paciente = DatosAdmision.Hor_Pac_Id;
                Cia = DatosAdmision.Hor_Pac_Cia;
                Ase = DatosAdmision.Hor_Pac_Ase;
                Prof = DatosAdmision.Hor_Pac_Bod;
                CUP = DatosAdmision.Hor_Pac_Cup;
                TSERV = DatosAdmision.Hor_Pac_Tipo_Serv;
                Fecha_Serv = Convert.ToDateTime(DatosAdmision.Hor_Pac_Fecha_Cita);
                Reg_RIP = DatosAdmision.Hor_Regimen;
                CMANID = DatosAdmision.Pac_IdNum.ToString();
                CMANTID = DatosAdmision.Pac_TipoId.ToString();

                var VAl = repoConvenios.ServicioNombre(CUP, Ase, TSERV);
                if (VAl == null)
                {
                    MessageBox.Show("Error grave cargardo valor del servicio, vuelva a ingresar a la admision",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    this.Dispose();
                    this.Close();
                    return;
                }

                Valor = VAl.Con_Valor;
                Serv = VAl.Con_Nombre;

                textBox5.Text = Admision.ToString();
                textBox1.Text = DatosAdmision.Hor_Imp_Age.ToString();
                textBox3.Text = Convert.ToDateTime(DatosAdmision.Pac_FechaNto).ToString(Conexion.ConectionDictionary["Format_Fecha"]);
                textBox2.Text = DatosAdmision.Pac_TipoId.ToString() + " " + DatosAdmision.Pac_IdNum.ToString();

                DateTime nacimiento = Convert.ToDateTime(DatosAdmision.Pac_FechaNto);
                int edad = DateTime.Today.AddTicks(-nacimiento.Ticks).Year - 1;
                textBox4.Text = edad.ToString();

                textBox6.Text = "Terapia Fisica";

                ActualizarPAC();

                string getRecos = repoConfSystem.getListado()["Recomendaciones"];
                if (getRecos == "A")
                {
                    Extras.Recomendaciones r = new Extras.Recomendaciones(DatosAdmision.Hor_Pac_Id);
                    r.ShowDialog();

                    CargarOpcionesRecomendaciones();

                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        void CargarOpcionesRecomendaciones()
        {
            try
            {
                List<string> getConditions = repoCond.getCondiciones(this.Paciente);
                if (getConditions != null)
                {
                    string resultado = getConditions.Find(x => x == "CAIDA");
                    if (resultado != null)
                    {
                        CaidaTxt.BackColor = Color.DodgerBlue;
                        CaidaTxt.ForeColor = Color.Navy;
                    }
                    else
                    {
                        CaidaTxt.BackColor = Color.White;
                        CaidaTxt.ForeColor = Color.Black;
                    }

                    resultado = getConditions.Find(x => x == "INFECCION");
                    if (resultado != null)
                    {
                        infeccionTxt.BackColor = Color.LightGreen;
                        infeccionTxt.ForeColor = Color.Green;
                    }
                    else
                    {
                        infeccionTxt.BackColor = Color.White;
                        infeccionTxt.ForeColor = Color.Black;
                    }

                    resultado = getConditions.Find(x => x == "DETERIORO DE LA PIEL");
                    if (resultado != null)
                    {
                        deterioroTxt.BackColor = Color.Violet;
                        deterioroTxt.ForeColor = Color.DarkViolet;
                    }
                    else
                    {
                        deterioroTxt.BackColor = Color.White;
                        deterioroTxt.ForeColor = Color.Black;
                    }

                    resultado = getConditions.Find(x => x == "ALERGIA");
                    if (resultado != null)
                    {
                        alergiaTxt.BackColor = Color.Red;
                        alergiaTxt.ForeColor = Color.White;
                    }
                    else
                    {
                        alergiaTxt.BackColor = Color.White;
                        alergiaTxt.ForeColor = Color.Black;
                    }

                    resultado = getConditions.Find(x => x == "DIFICULTAD DE COMUNICACION");
                    if (resultado != null)
                    {
                        dificultadTxt.BackColor = Color.Yellow;
                        dificultadTxt.ForeColor = Color.Black;
                    }
                    else
                    {
                        dificultadTxt.BackColor = Color.White;
                        dificultadTxt.ForeColor = Color.Black;
                    }

                    resultado = getConditions.Find(x => x == "PACIENTE PSIQUIATRICO");
                    if (resultado != null)
                    {
                        psiquiatricoTxt.BackColor = Color.DarkViolet;
                        psiquiatricoTxt.ForeColor = Color.Thistle;
                    }
                    else
                    {
                        psiquiatricoTxt.BackColor = Color.White;
                        psiquiatricoTxt.ForeColor = Color.Black;
                    }

                    resultado = getConditions.Find(x => x == "MAYOR DE 70 AÑOS");
                    if (resultado != null)
                    {
                        mayorTxt.BackColor = Color.DarkTurquoise;
                        mayorTxt.ForeColor = Color.Blue;
                    }
                    else
                    {
                        mayorTxt.BackColor = Color.White;
                        mayorTxt.ForeColor = Color.Black;
                    }

                    resultado = getConditions.Find(x => x == "PACIENTE DIFICIL");
                    if (resultado != null)
                    {
                        dificilTxt.BackColor = Color.Orange;
                        dificilTxt.ForeColor = Color.AntiqueWhite;
                    }
                    else
                    {
                        dificilTxt.BackColor = Color.White;
                        dificilTxt.ForeColor = Color.Black;
                    }

                    resultado = getConditions.Find(x => x == "MEDICO LO REQUIERE");
                    if (resultado != null)
                    {
                        requiereTxt.BackColor = Color.Gray;
                        requiereTxt.ForeColor = Color.White;
                    }
                    else
                    {
                        requiereTxt.BackColor = Color.White;
                        requiereTxt.ForeColor = Color.Black;
                    }
                }
                else
                {
                    CaidaTxt.BackColor = Color.White;
                    CaidaTxt.ForeColor = Color.Black;
                    infeccionTxt.BackColor = Color.White;
                    infeccionTxt.ForeColor = Color.Black;
                    alergiaTxt.BackColor = Color.White;
                    alergiaTxt.ForeColor = Color.Black;
                    deterioroTxt.BackColor = Color.White;
                    deterioroTxt.ForeColor = Color.Black;
                    psiquiatricoTxt.BackColor = Color.White;
                    psiquiatricoTxt.ForeColor = Color.Black;
                    dificultadTxt.BackColor = Color.White;
                    dificultadTxt.ForeColor = Color.Black;
                    dificilTxt.BackColor = Color.White;
                    dificilTxt.ForeColor = Color.Black;
                    mayorTxt.BackColor = Color.White;
                    mayorTxt.ForeColor = Color.Black;
                    requiereTxt.BackColor = Color.White;
                    requiereTxt.ForeColor = Color.Black;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException
                {
                    FechaHora = DateTime.Now,
                    Error = ex.Message,
                    Formulario = this.Name,
                    Metodo = OverridesExtern.GetCurrentMethodName(),
                    Usuario = Contenedor.UsuarioLogueado
                };

                OverridesExtern.GenerarTXTException(T);
            }
        }

        private void ActualizarPAC()
        {
            Medicina.ActualizarPaciente Historia_Edita_Paciente = new Medicina.ActualizarPaciente(Paciente);
            Historia_Edita_Paciente.ShowDialog();
        }

        private void CaidaTxt_Click(object sender, EventArgs e)
        {
            openConditions("CAIDA");
        }

        void openConditions(string Tipo)
        {
            c = new CondicionesP(Tipo, this.Paciente);
            c.ShowDialog();

            CargarOpcionesRecomendaciones();
        }

        private void infeccionTxt_Click(object sender, EventArgs e)
        {
            openConditions("INFECCION");
        }

        private void deterioroTxt_Click(object sender, EventArgs e)
        {
            openConditions("DETERIORO DE LA PIEL");
        }

        private void alergiaTxt_Click(object sender, EventArgs e)
        {
            Alergias c = new Alergias("ALERGIA", this.Paciente);
            c.ShowDialog();
            CargarOpcionesRecomendaciones();
        }

        private void dificultadTxt_Click(object sender, EventArgs e)
        {
            openConditions("DIFICULTAD DE COMUNICACION");
        }

        private void psiquiatricoTxt_Click(object sender, EventArgs e)
        {
            openConditions("PACIENTE PSIQUIATRICO");
        }

        private void mayorTxt_Click(object sender, EventArgs e)
        {
            openConditions("MAYOR DE 70 AÑOS");
        }

        private void dificilTxt_Click(object sender, EventArgs e)
        {
            openConditions("PACIENTE DIFICIL");
        }

        private void requiereTxt_Click(object sender, EventArgs e)
        {
            openConditions("MEDICO LO REQUIERE");
        }

        private void textBox11_DoubleClick(object sender, EventArgs e)
        {
            Medicina.CIE10 C10 = new Medicina.CIE10("TFIS");
            C10.ShowDialog();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            textBox11.Text = "";
            textBox12.Text = "";
        }

        string CUP, TSERV, Reg_RIP, CMANTID, CMANID, Serv;
        public Historia_TerFisica()
        {
            InitializeComponent();

            
            ConfigForm.MoverForma(label1, this);
        }
    }
}
