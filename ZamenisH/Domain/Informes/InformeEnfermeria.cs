using System.Collections.Generic;

namespace Domain.Informes
{
    public class InformeEnfermeria : ComplementoInformePatologias
    {
        public string MesGenera { get; set; }
        public byte[] Logo { get; set; }

        public string Enfermero { get; set; }
        public int CantidadEnfermero { get; set; }
        public int SumatoriaCantAtendidos { get; set; }
        public double PorcentajeEnfermeroAtendidos { get; set; }
        public int CantPatologiaVasculares { get; set; }
        public int CantPatologiaPosOperatorio { get; set; }
        public int CantPatologiaZonadePresion { get; set; }
        public int CantPatologiaTrauma { get; set; }
        public int CantPatologiaQuemado { get; set; }
        public int CantPatologiaOtros { get; set; }
        public int CantPatologiaPieDiabetico { get; set; }
        public int Cant4160 { get; set; }
        public int Cant6180 { get; set; }
        public int Cant2140 { get; set; }
        public int Cant80 { get; set; }
        public int Cant020 { get; set; }
        public int cantAtendidosYearEnero { get; set; }
        public int cantAtendidosYearFebrero { get; set; }
        public int cantAtendidosYearMarzo { get; set; }
        public int cantAtendidosYearAbril { get; set; }
        public int cantAtendidosYearMayo { get; set; }
        public int cantAtendidosYearJunio { get; set; }
        public int cantAtendidosYearJulio { get; set; }
        public int cantAtendidosYearAgosto { get; set; }
        public int cantAtendidosYearSeptiembre { get; set; }
        public int cantAtendidosYearOctubre { get; set; }
        public int cantAtendidosYearNoviembre { get; set; }
        public int cantAtendidosYearDiciembre { get; set; }
        public int SumatoriaYear { get; set; }

        public int cantAtendidosYearEneroGeneral { get; set; }
        public int cantAtendidosYearFebreroGeneral { get; set; }
        public int cantAtendidosYearMarzoGeneral { get; set; }
        public int cantAtendidosYearAbrilGeneral { get; set; }
        public int cantAtendidosYearMayoGeneral { get; set; }
        public int cantAtendidosYearJunioGeneral { get; set; }
        public int cantAtendidosYearJulioGeneral { get; set; }
        public int cantAtendidosYearAgostoGeneral { get; set; }
        public int cantAtendidosYearSeptiembreGeneral { get; set; }
        public int cantAtendidosYearOctubreGeneral { get; set; }
        public int cantAtendidosYearNoviembreGeneral { get; set; }
        public int cantAtendidosYearDiciembreGeneral { get; set; }
        public int SumatoriaYearGeneral { get; set; }


        public double cantAtendidosYearEneroPorcentual { get; set; }
        public double cantAtendidosYearFebreroPorcentual { get; set; }
        public double cantAtendidosYearMarzoPorcentual { get; set; }
        public double cantAtendidosYearAbrilPorcentual { get; set; }
        public double cantAtendidosYearMayoPorcentual { get; set; }
        public double cantAtendidosYearJunioPorcentual { get; set; }
        public double cantAtendidosYearJulioPorcentual { get; set; }
        public double cantAtendidosYearAgostoPorcentual { get; set; }
        public double cantAtendidosYearSeptiembrePorcentual { get; set; }
        public double cantAtendidosYearOctubrePorcentual { get; set; }
        public double cantAtendidosYearNoviembrePorcentual { get; set; }
        public double cantAtendidosYearDiciembrePorcentual { get; set; }

        public double cantAtendidosYearPromedio { get; set; }

        public string UserGenera { get; set; }

        public int cantAtendidosYearEneroMG { get; set; }
        public int cantAtendidosYearFebreroMG { get; set; }
        public int cantAtendidosYearMarzoMG { get; set; }
        public int cantAtendidosYearAbrilMG { get; set; }
        public int cantAtendidosYearMayoMG { get; set; }
        public int cantAtendidosYearJunioMG { get; set; }
        public int cantAtendidosYearJulioMG { get; set; }
        public int cantAtendidosYearAgostoMG { get; set; }
        public int cantAtendidosYearSeptiembreMG { get; set; }
        public int cantAtendidosYearOctubreMG { get; set; }
        public int cantAtendidosYearNoviembreMG { get; set; }
        public int cantAtendidosYearDiciembreMG { get; set; }
        public int SumatoriaYearMG { get; set; }

