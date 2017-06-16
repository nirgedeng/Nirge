using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Nirge.Core;
using System.Net;
using System.Net.Sockets;

namespace ser
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        CTcpServer ser;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            ser = new CTcpServer(new CTcpServerArgs()
            {
                SendBufferSize = 8192,
                ReceiveBufferSize = 8192,
                SendQueueSize = 1024,
                RecvQueueSize = 1024,
                Capacity = 1024,
                Log = null,
            });

            ser.Closed += Ser_Closed;
            ser.CliConnected += Ser_CliConnected;
            ser.CliClosed += Ser_CliClosed;
            ser.CliRecved += Ser_CliRecved;


            ser.Open(new IPEndPoint(IPAddress.Any, 9527));

            timerExec.Tick += TimerExec_Tick;
        }

        private void TimerExec_Tick(object sender, EventArgs e)
        {
            ser.Exec();
        }

        private void Ser_Closed(object sender, CDataEventArgs<CTcpServerCloseArgs> e)
        {
        }

        private void Ser_CliRecved(int arg1, byte[] arg2, int arg3, int arg4)
        {
            Console.WriteLine("cli:{0}, pkg:{1},{2}", arg1, arg4, arg2[0]);
        }

        private void Ser_CliClosed(object sender, CDataEventArgs<int, CTcpClientCloseArgs> e)
        {
        }

        private void Ser_CliConnected(object sender, CDataEventArgs<int> e)
        {
        }


    }
}
