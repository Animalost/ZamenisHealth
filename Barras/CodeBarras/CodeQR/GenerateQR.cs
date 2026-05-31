using System.Drawing;
using ZXing;

namespace CodeBarras.CodeQR
{
    public class GenerateQR
    {
        public static Bitmap GenerateQRCode(string content)
        {
            BarcodeWriter barcodeWriter = new BarcodeWriter();
            barcodeWriter.Format = BarcodeFormat.QR_CODE;
            barcodeWriter.Options = new ZXing.Common.EncodingOptions
            {
                Width = 300,
                Height = 300
            };

            Bitmap bitmap = barcodeWriter.Write(content);
            //bitmap.Save(filePath);
            return bitmap;
        }
    }
}