        public int CantPacEneroCU { get; set; }
        public int CantPacFebreroCU { get; set; }
        public int CantPacMarzoCU { get; set; }
        public int CantPacAbrilCU { get; set; }
        public int CantPacMayoCU { get; set; }
        public int CantPacJunioCU { get; set; }
        public int CantPacJulioCU { get; set; }
        public int CantPacAgostoCU { get; set; }
        public int CantPacSeptiembreCU { get; set; }
        public int CantPacOctubreCU { get; set; }
        public int CantPacNoviembreCU { get; set; }
        public int CantPacDiciembreCU { get; set; }

        public int SumatoriaPacsYear { get; set; }
        public double PromedioCurByPac { get; set; }

        public byte[] Grafico1 { get; set; }
        public byte[] Grafico2 { get; set; }
        public byte[] Grafico3 { get; set; }
        public byte[] Grafico4 { get; set; }

    }

    public class ComplementoInformePatologias
    {
        public int CantPatologiaEneroPOP { get; set; }
        public int CantPatologiaFebreroPOP { get; set; }
        public int CantPatologiaMarzoPOP { get; set; }
        public int CantPatologiaAbrilPOP { get; set; }
        public int CantPatologiaMayoPOP { get; set; }
        public int CantPatologiaJunioPOP { get; set; }
        public int CantPatologiaJulioPOP { get; set; }
        public int CantPatologiaAgostoPOP { get; set; }
        public int CantPatologiaSeptiembrePOP { get; set; }
        public int CantPatologiaOctubrePOP { get; set; }
        public int CantPatologiaNoviembrePOP { get; set; }
        public int CantPatologiaDiciembrePOP { get; set; }


        public int CantPatologiaEneroZP { get; set; }
        public int CantPatologiaFebreroZP { get; set; }
        public int CantPatologiaMarzoZP { get; set; }
        public int CantPatologiaAbrilZP { get; set; }
        public int CantPatologiaMayoZP { get; set; }
        public int CantPatologiaJunioZP { get; set; }
        public int CantPatologiaJulioZP { get; set; }
        public int CantPatologiaAgostoZP { get; set; }
        public int CantPatologiaSeptiembreZP { get; set; }
        public int CantPatologiaOctubreZP { get; set; }
        public int CantPatologiaNoviembreZP { get; set; }
        public int CantPatologiaDiciembreZP { get; set; }


        public int CantPatologiaEneroPD { get; set; }
        public int CantPatologiaFebreroPD { get; set; }
        public int CantPatologiaMarzoPD { get; set; }
        public int CantPatologiaAbrilPD { get; set; }
        public int CantPatologiaMayoPD { get; set; }
        public int CantPatologiaJunioPD { get; set; }
        public int CantPatologiaJulioPD { get; set; }
        public int CantPatologiaAgostoPD { get; set; }
        public int CantPatologiaSeptiembrePD { get; set; }
        public int CantPatologiaOctubrePD { get; set; }
        public int CantPatologiaNoviembrePD { get; set; }
        public int CantPatologiaDiciembrePD { get; set; }


        public int CantPatologiaEneroQX { get; set; }
        public int CantPatologiaFebreroQX { get; set; }
        public int CantPatologiaMarzoQX { get; set; }
        public int CantPatologiaAbrilQX { get; set; }
        public int CantPatologiaMayoQX { get; set; }
        public int CantPatologiaJunioQX { get; set; }
        public int CantPatologiaJulioQX { get; set; }
        public int CantPatologiaAgostoQX { get; set; }
        public int CantPatologiaSeptiembreQX { get; set; }
        public int CantPatologiaOctubreQX { get; set; }
        public int CantPatologiaNoviembreQX { get; set; }
        public int CantPatologiaDiciembreQX { get; set; }


        public int CantPatologiaEneroTX { get; set; }
        public int CantPatologiaFebreroTX { get; set; }
        public int CantPatologiaMarzoTX { get; set; }
        public int CantPatologiaAbrilTX { get; set; }
        public int CantPatologiaMayoTX { get; set; }
        public int CantPatologiaJunioTX { get; set; }
        public int CantPatologiaJulioTX { get; set; }
        public int CantPatologiaAgostoTX { get; set; }
        public int CantPatologiaSeptiembreTX { get; set; }
        public int CantPatologiaOctubreTX { get; set; }
        public int CantPatologiaNoviembreTX { get; set; }
        public int CantPatologiaDiciembreTX { get; set; }


