/*------------------------------------------------------------------
    Copyright ? : All rights reserved
    Author      : 邓晓峰
------------------------------------------------------------------*/

using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using log4net;
using System;

namespace Nirge.Core
{
    #region

    public class CTcpClientArgs
    {
        int _sendBufSize;
        int _recvBufSize;
        int _sendCacheSize;
        int _recvCacheSize;

        public int SendBufSize
        {
            get
            {
                return _sendBufSize;
            }
        }

        public int RecvBufSize
        {
            get
            {
                return _recvBufSize;
            }
        }

        public int SendCacheSize
        {
            get
            {
                return _sendCacheSize;
            }
        }

        public int RecvCacheSize
        {
            get
            {
                return _recvCacheSize;
            }
        }

        public CTcpClientArgs(int sendBufSize = 0, int recvBufSize = 0, int sendCacheSize = 0, int recvCacheSize = 0)
        {
            _sendBufSize = sendBufSize;
            _recvBufSize = recvBufSize;
            _sendCacheSize = sendCacheSize;
            _recvCacheSize = recvCacheSize;

            if (_sendBufSize < 8192)
                _sendBufSize = 8192;
            if (_recvBufSize < 8192)
                _recvBufSize = 8192;
            if (_sendCacheSize < 1048576)
                _sendCacheSize = 1048576;
            if (_recvCacheSize < 1048576)
                _recvCacheSize = 1048576;
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
        Success,
        Fail,
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

    public class CTcpClient : IObjAlloc<CTcpClientArgs, ILog, ITcpClientCache>, IObjCollect
    {
        CTcpClientArgs _args;
        ILog _log;
        ITcpClientCache _cache;

        eTcpClientState _state;
        CTcpClientConnectArgs _connectTag;
        CTcpClientCloseArgs _closeTag;

        TcpClient _cli;

        SocketAsyncEventArgs _sendArgs;
        Queue<ArraySegment<byte>> _sends;
        List<ArraySegment<byte>> _sendsPost;
        bool _sending;
        int _sendCacheSize;

        SocketAsyncEventArgs _recvArgs;
        Queue<ArraySegment<byte>> _recvs;
        Queue<ArraySegment<byte>> _recvsPost;
        bool _recving;
        int _recvCacheSize;

        public CTcpClientArgs Args
        {
            get
            {
                return _args;
            }
        }

        public eTcpClientState State
        {
            get
            {
                return _state;
            }
        }

        public CTcpClient(CTcpClientArgs args, ILog log, ITcpClientCache cache)
        {
            Alloc(args, log, cache);
        }

        public CTcpClient(ILog log)
            :
            this(new CTcpClientArgs(), log, new CTcpClientCache(new CTcpClientCacheArgs(8192, 10485760, 10485760)))
        {
        }

        public void Alloc(CTcpClientArgs args, ILog log, ITcpClientCache cache)
        {
            _args = args;
            _log = log;
            _cache = cache;

            _sendArgs = new SocketAsyncEventArgs();
            _sendArgs.Completed += (sender, e) =>
            {
                EndSend(_sendArgs);
            };
            _sends = new Queue<ArraySegment<byte>>();
            _sendsPost = new List<ArraySegment<byte>>();

            _recvArgs = new SocketAsyncEventArgs();
            _recvArgs.Completed += (sender, e) =>
            {
                EndRecv(_recvArgs);
            };
            _recvs = new Queue<ArraySegment<byte>>();
            _recvsPost = new Queue<ArraySegment<byte>>();

            Clear();
        }

        public void Collect()
        {
            switch (_state)
            {
            case eTcpClientState.Closed:
                _args = null;
                _log = null;
                _cache = null;

                _connectTag = null;
                _closeTag = null;

                _cli = null;

                _sendArgs.Dispose();
                _sendArgs = null;
                _sends = null;
                _sendsPost = null;

                _recvArgs.Dispose();
                _recvArgs = null;
                _recvs = null;
                _recvsPost = null;
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

            _connectTag = new CTcpClientConnectArgs()
            {
                Result = eTcpClientConnectResult.None,
                Error = eTcpError.None,
                SocketError = SocketError.Success,
            };

            _closeTag = new CTcpClientCloseArgs()
            {
                Reason = eTcpClientCloseReason.None,
                Error = eTcpError.None,
                SocketError = SocketError.Success,
            };

            _cli = null;

            _sendArgs.AcceptSocket = null;
            while (_sends.Count > 0)
                _cache.CollectSendBuf(_sends.Dequeue().Array);
            if (_sendsPost.Count > 0)
            {
                foreach (var i in _sendsPost)
                    _cache.CollectSendBuf(i.Array);
                _sendsPost.Clear();
            }
            _sending = false;
            _sendCacheSize = 0;

            _recvArgs.AcceptSocket = null;
            while (_recvs.Count > 0)
                _cache.CollectRecvBuf(_recvs.Dequeue().Array);
            while (_recvsPost.Count > 0)
                _cache.CollectRecvBuf(_recvsPost.Dequeue().Array);
            _recving = false;
            _recvCacheSize = 0;
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

        public eTcpError Connect(IPEndPoint endPoint)
        {
            switch (_state)
            {
            case eTcpClientState.Closed:
                _state = eTcpClientState.Connecting;
                ConnectAsync(endPoint);
                return eTcpError.Success;
            case eTcpClientState.Connecting:
            case eTcpClientState.Connected:
            case eTcpClientState.Closing:
            case eTcpClientState.ClosingWait:
            default:
                return eTcpError.WrongTcpState;
            }
        }

        public eTcpError Connect(TcpClient cli)
        {
            if (cli == null)
                return eTcpError.CliNull;

            switch (_state)
            {
            case eTcpClientState.Closed:
                _state = eTcpClientState.Connecting;
                _cli = cli;
                Connect();
                _state = eTcpClientState.Connected;

                var e = new CTcpClientConnectArgs()
                {
                    Result = eTcpClientConnectResult.Success,
                    Error = eTcpError.None,
                    SocketError = SocketError.Success,
                };

                try
                {
                    OnConnected(e);
                }
                catch (Exception exception)
                {
                    _log.Error(string.Format("NET cli OnConnected exception addr {0} {1} connectArgs {2} {3} {4}"
                        , _cli.Client.LocalEndPoint
                        , _cli.Client.RemoteEndPoint
                        , e.Result
                        , e.Error
                        , e.SocketError), exception);
                }

                _recving = true;
                BeginRecv();
                return eTcpError.Success;
            case eTcpClientState.Connecting:
            case eTcpClientState.Connected:
            case eTcpClientState.Closing:
            case eTcpClientState.ClosingWait:
            default:
                return eTcpError.WrongTcpState;
            }
        }

        void Connect()
        {
            _cli.SendBufferSize = _args.SendBufSize;
            _cli.ReceiveBufferSize = _args.RecvBufSize;
            _cli.NoDelay = true;
        }

        async void ConnectAsync(IPEndPoint endPoint)
        {
            _cli = new TcpClient();

            var pass = false;
            try
            {
                await _cli.ConnectAsync(endPoint.Address, endPoint.Port);
                pass = true;
            }
            catch (SocketException exception)
            {
                lock (_connectTag)
                {
                    switch (_connectTag.Result)
                    {
                    case eTcpClientConnectResult.None:
                        _connectTag.Result = eTcpClientConnectResult.Fail;
                        _connectTag.Error = eTcpError.SocketError;
                        _connectTag.SocketError = exception.SocketErrorCode;
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
                        _connectTag.Result = eTcpClientConnectResult.Fail;
                        _connectTag.Error = eTcpError.SysException;
                        _connectTag.SocketError = SocketError.Success;
                        break;
                    }
                }
            }

            if (pass)
            {
                switch (_state)
                {
                case eTcpClientState.Connecting:
                    lock (_connectTag)
                    {
                        switch (_connectTag.Result)
                        {
                        case eTcpClientConnectResult.None:
                            _connectTag.Result = eTcpClientConnectResult.Success;
                            _connectTag.Error = eTcpError.None;
                            _connectTag.SocketError = SocketError.Success;
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
                            _sendCacheSize = 0;
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

        public eTcpError Send(ArraySegment<byte> pkg)
        {
            if (pkg == null)
                return eTcpError.BlockNull;
            if (pkg.Count == 0)
                return eTcpError.BlockSizeIsZero;

            switch (_state)
            {
            case eTcpClientState.Connected:
                if (_sending)
                {
                    if (_sendCacheSize > _args.SendCacheSize)
                    {
                        _log.WarnFormat("NET send cache full threshold {0} cur {1}", _args.SendCacheSize, _sendCacheSize);
                        return eTcpError.SendCacheFull;
                    }

                    lock (_sends)
                    {
                        _sends.Enqueue(pkg);
                        _sendCacheSize += pkg.Count;
                    }
                }
                else
                {
                    var pass = false;
                    try
                    {
                        _sendArgs.BufferList = null;
                        _sendArgs.SetBuffer(pkg.Array, pkg.Offset, pkg.Count);
                        pass = true;
                    }
                    catch (SocketException exception)
                    {
                        lock (_closeTag)
                        {
                            switch (_closeTag.Reason)
                            {
                            case eTcpClientCloseReason.None:
                                _closeTag.Reason = eTcpClientCloseReason.Exception;
                                _closeTag.Error = eTcpError.SocketError;
                                _closeTag.SocketError = exception.SocketErrorCode;
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
                                _closeTag.Reason = eTcpClientCloseReason.Exception;
                                _closeTag.Error = eTcpError.SysException;
                                _closeTag.SocketError = SocketError.Success;
                                break;
                            }
                        }
                    }

                    if (pass)
                    {
                        _sending = true;
                        BeginSend();
                    }
                }
                return eTcpError.Success;
            case eTcpClientState.Closed:
            case eTcpClientState.Connecting:
            case eTcpClientState.Closing:
            case eTcpClientState.ClosingWait:
            default:
                return eTcpError.WrongTcpState;
            }
        }

        public eTcpError Send(Queue<ArraySegment<byte>> pkgs)
        {
            if (pkgs == null)
                return eTcpError.BlockNull;
            if (pkgs.Count == 0)
                return eTcpError.BlockSizeIsZero;

            switch (_state)
            {
            case eTcpClientState.Connected:
                if (_sending)
                {
                    if (_sendCacheSize > _args.SendCacheSize)
                    {
                        _log.WarnFormat("NET send cache full threshold {0} cur {1}", _args.SendCacheSize, _sendCacheSize);
                        return eTcpError.SendCacheFull;
                    }

                    lock (_sends)
                    {
                        while (pkgs.Count > 0)
                        {
                            var pkg = pkgs.Dequeue();
                            _sendCacheSize += pkg.Count;
                            _sends.Enqueue(pkg);
                        }
                    }
                }
                else
                {
                    while (pkgs.Count > 0)
                        _sendsPost.Add(pkgs.Dequeue());

                    var pass = false;
                    try
                    {
                        _sendArgs.SetBuffer(null, 0, 0);
                        _sendArgs.BufferList = _sendsPost;
                        pass = true;
                    }
                    catch (SocketException exception)
                    {
                        lock (_closeTag)
                        {
                            switch (_closeTag.Reason)
                            {
                            case eTcpClientCloseReason.None:
                                _closeTag.Reason = eTcpClientCloseReason.Exception;
                                _closeTag.Error = eTcpError.SocketError;
                                _closeTag.SocketError = exception.SocketErrorCode;
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
                                _closeTag.Reason = eTcpClientCloseReason.Exception;
                                _closeTag.Error = eTcpError.SysException;
                                _closeTag.SocketError = SocketError.Success;
                                break;
                            }
                        }
                    }

                    if (pass)
                    {
                        _sending = true;
                        BeginSend();
                    }
                }
                return eTcpError.Success;
            case eTcpClientState.Closed:
            case eTcpClientState.Connecting:
            case eTcpClientState.Closing:
            case eTcpClientState.ClosingWait:
            default:
                return eTcpError.WrongTcpState;
            }
        }

        void Send()
        {
            lock (_sends)
            {
                while (_sends.Count > 0)
                    _sendsPost.Add(_sends.Dequeue());
                _sendCacheSize = 0;
            }

            if (_sendsPost.Count > 0)
            {
                var pass = false;
                try
                {
                    _sendArgs.SetBuffer(null, 0, 0);
                    _sendArgs.BufferList = _sendsPost;
                    pass = true;
                }
                catch (SocketException exception)
                {
                    lock (_closeTag)
                    {
                        switch (_closeTag.Reason)
                        {
                        case eTcpClientCloseReason.None:
                            _closeTag.Reason = eTcpClientCloseReason.Exception;
                            _closeTag.Error = eTcpError.SocketError;
                            _closeTag.SocketError = exception.SocketErrorCode;
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
                            _closeTag.Reason = eTcpClientCloseReason.Exception;
                            _closeTag.Error = eTcpError.SysException;
                            _closeTag.SocketError = SocketError.Success;
                            break;
                        }
                    }
                }

                if (pass)
                    BeginSend();
                else
                    _sending = false;
            }
            else
                _sending = false;
        }

        void BeginSend()
        {
            var pass = false;
            try
            {
                if (_cli.Client.SendAsync(_sendArgs))
                    return;
                pass = true;
            }
            catch (SocketException exception)
            {
                lock (_closeTag)
                {
                    switch (_closeTag.Reason)
                    {
                    case eTcpClientCloseReason.None:
                        _closeTag.Reason = eTcpClientCloseReason.Exception;
                        _closeTag.Error = eTcpError.SocketError;
                        _closeTag.SocketError = exception.SocketErrorCode;
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
                        _closeTag.Reason = eTcpClientCloseReason.Exception;
                        _closeTag.Error = eTcpError.SysException;
                        _closeTag.SocketError = SocketError.Success;
                        break;
                    }
                }
            }

            if (pass)
                EndSend(_sendArgs);
            else
                _sending = false;
        }

        void EndSend(SocketAsyncEventArgs e)
        {
            switch (_state)
            {
            case eTcpClientState.Closed:
            case eTcpClientState.Connecting:
                break;
            case eTcpClientState.Connected:
            case eTcpClientState.Closing:
            case eTcpClientState.ClosingWait:
                if (_sendArgs.Buffer != null)
                {
                    _cache.CollectSendBuf(_sendArgs.Buffer);
                    _sendArgs.SetBuffer(null, 0, 0);
                }
                else if (_sendArgs.BufferList != null)
                {
                    foreach (var i in _sendArgs.BufferList)
                        _cache.CollectSendBuf(i.Array);
                    _sendArgs.BufferList.Clear();
                    _sendArgs.BufferList = null;
                }
                break;
            }

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
                            _closeTag.Reason = eTcpClientCloseReason.Exception;
                            _closeTag.Error = eTcpError.SocketError;
                            _closeTag.SocketError = e.SocketError;
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
            var pass = false;

            if (_recvCacheSize > _args.RecvCacheSize)
                _log.WarnFormat("NET recv cache full threshold {0} cur {1}", _args.RecvCacheSize, _recvCacheSize);
            else if (!_cache.CanAllocRecvBuf)
                _log.WarnFormat("NET global recv cache alloc {0}", _cache.RecvCacheSizeAlloc);
            else
            {
                byte[] buf;
                var result = _cache.AllocRecvBuf(out buf);
                if (result != eTcpError.Success)
                    _log.WarnFormat("NET global recv cache cant alloc {0}", result);
                else
                {
                    try
                    {
                        _recvArgs.BufferList = null;
                        _recvArgs.SetBuffer(buf, 0, buf.Length);
                        if (_cli.Client.ReceiveAsync(_recvArgs))
                            return;
                        pass = true;
                    }
                    catch (SocketException exception)
                    {
                        lock (_closeTag)
                        {
                            switch (_closeTag.Reason)
                            {
                            case eTcpClientCloseReason.None:
                                _closeTag.Reason = eTcpClientCloseReason.Exception;
                                _closeTag.Error = eTcpError.SocketError;
                                _closeTag.SocketError = exception.SocketErrorCode;
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
                                _closeTag.Error = eTcpError.SysException;
                                _closeTag.SocketError = SocketError.Success;
                                _closeTag.Reason = eTcpClientCloseReason.Exception;
                                break;
                            }
                        }
                    }
                }
            }

            if (pass)
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
                        lock (_recvs)
                        {
                            var pkg = new ArraySegment<byte>(e.Buffer, 0, e.BytesTransferred);
                            _recvs.Enqueue(pkg);
                            _recvCacheSize += pkg.Count;
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
                                _closeTag.Reason = eTcpClientCloseReason.Unactive;
                                _closeTag.Error = eTcpError.None;
                                _closeTag.SocketError = SocketError.Success;
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
                            _closeTag.Reason = eTcpClientCloseReason.Exception;
                            _closeTag.Error = eTcpError.SocketError;
                            _closeTag.SocketError = e.SocketError;
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
                        Result = _connectTag.Result,
                        Error = _connectTag.Error,
                        SocketError = _connectTag.SocketError,
                    };

                    _connectTag.Result = eTcpClientConnectResult.None;
                    _connectTag.Error = eTcpError.None;
                    _connectTag.SocketError = SocketError.Success;

                    _state = eTcpClientState.Closed;

                    try
                    {
                        OnConnected(e);
                    }
                    catch (Exception exception)
                    {
                        _log.Error(string.Format("NET cli OnConnected exception connectArgs {0} {1} {2}"
                            , e.Result
                            , e.Error
                            , e.SocketError), exception);
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
                        _log.Error(string.Format("NET cli OnConnected exception addr {0} {1} connectArgs {2} {3} {4}"
                                , _cli.Client.LocalEndPoint
                                , _cli.Client.RemoteEndPoint
                                , _connectTag.Result
                                , _connectTag.Error
                                , _connectTag.SocketError), exception);
                    }

                    _connectTag.Result = eTcpClientConnectResult.None;
                    _connectTag.Error = eTcpError.None;
                    _connectTag.SocketError = SocketError.Success;

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
                                _recvsPost.Enqueue(_recvs.Dequeue());
                            _recvCacheSize = 0;
                        }
                    }

                    while (_recvsPost.Count > 0)
                    {
                        var pkg = _recvsPost.Dequeue();

                        try
                        {
                            if (Recved != null)
                                Recved(this, pkg.Array, pkg.Offset, pkg.Count);
                        }
                        catch (Exception exception)
                        {
                        }

                        _cache.CollectRecvBuf(pkg.Array);
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
                                    _closeTag.Reason = eTcpClientCloseReason.Active;
                                    _closeTag.Error = eTcpError.None;
                                    _closeTag.SocketError = SocketError.Success;
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
                            Reason = _closeTag.Reason,
                            Error = _closeTag.Error,
                            SocketError = _closeTag.SocketError,
                        };

                        Clear();
                        _state = eTcpClientState.Closed;

                        try
                        {
                            OnClosed(e);
                        }
                        catch (Exception exception)
                        {
                            _log.Error(string.Format("NET cli OnClosed exception closeArgs {0} {1} {2}"
                                , e.Reason
                                , e.Error
                                , e.SocketError), exception);
                        }
                    }
                break;
            }
        }

        #endregion
    }
}
