using Domain.CXN;

using FormAndControls;

using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;

using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ZamenisHealth.Comunes.Extras
{
    public partial class SMSDisponibles : Forma2
    {
        
        private readonly static ICompañia repositorioCompañia = new MCompañia();

        public SMSDisponibles()
        {
            InitializeComponent();
        
        }

        private void SMSDisponibles_Load(object sender, EventArgs e)
        {
            try
            {
                Titulo.Text = "Mensajes de Texto";
                SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";
               
                List<CXN_CIA> getCias =  repositorioCompañia.getAllCompañias();
                
                if (getCias != null) 
                {
                    foreach (CXN_CIA c in getCias) 
                    {
                        comboBox1.Items.Add(c.Com_Nombre);
                    }

                    comboBox1.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                
                    label2.Text = repositorioCompañia.getPrestadorbyName(comboBox1.Text).Com_SMS.ToString();
                                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
