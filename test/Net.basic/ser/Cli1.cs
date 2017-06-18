/*------------------------------------------------------------------
    Copyright © : All rights reserved
    Author      : 邓晓峰
------------------------------------------------------------------*/

using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Linq;
using System.Text;
using Nirge.Core;
using System.Net;
using System;

namespace ser
{
    public class Cli1
    {
        public int _cli;

        public Cli1(int cli)
        {
            _cli = cli;
        }

        public void OnConnected()
        {
            Console.WriteLine("OnConnected, {0}", _cli);
        }

        public void OnClosed(CTcpClientCloseArgs e)
        {
            Console.WriteLine("OnClosed, {0},{1},{2},{3}", _cli, e.Reason, e.Error, e.SocketError);
        }

        public void OnRecved(byte[] buf, int offset, int count)
        {
            //Console.WriteLine("OnRecved, {0},{1},{2}", _cli, count, buf[0]);
        }
    }
}
