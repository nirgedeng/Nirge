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

    #endregion

    public class CTcpServer
    {
        CTcpServerArgs _args;

        Queue<CTcpClient> _clisPool;
        int _cliid;
        Dictionary<int, CTcpClient> _clis;


        public CTcpServer(CTcpServerArgs args)
        {
            _args = args;

            _clisPool = new Queue<CTcpClient>(_args.Capacity);
            _cliid = 0;
            _clis = new Dictionary<int, CTcpClient>(_args.Capacity);
        }

        void Clear()
        {
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

        public eTcpConnError Open(IPEndPoint addr)
        {
            return eTcpConnError.None;
        }

        public void Close()
        {
        }

        public void Close(int cli)
        {
        }

        #endregion

        #region

        void BeginAccept()
        {
        }

        void EndAccept(SocketAsyncEventArgs e)
        {
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
