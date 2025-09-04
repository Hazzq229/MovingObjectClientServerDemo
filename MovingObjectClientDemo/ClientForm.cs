using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MovingObjectClientDemo
{
    public partial class ClientForm : Form
    {

        Pen red = new Pen(Color.Red);
        Rectangle rect = new Rectangle(20, 20, 30, 30);
        SolidBrush fillBlue = new SolidBrush(Color.Blue);

        private Socket clientSocket;
        private byte[] buffer = new byte[4];


        public ClientForm()
        {
            InitializeComponent();
            ConnectToServer();
        }

        private void ConnectToServer()
        {
            try
            {
                clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                clientSocket.Connect("127.0.0.1", 3333);

                clientSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, ReceiveCallback, null);
            }
            catch (SocketException ex)
            {
                MessageBox.Show("Error connecting: " + ex.Message);
            }
        }
        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                int received = clientSocket.EndReceive(ar);
                if (received > 0)
                {
                    int posX = BitConverter.ToInt32(buffer, 0);
                    rect.X = posX;
                    Invalidate();
                }

                clientSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, ReceiveCallback, null);
            }
            catch (SocketException exception)
            {
                MessageBox.Show("Connection lost.");
            }
        }
        private void ClientForm_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            g.DrawRectangle(red, rect);
            g.FillRectangle(fillBlue, rect);
        }
    }
}