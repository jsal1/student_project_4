using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace photoshop_mod_bar_graph
{
    public partial class I_am_server : Form
    {
        Form main;
        Socket Server;
        Socket Handler;
        public I_am_server(WhoI m)
        {
            InitializeComponent();
            main=m;
            Show();
            handler();

        }
        public void handler()
        {
            try
            {
                Server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                
                IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
                MessageBox.Show(ipHost.HostName.ToString());
                IPAddress IpAdd = ipHost.AddressList[1];
                MessageBox.Show(IpAdd.ToString());
                IPEndPoint IpEP = new IPEndPoint(IpAdd, 11111);

                Server.Bind(IpEP);
                Server.Listen(10);

                while (true)
                {
                                        
                    MessageBox.Show("Ожидаем соединение через порт:" + IpEP);
                    Handler = Server.Accept();

                    int bytes = 0;
                    byte[] data = new byte[500000];
                    StringBuilder builder = new StringBuilder();

                    do
                    {
                        Handler.Receive(data);
                    }
                    while (Handler.Available > 0);

                    MessageBox.Show(bytes.ToString());

                    MemoryStream ms = new MemoryStream(data);
                    Bitmap pic = new Bitmap(ms);
                    
                    //строим гистограмму
                    int[] GIST = new int[256];
                    for (int w = 0; w < pic.Width; w++)
                        for (int h = 0; h < pic.Height; h++)
                        {
                            int f_R, f_G, f_B, f_SR;
                            f_R = (int)(pic.GetPixel(w, h).R); //Красный
                            f_G = (int)(pic.GetPixel(w, h).G); //Зеленый
                            f_B = (int)(pic.GetPixel(w, h).B); //Синий

                            f_SR = (int)(f_R + f_G + f_B) / 3;// Среднее
                            GIST[f_SR]++;
                        }
                    string data_gist="";
                    foreach (int item in GIST)
                    {
                        data_gist += item.ToString()+";";
                        
                    }
                    MessageBox.Show(data_gist);

                    data = Encoding.UTF8.GetBytes(data_gist);
                    Handler.Send(data);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error :  " + ex);
                Environment.Exit(0);
            }
            finally
            {
                Server.Shutdown(SocketShutdown.Both);
                Server.Close();
                Handler.Shutdown(SocketShutdown.Both);
                Handler.Close();
            }
        }
        
        private void I_am_server_FormClosed(object sender, FormClosedEventArgs e)
        {
            main.Close();
        }
    }
}
