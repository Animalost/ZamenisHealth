using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ZamenisHealth.Clases.Controles
{
    public class ModernPanel : Panel
    {
        private int _borderRadius = 15;
        public int BorderRadius
        {
            get => _borderRadius;
            set { _borderRadius = value; Invalidate(); }
        }

        public bool Gradient { get; set; } = true;

        public ModernPanel()
        {
            DoubleBuffered = true;
            BackColor = Color.FromArgb(229, 235, 255);
            AutoScroll = true;
            //Cursor = Cursors.Hand;
            UIUtils.ApplyRoundedCorners(this, 15);
        }

        protected override void OnResize(EventArgs eventargs)
        {
            base.OnResize(eventargs);
            SetRoundedRegion();
        }

        private void SetRoundedRegion()
        {
            GraphicsPath path = GetRoundedPath(new Rectangle(0, 0, Width, Height), BorderRadius);
            this.Region = new Region(path);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            Rectangle rect = new Rectangle(0, 0, Width, Height);

            // Fondo
            if (Gradient)
            {
                using (LinearGradientBrush brush = new LinearGradientBrush(
                    rect,
                    Color.FromArgb(229, 235, 255),
                    Color.FromArgb(200, 200, 230),
                    90f))
                {
                    e.Graphics.FillPath(brush, GetRoundedPath(rect, BorderRadius));
                }
            }
            else
            {
                using (SolidBrush br = new SolidBrush(BackColor))
                    e.Graphics.FillPath(br, GetRoundedPath(rect, BorderRadius));
            }

            // Borde
            using (Pen p = new Pen(Color.FromArgb(25, 60, 100), 1.5f))
            {
                e.Graphics.DrawPath(p, GetRoundedPath(rect, BorderRadius));
            }
        }

        private GraphicsPath GetRoundedPath(Rectangle rect, int radius)
        {
            int d = radius * 2;
            GraphicsPath path = new GraphicsPath();

            path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
            path.CloseFigure();

            return path;
        }
    }
}
