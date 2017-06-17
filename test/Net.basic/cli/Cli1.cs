using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using Nirge.Core;

namespace cli
{
    public class Cli1
    {
        CTcpClient _cli;
        List<byte[]> _pkgs;
        int _batch;

        public Cli1()
        {
            _pkgs = new List<byte[]>();

            for (int i = 1; i <= 200; ++i)
            {
                var pkg = new byte[i + 1000];
                pkg[0] = (byte)i;
                _pkgs.Add(pkg);
            }
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
            if (_cli == null)
                return;

            _cli.Exec();

            for (int i = _batch * 20; i < (_batch + 1) * 20; ++i)
            {
                _cli.Send(_pkgs[i], 0, _pkgs[i].Length);
            }

            _batch = ++_batch % 10;
        }

    }
}
