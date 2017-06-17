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

        int _pkgSize;
        int _pkgsPerSecond;
        int _bytesPerSecond;
        int _tick;
        int _pkgsPerOnce;
        int _batchs;
        int _count;

        public Cli1()
        {
            _pkgs = new List<byte[]>();

            for (int i = 1; i <= 1024; ++i)
            {
                var pkg = new byte[i];
                pkg[0] = (byte)(i % 255);
                _pkgs.Add(pkg);
            }

            _bytesPerSecond = 2 * 1024 * 1024;
            _pkgSize = 100;
            _pkgsPerSecond = _bytesPerSecond / _pkgSize;
            _tick = 10;
            _pkgsPerOnce = _pkgsPerSecond / (1000 / _tick);
            _batchs = _pkgs.Count / _pkgsPerOnce;
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
                for (var i = _count * _pkgsPerOnce; i < (_count + 1) * _pkgsPerOnce; ++i)
                {
                    _cli.Send(_pkgs[i], 0, _pkgs[i].Length);
                }

                _count = ++_count % _batchs;
                break;
            }
        }
    }
}
