using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ZamenisHealth.Clases.Controles
{
    public class PanelCSS : Panel
    {
        public int RadioEsquinas { get; set; } = 18;
        public int IntensidadSombra { get; set; } = 12;
        public int MargenSombra { get; set; } = 14;

        public PanelCSS()
        {
            SetStyle(
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw |
                ControlStyles.SupportsTransparentBackColor,
                true);

            BackColor = Color.Transparent;
            BorderStyle = BorderStyle.None;
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);

            Graphics g = e.Graphics;

            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.CompositingQuality = CompositingQuality.HighQuality;

            Rectangle rect = new Rectangle(
                MargenSombra,
                MargenSombra,
                Width - (MargenSombra * 2),
                Height - (MargenSombra * 2)
            );

            using (GraphicsPath path = CrearPath(rect, RadioEsquinas))
            {
                // Sombra difuminada
                for (int i = IntensidadSombra; i >= 1; i--)
                {
                    int alpha = Math.Max(1, 18 - i);

                    using (Pen sombra = new Pen(
                        Color.FromArgb(alpha, 0, 0, 0),
                        i * 2))
                    {
                        sombra.LineJoin = LineJoin.Round;
                        g.DrawPath(sombra, path);
                    }
                }

                // Fondo de la tarjeta
                using (SolidBrush fondo = new SolidBrush(BackColor == Color.Transparent
                    ? Color.White
                    : BackColor))
                {
                    g.FillPath(fondo, path);
                }
            }
        }
        private GraphicsPath CrearPath(Rectangle rect, int radio)
        {
            GraphicsPath path = new GraphicsPath();

            int diametro = radio * 2;

            path.AddArc(rect.X, rect.Y, diametro, diametro, 180, 90);

            path.AddArc(
                rect.Right - diametro,
                rect.Y,
                diametro,
                diametro,
                270,
                90);

            path.AddArc(
                rect.Right - diametro,
                rect.Bottom - diametro,
                diametro,
                diametro,
                0,
                90);

            path.AddArc(
                rect.X,
                rect.Bottom - diametro,
                diametro,
                diametro,
                90,
                90);

            path.CloseFigure();

            return path;
        }
    }
}
