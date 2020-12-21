using System;
using System.Windows.Forms;
using BytesRoad.Net.Ftp;

namespace ftp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        FtpClient client;

        int TimeoutFTP; //Таймаут
        string FTP_SERVER;
        int FTP_PORT;
        string FTP_USER;
        string FTP_PASSWORD;

        private void button1_Click(object sender, EventArgs e)
        {
            client = new FtpClient();
            client.PassiveMode = true;

            TimeoutFTP = 30000; //Таймаут
            FTP_SERVER = "localhost";
            FTP_PORT = 21;
            FTP_USER = "student";
            FTP_PASSWORD = "student";

            //Подключаемся к FTP серверу. 
            try
            {
                client.Connect(TimeoutFTP, FTP_SERVER, FTP_PORT);
            }
            catch (BytesRoad.Net.Ftp.FtpTimeoutException error)
            {
                MessageBox.Show("Время ожидания истекло! Сервер не отвечает. "
                    + error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            catch (System.Net.Sockets.SocketException error)
            {
                MessageBox.Show(error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                client.Login(TimeoutFTP, FTP_USER, FTP_PASSWORD);
            }
            catch (BytesRoad.Net.Ftp.FtpTimeoutException error)
            {
                MessageBox.Show("Время ожидания истекло! Сервер не отвечает. "
                    + error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            catch
            (System.Net.Sockets.SocketException error)
            {
                MessageBox.Show(error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            otris();
            /*
            if (dirs.Count() > 0)
            {
                saveFileDialog1.FileName = dirs[0].Name;
                if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    client.GetFile(TimeoutFTP, saveFileDialog1.FileName, dirs[0].Name);
                }
            }*/
        }

        private void otris()
        {
            dataGridView1.Rows.Clear();
            dataGridView1.Rows.Add("...");            

            FtpItem[] dirs = client.GetDirectoryList(TimeoutFTP);
            foreach (FtpItem dir in dirs)
            {
                dataGridView1.Rows.Add(dir.Name, dir.ItemType, dir.Size);
            }
        }


        private void dataGridView1_CellContentDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            FtpItem[] dirs = client.GetDirectoryList(TimeoutFTP);
            //проверка на тип
            if (dataGridView1.CurrentCell.RowIndex > 0 && dirs[dataGridView1.CurrentCell.RowIndex-1].ItemType == FtpItemType.Directory)
            {
                client.ChangeDirectory(TimeoutFTP, dataGridView1[0, dataGridView1.CurrentCell.RowIndex].Value.ToString());
                otris();
            }
            else if (dataGridView1.CurrentCell.RowIndex == 0)
            { client.ChangeDirectory(TimeoutFTP, "...");
                otris();
            }
            else MessageBox.Show("Нельзя перейти это файл");
        }
    }
}
