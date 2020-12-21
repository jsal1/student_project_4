using System;
using System.Windows.Forms;

namespace photoshop_mod_bar_graph
{
    public partial class WhoI : Form
    {
        public WhoI()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            I_client mainform = new I_client(this);
            mainform.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();
            I_am_server server = new I_am_server(this);
            server.Show();
        }
    }
}