        public int CantPatologiaEneroUV { get; set; }
        public int CantPatologiaFebreroUV { get; set; }
        public int CantPatologiaMarzoUV { get; set; }
        public int CantPatologiaAbrilUV { get; set; }
        public int CantPatologiaMayoUV { get; set; }
        public int CantPatologiaJunioUV { get; set; }
        public int CantPatologiaJulioUV { get; set; }
        public int CantPatologiaAgostoUV { get; set; }
        public int CantPatologiaSeptiembreUV { get; set; }
        public int CantPatologiaOctubreUV { get; set; }
        public int CantPatologiaNoviembreUV { get; set; }
        public int CantPatologiaDiciembreUV { get; set; }


        public int CantPatologiaEneroOtros { get; set; }
        public int CantPatologiaFebreroOtros { get; set; }
        public int CantPatologiaMarzoOtros { get; set; }
        public int CantPatologiaAbrilOtros { get; set; }
        public int CantPatologiaMayoOtros { get; set; }
        public int CantPatologiaJunioOtros { get; set; }
        public int CantPatologiaJulioOtros { get; set; }
        public int CantPatologiaAgostoOtros { get; set; }
        public int CantPatologiaSeptiembreOtros { get; set; }
        public int CantPatologiaOctubreOtros { get; set; }
        public int CantPatologiaNoviembreOtros { get; set; }
        public int CantPatologiaDiciembreOtros { get; set; }


        public int TotalPatologiaAnualVascular { get; set; }
        public int TotalPatologiaAnualPos { get; set; }
        public int TotalPatologiaAnualZP { get; set; }
        public int TotalPatologiaAnualTX { get; set; }
        public int TotalPatologiaAnualQX { get; set; }
        public int TotalPatologiaAnualOtros { get; set; }
        public int TotalPatologiaAnualPD { get; set; }
        public int TotalAñoTodasPatologiasH { get; set; }

        public int TotalPatologiaAnualEneroVertical { get; set; }
        public int TotalPatologiaAnualFebreroVertical { get; set; }
        public int TotalPatologiaAnualMarzoVertical { get; set; }
        public int TotalPatologiaAnualAbrilVertical { get; set; }
        public int TotalPatologiaAnualMayoVertical { get; set; }
        public int TotalPatologiaAnualJunioVertical { get; set; }
        public int TotalPatologiaAnualJulioVertical { get; set; }
        public int TotalPatologiaAnualAgostoVertical { get; set; }
        public int TotalPatologiaAnualSeptiembreVertical { get; set; }
        public int TotalPatologiaAnualOctubreVertical { get; set; }
        public int TotalPatologiaAnualNoviembreVertical { get; set; }
        public int TotalPatologiaAnualDiciembreVertical { get; set; }


        public double TotalEneroPatologiaPOPVertical { get; set; }
        public double TotalEneroPatologiaZPVertical { get; set; }
        public double TotalEneroPatologiaPDVertical { get; set; }
        public double TotalEneroPatologiaQXVertical { get; set; }
        public double TotalEneroPatologiaTXVertical { get; set; }
        public double TotalEneroPatologiaUVVertical { get; set; }
        public double TotalEneroPatologiaOtrosVertical { get; set; }
        
        public double TotalFebreroPatologiaPOPVertical { get; set; }
        public double TotalFebreroPatologiaZPVertical { get; set; }
        public double TotalFebreroPatologiaPDVertical { get; set; }
        public double TotalFebreroPatologiaQXVertical { get; set; }
        public double TotalFebreroPatologiaTXVertical { get; set; }
        public double TotalFebreroPatologiaUVVertical { get; set; }
        public double TotalFebreroPatologiaOtrosVertical { get; set; }
        
        public double TotalMarzoPatologiaPOPVertical { get; set; }
        public double TotalMarzoPatologiaZPVertical { get; set; }
        public double TotalMarzoPatologiaPDVertical { get; set; }
        public double TotalMarzoPatologiaQXVertical { get; set; }
        public double TotalMarzoPatologiaTXVertical { get; set; }
        public double TotalMarzoPatologiaUVVertical { get; set; }
        public double TotalMarzoPatologiaOtrosVertical { get; set; }

        public double TotalAbrilPatologiaPOPVertical { get; set; }
        public double TotalAbrilPatologiaZPVertical { get; set; }
        public double TotalAbrilPatologiaPDVertical { get; set; }
        public double TotalAbrilPatologiaQXVertical { get; set; }
        public double TotalAbrilPatologiaTXVertical { get; set; }
        public double TotalAbrilPatologiaUVVertical { get; set; }
        public double TotalAbrilPatologiaOtrosVertical { get; set; }

