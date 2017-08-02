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
        void g(int channel, RpcCallArgsEmpty args);

        void h(int channel);
        void p(int channel, RpcCallArgsEmpty args);
        RpcCallArgsEmpty q(int channel, RpcCallArgsEmpty args);
    }

    public class GameRpcService : IGameRpcService
    {
        public void f(int channel)
        {
            Console.WriteLine("f");
        }

        public void g(int channel, RpcCallArgsEmpty args)
        {
            Console.WriteLine("g,{0}", args);
        }

        public void h(int channel)
        {
            Console.WriteLine("h");
        }
        public void p(int channel, RpcCallArgsEmpty args)
        {
            Console.WriteLine("p,{0}", args);
        }
        public RpcCallArgsEmpty q(int channel, RpcCallArgsEmpty args)
        {
            Console.WriteLine("q,{0}", args);
            return null;
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
            Call<RpcCallArgsEmpty>(channel, 1, 1, ArgsEmpty);
        }

        public void g(RpcCallArgsEmpty args, int channel = 0)
        {
            Call<RpcCallArgsEmpty>(channel, 1, 2, args);
        }

        public Task<RpcCallArgsEmpty> h(int channel = 0)
        {
            return CallAsync<RpcCallArgsEmpty, RpcCallArgsEmpty>(channel, 1, 3, ArgsEmpty);
        }
        public Task<RpcCallArgsEmpty> p(RpcCallArgsEmpty args, int channel = 0)
        {
            return CallAsync<RpcCallArgsEmpty, RpcCallArgsEmpty>(channel, 1, 4, args);
        }
        public Task<RpcCallArgsEmpty> q(RpcCallArgsEmpty args, int channel = 0)
        {
            return CallAsync<RpcCallArgsEmpty, RpcCallArgsEmpty>(channel, 1, 5, args);
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
                Call<RpcCallArgsEmpty, RpcCallArgsEmpty>(channel, req, (_1, args) =>
                {
                    _service.f(channel);
                    return ArgsEmpty;
                });
                break;
            case 2:
                Call<RpcCallArgsEmpty, RpcCallArgsEmpty>(channel, req, (_1, args) =>
                {
                    _service.g(channel, args);
                    return ArgsEmpty;
                });
                break;
            case 3:
                CallAsync<RpcCallArgsEmpty, RpcCallArgsEmpty>(channel, req, (_1, args) =>
                {
                    _service.h(channel);
                    return ArgsEmpty;
                });
                break;
            case 4:
                CallAsync<RpcCallArgsEmpty, RpcCallArgsEmpty>(channel, req, (_1, args) =>
                {
                    _service.p(channel, args);
                    return ArgsEmpty;
                });
                break;
            case 5:
                CallAsync<RpcCallArgsEmpty, RpcCallArgsEmpty>(channel, req, (_1, args) =>
                {
                    return _service.q(channel, args);
                });
                break;
            default:
                base.Call(channel, req);
                break;
            }
        }

    }
}
