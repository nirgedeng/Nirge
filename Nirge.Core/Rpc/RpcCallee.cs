/*------------------------------------------------------------------
    Copyright © : All rights reserved
    Author      : 邓晓峰
------------------------------------------------------------------*/

using System.Reflection;
using System.Threading;
using System.Linq;
using System.Text;
using System.IO;
using log4net;
using System;

namespace Nirge.Core
{
    #region

    public class CRpcCalleeArgs
    {
        bool _logCall;

        public bool LogCall
        {
            get
            {
                return _logCall;
            }
        }

        public CRpcCalleeArgs(bool logCall)
        {
            _logCall = logCall;
        }
    }

    #endregion

    public class CRpcCallee<TRpcService> where TRpcService : IRpcService
    {
        CRpcCalleeArgs _args;
        ILog _log;
        CBufStream _stream;
        CRpcCommunicator _communicator;
        protected IRpcService _service;

        public CRpcCallee(CRpcCalleeArgs args, ILog log, CBufStream stream, CRpcCommunicator communicator, TRpcService service)
        {
            _args = args;
            _log = log;
            _stream = stream;
            _communicator = communicator;
            _service = service;
        }

        public virtual void Call(int channel, RpcCallReq req)
        {
        }

        protected void Call<TArgs>(int channel, RpcCallReq req, Action<int> call)
        {
        }

        protected void Call<TArgs>(int channel, RpcCallReq req, Action<TArgs, int> call)
        {
        }

        protected void Call<TArgs, TRet>(int channel, RpcCallReq req, Func<TArgs, int, TRet> call)
        {
        }
    }
}
