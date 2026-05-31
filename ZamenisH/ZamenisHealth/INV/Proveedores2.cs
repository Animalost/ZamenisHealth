using Domain.CXN;
using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Linq;
using System.Windows.Forms;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.INV
{
    public partial class Proveedores2 : Forma
    {
        private IProveedores proveedores;
        private int CodeProveedor;
        private MensajesGeneral MG;

        public Proveedores2(int codeProveedor)
        {
            InitializeComponent();
            CodeProveedor = codeProveedor;
            proveedores = new MProveedores();
        }

        private void Proveedores2_Load(object sender, EventArgs e)
        {
            Titulo.Text = "Proveedores";
            LogoMain.Image = Properties.Resources.Splash;
            SubTitulo.Text = $"Zamenis Health {Conexion.VersionApp}";

            ToolStripButton btnSave;

            btnSave = new ToolStripButton();
            btnSave = createToolButton("Guardar");
            MenuLateral.Items.Add(btnSave);
            btnSave.Click += toolStripButton2_Click;

            if (CodeProveedor > 0)
            {
                CXN_PROVEEDORES P = proveedores.getProvByCode(CodeProveedor);
                if (P != null) 
                { 
                    textBox1.Text = CodeProveedor.ToString().Trim();
                    textBox2.Text = P.Nombre;
                    textBox3.Text = P.Identificacion;
                    textBox4.Text = P.Telefono;
                    textBox5.Text = P.Direccion;
                    textBox6.Text = P.Email;
                    textBox7.Text = P.Responsable;

                    textBox1.Enabled = false;
                }
                else
                {
                    MG = new MensajesGeneral();
                    MG.Mensaje = "No se encontró el proveedor.";
                    MG.TipoImagen = 3;
                    MG.ShowDialog();

                    this.Dispose();
                    this.Close();
                }
            }
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Dispose();
            this.Close();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(textBox1.Text.Trim()) || string.IsNullOrEmpty(textBox2.Text.Trim()) || string.IsNullOrEmpty(textBox3.Text.Trim()))
                {
                    MG = new MensajesGeneral();
                    MG.Mensaje = "Debe diligenciar los campos obligatorios como minimo";
                    MG.TipoImagen = 3;
                    MG.ShowDialog();
                    return;
                }
                else
                {
                    CXN_PROVEEDORES P = new CXN_PROVEEDORES
                    {
                        Nombre = textBox2.Text.Trim(),
                        Identificacion = textBox3.Text.Trim(),
                        Telefono = textBox4.Text.Trim(),
                        Direccion = textBox5.Text.Trim(),
                        Codigo = Convert.ToInt32(textBox1.Text.Trim()),
                        Email = textBox6.Text.Trim(),
                        Responsable = textBox7.Text.Trim()
                    };

                    if (textBox1.Enabled == false)
                    {
                        //actualizar
                        bool update = proveedores.updateProveedor(P);
                        if (update == true)
                        {
                            MG = new MensajesGeneral();
                            MG.Mensaje = "El proveedor se actualizó correctamente.";
                            MG.TipoImagen = 3;
                            MG.ShowDialog();

                            Proveedores f7 = Application.OpenForms.OfType<Proveedores>().LastOrDefault();
                            f7.CargarGrilla();

                            this.Dispose();
                            this.Close();
                        }
                        else
                        {
                            MG = new MensajesGeneral();
                            MG.Mensaje = "No se logro actualizar el proveedor";
                            MG.TipoImagen = 1000;
                            MG.ShowDialog();
                        }                        
                    }
                    else
                    {
                        //crear
                        CXN_PROVEEDORES search = proveedores.getProvByCode(Convert.ToInt32(textBox1.Text.Trim()));
                        if (search != null)
                        {
                            MG = new MensajesGeneral();
                            MG.Mensaje = "El código del proveedor ya existe, por favor ingrese otro código.";
                            MG.TipoImagen = 1000;
                            MG.ShowDialog();
                        }
                        else
                        {
                            bool create = proveedores.createProveedor(P);
                            if (create == false)
                            {
                                MG = new MensajesGeneral();
                                MG.Mensaje = "No se logro crear el proveedor";
                                MG.TipoImagen = 1000;
                                MG.ShowDialog();
                            }
                            else
                            {
                                MG = new MensajesGeneral();
                                MG.Mensaje = "El proveedor se guardó correctamente.";
                                MG.TipoImagen = 1;
                                MG.ShowDialog();

                                Proveedores f7 = Application.OpenForms.OfType<Proveedores>().LastOrDefault();
                                f7.CargarGrilla();

                                this.Dispose();
                                this.Close();
                            }
                        }                       
                    }
                }
            }
            catch (Exception ex)
            {
                MG = new MensajesGeneral();
                MG.Mensaje = "Ocurrió un error al guardar el proveedor.\n" + ex.Message;
                MG.TipoImagen = 3;
                MG.ShowDialog();
            }
        }
    }
}
