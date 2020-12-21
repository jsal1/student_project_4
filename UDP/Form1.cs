using System;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Threading;

namespace udp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        int fSize = 0;
        FileStream fStream = null;
        int bytesWrited = 0;
        string filename = "";
        //Создание слушающего UDP сокета
        private void StartListener(int port)
        {
            //Создание конечной точки подключения   
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, port);
            //Создание слушающего UDP сокета  
            UdpClient listener = new UdpClient(endPoint);
            //Установка свойств UDP сокета  
            UDPState listenerState = new UDPState();
            listenerState.client = listener;
            listenerState.address = endPoint;
            //Начало операции приема данных 
            bytesWrited = 0;
            fSize = 0;
            fStream = null;
            listener.BeginReceive(new AsyncCallback(ReceiveCallback), listenerState);
        }

        //Класс для описания UDP сокета
        public class UDPState
        {
            public UdpClient client = null;
            public IPEndPoint address = null;
        }
        //Прием файла
        private void ReceiveCallback(IAsyncResult ar)
        {

            //Создание UDP сокета для приема данных
            UdpClient client = (UdpClient)((UDPState)(ar.AsyncState)).client;
            IPEndPoint address = (IPEndPoint)((UDPState)(ar.AsyncState)).address;
            //Буфер для приема байт данных
            byte[] receivedBytes = client.EndReceive(ar, ref address);
            //Строковый буфер для принимаемых данных   
            string receivedString = Encoding.ASCII.GetString(receivedBytes);
            //Проверка типа пришедшей дэйтаграммы  
            //если дэйтаграмма начинается с символов #file#,  
            //то требуется начать процедуру записи в файл  
            //если дэйтаграмма начинается с символов #endfile#,  
            //то требуется завершить процедуру записи в файл 
            //во всех остальных случаях записать принятые байты в файл 
            //(отправка таких дэйтаграмм реализована отдельно) 

            if (receivedString.StartsWith("#file#"))
            {
                //Определение имени и размера принимаемого файла  
                //формат дэйтаграммы #file#имя_файла#размер#     
                string str = receivedString.Substring(6, receivedString.Length - 6);
                string fName = str.Substring(0, str.IndexOf("#"));
                string[] fN = fName.Split(char.Parse(@"\"));
                filename = fName = fN[fN.Length - 1];
                str = str.Substring(str.IndexOf("#") + 1);
                fSize = int.Parse(str.Substring(0, str.Length - 1));
                //Создание потока на запись в файл  
                fStream = new FileStream(Environment.CurrentDirectory+ @"\" + fName, FileMode.Create, FileAccess.Write);
            }
            else if (receivedString.StartsWith("#endfile#"))
            {     //формат дэйтаграммы #endfile#  
                fStream.Dispose();
                MessageBox.Show("Принят файл: " + filename + "\r\nФайл сохранен в месте запуска программы ");
            }
            else
            {
                //прием файла

                fStream.WriteAsync(receivedBytes, 0, 1024);
                bytesWrited += receivedBytes.Length;
            }
            //Начало операции приема следующей порции данных  
            client.BeginReceive(new AsyncCallback(ReceiveCallback), ((UDPState)(ar.AsyncState)));
        }

        //Отправка файла
        private void SendFile(string fName, string sAddr, int sPort)
        {
            //Открытие файла и определение его размера 
            FileInfo fInfo = new FileInfo(fName);
            int fSize = Convert.ToInt32(fInfo.Length);
            //Настройка UDP клиента 
            UdpClient client = new UdpClient(sAddr, sPort);
            //Отправка уведомления о передаче файла  
            byte[] sendBytes = Encoding.ASCII.GetBytes("#file#" + fName + "#" + Convert.ToString(fSize) + "#");
            try
            {
                client.Send(sendBytes, sendBytes.Length);
            }
            catch (Exception err)
            {
                //Вывод сообщения об ошибке  
                MessageBox.Show("err\r\n" + err);
            }
            //Создание потока на чтение из файла
            FileStream fStream = fInfo.OpenRead();
            //Количество байт, считанных из файла  
            int bytesReaded = 0;
            //Буфер для хранения байт, считанных из файла
            byte[] bytesArray = new byte[1024];

            //Пока количество считанных байт меньше размера файла
            while (bytesReaded < fSize)
            {
                //Отправка части файла
                bytesReaded += fStream.Read(bytesArray, 0, 1024);
                client.Send(bytesArray, bytesArray.Length);
                //timeout
                Thread.Sleep(1);
            }
            //Отправка уведомления о завершении передачи файла 
            sendBytes = Encoding.ASCII.GetBytes("#endfile#");
            client.Send(sendBytes, sendBytes.Length);
            button1.Enabled = true;
            textBox3.Enabled = true;
            textBox2.Enabled = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {//принять

            if (textBox1.Text != null && textBox1.Text != "")
            {
                button2.Text = "ОЖИДАНИЕ ...";
                //block b1
                button1.Enabled = false;
                button2.Enabled = false;
                textBox1.Enabled = false;
                textBox3.Enabled = false;
                textBox2.Enabled = false;
                //--
                StartListener(int.Parse(textBox1.Text));
            }
        }
        
        private void button1_Click(object sender, EventArgs e)
        {//отправить

            if (textBox2.Text != null && textBox2.Text != "" &&
                textBox3.Text != null && textBox3.Text != "")
            {
                //block b2
                button2.Enabled = false;
                button2.Enabled = false;
                textBox1.Enabled = false;
                textBox3.Enabled = false;
                textBox2.Enabled = false;
                //--
                using (OpenFileDialog ofd = new OpenFileDialog())
                {
                    ofd.InitialDirectory = @"c:\";
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        SendFile(ofd.FileName, textBox2.Text, int.Parse(textBox3.Text));
                    }
                }
                
            }
        }
    }
}
