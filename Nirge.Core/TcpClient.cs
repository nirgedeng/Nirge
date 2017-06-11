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
        Active,
        Unactive,
    }

    public struct CTcpClientCloseArgs
    {
        public eTcpClientCloseReason Reason
        {
            get;
            set;
        }

        public SocketError SocketError
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
        eConnectResult _connectTag;
        CTcpClientCloseArgs _closeTag;

        byte[] _pkgLen;

        SocketAsyncEventArgs _sendArgs;
        List<ArraySegment<byte>> _sends;

        byte[] _recv;
        SocketAsyncEventArgs _recvArgs;
        CRingBuf _recvBuf;
        Queue<byte[]> _recvsBefore;
        Queue<byte[]> _recvs;
        Queue<byte[]> _recvsAfter;

        public CTcpClient(CTcpClientArgs args)
        {
            _args = args;

            _state = eTcpClientState.Closed;
            _connectTag = eConnectResult.None;
            _closeTag = new CTcpClientCloseArgs()
            {
                Reason = eTcpClientCloseReason.None,
                SocketError = SocketError.Success,
            };

            _pkgLen = new byte[2];

            _sendArgs = new SocketAsyncEventArgs();
            _sendArgs.Completed += (sender, e) =>
            {
                EndSend(_sendArgs);
            };
            _sends = new List<ArraySegment<byte>>(32);

            _recv = new byte[_args.ReceiveBufferSize];
            _recvArgs = new SocketAsyncEventArgs();
            _recvArgs.SetBuffer(_recv, 0, _args.ReceiveBufferSize);
            _recvArgs.Completed += (sender, e) =>
            {
                EndRecv(_recvArgs);
            };
            _recvBuf = new CRingBuf(_args.ReceiveBufferSize << 1);
            _recvsBefore = new Queue<byte[]>(32);
            _recvs = new Queue<byte[]>(32);
            _recvsAfter = new Queue<byte[]>(32);
        }

        #region

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

        public event EventHandler<CDataEventArgs<CTcpClientCloseArgs>> Closed;
        void RaiseClosed(CDataEventArgs<CTcpClientCloseArgs> e)
        {
            var h = Closed;
            if (h != null)
            {
                h(this, e);
            }
        }
        void OnClosed(CTcpClientCloseArgs args)
        {
            RaiseClosed(CDataEventArgs.Create(args));
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

        void Connect(TcpClient cli)
        {
            _cli = cli;
            _cli.SendBufferSize = _args.SendBufferSize;
            _cli.ReceiveBufferSize = _args.ReceiveBufferSize;
        }

        bool BeginConnect(IPEndPoint addr)
        {
            try
            {
                var cli = new TcpClient();

                cli.BeginConnect(addr.Address, addr.Port, (e) =>
                {
                    EndConnect(cli, e);
                }, this);

                return true;
            }
            catch (Exception exception)
            {
                _args.Log.Error("", exception);
            }

            return false;
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
                _connectTag = eConnectResult.Fail;
                return;
            }

            Connect(cli);
            _connectTag = eConnectResult.Success;
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

        #endregion

        #region

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

        #endregion

        #region

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

                _closeTag.Reason = eTcpClientCloseReason.Exception;
                _closeTag.SocketError = SocketError.Success;
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

                        if (_recvsBefore.Count > 0)
                        {
                            lock (_recvs)
                            {
                                while (_recvsBefore.Count > 0)
                                    _recvs.Enqueue(_recvsBefore.Dequeue());
                            }
                        }

                        BeginRecv();
                    }
                    else
                    {
                        _closeTag.Reason = eTcpClientCloseReason.Unactive;
                        _closeTag.SocketError = e.SocketError;
                    }
                    break;
                default:
                    _args.Log.Error("");

                    _closeTag.Reason = eTcpClientCloseReason.Exception;
                    _closeTag.SocketError = SocketError.Success;
                    break;
                }
                break;
            case eTcpClientState.Closing:
                break;
            }
        }

        protected virtual void OnRecv(byte[] buf, int offset, int count)
        {
        }

        #endregion

        #region

        public void Exec()
        {
            switch (_state)
            {
            case eTcpClientState.Closed:
                break;
            case eTcpClientState.Connecting:
                switch (_connectTag)
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
                if (_recvs.Count > 0)
                {
                    lock (_recvs)
                    {
                        while (_recvs.Count > 0)
                            _recvsAfter.Enqueue(_recvs.Dequeue());
                    }
                }

                while (_recvsAfter.Count > 0)
                {
                    var pkg = _recvsAfter.Dequeue();

                    try
                    {
                        OnRecv(pkg, 0, pkg.Length);
                    }
                    catch (Exception exception)
                    {
                        _args.Log.Error("", exception);
                    }
                }
                break;
            case eTcpClientState.Closing:
                break;
            }
        }

        #endregion
    }
}
