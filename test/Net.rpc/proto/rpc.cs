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

namespace Nirge.Core
{
    public interface IGameRpcService : IRpcService
    {
        void f(int channel);
        void g(int channel, gargs args);

        void h(int channel);
        void p(int channel, pargs args);
        qret q(int channel, qargs args);
    }

    public class CGameRpcService : IGameRpcService
    {
        public void f(int channel)
        {
            Console.WriteLine("f");
        }

        public void g(int channel, gargs args)
        {
            Console.WriteLine("g,{0}", args);
        }

        public void h(int channel)
        {
            Console.WriteLine("h");
        }
        public void p(int channel, pargs args)
        {
            Console.WriteLine("p,{0}", args);
        }
        public qret q(int channel, qargs args)
        {
            Console.WriteLine("q,{0}", args);

            return new qret()
            {
                A = args.C,
                B = args.B,
                C = args.A,
            };
        }

    }

    public class CGameRpcCaller : CRpcCaller
    {
        public CGameRpcCaller(CRpcCallerArgs args, ILog log, CRpcStream stream, CRpcCommunicator communicator, CRpcCallStubProvider stubs)
            : base(args, log, stream, communicator, stubs, 1)
        {
        }

        public void f(int channel = 0)
        {
            Call<RpcCallArgsEmpty>(channel, 1, ArgsEmpty);
        }

        public void g(gargs args, int channel = 0)
        {
            Call<gargs>(channel, 2, args);
        }

        public Task<RpcCallArgsEmpty> h(int channel = 0)
        {
            return CallAsync<RpcCallArgsEmpty, RpcCallArgsEmpty>(channel, 3, ArgsEmpty);
        }
        public Task<RpcCallArgsEmpty> p(pargs args, int channel = 0)
        {
            return CallAsync<pargs, RpcCallArgsEmpty>(channel, 4, args);
        }
        public Task<qret> q(qargs args, int channel = 0)
        {
            return CallAsync<qargs, qret>(channel, 5, args);
        }

    }

    public class CGameRpcCallee : CRpcCallee<CGameRpcService>
    {
        public CGameRpcCallee(CRpcCalleeArgs args, ILog log, CRpcStream stream, CRpcCommunicator communicator, CGameRpcService service)
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
                Call<gargs, RpcCallArgsEmpty>(channel, req, (_1, args) =>
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
                CallAsync<pargs, RpcCallArgsEmpty>(channel, req, (_1, args) =>
                {
                    _service.p(channel, args);
                    return ArgsEmpty;
                });
                break;
            case 5:
                CallAsync<qargs, qret>(channel, req, (_1, args) =>
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
