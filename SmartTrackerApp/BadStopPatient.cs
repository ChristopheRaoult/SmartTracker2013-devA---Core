using System;
using System.Windows.Forms;

namespace smartTracker
{
    public partial class BadStopPatient : Form
    {
        public BadStopPatient()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Close();
        }

        
    }
}
