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
        static RpcCallArgsEmpty ArgsEmpty = new RpcCallArgsEmpty();

        CRpcCalleeArgs _args;
        ILog _log;
        CRpcStream _stream;
        CRpcCommunicator _communicator;
        protected TRpcService _service;

        public CRpcCallee(CRpcCalleeArgs args, ILog log, CRpcStream stream, CRpcCommunicator communicator, TRpcService service)
        {
            _args = args;
            _log = log;
            _stream = stream;
            _communicator = communicator;
            _service = service;
        }

        protected TRet Call<TArgs, TRet>(int channel, RpcCallReq req, Func<TArgs, int, TRet> call)
        {
            return default(TRet);
        }

        protected void CallAsync<TArgs, TRet>(int channel, RpcCallReq req, Func<TArgs, int, TRet> call)
        {
        }

        public virtual void Call(int channel, RpcCallReq req)
        {
        }
    }
}
