using System;
using System.Drawing;
using System.Windows.Forms;

namespace ZamenisHealth.Clases.Controles
{
    public partial class GridZH : UserControl
    {
        public bool CeldaHeight = false;

        public GridZH()
        {
            InitializeComponent();

            dataGridView1.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.EnableHeadersVisualStyles = false;
        }

        private void dataGridView1_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            try
            {
                if (dataGridView1.Rows.Count > 0)
                {
                    dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells + 10;
                    if (CeldaHeight == true)
                    {
                        dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
                    }
                    dataGridView1.DefaultCellStyle.WrapMode = DataGridViewTriState.True;

                    dataGridView1.ClearSelection();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void dataGridView1_Click(object sender, EventArgs e)
        {
            try
            {
                dataGridView1.ClearSelection();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }            
        }
        private void dataGridView1_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            if (e.RowIndex % 2 == 0)
            {
                dataGridView1.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.FromArgb(244, 244, 250);
            }
            else
            {
                dataGridView1.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.FromArgb(255, 255, 255);
            }
        }
        private void dataGridView1_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            try
            {
                foreach (DataGridViewColumn col in dataGridView1.Columns)
                {
                    col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    col.SortMode = DataGridViewColumnSortMode.NotSortable;
                }                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }            
        }
    }
    public class DataGridViewSinOrden : DataGridView
    {
        protected override void OnColumnHeaderMouseClick(DataGridViewCellMouseEventArgs e)
        {
            return;
        }

        protected override void OnSorted(EventArgs e)
        {
            return;
        }
    }
}
