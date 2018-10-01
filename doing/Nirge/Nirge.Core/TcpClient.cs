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
        int _pkgSize;
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

        public int PkgSize
        {
            get
            {
                return _pkgSize;
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

        public CTcpClientArgs(int sendBufSize = 0, int recvBufSize = 0, int pkgSize = 0, int sendCacheSize = 0, int recvCacheSize = 0)
        {
            _sendBufSize = sendBufSize;
            _recvBufSize = recvBufSize;
            _pkgSize = pkgSize;
            _sendCacheSize = sendCacheSize;
            _recvCacheSize = recvCacheSize;

            if (_sendBufSize < 8192)
                _sendBufSize = 8192;
            if (_recvBufSize < 8192)
                _recvBufSize = 8192;
            if (_pkgSize < 8192)
                _pkgSize = 8192;
            if (_pkgSize > 1048576)
                _pkgSize = 1048576;
            if (_sendCacheSize < 2097152)
                _sendCacheSize = 2097152;
            if (_recvCacheSize < 2097152)
                _recvCacheSize = 2097152;
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
        SocketError _socketError;
        Exception _exception;

        public eTcpClientConnectResult Result
        {
            get
            {
                return _result;
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

        public void Set(eTcpClientConnectResult result, Exception exception, SocketError socketError)
        {
            _result = result;
            _exception = exception;
            _socketError = socketError;
        }

        public CTcpClientConnectArgs(eTcpClientConnectResult result, Exception exception, SocketError socketError)
        {
            Set(result, exception, socketError);
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
        Exception _exception;
        SocketError _socketError;

        public eTcpClientCloseReason Reason
        {
            get
            {
                return _reason;
            }
        }

        public Exception Exception
        {
            get
            {
                return _exception;
            }
        }

        public SocketError SocketError
        {
            get
            {
                return _socketError;
            }
        }

        public void Set(eTcpClientCloseReason reason, Exception exception, SocketError socketError)
        {
            _reason = reason;
            _exception = exception;
            _socketError = socketError;
        }

        public CTcpClientCloseArgs(eTcpClientCloseReason reason, Exception exception, SocketError socketError)
        {
            Set(reason, exception, socketError);
        }
    }

    #endregion

    public class CTcpClient : IObjAlloc<CTcpClientArgs, ILog, ITcpClientCache, CTcpClientPkgFill>, IObjCollect
    {
        CTcpClientArgs _args;
        ILog _log;
        ITcpClientCache _cache;
        ITcpClientPkgFill _fill;

        eTcpClientState _state;
        CTcpClientConnectArgs _connectTag;
        CTcpClientCloseArgs _closeTag;

        TcpClient _cli;
        ITcpClientPkgHead _head;

        SocketAsyncEventArgs _sendArgs;
        Queue<ArraySegment<byte>> _sends;
        List<ArraySegment<byte>> _sendsPost;
        bool _sending;
        int _sendCacheSize;
        ulong _sendBlockSize;

        byte[] _recv;
        SocketAsyncEventArgs _recvArgs;
        CRingArraySegment _recvSeg;
        Queue<ValueTuple<int, ArraySegment<byte>>> _recvsPre;
        Queue<ValueTuple<int, ArraySegment<byte>>> _recvs;
        Queue<ValueTuple<int, ArraySegment<byte>>> _recvsPost;
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

        public CTcpClient(CTcpClientArgs args, ILog log, ITcpClientCache cache, CTcpClientPkgFill fill)
        {
            Alloc(args, log, cache, fill);
        }

        public CTcpClient(ILog log, CTcpClientPkgFill fill)
            :
            this(new CTcpClientArgs(), log, new CTcpClientCache(new CTcpClientCacheArgs(10485760, 10485760), log), fill)
        {
        }

        public void Alloc(CTcpClientArgs args, ILog log, ITcpClientCache cache, CTcpClientPkgFill fill)
        {
            if (args == null)
                throw new ArgumentNullException("args");
            if (log == null)
                throw new ArgumentNullException("log");
            if (cache == null)
                throw new ArgumentNullException("cache");
            if (fill == null)
                throw new ArgumentNullException("fill");

            _args = args;
            _log = log;
            _cache = cache;
            _fill = fill;

            _head = new CTcpClientPkgHead();

            _sendArgs = new SocketAsyncEventArgs();
            _sendArgs.Completed += (sender, e) =>
            {
                EndSend(_sendArgs);
            };
            _sends = new Queue<ArraySegment<byte>>();
            _sendsPost = new List<ArraySegment<byte>>();

            _recv = new byte[_args.RecvBufSize];
            _recvArgs = new SocketAsyncEventArgs();
            _recvArgs.SetBuffer(_recv, 0, _recv.Length);
            _recvArgs.Completed += (sender, e) =>
            {
                EndRecv(_recvArgs);
            };
            _recvSeg = new CRingArraySegment(_args.PkgSize + _args.RecvBufSize);
            _recvsPre = new Queue<ValueTuple<int, ArraySegment<byte>>>();
            _recvs = new Queue<ValueTuple<int, ArraySegment<byte>>>();
            _recvsPost = new Queue<ValueTuple<int, ArraySegment<byte>>>();

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
                _fill = null;

                _connectTag = null;
                _closeTag = null;

                _cli = null;
                _head = null;

                _sendArgs.Dispose();
                _sendArgs = null;
                _sends = null;
                _sendsPost = null;

                _recv = null;
                _recvArgs.Dispose();
                _recvArgs = null;
                _recvSeg = null;
                _recvsPre = null;
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

            _connectTag = new CTcpClientConnectArgs(eTcpClientConnectResult.None, null, SocketError.Success);
            _closeTag = new CTcpClientCloseArgs(eTcpClientCloseReason.None, null, SocketError.Success);

            _cli = null;
            _head.Clear();

            _sendArgs.AcceptSocket = null;
            if (_sendArgs.BufferList != null)
            {
                foreach (var i in _sendArgs.BufferList)
                {
                    _sendBlockSize += (ulong)i.Count;
                    _cache.CollectSendBuf(i.Array);
                }
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
            _recvSeg.Clear();
            while (_recvsPre.Count > 0)
                _cache.CollectRecvBuf(_recvsPre.Dequeue().Item2.Array);
            while (_recvs.Count > 0)
                _cache.CollectRecvBuf(_recvs.Dequeue().Item2.Array);
            while (_recvsPost.Count > 0)
                _cache.CollectRecvBuf(_recvsPost.Dequeue().Item2.Array);
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

        public void Connect(IPEndPoint endPoint)
        {
            if (endPoint == null)
                throw new ArgumentNullException("endPoint");

            switch (_state)
            {
            case eTcpClientState.Closed:
                _state = eTcpClientState.Connecting;
                ConnectAsync(endPoint);
                break;
            case eTcpClientState.Connecting:
            case eTcpClientState.Connected:
            case eTcpClientState.Closing:
            case eTcpClientState.ClosingWait:
            default:
                throw new CNetException("wrong tcp state");
            }
        }

        public void Connect(TcpClient cli)
        {
            if (cli == null)
                throw new ArgumentNullException("cli");

            switch (_state)
            {
            case eTcpClientState.Closed:
                _state = eTcpClientState.Connecting;
                _cli = cli;
                Connect();
                _state = eTcpClientState.Connected;

                var e = new CTcpClientConnectArgs(eTcpClientConnectResult.Success, null, SocketError.Success);

                try
                {
                    OnConnected(e);
                }
                catch (Exception exception)
                {
                    _log.WriteLine(eLogPattern.Error, string.Format("NET cli OnConnected exception addr {0} {1}"
                        , _cli.Client.LocalEndPoint
                        , _cli.Client.RemoteEndPoint), exception);
                }

                if (PreRecv())
                {
                    _recving = true;
                    BeginRecv();
                }
                break;
            case eTcpClientState.Connecting:
            case eTcpClientState.Connected:
            case eTcpClientState.Closing:
            case eTcpClientState.ClosingWait:
            default:
                throw new CNetException("wrong tcp state");
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
            catch (Exception exception)
            {
                if (_connectTag.Result == eTcpClientConnectResult.None)
                    _connectTag.Set(eTcpClientConnectResult.Fail, exception, SocketError.Success);
            }

            if (pass)
            {
                switch (_state)
                {
                case eTcpClientState.Connecting:
                    if (_connectTag.Result == eTcpClientConnectResult.None)
                        _connectTag.Set(eTcpClientConnectResult.Success, null, SocketError.Success);
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

        public void Send(object pkg)
        {
            if (pkg == null)
                throw new ArgumentNullException("pkg");

            switch (_state)
            {
            case eTcpClientState.Connected:
                if (_sendCacheSize > _args.SendCacheSize)
                    throw new CNetException(string.Format("NET cli send cache full {0} over {1}", _sendCacheSize, _args.SendCacheSize));

                if (!_cache.CanAllocSendBuf)
                    throw new CNetException(string.Format("NET g send cache used up {0} over {1}", _cache.SendCacheSizeAlloc, _cache.SendCacheSize));

                var i = _fill.Fill(_head, _args.PkgSize, pkg, _cache);
                _head.Fill(i.Array);

                if (_sending)
                {
                    lock (_sends)
                    {
                        _sendCacheSize += i.Count;
                        _sends.Enqueue(i);
                    }
                }
                else
                {
                    _sendsPost.Add(i);

                    var pass = false;
                    try
                    {
                        _sendArgs.BufferList = _sendsPost;
                        pass = true;
                    }
                    catch (Exception exception)
                    {
                        lock (_closeTag)
                        {
                            if (_closeTag.Reason == eTcpClientCloseReason.None)
                                _closeTag.Set(eTcpClientCloseReason.Exception, exception, SocketError.Success);
                        }
                    }

                    if (pass)
                    {
                        _sending = true;
                        BeginSend();
                    }
                }
                break;
            case eTcpClientState.Closed:
            case eTcpClientState.Connecting:
            case eTcpClientState.Closing:
            case eTcpClientState.ClosingWait:
            default:
                throw new CNetException("wrong tcp state");
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
                    _sendArgs.BufferList = _sendsPost;
                    pass = true;
                }
                catch (Exception exception)
                {
                    lock (_closeTag)
                    {
                        if (_closeTag.Reason == eTcpClientCloseReason.None)
                            _closeTag.Set(eTcpClientCloseReason.Exception, exception, SocketError.Success);
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
            catch (Exception exception)
            {
                lock (_closeTag)
                {
                    if (_closeTag.Reason == eTcpClientCloseReason.None)
                        _closeTag.Set(eTcpClientCloseReason.Exception, exception, SocketError.Success);
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
                if (_sendArgs.BufferList != null)
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
                            _closeTag.Set(eTcpClientCloseReason.Exception, null, e.SocketError);
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

        public event Action<object, object> Recved;

        bool PreRecv()
        {
            if (_recvCacheSize > _args.RecvCacheSize)
            {
                _log.WriteLine(eLogPattern.Warn, string.Format("NET cli recv cache full {0} over {1}", _recvCacheSize, _args.RecvCacheSize));
                return false;
            }

            if (!_cache.CanAllocRecvBuf)
            {
                _log.WriteLine(eLogPattern.Warn, string.Format("NET g recv cache used up {0} over {1}", _cache.RecvCacheSizeAlloc, _cache.RecvCacheSize));
                return false;
            }

            return true;
        }

        void BeginRecv()
        {
            var pass = false;
            try
            {
                if (_cli.Client.ReceiveAsync(_recvArgs))
                    return;
                pass = true;
            }
            catch (Exception exception)
            {
                lock (_closeTag)
                {
                    if (_closeTag.Reason == eTcpClientCloseReason.None)
                        _closeTag.Set(eTcpClientCloseReason.Exception, exception, SocketError.Success);
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
                        try
                        {
                            _recvSeg.Write(_recv, 0, e.BytesTransferred);

                            for (; _recvSeg.UsedSize > _head.PkgHeadSize;)
                            {
                                _recvSeg.Peek(_head.RecvPkgHeadBuf, 0, _head.PkgHeadSize);
                                _head.UnFill();

                                if (_head.RecvPkgSize > _args.PkgSize)
                                    throw new CNetException("pkg size overflow");
                                if (_recvSeg.UsedSize < (_head.PkgHeadSize + _head.RecvPkgSize))
                                    break;

                                _recvSeg.Skip(_head.PkgHeadSize);
                                var pkg = _cache.AllocRecvBuf(_head.RecvPkgSize);
                                _recvSeg.Read(pkg, 0, _head.RecvPkgSize);

                                _recvsPre.Enqueue(ValueTuple.Create(_head.RecvPkgType, new ArraySegment<byte>(pkg, 0, _head.RecvPkgSize)));
                            }
                        }
                        catch (Exception exception)
                        {
                            lock (_closeTag)
                            {
                                if (_closeTag.Reason == eTcpClientCloseReason.None)
                                    _closeTag.Set(eTcpClientCloseReason.Exception, exception, SocketError.Success);
                            }

                            _recving = false;
                            break;
                        }

                        if (_recvsPre.Count > 0)
                        {
                            lock (_recvs)
                            {
                                while (_recvsPre.Count > 0)
                                {
                                    var pkg = _recvsPre.Dequeue();
                                    _recvCacheSize += pkg.Item2.Count;
                                    _recvBlockSize += (ulong)pkg.Item2.Count;
                                    _recvs.Enqueue(pkg);
                                }
                            }
                        }

                        if (PreRecv())
                            BeginRecv();
                        else
                            _recving = false;
                    }
                    else
                    {
                        lock (_closeTag)
                        {
                            if (_closeTag.Reason == eTcpClientCloseReason.None)
                                _closeTag.Set(eTcpClientCloseReason.Unactive, null, SocketError.Success);
                        }

                        _recving = false;
                    }
                    break;
                default:
                    lock (_closeTag)
                    {
                        if (_closeTag.Reason == eTcpClientCloseReason.None)
                            _closeTag.Set(eTcpClientCloseReason.Exception, null, e.SocketError);
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

                    var e = new CTcpClientConnectArgs(_connectTag.Result, _connectTag.Exception, _connectTag.SocketError);

                    _connectTag.Set(eTcpClientConnectResult.None, null, SocketError.Success);
                    _state = eTcpClientState.Closed;

                    try
                    {
                        OnConnected(e);
                    }
                    catch (Exception exception)
                    {
                        _log.WriteLine(eLogPattern.Error, string.Format("NET cli OnConnected exception connectArgs {0} {1}"
                            , e.Result
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
                        _log.WriteLine(eLogPattern.Error, string.Format("NET cli OnConnected exception addr {0} {1}"
                                , _cli.Client.LocalEndPoint
                                , _cli.Client.RemoteEndPoint), exception);
                    }

                    _connectTag.Set(eTcpClientConnectResult.None, null, SocketError.Success);

                    if (PreRecv())
                    {
                        _recving = true;
                        BeginRecv();
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
                        if (PreRecv())
                        {
                            _recving = true;
                            BeginRecv();
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
                        var i = _recvsPost.Dequeue();

                        object pkg = null;
                        try
                        {
                            pkg = _fill.UnFill(i.Item1, i.Item2, _cache);
                        }
                        catch (Exception exception)
                        {
                            _log.WriteLine(eLogPattern.Error, string.Format("NET cli UnFill exception pkg {0}"
                                , i.Item2.Count), exception);
                        }

                        if (pkg != null)
                        {
                            try
                            {
                                if (Recved != null)
                                    Recved(this, pkg);
                            }
                            catch (Exception exception)
                            {
                                _log.WriteLine(eLogPattern.Error, string.Format("NET cli Recved exception pkg {0}"
                                    , pkg), exception);
                            }
                        }

                        _cache.CollectRecvBuf(i.Item2.Array);
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
                                    _closeTag.Set(eTcpClientCloseReason.Active, null, SocketError.Success);
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
                        var e = new CTcpClientCloseArgs(_closeTag.Reason, null, _closeTag.SocketError);
                        Clear();
                        _state = eTcpClientState.Closed;

                        try
                        {
                            OnClosed(e);
                        }
                        catch (Exception exception)
                        {
                            _log.WriteLine(eLogPattern.Error, string.Format("NET cli OnClosed exception closeArgs {0} {1}"
                                , e.Reason
                                , e.SocketError), exception);
                        }
                    }
                break;
            }
        }

        #endregion
    }
}
