using System;
using System.Windows.Forms;
using System.Net;


namespace photoshop_mod_bar_graph
{
    public partial class Connect_Server : Form
    {
        bool IPvalid = false,
            PORTvalid=false;
        public Connect_Server()
        {
            InitializeComponent();
            IPaddress.ValidatingType = typeof(IPAddress);
        }
                
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                int port = int.Parse(PORT.Text);
                PORTvalid = true;
            }
            catch { }
            if (IPvalid && PORTvalid)
            {                
                I_client.IP_servera = IPaddress.Text;
                I_client.port_servera = PORT.Text;
                this.Close();
            }
            else
            {
                MessageBox.Show("Не все поля верны");
            }
        }

        private void IPaddress_TypeValidationCompleted(object sender, TypeValidationEventArgs e)
        {
            if (IPaddress.MaskCompleted)
            {
                IPvalid = true;
            }            
        }
    }
}