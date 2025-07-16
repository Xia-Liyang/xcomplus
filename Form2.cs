using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace xcomplus
{
    public partial class Form2 : Form
    {
        DataTable dataTable = new DataTable();
        string cnnect_string = "Data Source=localhost;Initial Catalog=sql_server_tempandsoon;Integrated Security=True";
        public Form2()
        {
            InitializeComponent();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void Form2_Load(object sender, EventArgs e)
        {
            sqlserver find = new sqlserver(dataTable);
            dataGridView1.DataSource = dataTable;
        }
    }
}
