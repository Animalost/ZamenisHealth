using Domain;
using Domain.CXN;
using FormAndControls;
using Persistence;
using Persistence.CXN.Interfaces;
using Persistence.CXN.Metodos;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ZamenisHealth.Clases;
using ZamenisHealth.Comunes;

namespace ZamenisHealth.Recepcion
{
    public partial class Particulares : Forma2
    {
        private static readonly ICompañia repositorioCompañias = new MCompañia();
        private static readonly IConvenios repositorioConvenios = new MConvenios();
        private static readonly IInventario repositorioInventario = new MInventario();
        private static readonly ICargos repositorioCargos = new MCargos();
        private static readonly IFacturacion repositorioFactura = new MFacturacion();
        private static readonly IPacientes repositorioPacientes = new MPacientes();

        int cia, Serv_1, Serv_2, Id;
        const int Ase = 99;
        
        private void Particulares_Load(object sender, EventArgs e)
        {
            try
            {
                this.Titulo.Text = "Cotizaciones";
                this.ImageClose.Visible = false;

                
                CargarDocumentos();

                comboBox2.Items.Clear();

                var cias = repositorioCompañias.getAllCompañias();
                if (cias != null)
                {
                    foreach (var i in cias)
                    {
                        comboBox2.Items.Add(i.Com_Nombre);
                    }

                    comboBox2.SelectedIndex = 0;
                }

                List<string> _serv =  repositorioConvenios.CargarServiciosxASE(99);
                if (_serv != null)
                {
                    foreach (var s in _serv)
                    {
                        comboBox3.Items.Add(s);
                        comboBox4.Items.Add(s);
                    }
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void toolStripButton7_Click(object sender, EventArgs e)
        {
            this.Dispose();
            this.Close();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            Extras.Precios P = new Extras.Precios();
            P.btnCotizaciones.Enabled = false;
            P.ShowDialog();
        }

        private void textBox61_DoubleClick(object sender, EventArgs e)
        {
            Comunes.BuscarPacientes P = new Comunes.BuscarPacientes("Cotizaciones");
            P.ShowDialog();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            CXN_CIA Idcia = repositorioCompañias.getPrestadorbyName(comboBox2.Text);           
            cia = Idcia.Com_Identificador;
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (comboBox3.Text == "Ninguno") { textBox2.Text = ""; Serv_1 = 0; Operacion(); return; }

                CXN_CONVENIOS _datosServ = repositorioConvenios.ServicioCUP(comboBox3.Text, 99);
                if (_datosServ != null)
                {
                    textBox2.Text = _datosServ.Con_Id_Serv;
                    Serv_1 = _datosServ.Con_Valor;
                    Operacion();
                }
                else
                {
                    textBox2.Text = "";
                    Serv_1 = 0;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (comboBox4.Text == "Ninguno") { textBox59.Text = ""; Serv_2 = 0; Operacion(); return; }

                CXN_CONVENIOS _datosServ = repositorioConvenios.ServicioCUP(comboBox4.Text, 99);             
                if (_datosServ != null)
                {
                    textBox59.Text = _datosServ.Con_Id_Serv;
                    Serv_2 = _datosServ.Con_Valor;
                    Operacion();
                }
                else
                {
                    textBox59.Text = "";
                    Serv_2 = 0;
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
        private void Operacion()
        {
            try
            {
                int O1 = Convert.ToInt32(Serv_1) * Convert.ToInt32(textBox1.Text);
                textBox3.Text = O1.ToString();

                int O2 = Convert.ToInt32(Serv_2) * Convert.ToInt32(textBox60.Text);
                textBox4.Text = O2.ToString();
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
                textBox3.Text = "";
                textBox4.Text = "";
            }
        }
        public Particulares()
        {
            InitializeComponent();          
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            try
            {
                if (textBox7.Text == "") { textBox7.Text = "0"; }
                if (textBox10.Text == "") { textBox10.Text = "0"; }
                if (textBox11.Text == "") { textBox11.Text = "0"; }
                if (textBox14.Text == "") { textBox14.Text = "0"; }
                if (textBox17.Text == "") { textBox17.Text = "0"; }
                if (textBox20.Text == "") { textBox20.Text = "0"; }
                if (textBox23.Text == "") { textBox23.Text = "0"; }
                if (textBox26.Text == "") { textBox26.Text = "0"; }
                if (textBox29.Text == "") { textBox29.Text = "0"; }
                if (textBox56.Text == "") { textBox56.Text = "0"; }
                if (textBox53.Text == "") { textBox53.Text = "0"; }
                if (textBox50.Text == "") { textBox50.Text = "0"; }
                if (textBox47.Text == "") { textBox47.Text = "0"; }
                if (textBox44.Text == "") { textBox44.Text = "0"; }
                if (textBox41.Text == "") { textBox41.Text = "0"; }
                if (textBox38.Text == "") { textBox38.Text = "0"; }
                if (textBox35.Text == "") { textBox35.Text = "0"; }
                if (textBox32.Text == "") { textBox32.Text = "0"; }

                int Total = 0;
                int Cargo = 0;

                var Val = Valor(textBox5.Text);
                Cargo = Val * Convert.ToInt32(textBox7.Text);
                Total = Total + Cargo;

                Val = Valor(textBox8.Text);
                Cargo = Val * Convert.ToInt32(textBox10.Text);
                Total = Total + Cargo;

                Val = Valor(textBox13.Text);
                Cargo = Val * Convert.ToInt32(textBox11.Text);
                Total = Total + Cargo;

                Val = Valor(textBox16.Text);
                Cargo = Val * Convert.ToInt32(textBox14.Text);
                Total = Total + Cargo;

                Val = Valor(textBox19.Text);
                Cargo = Val * Convert.ToInt32(textBox17.Text);
                Total = Total + Cargo;

                Val = Valor(textBox22.Text);
                Cargo = Val * Convert.ToInt32(textBox20.Text);
                Total = Total + Cargo;

                Val = Valor(textBox25.Text);
                Cargo = Val * Convert.ToInt32(textBox23.Text);
                Total = Total + Cargo;

                Val = Valor(textBox28.Text);
                Cargo = Val * Convert.ToInt32(textBox26.Text);
                Total = Total + Cargo;

                Val = Valor(textBox31.Text);
                Cargo = Val * Convert.ToInt32(textBox29.Text);
                Total = Total + Cargo;

                Val = Valor(textBox58.Text);
                Cargo = Val * Convert.ToInt32(textBox56.Text);
                Total = Total + Cargo;

                Val = Valor(textBox55.Text);
                Cargo = Val * Convert.ToInt32(textBox53.Text);
                Total = Total + Cargo;

                Val = Valor(textBox52.Text);
                Cargo = Val * Convert.ToInt32(textBox50.Text);
                Total = Total + Cargo;

                Val = Valor(textBox49.Text);
                Cargo = Val * Convert.ToInt32(textBox47.Text);
                Total = Total + Cargo;

                Val = Valor(textBox46.Text);
                Cargo = Val * Convert.ToInt32(textBox44.Text);
                Total = Total + Cargo;

                Val = Valor(textBox43.Text);
                Cargo = Val * Convert.ToInt32(textBox41.Text);
                Total = Total + Cargo;

                Val = Valor(textBox40.Text);
                Cargo = Val * Convert.ToInt32(textBox38.Text);
                Total = Total + Cargo;

                Val = Valor(textBox37.Text);
                Cargo = Val * Convert.ToInt32(textBox35.Text);
                Total = Total + Cargo;

                Val = Valor(textBox34.Text);
                Cargo = Val * Convert.ToInt32(textBox32.Text);
                Total = Total + Cargo;

                int Val1, Val2;
                Operacion();

                Val1 = Convert.ToInt32(textBox3.Text);
                Val2 = Convert.ToInt32(textBox4.Text);

                Total = Total + Val1 + Val2;


                int Val3 = Val1 + Val2;

                if (checkBox2.Checked == true)
                {
                    Total = Total - Convert.ToInt32(Val2);
                    label16.Text = "$" + Total.ToString("N0");
                }
                else
                {
                    label16.Text = "$" + Total.ToString("N0");
                }

                if (textBox7.Text == "0") { textBox7.Text = ""; }
                if (textBox10.Text == "0") { textBox10.Text = ""; }
                if (textBox11.Text == "0") { textBox11.Text = ""; }
                if (textBox14.Text == "0") { textBox14.Text = ""; }
                if (textBox17.Text == "0") { textBox17.Text = ""; }
                if (textBox20.Text == "0") { textBox20.Text = ""; }
                if (textBox23.Text == "0") { textBox23.Text = ""; }
                if (textBox26.Text == "0") { textBox26.Text = ""; }
                if (textBox29.Text == "0") { textBox29.Text = ""; }
                if (textBox56.Text == "0") { textBox56.Text = ""; }
                if (textBox53.Text == "0") { textBox53.Text = ""; }
                if (textBox50.Text == "0") { textBox50.Text = ""; }
                if (textBox47.Text == "0") { textBox47.Text = ""; }
                if (textBox44.Text == "0") { textBox44.Text = ""; }
                if (textBox41.Text == "0") { textBox41.Text = ""; }
                if (textBox38.Text == "0") { textBox38.Text = ""; }
                if (textBox35.Text == "0") { textBox35.Text = ""; }
                if (textBox32.Text == "0") { textBox32.Text = ""; }
            }
            catch (Exception ex)
            {
                label16.Text = "0";
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        (int valor, string item, string detalle) MethodFather(int ase, string code)
        {
            (int valor, string item, string detalle) DatoProd = (0, "", "");

            
                DatoProd = repositorioInventario.ConsultarValor(Ase, code);
            

            return DatoProd;
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            var DatoProd = MethodFather(Ase, textBox5.Text);
            if (DatoProd == (0, "", ""))
            {
                textBox6.Text = "";
                return;
            }

            textBox6.Text = DatoProd.item;
        }

        private void textBox8_TextChanged(object sender, EventArgs e)
        {
            var DatoProd = MethodFather(Ase, textBox8.Text);
            if (DatoProd == (0, "", ""))
            {
                textBox9.Text = "";
                return;
            }
            textBox9.Text = DatoProd.item;
        }

        private void textBox13_TextChanged(object sender, EventArgs e)
        {
            var DatoProd = MethodFather(Ase, textBox13.Text);
            if (DatoProd == (0, "", ""))
            {
                textBox12.Text = "";
                return;
            }
            textBox12.Text = DatoProd.item;
        }

        private void textBox16_TextChanged(object sender, EventArgs e)
        {
            var DatoProd = MethodFather(Ase, textBox16.Text);
            if (DatoProd == (0, "", ""))
            {
                textBox15.Text = "";
                return;
            }
            textBox15.Text = DatoProd.item;
        }

        private void textBox19_TextChanged(object sender, EventArgs e)
        {
            var DatoProd = MethodFather(Ase, textBox19.Text);
            if (DatoProd == (0, "", ""))
            {
                textBox18.Text = "";
                return;
            }
            textBox18.Text = DatoProd.item;
        }

        private void textBox22_TextChanged(object sender, EventArgs e)
        {
            var DatoProd = MethodFather(Ase, textBox22.Text);
            if (DatoProd == (0, "", ""))
            {
                textBox21.Text = "";
                return;
            }
            textBox21.Text = DatoProd.item;
        }

        private void textBox25_TextChanged(object sender, EventArgs e)
        {
            var DatoProd = MethodFather(Ase, textBox25.Text);
            if (DatoProd == (0, "", ""))
            {
                textBox24.Text = "";
                return;
            }
            textBox24.Text = DatoProd.item;
        }

        private void textBox28_TextChanged(object sender, EventArgs e)
        {
            var DatoProd = MethodFather(Ase, textBox28.Text);
            if (DatoProd == (0, "", ""))
            {
                textBox27.Text = "";
                return;
            }
            textBox27.Text = DatoProd.item;
        }

        private void textBox31_TextChanged(object sender, EventArgs e)
        {
            var DatoProd = MethodFather(Ase, textBox31.Text);
            if (DatoProd == (0, "", ""))
            {
                textBox30.Text = "";
                return;
            }
            textBox30.Text = DatoProd.item;
        }

        private void textBox58_TextChanged(object sender, EventArgs e)
        {
            var DatoProd = MethodFather(Ase, textBox58.Text);
            if (DatoProd == (0, "", ""))
            {
                textBox57.Text = "";
                return;
            }
            textBox57.Text = DatoProd.item;
        }

        private void textBox55_TextChanged(object sender, EventArgs e)
        {
            var DatoProd = MethodFather(Ase, textBox55.Text);
            if (DatoProd == (0, "", ""))
            {
                textBox54.Text = "";
                return;
            }
            textBox54.Text = DatoProd.item;
        }

        private void textBox52_TextChanged(object sender, EventArgs e)
        {
            var DatoProd = MethodFather(Ase, textBox52.Text);
            if (DatoProd == (0, "", ""))
            {
                textBox51.Text = "";
                return;
            }
            textBox51.Text = DatoProd.item;
        }

        private void textBox49_TextChanged(object sender, EventArgs e)
        {
            var DatoProd = MethodFather(Ase, textBox49.Text);
            if (DatoProd == (0, "", ""))
            {
                textBox48.Text = "";
                return;
            }
            textBox48.Text = DatoProd.item;
        }

        private void textBox46_TextChanged(object sender, EventArgs e)
        {
            var DatoProd = MethodFather(Ase, textBox46.Text);
            if (DatoProd == (0, "", ""))
            {
                textBox45.Text = "";
                return;
            }
            textBox45.Text = DatoProd.item;
        }

        private void textBox43_TextChanged(object sender, EventArgs e)
        {
            var DatoProd = MethodFather(Ase, textBox43.Text);
            if (DatoProd == (0, "", ""))
            {
                textBox42.Text = "";
                return;
            }
            textBox42.Text = DatoProd.item;
        }

        private void textBox40_TextChanged(object sender, EventArgs e)
        {
            var DatoProd = MethodFather(Ase, textBox40.Text);
            if (DatoProd == (0, "", ""))
            {
                textBox39.Text = "";
                return;
            }
            textBox39.Text = DatoProd.item;
        }

        private void textBox37_TextChanged(object sender, EventArgs e)
        {
            var DatoProd = MethodFather(Ase, textBox37.Text);
            if (DatoProd == (0, "", ""))
            {
                textBox36.Text = "";
                return;
            }
            textBox36.Text = DatoProd.item;
        }

        private void textBox34_TextChanged(object sender, EventArgs e)
        {
            var DatoProd = MethodFather(Ase, textBox34.Text);
            if (DatoProd == (0, "", ""))
            {
                textBox33.Text = "";
                return;
            }
            textBox33.Text = DatoProd.item;
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            try
            {
                if (textBox6.Text != "" && textBox7.Text == "") { MessageBox.Show("Revise datos"); return; }
                if (textBox9.Text != "" && textBox10.Text == "") { MessageBox.Show("Revise datos"); return; }
                if (textBox12.Text != "" && textBox11.Text == "") { MessageBox.Show("Revise datos"); return; }
                if (textBox15.Text != "" && textBox14.Text == "") { MessageBox.Show("Revise datos"); return; }
                if (textBox18.Text != "" && textBox17.Text == "") { MessageBox.Show("Revise datos"); return; }
                if (textBox21.Text != "" && textBox20.Text == "") { MessageBox.Show("Revise datos"); return; }
                if (textBox24.Text != "" && textBox23.Text == "") { MessageBox.Show("Revise datos"); return; }
                if (textBox27.Text != "" && textBox26.Text == "") { MessageBox.Show("Revise datos"); return; }
                if (textBox30.Text != "" && textBox29.Text == "") { MessageBox.Show("Revise datos"); return; }
                if (textBox57.Text != "" && textBox56.Text == "") { MessageBox.Show("Revise datos"); return; }
                if (textBox54.Text != "" && textBox53.Text == "") { MessageBox.Show("Revise datos"); return; }
                if (textBox51.Text != "" && textBox50.Text == "") { MessageBox.Show("Revise datos"); return; }
                if (textBox48.Text != "" && textBox47.Text == "") { MessageBox.Show("Revise datos"); return; }
                if (textBox45.Text != "" && textBox44.Text == "") { MessageBox.Show("Revise datos"); return; }
                if (textBox42.Text != "" && textBox41.Text == "") { MessageBox.Show("Revise datos"); return; }
                if (textBox39.Text != "" && textBox38.Text == "") { MessageBox.Show("Revise datos"); return; }
                if (textBox36.Text != "" && textBox35.Text == "") { MessageBox.Show("Revise datos"); return; }
                if (textBox33.Text != "" && textBox32.Text == "") { MessageBox.Show("Revise datos"); return; }

                if (textBox5.Text != "" && textBox6.Text == "") { MessageBox.Show("Revise datos"); return; }
                if (textBox8.Text != "" && textBox9.Text == "") { MessageBox.Show("Revise datos"); return; }
                if (textBox13.Text != "" && textBox12.Text == "") { MessageBox.Show("Revise datos"); return; }
                if (textBox16.Text != "" && textBox15.Text == "") { MessageBox.Show("Revise datos"); return; }
                if (textBox19.Text != "" && textBox18.Text == "") { MessageBox.Show("Revise datos"); return; }
                if (textBox22.Text != "" && textBox21.Text == "") { MessageBox.Show("Revise datos"); return; }
                if (textBox25.Text != "" && textBox24.Text == "") { MessageBox.Show("Revise datos"); return; }
                if (textBox28.Text != "" && textBox27.Text == "") { MessageBox.Show("Revise datos"); return; }
                if (textBox31.Text != "" && textBox30.Text == "") { MessageBox.Show("Revise datos"); return; }
                if (textBox58.Text != "" && textBox57.Text == "") { MessageBox.Show("Revise datos"); return; }
                if (textBox55.Text != "" && textBox54.Text == "") { MessageBox.Show("Revise datos"); return; }
                if (textBox52.Text != "" && textBox51.Text == "") { MessageBox.Show("Revise datos"); return; }
                if (textBox49.Text != "" && textBox48.Text == "") { MessageBox.Show("Revise datos"); return; }
                if (textBox46.Text != "" && textBox45.Text == "") { MessageBox.Show("Revise datos"); return; }
                if (textBox43.Text != "" && textBox42.Text == "") { MessageBox.Show("Revise datos"); return; }
                if (textBox40.Text != "" && textBox39.Text == "") { MessageBox.Show("Revise datos"); return; }
                if (textBox37.Text != "" && textBox36.Text == "") { MessageBox.Show("Revise datos"); return; }
                if (textBox34.Text != "" && textBox33.Text == "") { MessageBox.Show("Revise datos"); return; }
                if (textBox61.Text != "" && textBox60.Text == "") { MessageBox.Show("Revise datos"); return; }

                if (label14.Text == "") { MessageBox.Show("Debe buscar un paciente"); return; }

                CXN_CIA Doc = new CXN_CIA();

                
                    Doc = repositorioCompañias.getPrestadorbyCode(cia);
                

                if (Doc == null)
                {
                    MessageBox.Show("Error en consecutivos de facturacion",
                        "Error Grave",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }

                if (Doc.Com_Cotiza == 0)
                {
                    MessageBox.Show("Error en consecutivos de facturacion",
                        "Error Grave",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }

                CXN_CARGOS S1 = new CXN_CARGOS();
                S1.Car_Cant = Convert.ToInt32(textBox1.Text);
                S1.Car_Val_Tot = Convert.ToInt32(textBox3.Text);
                S1.Car_Pac = Convert.ToInt32(Id);
                S1.Car_Cia = Convert.ToInt32(cia);
                S1.Car_Ase = Convert.ToInt32(Ase);
                S1.Car_Cod = textBox2.Text;
                S1.Car_Item = comboBox3.Text;
                S1.Car_Factura = Doc.Com_Cotiza.ToString();
                S1.Car_Estado = "C";
                S1.Car_Tipo = "Cotizacion";
                S1.Car_Tipo_Doc = "CO";
                S1.Car_Usr_Graba = Comunes.Contenedor.UsuarioLogueado;                

                string resS1 = "";
                
                
                    resS1 = repositorioCargos.insertService(S1);
                
                
                if (resS1 != "OK")
                {
                    MessageBox.Show("Inconveniente con este servicio primero seleccionado", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (checkBox2.Checked == false)
                {
                    if (comboBox4.Text != "Ninguno")
                    {
                        S1.Car_Cant = Convert.ToInt32(textBox60.Text);
                        S1.Car_Val_Tot = Convert.ToInt32(textBox4.Text);
                        S1.Car_Pac = Convert.ToInt32(Id);
                        S1.Car_Cia = Convert.ToInt32(cia);
                        S1.Car_Ase = Convert.ToInt32(Ase);
                        S1.Car_Cod = textBox59.Text;
                        S1.Car_Item = comboBox4.Text;
                        S1.Car_Factura = Doc.Com_Cotiza.ToString();
                        S1.Car_Estado = "C";
                        S1.Car_Tipo = "Cotizacion";
                        S1.Car_Tipo_Doc = "CO";
                        S1.Car_Usr_Graba = Comunes.Contenedor.UsuarioLogueado;

                        string resS2 = "";

                        
                            resS2 = repositorioCargos.insertService(S1);
                        

                        if (resS2 != "OK")
                        {
                            MessageBox.Show("Inconveniente con este servicio segundo seleccionado", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                }

                int TotCar;

                //CARGO 1
                if (textBox5.Text != "" && textBox6.Text != "" && textBox7.Text != "")
                {
                    var Val = Valor(textBox5.Text); //izquierda
                    TotCar = Val * Convert.ToInt32(textBox7.Text); //derecha

                    S1.Car_Cant = Convert.ToInt32(textBox7.Text);
                    S1.Car_Val_Tot = Convert.ToInt32(TotCar);
                    S1.Car_Pac = Convert.ToInt32(Id);
                    S1.Car_Cia = Convert.ToInt32(cia);
                    S1.Car_Ase = Convert.ToInt32(Ase);
                    S1.Car_Cod = textBox5.Text;
                    S1.Car_Item = textBox6.Text;
                    S1.Car_Factura = Doc.Com_Cotiza.ToString();
                    S1.Car_Estado = "C";
                    S1.Car_Tipo = "Cotizacion";
                    S1.Car_Tipo_Doc = "CO";
                    S1.Car_Usr_Graba = Comunes.Contenedor.UsuarioLogueado;

                    string InsertServ = "";

                    
                        InsertServ = repositorioCargos.insertService(S1);
                    

                    if (InsertServ != "OK")
                    {
                        MessageBox.Show("Inconveniente con este item primero seleccionado: " + InsertServ.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                //CARGO2
                if (textBox8.Text != "" && textBox9.Text != "" && textBox10.Text != "")
                {
                    var Val = Valor(textBox8.Text); //izquierda
                    TotCar = Val * Convert.ToInt32(textBox10.Text); //derecha

                    S1.Car_Cant = Convert.ToInt32(textBox10.Text);
                    S1.Car_Val_Tot = Convert.ToInt32(TotCar);
                    S1.Car_Pac = Convert.ToInt32(Id);
                    S1.Car_Cia = Convert.ToInt32(cia);
                    S1.Car_Ase = Convert.ToInt32(Ase);
                    S1.Car_Cod = textBox8.Text;
                    S1.Car_Item = textBox9.Text;
                    S1.Car_Factura = Doc.Com_Cotiza.ToString();
                    S1.Car_Estado = "C";
                    S1.Car_Tipo = "Cotizacion";
                    S1.Car_Tipo_Doc = "CO";
                    S1.Car_Usr_Graba = Comunes.Contenedor.UsuarioLogueado;

                    string InsertServ = "";

                    
                        InsertServ = repositorioCargos.insertService(S1);
                    

                    if (InsertServ != "OK")
                    {
                        MessageBox.Show("Inconveniente con este item segundo seleccionado: " + InsertServ.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }


                //CARGO 3
                if (textBox13.Text != "" && textBox12.Text != "" && textBox11.Text != "")
                {
                    var Val = Valor(textBox13.Text); //izquierda
                    TotCar = Val * Convert.ToInt32(textBox11.Text); //derecha

                    S1.Car_Cant = Convert.ToInt32(textBox11.Text);
                    S1.Car_Val_Tot = Convert.ToInt32(TotCar);
                    S1.Car_Pac = Convert.ToInt32(Id);
                    S1.Car_Cia = Convert.ToInt32(cia);
                    S1.Car_Ase = Convert.ToInt32(Ase);
                    S1.Car_Cod = textBox13.Text;
                    S1.Car_Item = textBox12.Text;
                    S1.Car_Factura = Doc.Com_Cotiza.ToString();
                    S1.Car_Estado = "C";
                    S1.Car_Tipo = "Cotizacion";
                    S1.Car_Tipo_Doc = "CO";
                    S1.Car_Usr_Graba = Comunes.Contenedor.UsuarioLogueado;

                    string InsertServ = "";

                    
                        InsertServ = repositorioCargos.insertService(S1);
                    

                    if (InsertServ != "OK")
                    {
                        MessageBox.Show("Inconveniente con este item tercero seleccionado", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                //CARGO 4
                if (textBox16.Text != "" && textBox15.Text != "" && textBox14.Text != "")
                {
                    var Val = Valor(textBox16.Text); //izquierda
                    TotCar = Val * Convert.ToInt32(textBox14.Text); //derecha

                    S1.Car_Cant = Convert.ToInt32(textBox14.Text);
                    S1.Car_Val_Tot = Convert.ToInt32(TotCar);
                    S1.Car_Pac = Convert.ToInt32(Id);
                    S1.Car_Cia = Convert.ToInt32(cia);
                    S1.Car_Ase = Convert.ToInt32(Ase);
                    S1.Car_Cod = textBox16.Text;
                    S1.Car_Item = textBox15.Text;
                    S1.Car_Factura = Doc.Com_Cotiza.ToString();
                    S1.Car_Estado = "C";
                    S1.Car_Tipo = "Cotizacion";
                    S1.Car_Tipo_Doc = "CO";
                    S1.Car_Usr_Graba = Comunes.Contenedor.UsuarioLogueado;

                    string InsertServ = "";

                    
                        InsertServ = repositorioCargos.insertService(S1);
                    

                    if (InsertServ != "OK")
                    {
                        MessageBox.Show("Inconveniente con este item cuarto seleccionado", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                //CARGO 5
                if (textBox19.Text != "" && textBox18.Text != "" && textBox17.Text != "")
                {
                    var Val = Valor(textBox19.Text); //izquierda
                    TotCar = Val * Convert.ToInt32(textBox17.Text); //derecha

                    S1.Car_Cant = Convert.ToInt32(textBox17.Text);
                    S1.Car_Val_Tot = Convert.ToInt32(TotCar);
                    S1.Car_Pac = Convert.ToInt32(Id);
                    S1.Car_Cia = Convert.ToInt32(cia);
                    S1.Car_Ase = Convert.ToInt32(Ase);
                    S1.Car_Cod = textBox19.Text;
                    S1.Car_Item = textBox18.Text;
                    S1.Car_Factura = Doc.Com_Cotiza.ToString();
                    S1.Car_Estado = "C";
                    S1.Car_Tipo = "Cotizacion";
                    S1.Car_Tipo_Doc = "CO";
                    S1.Car_Usr_Graba = Comunes.Contenedor.UsuarioLogueado;

                    string InsertServ = "";

                    
                        InsertServ = repositorioCargos.insertService(S1);
                    

                    if (InsertServ != "OK")
                    {
                        MessageBox.Show("Inconveniente con este item quinto seleccionado", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                //CARGO 6
                if (textBox22.Text != "" && textBox21.Text != "" && textBox20.Text != "")
                {
                    var Val = Valor(textBox22.Text); //izquierda
                    TotCar = Val * Convert.ToInt32(textBox20.Text); //derecha

                    S1.Car_Cant = Convert.ToInt32(textBox20.Text);
                    S1.Car_Val_Tot = Convert.ToInt32(TotCar);
                    S1.Car_Pac = Convert.ToInt32(Id);
                    S1.Car_Cia = Convert.ToInt32(cia);
                    S1.Car_Ase = Convert.ToInt32(Ase);
                    S1.Car_Cod = textBox22.Text;
                    S1.Car_Item = textBox21.Text;
                    S1.Car_Factura = Doc.Com_Cotiza.ToString();
                    S1.Car_Estado = "C";
                    S1.Car_Tipo = "Cotizacion";
                    S1.Car_Tipo_Doc = "CO";
                    S1.Car_Usr_Graba = Comunes.Contenedor.UsuarioLogueado;

                    string InsertServ = "";

                    
                        InsertServ = repositorioCargos.insertService(S1);
                    

                    if (InsertServ != "OK")
                    {
                        MessageBox.Show("Inconveniente con este item sexto seleccionado", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                //CARGO 7
                if (textBox25.Text != "" && textBox24.Text != "" && textBox23.Text != "")
                {
                    var Val = Valor(textBox25.Text); //izquierda
                    TotCar = Val * Convert.ToInt32(textBox23.Text); //derecha

                    S1.Car_Cant = Convert.ToInt32(textBox23.Text);
                    S1.Car_Val_Tot = Convert.ToInt32(TotCar);
                    S1.Car_Pac = Convert.ToInt32(Id);
                    S1.Car_Cia = Convert.ToInt32(cia);
                    S1.Car_Ase = Convert.ToInt32(Ase);
                    S1.Car_Cod = textBox25.Text;
                    S1.Car_Item = textBox24.Text;
                    S1.Car_Factura = Doc.Com_Cotiza.ToString();
                    S1.Car_Estado = "C";
                    S1.Car_Tipo = "Cotizacion";
                    S1.Car_Tipo_Doc = "CO";
                    S1.Car_Usr_Graba = Comunes.Contenedor.UsuarioLogueado;

                    string InsertServ = "";

                    
                        InsertServ = repositorioCargos.insertService(S1);
                    

                    if (InsertServ != "OK")
                    {
                        MessageBox.Show("Inconveniente con este item septimo seleccionado", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                //CARGO 8
                if (textBox28.Text != "" && textBox27.Text != "" && textBox26.Text != "")
                {
                    var Val = Valor(textBox28.Text); //izquierda
                    TotCar = Val * Convert.ToInt32(textBox26.Text); //derecha

                    S1.Car_Cant = Convert.ToInt32(textBox26.Text);
                    S1.Car_Val_Tot = Convert.ToInt32(TotCar);
                    S1.Car_Pac = Convert.ToInt32(Id);
                    S1.Car_Cia = Convert.ToInt32(cia);
                    S1.Car_Ase = Convert.ToInt32(Ase);
                    S1.Car_Cod = textBox28.Text;
                    S1.Car_Item = textBox27.Text;
                    S1.Car_Factura = Doc.Com_Cotiza.ToString();
                    S1.Car_Estado = "C";
                    S1.Car_Tipo = "Cotizacion";
                    S1.Car_Tipo_Doc = "CO";
                    S1.Car_Usr_Graba = Comunes.Contenedor.UsuarioLogueado;

                    string InsertServ = "";

                    
                        InsertServ = repositorioCargos.insertService(S1);
                    

                    if (InsertServ != "OK")
                    {
                        MessageBox.Show("Inconveniente con este item octavo seleccionado", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                //CARGO 9
                if (textBox31.Text != "" && textBox30.Text != "" && textBox29.Text != "")
                {
                    var Val = Valor(textBox31.Text); //izquierda
                    TotCar = Val * Convert.ToInt32(textBox29.Text); //derecha

                    S1.Car_Cant = Convert.ToInt32(textBox29.Text);
                    S1.Car_Val_Tot = Convert.ToInt32(TotCar);
                    S1.Car_Pac = Convert.ToInt32(Id);
                    S1.Car_Cia = Convert.ToInt32(cia);
                    S1.Car_Ase = Convert.ToInt32(Ase);
                    S1.Car_Cod = textBox31.Text;
                    S1.Car_Item = textBox30.Text;
                    S1.Car_Factura = Doc.Com_Cotiza.ToString();
                    S1.Car_Estado = "C";
                    S1.Car_Tipo = "Cotizacion";
                    S1.Car_Tipo_Doc = "CO";
                    S1.Car_Usr_Graba = Comunes.Contenedor.UsuarioLogueado;

                    string InsertServ = "";

                    
                        InsertServ = repositorioCargos.insertService(S1);
                    

                    if (InsertServ != "OK")
                    {
                        MessageBox.Show("Inconveniente con este item noveno seleccionado", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                //CARGO 10
                if (textBox58.Text != "" && textBox57.Text != "" && textBox56.Text != "")
                {
                    var Val = Valor(textBox58.Text); //izquierda
                    TotCar = Val * Convert.ToInt32(textBox56.Text); //derecha

                    S1.Car_Cant = Convert.ToInt32(textBox56.Text);
                    S1.Car_Val_Tot = Convert.ToInt32(TotCar);
                    S1.Car_Pac = Convert.ToInt32(Id);
                    S1.Car_Cia = Convert.ToInt32(cia);
                    S1.Car_Ase = Convert.ToInt32(Ase);
                    S1.Car_Cod = textBox58.Text;
                    S1.Car_Item = textBox57.Text;
                    S1.Car_Factura = Doc.Com_Cotiza.ToString();
                    S1.Car_Estado = "C";
                    S1.Car_Tipo = "Cotizacion";
                    S1.Car_Tipo_Doc = "CO";
                    S1.Car_Usr_Graba = Comunes.Contenedor.UsuarioLogueado;

                    string InsertServ = "";

                    
                        InsertServ = repositorioCargos.insertService(S1);
                    

                    if (InsertServ != "OK")
                    {
                        MessageBox.Show("Inconveniente con este item decimo seleccionado", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                //CARGO 11
                if (textBox55.Text != "" && textBox54.Text != "" && textBox53.Text != "")
                {
                    var Val = Valor(textBox55.Text); //izquierda
                    TotCar = Val * Convert.ToInt32(textBox53.Text); //derecha

                    S1.Car_Cant = Convert.ToInt32(textBox53.Text);
                    S1.Car_Val_Tot = Convert.ToInt32(TotCar);
                    S1.Car_Pac = Convert.ToInt32(Id);
                    S1.Car_Cia = Convert.ToInt32(cia);
                    S1.Car_Ase = Convert.ToInt32(Ase);
                    S1.Car_Cod = textBox55.Text;
                    S1.Car_Item = textBox54.Text;
                    S1.Car_Factura = Doc.Com_Cotiza.ToString();
                    S1.Car_Estado = "C";
                    S1.Car_Tipo = "Cotizacion";
                    S1.Car_Tipo_Doc = "CO";
                    S1.Car_Usr_Graba = Comunes.Contenedor.UsuarioLogueado;

                    string InsertServ = "";

                    
                        InsertServ = repositorioCargos.insertService(S1);
                    

                    if (InsertServ != "OK")
                    {
                        MessageBox.Show("Inconveniente con este item undecimo seleccionado", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                //CARGO 12
                if (textBox52.Text != "" && textBox51.Text != "" && textBox50.Text != "")
                {
                    var Val = Valor(textBox52.Text); //izquierda
                    TotCar = Val * Convert.ToInt32(textBox50.Text); //derecha

                    S1.Car_Cant = Convert.ToInt32(textBox50.Text);
                    S1.Car_Val_Tot = Convert.ToInt32(TotCar);
                    S1.Car_Pac = Convert.ToInt32(Id);
                    S1.Car_Cia = Convert.ToInt32(cia);
                    S1.Car_Ase = Convert.ToInt32(Ase);
                    S1.Car_Cod = textBox52.Text;
                    S1.Car_Item = textBox51.Text;
                    S1.Car_Factura = Doc.Com_Cotiza.ToString();
                    S1.Car_Estado = "C";
                    S1.Car_Tipo = "Cotizacion";
                    S1.Car_Tipo_Doc = "CO";
                    S1.Car_Usr_Graba = Comunes.Contenedor.UsuarioLogueado;

                    string InsertServ = "";

                    
                        InsertServ = repositorioCargos.insertService(S1);
                    

                    if (InsertServ != "OK")
                    {
                        MessageBox.Show("Inconveniente con este item doceavo seleccionado", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                //CARGO 13
                if (textBox49.Text != "" && textBox48.Text != "" && textBox47.Text != "")
                {
                    var Val = Valor(textBox49.Text); //izquierda
                    TotCar = Val * Convert.ToInt32(textBox47.Text); //derecha

                    S1.Car_Cant = Convert.ToInt32(textBox47.Text);
                    S1.Car_Val_Tot = Convert.ToInt32(TotCar);
                    S1.Car_Pac = Convert.ToInt32(Id);
                    S1.Car_Cia = Convert.ToInt32(cia);
                    S1.Car_Ase = Convert.ToInt32(Ase);
                    S1.Car_Cod = textBox49.Text;
                    S1.Car_Item = textBox48.Text;
                    S1.Car_Factura = Doc.Com_Cotiza.ToString();
                    S1.Car_Estado = "C";
                    S1.Car_Tipo = "Cotizacion";
                    S1.Car_Tipo_Doc = "CO";
                    S1.Car_Usr_Graba = Comunes.Contenedor.UsuarioLogueado;

                    string InsertServ = "";

                    
                        InsertServ = repositorioCargos.insertService(S1);
                    

                    if (InsertServ != "OK")
                    {
                        MessageBox.Show("Inconveniente con este item treceavo seleccionado", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                //CARGO 14
                if (textBox46.Text != "" && textBox45.Text != "" && textBox44.Text != "")
                {
                    var Val = Valor(textBox46.Text); //izquierda
                    TotCar = Val * Convert.ToInt32(textBox44.Text); //derecha

                    S1.Car_Cant = Convert.ToInt32(textBox44.Text);
                    S1.Car_Val_Tot = Convert.ToInt32(TotCar);
                    S1.Car_Pac = Convert.ToInt32(Id);
                    S1.Car_Cia = Convert.ToInt32(cia);
                    S1.Car_Ase = Convert.ToInt32(Ase);
                    S1.Car_Cod = textBox46.Text;
                    S1.Car_Item = textBox45.Text;
                    S1.Car_Factura = Doc.Com_Cotiza.ToString();
                    S1.Car_Estado = "C";
                    S1.Car_Tipo = "Cotizacion";
                    S1.Car_Tipo_Doc = "CO";
                    S1.Car_Usr_Graba = Comunes.Contenedor.UsuarioLogueado;

                    string InsertServ = "";

                    
                        InsertServ = repositorioCargos.insertService(S1);
                    

                    if (InsertServ != "OK")
                    {
                        MessageBox.Show("Inconveniente con este item catorceavo seleccionado", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                //CARGO 15
                if (textBox43.Text != "" && textBox42.Text != "" && textBox41.Text != "")
                {
                    var Val = Valor(textBox43.Text); //izquierda
                    TotCar = Val * Convert.ToInt32(textBox41.Text); //derecha

                    S1.Car_Cant = Convert.ToInt32(textBox41.Text);
                    S1.Car_Val_Tot = Convert.ToInt32(TotCar);
                    S1.Car_Pac = Convert.ToInt32(Id);
                    S1.Car_Cia = Convert.ToInt32(cia);
                    S1.Car_Ase = Convert.ToInt32(Ase);
                    S1.Car_Cod = textBox43.Text;
                    S1.Car_Item = textBox42.Text;
                    S1.Car_Factura = Doc.Com_Cotiza.ToString();
                    S1.Car_Estado = "C";
                    S1.Car_Tipo = "Cotizacion";
                    S1.Car_Tipo_Doc = "CO";
                    S1.Car_Usr_Graba = Comunes.Contenedor.UsuarioLogueado;

                    string InsertServ = "";

                    
                        InsertServ = repositorioCargos.insertService(S1);
                    

                    if (InsertServ != "OK")
                    {
                        MessageBox.Show("Inconveniente con este item quinceavo seleccionado", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                //CARGO 16
                if (textBox40.Text != "" && textBox39.Text != "" && textBox38.Text != "")
                {
                    var Val = Valor(textBox40.Text); //izquierda
                    TotCar = Val * Convert.ToInt32(textBox38.Text); //derecha

                    S1.Car_Cant = Convert.ToInt32(textBox38.Text);
                    S1.Car_Val_Tot = Convert.ToInt32(TotCar);
                    S1.Car_Pac = Convert.ToInt32(Id);
                    S1.Car_Cia = Convert.ToInt32(cia);
                    S1.Car_Ase = Convert.ToInt32(Ase);
                    S1.Car_Cod = textBox40.Text;
                    S1.Car_Item = textBox39.Text;
                    S1.Car_Factura = Doc.Com_Cotiza.ToString();
                    S1.Car_Estado = "C";
                    S1.Car_Tipo = "Cotizacion";
                    S1.Car_Tipo_Doc = "CO";
                    S1.Car_Usr_Graba = Comunes.Contenedor.UsuarioLogueado;

                    string InsertServ = "";

                    
                        InsertServ = repositorioCargos.insertService(S1);
                    

                    if (InsertServ != "OK")
                    {
                        MessageBox.Show("Inconveniente con este item 16 seleccionado", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                //CARGO 17
                if (textBox37.Text != "" && textBox36.Text != "" && textBox35.Text != "")
                {
                    var Val = Valor(textBox37.Text); //izquierda
                    TotCar = Val * Convert.ToInt32(textBox35.Text); //derecha

                    S1.Car_Cant = Convert.ToInt32(textBox35.Text);
                    S1.Car_Val_Tot = Convert.ToInt32(TotCar);
                    S1.Car_Pac = Convert.ToInt32(Id);
                    S1.Car_Cia = Convert.ToInt32(cia);
                    S1.Car_Ase = Convert.ToInt32(Ase);
                    S1.Car_Cod = textBox37.Text;
                    S1.Car_Item = textBox36.Text;
                    S1.Car_Factura = Doc.Com_Cotiza.ToString();
                    S1.Car_Estado = "C";
                    S1.Car_Tipo = "Cotizacion";
                    S1.Car_Tipo_Doc = "CO";
                    S1.Car_Usr_Graba = Comunes.Contenedor.UsuarioLogueado;

                    string InsertServ = "";

                    
                        InsertServ = repositorioCargos.insertService(S1);
                    

                    if (InsertServ != "OK")
                    {
                        MessageBox.Show("Inconveniente con este item 17 seleccionado", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                //CARGO 18
                if (textBox34.Text != "" && textBox33.Text != "" && textBox32.Text != "")
                {
                    var Val = Valor(textBox34.Text); //izquierda
                    TotCar = Val * Convert.ToInt32(textBox32.Text); //derecha

                    S1.Car_Cant = Convert.ToInt32(textBox32.Text);
                    S1.Car_Val_Tot = Convert.ToInt32(TotCar);
                    S1.Car_Pac = Convert.ToInt32(Id);
                    S1.Car_Cia = Convert.ToInt32(cia);
                    S1.Car_Ase = Convert.ToInt32(Ase);
                    S1.Car_Cod = textBox34.Text;
                    S1.Car_Item = textBox33.Text;
                    S1.Car_Factura = Doc.Com_Cotiza.ToString();
                    S1.Car_Estado = "C";
                    S1.Car_Tipo = "Cotizacion";
                    S1.Car_Tipo_Doc = "CO";
                    S1.Car_Usr_Graba = Comunes.Contenedor.UsuarioLogueado;

                    string InsertServ = "";

                    
                        InsertServ = repositorioCargos.insertService(S1);
                    

                    if (InsertServ != "OK")
                    {
                        MessageBox.Show("Inconveniente con este item 18 seleccionado", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                CXN_FACTURA F = new CXN_FACTURA();
                F.Fac_Tipo_Doc = "CO";
                F.Fac_Num_Fac = Convert.ToInt32(Doc.Com_Cotiza);
                F.Fac_Cia = cia;
                F.Fac_Pac = Id;
                F.Fac_Res = "N/A";
                F.Homologo = Doc.Com_Cotiza.ToString();
                F.Fac_Ase = 99;
                F.Fac_Observa = "COTIZACION DE SERVICIOS";
                F.Fac_Usr_Graba = Comunes.Contenedor.UsuarioLogueado;
                F.Fac_Estado = "C";

                string _insertDoc = "";

                
                    _insertDoc = repositorioFactura.insertarDocumento(F);
                

                if (_insertDoc != "OK")
                {
                    MessageBox.Show("Se agrego todo el repertorio de servicios pero no se logro grabar el documento como cotizacion.  " +
                        "Reporte inmediatamente al administrador de sistema antes de continuar con la siguiente cotizacion.",
                        "Error Grave",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }

                int Nuevo = Convert.ToInt32(Doc.Com_Cotiza) + 1;

                bool ACCON = false;

                
                    ACCON = repositorioCompañias.ConsecutivoActualiza(cia, "COTIZA", Nuevo);
                                
                
                if (ACCON != true)
                {
                    MessageBox.Show("Se genero su documento tipo cotizacion pero no se logro actualizar el consecutivo siguiente, " +
                        "este es un error grave, debe reportar inmediato al administrador del sistema antes de crear otra cotizacion nueva",
                        "ADVERTENCIA!!!",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }

                MessageBox.Show("Hecho, numero de documento: " + Doc.Com_Cotiza,
                                    "Facturado Exitoso",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Exclamation);

                DialogResult result2 = MessageBox.Show("¿Desea Imprimir el Documento?", "Zamenis Health - Cotizaciones", MessageBoxButtons.YesNo);
                if (result2 == DialogResult.Yes)
                {
                    List<CotizacionR> Exporta = new List<CotizacionR>();

                    
                        Exporta = repositorioCargos.GenerarDocumento(Convert.ToInt32(Doc.Com_Cotiza), cia);
                    

                    if (Exporta == null)
                    {
                        MessageBox.Show("No se logro exportar el documento, por favor ingrese por la opcion de copias",
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        this.Dispose();
                        this.Close();
                    }

                    ConfigForm.GenerarReportViewer("DataSet_Cotizacion",
               "ZamenisHealth.Reportes.RDLC_Cotizacion.rdlc",
               Exporta);

                    this.Dispose();
                    this.Close();
                }
                if (result2 == DialogResult.No)
                {
                    this.Dispose();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            try
            {
                CXN_PACIENTES Datos = new CXN_PACIENTES();

                
                    Datos = repositorioPacientes.LlamarPacienteDOC(comboBox1.Text, textBox61.Text);
                

                if (Datos == null)
                {
                    Id = 0;
                    label14.Text = "";
                    MessageBox.Show("Paciente no existe", "Sin datos en sistema", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    CrearEditarPaciente Crear_Paciente = new CrearEditarPaciente();
                    Crear_Paciente.Size = new Size(619, 672);
                    Crear_Paciente.StartPosition = FormStartPosition.CenterScreen;
                    Crear_Paciente.FormBorderStyle = FormBorderStyle.FixedSingle;
                    Crear_Paciente.AutoScroll = false;
                    Crear_Paciente.ShowDialog();
                    return;
                }
                Id = Convert.ToInt32(Datos.Pac_Id);
                label14.Text = Datos.Pac_PrimerN + " " + Datos.Pac_SegundoN + " " + Datos.Pac_PrimerA + " " + Datos.Pac_SegundoA;
            }
            catch (Exception ex)
            {
                TXTException T = new TXTException { FechaHora = DateTime.Now, Error = ex.Message, Formulario = this.Name, Metodo = OverridesExtern.GetCurrentMethodName(), Usuario = Contenedor.UsuarioLogueado }; OverridesExtern.GenerarTXTException(T);
            }
        }
       

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            ListaCarga CargaPlant = new ListaCarga();

            
                CargaPlant = repositorioCargos.Carga_Plantilla();
            

            if (CargaPlant == null)
            {
                textBox5.Text = "";
                textBox8.Text = "";
                textBox13.Text = "";
                textBox16.Text = "";
                textBox19.Text = "";
                textBox22.Text = "";
                textBox25.Text = "";
                textBox28.Text = "";
                textBox31.Text = "";
                textBox58.Text = "";
                textBox55.Text = "";
                textBox52.Text = "";
                textBox49.Text = "";
                textBox46.Text = "";
                textBox43.Text = "";
                textBox40.Text = "";
                textBox37.Text = "";
                textBox34.Text = "";

                textBox7.Text = "";
                textBox10.Text = "";
                textBox11.Text = "";
                textBox14.Text = "";
                textBox17.Text = "";
                textBox20.Text = "";
                textBox23.Text = "";
                textBox26.Text = "";
                textBox29.Text = "";
                textBox56.Text = "";
                textBox53.Text = "";
                textBox50.Text = "";
                textBox47.Text = "";
                textBox44.Text = "";
                textBox41.Text = "";
                textBox38.Text = "";
                textBox35.Text = "";
                textBox32.Text = "";

                MessageBox.Show("Error cargando plantilla",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            textBox5.Text = CargaPlant.Cod1.ToString();
            textBox8.Text = CargaPlant.Cod2.ToString();
            textBox13.Text = CargaPlant.Cod3.ToString();
            textBox16.Text = CargaPlant.Cod4.ToString();
            textBox19.Text = CargaPlant.Cod5.ToString();
            textBox22.Text = CargaPlant.Cod6.ToString();
            textBox25.Text = CargaPlant.Cod7.ToString();
            textBox28.Text = CargaPlant.Cod8.ToString();
            textBox31.Text = CargaPlant.Cod9.ToString();
            textBox58.Text = CargaPlant.Cod10.ToString();
            textBox55.Text = CargaPlant.Cod11.ToString();
            textBox52.Text = CargaPlant.Cod12.ToString();
            textBox49.Text = CargaPlant.Cod13.ToString();
            textBox46.Text = CargaPlant.Cod14.ToString();
            textBox43.Text = CargaPlant.Cod15.ToString();
            textBox40.Text = CargaPlant.Cod16.ToString();
            textBox37.Text = CargaPlant.Cod17.ToString();
            textBox34.Text = CargaPlant.Cod18.ToString();

            textBox7.Text = CargaPlant.Can1.ToString();
            textBox10.Text = CargaPlant.Can2.ToString();
            textBox11.Text = CargaPlant.Can3.ToString();
            textBox14.Text = CargaPlant.Can4.ToString();
            textBox17.Text = CargaPlant.Can5.ToString();
            textBox20.Text = CargaPlant.Can6.ToString();
            textBox23.Text = CargaPlant.Can7.ToString();
            textBox26.Text = CargaPlant.Can8.ToString();
            textBox29.Text = CargaPlant.Can9.ToString();
            textBox56.Text = CargaPlant.Can10.ToString();
            textBox53.Text = CargaPlant.Can11.ToString();
            textBox50.Text = CargaPlant.Can12.ToString();
            textBox47.Text = CargaPlant.Can13.ToString();
            textBox44.Text = CargaPlant.Can14.ToString();
            textBox41.Text = CargaPlant.Can15.ToString();
            textBox38.Text = CargaPlant.Can16.ToString();
            textBox35.Text = CargaPlant.Can17.ToString();
            textBox32.Text = CargaPlant.Can18.ToString();

            textBox5.Select();
            textBox8.Select();
            textBox13.Select();
            textBox16.Select();
            textBox19.Select();
            textBox22.Select();
            textBox25.Select();
            textBox28.Select();
            textBox31.Select();
            textBox58.Select();
            textBox55.Select();
            textBox52.Select();
            textBox49.Select();
            textBox46.Select();
            textBox43.Select();
            textBox40.Select();
            textBox37.Select();
            textBox34.Select();
        }

        private void CargarDocumentos()
        {
            List<string> ListaDocs = new List<string>();

            
                ListaDocs = repositorioPacientes.ListaDocs();
            

            if (ListaDocs != null)
            {
                foreach (var i in ListaDocs)
                {
                    comboBox1.Items.Add(i);
                }
            }
        }

        int Valor(string Cod)
        {
            try
            {
                (int valor, string item, string detalle) datosProd = (0, "", "");

                
                    datosProd = repositorioInventario.ConsultarValor(Ase, Cod);
                

                return datosProd.valor;
            }
            catch
            {
                return 0;
            }
        }

    }
}
