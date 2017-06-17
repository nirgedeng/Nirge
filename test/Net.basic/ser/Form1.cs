﻿using System;
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

        CTcpServer _ser;
        Cli1 _cli1;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            _cli1 = new Cli1(1);

            _ser = new CTcpServer(new CTcpServerArgs()
            {
                SendBufferSize = 8192,
                ReceiveBufferSize = 8192,
                SendQueueSize = 1024,
                RecvQueueSize = 1024,
                Capacity = 1024,
            }, null);

            _ser.Closed += Ser_Closed;
            _ser.CliConnected += Ser_CliConnected;
            _ser.CliClosed += Ser_CliClosed;
            _ser.CliRecved += Ser_CliRecved;

            _ser.Open(new IPEndPoint(IPAddress.Any, 9527));

            timerExec.Tick += TimerExec_Tick;
        }

        private void TimerExec_Tick(object sender, EventArgs e)
        {
            for (int i = 0; i < 8; ++i)
                _ser.Exec();
        }

        private void Ser_Closed(object sender, CDataEventArgs<CTcpServerCloseArgs> e)
        {
        }

        private void Ser_CliRecved(object sender, int cli, byte[] arg2, int arg3, int arg4)
        {
            if (cli == _cli1._cli)
                _cli1.OnRecved(arg2, arg3, arg4);
        }

        private void Ser_CliClosed(object sender, CDataEventArgs<int, CTcpClientCloseArgs> e)
        {
            var cli = e.Arg1;

            if (cli == _cli1._cli)
                _cli1.OnClosed(e.Arg2);

        }

        private void Ser_CliConnected(object sender, CDataEventArgs<int> e)
        {
            var cli = e.Arg1;

            if (cli == _cli1._cli)
                _cli1.OnConnected();
        }
    }
}
