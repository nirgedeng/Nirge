/*------------------------------------------------------------------
    Copyright © : All rights reserved
    Author      : 邓晓峰
------------------------------------------------------------------*/

using System.Collections.Generic;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using Nirge.Core;
using System.Net;
using System;
using log4net;

namespace cli
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        ILog _log;

        List<byte[]> _pkgs;

        CTcpClient _cli;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            _log = LogManager.Exists("all");

            _pkgs = new List<byte[]>();
            for (int i = 0; i < 10240; ++i)
            {
                var size = i % 255 + 1;

                var pkg = new byte[size];
                pkg[0] = (byte)size;
                _pkgs.Add(pkg);
            }

            _cli = new CTcpClient(null);
            _cli.Connected += OnConnected;
            _cli.Closed += OnClosed;
            _cli.Recved += OnRecvd;
            _cli.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9527));

            timerExec.Tick += TimerExec_Tick;
        }

        void OnConnected(object sender, CDataEventArgs<CTcpClientConnectArgs> e)
        {
            _log.InfoFormat("OnConnect {0}:{1}:{2}", e.Arg1.Result, e.Arg1.Error, e.Arg1.SocketError);

            foreach (var i in _pkgs)
                _cli.Send(i, 0, i.Length);
        }

        void OnClosed(object sender, CDataEventArgs<CTcpClientCloseArgs> e)
        {
            _log.InfoFormat("OnClosed {0}:{1}:{2}", e.Arg1.Reason, e.Arg1.Error, e.Arg1.SocketError);
        }

        void OnRecvd(object sender, byte[] arg1, int arg2, int arg3)
        {
            _cli.Send(arg1, arg2, arg3);
        }

        private void TimerExec_Tick(object sender, EventArgs e)
        {
            for (int i = 0; i < 8; ++i)
                _cli.Exec();
        }
    }
}
