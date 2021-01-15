using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.IO;


namespace WindowsFormsApplication3
{
    public partial class Form1 : Form
    {
        private TcpClient client;
        public StreamReader streamread;
        public StreamWriter streamwrite;
        public string receive;
        public string textsend;

        public Form1()
        {
            InitializeComponent();
            IPAddress[] localIP = Dns.GetHostAddresses(Dns.GetHostName());
            foreach(IPAddress address in localIP)
            {
                if (address.AddressFamily == AddressFamily.InterNetwork)
                    ServerIP.Text = address.ToString();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            TcpListener listener = new TcpListener(IPAddress.Any, int.Parse(ServerPort.Text));
            listener.Start();
            client=listener.AcceptTcpClient();
            streamread = new StreamReader(client.GetStream());
            streamwrite = new StreamWriter(client.GetStream());
            streamwrite.AutoFlush = true;
            backgroundWorker1.RunWorkerAsync();
            backgroundWorker2.WorkerSupportsCancellation = true;

        }

        private void button2_Click(object sender, EventArgs e)
        {
            client = new TcpClient();
            IPEndPoint IPEnd = new IPEndPoint(IPAddress.Parse(ClientIP.Text), int.Parse(ClientPort.Text));
            try
            {
                client.Connect(IPEnd);
                if (client.Connected)
                {
                    Conversation.AppendText("Connected to Server" + "\n");
                    streamread = new StreamReader(client.GetStream());
                    streamwrite = new StreamWriter(client.GetStream());
                    streamwrite.AutoFlush=true;
                    backgroundWorker1.RunWorkerAsync();
                    backgroundWorker2.WorkerSupportsCancellation = true;


                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void Conversation_TextChanged(object sender, EventArgs e)
        {
          

        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            while (client.Connected)
            {
                try
                {
                    receive = streamread.ReadLine();
                    this.Conversation.Invoke(new MethodInvoker(delegate(){Conversation.AppendText("You:" + receive + "\n");}));
                    receive = "";
                        
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }

            }
        }

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            if (client.Connected)
            {
                streamwrite.WriteLine (textsend);
                this.Conversation.Invoke(new MethodInvoker(delegate() { Conversation.AppendText("Me:" + textsend + "\n"); }));
            }
            else
            {
                MessageBox.Show("Failed to send...");
            }
            backgroundWorker2.CancelAsync();
        }

        private void Sendbtn_Click(object sender, EventArgs e)
        {
            if (MessageText.Text != "")
            {
                textsend = MessageText.Text;
                backgroundWorker2.RunWorkerAsync();
            }
            MessageText.Text = "";
        }
    }
}
