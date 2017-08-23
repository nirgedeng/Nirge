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
    public class CGameRpcService : IGameRpcService
    {
        public void f(int channel)
        {
        }

        public void g(int channel, gargs args)
        {
        }

        public void h(int channel)
        {
        }
        public void p(int channel, pargs args)
        {
        }
        public qret q(int channel, qargs args)
        {
            return new qret()
            {
                A = args.C,
                B = args.B,
                C = args.A,
            };
        }

    }
}
