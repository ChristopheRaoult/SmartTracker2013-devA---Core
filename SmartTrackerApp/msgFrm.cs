using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace smartTracker
{
    public partial class msgFrm : Form
    {
        public bool BUserClose = false;
        public string name;
        private string txt;
        public msgFrm(string txt,string name )
        {
            InitializeComponent();
            this.txt = txt;
            this.name = name;
        }

        private void msgFrm_Load(object sender, EventArgs e)
        {
            textBox1.Text = txt;
            this.Text = "LED On " + name;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            BUserClose = true;
            Close();
        }

        
    }
}
