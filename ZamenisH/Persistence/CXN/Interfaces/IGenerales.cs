using Domain.CXN;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Persistence.CXN.Interfaces
{
    public interface IGenerales
    {
        Dictionary<string, string> SugerenciaServicio(int Paciente);
        //int getCodeRegimen(string Regimen);
        bool ValidarNumerico(string Dato);
        System.Drawing.Image CodifyQR(string T_Codifica);
        byte[] GetBytes(System.Drawing.Image imageIn);
        string GetMD5(string str);
        string Base64Decode(string base64EncodedData);
        string Base64Encode(string plainText);
        Bitmap ByteToImage(byte[] blob);
        void ExportarGrilla(DataGridView dataGridX);
        List<CXN_DISCAPACIDAD> ListaDiscapacidades();
        string GetCodeDiscapacidad(string NameDiscapacidad);
        List<CXN_ETNIA> ListaEtnias();
        string GetCodeEtnia(string NameEtnia);
    }
}
