using FormAndControls;
using Persistence;
using System;
using System.Windows.Forms;

namespace ZamenisHealth.FrontFHIR.VisorZamenis
{
    public partial class VisorPDF : Forma2
    {
        string Datos, LimpioB64;

        public VisorPDF(string datos, string limpioB64)
        {
            InitializeComponent();
            this.Datos = datos;
            this.LimpioB64 = limpioB64;
        }

        private async void VisorPDF_Load(object sender, EventArgs e)
        {
            try
            {
                var webView21 = new Microsoft.Web.WebView2.WinForms.WebView2();
                webView21.Dock = DockStyle.Fill;
                webView21.Cursor = Cursors.No;

                this.Controls.Add(webView21);

                await webView21.EnsureCoreWebView2Async();

                webView21.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
                webView21.CoreWebView2.Settings.AreBrowserAcceleratorKeysEnabled = false;
                webView21.CoreWebView2.Settings.AreDevToolsEnabled = false;
                webView21.CoreWebView2.Settings.IsStatusBarEnabled = false;

                webView21.CoreWebView2.DownloadStarting += (senderr, er) =>
                {
                    er.Cancel = true;
                };

             /*   webView21.CoreWebView2.NavigationCompleted += async (s, ers) =>
                {
                                await webView21.CoreWebView2.ExecuteScriptAsync(@"
                                    document.body.style.userSelect = 'none';
                                    document.body.style.pointerEvents = 'none';
                                ");
                };*/

                await webView21.CoreWebView2.ExecuteScriptAsync(
                    @"document.body.style.userSelect = 'none';"
                );

                Titulo.Text = "Visor PDF - FHIR";
                SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";
                toolStrip1.SendToBack();

                webView21.Cursor = Cursors.No;
                webView21.NavigateToString(Datos);

                webView21.KeyDown += WebView21_KeyDown;
                webView21.MouseDown += WebView21_MouseDown;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }            
        }
        private void WebView21_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                return;
            }
        }
        private void WebView21_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && (e.KeyCode == Keys.C || e.KeyCode == Keys.V || e.KeyCode == Keys.X || e.KeyCode == Keys.P || e.KeyCode == Keys.F12))
            {
                e.SuppressKeyPress = true;
                e.Handled = true;
            }
        }
    }
}
