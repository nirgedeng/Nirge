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

    #endregion

    public class CTcpServer
    {
        CTcpServerArgs _args;

        TcpListener _lis;
        bool _lising;

        eTcpServerState _state;

        Queue<CTcpClient> _clisPool;
        int _cliid;
        List<TcpClient> _clisBefore;
        Dictionary<int, CTcpClient> _clis;
        List<int> _clisAfter;

        public CTcpServer(CTcpServerArgs args)
        {
            _args = args;

            _state = eTcpServerState.Closed;

            _lising = false;

            _clisPool = new Queue<CTcpClient>(_args.Capacity);
            _cliid = 0;
            _clisBefore = new List<TcpClient>(32);
            _clis = new Dictionary<int, CTcpClient>(_args.Capacity);
            _clisAfter = new List<int>(_args.Capacity);
        }

        void Clear()
        {
            _clisPool.Clear();
            _cliid = 0;
            _clisBefore.Clear();
            _clis.Clear();
            _clisAfter.Clear();
        }

        #region

        public event EventHandler<CDataEventArgs<int>> Connected;
        void RaiseConnected(CDataEventArgs<int> e)
        {
            var h = Connected;
            if (h != null)
            {
                h(this, e);
            }
        }
        void OnConnected(int cli)
        {
            RaiseConnected(CDataEventArgs.Create(cli));
        }

        public event EventHandler<CDataEventArgs<int, CTcpClientCloseArgs>> Closed;
        void RaiseClosed(CDataEventArgs<int, CTcpClientCloseArgs> e)
        {
            var h = Closed;
            if (h != null)
            {
                h(this, e);
            }
        }
        void OnClosed(int cli, CTcpClientCloseArgs args)
        {
            RaiseClosed(CDataEventArgs.Create(cli, args));
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
            case eTcpServerState.Closed:
            case eTcpServerState.Opening:
            case eTcpServerState.Opened:
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
        }

        #endregion

        #region

        void BeginLis()
        {
            try
            {
                _lis.BeginAcceptTcpClient((e) =>
                {
                    EndLis(e);
                }, this);
            }
            catch (SocketException exception)
            {
            }
            catch
            {
            }
        }

        void EndLis(IAsyncResult e)
        {
            TcpClient cli;
            try
            {
                cli = _lis.EndAcceptTcpClient(e);
            }
            catch
            {
            }

            switch (_state)
            {
            case eTcpServerState.Opened:
                break;
            case eTcpServerState.Closed:
            case eTcpServerState.Opening:
            case eTcpServerState.Closing:
            case eTcpServerState.ClosingWait:
                _lising = false;
                break;
            }
        }

        #endregion

        #region

        public eTcpConnError Send(int cli, byte[] buf, int offset, int count)
        {
            return eTcpConnError.None;
        }

        #endregion

        #region

        public event Action<int, byte[], int, int> Recved;

        #endregion

        #region

        public void Exec()
        {
        }

        #endregion
    }
}
