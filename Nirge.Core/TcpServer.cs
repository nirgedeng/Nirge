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
    #region

    public class CTcpServerArgs
    {
        int _sendBufferSize;
        int _receiveBufferSize;
        int _pkgSize;
        int _sendQueueSize;
        int _recvQueueSize;
        int _capacity;

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

        public int Capacity
        {
            get
            {
                return _capacity;
            }
        }

        public CTcpServerArgs(int sendBufferSize = 0, int receiveBufferSize = 0, int pkgSize = 0, int sendQueueSize = 0, int recvQueueSize = 0, int capacity = 0)
        {
            _sendBufferSize = sendBufferSize;
            _receiveBufferSize = receiveBufferSize;
            _pkgSize = pkgSize;
            _sendQueueSize = sendQueueSize;
            _recvQueueSize = recvQueueSize;
            _capacity = capacity;

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

    public struct CTcpServerOpenArgs
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

    public class CTcpServer : IObjCtor<CTcpServerArgs, ILog>, IObjDtor
    {
        CTcpServerArgs _args;
        ILog _log;

        eTcpServerState _state;
        CTcpServerCloseArgs _closeTag;

        TcpListener _lis;
        bool _lising;

        Queue<CTcpClient> _clisPool;
        int _cliid;
        Queue<TcpClient> _clisPre;
        Queue<TcpClient> _clisPost;
        Dictionary<int, CTcpClient> _clis;
        List<int> _clisAfter;

        public eTcpServerState State
        {
            get
            {
                return _state;
            }
        }

        public CTcpServer(CTcpServerArgs args, ILog log)
        {
            Init(args, log);
        }

        public CTcpServer(ILog log)
            :
            this(new CTcpServerArgs(), log)
        {
        }

        public void Init(CTcpServerArgs args, ILog log)
        {
            _args = args;

            _log = log;

            _state = eTcpServerState.Closed;
            _closeTag = new CTcpServerCloseArgs()
            {
                Error = eTcpError.None,
                SocketError = SocketError.Success,
                Reason = eTcpServerCloseReason.None,
            };

            _lising = false;

            _clisPool = new Queue<CTcpClient>(_args.Capacity);
            _cliid = 0;
            _clisPre = new Queue<TcpClient>(32);
            _clisPost = new Queue<TcpClient>(32);
            _clis = new Dictionary<int, CTcpClient>(_args.Capacity);
            _clisAfter = new List<int>(_args.Capacity);
        }

        public void Destroy()
        {
            switch (_state)
            {
            case eTcpServerState.Closed:
                _clisPool = null;
                _clisPre = null;
                _clisPost = null;
                _clis = null;
                _clisAfter = null;
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

            _cliid = 0;
            _clisPost.Clear();
            _clis.Clear();
            _clisAfter.Clear();
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
                    BeginLis();
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
                if (_clis.TryGetValue(cli, out e))
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

        void BeginLis()
        {
            var safe = false;
            try
            {
                _lis.BeginAcceptTcpClient((e) =>
                {
                    EndLis(e);
                }, this);

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

            if (!safe)
                _lising = false;
        }

        void EndLis(IAsyncResult e)
        {
            var safe = false;
            TcpClient cli = null;

            try
            {
                cli = _lis.EndAcceptTcpClient(e);
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
                    lock (_clis)
                    {
                        _clisPre.Enqueue(cli);
                    }

                    BeginLis();
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
                if (!_clis.TryGetValue(cli, out e))
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
                        lock (_clis)
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
                            cli = new CTcpClient(new CTcpClientArgs(_args.SendBufferSize, _args.ReceiveBufferSize, _args.PkgSize, _args.SendQueueSize, _args.RecvQueueSize), _log);

                        var cliid = ++_cliid;
                        _clis.Add(cliid, cli);

                        EventHandler<CDataEventArgs<CTcpClientConnectArgs>> cbCliConnected = null;
                        EventHandler<CDataEventArgs<CTcpClientCloseArgs>> cbCliClosed = null;
                        Action<object, byte[], int, int> cbCliRecved = null;

                        cbCliConnected = (sender, e) =>
                        {
                            try
                            {
                                OnCliConnected(cliid);
                            }
                            catch (Exception exception)
                            {
                                _log.Error(string.Format("[TcpServer]OnCliConnected exception, cli:\"{0}\", connectArgs:\"{1},{2},{3}\"", cliid, e.Arg1.Error, e.Arg1.Error, e.Arg1.SocketError), exception);
                            }
                        };
                        cbCliClosed = (sender, e) =>
                        {
                            _clis.Remove(cliid);

                            cli.Connected -= cbCliConnected;
                            cli.Closed -= cbCliClosed;
                            cli.Recved -= cbCliRecved;

                            _clisPool.Enqueue(cli);

                            try
                            {
                                OnCliClosed(cliid, e.Arg1);
                            }
                            catch (Exception exception)
                            {
                                _log.Error(string.Format("[TcpServer]OnCliClosed exception, cli:\"{0}\", closeArgs:\"{1},{2},{3}\"", cliid, e.Arg1.Reason, e.Arg1.Error, e.Arg1.SocketError), exception);
                            }
                        };
                        cbCliRecved = (sender, buf, offset, count) =>
                        {
                            try
                            {
                                if (CliRecved != null)
                                    CliRecved(this, cliid, buf, offset, count);
                            }
                            catch (Exception exception)
                            {
                                _log.Error(string.Format("[TcpServer]CliRecved exception, cli:\"{0}\", pkg:\"{1}\"", cliid, count), exception);
                            }
                        };

                        cli.Connected += cbCliConnected;
                        cli.Closed += cbCliClosed;
                        cli.Recved += cbCliRecved;

                        cli.Connect(_clisPost.Dequeue());
                    }

                    if (_clis.Count > 0)
                    {
                        _clisAfter.AddRange(_clis.Keys);
                        foreach (var i in _clisAfter)
                        {
                            CTcpClient cli;
                            if (_clis.TryGetValue(i, out cli))
                                cli.Exec();
                        }
                        _clisAfter.Clear();
                    }

                    break;
                case eTcpServerCloseReason.Active:
                case eTcpServerCloseReason.Exception:
                    eClose();
                    foreach (var i in _clis.Values)
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
                    foreach (var i in _clis.Values)
                        i.Close(graceful: true);
                    _state = eTcpServerState.ClosingWait;
                    break;
                case eTcpServerCloseReason.Exception:
                    eClose();
                    foreach (var i in _clis.Values)
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
                            _log.Error(string.Format("[TcpServer]OnClosed exception, addr:\"{0}\", closeArgs:\"{1},{2},{3}\"", "", e.Reason, e.Error, e.SocketError), exception);
                        }
                    }
                }
                break;
            }
        }

        #endregion
    }
}
