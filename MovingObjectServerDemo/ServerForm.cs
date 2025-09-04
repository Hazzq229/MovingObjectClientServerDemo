using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;


namespace MovingObjectServerDemo

{
    public partial class ServerForm : Form
    {

        Pen red = new Pen(Color.Red);
        Rectangle rect = new Rectangle(20, 20, 30, 30);
        SolidBrush fillBlue = new SolidBrush(Color.Blue);
        int slide = 10;
        private Socket serverSocket;
        private List<Socket> clientSockets = new List<Socket>();
        private byte[] buffer;

        public ServerForm()
        {
            InitializeComponent();
            StartServer();
            timer1.Interval = 50;
            timer1.Enabled = true;
        }
        private static void ShowErrorDialog(string message)
        {
            MessageBox.Show(message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        private void StartServer()
        {
            try
            {
                serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                serverSocket.Bind(new IPEndPoint(IPAddress.Any, 3333));
                serverSocket.Listen(10);
                serverSocket.BeginAccept(AcceptCallback, null);
            }
            catch (SocketException ex)
            {
                ShowErrorDialog(ex.Message);
            }
            catch (ObjectDisposedException ex)
            {
                ShowErrorDialog(ex.Message);
            }
        }
        private void AcceptCallback(IAsyncResult ar)
        {
            try
            {
                Socket client = serverSocket.EndAccept(ar);
                clientSockets.Add(client);

                byte[] initData = BitConverter.GetBytes(rect.X);
                client.Send(initData);

                serverSocket.BeginAccept(AcceptCallback, null);
            }
            catch (SocketException ex)
            {
                MessageBox.Show(ex.Message, "Socket Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (ObjectDisposedException ex)
            {
                ShowErrorDialog(ex.Message);
            }
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            back();

            rect.X += slide;
            Invalidate();

            BroadcastPosition();
        }
        private void BroadcastPosition()
        {
            byte[] data = BitConverter.GetBytes(rect.X);
            foreach (var c in clientSockets.ToList())
            {
                try { c.Send(data); }
                catch { clientSockets.Remove(c); }
            }
        }

        private void back()
        {
            if (rect.X >= this.Width - rect.Width * 2)
                slide = -10;
            else
            if (rect.X <= rect.Width / 2)
                slide = 10;
        }

        private void ServerForm_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            g.DrawRectangle(red, rect);
            g.FillRectangle(fillBlue, rect);
        }
    }
}