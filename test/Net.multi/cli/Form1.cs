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
using System.Net;
using Nirge.Core;
using log4net;
using System;

namespace cli
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        ILog _log;

        List<CTcpClient> _clis;

        List<byte[]> _pkgs;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            _log = LogManager.Exists("all");

            var clients = 1024;
            var pkgs = 2;

            _clis = new List<CTcpClient>();
            for (var i = 0; i < clients; ++i)
            {
                var cli = new CTcpClient(_log);
                _clis.Add(cli);

                cli.Connected += OnConnected;
                cli.Closed += OnClosed;
                cli.Recved += OnRecvd;

                cli.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9527));
            }

            _pkgs = new List<byte[]>();
            for (int i = 0; i < pkgs; ++i)
            {
                var size = i % 255 + 1 + 32;

                var pkg = new byte[size];
                pkg[0] = (byte)size;
                _pkgs.Add(pkg);
            }

            timerExec.Tick += TimerExec_Tick;
        }

        void OnConnected(object sender, CDataEventArgs<CTcpClientConnectArgs> e)
        {
            CTcpClient cli = (CTcpClient)sender;

            _log.InfoFormat("OnConnect {0}:{1}:{2}", e.Arg1.Result, e.Arg1.Error, e.Arg1.SocketError);

            foreach (var i in _pkgs)
                cli.Send(i, 0, i.Length);
        }

        void OnClosed(object sender, CDataEventArgs<CTcpClientCloseArgs> e)
        {
            CTcpClient cli = (CTcpClient)sender;

            _log.InfoFormat("OnClosed {0}:{1}:{2}", e.Arg1.Reason, e.Arg1.Error, e.Arg1.SocketError);
        }

        void OnRecvd(object sender, byte[] arg1, int arg2, int arg3)
        {
            CTcpClient cli = (CTcpClient)sender;

            cli.Send(arg1, arg2, arg3);
        }

        private void TimerExec_Tick(object sender, EventArgs e)
        {
            for (var i = 0; i < 8; ++i)
                Exec();
        }

        void Exec()
        {
            foreach (var i in _clis)
                i.Exec();
        }
    }
}
