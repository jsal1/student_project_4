using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace photoshop_mod_bar_graph
{
    public partial class I_client : Form
    {
        public static string IP_servera,
                             port_servera;
        Socket Client;
        Form main;

        public I_client(WhoI m)
        {
            InitializeComponent();
             main = m;
        }

        private void PACH_BUTTON_Click(object sender, EventArgs e)
        {
            OpenFileDialog OF = new OpenFileDialog();
            OF.Filter = "IMAGE (*.jpg)|*.jpg;*.png;*.bmp";
            if (OF.ShowDialog() == DialogResult.OK)
            {
                PICTURE.Image = Image.FromFile(OF.FileName);
                PACH_TB.Text = OF.FileName;
            }            
        }

        private void CONSOLE_TB_VisibleChanged(object sender, EventArgs e)
        {
            CONSOLE_TB.SelectionStart = CONSOLE_TB.TextLength;
            CONSOLE_TB.ScrollToCaret();
        }

        private void SEND_BUTTON_Click(object sender, EventArgs e)
        {
            bool connected = false;
            string temp_ip = "",
                temp_port = "";

            //перевод в полутон
            if (PICTURE.Image != null)
            {
                Bitmap picture = new Bitmap(PICTURE.Image);

                int f_R, f_G, f_B, f_SR;
                int[,] image2 = new int[picture.Width, picture.Height];
                for (int w = 0; w < picture.Width; w++)
                    for (int h = 0; h < picture.Height; h++)
                    {
                        f_R = (int)(picture.GetPixel(w, h).R); //Красный
                        f_G = (int)(picture.GetPixel(w, h).G); //Зеленый
                        f_B = (int)(picture.GetPixel(w, h).B); //Синий

                        f_SR = (int)(f_R + f_G + f_B) / 3;// Среднее
                        // Устанавливаем цвет
                        Color p = Color.FromArgb(255, f_SR, f_SR, f_SR);
                        picture.SetPixel(w, h, p);
                        image2[w, h] = f_SR;
                    }
                PICTURE.Image = picture;

                //ip-здесь
                Connect_Server server = new Connect_Server();
                server.ShowDialog();

                while (true)
                {
                    while (true)
                    {
                        if (IP_servera != temp_ip && port_servera != temp_port)
                        {

                            if (IP_servera != null)
                            {
                                string ip_new = null;
                                foreach (char s in IP_servera)
                                {

                                    char t = '.';
                                    if (s == ',')
                                        ip_new += t;
                                    else
                                        ip_new += s;
                                }
                                IP_servera = ip_new;
                            }
                            MessageBox.Show(IP_servera);
                            break;
                        }
                        else
                        { server.ShowDialog(); }
                    }

                    //отправка на сервер

                    try
                    {
                        IPAddress ipAdd = IPAddress.Parse(IP_servera);
                        int port = int.Parse(port_servera);
                        Client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        IPEndPoint ipEndAdd = new IPEndPoint(ipAdd, port);
                        Client.Connect(ipEndAdd);
                        if (Client.Connected)
                        {
                            connected = true;
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("err: \r\n" + ex);
                        break;
                    }
                }
                if (connected)
                {
                    try
                    {
                        MemoryStream ms = new MemoryStream();
                        PICTURE.Image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                         byte[] msg = ms.ToArray();
                        MessageBox.Show(msg.Length.ToString());
                        Client.Send(msg);

                        byte[] data = new byte[256]; // буфер для ответа
                        StringBuilder builder = new StringBuilder();
                        int bytes = 0; // количество полученных байт

                        do
                        {
                            bytes = Client.Receive(data, data.Length, 0);
                            builder.Append(Encoding.UTF8.GetString(data, 0, bytes));
                        }
                        while (Client.Available > 0);
                        int[] GIST = new int[256];
                        MessageBox.Show(builder.ToString());
                        string[] data_gist = (builder.ToString()).Split(';');
                        for (int i = 0; i < GIST.Length; i++)
                        {
                            GIST[i] = int.Parse(data_gist[i]);
                        }
                        MessageBox.Show("build");
                        CHART.Series[0].Points.DataBindY(GIST);
                        CHART.Series[0].Color = Color.DarkRed;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("err:\r\n" + ex);
                    }
                    finally
                    {
                        Client.Shutdown(SocketShutdown.Both);
                        Client.Close();
                    }
                }
            }
        }

        private void I_client_FormClosed(object sender, FormClosedEventArgs e)
        {
            main.Close();
        }
    }
}
