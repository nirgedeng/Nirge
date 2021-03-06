﻿/*------------------------------------------------------------------
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
        int _sendCapacity;
        int _recvCapacity;
        int _capacity;

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

        public int SendCapacity
        {
            get
            {
                return _sendCapacity;
            }
        }

        public int RecvCapacity
        {
            get
            {
                return _recvCapacity;
            }
        }

        public int Capacity
        {
            get
            {
                return _capacity;
            }
        }

        public CTcpServerArgs(int sendBufSize = 0, int recvBufSize = 0, int pkgSize = 0, int sendCapacity = 0, int recvCapacity = 0, int capacity = 0)
        {
            _sendBufSize = sendBufSize;
            _recvBufSize = recvBufSize;
            _pkgSize = pkgSize;
            _sendCapacity = sendCapacity;
            _recvCapacity = recvCapacity;
            _capacity = capacity;

            if (_sendBufSize == 0)
                _sendBufSize = 8192;
            else if (_sendBufSize < 8192)
                _sendBufSize = 8192;
            else if (_sendBufSize > 16384)
                _sendBufSize = 16384;
            if (_recvBufSize == 0)
                _recvBufSize = 8192;
            else if (_recvBufSize < 8192)
                _recvBufSize = 8192;
            else if (_recvBufSize > 16384)
                _recvBufSize = 16384;
            if (_pkgSize == 0)
                _pkgSize = 8192;
            else if (_pkgSize < 8192)
                _pkgSize = 8192;
            else if (_pkgSize > 1048576)
                _pkgSize = 1048576;
            _sendCapacity = 128;
            _recvCapacity = 128;
            _capacity = 1024;
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

    public class CTcpServer : IObjCtor<CTcpServerArgs, ILog, ITcpClientCache>, IObjDtor
    {
        CTcpServerArgs _args;
        ILog _log;
        ITcpClientCache _cache;

        eTcpServerState _state;
        CTcpServerCloseArgs _closeTag;

        TcpListener _lis;
        bool _lising;

        Queue<CTcpClient> _clisPool;
        int _cliId;
        Queue<TcpClient> _clisPre;
        Queue<TcpClient> _clisPost;
        List<CTcpClient> _clis;
        Dictionary<int, CTcpClient> _clisDict;

        public eTcpServerState State
        {
            get
            {
                return _state;
            }
        }

        public CTcpServer(CTcpServerArgs args, ILog log, ITcpClientCache cache)
        {
            Init(args, log, cache);
        }

        public CTcpServer(ILog log)
            :
            this(new CTcpServerArgs(), log, new CTcpClientCacheEmpty(new CTcpClientCacheArgs(25600, 12800, 6400, 25600, 12800, 6400)))
        {
        }

        public void Init(CTcpServerArgs args, ILog log, ITcpClientCache cache)
        {
            _args = args;
            _log = log;
            _cache = cache;

            _state = eTcpServerState.Closed;
            _closeTag = new CTcpServerCloseArgs()
            {
                Error = eTcpError.None,
                SocketError = SocketError.Success,
                Reason = eTcpServerCloseReason.None,
            };

            _lising = false;

            _clisPool = new Queue<CTcpClient>(_args.Capacity);
            _cliId = 0;
            _clisPre = new Queue<TcpClient>(32);
            _clisPost = new Queue<TcpClient>(32);
            _clis = new List<CTcpClient>(_args.Capacity);
            _clisDict = new Dictionary<int, CTcpClient>(_args.Capacity);
        }

        public void Destroy()
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

            _closeTag.Error = eTcpError.None;
            _closeTag.SocketError = SocketError.Success;
            _closeTag.Reason = eTcpServerCloseReason.None;

            _lis = null;

            foreach (var i in _clisPool)
                i.Destroy();
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

            _cliId = 0;
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

        public CTcpServerOpenArgs Open(IPEndPoint addr)
        {
            switch (_state)
            {
            case eTcpServerState.Closed:
                _state = eTcpServerState.Opening;

                var e = new CTcpServerOpenArgs()
                {
                    Error = eTcpError.None,
                    SocketError = SocketError.Success,
                    Result = eTcpServerOpenResult.Success,
                };

                _lis = new TcpListener(addr);
                try
                {
                    _lis.Start();
                }
                catch (SocketException exception)
                {
                    e.Error = eTcpError.SocketError;
                    e.SocketError = exception.SocketErrorCode;
                    e.Result = eTcpServerOpenResult.Fail;
                }
                catch
                {
                    e.Error = eTcpError.Exception;
                    e.SocketError = SocketError.Success;
                    e.Result = eTcpServerOpenResult.Fail;
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
                    Error = eTcpError.WrongState,
                    SocketError = SocketError.Success,
                    Result = eTcpServerOpenResult.Fail,
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
                        _closeTag.Error = eTcpError.None;
                        _closeTag.SocketError = SocketError.Success;
                        _closeTag.Reason = eTcpServerCloseReason.Active;
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

            var safe = false;
            try
            {
                cli = await _lis.AcceptTcpClientAsync();
                safe = true;
            }
            catch (SocketException exception)
            {
                lock (_closeTag)
                {
                    switch (_closeTag.Reason)
                    {
                    case eTcpServerCloseReason.None:
                        _closeTag.Error = eTcpError.SocketError;
                        _closeTag.SocketError = exception.SocketErrorCode;
                        _closeTag.Reason = eTcpServerCloseReason.Exception;
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
                        _closeTag.Error = eTcpError.Exception;
                        _closeTag.SocketError = SocketError.Success;
                        _closeTag.Reason = eTcpServerCloseReason.Exception;
                        break;
                    }
                }
            }

            if (safe)
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

        public eTcpError Send(int cli, byte[] buf, int offset, int count)
        {
            switch (_state)
            {
            case eTcpServerState.Opened:
                CTcpClient e;
                if (!_clisDict.TryGetValue(cli, out e))
                    return eTcpError.CliOutOfRange;
                return e.Send(buf, offset, count);
            case eTcpServerState.Closed:
            case eTcpServerState.Opening:
            case eTcpServerState.Closing:
            case eTcpServerState.ClosingWait:
            default:
                return eTcpError.WrongState;
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
                        CTcpClient cli;
                        if (_clisPool.Count > 0)
                            cli = _clisPool.Dequeue();
                        else
                            cli = new CTcpClient(new CTcpClientArgs(_args.SendBufSize, _args.RecvBufSize, _args.PkgSize, _args.SendCapacity, _args.RecvCapacity), _log, _cache);

                        var cliId = ++_cliId;
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
                            Error = _closeTag.Error,
                            SocketError = _closeTag.SocketError,
                            Reason = _closeTag.Reason,
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
