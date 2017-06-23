﻿/*------------------------------------------------------------------
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

    public class CTcpClientArgs
    {
        int _sendBufferSize;
        int _receiveBufferSize;
        int _pkgSize;
        int _sendQueueSize;
        int _recvQueueSize;

        public int SendBufferSize
        {
            get
            {
                return _sendBufferSize;
            }
        }

        public int ReceiveBufferSize
        {
            get
            {
                return _receiveBufferSize;
            }
        }

        public int PkgSize
        {
            get
            {
                return _pkgSize;
            }
        }

        public int SendQueueSize
        {
            get
            {
                return _sendQueueSize;
            }
        }

        public int RecvQueueSize
        {
            get
            {
                return _recvQueueSize;
            }
        }

        public CTcpClientArgs(int sendBufferSize = 0, int receiveBufferSize = 0, int pkgSize = 0, int sendQueueSize = 0, int recvQueueSize = 0)
        {
            _sendBufferSize = sendBufferSize;
            _receiveBufferSize = receiveBufferSize;
            _pkgSize = pkgSize;
            _sendQueueSize = sendQueueSize;
            _recvQueueSize = recvQueueSize;

            if (_sendBufferSize == 0)
                _sendBufferSize = 16384;
            else if (_sendBufferSize < 8192)
                _sendBufferSize = 8192;
            else if (_sendBufferSize > 16384)
                _sendBufferSize = 16384;
            if (_receiveBufferSize == 0)
                _receiveBufferSize = 16384;
            else if (_receiveBufferSize < 8192)
                _receiveBufferSize = 8192;
            else if (_receiveBufferSize > 16384)
                _receiveBufferSize = 16384;
            if (_pkgSize == 0)
                _pkgSize = 16384;
            else if (_pkgSize < 8192)
                _pkgSize = 8192;
            else if (_pkgSize > 1048576)
                _pkgSize = 1048576;
            _sendQueueSize = 1024;
            _recvQueueSize = 1024;
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

    public enum eTcpClientConnectResult
    {
        None,
        Fail,
        Success,
    }

    public class CTcpClientConnectArgs
    {
        public eTcpClientConnectResult Result
        {
            get;
            set;
        }

        public eTcpError Error
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

        public eTcpError Error
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

    public class CTcpClient : IObjCtor<CTcpClientArgs, ILog>, IObjDtor
    {
        CTcpClientArgs _args;
        ILog _log;

        eTcpClientState _state;
        CTcpClientConnectArgs _connectTag;
        CTcpClientCloseArgs _closeTag;

        TcpClient _cli;

        byte[] _pkgLen;

        SocketAsyncEventArgs _sendArgs;
        Queue<ArraySegment<byte>> _sends;
        List<ArraySegment<byte>> _sendsAfter;
        bool _sending;

        byte[] _recv;
        SocketAsyncEventArgs _recvArgs;
        CRingBuf _recvBuf;
        Queue<byte[]> _recvsBefore;
        Queue<byte[]> _recvs;
        Queue<byte[]> _recvsAfter;
        bool _recving;

        public eTcpClientState State
        {
            get
            {
                return _state;
            }
        }

        public CTcpClient(CTcpClientArgs args, ILog log)
        {
            Init(args, log);
        }

        public CTcpClient(ILog log)
            :
            this(new CTcpClientArgs(), log)
        {
        }

        public void Init(CTcpClientArgs args, ILog log)
        {
            _args = args;

            _log = log;

            _state = eTcpClientState.Closed;
            _connectTag = new CTcpClientConnectArgs()
            {
                Error = eTcpError.None,
                SocketError = SocketError.Success,
                Result = eTcpClientConnectResult.None,
            };
            _closeTag = new CTcpClientCloseArgs()
            {
                Error = eTcpError.None,
                SocketError = SocketError.Success,
                Reason = eTcpClientCloseReason.None,
            };

            _pkgLen = new byte[4];

            _sendArgs = new SocketAsyncEventArgs();
            _sendArgs.Completed += (sender, e) =>
            {
                EndSend(_sendArgs);
            };
            _sends = new Queue<ArraySegment<byte>>(_args.SendQueueSize);
            _sendsAfter = new List<ArraySegment<byte>>(_args.SendQueueSize);
            _sending = false;

            _recv = new byte[_args.ReceiveBufferSize];
            _recvArgs = new SocketAsyncEventArgs();
            _recvArgs.SetBuffer(_recv, 0, _recv.Length);
            _recvArgs.Completed += (sender, e) =>
            {
                EndRecv(_recvArgs);
            };
            _recvBuf = new CRingBuf(_args.PkgSize + _args.ReceiveBufferSize);
            _recvsBefore = new Queue<byte[]>(32);
            _recvs = new Queue<byte[]>(_args.RecvQueueSize);
            _recvsAfter = new Queue<byte[]>(_args.RecvQueueSize);
            _recving = false;
        }

        public void Destroy()
        {
            switch (_state)
            {
            case eTcpClientState.Closed:
                _pkgLen = null;

                _sendArgs.Dispose();
                _sendArgs = null;
                _sends = null;
                _sendsAfter = null;

                _recv = null;
                _recvArgs.Dispose();
                _recvArgs = null;
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
            _state = eTcpClientState.Closed;

            _connectTag.Error = eTcpError.None;
            _connectTag.SocketError = SocketError.Success;
            _connectTag.Result = eTcpClientConnectResult.None;

            _closeTag.Error = eTcpError.None;
            _closeTag.SocketError = SocketError.Success;
            _closeTag.Reason = eTcpClientCloseReason.None;

            _cli = null;

            _sends.Clear();
            _sendsAfter.Clear();

            _recvBuf.Clear();
            _recvsBefore.Clear();
            _recvs.Clear();
            _recvsAfter.Clear();
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

        public eTcpError Connect(IPEndPoint addr)
        {
            switch (_state)
            {
            case eTcpClientState.Closed:
                _state = eTcpClientState.Connecting;
                ConnectAsync(addr);
                return eTcpError.None;
            case eTcpClientState.Connecting:
            case eTcpClientState.Connected:
            case eTcpClientState.Closing:
            case eTcpClientState.ClosingWait:
            default:
                return eTcpError.WrongState;
            }
        }

        public eTcpError Connect(TcpClient cli)
        {
            if (cli == null)
                return eTcpError.ArgumentNullRange;

            switch (_state)
            {
            case eTcpClientState.Closed:
                _state = eTcpClientState.Connecting;
                _cli = cli;
                Connect();
                _state = eTcpClientState.Connected;

                var e = new CTcpClientConnectArgs()
                {
                    Error = eTcpError.None,
                    SocketError = SocketError.Success,
                    Result = eTcpClientConnectResult.Success,
                };

                try
                {
                    OnConnected(e);
                }
                catch (Exception exception)
                {
                    _log.Error(string.Format("[TcpClient]OnConnected exception, addr:\"{0},{1}\", connectArgs:\"{2},{3},{4}\"", _cli.Client.LocalEndPoint, _cli.Client.RemoteEndPoint, e.Result, e.Error, e.SocketError), exception);
                }

                _recving = true;
                BeginRecv();
                return eTcpError.None;
            case eTcpClientState.Connecting:
            case eTcpClientState.Connected:
            case eTcpClientState.Closing:
            case eTcpClientState.ClosingWait:
            default:
                return eTcpError.WrongState;
            }
        }

        void Connect()
        {
            _cli.SendBufferSize = _args.SendBufferSize;
            _cli.ReceiveBufferSize = _args.ReceiveBufferSize;
            _cli.NoDelay = true;
        }

        async void ConnectAsync(IPEndPoint addr)
        {
            _cli = new TcpClient();

            var safe = false;
            try
            {
                await _cli.ConnectAsync(addr.Address, addr.Port);
                safe = true;
            }
            catch (SocketException exception)
            {
                lock (_connectTag)
                {
                    switch (_connectTag.Result)
                    {
                    case eTcpClientConnectResult.None:
                        _connectTag.Error = eTcpError.SocketError;
                        _connectTag.SocketError = exception.SocketErrorCode;
                        _connectTag.Result = eTcpClientConnectResult.Fail;
                        break;
                    }
                }
            }
            catch
            {
                lock (_connectTag)
                {
                    switch (_connectTag.Result)
                    {
                    case eTcpClientConnectResult.None:
                        _connectTag.Error = eTcpError.Exception;
                        _connectTag.SocketError = SocketError.Success;
                        _connectTag.Result = eTcpClientConnectResult.Fail;
                        break;
                    }
                }
            }

            if (safe)
            {
                switch (_state)
                {
                case eTcpClientState.Connecting:
                    lock (_connectTag)
                    {
                        switch (_connectTag.Result)
                        {
                        case eTcpClientConnectResult.None:
                            _connectTag.Error = eTcpError.None;
                            _connectTag.SocketError = SocketError.Success;
                            _connectTag.Result = eTcpClientConnectResult.Success;
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
            catch (SocketException exception)
            {
                lock (_connectTag)
                {
                    switch (_connectTag.Result)
                    {
                    case eTcpClientConnectResult.None:
                        _connectTag.Error = eTcpError.SocketError;
                        _connectTag.SocketError = exception.SocketErrorCode;
                        _connectTag.Result = eTcpClientConnectResult.Fail;
                        break;
                    }
                }
            }
            catch
            {
                lock (_connectTag)
                {
                    switch (_connectTag.Result)
                    {
                    case eTcpClientConnectResult.None:
                        _connectTag.Error = eTcpError.Exception;
                        _connectTag.SocketError = SocketError.Success;
                        _connectTag.Result = eTcpClientConnectResult.Fail;
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

                    lock (_connectTag)
                    {
                        switch (_connectTag.Result)
                        {
                        case eTcpClientConnectResult.None:
                            _connectTag.Error = eTcpError.None;
                            _connectTag.SocketError = SocketError.Success;
                            _connectTag.Result = eTcpClientConnectResult.Success;
                            break;
                        }
                    }
                }
                catch (SocketException exception)
                {
                    lock (_connectTag)
                    {
                        switch (_connectTag.Result)
                        {
                        case eTcpClientConnectResult.None:
                            _connectTag.Error = eTcpError.SocketError;
                            _connectTag.SocketError = exception.SocketErrorCode;
                            _connectTag.Result = eTcpClientConnectResult.Fail;
                            break;
                        }
                    }
                }
                catch
                {
                    lock (_connectTag)
                    {
                        switch (_connectTag.Result)
                        {
                        case eTcpClientConnectResult.None:
                            _connectTag.Error = eTcpError.Exception;
                            _connectTag.SocketError = SocketError.Success;
                            _connectTag.Result = eTcpClientConnectResult.Fail;
                            break;
                        }
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

        public void Close(bool graceful = true)
        {
            switch (_state)
            {
            case eTcpClientState.Connecting:
                eClose();
                break;
            case eTcpClientState.Connected:
                _state = eTcpClientState.Closing;
                if (!graceful)
                {
                    if (_sends.Count > 0)
                    {
                        lock (_sends)
                        {
                            _sends.Clear();
                        }
                    }
                }
                break;
            case eTcpClientState.Closed:
            case eTcpClientState.Closing:
            case eTcpClientState.ClosingWait:
                break;
            }
        }

        void eClose()
        {
            try
            {
                _cli.Close();
            }
            catch
            {
            }
        }

        #endregion

        #region

        public eTcpError Send(byte[] buf, int offset, int count)
        {
            if (buf == null)
                return eTcpError.ArgumentNullRange;
            if (count == 0)
                return eTcpError.PkgSizeOutOfRange;
            var pkgLen = _pkgLen.Length + count;
            if (pkgLen > _args.PkgSize)
                return eTcpError.PkgSizeOutOfRange;

            switch (_state)
            {
            case eTcpClientState.Connected:
                var pkg = new byte[pkgLen];
                var len = BitConverter.GetBytes(count);
                Buffer.BlockCopy(len, 0, pkg, 0, _pkgLen.Length);
                Buffer.BlockCopy(buf, offset, pkg, _pkgLen.Length, count);

                if (_sending)
                {
                    lock (_sends)
                    {
                        _sends.Enqueue(new ArraySegment<byte>(pkg, 0, pkg.Length));
                    }
                }
                else
                {
                    var safe = false;
                    try
                    {
                        _sendArgs.BufferList = null;
                        _sendArgs.SetBuffer(pkg, 0, pkg.Length);
                        safe = true;
                    }
                    catch (SocketException exception)
                    {
                        lock (_closeTag)
                        {
                            switch (_closeTag.Reason)
                            {
                            case eTcpClientCloseReason.None:
                                _closeTag.Error = eTcpError.SocketError;
                                _closeTag.SocketError = exception.SocketErrorCode;
                                _closeTag.Reason = eTcpClientCloseReason.Exception;
                                break;
                            }
                        }
                    }
                    catch
                    {
                        lock (_closeTag)
                        {
                            switch (_closeTag.Reason)
                            {
                            case eTcpClientCloseReason.None:
                                _closeTag.Error = eTcpError.Exception;
                                _closeTag.SocketError = SocketError.Success;
                                _closeTag.Reason = eTcpClientCloseReason.Exception;
                                break;
                            }
                        }
                    }

                    if (safe)
                    {
                        _sending = true;
                        BeginSend();
                    }
                }
                return eTcpError.None;
            case eTcpClientState.Closed:
            case eTcpClientState.Connecting:
            case eTcpClientState.Closing:
            case eTcpClientState.ClosingWait:
            default:
                return eTcpError.WrongState;
            }
        }

        void Send()
        {
            lock (_sends)
            {
                while (_sends.Count > 0)
                {
                    _sendsAfter.Add(_sends.Dequeue());
                }
            }

            if (_sendsAfter.Count > 0)
            {
                var safe = false;
                try
                {
                    _sendArgs.SetBuffer(null, 0, 0);
                    _sendArgs.BufferList = _sendsAfter;
                    _sendsAfter.Clear();
                    safe = true;
                }
                catch (SocketException exception)
                {
                    lock (_closeTag)
                    {
                        switch (_closeTag.Reason)
                        {
                        case eTcpClientCloseReason.None:
                            _closeTag.Error = eTcpError.SocketError;
                            _closeTag.SocketError = exception.SocketErrorCode;
                            _closeTag.Reason = eTcpClientCloseReason.Exception;
                            break;
                        }
                    }
                }
                catch
                {
                    lock (_closeTag)
                    {
                        switch (_closeTag.Reason)
                        {
                        case eTcpClientCloseReason.None:
                            _closeTag.Error = eTcpError.Exception;
                            _closeTag.SocketError = SocketError.Success;
                            _closeTag.Reason = eTcpClientCloseReason.Exception;
                            break;
                        }
                    }
                }

                if (safe)
                    BeginSend();
                else
                    _sending = false;
            }
            else
                _sending = false;
        }

        void BeginSend()
        {
            var safe = false;
            try
            {
                if (_cli.Client.SendAsync(_sendArgs))
                    return;
                safe = true;
            }
            catch (SocketException exception)
            {
                lock (_closeTag)
                {
                    switch (_closeTag.Reason)
                    {
                    case eTcpClientCloseReason.None:
                        _closeTag.Error = eTcpError.SocketError;
                        _closeTag.SocketError = exception.SocketErrorCode;
                        _closeTag.Reason = eTcpClientCloseReason.Exception;
                        break;
                    }
                }
            }
            catch
            {
                lock (_closeTag)
                {
                    switch (_closeTag.Reason)
                    {
                    case eTcpClientCloseReason.None:
                        _closeTag.Error = eTcpError.Exception;
                        _closeTag.SocketError = SocketError.Success;
                        _closeTag.Reason = eTcpClientCloseReason.Exception;
                        break;
                    }
                }
            }

            if (safe)
                EndSend(_sendArgs);
            else
                _sending = false;
        }

        void EndSend(SocketAsyncEventArgs e)
        {
            switch (_state)
            {
            case eTcpClientState.Connected:
            case eTcpClientState.Closing:
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
                            _closeTag.Error = eTcpError.SocketError;
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

        public event Action<object, byte[], int, int> Recved;

        void BeginRecv()
        {
            var safe = false;
            try
            {
                if (_cli.Client.ReceiveAsync(_recvArgs))
                    return;
                safe = true;
            }
            catch (SocketException exception)
            {
                lock (_closeTag)
                {
                    switch (_closeTag.Reason)
                    {
                    case eTcpClientCloseReason.None:
                        _closeTag.Error = eTcpError.SocketError;
                        _closeTag.SocketError = exception.SocketErrorCode;
                        _closeTag.Reason = eTcpClientCloseReason.Exception;
                        break;
                    }
                }
            }
            catch
            {
                lock (_closeTag)
                {
                    switch (_closeTag.Reason)
                    {
                    case eTcpClientCloseReason.None:
                        _closeTag.Error = eTcpError.Exception;
                        _closeTag.SocketError = SocketError.Success;
                        _closeTag.Reason = eTcpClientCloseReason.Exception;
                        break;
                    }
                }
            }

            if (safe)
                EndRecv(_recvArgs);
            else
                _recving = false;
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
                                    _closeTag.Error = eTcpError.PkgSizeOutOfRange;
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
                                _closeTag.Error = eTcpError.None;
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
                            _closeTag.Error = eTcpError.SocketError;
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
                var pkgLen = BitConverter.ToInt32(_pkgLen, 0);

                if (pkgLen > _args.PkgSize)
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
                case eTcpClientConnectResult.Fail:
                    eClose();

                    var e = new CTcpClientConnectArgs()
                    {
                        Error = _connectTag.Error,
                        SocketError = _connectTag.SocketError,
                        Result = _connectTag.Result,
                    };

                    _connectTag.Error = eTcpError.None;
                    _connectTag.SocketError = SocketError.Success;
                    _connectTag.Result = eTcpClientConnectResult.None;

                    _state = eTcpClientState.Closed;

                    try
                    {
                        OnConnected(e);
                    }
                    catch (Exception exception)
                    {
                        _log.Error(string.Format("[TcpClient]OnConnected exception, addr:\"{0}\", connectArgs:\"{1},{2},{3}\"", "", e.Result, e.Error, e.SocketError), exception);
                    }
                    break;
                case eTcpClientConnectResult.Success:
                    Connect();
                    _state = eTcpClientState.Connected;

                    try
                    {
                        OnConnected(_connectTag);
                    }
                    catch (Exception exception)
                    {
                        _log.Error(string.Format("[TcpClient]OnConnected exception, addr:\"{0},{1}\", connectArgs:\"{2},{3},{4}\"", _cli.Client.LocalEndPoint, _cli.Client.RemoteEndPoint, _connectTag.Result, _connectTag.Error, _connectTag.SocketError), exception);
                    }

                    _connectTag.Error = eTcpError.None;
                    _connectTag.SocketError = SocketError.Success;
                    _connectTag.Result = eTcpClientConnectResult.None;

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
                        {
                            _sending = true;
                            Send();
                        }
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
                            if (Recved != null)
                                Recved(this, pkg, 0, pkg.Length);
                        }
                        catch (Exception exception)
                        {
                            _log.Error(string.Format("[TcpClient]Recved exception, addr:\"{0},{1}\", pkg:\"{2}\"", _cli.Client.LocalEndPoint, _cli.Client.RemoteEndPoint, pkg.Length), exception);
                        }
                    }
                    break;
                case eTcpClientCloseReason.Active:
                case eTcpClientCloseReason.Unactive:
                case eTcpClientCloseReason.Exception:
                case eTcpClientCloseReason.User:
                    eClose();
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
                        {
                            _sending = true;
                            Send();
                        }
                        else
                        {
                            lock (_closeTag)
                            {
                                switch (_closeTag.Reason)
                                {
                                case eTcpClientCloseReason.None:
                                    _closeTag.Error = eTcpError.None;
                                    _closeTag.SocketError = SocketError.Success;
                                    _closeTag.Reason = eTcpClientCloseReason.Active;
                                    break;
                                }
                            }

                            eClose();
                            _state = eTcpClientState.ClosingWait;
                        }
                    }
                    break;
                case eTcpClientCloseReason.Active:
                case eTcpClientCloseReason.Unactive:
                case eTcpClientCloseReason.Exception:
                case eTcpClientCloseReason.User:
                    eClose();
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
                            _log.Error(string.Format("[TcpClient]OnClosed exception, addr:\"{0},{1}\", closeArgs:\"{2},{3},{4}\"", "", "", e.Reason, e.Error, e.SocketError), exception);
                        }
                    }
                break;
            }
        }

        #endregion
    }
}