        public double TotalMayoPatologiaPOPVertical { get; set; }
        public double TotalMayoPatologiaZPVertical { get; set; }
        public double TotalMayoPatologiaPDVertical { get; set; }
        public double TotalMayoPatologiaQXVertical { get; set; }
        public double TotalMayoPatologiaTXVertical { get; set; }
        public double TotalMayoPatologiaUVVertical { get; set; }
        public double TotalMayoPatologiaOtrosVertical { get; set; }

        public double TotalJunioPatologiaPOPVertical { get; set; }
        public double TotalJunioPatologiaZPVertical { get; set; }
        public double TotalJunioPatologiaPDVertical { get; set; }
        public double TotalJunioPatologiaQXVertical { get; set; }
        public double TotalJunioPatologiaTXVertical { get; set; }
        public double TotalJunioPatologiaUVVertical { get; set; }
        public double TotalJunioPatologiaOtrosVertical { get; set; }

        public double TotalJulioPatologiaPOPVertical { get; set; }
        public double TotalJulioPatologiaZPVertical { get; set; }
        public double TotalJulioPatologiaPDVertical { get; set; }
        public double TotalJulioPatologiaQXVertical { get; set; }
        public double TotalJulioPatologiaTXVertical { get; set; }
        public double TotalJulioPatologiaUVVertical { get; set; }
        public double TotalJulioPatologiaOtrosVertical { get; set; }

        public double TotalAgostoPatologiaPOPVertical { get; set; }
        public double TotalAgostoPatologiaZPVertical { get; set; }
        public double TotalAgostoPatologiaPDVertical { get; set; }
        public double TotalAgostoPatologiaQXVertical { get; set; }
        public double TotalAgostoPatologiaTXVertical { get; set; }
        public double TotalAgostoPatologiaUVVertical { get; set; }
        public double TotalAgostoPatologiaOtrosVertical { get; set; }

        public double TotalSeptiembrePatologiaPOPVertical { get; set; }
        public double TotalSeptiembrePatologiaZPVertical { get; set; }
        public double TotalSeptiembrePatologiaPDVertical { get; set; }
        public double TotalSeptiembrePatologiaQXVertical { get; set; }
        public double TotalSeptiembrePatologiaTXVertical { get; set; }
        public double TotalSeptiembrePatologiaUVVertical { get; set; }
        public double TotalSeptiembrePatologiaOtrosVertical { get; set; }

        public double TotalOctubrePatologiaPOPVertical { get; set; }
        public double TotalOctubrePatologiaZPVertical { get; set; }
        public double TotalOctubrePatologiaPDVertical { get; set; }
        public double TotalOctubrePatologiaQXVertical { get; set; }
        public double TotalOctubrePatologiaTXVertical { get; set; }
        public double TotalOctubrePatologiaUVVertical { get; set; }
        public double TotalOctubrePatologiaOtrosVertical { get; set; }

        public double TotalNoviembrePatologiaPOPVertical { get; set; }
        public double TotalNoviembrePatologiaZPVertical { get; set; }
        public double TotalNoviembrePatologiaPDVertical { get; set; }
        public double TotalNoviembrePatologiaQXVertical { get; set; }
        public double TotalNoviembrePatologiaTXVertical { get; set; }
        public double TotalNoviembrePatologiaUVVertical { get; set; }
        public double TotalNoviembrePatologiaOtrosVertical { get; set; }

        public double TotalDiciembrePatologiaPOPVertical { get; set; }
        public double TotalDiciembrePatologiaZPVertical { get; set; }
        public double TotalDiciembrePatologiaPDVertical { get; set; }
        public double TotalDiciembrePatologiaQXVertical { get; set; }
        public double TotalDiciembrePatologiaTXVertical { get; set; }
        public double TotalDiciembrePatologiaUVVertical { get; set; }
        public double TotalDiciembrePatologiaOtrosVertical { get; set; }

        public int TOTALPATOLOGIASANUAL { get; set; }

        public double TOTALPATOLOGIASPOPHORIZONTAL { get; set; }
        public double TOTALPATOLOGIASZPHORIZONTAL { get; set; }
        public double TOTALPATOLOGIASPDHORIZONTAL { get; set; }
        public double TOTALPATOLOGIASQXHORIZONTAL { get; set; }
        public double TOTALPATOLOGIASTXHORIZONTAL { get; set; }
        public double TOTALPATOLOGIASUVHORIZONTAL { get; set; }
        public double TOTALPATOLOGIASOTROSHORIZONTAL { get; set; }
    }
}
