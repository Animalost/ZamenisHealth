using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ZamenisHealth.Clases.Controles
{
    public static class UIUtils
    {
        public static void ApplyRoundedCorners(Control c, int radius)
        {
            var path = new GraphicsPath();
            path.AddArc(0, 0, radius, radius, 180, 90);
            path.AddArc(c.Width - radius, 0, radius, radius, 270, 90);
            path.AddArc(c.Width - radius, c.Height - radius, radius, radius, 0, 90);
            path.AddArc(0, c.Height - radius, radius, radius, 90, 90);
            path.CloseFigure();
            c.Region = new Region(path);
        }
    }
}
