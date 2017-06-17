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
using Nirge.Core;

namespace cli
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        int _clisCount;
        List<CTcpClient> _clis;

        List<byte[]> _pkgs;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            _clisCount = 1024;

            _clis = new List<CTcpClient>();
            for (var i = 0; i < _clisCount; ++i)
            {
                var cli = new CTcpClient(null);
                _clis.Add(cli);

                cli.Connected += OnConnected;
                cli.Closed += OnClosed;
                cli.Recved += OnRecvd;

                cli.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9527));
            }

            _pkgs = new List<byte[]>();

            for (int i = 0; i < 1024; ++i)
            {
                var size = i % 255 + 1;

                var pkg = new byte[size];
                pkg[0] = (byte)size;
                _pkgs.Add(pkg);
            }

            timerExec.Tick += TimerExec_Tick;

        }

        void OnConnected(object sender, CDataEventArgs<CTcpClientConnectArgs> e)
        {
            Console.WriteLine("OnConnect {0}:{1}:{2}", e.Arg1.Result, e.Arg1.Error, e.Arg1.SocketError);
        }

        void OnClosed(object sender, CDataEventArgs<CTcpClientCloseArgs> e)
        {
            Console.WriteLine("OnClosed {0}:{1}:{2}", e.Arg1.Reason, e.Arg1.Error, e.Arg1.SocketError);
        }

        void OnRecvd(object sender, byte[] arg1, int arg2, int arg3)
        {
        }

        private void TimerExec_Tick(object sender, EventArgs e)
        {
            for (int i = 0; i < _clis.Count; ++i)
            {
                var cli = _clis[i];

                cli.Exec();

                var p = i % 1024;
                cli.Send(_pkgs[p], 0, _pkgs[p].Length);
            }
        }
    }
}
