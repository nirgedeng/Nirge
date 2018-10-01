/*------------------------------------------------------------------
    Copyright © : All rights reserved
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

    public class CTcpServerArgs
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

        public CTcpServerArgs(int sendBufSize = 0, int recvBufSize = 0, int pkgSize = 0, int sendCacheSize = 0, int recvCacheSize = 0)
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

    public enum eTcpServerState
    {
        Closed,
        Opening,
        Opened,
        Closing,
        ClosingWait,
    }

    public enum eTcpServerOpenResult
    {
        None,
        Fail,
        Success,
    }

    public class CTcpServerOpenArgs
    {
        eTcpServerOpenResult _result;
        Exception _exception;
        SocketError _socketError;

        public eTcpServerOpenResult Result
        {
            get
            {
                return _result;
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

        public void Set(eTcpServerOpenResult result, Exception exception, SocketError socketError)
        {
            _result = result;
            _exception = exception;
            _socketError = socketError;
        }

        public CTcpServerOpenArgs(eTcpServerOpenResult result, Exception exception, SocketError socketError)
        {
            Set(result, exception, socketError);
        }
    }

    public enum eTcpServerCloseReason
    {
        None,
        Active,
        Exception,
    }

    public class CTcpServerCloseArgs
    {
        eTcpServerCloseReason _reason;
        Exception _exception;
        SocketError _socketError;

        public eTcpServerCloseReason Reason
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

        public void Set(eTcpServerCloseReason reason, Exception exception, SocketError socketError)
        {
            _reason = reason;
            _exception = exception;
            _socketError = socketError;
        }

        public CTcpServerCloseArgs(eTcpServerCloseReason reason, Exception exception, SocketError socketError)
        {
            Set(reason, exception, socketError);
        }
    }

    #endregion

    public class CTcpServer : IObjAlloc<CTcpServerArgs, ILog, ITcpClientCache, CTcpClientPkgFill>, IObjCollect
    {
        CTcpServerArgs _args;
        ILog _log;
        ITcpClientCache _cache;
        CTcpClientPkgFill _fill;

        eTcpServerState _state;
        CTcpServerCloseArgs _closeTag;

        TcpListener _lis;
        bool _lising;

        Queue<CTcpClient> _clisPool;
        int _clisSeed;
        Queue<TcpClient> _clisPre;
        Queue<TcpClient> _clisPost;
        List<CTcpClient> _clis;
        Dictionary<int, CTcpClient> _clisDict;

        public CTcpServerArgs Args
        {
            get
            {
                return _args;
            }
        }

        public eTcpServerState State
        {
            get
            {
                return _state;
            }
        }

        public CTcpServer(CTcpServerArgs args, ILog log, ITcpClientCache cache, CTcpClientPkgFill fill)
        {
            Alloc(args, log, cache, fill);
        }

        public CTcpServer(ILog log, CTcpClientPkgFill fill)
            :
            this(new CTcpServerArgs(), log, new CTcpClientCache(new CTcpClientCacheArgs(104857600, 104857600), log), fill)
        {
        }

        public void Alloc(CTcpServerArgs args, ILog log, ITcpClientCache cache, CTcpClientPkgFill fill)
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

            _state = eTcpServerState.Closed;
            _closeTag = new CTcpServerCloseArgs(eTcpServerCloseReason.None, null, SocketError.Success);

            _lising = false;

            _clisPool = new Queue<CTcpClient>();
            _clisSeed = 0;
            _clisPre = new Queue<TcpClient>();
            _clisPost = new Queue<TcpClient>();
            _clis = new List<CTcpClient>();
            _clisDict = new Dictionary<int, CTcpClient>();
        }

        public void Collect()
        {
            switch (_state)
            {
            case eTcpServerState.Closed:
                _args = null;
                _log = null;
                _cache.Clear();
                _cache = null;
                _fill = null;

                _closeTag = null;

                _clisPool = null;
                _clisPre = null;
                _clisPost = null;
                _clis = null;
                _clisDict = null;
                break;
            case eTcpServerState.Opening:
            case eTcpServerState.Opened:
            case eTcpServerState.Closing:
            case eTcpServerState.ClosingWait:
                break;
            }
        }

        void Clear()
        {
            _state = eTcpServerState.Closed;

            _closeTag = new CTcpServerCloseArgs(eTcpServerCloseReason.None, null, SocketError.Success);

            _lis = null;

            foreach (var i in _clisPool)
                i.Collect();
            _clisPool.Clear();

            while (_clisPre.Count > 0)
            {
                var cli = _clisPre.Dequeue();
                try
                {
                    cli.Close();
                }
                catch
                {
                }
            }

            _clisSeed = 0;
            _clisPost.Clear();
            _clis.Clear();
            _clisDict.Clear();
        }

        #region

        public event EventHandler<CDataEventArgs<CTcpServerCloseArgs>> Closed;
        void RaiseClosed(CDataEventArgs<CTcpServerCloseArgs> e)
        {
            var h = Closed;
            if (h != null)
            {
                h(this, e);
            }
        }
        void OnClosed(CTcpServerCloseArgs args)
        {
            RaiseClosed(CDataEventArgs.Create(args));
        }

        public event EventHandler<CDataEventArgs<int>> CliConnected;
        void RaiseCliConnected(CDataEventArgs<int> e)
        {
            var h = CliConnected;
            if (h != null)
            {
                h(this, e);
            }
        }
        void OnCliConnected(int cli)
        {
            RaiseCliConnected(CDataEventArgs.Create(cli));
        }

        public event EventHandler<CDataEventArgs<int, CTcpClientCloseArgs>> CliClosed;
        void RaiseCliClosed(CDataEventArgs<int, CTcpClientCloseArgs> e)
        {
            var h = CliClosed;
            if (h != null)
            {
                h(this, e);
            }
        }
        void OnCliClosed(int cli, CTcpClientCloseArgs args)
        {
            RaiseCliClosed(CDataEventArgs.Create(cli, args));
        }

        public void Open(IPEndPoint endPoint)
        {
            if (endPoint == null)
                throw new ArgumentNullException("endPoint");

            switch (_state)
            {
            case eTcpServerState.Closed:
                _state = eTcpServerState.Opening;
                _lis = new TcpListener(endPoint);
                _lis.Start();
                _state = eTcpServerState.Opened;
                _lising = true;
                LisAsync();
                break;
            case eTcpServerState.Opening:
            case eTcpServerState.Opened:
            case eTcpServerState.Closing:
            case eTcpServerState.ClosingWait:
            default:
                throw new CNetException(string.Format("NET ser tcp state {0} expected {1}", _state, eTcpServerState.Closed));
            }
        }

        public void Close()
        {
            switch (_state)
            {
            case eTcpServerState.Opened:
                _state = eTcpServerState.Closing;
                lock (_closeTag)
                {
                    if (_closeTag.Reason == eTcpServerCloseReason.None)
                        _closeTag.Set(eTcpServerCloseReason.Active, null, SocketError.Success);
                }
                break;
            case eTcpServerState.Closed:
            case eTcpServerState.Opening:
            case eTcpServerState.Closing:
            case eTcpServerState.ClosingWait:
                break;
            }
        }

        void eClose()
        {
            try
            {
                _lis.Stop();
            }
            catch
            {
            }
        }

        public void Close(int cli)
        {
            switch (_state)
            {
            case eTcpServerState.Opened:
                CTcpClient e;
                if (_clisDict.TryGetValue(cli, out e))
                    e.Close(graceful: true);
                break;
            case eTcpServerState.Closed:
            case eTcpServerState.Opening:
            case eTcpServerState.Closing:
            case eTcpServerState.ClosingWait:
                break;
            }
        }

        #endregion

        #region

        async void LisAsync()
        {
            TcpClient cli = null;
            try
            {
                cli = await _lis.AcceptTcpClientAsync();
            }
            catch (Exception exception)
            {
                lock (_closeTag)
                {
                    if (_closeTag.Reason == eTcpServerCloseReason.None)
                        _closeTag.Set(eTcpServerCloseReason.Exception, exception, SocketError.Success);
                }
            }

            if (cli != null)
            {
                switch (_state)
                {
                case eTcpServerState.Opened:
                    lock (_clisPre)
                    {
                        _clisPre.Enqueue(cli);
                    }

                    LisAsync();
                    break;
                case eTcpServerState.Closing:
                case eTcpServerState.Closed:
                case eTcpServerState.Opening:
                case eTcpServerState.ClosingWait:
                    try
                    {
                        cli.Close();
                    }
                    catch
                    {
                    }

                    _lising = false;
                    break;
                }
            }
        }

        #endregion

        #region

        public void Send(int cli, object pkg)
        {
            if (pkg == null)
                throw new ArgumentNullException("pkg");

            switch (_state)
            {
            case eTcpServerState.Opened:
                CTcpClient e;
                if (!_clisDict.TryGetValue(cli, out e))
                    throw new ArgumentOutOfRangeException("cli");
                e.Send(pkg);
                break;
            case eTcpServerState.Closed:
            case eTcpServerState.Opening:
            case eTcpServerState.Closing:
            case eTcpServerState.ClosingWait:
            default:
                throw new CNetException(string.Format("NET ser tcp state {0} expected {1}", _state, eTcpServerState.Opened));
            }
        }

        #endregion

        #region

        public event Action<object, int, object> CliRecved;

        #endregion

        #region

        public void Exec()
        {
            switch (_state)
            {
            case eTcpServerState.Closed:
            case eTcpServerState.Opening:
                break;
            case eTcpServerState.Opened:
                switch (_closeTag.Reason)
                {
                case eTcpServerCloseReason.None:
                    if (_clisPre.Count > 0)
                    {
                        lock (_clisPre)
                        {
                            while (_clisPre.Count > 0)
                                _clisPost.Enqueue(_clisPre.Dequeue());
                        }
                    }

                    while (_clisPost.Count > 0)
                    {
                        CTcpClient cli;
                        if (_clisPool.Count > 0)
                            cli = _clisPool.Dequeue();
                        else
                            cli = new CTcpClient(new CTcpClientArgs(_args.SendBufSize, _args.RecvBufSize, _args.PkgSize, _args.SendCacheSize, _args.RecvCacheSize), _log, _cache, _fill);

                        var cliId = ++_clisSeed;
                        _clis.Add(cli);
                        _clisDict.Add(cliId, cli);

                        EventHandler<CDataEventArgs<CTcpClientConnectArgs>> cbCliConnected = null;
                        EventHandler<CDataEventArgs<CTcpClientCloseArgs>> cbCliClosed = null;
                        Action<object, object> cbCliRecved = null;

                        cbCliConnected = (sender, e) =>
                        {
                            try
                            {
                                OnCliConnected(cliId);
                            }
                            catch (Exception exception)
                            {
                                _log.Error(string.Format("[TcpServer]OnCliConnected exception, cli:\"{0}\", connectArgs:\"{1},{2}\""
                                    , cliId
                                    , e.Arg1.Result
                                    , e.Arg1.SocketError), exception);
                            }
                        };
                        cbCliClosed = (sender, e) =>
                        {
                            _clis.Remove(cli);
                            _clisDict.Remove(cliId);

                            cli.Connected -= cbCliConnected;
                            cli.Closed -= cbCliClosed;
                            cli.Recved -= cbCliRecved;

                            _clisPool.Enqueue(cli);

                            try
                            {
                                OnCliClosed(cliId, e.Arg1);
                            }
                            catch (Exception exception)
                            {
                                _log.Error(string.Format("[TcpServer]OnCliClosed exception, cli:\"{0}\", closeArgs:\"{1},{2}\""
                                    , cliId
                                    , e.Arg1.Reason
                                    , e.Arg1.SocketError), exception);
                            }
                        };
                        cbCliRecved = (sender, pkg) =>
                        {
                            try
                            {
                                if (CliRecved != null)
                                    CliRecved(this, cliId, pkg);
                            }
                            catch (Exception exception)
                            {
                                _log.Error(string.Format("[TcpServer]CliRecved exception, cli:\"{0}\""
                                    , cliId), exception);
                            }
                        };

                        cli.Connected += cbCliConnected;
                        cli.Closed += cbCliClosed;
                        cli.Recved += cbCliRecved;

                        cli.Connect(_clisPost.Dequeue());
                    }

                    if (_clis.Count > 0)
                    {
                        for (var i = _clis.Count - 1; i >= 0; --i)
                            _clis[i].Exec();
                    }
                    break;
                case eTcpServerCloseReason.Active:
                case eTcpServerCloseReason.Exception:
                    eClose();
                    foreach (var i in _clis)
                        i.Close(graceful: false);
                    _state = eTcpServerState.ClosingWait;
                    break;
                }
                break;
            case eTcpServerState.Closing:
                switch (_closeTag.Reason)
                {
                case eTcpServerCloseReason.None:
                case eTcpServerCloseReason.Active:
                    eClose();
                    foreach (var i in _clis)
                        i.Close(graceful: true);
                    _state = eTcpServerState.ClosingWait;
                    break;
                case eTcpServerCloseReason.Exception:
                    eClose();
                    foreach (var i in _clis)
                        i.Close(graceful: false);
                    _state = eTcpServerState.ClosingWait;
                    break;
                }
                break;
            case eTcpServerState.ClosingWait:
                if (!_lising)
                {
                    if (_clis.Count == 0)
                    {
                        var e = new CTcpServerCloseArgs(_closeTag.Reason, _closeTag.Exception, _closeTag.SocketError);

                        Clear();
                        _state = eTcpServerState.Closed;

                        try
                        {
                            OnClosed(e);
                        }
                        catch (Exception exception)
                        {
                            _log.Error(string.Format("[TcpServer]OnClosed exception, addr:\"{0}\", closeArgs:\"{1},{2}\""
                                , ""
                                , e.Reason
                                , e.SocketError), exception);
                        }
                    }
                }
                break;
            }
        }

        #endregion
    }
}
