using System;
using System.Drawing;
using System.Windows.Forms;

namespace ZamenisHealth.Clases.Controles
{
    public class TooltipEnriquecido : Form
    {
        private readonly RichTextBox richTextBox;

        public TooltipEnriquecido()
        {
            FormBorderStyle = FormBorderStyle.None;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.Manual;
            TopMost = true;
            BackColor = Color.White;
            Padding = new Padding(6);
            AutoSize = false;
            Size = new Size(600, 180);
            BackColor = Color.LightGray;

            richTextBox = new RichTextBox
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                BorderStyle = BorderStyle.None,
                BackColor = Color.WhiteSmoke,
                Font = new Font("Segoe UI", 9F),
                DetectUrls = false,
                TabStop = false,
                ScrollBars = RichTextBoxScrollBars.None
            };

            Controls.Add(richTextBox);
        }

        public void AgregarLinea(string etiqueta, string valor)
        {
            richTextBox.SelectionStart = richTextBox.TextLength;
            richTextBox.SelectionLength = 0;

            // Etiqueta en azul y negrita.
            richTextBox.SelectionColor = Color.RoyalBlue;
            richTextBox.SelectionFont = new Font(
                richTextBox.Font,
                FontStyle.Bold);

            richTextBox.AppendText(etiqueta);

            // Valor en negro y normal.
            richTextBox.SelectionColor = Color.Black;
            richTextBox.SelectionFont = new Font(
                richTextBox.Font,
                FontStyle.Regular);

            richTextBox.AppendText(" " + (valor ?? "") + Environment.NewLine);
        }
        public void MostrarEn(Point posicion)
        {
            richTextBox.SelectionStart = 0;
            richTextBox.SelectionLength = 0;

            richTextBox.SelectAll();
            richTextBox.SelectionLength = 0;

            richTextBox.Height = richTextBox.GetPositionFromCharIndex(
                Math.Max(0, richTextBox.TextLength - 1)).Y + 25;

            Height = Math.Max(50, richTextBox.Height + Padding.Vertical);
            Width = 320;

            Location = posicion;
            Show();
        }
    }
}
