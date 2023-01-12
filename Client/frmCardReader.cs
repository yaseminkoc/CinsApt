using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    public partial class frmCardReader : Form
    {

        public frmCardReader()
        {
            InitializeComponent();
            btnAdd.Enabled = false;

        }

        public String Textb()
        {
            return textBox1.Text;
        }

        public void slblU(String v)
        {
            lblName.Text = v;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            btnAdd.Enabled = true; 
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
        }

        private void formLogin_Load(object sender, EventArgs e)
        {
            
  
        }


    }
}
