/*------------------------------------------------------------------
    Copyright © : All rights reserved
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
    public enum CTcpClientState
    {
        Closed,
        Connecting,
        Connected,
        Closing,
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
        enum eConnectedResult
        {
            None,
            Fail,
            Success,
        }
        enum eClosedReason
        {
            None,
            Exception,
            Normal,
        }

        CTcpClientArgs _args;
        TcpClient _cli;
        CTcpClientState _state;
        eConnectedResult _connectedFlag;
        eClosedReason _closedFlag;

        SocketAsyncEventArgs _sendArgs;
        List<ArraySegment<byte>> _sends;

        byte[] _recv;
        SocketAsyncEventArgs _recvArgs;
        CRingBuf _recvBuf;
        byte[] _recvLen;
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

        public event EventHandler Closed;
        void RaiseClosed()
        {
            var h = Closed;
            if (h != null)
            {
                h(this, EventArgs.Empty);
            }
        }
        void OnClosed()
        {
            RaiseClosed();
        }

        public CTcpClient(CTcpClientArgs args)
        {
            _args = args;
            _state = CTcpClientState.Closed;
            _connectedFlag = eConnectedResult.None;
            _closedFlag = eClosedReason.None;

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
            _recvLen = new byte[2];
            _recvsBefore = new Queue<byte[]>(32);
            _recvs = new Queue<byte[]>(32);
            _recvsAfter = new Queue<byte[]>(32);
        }

        public bool Connect(IPEndPoint addr)
        {
            switch (_state)
            {
                case CTcpClientState.Closed:
                    if (BeginConnect(addr))
                        return true;
                    break;
                case CTcpClientState.Connecting:
                case CTcpClientState.Connected:
                case CTcpClientState.Closing:
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
                _connectedFlag = eConnectedResult.Fail;
                return;
            }

            _cli = cli;
            _cli.SendBufferSize = _args.SendBufferSize;
            _cli.ReceiveBufferSize = _args.ReceiveBufferSize;
            _connectedFlag = eConnectedResult.Success;
        }

        public void Close()
        {
            switch (_state)
            {
                case CTcpClientState.Closed:
                    break;
                case CTcpClientState.Connecting:
                    break;
                case CTcpClientState.Connected:
                    break;
                case CTcpClientState.Closing:
                    break;
            }
        }

        public bool Send(byte[] buf, int offset, int count)
        {
            switch (_state)
            {
                case CTcpClientState.Closed:
                    break;
                case CTcpClientState.Connecting:
                    break;
                case CTcpClientState.Connected:
                    break;
                case CTcpClientState.Closing:
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
                _closedFlag = eClosedReason.Exception;
                return;
            }

            EndRecv(_recvArgs);
        }
        void EndRecv(SocketAsyncEventArgs e)
        {
            switch (_state)
            {
                case CTcpClientState.Closed:
                    break;
                case CTcpClientState.Connecting:
                    break;
                case CTcpClientState.Connected:
                    switch (e.SocketError)
                    {
                        case SocketError.Success:
                            if (e.BytesTransferred > 0)
                            {
                                _recvBuf.Write(_recv, 0, e.BytesTransferred);

                                for (; _recvBuf.UsedCapacity > _recvLen.Length;)
                                {
                                    _recvBuf.Peek(_recvLen, 0, _recvLen.Length);
                                    var len = BitConverter.ToUInt16(_recvLen, 0);
                                    if (_recvBuf.UsedCapacity < (_recvLen.Length + len))
                                        break;

                                    _recvBuf.Read(_recvLen, 0, _recvLen.Length);
                                    var pkg = new byte[len];
                                    _recvBuf.Read(pkg, 0, len);

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
                                _closedFlag = eClosedReason.Normal;
                            }
                            break;
                        default:
                            _args.Log.Error("");
                            _closedFlag = eClosedReason.Exception;
                            break;
                    }
                    break;
                case CTcpClientState.Closing:
                    break;
            }
        }

        public void Exec()
        {
            switch (_state)
            {
                case CTcpClientState.Closed:
                    break;
                case CTcpClientState.Connecting:
                    switch (_connectedFlag)
                    {
                        case eConnectedResult.Fail:
                            _state = CTcpClientState.Closed;
                            break;
                        case eConnectedResult.Success:
                            _state = CTcpClientState.Connected;
                            BeginRecv();
                            break;
                    }
                    break;
                case CTcpClientState.Connected:
                    break;
                case CTcpClientState.Closing:
                    break;
            }
        }
    }
}
