/*------------------------------------------------------------------
    Copyright ? : All rights reserved
    Author      : 邓晓峰
------------------------------------------------------------------*/

using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Reflection;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using log4net;
using System;

namespace Nirge.Core
{
    public enum eTcpClientState
    {
        Closed,
        Connecting,
        Connected,
        Closing,
    }

    public enum eTcpClientCloseReason
    {
        None,
        Exception,
        Normal,
    }

    public struct CTcpClientArgs
    {
        public int SendBufferSize
        {
            get;
            set;
        }
        public int ReceiveBufferSize
        {
            get;
            set;
        }

        public ILog Log
        {
            get;
            set;
        }
    }

    public class CTcpClient
    {
        enum eConnectResult
        {
            None,
            Fail,
            Success,
        }

        CTcpClientArgs _args;
        TcpClient _cli;
        eTcpClientState _state;
        eConnectResult _connectFlag;
        eTcpClientCloseReason _closeFlag;

        byte[] _pkgLen;

        SocketAsyncEventArgs _sendArgs;
        List<ArraySegment<byte>> _sends;

        byte[] _recv;
        SocketAsyncEventArgs _recvArgs;
        CRingBuf _recvBuf;
        Queue<byte[]> _recvsBefore;
        Queue<byte[]> _recvs;
        Queue<byte[]> _recvsAfter;

        public event EventHandler Connected;
        void RaiseConnected()
        {
            var h = Connected;
            if (h != null)
            {
                h(this, EventArgs.Empty);
            }
        }
        void OnConnected()
        {
            RaiseConnected();
        }

        public event EventHandler<CDataEventArgs<eTcpClientCloseReason>> Closed;
        void RaiseClosed(CDataEventArgs<eTcpClientCloseReason> e)
        {
            var h = Closed;
            if (h != null)
            {
                h(this, e);
            }
        }
        void OnClosed(eTcpClientCloseReason reason)
        {
            RaiseClosed(CDataEventArgs.Create(reason));
        }

        public CTcpClient(CTcpClientArgs args)
        {
            _args = args;
            _state = eTcpClientState.Closed;
            _connectFlag = eConnectResult.None;
            _closeFlag = eTcpClientCloseReason.None;

            _pkgLen = new byte[2];

            _sendArgs = new SocketAsyncEventArgs();
            _sendArgs.Completed += (sender, e) =>
            {
                EndSend(e);
            };
            _sends = new List<ArraySegment<byte>>(32);

            _recv = new byte[_args.ReceiveBufferSize];
            _recvArgs = new SocketAsyncEventArgs();
            _recvArgs.SetBuffer(_recv, 0, _args.ReceiveBufferSize);
            _recvArgs.Completed += (sender, e) =>
            {
                EndRecv(e);
            };
            _recvBuf = new CRingBuf(_args.ReceiveBufferSize << 1);
            _recvsBefore = new Queue<byte[]>(32);
            _recvs = new Queue<byte[]>(32);
            _recvsAfter = new Queue<byte[]>(32);
        }

        public bool Connect(IPEndPoint addr)
        {
            switch (_state)
            {
            case eTcpClientState.Closed:
                if (BeginConnect(addr))
                    return true;
                break;
            case eTcpClientState.Connecting:
            case eTcpClientState.Connected:
            case eTcpClientState.Closing:
            default:
                break;
            }

            return false;
        }

        public bool Connect(TcpClient cli)
        {
            return true;
        }

        bool BeginConnect(IPEndPoint addr)
        {
            try
            {
                var cli = new TcpClient();

                cli.BeginConnect(addr.Address, addr.Port, (e) =>
                {
                    EndConnect(cli, e);
                }, cli);
            }
            catch (Exception exception)
            {
                _args.Log.Error("", exception);
                return false;
            }

            return true;
        }
        void EndConnect(TcpClient cli, IAsyncResult e)
        {
            try
            {
                cli.EndConnect(e);
            }
            catch (Exception exception)
            {
                _args.Log.Error("", exception);
                _connectFlag = eConnectResult.Fail;
                return;
            }

            _cli = cli;
            _cli.SendBufferSize = _args.SendBufferSize;
            _cli.ReceiveBufferSize = _args.ReceiveBufferSize;
            _connectFlag = eConnectResult.Success;
        }

        public void Close()
        {
            switch (_state)
            {
            case eTcpClientState.Closed:
                break;
            case eTcpClientState.Connecting:
                break;
            case eTcpClientState.Connected:
                break;
            case eTcpClientState.Closing:
                break;
            }
        }

        public bool Send(byte[] buf, int offset, int count)
        {
            switch (_state)
            {
            case eTcpClientState.Closed:
                break;
            case eTcpClientState.Connecting:
                break;
            case eTcpClientState.Connected:
                break;
            case eTcpClientState.Closing:
                break;
            }

            return true;
        }

        void BeginSend()
        {
        }
        void EndSend(SocketAsyncEventArgs e)
        {
        }

        void BeginRecv()
        {
            try
            {
                if (_cli.Client.ReceiveAsync(_recvArgs))
                    return;
            }
            catch (Exception exception)
            {
                _args.Log.Error("", exception);
                _closeFlag = eTcpClientCloseReason.Exception;
                return;
            }

            EndRecv(_recvArgs);
        }
        void EndRecv(SocketAsyncEventArgs e)
        {
            switch (_state)
            {
            case eTcpClientState.Closed:
                break;
            case eTcpClientState.Connecting:
                break;
            case eTcpClientState.Connected:
                switch (e.SocketError)
                {
                case SocketError.Success:
                    if (e.BytesTransferred > 0)
                    {
                        _recvBuf.Write(_recv, 0, e.BytesTransferred);

                        for (; _recvBuf.UsedCapacity > _pkgLen.Length;)
                        {
                            _recvBuf.Peek(_pkgLen, 0, _pkgLen.Length);
                            var pkgLen = BitConverter.ToUInt16(_pkgLen, 0);
                            if (_recvBuf.UsedCapacity < (_pkgLen.Length + pkgLen))
                                break;

                            _recvBuf.Read(_pkgLen, 0, _pkgLen.Length);
                            var pkg = new byte[pkgLen];
                            _recvBuf.Read(pkg, 0, pkgLen);

                            _recvsBefore.Enqueue(pkg);
                        }

                        lock (_recvs)
                        {
                            while (_recvsBefore.Count > 0)
                                _recvs.Enqueue(_recvsBefore.Dequeue());
                        }

                        BeginRecv();
                    }
                    else
                    {
                        _closeFlag = eTcpClientCloseReason.Normal;
                    }
                    break;
                default:
                    _args.Log.Error("");
                    _closeFlag = eTcpClientCloseReason.Exception;
                    break;
                }
                break;
            case eTcpClientState.Closing:
                break;
            }
        }

        public void Exec()
        {
            switch (_state)
            {
            case eTcpClientState.Closed:
                break;
            case eTcpClientState.Connecting:
                switch (_connectFlag)
                {
                case eConnectResult.Fail:
                    _state = eTcpClientState.Closed;
                    break;
                case eConnectResult.Success:
                    _state = eTcpClientState.Connected;
                    BeginRecv();
                    break;
                }
                break;
            case eTcpClientState.Connected:
                break;
            case eTcpClientState.Closing:
                break;
            }
        }
    }
}
