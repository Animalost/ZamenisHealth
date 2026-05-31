using Domain;
using Microsoft.Reporting.WinForms;
using Persistence;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.Clases
{
    public static class ConfigForm
    {       
        //Mover Formularios
        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        public extern static void ReleaseCapturing();
        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        public extern static void SendMessageMove(System.IntPtr hwnd, int wmsg, int wparam, int lparam);

        //Redimensionar Control
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(int nLeftRect, 
                                                        int nTopRect, 
                                                        int nRightRect, 
                                                        int nBottomRect, 
                                                        int nWidthEllipse, 
                                                        int nHeightEllipse);
        
        //Variables Llevaderas de datos para el MDIForm

        public static int FacZamenisMDI;
        public static int PosGrillaMDI;
        public static int CompañiaMDI;
        public static string TextoMDI;

        public class BaseForm : Form
        {
            private string tipoSonido;
            public Label Titulo;
            private ToolTip toolTitulo;
            private ToolTip toolCloseAppImage;
            public PictureBox ImageClose;

            public BaseForm()
            {
                this.FormBorderStyle = FormBorderStyle.None;
                this.StartPosition = FormStartPosition.CenterScreen;
                this.BackColor = Color.White;
                this.KeyPreview = true;
                this.DoubleBuffered = true;
                this.AutoScroll = false;
                this.ControlBox = false;
                this.Text = "";
                this.ShowInTaskbar = true;
                this.Icon = Properties.Resources2.estetoscopio;                

                MoverForma();
                this.Load += BaseForm_Load;
            }

            protected override void OnResize(EventArgs e)
            {
                var path = GetRoundedRectPath(this.ClientRectangle, 25);
                this.Region = new Region(path);
                base.OnResize(e);
            }

            private GraphicsPath GetRoundedRectPath(Rectangle rect, int radius)
            {
                GraphicsPath path = new GraphicsPath();
                int d = radius * 2;

                path.AddArc(rect.X, rect.Y, d, d, 180, 90);
                path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
                path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
                path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);

                path.CloseFigure();
                return path;
            }            

            async Task Sonar(string TipoSonido)
            {
                if (TipoSonido == "ToolStrip")
                {
                    await Task.Run(() =>
                    {
                        Program.soundPlayer.Play(); // Si Play es sincrónico, lo envuelve en un Task
                    });
                }
                else if (TipoSonido == "Button")
                {
                    await Task.Run(() =>
                    {
                        Program.soundButton.Play(); 
                    });
                }
            }
            private async void EmularSonido(object s, EventArgs e)
            {
                if (Preferencias.AppSound == "A")
                {
                    await Sonar(this.tipoSonido);
                }                
            }

            public int getMonthNumber(string Month)
            {
                switch (Month)
                {
                    case "ENERO":
                        return 01;
                    case "FEBRERO":
                        return 02;
                    case "MARZO":
                        return 03;
                    case "ABRIL":
                        return 04;
                    case "MAYO":
                        return 05;
                    case "JUNIO":
                        return 06;
                    case "JULIO":
                        return 07;
                    case "AGOSTO":
                        return 08;
                    case "SEPTIEMBRE":
                        return 09;
                    case "OCTUBRE":
                        return 10;
                    case "NOVIEMBRE":
                        return 11;
                    case "DICIEMBRE":
                        return 12;
                    default:
                        return 0;
                }
            }

            public int getMonthLastDay(string Month)
            {
                switch (Month)
                {
                    case "ENERO":
                        return 31;
                    case "FEBRERO":
                        return 28;
                    case "MARZO":
                        return 31;
                    case "ABRIL":
                        return 30;
                    case "MAYO":
                        return 31;
                    case "JUNIO":
                        return 30;
                    case "JULIO":
                        return 31;
                    case "AGOSTO":
                        return 31;
                    case "SEPTIEMBRE":
                        return 30;
                    case "OCTUBRE":
                        return 31;
                    case "NOVIEMBRE":
                        return 30;
                    case "DICIEMBRE":
                        return 31;
                    default:
                        return 0;
                }
            }

            private void BaseForm_Load(object sender, EventArgs e)
            {
                toolTitulo = new ToolTip();
                toolTitulo.ShowAlways = true;
                toolCloseAppImage = new ToolTip();
                toolCloseAppImage.ShowAlways = true;

                Titulo = new Label();
                //Titulo.Text = "";
                Titulo.Location = new Point(0, 0);
                Titulo.Size = new Size(this.Size.Width, 40);
                Titulo.Anchor = AnchorStyles.Left;
                Titulo.Anchor = AnchorStyles.Right;
                Titulo.Anchor = AnchorStyles.Top;
                Titulo.BackColor = Color.RoyalBlue;
                Titulo.Font = new Font("Arial", 12, FontStyle.Regular);
                Titulo.ForeColor = Color.White;
                Titulo.Padding = new Padding(15, 0, 0, 0);
                Titulo.TextAlign = ContentAlignment.MiddleLeft;
                toolTitulo.SetToolTip(Titulo, "Mantenga presionado el boton izquierdo del mouse para mover la ventana");
                this.Controls.Add(Titulo);
                Titulo.BringToFront();
                Titulo.MouseDown += MoverForma;

                ImageClose = new PictureBox();
                ImageClose.SizeMode = PictureBoxSizeMode.StretchImage;
                ImageClose.Image = Properties.Resources.cerca;
                ImageClose.BackColor = Color.RoyalBlue;
                ImageClose.Size = new Size(34, 34);
                ImageClose.Location = new Point(Titulo.Size.Width - 45, 2);
                ImageClose.Anchor = AnchorStyles.Right;
                ImageClose.Anchor = AnchorStyles.Top;
                ImageClose.Cursor = Cursors.Hand;
                toolCloseAppImage.SetToolTip(ImageClose, "Cerrar este formulario");
                this.Controls.Add(ImageClose);
                ImageClose.BringToFront();
                ImageClose.Click += ImageClose_Click;

                TransformarControl();
            }
            private void MoverForma(object sender, MouseEventArgs e)
            {
                ReleaseCapturing();
                SendMessageMove(this.Handle, 0x112, 0xf012, 0);
            }
            private void ImageClose_Click(object sender, EventArgs e)
            {
                this.Dispose();
                this.Close();
            }
            private void BloquearRuedaMouse(ComboBox C)
            {
                C.MouseWheel += (sender, e) =>
                {
                    ((HandledMouseEventArgs)e).Handled = true;
                };
            }

            public void ExpandeContraer(Label L, Panel P, int contrae, int Expande)
            {
                if (L.Text == "Contraer ^^")
                {
                    P.Height = contrae;
                    L.Text = "Expandir VV";
                    L.BackColor = Color.FromArgb(128, 255, 128);
                    return;
                }

                if (L.Text == "Expandir VV")
                {
                    P.Height = Expande;
                    L.Text = "Contraer ^^";
                    L.BackColor = Color.DarkGray;
                    return;
                }
            }

            public void MoverForma()
            {
                this.MouseDown += (sender, e) =>
                {
                    ReleaseCapturing();
                    SendMessageMove(this.Handle, 0x112, 0xf012, 0);
                };
            }            
            public void TransformarControl()
            {
                foreach (Control control in this.Controls)
                {
                    if (control is ToolStrip toolStrip)
                    {
                        foreach (ToolStripItem item in toolStrip.Items)
                        {
                            if (item is ToolStripButton toolStripButton)
                            {
                                this.tipoSonido = "ToolStrip";                                
                                toolStripButton.Click += EmularSonido;
                            }
                        }
                    }
                    else if (control is TextBox textBox)
                    {
                        textBox.TextChanged += TextBox_TextChanged;
                    }
                    else if(control is Button button)
                    {
                        this.tipoSonido = "Button";
                        PersonalizarBoton(button);
                        button.Click += EmularSonido;
                    }
                    else if(control is ComboBox comboBox)
                    {
                        BloquearRuedaMouse(comboBox);
                        PersonalizarCombo(comboBox);
                    }
                    // Si hay controles anidados, llamamos un método recursivo
                    RegistrarEventosEnControlesAnidados(control);
                }
            }
            private void PersonalizarBoton(Button button)
            {
                button.FlatStyle = FlatStyle.Flat; 
                button.BackColor = Color.RoyalBlue; 
                button.FlatAppearance.BorderSize = 0; 
                button.FlatAppearance.BorderColor = Color.RoyalBlue;
                button.Font = new Font("Arial", 9, FontStyle.Bold); 
                button.ForeColor = Color.White;
                button.Cursor = Cursors.Hand;

                button.MouseEnter += (s, e) => button.BackColor = Color.LightBlue;
                button.MouseLeave += (s, e) => button.BackColor = Color.RoyalBlue;

                button.Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, button.Width + 1, button.Height + 1, 10, 10));

                using (Graphics g = button.CreateGraphics())
                {
                    using (Pen borderPen = new Pen(Color.RoyalBlue, 0))
                    {
                        g.DrawRectangle(borderPen, 0, 0, button.Width + 1, button.Height + 1);
                    }
                }
            }
            private void PersonalizarCombo(ComboBox combo)
            {
                combo.BackColor = Color.LightBlue;
                combo.Font = new Font("Arial", 10, FontStyle.Regular);
                combo.ForeColor = Color.DarkBlue;
                combo.Cursor = Cursors.Hand;
                combo.DropDownStyle = ComboBoxStyle.DropDownList;
            }
            private void TextBox_TextChanged(object sender, EventArgs e)
            {
                if (sender is TextBox textBox)
                {
                    if (string.IsNullOrEmpty(textBox.Text))
                    {
                        textBox.BackColor = Color.White;
                    }
                    else
                    {
                        textBox.BackColor = Color.Honeydew;
                    }
                }
            }
            private void RegistrarEventosEnControlesAnidados(Control parent)
            {
                foreach (Control control in parent.Controls)
                {
                    if (control is ToolStrip toolStrip)
                    {
                        foreach (ToolStripItem item in toolStrip.Items)
                        {
                            if (item is ToolStripButton toolStripButton)
                            {
                                toolStripButton.Click += EmularSonido;
                            }
                        }
                    }
                    else if (control is TextBox textBox)
                    {
                        textBox.TextChanged += TextBox_TextChanged;
                    }
                    else if (control is Button button)
                    {
                        PersonalizarBoton(button);
                        button.Click += EmularSonido;
                    }
                    else if (control is ComboBox comboBox)
                    {
                        BloquearRuedaMouse(comboBox);
                        PersonalizarCombo(comboBox);
                    }

                    // Llamada recursiva para buscar controles anidados
                    if (control.HasChildren)
                    {
                        RegistrarEventosEnControlesAnidados(control);
                    }
                }
            }
        }        

        public static void MoverForma(Label L, Form F)
        {
            L.MouseDown += (sender, e) =>
            {
                ReleaseCapturing();
                SendMessageMove(F.Handle, 0x112, 0xf012, 0);
            };
        }

        public static void MoverForma(Panel L, Form F)
        {
            L.MouseDown += (sender, e) =>
            {
                ReleaseCapturing();
                SendMessageMove(F.Handle, 0x112, 0xf012, 0);
            };
        }
        public static Control GraficarControl(Control ctrl, int borderWidth, Color borderColor)
        {
            if (ctrl is TextBox txt)
            {
                txt.BorderStyle = BorderStyle.None;
                txt.Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, txt.Width + 1, txt.Height + 1, 10, 10));

                using (Graphics g = txt.CreateGraphics())
                {
                    using (Pen borderPen = new Pen(borderColor, borderWidth))
                    {
                        g.DrawRectangle(borderPen, 0, 0, txt.Width - 1, txt.Height - 1);
                    }
                }

                ctrl = txt;
            }
            else if (ctrl is Button btn)
            {
                btn.Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, btn.Width + 1, btn.Height + 1, 10, 10));
                btn.BackColor = Color.RoyalBlue;
                btn.ForeColor = Color.White;
                btn.Font = new Font("Arial", 10);

                using (Graphics g = btn.CreateGraphics())
                {
                    using (Pen borderPen = new Pen(borderColor, borderWidth))
                    {
                        g.DrawRectangle(borderPen, 0, 0, btn.Width - 2, btn.Height - 2);
                    }
                }

                btn.MouseMove += (sender, e) =>
                {
                    btn.BackColor = Color.PaleTurquoise;
                    btn.ForeColor = Color.Black;
                    btn.Cursor = Cursors.Hand;
                };

                btn.MouseLeave += (sender, e) =>
                {
                    btn.BackColor = Color.RoyalBlue;
                    btn.ForeColor = Color.White;
                    btn.Cursor = Cursors.Hand;
                };

                ctrl = btn;
            }
            else if (ctrl is Label lbl)
            {
                lbl.Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(5, 5, lbl.Width - 5, lbl.Height - 5, 10, 10));

                using (Graphics g = lbl.CreateGraphics())
                {
                    using (Pen borderPen = new Pen(borderColor, borderWidth))
                    {
                        g.DrawRectangle(borderPen, 0, 0, lbl.Width - 1, lbl.Height - 1);
                    }
                }

                lbl.MouseMove += (sender, e) =>
                {
                    lbl.BackColor = Color.PaleTurquoise;
                    lbl.ForeColor = Color.Black;
                    lbl.Cursor = Cursors.Hand;
                };

                lbl.MouseLeave += (sender, e) =>
                {
                    lbl.BackColor = Color.RoyalBlue;
                    lbl.ForeColor = Color.White;
                    lbl.Cursor = Cursors.Hand;
                };

                ctrl = lbl;
            }            

            return ctrl;
        }
        public static ToolStripButton customToolstriplabel(ToolStripButton lbl)
        {            
            lbl.MouseMove += (sender, e) =>
            {
                lbl.BackColor = Color.PaleTurquoise;
                lbl.ForeColor = Color.Black;
            };

            lbl.MouseLeave += (sender, e) =>
            {
                lbl.BackColor = Color.RoyalBlue;
                lbl.ForeColor = Color.White;
            };

            return lbl;
        }        
        public static DataGridView colorGrid(DataGridView D)
        {
            try
            {
                foreach (DataGridViewRow row in D.Rows)
                {
                    int Numero = Convert.ToInt32(row.Cells["POS"].Value.ToString());

                    if ((Numero % 2) == 0)
                    {
                        row.DefaultCellStyle.BackColor = Color.Aquamarine;
                    }
                    else
                    {
                        row.DefaultCellStyle.BackColor = Color.MediumAquamarine;
                    }
                }

                return D;
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = "Clase colorGrid", Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
                return D;
            }            
        }
        public static async Task FadeInControl(PictureBox pictureBox)
        {
            try
            {
                int cTemp = pictureBox.Width;

                System.Drawing.Drawing2D.GraphicsPath gp = new System.Drawing.Drawing2D.GraphicsPath();
                gp.AddEllipse(0, 0, pictureBox.Width - 1, pictureBox.Height - 1);
                Region rg = new Region(gp);
                pictureBox.Region = rg;

                pictureBox.Width = 0;

                for (int i = 0; i <= cTemp; i = i + 2)
                {
                    pictureBox.Width = i;
                    await Task.Delay(10);
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = "Task - FadeInControl", Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        public static void GenerarReportViewer(string _dataSet, string _reporte, System.Collections.IEnumerable _clase)
        {
            try
            {
                Reportes.Maestro maestro = new Reportes.Maestro();
                maestro.Universal.LocalReport.DataSources.Clear();
                maestro.Universal.LocalReport.DataSources.Add(new ReportDataSource(_dataSet, _clase));
                maestro.Universal.LocalReport.ReportEmbeddedResource = _reporte;
                maestro.Universal.SetDisplayMode(DisplayMode.PrintLayout);
                maestro.Universal.ZoomMode = ZoomMode.Percent;
                maestro.Universal.ZoomPercent = 100;
                maestro.Universal.LocalReport.EnableExternalImages = true;
                maestro.Universal.Font = new Font("Arial", 8);
                maestro.Universal.RefreshReport();
                maestro.Universal.Visible = true;
                maestro.Universal.Dock = System.Windows.Forms.DockStyle.Fill;
                maestro.ShowDialog();
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = "GenerarReportViewer", Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
                MessageBox.Show(ex.Message);
            }
        }
        public static void SoloNumeros(TextBox T)
        {
            T.KeyPress += (sender, e) =>
            {
                if (!(char.IsNumber(e.KeyChar)) && (e.KeyChar != (char)Keys.Back))
                {
                    e.Handled = true;
                    return;
                }
            };            
        }

        public static void OpenIniFile()
        {
            string iniPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ZamenisIni", "config.ini");
            string folderPath = Path.GetDirectoryName(iniPath);

            if (Directory.Exists(folderPath))
            {
                System.Diagnostics.Process.Start("explorer.exe", folderPath);
            }
            else
            {
                MessageBox.Show("La carpeta no existe.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }    
}
