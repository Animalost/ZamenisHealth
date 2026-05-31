using System;
using System.Windows.Forms;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using ZamenisHealth.Clases;

namespace ZamenisHealth.HistoriasClinicas
{
    public partial class Historia_JM_Completar_Seleccion : ConfigForm.BaseForm
    {
        private static readonly ILogin repoLogin = new MLogin();
        private int Admision = 0;

        public Historia_JM_Completar_Seleccion()
        {
            InitializeComponent();
        }

        public Historia_JM_Completar_Seleccion(int Adm)
        {
            InitializeComponent();
            this.Admision = Adm;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            bool Acceder = Consulta("TO");
            if (Acceder == true)
            {
                this.Dispose();
                this.Close();

                if (this.Admision == 0) 
                {
                    HistoriasClinicas.Historia_JM_Completar C = new Historia_JM_Completar("TO");
                    C.ShowDialog();
                }
                else
                {
                    HistoriasClinicas.Historia_JM_Completar C = new Historia_JM_Completar("TO", this.Admision);
                    C.ShowDialog();
                }

                return;
            }

            MessageBox.Show("Su usuario no corresponde a la especialidad seleccionada", "No se puede continuar", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        bool Consulta(string TipoUser)
        {
            return repoLogin.ValidarTipoUsuarioEspecialidad(Comunes.Contenedor.UsuarioLogueado, TipoUser);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            bool Acceder = Consulta("PS");
            if (Acceder == true)
            {
                this.Dispose();
                this.Close();

                if (this.Admision == 0)
                {
                    HistoriasClinicas.Historia_JM_Completar C = new Historia_JM_Completar("PS");
                    C.ShowDialog();
                }
                else
                {
                    HistoriasClinicas.Historia_JM_Completar C = new Historia_JM_Completar("PS", this.Admision);
                    C.ShowDialog();
                }
                return;
            }

            MessageBox.Show("Su usuario no corresponde a la especialidad seleccionada", "No se puede continuar", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            bool Acceder = Consulta("FI");
            if (Acceder == true)
            {
                this.Dispose();
                this.Close();

                if (this.Admision == 0)
                {
                    HistoriasClinicas.Historia_JM_Completar C = new Historia_JM_Completar("FI");
                    C.ShowDialog();
                }
                else
                {
                    HistoriasClinicas.Historia_JM_Completar C = new Historia_JM_Completar("FI", this.Admision);
                    C.ShowDialog();
                }
                return;
            }

            MessageBox.Show("Su usuario no corresponde a la especialidad seleccionada", "No se puede continuar", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            bool Acceder = Consulta("TF");
            if (Acceder == true)
            {
                this.Dispose();
                this.Close();

                if (this.Admision == 0)
                {
                    HistoriasClinicas.Historia_JM_Completar C = new Historia_JM_Completar("TF");
                    C.ShowDialog();
                }
                else
                {
                    HistoriasClinicas.Historia_JM_Completar C = new Historia_JM_Completar("TF", this.Admision);
                    C.ShowDialog();
                }
                return;
            }

            MessageBox.Show("Su usuario no corresponde a la especialidad seleccionada", "No se puede continuar", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void Historia_JM_Completar_Seleccion_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Completar Juntas";
        }
    }
}
