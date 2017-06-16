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

namespace cli
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        CTcpClient cli;
        List<byte[]> _pkgs;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            int pkgs = 128;

            _pkgs = new List<byte[]>();
            for (int i = 0; i < pkgs; ++i)
            {
                var pkg = new byte[128];
                pkg[0] = (byte)(i % 128);
                _pkgs.Add(pkg);
            }

            cli = new CTcpClient(new CTcpClientArgs()
            {
                SendBufferSize = 8192,
                ReceiveBufferSize = 8192,
                SendQueueSize = 1024,
                RecvQueueSize = 1024,
                Log = null,
            });

            cli.Connected += Cli_Connected;
            cli.Closed += Cli_Closed;
            cli.Recved += Cli_Recved;

            cli.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9527));

            timerExec.Tick += TimerExec_Tick;
        }

        private void Cli_Connected(object sender, CDataEventArgs<CTcpClientConnectArgs> e)
        {

        }
        private void Cli_Closed(object sender, CDataEventArgs<CTcpClientCloseArgs> e)
        {
        }
        private void Cli_Recved(byte[] arg1, int arg2, int arg3)
        {
        }

        private void TimerExec_Tick(object sender, EventArgs e)
        {
            cli.Exec();

            foreach (var i in _pkgs)
            {
                cli.Send(i, 0, i.Length);
            }
        }
    }
}
