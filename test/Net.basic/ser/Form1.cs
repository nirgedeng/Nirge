﻿/*------------------------------------------------------------------
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
using log4net;
using System;

namespace ser
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        ILog _log;

        CTcpServer _ser;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            _log = LogManager.Exists("all");

            _ser = new CTcpServer(_log);

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
            var cmd = arg2[0];

            switch (cmd)
            {
            case 250:
                _log.InfoFormat("OnClosed(cmd), {0},{1}", cli, cmd);
                break;
            default:
                _ser.Send(cli, arg2, arg3, arg4);
                break;
            }
        }

        private void Ser_CliClosed(object sender, CDataEventArgs<int, CTcpClientCloseArgs> e)
        {
            var cli = e.Arg1;

            _log.InfoFormat("OnClosed, {0},{1},{2},{3}", cli, e.Arg2.Reason, e.Arg2.Error, e.Arg2.SocketError);
        }

        private void Ser_CliConnected(object sender, CDataEventArgs<int> e)
        {
            var cli = e.Arg1;

            _log.InfoFormat("OnConnected, {0}", cli);
        }
    }
}
