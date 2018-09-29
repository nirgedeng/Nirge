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


        public CTcpServerArgs(int sendBufSize = 0, int recvBufSize = 0, int sendCacheSize = 0, int recvCacheSize = 0)
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
        public eTcpServerOpenResult Result
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

    public enum eTcpServerCloseReason
    {
        None,
        Active,
        Exception,
    }

    public class CTcpServerCloseArgs
    {
        public eTcpServerCloseReason Reason
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

    public class CTcpServer<T> : IObjAlloc<CTcpServerArgs, ILog, ITcpClientCache>, IObjCollect where T : CTcpClientBase, new()
    {
        CTcpServerArgs _args;
        ILog _log;
        ITcpClientCache _cache;

        eTcpServerState _state;
        CTcpServerCloseArgs _closeTag;

        TcpListener _lis;
        bool _lising;

        Queue<T> _clisPool;
        int _clisSeed;
        Queue<TcpClient> _clisPre;
        Queue<TcpClient> _clisPost;
        List<T> _clis;
        Dictionary<int, T> _clisDict;

        public eTcpServerState State
        {
            get
            {
                return _state;
            }
        }

        public CTcpServer(CTcpServerArgs args, ILog log, ITcpClientCache cache)
        {
            Alloc(args, log, cache);
        }

        public CTcpServer(ILog log)
            :
            this(new CTcpServerArgs(), log, new CTcpClientCache(new CTcpClientCacheArgs(8192, 1073741824, 1073741824), log))
        {
        }

        public void Alloc(CTcpServerArgs args, ILog log, ITcpClientCache cache)
        {
            _args = args;
            _log = log;
            _cache = cache;

            _state = eTcpServerState.Closed;
            _closeTag = new CTcpServerCloseArgs()
            {
                Reason = eTcpServerCloseReason.None,
                Error = eTcpError.None,
                SocketError = SocketError.Success,
            };

            _lising = false;

            _clisPool = new Queue<T>();
            _clisSeed = 0;
            _clisPre = new Queue<TcpClient>(32);
            _clisPost = new Queue<TcpClient>(32);
            _clis = new List<T>();
            _clisDict = new Dictionary<int, T>();
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

            _closeTag.Reason = eTcpServerCloseReason.None;
            _closeTag.Error = eTcpError.None;
            _closeTag.SocketError = SocketError.Success;

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

        public CTcpServerOpenArgs Open(IPEndPoint endPoint)
        {
            switch (_state)
            {
            case eTcpServerState.Closed:
                _state = eTcpServerState.Opening;

                var e = new CTcpServerOpenArgs()
                {
                    Result = eTcpServerOpenResult.Success,
                    Error = eTcpError.None,
                    SocketError = SocketError.Success,
                };

                _lis = new TcpListener(endPoint);
                try
                {
                    _lis.Start();
                }
                catch (SocketException exception)
                {
                    e.Result = eTcpServerOpenResult.Fail;
                    e.Error = eTcpError.SocketError;
                    e.SocketError = exception.SocketErrorCode;
                }
                catch
                {
                    e.Result = eTcpServerOpenResult.Fail;
                    e.Error = eTcpError.SysException;
                    e.SocketError = SocketError.Success;
                }

                switch (e.Result)
                {
                case eTcpServerOpenResult.Fail:
                    eClose();
                    _state = eTcpServerState.Closed;
                    break;
                case eTcpServerOpenResult.Success:
                    _state = eTcpServerState.Opened;
                    _lising = true;
                    LisAsync();
                    break;
                }

                return e;
            case eTcpServerState.Opening:
            case eTcpServerState.Opened:
            case eTcpServerState.Closing:
            case eTcpServerState.ClosingWait:
            default:
                return new CTcpServerOpenArgs()
                {
                    Result = eTcpServerOpenResult.Fail,
                    Error = eTcpError.WrongTcpState,
                    SocketError = SocketError.Success,
                };
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
                    switch (_closeTag.Reason)
                    {
                    case eTcpServerCloseReason.None:
                        _closeTag.Reason = eTcpServerCloseReason.Active;
                        _closeTag.Error = eTcpError.None;
                        _closeTag.SocketError = SocketError.Success;
                        break;
                    }
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
                T e;
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

            var pass = false;
            try
            {
                cli = await _lis.AcceptTcpClientAsync();
                pass = true;
            }
            catch (SocketException exception)
            {
                lock (_closeTag)
                {
                    switch (_closeTag.Reason)
                    {
                    case eTcpServerCloseReason.None:
                        _closeTag.Reason = eTcpServerCloseReason.Exception;
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
                    case eTcpServerCloseReason.None:
                        _closeTag.Reason = eTcpServerCloseReason.Exception;
                        _closeTag.Error = eTcpError.SysException;
                        _closeTag.SocketError = SocketError.Success;
                        break;
                    }
                }
            }

            if (pass)
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
            else
            {
                try
                {
                    cli.Close();
                }
                catch
                {
                }

                _lising = false;
            }
        }

        #endregion

        #region

        public eTcpError Send(int cli, object pkg)
        {
            switch (_state)
            {
            case eTcpServerState.Opened:
                T e;
                if (!_clisDict.TryGetValue(cli, out e))
                    return eTcpError.CliOutOfRange;
                return e.Send(pkg);
            case eTcpServerState.Closed:
            case eTcpServerState.Opening:
            case eTcpServerState.Closing:
            case eTcpServerState.ClosingWait:
            default:
                return eTcpError.WrongTcpState;
            }
        }

        #endregion

        #region

        public event Action<object, int, byte[], int, int> CliRecved;

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
                        T cli;
                        if (_clisPool.Count > 0)
                            cli = _clisPool.Dequeue();
                        else
                        {
                            cli = new T();
                            cli.Alloc(new CTcpClientArgs(_args.SendBufSize, _args.RecvBufSize, _args.SendCacheSize, _args.RecvCacheSize), _log, _cache);
                        }

                        var cliId = ++_clisSeed;
                        _clis.Add(cli);
                        _clisDict.Add(cliId, cli);

                        EventHandler<CDataEventArgs<CTcpClientConnectArgs>> cbCliConnected = null;
                        EventHandler<CDataEventArgs<CTcpClientCloseArgs>> cbCliClosed = null;
                        Action<object, byte[], int, int> cbCliRecved = null;

                        cbCliConnected = (sender, e) =>
                        {
                            try
                            {
                                OnCliConnected(cliId);
                            }
                            catch (Exception exception)
                            {
                                _log.Error(string.Format("[TcpServer]OnCliConnected exception, cli:\"{0}\", connectArgs:\"{1},{2},{3}\""
                                    , cliId
                                    , e.Arg1.Error
                                    , e.Arg1.Error
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
                                _log.Error(string.Format("[TcpServer]OnCliClosed exception, cli:\"{0}\", closeArgs:\"{1},{2},{3}\""
                                    , cliId
                                    , e.Arg1.Reason
                                    , e.Arg1.Error
                                    , e.Arg1.SocketError), exception);
                            }
                        };
                        cbCliRecved = (sender, buf, offset, count) =>
                        {
                            try
                            {
                                if (CliRecved != null)
                                    CliRecved(this, cliId, buf, offset, count);
                            }
                            catch (Exception exception)
                            {
                                _log.Error(string.Format("[TcpServer]CliRecved exception, cli:\"{0}\", pkg:\"{1}\""
                                    , cliId
                                    , count), exception);
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
                        var e = new CTcpServerCloseArgs()
                        {
                            Reason = _closeTag.Reason,
                            Error = _closeTag.Error,
                            SocketError = _closeTag.SocketError,
                        };

                        Clear();
                        _state = eTcpServerState.Closed;

                        try
                        {
                            OnClosed(e);
                        }
                        catch (Exception exception)
                        {
                            _log.Error(string.Format("[TcpServer]OnClosed exception, addr:\"{0}\", closeArgs:\"{1},{2},{3}\""
                                , ""
                                , e.Reason
                                , e.Error
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
