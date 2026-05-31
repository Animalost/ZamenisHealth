using System;
using System.Linq;
using System.Windows.Forms;
using ZamenisHealth.Comunes;

namespace ZamenisHealth
{
    public class Inactividad
    {
        public static int _counter;

        public static void TimerEventProcessor(object sender, EventArgs e)
        {
            _counter++;

            if (_counter >= 10800)  //segundos = 3 horas
            {
                _counter = 0; // evita que dispare muchas veces

                Comunes.Inactivate I = new Comunes.Inactivate();
                I.ShowDialog();
            }
        }

        public static void Contenedores_Click(object sender, EventArgs e)
        {
            _counter = 0;
        }

        public static void AddEvents(Form form)
        {
            try
            {
                Contenedor conte = Application.OpenForms.OfType<Contenedor>().SingleOrDefault();
                if (conte.linkLabel5.Visible == false)
                {
                    Analogo f = Application.OpenForms.OfType<Analogo>().SingleOrDefault();
                    if (f.Visible == true)
                    {
                        f.Dispose();
                        f.Close();

                        Analogo f2 = new Analogo(); f2.Show();
                    }
                }              

                foreach (Control control in form.Controls)
                {
                    if (recursiva(control))
                    {

                    }
                    else
                    {
                        control.Click += new System.EventHandler(Contenedores_Click);
                        control.KeyDown += new KeyEventHandler(Contenedores_Click);
                    }
                }
            }
            catch
            {
                foreach (Control control in form.Controls)
                {
                    if (recursiva(control))
                    {

                    }
                    else
                    {
                        control.Click += new System.EventHandler(Contenedores_Click);
                        control.KeyDown += new KeyEventHandler(Contenedores_Click);
                    }
                }
            }           
        }

        public static void AddEventsContenedor(Form form)
        {
            foreach (Control control in form.Controls)
            {
                if (recursiva(control))
                {

                }
                else
                {
                    control.Click += new System.EventHandler(Contenedores_Click);
                    control.KeyDown += new KeyEventHandler(Contenedores_Click);
                }
            }
        }

        public static bool recursiva(Control control)
        {
            if (control is GroupBox)
            {
                foreach (Control controlinside in control.Controls)
                {
                    if (recursiva(controlinside))
                    {


                    }
                    else
                    {

                        controlinside.Click += new System.EventHandler(Contenedores_Click);
                        controlinside.KeyDown += new KeyEventHandler(Contenedores_Click);
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
