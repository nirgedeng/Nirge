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

    public struct CTcpServerArgs
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

        public int SendQueueSize
        {
            get;
            set;
        }

        public int RecvQueueSize
        {
            get;
            set;
        }

        public int Capacity
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

        public eTcpConnError Error
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

        public eTcpConnError Error
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

    public class CTcpServer
    {
        CTcpServerArgs _args;

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

        public CTcpServer(CTcpServerArgs args)
        {
            _args = args;

            _state = eTcpServerState.Closed;
            _closeTag = new CTcpServerCloseArgs()
            {
                Error = eTcpConnError.None,
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

        void Clear()
        {
            _state = eTcpServerState.Closed;

            _closeTag.Error = eTcpConnError.None;
            _closeTag.SocketError = SocketError.Success;
            _closeTag.Reason = eTcpServerCloseReason.None;

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

            foreach (var i in _clis.Values)
                i.Destroy();
            _clis.Clear();

            _clisPool.Clear();
            _cliid = 0;
            _clisPost.Clear();
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
                    Error = eTcpConnError.None,
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
                    e.Error = eTcpConnError.SocketError;
                    e.SocketError = exception.SocketErrorCode;
                    e.Result = eTcpServerOpenResult.Fail;
                }
                catch
                {
                    e.Error = eTcpConnError.Exception;
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
                    Error = eTcpConnError.WrongState,
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
                        _closeTag.Error = eTcpConnError.None;
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
                        _closeTag.Error = eTcpConnError.SocketError;
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
                        _closeTag.Error = eTcpConnError.Exception;
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
                        _closeTag.Error = eTcpConnError.SocketError;
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
                        _closeTag.Error = eTcpConnError.Exception;
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

        public eTcpConnError Send(int cli, byte[] buf, int offset, int count)
        {
            switch (_state)
            {
            case eTcpServerState.Opened:
                CTcpClient e;
                if (!_clis.TryGetValue(cli, out e))
                    return eTcpConnError.CliOutOfRange;
                return e.Send(buf, offset, count);
            case eTcpServerState.Closed:
            case eTcpServerState.Opening:
            case eTcpServerState.Closing:
            case eTcpServerState.ClosingWait:
            default:
                return eTcpConnError.WrongState;
            }
        }

        #endregion

        #region

        public event Action<int, byte[], int, int> CliRecved;

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
                            cli = new CTcpClient(new CTcpClientArgs() { SendBufferSize = _args.SendBufferSize, ReceiveBufferSize = _args.ReceiveBufferSize, SendQueueSize = _args.SendQueueSize, RecvQueueSize = _args.RecvQueueSize, Log = _args.Log, });

                        var cliid = ++_cliid;
                        _clis.Add(cliid, cli);

                        EventHandler<CDataEventArgs<CTcpClientConnectArgs>> eCliConnected = null;
                        EventHandler<CDataEventArgs<CTcpClientCloseArgs>> eCliClosed = null;
                        Action<byte[], int, int> eCliRecved = null;

                        eCliConnected = (sender, e) =>
                        {
                            try
                            {
                                OnCliConnected(cliid);
                            }
                            catch (Exception exception)
                            {
                                _args.Log.Error(string.Format("[TcpServer]OnCliConnected exception, cli:\"{0}\", connectArgs:\"{1},{2},{3}\"", cliid, e.Arg1.Error, e.Arg1.Error, e.Arg1.SocketError), exception);
                            }
                        };
                        eCliClosed = (sender, e) =>
                        {
                            _clis.Remove(cliid);

                            cli.Connected -= eCliConnected;
                            cli.Closed -= eCliClosed;
                            cli.Recved -= eCliRecved;

                            _clisPool.Enqueue(cli);

                            try
                            {
                                OnCliClosed(cliid, e.Arg1);
                            }
                            catch (Exception exception)
                            {
                                _args.Log.Error(string.Format("[TcpServer]OnCliClosed exception, cli:\"{0}\", closeArgs:\"{1},{2},{3}\"", cliid, e.Arg1.Reason, e.Arg1.Error, e.Arg1.SocketError), exception);
                            }
                        };
                        eCliRecved = (buf, offset, count) =>
                        {
                            try
                            {
                                CliRecved(cliid, buf, offset, count);
                            }
                            catch (Exception exception)
                            {
                                _args.Log.Error(string.Format("[TcpServer]CliRecved exception, cli:\"{0}\", pkg:\"{1}\"", cliid, count), exception);
                            }
                        };

                        cli.Connected += eCliConnected;
                        cli.Closed += eCliClosed;
                        cli.Recved += eCliRecved;

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
                        i.Close();
                    _state = eTcpServerState.ClosingWait;
                    break;
                }
                break;
            case eTcpServerState.Closing:
                switch (_closeTag.Reason)
                {
                case eTcpServerCloseReason.None:
                case eTcpServerCloseReason.Active:
                case eTcpServerCloseReason.Exception:
                    eClose();
                    foreach (var i in _clis.Values)
                        i.Close();
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
                            _args.Log.Error(string.Format("[TcpServer]OnClosed exception, addr:\"{0}\", closeArgs:\"{1},{2},{3}\"", "", e.Reason, e.Error, e.SocketError), exception);
                        }
                    }
                }
                break;
            }
        }

        #endregion
    }
}
