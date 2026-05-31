using System.Linq;
using System.Windows.Forms;

using ZamenisHealth.Comunes;
using ZamenisHealth.Tipificador;

namespace ZamenisHealth.Clases
{
    public class CloseAppInactivity : IMessageFilter
    {
        public bool PreFilterMessage(ref Message m)
        {
            const int WM_KEYDOWN = 0x0100;

            const int WM_LBUTTONDOWN = 0x0201;
            const int WM_RBUTTONDOWN = 0x0204;
            const int WM_MOUSEWHEEL = 0x020A;

            // 🔥 Detectar actividad general (teclado + mouse)
            if (m.Msg == WM_KEYDOWN ||
                m.Msg == WM_LBUTTONDOWN ||
                m.Msg == WM_RBUTTONDOWN ||
                m.Msg == WM_MOUSEWHEEL)
            {
                Inactividad._counter = 0;
            }

            if (m.Msg == WM_KEYDOWN)
            {
                Keys key = (Keys)m.WParam | Control.ModifierKeys;

                if (key == (Keys.Shift | Keys.F12))
                {
                    AbrirFormularioEspecial();
                    return true;
                }
                if (key == (Keys.Shift | Keys.F10))
                {
                    AbrirFormularioTipificador();
                    return true;
                }
            }

            return false;
        }

        private void AbrirFormularioEspecial()
        {
            // Evitar abrir múltiples instancias
            var frm = Application.OpenForms.OfType<MensajeroSend>().FirstOrDefault();

            if (frm == null)
            {
                frm = new MensajeroSend();
                frm.Show();
            }
            else
            {
                frm.BringToFront();
            }
        }

        private void AbrirFormularioTipificador()
        {
            // Evitar abrir múltiples instancias
            var frm = Application.OpenForms.OfType<Ingreso>().FirstOrDefault();

            if (frm == null)
            {
                frm = new Ingreso();
                frm.Show();
            }
            else
            {
                frm.BringToFront();
            }
        }
    }
}
