/*------------------------------------------------------------------
    Copyright © : All rights reserved
    Author      : 邓晓峰
------------------------------------------------------------------*/

using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Text;
using Nirge.Core;
using log4net;
using System;

namespace proto
{
    public interface IGameRpcService : IRpcService
    {
        void f(int channel);
    }

    public class GameRpcService : IGameRpcService
    {
        public void f(int channel)
        {
        }
    }

    public class CGameRpcCaller : CRpcCaller
    {
        public CGameRpcCaller(CRpcCallerArgs args, ILog log, CRpcStream stream, CRpcCommunicator communicator, CRpcCallStubProvider stubs)
            : base(args, log, stream, communicator, stubs)
        {
        }

        public void f(int channel = 0)
        {
            Call(channel, 1, 1, ArgsEmpty);
        }

    }

    public class CGameRpcCallee : CRpcCallee<GameRpcService>
    {
        public CGameRpcCallee(CRpcCalleeArgs args, ILog log, CRpcStream stream, CRpcCommunicator communicator, GameRpcService service)
            : base(args, log, stream, communicator, service)
        {
        }

        public override void Call(int channel, RpcCallReq req)
        {
            switch (req.Call)
            {
            case 1:
                Call<RpcCallArgsEmpty, RpcCallArgsEmpty>(channel, req, (_1, _2) =>
                {
                    _service.f(channel);
                    return ArgsEmpty;
                });
                break;
            case 2:
                break;
            default:
                base.Call(channel, req);
                break;
            }
        }

    }
}
