using System.Drawing;
using Tulpep.NotificationWindow;

namespace ZamenisHealth
{
    public class PopUps
    {
        public static PopupNotifier setPopUp(Image imagen, Color colorfondo, string titulo, Color colortitulo, string contenido)
        {
            PopupNotifier popupNotifier = new PopupNotifier();
            popupNotifier.Image = imagen;
            popupNotifier.ImageSize = new Size(40, 40);
            popupNotifier.BodyColor = colorfondo;
            popupNotifier.TitleText = titulo;
            popupNotifier.TitleColor = colortitulo;
            popupNotifier.TitleFont = new Font("Arial Black", 20, FontStyle.Bold);

            popupNotifier.ContentText = contenido;
            popupNotifier.ContentColor = Color.Black;
            popupNotifier.TitleFont = new Font("Arial", 15, FontStyle.Regular);

            return popupNotifier;
        }     
    }
}
