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
    #region

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
        ClosingWait,
    }

    public enum eConnectResult
    {
        None,
        Fail,
        Success,
    }

    public class CTcpClientConnectArgs
    {
        public eConnectResult Result
        {
            get;
            set;
        }

        public eTcpClientError Error
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

    public enum eTcpClientCloseReason
    {
        None,
        Active,
        Unactive,
        User,
        Exception,
    }

    public class CTcpClientCloseArgs
    {
        public eTcpClientCloseReason Reason
        {
            get;
            set;
        }

        public eTcpClientError Error
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

    #endregion

    public class CTcpClient : IObjCtor<CTcpClientArgs>, IObjDtor
    {
        CTcpClientArgs _args;

        TcpClient _cli;

        eTcpClientState _state;
        CTcpClientConnectArgs _connectTag;
        CTcpClientCloseArgs _closeTag;

        byte[] _pkgLen;

        SocketAsyncEventArgs _sendArgs;
        List<ArraySegment<byte>> _sends;
        bool _sending;

        byte[] _recv;
        SocketAsyncEventArgs _recvArgs;
        CRingBuf _recvBuf;
        Queue<byte[]> _recvsBefore;
        Queue<byte[]> _recvs;
        Queue<byte[]> _recvsAfter;
        bool _recving;

        public CTcpClient(CTcpClientArgs args)
        {
            Init(args);
        }

        public void Init(CTcpClientArgs args)
        {
            _args = args;

            _state = eTcpClientState.Closed;
            _connectTag = new CTcpClientConnectArgs()
            {
                Error = eTcpClientError.None,
                SocketError = SocketError.Success,
                Result = eConnectResult.None,
            };
            _closeTag = new CTcpClientCloseArgs()
            {
                Error = eTcpClientError.None,
                SocketError = SocketError.Success,
                Reason = eTcpClientCloseReason.None,
            };

            _pkgLen = new byte[2];

            _sendArgs = new SocketAsyncEventArgs();
            _sendArgs.Completed += (sender, e) =>
            {
                EndSend(_sendArgs);
            };
            _sends = new List<ArraySegment<byte>>(32);
            _sending = false;

            _recv = new byte[_args.ReceiveBufferSize];
            _recvArgs = new SocketAsyncEventArgs();
            _recvArgs.SetBuffer(_recv, 0, _recv.Length);
            _recvArgs.Completed += (sender, e) =>
            {
                EndRecv(_recvArgs);
            };
            _recvBuf = new CRingBuf(_args.SendBufferSize + _args.ReceiveBufferSize);
            _recvsBefore = new Queue<byte[]>(32);
            _recvs = new Queue<byte[]>(32);
            _recvsAfter = new Queue<byte[]>(32);
            _recving = false;
        }

        public void Destroy()
        {
            switch (_state)
            {
            case eTcpClientState.Closed:
                _pkgLen = null;

                _sendArgs.Dispose();
                _sends = null;

                _recv = null;
                _recvArgs.Dispose();
                _recvBuf = null;
                _recvsBefore = null;
                _recvs = null;
                _recvsAfter = null;
                break;
            case eTcpClientState.Connecting:
            case eTcpClientState.Connected:
            case eTcpClientState.Closing:
            case eTcpClientState.ClosingWait:
                break;
            }
        }

        void Clear()
        {
            try
            {
                using (_cli)
                {
                }
            }
            catch
            {
            }

            _state = eTcpClientState.Closed;

            _connectTag.Error = eTcpClientError.None;
            _connectTag.SocketError = SocketError.Success;
            _connectTag.Result = eConnectResult.None;

            _closeTag.Error = eTcpClientError.None;
            _closeTag.SocketError = SocketError.Success;
            _closeTag.Reason = eTcpClientCloseReason.None;

            _sends.Clear();
            _sending = false;

            _recvBuf.Clear();
            _recvsBefore.Clear();
            _recvs.Clear();
            _recvsAfter.Clear();
            _recving = false;
        }

        #region

        public event EventHandler<CDataEventArgs<CTcpClientConnectArgs>> Connected;
        void RaiseConnected(CDataEventArgs<CTcpClientConnectArgs> e)
        {
            var h = Connected;
            if (h != null)
            {
                h(this, e);
            }
        }
        void OnConnected(CTcpClientConnectArgs args)
        {
            RaiseConnected(CDataEventArgs.Create(args));
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

        public eTcpClientError Connect(IPEndPoint addr)
        {
            switch (_state)
            {
            case eTcpClientState.Closed:
                _state = eTcpClientState.Connecting;
                BeginConnect(addr);
                return eTcpClientError.None;
            case eTcpClientState.Connecting:
            case eTcpClientState.Connected:
            case eTcpClientState.Closing:
            case eTcpClientState.ClosingWait:
            default:
                return eTcpClientError.WrongState;
            }
        }

        public eTcpClientError Connect(TcpClient cli)
        {
            if (cli == null)
                return eTcpClientError.ArgumentOutOfRange;

            switch (_state)
            {
            case eTcpClientState.Closed:
                _state = eTcpClientState.Connecting;
                _cli = cli;
                Connect();
                _state = eTcpClientState.Connected;

                var e = new CTcpClientConnectArgs()
                {
                    Error = eTcpClientError.None,
                    SocketError = SocketError.Success,
                    Result = eConnectResult.Success,
                };

                try
                {
                    OnConnected(e);
                }
                catch (Exception exception)
                {
                    _args.Log.Error(string.Format("[TcpClient]OnConnected exception"), exception);
                }

                _recving = true;
                BeginRecv();
                return eTcpClientError.None;
            case eTcpClientState.Connecting:
            case eTcpClientState.Connected:
            case eTcpClientState.Closing:
            case eTcpClientState.ClosingWait:
            default:
                return eTcpClientError.WrongState;
            }
        }

        void Connect()
        {
            _cli.SendBufferSize = _args.SendBufferSize;
            _cli.ReceiveBufferSize = _args.ReceiveBufferSize;
            _cli.NoDelay = true;
        }

        void BeginConnect(IPEndPoint addr)
        {
            try
            {
                _cli = new TcpClient();

                _cli.BeginConnect(addr.Address, addr.Port, (e) =>
                {
                    EndConnect(e);
                }, this);
            }
            catch
            {
                try
                {
                    using (_cli)
                    {
                    }
                }
                catch
                {
                }

                lock (_connectTag)
                {
                    switch (_connectTag.Result)
                    {
                    case eConnectResult.None:
                        _connectTag.Error = eTcpClientError.Exception;
                        _connectTag.SocketError = SocketError.Success;
                        _connectTag.Result = eConnectResult.Fail;
                        break;
                    }
                }
            }
        }

        void EndConnect(IAsyncResult e)
        {
            switch (_state)
            {
            case eTcpClientState.Connecting:
                try
                {
                    _cli.EndConnect(e);
                }
                catch
                {
                    try
                    {
                        using (_cli)
                        {
                        }
                    }
                    catch
                    {
                    }

                    lock (_connectTag)
                    {
                        switch (_connectTag.Result)
                        {
                        case eConnectResult.None:
                            _connectTag.Error = eTcpClientError.Exception;
                            _connectTag.SocketError = SocketError.Success;
                            _connectTag.Result = eConnectResult.Fail;
                            break;
                        }
                    }

                    return;
                }

                lock (_connectTag)
                {
                    switch (_connectTag.Result)
                    {
                    case eConnectResult.None:
                        _connectTag.Error = eTcpClientError.None;
                        _connectTag.SocketError = SocketError.Success;
                        _connectTag.Result = eConnectResult.Success;
                        break;
                    }
                }
                break;
            case eTcpClientState.Closed:
            case eTcpClientState.Connected:
            case eTcpClientState.Closing:
            case eTcpClientState.ClosingWait:
                break;
            }
        }

        public void Close(bool grace = false)
        {
            switch (_state)
            {
            case eTcpClientState.Connecting:
                break;
            case eTcpClientState.Connected:
                _state = eTcpClientState.Closing;
                break;
            case eTcpClientState.Closed:
            case eTcpClientState.Closing:
            case eTcpClientState.ClosingWait:
                break;
            }
        }

        #endregion

        #region

        public eTcpClientError Send(byte[] buf, int offset, int count)
        {
            if (buf == null)
                return eTcpClientError.ArgumentOutOfRange;
            if (count == 0)
                return eTcpClientError.ArgumentOutOfRange;
            if (count > ushort.MaxValue)
                return eTcpClientError.PkgSizeOutOfRange;
            var pkgLen = _pkgLen.Length + count;
            if (pkgLen > _args.SendBufferSize)
                return eTcpClientError.PkgSizeOutOfRange;

            switch (_state)
            {
            case eTcpClientState.Connected:
                var pkg = new byte[pkgLen];
                var len = BitConverter.GetBytes((ushort)count);
                Buffer.BlockCopy(len, 0, pkg, 0, _pkgLen.Length);
                Buffer.BlockCopy(buf, offset, pkg, _pkgLen.Length, count);

                if (_sending)
                {
                    lock (_sends)
                    {
                        _sends.Add(new ArraySegment<byte>(pkg, 0, pkg.Length));
                    }
                }
                else
                {
                    _sendArgs.BufferList = null;
                    _sendArgs.SetBuffer(pkg, 0, pkg.Length);

                    _sending = true;
                    BeginSend();
                }
                return eTcpClientError.None;
            case eTcpClientState.Closed:
            case eTcpClientState.Connecting:
            case eTcpClientState.Closing:
            case eTcpClientState.ClosingWait:
            default:
                return eTcpClientError.WrongState;
            }
        }

        void Send()
        {
            lock (_sends)
            {
                _sendArgs.BufferList = _sends;
                _sends.Clear();
            }

            _sendArgs.SetBuffer(null, 0, 0);

            BeginSend();
        }

        void BeginSend()
        {
            try
            {
                if (_cli.Client.SendAsync(_sendArgs))
                    return;
            }
            catch
            {
                lock (_closeTag)
                {
                    switch (_closeTag.Reason)
                    {
                    case eTcpClientCloseReason.None:
                        _closeTag.Error = eTcpClientError.Exception;
                        _closeTag.SocketError = SocketError.Success;
                        _closeTag.Reason = eTcpClientCloseReason.Exception;
                        break;
                    }
                }

                _sending = false;

                return;
            }

            EndSend(_sendArgs);
        }

        void EndSend(SocketAsyncEventArgs e)
        {
            switch (_state)
            {
            case eTcpClientState.Connected:
                switch (e.SocketError)
                {
                case SocketError.Success:
                    if (_sends.Count > 0)
                        Send();
                    else
                        _sending = false;
                    break;
                default:
                    lock (_closeTag)
                    {
                        switch (_closeTag.Reason)
                        {
                        case eTcpClientCloseReason.None:
                            _closeTag.Error = eTcpClientError.SocketError;
                            _closeTag.SocketError = e.SocketError;
                            _closeTag.Reason = eTcpClientCloseReason.Exception;
                            break;
                        }
                    }

                    _sending = false;
                    break;
                }
                break;
            case eTcpClientState.Closing:
                switch (e.SocketError)
                {
                case SocketError.Success:
                    if (_sends.Count > 0)
                        Send();
                    else
                    {
                        lock (_closeTag)
                        {
                            switch (_closeTag.Reason)
                            {
                            case eTcpClientCloseReason.None:
                                _closeTag.Error = eTcpClientError.None;
                                _closeTag.SocketError = SocketError.Success;
                                _closeTag.Reason = eTcpClientCloseReason.Active;
                                break;
                            }
                        }

                        _sending = false;
                    }
                    break;
                default:
                    lock (_closeTag)
                    {
                        switch (_closeTag.Reason)
                        {
                        case eTcpClientCloseReason.None:
                            _closeTag.Error = eTcpClientError.SocketError;
                            _closeTag.SocketError = e.SocketError;
                            _closeTag.Reason = eTcpClientCloseReason.Exception;
                            break;
                        }
                    }

                    _sending = false;
                    break;
                }
                break;
            case eTcpClientState.Closed:
            case eTcpClientState.Connecting:
            case eTcpClientState.ClosingWait:
                _sending = false;
                break;
            }
        }

        #endregion

        #region

        public event Action<byte[], int, int> Recved;

        void BeginRecv()
        {
            try
            {
                if (_cli.Client.ReceiveAsync(_recvArgs))
                    return;
            }
            catch
            {
                lock (_closeTag)
                {
                    switch (_closeTag.Reason)
                    {
                    case eTcpClientCloseReason.None:
                        _closeTag.Error = eTcpClientError.Exception;
                        _closeTag.SocketError = SocketError.Success;
                        _closeTag.Reason = eTcpClientCloseReason.Exception;
                        break;
                    }
                }

                _recving = false;

                return;
            }

            EndRecv(_recvArgs);
        }

        void EndRecv(SocketAsyncEventArgs e)
        {
            switch (_state)
            {
            case eTcpClientState.Connected:
                switch (e.SocketError)
                {
                case SocketError.Success:
                    if (e.BytesTransferred > 0)
                    {
                        if (Unpack(e))
                        {
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
                            lock (_closeTag)
                            {
                                switch (_closeTag.Reason)
                                {
                                case eTcpClientCloseReason.None:
                                    _closeTag.Error = eTcpClientError.PkgSizeOutOfRange;
                                    _closeTag.SocketError = SocketError.Success;
                                    _closeTag.Reason = eTcpClientCloseReason.User;
                                    break;
                                }
                            }

                            _recving = false;
                        }
                    }
                    else
                    {
                        lock (_closeTag)
                        {
                            switch (_closeTag.Reason)
                            {
                            case eTcpClientCloseReason.None:
                                _closeTag.Error = eTcpClientError.None;
                                _closeTag.SocketError = SocketError.Success;
                                _closeTag.Reason = eTcpClientCloseReason.Unactive;
                                break;
                            }
                        }

                        _recving = false;
                    }
                    break;
                default:
                    lock (_closeTag)
                    {
                        switch (_closeTag.Reason)
                        {
                        case eTcpClientCloseReason.None:
                            _closeTag.Error = eTcpClientError.SocketError;
                            _closeTag.SocketError = e.SocketError;
                            _closeTag.Reason = eTcpClientCloseReason.Exception;
                            break;
                        }
                    }

                    _recving = false;
                    break;
                }
                break;
            case eTcpClientState.Closed:
            case eTcpClientState.Connecting:
            case eTcpClientState.Closing:
            case eTcpClientState.ClosingWait:
                _recving = false;
                break;
            }
        }

        bool Unpack(SocketAsyncEventArgs e)
        {
            if (!_recvBuf.Write(_recv, 0, e.BytesTransferred))
                return false;

            for (; _recvBuf.UsedCapacity > _pkgLen.Length;)
            {
                _recvBuf.Peek(_pkgLen, 0, _pkgLen.Length);
                var pkgLen = BitConverter.ToUInt16(_pkgLen, 0);

                if (pkgLen > _args.SendBufferSize)
                    return false;
                if (_recvBuf.UsedCapacity < (_pkgLen.Length + pkgLen))
                    break;

                _recvBuf.Read(_pkgLen, 0, _pkgLen.Length);
                var pkg = new byte[pkgLen];
                _recvBuf.Read(pkg, 0, pkgLen);

                _recvsBefore.Enqueue(pkg);
            }

            return true;
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
                switch (_connectTag.Result)
                {
                case eConnectResult.Fail:
                    var e = new CTcpClientConnectArgs()
                    {
                        Error = _connectTag.Error,
                        SocketError = _connectTag.SocketError,
                        Result = _connectTag.Result,
                    };

                    _connectTag.Error = eTcpClientError.None;
                    _connectTag.SocketError = SocketError.Success;
                    _connectTag.Result = eConnectResult.None;

                    _state = eTcpClientState.Closed;

                    try
                    {
                        OnConnected(e);
                    }
                    catch (Exception exception)
                    {
                        _args.Log.Error(string.Format("[TcpClient]OnConnected exception"), exception);
                    }
                    break;
                case eConnectResult.Success:
                    Connect();
                    _state = eTcpClientState.Connected;

                    try
                    {
                        OnConnected(_connectTag);
                    }
                    catch (Exception exception)
                    {
                        _args.Log.Error(string.Format("[TcpClient]OnConnected exception"), exception);
                    }

                    _connectTag.Error = eTcpClientError.None;
                    _connectTag.SocketError = SocketError.Success;
                    _connectTag.Result = eConnectResult.None;

                    _recving = true;
                    BeginRecv();
                    break;
                }
                break;
            case eTcpClientState.Connected:
                switch (_closeTag.Reason)
                {
                case eTcpClientCloseReason.None:
                    if (!_sending)
                    {
                        if (_sends.Count > 0)
                            Send();
                    }

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
                            Recved(pkg, 0, pkg.Length);
                        }
                        catch (Exception exception)
                        {
                            _args.Log.Error(string.Format("[TcpClient]Recved exception"), exception);
                        }
                    }
                    break;
                case eTcpClientCloseReason.Unactive:
                case eTcpClientCloseReason.Exception:
                case eTcpClientCloseReason.User:
                    _state = eTcpClientState.ClosingWait;
                    break;
                }
                break;
            case eTcpClientState.Closing:
                switch (_closeTag.Reason)
                {
                case eTcpClientCloseReason.None:
                    if (!_sending)
                    {
                        if (_sends.Count > 0)
                            Send();
                    }
                    break;
                case eTcpClientCloseReason.Active:
                case eTcpClientCloseReason.Unactive:
                case eTcpClientCloseReason.Exception:
                case eTcpClientCloseReason.User:
                    _state = eTcpClientState.ClosingWait;
                    break;
                }
                break;
            case eTcpClientState.ClosingWait:
                if (!_sending)
                    if (!_recving)
                    {
                        var e = new CTcpClientCloseArgs()
                        {
                            Error = _closeTag.Error,
                            SocketError = _closeTag.SocketError,
                            Reason = _closeTag.Reason,
                        };

                        Clear();
                        _state = eTcpClientState.Closed;

                        try
                        {
                            OnClosed(e);
                        }
                        catch (Exception exception)
                        {
                            _args.Log.Error(string.Format("[TcpClient]OnClosed exception"), exception);
                        }
                    }
                break;
            }
        }

        #endregion
    }
}
