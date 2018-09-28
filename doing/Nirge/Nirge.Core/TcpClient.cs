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
        eTcpClientConnectResult _result;
        eTcpError _error;
        SocketError _socketError;
        Exception _exception;

        public eTcpClientConnectResult Result
        {
            get
            {
                return _result;
            }
        }

        public eTcpError Error
        {
            get
            {
                return _error;
            }
        }

        public SocketError SocketError
        {
            get
            {
                return _socketError;
            }
        }

        public Exception Exception
        {
            get
            {
                return _exception;
            }
        }

        public void Set(eTcpClientConnectResult result, eTcpError error, SocketError socketError, Exception exception)
        {
            _result = result;
            _error = error;
            _socketError = socketError;
            _exception = exception;
        }

        public CTcpClientConnectArgs(eTcpClientConnectResult result, eTcpError error, SocketError socketError, Exception exception)
        {
            Set(result, error, socketError, exception);
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
        eTcpClientCloseReason _reason;
        eTcpError _error;
        SocketError _socketError;
        Exception _exception;

        public eTcpClientCloseReason Reason
        {
            get
            {
                return _reason;
            }
        }

        public eTcpError Error
        {
            get
            {
                return _error;
            }
        }

        public SocketError SocketError
        {
            get
            {
                return _socketError;
            }
        }

        public Exception Exception
        {
            get
            {
                return _exception;
            }
        }

        public void Set(eTcpClientCloseReason reason, eTcpError error, SocketError socketError, Exception exception)
        {
            _reason = reason;
            _error = error;
            _socketError = socketError;
            _exception = exception;
        }

        public CTcpClientCloseArgs(eTcpClientCloseReason reason, eTcpError error, SocketError socketError, Exception exception)
        {
            Set(reason, error, socketError, exception);
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
        ulong _sendBlockSize;

        SocketAsyncEventArgs _recvArgs;
        Queue<ArraySegment<byte>> _recvs;
        Queue<ArraySegment<byte>> _recvsPost;
        bool _recving;
        int _recvCacheSize;
        ulong _recvBlockSize;

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

        public int SendCacheSize
        {
            get
            {
                return _sendCacheSize;
            }
        }

        public ulong SendBlockSize
        {
            get
            {
                return _sendBlockSize;
            }
        }

        public int RecvCacheSize
        {
            get
            {
                return _recvCacheSize;
            }
        }

        public ulong RecvBlockSize
        {
            get
            {
                return _recvBlockSize;
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

            _connectTag = new CTcpClientConnectArgs(eTcpClientConnectResult.None, eTcpError.None, SocketError.Success, null);
            _closeTag = new CTcpClientCloseArgs(eTcpClientCloseReason.None, eTcpError.None, SocketError.Success, null);

            _cli = null;

            _sendArgs.AcceptSocket = null;
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
            _sendBlockSize = 0;

            _recvArgs.AcceptSocket = null;
            if (_recvArgs.Buffer != null)
            {
                _cache.CollectRecvBuf(_recvArgs.Buffer);
                _recvArgs.SetBuffer(null, 0, 0);
            }
            while (_recvs.Count > 0)
                _cache.CollectRecvBuf(_recvs.Dequeue().Array);
            while (_recvsPost.Count > 0)
                _cache.CollectRecvBuf(_recvsPost.Dequeue().Array);
            _recving = false;
            _recvCacheSize = 0;
            _recvBlockSize = 0;
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

                var e = new CTcpClientConnectArgs(eTcpClientConnectResult.Success, eTcpError.None, SocketError.Success, null);

                try
                {
                    OnConnected(e);
                }
                catch (Exception exception)
                {
                    _log.WriteLine(eLogPattern.Error, string.Format("NET cli OnConnected exception addr {0} {1} connectArgs {2} {3} {4}"
                        , _cli.Client.LocalEndPoint
                        , _cli.Client.RemoteEndPoint
                        , e.Result
                        , e.Error
                        , e.SocketError), exception);
                }

                byte[] buf;
                if (PreRecv(out buf))
                {
                    _recving = true;
                    BeginRecv(buf);
                }
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
                    if (_connectTag.Result == eTcpClientConnectResult.None)
                        _connectTag.Set(eTcpClientConnectResult.Fail, eTcpError.SocketError, exception.SocketErrorCode, exception);
                }
            }
            catch (Exception exception)
            {
                lock (_connectTag)
                {
                    if (_connectTag.Result == eTcpClientConnectResult.None)
                        _connectTag.Set(eTcpClientConnectResult.Fail, eTcpError.SysException, SocketError.Success, exception);
                }
            }

            if (pass)
            {
                switch (_state)
                {
                case eTcpClientState.Connecting:
                    lock (_connectTag)
                    {
                        if (_connectTag.Result == eTcpClientConnectResult.None)
                            _connectTag.Set(eTcpClientConnectResult.Success, eTcpError.None, SocketError.Success, null);
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
                            if (_closeTag.Reason == eTcpClientCloseReason.None)
                                _closeTag.Set(eTcpClientCloseReason.Exception, eTcpError.SocketError, exception.SocketErrorCode, exception);
                        }
                    }
                    catch (Exception exception)
                    {
                        lock (_closeTag)
                        {
                            if (_closeTag.Reason == eTcpClientCloseReason.None)
                                _closeTag.Set(eTcpClientCloseReason.Exception, eTcpError.SysException, SocketError.Success, exception);
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
                        if (_closeTag.Reason == eTcpClientCloseReason.None)
                            _closeTag.Set(eTcpClientCloseReason.Exception, eTcpError.SocketError, exception.SocketErrorCode, exception);
                    }
                }
                catch (Exception exception)
                {
                    lock (_closeTag)
                    {
                        if (_closeTag.Reason == eTcpClientCloseReason.None)
                            _closeTag.Set(eTcpClientCloseReason.Exception, eTcpError.SysException, SocketError.Success, exception);
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
                    if (_closeTag.Reason == eTcpClientCloseReason.None)
                        _closeTag.Set(eTcpClientCloseReason.Exception, eTcpError.SocketError, exception.SocketErrorCode, exception);
                }
            }
            catch (Exception exception)
            {
                lock (_closeTag)
                {
                    if (_closeTag.Reason == eTcpClientCloseReason.None)
                        _closeTag.Set(eTcpClientCloseReason.Exception, eTcpError.SysException, SocketError.Success, exception);
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
                    _sendBlockSize += (ulong)_sendArgs.Count;
                    _cache.CollectSendBuf(_sendArgs.Buffer);
                    _sendArgs.SetBuffer(null, 0, 0);
                }
                else if (_sendArgs.BufferList != null)
                {
                    foreach (var i in _sendArgs.BufferList)
                    {
                        _sendBlockSize += (ulong)i.Count;
                        _cache.CollectSendBuf(i.Array);
                    }
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
                        if (_closeTag.Reason == eTcpClientCloseReason.None)
                            _closeTag.Set(eTcpClientCloseReason.Exception, eTcpError.SocketError, e.SocketError, null);
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

        bool PreRecv(out byte[] buf)
        {
            if (_recvCacheSize > _args.RecvCacheSize)
            {
                _log.WriteLine(eLogPattern.Warn, string.Format("NET cli recv cache full {0} over {1}", _recvCacheSize, _args.RecvCacheSize));
                buf = null;
                return false;
            }

            if (!_cache.CanAllocRecvBuf)
            {
                _log.WriteLine(eLogPattern.Warn, string.Format("NET g recv cache used up {0} over {1}", _cache.RecvCacheSizeAlloc, _cache.RecvCacheSize));
                buf = null;
                return false;
            }

            switch (_cache.AllocRecvBuf(out buf))
            {
            case eTcpError.Success:
                break;
            default:
                _log.WriteLine(eLogPattern.Warn, string.Format("NET g failed to alloc recv buf {0}", _cache.RecvBufSize));
                buf = null;
                break;
            }

            return true;
        }

        void BeginRecv(byte[] buf)
        {
            if (_recvArgs.Buffer != null)
                _cache.CollectRecvBuf(_recvArgs.Buffer);

            var pass = false;
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
                    if (_closeTag.Reason == eTcpClientCloseReason.None)
                        _closeTag.Set(eTcpClientCloseReason.Exception, eTcpError.SocketError, exception.SocketErrorCode, exception);
                }
            }
            catch (Exception exception)
            {
                lock (_closeTag)
                {
                    if (_closeTag.Reason == eTcpClientCloseReason.None)
                        _closeTag.Set(eTcpClientCloseReason.Exception, eTcpError.SysException, SocketError.Success, exception);
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
                            _recvBlockSize += (ulong)pkg.Count;
                        }

                        _recvArgs.SetBuffer(null, 0, 0);

                        byte[] buf;
                        if (PreRecv(out buf))
                        {
                            BeginRecv(buf);
                        }
                    }
                    else
                    {
                        lock (_closeTag)
                        {
                            if (_closeTag.Reason == eTcpClientCloseReason.None)
                                _closeTag.Set(eTcpClientCloseReason.Unactive, eTcpError.None, SocketError.Success, null);
                        }

                        _recving = false;
                    }
                    break;
                default:
                    lock (_closeTag)
                    {
                        if (_closeTag.Reason == eTcpClientCloseReason.None)
                            _closeTag.Set(eTcpClientCloseReason.Exception, eTcpError.SocketError, e.SocketError, null);
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

                    var e = new CTcpClientConnectArgs(_connectTag.Result, _connectTag.Error, _connectTag.SocketError, null);

                    _connectTag.Set(eTcpClientConnectResult.None, eTcpError.None, SocketError.Success, null);
                    _state = eTcpClientState.Closed;

                    try
                    {
                        OnConnected(e);
                    }
                    catch (Exception exception)
                    {
                        _log.WriteLine(eLogPattern.Error, string.Format("NET cli OnConnected exception connectArgs {0} {1} {2}"
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
                        _log.WriteLine(eLogPattern.Error, string.Format("NET cli OnConnected exception addr {0} {1} connectArgs {2} {3} {4}"
                                , _cli.Client.LocalEndPoint
                                , _cli.Client.RemoteEndPoint
                                , _connectTag.Result
                                , _connectTag.Error
                                , _connectTag.SocketError), exception);
                    }

                    _connectTag.Set(eTcpClientConnectResult.None, eTcpError.None, SocketError.Success, null);

                    byte[] buf;
                    if (PreRecv(out buf))
                    {
                        _recving = true;
                        BeginRecv(buf);
                    }
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

                    if (!_recving)
                    {
                        byte[] buf;
                        if (PreRecv(out buf))
                        {
                            _recving = true;
                            BeginRecv(buf);
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
                        catch
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
                                if (_closeTag.Reason == eTcpClientCloseReason.None)
                                    _closeTag.Set(eTcpClientCloseReason.Active, eTcpError.None, SocketError.Success, null);
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
                        var e = new CTcpClientCloseArgs(_closeTag.Reason, _closeTag.Error, _closeTag.SocketError, null);
                        Clear();
                        _state = eTcpClientState.Closed;

                        try
                        {
                            OnClosed(e);
                        }
                        catch (Exception exception)
                        {
                            _log.WriteLine(eLogPattern.Error, string.Format("NET cli OnClosed exception closeArgs {0} {1} {2}"
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
