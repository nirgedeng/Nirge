/*------------------------------------------------------------------
    Copyright © : All rights reserved
    Author      : 邓晓峰
------------------------------------------------------------------*/

using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Linq;
using System.Text;
using System.Net;
using Nirge.Core;
using System;

namespace cli
{
    public class Cli1
    {
        CTcpClient _cli;
        List<byte[]> _pkgs;

        int _tick;
        int _bytesPerSecond;
        int _pkgSize;
        int _pkgsPerSecond;
        int _pkgsPerOnce;

        public Cli1()
        {
            _pkgs = new List<byte[]>();

            for (int i = 0; i < 1024; ++i)
            {
                var size = i % 255 + 1;

                var pkg = new byte[size];
                pkg[0] = (byte)size;
                _pkgs.Add(pkg);
            }

            _tick = 10;
            _bytesPerSecond = 4 * 1024 * 1024;
            _pkgSize = 100;
            _pkgsPerSecond = _bytesPerSecond / _pkgSize;
            _pkgsPerOnce = _pkgsPerSecond / (1000 / _tick);
            if (_pkgsPerOnce > _pkgs.Count)
                _pkgsPerOnce = _pkgs.Count;
        }

        public void Connect(IPEndPoint addr)
        {
            _cli = new CTcpClient(null);

            _cli.Connected += OnConnected;
            _cli.Closed += OnClosed;
            _cli.Recved += OnRecvd;

            _cli.Connect(addr);
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
        public void Exec()
        {
            _cli.Exec();

            switch (_cli.State)
            {
            case eTcpClientState.Connected:
                for (var i = 0; i < _pkgsPerOnce; ++i)
                {
                    _cli.Send(_pkgs[i], 0, _pkgs[i].Length);
                }

                break;
            }
        }
    }
}
