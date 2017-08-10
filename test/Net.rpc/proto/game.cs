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
}
